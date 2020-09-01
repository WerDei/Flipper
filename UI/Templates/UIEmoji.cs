using DSharpPlus.Entities;

namespace Flipper.UI
{
    public static class UIEmoji
    {
        public static readonly DiscordEmoji Previous = DiscordEmoji.FromName(Flipper.Bot, ":arrow_backward:");
        public static readonly DiscordEmoji Next = DiscordEmoji.FromName(Flipper.Bot, ":arrow_forward:");
        public static readonly DiscordEmoji Random = DiscordEmoji.FromName(Flipper.Bot, ":twisted_rightwards_arrows:");
        public static readonly DiscordEmoji First = DiscordEmoji.FromName(Flipper.Bot, ":rewind:");
        public static readonly DiscordEmoji Last = DiscordEmoji.FromName(Flipper.Bot, ":fast_forward:");
        
        public static readonly DiscordEmoji Like = DiscordEmoji.FromName(Flipper.Bot, ":heart_decoration:");
    }
}