using System.Collections.Generic;
using System.Threading.Tasks;
using NancyWebApiCore.Views;

namespace NancyWebApiCore.Services
{
    public interface IArticleService
    {
        /// <summary>
        ///     Check if the service is working
        /// </summary>
        /// <returns></returns>
        Task<bool> IsServiceWorkingAsync();

        /// <summary>
        ///     Get enumeration of NancyWebApiCore.Views.ArticleViews by section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        Task<IEnumerable<ArticleView>> GetArticlesBySectionAsync(string section);

        /// <summary>
        ///     Get first NancyWebApiCore.Views.ArticleView by section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        Task<ArticleView> GetFirstArticleBySectionAsync(string section);

        /// <summary>
        ///     Get enumeration of NancyWebApiCore.Views.ArticleViews by section and date
        /// </summary>
        /// <param name="section"></param>
        /// <param name="stringDate"></param>
        /// <returns></returns>
        Task<IEnumerable<ArticleView>> GetArticlesBySectionAndDateAsync(string section, string stringDate);

        /// <summary>
        ///     Get first or default NancyWebApiCore.Views.ArticleView by shortUrl
        /// </summary>
        /// <param name="shortUrl"></param>
        /// <returns></returns>
        Task<ArticleView> GetArticlesByShortUrlAsync(string shortUrl);

        /// <summary>
        ///     Get enumeration of NancyWebApiCore.Views.ArticleGroupByDateView by section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        Task<IEnumerable<ArticleGroupByDateView>> GetArticleGroupByDateViewsAsync(string section);
    }
}