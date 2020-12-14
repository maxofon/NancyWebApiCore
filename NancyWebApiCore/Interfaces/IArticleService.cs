using System.Collections.Generic;
using System.Threading.Tasks;
using NancyWebApiCore.Views;

namespace NancyWebApiCore.Interfaces
{
    public interface IArticleService
    {
        Task<bool> IsServiceWorkingAsync();
        Task<IEnumerable<ArticleView>> GetArticlesBySectionAsync(string section);
        Task<ArticleView> GetFirstArticleBySectionAsync(string section);
        Task<IEnumerable<ArticleView>> GetArticlesBySectionAndDateAsync(string section, string stringDate);
        Task<ArticleView> GetArticlesByShortUrlAsync(string shortUrl);
        Task<IEnumerable<ArticleGroupByDateView>> GetArticleGroupByDateViewsAsync(string section);
    }
}