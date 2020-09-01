using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Flipper.UI;

namespace Flipper
{
    class Flipper
    {
        public static DiscordClient Bot { get; private set; }
        public static HttpClient Http {get; private set; }
        public static Random Random { get; private set; }
        public static string MentionSelf { get; private set; }

        private static List<UIMessage> books = new List<UIMessage>();

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        
        static async Task MainAsync(string[] launchArguments)
        {
            Console.WriteLine("Bot is starting...");
            // TODO message restoration to memory
            
            OfflineSetup();
            await Bot.ConnectAsync();
            OnlineSetup();
            
            Console.WriteLine("Start is successful");

            WaitForShutdown();
            
            Console.WriteLine("Shutting down the bot...");

            await BeforeShutdown();
            
            await Bot.DisconnectAsync();
            Bot.Dispose();
        }

        
        private static void OfflineSetup()
        {
            var token = System.IO.File.ReadAllText("token.txt");
            Bot = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });
            Http = new HttpClient();
            Random = new Random();

            
            Bot.MessageCreated += async args =>
            {
                if (! args.MentionedUsers.Contains(Bot.CurrentUser)) return;
                
                UIMessage message = null;
                Console.WriteLine(args.Message.Content);

                string command = args.Message.Content.Replace(MentionSelf, "").ToLower();
                
                Console.WriteLine($"Received command: {command}");
                
                if (command.Contains("test"))
                    message = new TestBook();
                if (command.Contains("xkcd"))
                    message = new XkcdComic();

                if (message != null)
                    await Task.Run( () => CreateUIMessage(message, args));
            }; // TODO bot reacts to mentions and not this crap
        }

        
        private static void OnlineSetup()
        {
            MentionSelf = Bot.CurrentUser.Mention;
        }


        private static void WaitForShutdown()
        {
            bool finished = false;
            while (!finished)
            {
                try
                {
                    finished = Console.ReadLine().ToLower().Equals("stop");
                }
                catch (Exception ignored)
                {
                    Console.WriteLine(ignored.StackTrace);
                }
            }
        }


        private static async Task BeforeShutdown()
        {
            var bookRemovals = new List<Task>();
            books.ForEach(b => bookRemovals.Add(b.Destroy()));
            await Task.WhenAll(bookRemovals);
        }


        private static async void CreateUIMessage(UIMessage message, MessageCreateEventArgs createCommand)
        {
            await message.Create(createCommand.Channel);
            books.Add(message);
            await createCommand.Message.DeleteAsync();
        }
    }
}