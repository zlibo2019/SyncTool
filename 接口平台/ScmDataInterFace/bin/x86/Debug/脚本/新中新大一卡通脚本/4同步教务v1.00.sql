use scm_main_edu

------------添加列---------------------------
IF COL_LENGTH('TKQ_SUBJECT', 'F_ID') IS  NULL  
ALTER TABLE dbo.TKQ_SUBJECT ADD F_ID VARCHAR( 50 ) NULL

IF COL_LENGTH('TKQ_ROOM', 'F_ID') IS  NULL  
ALTER TABLE dbo.TKQ_ROOM ADD F_ID VARCHAR( 50 ) NULL

IF COL_LENGTH('TKQ_Class', 'F_ID') IS  NULL 
ALTER TABLE dbo.TKQ_Class ADD F_ID VARCHAR( 50 ) NULL

IF COL_LENGTH('tkq_teach_sub', 'F_teach') IS  NULL
ALTER TABLE dbo.tkq_teach_sub ADD F_teach VARCHAR( 50 ) NULL
IF COL_LENGTH('tkq_teach_sub', 'F_sub') IS  NULL
ALTER TABLE dbo.tkq_teach_sub ADD F_sub VARCHAR( 50 ) NULL



-----------------建表-----------------------------
if object_id(N'tmp_tkq_rank_deal',N'U') is not null
drop table tmp_tkq_rank_deal
CREATE TABLE [dbo].[tmp_tkq_rank_deal](
	[Xh] [int] IDENTITY(1,1) NOT NULL,
	[Parent_bh] [varchar](20) NULL,
	[Rq] [datetime] NULL,
	[Week] [int] NULL,
	[Class] [int] NULL,
	[Teach_serial] [bigint] NULL,
	[Sub_serial] [int] NULL,
	[Student_serial] [int] NULL,
	[Room_serial] [int] NULL,
	[Sj] [datetime] NULL,
	[term_name] [nvarchar](50) NULL	
PRIMARY KEY CLUSTERED 
(
	[Xh] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



---------------------获取上课时间-------------------
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetRankDate]') and xtype in (N'FN', N'IF', N'TF'))
drop function GetRankDate
go
create function GetRankDate(@date varchar(20),@week int,@weekindex int)
returns  datetime
as
begin
  declare @RankDate datetime
  declare @we int
  select @we=DATEPART(WEEKDAY,@date)-1
  if (@week<@we)
  SELECT @RankDate=DATEADD(WEEK,@weekindex-1,DATEADD(DAY,7-@we+@week,@date))
  if (@week>=@we)
  SELECT @RankDate=DATEADD(WEEK,@weekindex-1,DATEADD(DAY,@week-@we,@date))
  return @RankDate
  
  
end 
go



--------------公共存储过程创建场所返回dep_serial和dep_no----------------------
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_get_serial_no]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_get_serial_no]
GO
create proc sp_get_serial_no
@dep_serial int output,
@dep_no varchar(20)output,
@depname varchar(20),        ----部门名称
@type int --0.room 1.subject

as
begin
	set @dep_serial=0
	set @dep_no=''
	--select dep_serial,dep_no from dt_ac_dep where dep_parent=10000 and left(dep_serial,3)=200 and Dep_type=0 
    select top 1 @dep_serial=dep_serial,@dep_no=dep_no from dt_ac_dep where dep_parent=10000 and left(dep_serial,3)=200 and Dep_type=@type 
    ------如果数据不存在插入新数据-------
    if(@dep_serial=0)
    begin
        -------获取dep_order---
        declare @dep_order int
        declare @dep_nonew varchar(20)
        
        select @dep_order=max(dep_order)+1 from dt_ac_dep where dep_parent=10000
        if(LEN (@dep_order)<2)
        set @dep_nonew='0010'+CONVERT (varchar,@dep_order)
        else
        set @dep_nonew='001'+CONVERT (varchar,@dep_order)
        -------获取最大部门场所数
        declare @depid int
        select @depid=Module_place+1 from WT_MODULE where Module_id='0021'
        insert into dt_ac_dep(dep_serial,dep_parent,dep_order,dep_name,dep_no,dep_rule,sj,module_id,Dep_type)
        values (@depid,10000,@dep_order,@depname,@dep_nonew,0,GETDATE(),'0021',@type)
        declare @res int
        select @res=COUNT(*)from dt_ac_dep where dep_no=@dep_nonew
        if(@res>0)--数据插入成功
        begin
           set @dep_serial= @depid
           set @dep_no=@dep_nonew
           update WT_MODULE set Module_place=@depid where Module_id='0021'
        end
        
        
    end
end
go


----------------同步科目--------------------------
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_update_subject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_update_subject]
GO
CREATE procedure sp_update_subject
as
-- -------------------------------处理数据------------------------------------------------↓
begin
    ---------------获取上级id-------------------------------------
    declare @dep_serial int,@dep_no1 varchar(20)
   
    exec sp_get_serial_no @dep_serial output,@dep_no1 output,'测试科目',1
    
    --------------删除科目
    delete from  tkq_subject  where not exists  ( select * from tmp_tkq_subject where tmp_tkq_subject.F_ID = tkq_subject.F_ID );
    -------------修改
     update a set a.Dep_no=a.Dep_no,
    a.Dep_parent = @dep_serial,a.Dep_name = b.Dep_name ,a.Sj = b.Sj 
    from tkq_subject a inner join tmp_tkq_subject b on b.F_ID = a.F_ID 
    
    -------------增加
    select * into #tm_tkq_subject
	from tmp_tkq_subject t where t.F_ID not in (select F_ID from tkq_subject where F_ID is not null)
	declare @dep_no varchar(50)
	declare @Dep_name nvarchar(50)
	declare @Dep_simple varchar(50)
	declare @F_ID int
	   
	
	declare cur_tt cursor for
    select dep_no,Dep_name,Dep_simple,F_ID from #tm_tkq_subject
    open cur_tt
    FETCH NEXT FROM cur_tt into @dep_no,@Dep_name,@Dep_simple,@F_ID
	while @@FETCH_STATUS = 0 
	begin
	    declare @pnum int
        select @pnum=(Module_psam) from WT_MODULE where Module_id='0011'
        insert into TKQ_SUBJECT
		(
			Dep_serial,
			Dep_no,
			Dep_name,
			Dep_simple,
			Dep_parent,
			F_ID
		) 
		values (@pnum+1,@dep_no,@Dep_name,@Dep_simple,@dep_serial,@F_ID) 
		update WT_MODULE set Module_psam=Module_psam+1 where Module_id='0011'
	
	 
	FETCH NEXT FROM cur_tt into @dep_no,@Dep_name,@Dep_simple,@F_ID
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
	drop table #tm__tkq_subject
	

	
	---------------------------置dep_order
    	update tkq_subject  set tkq_subject.dep_order=b.rn-1 from tkq_subject  a,
	(
		select t.*, rn=(select count(1) from tkq_subject where 
		dep_no<=t.dep_no and dep_parent=t.dep_parent)
		from tkq_subject t
	
	)b
	where a.dep_serial=b.dep_serial
	
  
	
end
go
--------------------------------------同步教室-----------------------------------------------
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_update_TKQ_ROOM]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_update_TKQ_ROOM]
GO
CREATE procedure sp_update_TKQ_ROOM
as
-------------------------------处理数据------------------------------------------------↓
begin 
    
    -----------------获取上级id,dep_no
    declare @dep_serial int,@dep_no1 varchar(20)
   
    exec sp_get_serial_no @dep_serial output,@dep_no1 output,'测试教室',0

    
	------删除冗余数据
	delete from  tkq_room  where not exists  ( select * from tmp_tkq_room where tmp_tkq_room.F_ID = tkq_room.F_ID );
	------修改
	update a set a.Dep_no=b.Dep_no,a.Dep_parent = @dep_serial,a.Dep_name = b.Dep_name,
	a.Dep_address = b.Dep_address,a.Dep_type = b.Dep_type,
	a.Dep_door = b.Dep_door,a.Dep_user = b.Dep_user,a.Dep_exam = b.Dep_exam,
	a.Dep_level =b.Dep_level,a.Sj = b.Sj 
	from tkq_room a inner join tmp_tkq_room  b on a.F_ID = b.F_ID
	------新增教室
	select t.Dep_no,t.Dep_name,
	case when charindex('普通',t.dep_type) > 0 then 0 when charindex('会',t.dep_type) > 0 then 2 else 1 end dep_type,
	t.Dep_user,t.Dep_exam,t.F_ID
	into #tm_tkq_room from tmp_tkq_room t
	where t.F_ID not in (select F_ID from tkq_room where F_ID is not null)
	
	declare @dep_no varchar(50)
	declare @Dep_name nvarchar(50)
	declare @Dep_type int
	declare @Dep_user int
	declare @Dep_exam int
	declare @F_ID int
	
	
	declare cur_tt cursor for
    select dep_no,Dep_name,Dep_type,Dep_user,Dep_exam,F_ID from #tm_tkq_room
    open cur_tt
    FETCH NEXT FROM cur_tt into @dep_no,@Dep_name,@Dep_type,@Dep_user,@Dep_exam,@F_ID
	while @@FETCH_STATUS = 0 
	begin
	    declare @pnums int
        select @pnums=Module_dep from WT_MODULE where Module_id='0011'
        insert into TKQ_ROOM
		(
			Dep_serial,
			Dep_no,
			Dep_parent,
			Dep_name,
			Dep_type,
			Dep_user,
			Dep_exam,
			Sj,
			F_ID
		)  
		values (@pnums+1,@dep_no,@dep_serial,@Dep_name,@Dep_type,@Dep_user,@Dep_exam,@F_ID) 
		update WT_MODULE set Module_dep=Module_dep+1 where Module_id='0011'
	
	 
	   FETCH NEXT FROM cur_tt into @dep_no,@Dep_name,@Dep_type,@Dep_user,@Dep_exam,@F_ID
	end
	CLOSE cur_tt
	DEALLOCATE cur_tt  
	drop table #tm__tkq_room
	
	---------------------------置dep_order
    	update tkq_room  set tkq_room.dep_order=b.rn-1 from tkq_room  a,
	(
		select t.*, rn=(select count(1) from tkq_room where 
		dep_no<=t.dep_no and dep_parent=t.dep_parent)
		from tkq_room t
	
	)b
	where a.dep_serial=b.dep_serial

end

go




-------------------------------同步教师与科目关系表------------------------------------------
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_update_TKQ_TEACH_SUB]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_update_TKQ_TEACH_SUB]
GO
CREATE procedure sp_update_TKQ_TEACH_SUB
as
-------------------------------处理数据------------------------------------------------↓
begin 
-----------------------删除
  delete from  tkq_teach_sub  where not exists  ( select * from tmp_tkq_teach_sub where tmp_tkq_teach_sub.F_teach = tkq_teach_sub.F_teach and tmp_tkq_teach_sub.F_sub = tkq_teach_sub.F_sub )
-----------------------修改
  update a set a.Lx = b.Lx,a.Sj = b.Sj from tkq_teach_sub a inner join tmp_tkq_teach_sub b on b.F_teach = a.F_teach and b.F_sub = a.F_sub;
-----------------------添加
 
	insert into TKQ_TEACH_SUB(Lx,
			Dep_serial,
			Teach_serial,
			Sj,
			F_sub,
			F_teach)
	select  0,
			c.Dep_serial,
			b.user_serial,
			GETDATE(),
			a.F_sub,
			a.F_teach
	from tmp_tkq_teach_sub a
    inner join dt_user b on a.F_teach = b.zh
    inner join tkq_subject c on a.F_sub = c.F_ID
    where not exists(select * from tkq_teach_sub f where a.F_teach = f.F_teach and a.F_sub = f.F_sub )
    
 
end;
go



-----------同步课程表----------------
if exists(select * from sysobjects where name='sp_update_TKQ_RANKF')
drop proc sp_update_TKQ_RANKF
go
create proc sp_update_TKQ_RANKF
as
-------------------------------处理数据------------------------------------------------↓
begin 

   truncate table tmp_tkq_rank_deal
   -----------处理tmp_tkq_rank
   INSERT INTO [tmp_tkq_rank_deal]
           ([Rq]
           ,[Week]
           ,[Class]
           ,[Teach_serial]
           ,[Student_serial]
           ,[Sub_serial]
           ,[Room_serial]
           ,[term_name])

	select dbo.GetRankDate(g.Kssj,a.weeknum,a.week),a.week,a.class,c.user_serial,f.user_serial,d.Dep_serial,
	e.Dep_serial,a.term_name
	from tmp_tkq_rank a 
	inner join dt_user c on a.F_teach = c.user_no
	inner join tkq_subject d on a.F_sub = d.F_ID
	inner join tkq_room e on a.F_room=e.F_ID
	inner join dt_user f on a.F_student=f.user_no
	inner join tkq_calendar g on a.term_name=g.Lname
	
	   
	
	--------------------------处理多个班级上同一堂课的情况
	
	select max(xh) id,rq,term_name,Week,class,Room_serial
	into #tmp_tkq_rank_press
	from tmp_tkq_rank_deal
	group by rq,term_name,Week,class,Room_serial
	select * from tmp_tkq_rank_deal
	

	
	
	-------------------------清空 教学班表、教学班与学员关系表
	--truncate table tkq_class
	--truncate table TKQ_CLASS_USER
	 --------------------插入教学班表
	insert into tkq_class(Class_serial,Class_no,class_name,Lx,Parent_bh,Gly_no,Sj)
	select  a.id Class_serial,a.id  Class_no ,'教学班'+cast(a.id as varchar) class_name,0 lx,b.bh Parent_bh,'syn',getdate()
	from #tmp_tkq_rank_press a
	inner join tkq_calendar b on a.term_name = b.Lname
	order by a.id 
	
	---------------------插入教学班与学员关系表
	insert into TKQ_CLASS_USER(Lx,Class_serial,User_serial,Dep_serial,No_study)
	select 1 Lx,a.id Class_serial,c.user_serial,c.user_dep,0 No_study 
	from #tmp_tkq_rank_press a
	inner join tmp_tkq_rank_deal b on a.rq = b.Rq and a.term_name = b.term_name and a.Week = b.Week and a.class = b.class and a.Room_serial = b.Room_serial
	inner join dt_user c on b.student_serial = c.user_serial
    order by id

    
	-------------------------当前导入数据中的学期编号
	select distinct b.bh bh 
	into #bh
	from #tmp_tkq_rank_press a 
	inner join tkq_calendar b on a.term_name = b.Lname

	---------------先删除课程表中这些学期编号的数据
	delete from tkq_rank 
	where Parent_bh in (select bh from #bh)


	---------------插入课程表
	insert into tkq_rank(Parent_bh,rq,Week,Class,Class_serial,Teach_serial,Sub_serial,Room_serial,state)
	select c.bh,b.rq,a.week,a.class,a.id,b.teach_serial,b.sub_serial,b.room_serial,0 state
	from #tmp_tkq_rank_press a 
	inner join tmp_tkq_rank_deal b on a.id = b.Xh and a.rq = b.rq and a.term_name = b.term_name and a.Week = b.Week and a.class = b.class 
	inner join tkq_calendar c on a.term_name = c.Lname
	
    ----------------删除临时表
    drop table #tmp_tkq_rank_press
    drop table #bh
   
end
go










