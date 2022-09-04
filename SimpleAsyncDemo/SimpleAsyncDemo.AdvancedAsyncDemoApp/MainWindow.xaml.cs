using SimpleAsyncDemo.AdvancedAsyncDemoApp.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace SimpleAsyncDemo.AdvancedAsyncDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExecuteSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            List<WebsiteDataModel> results = Downloader.RunDownload();
            PrintResults(results);

            watch.Stop();
            var elapsedMiliseconds = watch.ElapsedMilliseconds;

            txtResult.Text += $"Total execution time: {elapsedMiliseconds}";
        }

        private void btnExecuteParallelForeachSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            List<WebsiteDataModel> results = Downloader.RunDownloadParallelForeachSync();
            PrintResults(results);

            watch.Stop();
            var elapsedMiliseconds = watch.ElapsedMilliseconds;

            txtResult.Text += $"Total execution time: {elapsedMiliseconds}";
        }

        private async void btnExecuteParallelForeachAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += Progress_ProgressChanged;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            List<WebsiteDataModel> results = await Downloader.RunDownloadParallelForeachAsync(progress);
            PrintResults(results);

            watch.Stop();
            var elapsedMiliseconds = watch.ElapsedMilliseconds;

            txtResult.Text += $"Total execution time: {elapsedMiliseconds}";
        }

        private async void btnExecuteAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += Progress_ProgressChanged;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                List<WebsiteDataModel> results = await Downloader.RunDownloadAsync(progress, cancellationTokenSource.Token);
                PrintResults(results);
            }
            catch (OperationCanceledException)
            {
                txtResult.Text += $"The async download was canceled.{Environment.NewLine}";
            }            

            watch.Stop();
            var elapsedMiliseconds = watch.ElapsedMilliseconds;

            txtResult.Text += $"Total execution time: {elapsedMiliseconds}";
        }

        private async void btnExecuteParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            List<WebsiteDataModel> results = await Downloader.RunDownloadParallelAsync();
            PrintResults(results);

            watch.Stop();
            var elapsedMiliseconds = watch.ElapsedMilliseconds;

            txtResult.Text += $"Total execution time: {elapsedMiliseconds}";
        }                   

        private void PrintResults(List<WebsiteDataModel> results)
        {
            txtResult.Text = String.Empty;
            foreach (WebsiteDataModel result in results)
            {
                txtResult.Text += $"{result.WebsiteUrl} downloaded: {result.WebsiteData.Length} characters long.{Environment.NewLine}";
            }
        }

        private void btnCancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        private void Progress_ProgressChanged(object? sender, ProgressReportModel e)
        {
            prbProgress.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }
    }
}
