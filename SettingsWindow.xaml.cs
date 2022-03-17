using MySqlConnector;
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


        public static volatile Style btnStyle;
        public static string lang;

        private long id;
        private string username;
        private bool isAdmin;
        public SettingsWindow(long id, string username, bool isAdmin)
        {
            this.isAdmin = isAdmin;
            this.username = username;
            this.id = id;
            InitializeComponent();
            SetBtnStyles();
            CheckLanguageRadioButton();
        }
        private void CheckLanguageRadioButton()
        {
            MySqlConnection conn = new MySqlConnection(Config.dbConfigString);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"select jezik, stil from podesavanja where zaposleni_idOsoba='{id}'";
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            string language = reader.GetString(0);
            string style = reader.GetString(1);
            if (language.Contains("en"))
            {
                lang = "en";
                IsCheckedFromCode = true;
                englishRB.IsChecked = true;
            }
            else
            {
                lang = "sr";
                IsCheckedFromCode = true;
                serbianRB.IsChecked = true;
            }
            if (style.Equals("#49D191"))
            {
                greenRB.IsChecked = true;
            }
            else if (style.Equals("#ADD8E6"))
            {
                blueRB.IsChecked = true;
            }
            else
            {
                orangeRB.IsChecked = true;
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
                lang = "sr";
                Thread.CurrentThread.CurrentCulture = new CultureInfo("sr");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("sr");
                IsCheckedFromCode = false;
            }
        }
        private void englishRB_Checked(object sender, RoutedEventArgs e)
        {

            if ((sender as RadioButton).IsLoaded)
            {
                lang = "en";
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                IsCheckedFromCode = false;

            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveStyle(username);
            if (isAdmin)
            {

                new AdminWindow(id, username).Show();
            }
            else
            {
                new EmployeeWindow(id, username).Show();
            }
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
            Style btnOld = btnStyle;
            LoadBtnStyle();
            Setter background = (Setter)SettingsWindow.btnStyle.Setters[1];
            background.Value = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA500");
            Style bnt = btnStyle;
        }
        public static void LoadBtnStyle()
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/Styles.xaml", UriKind.RelativeOrAbsolute);
            SettingsWindow.btnStyle = (Style)resourceDictionary["GreenBtnStyle"];
        }
        private void SaveStyle(string username)
        {
            MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);
            long id = LoginWindow.GetUserId(username);
            string colorhex;
            if (blueRB.IsChecked == true)
            {
                colorhex = "#ADD8E6";
            }
            else if (orangeRB.IsChecked == true)
            {
                colorhex = "#FFA500";
            }
            else
            {
                colorhex = "#49D191";
            }
            string querry = $"insert into podesavanja (zaposleni_idosoba, stil, jezik) values('{id}','{colorhex}','{SettingsWindow.lang}')" +
                $" on duplicate key update `zaposleni_idosoba`='{id}', `stil`='{colorhex}', `jezik`='{SettingsWindow.lang}'";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            cmd.ExecuteNonQuery();
            teretanaDB.Close();
        }
        public static void SetDefaultStyle(string username)
        {
            MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);
            long id = LoginWindow.GetUserId(username);
            string colorhex = "#49D191";
            string lang = "en";
            string querry = $"insert into podesavanja (zaposleni_idosoba, stil, jezik) values('{id}','{colorhex}','{lang}')" +
                $" on duplicate key update `zaposleni_idosoba`='{id}', `stil`='{colorhex}', `jezik`='{lang}'";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            cmd.ExecuteNonQuery();
            teretanaDB.Close();
        }
    }
}