using System;
using System.IO;
using Dsmviz.Util;

namespace Dsmviz.Viewmodel.Settings
{
    public static class ViewerSetting
    {
        private static readonly string ApplicationSettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dsmviz");
        private static readonly string SettingsFilePath = Path.Combine(ApplicationSettingsFolder, "ViewerSettings.xml");

        private static ViewerSettingsData _viewerSettings = ViewerSettingsData.CreateDefault();

        public static void Read()
        {
            if (!Directory.Exists(ApplicationSettingsFolder))
            {
                Directory.CreateDirectory(ApplicationSettingsFolder);
            }
            
            FileInfo settingsFileInfo = new FileInfo(SettingsFilePath);
            if (!settingsFileInfo.Exists)
            {
                ViewerSettingsData.WriteToFile(SettingsFilePath, _viewerSettings);
            }
            else
            {
                _viewerSettings = ViewerSettingsData.ReadFromFile(settingsFileInfo.FullName);
            }
        }

        public static LogLevel LogLevel
        {
            set { _viewerSettings.LogLevel = value; }
            get { return _viewerSettings.LogLevel; }
        }

        public static Theme Theme
        {
            set { _viewerSettings.Theme = value; }
            get { return _viewerSettings.Theme; }
        }

        public static void Write()
        {
            ViewerSettingsData.WriteToFile(SettingsFilePath, _viewerSettings);
        }
    }
}
