using Starter.Services;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
namespace Starter
{
    /// <summary>
    /// 字符串转Brush转换器
    /// </summary>
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString)
            {
                try
                {
                    if (colorString.StartsWith("#"))
                    {
                        var color = (Color)ColorConverter.ConvertFromString(colorString);
                        return new SolidColorBrush(color);
                    }
                    return new SolidColorBrush(Colors.Gray);
                }
                catch
                {
                    return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 布尔值转高斯模糊效果转换器
    /// </summary>
    public class BooleanToBlurEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool enable && enable)
            {
                return new BlurEffect { Radius = 20, KernelType = KernelType.Gaussian };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 路径转图标转换器
    /// </summary>
    public class PathToIconConverter : IValueConverter
    {
        private static readonly IconService _iconService = new IconService();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrEmpty(path))
            {
                return _iconService.GetIcon(path, 0);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}