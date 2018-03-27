using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Semes_01
{
    public partial class GUI : Form
    {
        private LoadData ld;
        private Databazka db;
        public GUI()
        {
            ld = new LoadData();
            db = new Databazka();
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Browse";
            fileDialog.Filter = "html files (*.html)|*.html";
            fileDialog.ShowDialog();
            string nazovSuboru = fileDialog.FileName;
            if (!nazovSuboru.Equals(""))
            {
                ld.parsingTable(nazovSuboru);
                ld.naplnTypZaznamuSubor();
                MessageBox.Show("uspesne nacitanie zo suboru");
                db.insertData(ld.TabulkaZoSuboru);
                MessageBox.Show("uspesne pridane data do databazy");
                ld.clearData();

            }

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

            db.odpoj();
            if (ld.Driver != null)
            {
                ld.Driver.Close();
                ld.Driver.Quit();
            }
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            label2.Enabled = false;
            ld.nacitajStranku();
            MessageBox.Show("stranka uspesne nacitana");
            ld.rozparsuj_WebStranku();
            MessageBox.Show("stranka uspesne rozparsovana");
            ld.naplnTypZaznamuWeb();
            db.insertData(ld.TabulkaZWebStranky);
            MessageBox.Show("stranka insertnute");
            ld.clearData();
            label2.Enabled = true;
        }
    }
}
