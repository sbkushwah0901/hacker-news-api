using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerNewsApp.BusinessEntities;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using HackerNewsApp.Repository.Interface;
using HackerNewsApp.Infrastructure;
using System.Net;

namespace HackerNewsApp.Repository
{
    public class HackerNewsRepository : IHackerNewsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public HackerNewsRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<IEnumerable<int>> GetNewStoryIdsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_configuration.GetSection("HackerNewsBaseUrl").Value}/newstories.json");
                return JsonSerializer.Deserialize<IEnumerable<int>>(response);
            }
            catch (Exception ex)
            {
                throw new CustomException("Failed to fetch new story IDs.", HttpStatusCode.ServiceUnavailable);
            }
        }

        public async Task<HackerNewsStoryModel> GetStoryAsync(int id)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };
                var response = await _httpClient.GetStringAsync($"{_configuration.GetSection("HackerNewsBaseUrl").Value}/item/{id}.json");
                return JsonSerializer.Deserialize<HackerNewsStoryModel>(response, options);
            }
            catch (Exception ex)
            {
                throw new CustomException($"Failed to fetch story with ID {id}.", HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
