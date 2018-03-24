using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semes_01
{
    class Databazka
    {
        OracleConnection conection;
        public Databazka()
        {
            string conStr = "Data Source=(DESCRIPTION =(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST = obelix.fri.uniza.sk)(PORT = 1521)))"
                + "(CONNECT_DATA =(SERVICE_NAME = orcl2.fri.uniza.sk)));User ID=uhrin9;Password=dodo2331996;";
            this.conection = new OracleConnection(conStr);
            this.conection.Open();
        }
        private void insertData(string tabulka) {
            
        }
        private bool kontrolaOriginality(string tabulka,string stlpec, string data) {

            string comand = "select * from "+tabulka+" where " + stlpec + "= '"+data+"'";
            OracleCommand orclCom = new OracleCommand(comand,conection);

            OracleDataReader orclReader = orclCom.ExecuteReader();
            if(orclReader.HasRows){
                return true;
            }
            return false;
        }
    }
}
