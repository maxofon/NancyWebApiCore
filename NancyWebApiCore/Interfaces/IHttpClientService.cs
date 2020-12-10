using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NancyWebApiCore.Entities;

namespace NancyWebApiCore.Interfaces
{
    public interface IHttpClientService
    {
        Task<bool> IsServiceWorkingAsync();
        Task<IEnumerable<Article>> GetArticlesAsync(string section = "");
    }
}
