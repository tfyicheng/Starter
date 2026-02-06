using Starter.Infrastructure;
using Starter.Models;
using Starter.Services;
using Starter.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Starter.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ConfigService _configService;
        private readonly IconService _iconService;
        private readonly SettingsWindow setWindow = new SettingsWindow();

        public ObservableCollection<string> TestItems { get; } =
    new ObservableCollection<string>(
        Enumerable.Range(1, 30).Select(i => $"Item {i}"));

        #region Properties

        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                _isLocked = value;
                OnPropertyChanged("IsLocked");
            }

        }

        private double _windowCornerRadius = 12;
        public double WindowCornerRadius
        {
            get { return _windowCornerRadius; }
            set
            {
                _windowCornerRadius = value;
                OnPropertyChanged("WindowCornerRadius");
            }
        }

        private string _windowBackground = "#CC1E1E1E";
        public string WindowBackground
        {
            get { return _windowBackground; }
            set
            {

                _windowBackground = value;
                OnPropertyChanged("WindowBackground");
            }
        }

        private bool _enableBlur = true;
        public bool EnableBlur
        {
            get { return _enableBlur; }
            set
            {
                _enableBlur = value;
                OnPropertyChanged("EnableBlur");
            }
        }

        private int _blurRadius = 20;
        public int BlurRadius
        {
            get { return _blurRadius; }
            set { _blurRadius = value; OnPropertyChanged("BlurRadius"); }
        }

        private string _backgroundImagePath = "";
        public string BackgroundImagePath
        {
            get { return _backgroundImagePath; }
            set { _backgroundImagePath = value; OnPropertyChanged("BackgroundImagePath"); }
        }

        private bool _useBackgroundImage;
        public bool UseBackgroundImage
        {
            get { return _useBackgroundImage; }
            set { _useBackgroundImage = value; OnPropertyChanged("UseBackgroundImage"); }
        }

        private MenuGroup _selectedGroup;
        public MenuGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set { _selectedGroup = value; OnPropertyChanged("SelectedGroup"); }
        }

        public ObservableCollection<MenuGroup> MenuGroups { get; } = new ObservableCollection<MenuGroup>();

        #endregion

        #region Commands

        protected RelayCommand lockCommand;
        public RelayCommand LockCommand
        {
            get
            {
                if (lockCommand == null)
                {
                    lockCommand = new RelayCommand(param => this.Lock(), param => this.LockCanExecuted());
                }
                return lockCommand;
            }
        }


        /// <summary>
        /// 锁定/解锁
        /// </summary>
        public void Lock()
        {

            IsLocked = !IsLocked;
            Helper.Common.SetWindowToTopmost(IsLocked);
            //SaveCurrentConfig();
            Console.WriteLine("锁定/解锁：" + IsLocked);
        }

        public bool LockCanExecuted()
        {
            return true;
        }


        protected RelayCommand closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                {
                    closeCommand = new RelayCommand(param => this.Close(), param => this.CloseCanExecuted());
                }
                return closeCommand;
            }
        }


        public void Close()
        {
            Application.Current.MainWindow?.Hide();
            return;
            AppConfig config = _configService.GetConfig();
            if (config.ShowTrayIcon)
            {
                // 隐藏到托盘
                Application.Current.MainWindow?.Hide();
            }
            else
            {
                // 退出程序
                Application.Current.Shutdown();
            }
        }

        public bool CloseCanExecuted()
        {
            return true;
        }

        protected RelayCommand addGroupCommand;
        public RelayCommand AddGroupCommand
        {
            get
            {
                if (addGroupCommand == null)
                {
                    addGroupCommand = new RelayCommand(param => this.AddGroup(), param => this.AddGroupCanExecuted());
                }
                return addGroupCommand;
            }
        }

        public void AddGroup()
        {
            MenuGroup newGroup = new MenuGroup
            {
                Name = "分组" + (MenuGroups.Count + 1),
                Type = "Custom"
            };
            _configService.AddMenuGroup(newGroup);
            MenuGroups.Add(newGroup);
            SelectedGroup = newGroup;
        }

        public bool AddGroupCanExecuted()
        {
            return true;
        }



        protected RelayCommand setCommand;
        public RelayCommand SetCommand
        {
            get
            {
                if (setCommand == null)
                {
                    setCommand = new RelayCommand(param => this.Set(), param => this.SetCanExecuted());
                }
                return setCommand;
            }
        }



        public void Set()
        {
            setWindow.Show();
        }

        public bool SetCanExecuted()
        {
            return true;
        }



        #endregion


        public TabBarViewModel TabBar { get; } = new();

        public MainWindowViewModel() : base(null)
        {


            _configService = new ConfigService();
            _iconService = new IconService();


            // 加载配置
            LoadConfig();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadConfig()
        {
            AppConfig config = _configService.GetConfig();

            WindowCornerRadius = config.WindowCornerRadius;
            WindowBackground = config.WindowBackground;
            EnableBlur = config.EnableBlur;
            BlurRadius = config.BlurRadius;
            BackgroundImagePath = config.BackgroundImagePath;
            UseBackgroundImage = config.UseBackgroundImage;
            IsLocked = config.IsLocked;

            // 加载菜单组
            MenuGroups.Clear();
            foreach (MenuGroup group in config.MenuGroups)
            {
                MenuGroups.Add(group);
            }

            // 默认选中第一个
            if (MenuGroups.Count > 0 && SelectedGroup == null)
            {
                SelectedGroup = MenuGroups[0];
            }
            else if (MenuGroups.Count == 0)
            {
                // 添加默认分组
                AddDefaultGroup();
            }
        }

        /// <summary>
        /// 添加默认分组
        /// </summary>
        private void AddDefaultGroup()
        {
            MenuGroup defaultGroup = new MenuGroup
            {
                Name = "默认",
                Type = "Custom"
            };
            _configService.AddMenuGroup(defaultGroup);
            MenuGroups.Add(defaultGroup);
            SelectedGroup = defaultGroup;
        }



        /// <summary>
        /// 关闭（隐藏到托盘或退出）
        /// </summary>
        private void ExecuteClose()
        {
            Application.Current.MainWindow?.Hide();
            return;
            AppConfig config = _configService.GetConfig();
            if (config.ShowTrayIcon)
            {
                // 隐藏到托盘
                Application.Current.MainWindow?.Hide();
            }
            else
            {
                // 退出程序
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 保存当前配置
        /// </summary>
        private void SaveCurrentConfig()
        {
            AppConfig config = _configService.GetConfig();
            config.WindowCornerRadius = WindowCornerRadius;
            config.WindowBackground = WindowBackground;
            config.EnableBlur = EnableBlur;
            config.BlurRadius = BlurRadius;
            config.BackgroundImagePath = BackgroundImagePath;
            config.UseBackgroundImage = UseBackgroundImage;
            config.IsLocked = IsLocked;
            _configService.UpdateConfig(config);
        }
    }
}
