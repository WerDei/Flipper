using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Flipper.UI
{
    public class XkcdComic : PageFlipMessage
    {
        private static readonly TimeSpan CacheUpdateInterval = new TimeSpan(1, 0, 0);
        private static Timer CacheUpdateTimer = new Timer(CacheUpdateInterval.TotalMilliseconds);
        
        private static int _lastPageCache;

        static XkcdComic()
        {
            CacheUpdateTimer.AutoReset = true;
            CacheUpdateTimer.Enabled = true;
            CacheUpdateTimer.Elapsed += UpdatePageCount;
            UpdatePageCount(null, null);
        }

        private static async void UpdatePageCount(Object source, ElapsedEventArgs e)
        {
            _lastPageCache = await GetPageCount();
            Console.WriteLine("Updating the xkcd page count cache");
        }

        private static async Task<int> GetPageCount()
        {
            var response = await Flipper.Http.GetStringAsync("https://xkcd.com/info.0.json");
            return JsonConvert.DeserializeObject<XkcdPageInfo>(response).num;
        }
        
        
        // ----- //

        protected override int FirstPage => 1;
        protected override int StartingPage => LastPage;
        
        protected override int LastPage => _lastPageCache;

        private DiscordEmbed currentPage;


        protected override async Task<MessageContents> BuildPage()
        {
            var pageInfo = await GetPageInfo(Page);
            
            currentPage = new DiscordEmbedBuilder
            {
                ImageUrl = pageInfo.img,
                Title = $"xkcd: {pageInfo.title}",
                Url = $"https://xkcd.com/{Page}/",
                Description = $"{Page}/{LastPage}",
                Color = DiscordColor.Azure,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"{pageInfo.alt}" +
                           $"{Environment.NewLine + Environment.NewLine}" +
                           $"{pageInfo.day}.{pageInfo.month}.{pageInfo.year}"
                }
            };

            return new MessageContents("", currentPage);
        }

        private async Task<XkcdPageInfo> GetPageInfo(int page)
        {
            var response = await Flipper.Http.GetStringAsync($"https://xkcd.com/{page}/info.0.json");
            return JsonConvert.DeserializeObject<XkcdPageInfo>(response);
        }
    }

    
    
    public struct XkcdPageInfo
    {
        public int num;
        public string img;
        public string title;
        public string safe_title;
        
        public string year;
        public string month;
        public string day;
        
        public string transcript;
        public string alt;
        
        public string link;
        public string news;
    }
}