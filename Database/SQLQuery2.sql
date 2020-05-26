


-- Add the parameters for the stored procedure here                         
declare             
  @V_FromInvDate date = '3/20/2019',          
  @V_ToInvDate date = '3/23/2019',      
  @V_InvName nvarchar(100) = 'HOPEWAY LIMITED',          
  @V_chk int =0                        
            
    if (@V_chk = 1) -- heel          
    begin          
		select distinct H.c055, H.c032 as InvNumber 
		from          
		(select * from t025 where c028 is not null and c004 =  @V_InvName and c032 between @V_FromInvDate and @V_ToInvDate and c055 is not null) H                  
		inner join (select * from t026 where c055 <> 13) D on H.c021 = D.c044 and H.c022 = D.c045   
		inner join (Select * from t024 (nolock))P on P.c002 = D.c003 and P.mesocomp = D.mesocomp and  p.mesocomp='VNT1'   
		where H.c032 between @V_FromInvDate and @V_ToInvDate           
		--and ltrim(rtrim(upper(D.c004))) like ltrim(rtrim(upper('Heel%Counter%')))   
		and (SUBSTRING(p.c002,3,2)='01' OR SUBSTRING(p.c002,3,2)= '11')  
		and P.C078 <> '00005-00005-00000-00000-00000'  
		and H.c055 is not null and H.mesoyear=1428    
		order by H.c055          
		end           
	else -- other          
	begin           
		select distinct H.c055, H.c032 as InvNumber 
		from          
		(select * from t025 where c028 is not null and c004 =  @V_InvName and c032 between @V_FromInvDate and @V_ToInvDate and c055 is not null) H                  
		inner join (select * from t026 where c055 <> 13) D on H.c021 = D.c044 and H.c022 = D.c045   
		inner join (Select * from t024 (nolock))P on P.c002 = D.c003 and P.mesocomp = D.mesocomp  and p.mesocomp='VNT1'  
		where H.c032 between @V_FromInvDate and @V_ToInvDate     
		and (SUBSTRING(p.c002,3,2)<>'01' and SUBSTRING(p.c002,3,2)<> '11') or   
		((SUBSTRING(p.c002,3,2)='01' or SUBSTRING(p.c002,3,2)= '11' ) and P.C078 = '00005-00005-00000-00000-00000' ) and H.mesoyear=1428    
		-- and ltrim(rtrim(upper(D.c004))) not like ltrim(rtrim(upper('Heel%Counter%')))  
		and H.c055 is not null          
		order by H.c055          
    end          
          


exec [SP_InvList] '3/20/2019','3/23/2019','HOPEWAY LIMITED',0







