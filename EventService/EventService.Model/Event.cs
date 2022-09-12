using System.ComponentModel.DataAnnotations.Schema;

namespace EventService.Model
{
    [Table("Events", Schema = "events")]
    public class Event
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; }
        public string RequestType { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string StatusCodeText { get; set; }
    }
}