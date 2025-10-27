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

    INSERT INTO [State] ([JobId], [Name], [Reason], [Data], [CreatedAt])
    VALUES (@JobId, @Name, @Reason, @Data, @CreatedAt);

    DECLARE @StateId BIGINT;
    SET @StateId = CAST(SCOPE_IDENTITY() AS BIGINT);
    
    UPDATE [Job]
    SET [CurrentStateId] = @StateId
       ,[CurrentStateName] = @Name
    WHERE [Id] = @JobId;

    ";
}
