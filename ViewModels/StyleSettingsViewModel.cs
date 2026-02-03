using Microsoft.Win32;
using Starter.Infrastructure;
using Starter.Models;
using System;

namespace Starter.ViewModels
{
    public class StyleSettingsViewModel : ViewModelBase
    {
        //private BackgroundSettings _bg;

        private readonly SettingsManager _settings = new SettingsManager();

        private readonly BackgroundSettings _bg;

        public StyleSettingsViewModel() : base(null)
        {
            _bg = _settings.LoadBackground();
        }

        /* ---------- 模式 ---------- */

        public string Mode
        {
            get => _bg.Mode;
            set
            {
                if (_bg.Mode == value) return;
                _bg.Mode = value;
                SaveAndNotify();
                OnPropertyChanged(nameof(IsImageMode));
            }
        }

        public bool IsImageMode => Mode == "Image";

        /* ---------- 颜色 / 图片 ---------- */

        public string Color
        {
            get => _bg.Color;
            set
            {
                if (_bg.Color == value) return;
                _bg.Color = value;
                SaveAndNotify();
            }
        }

        public string ImagePath
        {
            get => _bg.ImagePath;
            set
            {
                if (_bg.ImagePath == value) return;
                _bg.ImagePath = value;
                OnPropertyChanged("ImagePath");
                SaveAndNotify();
            }
        }

        /* ---------- 外观 ---------- */

        public double Opacity
        {
            get => _bg.Opacity;
            set
            {
                if (Math.Abs(_bg.Opacity - value) < 0.01) return;
                _bg.Opacity = value;
                SaveAndNotify();
            }
        }

        public double CornerRadius
        {
            get => _bg.CornerRadius;
            set
            {
                if (Math.Abs(_bg.CornerRadius - value) < 0.1) return;
                _bg.CornerRadius = value;
                SaveAndNotify();
            }
        }

        /* ---------- 高斯模糊 ---------- */

        public bool Blur
        {
            get => _bg.Blur;
            set
            {
                if (_bg.Blur == value) return;
                _bg.Blur = value;
                SaveAndNotify();

                OnPropertyChanged(nameof(IsBlurEnabled));
                OnPropertyChanged(nameof(IsNormalStyleEnabled));
                OnPropertyChanged(nameof(NeedRestartHint));
            }
        }

        public double BlurRadius
        {
            get => _bg.BlurRadius;
            set
            {
                if (Math.Abs(_bg.BlurRadius - value) < 0.1) return;
                _bg.BlurRadius = value;
                SaveAndNotify();
            }
        }

        /* ---------- 状态派生 ---------- */

        public bool IsBlurEnabled => Blur;

        // Blur=true 时，圆角 / 图片 / 颜色都禁用
        public bool IsNormalStyleEnabled => !Blur;

        // 用于 UI 提示
        public bool NeedRestartHint => Blur;

        /* ---------- 保存 ---------- */

        private void SaveAndNotify()
        {
            _settings.SaveBackground(_bg);
        }


        #region SelectImgCmd

        protected RelayCommand selectImgCmd;
        public RelayCommand SelectImgCmd
        {
            get
            {
                if (selectImgCmd == null)
                {
                    selectImgCmd = new RelayCommand(param => this.SelectImg(), param => this.SelectImgCanExecuted());
                }
                return selectImgCmd;
            }
        }

        public void SelectImg()
        {
            var dialog = new OpenFileDialog
            {
                Title = "选择文件",
                Filter = "图片文件 (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                CheckFileExists = true,
                Multiselect = false
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                ImagePath = dialog.FileName;
            }
        }

        public bool SelectImgCanExecuted()
        {
            return true;
        }
        #endregion
    }

}
