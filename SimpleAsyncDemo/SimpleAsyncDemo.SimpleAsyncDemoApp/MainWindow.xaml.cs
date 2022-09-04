using SimpleAsyncDemo.SimpleAsyncDemoApp.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleAsyncDemo.SimpleAsyncDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExecuteSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownload();

            watch.Stop();
            var elapsedMiliseconds = watch.ElapsedMilliseconds;

            txtResult.Text += $"Total execution time: {elapsedMiliseconds}";
        }

        private async void btnExecuteAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadAsync();

            watch.Stop();
            var elapsedMiliseconds = watch.ElapsedMilliseconds;

            txtResult.Text += $"Total execution time: {elapsedMiliseconds}";
        }

        private async void btnExecuteParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMiliseconds = watch.ElapsedMilliseconds;

            txtResult.Text += $"Total execution time: {elapsedMiliseconds}";
        }

        private void RunDownload()
        {
            List<string> websites = PrepData();
            foreach (string website in websites)
            {
                WebsiteDataModel results = DownloadWebsite(website);
                ReportWebsiteInfo(results);
            }
        }

        private async Task RunDownloadAsync()
        {
            List<string> websites = PrepData();
            foreach (string website in websites)
            {
                WebsiteDataModel result = await Task.Run(() => DownloadWebsite(website));
                ReportWebsiteInfo(result);
            }
        }

        private async Task RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string website in websites)
            {
                tasks.Add(DownloadWebsiteAsync(website));
            }

            WebsiteDataModel[] results = await Task.WhenAll(tasks);

            foreach (WebsiteDataModel result in results)
            {
                ReportWebsiteInfo(result);
            }
        }

        private WebsiteDataModel DownloadWebsite(string websiteUrl)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteUrl;
            output.WebsiteData = client.DownloadString(websiteUrl);

            return output;
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteUrl;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteUrl);

            return output;
        }

        private List<string> PrepData()
        {
            List<string> output = new List<string>();

            txtResult.Text = String.Empty;

            output.Add("http://www.google.com");
            output.Add("http://www.f-1.lt");
            output.Add("http://www.f1.com");
            output.Add("http://www.delfi.lt");
            output.Add("http://www.lrytas.lt");
            output.Add("http://www.msdn.com");
            output.Add("http://www.nba.com");

            return output;
        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            txtResult.Text += $"{data.WebsiteUrl} downloaded: {data.WebsiteData.Length} characters long.{Environment.NewLine}";
        }
    }
}
