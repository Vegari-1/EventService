using EventService.Model;
using EventService.Repository.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace EventService.UnitTests
{
    public class EventServiceTest
    {

        private static readonly Guid id1 = Guid.NewGuid();
        private static readonly DateTime timestamp1 = DateTime.MinValue;
        private static readonly string source = "source";
        private static readonly string requestType = "type";
        private static readonly string message = "message";
        private static readonly int statusCode = 1;
        private static readonly string statusCodeText = "status code text";
        private static readonly Guid id2 = Guid.NewGuid();
        private static readonly DateTime timestamp2 = DateTime.MaxValue;

        private static Event event1;
        private static Event event2;
        private static List<Event> events;

        private static Mock<IEventRepository> mockEventRepo = new Mock<IEventRepository>();
        private static Service.EventService eventService = new Service.EventService(mockEventRepo.Object);

        private static void SetUp()
        {
            event1 = new Event
            {
                Id = id1,
                Timestamp = timestamp1,
                Source = source,
                RequestType = requestType,
                Message = message,
                StatusCode = statusCode,
                StatusCodeText = statusCodeText
            };
            event2 = new Event
            {
                Id = id2,
                Timestamp = timestamp2,
                Source = source,
                RequestType = requestType,
                Message = message,
                StatusCode = statusCode,
                StatusCodeText = statusCodeText
            };
            events = new List<Event>
            {
                event1,
                event2
            };
        }

        [Fact]
        public async void GetEvents_EventList()
        {
            SetUp();

            mockEventRepo
               .Setup(repository => repository.GetAllSortByTimestamp())
               .ReturnsAsync(events);

            var response = await eventService.GetAllSortByTimestamp();

            Assert.IsType<List<Event>>(response);
            var responseList = response.ToList();
            Assert.True(responseList.Count == 2);
            var response1 = responseList[0];
            Assert.Equal(event1.Id, response1.Id);
            Assert.Equal(event1.Timestamp, response1.Timestamp);
            Assert.Equal(event1.Source, response1.Source);
            Assert.Equal(event1.RequestType, response1.RequestType);
            Assert.Equal(event1.Message, response1.Message);
            Assert.Equal(event1.StatusCode, response1.StatusCode);
            Assert.Equal(event1.StatusCodeText, response1.StatusCodeText);
            var response2 = responseList[1];
            Assert.Equal(event2.Id, response2.Id);
            Assert.Equal(event2.Timestamp, response2.Timestamp);
            Assert.Equal(event2.Source, response2.Source);
            Assert.Equal(event2.RequestType, response2.RequestType);
            Assert.Equal(event2.Message, response2.Message);
            Assert.Equal(event2.StatusCode, response2.StatusCode);
            Assert.Equal(event2.StatusCodeText, response2.StatusCodeText);
        }
    }
}