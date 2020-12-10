using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Nancy.Json;
using NancyWebApiCore.Entities;
using NancyWebApiCore.Interfaces;
using NancyWebApiCore.Services;
using NancyWebApiCore.Views;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NancyWebApiCore.Tests
{
    [TestFixture]
    public class ArticleServiceTests
    {
        private Mock<IHttpClientService> _clientService;
        private ArticleService _articleService;
        private readonly List<ArticleView> _articleViews;
        private readonly List<Article> _articles;

        public ArticleServiceTests()
        {
            var data = new List<(string, DateTime, string)>
            {
                ("Heading_1", DateTime.Today.AddDays(-1), "https://nyti.ms/short_1"),
                ("Heading_2", DateTime.Today.AddDays(-1), "https://nyti.ms/short_2"),
                ("Heading_3", DateTime.Today, "https://nyti.ms/short_3"),
                ("Heading_4", DateTime.Today.AddDays(1), "https://nyti.ms/short_4"),
                ("Heading_5", DateTime.Today.AddDays(1), "https://nyti.ms/short_5")
            };

            _articles = new List<Article>();
            _articleViews = new List<ArticleView>();
            foreach (var item in data)
            {
                _articles.Add(new Article{Title = item.Item1, UpdatedDate = item.Item2, ShortUrl = item.Item3});
                _articleViews.Add(new ArticleView{Heading = item.Item1, Updated = item.Item2, Link = item.Item3});
            }
        }

        [SetUp]
        public void Setup()
        {
            _clientService = new Mock<IHttpClientService>();
            _articleService = new ArticleService(_clientService.Object);
        }

        [Test]
        public async Task IsServiceWorkingAsync_ServerIsWorking_ReturnTrue()
        {
            //Arrange
            _clientService.Setup(x => x.IsServiceWorkingAsync()).ReturnsAsync(true);

            //Act
            var result = _articleService.IsServiceWorkingAsync().Result;

            // Assert
            _clientService.Verify(cs => cs.IsServiceWorkingAsync());
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsServiceWorkingAsync_ServerIsNotWorking_ReturnFalse()
        {
            //Arrange
            _clientService.Setup(x => x.IsServiceWorkingAsync()).ReturnsAsync(false);

            //Act
            var result = await _articleService.IsServiceWorkingAsync();

            // Assert
            _clientService.Verify(cs => cs.IsServiceWorkingAsync());
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetArticlesBySectionAsync_SectionPassed_ReturnArticleViews()
        {
            //Arrange
            var section = "arts";
            _clientService.Setup(x => x.GetArticlesAsync(section)).ReturnsAsync(_articles);

            //Act
            var result = await _articleService.GetArticlesBySectionAsync(section);

            // Assert
            _clientService.Verify(cs => cs.GetArticlesAsync(section));
            Assert.AreEqual(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(_articleViews));
        }

        [Test]
        public async Task GetFirstArticleBySectionAsync_SectionPassed_ReturnArticleView()
        {
            //Arrange
            var section = "health";
            _clientService.Setup(x => x.GetArticlesAsync(section)).ReturnsAsync(_articles);
            var firstArticleView = _articleViews.FirstOrDefault();

            //Act
            var result = await _articleService.GetFirstArticleBySectionAsync(section);

            // Assert
            _clientService.Verify(cs => cs.GetArticlesAsync(section));
            Assert.AreEqual(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(firstArticleView));
        }

        [Test]
        public async Task GetArticlesBySectionAndDateAsync_SectionAndDatePassed_ReturnArticleViews()
        {
            //Arrange
            var section = "us";
            _clientService.Setup(x => x.GetArticlesAsync(section)).ReturnsAsync(_articles);
            var updatedDate = DateTime.Today.AddDays(1);
            var updatedDateString = updatedDate.Date.ToString("yyyy-MM-dd");

            //Act
            var result = await _articleService.GetArticlesBySectionAndDateAsync(section, updatedDateString);

            // Assert
            _clientService.Verify(cs => cs.GetArticlesAsync(section));
            Assert.AreEqual(JsonConvert.SerializeObject(result), 
                JsonConvert.SerializeObject(_articleViews.Where(x => x.Updated == updatedDate)));
        }

        [Test]
        public async Task GetArticlesByShortUrlAsync_ShortUrlIsValid_ReturnArticleView()
        {
            //Arrange
            var shortUrl = "short_4";
            var section = "";
            _clientService.Setup(x => x.GetArticlesAsync(section)).ReturnsAsync(_articles);

            //Act
            var result = await _articleService.GetArticlesByShortUrlAsync(shortUrl);

            // Assert
            _clientService.Verify(cs => cs.GetArticlesAsync(section));
            Assert.AreEqual(JsonConvert.SerializeObject(result), 
                JsonConvert.SerializeObject(_articleViews.FirstOrDefault(x => x.Link.EndsWith(shortUrl))));
        }

        [Test]
        public async Task GetArticlesByShortUrlAsync_ShortUrlIsNotValid_ThrowException()
        {
            //Arrange
            var shortUrl = "short_4_";
            var section = "";
            _clientService.Setup(x => x.GetArticlesAsync(section)).ReturnsAsync(_articles);

            //Act

            // Assert
            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _articleService.GetArticlesByShortUrlAsync(section);
            });
        }

        [Test]
        public async Task GetArticleGroupByDateViewsAsync_SectionPassed_ReturnArticleGroupByDateViews()
        {
            //Arrange
            var section = "health";
            _clientService.Setup(x => x.GetArticlesAsync(section)).ReturnsAsync(_articles);
            var expectedResult = new List<ArticleGroupByDateView>
            {
                new ArticleGroupByDateView{Date = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"), Total = 2},
                new ArticleGroupByDateView{Date = DateTime.Today.ToString("yyyy-MM-dd"), Total = 1},
                new ArticleGroupByDateView{Date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"), Total = 2}
            };

            //Act
            var result = await _articleService.GetArticleGroupByDateViewsAsync(section);

            // Assert
            _clientService.Verify(cs => cs.GetArticlesAsync(section));
            Assert.AreEqual(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(expectedResult));
        }
    }
}
