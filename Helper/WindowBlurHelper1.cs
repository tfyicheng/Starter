using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Starter.Helper
{
    public static class WindowBlurHelper1
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute,
            ref int pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
            int x, int y, int width, int height, uint flags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_DLGMODALFRAME = 0x0001;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;

        [StructLayout(LayoutKind.Sequential)]
        private struct Margins
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        /// <summary>
        /// 启用 Windows 11 的 Mica 材质
        /// </summary>
        public static void EnableMica(Window window, bool darkTheme = false)
        {
            var hwnd = new WindowInteropHelper(window).Handle;

            // 确保窗口句柄已创建
            if (hwnd == IntPtr.Zero)
            {
                window.SourceInitialized += (s, e) => EnableMica(window, darkTheme);
                return;
            }

            try
            {
                // 移除窗口边框样式，使圆角生效
                int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_DLGMODALFRAME);
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);

                // 启用 Mica 背景
                int trueValue = 0x01;
                int result = DwmSetWindowAttribute(hwnd, 38 /* DWMWA_USE_HOSTBACKDROPBRUSH */,
                    ref trueValue, sizeof(int));

                if (result == 0) // 成功
                {
                    // 设置暗色/亮色主题
                    int useDarkMode = darkTheme ? 1 : 0;
                    DwmSetWindowAttribute(hwnd, 20 /* DWMWA_USE_IMMERSIVE_DARK_MODE */,
                        ref useDarkMode, sizeof(int));

                    // 扩展框架到客户区
                    var margins = new Margins()
                    {
                        leftWidth = -1,
                        rightWidth = -1,
                        topHeight = -1,
                        bottomHeight = -1
                    };
                    DwmExtendFrameIntoClientArea(hwnd, ref margins);
                }
                else
                {
                    // 如果 Mica 失败，回退到传统模糊
                    EnableAcrylicBlur(window);
                }
            }
            catch
            {
                // 如果系统不支持 Mica，回退到传统方法
                EnableAcrylicBlur(window);
            }
        }

        /// <summary>
        /// 传统的 Acrylic 模糊效果（Windows 10+）
        /// </summary>
        public static void EnableAcrylicBlur(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;

            if (hwnd == IntPtr.Zero)
            {
                window.SourceInitialized += (s, e) => EnableAcrylicBlur(window);
                return;
            }

            // 扩展框架到客户区以实现透明效果
            var margins = new Margins()
            {
                leftWidth = -1,
                rightWidth = -1,
                topHeight = -1,
                bottomHeight = -1
            };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }

        /// <summary>
        /// 检查系统是否支持 Mica 效果
        /// </summary>
        public static bool IsMicaSupported()
        {
            var osVersion = Environment.OSVersion.Version;
            // Windows 11 版本 22000 或更高
            return osVersion.Major >= 10 && osVersion.Build >= 22000;
        }

        /// <summary>
        /// 应用圆角到窗口
        /// </summary>
        public static void ApplyRoundCorners(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;

            if (hwnd == IntPtr.Zero)
            {
                window.SourceInitialized += (s, e) => ApplyRoundCorners(window);
                return;
            }

            try
            {
                // Windows 11 圆角窗口属性
                int attribute = 33; // DWMWA_WINDOW_CORNER_PREFERENCE
                int preference = 2; // DWMWCP_ROUND - 圆角

                // Windows 11 版本检查
                var osVersion = Environment.OSVersion.Version;
                if (osVersion.Major >= 10 && osVersion.Build >= 22000)
                {
                    DwmSetWindowAttribute(hwnd, (uint)attribute, ref preference, sizeof(int));
                }
            }
            catch
            {
                // 如果系统不支持圆角设置，使用 WPF 的 Border 来实现
                UseWpfRoundCorners(window);
            }
        }

        /// <summary>
        /// 使用 WPF Border 实现圆角（兼容性方案）
        /// </summary>
        private static void UseWpfRoundCorners(Window window)
        {
            // 这个方法会在 XAML 中实现，这里只是占位
        }
    }
}