using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Console1
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.CountPages();
            Console.WriteLine("finished. press enter");
            Console.ReadLine();
        }

        public void CountPages()
        {
            Console.WriteLine("count number of characters in web pages; press enter");
            Console.ReadLine();
            var pages = new[]
            {
                "https://en.wikipedia.org/wiki/Fruit_preserves" ,
                "https://en.wikipedia.org/wiki/Vegetable",
                "https://en.wikipedia.org/wiki/Sugar",
                "https://en.wikipedia.org/wiki/Canning",
                "https://en.wikipedia.org/wiki/Cucurbita",
                "https://en.wikipedia.org/wiki/Tomato",
                "https://en.wikipedia.org/wiki/Apricot",
                "https://en.wikipedia.org/wiki/Strawberry"
            };
            int total = 0;

            Task.WaitAll(pages.Select(p => CountPagesAsync(p).ContinueWith(cnt =>
            {
                var r = cnt.Result;
                Interlocked.Add(ref total, r.Item2);
                Console.WriteLine("{0} : {1} characters.", r.Item1, r.Item2);
            })).ToArray());

            Console.WriteLine("Total words = {0}", total);
        }

        public async Task<Tuple<string,int>> CountPagesAsync(string uri)
        {
            var wc = new WebClient();
            var html = await wc.DownloadStringTaskAsync(uri);
            return Tuple.Create(uri,html.Length);
        }
    }
}
