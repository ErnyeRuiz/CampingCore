using CampingCore.Domain.Common;

namespace CampingCore.Domain.Entities;

public class CampSite
{
    public static class Errors
    {
        public static readonly Error NameRequired           = Error.Validation("CampSite.NameRequired",           "El nombre es obligatorio.");
        public static readonly Error NameTooLong            = Error.Validation("CampSite.NameTooLong",            "El nombre no puede superar 100 caracteres.");
        public static readonly Error LatitudeOutOfRange     = Error.Validation("CampSite.LatitudeOutOfRange",     "La latitud debe estar entre -90 y 90.");
        public static readonly Error LongitudeOutOfRange    = Error.Validation("CampSite.LongitudeOutOfRange",    "La longitud debe estar entre -180 y 180.");
        public static readonly Error InvalidPricePerNight   = Error.Validation("CampSite.InvalidPricePerNight",   "El precio por noche debe ser mayor a 0.");
        public static readonly Error InvalidCreatedByUserId = Error.Validation("CampSite.InvalidCreatedByUserId", "El identificador del usuario creador debe ser mayor a 0.");
        public static readonly Error IdProvinciaRequired    = Error.Validation("CampSite.IdProvinciaRequired",    "El identificador de provincia debe ser mayor a 0.");
        public static readonly Error IdCantonRequired       = Error.Validation("CampSite.IdCantonRequired",       "El identificador de cantón debe ser mayor a 0.");
        public static readonly Error IdDistritoRequired     = Error.Validation("CampSite.IdDistritoRequired",     "El identificador de distrito debe ser mayor a 0.");
        public static readonly Error NotPersisted            = Error.Validation("CampSite.NotPersisted",            "El sitio debe estar persistido antes de gestionar imágenes.");
        public static readonly Error InvalidImageIdToKeep    = Error.Validation("CampSite.InvalidImageIdToKeep",    "Uno o más identificadores de imagen no pertenecen a este sitio.");
        public static readonly Error TooManyImages           = Error.Validation("CampSite.TooManyImages",           "Se superó el número máximo de imágenes permitidas.");
        public static readonly Error DuplicateImageIdsToKeep = Error.Validation("CampSite.DuplicateImageIdsToKeep", "La lista de imágenes a conservar contiene valores duplicados.");
    }

    public const int MaxImagesPerCampSite = 20;

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public decimal PricePerNight { get; private set; }
    public bool HasWater { get; private set; }
    public bool HasElectricity { get; private set; }
    public int CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int IdProvincia { get; private set; }
    public int IdCanton { get; private set; }
    public int IdDistrito { get; private set; }
    public string? DireccionExacta { get; private set; }

    public decimal Rating { get; private set; }

    public void UpdateRating(decimal rating) => Rating = rating;

    public User? CreatedByUser { get; private set; }
    public ICollection<CampSiteImage> Images { get; private set; } = new List<CampSiteImage>();
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
    public ICollection<Favorite> Favorites { get; private set; } = new List<Favorite>();
    public ICollection<TripCampSite> TripCampSites { get; private set; } = new List<TripCampSite>();

    protected CampSite() { }

    private CampSite(string name, string? description, decimal latitude, decimal longitude,
        decimal pricePerNight, bool hasWater, bool hasElectricity, int createdByUserId,
        int idProvincia, int idCanton, int idDistrito, string? direccionExacta)
    {
        Name            = name;
        Description     = description;
        Latitude        = latitude;
        Longitude       = longitude;
        PricePerNight   = pricePerNight;
        HasWater        = hasWater;
        HasElectricity  = hasElectricity;
        CreatedByUserId = createdByUserId;
        CreatedAt       = DateTime.UtcNow;
        IdProvincia     = idProvincia;
        IdCanton        = idCanton;
        IdDistrito      = idDistrito;
        DireccionExacta = direccionExacta;
    }

    public static Result<CampSite> Create(string name, string? description, decimal latitude, decimal longitude,
        decimal pricePerNight, bool hasWater, bool hasElectricity, int createdByUserId,
        int idProvincia, int idCanton, int idDistrito, string? direccionExacta)
    {
        if (string.IsNullOrWhiteSpace(name))     return Result.Failure<CampSite>(Errors.NameRequired);
        if (name.Length > 100)                   return Result.Failure<CampSite>(Errors.NameTooLong);
        if (latitude < -90 || latitude > 90)     return Result.Failure<CampSite>(Errors.LatitudeOutOfRange);
        if (longitude < -180 || longitude > 180) return Result.Failure<CampSite>(Errors.LongitudeOutOfRange);
        if (pricePerNight <= 0)                  return Result.Failure<CampSite>(Errors.InvalidPricePerNight);
        if (createdByUserId <= 0)                return Result.Failure<CampSite>(Errors.InvalidCreatedByUserId);
        if (idProvincia <= 0)                    return Result.Failure<CampSite>(Errors.IdProvinciaRequired);
        if (idCanton <= 0)                       return Result.Failure<CampSite>(Errors.IdCantonRequired);
        if (idDistrito <= 0)                     return Result.Failure<CampSite>(Errors.IdDistritoRequired);

        return new CampSite(name, description, latitude, longitude, pricePerNight, hasWater, hasElectricity,
            createdByUserId, idProvincia, idCanton, idDistrito, direccionExacta);
    }

    public Result Update(string name, string? description, decimal latitude, decimal longitude,
        decimal pricePerNight, bool hasWater, bool hasElectricity,
        int idProvincia, int idCanton, int idDistrito, string? direccionExacta)
    {
        if (string.IsNullOrWhiteSpace(name))     return Result.Failure(Errors.NameRequired);
        if (name.Length > 100)                   return Result.Failure(Errors.NameTooLong);
        if (latitude < -90 || latitude > 90)     return Result.Failure(Errors.LatitudeOutOfRange);
        if (longitude < -180 || longitude > 180) return Result.Failure(Errors.LongitudeOutOfRange);
        if (pricePerNight <= 0)                  return Result.Failure(Errors.InvalidPricePerNight);
        if (idProvincia <= 0)                    return Result.Failure(Errors.IdProvinciaRequired);
        if (idCanton <= 0)                       return Result.Failure(Errors.IdCantonRequired);
        if (idDistrito <= 0)                     return Result.Failure(Errors.IdDistritoRequired);

        Name            = name;
        Description     = description;
        Latitude        = latitude;
        Longitude       = longitude;
        PricePerNight   = pricePerNight;
        HasWater        = hasWater;
        HasElectricity  = hasElectricity;
        IdProvincia     = idProvincia;
        IdCanton        = idCanton;
        IdDistrito      = idDistrito;
        DireccionExacta = direccionExacta;

        return Result.Success();
    }

    /// <summary>
    /// Reemplaza el álbum de imágenes: conserva las indicadas en <paramref name="imageIdsToKeep"/>
    /// (vacío o <c>null</c> = no conservar ninguna existente) y añade <paramref name="newImageBase64Values"/>.
    /// Requiere <see cref="Id"/> mayor que cero (sitio ya persistido).
    /// </summary>
    public Result ReplaceImages(IReadOnlyCollection<int>? imageIdsToKeep, IReadOnlyList<string> newImageBase64Values)
    {
        if (Id <= 0)
            return Result.Failure(Errors.NotPersisted);

        var keep = imageIdsToKeep is null || imageIdsToKeep.Count == 0
            ? new HashSet<int>()
            : new HashSet<int>(imageIdsToKeep);

        if (imageIdsToKeep is not null && keep.Count != imageIdsToKeep.Count)
            return Result.Failure(Errors.DuplicateImageIdsToKeep);

        var existingIds = Images.Select(i => i.Id).ToHashSet();
        foreach (var id in keep)
        {
            if (!existingIds.Contains(id))
                return Result.Failure(Errors.InvalidImageIdToKeep);
        }

        if (keep.Count + newImageBase64Values.Count > MaxImagesPerCampSite)
            return Result.Failure(Errors.TooManyImages);

        var toRemove = Images.Where(i => !keep.Contains(i.Id)).ToList();
        foreach (var img in toRemove)
            Images.Remove(img);

        foreach (var b64 in newImageBase64Values)
        {
            var created = CampSiteImage.Create(Id, b64);
            if (created.IsFailure)
                return Result.Failure(created.Error);

            Images.Add(created.Value);
        }

        return Result.Success();
    }
}
