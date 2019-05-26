using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace functions_csharp_entityframeworkcore
{
    public class HttpTrigger
    {
        private readonly BloggingContext _context;
        public HttpTrigger(BloggingContext context)
        {
            _context = context;
        }

        [FunctionName("GetPosts")]
        public IActionResult GetPosts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "posts")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP GET/posts trigger function processed a request.");

            var postsArray = _context.Posts.OrderBy(p => p.Title).ToArray();
            return new OkObjectResult(postsArray);
        }

        [FunctionName("PostPost")]
        public async Task<IActionResult> PostPostAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blog/{blogId}/post")] HttpRequest req,
            int blogId,
            CancellationToken cts,
            ILogger log)
        {
            log.LogInformation("C# HTTP POST/post trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Post data = JsonConvert.DeserializeObject<Post>(requestBody);

            Post p = new Post
            {
                BlogId = blogId,
                Content = data.Content,
                Title = data.Title
            };
            var entity = await _context.Posts.AddAsync(p, cts);
            await _context.SaveChangesAsync(cts);
            return new OkObjectResult(JsonConvert.SerializeObject(entity.Entity));
        }

        [FunctionName("PostBlog")]
        public async Task<IActionResult> PostBlogAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blog")] HttpRequest req,
            CancellationToken cts,
            ILogger log)
        {
            log.LogInformation("C# HTTP POST/blog trigger function processed a request.");

            var entity = await _context.Blogs.AddAsync(new Blog(), cts);
            await _context.SaveChangesAsync(cts);
            return new OkObjectResult(JsonConvert.SerializeObject(entity.Entity));
        }
    }
}
