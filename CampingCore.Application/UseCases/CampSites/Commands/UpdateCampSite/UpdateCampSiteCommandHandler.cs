using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Abstractions.Security;
using CampingCore.Application.Abstractions.Storage;
using CampingCore.Application.UseCases.CampSites.Commands.UpdateCampSite;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.CampSites.Commands.UpdateCampSite;

internal sealed class UpdateCampSiteCommandHandler : ICommandHandler<UpdateCampSiteCommand>
{
    private static readonly Error Forbidden =
        new("CampSite.Forbidden", "No tienes permiso para modificar este sitio de camping.");

    private readonly ICampSiteRepository _campSiteRepository;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly ICurrentUser        _currentUser;
    private readonly IR2StorageService   _r2;

    public UpdateCampSiteCommandHandler(
        ICampSiteRepository campSiteRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IR2StorageService r2)
    {
        _campSiteRepository = campSiteRepository;
        _unitOfWork         = unitOfWork;
        _currentUser        = currentUser;
        _r2                 = r2;
    }

    public async Task<Result> Handle(UpdateCampSiteCommand request, CancellationToken cancellationToken)
    {
        var campSite = await _campSiteRepository.GetByIdAsync(request.Id, cancellationToken);

        if (campSite is null)
            return Result.Failure(Error.NotFound(nameof(CampSite), request.Id));

        if (!_currentUser.IsSuperUser && campSite.CreatedByUserId != request.RequestingUserId)
            return Result.Failure(Forbidden);

        var updateResult = campSite.Update(
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.PricePerNight,
            request.HasWater,
            request.HasElectricity,
            request.IdProvincia,
            request.IdCanton,
            request.IdDistrito,
            request.DireccionExacta);

        if (updateResult.IsFailure)
            return updateResult;

        var imageUrls = new List<string>();
        foreach (var img in request.NewImages)
        {
            var (contentType, ext) = ResolveFormat(img.ContentType);
            var key = $"campsites/{campSite.Id}/{Guid.NewGuid():N}.{ext}";
            var url = await _r2.UploadAsync(img.Bytes, key, contentType, cancellationToken);
            imageUrls.Add(url);
        }

        var replace = campSite.ReplaceImages(request.ImageIdsToKeep, imageUrls);
        if (replace.IsFailure)
            return replace;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

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
