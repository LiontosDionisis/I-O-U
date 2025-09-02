namespace Api.IOU.Exceptions;

public class NoNotificationsException : Exception
{
    public NoNotificationsException(string message) : base(message){}
}