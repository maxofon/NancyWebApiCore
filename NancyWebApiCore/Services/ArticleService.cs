using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NancyWebApiCore.Interfaces;
using NancyWebApiCore.Views;

namespace NancyWebApiCore.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpClientService _httpClientService;

        public ArticleService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<bool> IsServiceWorkingAsync()
        {
            var result = await _httpClientService.IsServiceWorkingAsync();
            return result;
        }

        public async Task<IEnumerable<ArticleView>> GetArticlesBySectionAsync(string section)
        {
            IEnumerable<ArticleView> articleViewsEnumerable = await GetArticleViewsAsync(section);
            var articleViews = articleViewsEnumerable.ToList();

            return articleViews;
        }

        public async Task<ArticleView> GetFirstArticleBySectionAsync(string section)
        {
            IEnumerable<ArticleView> articleViewsEnumerable = await GetArticleViewsAsync(section);
            var firstArticleView = articleViewsEnumerable.FirstOrDefault();

            return firstArticleView;
        }

        public async Task<IEnumerable<ArticleView>> GetArticlesBySectionAndDateAsync(string section, string stringDate)
        {
            var updatedDate = ParseDate(stringDate);
            IEnumerable<ArticleView> articleViewsEnumerable = await GetArticleViewsAsync(section);
            var articleViews = articleViewsEnumerable.Where(x => x.Updated.Date == updatedDate).ToList();

            return articleViews;
        }

        public async Task<ArticleView> GetArticlesByShortUrlAsync(string shortUrl)
        {
            CheckUrl(shortUrl);

            IEnumerable<ArticleView> articleViewsEnumerable = await GetArticleViewsAsync();
            var articleView = articleViewsEnumerable.FirstOrDefault(x => x.Link.EndsWith(shortUrl));

            return articleView;
        }

        public async Task<IEnumerable<ArticleGroupByDateView>> GetArticleGroupByDateViewsAsync(string section)
        {
            var articleGroupByDateViews = await GetGroupViewsAsync(section);

            return articleGroupByDateViews;
        }

        private async Task<IEnumerable<ArticleView>> GetArticleViewsAsync(string section = "")
        {
            var articles = await _httpClientService.GetArticlesAsync(section);

            var articleViews = articles.Select(x => new ArticleView
            {
                Heading = x.Title,
                Updated = x.UpdatedDate,
                Link = x.ShortUrl
            });

            return articleViews;
        }

        private async Task<IEnumerable<ArticleGroupByDateView>> GetGroupViewsAsync(string section = null)
        {
            var articles = await _httpClientService.GetArticlesAsync(section);

            var articleGroupByDateViews = from article in articles
                        group article by article.UpdatedDate.Date into g
                        select new ArticleGroupByDateView { Date = g.Key.ToString("yyyy-MM-dd"), Total = g.Count() };

            return articleGroupByDateViews;
        }

        private DateTime ParseDate(string inputDate)
        {
            var parsedDateTime = DateTime.Parse(inputDate);
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
