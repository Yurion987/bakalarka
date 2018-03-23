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

        List<List<string>> tabulkaZoSuboru;
        IWebDriver driver;
        List<string> tabulkaZWebStranky;
        public Program()
        {
            FirefoxOptions fo = new FirefoxOptions();
            fo.AddArgument("--headless");
            driver = new FirefoxDriver(fo);
            this.tabulkaZWebStranky = new List<string>();
            this.tabulkaZoSuboru = new List<List<string>>();
           
        }

        static void Main(string[] args)
        {

            Program p = new Program();
            p.nacitajStranku();
            p.parsingTable();
            p.rozparsuj_WebStranku();
            p.driver.Quit();
            Console.ReadLine();
        }

        public void parsingTable()
        {
            HtmlDocument htmlDoc;
            string html = System.IO.File.ReadAllText(@"C:\Users\Yurion\source\repos\Semes_01\Semes_01\doch_november.html");
            htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            foreach (HtmlNode row in htmlDoc.DocumentNode.SelectNodes("/html/body/table/tr").Skip(1))
            {
                tabulkaZoSuboru.Add(new List<string>());
                int stop = 0;
                foreach (HtmlNode cell in row.SelectNodes("th|td"))
                {
                    if (stop != 3)
                    {
                        string bunka = cell.InnerHtml;
                        if (stop == 2)
                        {
                            bunka = bunka.Substring(9, bunka.Length - 9);
                            string ID = bunka.Substring(0, bunka.IndexOf(":"));
                            tabulkaZoSuboru[tabulkaZoSuboru.Count - 1].Add(ID);
                            bunka = bunka.Substring(bunka.IndexOf(" "), bunka.Length - bunka.IndexOf(" "));
                        }
                        tabulkaZoSuboru[tabulkaZoSuboru.Count - 1].Add(bunka);

                        stop++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        public void rozparsuj_WebStranku() {
            //posledny je button
            var clasSec = driver.FindElements(By.ClassName("section"));
           
            for (int i = 0; i < clasSec.Count - 1; i++)
            {
                //dostanie datumu zo stranky
                var datum = clasSec[i].FindElement(By.ClassName("before_day")).Text;
                var datumMin = clasSec[i].FindElement(By.ClassName("small_day")).Text;
                foreach (var item in clasSec[i].FindElements(By.Id("timeline")))
                {

                    foreach (var item2 in item.FindElements(By.ClassName("ON")))
                    {

                        string meno = item2.FindElement(By.ClassName("name")).Text;
                        string cas = item2.FindElement(By.ClassName("time")).Text;
                        if (datumMin != "")
                        {
                            tabulkaZWebStranky.Add(datumMin);
                            tabulkaZWebStranky.Add(cas);
                            tabulkaZWebStranky.Add(meno);
                        }
                        else
                        {
                            tabulkaZWebStranky.Add(datum);
                            tabulkaZWebStranky.Add(cas);
                            tabulkaZWebStranky.Add(meno);
                        }
                    }
                }
            }
            foreach (var item in tabulkaZWebStranky)
            {
                Console.WriteLine(item);
            }
        }
        public void nacitajStranku() {
            int pocetLoadZaznamov = 4;

            driver.Navigate().GoToUrl("https://www.jablonet.net/");
            driver.FindElement(By.Id("login-opener")).Click();
            driver.FindElement(By.Id("login-email")).SendKeys("uhrin9@stud.uniza.sk");
            driver.FindElement(By.Id("login-heslo")).SendKeys("DGbfhk");
            driver.FindElement(By.Id("loginButton")).Click();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
            driver.Navigate().GoToUrl("https://www.jablonet.net/app/ja100?service=257168");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("more_info_icon")));
            for (int i = 0; i < pocetLoadZaznamov; i++)
            {
                driver.FindElement(By.ClassName("more_info_icon")).Click();
                System.Threading.Thread.Sleep(1500);
            }
        }
    }
}
