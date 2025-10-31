namespace JobEngine.SqlServer.Commons;

public static class SqlHelper
{
    public const string InsertJob = @"

    SET NOCOUNT ON;

    INSERT INTO [Job] (
        [InvocationData],
        [Arguments],
        [CreatedAt],
        [ExpireAt]
    ) VALUES (
        @InvocationData,
        @Arguments,
        @CreatedAt,
        @ExpireAt
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);"
    ;


    public const string InsertState = @"

    SET NOCOUNT ON;

    INSERT INTO [State] ([JobId], [Name], [Reason], [Data], [CreatedAt])
    VALUES (@JobId, @Name, @Reason, @Data, @CreatedAt);

    DECLARE @StateId BIGINT;
    SET @StateId = CAST(SCOPE_IDENTITY() AS BIGINT);
    
    UPDATE [Job]
    SET [CurrentStateId] = @StateId
       ,[CurrentStateName] = @Name
    WHERE [Id] = @JobId;

    ";


    public const string UpsertSet = @"

    MERGE [Set] WITH (XLOCK) AS [Target] USING (VALUES(@key, @value, @score)) AS [Source] ([Key], [Value], [Score])
    ON [Target].[Key] = [Source].[Key]
      AND [Target].[Value] = [Source].[Value]
    WHEN MATCHED
      THEN UPDATE
        SET [Score] = [Source].[Score]
    WHEN NOT MATCHED
      THEN INSERT ([Key],
        [Value],
        [Score])
          VALUES ([Source].[Key], [Source].[Value], [Source].[Score]);
    "
    ;


    public const string RemoveFromSet = @"

    DELETE [S] FROM [Set] AS [S] WITH (FORCESEEK)
    WHERE [Key] = @key
        AND [Value] = @value

    ";
}
