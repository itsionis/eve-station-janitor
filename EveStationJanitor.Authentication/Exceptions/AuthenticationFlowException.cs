namespace EveStationJanitor.Authentication.Exceptions;

public class AuthenticationFlowException : Exception
{
    public AuthenticationFlowException(string? message) : base(message)
    {
    }

    public AuthenticationFlowException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
