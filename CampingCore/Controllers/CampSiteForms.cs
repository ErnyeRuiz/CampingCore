namespace CampingCore.Controllers;

/// <summary>Campos de formulario para crear un sitio (multipart).</summary>
public class CreateCampSiteForm
{
    public string   Name              { get; set; } = string.Empty;
    public string?  Description       { get; set; }
    public decimal  Latitude          { get; set; }
    public decimal  Longitude         { get; set; }
    public decimal  PricePerNight     { get; set; }
    public bool     HasWater          { get; set; }
    public bool     HasElectricity    { get; set; }
    public int      IdProvincia       { get; set; }
    public int      IdCanton          { get; set; }
    public int      IdDistrito        { get; set; }
    public string?  DireccionExacta   { get; set; }
    /// <summary>Archivos nuevos (nombre de campo sugerido: <c>images</c>).</summary>
    public IFormFileCollection? Images { get; set; }
}

/// <summary>Formulario de actualización: mismos campos + ids de imágenes existentes a conservar.</summary>
public sealed class UpdateCampSiteForm : CreateCampSiteForm
{
    /// <summary>Ids de <c>CampSiteImage</c> que se mantienen; vacío o ausente = no conservar ninguna.</summary>
    public List<int>? ImageIdsToKeep { get; set; }
}
