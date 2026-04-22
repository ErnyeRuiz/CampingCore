using CampingCore.Domain.Common;

namespace CampingCore.Domain.Exceptions;

public class DomainException : Exception
{
    public Error Error { get; }

    public DomainException(Error error) : base(error.Description)
    {
        Error = error;
    }
}
