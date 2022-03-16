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
        public SettingsWindow()
        {
            InitializeComponent();
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
    }
}