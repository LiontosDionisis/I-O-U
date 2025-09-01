namespace Api.IOU.Exceptions;

public class NotificationDoesNotExistException : Exception
{
    public NotificationDoesNotExistException(string message) : base(message){}
}