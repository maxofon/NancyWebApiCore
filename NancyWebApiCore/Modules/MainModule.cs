using System;
using Nancy;
using NancyWebApiCore.Interfaces;
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
                    var result = await _articleService.IsServiceWorkingAsync();
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
                    var articleViews = await _articleService.GetArticlesBySectionAsync(args.section);

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
                    var firstArticleView = await _articleService.GetFirstArticleBySectionAsync(args.section);

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
                    var articleViews = await _articleService.GetArticlesBySectionAndDateAsync(args.section, args.updatedDate);
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
                    var articleView = await _articleService.GetArticlesByShortUrlAsync(args.shortUrl);

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
                    var articleGroupByDateViews = await _articleService.GetArticleGroupByDateViewsAsync(args.section);

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
