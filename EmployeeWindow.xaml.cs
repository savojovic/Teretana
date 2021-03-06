using Microsoft.Win32;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Teretana
{
    public partial class EmployeeWindow : Window
    {
        MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);
        private long id;
        private string username;
        public EmployeeWindow(long id, string username)
        {
            this.id = id;  
            this.username = username;
            AdminWindow.LoadUserStyle(username);
            InitializeComponent();
            LoadMembers();
            SetElementsLock(false);
            SetBtnStyles();
            citiesComboBox.ItemsSource = EmployeeWindow.GetAllCities();

        }
        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isAdmin = false;
            new SettingsWindow(id,username, isAdmin).Show();
            Close();
        }
        private void SetBtnStyles()
        {
            addBtn.Style = SettingsWindow.btnStyle;
            editBtn.Style = SettingsWindow.btnStyle;
            deleteBtn.Style = SettingsWindow.btnStyle;
            saveBtn.Style = SettingsWindow.btnStyle;
            logoutBtn.Style = SettingsWindow.btnStyle;
            newMembershipBtn.Style = SettingsWindow.btnStyle;
            extendMembershipBtn.Style = SettingsWindow.btnStyle;
            revokeBtn.Style = SettingsWindow.btnStyle;
            startTrainingBtn.Style = SettingsWindow.btnStyle;
            stopTrainingBtn.Style = SettingsWindow.btnStyle;
            avatarBtn.Style = SettingsWindow.btnStyle;
            settingsBtn.Style = SettingsWindow.btnStyle;
        }
        private void SetElementsLock(bool isUnLocked)
        {
            nameTextBox.IsEnabled = isUnLocked;
            surnameTextBox.IsEnabled = isUnLocked;
            datePicker.IsEnabled = isUnLocked;
            jmbgTextBox.IsEnabled = isUnLocked;
            emailTextBox.IsEnabled = isUnLocked;
            citiesComboBox.IsEnabled = isUnLocked;
            if (!isUnLocked)
            {
                saveBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                saveBtn.Visibility = Visibility.Visible;
            }
        }
        private void LoadMembers()
        {
            teretanaDB.Open();
            string querry = "select IdOsoba, ime, prezime, datumrodjenja from clan join osoba on Clan_IdOsoba=IdOsoba";

            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;

            MySqlDataReader reader = cmd.ExecuteReader();

            List<BasicMemberInfo> members = new List<BasicMemberInfo>();
            while (reader.Read())
            {
                members.Add(new BasicMemberInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3)));
            }
            membersListView.ItemsSource = members;
            teretanaDB.Close();
        }
        private void membersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAllFields(sender);
        }
        private void SetTrainings(int id)
        {
            try
            {
                string querry = $"select * from trening where Clan_IdOsoba='{id}'";
                teretanaDB.Open();
                MySqlCommand cmd = teretanaDB.CreateCommand();
                cmd.CommandText = querry;
                MySqlDataReader reader = cmd.ExecuteReader();

                List<Training> trainingList = new List<Training>();

                while (reader.Read())
                {
                    DateTime from = new DateTime();
                    try
                    {
                        from = reader.GetDateTime(0);
                        DateTime until = reader.GetDateTime(1);
                        trainingList.Add(new Training(from,until));
                    }
                    catch (InvalidCastException)
                    {
                        trainingList.Add(new Training(from));
                    }
                }
                trainingsListView.ItemsSource = trainingList;
                teretanaDB.Close();
            }
            catch (Exception ex)
            {
                teretanaDB.Close();
            }
        }
        private void SetMemberShip(int id)
        {
            try
            {
                teretanaDB.Open();
                string querry = $"select DatumUplate, TrajanjeClanarine from clanarina join clan on Clan.Clan_IdOsoba=Clanarina.Clan_IdOsoba where Clan.Clan_IdOsoba={id}";
                MySqlCommand cmd = teretanaDB.CreateCommand();
                cmd.CommandText = querry;

                MySqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                DateTime paidAt = reader.GetDateTime(0);
                string membershipDuration = reader.GetString(1);
                DateTime validUntil = paidAt.AddDays(int.Parse(membershipDuration));
                TimeSpan daysLeft = validUntil.Subtract(DateTime.Now);
                paidAtLbl.Content = paidAt.ToShortDateString();
                validUntilLbl.Content = validUntil.ToShortDateString();
                daysLeftLbl.Content = daysLeft.Days.ToString();

                teretanaDB.Close();
            }
            catch (Exception ex)
            {
                teretanaDB.Close();
                ClearMembershipFields();
            }
        }
        private void SetMemberInfo(ListView sender)
        {
            ListView? listView = sender;
            int idOsoba = ((BasicMemberInfo)listView.SelectedItem).Id;

            teretanaDB.Open();
            string querry = $"select * from osoba where IdOsoba={idOsoba}";

            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;

            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            BasicMemberInfo info = new BasicMemberInfo();
            info.Name = reader.GetString(1);
            info.Surname = reader.GetString(2);
            info.DateTime = reader.GetDateTime(3);
            info.JMBG = reader.GetString(4);
            info.Email = reader.GetString(5);
            info.PostanskiBroj = reader.GetInt32(6);

            nameTextBox.Text = info.Name;
            surnameTextBox.Text = info.Surname;
            datePicker.Text = info.DateTime.ToShortDateString();
            jmbgTextBox.Text = info.JMBG;
            emailTextBox.Text = info.Email;
            citiesComboBox.ItemsSource = EmployeeWindow.GetAllCities();
            citiesComboBox.Text = EmployeeWindow.GetCityName(info.PostanskiBroj);

            teretanaDB.Close();
        }
        public static List<string> GetAllCities()
        {
            List<string> list = new List<string>();
            MySqlConnection conn = new MySqlConnection(Config.dbConfigString);
            conn.Open();
            string querry = "select naziv from opstina";
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            conn.Close();
            return list;
        }
        private static string GetCityName(int postanskiBroj)
        {
            MySqlConnection conn = new MySqlConnection(Config.dbConfigString);
            conn.Open();
            string querry = $"select naziv from opstina where postanskibroj={postanskiBroj}";
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            string naziv = reader.GetString(0);
            conn.Close();
            return naziv;
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SetElementsLock(true);
        }
        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SetElementsLock(false);
            InsertNewMemberIntoDb(GetNewMemberInfo());
            LoadMembers();
        }
        private void InsertNewMemberIntoDb(BasicMemberInfo member)
        {
            try
            {
                string date = String.Format("{0:yyyy/MM/dd}", member.DateTime);

                string querry = $"insert into osoba (ime, prezime, datumrodjenja, jmbg, email,opstina_postanskibroj) " +
                    $"values ('{member.Name}','{member.Surname}','{date}','{member.JMBG}','{member.Email}','{member.PostanskiBroj}')" +
                    $"ON DUPLICATE KEY UPDATE `ime`='{member.Name}', `prezime`='{member.Surname}', `datumrodjenja`='{date}', `jmbg`='{member.JMBG}', `email`='{member.Email}'," +
                    $"`opstina_postanskibroj`= '{member.PostanskiBroj}'";
                teretanaDB.Open();
                MySqlCommand cmd = teretanaDB.CreateCommand();
                cmd.CommandText = querry;
                if (cmd.ExecuteNonQuery() == -1)
                {
                    MessageBox.Show(Teretana.Resources.Resources.Insert_new_member_err);
                }
                long newMemberId = cmd.LastInsertedId;
                string querry2 = $"insert into clan (clan_idosoba) values ('{newMemberId}') ON DUPLICATE KEY UPDATE " +
                    $"`clan_idosoba`='{newMemberId}'";
                cmd.CommandText = querry2;
                cmd.ExecuteNonQuery();
                teretanaDB.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                if (ex.Message.Contains("Cannot add or update a child row: a foreign key constraint fails"))
                {
                    MessageBox.Show(Teretana.Resources.Resources.Insert_new_member_err);
                }
                teretanaDB.Close();
            }
        }
        BasicMemberInfo GetNewMemberInfo()
        {
            BasicMemberInfo newMember = new BasicMemberInfo();
            try
            {
                newMember.Name = nameTextBox.Text;
                newMember.Surname = surnameTextBox.Text;
                newMember.JMBG = jmbgTextBox.Text;
                newMember.Email = emailTextBox.Text;
                newMember.DateTime = datePicker.SelectedDate.Value.Date;
                newMember.PostanskiBroj = GetPostalCode(citiesComboBox.SelectedItem.ToString());
            }
            catch (Exception ex)
            {

            }

            return newMember;
        }
        public static int GetPostalCode(string name)
        {
            MySqlConnection conn = new MySqlConnection(Config.dbConfigString);
            int postalCode;
            conn.Open();
            string querry = $"select postanskiBroj from opstina where naziv='{name}'";
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            postalCode = reader.GetInt32(0);
            conn.Close();
            return postalCode;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
        }
        private void ClearAllFields()
        {
            ClearMemberInfoFields();
            ClearMembershipFields();
            ClearTrainingsFields();
            SetElementsLock(true);
            string path = "C:\\Users\\jsavic\\Documents\\FaxProjects\\HCI\\WPF_Teretana\\Teretana\\Teretana\\assets\\avatar.png";
            avatarImage.Source = new BitmapImage(new Uri(path));
        }
        private void ClearMemberInfoFields()
        {
            nameTextBox.Clear();
            surnameTextBox.Clear();
            datePicker.Text = String.Empty;
            jmbgTextBox.Clear();
            emailTextBox.Clear();
        }
        private void ClearTrainingsFields()
        {
            trainingsListView.ItemsSource = new List<string>();
        }
        private void ClearMembershipFields()
        {
            paidAtLbl.Content = String.Empty;
            validUntilLbl.Content = String.Empty;
            daysLeftLbl.Content = String.Empty;
        }
        private void SetAllFields(object sender)
        {
            try
            {
                int id = ((BasicMemberInfo)(sender as ListView).SelectedItem).Id;
                SetMemberInfo(sender as ListView);
                SetMemberShip(id);
                SetTrainings(id);
                SetAvatar(id);
            }
            catch (Exception)
            {

            }
            
        }
        private void SetAvatar(int id)
        {
            string path;
            teretanaDB.Open();
            string querry = $"select avatarImg from osoba where idOsoba='{id}'";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            try
            {
                path = reader.GetString(0);
                if (path.Equals(String.Empty))
                    throw new Exception();
            }catch(Exception)
            {
                path = Config.DEFAULT_AVATAR_IMG_PATH;
            }
            avatarImage.Source = new BitmapImage(new Uri(path));
            teretanaDB.Close();
        }
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            int memberId = ((BasicMemberInfo)membersListView.SelectedItem).Id;
            string querryDeleteMembership = $"delete from clanarina where Clan_IdOsoba={memberId}";
            string querryDeleteTrainings = $"delete from trening where Clan_IdOsoba={memberId}";
            string querryDeleteMember = $"delete from clan where Clan_IdOsoba={memberId}";

            MessageBoxResult rsltMessageBox = AskToContinue();
            teretanaDB.Open();
            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    MySqlCommand cmd = teretanaDB.CreateCommand();
                    cmd.CommandText = querryDeleteMembership;
                    if (cmd.ExecuteNonQuery() != -1)
                    {
                        cmd.CommandText = querryDeleteTrainings;
                        if (cmd.ExecuteNonQuery() != -1)
                        {
                            cmd.CommandText = querryDeleteMember;
                            if (cmd.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show(Teretana.Resources.Resources.Delete_err);
                            }
                        }
                        else
                        {
                            MessageBox.Show(Teretana.Resources.Resources.Delete_err);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Teretana.Resources.Resources.Delete_err);
                    }
                    teretanaDB.Close();
                    break;

                case MessageBoxResult.No:
                    break;

                case MessageBoxResult.Cancel:
                    break;
            }
            teretanaDB.Close();
            LoadMembers();
        }
        public static MessageBoxResult AskToContinue()
        {
            string sMessageBoxText = Teretana.Resources.Resources.Continue_prompt;

            string sCaption = Teretana.Resources.Resources.Gym_name;

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
        }
        private void newMembershipsBtn_Click(object sender, RoutedEventArgs e)
        {
            string btntext = (e.OriginalSource as Button).Content.ToString();

            switch (btntext)
            {
                case "NEW":
                    AddNewMembership();
                    break;
                case "EDIT":
                    EditMembership();
                    break;
                case "REVOKE":
                    RevokeMembership();
                    break;
            }
            try
            {
                SetMemberShip(((BasicMemberInfo)(membersListView).SelectedItem).Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Teretana.Resources.Resources.Select_member);
            }
        }
        private void RevokeMembership()
        {
            MessageBoxResult rsltMessageBox = AskToContinue();
            if (rsltMessageBox==MessageBoxResult.Yes)
            {
                string querry = $"delete from clanarina where Clan_IdOsoba='{((BasicMemberInfo)membersListView.SelectedItem).Id}'";
                teretanaDB.Open();
                MySqlCommand cmd = teretanaDB.CreateCommand();
                cmd.CommandText = querry;
                if (cmd.ExecuteNonQuery() == -1)
                {
                    MessageBox.Show(Teretana.Resources.Resources.Delete_err);
                }
                else
                {
                    MessageBox.Show(Teretana.Resources.Resources.Succesfull_deletion);
                }
                teretanaDB.Close();
            }
        }
        private void AddNewMembership()
        {
            string querry = "";
            string memberName = string.Empty;
            bool isEditMembership = false;
            int memberId;
            try
            {
                memberId = ((BasicMemberInfo)membersListView.SelectedItem).Id;
                memberName = ((BasicMemberInfo)membersListView.SelectedItem).Name;

                if (paidAtLbl.Content.ToString() != string.Empty)
                    throw new ExistingMembershipException(Teretana.Resources.Resources.Existing_membership);
                new NewMembershipWindow(memberId, memberName, isEditMembership).ShowDialog();
            }
            catch (ExistingMembershipException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(Teretana.Resources.Resources.Select_member);
            }
        }
        private void EditMembership()
        {
            string querry = "";
            string memberName = string.Empty;
            bool isEditMembership = true;
            int memberId;
            try
            {
                memberId = ((BasicMemberInfo)membersListView.SelectedItem).Id;
                memberName = ((BasicMemberInfo)membersListView.SelectedItem).Name;
                if(paidAtLbl.Content.ToString() == string.Empty)
                {
                    throw new Exception(Teretana.Resources.Resources.Create_membership);
                }
                else
                {
                    //Start new membership window in edit mode
                    new NewMembershipWindow(memberId,memberName,isEditMembership).ShowDialog();
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(Teretana.Resources.Resources.Select_member);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void StartTrainingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (daysLeftLbl.Content.ToString() != string.Empty)
            {
                List<Training> trainings = (List<Training>)trainingsListView.ItemsSource;
                if (trainings == null)
                    trainings = new List<Training>();
                Training newTraining = new Training(DateTime.Now);
                trainings.Add(newTraining);

                string querry = $"insert into trening (clan_idosoba, dosaou) values ('{((BasicMemberInfo)membersListView.SelectedItem).Id}','{newTraining.startTime.ToString("yyyy:MM:dd hh:mm:ss").Replace('/', '-')}')";
                teretanaDB.Open();
                MySqlCommand cmd = teretanaDB.CreateCommand();
                cmd.CommandText = querry;

                if (cmd.ExecuteNonQuery() == -1)
                {
                    MessageBox.Show(Teretana.Resources.Resources.Training_start_error);
                }
                else
                {
                    trainingsListView.ItemsSource = null;
                    trainingsListView.ItemsSource = trainings;
                }
                teretanaDB.Close();
            }
            else
            {
                MessageBox.Show(Teretana.Resources.Resources.Create_membership);
            }
        }
        private void StopTrainingBtn_Click(object sender, RoutedEventArgs e)
        {
            teretanaDB.Open();
            try
            {
                Training selectedTraining = (Training)trainingsListView.SelectedItem;

                List<Training> trainings = (List<Training>)trainingsListView.ItemsSource;
                Training foundTraining = trainings[trainings.IndexOf(selectedTraining)];
                if (foundTraining.Until==null)
                {
                    foundTraining.SetEndTime(DateTime.Now);
                    int memberId = ((BasicMemberInfo)membersListView.SelectedItem).Id;
                    string endTime = foundTraining.endTime.ToString("yyyy:MM:dd hh:mm:ss").Replace('/', '-');

                    string querry = $"update trening set OtisaoU='{endTime}' where clan_idOsoba='{memberId}'";
                    MySqlCommand cmd = teretanaDB.CreateCommand();
                    cmd.CommandText = querry;

                    if (cmd.ExecuteNonQuery() != -1)
                    {
                        trainingsListView.ItemsSource = null;
                        trainingsListView.ItemsSource = trainings;
                    }
                    else
                    {
                        MessageBox.Show(Teretana.Resources.Resources.Training_stop_err);
                    }
                }
                else
                {
                    MessageBox.Show(Teretana.Resources.Resources.Training_ended_errr);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Teretana.Resources.Resources.Select_training);
            }
            
            teretanaDB.Close();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            OpenFileDialog fileDialog = new OpenFileDialog();
            try
            {
                if (fileDialog.ShowDialog() == true)
                {
                    string imgPath = fileDialog.FileName.Replace('\\','/');
                    //TODO: brise \
                    teretanaDB.Open();
                    MySqlCommand cmd = teretanaDB.CreateCommand();
                    int memberid = ((BasicMemberInfo)membersListView.SelectedItem).Id;
                    string querry = $"update osoba set avatarImg='{imgPath}' where idosoba='{memberid}'";

                    cmd.CommandText = querry;
                    if (cmd.ExecuteNonQuery() == -1)
                    {
                        MessageBox.Show(Teretana.Resources.Resources.Image_set_error);
                    }
                    else
                    {
                        avatarImage.Source = new BitmapImage(new Uri(imgPath));
                        MessageBox.Show(Teretana.Resources.Resources.Image_set_succesfully);
                    }
                }
            }
            catch (NotSupportedException)
            {
                MessageBox.Show(Teretana.Resources.Resources.Unsuported_format);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show(Teretana.Resources.Resources.Select_member);
            }
            finally
            {
                teretanaDB.Close();
            }
        }
    }
}