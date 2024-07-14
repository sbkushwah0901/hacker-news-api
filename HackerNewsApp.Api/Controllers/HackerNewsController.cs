using HackerNewsApp.Business.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;

        public HackerNewsController(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("newest")]
        public async Task<IActionResult> GetNewestStories()
        {
            var stories = await _hackerNewsService.GetNewestStoriesAsync();
            return Ok(stories);
        }
    }
}
