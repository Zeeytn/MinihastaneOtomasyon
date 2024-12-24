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
    public partial class BransDüzenlemePaneli : Form
    {
        public BransDüzenlemePaneli()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void VeriYukle()
        {


            try
            {
                // Veritabanı bağlantısını aç
                baglanti.Open();

                // Kullanıcıları getirmek için SQL sorgusu
                string sorgu = "SELECT * FROM tbl_branslar";
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

        private void BransDüzenlemePaneli_Load(object sender, EventArgs e)
        {
            VeriYukle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                try
                {
                    baglanti.Open();
                    string sorgu = "Insert Into tbl_branslar (BransAd) Values (@bransad)";
                    SqlCommand komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@bransad", textBox1.Text);
                    int sonuc = komut.ExecuteNonQuery();


                    if (sonuc > 0)
                    {
                        MessageBox.Show("Kayıt başarılı!");
                        textBox1.Clear();

                    }
                    else
                    {
                        MessageBox.Show("Kayıt başarısız.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                finally { baglanti.Close(); }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            VeriYukle();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // DataGridView'de bir satır seçilip seçilmediğini kontrol edin
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Seçili satırın KullaniciID'sini alın
                int BransID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                try
                {

                    // Veritabanı bağlantısını aç
                    baglanti.Open();


                    // Silme işlemi için SQL sorgusu
                    string sorgu = "DELETE FROM tbl_branslar WHERE ID = @bransID";
                    SqlCommand komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@bransID", BransID);

                    // Sorguyu çalıştır ve etkilenen satır sayısını kontrol et
                    int sonuc = komut.ExecuteNonQuery();

                    if (sonuc > 0)
                    {
                        MessageBox.Show("Kayıt başarıyla silindi!");


                    }
                    else
                    {
                        MessageBox.Show("Silme işlemi başarısız.");
                    }
                }
                finally { baglanti.Close(); }

            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz kaydı seçin.");
            }
        }

    }
}


