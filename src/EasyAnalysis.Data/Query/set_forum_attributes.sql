IF EXISTS (SELECT * FROM [dbo].[ForumAttributes] WHERE [Id] = @Id) 
UPDATE [dbo].[ForumAttributes]
   SET [Users] = @Users
      ,[Views] = @Views
      ,[Replies] = @Replies
	  ,[Answered] = @Answered
      ,[Timestamp] = @Timestamp
 WHERE [Id] = @Id
ELSE
INSERT INTO [dbo].[ForumAttributes]
           ([Id]
           ,[Users]
           ,[Views]
           ,[Replies]
		   ,[Answered]
           ,[Timestamp])
     VALUES
           (@Id
           ,@Users
           ,@Views
           ,@Replies
		   ,@Answered
           ,@Timestamp)