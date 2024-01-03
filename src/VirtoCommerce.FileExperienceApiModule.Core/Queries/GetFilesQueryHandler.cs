using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.FileExperienceApiModule.Core.Queries
{
    public class GetFilesQueryHandler : IQueryHandler<GetFilesQuery, GetFilesResponse>
    {
        public Task<GetFilesResponse> Handle(GetFilesQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetFilesResponse
            {
                Files = new List<FileItem>
                {
                    new() { Id = "1", Name = "test", MimeType = "image/png", Url = "/assets/quotes/test.png", Size = 1024*1024*5},
                    new() { Id = "2", Name = "Рецепт", MimeType = "image/png", Url = "/assets/quotes/Рецепт.png", Size = 1024*1024*7 + 1024 * 500},
                    new() { Id = "3", Name = "very-long-filename-for-clear-experiment-to-check-ui-is-correct", MimeType = "image/png", Url = "/assets/quotes/very-long-filename-for-clear-experiment-to-check-ui-is-correct.png", Size = 1024*1024*2},
                    new() { Id = "4", Name = "1", MimeType = "image/png", Url = "http://example.test/assets/quotes/1.png", Size = 1024*1024*16},
                }
            });
        }
    }
}
