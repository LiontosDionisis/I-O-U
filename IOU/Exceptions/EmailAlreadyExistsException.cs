namespace Api.IOU.Exceptions;

public class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException(string message) : base(message){}
}