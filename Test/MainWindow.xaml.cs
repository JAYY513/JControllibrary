using JControllibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BlueClick(object sender, RoutedEventArgs e)
        {
            Collection<ResourceDictionary> mergedDicts = base.Resources.MergedDictionaries;
            ResourceDictionary skinDict = Application.LoadComponent(
                new Uri(@"pack://application:,,,/JControllibrary;component/Themes/Colors/BlueColor.xaml",
                UriKind.Relative)) as ResourceDictionary;
            mergedDicts.Add(skinDict);
        }
        private static void ReplaceEntry(object entryName, object newValue, ResourceDictionary parentDictionary = null)
        {
            if (parentDictionary == null)
                parentDictionary = Application.Current.Resources;

            if (parentDictionary.Contains(entryName))
            {
                var brush = parentDictionary[entryName] as SolidColorBrush;
                if (brush != null && !brush.IsFrozen)
                {
                    var animation = new ColorAnimation
                    {
                        From = ((SolidColorBrush)parentDictionary[entryName]).Color,
                        To = ((SolidColorBrush)newValue).Color,
                        Duration = new Duration(TimeSpan.FromMilliseconds(300))
                    };
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                }
                else
                    parentDictionary[entryName] = newValue; //Set value normally
            }

            foreach (var dictionary in parentDictionary.MergedDictionaries)
                ReplaceEntry(entryName, newValue, dictionary);
        }
        private void RedClick(object sender, RoutedEventArgs e)
        {
            ReplaceEntry("DarkBrush", new SolidColorBrush(Colors.Red));
        }
        
    }
}