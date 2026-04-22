using CampingCore.Domain.Common;
using MediatR;

namespace CampingCore.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
