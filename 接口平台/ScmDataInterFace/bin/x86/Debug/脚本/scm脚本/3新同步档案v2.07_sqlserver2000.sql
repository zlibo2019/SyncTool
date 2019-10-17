-----------ע��-----------
--v1.01  scm�м���user_lx����
--v1.02 ȡ��ְ��Աʱ�ӹ�������������2013-12-7��־2013-12-7
--updateUser_small_scm��updateUser_big_scm��������ͬ����������ͬ��bigΪzh,smallΪzh+khsj
--v1.03�����γ�������user_finger������
--v1.04��һ��ͨ,����һ��ͨ������ʱ�����ڵ���Ա���ݲ���ְ���������ӿ������ص�������ȫ�����  ��־2014-3-18
--v1.05 ����v1.04�������޸�
--v1.06 �����Աʱ��select ��distinct��������ܴ��ڵ��ظ�����
--v1.07 ����V1.05�������޸�
--v1.08 ������ɾ��#tmp_delete�������
--v1.09 user_finger����ʱ��Ĭ��ֵ'0000000000'
--v1.10 @zh��������Ϊ200
--v2.00 ����ʵʱͬ��
--v2.01 ���Сһ��ͨ���������˻�����
--v2.02��ѯ�����޸�
--v2.03 �޸Ŀ��ȶ����
--v2.04 �޸Ŀ��Ա����
--v2.05 ������Ա��������
--v2.06 ������Ա������
--v2.07 �����¿��մ���
---------------------------------------------------ɾ������ʱ��
IF  EXISTS (SELECT * FROM sysobjects  where id = object_id('tmp_dt_dep'))                        
DROP TABLE tmp_dt_dep
GO

IF  EXISTS (SELECT * FROM sysobjects  where id = object_id('tmp_dt_user'))                        
DROP TABLE tmp_dt_user
GO

---------------------------------------------------ɾ��dt_user����user_no
Declare @Pk varChar(100); 
Select @Pk=Name from sysobjects where Parent_Obj=OBJECT_ID('dt_user') and xtype='PK';
if @Pk is not null
begin
	exec('Alter table dt_user Drop constraint '+ @Pk)  --ɾ��ԭ����
end

---------------------------------------------------��ʽ������
if not exists(select * from syscolumns where id = object_id( 'dt_dep') and name = 'id')
alter table dt_dep add id int identity
go

if not exists(select * from syscolumns where id = object_id( 'dt_dep') and name = 'dep_no_add')
alter table dt_dep add dep_no_add varchar(50)
go

if not exists(select * from syscolumns where id = object_id( 'dt_dep') and name = 'dep_no_parent')
alter table dt_dep add dep_no_parent varchar(50)
go

if not exists(select * from syscolumns where id = object_id( 'dt_user') and name = 'dep_no_add')
alter table dt_user add dep_no_add varchar(50)
go

if not exists(select * from syscolumns where id = object_id( 'dt_user') and name = 'zh')
alter table dt_user add zh varchar(200)
go

if not exists(select * from syscolumns where id = object_id( 'dt_user') and name = 'khsj')
alter table dt_user add khsj varchar(30)
go

if not exists(select * from syscolumns where id = object_id( 'dt_user') and name = 'user_lx')
alter table dt_user add user_lx int
go


---------------------------------------------------------------------��������
if exists (select * from dbo.sysobjects where name = 'get_ChildDep')
drop function [dbo].[get_ChildDep]
GO


CREATE function get_ChildDep(@ID int) returns @t_level table(id int,dep_serial int, level int)
as
begin
  declare @level int
  set @level = 1
  insert into @t_level select @id,@id , @level
  while @@ROWCOUNT > 0
  begin
    set @level = @level + 1
    insert into @t_level select @id,a.dep_serial, @level
    from dt_dep a , @t_Level b
    where a.dep_parent = b.dep_serial and b.level = @level - 1
  end
  return
end

GO

if exists (select * from dbo.sysobjects where name = 'Sys_Convert')
drop function [dbo].[Sys_Convert]
GO
create function Sys_Convert (@i int)
returns varchar(30)
as
begin

	declare @r varchar(30)
	set @r=''

	declare @m int
	declare @s int
	set @s=@i
	while @s>=36
	begin
		set @m=@s % 36
		set @r=case when @m<10 then cast(@m as varchar) 
		else cast(char(ascii('A')+@m-10) as varchar) end+@r
		set @s=@s/36
	end
	if @s>0 or (@s=0 and @r='')
		set @r=case when @s<10 then cast(@s as varchar) 
	else 
		cast(char(ascii('A')+@s-10) as varchar) end+@r

	if len(@r)<2
	begin
		set @r = RIGHT('0000'+CONVERT(varchar(10),@r),2)
	end 
	return @r
end

go


------------------------------------------------------------------�����洢����


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateDep]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateDep]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateDep_big]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateDep_big]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateDep_small]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateDep_small]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateUser_big_scm]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateUser_big_scm]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateUser_big_bs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateUser_big_bs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateUser_small_scm]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateUser_small_scm]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateUser_small_bs]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateUser_small_bs]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updatePhoto]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updatePhoto]
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE procedure updateDep
@bz integer = 1 --1Ϊscm 0Ϊbs
as
 set nocount on
 

	declare @dep_no_add varchar(100)
	declare @dep_no_parent varchar(100)
	declare @dep_name varchar(200)
	declare @dep_serial int

 ------------------------- ͬ��ɾ���Ĳ���
--        delete from dt_dep where dep_no_add not in(select a.dep_no_add  from tmp_dt_dep a inner join dt_dep b on a.dep_no_add = b.dep_no_add) and dep_parent <> 0

-------------------------���Ÿ���
	update dt_dep set dep_name = b.dep_name,dep_no_parent = b.dep_no_parent from dt_dep a inner join tmp_dt_dep b on a.dep_no_add = b.dep_no_add



--------------------------����ͷ�ڵ�
	delete from dt_dep where dep_parent = 0   --��ɾ��Ĭ��ͷ���        
    set @dep_name = ''
	select @dep_name = dep_name from tmp_dt_dep where dep_no_add = '000';
	if @dep_name = '' select @dep_name = reg_unit from wt_reg
	insert dt_dep(dep_serial,dep_parent,dep_order,dep_name,dep_no,dep_rule,dep_no_add) values(10000,0,0,@dep_name,'001',0,'000')

 ------------------------�������
	declare cur_tt cursor for 
	select dep_no_add,dep_no_parent,dep_name from tmp_dt_dep where dep_no_add not in(select a.dep_no_add from dt_dep a inner join tmp_dt_dep b on a.dep_no_add = b.dep_no_add) order by dep_no_add
	open cur_tt
	FETCH NEXT FROM cur_tt into @dep_no_add,@dep_no_parent,@dep_name
	while @@FETCH_STATUS = 0 
	begin
	    if @bz = 1
			select @dep_serial = module_dep+1 from wt_module where module_id = '0002'      --modify by zlibo 2013-05-18
		else
			select @dep_serial = reg_dep+1 from wt_reg 
		
		insert into dt_dep(dep_serial,dep_name,dep_no_add,dep_no_parent,dep_rule) values(@dep_serial,@dep_name,@dep_no_add,@dep_no_parent,0)
		
		if @bz = 1
			update wt_module set module_dep = module_dep + 1 where module_id = '0002'
		else
			update wt_reg set reg_dep = reg_dep + 1 
		FETCH NEXT FROM cur_tt into @dep_no_add,@dep_no_parent,@dep_name 
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  

--------------------------��dep_parent
	update dt_dep set dep_parent = isnull(b.dep_serial,10000) from dt_dep left join dt_dep b on dt_dep.dep_no_parent = b.dep_no_add where dt_dep.dep_serial <> 10000 

---------------------------��dep_order��dep_no
 

        declare @level int   
        declare @count int

    	update dt_dep set dt_dep.dep_order=b.rn-1 from dt_dep a,
	(
		select t.*, rn=(select count(1) from dt_dep where 
		id<=t.id and dep_parent=t.dep_parent)
		from dt_dep t
	
	)b
	where a.dep_serial=b.dep_serial
	
  	select @level = max(level) from get_ChildDep(0)
    while @level > 0 
    begin        
		update dt_dep set dt_dep.dep_no=cast(b.dep_no as varchar)+right(cast(power(10,2) as varchar)+Convert(varchar,dbo.Sys_Convert(a.dep_order)),2)
		from dt_dep a,
		(
		select b.* from dt_dep a,dt_dep b where a.dep_parent=b.dep_serial
		)b
		where a.dep_parent=b.dep_serial

		set @level= @level -1 
    end
 

return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE procedure updateDep_big
@bz integer = 1 --1Ϊscm 0Ϊbs
as
 set nocount on
 

	declare @dep_no_add varchar(100)
	declare @dep_no_parent varchar(100)
	declare @dep_name varchar(200)
	declare @dep_serial int

     ----------------------------����dep_no_add������dep_no_parent
     update tmp_dt_dep set dep_no_parent = (case when LEN(dep_no_add) > 3 then LEFT(dep_no_add,LEN(dep_no_add)-3) else '0' end)    


 ------------------------- ͬ��ɾ���Ĳ���
--        delete from dt_dep where dep_no_add not in(select a.dep_no_add  from tmp_dt_dep a inner join dt_dep b on a.dep_no_add = b.dep_no_add) and dep_parent <> 0

-------------------------���Ÿ���
	update dt_dep set dep_name = b.dep_name,dep_no_parent = b.dep_no_parent from dt_dep a inner join tmp_dt_dep b on a.dep_no_add = b.dep_no_add



--------------------------����ͷ�ڵ�
	delete from dt_dep where dep_parent = 0   --��ɾ��Ĭ��ͷ���        
    set @dep_name = ''
	select @dep_name = dep_name from tmp_dt_dep where dep_no_add = '000';
	if @dep_name = '' select @dep_name = reg_unit from wt_reg
	insert dt_dep(dep_serial,dep_parent,dep_order,dep_name,dep_no,dep_rule,dep_no_add) values(10000,0,0,@dep_name,'001',0,'000')

 ------------------------�������
	declare cur_tt cursor for 
	select dep_no_add,dep_no_parent,dep_name from tmp_dt_dep where dep_no_add not in(select a.dep_no_add from dt_dep a inner join tmp_dt_dep b on a.dep_no_add = b.dep_no_add) order by dep_no_add
	open cur_tt
	FETCH NEXT FROM cur_tt into @dep_no_add,@dep_no_parent,@dep_name
	while @@FETCH_STATUS = 0 
	begin
	    if @bz = 1
			select @dep_serial = module_dep+1 from wt_module where module_id = '0002'      --modify by zlibo 2013-05-18
		else
			select @dep_serial = reg_dep+1 from wt_reg 
		
		insert into dt_dep(dep_serial,dep_name,dep_no_add,dep_no_parent,dep_rule) values(@dep_serial,@dep_name,@dep_no_add,@dep_no_parent,0)
		
		if @bz = 1
			update wt_module set module_dep = module_dep + 1 where module_id = '0002'
		else
			update wt_reg set reg_dep = reg_dep + 1 
		FETCH NEXT FROM cur_tt into @dep_no_add,@dep_no_parent,@dep_name 
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  

--------------------------��dep_parent
	update dt_dep set dep_parent = isnull(b.dep_serial,10000) from dt_dep left join dt_dep b on dt_dep.dep_no_parent = b.dep_no_add where dt_dep.dep_serial <> 10000 

---------------------------��dep_order��dep_no
 

        declare @level int   
        declare @count int

    	update dt_dep set dt_dep.dep_order=b.rn-1 from dt_dep a,
	(
		select t.*, rn=(select count(1) from dt_dep where 
		id<=t.id and dep_parent=t.dep_parent)
		from dt_dep t
	
	)b
	where a.dep_serial=b.dep_serial
	

  	select @level = max(level) from get_ChildDep(0)
    while @level > 0 
    begin        
		update dt_dep set dt_dep.dep_no=cast(b.dep_no as varchar)+right(cast(power(10,2) as varchar)+Convert(varchar,dbo.Sys_Convert(a.dep_order)),2)
		from dt_dep a,
		(
		select b.* from dt_dep a,dt_dep b where a.dep_parent=b.dep_serial
		)b
		where a.dep_parent=b.dep_serial

		set @level= @level -1 
    end
   

return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO




SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE procedure updateDep_small
@bz integer = 1  --1Ϊscm 0Ϊbs
as
 set nocount on
 
 --���Ÿ���
	update dt_dep set dep_name = b.dep_name from dt_dep a inner join tmp_dt_dep b on a.dep_no_add = b.dep_no_add
 --�������
	declare @dep_no_add varchar(200)
	declare @dep_parentNo varchar(200)
	declare @dep_name varchar(200)
	declare @dep_serial int
	declare @dep_parent int
	declare @dep_order int

--------------------------����ͷ�ڵ�
	delete from dt_dep where dep_parent = 0   --��ɾ��Ĭ��ͷ���        
    set @dep_name = ''
	select @dep_name = dep_name from tmp_dt_dep where dep_no_add = '001'
	if @dep_name = '' select @dep_name = reg_unit from wt_reg
	insert dt_dep(dep_serial,dep_parent,dep_order,dep_name,dep_no,dep_rule,dep_no_add) values(10000,0,0,@dep_name,'001',0,'001')


--------------------------------------------- ��Ӳ���
	declare cur_tt cursor for 
    select dep_no_add,dep_name from tmp_dt_dep 
    where dep_no_add not in(select a.dep_no_add from dt_dep a inner join tmp_dt_dep b on a.dep_no_add = b.dep_no_add) and dep_no_add <> '001'
    order by dep_no_add
	open cur_tt
	FETCH NEXT FROM cur_tt into @dep_no_add,@dep_name
	while @@FETCH_STATUS = 0 
	begin
		set @dep_parentNo = substring(@dep_No_add,1,len(@dep_No_add)-2)    
		select @dep_parent= isnull(dep_serial,10000) from dt_dep where dep_no_add = @dep_parentNo   
		
		if @bz = 1
			select @dep_serial = module_dep+1 from wt_module where module_id = '0002'      --modify by zlibo 2013-05-18
		else
			select @dep_serial = reg_dep+1 from wt_reg 
	
		set @dep_order = 0
		select @dep_order = isnull(max(dep_order)+1,0) from dt_dep where dep_parent = @dep_parent
		insert into dt_dep(dep_serial,dep_parent,dep_order,dep_name,dep_no_add,dep_rule) values(@dep_serial,@dep_parent,@dep_order,@dep_name,@dep_no_add,0)
		
		if @bz = 1
			update wt_module set module_dep = module_dep + 1 where module_id = '0002'
		else
			update wt_reg set reg_dep = reg_dep + 1 

		FETCH NEXT FROM cur_tt into @dep_no_add,@dep_name 
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  


--����dep_no
        declare @level int   

    	update dt_dep set dt_dep.dep_order=b.rn-1 from dt_dep a,
	(
		select t.*, rn=(select count(1) from dt_dep where 
		id<=t.id and dep_parent=t.dep_parent)
		from dt_dep t
	
	)b
	where a.dep_serial=b.dep_serial
	

  	select @level = max(level) from get_ChildDep(0)
    while @level > 0 
    begin        
		update dt_dep set dt_dep.dep_no=cast(b.dep_no as varchar)+right(cast(power(10,2) as varchar)+Convert(varchar,dbo.Sys_Convert(a.dep_order)),2)
		from dt_dep a,
		(
		select b.* from dt_dep a,dt_dep b where a.dep_parent=b.dep_serial
		)b
		where a.dep_parent=b.dep_serial

		set @level= @level -1 
    end
    
---���������ַ�(")   
    update dt_dep set dep_name=replace(cast(dep_name as varchar(8000)),'"','��')
    update dt_dep set dep_name=replace(cast(dep_name as varchar(8000)),'��','��')

return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE procedure updateUser_big_bs
as
 set nocount on

 --��������ԱΪ��ְ
    --update dt_user set user_type = 51 
    --where zh not in(select a.zh from tmp_dt_user a inner join dt_user b on a.zh = b.zh) and isnull(user_type,0) <> 51  --2013-12-7 zlb   
 --��Ա����
	update dt_user set user_lname = b.user_lname,user_no = b.user_no,user_fname = b.user_fname,user_type = b.user_type,dep_no_add =b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
                           user_card = case when isnull(b.card_state,0) <> 0 then '' else b.user_card end,user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = b.user_xueli,user_birthday = b.user_birthday,
                           user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address,user_level = 0,user_sj = b.user_sj 
     from dt_user a inner join tmp_dt_user b on a.zh = b.zh
 --��Ա���
        declare @user_serial int
        declare @zh varchar(200)
	declare cur_tt cursor for 
	select distinct zh from tmp_dt_user 
        where zh not in(select a.zh from dt_user a inner join tmp_dt_user b on a.zh = b.zh) 
	open cur_tt
	FETCH NEXT FROM cur_tt into @zh
	while @@FETCH_STATUS = 0 
	begin
		select @user_serial = reg_user+1 from wt_reg


		insert into dt_user(zh,user_serial,user_1,user_no,user_type,user_lname,user_fname,dep_no_add,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,user_level,user_finger) 
		select top 1 zh,@user_serial,user_1,user_no,user_type,user_lname,user_fname,dep_no,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,0,'0000000000' 
		from tmp_dt_user where zh = @zh

		update wt_reg set reg_user = reg_user+1

	    FETCH NEXT FROM cur_tt into @zh
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
  
  --����dt_user�е�user_dep
       update dt_user set user_dep = b.dep_serial,user_DepName = b.Dep_Name, dep_no = b.dep_no from dt_user a inner join dt_dep b on a.dep_no_add = b.dep_no_add


return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE procedure updateUser_big_scm
@card_type int = 100,
@xf_style int = 2,
@old_card_deal int = 0  --0�˾ɿ����¿�1�ҵ������¿����ɿ�Ǯת�¿�
as 
set nocount on

	declare @user_flag int
    declare @user_serial int
    declare @card_hao varchar(50)
    declare @new_card varchar(50)
    declare @old_card varchar(50)
	declare @user_no varchar(50)
	declare @old_user_type int
	declare @new_user_type int
	declare @zh varchar(200)
	declare @card_state int  --0����1��ʧ2�˿�3���
	declare @count int
	declare @result int

    set @card_type = 100 

 --��������ԱΪ��ְ
	--select user_serial,user_card 
	--into #tmp_delete 
	--from dt_user 
 --   where zh not in(select a.zh from tmp_dt_user a inner join dt_user b on a.zh = b.zh) and isnull(user_type,0) <> 51  --2013-12-7 zlb
 --   declare cur_tt cursor for
	--select * from #tmp_delete 
	--open cur_tt
	--FETCH NEXT FROM cur_tt into @user_serial,@card_hao
	--while @@FETCH_STATUS = 0 
	--begin   
	--	exec user_destroy @user_serial,@card_hao  
	--  	FETCH NEXT FROM cur_tt into @user_serial,@card_hao
	--end
	--CLOSE cur_tt
	--DEALLOCATE cur_tt   
 --��Ա����
    
    --�ȴ���user_type�仯��
	select b.user_serial,isnull(b.user_card,'') old_card,isnull(a.user_card,'') new_card,isnull(a.user_type,0) new_user_type,isnull(b.user_type,0) old_user_type
    into #tmp_update_ChangeUserType
	from tmp_dt_user a inner join dt_user b on a.zh = b.zh
	                                        and ((a.user_type = 51 and ISNULL(b.user_type,0) <> 51) or (isnull(a.user_type,0) <> 51 and b.user_type = 51)) 
	declare cur_tt cursor for 
	select * from #tmp_update_ChangeUserType 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_serial,@old_card,@new_card,@new_user_type,@old_user_type
	while @@FETCH_STATUS = 0 
	begin
	        if @new_user_type = 51    --��ְ
				exec user_destroy @user_serial,@old_card
		    if @old_user_type = 51  --��ְ
			begin
				exec user_resume @user_serial                   --��ְ	
			end
		  	FETCH NEXT FROM cur_tt into @user_serial,@old_card,@new_card,@new_user_type,@old_user_type
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt    
	 
    ---�����仯��
    select b.user_serial,isnull(a.user_card,'') new_card,isnull(b.user_card,'') old_card,isnull(a.card_state,0) card_state
    into #tmp_update_CardChange
	from tmp_dt_user a 
	inner join dt_user b on a.zh = b.zh 
	                     and (
								(isnull(a.user_card,'') <> isnull(b.user_card,'') and isnull(a.user_card,'') <> '' and isnull(b.user_card,'') <> '')  --���� v2.04
							  or (isnull(a.user_card,'') = isnull(b.user_card,'') and isnull(a.user_card,'') <> '' and isnull(a.card_state,0) > 0)     --��ʧ���˿� v2.04
							  or (isnull(a.user_card,'') <> '' and isnull(b.user_card,'') = '' and (isnull(a.card_state,0) = 0 or isnull(a.card_state,0) = 2)) --�������˿� v2.04
	                         )
	where ISNULL(a.user_type,0) <> 51

    declare cur_tt cursor for 
	select * from #tmp_update_CardChange 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_serial,@new_card,@old_card,@card_state
	while @@FETCH_STATUS = 0 
	begin
	    if (@new_card <> @old_card) and (@new_card <> '') and (@old_card <> '')   --������
        begin
            if @old_card_deal = 0
            begin
				exec ClientPro_Card_back @user_serial,null,@old_card,null,null,@card_type,null,2,'syn','syn',null   --�˾ɿ�
 				exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card,null,null,@card_type,0,0,'syn','syn',null  --���¿�
            end;
            else
            begin
				exec ClientPro_Card_replace @user_serial,2,null,null,@old_card,100,null,@new_card,100,0,'syn',null,'syn'  --�Ҿɿ����¿����ɿ����ת�¿�
            end
		end
		else if (@new_card = @old_card) and (@new_card <> '') and (@card_state = 1)     --��ʧ��
        begin
			exec ClientPro_Card_loss @user_serial,@old_card,null,2,'syn','syn',null    --��ʧ
		end
		else if (@new_card <> '') and (@card_state = 2)    --�˿���
        begin   
            select @count = count(1) from dt_card where card_hao = @new_card
            if @count > 0
				exec ClientPro_Card_back @user_serial,null,@new_card,null,null,@card_type,null,2,'syn','syn',null  --�˿�       
		end
		else if (@new_card <> '') and (@old_card = '') and (@card_state = 0)     --���¿��򿨽��
        begin	
			select @count = count(1) from dt_card where card_hao = @new_card and card_type = 1
            if @count > 0 
				exec ClientPro_Card_unloss @user_serial,null,@new_card,@card_type,2,'syn','syn',null  -- ִ�н��
			else
				exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card,null,null,@card_type,0,0,'syn','syn',null  --���¿�
		end

	  	FETCH NEXT FROM cur_tt into @user_serial,@new_card,@old_card,@card_state
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt 
	
	
	
	
	
	---���������ֶα仯 

	update dt_user set user_lname = b.user_lname,user_no = b.user_no,user_fname = b.user_fname,user_lx = b.user_lx,user_type = b.user_type,dep_no_add =b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
	           user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = b.user_xueli,user_birthday = b.user_birthday,
	           user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address,user_1 = b.user_1,user_2 = b.user_2,user_level = 0,user_sj = GETDATE() 
	from dt_user a inner join tmp_dt_user b 
	     on a.zh = b.zh
		 and (
		 isnull(a.user_no,'') <> isnull(b.user_no,'') or
		 isnull(a.user_lname,'') <> isnull(b.user_lname,'') or
		 isnull(a.user_fname,'') <> isnull(b.user_fname,'') or
		 isnull(a.user_lx,0) <> isnull(b.user_lx,0) or
		 --isnull(a.user_type,0) <> isnull(b.user_type,0) or
		 isnull(a.dep_no_add,'') <> isnull(b.dep_no,'') or
		 isnull(a.user_sex,'') <> isnull(b.user_sex,'') or
		 isnull(a.user_workday,'') <> isnull(b.user_workday,'') or
		 isnull(a.user_duty,'') <> isnull(b.user_duty,'') or
		 isnull(a.user_xueli,'') <> isnull(b.user_xueli,'') or
		 isnull(a.user_id,'') <> isnull(b.user_id,'') or
		 isnull(a.user_telephone,'') <> isnull(b.user_telephone,'') or
		 isnull(a.user_address,'') <> isnull(b.user_address,'') or
		 isnull(a.user_1,'') <> isnull(b.user_1,'') or
		 isnull(a.user_2,'') <> isnull(b.user_2,'') 
		 )


 --��Ա���

	select distinct zh,isnull(user_card,'') card_hao
	into #tmp_insert
	from tmp_dt_user 
    where zh not in(select a.zh from tmp_dt_user a inner join dt_user b on a.zh = b.zh)

	declare cur_tt cursor for 
	select * from #tmp_insert 
	open cur_tt
	FETCH NEXT FROM cur_tt into @zh,@card_hao
	while @@FETCH_STATUS = 0 
	begin
        --add��Ա��
		select @user_serial = module_user+1 from wt_module where module_id = '0002'
		--select @user_serial = isnull(max(user_serial),0)+1 from dt_user
		
		--������Ա�˻�
		exec ClientPro_user_kh '0000000000000001',@user_serial,'syn',@result output
		if @result <> 0 
		begin
			continue;				--����
		end;
		

		insert into dt_user(user_serial,user_1,user_no,user_type,user_lname,user_lx,user_fname,dep_no_add,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,user_level,user_finger,user_sj,zh) 
		select top 1 @user_serial,user_1,user_no,user_type,user_lname,user_lx,user_fname,dep_no,user_workday,user_duty,null user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,0,'0000000000',getdate(),zh
		from tmp_dt_user where zh = @zh 
		order by user_card

		update wt_module set module_user = module_user + 1 where module_id = '0002'	
		--update wt_module set module_user = @user_serial where module_id = '0002'	

		
		--exec user_kh @user_serial                   --���˻�

        if @card_hao <> ''
			exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@card_hao,null,null,@card_type,0,0,'syn','syn',null   --���¿� 
			
		FETCH NEXT FROM cur_tt into @zh,@card_hao
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
  
  --����dt_user�е�user_dep
    update dt_user set user_dep = b.dep_serial,user_DepName = b.Dep_Name, dep_no = b.dep_no from dt_user a inner join dt_dep b on a.dep_no_add = b.dep_no_add


	--drop table #tmp_delete
	drop table #tmp_update_ChangeUserType
	drop table #tmp_update_CardChange
	drop table #tmp_insert


return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE procedure updateUser
@card_type int = 100,
@xf_style int = 2,
@old_card_deal int = 0  --0�˾ɿ����¿�1�ҵ������¿����ɿ�Ǯת�¿�
as 
set nocount on

	declare @user_flag int
    declare @user_serial int
    declare @card_hao varchar(50)
    declare @new_card varchar(50)
    declare @old_card varchar(50)
	declare @user_no varchar(50)
	declare @old_user_type int
	declare @new_user_type int
	declare @zh varchar(200)
	declare @card_state int  --0����1��ʧ2�˿�3���
	declare @count int
	declare @result int

    set @card_type = 100 

 --��������ԱΪ��ְ
	--select user_serial,user_card 
	--into #tmp_delete 
	--from dt_user 
 --   where zh not in(select a.zh from tmp_dt_user a inner join dt_user b on a.zh = b.zh) and isnull(user_type,0) <> 51  --2013-12-7 zlb
 --   declare cur_tt cursor for
	--select * from #tmp_delete 
	--open cur_tt
	--FETCH NEXT FROM cur_tt into @user_serial,@card_hao
	--while @@FETCH_STATUS = 0 
	--begin   
	--	exec user_destroy @user_serial,@card_hao  
	--  	FETCH NEXT FROM cur_tt into @user_serial,@card_hao
	--end
	--CLOSE cur_tt
	--DEALLOCATE cur_tt   
 --��Ա����
    
    --�ȴ���user_type�仯��
	select b.user_serial,isnull(b.user_card,'') old_card,isnull(a.user_card,'') new_card,isnull(a.user_type,0) new_user_type,isnull(b.user_type,0) old_user_type
    into #tmp_update_ChangeUserType
	from tmp_dt_user a inner join dt_user b on a.user_no = b.user_no
	                                        and ((a.user_type = 51 and ISNULL(b.user_type,0) <> 51) or (isnull(a.user_type,0) <> 51 and b.user_type = 51)) 
	declare cur_tt cursor for 
	select * from #tmp_update_ChangeUserType 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_serial,@old_card,@new_card,@new_user_type,@old_user_type
	while @@FETCH_STATUS = 0 
	begin
	        if @new_user_type = 51    --��ְ
				exec user_destroy @user_serial,@old_card
		    if @old_user_type = 51  --��ְ
			begin
				exec user_resume @user_serial                   --��ְ	
			end
		  	FETCH NEXT FROM cur_tt into @user_serial,@old_card,@new_card,@new_user_type,@old_user_type
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt    
	 
    ---�����仯��
    select b.user_serial,isnull(a.user_card,'') new_card,isnull(b.user_card,'') old_card,isnull(a.card_state,0) card_state
    into #tmp_update_CardChange
	from tmp_dt_user a 
	inner join dt_user b on a.user_no = b.user_no 
	                     and (
								(isnull(a.user_card,'') <> isnull(b.user_card,'') and isnull(a.user_card,'') <> '' and isnull(b.user_card,'') <> '')  --����
							  or (isnull(a.user_card,'') = isnull(b.user_card,'') and isnull(a.user_card,'') <> '' and isnull(a.card_state,0) > 0)     --��ʧ���˿�
							  or (isnull(a.user_card,'') <> '' and isnull(b.user_card,'') = '' and (isnull(a.card_state,0) = 0 or isnull(a.card_state,0) = 2)) --�������˿�
	                         )
	where ISNULL(a.user_type,0) <> 51

    declare cur_tt cursor for 
	select * from #tmp_update_CardChange 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_serial,@new_card,@old_card,@card_state
	while @@FETCH_STATUS = 0 
	begin
	    if (@new_card <> @old_card) and (@new_card <> '') and (@old_card <> '')   --������
        begin
            if @old_card_deal = 0
            begin
				exec ClientPro_Card_back @user_serial,null,@old_card,null,null,@card_type,null,2,'syn','syn',null   --�˾ɿ�
 				exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card,null,null,@card_type,0,0,'syn','syn',null  --���¿�
            end;
            else
            begin
				exec ClientPro_Card_replace @user_serial,2,null,null,@old_card,100,null,@new_card,100,0,'syn',null,'syn'  --�Ҿɿ����¿����ɿ����ת�¿�
            end
		end
		else if (@new_card = @old_card) and (@new_card <> '') and (@card_state = 1)     --��ʧ��
        begin
			exec ClientPro_Card_loss @user_serial,@old_card,null,2,'syn','syn',null    --��ʧ
		end
		else if (@new_card <> '') and (@card_state = 2)    --�˿���
        begin   
            select @count = count(1) from dt_card where card_hao = @new_card
            if @count > 0
				exec ClientPro_Card_back @user_serial,null,@new_card,null,null,@card_type,null,2,'syn','syn',null  --�˿�       
		end
		else if (@new_card <> '') and (@old_card = '') and (@card_state = 0)     --���¿��򿨽��
        begin	
			select @count = count(1) from dt_card where card_hao = @new_card and card_type = 1
            if @count > 0 
				exec ClientPro_Card_unloss @user_serial,null,@new_card,@card_type,2,'syn','syn',null  -- ִ�н��
			else
				exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card,null,null,@card_type,0,0,'syn','syn',null  --���¿�
		end

	  	FETCH NEXT FROM cur_tt into @user_serial,@new_card,@old_card,@card_state
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt 
	
	
	
	
	
	---���������ֶα仯 

	update dt_user set user_lname = b.user_lname,user_no = b.user_no,user_fname = b.user_fname,user_lx = b.user_lx,user_type = b.user_type,dep_no_add =b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
	           user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = b.user_xueli,user_birthday = b.user_birthday,
	           user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address,user_1 = b.user_1,user_2 = b.user_2,user_level = 0,user_sj = GETDATE() 
	from dt_user a inner join tmp_dt_user b 
	     on a.user_no = b.user_no
		 and (
		 isnull(a.user_lname,'') <> isnull(b.user_lname,'') or
		 isnull(a.user_fname,'') <> isnull(b.user_fname,'') or
		 isnull(a.user_lx,0) <> isnull(b.user_lx,0) or
		 --isnull(a.user_type,0) <> isnull(b.user_type,0) or
		 isnull(a.dep_no_add,'') <> isnull(b.dep_no,'') or
		 isnull(a.user_sex,'') <> isnull(b.user_sex,'') or
		 isnull(a.user_workday,'') <> isnull(b.user_workday,'') or
		 isnull(a.user_duty,'') <> isnull(b.user_duty,'') or
		 isnull(a.user_xueli,'') <> isnull(b.user_xueli,'') or
		 isnull(a.user_id,'') <> isnull(b.user_id,'') or
		 isnull(a.user_telephone,'') <> isnull(b.user_telephone,'') or
		 isnull(a.user_address,'') <> isnull(b.user_address,'') or
		 isnull(a.user_1,'') <> isnull(b.user_1,'') or
		 isnull(a.user_2,'') <> isnull(b.user_2,'') 
		 )


 --��Ա���

	select distinct user_no,isnull(user_card,'') card_hao
	into #tmp_insert
	from tmp_dt_user 
    where user_no not in(select a.user_no from tmp_dt_user a inner join dt_user b on a.user_no = b.user_no)

	declare cur_tt cursor for 
	select * from #tmp_insert 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_no,@card_hao
	while @@FETCH_STATUS = 0 
	begin
        --add��Ա��
		select @user_serial = module_user+1 from wt_module where module_id = '0002'
		
		--������Ա�˻�
		exec ClientPro_user_kh '0000000000000001',@user_serial,'syn',@result output
		if @result <> 0 
		begin
			continue;				--����
		end;

		insert into dt_user(user_serial,user_1,user_no,user_type,user_lname,user_lx,user_fname,dep_no_add,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,user_level,user_finger,user_sj,zh) 
		select top 1 @user_serial,user_1,user_no,user_type,user_lname,user_lx,user_fname,dep_no,user_workday,user_duty,null user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,0,'0000000000',getdate(),zh
		from tmp_dt_user where user_no = @user_no 
		order by user_card

		update wt_module set module_user = module_user + 1 where module_id = '0002'


		--exec user_kh @user_serial                   --���˻�

        if @card_hao <> ''
			exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@card_hao,null,null,@card_type,0,0,'syn','syn',null   --���¿� 
			
		FETCH NEXT FROM cur_tt into @user_no,@card_hao
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
  
  --����dt_user�е�user_dep
    update dt_user set user_dep = b.dep_serial,user_DepName = b.Dep_Name, dep_no = b.dep_no from dt_user a inner join dt_dep b on a.dep_no_add = b.dep_no_add


	--drop table #tmp_delete
	drop table #tmp_update_ChangeUserType
	drop table #tmp_update_CardChange
	drop table #tmp_insert


return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE procedure updateUser_small_scm
@card_type int = 100,
@xf_style int = 2,
@IsSynPhoto bit = 0,
@old_card_deal int = 0  --0�˾ɿ����¿�1�ҵ������¿����ɿ�Ǯת�¿�
as 
set nocount on

	declare @user_flag int
    declare @user_serial int
    declare @card_hao varchar(50)
    declare @new_card varchar(50)
    declare @old_card varchar(50)
	declare @user_no varchar(50)
	declare @old_user_type int
	declare @new_user_type int
	declare @zh varchar(200)
	declare @khsj varchar(50)
	declare @card_state int  --1��ʧ2�˿�
	declare @count int 
	declare @result int

    set @card_type = 100 

 --��������ԱΪ��ְ
	--select user_serial,user_card 
	--into #tmp_delete 
	--from dt_user a
 --   where not exists(select * from tmp_dt_user b where a.zh = b.zh and a.khsj = b.khsj) and isnull(a.user_type,0) <> 51  --2013-12-7 zlb

 --   declare cur_tt cursor for
	--select * from #tmp_delete 
	--open cur_tt
	--FETCH NEXT FROM cur_tt into @user_serial,@card_hao
	--while @@FETCH_STATUS = 0 
	--begin   
	--	exec user_destroy @user_serial,@card_hao  
	--  	FETCH NEXT FROM cur_tt into @user_serial,@card_hao
	--end
	--CLOSE cur_tt
	--DEALLOCATE cur_tt   
 --��Ա����
    
    --�ȴ���user_type�仯��
	select b.user_serial,isnull(b.user_card,'') old_card,isnull(a.user_card,'') new_card,isnull(a.user_type,0) new_user_type,isnull(b.user_type,0) old_user_type
    into #tmp_update_ChangeUserType
	from tmp_dt_user a inner join dt_user b on a.zh = b.zh and a.khsj = b.khsj
	                                        and ((a.user_type = 51 and ISNULL(b.user_type,0) <> 51) or (isnull(a.user_type,0) <> 51 and b.user_type = 51)) 
	declare cur_tt cursor for 
	select * from #tmp_update_ChangeUserType 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_serial,@old_card,@new_card,@new_user_type,@old_user_type
	while @@FETCH_STATUS = 0 
	begin
	        if @new_user_type = 51    --��ְ
				exec user_destroy @user_serial,@old_card
		    if @old_user_type = 51  --��ְ
			begin
				exec user_resume @user_serial                   --��ְ	
			end
		  	FETCH NEXT FROM cur_tt into @user_serial,@old_card,@new_card,@new_user_type,@old_user_type
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt    
	 
    ---�����仯��
	select b.user_serial,isnull(a.user_card,'') new_card,isnull(b.user_card,'') old_card,isnull(a.card_state,0) card_state
    into #tmp_update_CardChange
	from tmp_dt_user a 
	inner join dt_user b on a.zh = b.zh and a.khsj = b.khsj
	                     and (
								(isnull(a.user_card,'') <> isnull(b.user_card,'') and isnull(a.user_card,'') <> '' and isnull(b.user_card,'') <> '')  --����
							  or (isnull(a.user_card,'') = isnull(b.user_card,'') and isnull(a.user_card,'') <> '' and isnull(a.card_state,0) > 0)     --��ʧ���˿�
							  or (isnull(a.user_card,'') <> '' and isnull(b.user_card,'') = '' and (isnull(a.card_state,0) = 0 or isnull(a.card_state,0) = 2)) --�������˿�
	                         )
	where ISNULL(a.user_type,0) <> 51

    declare cur_tt cursor for 
	select * from #tmp_update_CardChange 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_serial,@new_card,@old_card,@card_state
	while @@FETCH_STATUS = 0 
	begin
	    if (@new_card <> @old_card) and (@new_card <> '') and (@old_card <> '')   --������
        begin
            if @old_card_deal = 0
            begin
				exec ClientPro_Card_back @user_serial,null,@old_card,null,null,@card_type,null,2,'syn','syn',null   --�˾ɿ�
 				exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card,null,null,@card_type,0,0,'syn','syn',null  --���¿�
            end;
            else
            begin
				exec ClientPro_Card_replace @user_serial,2,null,null,@old_card,100,null,@new_card,100,0,'syn',null,'syn'  --�Ҿɿ����¿����ɿ����ת�¿�
            end
		end
		else if (@new_card = @old_card) and (@new_card <> '') and (@card_state = 1)     --��ʧ��
        begin
			exec ClientPro_Card_loss @user_serial,@old_card,null,2,'syn','syn',null    --��ʧ
		end
		else if (@new_card <> '') and (@card_state = 2)    --�˿���
        begin   
            select @count = count(1) from dt_card where card_hao = @new_card
            if @count > 0
				exec ClientPro_Card_back @user_serial,null,@new_card,null,null,@card_type,null,2,'syn','syn',null  --�˿�       
		end
		else if (@new_card <> '') and (@old_card = '') and (@card_state = 0)     --���¿��򿨽��
        begin	
			select @count = count(1) from dt_card where card_hao = @new_card and card_type = 1
            if @count > 0 
				exec ClientPro_Card_unloss @user_serial,null,@new_card,@card_type,2,'syn','syn',null  -- ִ�н��
			else
				exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card,null,null,@card_type,0,0,'syn','syn',null  --���¿�
		end

	  	FETCH NEXT FROM cur_tt into @user_serial,@new_card,@old_card,@card_state
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
   	
	
	
	---���������ֶα仯 
    if @IsSynPhoto = 1
		update dt_user set user_lname = b.user_lname,user_no = b.user_no,user_fname = b.user_fname,user_lx = b.user_lx,user_type = b.user_type,dep_no_add =b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
				   user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = b.user_xueli,user_birthday = b.user_birthday,
				   user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address,user_level = 0,user_sj = GETDATE(),user_photo =b.user_photo 
		from dt_user a inner join tmp_dt_user b 
			 on a.zh = b.zh  and a.khsj = b.khsj
			 and (
			 isnull(a.user_no,'') <> isnull(b.user_no,'') or
			 isnull(a.user_lname,'') <> isnull(b.user_lname,'') or
			 isnull(a.user_fname,'') <> isnull(b.user_fname,'') or
			 isnull(a.user_lx,0) <> isnull(b.user_lx,0) or
			 --isnull(a.user_type,0) <> isnull(b.user_type,0) or
			 isnull(a.dep_no_add,'') <> isnull(b.dep_no,'') or
			 isnull(a.user_sex,'') <> isnull(b.user_sex,'') or
			 isnull(a.user_workday,'') <> isnull(b.user_workday,'') or
			 isnull(a.user_duty,'') <> isnull(b.user_duty,'') or
			 isnull(a.user_xueli,'') <> isnull(b.user_xueli,'') or
			 isnull(a.user_id,'') <> isnull(b.user_id,'') or
			 isnull(a.user_telephone,'') <> isnull(b.user_telephone,'') or
			 isnull(a.user_address,'') <> isnull(b.user_address,'') or
			 isnull(a.user_photo,0) <> isnull(b.user_photo,0)
			 )
	else
		update dt_user set user_lname = b.user_lname,user_no = b.user_no,user_fname = b.user_fname,user_lx = b.user_lx,user_type = b.user_type,dep_no_add =b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
				user_card = b.user_card,user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = b.user_xueli,user_birthday = b.user_birthday,
				user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address,user_level = 0,user_sj = GETDATE() 
		from dt_user a inner join tmp_dt_user b 
	     on a.zh = b.zh  and a.khsj = b.khsj
		 and (
		 isnull(a.user_no,'') <> isnull(b.user_no,'') or
		 isnull(a.user_lname,'') <> isnull(b.user_lname,'') or
		 isnull(a.user_fname,'') <> isnull(b.user_fname,'') or
		 isnull(a.user_lx,0) <> isnull(b.user_lx,0) or
		 isnull(a.user_type,0) <> isnull(b.user_type,0) or
		 isnull(a.dep_no_add,'') <> isnull(b.dep_no,'') or
		 isnull(a.user_sex,'') <> isnull(b.user_sex,'') or
		 isnull(a.user_workday,'') <> isnull(b.user_workday,'') or
		 isnull(a.user_duty,'') <> isnull(b.user_duty,'') or
		 isnull(a.user_xueli,'') <> isnull(b.user_xueli,'') or
		 isnull(a.user_id,'') <> isnull(b.user_id,'') or
		 isnull(a.user_telephone,'') <> isnull(b.user_telephone,'') or
		 isnull(a.user_address,'') <> isnull(b.user_address,'')
		 )

 --��Ա���

	select distinct zh,khsj,isnull(user_card,'') card_hao
	into #tmp_insert
	from tmp_dt_user a
    where not exists(select * from dt_user b where a.zh = b.zh  and a.khsj = b.khsj)

	declare cur_tt cursor for 
	select * from #tmp_insert 
	open cur_tt
	FETCH NEXT FROM cur_tt into @zh,@khsj,@card_hao
	while @@FETCH_STATUS = 0 
	begin
                --add��Ա��

		select @user_serial = module_user+1 from wt_module where module_id = '0002'
		
		--������Ա�˻�
		exec ClientPro_user_kh '0000000000000001',@user_serial,'syn',@result output
		if @result <> 0 
		begin
			continue;				--����
		end;
		

		insert into dt_user(user_serial,user_1,user_no,user_type,user_lname,user_lx,user_fname,dep_no_add,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,user_level,user_sj,zh,khsj,user_finger) 
		select top 1 @user_serial,user_1,user_no,user_type,user_lname,user_lx,user_fname,dep_no,user_workday,user_duty,null user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,0,getdate(),zh,khsj,'0000000000'
		from tmp_dt_user where zh = @zh and khsj = @khsj
		order by user_card

		update wt_module set module_user = module_user + 1 where module_id = '0002'
		
		--exec user_kh @user_serial                   --���˻�

        if @card_hao <> ''
			exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@card_hao,null,null,@card_type,0,0,'syn','syn',null   --���¿� 
              
		FETCH NEXT FROM cur_tt into @zh,@khsj,@card_hao
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
  
  --����dt_user�е�user_dep
        update dt_user set user_dep = b.dep_serial,user_DepName = b.Dep_Name, dep_no = b.dep_no from dt_user a inner join dt_dep b on a.dep_no_add = b.dep_no_add


	--drop table #tmp_delete
	drop table #tmp_update_ChangeUserType
	drop table #tmp_update_CardChange
	drop table #tmp_insert


return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE procedure updatePhoto
as 
set nocount on

 --ɾ��
        delete from dt_photo where not exists(select * from dt_user where dt_photo.user_serial = dt_user.user_serial)    

 --���
        
	
	insert into dt_photo(lx,user_serial,photo_name,photo_type,photo_path,sj)
	select 0,user_serial,cast(user_serial as varchar(16))+'.jpg',0,'../photo/'+cast(user_serial/1000 as varchar(10))+'/',getdate()
	from dt_user 
	where not exists(select * from dt_photo where dt_photo.user_serial = dt_user.user_serial) 
	      and user_photo = 1


 
return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE procedure updateUser_small_bs
@IsSynPhoto bit = 0
as
 set nocount on

 --��������ԱΪ��ְ
        --update dt_user set user_type = 51 
        --where user_serial not in(select a.user_serial from dt_user a inner join tmp_dt_user b on a.zh = b.zh and a.khsj = b.khsj) and isnull(user_type,0) <> 51  --2013-12-7 zlb
 
 --��Ա����
    if @IsSynPhoto = 1 --ͬ����Ƭ
		update dt_user set user_no = b.user_no,user_lname = b.user_lname,user_fname = b.user_fname,user_type = b.user_type,dep_no_add =b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
							   user_card = case when isnull(b.card_state,0) <> 0 then '' else b.user_card end,user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = b.user_xueli,user_birthday = b.user_birthday,
							   user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address,user_level = 0,user_sj = getdate(),user_photo = b.user_photo  
			from dt_user a inner join tmp_dt_user b on a.zh = b.zh and a.khsj = b.khsj
    else
		update dt_user set user_no = b.user_no,user_lname = b.user_lname,user_fname = b.user_fname,user_type = b.user_type,dep_no_add =b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
							   user_card = case when isnull(b.card_state,0) <> 0 then '' else b.user_card end,user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = b.user_xueli,user_birthday = b.user_birthday,
							   user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address,user_level = 0,user_sj = getdate()  
		from dt_user a inner join tmp_dt_user b on a.zh = b.zh and a.khsj = b.khsj
    
 --��Ա���
    declare @zh varchar(200)
    declare @khsj varchar(20)
    declare @user_serial int
	declare cur_tt cursor for 
	select distinct zh,khsj from tmp_dt_user a
    where not exists(select * from dt_user b where a.zh = b.zh and a.khsj = b.khsj)
	open cur_tt
	FETCH NEXT FROM cur_tt into @zh,@khsj
	while @@FETCH_STATUS = 0 
	begin
		select @user_serial = reg_user+1 from wt_reg


		insert into dt_user(user_serial,user_1,user_no,user_type,user_lname,user_fname,dep_no_add,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,user_level,user_sj,zh,khsj) 
		select top 1 @user_serial,user_1,user_no,user_type,user_lname,user_fname,dep_no,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,0,getdate(),zh,khsj 
		from tmp_dt_user where zh = @zh and khsj = @khsj

		update wt_reg set reg_user = reg_user+1
	
		FETCH NEXT FROM cur_tt into @zh,@khsj
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
  
  --����dt_user�е�user_dep
       update dt_user set user_dep = b.dep_serial,user_DepName = b.Dep_Name, dep_no = b.dep_no from dt_user a inner join dt_dep b on a.dep_no_add = b.dep_no_add


return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

--------------------------------------------------------------------------�¿���


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateUser_scm_xkp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateUser_scm_xkp]
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateUser_xkp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateUser_xkp]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[updateDep_xkp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[updateDep_xkp]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE procedure updateUser_xkp
@OutDutyDepNo varchar(400)
as
 set nocount on

 -- ͬ��ɾ������Ա
    update dt_user set user_type = 51 where user_no not in(select a.user_no from dt_user a inner join tmp_dt_user b on a.user_no = b.user_no)  
--  
 
 --��Ա����
	update dt_user set user_no = b.user_no,user_dep = b.user_dep,user_lname = b.user_lname,user_fname = b.user_fname,user_type = b.user_type,dep_no_add 
=b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
                           user_card = b.user_card,user_photo = b.user_photo,user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = 
b.user_xueli,user_birthday = b.user_birthday,
                           user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address
        from dt_user a inner join tmp_dt_user b on a.user_no = b.user_no

--��Ա���
         declare @user_serial int
        declare @user_no varchar(20)

	select user_no 
	into #user
	from tmp_dt_user 
    where user_no not in(select a.user_no from dt_user a inner join tmp_dt_user b on a.user_no = b.user_no)
	
	declare cur_tt cursor for 
	select * from #user
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_no
	while @@FETCH_STATUS = 0 
	begin
        select @user_serial = reg_user+1 from wt_reg

		insert into dt_user(user_serial,user_1,user_no,user_type,user_lname,user_fname,dep_no_add,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,user_level,user_finger) 
		select @user_serial,user_1,user_no,user_type,user_lname,user_fname,dep_no,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,0,user_finger 
		from tmp_dt_user where user_no = @user_no

		update wt_reg set reg_user = reg_user+1

	    
	    FETCH NEXT FROM cur_tt into @user_no
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  

 --����ְ���ŵ���Ա��ְ
    if @OutDutyDepNo <> '' 
       update dt_user set user_type = 51 where dep_no = @OutDutyDepNo 

  
  --����dt_user�е�dep_no
       update dt_user set dep_no = b.dep_no,user_dep = b.dep_serial,user_depname = b.dep_name from dt_user a inner join dt_dep b on a.dep_no_add = b.dep_no_add

    drop table #user
return
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE procedure updateUser_scm_xkp
@card_type int = 100,
@xf_style int = 2,
@old_card_deal int = 0,  --0�˾ɿ����¿�1�ҵ������¿����ɿ�Ǯת�¿�
@OutDutyDepNo varchar(400)
as 
set nocount on

	declare @user_flag int
    declare @user_serial int
    declare @card_hao varchar(50)
    declare @new_card varchar(50)
    declare @old_card varchar(50)
	declare @user_no varchar(50)
	declare @old_user_type int
	declare @new_user_type int
	declare @zh varchar(200)
	declare @card_state int  --0����1��ʧ2�˿�3���
	declare @count int
	declare @result int

    set @card_type = 100 

 --��������ԱΪ��ְ
	--select user_serial,user_card 
	--into #tmp_delete 
	--from dt_user 
 --   where zh not in(select a.zh from tmp_dt_user a inner join dt_user b on a.zh = b.zh) and isnull(user_type,0) <> 51  --2013-12-7 zlb

    --if @OutDutyDepNo <> '' 
    --insert into #tmp_delete(user_serial,user_card)
    --select user_serial,user_card from dt_user where dep_no = @OutDutyDepNo  

 --   declare cur_tt cursor for
	--select * from #tmp_delete 
	--open cur_tt
	--FETCH NEXT FROM cur_tt into @user_serial,@card_hao
	--while @@FETCH_STATUS = 0 
	--begin   
	--	exec user_destroy @user_serial,@card_hao  
	--  	FETCH NEXT FROM cur_tt into @user_serial,@card_hao
	--end
	--CLOSE cur_tt
	--DEALLOCATE cur_tt   
 --��Ա����
    
    --�ȴ���user_type�仯��
	select b.user_serial,isnull(b.user_card,'') old_card,isnull(a.user_card,'') new_card,isnull(a.user_type,0) new_user_type,isnull(b.user_type,0) 
old_user_type
    into #tmp_update_ChangeUserType
	from tmp_dt_user a inner join dt_user b on a.user_no = b.user_no
	                                        and ((a.user_type = 51 and ISNULL(b.user_type,0) <> 51) or (isnull(a.user_type,0) <> 51 and b.user_type = 
51)) 
	declare cur_tt cursor for 
	select * from #tmp_update_ChangeUserType 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_serial,@old_card,@new_card,@new_user_type,@old_user_type
	while @@FETCH_STATUS = 0 
	begin
	        if @new_user_type = 51    --��ְ
				exec user_destroy @user_serial,@old_card
		    if @old_user_type = 51  --��ְ
			begin
				exec user_resume @user_serial                   --��ְ	
			end
		  	FETCH NEXT FROM cur_tt into @user_serial,@old_card,@new_card,@new_user_type,@old_user_type
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt    
	 
    ---�����仯��
    select b.user_serial,isnull(a.user_card,'') new_card,isnull(b.user_card,'') old_card,isnull(a.card_state,0) card_state
    into #tmp_update_CardChange
	from tmp_dt_user a 
	inner join dt_user b on a.user_no = b.user_no 
	                     and (
								(isnull(a.user_card,'') <> isnull(b.user_card,'') and isnull(a.user_card,'') <> '' and isnull(b.user_card,'') <> '')  --���� v2.04
							  or (isnull(a.user_card,'') = isnull(b.user_card,'') and isnull(a.user_card,'') <> '' and isnull(a.card_state,0) > 0)     --��ʧ���˿� v2.04
							  or (isnull(a.user_card,'') <> '' and isnull(b.user_card,'') = '' and (isnull(a.card_state,0) = 0 or isnull(a.card_state,0) = 2)) --�������˿� v2.04
	                         )
	where ISNULL(a.user_type,0) <> 51

    declare cur_tt cursor for 
	select * from #tmp_update_CardChange 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_serial,@new_card,@old_card,@card_state
	while @@FETCH_STATUS = 0 
	begin
	   if (@new_card <> @old_card) and (@new_card <> '') and (@old_card <> '')   --������
        begin
            if @old_card_deal = 0
            begin
				exec ClientPro_Card_back @user_serial,null,@old_card,null,null,@card_type,null,2,'syn','syn',null   --�˾ɿ�
 				exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card,null,null,@card_type,0,0,'syn','syn',null  --���¿�
            end;
            else
            begin
				exec ClientPro_Card_replace @user_serial,2,null,null,@old_card,100,null,@new_card,100,0,'syn',null,'syn'  --�Ҿɿ����¿����ɿ����ת�¿�
            end
		end
		else if (@new_card = @old_card) and (@new_card <> '') and (@card_state = 1)     --��ʧ��
        begin
			exec ClientPro_Card_loss @user_serial,@old_card,null,2,'syn','syn',null    --��ʧ
		end
		else if (@new_card <> '') and (@card_state = 2)    --�˿���
        begin   
            select @count = count(1) from dt_card where card_hao = @new_card
            if @count > 0
				exec ClientPro_Card_back @user_serial,null,@new_card,null,null,@card_type,null,2,'syn','syn',null  --�˿�       
		end
		else if (@new_card <> '') and (@old_card = '') and (@card_state = 0)     --���¿��򿨽��
        begin	
			select @count = count(1) from dt_card where card_hao = @new_card and card_type = 1
            if @count > 0 
				exec ClientPro_Card_unloss @user_serial,null,@new_card,@card_type,2,'syn','syn',null  -- ִ�н��
			else
				exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@new_card,null,null,@card_type,0,0,'syn','syn',null  --���¿�
		end

	  	FETCH NEXT FROM cur_tt into @user_serial,@new_card,@old_card,@card_state
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt 
	
	
	
	
	
	---���������ֶα仯 

	update dt_user set user_lname = b.user_lname,user_no = b.user_no,user_fname = b.user_fname,user_lx = b.user_lx,user_type = b.user_type,dep_no_add =b.dep_no,user_workday = b.user_workday,user_duty = b.user_duty,
	           user_sex = b.user_sex,user_nation = b.user_nation,user_xueli = b.user_xueli,user_birthday = b.user_birthday,
	           user_id = b.user_id,user_telephone = b.user_telephone,user_address = b.user_address,user_1 = b.user_1,user_2 = b.user_2,user_level = 0,user_sj = GETDATE() 
	from dt_user a inner join tmp_dt_user b 
	     on a.user_no = b.user_no
		 and (
		 isnull(a.user_lname,'') <> isnull(b.user_lname,'') or
		 isnull(a.user_fname,'') <> isnull(b.user_fname,'') or
		 isnull(a.user_lx,0) <> isnull(b.user_lx,0) or
		 --isnull(a.user_type,0) <> isnull(b.user_type,0) or
		 isnull(a.dep_no_add,'') <> isnull(b.dep_no,'') or
		 isnull(a.user_sex,'') <> isnull(b.user_sex,'') or
		 isnull(a.user_workday,'') <> isnull(b.user_workday,'') or
		 isnull(a.user_duty,'') <> isnull(b.user_duty,'') or
		 isnull(a.user_xueli,'') <> isnull(b.user_xueli,'') or
		 isnull(a.user_id,'') <> isnull(b.user_id,'') or
		 isnull(a.user_telephone,'') <> isnull(b.user_telephone,'') or
		 isnull(a.user_address,'') <> isnull(b.user_address,'') or
		 isnull(a.user_1,'') <> isnull(b.user_1,'') or
		 isnull(a.user_2,'') <> isnull(b.user_2,'') 
		 )


 --��Ա���

	select distinct user_no,isnull(user_card,'') card_hao
	into #tmp_insert
	from tmp_dt_user 
    where user_no not in(select a.user_no from tmp_dt_user a inner join dt_user b on a.user_no = b.user_no)

	declare cur_tt cursor for 
	select * from #tmp_insert 
	open cur_tt
	FETCH NEXT FROM cur_tt into @user_no,@card_hao
	while @@FETCH_STATUS = 0 
	begin
        --add��Ա��
		select @user_serial = module_user+1 from wt_module where module_id = '0002'
		
		--������Ա�˻�
		exec ClientPro_user_kh '0000000000000001',@user_serial,'syn',@result output
		if @result <> 0 
		begin
			continue;				--����
		end;
		

		insert into dt_user(user_serial,user_1,user_no,user_type,user_lname,user_lx,user_fname,dep_no_add,user_workday,user_duty,user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,user_level,user_finger,user_sj,zh) 
		select top 1 @user_serial,user_1,user_no,user_type,user_lname,user_lx,user_fname,dep_no,user_workday,user_duty,null user_card,user_photo,user_sex,user_nation,user_xueli,user_birthday,user_id,user_telephone,user_address,0,'0000000000',getdate(),zh
		from tmp_dt_user where user_no = @user_no 
		order by user_card

		update wt_module set module_user = module_user + 1 where module_id = '0002'	

       

		--exec user_kh @user_serial                   --���˻�

        if @card_hao <> ''
			exec ClientPro_Card_add 'syn',2,'0000000000000001',@user_serial,@card_hao,null,null,@card_type,0,0,'syn','syn',null   --���¿� 
			
		FETCH NEXT FROM cur_tt into @user_no,@card_hao
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
  
  --����dt_user�е�user_dep
    update dt_user set user_dep = b.dep_serial,user_DepName = b.Dep_Name, dep_no = b.dep_no from dt_user a inner join dt_dep b on a.dep_no_add = b.dep_no_add


	--drop table #tmp_delete
	drop table #tmp_update_ChangeUserType
	drop table #tmp_update_CardChange
	drop table #tmp_insert


return
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE procedure updateDep_xkp
@company varchar(200)
as
 set nocount on
 


	declare @dep_no_add varchar(20)
	declare @dep_no_parent varchar(20)
	declare @dep_name varchar(200)
	declare @dep_serial int
	declare @dep_parent int
	declare @dep_order int

 ------------------------- ͬ��ɾ���Ĳ���
        delete from dt_dep where dep_no_add not in(select a.dep_no_add from dt_dep a inner join dep_temp b on a.dep_no_add = b.dep_no_add) and dep_parent <> 0
 --���Ÿ���
       update dt_dep set dep_name = b.dep_name,dep_no_parent = b.dep_no_parent from dt_dep a inner join dep_temp b on a.dep_no_add = b.dep_no_add


--------------------------����ͷ�ڵ�
        delete from dt_dep where dep_parent = 0   --��ɾ��Ĭ��ͷ���        

--        set @dep_name = '�����б�'
        insert dt_dep(dep_serial,dep_parent,dep_order,dep_name,dep_no,dep_rule,dep_no_add) values(10000,0,0,@company,'001',0,'0')

 ------------------------�������
	declare cur_tt cursor for 
	select dep_no_add,dep_no_parent,dep_name from dep_temp where dep_no_add not in(select a.dep_no_add from dt_dep a inner join dep_temp b on a.dep_no_add = b.dep_no_add) order by dep_no_add
	open cur_tt
	FETCH NEXT FROM cur_tt into @dep_no_add,@dep_no_parent,@dep_name
	while @@FETCH_STATUS = 0 
	begin
          set @dep_parent = 10000      
	  select @dep_parent= isnull(dep_serial,10000) from dt_dep where dep_no_add = @dep_no_parent   
          set @dep_order = 0
	  select @dep_order = max(dep_order)+1 from dt_dep where dep_parent = @dep_parent 

	  select @dep_serial = reg_dep+1 from wt_reg 

          begin tran
	    insert into dt_dep(dep_serial,dep_parent,dep_order,dep_name,dep_rule,dep_no_add,dep_no_parent) values(@dep_serial,@dep_parent,@dep_order,@dep_name,0,@dep_no_add,@dep_no_parent)
	    update wt_reg set reg_dep = reg_dep+1
	  commit tran
          if @@error <> 0 
          rollback tran     

	  FETCH NEXT FROM cur_tt into @dep_no_add,@dep_no_parent,@dep_name 
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  

--------------------------��dep_parent
	update dt_dep set dep_parent = isnull(b.dep_serial,10000) from dt_dep left join dt_dep b on dt_dep.dep_no_parent = b.dep_no_add where dt_dep.dep_serial <> 10000 

---------------------------��dep_order��dep_no
 

        declare @level int   
        declare @count int

    	update dt_dep set dt_dep.dep_order=b.rn-1 from dt_dep a,
	(
		select t.*, rn=(select count(1) from dt_dep where 
		id<=t.id and dep_parent=t.dep_parent)
		from dt_dep t
	
	)b
	where a.dep_serial=b.dep_serial
	


  	select @level = max(level) from get_ChildDep(0)
        while @level > 0 
        begin        
		update dt_dep set dt_dep.dep_no=cast(b.dep_no as varchar)+right(cast(power(10,2) as varchar)+Convert(varchar,a.dep_order),2)
		from dt_dep a,
		(
		select b.* from dt_dep a,dt_dep b where a.dep_parent=b.dep_serial
		)b
		where a.dep_parent=b.dep_serial

                set @level= @level -1 
        end

return
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO