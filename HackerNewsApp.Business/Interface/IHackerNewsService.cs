using HackerNewsApp.BusinessEntities;

namespace HackerNewsApp.Business.Interface
{
    public interface IHackerNewsService
    {
        Task<List<HackerNewsStoryModel>> GetNewestStoriesAsync();
    }
}
