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

    public Task<Result<int>> Handle(CreateCampSiteCommand request, CancellationToken cancellationToken) =>
        _unitOfWork.ExecuteTransactionalAsync(async () =>
        {
            var userExists = await _userRepository.GetByIdAsync(request.CreatedByUserId, cancellationToken);

            if (userExists is null)
                return Result.Failure<int>(Error.NotFound(nameof(User), request.CreatedByUserId));

            var siteResult = CampSite.Create(
                request.Name,
                request.Description,
                request.Latitude,
                request.Longitude,
                request.PricePerNight,
                request.HasWater,
                request.HasElectricity,
                request.CreatedByUserId,
                request.IdProvincia,
                request.IdCanton,
                request.IdDistrito,
                request.DireccionExacta);

            if (siteResult.IsFailure)
                return Result.Failure<int>(siteResult.Error);

            var campSite = siteResult.Value;
            _campSiteRepository.Add(campSite);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var newBase64 = request.NewImages.Select(i => i.Base64).ToList();
            var replace = campSite.ReplaceImages(imageIdsToKeep: null, newBase64);
            if (replace.IsFailure)
                return Result.Failure<int>(replace.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(campSite.Id);
        }, cancellationToken);
}
