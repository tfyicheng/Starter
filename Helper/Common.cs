using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Starter.Helper
{
    public static class Common
    {

        public enum WindowVisualMode
        {
            Opaque,   // Color / Image
            Blur      // Acrylic / Gaussian
        }

        public static myIcon ic = new myIcon();


        public class myIcon
        {
            //任务栏图标
            System.Windows.Forms.NotifyIcon notifyIcon = null;

            public myIcon()
            {
                //创建图标
                this.notifyIcon = new System.Windows.Forms.NotifyIcon();

                //程序打开时任务栏会有小弹窗
                //this.notifyIcon.BalloonTipText = "running...";

                //鼠标放在图标上时显示的文字
                this.notifyIcon.Text = "Starter-快点助手V1.0🏃‍";

                //图标图片的位置，注意这里要用绝对路径
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("Starter.icon.ico"))
                {
                    if (stream != null)
                    {
                        this.notifyIcon.Icon = new Icon(stream);
                    }
                    else
                    {
                        // 资源名错误
                        throw new Exception("图标资源未找到。资源名应为：项目名.文件夹名.文件名.ico");
                    }
                }

                //显示图标
                //this.notifyIcon.Visible = true;

                // 添加鼠标点击事件：用于左键点击打开窗口
                this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseClick);

                //右键菜单--菜单项
                System.Windows.Forms.MenuItem restart = new System.Windows.Forms.MenuItem("重启");
                System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");


                restart.Click += new EventHandler(RestartExt);
                exit.Click += new EventHandler(CloseWindow);

                //关联托盘控件
                System.Windows.Forms.MenuItem[] children = new System.Windows.Forms.MenuItem[] { restart, exit };
                notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(children);

                //this.notifyIcon.ShowBalloonTip(2000);
            }

            public void SendNotify(String title, String content, int time = 100000)
            {
                this.notifyIcon.BalloonTipTitle = title;
                this.notifyIcon.BalloonTipText = content;
                //this.notifyIcon.Visible = true;
                this.notifyIcon.ShowBalloonTip(time);
            }

            public void ShowTray()
            {
                this.notifyIcon.Visible = true;
            }

            public void HideTray()
            {
                this.notifyIcon.Visible = false;
            }

            //退出菜单项对应的处理方式
            public void CloseWindow(object sender, EventArgs e)
            {
                //Dispose()函数能够解决程序退出后图标还在，要鼠标划一下才消失的问题
                this.notifyIcon.Dispose();
                //关闭整个程序
                System.Windows.Application.Current.Shutdown();
            }

            public void RestartExt(object sender, EventArgs e)
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                // 判断是否为左键单击
                if (e.Button == MouseButtons.Left)
                {
                    ShowMainWindow();
                }
            }

            private void ShowMainWindow()
            {
                // 确保在 UI 线程上执行
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var windows = System.Windows.Application.Current.Windows;

                    for (int i = 0; i < windows.Count; i++)
                    {
                        var win = windows[i];

                        if (win.GetType().Name == "MainWindow")
                        {
                            if (win.IsVisible)
                            {
                                Common.SetWindowTop("Starter.Views.MainWindow");
                                //win.Activate(); // 激活窗口（置于前台）
                            }
                            else
                            {
                                win.Show();     // 显示窗口
                            }
                        }
                    }
                });
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// 设置窗体前置
        /// </summary>
        /// <param name="windowname"></param>
        public static void SetWindowTop(string windowname)
        {
            try
            {
                foreach (var item in System.Windows.Application.Current.Windows)
                {
                    //Console.WriteLine(item.ToString());
                    if (item.ToString() == windowname)
                    {
                        //Console.WriteLine("设置主窗体");
                        System.Windows.Window w = (System.Windows.Window)item;
                        w.WindowState = System.Windows.WindowState.Normal;
                        Common.SetWindowToForegroundWithAttachThreadInput(w);
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// 设置窗体激活前置
        /// </summary>
        /// <param name="window"></param>
        public static void SetWindowToForegroundWithAttachThreadInput(System.Windows.Window window)
        {
            var interopHelper = new WindowInteropHelper(window);
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindow = GetForegroundWindow();
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);

            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);

            window.Show();
            window.Activate();
            // 去掉和其他线程的输入链接
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);
            // 用于踢掉其他的在上层的窗口
            window.Topmost = true;
            window.Topmost = false;
        }


        /// <summary>
        /// 设置窗体置顶
        /// </summary>
        /// <param name="window"></param>
        public static void SetWindowToTopmost(bool isTop, string windowname = "Starter.Views.MainWindow")
        {
            try
            {
                foreach (var item in System.Windows.Application.Current.Windows)
                {
                    //Console.WriteLine(item.ToString());
                    if (item.ToString() == windowname)
                    {
                        //Console.WriteLine("设置主窗体");
                        System.Windows.Window w = (System.Windows.Window)item;
                        w.WindowState = System.Windows.WindowState.Normal;
                        var interopHelper = new WindowInteropHelper(w);
                        var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
                        var currentForegroundWindow = GetForegroundWindow();
                        var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);

                        AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);

                        w.Show();
                        w.Activate();
                        // 去掉和其他线程的输入链接
                        AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);
                        // 用于踢掉其他的在上层的窗口
                        w.Topmost = isTop;
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }


        }




    }
}
