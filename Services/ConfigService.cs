using Starter.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Starter.Services
{
    /// <summary>
    /// 配置服务
    /// </summary>
    public class ConfigService
    {
        private readonly string _configPath;
        private AppConfig _config = new();

        public ConfigService()
        {
            _configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                Infrastructure.Constants.DataDirectory,
                Infrastructure.Constants.ConfigFileName);

            LoadConfig();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath);
                    _config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
                else
                {
                    // 创建默认配置
                    EnsureDataDirectory();
                    SaveConfig();
                }
            }
            catch
            {
                _config = new AppConfig();
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            try
            {
                EnsureDataDirectory();
                var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存配置失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 确保数据目录存在
        /// </summary>
        private void EnsureDataDirectory()
        {
            var directory = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// 获取当前配置
        /// </summary>
        public AppConfig GetConfig() => _config;

        /// <summary>
        /// 更新配置
        /// </summary>
        public void UpdateConfig(AppConfig config)
        {
            _config = config;
            SaveConfig();
        }

        /// <summary>
        /// 添加菜单组
        /// </summary>
        public void AddMenuGroup(MenuGroup group)
        {
            group.OrderIndex = _config.MenuGroups.Count;
            _config.MenuGroups.Add(group);
            SaveConfig();
        }

        /// <summary>
        /// 删除菜单组
        /// </summary>
        public void RemoveMenuGroup(string groupId)
        {
            var group = _config.MenuGroups.FirstOrDefault(g => g.Id == groupId);
            if (group != null)
            {
                _config.MenuGroups.Remove(group);
                // 重新排序
                for (int i = 0; i < _config.MenuGroups.Count; i++)
                {
                    _config.MenuGroups[i].OrderIndex = i;
                }
                SaveConfig();
            }
        }

        /// <summary>
        /// 更新菜单组
        /// </summary>
        public void UpdateMenuGroup(MenuGroup group)
        {
            var index = _config.MenuGroups.FindIndex(g => g.Id == group.Id);
            if (index >= 0)
            {
                _config.MenuGroups[index] = group;
                SaveConfig();
            }
        }
    }
}
