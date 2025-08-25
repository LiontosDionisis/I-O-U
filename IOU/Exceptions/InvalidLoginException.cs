namespace Api.IOU.Exceptions;

public class InvalidLoginException : Exception
{
    public InvalidLoginException(string message) : base(message){}
}