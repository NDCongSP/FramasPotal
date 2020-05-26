
select top 5 * from T026

select c044 as OC,c045 as DN,c055 as Invoice,c021 AcountNumber,c030,c113,c022 SerilaNo,c136 ProjectNumber, c023,c024,c025,c026,c110,c020
from T025 
where c136 = '273793' --and c044 = 'OC167157'
order by c022


select c044 as OC,c045 as DN,c055 as Invoice,c021 AcountNumber,c030,c113,c022 SerilaNo,c136 ProjectNumber, c023,c024,c025,c026,c110,c020
from T025 
where c136 = '273795' --and c044 = 'OC167159'
order by c022


select c044 as OC,c045 as DN,c055 as Invoice,c021 AcountNumber,c030,c113,c022 SerilaNo,c136 ProjectNumber, c023,c024,c025,c026,c110,c020
from T025 
where c136 = '273793'
order by c022



select c044 as OC,c045 as DN,c055 as Invoice,c021 AcountNumber,c030,c113,c022 SerilaNo,c136 ProjectNumber, c023,c024,c025,c026,c110,c020
from T025 
where c044 = 'OC162132' --c136 = '273793' --and c044 = 'OC167157'
order by c022
