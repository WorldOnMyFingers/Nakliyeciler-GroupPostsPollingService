using System;
namespace GroupPostsPollingService.Models
{
    public class FacebookGroupPostsDataModel
    {
        public IAsyncEnumerable<GroupPost>? data { get; set; }
    }
}

