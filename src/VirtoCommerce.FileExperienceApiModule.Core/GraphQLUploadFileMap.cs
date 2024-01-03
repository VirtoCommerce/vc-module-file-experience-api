using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.FileExperienceApiModule.Core;

public class GraphQLUploadFileMap
{
    public List<object> Parts { get; set; }
    public IFormFile File { get; set; }
}
