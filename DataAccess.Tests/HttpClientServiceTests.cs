using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.Configs;
using DataAccess.Entities;
using Moq;
using Moq.Protected;
using NancyWebApiCore.Services;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DataAccess.Tests
{
    [TestFixture]
    public class HttpClientServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _config = new Mock<IAppConfiguration>();
            _config.Setup(x => x.NytSettings).Returns(new NytSettings
            {
                ApiKey = "apiKey",
                BaseUrl = "https://api.nytimes.com/svc/topstories/v2/{0}.json?api-key={1}",
                DefaultSection = "defaultSection"
            });

            _content =
                "{\"status\": \"OK\",\"section\": \"Arts\",\"num_results\": 34,\"results\": [{\"section\": \"arts\",\"title\": \"Donald Trump Lost His Battle. The Culture" +
                " War Goes On.\",\"abstract\": \"The reality-TV president was a practitioner, and a product, of a style of pop-cultural grievance that will outlast him.\",\"url\": " +
                "\"https://www.nytimes.com/2020/12/14/arts/television/donald-trump-culture-war.html\",\"uri\": \"nyt://article/d089ce37-2d76-5edf-84ec-a9588c64e1d0\",\"updated_date\": " +
                "\"2020-12-14T20:14:46-05:00\",\"short_url\": \"https://nyti.ms/34e1Zsy\"}]}";

            _expectedResult = new List<Article>
            {
                new Article
                {
                    Section = "arts",
                    Abstract =
                        "The reality-TV president was a practitioner, and a product, of a style of pop-cultural grievance that will outlast him.",
                    Title = "Donald Trump Lost His Battle. The Culture War Goes On.",
                    ShortUrl = "https://nyti.ms/34e1Zsy",
                    Uri = "nyt://article/d089ce37-2d76-5edf-84ec-a9588c64e1d0",
                    Url = "https://www.nytimes.com/2020/12/14/arts/television/donald-trump-culture-war.html",
                    UpdatedDate = DateTime.Parse("2020-12-14T20:14:46-05:00")
                }
            };
        }

        private string _content;
        private List<Article> _expectedResult;

        private Mock<IAppConfiguration> _config;

        private HttpClient MockHttpClient(HttpStatusCode code, string content, string reason = null)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = code,
                    Content = new StringContent(content),
                    ReasonPhrase = reason
                })
                .Verifiable();

            return new HttpClient(handlerMock.Object);
        }

        [Test]
        public async Task GetArticlesAsync_SectionNotPassed_ReturnArticleList()
        {
            // Arrange
            var httpClient = MockHttpClient(HttpStatusCode.OK, _content);
            var clientService = new HttpClientService(httpClient, _config.Object);

            // Act
            var result = await clientService.GetArticlesAsync();

            // Assert
            Assert.AreEqual(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(_expectedResult));
        }

        [Test]
        public async Task GetArticlesAsync_SectionPassed_ReturnArticleList()
        {
            // Arrange
            var httpClient = MockHttpClient(HttpStatusCode.OK, _content);
            var clientService = new HttpClientService(httpClient, _config.Object);

            // Act
            var result = await clientService.GetArticlesAsync("section");

            // Assert
            Assert.AreEqual(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(_expectedResult));
        }

        [Test]
        public async Task GetArticlesAsync_ServiceUnavailable_ThrowException()
        {
            // Arrange
            var httpClient = MockHttpClient(HttpStatusCode.ServiceUnavailable, _content);
            var clientService = new HttpClientService(httpClient, _config.Object);

            // Act

            // Assert
            Assert.ThrowsAsync<Exception>(async () => { await clientService.GetArticlesAsync(); });
        }

        [Test]
        public async Task IsServiceWorkingAsync_ClientAvailable_ReturnTrue()
        {
            // Arrange
            var httpClient = MockHttpClient(HttpStatusCode.OK, "");
            var clientService = new HttpClientService(httpClient, _config.Object);

            // Act
            var result = await clientService.IsServiceWorkingAsync();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsServiceWorkingAsync_ClientUnavailable_ReturnFalse()
        {
            // Arrange
            var httpClient = MockHttpClient(HttpStatusCode.ServiceUnavailable, "");
            var clientService = new HttpClientService(httpClient, _config.Object);

            // Act
            var result = await clientService.IsServiceWorkingAsync();

            // Assert
            Assert.That(result, Is.False);
        }
    }
}