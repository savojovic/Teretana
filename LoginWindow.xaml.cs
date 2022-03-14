using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    public partial class LoginWindow : Window
    {
        string noEmptyFields = "You must enter your Username and Password!";
        string wrnogCredentials = "Username and password do not match!";

        
        MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);
        
        public LoginWindow()
        {
            InitializeComponent();
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
                OpenNewWindow(IsAdmin(username));
            }
            else
            {
                MessageBox.Show(wrnogCredentials);
            }
        }
        private void OpenNewWindow(bool isAdmin)
        {
            if (isAdmin)
            {
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
            }
            else
            {
                EmployeeWindow employeeWindow = new EmployeeWindow();
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
                OpenNewWindow(IsAdmin(username));
            }
            else if(e.Key == Key.Enter)
            {
                MessageBox.Show(wrnogCredentials);
            }
        }
    }
}
