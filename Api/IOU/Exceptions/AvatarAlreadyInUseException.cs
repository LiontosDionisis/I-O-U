namespace Api.IOU.Exceptions;

public class AvatarAlreadyInUseException : Exception
{
    public AvatarAlreadyInUseException(string message) : base(message){}
}