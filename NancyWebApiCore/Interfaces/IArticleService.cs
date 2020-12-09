using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NancyWebApiCore.Modules;
using NancyWebApiCore.Views;

namespace NancyWebApiCore.Interfaces
{
    public interface IArticleService
    {
        Task<bool> IsServiceWorking();
        Task<IEnumerable<ArticleView>> GetArticlesBySection(string section);
        Task<ArticleView> GetFirstArticleBySection(string section);
        Task<IEnumerable<ArticleView>> GetArticlesBySectionAndDate(string section, string stringDate);
        Task<ArticleView> GetArticlesByShortUrl(string shortUrl);
        Task<IEnumerable<ArticleGroupByDateView>> GetArticleGroupByDateViews(string section);
    }
}
