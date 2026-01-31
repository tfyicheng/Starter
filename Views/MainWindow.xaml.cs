using Starter.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Starter.Views
{
    /// <summary>
    /// 主窗口
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDragging = false;

        public MainWindow()
        {
            InitializeComponent();

            // 设置数据上下文
            DataContext = new MainWindowViewModel();

            // 初始位置 - 屏幕右下角
            this.Loaded += (s, e) =>
            {
                var desktopWorkingArea = SystemParameters.WorkArea;
                this.Left = desktopWorkingArea.Right - this.Width - 20;
                this.Top = desktopWorkingArea.Bottom - this.Height - 20;
            };

            // 处理外部点击隐藏
            this.Deactivated += MainWindow_Deactivated;
        }

        /// <summary>
        /// 鼠标按下拖动窗口
        /// </summary>
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // 双击锁定/解锁
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.LockCommand.Execute(null);
                }
            }
            else
            {
                _isDragging = true;
                DragMove();
            }
        }

        /// <summary>
        /// 鼠标松开
        /// </summary>
        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        /// <summary>
        /// Tab项点击
        /// </summary>
        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Models.MenuGroup group)
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.SelectedGroup = group;
                }
            }
        }

        /// <summary>
        /// 窗口失去焦点时隐藏（未锁定时）
        /// </summary>
        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            if (DataContext is MainWindowViewModel vm && !vm.IsLocked)
            {
                // 延迟隐藏，避免影响其他操作
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!IsMouseOver && !_isDragging)
                    {
                        Hide();
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        /// <summary>
        /// 窗口关闭时
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            var config = new Services.ConfigService().GetConfig();
            if (!config.ShowTrayIcon)
            {
                // 如果不显示托盘，则完全退出
                Application.Current.Shutdown();
            }
            else
            {
                // 否则隐藏到托盘
                e.Cancel = true;
                Hide();
            }

            base.OnClosing(e);
        }
    }
}
