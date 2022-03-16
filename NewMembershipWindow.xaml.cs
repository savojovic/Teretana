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
    /// Interaction logic for NewMembershipWindow.xaml
    /// </summary>
    public partial class NewMembershipWindow : Window
    {
        int memberId;
        string memberName;
        const int MEMBERSHIP_PRICE = 30;
        MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);


        public NewMembershipWindow(int memberId)
        {
            InitializeComponent();
            SetBtnStyles();
        }
        private void SetBtnStyles()
        {
            saveBtn.Style = SettingsWindow.btnStyle;
            cancelButton.Style = SettingsWindow.btnStyle;
        }
        public NewMembershipWindow(int memberId, string memberName, bool isEdit)
        {
            InitializeComponent();
            SetBtnStyles();
            this.memberId = memberId;
            this.memberName = memberName;
            nameLbl.Content = memberName;
            if (isEdit)
            {
                SetExistingMembershipInfo();
            }
            else
            {
                SetDefaultMembershipInfo();
            }
        }
        private void SetExistingMembershipInfo()
        {
            
            teretanaDB.Open();
            string querry = $"select iznosclanarine, popust, datumuplate, trajanjeclanarine from clanarina where Clan_IdOsoba='{memberId}'";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;
            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();

            priceTextBox.Text = reader.GetDouble(0).ToString();
            discountTextBox.Text = reader.GetDouble(1).ToString();
            paidAtDatePicker.Text = reader.GetDateTime(2).ToShortDateString();
            validForDaysTextBox.Text = reader.GetString(3);
            teretanaDB.Close();
        }
        private void SetDefaultMembershipInfo()
        {
            discountTextBox.Text = "0";
            paidAtDatePicker.Text = DateTime.Now.ToShortDateString();
            validForDaysTextBox.Text = "31";
            priceTextBox.Text = "30";
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            double discount = int.Parse(discountTextBox.Text);
            string date = String.Format("{0:yyyy/MM/dd}", paidAtDatePicker.SelectedDate).Replace('/','-');
            int numOfValidDays = int.Parse(validForDaysTextBox.Text);
            int price = int.Parse(priceTextBox.Text);

            teretanaDB.Open();
            string querry = $"insert into clanarina (iznosclanarine, popust, datumuplate, trajanjeclanarine, clan_idosoba) " +
                $"values ('{price}','{discount}','{date}','{numOfValidDays}','{memberId}')" +
                $"ON DUPLICATE KEY UPDATE `iznosclanarine`='{price}', `popust`='{discount}', `datumuplate`='{date}', `trajanjeclanarine`='{numOfValidDays}', `clan_idosoba`='{memberId}'";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;

            if (cmd.ExecuteNonQuery() == -1)
            {
                MessageBox.Show("Failure. Try again.");
            }
            else
            {
                MessageBox.Show("Changes saved succesfully.");
                this.Close();
            }

            //TODO: get all fields and save them into db
            //TODO: get changes entered in dicount and calculate new price
        }

        private void discountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double discount = double.Parse(discountTextBox.Text);
                if (discount >= 0)
                {
                    double newPrice = MEMBERSHIP_PRICE - (MEMBERSHIP_PRICE * (discount / 100));
                    if(newPrice>=0)
                        priceTextBox.Text = newPrice.ToString();
                }
            }
            catch (FormatException)
            {

            }
           
        }
    }
}