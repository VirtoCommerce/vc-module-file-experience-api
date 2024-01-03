using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.FileExperienceApiModule.Core.Queries
{
    public class GetFilesQuery : IQuery<GetFilesResponse>
    {
        public string Scope { get; set; }
        public string ObjectId { get; set; }
    }
}
