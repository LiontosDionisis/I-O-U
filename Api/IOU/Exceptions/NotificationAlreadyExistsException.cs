namespace Api.IOU.Exceptions;

public class NotificationAlreadyExistsException : Exception
{
    public NotificationAlreadyExistsException(string message) : base(message){}
}