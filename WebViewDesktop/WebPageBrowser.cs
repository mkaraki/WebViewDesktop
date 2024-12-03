using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WebViewDesktop
{
    public partial class WebPageBrowser : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out IntPtr lpdwResult);

        [Flags]
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public int Id { get; private set; }

        private readonly BrowserConfig _bc;

        public bool IsParentSet { get; private set; }

        public WebPageBrowser(int id, BrowserConfig bc)
        {
            InitializeComponent();

            // FormBorderStyle = FormBorderStyle.Sizable;

            Id = id;
            webView.Source = new Uri(bc.Url);
            Text = $"{bc.Name} - WebViewDesktop";
            Location = bc.Position.Location;
            Size = bc.Position.Size;
            _bc = bc;
        }

        private void WebPageBrowser_Shown(object sender, EventArgs e)
        {
            
        }

        private void WebPageBrowser_Load(object sender, EventArgs e)
        {
            IntPtr progmanWindowHandle = FindWindow("Progman", null);

            IntPtr res = IntPtr.Zero;

            SendMessageTimeout(progmanWindowHandle, 0x052C, IntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out res);

            IntPtr workerW = IntPtr.Zero;

            EnumWindows(new EnumWindowsProc((hndl, _) =>
            {
                IntPtr wHnd = FindWindowEx(hndl, IntPtr.Zero, "SHELLDLL_DefView", null);

                if (wHnd != IntPtr.Zero)
                {
                    workerW = wHnd;
                    //workerW = FindWindowEx(IntPtr.Zero, hndl, "WorkerW", null);
                }

                return true;
            }), IntPtr.Zero);

            SetParent(this.Handle, workerW);

            // Due to change parent, Location config will reset.
            // So, set location in here.
            Location = _bc.Position.Location;

            IsParentSet = true;
        }

        private void webView_Click(object sender, EventArgs e)
        {

        }
    }
}
