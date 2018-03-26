using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            fileDialog.Filter = "html files (*.html)|*.html|excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            fileDialog.ShowDialog();
            string nazovSuboru = fileDialog.FileName;    
            ld.parsingTable(nazovSuboru);
            ld.naplnTypZaznamuSubor();
            db.insertData(ld.TabulkaZoSuboru);
            MessageBox.Show("uspesne pridane data do databazy");
            ld.clearData();

        }

        private void label3_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            ld.nacitajStranku();
            MessageBox.Show("stranka uspesne nacitana");
            ld.rozparsuj_WebStranku();
            MessageBox.Show("stranka uspesne rozparsovana");
            ld.naplnTypZaznamuWeb();
            db.insertData(ld.TabulkaZWebStranky);
            MessageBox.Show("stranka insertnute");
            ld.clearData();
        }
    }
}
