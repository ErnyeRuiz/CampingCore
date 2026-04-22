using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.CampSites.Commands.CreateCampSite;

internal sealed class CreateCampSiteCommandHandler : ICommandHandler<CreateCampSiteCommand, int>
{
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCampSiteCommandHandler(
        ICampSiteRepository campSiteRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _campSiteRepository = campSiteRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateCampSiteCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.GetByIdAsync(request.CreatedByUserId, cancellationToken);

        if (userExists is null)
            return Result.Failure<int>(Error.NotFound(nameof(User), request.CreatedByUserId));

        var result = CampSite.Create(
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.PricePerNight,
            request.HasWater,
            request.HasElectricity,
            request.CreatedByUserId);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        _campSiteRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
