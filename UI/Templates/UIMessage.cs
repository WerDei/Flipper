using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Flipper.UI
{
    public abstract class UIMessage // TODO Delegate one thread to one message
    {
        protected delegate Task UIAction(UIActionArgs args);
        
        protected DiscordMessage message;

        public abstract Task Create(DiscordChannel channel);

        public virtual async Task Destroy()
        {
            await message.DeleteAsync();
        }

        protected async Task AddReactionButton(DiscordEmoji emoji, UIAction pressedAction)
        {
            await message.CreateReactionAsync(emoji);
            Flipper.Bot.MessageReactionAdded += async args =>
            {
                if (args.Message == message && args.Emoji == emoji && args.User != Flipper.Bot.CurrentUser)
                {
                    var press = pressedAction(args.ToUIActionArgs());
                    var delete = message.DeleteReactionAsync(emoji, args.User);
                    await Task.WhenAll(press, delete);
                }
            };
        }
        
        protected async void AddReactionToggle(DiscordEmoji emoji, UIAction activatedAction, UIAction deactivatedAction)
        {
            await message.CreateReactionAsync(emoji);
            Flipper.Bot.MessageReactionAdded += async args =>
            {
                if (args.Message == message && args.Emoji == emoji && args.User != Flipper.Bot.CurrentUser)
                    await  activatedAction(args.ToUIActionArgs());
            };

            Flipper.Bot.MessageReactionRemoved += async args =>
            {
                if (args.Message == message && args.Emoji == emoji && args.User != Flipper.Bot.CurrentUser)
                    await deactivatedAction(args.ToUIActionArgs());
            };
        }
        
        
        /// <summary>Change the message associated with this PageFlipMessage</summary>
        protected async Task SetMessage(MessageContents contents)
        {
            await message.ModifyAsync(contents.text, contents.embed);
        }
    }

    public struct MessageContents
    {
        public string text;
        public DiscordEmbed embed;

        public MessageContents(string text = null, DiscordEmbed embed = null)
        {
            this.text = text;
            this.embed = embed;
        }
    }
}