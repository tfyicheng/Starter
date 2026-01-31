using System.Collections.Generic;

namespace Starter.Models
{
    /// <summary>
    /// 应用程序配置
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 窗口圆角
        /// </summary>
        public double WindowCornerRadius { get; set; } = 12;

        /// <summary>
        /// 窗口背景色
        /// </summary>
        public string WindowBackground { get; set; } = "#CC1E1E1E";

        /// <summary>
        /// 背景图片路径
        /// </summary>
        public string BackgroundImagePath { get; set; } = string.Empty;

        /// <summary>
        /// 是否使用背景图片
        /// </summary>
        public bool UseBackgroundImage { get; set; } = false;

        /// <summary>
        /// 是否启用高斯模糊
        /// </summary>
        public bool EnableBlur { get; set; } = true;

        /// <summary>
        /// 模糊强度
        /// </summary>
        public int BlurRadius { get; set; } = 20;

        /// <summary>
        /// 是否锁定（点击外部不隐藏）
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// 是否显示托盘图标
        /// </summary>
        public bool ShowTrayIcon { get; set; } = false;

        /// <summary>
        /// 菜单组列表
        /// </summary>
        public List<MenuGroup> MenuGroups { get; set; } = new();
    }
}
