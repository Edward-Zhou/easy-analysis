SELECT *
FROM (SELECT
  [Question].[question_id] AS [Id],
  [Question].[title],
  [Question].[link],
  DATEADD(s, CAST([Question].[creation_date] AS int), '19700101') AS CreatedOn,
  [Question].owner_id AS AuthorId
FROM [Question]
INNER JOIN (SELECT
DISTINCT
  [question_id]
FROM [StackOverFlowDevDB].[dbo].[Question_TagsRelation]
WHERE [tagname] IN ('office365', 'office365-apps', 'office365-restapi', 'office365api')) [Filter]
  ON [Question].[question_id] = [Filter].[question_id]) [Query]
WHERE CreatedOn BETWEEN @start AND @end