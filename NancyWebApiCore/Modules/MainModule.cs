using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Nancy;
using NancyWebApiCore.Entities;
using NancyWebApiCore.Helpers;
using NancyWebApiCore.Interfaces;
using NancyWebApiCore.Views;
using Newtonsoft.Json;

namespace NancyWebApiCore.Modules
{
	public class MainModule : NancyModule
    {
        private readonly IArticleService _articleService;
        public MainModule(IArticleService articleService)
        {
            _articleService = articleService;

            Get("/", async _ =>
            {
                try
                {
                    var result = await _articleService.IsServiceWorking();
                    var message = result ? "api is working :)" : "api is not working :(";
                    return JsonConvert.SerializeObject(new { result = message });
                }
                catch (Exception e)
                {
                    return JsonConvert.SerializeObject(new { error = e.Message });
                }
                
            });

            Get("/list/{section}", async args =>
            {
                try
                {
                    var articleViews = await _articleService.GetArticlesBySection(args.section);

                    return JsonConvert.SerializeObject(articleViews);
                }
                catch (Exception e)
                {
                    return JsonConvert.SerializeObject(new { error = e.Message });
                }
            });

            Get("/list/{section}/first", async args =>
            {
                try
                {
                    var firstArticleView = await _articleService.GetFirstArticleBySection(args.section);

                    return JsonConvert.SerializeObject(firstArticleView);

                }
                catch (Exception e)
                {
                    return JsonConvert.SerializeObject(new { error = e.Message });
                }
            });

            Get("/list/{section}/{updatedDate}", async args =>
            {
                try
                {
                    var articleViews = await _articleService.GetArticlesBySectionAndDate(args.section, args.updatedDate);
                    return JsonConvert.SerializeObject(articleViews);
                }
                catch (Exception e)
                {
                    return JsonConvert.SerializeObject(new { error = e.Message });
                }
            });

            Get("/article/{shortUrl}", async args =>
            {
                try
                {
                    var articleView = await _articleService.GetArticlesByShortUrl(args.shortUrl);

                    return JsonConvert.SerializeObject(articleView);
                }
                catch (Exception e)
                {
                    return JsonConvert.SerializeObject(new { error = e.Message });
                }
            });

            Get("/group/{section}", async args =>
            {
                try
                {
                    var articleGroupByDateViews = await _articleService.GetArticleGroupByDateViews(args.section);

                    return JsonConvert.SerializeObject(articleGroupByDateViews);
                }
                catch (Exception e)
                {
                    return JsonConvert.SerializeObject(new { error = e.Message });
                }

            });
        }
    }
}
