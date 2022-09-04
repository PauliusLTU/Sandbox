using System.Collections.Generic;

namespace SimpleAsyncDemo.AdvancedAsyncDemoApp.Models
{
    public class ProgressReportModel
    {
        public int PercentageComplete { get; set; } = 0;
        public List<WebsiteDataModel> SitesDownloaded { get; set; } = new List<WebsiteDataModel>();
    }
}
