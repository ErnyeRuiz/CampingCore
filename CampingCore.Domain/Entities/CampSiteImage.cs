using CampingCore.Domain.Common;

namespace CampingCore.Domain.Entities;

public class CampSiteImage
{
    public const int MaxBase64Length = 12_000_000;

    public static class Errors
    {
        public static readonly Error InvalidCampSiteId    = Error.Validation("CampSiteImage.InvalidCampSiteId",    "El identificador del sitio de camping debe ser mayor a 0.");
        public static readonly Error ImageBase64Required  = Error.Validation("CampSiteImage.ImageBase64Required",  "La imagen en base64 es obligatoria.");
        public static readonly Error ImageBase64TooLong   = Error.Validation("CampSiteImage.ImageBase64TooLong",   "La imagen supera el tamaño máximo permitido.");
    }

    public int Id { get; private set; }
    public int CampSiteId { get; private set; }
    public string ImageBase64 { get; private set; } = string.Empty;

    public CampSite? CampSite { get; private set; }

    protected CampSiteImage() { }

    private CampSiteImage(int campSiteId, string imageBase64)
    {
        CampSiteId   = campSiteId;
        ImageBase64  = imageBase64;
    }

    public static Result<CampSiteImage> Create(int campSiteId, string imageBase64)
    {
        if (campSiteId <= 0)                           return Result.Failure<CampSiteImage>(Errors.InvalidCampSiteId);
        if (string.IsNullOrWhiteSpace(imageBase64))    return Result.Failure<CampSiteImage>(Errors.ImageBase64Required);
        if (imageBase64.Length > MaxBase64Length)      return Result.Failure<CampSiteImage>(Errors.ImageBase64TooLong);

        return new CampSiteImage(campSiteId, imageBase64);
    }
}
