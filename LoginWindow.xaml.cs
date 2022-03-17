using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Teretana
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// TODO: Prilikom promjene stila treba da se unese username i password pa da se sacuvaju promjene za korisnika u db
    /// kad se uloguje, objekat styles treba napraviti tako da zadovoljava konfig iz baze
    public partial class LoginWindow : Window
    {
        string noEmptyFields = "You must enter your Username and Password!";
        string wrnogCredentials = "Username and password do not match!";

        MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);

        public LoginWindow()
        {
            if (SettingsWindow.btnStyle == null)
                LoadBtnStyle();
            InitializeComponent();
            SetBtnStyles();
        }
        private void SetBtnStyles()
        {
            loginBtn.Style = SettingsWindow.btnStyle;
        }
        private void LoadBtnStyle()
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/Styles.xaml", UriKind.RelativeOrAbsolute);
            SettingsWindow.btnStyle = (Style)resourceDictionary["GreenBtnStyle"];
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password.ToString();

            if ("".Equals(username) || "".Equals(password))
            {
                MessageBox.Show(noEmptyFields);
            }
            else if (isAuthorized(username, password))
            {
                //LoadStyle(username);
                //SaveStyle(username);
                OpenNewWindow(GetUserId(username), username, IsAdmin(username));
            }
            else
            {
                MessageBox.Show(wrnogCredentials);
            }
        }
        private void LoadStyle(string username)
        {
            string querry = $"select stil, jezik from podesavanja where zaposleni_idosoba='{GetUserId(username)}'";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string color = reader.GetString(0);
            string lang = reader.GetString(1);

            SettingsWindow.LoadBtnStyle();
            Setter background = (Setter)SettingsWindow.btnStyle.Setters[1];
            background.Value = (SolidColorBrush)new BrushConverter().ConvertFrom(color);

            //Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            teretanaDB.Close();
        }
        public static long GetUserId(string username)
        {
            MySqlConnection con = new MySqlConnection(Config.dbConfigString);
            con.Open();
            string querry = $"select zaposleni_idosoba from zaposleni where username='{username}'";
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            long id = reader.GetInt64(0);
            con.Close();
            return id;
        }
        private void OpenNewWindow(long id, string username, bool isAdmin)
        {
            if (isAdmin)
            {
                AdminWindow adminWindow = new AdminWindow(id, username);
                adminWindow.Show();
            }
            else
            {
                EmployeeWindow employeeWindow = new EmployeeWindow(id, username);
                employeeWindow.Show();
            }
            this.Close();
        }
        private bool IsAdmin(string username)
        {
            string querry = $"select IsAdmin from zaposleni join osoba on Zaposleni_IdOsoba=IdOsoba where username='{username}'";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;

            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.GetBoolean("IsAdmin"))
            {
                reader.Close();
                return true;
            }
            reader.Close();
            teretanaDB.Close();
            return false;
        }
        private bool isAuthorized(string username, string password)
        {
            SHA256 sha256 = SHA256.Create();
            string passhash = Config.GetHash(sha256, password);
            string querry = $"select PassHash from zaposleni join osoba on Zaposleni_IdOsoba=IdOsoba where username='{username}'";
            teretanaDB.Open();

            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;

            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string realhash;
            try
            {
                realhash = reader.GetString("PassHash");
                reader.Close();
                teretanaDB.Close();
                if (passhash.Equals(realhash))
                    return true;
                return false;
            }
            catch (Exception)
            {
                reader.Close();
                teretanaDB.Close();
                return false;
            }
        }


        private void credentialsBox_KeyDown(object sender, KeyEventArgs e)
        {
            string username = usernameTextBox.Text;
            if (e.Key == Key.Enter && isAuthorized(username, passwordBox.Password.ToString()))
            {
                OpenNewWindow(GetUserId(username), username, IsAdmin(username));
            }
            else if (e.Key == Key.Enter)
            {
                MessageBox.Show(wrnogCredentials);
            }
        }
    }
}
