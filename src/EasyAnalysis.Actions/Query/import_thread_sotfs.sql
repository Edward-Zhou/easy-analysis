SELECT *
FROM
  (SELECT c.[question_id] AS [Id],
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
           WHERE tagname ='tfs'
             OR tagname ='tfs2013'
             OR tagname='tfs2015'
             OR tagname='team-explorer-everywhere'
             OR tagname='tfsbuild'
           GROUP BY question_id)) a
   INNER JOIN
     (SELECT *
      FROM Question
      WHERE creation_date >= datediff(s, '1970-01-01', '2015-09-01')) c ON a.question_id = c.question_id) t
WHERE [CreatedOn] >= @start
  AND [CreatedOn] <= @end