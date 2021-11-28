USE [KabaAccounting]
GO

UPDATE [dbo].[tbl_pos]
   SET cost_total=(
   SELECT cost_total 
   FROM [KabaAccountingOld].[dbo].tbl_pos
   WHERE [KabaAccounting].[dbo].tbl_pos.id=[KabaAccountingOld].[dbo].tbl_pos.id
   );
GO


