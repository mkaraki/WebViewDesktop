using System.Drawing;

namespace WebViewDesktop
{
    public class BrowserConfig
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public Rectangle Position { get; set; } = new Rectangle(30, 30, 300, 300);
    }
}
