using System;
namespace GroupPostsPollingService.Models
{
    public class GroupPost
    {
        public string? id { get; set; }
        public string? updated_time { get; set; }
        public string? message { get; set; }
        public string? story { get; set; }
        public IEnumerable<string>? tags { get; set; }
    }
}
