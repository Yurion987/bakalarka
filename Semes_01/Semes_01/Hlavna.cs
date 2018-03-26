using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Semes_01
{
    class Hlavna
    {
        [STAThread]
        static void Main(string[] args)
        {

            // LoadData p = new LoadData();
            // Databazka d = new Databazka();
             // p.nacitajStranku();
            //  p.parsingTable();
            //  p.rozparsuj_WebStranku();
            //  p.naplnTypZaznamu();          
            //   d.insertData(p.TabulkaZWebStranky);
            //  p.driver.Quit();

            Application.Run(new GUI());
           
        }
    }
}
