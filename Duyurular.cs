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
    public partial class Duyurular : Form
    {
        public Duyurular()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Duyurular_Load(object sender, EventArgs e)
        {
            VeriYukle();
            dataGridView1.Columns["ID"].HeaderText = "Duyuru No";
            dataGridView1.Columns["SekreterAdSoyad"].HeaderText = "Sekreter Adı Soyadı";
            dataGridView1.Columns["Duyuru"].HeaderText = "Duyuru";
            dataGridView1.Columns["DuyuruTarihi"].HeaderText = "Tarih";
        }
        private void VeriYukle()
        {


            try
            {
                // Veritabanı bağlantısını aç
                baglanti.Open();

                // Kullanıcıları getirmek için SQL sorgusu
                string sorgu = @"SELECT 
                             d.ID, 
                             s.Ad + ' ' + s.Soyad AS SekreterAdSoyad, 
                             d.Duyuru, 
                             d.DuyuruTarihi
                         FROM 
                             tbl_duyurular d
                         INNER JOIN 
                             tbl_sekreter s ON d.SekreterID = s.ID
                         ORDER BY 
                             d.DuyuruTarihi DESC";
                // SqlDataAdapter: Veritabanından veri almayı veya veri yazmayı sağlayan bir nesnedir.
                SqlDataAdapter veriler = new SqlDataAdapter(sorgu, baglanti);
                //DataTable: Bellekte tablo yapısında veri saklamak için kullanılan bir sınıftır
                DataTable sakla = new DataTable();
                veriler.Fill(sakla);

                // DataGridView'i doldur
                dataGridView1.DataSource = sakla;

            }

            finally
            {
                // Veritabanı bağlantısını kapat
                baglanti.Close();
            }
            dataGridView1.ClearSelection();
        }
    }
}
