using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Flipper.UI
{
    public class UIActionArgs
    {
        public DiscordChannel Channel { get; internal set; }
        public DiscordUser User { get; internal set; }
        public DiscordMessage Message { get; internal set; }
    }
    
    public static class UIActionArgsConversionExtension
    {
        public static UIActionArgs ToUIActionArgs(this MessageReactionAddEventArgs args)
        {
            return new UIActionArgs {Channel = args.Channel, User = args.User, Message = args.Message};
        }
        
        public static UIActionArgs ToUIActionArgs(this MessageReactionRemoveEventArgs args)
        {
            return new UIActionArgs {Channel = args.Channel, User = args.User, Message = args.Message};
        }
    }
}