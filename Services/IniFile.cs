using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Starter.Infrastructure
{
    internal class IniFile
    {
        public string FilePath { get; }

        public IniFile(string fileName = "Settings.ini")
        {
            FilePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                fileName
            );

            if (!File.Exists(FilePath))
            {
                File.Create(FilePath).Dispose();
            }
        }

        #region Win32

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string defaultValue,
            StringBuilder retVal,
            int size,
            string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool WritePrivateProfileString(
            string section,
            string key,
            string value,
            string filePath);

        #endregion

        #region Read

        public string ReadString(string section, string key, string defaultValue = "")
        {
            var sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, FilePath);
            return sb.ToString();
        }

        public int ReadInt(string section, string key, int defaultValue = 0)
        {
            var value = ReadString(section, key, defaultValue.ToString());
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public double ReadDouble(string section, string key, double defaultValue = 0)
        {
            var value = ReadString(section, key, defaultValue.ToString());
            return double.TryParse(value, out var result) ? result : defaultValue;
        }

        public bool ReadBool(string section, string key, bool defaultValue = false)
        {
            var value = ReadString(section, key, defaultValue ? "true" : "false");
            return value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value == "1";
        }

        #endregion

        #region Write

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, FilePath);
        }

        public void Write(string section, string key, int value)
        {
            Write(section, key, value.ToString());
        }

        public void Write(string section, string key, double value)
        {
            Write(section, key, value.ToString());
        }

        public void Write(string section, string key, bool value)
        {
            Write(section, key, value ? "true" : "false");
        }

        #endregion
    }
}
