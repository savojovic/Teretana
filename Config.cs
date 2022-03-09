using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teretana
{
    internal class Config
    {
        public static string DB_SERVER = "localhost";
        public static string DB_USERNAME = "root";
        public static string DB_PASSWORD = "admin";
        public static string DB_NAME = "Teretana";
        public static string dbConfigString = $"server={Config.DB_SERVER};userid={Config.DB_USERNAME};password={Config.DB_PASSWORD};database={Config.DB_NAME}";
        public static string DEFAULT_AVATAR_IMG_PATH = "C:\\Users\\jsavic\\Documents\\FaxProjects\\HCI\\WPF_Teretana\\Teretana\\Teretana\\assets\\avatar.png";
    }
}
