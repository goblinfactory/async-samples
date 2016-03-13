using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSamples
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // let's time these! mmm, still need to do the timing, test my theory (if I read the notes correctly), that async should be a bit slower.
            var sw = new Stopwatch();
            sw.Start();
            AddIcons();
            sw.Stop();
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            await AddIconsAsync();
        }

        private void AddIcons()
        {
            var domains = new[] { "google.co.uk", "youtube.com", "Yahoo.com", "baudu.com", "amazon.co.uk", "wikipedia.org", "taobao.com", "MSN.com", "facebook.com", "twitter.com", "goblinfactory.co.uk", "reddit.com", "stackoverflow.com", "google.co.jp", "vk.com", "instagram.com", "ebay.com", "mail.ru", "360.cn", "pintrest.com" };

            foreach (var d in domains)
            {
                AddIcon(d, Stack1);
            }
        }

        private async Task AddIconsAsync()
        {
            var domains = new[] { "google.co.uk", "youtube.com", "Yahoo.com", "baudu.com", "amazon.co.uk", "wikipedia.org", "taobao.com", "MSN.com", "facebook.com", "twitter.com", "goblinfactory.co.uk", "reddit.com", "stackoverflow.com", "google.co.jp", "vk.com", "instagram.com", "ebay.com", "mail.ru", "360.cn", "pintrest.com" };

            foreach (var d in domains)
            {
                AddIcon(d, Stack2);
            }
        }

        public async Task AddIcon(string domain, StackPanel panel)
        {
            var wc = new WebClient();
            var t = wc.DownloadDataTaskAsync(new Uri("http://" + domain + "/favicon.ico"));
            var bytes = await t;
            var img = MakeImageControl(bytes);
            var imageControl = new Image() { Width = 30 };
            imageControl.Source = img;
            panel.Children.Add(imageControl);
        }

        private BitmapImage MakeImageControl(byte[] bytes)
        {
            
            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = ms;
                    img.EndInit();
                    return img;                    
                }
            }
            catch (Exception)
            {
                return new BitmapImage();
            }
        }

        private void ButtonDialog_OnClick(object sender, RoutedEventArgs e)
        {
            // ask ... using continue with...how do I 'catch' the exception?

            try
            {
                AskWindow.Ask(p => lblAsk.Content = p);

            }
            catch (Exception ex)
            {
                lblAsk.Content += " CAUGHT IT! (" + ex.Message + ") ";
            }

        }

        private async void ButtonAsync_OnClick(object sender, RoutedEventArgs e)
        {
            // ask ... using async and TaskCompletionSource
            // this is a contrived example, since continue with on face it things looks equally simple to follow.
            // ok, so now how do you handle exceptions, what if the code doing the asking throws an exception? before it issues the callback?

            try
            {
                lblAsk.Content = await AskWindow.AskWithTask();
            }
            catch (AskException ae)
            {
                lblAsk.Content = ae.Message;
            }
            
        }

        private  async void btUpdateProgress_Click(object sender, RoutedEventArgs e)
        {
            btUpdateProgress.IsEnabled = false;
            try
            {
                btCancelProgress.Visibility = Visibility.Visible;
                var ct = new CancellationTokenSource();
                btCancelProgress.Click += delegate { ct.Cancel(); };
                await UpdateProgressTask(new Progress<int>(i => { pbDemo.Value = i; }), ct.Token);
                btUpdateProgress.IsEnabled = true;
                pbDemo.Value = 0;
                return;
            }
            catch (Exception ex)
            {
                lblAsk.Content = ex.Message;
            }
            // should only get here if there was an exception
            await Task.Delay(1000);
            pbDemo.Value = 0;
            btUpdateProgress.IsEnabled = true;
            btCancelProgress.Visibility = Visibility.Hidden;

        }


        // caller creates cancellationTokenSource but this method takes a CancellationToken
        public async Task UpdateProgressTask(IProgress<int> update, CancellationToken ct)
        {

            await Task.Run(async() =>
            {
                for (int i = 1; i < 100; i+=1)
                {
                    ct.ThrowIfCancellationRequested();
                    update.Report(i+1);
                    await Task.Delay(100);

                    // throw an exception at > 50% ... what happens?
                    //if (i > 50) throw new Exception("Bwaaam!");
                }
                update.Report(100);
            });
        }

        private void BtCancelProgress_OnClick(object sender, RoutedEventArgs e)
        {
            btCancelProgress.Visibility = Visibility.Hidden;
        }
    }


    
}


