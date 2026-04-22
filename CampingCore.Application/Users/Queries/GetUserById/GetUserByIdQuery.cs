using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(int UserId) : IQuery<UserResponse>;
