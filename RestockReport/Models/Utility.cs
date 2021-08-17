using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RestockReport.Models
{
    public class Utility
    {
        static String constr = ConfigurationManager.ConnectionStrings["MediaFileEntities"].ConnectionString.ToString();
        SqlConnection objConnection = new SqlConnection(constr);
        public int InsertUpdate(String qry)
        {
            SqlCommand cmd = new SqlCommand(qry, objConnection);
            try
            {
                objConnection.Open();
                cmd.ExecuteNonQuery();
                objConnection.Close();
                return 1;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }


        }
       
    }
}