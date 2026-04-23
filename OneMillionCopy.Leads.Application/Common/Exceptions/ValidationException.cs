namespace OneMillionCopy.Leads.Application.Common.Exceptions;

public sealed class ValidationException : AppException
{
    public ValidationException(string message)
        : base(message, 400)
    {
    }
}
