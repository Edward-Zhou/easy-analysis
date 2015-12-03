SELECT *
FROM
  ( SELECT c.[question_id] AS [Id],
                              c.[title],
                              c.[link],
                              DATEADD(s, CAST (c.[creation_date] AS int), '19700101') AS CreatedOn,
                                                                                         c.owner_id AS AuthorId
   FROM
     (SELECT question_id
      FROM Question
      WHERE question_id IN
          (SELECT question_id
           FROM Question_TagsRelation
           WHERE tagname ='visual-studio-online'
           GROUP BY question_id)) a
   INNER JOIN
     (SELECT *
      FROM Question
      WHERE creation_date >= datediff(s, '1970-01-01', '2015-09-01')) c ON a.question_id = c.question_id ) t
WHERE [CreatedOn] >= @start
  AND [CreatedOn] <= @end