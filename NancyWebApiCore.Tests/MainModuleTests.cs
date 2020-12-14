using System.Threading.Tasks;
using Moq;
using Nancy;
using Nancy.Testing;
using NancyWebApiCore.Helpers;
using NancyWebApiCore.Interfaces;
using NancyWebApiCore.Views;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NancyWebApiCore.Tests
{
    [TestFixture]
    public class MainModuleTests
    {
        [SetUp]
        public void Setup()
        {
            _config = new Mock<IAppConfiguration>();


            var bootsrapper = new Bootstrapper(_config.Object);
            _browser = new Browser(bootsrapper);
        }

        private Browser _browser;
        private Mock<IAppConfiguration> _config;

        [Test]
        public async Task GetHome_ApiKeyIsNotValid_ReturnServiceIsNotWorking()
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = "not_valid_key",
                    BaseUrl = "https://api.nytimes.com/svc/topstories/v2/{0}.json?api-key={1}", DefaultSection = "home"
                });

            //Act
            var response = await _browser.Get("/", with => { with.HttpRequest(); });

            // Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            Assert.That(response.Body.AsString().Contains("is not working"));
        }

        [Test]
        public async Task GetHome_ApiKeyIsValid_ReturnServiceIsWorking()
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = "k0XA0k0jJGAVuv8Jr5wAIcKDGPuznmRJ",
                    BaseUrl = "https://api.nytimes.com/svc/topstories/v2/{0}.json?api-key={1}", DefaultSection = "home"
                });

            //Act
            var response = await _browser.Get("/", with => { with.HttpRequest(); });

            // Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            Assert.That(response.Body.AsString().Contains("is working"));
        }

        [Test]
        public async Task GetListSection_SectionPassed_ReturnJson()
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = "k0XA0k0jJGAVuv8Jr5wAIcKDGPuznmRJ",
                    BaseUrl = "https://api.nytimes.com/svc/topstories/v2/{0}.json?api-key={1}", DefaultSection = "home"
                });
            var section = "health";

            //Act
            var response = await _browser.Get($"/list/{section}", with => { with.HttpRequest(); });

            var resultString = response.Body.AsString();
            var items = JsonConvert.DeserializeObject<ArticleView[]>(resultString);

            // Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            Assert.That(items.Length > 0, Is.True);
        }

        [Test]
        public async Task GetListSectionFirst_SectionPassed_ReturnJson()
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = "k0XA0k0jJGAVuv8Jr5wAIcKDGPuznmRJ",
                    BaseUrl = "https://api.nytimes.com/svc/topstories/v2/{0}.json?api-key={1}", DefaultSection = "home"
                });
            var section = "health";

            //Act
            var response = await _browser.Get($"/list/{section}/first", with => { with.HttpRequest(); });

            var resultString = response.Body.AsString();
            var items = JsonConvert.DeserializeObject<ArticleView>(resultString);

            // Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
        }
    }
}