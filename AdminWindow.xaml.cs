using Microsoft.Win32;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
            cityComboBox.ItemsSource = EmployeeWindow.GetAllCities();
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
            avatarImgBtn.IsEnabled = true;
            ClearAllFields();
            try
            {
                int id = ((BasicMemberInfo)(sender as ListView).SelectedItem).Id;
                SetAllFields(id);
                saveBtn.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                teretanaDB.Close();
                //When deselecting in emplyeeListView through code, selectionchanged() is triggered
                //this is the way of ignoring that trigger
                //ugly but effective
            }
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
            isAdminCheckBox.IsEnabled = !isLocked;
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
            isAdminCheckBox.IsChecked = reader.GetBoolean(5);
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
            avatarImgBtn.IsEnabled = false;
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
            avatarImgBtn.IsEnabled = true;
            saveBtn.Visibility = Visibility.Hidden;
            LockAllFields(true);
            if (employeeListView.SelectedItem != null)
            {
                EditEmployee();
            }
            else
            {
                AddEmploymentInfo(AddBasicInfo());
            }
            LoadEmployees();
        }
        private void AddEmploymentInfo(long id)
        {
            if (id != 0)
            {
                int isAdmin = isAdminCheckBox.IsChecked == true ? 1 : 0;
                string querry = $"insert into zaposleni (zaposleni_idOsoba, datumZaposlenja, plata, trajanjeugovora, passhash, isadmin, username)" +
                                $"values ('{id}', '{employmentDatePicker.DisplayDate.ToString("yyyy-MM-dd")}', '{salaryTextBox.Text}', " +
                                $"'{contractDuartionDatePicker.DisplayDate.ToString("yyyy-MM-dd")}', '{GetPasswordHash()}', '{isAdmin}', '{usernameTextBox.Text}')";
                teretanaDB.Open();
                MySqlCommand cmd = teretanaDB.CreateCommand();
                cmd.CommandText = querry;
                if (cmd.ExecuteNonQuery() == -1)
                {
                    MessageBox.Show("Unable to add employment info");
                }
                else
                {
                    MessageBox.Show("New employee added succesfully.");
                }
                teretanaDB.Close();
            }
        }
        private long AddBasicInfo()
        {
            long ret=0;
            try
            {
                int postalCode = EmployeeWindow.GetPostalCode(cityComboBox.SelectedItem.ToString());
                string querry = $"insert into osoba (Ime, Prezime, DatumRodjenja, JMBG, Email, Opstina_PostanskiBroj, avatarImg)" +
                                $" values('{nameTextBox.Text}', '{surnameTextBox.Text}', '{dateofBirthDatePicker.DisplayDate.ToString("yyyy-MM-dd")}', '{jmbgTextBox.Text}', " +
                                $"'{emailTextBox.Text}', '{postalCode}', '{Config.DEFAULT_AVATAR_IMG_PATH}')";
                teretanaDB.Open();
                MySqlCommand cmd = teretanaDB.CreateCommand();
                cmd.CommandText = querry;

                if (cmd.ExecuteNonQuery() == -1)
                {
                    MessageBox.Show("Unable to add new employee.\nCheck if all fields are filled correctly.");
                }
                teretanaDB.Close();
                return cmd.LastInsertedId;
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to add new employee.\nCheck if all fields are filled correctly.");
                return ret;
            }

        }
        private void EditEmployee()
        {
            EditBasicInfo();
            EditEmploymentInfo();
        } 
        private void EditEmploymentInfo()
        {
            int id = ((BasicMemberInfo)employeeListView.SelectedItem).Id;
            int isAdmin = isAdminCheckBox.IsChecked == true ? 1 : 0;

            string querry = $"insert into zaposleni (zaposleni_idOsoba, datumZaposlenja, plata, trajanjeugovora, passhash, isadmin, username )" +
                            $"values('{id}','{employmentDatePicker.DisplayDate.ToString("yyyy-MM-dd")}', '{salaryTextBox.Text}', " +
                            $"'{contractDuartionDatePicker.DisplayDate.ToString("yyyy-MM-dd")}', '{GetPasswordHash()}', '{isAdmin}', '{usernameTextBox.Text}') " +
                            $"ON DUPLICATE KEY UPDATE " +
                            $"`zaposleni_idOsoba`='{id}', `datumZaposlenja`='{employmentDatePicker.DisplayDate.ToString("yyyy-MM-dd")}', `plata`='{salaryTextBox.Text}'," +
                            $"`trajanjeugovora`='{contractDuartionDatePicker.DisplayDate.ToString("yyyy-MM-dd")}', `passHash`='{GetPasswordHash()}', `isAdmin`='{isAdmin}'," +
                            $"`username`='{usernameTextBox.Text}'";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            if (cmd.ExecuteNonQuery() == -1)
            {
                MessageBox.Show("Unable to update employment info. Check if all fields are filled correctly.");
            }
            teretanaDB.Close();
        }
        private void EditBasicInfo()
        {
            int postalCode = EmployeeWindow.GetPostalCode(cityComboBox.SelectedItem.ToString());
            int id = ((BasicMemberInfo)employeeListView.SelectedItem).Id;

            string querry = $"insert into osoba (IdOsoba, ime, prezime, datumrodjenja, jmbg, email,opstina_postanskibroj) " +
                            $"values ('{id}', '{nameTextBox.Text}', '{surnameTextBox.Text}', '{dateofBirthDatePicker.DisplayDate.ToString("yyyy-MM-dd")}','{jmbgTextBox.Text}', '{emailTextBox.Text}', '{postalCode}')" +
                            $"ON DUPLICATE KEY UPDATE " +
                            $"`IdOsoba`='{id}', `ime`='{nameTextBox.Text}', `prezime`='{surnameTextBox.Text}', `datumrodjenja`='{dateofBirthDatePicker.DisplayDate.ToString("yyyy-MM-dd")}', `jmbg`='{jmbgTextBox.Text}'," +
                            $"`email`='{emailTextBox.Text}', `opstina_postanskibroj`='{postalCode}'";

            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            if (cmd.ExecuteNonQuery() == -1)
            {
                MessageBox.Show("Unable to edit employee.\nCheck if fields are filled correctly.");
            }
            else
            {
                MessageBox.Show("Employee info succesfully updated.");
            }
            teretanaDB.Close();
        }
        private string GetPasswordHash()
        {
            if (pass1PasswordBox.Password.ToString().Equals(string.Empty))
            {
                MySqlConnection conn = new MySqlConnection(Config.dbConfigString);
                conn.Open();
                int id = ((BasicMemberInfo)employeeListView.SelectedItem).Id;
                string querry = $"select PassHash from zaposleni where Zaposleni_IdOsoba='{id}'";

                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText= querry;
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return reader.GetString(0);
            }
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
            employeeListView.SelectedItem = null;
            ClearAllFields();
            LockAllFields(false);
            avatarImgBtn.IsEnabled = false;
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
            pass1PasswordBox.Password = String.Empty;
            pass2PasswordBox.Password = String.Empty;
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (employeeListView.SelectedItem == null)
            {
                MessageBox.Show("First select an employee.");
            }
            else
            {
                MessageBoxResult rsltMessageBox = EmployeeWindow.AskToContinue();
                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        int id = ((BasicMemberInfo)employeeListView.SelectedItem).Id;
                        string querry = $"delete from zaposleni where zaposleni_idOsoba='{id}'";
                        teretanaDB.Open();
                        MySqlCommand cmd = teretanaDB.CreateCommand();
                        cmd.CommandText = querry;
                        if (cmd.ExecuteNonQuery() == -1)
                        {
                            MessageBox.Show("Unable to delete employee.");
                        }
                        else
                        {
                            cmd.CommandText = $"delete from osoba where idosoba='{id}'";
                            if (cmd.ExecuteNonQuery() != -1)
                            {
                                MessageBox.Show("Employee deleted succesfully.");
                            }
                            else
                            {
                                MessageBox.Show("Unable to delete employee.");
                            }
                        }
                        teretanaDB.Close();
                        break;
                }
            }
        }

        private void avatarImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            try
            {
                if (fileDialog.ShowDialog() == true)
                {
                    string imgPath = fileDialog.FileName.Replace('\\', '/');
                    teretanaDB.Open();
                    MySqlCommand cmd = teretanaDB.CreateCommand();
                    int memberid = ((BasicMemberInfo)employeeListView.SelectedItem).Id;
                    string querry = $"update osoba set avatarImg='{imgPath}' where idosoba='{memberid}'";

                    cmd.CommandText = querry;
                    if (cmd.ExecuteNonQuery() == -1)
                    {
                        MessageBox.Show("Unable to set image");
                    }
                    else
                    {
                        avatarImage.Source = new BitmapImage(new Uri(imgPath));
                        MessageBox.Show("Image succesfully set.");
                    }
                }
            }
            catch (NotSupportedException)
            {
                MessageBox.Show("File format not supported.");
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("First select a member.");
            }
            finally
            {
                teretanaDB.Close();
            }
        }

       
    }
}
