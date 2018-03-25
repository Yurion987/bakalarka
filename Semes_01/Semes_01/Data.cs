using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using HtmlAgilityPack;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Semes_01
{

    class LoadData
    {

        List<List<string>> tabulkaZoSuboru;
        IWebDriver driver;
        List<Zaznam> tabulkaZWebStranky;

        public List<List<string>> TabulkaZoSuboru { get => tabulkaZoSuboru; set => tabulkaZoSuboru = value; }
        public List<Zaznam> TabulkaZWebStranky { get => tabulkaZWebStranky; set => tabulkaZWebStranky = value; }

        public LoadData()
        {
            FirefoxOptions fo = new FirefoxOptions();
            fo.AddArgument("--headless");
            driver = new FirefoxDriver(fo);
            this.tabulkaZWebStranky = new List<Zaznam>();
            this.tabulkaZoSuboru = new List<List<string>>();

        }

        public void parsingTable()
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            string html = System.IO.File.ReadAllText(@"C:\Users\Yurion\source\repos\Semes_01\Semes_01\doch_november.html");
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
        public void rozparsuj_WebStranku()
        {
            //posledny je button
            var sekciaPrichodov = driver.FindElements(By.ClassName("section"));

            for (int i = 0; i < sekciaPrichodov.Count - 1; i++)
            {
                //dostanie datumu zo stranky
                var datum = sekciaPrichodov[i].FindElement(By.ClassName("before_day")).Text;
                var datumMin = sekciaPrichodov[i].FindElement(By.ClassName("small_day")).Text;

                foreach (IWebElement konkretnyZaznam in sekciaPrichodov[i].FindElements(By.ClassName("ON")))
                {
                    string meno = konkretnyZaznam.FindElement(By.ClassName("name")).Text;
                    string cas = konkretnyZaznam.FindElement(By.ClassName("time")).Text;
                    if (datumMin != "")
                    {

                        tabulkaZWebStranky.Add(new Zaznam(meno, formatuj_Datum(datumMin), cas , ""));
                    }
                    else
                    {
                        tabulkaZWebStranky.Add(new Zaznam(meno, formatuj_Datum(datum), cas , ""));
                    }
                }

            }
        }
        public void nacitajStranku()
        {
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
        public string formatuj_Datum(string datum) {
            datum = Regex.Replace(datum, "[^A-Za-z0-9 ]", "");
            var originDatum = datum.Split(' ');
            if (originDatum[0].Length != 2) originDatum[0] = "0" + originDatum[0];
            switch (originDatum[1])
            {
                case "January":
                    originDatum[1] = "01";
                    break;
                case "February":
                    originDatum[1] = "02";
                    break;
                case "March":
                    originDatum[1] = "03";
                    break;
                case "April":
                    originDatum[1] = "04";
                    break;
                case "May":
                    originDatum[1] = "05";
                    break;
                case "June":
                    originDatum[1] = "06";
                    break;
                case "July":
                    originDatum[1] = "07";
                    break;
                case "August":
                    originDatum[1] = "08";
                    break;
                case "September":
                    originDatum[1] = "09";
                    break;
                case "October":
                    originDatum[1] = "10";
                    break;
                case "November":
                    originDatum[1] = "11";
                    break;
                case "December":
                    originDatum[1] = "12";
                    break;
                default:
                    originDatum[1] = "00";
                    break;
            }
            string celyDatum = "";
            for (int i = 0; i < originDatum.Length; i++)
            {
                if (i < 2)
                {
                    celyDatum += originDatum[i] + ".";
                }
                else {
                    celyDatum += originDatum[i];
                    break;
                }

            }

            return celyDatum;
        }
        public void naplnTypZaznamu() {
            string poslednyDen = tabulkaZWebStranky[tabulkaZWebStranky.Count - 1].Datum.Substring(0, 2);
            int kolkoVymazadOdKonca = 0;
            for (int i = tabulkaZWebStranky.Count - 1; i >= 0; i--)
            {
                if (tabulkaZWebStranky[i].Datum.Substring(0, 2).Equals(poslednyDen)) {
                    kolkoVymazadOdKonca++;
                }
                else
                {
                    break;
                }

            }
            tabulkaZWebStranky.RemoveRange(tabulkaZWebStranky.Count - kolkoVymazadOdKonca, kolkoVymazadOdKonca);
            tabulkaZWebStranky.Reverse();

            for (int i = 0; i < tabulkaZWebStranky.Count; i++)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (tabulkaZWebStranky[i].Meno.Equals(tabulkaZWebStranky[j].Meno) && tabulkaZWebStranky[j].Typ.Equals("odchod"))
                    {
                        tabulkaZWebStranky[i].Typ = "prichod";
                        break;
                    }
                    else if (tabulkaZWebStranky[i].Meno.Equals(tabulkaZWebStranky[j].Meno) && tabulkaZWebStranky[j].Typ.Equals("prichod"))
                    {
                        tabulkaZWebStranky[i].Typ = "odchod";
                        break;
                    }
                    else if (!tabulkaZWebStranky[i].Datum.Equals(tabulkaZWebStranky[j].Datum))
                    {
                        break;
                    }

                }
                if (tabulkaZWebStranky[i].Typ.Equals(""))
                {
                    tabulkaZWebStranky[i].Typ = "prichod";
                }

            }
            Console.WriteLine("\n");
            for (int i = 0; i < tabulkaZWebStranky.Count; i++)
            {
                if (i != 0 && !tabulkaZWebStranky[i].Datum.Equals(tabulkaZWebStranky[i - 1].Datum)) Console.WriteLine("\n" + tabulkaZWebStranky[i].Datum);
                if (i == 0) Console.WriteLine(tabulkaZWebStranky[i].Datum);

                Console.WriteLine(tabulkaZWebStranky[i].Meno + " " + tabulkaZWebStranky[i].Cas + " " + tabulkaZWebStranky[i].Typ);
            }

        }
        public void naplnSekundy ()
        {
            for (int i = 1; i < tabulkaZWebStranky.Count; i++)
            {
                int sekundy = Int32.Parse(tabulkaZWebStranky[i].Cas.Substring(tabulkaZWebStranky[i].Cas.Length - 2,2));
                if (sekundy == Int32.Parse(tabulkaZWebStranky[i-1].Cas.Substring(tabulkaZWebStranky[i-1].Cas.Length - 2, 2)))
                {

                }
            }
        }
    }
}
