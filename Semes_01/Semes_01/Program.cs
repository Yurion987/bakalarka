using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using HtmlAgilityPack;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;

namespace Semes_01
{

    class Program
    {
        List<List<string>> tabulka;
        HtmlDocument htmlDoc;
        IWebDriver driver;
        public Program()
        {
            FirefoxOptions fo = new FirefoxOptions();
           // fo.AddArgument("--headless");
           driver = new FirefoxDriver(fo);
            this.tabulka = new List<List<string>> ();
            string html = System.IO.File.ReadAllText(@"C:\Users\Yurion\source\repos\Semes_01\Semes_01\doch_november.html");
            this.htmlDoc = new HtmlDocument();
            this.htmlDoc.LoadHtml(html);
        }




        static void Main(string[] args)
        {

            Program p = new Program();
            // ZISKANIE HTML CODU A ULOZENIE DO KONKRETNEHO SUBORU

            //  WebClient wb = new WebClient();
            // wb.DownloadFile("https://www.arclab.com/en/kb/csharp/download-file-from-internet-to-string-or-file.html", @"C:\Users\Yurion\source\repos\Semes_01\Semes_01\testDownload");
            //string text = System.IO.File.ReadAllText(@"C:\Users\Yurion\source\repos\Semes_01\Semes_01\doch_november.html");

            //NEVIEM MOZNO AJ FUNGUJE

            // doc.Load(@"C:\Users\Yurion\source\repos\Semes_01\Semes_01\doch_november.html");
            // var web = new HtmlWeb();
            // var htmlDoc = new HtmlDocument();
            //var doc = web.Load("https://www.arclab.com/en/kb/csharp/download-file-from-internet-to-string-or-file.html");
            // htmlDoc.LoadHtml(text);

           

             //IST NA WEB STRANKU
             p.driver.Navigate().GoToUrl("https://www.jablonet.net/");
             p.driver.FindElement(By.Id("login-opener")).Click();
             p.driver.FindElement(By.Id("login-email")).SendKeys("uhrin9@stud.uniza.sk");
             p.driver.FindElement(By.Id("login-heslo")).SendKeys("DGbfhk");
             p.driver.FindElement(By.Id("loginButton")).Click();
             WebDriverWait wait = new WebDriverWait(p.driver, TimeSpan.FromSeconds(50));
             p.driver.Navigate().GoToUrl("https://www.jablonet.net/app/ja100?service=257168");
            
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("more_info_icon")));
            
            for (int i = 0; i < 10; i++)
            {
                p.driver.FindElement(By.ClassName("more_info_icon")).Click();
                System.Threading.Thread.Sleep(1000);
            }

            // p.driver.Quit();
            p.parsingTable();

        }

        public void parsingTable()
        {
            foreach (HtmlNode row in htmlDoc.DocumentNode.SelectNodes("/html/body/table/tr").Skip(1))
            {
                tabulka.Add(new List<string>());
                int stop = 0;
                foreach (HtmlNode cell in row.SelectNodes("th|td"))
                {
                    if (stop != 3)
                    {
                        string bunka = cell.InnerHtml;
                        if (stop == 2) {
                            bunka = bunka.Substring(9, bunka.Length-9);
                            string ID = bunka.Substring(0,bunka.IndexOf(":"));
                            tabulka[tabulka.Count - 1].Add(ID);
                            bunka = bunka.Substring(bunka.IndexOf(" "), bunka.Length - bunka.IndexOf(" "));
                        }
                        tabulka[tabulka.Count - 1].Add(bunka);
                        
                        stop++;                
                    }
                    else {
                        break;
                    }
                }
            }
        }
    }
}
