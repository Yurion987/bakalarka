using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Windows;

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
            try
            {
                this.conection = new OracleConnection(conStr);
                this.conection.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("nepodarilo sa napojit na databazu");

                System.Windows.Forms.Application.Exit();
            }

        }
        public void insertData(List<Zaznam> data)
        {
            OracleCommand orclCom = new OracleCommand();
            orclCom.Connection = conection;
            OracleDataReader orclReader = null;
            foreach (var zaznam in data)
            {

                if (kontrolaOriginalityMena(zaznam.Meno))
                {
                    orclCom.CommandText = "insert into pouzivatel (meno,heslo) values('" + zaznam.Meno + "',' ')";
                    orclCom.ExecuteNonQuery();
                }

                orclCom.CommandText = "select id_pouzivatela from pouzivatel where meno = '" + zaznam.Meno + "'";
                orclReader = orclCom.ExecuteReader();
                int ID = -1;
                if (orclReader.Read())
                {
                    ID = Int32.Parse(orclReader["id_pouzivatela"].ToString());
                }
                if (ID != -1 && kontrolaOriginalityZaznamu(zaznam.Cas, zaznam.Datum, ID))
                {
                    orclCom.CommandText = "insert into zaznam (pouzivatel,cas,typ) " +
                        "values (" + ID + ",TO_DATE('" + zaznam.Datum + " " + zaznam.Cas + "','dd.mm.yyyy hh24:mi'),'" + zaznam.Typ + "')";
                    orclCom.ExecuteNonQuery();

                }
                orclReader.Close();
            }


        }

        public void odpoj()
        {
            conection.Clone();
        }
        private bool kontrolaOriginalityMena(string data)
        {

            string comand = "select * from pouzivatel where meno = '" + data + "'";
            OracleCommand orclCom = new OracleCommand(comand, conection);

            OracleDataReader orclReader = orclCom.ExecuteReader();
            if (orclReader.HasRows)
            {
                orclReader.Close();
                return false;
            }
            orclReader.Close();
            return true;
        }
        private bool kontrolaOriginalityZaznamu(string cas, string datum, int ID)
        {
            string comand = "select * from zaznam where cas = TO_DATE('" + datum + " " + cas + "','dd.mm.yyyy hh24:mi') and pouzivatel = " + ID + "";
            OracleCommand orclCom = new OracleCommand(comand, conection);

            OracleDataReader orclReader = orclCom.ExecuteReader();
            if (orclReader.HasRows)
            {
                orclReader.Close();
                return false;
            }
            orclReader.Close();
            return true;
        }
    }
}
