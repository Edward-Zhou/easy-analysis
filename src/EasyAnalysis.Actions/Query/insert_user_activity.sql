INSERT INTO [dbo].[UserActivities]
           ([Hash]
           ,[UserId]
           ,[Action]
           ,[Time]
           ,[EffectOn]
		   ,[Timestamp])
     VALUES
           (@Hash
           ,@UserId
           ,@Action
           ,@Time
           ,@EffectOn
		   ,@Timestamp)