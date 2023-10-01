﻿using Dsmviz.Datamodel.Dsm.Interfaces;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dsmviz.View.ValueConverters
{
    public class SearchElementNotFoundToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IDsmElement foundElememt = (IDsmElement)value;
            return (foundElememt == null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
