--v1.01 对发卡过程对dt_card有正常卡但dt_user无卡号的情况进行处理
--v1.02 增加开户存储过程，并替代发卡过程中的开户部分
--v1.03 解决同步补卡的问题
--v1.04 解决发卡提示“rollback traction..问题”
--v1.05 人员复职增加账户处理






if object_id('ClientPro_user_kh') is not null
drop proc ClientPro_user_kh
go
create procedure ClientPro_user_kh 	--开户
(
@ac_type char(16)=null,		--人员账户类型       ????????????????? ver???
@user_serial bigint,		--人员序号
@gly_no nvarchar(50) = 'syn',		--操作员
@result int output
)
as
set nocount on

declare @rec_count int 
declare @ac_no char(16)			--账户编号

set @result = 0;

begin try

	begin transaction
		update dt_user set user_ac = 1 where user_serial=@user_serial
	    
		if exists(select 1 from dt_Ac_user where user_serial=@user_serial)
		begin
		
			update dt_ac_user set ac_state=0,ac_jssj=dateadd(year,20,getdate())
			where user_serial=@user_serial
		 
		end
		else
		begin
			while 1 = 1
			begin

				set @ac_no = replace(replace(replace(convert(varchar(19),getdate(),21),'-',''),':',''),' ','') + right(replicate('0', 1) + convert(varchar(10),ceiling(rand()*99)), 2) 
				select @rec_count = count(*) from dt_ac_user where ac_no = @ac_no
				if @rec_count > 0
				begin
					waitfor delay '00:00:00.20'
					continue
				end
				else
				begin
					
					insert into dt_ac_user(ac_no,ac_type,user_serial,ac_pass,ac_kssj,ac_jssj,ac_state,sj,gly_no,ac_money,ac_make,ac_addo,ac_subo) 
					select @ac_no,@ac_type,@user_serial,ac_pass,getdate() as kssj,
					case money_type when 0  then DATEADD(year,ac_limit,getdate()) when 1 then DATEADD(month,ac_limit,getdate()) when 2  then DATEADD(day,ac_limit,getdate()) end as jssj,
					0,getdate(),@gly_no,0,0,0,0 from DT_AC_TYPE where Ac_bh=@ac_type
					
					break
				end
				
			end
		end		
	commit transaction			
end try

begin catch	--异常捕获 
    ROLLBACK TRANSACTION
	set @result = 1;
	print '开户失败,员工序号:'+@user_serial+',原因:'+ERROR_MESSAGE() 	
end catch

go


if object_id('user_resume') is not null
drop proc user_resume
go

CREATE procedure user_resume
@user_serial int
as
set nocount on


begin try
    declare @user_no varchar(100)
	begin transaction
	    select @user_no = user_no from dt_user where user_serial = @user_serial
	
		update dt_user set user_type = 0,user_ac = 1,user_sj=getdate() where user_serial = @user_serial
		
		update dt_ac_user set ac_state = 0 where user_serial = @user_serial

		insert into WT_USER_LOG(Lx,Log_type,Log_state,Module_id,Log_bz,Log_ip,Gly_no,Log_sj) 
		VALUES (3,0,0,'0002','syn','','syn',getdate())

		insert into WT_USER_UP(lx,log_type,user_serial,log_sj,log_ip,gly_no) 
		VALUES (3,1,@user_serial,getdate(),'','syn')
	commit transaction
end try
begin catch	--异常捕获
	
	ROLLBACK TRANSACTION;
	print '工号:'+@user_no+' 复职失败:'+ERROR_MESSAGE() 

end catch


set nocount off
GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[user_destroy]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop proc user_destroy
GO


create proc user_destroy    
    @user_serial int,           --人员序号
    @card_hao varchar(20)  --卡号
as       
declare @user_no varchar(100)
begin try
	begin transaction
		select @user_no = user_no from dt_user where user_serial = @user_serial
	
		update dt_user set user_type=51,user_sj=getdate() where user_serial = @user_serial

		insert into WT_USER_LOG(Lx,Log_type,Log_state,Module_id,Log_bz,Log_ip,Gly_no,Log_sj) 
		VALUES (3,0,0,'0002','syn','','syn',getdate())

		insert into WT_USER_UP(lx,log_type,user_serial,log_sj,log_ip,gly_no) 
		VALUES (3,1,@user_serial,getdate(),'','syn')
	commit transaction
end try
begin catch	--异常捕获
	
	ROLLBACK TRANSACTION;
	print '工号:'+@user_no+' 离职失败:'+ERROR_MESSAGE()  

end catch



GO


if object_id('ClientPro_Card_add') is not null
drop proc ClientPro_Card_add
go
create procedure ClientPro_Card_add 	--发卡处理过程
(
@dev_serial char(7),		--发卡设备编号
@lx int,					--过程类型（0：预处理；1：更新结果状态; 2:同步发卡）
@ac_type char(16)=null,		--人员账户类型       ????????????????? ver???
@user_serial bigint,		--人员序号
@card_hao varchar(20),		--物理卡号
@jm_kh varchar(20) = null,			--加密卡号
@card_serial char(8) = null,		--卡顺序号
@card_lx int = 100,				--卡类型 1 s50 
@card_work int,				--读写状态（同步时为0）
@card_type int,				--卡状态（同步时为0）
@ip  varchar(20) = 'syn',     		--操作IP
@gly_no nvarchar(50) = 'syn',		--操作员
@reg_serial varchar(50) =null		--企业编号
)
as
--返回值必须添加的内容
set nocount on

--变量定义
declare @ac_no char(16)			--账户编号
declare @rec_count int			--计数器
declare @card_order int			--卡号顺序
declare @xh int					--卡号表序号

declare @pxh varchar(8)      	 --卡父序号 
declare @tt_form varchar(50)	 --卡转换格式
declare @tempkh varchar(50) 	 --转换后卡号

declare @tt_xh int          	 --卡转换格式序号 
declare @tt_cut int				 --截取位数
declare @tt_hex int			 	 --进制
declare @tt_fields varchar(50)	 --

declare @user_no varchar(100)
declare @result int
declare @tmp_card varchar(50)
declare @module_card int
declare @card_serial_max varchar(50)
declare @count int

--变量赋值
set @rec_count=0
set @card_order=0

begin try
---------////////软件发卡处理流程/////////------------------------
	if(@lx=0)	--发卡预处理过程
	begin

		--1.人员是否有卡
		if (select isnull(user_card,'') from dt_user where user_serial=@user_serial)<>''
		begin
			select 2	
			return
		end
		
	    --3.该人员是否已经有卡（针对dt_card有卡号但dt_user无卡号的情况）   --v1.01
		if (select count(1) from dt_card where card_hao=@card_hao and user_serial = @user_serial and card_type = 0)>0
		begin
			update dt_user 
		    set user_card = @card_hao  
		    where user_serial = @user_serial
			select 3
			return
		end
		
		--2.新卡是否被别人占用
		if (select count(card_hao) from dt_card where card_hao=@card_hao and card_type<>4)>0
		begin
			select 3
			return
		end


	


		begin transaction
			
			---1.1插dt_card表数据
			--计算Card_order
			select @card_order=(max(card_order)+1) from dt_card where user_serial=@user_serial
				
			insert into dt_card(
			lx, gly_no, sj, user_serial, card_lx, 
			card_type, card_hao, card_order, card_serial, 
			loss_count, isCardReplace
			)values(
			0,@gly_no,getdate(),@user_serial,@card_lx,
			@card_type,@card_hao,@card_order,@card_Serial,
			0,0
			)
			
			--1.2写预处理日志
			insert into wt_card_log(
			lx,log_type,log_state,user_serial,dev_serial,card_old,
			card_new,log_xh,log_sj,log_ip,gly_no,log_lx,log_group,
			log_row,log_erro,bz,regserial
			)values(
			12,0,0,@user_serial,@dev_serial,@card_hao,
			@card_serial,@card_lx,getdate(),@ip,@gly_no,0,null,
			null,0,null,@reg_serial
			)
			
		commit transaction
	end
	
	else if(@lx=1)	--完成并更新结果
	begin		
		
		begin transaction
	
			select @xh=max(xh) from dt_card where user_serial=@user_serial and card_serial=@card_serial
			and ((card_hao=@card_hao and card_type=4)or card_type=5)
			if(@card_type=5)
			begin
				
				update dt_card set card_type=@card_type,card_hao=''
				where xh=@xh
				
				commit transaction
				select 1
				return
			end
			
			else
			begin
			
				if(isnull(@ac_type,'')<>'')	--开户
				begin
					
					if exists(select 1 from dt_Ac_user where user_serial=@user_serial)
					begin
					
						update dt_ac_user set ac_type=@ac_type,ac_state=0,ac_jssj=dateadd(year,20,getdate())
						where user_serial=@user_serial
					 
					end
					else
					begin
						while 1 = 1
						begin

							set @ac_no = replace(replace(replace(convert(varchar(19),getdate(),21),'-',''),':',''),' ','') + right(replicate('0', 1) + convert(varchar(10),ceiling(rand()*99)), 2) 
							select @rec_count = count(*) from dt_ac_user where ac_no = @ac_no
							if @rec_count > 0
								begin
									waitfor delay '00:00:00.20'
									continue
								end
							else
							begin
								
								insert into dt_ac_user(ac_no,ac_type,user_serial,ac_pass,ac_kssj,ac_jssj,ac_state,sj,gly_no,ac_money,ac_make,ac_addo,ac_subo) select @ac_no,@ac_type,@user_serial,ac_pass,getdate() as kssj,case money_type when 0  then DATEADD(year,ac_limit,getdate()) when 1 then DATEADD(month,ac_limit,getdate()) when 2  then DATEADD(day,ac_limit,getdate()) end as jssj,0,getdate(),@gly_no,0,0,0,0 from DT_AC_TYPE where Ac_bh=@ac_type
								
								break
							end
							
						end
					end
						
				end
				
				---更新卡表状态
				update dt_card set card_hao=@card_hao,card_type=@card_type,card_work=@card_work,card_lx=@card_lx,sj=getdate()
				where xh=@xh
				
				---生成转换格式数据
				exec ClientPro_Card_formate @xh,@card_lx
				
				---生成人员卡户
				--计算Card_order
				select @card_order=(max(card_order)+1) from dt_ac_card where user_serial=@user_serial
				insert into dt_ac_card(
				user_serial,card_serial,card_hao,card_order,ac_money,ac_addm,ac_addo,ac_subm,
				ac_subo,ac_regm,ac_make,ac_eachm,ac_eacho,ac_state,sj,ks_sj,gly_no
				)VALUES(
				@user_serial,@card_serial,@card_hao,isnull(@card_order,1),0,0,0,0,
				0,0,0,0,0,0,getdate(),getdate(),@gly_no
				)
				
				---更新人员表状态
				update dt_user set user_ac = 1,user_card=@card_hao,user_sj=getdate()
				where user_serial = @user_serial
			
				---生成增量档案日志
				insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
				values(1,1,0,@user_serial,@card_serial,getdate(),@ip,@gly_no) 
				
				--新增照片日志
				insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
				values(1,2,0,@user_serial,@card_serial,getdate(),@ip,@gly_no)
				
				--新增指纹日志
				insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
				values(1,3,0,@user_serial,@card_serial,getdate(),@ip,@gly_no)
				
				---更新发卡日志
				Update WT_CARD_LOG set lx=0,log_sj=getdate(),log_ip=@ip,gly_no=@gly_no where lx=12 and user_serial=@user_serial and card_new=@card_serial
				
			end
			
		commit transaction

	end
	
---------////////同步发卡处理流程/////////------------------------
	else if(@lx=2)
	begin
		---判断人员信息
	    --1.人员是否有卡
	    select @user_no = user_no from dt_user where user_serial = @user_serial
	    select top 1 @tmp_card = isnull(user_card,'') from dt_user where user_serial=@user_serial
		if @tmp_card <> ''
		begin
		
			select 2	--人员存在正在使用的卡片
			print '人员存在正在使用的卡片,工号:'+@user_no
			return
			
		end
		
		--2.新卡是否占用
		if (select count(card_hao) from dt_card where card_hao=@card_hao and card_type<>4)>0
		begin
		
			select 3	--新卡被占用
			print '新卡被占用,卡号:'+@card_hao
			return
			
		end
			
        --3.创建人员账户//////建议去掉！！！！！！！！！！！
		exec ClientPro_user_kh @ac_type,@user_serial,@gly_no,@result output            
		if @result <> 0 
		begin
			print '开户失败,卡号:'+@card_hao	
			return				--返回
		end;
		
		begin transaction
			--4.生成卡信息
			--计算Card_order
			
			select @card_order=(max(card_order)+1) from dt_card where user_serial=@user_serial
			
			--获取卡顺序号

			select @module_card = module_card+1 from wt_module where module_id='0009'
			set @card_serial = right('00000000'+convert(varchar(8),@module_card),8)  --右取8位 不足位前补0
			update wt_module set module_card=@module_card where module_id='0009'
		
			
			--.写卡片操作日志（未决状态）
			insert into wt_card_log(
			lx,log_type,log_state,user_serial,dev_serial,card_old,
			card_new,log_xh,log_sj,log_ip,gly_no,log_lx,log_group,
			log_row,log_erro,bz,regserial
			)values(
			12,0,0,@user_serial,@dev_serial,@card_hao,
			@card_serial,@card_lx,getdate(),@ip,@gly_no,0,null,
			null,0,null,@reg_serial
			)
			
			
			insert into dt_card(
			lx, gly_no, sj, user_serial, card_lx, 
			card_type, card_hao, card_order, card_serial, 
			loss_count, isCardReplace
			)values(
			0,@gly_no,getdate(),@user_serial,@card_lx,
			@card_type,@card_hao,@card_order,@card_Serial,
			0,0
			)
			set @xh=SCOPE_IDENTITY()
			
			--生成转换卡号格式
			exec ClientPro_Card_formate @xh,@card_lx
			
			--5.生成个人账户
			--计算Card_order
			select @card_order=(max(card_order)+1) from dt_ac_card where user_serial=@user_serial
			insert into dt_ac_card(
			user_serial,card_serial,card_hao,card_order,ac_money,ac_addm,ac_addo,ac_subm,
			ac_subo,ac_regm,ac_make,ac_eachm,ac_eacho,ac_state,sj,ks_sj,gly_no
			)VALUES(
			@user_serial,@card_serial,@card_hao,isnull(@card_order,1),0,0,0,0,
			0,0,0,0,0,0,getdate(),getdate(),@gly_no
			)		
			
			
			--5.生成联机账户
			if (select count(1) from dt_ac_link where user_serial=@user_serial)=0
			begin
				insert into dt_ac_link(
				user_serial,card_serial,card_order,ac_money,ac_addm,ac_addo,ac_subm,
				ac_subo,ac_regm,ac_make,ac_eachm,ac_eacho,ac_state,sj,gly_no
				)VALUES(
				@user_serial,@card_serial,@card_order,0,0,0,0,
				0,0,0,0,0,0,getdate(),'admin'
				)
			end
			
			--6.更新人员表状态
			update dt_user set user_ac = 1,user_card=@card_hao,user_sj=getdate()
			where user_serial = @user_serial
			
			
			--7.生成增量档案日志
			insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
			values(1,1,0,@user_serial,@card_serial,getdate(),@ip,@gly_no) 
			
			--新增照片日志
			insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
			values(1,2,0,@user_serial,@card_serial,getdate(),@ip,@gly_no)
			
			--新增指纹日志
			insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
			values(1,3,0,@user_serial,@card_serial,getdate(),@ip,@gly_no)
			
			--增加门禁员工密码日志
			insert into wt_mj_ver(type,lx,log_sj,log_ip,gly_no,gate_bh,log_lx,log_bz,fx)
			values(0,2,getdate(),null,null,null,1,@user_serial,null)
			
			---更新发卡日志
			Update WT_CARD_LOG set lx=0,log_sj=getdate(),log_ip=@ip,gly_no=@gly_no where lx=12 and user_serial=@user_serial and card_new=@card_serial
					
			
			/*
			insert into dt_ac_link(
			user_serial,card_serial,card_order,ac_money,ac_addm,ac_addo,ac_subm,
			ac_subo,ac_regm,ac_make,ac_eachm,ac_eacho,ac_state,sj,gly_no
			)VALUES(
			@user_serial,@card_serial,@card_order,0,0,0,0,
			0,0,0,0,0,0,getdate(),'admin'
			)
			*/
			---判断卡片信息
			---生成dt_card
			---生成dt_card_user
			---生成dt_ac_card
			---更新dt_user
			---生成更新日志
			---生成操作日志		

			--return
		commit transaction		
	end	
	----成功统一返回值
	if @lx <> 2
	select 1
end try

begin catch	--异常捕获
	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	begin		 
		select 4	
		print '发卡失败,工号:'+@user_no+',原因:'+ERROR_MESSAGE() 
		ROLLBACK TRANSACTION;
    end;
end catch

go

if object_id('ClientPro_Card_loss') is not null
drop proc ClientPro_Card_loss
go
create procedure ClientPro_Card_loss 	--挂失处理过程
(
@user_serial bigint,		--人员序号
@card_hao varchar(20),		--卡号
@bz nvarchar(50),			--摘要
@lx int,					--过程类型（1:软件处理； 2:同步挂失）
@ip varchar(20),			--ip
@gly_no nvarchar(50),		--管理员
@reg_serial varchar(50)		--企业编号
)
as
set nocount on

---
declare @card_serial char(8)	--卡顺序号
declare @card_lx int			--卡类型
declare @card_type int			--卡状态
declare @loss_count int			--挂失次数
declare @user_no varchar(100)

begin try

	select @card_serial=card_serial,@card_type=card_type,@loss_count=isnull(loss_count,0)+1
	from dt_card 
	where user_serial=@user_serial and card_hao=@card_hao
	
	select @user_no = user_no from dt_user where user_serial = @user_serial
	
	--1.人员不存在
	if (select count(1) from dt_user where user_serial=@user_serial)=0
	begin

		select 2
		print '人员不存在!'	
		return
		
	end

	--2.卡号状态不正常
	if(@card_type=1 or @card_type=2)	--挂失状态判断
	begin

		select 3	--卡号状态不正常 无法挂失
		print '卡号状态不正常,无法挂失,卡号:'+@card_hao
		return

	end

	begin transaction

		--1.更新卡户状态
		update dt_ac_card 
		set ac_state=1 
		where user_serial=@user_serial 
		and card_serial=@card_serial
		
		--2.更新卡号表状态
		update dt_card 
		set card_type=1,sj=getdate(),card_bz=@bz,loss_count=@loss_count
		where user_serial=@user_serial 
		and card_serial=@card_serial
		
		--3.更新人员账户表
		update dt_user 
		set user_card='' 
		where user_serial=@user_serial
		
		--4.删除卡号转换表
		delete a from dt_card_user a,dt_card b 
		where a.Parent_xh=b.xh 
		and b.user_serial=@user_serial 
		and b.card_serial=@card_serial
		
		--5.生成增量日志
		insert into WT_USER_UP(lx,log_type,card_serial,user_serial,log_sj,log_ip,gly_no) 
		values(2,1,@card_Serial,@user_serial,getdate(),@ip,@gly_no)
		
		--6.生成卡挂失日志
		insert into WT_CARD_LOG(
		lx,log_type,log_state,user_serial,card_old,card_new,
		log_xh,log_sj,log_ip,gly_no,regserial) 
		VALUES(
		1,0,0,@user_serial,@card_hao,@card_serial,
		@card_lx,getdate(),@ip,null,@reg_serial
		)

		--7.返回成功
		if @lx <> 2
		select 1
		
	commit transaction

end try

begin catch			--异常捕获

	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	BEGIN
		
		select 4	--过程异常失败
		print '挂失失败,工号:'+@user_no+',原因:'+ERROR_MESSAGE() 
		ROLLBACK TRANSACTION
		 
	END

end catch


go

if object_id('ClientPro_Card_unloss') is not null
drop proc ClientPro_Card_unloss
go
create procedure ClientPro_Card_unloss 	--解挂处理过程
(
@user_serial bigint,	--人员序号
@card_serial char(8),	--卡顺序号
@card_hao varchar(20),	--物理卡号
@card_lx int,			--卡类型
@lx int,				--过程类型（1:软件处理； 2:同步解挂）
@ip varchar(20),		--ip
@gly_no nvarchar(50),	--管理员
@reg_serial varchar(20)	--企业编号
)
as

set nocount on

declare @xh int		--卡表序号
declare @user_no varchar(100)

if(@lx=2)	--同步解挂
begin

	select @card_serial=card_serial,@user_serial=user_serial 
	from dt_card where card_hao=@card_hao and card_type=1

end

begin try
	select @user_no = user_no from dt_user where user_serial = @user_serial

	if not exists(select 1 from dt_card where user_serial=@user_serial and card_serial=@card_serial
	and card_hao=@card_hao and card_type=1)
	begin
	
		select 2	--不存在挂失卡片,无法解挂
		print '不存在挂失卡片,无法解挂,工号:'+@user_no
		return
		
	end
	
	begin transaction	--开始事务
	
		select @xh=max(xh) from dt_card where user_serial=@user_serial and card_serial=@card_serial
		
		--1.更新卡户状态
		update dt_ac_card
		set ac_state=0
		where user_serial=@user_serial
		and card_serial=@card_serial
		
		--2.更新卡表状态
		update dt_card
		set card_type=0,sj=getdate()
		where user_serial=@user_serial
		and card_serial=@card_serial
		
		--3.更新人员档案卡号
		update dt_user
		set user_card=@card_hao 
		where user_serial=@user_serial
		
		--4.生成卡号转换格式表数据
		exec ClientPro_Card_formate @xh,@card_lx
		
		--5.生成增量更新日志
		insert into WT_USER_UP(lx,log_type,card_serial,user_serial,log_sj,log_ip,gly_no)
		values (2,1,@card_Serial,@user_serial,getdate(),@ip,@gly_no)
		
		--6.生成卡片解挂日志
		insert into WT_CARD_LOG(
		lx,log_type,log_state,user_serial,card_old,card_new,
		log_xh,log_sj,log_ip,gly_no,regserial
		)values(
		2,0,0,@user_serial,@card_hao,@card_serial,
		@card_lx,getdate(),@ip,@gly_no,@reg_serial
		)
		
		--7.返回成功
		if @lx <> 2
		select 1
		
	commit transaction
	
end try

begin catch		--异常捕获

	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	BEGIN
		
		select 4	--过程异常失败
		print '解挂失败,工号:'+@user_no+',原因:'+ERROR_MESSAGE() 
		ROLLBACK TRANSACTION
		 
	END

end catch
go


if object_id('ClientPro_Card_back') is not null
drop proc ClientPro_Card_back
go
create procedure ClientPro_Card_back 	--退卡处理过程
(
@user_serial bigint,		 --人员序号
@card_serial char(8),		 --卡顺序号
@card_hao varchar(20),	 	 --物理卡号
@card_verify varchar(20),	 --卡片认证码
@card_form varchar(20),		 --卡片目录
@card_lx int,			 	 --卡类型
@card_type int,				 --卡状态
@lx int,					 --过程类型（0：预处理；1：更新结果状态； 2:同步退卡）
@ip varchar(20),			 --ip
@gly_no nvarchar(50),		 --管理员
@reg_serial varchar(20)		 --企业编号
)
as

set nocount on

---变量定义
declare @xh int
declare @readd_count int
declare @ac_money money
declare @user_no varchar(100)

set @readd_count=0
set @ac_money=0

begin try

	if(@lx=0)	--退卡预处理
	begin
		
		begin transaction
			
			--1.生成dt_card_temp记录
			insert into dt_card_temp(lx,user_serial,gly_no,card_lx,card_hao,card_serial,card_verify,card_type,card_order,sj) 
			select lx,user_serial,gly_no,card_lx,@card_hao,@card_serial,@card_verify,@card_type,card_order,GETDATE()
			from dt_card
			where user_serial = @user_serial
			and card_serial = @card_serial
			and card_hao = @card_hao
			
			--2.更新卡号表卡片认证码和目录区
			update dt_card set card_verify=@card_verify,card_form=@card_form
			where user_serial = @user_serial
			and card_serial = @card_serial
			and card_hao = @card_hao
			
			--3.生成预退卡日志
			insert into wt_card_log(
			lx,log_type,log_state,user_serial,card_old,
			card_new,log_xh,log_sj,log_ip,gly_no,log_lx,log_group,
			log_row,log_erro,bz,regserial
			)values(
			13,0,0,@user_serial,@card_hao,
			@card_serial,@card_lx,getdate(),@ip,@gly_no,0,null,
			null,0,null,@reg_serial
			)
		
		commit transaction
		
	end

	else if(@lx=1)	--完成并更新结果
	begin

		begin transaction
		
		select @xh=max(xh) from dt_card_temp where user_serial=@user_serial and card_serial=@card_serial
		and ((card_hao=@card_hao and card_type=4) or card_type=5)
		
		select @readd_count=isnull(readd_count,0) from dt_card where user_serial=@user_serial 
		and card_serial=@card_serial
		and card_hao=@card_hao
		
		if(@card_type=5)	--坏卡处理
		begin
		
			update dt_card_temp set card_type=@card_type,card_hao=''
			where xh=@xh
			
			commit transaction
			select 1
			return

		end
		else
		begin
		
			--1.删除转换卡数据
			delete a from DT_CARD_USER a,dt_card b where a.Parent_xh=b.xh and b.user_serial = @user_serial and b.card_serial = @card_serial
			
			--2.删除卡表数据
			delete from dt_card where user_serial = @user_serial and card_serial = @card_serial
			
			--3.更新人员表数据
			update dt_user 
			set user_card='' 
			where user_serial = @user_serial 
			and user_card = @card_hao
			
			--4.更新人员账户信息
			update dt_ac_user
			set ac_eacho=a.ac_eacho-b.ac_eacho
			from dt_ac_user a,dt_ac_card b
			where a.user_serial=b.user_serial
			and b.user_serial = @user_serial
			and b.card_serial = @card_serial
			
			--5.更新人员卡户信息
			update dt_ac_card 
			set ac_state=2,ac_eacho=0 
			where user_serial = @user_serial 
			and card_serial = @card_serial
			
			--生成增量更新数据
			--正常卡退卡&挂失卡退卡(未补卡)
			if(@readd_count=0)
			begin
				insert into WT_USER_UP(lx,log_type,card_serial,user_serial,log_sj,log_ip,gly_no) 
				values(2,1,@card_Serial,@user_serial,getdate(),@ip,@gly_no)
			end
			
			---人员是否离职（离职人员进行销户）
			--if exists(select 1 from dt_user where user_serial=@user_serial and user_type=51)
			--begin
			--	
			--	--更新人员账户为初始状态
			--	update dt_ac_user set ac_type='0000000000000001',ac_pass='123456',ac_jssj=getdate(),ac_state=1,sj=getdate(),Ac_addm=0,Ac_subm=0,Ac_regm=0,Ac_make=0,Ac_eachm=0 where user_serial = @user_serial
			--	--更新人员表账户状态为0
			--	update dt_user set user_ac=0 where user_serial=@user_serial
			--	
			--end
			
			--6.更新退卡日志状态
			update WT_CARD_LOG 
			set lx=5,card_old = @card_hao,log_xh = @card_lx,
				log_sj = getdate(),log_ip =@ip,gly_no=@gly_no
			where lx=13
			and user_serial = @user_serial
			and card_new = @card_serial

		
		end
		
		---返回成功
		
		
		commit transaction

	end
------------------/////////////////////同步退卡处理/////////////----------
	else if(@lx=2)	--同步退卡处理
	begin
		select @user_no = user_no from dt_user where user_serial = @user_serial
	
		select @card_serial = card_serial 
		from dt_card where card_hao= @card_hao

		select @ac_money=ac_money from dt_ac_link where user_serial=@user_serial
		if(@ac_money<>0)
		begin
		
			select 3	--账户存在余额,先退款处理
			print '账户存在余额,先退款处理,工号:'+@user_no
			return
		
		end
		
		
		--0.生成预退卡日志
		insert into wt_card_log(
		lx,log_type,log_state,user_serial,card_old,
		card_new,log_xh,log_sj,log_ip,gly_no,log_lx,log_group,
		log_row,log_erro,bz,regserial
		)values(
		13,0,0,@user_serial,@card_hao,
		@card_serial,@card_lx,getdate(),@ip,@gly_no,0,null,
		null,0,null,@reg_serial
		)
		
		--1.生成dt_card_temp记录
		insert into dt_card_temp(lx,user_serial,gly_no,card_lx,card_hao,card_serial,card_verify,card_type,card_order,sj) 
		select lx,user_serial,gly_no,card_lx,@card_hao,@card_serial,@card_verify,@card_type,card_order,GETDATE()
		from dt_card
		where user_serial = @user_serial
		and card_serial = @card_serial
		and card_hao = @card_hao
		
		--1.删除转换卡数据
		delete a from DT_CARD_USER a,dt_card b where a.Parent_xh=b.xh and b.user_serial = @user_serial and b.card_serial = @card_serial
		
		--2.删除卡表数据
		delete from dt_card where user_serial = @user_serial and card_serial = @card_serial
		
		--3.更新人员表数据
		update dt_user 
		set user_card='' 
		where user_serial = @user_serial 
		and user_card = @card_hao
		
		--4.更新人员账户信息
		update dt_ac_user
		set ac_eacho=a.ac_eacho-b.ac_eacho
		from dt_ac_user a,dt_ac_card b
		where a.user_serial=b.user_serial
		and b.user_serial = @user_serial
		and b.card_serial = @card_serial
		
		--5.更新人员卡户信息
		update dt_ac_card 
		set ac_state=2,ac_eacho=0 
		where user_serial = @user_serial 
		and card_serial = @card_serial
		
		--生成增量更新数据
		--正常卡退卡&挂失卡退卡(未补卡)

		insert into WT_USER_UP(lx,log_type,card_serial,user_serial,log_sj,log_ip,gly_no) 
		values(2,1,@card_Serial,@user_serial,getdate(),@ip,@gly_no)

		
		---人员是否离职（离职人员进行销户）
		--if exists(select 1 from dt_user where user_serial=@user_serial and user_type=51)
		--begin
		--	
		--	--更新人员账户为初始状态
		--	update dt_ac_user set ac_type='0000000000000001',ac_pass='123456',ac_jssj=getdate(),ac_state=1,sj=getdate(),Ac_addm=0,Ac_subm=0,Ac_regm=0,Ac_make=0,Ac_eachm=0 where user_serial = @user_serial
		--	--更新人员表账户状态为0
		--	update dt_user set user_ac=0 where user_serial=@user_serial
		--	
		--end
		
		--6.更新退卡日志状态
		update WT_CARD_LOG 
		set lx=5,card_old = @card_hao,log_xh = @card_lx,
			log_sj = getdate(),log_ip =@ip,gly_no=@gly_no
		where lx=13
		and user_serial = @user_serial
		and card_new = @card_serial		
	
	end
	
end try

begin catch		--异常捕获

	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	BEGIN
		
		select 4	--过程异常失败
		print '退卡失败,工号:'+@user_no+',原因:'+ERROR_MESSAGE() 
		ROLLBACK TRANSACTION
		 
	END

end catch
if @lx <> 2 select 1
go


if object_id('ClientPro_Card_replace') is not null
drop proc ClientPro_Card_replace
go
create procedure ClientPro_Card_replace 	--补卡卡处理过程
(
@user_serial bigint,				--人员序号
--@ac_type varchar(16),				--账户类型
@lx int,							--预处理；更新成功(lx=2 同步补卡)
@card_type int,						--卡号状态
@old_card_serial char(8),			--旧卡顺序号
@old_card_hao varchar(20),			--旧卡物理卡号
@old_card_lx int,					--旧卡卡类型
--@cash_amt money,					--现金余额
--@sub_amt money,						--补贴余额
--@each_amt int,						--份余额
@new_card_serial char(8),			--新卡顺序号
@new_card_hao varchar(20),			--新物理卡号
@new_card_lx int,					--新卡类型(同步为100)
@card_work int,						--卡片读写标志(同步为0)
@ip varchar(20),					--ip
@reg_serial varchar(20),			--企业编号
@gly_no nvarchar(50)				--管理员编号
)
as
set nocount on
set ansi_warnings off
---变量定义
declare @xh int
declare @casher_bh  varchar(16)	--收款员编号
declare @ac_no char(16)			--账户编号
declare @rec_count int			--计数器
declare @card_order int			--卡号顺序
declare @sub_bh varchar(16)
declare @each_bh varchar(16)
declare @dep_serial bigint
declare @deal_regserial varchar(20)
declare @deal_site int
declare @version_no int
declare @mould int
declare @deal_lx int
declare @isAccChange int
declare @isCardChange int
declare @isCardReplace int
declare @isUndo int
declare @old_cash money
declare @card_count int
declare @old_card_cash money
declare @old_card_subsidy money
declare @old_card_money money
declare @old_card_each int
declare @WAcc_state int
declare @WCard_state int
declare @jl_sj datetime
declare @sj datetime
declare @deal_sj datetime
declare @mx_xh int
declare @old_subsidy money
declare @old_sub_each int
declare @new_cash money
declare @old_each int
declare @old_sub_cash money
declare @old_sub_subsidy money
declare @new_subsidy money
declare @new_each int
declare @new_sub_cash money
declare @new_sub_subsidy money
declare @new_sub_each int
declare @bill_no varchar(16)
declare @state int
declare @cash_amt money					--现金余额
declare @sub_amt money					--补贴余额
declare @each_amt int					--份余额
declare @sub_kssj datetime		--补贴开始时间
declare @sub_jssj datetime		--补贴清零日期
declare @each_kssj datetime 	--份开始时间
declare @each_jssj datetime		--份清零日期
declare @isvalid_date int		--是否逾期清零

set @deal_sj=getdate()
set @jl_sj=getdate()
set @card_count=1


select @Deal_regserial=gly_regserial from wt_gly where gly_no=@gly_no
set @deal_site=case when isnull(@reg_serial,'')=isnull(@deal_regserial,'') then 0 else 1 end

select @dep_serial=user_dep
from dt_user where user_serial=@user_serial


begin try

	if(@lx=0)
	begin
	
		--1.人员是否有卡
		if (select isnull(user_card,'') from dt_user where user_serial=@user_serial)<>''
		begin
		
			select 2	
			return
			
		end
		
		--2.新卡是否占用
		if (select count(card_hao) from dt_card where card_hao=@new_card_hao and card_type<>4)>0
		begin
		
			select 3
			return
			
		end
		begin transaction
			
			---1.1插dt_card表数据
			--计算Card_order

			select @cash_amt=ac_addo,@sub_amt=ac_subo,@each_amt=ac_eacho
			from dt_ac_card
			where card_serial=@old_card_serial and card_hao=@old_card_hao
			
			select @card_order=(max(card_order)+1) from dt_card where user_serial=@user_serial
				
			insert into dt_card(
			lx, gly_no, sj, user_serial, card_lx, 
			card_type, card_hao, card_order, card_serial, 
			loss_count, isCardReplace
			)values(
			0,@gly_no,getdate(),@user_serial,@new_card_lx,
			@card_type,@new_card_hao,isnull(@card_order,1),@new_card_serial,
			0,1
			)
			
			--1.2写预处理日志
			insert into wt_card_log(
			lx,log_type,log_state,user_serial,card_old,
			card_new,log_xh,log_sj,log_ip,gly_no,log_lx,log_group,
			log_row,log_erro,bz,regserial
			)values(
			12,0,0,@user_serial,@new_card_hao,
			@new_card_serial,@new_card_lx,getdate(),@ip,@gly_no,0,null,
			null,0,null,@reg_serial
			)
			
			--1.3生成预处理记录（充值、补贴、份）
			if(@cash_amt>0)	--补卡充值
			begin
			
				set @mould=0		--金额模式
				set @lx=2			--充值
				set @deal_lx=13		--补卡充值
				set @isAccchange=0
				set @isCardchange=1
				set @iscardreplace=1
				set @isundo=0
				set @old_cash=@cash_amt
				set @old_card_cash=0
				set @old_card_money=0
				set @old_card_subsidy=0
				set @old_card_each=0
				set @WAcc_state=0
				set @Wcard_state=0	--未写卡
				set @jl_sj=getdate()

				
						--1.获取收款员报表id
				if not exists(select 1 from real_xf_casher where gly_no=@gly_no and Settle_acc_state=0 
				and isnull(reg_serial,'')=@reg_serial)
				begin
				
					--生成报表ID  插入收款单据数据
					insert into real_xf_casher(bh,Reg_serial, Add_qty, Add_amt, Local_add_qty, Local_add_amt, 
					Other_add_qty, Other_add_amt, Local_undo_qty, Local_undo_amt, Other_undo_qty, Other_undo_amt, 
					Draw_qty, Draw_amt, Draw_sub_qty, Draw_sub_amt, Total_amt, kssj, jssj, 
					Settle_acc_state, Gly_no, Ip)
					values(
					null,@reg_serial,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,getdate(),null,0,@gly_no,@ip
					)
					
					--select @casher_bh=CONVERT(varchar(100), GETDATE(),12)+cast(right('0000000000'+cast(xh as varchar(10)),10) as varchar(10))
					--from real_xf_casher
					--where gly_no=@gly_no and Settle_acc_state=0
					--and reg_serial=@reg_serial and bh is null
					
					update real_xf_casher set bh=CONVERT(varchar(100), GETDATE(),12)+cast(right('0000000000'+cast(xh as varchar(10)),10) as varchar(10))
					where bh is null			
				
				end
				
				select @casher_bh=bh from real_xf_casher
				where isnull(reg_serial,'')=@reg_serial
				and gly_no=@gly_no
				and Settle_acc_state=0

				insert into Real_XF_Cash(
				casher_bh,user_regserial,deal_regserial,deal_site,dep_serial,user_serial,card_lx,card_hao,
				card_serial,mould,lx,deal_lx,isAccChange,isCardChange,isCardReplace,isUndo,old_cash,
				cash_amt,card_count,old_card_cash,old_card_money,old_card_subsidy,old_card_each,WAcc_state,WCard_state,sj,gly_no,ip
				)values(
				@casher_bh,@reg_serial,@deal_regserial,@deal_site,@dep_serial,@user_serial,@new_card_lx,@new_card_hao,
				@new_card_serial,@mould,@lx,@deal_lx,@isAccChange,@isCardChange,@isCardReplace,@isUndo,@old_cash,
				@cash_amt,@card_count,@old_card_cash,@old_card_money,@old_card_subsidy,@old_card_each,@WAcc_state,@WCard_state,
				@jl_sj,@gly_no,@ip
				)
				
			end
			
			if(@sub_amt>0)		--补卡补贴
			begin
			
				set @mould=0
				set @isvalid_date=0
				
				select top 1 @sub_kssj=sub_kssj,@sub_jssj=sub_jssj from real_xf_sublog where 
				user_serial=@user_serial and card_serial=@old_card_serial
				order by xh desc
				
				if(@sub_jssj is not null)
				begin
				
					set @isvalid_date=1
				
				end
				
				select @version_no=max(version_no)+1 from real_sub_slave where user_serial=@user_serial
				and card_serial=@new_card_serial
				
				insert into real_sub_master(bh,reg_serial,lx,deal_lx,total_amt,
				isValid_date,isClear_old,iscardreplace,kssj,jssj,sj,WAcc_sj,state,
				gly_no,ip,acc_gly_no,acc_ip,bz)
				values(null,@reg_serial,0,141,@sub_amt,
				@isValid_date,0,1,@sub_kssj,@sub_jssj,getdate(),null,0,
				@gly_no,@ip,null,null,'补卡补贴单据')
				
				--select @sub_bh=CONVERT(varchar(100), GETDATE(),12)+cast(right('0000000000'+cast(xh as varchar(10)),10) as varchar(10))
				--from real_sub_master
				--where gly_no=@gly_no and state=0
				--and reg_serial=@reg_serial and bh is null
				
				update real_sub_master set bh=CONVERT(varchar(100), GETDATE(),12)+cast(right('0000000000'+cast(xh as varchar(10)),10) as varchar(10))
				where  bh is null
				
				select @sub_bh=bh from real_sub_master
				where isnull(reg_serial,'')=@reg_serial and gly_no=@gly_no
				and state=0 and iscardreplace=1
				
				insert into real_sub_slave(parent_bh,lx,user_serial,dep_serial,card_lx,
				card_hao,card_serial,amt,version_no,WAcc_state,WCard_state,iscardreplace,
				sj,deal_sj,gly_no,ip
				)
				values(@sub_bh,0,@user_serial,@dep_serial,@new_card_lx,
				@new_card_hao,@new_card_serial,@sub_amt,@version_no,0,0,1,
				getdate(),null,@gly_no,@ip
				)
				
			end
			
			if(@each_amt>0)	--补卡份
			begin
				
				set @mould=1
				set @mould=0
				set @isvalid_date=0
				
				select top 1 @each_kssj=sub_kssj,@each_jssj=sub_jssj from real_xf_sublog where 
				user_serial=@user_serial and card_serial=@old_card_serial
				order by xh desc
				
				if(@each_jssj is not null)
				begin
				
					set @isvalid_date=1
				
				end				
				
				
				
				select @version_no=max(version_no)+1 from real_each_slave where user_serial=@user_serial
				and card_serial=@new_card_serial
				
				insert into real_each_master(bh,reg_serial,lx,deal_lx,total_amt,
				isValid_date,isClear_old,iscardreplace,kssj,jssj,sj,WAcc_sj,state,
				gly_no,ip,acc_gly_no,acc_ip,bz)
				values(null,@reg_serial,0,151,@each_amt,
				@isvalid_date,0,1,@each_kssj,@each_jssj,getdate(),null,0,
				@gly_no,@ip,null,null,'补卡份单据')
				
				--select @each_bh=CONVERT(varchar(100), GETDATE(),12)+cast(right('0000000000'+cast(xh as varchar(10)),10) as varchar(10))
				--from real_each_master
				--where gly_no=@gly_no and state=0
				--and reg_serial=@reg_serial and bh is null
				
				update real_each_master set bh=CONVERT(varchar(100), GETDATE(),12)+cast(right('0000000000'+cast(xh as varchar(10)),10) as varchar(10))
				where bh is null
				
				select @each_bh=bh from real_each_master
				where isnull(reg_serial,'')=@reg_serial and gly_no=@gly_no
				and state=0 and iscardreplace=1
				
				
				insert into real_each_slave(parent_bh,lx,user_serial,dep_serial,card_lx,
				card_hao,card_serial,amt,version_no,WAcc_state,WCard_state,iscardreplace,
				sj,deal_sj,gly_no,ip)
				values(@each_bh,0,@user_serial,@dep_serial,@new_card_lx,
				@new_card_hao,@new_card_serial,@each_amt,@version_no,0,0,1,
				@sj,@deal_sj,@gly_no,@ip)
				
			end
			
		commit transaction
		
	end
	
	else if(@lx=1)	--成功后更新
	begin
	
		begin transaction
		
		select @xh=max(xh) from dt_card where user_serial=@user_serial and card_serial=@new_card_serial
		and ((card_hao=@new_card_hao and card_type=4) or card_type=5)
		
		select @cash_amt=isnull(ac_addo,0),@sub_amt=isnull(ac_subo,0),@each_amt=isnull(ac_eacho,0)
		from dt_ac_card
		where card_serial=@old_card_serial and card_hao=@old_card_hao
		
		if(@card_type=5)	--坏卡
		begin
		
			--
			update dt_card set card_type=@card_type,card_hao=''
			where xh=@xh
			
			--更新预处理记录
			if(@cash_amt>0)
			begin
				update real_xf_cash set card_hao='' where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1 and card_hao=@new_card_hao
			end
			
			if(@sub_amt>0)
			begin
				update real_sub_slave set card_hao='' where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1 and card_hao=@new_card_hao
			end
			
			if(@each_amt>0)
			begin	
				update real_each_slave set card_hao='' where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1 and card_hao=@new_card_hao
			end
			
			
			commit transaction
			
			select 1
			return
		
		end
		
		else
		begin
		--select @cash_amt,@sub_amt,@each_amt
		--	if(isnull(@ac_type,'')<>'')	--开户
		--	begin
		--	
		--		while 1 = 1
		--		begin
        --
		--			set @ac_no = replace(replace(replace(convert(varchar(19),getdate(),21),'-',''),':',''),' ','') + right(replicate('0', 1) + convert(varchar(10),ceiling(rand()*99)), 2) 
		--			select @rec_count = count(*) from dt_ac_user where ac_no = @ac_no
		--			if @rec_count > 0
		--				begin
		--					waitfor delay '00:00:00.20'
		--					continue
		--				end
		--			else
		--			begin
		--			
		--				insert into dt_ac_user(ac_no,ac_type,user_serial,ac_pass,ac_kssj,ac_jssj,ac_state,sj,gly_no,ac_money,ac_make,ac_addo,ac_subo) select @ac_no,@ac_type,@user_serial,ac_pass,getdate() as kssj,case money_type when 0  then DATEADD(year,ac_limit,getdate()) when 1 then DATEADD(month,ac_limit,getdate()) when 2  then DATEADD(day,ac_limit,getdate()) end as jssj,0,getdate(),@gly_no,0,0,0,0 from DT_AC_TYPE where Ac_bh=@ac_type
		--				
		--				break
		--			end
		--			
		--		end
		--			
		--	end
			
			---更新卡表状态
			update dt_card set card_hao=@new_card_hao,card_type=@card_type,card_work=@card_work,
					card_lx=@new_card_lx,sj=getdate()
			where xh=@xh
			
			--更新旧卡补卡状态
			update dt_card set readd_count=1 where user_serial=@user_serial 
			and card_serial=@old_card_serial
			and card_hao=@old_card_hao
			
			---生成转换格式数据
			exec ClientPro_Card_formate @xh,@new_card_lx
			
			---生成人员卡户
			--计算Card_order
			select @card_order=(max(card_order)+1) from dt_ac_card where user_serial=@user_serial
			insert into dt_ac_card(
			user_serial,card_serial,card_hao,card_order,ac_money,ac_addm,ac_addo,ac_subm,
			ac_subo,ac_regm,ac_make,ac_eachm,ac_eacho,ac_state,sj,ks_sj,gly_no
			)VALUES(
			@user_serial,@new_card_serial,@new_card_hao,isnull(@card_order,1),0,0,0,0,
			0,0,0,0,0,0,getdate(),getdate(),@gly_no
			)
			
			---更新人员表状态
			update dt_user set user_ac = 1,user_card=@new_card_hao,user_sj=getdate()
			where user_serial = @user_serial
		
			---生成增量日志
			insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
			values(1,1,0,@user_serial,@new_card_serial,getdate(),@ip,@gly_no)
			
		
			
			---更新发卡日志
			Update WT_CARD_LOG set lx=3,log_sj=getdate(),log_ip=@ip,gly_no=@gly_no where lx=12 and user_serial=@user_serial and card_new=@new_card_serial
			
			---记账处理
			
			--明细账处理（补卡充值、补卡补贴记账、补卡份记账）
			if(@cash_amt>0)
			begin
				--select @mx_xh=max(xh)+1 from real_xf_mx 
				
				update real_xf_cash set card_hao=@new_card_hao,wcard_state=1
				 where user_serial=@user_serial
				 and card_serial=@new_card_serial and iscardreplace=1 
				
				---取交易前子账户\总账户余额
				select @old_cash=ac_addo,@Old_subsidy=ac_subo,@Old_each=ac_eacho
				from dt_ac_user where user_serial=@user_serial
				
				select @old_sub_cash=ac_addo,@Old_sub_subsidy=ac_subo,@Old_sub_each=ac_eacho
				from dt_ac_card where user_serial=@user_serial and card_serial=@new_card_serial
				
				set @new_cash=@old_cash
				set @new_subsidy=@old_subsidy
				set @new_each=@old_each
				
				set @new_sub_cash=@old_sub_cash+@cash_amt
				set @new_sub_subsidy=@Old_sub_subsidy
				set @new_sub_each=@old_sub_each
				
				
				insert into real_xf_mx(	--补卡充值明细
				Dep_serial, User_serial, card_lx, card_hao, Card_serial, Card_count, 
				user_regserial, Deal_regserial,Deal_site,Merchant_no,Dev_serial, 
				Jl_serial, Jl_source, Mould, deal_lx, lx, isAccChange, iscardchange, 
				old_cash, Old_subsidy, Old_each, New_cash, 
				New_subsidy,New_each, Old_sub_cash, Old_sub_subsidy, 
				Old_sub_each, New_sub_cash, New_sub_subsidy, 
				New_sub_each, Old_card_cash,Old_card_subsidy, 
				Old_card_each, New_card_cash, New_card_subsidy, 
				New_card_each, cash_amt, Sub_amt, Each_amt, Each_price, Deal_sj, 
				WAcc_sj, Settle_acc_id, Settle_acc_state, Settle_acc_rq, Dev_time_no, Acc_time_no, 
				Gly_no, Bz, Version_no,isCardReplace,isUndo
				)values(
				@Dep_serial, @User_serial, @new_card_lx, @new_card_hao, @new_Card_serial, @card_count, 
				@reg_serial, @Deal_regserial,@Deal_site,0,0, 
				0, 1, 0, 13, 2, 0, 1, 
				@old_cash, @old_subsidy, @old_each, @cash_amt, 
				@New_subsidy,@New_each, @Old_sub_cash, @Old_sub_subsidy, 
				@Old_sub_each, @New_sub_cash, @New_sub_subsidy, 
				@New_sub_each, 0,0, 
				0, @cash_amt, 0, 
				0, @cash_amt, 0, 0, 0, getdate(), 
				getdate(), 0, 0, null, null, null, 
				@Gly_no, null, 0,1,0
				)
			end
			
			if(@sub_amt>0)	--补卡补贴名单记账
			begin
			
				--select @mx_xh=max(xh)+1 from real_xf_mx 
				
				update real_sub_slave set card_hao=@new_card_hao where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1
				---获取单据编号
				select @bill_no=parent_bh from real_sub_slave where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1
				
				select @state=state from Real_sub_master where Bh=@bill_no
				
				if(@state=0)		--单据未记账
				begin
				
					exec Real_BillSubsidy_account  @reg_serial,@bill_no,@ip,@gly_no
					
--					---取交易前子账户\总账户余额
--					select @old_cash=ac_addo,@Old_subsidy=ac_subo,@Old_each=ac_eacho
--					from dt_ac_user where user_serial=@user_serial
--					
--					select @old_sub_cash=ac_addo,@Old_sub_subsidy=ac_subo,@Old_sub_each=ac_eacho
--					from dt_ac_card where user_serial=@user_serial and card_serial=@new_card_serial
--					
--					set @new_cash=@old_cash
--					set @new_subsidy=@old_subsidy
--					set @new_each=@old_each
--					
--					set @new_sub_cash=@old_sub_cash
--					set @new_sub_subsidy=@Old_sub_subsidy+@sub_amt
--					set @new_sub_each=@old_sub_each
--					
--					----生成补卡补贴明细记账记录
--					insert into real_xf_mx(		--补贴明细
--					xh,Dep_serial, User_serial, card_lx, card_hao, Card_serial, Card_count, 
--					user_regserial, Deal_regserial,Deal_site,Merchant_no,Dev_serial, 
--					Jl_serial, Jl_source, Mould, deal_lx, lx, isAccChange, iscardchange, 
--					old_cash, Old_subsidy, Old_each, New_cash, 
--					New_subsidy,New_each, Old_sub_cash, Old_sub_subsidy, 
--					Old_sub_each, New_sub_cash, New_sub_subsidy, 
--					New_sub_each, Old_card_cash,Old_card_subsidy, 
--					Old_card_each, New_card_cash, New_card_subsidy, 
--					New_card_each, cash_amt, Sub_amt, Each_amt, Each_price, Deal_sj, 
--					WAcc_sj, Settle_acc_id, Settle_acc_state, Settle_acc_rq, Dev_time_no, Acc_time_no, 
--					Gly_no, Bz, Version_no,isCardReplace,isUndo				
--					)values(
--					isnull(@mx_xh,1),@Dep_serial, @User_serial, @new_card_lx, @new_card_hao, @new_Card_serial, 0, 
--					@reg_serial, @Deal_regserial,@Deal_site,0,0, 
--					0, 1, 0, 141, 3, 0, 1, 
--					@old_cash, @old_subsidy, @old_each, @new_cash, 
--					@New_subsidy,@New_each, @Old_sub_cash, @Old_sub_subsidy, 
--					@Old_sub_each,@New_sub_cash,@New_sub_subsidy, 
--					@New_sub_each,0,0,
--					0, 0, 0,
--					0, 0, @sub_amt, 0, 0, getdate(),
--					getdate(), 0, 0, null, null, null,
--					@Gly_no, null, 0,1,0
--					)
--					
--					update Real_each_master 
--					set state=1,acc_gly_no=@gly_no
--					where isnull(Reg_serial,'')=@Reg_serial 
--					and Bh=@bill_no
					
				end
				
			end
			
			if(@each_amt>0)
			begin
			
			
				update real_each_slave set card_hao=@new_card_hao where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1
				--select @mx_xh=max(xh)+1 from real_xf_mx
				
				---获取单据编号
				select @bill_no=parent_bh from real_each_slave where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1
				
				select @state=state from Real_each_master where Bh=@bill_no
				
				if(@state=0)
				begin
				
					exec Real_BillEach_account @reg_serial,@bill_no,@ip,@gly_no
--					---取交易前子账户\总账户余额
--					select @old_cash=ac_addo,@Old_subsidy=ac_subo,@Old_each=ac_eacho
--					from dt_ac_user where user_serial=@user_serial
--					
--					select @old_sub_cash=ac_addo,@Old_sub_subsidy=ac_subo,@Old_sub_each=ac_eacho
--					from dt_ac_card where user_serial=@user_serial and card_serial=@new_card_serial
--					
--					set @new_cash=@old_cash
--					set @new_subsidy=@old_subsidy
--					set @new_each=@old_each
--					
--					set @new_sub_cash=@old_sub_cash
--					set @new_sub_subsidy=@Old_sub_subsidy+@sub_amt
--					set @new_sub_each=@old_sub_each
--					
--					----生成补卡补贴明细记账记录
--					insert into real_xf_mx(		--补贴明细
--					xh,Dep_serial, User_serial, card_lx, card_hao, Card_serial, Card_count, 
--					user_regserial, Deal_regserial,Deal_site,Merchant_no,Dev_serial, 
--					Jl_serial, Jl_source, Mould, deal_lx, lx, isAccChange, iscardchange, 
--					old_cash, Old_subsidy, Old_each, New_cash, 
--					New_subsidy,New_each, Old_sub_cash, Old_sub_subsidy, 
--					Old_sub_each, New_sub_cash, New_sub_subsidy, 
--					New_sub_each, Old_card_cash,Old_card_subsidy, 
--					Old_card_each, New_card_cash, New_card_subsidy, 
--					New_card_each, cash_amt, Sub_amt, Each_amt, Each_price, Deal_sj, 
--					WAcc_sj, Settle_acc_id, Settle_acc_state, Settle_acc_rq, Dev_time_no, Acc_time_no, 
--					Gly_no, Bz, Version_no,isCardReplace,isUndo				
--					)values(
--					isnull(@mx_xh,1),@Dep_serial, @User_serial, @new_card_lx, @new_card_hao, @new_Card_serial, 0, 
--					@reg_serial, @Deal_regserial,@Deal_site,0,0, 
--					0, 1, 0, 141, 3, 0, 1, 
--					@old_cash, @old_subsidy, @old_each, @new_cash, 
--					@New_subsidy,@New_each, @Old_sub_cash, @Old_sub_subsidy, 
--					@Old_sub_each, @New_sub_cash, @New_sub_subsidy, 
--					@New_sub_each, 0,0,
--					0, 0, 0, 
--					0, 0, @sub_amt, 0, 0, getdate(), 
--					getdate(), 0, 0, null, null, null, 
--					@Gly_no, null, 0,1,0
--					)
--					
--					update Real_each_master 
--					set state=1,acc_gly_no=@gly_no
--					where isnull(Reg_serial,'')=@Reg_serial 
--					and Bh=@bill_no
				
				end
			
			end
			
			--更新子账户余额（原子账户余额清零）人员总账户金额不变
			
			update dt_ac_card 
			set ac_money=ac_money+@cash_amt,ac_addo=ac_addo+@cash_amt,
				ac_addm=ac_addm+@cash_amt
			where user_serial=@user_serial
			and card_serial=@new_card_serial
			and card_hao=@new_card_hao
			
			update dt_ac_card 
			set ac_money=ac_money-(@cash_amt+@sub_amt),ac_addo=ac_addo-@cash_amt,
				ac_subo=ac_subo-@sub_amt,ac_eacho=ac_eacho-@each_amt	--还原挂失卡余额
			where user_serial=@user_serial
			and card_serial=@old_card_serial
			and card_hao=@old_card_hao
			and ac_state=1
			
		end
		commit transaction
	end
	
--------------------////////同步补卡过程////////////-------------
	else if(@lx=2)   --v1.03
	begin
		declare @dev_serial varchar(7)
		declare @out_put int
		--declare @card_lx int

		set @dev_serial=null

	--挂失旧卡片，发新卡片
		select @old_card_serial=card_serial,@card_type=card_type
		from dt_card 
		where user_serial=@user_serial and card_hao=@old_card_hao
		
		create table #temp_return(
		xh int identity(1,1),
		out_put int
		)
		
		insert into #temp_return exec ClientPro_Card_loss @user_serial,@old_card_hao,null,2,'syn','syn',null    --挂失
		
		select @out_put=out_put from #temp_return
		
		if(@out_put =1 or @out_put=3)	--挂失成功
		begin
			
			truncate table #temp_return
			set @out_put=-1
			insert into #temp_return exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card_hao,null,null,0,0,0,'syn','syn',null  --发新卡
			
			select @out_put=out_put from #temp_return
		end
		
		--if(@out_put =1)		--发卡成功
		--begin
		--	select 0
		--end
		--else
		--begin
		--	select 1
		--end
		
		drop table #temp_return
	end;
	
end try

begin catch

			
	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	BEGIN
		
		select ERROR_MESSAGE()+cast(ERROR_LINE() as varchar(10))
		select 4	--过程异常失败
		ROLLBACK TRANSACTION
		 
	END

end catch


go

