using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Abstractions.Storage;
using CampingCore.Application.UseCases.CampSites.Commands.CreateCampSite;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.CampSites.Commands.CreateCampSite;

internal sealed class CreateCampSiteCommandHandler : ICommandHandler<CreateCampSiteCommand, int>
{
    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUserRepository     _userRepository;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly IR2StorageService   _r2;

    public CreateCampSiteCommandHandler(
        ICampSiteRepository campSiteRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IR2StorageService r2)
    {
        _campSiteRepository = campSiteRepository;
        _userRepository     = userRepository;
        _unitOfWork         = unitOfWork;
        _r2                 = r2;
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

            var imageUrls = new List<string>();
            foreach (var img in request.NewImages)
            {
                var (contentType, ext) = ResolveFormat(img.ContentType);
                var key = $"campsites/{campSite.Id}/{Guid.NewGuid():N}.{ext}";
                var url = await _r2.UploadAsync(img.Bytes, key, contentType, cancellationToken);
                imageUrls.Add(url);
            }

            var replace = campSite.ReplaceImages(imageIdsToKeep: null, imageUrls);
            if (replace.IsFailure)
                return Result.Failure<int>(replace.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(campSite.Id);
        }, cancellationToken);

    private static (string contentType, string extension) ResolveFormat(string? contentType)
    {
        var mime = contentType?.Split(';')[0].Trim().ToLowerInvariant();
        return mime switch
        {
            "image/png"  => ("image/png",  "png"),
            "image/webp" => ("image/webp", "webp"),
            "image/gif"  => ("image/gif",  "gif"),
            _            => ("image/jpeg", "jpg"),
        };
    }
}
