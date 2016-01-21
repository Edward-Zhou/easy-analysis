SELECT *
FROM
  ( SELECT a.question_id AS Id,
       c.title,
       c.link,
       DATEADD(s, CONVERT(int, c.creation_date), '1/1/1970') AS CreatedOn,
       c.owner_id AS AuthorId
FROM
  (SELECT question_id
   FROM dbo.Question
   WHERE (question_id IN
            (SELECT question_id
             FROM dbo.Question_TagsRelation
             WHERE (tagname = 'windows-10')
             GROUP BY question_id))
   GROUP BY question_id
   UNION SELECT question_id
   FROM dbo.Question AS Question_2
   WHERE (question_id IN
            (SELECT question_id
             FROM dbo.Question_TagsRelation AS Question_TagsRelation_1
             WHERE (tagname = 'uwp')
             GROUP BY question_id))
   GROUP BY question_id) AS a
INNER JOIN
  (SELECT question_id,
          title,
          link,
          body,
          last_edit_date,
          creation_date,
          last_activity_date,
          record_update_date,
          score,
          answer_count,
          accepted_answer_id,
          up_vote_count,
          down_vote_count,
          favorite_count,
          view_count,
          is_answered,
          owner_id,
          tags,
          status,
          IRT
   FROM dbo.Question AS Question_1) AS c ON a.question_id = c.question_id
LEFT JOIN
  (SELECT answer_id,
          question_id,
          creation_date,
          last_edit_date,
          last_activity_date,
          score,
          is_accepted,
          owner_id
   FROM dbo.Answers) AS b ON c.question_id = b.question_id
INNER JOIN dbo.QuestionOwnership AS d ON d.question_id = c.question_id ) t
WHERE [CreatedOn] >= @start
  AND [CreatedOn] <= @end