using System;

namespace Starter.Models
{

    /// <summary>
    /// 菜单项（快捷方式）
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 目标路径
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkingDirectory { get; set; } = string.Empty;

        /// <summary>
        /// 运行参数
        /// </summary>
        public string Arguments { get; set; } = string.Empty;

        /// <summary>
        /// 图标路径
        /// </summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// 图标索引
        /// </summary>
        public int IconIndex { get; set; }

        /// <summary>
        /// 圆角
        /// </summary>
        public double CornerRadius { get; set; } = 8;

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string Background { get; set; } = "#4DFFFFFF";

        /// <summary>
        /// 排序索引
        /// </summary>
        public int OrderIndex { get; set; }
    }
}
