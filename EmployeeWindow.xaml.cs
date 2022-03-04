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
    //TODO: prilikom dodavanja nove osobe, dodati ga i kao novog clana
    public partial class EmployeeWindow : Window
    {
        MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);
        public EmployeeWindow()
        {
            InitializeComponent();
            LoadMembers();
            SetElementsLock(false);
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
                    trainingList.Add(new Training(reader.GetDateTime(0), reader.GetDateTime(1)));
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
            teretanaDB.Close();

            nameTextBox.Text = info.Name;
            surnameTextBox.Text = info.Surname;
            datePicker.Text = info.DateTime.ToShortDateString();
            jmbgTextBox.Text = info.JMBG;
            emailTextBox.Text = info.Email;
            citiesComboBox.ItemsSource = GetAllCities();
            citiesComboBox.Text = GetCityName(info.PostanskiBroj);
        }
        private List<string> GetAllCities()
        {
            List<string> list = new List<string>();
            teretanaDB.Open();
            string querry = "select naziv from opstina";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            teretanaDB.Close();
            return list;
        }
        private string GetCityName(int postanskiBroj)
        {
            teretanaDB.Open();
            string querry = $"select naziv from opstina where postanskibroj={postanskiBroj}";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            string naziv = reader.GetString(0);
            teretanaDB.Close();
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
                    MessageBox.Show("Error Inserting new member");
                }
                long newMemberId = cmd.LastInsertedId;
                string querry2 = $"insert into clan (clan_idosoba,brojdolazaka) values ('{newMemberId}', '{0}') ON DUPLICATE KEY UPDATE " +
                    $"`clan_idosoba`='{newMemberId}'";
                cmd.CommandText = querry2;
                cmd.ExecuteNonQuery();
                teretanaDB.Close();
            }
            catch (MySqlException ex)
            {
                if (!ex.Message.Contains("Cannot add or update a child row: a foreign key constraint fails"))
                {
                    MessageBox.Show("Unable to add a new member.");
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
        private int GetPostalCode(string name)
        {
            int postalCode;
            teretanaDB.Open();
            string querry = $"select postanskiBroj from opstina where naziv='{name}'";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            postalCode = reader.GetInt32(0);
            teretanaDB.Close();
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
            int id = ((BasicMemberInfo)(sender as ListView).SelectedItem).Id;
            SetMemberInfo(sender as ListView);
            SetMemberShip(id);
            SetTrainings(id);
        }

        private void gradoviComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                                MessageBox.Show("Failed to delete the member.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the member.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the member.");
                    }
                    teretanaDB.Close();
                    break;

                case MessageBoxResult.No:
                    break;

                case MessageBoxResult.Cancel:
                    break;
            }
            teretanaDB.Close();
        }
        private MessageBoxResult AskToContinue()
        {
            string sMessageBoxText = "Do you want to continue?";
            string sCaption = "Power Gym";

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
                    MessageBox.Show("Deleting failed.");
                }
                else
                {
                    MessageBox.Show("Membeship succesfully deleted.");
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
                    throw new ExistingMembershipException("Member already has a membership.");
                new NewMembershipWindow(memberId, memberName, isEditMembership).ShowDialog();
            }
            catch (ExistingMembershipException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Select a member first.");
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
                    throw new Exception("First create a new membership.");
                }
                else
                {
                    //Start new membership window in edit mode
                    new NewMembershipWindow(memberId,memberName,isEditMembership).ShowDialog();
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Select a member first.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void startTrainingBtn_Click(object sender, RoutedEventArgs e)
        {
            List<Training> trainings = (List<Training>)trainingsListView.ItemsSource;
            if (trainings == null)
                trainings = new List<Training>();
            Training newTraining = new Training(DateTime.Now);
            trainings.Add(newTraining);

            string querry = $"insert into trening (clan_idosoba, dosaou) values ('{((BasicMemberInfo)membersListView.SelectedItem).Id}','{newTraining.GetStartTimeInMySqlFormat()}')";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;

            if (cmd.ExecuteNonQuery() == -1)
            {
                MessageBox.Show("Failed to start a training.");
            }
            else
            {
                trainingsListView.ItemsSource = null;
                trainingsListView.ItemsSource = trainings;
            }
            teretanaDB.Close();
        }
    }
}
