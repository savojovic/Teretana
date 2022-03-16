using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Teretana
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        bool IsCheckedFromCode = false;
        CultureInfo oldCultureInfo = Thread.CurrentThread.CurrentCulture;

        public static Style btnStyle;
        public SettingsWindow()
        {
            InitializeComponent();
            SetBtnStyles();

            oldCultureInfo = Thread.CurrentThread.CurrentCulture;
            string language = oldCultureInfo.Name;
            if (language.Contains("en"))
            {
                IsCheckedFromCode = true;
                englishRB.IsChecked = true;
            }
            else
            {
                IsCheckedFromCode=true;
                serbianRB.IsChecked = true;
            }
        }
        private void SetBtnStyles()
        {
            saveBtn.Style = btnStyle;
            deleteBtn.Style = btnStyle;
        }
        private void serbianRB_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsLoaded)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("sr");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("sr");
                IsCheckedFromCode = false;
            }
        }
        private void englishRB_Checked(object sender, RoutedEventArgs e)
        {
            
            if ((sender as RadioButton).IsLoaded)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                IsCheckedFromCode = false;
                
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = oldCultureInfo;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(oldCultureInfo.Name);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(oldCultureInfo.Name);
            new LoginWindow().Show();
            Close();
        }

        private void greenRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadBtnStyle();
            Setter background = (Setter)SettingsWindow.btnStyle.Setters[1];
            background.Value = (SolidColorBrush)new BrushConverter().ConvertFrom("#49D191");
        }
        
        private void blueRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadBtnStyle();
            Setter background = (Setter)SettingsWindow.btnStyle.Setters[1];
            background.Value = (SolidColorBrush)new BrushConverter().ConvertFrom("#ADD8E6");
        }
        private void orangeRB_Checked(object sender, RoutedEventArgs e)
        {
            LoadBtnStyle();
            Setter background = (Setter)SettingsWindow.btnStyle.Setters[1];
            background.Value = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA500");
        }
        public void LoadBtnStyle()
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/Styles.xaml", UriKind.RelativeOrAbsolute);
            SettingsWindow.btnStyle = (Style)resourceDictionary["GreenBtnStyle"];
        }
    }
}