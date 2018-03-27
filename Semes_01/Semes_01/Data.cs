using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using HtmlAgilityPack;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;

namespace Semes_01
{

    class LoadData
    {

       private List<Zaznam> tabulkaZoSuboru;
       private List<Zaznam> tabulkaZWebStranky;
       private IWebDriver driver;

        public List<Zaznam> TabulkaZoSuboru { get => tabulkaZoSuboru; set => tabulkaZoSuboru = value; }
        public List<Zaznam> TabulkaZWebStranky { get => tabulkaZWebStranky; set => tabulkaZWebStranky = value; }
        public IWebDriver Driver { get => driver; set => driver = value; }

        public LoadData()
        {
            this.tabulkaZWebStranky = new List<Zaznam>();
            this.tabulkaZoSuboru = new List<Zaznam>();
            driver = null;

        }

        public void parsingTable(string cesta)
        {


                HtmlDocument htmlDoc = new HtmlDocument();
                string html = System.IO.File.ReadAllText(@cesta);
                htmlDoc.LoadHtml(html);
                foreach (HtmlNode row in htmlDoc.DocumentNode.SelectNodes("/html/body/table/tr").Skip(1))
                {
                    int stop = 0;
                    string meno = "";
                    string cas = "";
                    foreach (HtmlNode cell in row.SelectNodes("th|td").Skip(1))
                    {
                        if (stop != 2)
                        {
                            string bunka = cell.InnerHtml;
                            if (stop == 1)
                            {
                                if (!bunka.Contains("ústredňa"))
                                {
                                    meno = bunka.Substring(bunka.IndexOf(":") + 2);
                                    // bunka.Length - bunka.IndexOf(":") - 2
                                }
                            }
                            else
                            {
                                cas = bunka;
                            }

                            stop++;
                        }
                        else
                        {
                            if (!meno.Equals(""))
                            {
                                string datum = formatuj_Datum(cas.Substring(0, cas.IndexOf(":") - 2));
                                cas = cas.Substring(cas.Length - 8, 5);
                                if (cas[0] == ' ') cas = "0" + cas.Substring(1);
                                tabulkaZoSuboru.Add(new Zaznam(meno, datum, cas, ""));
                                break;
                            }

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
            if (driver == null) {
                var cestaDoGecko = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                FirefoxOptions fo = new FirefoxOptions();
                fo.AddArgument("--headless");
                driver = new FirefoxDriver(cestaDoGecko, fo);
                driver.Navigate().GoToUrl("https://www.jablonet.net/");
                driver.FindElement(By.Id("login-opener")).Click();
                driver.FindElement(By.Id("login-email")).SendKeys("uhrin9@stud.uniza.sk");
                driver.FindElement(By.Id("login-heslo")).SendKeys("DGbfhk");
                driver.FindElement(By.Id("loginButton")).Click();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
                driver.Navigate().GoToUrl("https://www.jablonet.net/app/ja100?service=257168");
                wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("more_info_icon")));
            }

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
                case "1":
                    originDatum[1] = "01";
                    break;
                case "February":
                case "2":
                    originDatum[1] = "02";
                    break;
                case "March":
                case "3":
                    originDatum[1] = "03";
                    break;
                case "April":
                case "4":
                    originDatum[1] = "04";
                    break;
                case "May":
                case "5":
                    originDatum[1] = "05";
                    break;
                case "June":
                case "6":
                    originDatum[1] = "06";
                    break;
                case "July":
                case "7":
                    originDatum[1] = "07";
                    break;
                case "August":
                case "8":
                    originDatum[1] = "08";
                    break;
                case "September":
                case "9":
                    originDatum[1] = "09";
                    break;
                case "October":
                case "10":
                    originDatum[1] = "10";
                    break;
                case "November":
                case "11":
                    originDatum[1] = "11";
                    break;
                case "December":
                case "12":
                    originDatum[1] = "12";
                    break;
                default:
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
        public void naplnTypZaznamuWeb() {
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
        }
        public void naplnTypZaznamuSubor() {
            for (int i = 0; i < TabulkaZoSuboru.Count; i++)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (TabulkaZoSuboru[i].Meno.Equals(TabulkaZoSuboru[j].Meno) && TabulkaZoSuboru[j].Typ.Equals("odchod"))
                    {
                        TabulkaZoSuboru[i].Typ = "prichod";
                        break;
                    }
                    else if (TabulkaZoSuboru[i].Meno.Equals(TabulkaZoSuboru[j].Meno) && TabulkaZoSuboru[j].Typ.Equals("prichod"))
                    {
                        TabulkaZoSuboru[i].Typ = "odchod";
                        break;
                    }
                    else if (!TabulkaZoSuboru[i].Datum.Equals(TabulkaZoSuboru[j].Datum))
                    {
                        break;
                    }

                }
                if (TabulkaZoSuboru[i].Typ.Equals(""))
                {
                    TabulkaZoSuboru[i].Typ = "prichod";
                }

            }
        }
        public void clearData() {
            TabulkaZoSuboru.Clear();
            tabulkaZWebStranky.Clear();
        }
    }
}
