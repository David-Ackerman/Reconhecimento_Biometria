using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APS6.model
{
    class DaoComandos
    {
        public bool cadExiste = false;
        public bool tem = false;
        public String username = "";
        public String mensagem;
        SqlCommand cmd = new SqlCommand();
        Conexao con = new Conexao();
        SqlDataReader dr;

        public bool verificarLogin(String digital)
        {
            // Comandos sql para verifcar se usuario existe no banco
            cmd.CommandText = "select * from Usuarios where Username = @digital";
            cmd.Parameters.AddWithValue("@digital", "Ackerman");
            Console.WriteLine("verificando login");
            try
            {
                cmd.Connection = con.conectar();
                dr = cmd.ExecuteReader();
                username = dr.ToString();
                if (dr.HasRows)
                {
                    Console.WriteLine("achou linha");
                    
                    tem = true;
                }
                con.desconectar();
                dr.Close();
            }
            catch (SqlException)
            {

                this.mensagem = "Erro com o banco de dados";
            }
            return tem;
        }
        public string cadastrar(String userName, String name, String digital)
        {
            // Comandos para inserir usuarios no Banco de dados
            cmd.CommandText = "select * from Usuarios where Digital = @digital";
            cmd.Parameters.AddWithValue("@digital", digital);
            try
            {
                cmd.Connection = con.conectar();
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    cadExiste = true;
                }
                con.desconectar();
                dr.Close();
            }
            catch (SqlException)
            {
                this.mensagem = "Erro com o banco de dados";
            }

            if (!cadExiste)
            {
                tem = false;
                cmd.CommandText = "insert into Usuarios(Username, Digital, Nivel, Nome) values (@userName, @dig, 1, @nome);";
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@dig", digital);
                cmd.Parameters.AddWithValue("@nome", name);             
                try
                {
                    cmd.Connection = con.conectar();
                    dr = cmd.ExecuteReader();
                    con.desconectar();
                    tem = true;
                }
                catch (SqlException)
                {
                    this.mensagem = "Erro com o banco de dados";
                }
            }
            else
            {
                this.mensagem = "Você já está cadastrado";
                cadExiste = false;
            }

            return mensagem;
        }
        public String alterarCadastro(String userName, int acesso)
        {
            
            cmd.CommandText = "select * from Usuarios where Username= @userName";
            cmd.Parameters.AddWithValue("@userName", userName);
            try
            {
                cmd.Connection = con.conectar();
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    cadExiste = true;
                }
                con.desconectar();
                dr.Close();
            }
            catch (SqlException)
            {
                this.mensagem = "Erro com o banco de dados";
            }
            if (cadExiste)
            {
                tem = false;
                cmd.CommandText = " Update Usuarios set Nivel = @acesso where Username = @userName;";
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@acesso", acesso);
                try
                {
                    cmd.Connection = con.conectar();
                    cmd.ExecuteNonQuery();
                    con.desconectar();
                    this.mensagem = "Usuario atualizado com sucesso";
                    tem = true;
                }
                catch (SqlException)
                {
                    this.mensagem = "Erro com o banco de dados";
                }
            }
            else
            {
                this.mensagem = "Usuario não enontrado";
                cadExiste = false;
            }
            return mensagem;
        }
        public int veriAcesso (String digital)
        {
            int a = 0;
            cmd.CommandText = "select Nivel from Usuarios where Digital = @digital";
            cmd.Parameters.AddWithValue("@udigital", digital);
            try
            {
                cmd.Connection = con.conectar();
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    a = dr.GetInt16(1);                 
                }
                con.desconectar();
                dr.Close();
            }
            catch (SqlException)
            {
                this.mensagem = "Erro com o banco de dados";
            }
            return a;
        }
        public String pegaUser(String digital)
        {
            String user = "";
            cmd.CommandText = "select Username from Usuarios where Digital = @digital";
            cmd.Parameters.AddWithValue("@udigital", digital);
            try
            {
                cmd.Connection = con.conectar();
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    user = dr.ToString();
                    Console.WriteLine(user);
                }
                con.desconectar();
                dr.Close();
            }
            catch (SqlException)
            {
                this.mensagem = "Erro com o banco de dados";
            }
            return user;
        }
    }
}
