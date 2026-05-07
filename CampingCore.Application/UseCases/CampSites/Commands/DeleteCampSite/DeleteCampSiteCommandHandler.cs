using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Abstractions.Security;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.CampSites.Commands.DeleteCampSite;

internal sealed class DeleteCampSiteCommandHandler : ICommandHandler<DeleteCampSiteCommand>
{
    private static readonly Error Forbidden =
        new("CampSite.Forbidden", "No tienes permiso para eliminar este sitio de camping.");

    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly ICurrentUser        _currentUser;

    public DeleteCampSiteCommandHandler(
        ICampSiteRepository campSiteRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _campSiteRepository = campSiteRepository;
        _unitOfWork         = unitOfWork;
        _currentUser        = currentUser;
    }

    public async Task<Result> Handle(DeleteCampSiteCommand request, CancellationToken cancellationToken)
    {
        var campSite = await _campSiteRepository.GetByIdAsync(request.Id, cancellationToken);

        if (campSite is null)
            return Result.Failure(Error.NotFound(nameof(CampSite), request.Id));

        if (!_currentUser.IsSuperUser && campSite.CreatedByUserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        _campSiteRepository.Remove(campSite);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
