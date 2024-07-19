using HackerNewsApp.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsApp.Repository.Interface
{
    public interface IHackerNewsRepository
    {
        Task<IEnumerable<int>> GetNewStoryIdsAsync();

        Task<HackerNewsStoryModel> GetStoryAsync(int id);
    }
}
