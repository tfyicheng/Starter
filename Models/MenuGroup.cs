using System;
using System.Collections.Generic;

namespace Starter.Models
{
    /// <summary>
    /// 菜单组（Tab项）
    /// </summary>
    public class MenuGroup
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; } = "新建分组";

        /// <summary>
        /// 类型：Custom-自定义, Desktop-桌面, ControlPanel-控制面板
        /// </summary>
        public string Type { get; set; } = "Custom";

        /// <summary>
        /// 预设类型（当Type为Preset时使用）
        /// </summary>
        public string PresetType { get; set; } = string.Empty;

        /// <summary>
        /// 排序索引
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 菜单项列表
        /// </summary>
        public List<MenuItem> Items { get; set; } = new();
    }
}
