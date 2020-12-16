using System.Threading.Tasks;
using DataAccess.Configs;
using Moq;
using Nancy;
using Nancy.Testing;
using NancyWebApiCore.Helpers;
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
        private readonly string _apiKey = "k0XA0k0jJGAVuv8Jr5wAIcKDGPuznmRJ";
        private readonly string _baseUrl = "https://api.nytimes.com/svc/topstories/v2/{0}.json?api-key={1}";
        private readonly string _defaultSection = "home";

        [Test]
        public async Task GetHome_ApiKeyIsNotValid_ReturnServiceIsNotWorking()
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = "not_valid_key",
                    BaseUrl = _baseUrl,
                    DefaultSection = _defaultSection
                });

            //Act
            var response = await _browser.Get("/", with => { with.HttpRequest(); });

            //Assert
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
                    ApiKey = _apiKey,
                    BaseUrl = _baseUrl,
                    DefaultSection = _defaultSection
                });

            //Act
            var response = await _browser.Get("/", with => { with.HttpRequest(); });

            //Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            Assert.That(response.Body.AsString().Contains("is working"));
        }

        [Test]
        public async Task GetListSection_NotValidSectionPassed_ReturnJsonWithError()
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = _apiKey,
                    BaseUrl = _baseUrl,
                    DefaultSection = _defaultSection
                });
            var section = "invalid_section";

            //Act
            var response = await _browser.Get($"/list/{section}", with => { with.HttpRequest(); });

            var resultString = response.Body.AsString();
            var items = JsonConvert.DeserializeObject<ArticleView[]>(resultString,
                new JsonSerializerSettings {Error = (se, ev) => ev.ErrorContext.Handled = true});

            //Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            Assert.That(resultString.Contains("Section not found"));
            Assert.Null(items);
        }

        [Test]
        [TestCase("arts")]
        [TestCase("home")]
        [TestCase("health")]
        public async Task GetListSection_ValidSectionPassed_ReturnJson(string section)
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = _apiKey,
                    BaseUrl = _baseUrl,
                    DefaultSection = _defaultSection
                });

            //Act
            var response = await _browser.Get($"/list/{section}", with => { with.HttpRequest(); });

            var resultString = response.Body.AsString();
            var items = JsonConvert.DeserializeObject<ArticleView[]>(resultString);

            //Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            Assert.That(items.Length > 0, Is.True);
        }

        [Test]
        public async Task GetListSectionFirst_NotValidSectionPassed_ReturnJsonWithError()
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = _apiKey,
                    BaseUrl = _baseUrl,
                    DefaultSection = _defaultSection
                });
            var section = "invalid_section";

            //Act
            var response = await _browser.Get($"/list/{section}/first", with => { with.HttpRequest(); });
            var resultString = response.Body.AsString();

            //Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            Assert.That(resultString.Contains("Section not found"));
        }

        [Test]
        public async Task GetListSectionFirst_ValidSectionPassed_ReturnJsonWithError()
        {
            //Arrange
            _config.Setup(x => x.NytSettings)
                .Returns(new NytSettings
                {
                    ApiKey = _apiKey,
                    BaseUrl = _baseUrl,
                    DefaultSection = _defaultSection
                });
            var section = "health";

            //Act
            var response = await _browser.Get($"/list/{section}/first", with => { with.HttpRequest(); });
            var resultString = response.Body.AsString();
            var item = JsonConvert.DeserializeObject<ArticleView>(resultString,
                new JsonSerializerSettings {Error = (se, ev) => ev.ErrorContext.Handled = true});

            //Assert
            Assert.That(HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
            Assert.NotNull(item);
        }
    }
}