using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Starter.Services
{
    /// <summary>
    /// 图标服务
    /// </summary>
    public class IconService
    {
        /// <summary>
        /// 从文件路径获取图标
        /// </summary>
        public BitmapSource GetIcon(string filePath, int iconIndex = 0)
        {
            try
            {
                if (!File.Exists(filePath) && !Directory.Exists(filePath))
                    return null;

                Icon icon = null;

                if (iconIndex == 0)
                {
                    icon = Icon.ExtractAssociatedIcon(filePath);
                }
                else
                {
                    icon = ExtractIconByIndex(filePath, iconIndex);
                }

                if (icon != null)
                {
                    BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    icon.Dispose();
                    return source;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// 提取指定索引的图标
        /// </summary>
        private Icon ExtractIconByIndex(string filePath, int iconIndex)
        {
            try
            {
                // 暂时返回默认图标
                return GetDefaultIcon();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取默认图标
        /// </summary>
        public Icon GetDefaultIcon()
        {
            try
            {
                return Icon.ExtractAssociatedIcon("shell32.dll");
            }
            catch
            {
                return null;
            }
        }
    }
}
