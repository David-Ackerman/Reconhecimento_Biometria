using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APS6.model
{
    public class Conexao
    {

        SqlConnection con = new SqlConnection();
        public Conexao()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "fdb16.awardspace.net:3306/3233506_david";
            builder.UserID = "3233506_david";
            builder.Password = "DAvi8699";
            //Console.WriteLine("conectando");
            //builder.InitialCatalog = "APS6";
            //con.ConnectionString = builder.ConnectionString; //@"Driver={ODBC Driver 13 for SQL Server};server=tcp:david-srv-bd.database.windows.net,1433;database=APS5;uid=david-bd-adm;encrypt=yes;trustservercertificate=no;connection timeout=30";
             con.ConnectionString = @"Data Source=DESKTOP-IOAIB43\SQLEXPRESS;Initial Catalog=APS6;Integrated Security=True";
            //System.out.println("conectou");
        }
        public SqlConnection conectar()
        {
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
            return con;
        }
        public void desconectar()
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }
    }
}
