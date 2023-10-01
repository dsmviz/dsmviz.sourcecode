﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Dsmviz.Util;
using Dsmviz.Application.Interfaces;
using Dsmviz.Viewmodel.Common;

namespace Dsmviz.Viewmodel.Settings
{
    public class SettingsViewmodel : ViewmodelBase
    {
        private const string PastelThemeName = "Pastel";
        private const string LightThemeName = "Light";

        private readonly IDsmApplication _application;
        private LogLevel _logLevel;
        private string _selectedThemeName;

        private readonly Dictionary<Theme, string> _supportedThemes;

        public SettingsViewmodel(IDsmApplication application)
        {
            _application = application;
            _supportedThemes = new Dictionary<Theme, string>
            {
                [Theme.Pastel] = PastelThemeName,
                [Theme.Light] = LightThemeName
            };

            LogLevel = ViewerSetting.LogLevel;
            SelectedThemeName = _supportedThemes[ViewerSetting.Theme];

            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public ICommand AcceptChangeCommand { get; }

        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set
            {
                _logLevel = value;
                OnPropertyChanged();
            }
        }

        public List<string> SupportedThemeNames => _supportedThemes.Values.ToList();

        public string SelectedThemeName
        {
            get { return _selectedThemeName; }
            set
            {
                _selectedThemeName = value;
                OnPropertyChanged();
            }
        }

        private void AcceptChangeExecute(object parameter)
        {
            ViewerSetting.LogLevel = LogLevel;
            ViewerSetting.Theme = _supportedThemes.FirstOrDefault(x => x.Value == SelectedThemeName).Key;
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }
    }
}
