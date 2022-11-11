using System;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GroupPostsPollingService.UnitTest
{
    
    public class WorkerTests
    {
        private readonly Mock<ILogger<Worker>> _logger;
        private Worker _worker;

        public WorkerTests()
        {
            _logger = new Mock<ILogger<Worker>>();
            _worker = new Worker(_logger.Object);
        }

        [Fact]
        public void GetMatchingTagsTest_2IlceShouldReturn4Tags()
        {
            //arrange
            var message = 

            //Act
            var result = _worker.GetMatchingTags()
        }
    }
}

