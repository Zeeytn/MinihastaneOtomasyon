using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minihastaneotomasyonu
{
    public partial class GirisPaneli : Form
    {
        public GirisPaneli()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HastaGiris h = new HastaGiris();
            h.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SekreterGiris s = new SekreterGiris();
            s.Show();
            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DoktorGiris d = new DoktorGiris();
            d.Show();
            
        }
    }
}
