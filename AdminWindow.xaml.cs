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
    }
}
