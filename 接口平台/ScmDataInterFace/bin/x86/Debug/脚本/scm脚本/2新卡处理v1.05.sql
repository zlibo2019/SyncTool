--v1.01 �Է������̶�dt_card����������dt_user�޿��ŵ�������д���
--v1.02 ���ӿ����洢���̣���������������еĿ�������
--v1.03 ���ͬ������������
--v1.04 ���������ʾ��rollback traction..���⡱
--v1.05 ��Ա��ְ�����˻�����






if object_id('ClientPro_user_kh') is not null
drop proc ClientPro_user_kh
go
create procedure ClientPro_user_kh 	--����
(
@ac_type char(16)=null,		--��Ա�˻�����       ????????????????? ver???
@user_serial bigint,		--��Ա���
@gly_no nvarchar(50) = 'syn',		--����Ա
@result int output
)
as
set nocount on

declare @rec_count int 
declare @ac_no char(16)			--�˻����

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

begin catch	--�쳣���� 
    ROLLBACK TRANSACTION
	set @result = 1;
	print '����ʧ��,Ա�����:'+@user_serial+',ԭ��:'+ERROR_MESSAGE() 	
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
begin catch	--�쳣����
	
	ROLLBACK TRANSACTION;
	print '����:'+@user_no+' ��ְʧ��:'+ERROR_MESSAGE() 

end catch


set nocount off
GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[user_destroy]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop proc user_destroy
GO


create proc user_destroy    
    @user_serial int,           --��Ա���
    @card_hao varchar(20)  --����
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
begin catch	--�쳣����
	
	ROLLBACK TRANSACTION;
	print '����:'+@user_no+' ��ְʧ��:'+ERROR_MESSAGE()  

end catch



GO


if object_id('ClientPro_Card_add') is not null
drop proc ClientPro_Card_add
go
create procedure ClientPro_Card_add 	--�����������
(
@dev_serial char(7),		--�����豸���
@lx int,					--�������ͣ�0��Ԥ����1�����½��״̬; 2:ͬ��������
@ac_type char(16)=null,		--��Ա�˻�����       ????????????????? ver???
@user_serial bigint,		--��Ա���
@card_hao varchar(20),		--������
@jm_kh varchar(20) = null,			--���ܿ���
@card_serial char(8) = null,		--��˳���
@card_lx int = 100,				--������ 1 s50 
@card_work int,				--��д״̬��ͬ��ʱΪ0��
@card_type int,				--��״̬��ͬ��ʱΪ0��
@ip  varchar(20) = 'syn',     		--����IP
@gly_no nvarchar(50) = 'syn',		--����Ա
@reg_serial varchar(50) =null		--��ҵ���
)
as
--����ֵ������ӵ�����
set nocount on

--��������
declare @ac_no char(16)			--�˻����
declare @rec_count int			--������
declare @card_order int			--����˳��
declare @xh int					--���ű����

declare @pxh varchar(8)      	 --������� 
declare @tt_form varchar(50)	 --��ת����ʽ
declare @tempkh varchar(50) 	 --ת���󿨺�

declare @tt_xh int          	 --��ת����ʽ��� 
declare @tt_cut int				 --��ȡλ��
declare @tt_hex int			 	 --����
declare @tt_fields varchar(50)	 --

declare @user_no varchar(100)
declare @result int
declare @tmp_card varchar(50)
declare @module_card int
declare @card_serial_max varchar(50)
declare @count int

--������ֵ
set @rec_count=0
set @card_order=0

begin try
---------////////���������������/////////------------------------
	if(@lx=0)	--����Ԥ�������
	begin

		--1.��Ա�Ƿ��п�
		if (select isnull(user_card,'') from dt_user where user_serial=@user_serial)<>''
		begin
			select 2	
			return
		end
		
	    --3.����Ա�Ƿ��Ѿ��п������dt_card�п��ŵ�dt_user�޿��ŵ������   --v1.01
		if (select count(1) from dt_card where card_hao=@card_hao and user_serial = @user_serial and card_type = 0)>0
		begin
			update dt_user 
		    set user_card = @card_hao  
		    where user_serial = @user_serial
			select 3
			return
		end
		
		--2.�¿��Ƿ񱻱���ռ��
		if (select count(card_hao) from dt_card where card_hao=@card_hao and card_type<>4)>0
		begin
			select 3
			return
		end


	


		begin transaction
			
			---1.1��dt_card������
			--����Card_order
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
			
			--1.2дԤ������־
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
	
	else if(@lx=1)	--��ɲ����½��
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
			
				if(isnull(@ac_type,'')<>'')	--����
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
				
				---���¿���״̬
				update dt_card set card_hao=@card_hao,card_type=@card_type,card_work=@card_work,card_lx=@card_lx,sj=getdate()
				where xh=@xh
				
				---����ת����ʽ����
				exec ClientPro_Card_formate @xh,@card_lx
				
				---������Ա����
				--����Card_order
				select @card_order=(max(card_order)+1) from dt_ac_card where user_serial=@user_serial
				insert into dt_ac_card(
				user_serial,card_serial,card_hao,card_order,ac_money,ac_addm,ac_addo,ac_subm,
				ac_subo,ac_regm,ac_make,ac_eachm,ac_eacho,ac_state,sj,ks_sj,gly_no
				)VALUES(
				@user_serial,@card_serial,@card_hao,isnull(@card_order,1),0,0,0,0,
				0,0,0,0,0,0,getdate(),getdate(),@gly_no
				)
				
				---������Ա��״̬
				update dt_user set user_ac = 1,user_card=@card_hao,user_sj=getdate()
				where user_serial = @user_serial
			
				---��������������־
				insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
				values(1,1,0,@user_serial,@card_serial,getdate(),@ip,@gly_no) 
				
				--������Ƭ��־
				insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
				values(1,2,0,@user_serial,@card_serial,getdate(),@ip,@gly_no)
				
				--����ָ����־
				insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
				values(1,3,0,@user_serial,@card_serial,getdate(),@ip,@gly_no)
				
				---���·�����־
				Update WT_CARD_LOG set lx=0,log_sj=getdate(),log_ip=@ip,gly_no=@gly_no where lx=12 and user_serial=@user_serial and card_new=@card_serial
				
			end
			
		commit transaction

	end
	
---------////////ͬ��������������/////////------------------------
	else if(@lx=2)
	begin
		---�ж���Ա��Ϣ
	    --1.��Ա�Ƿ��п�
	    select @user_no = user_no from dt_user where user_serial = @user_serial
	    select top 1 @tmp_card = isnull(user_card,'') from dt_user where user_serial=@user_serial
		if @tmp_card <> ''
		begin
		
			select 2	--��Ա��������ʹ�õĿ�Ƭ
			print '��Ա��������ʹ�õĿ�Ƭ,����:'+@user_no
			return
			
		end
		
		--2.�¿��Ƿ�ռ��
		if (select count(card_hao) from dt_card where card_hao=@card_hao and card_type<>4)>0
		begin
		
			select 3	--�¿���ռ��
			print '�¿���ռ��,����:'+@card_hao
			return
			
		end
			
        --3.������Ա�˻�//////����ȥ������������������������
		exec ClientPro_user_kh @ac_type,@user_serial,@gly_no,@result output            
		if @result <> 0 
		begin
			print '����ʧ��,����:'+@card_hao	
			return				--����
		end;
		
		begin transaction
			--4.���ɿ���Ϣ
			--����Card_order
			
			select @card_order=(max(card_order)+1) from dt_card where user_serial=@user_serial
			
			--��ȡ��˳���

			select @module_card = module_card+1 from wt_module where module_id='0009'
			set @card_serial = right('00000000'+convert(varchar(8),@module_card),8)  --��ȡ8λ ����λǰ��0
			update wt_module set module_card=@module_card where module_id='0009'
		
			
			--.д��Ƭ������־��δ��״̬��
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
			
			--����ת�����Ÿ�ʽ
			exec ClientPro_Card_formate @xh,@card_lx
			
			--5.���ɸ����˻�
			--����Card_order
			select @card_order=(max(card_order)+1) from dt_ac_card where user_serial=@user_serial
			insert into dt_ac_card(
			user_serial,card_serial,card_hao,card_order,ac_money,ac_addm,ac_addo,ac_subm,
			ac_subo,ac_regm,ac_make,ac_eachm,ac_eacho,ac_state,sj,ks_sj,gly_no
			)VALUES(
			@user_serial,@card_serial,@card_hao,isnull(@card_order,1),0,0,0,0,
			0,0,0,0,0,0,getdate(),getdate(),@gly_no
			)		
			
			
			--5.���������˻�
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
			
			--6.������Ա��״̬
			update dt_user set user_ac = 1,user_card=@card_hao,user_sj=getdate()
			where user_serial = @user_serial
			
			
			--7.��������������־
			insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
			values(1,1,0,@user_serial,@card_serial,getdate(),@ip,@gly_no) 
			
			--������Ƭ��־
			insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
			values(1,2,0,@user_serial,@card_serial,getdate(),@ip,@gly_no)
			
			--����ָ����־
			insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
			values(1,3,0,@user_serial,@card_serial,getdate(),@ip,@gly_no)
			
			--�����Ž�Ա��������־
			insert into wt_mj_ver(type,lx,log_sj,log_ip,gly_no,gate_bh,log_lx,log_bz,fx)
			values(0,2,getdate(),null,null,null,1,@user_serial,null)
			
			---���·�����־
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
			---�жϿ�Ƭ��Ϣ
			---����dt_card
			---����dt_card_user
			---����dt_ac_card
			---����dt_user
			---���ɸ�����־
			---���ɲ�����־		

			--return
		commit transaction		
	end	
	----�ɹ�ͳһ����ֵ
	if @lx <> 2
	select 1
end try

begin catch	--�쳣����
	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	begin		 
		select 4	
		print '����ʧ��,����:'+@user_no+',ԭ��:'+ERROR_MESSAGE() 
		ROLLBACK TRANSACTION;
    end;
end catch

go

if object_id('ClientPro_Card_loss') is not null
drop proc ClientPro_Card_loss
go
create procedure ClientPro_Card_loss 	--��ʧ�������
(
@user_serial bigint,		--��Ա���
@card_hao varchar(20),		--����
@bz nvarchar(50),			--ժҪ
@lx int,					--�������ͣ�1:������� 2:ͬ����ʧ��
@ip varchar(20),			--ip
@gly_no nvarchar(50),		--����Ա
@reg_serial varchar(50)		--��ҵ���
)
as
set nocount on

---
declare @card_serial char(8)	--��˳���
declare @card_lx int			--������
declare @card_type int			--��״̬
declare @loss_count int			--��ʧ����
declare @user_no varchar(100)

begin try

	select @card_serial=card_serial,@card_type=card_type,@loss_count=isnull(loss_count,0)+1
	from dt_card 
	where user_serial=@user_serial and card_hao=@card_hao
	
	select @user_no = user_no from dt_user where user_serial = @user_serial
	
	--1.��Ա������
	if (select count(1) from dt_user where user_serial=@user_serial)=0
	begin

		select 2
		print '��Ա������!'	
		return
		
	end

	--2.����״̬������
	if(@card_type=1 or @card_type=2)	--��ʧ״̬�ж�
	begin

		select 3	--����״̬������ �޷���ʧ
		print '����״̬������,�޷���ʧ,����:'+@card_hao
		return

	end

	begin transaction

		--1.���¿���״̬
		update dt_ac_card 
		set ac_state=1 
		where user_serial=@user_serial 
		and card_serial=@card_serial
		
		--2.���¿��ű�״̬
		update dt_card 
		set card_type=1,sj=getdate(),card_bz=@bz,loss_count=@loss_count
		where user_serial=@user_serial 
		and card_serial=@card_serial
		
		--3.������Ա�˻���
		update dt_user 
		set user_card='' 
		where user_serial=@user_serial
		
		--4.ɾ������ת����
		delete a from dt_card_user a,dt_card b 
		where a.Parent_xh=b.xh 
		and b.user_serial=@user_serial 
		and b.card_serial=@card_serial
		
		--5.����������־
		insert into WT_USER_UP(lx,log_type,card_serial,user_serial,log_sj,log_ip,gly_no) 
		values(2,1,@card_Serial,@user_serial,getdate(),@ip,@gly_no)
		
		--6.���ɿ���ʧ��־
		insert into WT_CARD_LOG(
		lx,log_type,log_state,user_serial,card_old,card_new,
		log_xh,log_sj,log_ip,gly_no,regserial) 
		VALUES(
		1,0,0,@user_serial,@card_hao,@card_serial,
		@card_lx,getdate(),@ip,null,@reg_serial
		)

		--7.���سɹ�
		if @lx <> 2
		select 1
		
	commit transaction

end try

begin catch			--�쳣����

	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	BEGIN
		
		select 4	--�����쳣ʧ��
		print '��ʧʧ��,����:'+@user_no+',ԭ��:'+ERROR_MESSAGE() 
		ROLLBACK TRANSACTION
		 
	END

end catch


go

if object_id('ClientPro_Card_unloss') is not null
drop proc ClientPro_Card_unloss
go
create procedure ClientPro_Card_unloss 	--��Ҵ������
(
@user_serial bigint,	--��Ա���
@card_serial char(8),	--��˳���
@card_hao varchar(20),	--������
@card_lx int,			--������
@lx int,				--�������ͣ�1:������� 2:ͬ����ң�
@ip varchar(20),		--ip
@gly_no nvarchar(50),	--����Ա
@reg_serial varchar(20)	--��ҵ���
)
as

set nocount on

declare @xh int		--�������
declare @user_no varchar(100)

if(@lx=2)	--ͬ�����
begin

	select @card_serial=card_serial,@user_serial=user_serial 
	from dt_card where card_hao=@card_hao and card_type=1

end

begin try
	select @user_no = user_no from dt_user where user_serial = @user_serial

	if not exists(select 1 from dt_card where user_serial=@user_serial and card_serial=@card_serial
	and card_hao=@card_hao and card_type=1)
	begin
	
		select 2	--�����ڹ�ʧ��Ƭ,�޷����
		print '�����ڹ�ʧ��Ƭ,�޷����,����:'+@user_no
		return
		
	end
	
	begin transaction	--��ʼ����
	
		select @xh=max(xh) from dt_card where user_serial=@user_serial and card_serial=@card_serial
		
		--1.���¿���״̬
		update dt_ac_card
		set ac_state=0
		where user_serial=@user_serial
		and card_serial=@card_serial
		
		--2.���¿���״̬
		update dt_card
		set card_type=0,sj=getdate()
		where user_serial=@user_serial
		and card_serial=@card_serial
		
		--3.������Ա��������
		update dt_user
		set user_card=@card_hao 
		where user_serial=@user_serial
		
		--4.���ɿ���ת����ʽ������
		exec ClientPro_Card_formate @xh,@card_lx
		
		--5.��������������־
		insert into WT_USER_UP(lx,log_type,card_serial,user_serial,log_sj,log_ip,gly_no)
		values (2,1,@card_Serial,@user_serial,getdate(),@ip,@gly_no)
		
		--6.���ɿ�Ƭ�����־
		insert into WT_CARD_LOG(
		lx,log_type,log_state,user_serial,card_old,card_new,
		log_xh,log_sj,log_ip,gly_no,regserial
		)values(
		2,0,0,@user_serial,@card_hao,@card_serial,
		@card_lx,getdate(),@ip,@gly_no,@reg_serial
		)
		
		--7.���سɹ�
		if @lx <> 2
		select 1
		
	commit transaction
	
end try

begin catch		--�쳣����

	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	BEGIN
		
		select 4	--�����쳣ʧ��
		print '���ʧ��,����:'+@user_no+',ԭ��:'+ERROR_MESSAGE() 
		ROLLBACK TRANSACTION
		 
	END

end catch
go


if object_id('ClientPro_Card_back') is not null
drop proc ClientPro_Card_back
go
create procedure ClientPro_Card_back 	--�˿��������
(
@user_serial bigint,		 --��Ա���
@card_serial char(8),		 --��˳���
@card_hao varchar(20),	 	 --������
@card_verify varchar(20),	 --��Ƭ��֤��
@card_form varchar(20),		 --��ƬĿ¼
@card_lx int,			 	 --������
@card_type int,				 --��״̬
@lx int,					 --�������ͣ�0��Ԥ����1�����½��״̬�� 2:ͬ���˿���
@ip varchar(20),			 --ip
@gly_no nvarchar(50),		 --����Ա
@reg_serial varchar(20)		 --��ҵ���
)
as

set nocount on

---��������
declare @xh int
declare @readd_count int
declare @ac_money money
declare @user_no varchar(100)

set @readd_count=0
set @ac_money=0

begin try

	if(@lx=0)	--�˿�Ԥ����
	begin
		
		begin transaction
			
			--1.����dt_card_temp��¼
			insert into dt_card_temp(lx,user_serial,gly_no,card_lx,card_hao,card_serial,card_verify,card_type,card_order,sj) 
			select lx,user_serial,gly_no,card_lx,@card_hao,@card_serial,@card_verify,@card_type,card_order,GETDATE()
			from dt_card
			where user_serial = @user_serial
			and card_serial = @card_serial
			and card_hao = @card_hao
			
			--2.���¿��ű�Ƭ��֤���Ŀ¼��
			update dt_card set card_verify=@card_verify,card_form=@card_form
			where user_serial = @user_serial
			and card_serial = @card_serial
			and card_hao = @card_hao
			
			--3.����Ԥ�˿���־
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

	else if(@lx=1)	--��ɲ����½��
	begin

		begin transaction
		
		select @xh=max(xh) from dt_card_temp where user_serial=@user_serial and card_serial=@card_serial
		and ((card_hao=@card_hao and card_type=4) or card_type=5)
		
		select @readd_count=isnull(readd_count,0) from dt_card where user_serial=@user_serial 
		and card_serial=@card_serial
		and card_hao=@card_hao
		
		if(@card_type=5)	--��������
		begin
		
			update dt_card_temp set card_type=@card_type,card_hao=''
			where xh=@xh
			
			commit transaction
			select 1
			return

		end
		else
		begin
		
			--1.ɾ��ת��������
			delete a from DT_CARD_USER a,dt_card b where a.Parent_xh=b.xh and b.user_serial = @user_serial and b.card_serial = @card_serial
			
			--2.ɾ����������
			delete from dt_card where user_serial = @user_serial and card_serial = @card_serial
			
			--3.������Ա������
			update dt_user 
			set user_card='' 
			where user_serial = @user_serial 
			and user_card = @card_hao
			
			--4.������Ա�˻���Ϣ
			update dt_ac_user
			set ac_eacho=a.ac_eacho-b.ac_eacho
			from dt_ac_user a,dt_ac_card b
			where a.user_serial=b.user_serial
			and b.user_serial = @user_serial
			and b.card_serial = @card_serial
			
			--5.������Ա������Ϣ
			update dt_ac_card 
			set ac_state=2,ac_eacho=0 
			where user_serial = @user_serial 
			and card_serial = @card_serial
			
			--����������������
			--�������˿�&��ʧ���˿�(δ����)
			if(@readd_count=0)
			begin
				insert into WT_USER_UP(lx,log_type,card_serial,user_serial,log_sj,log_ip,gly_no) 
				values(2,1,@card_Serial,@user_serial,getdate(),@ip,@gly_no)
			end
			
			---��Ա�Ƿ���ְ����ְ��Ա����������
			--if exists(select 1 from dt_user where user_serial=@user_serial and user_type=51)
			--begin
			--	
			--	--������Ա�˻�Ϊ��ʼ״̬
			--	update dt_ac_user set ac_type='0000000000000001',ac_pass='123456',ac_jssj=getdate(),ac_state=1,sj=getdate(),Ac_addm=0,Ac_subm=0,Ac_regm=0,Ac_make=0,Ac_eachm=0 where user_serial = @user_serial
			--	--������Ա���˻�״̬Ϊ0
			--	update dt_user set user_ac=0 where user_serial=@user_serial
			--	
			--end
			
			--6.�����˿���־״̬
			update WT_CARD_LOG 
			set lx=5,card_old = @card_hao,log_xh = @card_lx,
				log_sj = getdate(),log_ip =@ip,gly_no=@gly_no
			where lx=13
			and user_serial = @user_serial
			and card_new = @card_serial

		
		end
		
		---���سɹ�
		
		
		commit transaction

	end
------------------/////////////////////ͬ���˿�����/////////////----------
	else if(@lx=2)	--ͬ���˿�����
	begin
		select @user_no = user_no from dt_user where user_serial = @user_serial
	
		select @card_serial = card_serial 
		from dt_card where card_hao= @card_hao

		select @ac_money=ac_money from dt_ac_link where user_serial=@user_serial
		if(@ac_money<>0)
		begin
		
			select 3	--�˻��������,���˿��
			print '�˻��������,���˿��,����:'+@user_no
			return
		
		end
		
		
		--0.����Ԥ�˿���־
		insert into wt_card_log(
		lx,log_type,log_state,user_serial,card_old,
		card_new,log_xh,log_sj,log_ip,gly_no,log_lx,log_group,
		log_row,log_erro,bz,regserial
		)values(
		13,0,0,@user_serial,@card_hao,
		@card_serial,@card_lx,getdate(),@ip,@gly_no,0,null,
		null,0,null,@reg_serial
		)
		
		--1.����dt_card_temp��¼
		insert into dt_card_temp(lx,user_serial,gly_no,card_lx,card_hao,card_serial,card_verify,card_type,card_order,sj) 
		select lx,user_serial,gly_no,card_lx,@card_hao,@card_serial,@card_verify,@card_type,card_order,GETDATE()
		from dt_card
		where user_serial = @user_serial
		and card_serial = @card_serial
		and card_hao = @card_hao
		
		--1.ɾ��ת��������
		delete a from DT_CARD_USER a,dt_card b where a.Parent_xh=b.xh and b.user_serial = @user_serial and b.card_serial = @card_serial
		
		--2.ɾ����������
		delete from dt_card where user_serial = @user_serial and card_serial = @card_serial
		
		--3.������Ա������
		update dt_user 
		set user_card='' 
		where user_serial = @user_serial 
		and user_card = @card_hao
		
		--4.������Ա�˻���Ϣ
		update dt_ac_user
		set ac_eacho=a.ac_eacho-b.ac_eacho
		from dt_ac_user a,dt_ac_card b
		where a.user_serial=b.user_serial
		and b.user_serial = @user_serial
		and b.card_serial = @card_serial
		
		--5.������Ա������Ϣ
		update dt_ac_card 
		set ac_state=2,ac_eacho=0 
		where user_serial = @user_serial 
		and card_serial = @card_serial
		
		--����������������
		--�������˿�&��ʧ���˿�(δ����)

		insert into WT_USER_UP(lx,log_type,card_serial,user_serial,log_sj,log_ip,gly_no) 
		values(2,1,@card_Serial,@user_serial,getdate(),@ip,@gly_no)

		
		---��Ա�Ƿ���ְ����ְ��Ա����������
		--if exists(select 1 from dt_user where user_serial=@user_serial and user_type=51)
		--begin
		--	
		--	--������Ա�˻�Ϊ��ʼ״̬
		--	update dt_ac_user set ac_type='0000000000000001',ac_pass='123456',ac_jssj=getdate(),ac_state=1,sj=getdate(),Ac_addm=0,Ac_subm=0,Ac_regm=0,Ac_make=0,Ac_eachm=0 where user_serial = @user_serial
		--	--������Ա���˻�״̬Ϊ0
		--	update dt_user set user_ac=0 where user_serial=@user_serial
		--	
		--end
		
		--6.�����˿���־״̬
		update WT_CARD_LOG 
		set lx=5,card_old = @card_hao,log_xh = @card_lx,
			log_sj = getdate(),log_ip =@ip,gly_no=@gly_no
		where lx=13
		and user_serial = @user_serial
		and card_new = @card_serial		
	
	end
	
end try

begin catch		--�쳣����

	IF ((XACT_STATE()) = -1 or (XACT_STATE()) = 1)
	BEGIN
		
		select 4	--�����쳣ʧ��
		print '�˿�ʧ��,����:'+@user_no+',ԭ��:'+ERROR_MESSAGE() 
		ROLLBACK TRANSACTION
		 
	END

end catch
if @lx <> 2 select 1
go


if object_id('ClientPro_Card_replace') is not null
drop proc ClientPro_Card_replace
go
create procedure ClientPro_Card_replace 	--�������������
(
@user_serial bigint,				--��Ա���
--@ac_type varchar(16),				--�˻�����
@lx int,							--Ԥ�������³ɹ�(lx=2 ͬ������)
@card_type int,						--����״̬
@old_card_serial char(8),			--�ɿ�˳���
@old_card_hao varchar(20),			--�ɿ�������
@old_card_lx int,					--�ɿ�������
--@cash_amt money,					--�ֽ����
--@sub_amt money,						--�������
--@each_amt int,						--�����
@new_card_serial char(8),			--�¿�˳���
@new_card_hao varchar(20),			--��������
@new_card_lx int,					--�¿�����(ͬ��Ϊ100)
@card_work int,						--��Ƭ��д��־(ͬ��Ϊ0)
@ip varchar(20),					--ip
@reg_serial varchar(20),			--��ҵ���
@gly_no nvarchar(50)				--����Ա���
)
as
set nocount on
set ansi_warnings off
---��������
declare @xh int
declare @casher_bh  varchar(16)	--�տ�Ա���
declare @ac_no char(16)			--�˻����
declare @rec_count int			--������
declare @card_order int			--����˳��
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
declare @cash_amt money					--�ֽ����
declare @sub_amt money					--�������
declare @each_amt int					--�����
declare @sub_kssj datetime		--������ʼʱ��
declare @sub_jssj datetime		--������������
declare @each_kssj datetime 	--�ݿ�ʼʱ��
declare @each_jssj datetime		--����������
declare @isvalid_date int		--�Ƿ���������

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
	
		--1.��Ա�Ƿ��п�
		if (select isnull(user_card,'') from dt_user where user_serial=@user_serial)<>''
		begin
		
			select 2	
			return
			
		end
		
		--2.�¿��Ƿ�ռ��
		if (select count(card_hao) from dt_card where card_hao=@new_card_hao and card_type<>4)>0
		begin
		
			select 3
			return
			
		end
		begin transaction
			
			---1.1��dt_card������
			--����Card_order

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
			
			--1.2дԤ������־
			insert into wt_card_log(
			lx,log_type,log_state,user_serial,card_old,
			card_new,log_xh,log_sj,log_ip,gly_no,log_lx,log_group,
			log_row,log_erro,bz,regserial
			)values(
			12,0,0,@user_serial,@new_card_hao,
			@new_card_serial,@new_card_lx,getdate(),@ip,@gly_no,0,null,
			null,0,null,@reg_serial
			)
			
			--1.3����Ԥ�����¼����ֵ���������ݣ�
			if(@cash_amt>0)	--������ֵ
			begin
			
				set @mould=0		--���ģʽ
				set @lx=2			--��ֵ
				set @deal_lx=13		--������ֵ
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
				set @Wcard_state=0	--δд��
				set @jl_sj=getdate()

				
						--1.��ȡ�տ�Ա����id
				if not exists(select 1 from real_xf_casher where gly_no=@gly_no and Settle_acc_state=0 
				and isnull(reg_serial,'')=@reg_serial)
				begin
				
					--���ɱ���ID  �����տ������
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
			
			if(@sub_amt>0)		--��������
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
				@gly_no,@ip,null,null,'������������')
				
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
			
			if(@each_amt>0)	--������
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
				@gly_no,@ip,null,null,'�����ݵ���')
				
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
	
	else if(@lx=1)	--�ɹ������
	begin
	
		begin transaction
		
		select @xh=max(xh) from dt_card where user_serial=@user_serial and card_serial=@new_card_serial
		and ((card_hao=@new_card_hao and card_type=4) or card_type=5)
		
		select @cash_amt=isnull(ac_addo,0),@sub_amt=isnull(ac_subo,0),@each_amt=isnull(ac_eacho,0)
		from dt_ac_card
		where card_serial=@old_card_serial and card_hao=@old_card_hao
		
		if(@card_type=5)	--����
		begin
		
			--
			update dt_card set card_type=@card_type,card_hao=''
			where xh=@xh
			
			--����Ԥ�����¼
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
		--	if(isnull(@ac_type,'')<>'')	--����
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
			
			---���¿���״̬
			update dt_card set card_hao=@new_card_hao,card_type=@card_type,card_work=@card_work,
					card_lx=@new_card_lx,sj=getdate()
			where xh=@xh
			
			--���¾ɿ�����״̬
			update dt_card set readd_count=1 where user_serial=@user_serial 
			and card_serial=@old_card_serial
			and card_hao=@old_card_hao
			
			---����ת����ʽ����
			exec ClientPro_Card_formate @xh,@new_card_lx
			
			---������Ա����
			--����Card_order
			select @card_order=(max(card_order)+1) from dt_ac_card where user_serial=@user_serial
			insert into dt_ac_card(
			user_serial,card_serial,card_hao,card_order,ac_money,ac_addm,ac_addo,ac_subm,
			ac_subo,ac_regm,ac_make,ac_eachm,ac_eacho,ac_state,sj,ks_sj,gly_no
			)VALUES(
			@user_serial,@new_card_serial,@new_card_hao,isnull(@card_order,1),0,0,0,0,
			0,0,0,0,0,0,getdate(),getdate(),@gly_no
			)
			
			---������Ա��״̬
			update dt_user set user_ac = 1,user_card=@new_card_hao,user_sj=getdate()
			where user_serial = @user_serial
		
			---����������־
			insert into wt_user_up(lx,log_type,log_finger,user_serial,card_serial,log_sj,log_ip,gly_no)
			values(1,1,0,@user_serial,@new_card_serial,getdate(),@ip,@gly_no)
			
		
			
			---���·�����־
			Update WT_CARD_LOG set lx=3,log_sj=getdate(),log_ip=@ip,gly_no=@gly_no where lx=12 and user_serial=@user_serial and card_new=@new_card_serial
			
			---���˴���
			
			--��ϸ�˴���������ֵ�������������ˡ������ݼ��ˣ�
			if(@cash_amt>0)
			begin
				--select @mx_xh=max(xh)+1 from real_xf_mx 
				
				update real_xf_cash set card_hao=@new_card_hao,wcard_state=1
				 where user_serial=@user_serial
				 and card_serial=@new_card_serial and iscardreplace=1 
				
				---ȡ����ǰ���˻�\���˻����
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
				
				
				insert into real_xf_mx(	--������ֵ��ϸ
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
			
			if(@sub_amt>0)	--����������������
			begin
			
				--select @mx_xh=max(xh)+1 from real_xf_mx 
				
				update real_sub_slave set card_hao=@new_card_hao where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1
				---��ȡ���ݱ��
				select @bill_no=parent_bh from real_sub_slave where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1
				
				select @state=state from Real_sub_master where Bh=@bill_no
				
				if(@state=0)		--����δ����
				begin
				
					exec Real_BillSubsidy_account  @reg_serial,@bill_no,@ip,@gly_no
					
--					---ȡ����ǰ���˻�\���˻����
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
--					----���ɲ���������ϸ���˼�¼
--					insert into real_xf_mx(		--������ϸ
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
				
				---��ȡ���ݱ��
				select @bill_no=parent_bh from real_each_slave where user_serial=@user_serial
				and card_serial=@new_card_serial and iscardreplace=1
				
				select @state=state from Real_each_master where Bh=@bill_no
				
				if(@state=0)
				begin
				
					exec Real_BillEach_account @reg_serial,@bill_no,@ip,@gly_no
--					---ȡ����ǰ���˻�\���˻����
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
--					----���ɲ���������ϸ���˼�¼
--					insert into real_xf_mx(		--������ϸ
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
			
			--�������˻���ԭ���˻�������㣩��Ա���˻�����
			
			update dt_ac_card 
			set ac_money=ac_money+@cash_amt,ac_addo=ac_addo+@cash_amt,
				ac_addm=ac_addm+@cash_amt
			where user_serial=@user_serial
			and card_serial=@new_card_serial
			and card_hao=@new_card_hao
			
			update dt_ac_card 
			set ac_money=ac_money-(@cash_amt+@sub_amt),ac_addo=ac_addo-@cash_amt,
				ac_subo=ac_subo-@sub_amt,ac_eacho=ac_eacho-@each_amt	--��ԭ��ʧ�����
			where user_serial=@user_serial
			and card_serial=@old_card_serial
			and card_hao=@old_card_hao
			and ac_state=1
			
		end
		commit transaction
	end
	
--------------------////////ͬ����������////////////-------------
	else if(@lx=2)   --v1.03
	begin
		declare @dev_serial varchar(7)
		declare @out_put int
		--declare @card_lx int

		set @dev_serial=null

	--��ʧ�ɿ�Ƭ�����¿�Ƭ
		select @old_card_serial=card_serial,@card_type=card_type
		from dt_card 
		where user_serial=@user_serial and card_hao=@old_card_hao
		
		create table #temp_return(
		xh int identity(1,1),
		out_put int
		)
		
		insert into #temp_return exec ClientPro_Card_loss @user_serial,@old_card_hao,null,2,'syn','syn',null    --��ʧ
		
		select @out_put=out_put from #temp_return
		
		if(@out_put =1 or @out_put=3)	--��ʧ�ɹ�
		begin
			
			truncate table #temp_return
			set @out_put=-1
			insert into #temp_return exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card_hao,null,null,0,0,0,'syn','syn',null  --���¿�
			
			select @out_put=out_put from #temp_return
		end
		
		--if(@out_put =1)		--�����ɹ�
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
		select 4	--�����쳣ʧ��
		ROLLBACK TRANSACTION
		 
	END

end catch


go

