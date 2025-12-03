namespace JobEngine.Core.Exceptions;

public class JobClientException : Exception
{
    public JobClientException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
