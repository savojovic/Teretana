﻿using MySqlConnector;
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
        MySqlConnection teretanaDB = new MySqlConnection(Config.dbConfigString);


        public NewMembershipWindow(int memberId)
        {
            InitializeComponent();

        }
        public NewMembershipWindow(int memberId, string memberName, bool isEdit)
        {
            InitializeComponent();
            this.memberId = memberId;
            this.memberName = memberName;
            if (isEdit)
            {
                SetDefaultMembershipInfo();
            }
            else
            {
                SetExistingMembershipInfo();
            }
        }
        private void SetExistingMembershipInfo()
        {

        }
        private void SetDefaultMembershipInfo()
        {
            nameLbl.Content = memberName;
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
                $"values ('{price}','{discount}','{date}','{numOfValidDays}','{memberId}')";
            MySqlCommand cmd = teretanaDB.CreateCommand();
            cmd.CommandText = querry;

            if (cmd.ExecuteNonQuery() == -1)
            {
                MessageBox.Show("Failure. Try again.");
            }
            else
            {
                MessageBox.Show("New membership created succesfully.");
                this.Close();
            }

            //TODO: get all fields and save them into db
            //TODO: get changes entered in dicount and calculate new price
        }
    }
}