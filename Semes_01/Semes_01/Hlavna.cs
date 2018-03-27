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
            Application.Run(new GUI());
           
        }
    }
}
