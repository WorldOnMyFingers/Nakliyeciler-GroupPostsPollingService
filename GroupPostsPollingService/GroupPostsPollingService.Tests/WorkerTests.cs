using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace GroupPostsPollingService.Tests
{
    public class WorkerTests
    {
        private readonly Mock<ILogger<Worker>> _logger;
        private readonly Mock<IFbGroupPostsClient> _fbGroupPostsClient;
        private Worker _worker;

        public WorkerTests()
        {
            _logger = new Mock<ILogger<Worker>>();
            _fbGroupPostsClient = new Mock<IFbGroupPostsClient>();
            _worker = new Worker(_logger.Object, _fbGroupPostsClient.Object);
        }

        [Fact]
        public void GetMatchingTagsTest_3IlceShouldReturn5Tags()
        {
            //arrange
            var message = "**Haymana yükleme**" +
                          "** Tavşanlı tamper tır**" +
                          "**05383339263 * *" +
                          "**Polatlı nak Lale Nak Ş**";

            //Act
            var result = _worker.GetMatchingTags(message);

            //assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void GetMatchingTagsTest_2IlceShouldReturn4Tags()
        {
            //arrange
            var message = "Sorgundan tarsusa 13.60 tir";

            //Act
            var result = _worker.GetMatchingTags(message);

            //assert
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void RemapInternationalCharToAsciiTest_InputıShouldReturni()
        {
            //arrange
            var c = 'ı';

            //act
            var result = _worker.RemapInternationalCharToAscii(c);

            //assert
            Assert.Equal('i', result);
        }

        [Fact]
        public void GetIlcelerTest()
        {
            //arrange
            

            //act
            var result = _worker.GetIlceler();

            //assert
            
        }
    }
}

