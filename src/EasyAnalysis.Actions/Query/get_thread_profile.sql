﻿SELECT RTRIM([Id]) AS [Id]
      ,[Title]
      ,[CreateOn] as [CreatedOn]
      ,[Category]
      ,[Type]
      ,[Repository]
  FROM [uwpdb].[dbo].[VwThreads]
  WHERE [Repository] = @repository AND [CreateOn] BETWEEN @start and @end