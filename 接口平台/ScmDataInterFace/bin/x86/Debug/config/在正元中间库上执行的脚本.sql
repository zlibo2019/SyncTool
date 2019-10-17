
if object_id('inttohex') is not null
drop function inttohex
GO
CREATE function [dbo].[inttohex](@int10 bigint)
returns varchar(16)
begin
    declare @str16 nvarchar(16)
    set @str16=''

    if(@int10>0)
    begin
        while @int10>0
        begin
            set @str16=substring('0123456789ABCDEF',@int10%16+1,1)+@str16
            set @int10=@int10/16
        end
    end
        else
        begin
            set @str16='0'
        end
    return @str16
end
GO

IF EXISTS(SELECT 1 FROM sys.views WHERE name='view_ac_dict_Accounts')
	DROP VIEW [view_ac_dict_Accounts]
GO
CREATE VIEW [view_ac_dict_Accounts] AS
	select a.*,b.Pic photo,c.DepCase dep_no,d.ClsName
	from ac_dict_Accounts a
	left join ac_dict_AccPicture b on a.AccNo = b.AccNo
	left join ac_dict_AccDep c on a.DepCode = c.DepCode
	left join ac_dict_AccountClass d on a.AccClsCode = d.ClsCode
	where a.AccStatus > 0
GO
