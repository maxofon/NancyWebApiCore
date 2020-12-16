using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DataAccess.Configs;
using DataAccess.Data;
using DataAccess.Entities;
using Newtonsoft.Json;

namespace NancyWebApiCore.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IAppConfiguration _config;
        private readonly HttpClient _httpClient;

        public HttpClientService(HttpClient httpClient, IAppConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<bool> IsServiceWorkingAsync()
        {
            var query = GetDefualtQuery();
            var response = await _httpClient.GetAsync(query);
            var result = response.IsSuccessStatusCode;
            return result;
        }

        public async Task<IEnumerable<Article>> GetArticlesAsync(string section = "")
        {
            var query = string.IsNullOrEmpty(section) ? GetDefualtQuery() : GetQuery(section);

            var response = await _httpClient.GetAsync(query);
            var data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var resultWrapper = JsonConvert.DeserializeObject<ResultWrapper>(data);
                return resultWrapper.Articles;
            }

            throw new Exception(data);
        }

        private string GetDefualtQuery()
        {
            return GetQuery(_config.NytSettings.DefaultSection);
        }

        private string GetQuery(string section)
        {
            return string.Format(_config.NytSettings.BaseUrl, section, _config.NytSettings.ApiKey);
        }
    }
}