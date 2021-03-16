
  SELECT [dbo].[tbl_pos].[id], [dbo].[tbl_customer].[name], [dbo].[tbl_pos].[cost_total]
  FROM	[dbo].[tbl_pos]
  INNER JOIN tbl_customer ON [dbo].[tbl_pos].customer_id=[dbo].[tbl_customer].[id];