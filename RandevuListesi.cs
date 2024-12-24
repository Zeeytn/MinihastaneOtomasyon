using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minihastaneotomasyonu
{
    public partial class RandevuListesi : Form
    {
        public RandevuListesi()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            randevular();
        }

        public void randevular()
        {
            baglanti.Open();
            string query = @"SELECT 
                        r.ID AS RandevuID,
                        h.Ad + ' ' + h.Soyad AS HastaAdSoyad,
                        d.Ad + ' ' + d.Soyad AS DoktorAdSoyad,
                          b.BransAd AS BransAdi,
                        r.RandevuTarihi AS RandevuTarihi,
                        r.Durum AS RandevuDurumu,
                       CASE 
                            WHEN r.RandevuTarihi < GETDATE() THEN 'Geçmiş' 
                    ELSE 'Aktif' 
                        END AS RandevuGecmis
                    FROM tbl_randevular r
                    INNER JOIN tbl_hastalar h ON r.HastaID = h.ID
                    INNER JOIN tbl_doktorlar d ON r.DoktorID = d.ID
                    INNER JOIN tbl_branslar b ON r.BransID = b.ID;";

            SqlDataAdapter da = new SqlDataAdapter(query, baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            baglanti.Close();
        }

    }
}
