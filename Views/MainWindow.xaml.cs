using Starter.Helper;
using Starter.Infrastructure;
using Starter.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Starter.Views
{
    /// <summary>
    /// 主窗口
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SettingsManager _settings = new SettingsManager();

        private BackgroundSettings _bg;

        public MainWindow()
        {
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;

            InitializeComponent();
            RestoreWindowSize();
            LoadBackground();


            //this.Background = Brushes.Transparent;

            //this.Loaded += OnWindowLoaded;

            Loaded += MainWindow_Loaded;
            LocationChanged += MainWindow_LocationChanged;
            SizeChanged += MainWindow_SizeChanged;
            RootBorder.SizeChanged += RootBorder_SizeChanged;
            SettingsManager.BackgroundChanged += ReloadBackground;


            Loaded += (_, __) =>
            {
                SettingsManager.BackgroundChanged += OnBackgroundChanged;
            };

            Unloaded += (_, __) =>
            {
                SettingsManager.BackgroundChanged -= OnBackgroundChanged;
            };

            if (_settings.LoadGeneral().OpenTray)
            {
                Helper.Common.ic.ShowTray();
            }
        }

        private bool _initialized;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var ws = _settings.LoadWindow();

            Width = ws.Width;
            Height = ws.Height;
            Left = ws.Left;
            Top = ws.Top;

            _initialized = true;
        }


        private void ReloadBackground()
        {
            Dispatcher.Invoke(() =>
            {
                _bg = _settings.LoadBackground();
                ApplyWindowMode();
            });
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            RestoreWindowPlacement();

            _bg = _settings.LoadBackground();
            ApplyWindowMode();
        }


        private void ApplyWindowMode()
        {
            if (_bg.Blur)
            {
                ApplyBlurMode();
            }
            else
            {
                ApplyNormalMode();
            }
        }

        private void RootBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateClip();
        }


        private void OnBackgroundChanged()
        {
            Dispatcher.Invoke(() =>
            {
                //ApplyBackground();
                UpdateClip();
            });
        }


        private void UpdateClip()
        {
            var bg = _settings.LoadBackground();
            double radius = bg.CornerRadius;

            RootBorder.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, RootBorder.ActualWidth, RootBorder.ActualHeight),
                RadiusX = radius,
                RadiusY = radius
            };
        }


        private void ApplyBlurMode()
        {
            // 关闭 WPF 圆角体系
            RootBorder.CornerRadius = new CornerRadius(0);
            RootBorder.Background = Brushes.Transparent;

            BackgroundGrid.Background = Brushes.Transparent;

            // Window 自身负责 tint
            this.Background = CreateColorBrush(_bg.Color, _bg.Opacity);

            WindowBlurHelper.EnableBlur(this);
        }


        private void ApplyNormalMode()
        {
            // 禁用系统模糊（重启窗口才会完全生效，但逻辑上分离）
            RootBorder.CornerRadius = new CornerRadius(_bg.CornerRadius);

            this.Background = Brushes.Transparent;

            Brush bgBrush;

            if (_bg.Mode == "Image" && File.Exists(_bg.ImagePath))
            {
                bgBrush = CreateImageBrush(_bg.ImagePath);
            }
            else
            {
                bgBrush = CreateColorBrush(_bg.Color, _bg.Opacity);
            }

            RootBorder.Background = bgBrush;
        }

        private Brush CreateColorBrush(string colorText, double opacity)
        {
            var color = (Color)ColorConverter.ConvertFromString(colorText);
            return new SolidColorBrush(color) { Opacity = opacity };
        }


        private Brush CreateImageBrush(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();

            return new ImageBrush(bmp)
            {
                Stretch = Stretch.UniformToFill
            };
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1 && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void RestoreWindowPlacement()
        {
            var win = _settings.LoadWindow();

            Width = win.Width;
            Height = win.Height;

            if (win.Left >= 0 && win.Top >= 0)
            {
                Left = win.Left;
                Top = win.Top;
            }
        }


        private DispatcherTimer _saveTimer;

        private void EnsureSaveTimer()
        {
            if (_saveTimer != null) return;

            _saveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _saveTimer.Tick += (_, __) =>
            {
                _saveTimer.Stop();
                SaveWindowSettings();
            };
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_initialized) return;
            EnsureSaveTimer();
            _saveTimer.Stop();
            _saveTimer.Start();
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (!_initialized) return;
            EnsureSaveTimer();
            _saveTimer.Stop();
            _saveTimer.Start();
        }

        private void SaveWindowSettings()
        {
            _settings.SaveWindow(new WindowSetting
            {
                Width = Width,
                Height = Height,
                Left = Left,
                Top = Top
            });
        }



        //-------




        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 拖拽窗口
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


        private void SetWindowBackgroundImage(string imagePath)
        {
            try
            {
                // 创建 ImageBrush
                var imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

                // 设置拉伸模式
                imageBrush.Stretch = Stretch.UniformToFill; // 填充整个窗口，保持比例

                // 应用背景
                this.Background = imageBrush;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载背景图片失败: {ex.Message}");
            }
        }





        private void RestoreWindowSize()
        {
            var ini = new IniFile();

            Width = ini.ReadInt("Window", "Width", 480);
            Height = ini.ReadInt("Window", "Height", 360);
        }


        private void RootGrid_SizeChanged1(object sender, SizeChangedEventArgs e)
        {
            RootGrid.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, ActualWidth, ActualHeight),
                RadiusX = RootBorder.CornerRadius.TopLeft,
                RadiusY = RootBorder.CornerRadius.TopLeft
            };
        }



        private void LoadBackground()
        {
            var bg = _settings.LoadBackground();

            // 透明度
            RootBorder.Opacity = Math.Max(0, Math.Min(1, bg.Opacity));

            // 圆角
            RootBorder.CornerRadius = new CornerRadius(bg.CornerRadius);

            // 背景模式
            if (bg.Mode.Equals("Image", StringComparison.OrdinalIgnoreCase)
                && File.Exists(bg.ImagePath))
            {
                //RootBorder.Background = CreateImageBrush(bg.ImagePath);
                SetWindowBackgroundImage(bg.ImagePath);
            }
            else
            {
                SetWindowBackgroundImage(bg.ImagePath);
                //RootBorder.Background = CreateColorBrush(bg.Color);
            }
        }

        private void test(object sender, RoutedEventArgs e)
        {
            SettingsWindow s

                = new SettingsWindow();

            s.Show();
        }
    }
}
