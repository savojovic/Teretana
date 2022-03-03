using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teretana
{
    internal class ExistingMembershipException : Exception
    {
        public ExistingMembershipException():base()
        {

        }
        public ExistingMembershipException(string msg):base(msg)
        {
            
        }

    }
}
