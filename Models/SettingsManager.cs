using Starter.Infrastructure;
using System;

namespace Starter.Models
{
    class SettingsManager
    {
        static readonly IniFile _ini = new IniFile();

        public BackgroundSettings LoadBackground()
        {
            return new BackgroundSettings
            {
                Mode = _ini.ReadString("Background", "Mode", "Color"),
                Color = _ini.ReadString("Background", "Color", "#FFFFFFFF"),
                ImagePath = _ini.ReadString("Background", "ImagePath", ""),
                Opacity = _ini.ReadDouble("Background", "Opacity", 1.0),
                CornerRadius = _ini.ReadDouble("Background", "CornerRadius", 12),
                Blur = _ini.ReadBool("Background", "Blur", false)
            };
        }

        public void SaveBackground(BackgroundSettings s)
        {
            _ini.Write("Background", "Mode", s.Mode);
            _ini.Write("Background", "Color", s.Color);
            _ini.Write("Background", "ImagePath", s.ImagePath);
            _ini.Write("Background", "Opacity", s.Opacity);
            _ini.Write("Background", "CornerRadius", s.CornerRadius);
            _ini.Write("Background", "Blur", s.Blur);

            BackgroundChanged?.Invoke();
        }


        public WindowSetting LoadWindow()
        {
            return new WindowSetting
            {

                Width = _ini.ReadDouble("Window", "Width", 480),
                Height = _ini.ReadDouble("Window", "Height", 360),
                Left = _ini.ReadDouble("Window", "Left", 0),
                Top = _ini.ReadDouble("Window", "Top", 0),
            };
        }

        public void SaveWindow(WindowSetting s)
        {
            _ini.Write("Window", "Width", s.Width);
            _ini.Write("Window", "Height", s.Height);
            _ini.Write("Window", "Left", s.Left);
            _ini.Write("Window", "Top", s.Top);
        }

        public static event Action BackgroundChanged;



        public GeneralSettings LoadGeneral()
        {
            return new GeneralSettings
            {
                OpenTray = _ini.ReadBool("General", "OpenTray", true)
            };
        }

        public void SaveGeneral(GeneralSettings s)
        {

            _ini.Write("General", "OpenTray", s.OpenTray);

        }

    }

}
