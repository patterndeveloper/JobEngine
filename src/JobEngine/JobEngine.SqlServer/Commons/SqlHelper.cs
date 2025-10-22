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
}
