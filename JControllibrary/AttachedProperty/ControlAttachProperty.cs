using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JControllibrary.AttachedProperty
{
    public static class ControlAttachProperty
    {
        #region 圆角

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region ComboBox IsDropDownOpen


        public static bool GetIsDropDownOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDropDownOpenProperty);
        }

        public static void SetIsDropDownOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDropDownOpenProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.RegisterAttached("IsDropDownOpen", typeof(bool), typeof(ControlAttachProperty), new PropertyMetadata(false, IsDropDownOpenClick));

        private static void IsDropDownOpenClick(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox comboBox)
            {
                if ((bool)e.NewValue)
                {
                    comboBox.Foreground = Brushes.White;
                    foreach (var item in comboBox.Items)
                    {
                        if (item is ComboBoxItem comboBoxItem)
                            comboBoxItem.Foreground = Brushes.Black;
                    }

                }
                else
                {
                    comboBox.Foreground = Brushes.Black;
                }
            }
        }

        #endregion
    }
}
