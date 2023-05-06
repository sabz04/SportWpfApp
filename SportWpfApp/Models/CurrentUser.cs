using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportWpfApp.Models
{
    public static  class CurrentUser
    {
        public static User UserCurrent { get; set; }

        public static String GetCreds()
        {
            return $"{UserCurrent.Name} {UserCurrent.Patronymic} {UserCurrent.Surname}  ";

        }
    }
}
