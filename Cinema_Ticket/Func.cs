using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema_Ticket
{
    public class Func
    {
        public byte[] GetHashPassword(string pass)
        {
            byte[] tmpHash;
            tmpHash = ASCIIEncoding.ASCII.GetBytes(pass);

            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpHash);
            return tmpHash;
        }

        public bool CheckPassword(byte[] one, byte[] two)
        {
            bool bEqual = false;
            if (two.Length == one.Length)
            {
                int i = 0;
                while ((i < two.Length) && (two[i] == one[i]))
                {
                    i += 1;
                }
                if (i == two.Length)
                {
                    bEqual = true;
                }
            }

            if (bEqual)
                return true;
            else
                return false;
        }
    }
}