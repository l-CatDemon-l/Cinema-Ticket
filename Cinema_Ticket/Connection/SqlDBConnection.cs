using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Ticket.Connection
{
    class SqlDBConnection
    {
        public static string connection = 
                  "Network Library=DBMSSOCN;" +
                  "Data Source=192.168.50.71,1433;" +
                  "Initial Catalog=CinemaTicketDB;" +
                  "User Id=admin;" +
                  "Password=admin;";
        /*public static string connection =
          "Network Library=DBMSSOCN;" +
          "Data Source=192.168.1.66,1433;" +
          "Initial Catalog=CinemaTicketDB;" +
          "User Id=admin;" +
          "Password=admin;";*/
    }
}
