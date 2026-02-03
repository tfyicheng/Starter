using Starter.Infrastructure;
using Starter.Models;
using Starter.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Starter.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ConfigService _configService;
        private readonly IconService _iconService;

        #region Properties

        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set { OnPropertyChanged("IsLocked"); }

        }

        private double _windowCornerRadius = 12;
        public double WindowCornerRadius
        {
            get { return _windowCornerRadius; }
            set { OnPropertyChanged("WindowCornerRadius"); }
        }

        private string _windowBackground = "#CC1E1E1E";
        public string WindowBackground
        {
            get { return _windowBackground; }
            set { OnPropertyChanged("WindowBackground"); }
        }

        private bool _enableBlur = true;
        public bool EnableBlur
        {
            get { return _enableBlur; }
            set { OnPropertyChanged("EnableBlur"); }
        }

        private int _blurRadius = 20;
        public int BlurRadius
        {
            get { return _blurRadius; }
            set { OnPropertyChanged("BlurRadius"); }
        }

        private string _backgroundImagePath = "";
        public string BackgroundImagePath
        {
            get { return _backgroundImagePath; }
            set { OnPropertyChanged("BackgroundImagePath"); }
        }

        private bool _useBackgroundImage;
        public bool UseBackgroundImage
        {
            get { return _useBackgroundImage; }
            set { OnPropertyChanged("UseBackgroundImage"); }
        }

        private MenuGroup _selectedGroup;
        public MenuGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set { OnPropertyChanged("SelectedGroup"); }
        }

        public ObservableCollection<MenuGroup> MenuGroups { get; } = new ObservableCollection<MenuGroup>();

        #endregion

        #region Commands

        public ICommand LockCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand AddGroupCommand { get; private set; }

        #endregion

        public MainWindowViewModel() : base(null)
        {
            _configService = new ConfigService();
            _iconService = new IconService();

            // 初始化命令
            LockCommand = new RelayCommand(_ => ExecuteLock());
            CloseCommand = new RelayCommand(_ => ExecuteClose());
            AddGroupCommand = new RelayCommand(_ => ExecuteAddGroup());

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
        /// 锁定/解锁
        /// </summary>
        private void ExecuteLock()
        {
            IsLocked = !IsLocked;
            SaveCurrentConfig();
        }

        /// <summary>
        /// 关闭（隐藏到托盘或退出）
        /// </summary>
        private void ExecuteClose()
        {
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
        /// 添加分组
        /// </summary>
        private void ExecuteAddGroup()
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

    /// <summary>
    /// 简单RelayCommand实现
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
