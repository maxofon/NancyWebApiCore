using Nancy;
using Nancy.TinyIoc;
using NancyWebApiCore.Interfaces;
using NancyWebApiCore.Services;

namespace NancyWebApiCore.Helpers
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAppConfiguration appConfig;

        public Bootstrapper()
        {
        }

        public Bootstrapper(IAppConfiguration appConfig)
        {
            this.appConfig = appConfig;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IAppConfiguration>(appConfig);
            container.Register<IHttpClientService, HttpClientService>();
            container.Register<IArticleService, ArticleService>();
        }
    }
}
