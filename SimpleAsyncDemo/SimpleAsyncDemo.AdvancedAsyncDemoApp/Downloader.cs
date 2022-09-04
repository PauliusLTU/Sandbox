using SimpleAsyncDemo.AdvancedAsyncDemoApp.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleAsyncDemo.AdvancedAsyncDemoApp
{
    public class Downloader
    {
        private static List<string> PrepData()
        {
            List<string> output = new List<string>();

            output.Add("http://www.google.com");
            output.Add("http://www.f-1.lt");
            output.Add("http://www.f1.com");
            output.Add("http://www.delfi.lt");
            output.Add("http://www.lrytas.lt");
            output.Add("http://www.msdn.com");
            output.Add("http://www.nba.com");

            return output;
        }

        public static List<WebsiteDataModel> RunDownload()
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            foreach (string website in websites)
            {
                WebsiteDataModel results = DownloadWebsite(website);
                output.Add(results);
            }

            return output;
        }

        public static async Task<List<WebsiteDataModel>> RunDownloadAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();

            foreach (string website in websites)
            {
                WebsiteDataModel results = await DownloadWebsiteAsync(website);
                output.Add(results);

                if (cancellationToken.IsCancellationRequested)
                {
                    // Cleanup
                }

                cancellationToken.ThrowIfCancellationRequested();

                report.PercentageComplete = (output.Count * 100) / websites.Count;
                report.SitesDownloaded = output;
                progress.Report(report);
            }

            return output;
        }

        public static List<WebsiteDataModel> RunDownloadParallelForeachSync()
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            Parallel.ForEach<string>(websites, (website) =>
            {
                WebsiteDataModel results = DownloadWebsite(website);
                output.Add(results);
            });

            return output;
        }

        public static async Task<List<WebsiteDataModel>> RunDownloadParallelForeachAsync(IProgress<ProgressReportModel> progress)
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();

            await Task.Run(() =>
            {
                Parallel.ForEach<string>(websites, (website) =>
                {
                    WebsiteDataModel results = DownloadWebsite(website);
                    output.Add(results);

                    report.PercentageComplete = (output.Count * 100) / websites.Count;
                    report.SitesDownloaded = output;
                    progress.Report(report);
                });
            });

            return output;
        }

        public static async Task<List<WebsiteDataModel>> RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string website in websites)
            {
                tasks.Add(DownloadWebsiteAsync(website));
            }

            WebsiteDataModel[] results = await Task.WhenAll(tasks);

            return new List<WebsiteDataModel>(results);
        }

        private static WebsiteDataModel DownloadWebsite(string websiteUrl)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteUrl;
            output.WebsiteData = client.DownloadString(websiteUrl);

            return output;
        }

        private static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteUrl;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteUrl);

            return output;
        }
    }
}
