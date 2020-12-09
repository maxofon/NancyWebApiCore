using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NancyWebApiCore.Entities;
using NancyWebApiCore.Helpers;
using NancyWebApiCore.Interfaces;
using NancyWebApiCore.Modules;
using NancyWebApiCore.Views;
using Newtonsoft.Json;

namespace NancyWebApiCore.Services
{
    public class ArticleService : IArticleService
    {
        private readonly HttpClient _httpClient;
        private readonly IAppConfiguration _config;

        public ArticleService(HttpClient httpClient, IAppConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<bool> IsServiceWorking()
        {
            var query = GetDefualtQuery();
            var response = await _httpClient.GetAsync(query);
            var result = response.IsSuccessStatusCode;
            return result;
        }

        public async Task<IEnumerable<ArticleView>> GetArticlesBySection(string section)
        {
            IEnumerable<ArticleView> articleViewsEnumerable = await GetArticles(section);
            var articleViews = articleViewsEnumerable.ToList();

            return articleViews;
        }

        public async Task<ArticleView> GetFirstArticleBySection(string section)
        {
            IEnumerable<ArticleView> articleViewsEnumerable = await GetArticles(section);
            var firstArticleView = articleViewsEnumerable.FirstOrDefault();

            return firstArticleView;
        }

        public async Task<IEnumerable<ArticleView>> GetArticlesBySectionAndDate(string section, string stringDate)
        {
            var updatedDate = ParseDate(stringDate);
            IEnumerable<ArticleView> articleViewsEnumerable = await GetArticles(section);
            var articleViews = articleViewsEnumerable.Where(x => x.Updated.Date == updatedDate).ToList();

            return articleViews;
        }

        public async Task<ArticleView> GetArticlesByShortUrl(string shortUrl)
        {
            CheckUrl(shortUrl);

            IEnumerable<ArticleView> articleViewsEnumerable = await GetArticles();
            var articleView = articleViewsEnumerable.FirstOrDefault(x => x.Link.EndsWith(shortUrl));

            return articleView;
        }

        public async Task<IEnumerable<ArticleGroupByDateView>> GetArticleGroupByDateViews(string section)
        {
            var query = GetQuery(section);
            var response = await _httpClient.GetAsync(query);
            IEnumerable<ArticleGroupByDateView> articleGroupByDateViewsEnumerable = await GetGroups(response);
            var articleGroupByDateViews = articleGroupByDateViewsEnumerable.ToList();

            return articleGroupByDateViews;
        }

        private string GetDefualtQuery()
        {
            return GetQuery(_config.NytSettings.DefaultSection);
        }

        private string GetQuery(string section)
        {
            return string.Format(_config.NytSettings.BaseUrl, section, _config.NytSettings.ApiKey);
        }

        private async Task<IEnumerable<ArticleView>> GetArticles(string section = null)
        {
            var query = section != null ? GetQuery(section) : GetDefualtQuery();

            var response = await _httpClient.GetAsync(query);

            var data = await response.Content.ReadAsStringAsync();
            var resultWrapper = JsonConvert.DeserializeObject<ResultWrapper>(data) as ResultWrapper;
            var result = resultWrapper.Articles.Select(x => new ArticleView
            {
                Heading = x.Title,
                Updated = x.UpdatedDate,
                Link = x.ShortUrl
            });

            return result;
        }

        private async Task<IEnumerable<ArticleGroupByDateView>> GetGroups(HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();
            var resultWrapper = JsonConvert.DeserializeObject<ResultWrapper>(data) as ResultWrapper;
            var result = from article in resultWrapper.Articles
                group article by article.UpdatedDate.Date into g
                select new ArticleGroupByDateView { Date = g.Key.ToString("yyyy-MM-dd"), Total = g.Count() };

            return result;
        }

        private DateTime ParseDate(string inputDate)
        {
            var parsedDateTime = (DateTime)DateTime.Parse(inputDate);
            return parsedDateTime.Date;
        }

        private void CheckUrl(string url)
        {
            if (url.Length != 7)
            {
                throw new Exception("Url should be in format XXXXXXX");
            }
        }
    }
}
