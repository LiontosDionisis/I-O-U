namespace Api.IOU.Exceptions;

public class FriendRequestNotFoundException : Exception
{
    public FriendRequestNotFoundException(string message) : base(message){}
}