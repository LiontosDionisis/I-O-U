namespace Api.IOU.Exceptions;

public class ExpenseNotFoundException : Exception
{
    public ExpenseNotFoundException(string message) : base(message){}
}