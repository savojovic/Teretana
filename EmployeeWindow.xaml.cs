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
    /// Interaction logic for EmployeeWindow.xaml
    /// </summary>
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
                members.Add(new BasicMemberInfo(reader.GetInt32(0),reader.GetString(1), reader.GetString(2), reader.GetDateTime(3)));
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
        private void SetMemberShip(int id)
        {

            teretanaDB.Open();
            string querry = $"select DatumUplate, TrajanjeClanarine from clanarina join clan on Clan.Clan_IdOsoba=Clanarina.Clan_IdOsoba where Clan.Clan_IdOsoba={id}";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText= querry;

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
        private void SetMemberInfo(ListView sender)
        {
            ListView? listView = sender;
            int idOsoba = ((BasicMemberInfo)listView.Items.CurrentItem).Id;

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
            string date = String.Format("{0:yyyy/MM/dd}", member.DateTime);

            string querry = $"insert into osoba (ime, prezime, datumrodjenja, jmbg, email,opstina_postanskibroj) " +
                $"values ('{member.Name}','{member.Surname}','{date}','{member.JMBG}','{member.Email}','{member.PostanskiBroj}')";
            teretanaDB.Open();
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            if (cmd.ExecuteNonQuery() == -1)
            {
                MessageBox.Show("Error Inserting new member");
            }
            teretanaDB.Close();
        }
        BasicMemberInfo GetNewMemberInfo()
        {
            BasicMemberInfo newMember = new BasicMemberInfo();
            newMember.Name = nameTextBox.Text;
            newMember.Surname = surnameTextBox.Text;
            newMember.JMBG = jmbgTextBox.Text;
            newMember.Email = emailTextBox.Text;
            newMember.DateTime = datePicker.SelectedDate.Value.Date;
            newMember.PostanskiBroj = GetPostalCode(citiesComboBox.SelectedItem.ToString());
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
            nameTextBox.Clear();
            surnameTextBox.Clear();
            datePicker.Text=String.Empty;
            jmbgTextBox.Clear();
            emailTextBox.Clear();
            paidAtLbl.Content = String.Empty;
            validUntilLbl.Content = String.Empty;
            daysLeftLbl.Content = String.Empty; 
            trainingsListView.ItemsSource = new List<string>();
            SetElementsLock(true);
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
    }
}
