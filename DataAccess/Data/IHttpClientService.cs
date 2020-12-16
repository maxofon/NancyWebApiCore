using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace DataAccess.Data
{
    public interface IHttpClientService
    {
        /// <summary>
        ///     Check if the api service is working
        /// </summary>
        /// <returns></returns>
        Task<bool> IsServiceWorkingAsync();

        /// <summary>
        ///     Get enumerations of DataAccess.Entities.Articles by section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        Task<IEnumerable<Article>> GetArticlesAsync(string section = "");
    }
}