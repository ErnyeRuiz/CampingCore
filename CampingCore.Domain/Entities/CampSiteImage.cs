using CampingCore.Domain.Common;

namespace CampingCore.Domain.Entities;

public class CampSiteImage
{
    public static class Errors
    {
        public static readonly Error InvalidCampSiteId = Error.Validation("CampSiteImage.InvalidCampSiteId", "El identificador del sitio de camping debe ser mayor a 0.");
        public static readonly Error ImageUrlRequired  = Error.Validation("CampSiteImage.ImageUrlRequired",  "La URL de la imagen es obligatoria.");
        public static readonly Error ImageUrlTooLong   = Error.Validation("CampSiteImage.ImageUrlTooLong",   "La URL de la imagen no puede superar 2083 caracteres.");
    }

    public int Id { get; private set; }
    public int CampSiteId { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;

    public CampSite? CampSite { get; private set; }

    protected CampSiteImage() { }

    private CampSiteImage(int campSiteId, string imageUrl)
    {
        CampSiteId = campSiteId;
        ImageUrl = imageUrl;
    }

    public static Result<CampSiteImage> Create(int campSiteId, string imageUrl)
    {
        if (campSiteId <= 0)                        return Result.Failure<CampSiteImage>(Errors.InvalidCampSiteId);
        if (string.IsNullOrWhiteSpace(imageUrl))    return Result.Failure<CampSiteImage>(Errors.ImageUrlRequired);
        if (imageUrl.Length > 2083)                 return Result.Failure<CampSiteImage>(Errors.ImageUrlTooLong);

        return new CampSiteImage(campSiteId, imageUrl);
    }
}
