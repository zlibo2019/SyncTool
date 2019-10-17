--函数
--十进制转十六进制（包含负数）
--需要取后8位
if OBJECT_ID('varbin2hexstr') is not null
drop function varbin2hexstr
go
CREATE function [dbo].[varbin2hexstr](
    @bin varbinary(8000)
    )returns varchar(8000)
    as
    begin
        declare @re varchar(8000),@i int
        select @re='',@i=datalength(@bin)
        while @i>0
            select @re=substring('0123456789ABCDEF',substring(@bin,@i,1)/16+1,1)
                    +substring('0123456789ABCDEF',substring(@bin,@i,1)%16+1,1)
                    +@re
                ,@i=@i-1
       -- return('0x'+@re)
        return @re
    end
GO

if OBJECT_ID('inttohex') is not null
drop function inttohex
go
CREATE   function [dbo].[inttohex](@num bigint) 
returns varchar(16)  
begin 
declare @num2 varbinary(8),@r varchar(50)
set @num2=convert(varbinary(8),@num)--直接转换为二进制
set @r= dbo.varbin2hexstr(@num2)--二进制转16进制字符串
return @r 
end 
GO


-- 十进制转二进制
if object_id('inttobit') is not null
drop FUNCTION inttobit
go

CREATE FUNCTION  dbo.inttobit (@number int)
returns varchar(100)
as
BEGIN
DECLARE @i int
DECLARE @j float
DECLARE @m int
DECLARE @OUT1 varCHAR(1)
DECLARE @OUT2 varchar(20)
SET @i=@number
set @out2=' '
WHILE @i>=1
BEGIN 
SET @j=@i/2
SET @m=@i%2
SET @i=floor(@j)
SET @OUT1=cast(@m as char(1))
SET @OUT2=@OUT1+@OUT2
END
RETURN @OUT2
END 
go


-- 人员表
create view v_account as
select  a.accountno as account,right([dbo].[inttohex](b.CardSN),8) as cardid,  
[dbo].[inttobit](a.Condition) as flag,a.CreateTime ,a.Name as name,
 0 as user_type,a.PersonID as user_id,a.Birthday  ,
 LEFT([dbo].[inttohex](a.Depart),(LEN([dbo].[inttohex](a.Depart))-14))+REPLACE(right([dbo].[inttohex](a.Depart),14),'00','') as Depart,
 a.Tel as phone,a.Email,a.Identi as user_role, c.Name as role_name,CONVERT(varchar(100), CreateTime, 120) as khsj,a.certcode  from TabRecord a
left join TabRecExt b
on a.AccountNo=b.AccountNo
left join TabIdenti c
on a.Identi=c.Type

go

--部门表
create view v_depart as 
select LEFT([dbo].[inttohex](List),(LEN([dbo].[inttohex](List))-14))+REPLACE(right([dbo].[inttohex](List),14),'00','') as code,name from TabDepart

go


