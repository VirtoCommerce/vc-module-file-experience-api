using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.FileExperienceApiModule.Core;
using VirtoCommerce.FileExperienceApiModule.Core.Models;
using VirtoCommerce.FileExperienceApiModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApiModule.Web.Controllers
{
    [Route("api/file-upload")]
    public class FileUploadController : Controller
    {
        private static readonly Dictionary<string, FileEntity> InMemoryDatabase = new();

        [HttpPost]
        public async Task<ActionResult> UploadFile([FromForm] FileUploadModel model)
        {
            if (model?.Files == null || model.Files.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var entities = new List<FileEntity>();
            foreach (var file in model.Files)
            {
                var uniqueFileName = Path.GetRandomFileName();
                var filePath = Path.Combine(path, uniqueFileName);

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                entities.Add(new FileEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = file.FileName,
                    Size = file.Length,
                    MimeType = file.ContentType,
                    Scope = model.Scope,
                    Url = $"/uploads/{uniqueFileName}",
                });
            }

            InMemoryDatabase.AddRange(entities.Select(x => new KeyValuePair<string, FileEntity>(x.Id, x)));

            var result = entities.Select(x => new FileDescriptor
            {
                Id = x.Id,
                Name = x.Name,
                MimeType = x.MimeType,
                Size = x.Size,
            });

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFile([FromRoute] string id)
        {
            if (InMemoryDatabase.ContainsKey(id))
            {
                var file = InMemoryDatabase[id];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", Path.GetFileName(file.Url));
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                InMemoryDatabase.Remove(id);
            }
            return await Task.FromResult(Ok());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetFile([FromRoute] string id)
        {
            if (InMemoryDatabase.ContainsKey(id))
            {
                var file = InMemoryDatabase[id];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", Path.GetFileName(file.Url));
                if (System.IO.File.Exists(path))
                {
                    var memory = new MemoryStream();
                    await using var stream = new FileStream(path, FileMode.Open);
                    await stream.CopyToAsync(memory);
                    memory.Position = 0;
                    return File(memory, file.MimeType, file.Name);
                }
            }
            return await Task.FromResult(Ok());
        }

        [HttpGet("configuration/{scope}")]
        public async Task<ActionResult> Configuration([FromRoute] string scope)
        {
            return await Task.FromResult(Ok(new ScopeConfiguration
            {
                Scope = scope,
                MaxFileSize = 1024 * 1024 * 5,
            }));
        }
    }
}
