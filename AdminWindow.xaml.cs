using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);

        public AdminWindow()
        {
            InitializeComponent();
            LoadEmployees();
            LockAllFields(true);
        }
        private void LoadEmployees()
        {
            string querry = $"select IdOsoba, ime, prezime, DatumRodjenja from zaposleni join osoba on Zaposleni_IdOsoba=IdOsoba";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            List<EmployeeInfo> employees = new List<EmployeeInfo>();

            while (reader.Read())
            {
                employees.Add(new EmployeeInfo(reader.GetInt32(0),reader.GetString(1), reader.GetString(2) ,reader.GetDateTime(3)));
            }
            employeeListView.ItemsSource = employees;
            teretanaDB.Close();
        }

        private void employeeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = ((BasicMemberInfo)(sender as ListView).SelectedItem).Id;
            SetAllFields(id);
        }
        private void SetAllFields(int id)
        {
            SetInfoFields(id);
            SetEmploymentInfo(id);
            SetAvatar(id);
            LockAllFields(true);
        }
       
        private void LockAllFields(bool isLocked)
        {
            nameTextBox.IsEnabled = !isLocked;
            surnameTextBox.IsEnabled = !isLocked;
            dateofBirthDatePicker.IsEnabled = !isLocked;
            jmbgTextBox.IsEnabled = !isLocked;
            emailTextBox.IsEnabled = !isLocked;
            employmentDateTextBox.IsEnabled = !isLocked;
            cityComboBox.IsEnabled = !isLocked;
            employmentDateTextBox.IsEnabled = !isLocked;
            salaryTextBox.IsEnabled = !isLocked;
            contractDuartionDatePicker.IsEnabled = !isLocked;
            usernameTextBox.IsEnabled = !isLocked;
        }
        private void SetAvatar(int id)
        {
            teretanaDB.Open();
            string querry = $"select avatarImg from osoba where idosoba='{id}'";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText= querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string path = reader.GetString(0);
            if (string.Empty.Equals(path))
            {
                avatarImage.Source = new BitmapImage(new Uri(Config.DEFAULT_AVATAR_IMG_PATH));
            }
            else
            {
                avatarImage.Source = new BitmapImage(new Uri(path));
            }
            teretanaDB.Close();
        }
        private void SetEmploymentInfo(int id)
        {
            string querry = $"select * from zaposleni where Zaposleni_IdOsoba='{id}'";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText= querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            employmentDateTextBox.Text = reader.GetDateTime(1).ToShortDateString();
            employmentDateTextBox.IsEnabled = false;
            salaryTextBox.Text = reader.GetDouble(2).ToString();
            contractDuartionDatePicker.Text = reader.GetDateTime(3).ToShortDateString();
            usernameTextBox.Text = reader.GetString(6);

            teretanaDB.Close();
        }
        private void SetInfoFields(int id)
        {
            teretanaDB.Open();
            string querry = $"select * from osoba join opstina on PostanskiBroj=Opstina_PostanskiBroj where IdOsoba='{id}'";
            MySqlCommand command = teretanaDB.CreateCommand();
            command.CommandText = querry;
            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            nameTextBox.Text = reader.GetString(1);
            surnameTextBox.Text = reader.GetString(2);
            dateofBirthDatePicker.Text = reader.GetDateTime(3).ToShortDateString();
            jmbgTextBox.Text = reader.GetString(4);
            emailTextBox.Text = reader.GetString(5);
            cityComboBox.Text = reader.GetString(9);
            teretanaDB.Close();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            if (employeeListView.SelectedItem != null)
            {
                LockAllFields(false);
                saveBtn.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("First select an employee.");
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.Visibility = Visibility.Hidden;
            LockAllFields(true);
            //TODO: Save edited updates to db or add a new employee
        }
    }
}
