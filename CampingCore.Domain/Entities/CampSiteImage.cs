using CampingCore.Domain.Common;
using CampingCore.Domain.Primitives;

namespace CampingCore.Domain.Entities;

public class CampSiteImage : Entity<int>
{
    public static class Errors
    {
        public static readonly Error InvalidCampSiteId = Error.Validation("CampSiteImage.InvalidCampSiteId", "El identificador del sitio de camping debe ser mayor a 0.");
        public static readonly Error ImageUrlRequired  = Error.Validation("CampSiteImage.ImageUrlRequired",  "La URL de la imagen es obligatoria.");
    }

    public int    CampSiteId { get; private set; }
    public string ImageUrl   { get; private set; } = string.Empty;

    public CampSite? CampSite { get; private set; }

    protected CampSiteImage() : base(0) { }

    private CampSiteImage(int campSiteId, string imageUrl) : base(0)
    {
        CampSiteId = campSiteId;
        ImageUrl   = imageUrl;
    }

    public static Result<CampSiteImage> CreateWithUrl(int campSiteId, string imageUrl)
    {
        if (campSiteId <= 0)                      return Result.Failure<CampSiteImage>(Errors.InvalidCampSiteId);
        if (string.IsNullOrWhiteSpace(imageUrl))  return Result.Failure<CampSiteImage>(Errors.ImageUrlRequired);

        return new CampSiteImage(campSiteId, imageUrl);
    }
}
