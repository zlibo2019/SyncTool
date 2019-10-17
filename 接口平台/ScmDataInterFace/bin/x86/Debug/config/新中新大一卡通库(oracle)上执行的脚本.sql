create or replace view v_department as
select code,name from area 
union
select areacode||code code,name from department
with read only;



create or replace view v_account as
select a.*,b.Name PName,b.code code,b.class class
from account a,PID b
where a.PID = b.code
with read only;

create or replace view v_account_cardType as
select a.*,b.NAME CardTypeName
from 
v_account a,
configinfo@dl_syntong b
where b.code = a.cardtype
with read only;


create or replace view v_account_inc as
select b.*
from 
(
  select distinct fromaccount 
  from trjn@dl_syntong
  where trancode in('01','02','03','04','05','06','07','12','40','41','42','43')
)a,
v_account_cardType b
where a.fromaccount = b.account
with read only;


create or replace view v_account_jszc as
select a.*,b.Name PName,b.code code,b.class class,c.addr,d.jszc
from account a,PID b,IDINFORMATION c,TEACHEREX d
where a.PID = b.code and a.account = c.account and c.no = d.no
with read only;


