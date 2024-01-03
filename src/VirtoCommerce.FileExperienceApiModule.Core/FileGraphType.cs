using GraphQL.Types;

namespace VirtoCommerce.FileExperienceApiModule.Core;

public class FileItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }
    public long Size { get; set; }
    public string Url { get; set; }
}

public class FileGraphType : ObjectGraphType<FileItem>
{
    public FileGraphType()
    {
        Field(f => f.Id).Name("id");
        Field(f => f.Name).Name("name");
        Field(f => f.MimeType).Name("mimetype");
        Field(f => f.Size).Name("size");
        Field(f => f.Url).Name("url");
    }
}
