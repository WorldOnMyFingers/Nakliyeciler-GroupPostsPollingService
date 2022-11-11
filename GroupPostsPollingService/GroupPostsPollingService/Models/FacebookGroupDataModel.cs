using System;
namespace GroupPostsPollingService.Models
{
    public class FacebookGroupPostsDataModel
    {
        public IEnumerable<GroupPost>? data { get; set; }
    }
}

