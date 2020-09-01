using System.Threading.Tasks;

namespace Flipper.UI
{
    public class TestBook : PageFlipMessage
    {
        protected override int FirstPage => 1;
        protected override int LastPage => 20;
        
        protected override async Task<MessageContents> BuildPage()
        {
            await Task.Delay(2000);
            return new MessageContents(Page.ToString());
        }
    }
}