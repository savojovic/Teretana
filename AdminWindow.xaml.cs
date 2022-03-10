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
using System.Windows.Shapes;

namespace Teretana
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);

        int employeeId;

        public AdminWindow()
        {
            InitializeComponent();
            LoadEmployees();
            LockAllFields(true);
            teretanaDB.Open();
            cityComboBox.ItemsSource = EmployeeWindow.GetAllCities();
            teretanaDB.Close();
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
            employeeId = ((BasicMemberInfo)(sender as ListView).SelectedItem).Id;
            SetAllFields(employeeId);
            saveBtn.Visibility = Visibility.Hidden;
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
            employmentDatePicker.IsEnabled = !isLocked;
            cityComboBox.IsEnabled = !isLocked;
            employmentDatePicker.IsEnabled = !isLocked;
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
            string path;
            try
            {
                path = reader.GetString(0);
            }
            catch (Exception)
            {
                path = Config.DEFAULT_AVATAR_IMG_PATH;
            }
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

            employmentDatePicker.Text = reader.GetDateTime(1).ToShortDateString();
            employmentDatePicker.IsEnabled = false;
            salaryTextBox.Text = reader.GetDouble(2).ToString();
            contractDuartionDatePicker.Text = reader.GetDateTime(3).ToShortDateString();
            usernameTextBox.Text = reader.GetString(6);

            teretanaDB.Close();
        }
        private void SetInfoFields(int id)
        {
            teretanaDB.Open();
            cityComboBox.ItemsSource = EmployeeWindow.GetAllCities();
            string querry = $"select * from osoba join opstina on PostanskiBroj=Opstina_PostanskiBroj where IdOsoba='{id}'";
            MySqlCommand command = teretanaDB.CreateCommand();
            command.CommandText = querry;
            MySqlDataReader reader = command.ExecuteReader();

            reader.Read();

            
            cityComboBox.Text = reader.GetString(9);
            nameTextBox.Text = reader.GetString(1);
            surnameTextBox.Text = reader.GetString(2);
            dateofBirthDatePicker.Text = reader.GetDateTime(3).ToShortDateString();
            jmbgTextBox.Text = reader.GetString(4);
            emailTextBox.Text = reader.GetString(5);
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
            PersistBasicInfo();
            //TODO: Save edited updates to db or add a new employee
        }
        private void PersistBasicInfo()
        {
            try
            {
                DateTime dateOfBirth = dateofBirthDatePicker.DisplayDate;
                string dateOfBirthString = dateOfBirth.ToString("yyyy-MM-dd");
                int postalCode = EmployeeWindow.GetPostalCode(teretanaDB, cityComboBox.SelectedItem.ToString());
                string querry = $"insert into osoba (ime, prezime, datumrodjenja, jmbg, email,opstina_postanskibroj) " +
                        $"values ('{usernameTextBox.Text}','{surnameTextBox.Text}','{dateOfBirthString}','{jmbgTextBox.Text}','{emailTextBox.Text}','{postalCode}')" +
                        $"ON DUPLICATE KEY UPDATE `ime`='{nameTextBox.Text}', `prezime`='{surnameTextBox.Text}', `datumrodjenja`='{dateOfBirthString}', `jmbg`='{jmbgTextBox.Text}', " +
                        $"`email`='{emailTextBox.Text}'," +
                        $"`opstina_postanskibroj`= '{postalCode}'";

                teretanaDB.Open();
                MySqlCommand cmd = teretanaDB.CreateCommand();
                cmd.CommandText = querry;
                if (cmd.ExecuteNonQuery() == -1)
                {
                    MessageBox.Show("Unable to add basic info of a new employee.\nCheck if fields are filled correctly.");
                    teretanaDB.Close();
                    return;
                }
                string employmentDate = employmentDatePicker.DisplayDate.ToString("yyyy-MM-dd");
                string contractDuration = contractDuartionDatePicker.DisplayDate.ToString("yyyy-MM-dd");
                string passHash = GetPasswordHash();

                int isAdmin = (bool)isAdminCheckBox.IsChecked ? 1 : 0;

                string querry2 = $"insert into zaposleni (Zaposleni_IdOsoba, DatumZaposlenja, Plata, TrajanjeUgovora, PassHash, IsAdmin, Username) " +
                    $"values ('{cmd.LastInsertedId}','{employmentDate}','{salaryTextBox.Text}','{contractDuration}','{passHash}','{isAdmin}','{usernameTextBox.Text}')";
                cmd.CommandText = querry2;
                if (cmd.ExecuteNonQuery()==-1)
                {
                    MessageBox.Show("Unable to add employment info of a new employee.\nCheck if fields are filled correctly.");
                }
                else
                {
                    MessageBox.Show("New employee added succesfully.");
                }
            }
            catch (MySqlException)
            {
                MessageBox.Show("Unable to add a new employee.\nCheck if fields are filled correctly.");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                teretanaDB.Close();
            }
        }
        private string GetPasswordHash()
        {
            if (pass1PasswordBox.Password.ToString().Equals(pass2PasswordBox.Password.ToString()))
            {
                SHA256 sha256 = SHA256.Create();
                string passhash = Config.GetHash(sha256,pass1PasswordBox.Password.ToString());
                return passhash;
            }
            else
            {
                throw new Exception("Passwords do not match.");
            }
        }
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
            LockAllFields(false);
            saveBtn.Visibility = Visibility.Visible;
        }
        private void ClearAllFields()
        {
            ClearBasicInfoFields();
            ClearEmploymentInfoFields();
        }
        private void ClearBasicInfoFields()
        {
            nameTextBox.Text = String.Empty;
            surnameTextBox.Text= String.Empty;
            emailTextBox.Text= String.Empty;
            jmbgTextBox.Text= String.Empty;
            dateofBirthDatePicker.Text= String.Empty;
        }
        private void ClearEmploymentInfoFields()
        {
            employmentDatePicker.Text = String.Empty;
            salaryTextBox.Text = String.Empty;
            contractDuartionDatePicker.Text = String.Empty;
            usernameTextBox.Text = String.Empty;
        }

    }
}
