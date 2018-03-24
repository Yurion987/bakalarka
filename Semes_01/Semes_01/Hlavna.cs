using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semes_01
{
    class Hlavna
    {
        static void Main(string[] args)
        {
             LoadData p = new LoadData();
            p.nacitajStranku();
            //  p.parsingTable();
            p.rozparsuj_WebStranku();
            p.naplnTypZaznamu();
            //  p.driver.Quit();
           // Databazka d = new Databazka();
            Console.ReadLine();
        }
    }
}
