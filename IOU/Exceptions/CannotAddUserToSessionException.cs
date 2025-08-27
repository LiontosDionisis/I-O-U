namespace Api.IOU.Exceptions;

public class CannotAddUserToSessionException : Exception
{
    public CannotAddUserToSessionException(string message) : base(message){}
}