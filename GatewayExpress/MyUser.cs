using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Data;
using System.Threading;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace GatewayExpress
{
    class MyUser
    {

        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string DRIVERSID { get; set; }
        public string GIVEN_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string BALANCE { get; set; }
        public string CITY_ADDRESS { get; set; }
        public string DATE_AND_TIME { get; set; }
        public string BIRTHDATE { get; set; }
        public string DATE { get; set; }
        public string TIME { get; set; }
        public string GENDER { get; set; }
        public string HOME_ADDRESS { get; set; }
        public string LICENSE_NUMBER { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string NATIONALITY { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string TELEPHONE_NUMBER { get; set; }
        public string AMOUNT { get; set; }
        public string ENTRY_POINT { get; set; }
        public string EXIT_POINT { get; set; }
        public string COUNTER { get; set; }
        public string LCOUNTER { get; set; }
        public string OR_NUMBER { get; set; }
        public string REFERENCE_NUMBER { get; set; }
        public string FROM { get; set; }
        public string FROM_USERNAME { get; set; }
        public string TYPE { get; set; }
        public string ADMIN_COUNTER { get; set; }
        public string ID { get; set; }
        public string ID_NUMBER { get; set; }
        public string STATUS { get; set; }
        public string ACCOUNT_STATUS { get; set; }
        public string USER_COUNTER { get; set; }
        public string PURPOSE { get; set; }
        

        private static string error = "there was some error";

        public static void ShowError()
        {
            System.Windows.Forms.MessageBox.Show(error);
        }
            

        public static bool IsEqual(MyUser user1, MyUser user2)
        {
            
            //System.Windows.Forms.MessageBox.Show(USERNAME.ToString());
            if (user1 == null || user2 == null) { return false; }

            if (user1.USERNAME != user2.USERNAME)
            {
                error = "Username does not exist!";
                return false;
            }
            else if (user1.PASSWORD != user2.PASSWORD)
            {
                error = "Username and Password does not match!";
                return false;
            }
            return true;
        }

    }

}
