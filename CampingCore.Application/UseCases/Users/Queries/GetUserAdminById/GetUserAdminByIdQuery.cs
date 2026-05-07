using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Users.Queries.GetUserAdminById;

public record GetUserAdminByIdQuery(int UserId) : IQuery<UserAdminDetailResponse>;
