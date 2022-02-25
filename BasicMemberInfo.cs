using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teretana
{
    internal class BasicMemberInfo
    {
        public BasicMemberInfo()
        {

        }
        public BasicMemberInfo(int id, string name, string surname, DateTime dateTime)
        {
            Id = id;
            Name = name;
            Surname = surname;
            DateTime = dateTime;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateTime { get; set; }
        public string JMBG { get; set; }
        public int PostanskiBroj { get; set; }
        public string Email { get; set; }
    }
}
