using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace ChAgent
{
    public partial class MainForm : Form
    {
        public static List<string> SelectedProcessList = new List<string>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void AddResult(string result)
        {
            if (this.listBox_result.Items.Count >= 100)
            {
                this.listBox_result.Items.RemoveAt(0);
            }
            this.listBox_result.Items.Add(result);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox_result.Items.Clear();

            foreach (var process in Process.GetProcesses().OrderBy(x => x.ProcessName))
            {
                if (string.IsNullOrEmpty(process.MainWindowTitle) == false)
                {
                    listBox_result.Items.Add(process.MainWindowTitle);
                }
            }

            //var i = 0;
            //foreach (var proc in Process.GetProcessesByName("LdBoxHeadless"))
            //{
            //    var filename = "Image" + i + ".png";
            //    var bitmap = PrintWindow(proc.MainWindowHandle);
            //    bitmap.Save(Application.StartupPath + "//" + filename);
            //    bitmap.Dispose();
            //    listBox1.Items.Add(filename + " saved");
            //    ++i;
            //}

            //Foo("강공주");
        }

        public Bitmap PrintWindow(IntPtr hwnd)
        {
            Rectangle rc = Rectangle.Empty;
            Graphics gfxWin = Graphics.FromHwnd(hwnd);
            rc = Rectangle.Round(gfxWin.VisibleClipBounds);

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfxBmp = Graphics.FromImage(bmp))
            {
                IntPtr hdcBitmap = gfxBmp.GetHdc();
                bool succeeded = WinApi.User32.PrintWindow(hwnd, hdcBitmap, 0x03);
                gfxBmp.ReleaseHdc(hdcBitmap);
                if (!succeeded)
                {
                    gfxBmp.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(Point.Empty, bmp.Size));
                }

                IntPtr hRgn = WinApi.Gdi32.CreateRectRgn(0, 0, 0, 0);
                WinApi.User32.GetWindowRgn(hwnd, hRgn);
                Region region = Region.FromHrgn(hRgn);
                if (!region.IsEmpty(gfxBmp))
                {
                    gfxBmp.ExcludeClip(region);
                    gfxBmp.Clear(Color.Transparent);
                }
            }

            return bmp;
        }

        private void button_select_Click(object sender, EventArgs e)
        {
            var processList = new ProcessList();

            if (processList.ShowDialog() == DialogResult.OK)
            {
                foreach (var processName in MainForm.SelectedProcessList)
                {
                    AddResult($"{processName} 선택되었습니다.");
                }
            }
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            foreach (var processName in MainForm.SelectedProcessList)
            {
                var yyyyMMddHHmmss = DateTime.Now.ToString("yyyyMMddHHmmss");
                PrintProcess(processName, $"//{processName}_{yyyyMMddHHmmss}.jpeg");
            }
        }

        private void PrintProcess(string processName, string filename)
        {
            IntPtr findwindow = WinApi.User32.FindWindow(null, processName);
            if (findwindow != IntPtr.Zero)
            {
                AddResult($"{processName} 프로세스를 찾았습니다.");

                Graphics Graphicsdata = Graphics.FromHwnd(findwindow);
                Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    WinApi.User32.PrintWindow(findwindow, hdc, 0x2);
                    g.ReleaseHdc(hdc);
                }

                bmp.Save(Application.StartupPath + filename);
            }
            else
            {
                AddResult($"{processName} 프로세스를 찾지 못했습니다.");
            }
        }

        private void timer_main_Tick(object sender, EventArgs e)
        {

        }
    }
}
