namespace EveStationJanitor.Authentication.Exceptions;

public class ValidateTokenException : Exception
{
    public ValidateTokenException()
    {
    }

    public ValidateTokenException(string? message) : base(message)
    {
    }

    public ValidateTokenException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
