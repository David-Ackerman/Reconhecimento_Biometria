using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APS6.model;

namespace APS6.control

{
    public class Controle
    {
        public bool tem = false;
        public String mensagem = "";
        public static string esteUser;
        //SqlCommand cmd = new SqlCommand();
        public bool Acessar(String digital)
        {
            esteUser = digital;
            DaoComandos loginDao = new DaoComandos();
            tem = loginDao.verificarLogin(digital);
            //if (!loginDao.mensagem.Equals(""))
            //{
               // this.mensagem = loginDao.mensagem;
            //}
            return tem;
        }

        public String Cadastrar(String userName, String nome, String digital)
        {
            this.tem = false;
            DaoComandos cadastroDao = new DaoComandos();
            this.mensagem = cadastroDao.cadastrar(userName, nome, digital);
            if (cadastroDao.tem)
            {
                this.tem = true;
            }
            return mensagem;
        }
        public int NivelAcesso(string digital)
        {        
            DaoComandos acessoDao = new DaoComandos();
            return acessoDao.veriAcesso(digital);
        }
        public String Alterar(String userName, int nivel)
        {
            DaoComandos altera = new DaoComandos();
            return altera.alterarCadastro(userName, nivel); 
        }
        public String pegaUser(String digital)
        {
            DaoComandos user = new DaoComandos();
            String usuario = user.pegaUser(digital); ;
            return usuario;
        }
    }
}
