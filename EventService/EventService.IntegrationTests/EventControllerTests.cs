using EventService.Model;
using EventService.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EventService.IntegrationTests
{
    public class EventControllerTests : IClassFixture<IntegrationWebApplicationFactory<Program, AppDbContext>>
    {
        private readonly IntegrationWebApplicationFactory<Program, AppDbContext> _factory;
        private readonly HttpClient _client;

        public EventControllerTests(IntegrationWebApplicationFactory<Program, AppDbContext> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private static readonly string schemaName = "events";
        private static readonly string tableName = "Events";
        private static readonly Guid id1 = Guid.NewGuid();
        private static readonly DateTime timestamp1 = DateTime.MinValue;
        private static readonly string source = "source";
        private static readonly string requestType = "type";
        private static readonly string message = "message";
        private static readonly int statusCode = 1;
        private static readonly string statusCodeText = "status code text";
        private static readonly Guid id2 = Guid.NewGuid();
        private static readonly DateTime timestamp2 = DateTime.MaxValue;

        [Fact]
        public async Task GetEvents_EventList()
        {
            // Given
            Event event1 = new Event
            {
                Id = id1,
                Timestamp = timestamp1,
                Source = source,
                RequestType = requestType,
                Message = message,
                StatusCode = statusCode,
                StatusCodeText = statusCodeText
            };
            Event event2 = new Event
            {
                Id = id2,
                Timestamp = timestamp2,
                Source = source,
                RequestType = requestType,
                Message = message,
                StatusCode = statusCode,
                StatusCodeText = statusCodeText
            };
            _factory.Insert(schemaName, tableName, event1);
            _factory.Insert(schemaName, tableName, event2);

            // When
            var response = await _client.GetAsync("/api/event");

            // Then
            response.EnsureSuccessStatusCode();
            var responseContentString = await response.Content.ReadAsStringAsync();
            var responseContentObject = JsonConvert.DeserializeObject<List<Event>>(responseContentString);

            Assert.True(responseContentObject.Count == 2);
            var response1 = responseContentObject[0];
            Assert.Equal(event2.Id, response1.Id);
            Assert.Equal(event2.Timestamp, response1.Timestamp);
            Assert.Equal(event2.Source, response1.Source);
            Assert.Equal(event2.RequestType, response1.RequestType);
            Assert.Equal(event2.Message, response1.Message);
            Assert.Equal(event2.StatusCode, response1.StatusCode);
            Assert.Equal(event2.StatusCodeText, response1.StatusCodeText);
            var response2 = responseContentObject[1];
            Assert.Equal(event1.Id, response2.Id);
            Assert.Equal(event1.Timestamp, response2.Timestamp);
            Assert.Equal(event1.Source, response2.Source);
            Assert.Equal(event1.RequestType, response2.RequestType);
            Assert.Equal(event1.Message, response2.Message);
            Assert.Equal(event1.StatusCode, response2.StatusCode);
            Assert.Equal(event1.StatusCodeText, response2.StatusCodeText);

            Assert.Equal(2L, _factory.CountTableRows(schemaName, tableName));

            // Rollback
            _factory.DeleteById(schemaName, tableName, response1.Id);
            _factory.DeleteById(schemaName, tableName, response2.Id);
        }

    }
}
