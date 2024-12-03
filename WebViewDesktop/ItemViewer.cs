using System;
using System.Drawing;
using System.Windows.Forms;

namespace WebViewDesktop
{
    public partial class ItemViewer : Form
    {
        public BrowserConfig BrowserConfig = new BrowserConfig();

        public Rectangle CurrentPosition;

        public ItemViewer()
        {
            InitializeComponent();
        }

        private void btn_apply_Click(object sender, EventArgs e)
        {
            BrowserConfig.Name = tbox_name.Text;
            BrowserConfig.Url = tbox_url.Text;
            BrowserConfig.Position = CurrentPosition;
            btn_cancel_Click(null, null);
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            CurrentPosition = new Rectangle(30, 30, 300, 300);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ItemViewer_Load(object sender, EventArgs e)
        {
            CurrentPosition = BrowserConfig.Position;
            tbox_name.Text = BrowserConfig.Name;
            tbox_url.Text = BrowserConfig.Url;
        }
    }
}
