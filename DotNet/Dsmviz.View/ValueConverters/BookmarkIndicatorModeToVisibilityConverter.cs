﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Dsmviz.Viewmodel.Main;

namespace Dsmviz.View.ValueConverters
{
    public class BookmarkIndicatorModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IndicatorViewMode viewMode = (IndicatorViewMode)value;
            return (viewMode == IndicatorViewMode.Bookmarks) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
