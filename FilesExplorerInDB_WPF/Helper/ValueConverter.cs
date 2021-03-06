﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FilesExplorerInDB_WPF.Helper
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
            //return value != null && (Visibility) value == Visibility.Visible;
        }
    }

    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility) value == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
            //return value != null && (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public class VisibilityToBoolConverterReverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility) value != Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
            //return value != null && (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}