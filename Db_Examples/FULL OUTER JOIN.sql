USE [KabaAccounting]
GO
--SELECT *
--  FROM [dbo].[tbl_pos] FULL OUTER JOIN [dbo].[tbl_pos_detailed] ON tbl_pos_detailed.id=tbl_pos.id WHERE added_date BETWEEN '2021-04-01 23:38:22.533' AND '2021-10-29 20:39:16.597'
SELECT *
  FROM [dbo].[tbl_pos] FULL OUTER JOIN [dbo].[tbl_pos_detailed] ON tbl_pos_detailed.id=tbl_pos.id WHERE added_date >= '2021-04-01 23:38:22.533' AND added_date <= '2021-10-29 20:39:16.597'
GO


