﻿using Dsmviz.Viewmodel.Main;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Dsmviz.View.ValueConverters
{
    public class SearchStateToColorConverter : IValueConverter
    {
        public SolidColorBrush NoMatchBrush { get; set; }
        public SolidColorBrush MatchBrush { get; set; }
        public SolidColorBrush MultipleMatchesBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush;
            SearchState searchState = (SearchState)value;
            switch(searchState)
            {
                case SearchState.NoInput:
                    brush = NoMatchBrush;
                    break;
                case SearchState.NoMatch:
                    brush = NoMatchBrush;
                    break;
                case SearchState.Match:
                    brush = MatchBrush;
                    break;
                default:
                    brush = NoMatchBrush;
                    break;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
