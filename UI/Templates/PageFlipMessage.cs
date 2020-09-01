using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.VisualBasic.CompilerServices;

namespace Flipper.UI
{
    public abstract class PageFlipMessage : UIMessage
    {
        protected abstract int FirstPage { get; }
        protected abstract int LastPage { get; }
        protected virtual int StartingPage => FirstPage;

        private int _page;
        protected int Page => _page;
        private MessageContents currentPage;
        private async Task SetPage(int value)
        {
            if (value < FirstPage) value = FirstPage;
            if (value > LastPage) value = LastPage;

            if (Page == value) return; // Do nothing if the page didn't change
                
            _page = value;

            currentPage = await BuildPage();
            await ShowCurrentPage();
        }

        protected abstract Task<MessageContents> BuildPage();


        /// <summary>Create a new instance of the PageFlipMessage.</summary>
        public override async Task Create(DiscordChannel channel)
        {
            message = await Flipper.Bot.SendMessageAsync(channel, "Loading...");
            
            
            await SetPage(StartingPage);
            await AddReactionInterface();
            
            Console.WriteLine("New book created");
        }
        

        private async Task AddReactionInterface()
        {
            await AddReactionButton(UIEmoji.First, 
                async a => await SetPage(FirstPage));
            await AddReactionButton(UIEmoji.Previous, 
                async a => await SetPage(Page - 1));
            await AddReactionButton(UIEmoji.Random, 
                async a => await SetPage(Flipper.Random.Next(FirstPage, LastPage + 1)));
            await AddReactionButton(UIEmoji.Next, 
                async a => await SetPage(Page + 1));
            await AddReactionButton(UIEmoji.Last, 
                async a => await SetPage(LastPage));
            
            await AddReactionButton(UIEmoji.Like, 
                async a => await SaveLikedPage(a.User));
        }
        

        private async Task ShowCurrentPage()
        {
            await SetMessage(currentPage);
        }

        private async Task SaveLikedPage(DiscordUser user)
        {
            var member = await message.Channel.Guild.GetMemberAsync(user.Id);
            await member.SendMessageAsync(UIStrings.YouLikedAPage);
            await member.SendMessageAsync(currentPage.text, false, currentPage.embed);
        }
    }
}