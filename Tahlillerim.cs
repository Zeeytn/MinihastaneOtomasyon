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
    public partial class Tahlillerim : Form
    {
        public Tahlillerim()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");
        public string HastaTC;
        private void Tahlillerim_Load(object sender, EventArgs e)
        {
            VeriYukle();
        }

        private void VeriYukle()
        {
            try
            {
                // Veritabanı bağlantısını aç
                baglanti.Open();

                // Parametreli sorgu
                string query = @"
            SELECT 
             
                t.TahlilTürü AS 'Tahlil Türü',
                t.TahlilTarihi AS 'Tahlil Tarihi',
                d.Ad + ' ' + d.Soyad AS 'Doktor Adı Soyadı'
            FROM tbl_tahliller t
            INNER JOIN tbl_hastalar h ON t.HastaID = h.ID
            INNER JOIN tbl_doktorlar d ON t.DoktorID = d.ID
            WHERE h.TC = @tc";
                // "SELECT t.TahlilTürü, t.TahlilTarihi, t.TahlilSonucu " +
                // "FROM tbl_tahliller t " +
                // "INNER JOIN tbl_hastalar h ON t.HastaID = h.ID " +
                // "WHERE h.TC = @tc";

                // SqlCommand oluşturun ve parametreyi ekleyin
                SqlCommand komut = new SqlCommand(query, baglanti);
                komut.Parameters.AddWithValue("@tc", HastaTC); // HastaTC değişkenini kullanarak parametreyi ekleyin

                // SqlDataAdapter kullanarak veriyi çekin
                SqlDataAdapter veriler = new SqlDataAdapter(komut); // SqlCommand nesnesini SqlDataAdapter'a veriyoruz
                DataTable sakla = new DataTable();
                veriler.Fill(sakla);

                // DataGridView'i doldur
                dataGridView1.DataSource = sakla;
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // Veritabanı bağlantısını kapat
                baglanti.Close();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
