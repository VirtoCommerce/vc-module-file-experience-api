using System.IO;
using Newtonsoft.Json;

namespace VirtoCommerce.FileExperienceApi.Core.Models;

public class File
{
    public string Scope { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Url { get; set; }

    [JsonIgnore]
    public Stream Stream { get; set; }
}
