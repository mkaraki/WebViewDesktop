using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebViewDesktop
{
    public partial class MainConfigForm : Form
    {
        private List<BrowserConfig> browserConfigs = new List<BrowserConfig>();
        private Stack<WebPageBrowser> webPageBrowsers = new Stack<WebPageBrowser>();

        public MainConfigForm()
        {
            InitializeComponent();
            SetItemEditable(false);
        }

        private void MainConfigForm_Shown(object sender, EventArgs e)
        {
            if (Program.BootFromStartup)
                this.Hide();

            ReadBrowserConfig();
            OpenBrowserWindows();
            SetItemEditable(true);
        }

        private void CloseBrowserWindows()
        {
            while (webPageBrowsers.Count > 0)
            {
                var wpb = webPageBrowsers.Pop();
                wpb.Close();
                wpb.Dispose();
            }
        }

        private void OpenBrowserWindows()
        {
            if (webPageBrowsers.Count > 0) { CloseBrowserWindows(); }

            for (int i = 0; i < browserConfigs.Count; i++) 
            {
                var bc = browserConfigs[i];
                var wpb = new WebPageBrowser(i, bc);
                webPageBrowsers.Push(wpb);
                wpb.Show();
            }
        }

        private void SaveBrowserConfig()
        {
            SetItemEditable(false);
            var jsonString = JsonConvert.SerializeObject(browserConfigs);

            System.IO.File.WriteAllText("wpb_config.json", jsonString, Encoding.UTF8);
            SetItemEditable(true);
        }

        private void ReadBrowserConfig()
        {
            if (!System.IO.File.Exists("wpb_config.json"))
            {
                SaveBrowserConfig();
                return;
            }

            var jsonString = System.IO.File.ReadAllText("wpb_config.json", Encoding.UTF8);

            browserConfigs = JsonConvert.DeserializeObject<List<BrowserConfig>>(jsonString);
            UpdateList();
        }

        private void UpdateList()
        { 
            lview_browser_list.Items.Clear();
            foreach (var browser in browserConfigs)
            {
                var lviewItem = new ListViewItem();
                lviewItem.Text = browser.Name;
                lviewItem.SubItems.Add(browser.Url);

                lview_browser_list.Items.Add(lviewItem);
            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            using (var iv = new ItemViewer())
            {
                iv.ShowDialog();
                var browserConf = iv.BrowserConfig;
                browserConfigs.Add(browserConf);
            }

            UpdateList();
            OpenBrowserWindows();
            SaveBrowserConfig();
        }

        private int? GetSelectedItemIndexOrAlert()
        {
            var selected_item = lview_browser_list.SelectedItems;

            if (selected_item.Count == 0)
            {
                MessageBox.Show("Not selected");
                return null;
            }

            return lview_browser_list.SelectedItems[0].Index;
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            var item = GetSelectedItemIndexOrAlert();
            if (item == null) { return; }

            using (var iv = new ItemViewer())
            {
                iv.BrowserConfig = browserConfigs[item.Value];
                iv.ShowDialog();
            }

            UpdateList();
            OpenBrowserWindows();
            SaveBrowserConfig();
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            var item = GetSelectedItemIndexOrAlert();
            if (item == null) { return; }

            browserConfigs.RemoveAt(item.Value);

            UpdateList();
            OpenBrowserWindows();
            SaveBrowserConfig();
        }

        private bool _isEditMode = false;

        private void btn_edit_mode_Click(object sender, EventArgs e)
        {
            if (_isEditMode)
            {
                // To end edit mode
                _isEditMode = false;
                foreach (var item in webPageBrowsers)
                {
                    item.FormBorderStyle = FormBorderStyle.None;

                    browserConfigs[item.Id].Position = new Rectangle(item.Location, item.Size);
                }

                SaveBrowserConfig();
                SetItemEditable(true);
            }
            else /* !_isEditMode */
            {
                // To enable edit mode
                _isEditMode = true;
                foreach (var item in webPageBrowsers)
                {
                    item.FormBorderStyle = FormBorderStyle.Sizable;
                }
                SetItemEditable(false);
            }
        }

        private void SetItemEditable(bool editable = false)
        {
            lview_browser_list.Enabled = editable;
            btn_add.Enabled = editable;
            btn_remove.Enabled = editable;
            btn_edit.Enabled = editable;
        }

        private void btn_license_Click(object sender, EventArgs e)
        {
            using(var licenseWindow = new LicenseViewer())
            {
                licenseWindow.ShowDialog();
            }
        }

        private void MainConfigForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void MainConfigForm_Load(object sender, EventArgs e)
        {
            
        }

        private void btn_reload_Click(object sender, EventArgs e)
        {
            CloseBrowserWindows();
            OpenBrowserWindows();
        }
    }
}
