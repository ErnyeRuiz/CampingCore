namespace CampingCore.Application.Options;

public class AdminSettings
{
    public const string SectionName = "AdminSettings";

    public int ActivationDelayHours { get; set; } = 24;
}
