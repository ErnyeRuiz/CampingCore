using CampingCore.Application.CampSites;

namespace CampingCore.Controllers;

internal static class CampSiteImageFormMapper
{
    public static async Task<IReadOnlyList<ImageFileDto>> ToImageFileDtosAsync(
        IFormFileCollection? files,
        CancellationToken cancellationToken)
    {
        if (files is null || files.Count == 0)
            return Array.Empty<ImageFileDto>();

        var list = new List<ImageFileDto>();
        foreach (var file in files)
        {
            if (file.Length == 0)
                continue;

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            var base64 = Convert.ToBase64String(ms.ToArray());
            list.Add(new ImageFileDto(base64, file.ContentType, file.FileName));
        }

        return list;
    }
}
