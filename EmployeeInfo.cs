using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teretana
{
    internal class EmployeeInfo : BasicMemberInfo
    {
        DateTime employmentDate;
        double salary;
        int contractDuration;
        string username;

        public EmployeeInfo(int id, string name, string surname, DateTime dateOfBirth):base(id,name,surname, dateOfBirth)
        {

        }
    }
}
