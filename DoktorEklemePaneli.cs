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
    public partial class DoktorEklemePaneli : Form
    {
        public DoktorEklemePaneli()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void DoktorEklemePaneli_Load(object sender, EventArgs e)
        {
            VeriYukle();
            BranslariYukle();
            
        }
        private void VeriYukle()
        {


            try
            {
                // Veritabanı bağlantısını aç
                baglanti.Open();

                // Kullanıcıları getirmek için SQL sorgusu
                string sorgu = "SELECT d.ID, d.Ad, d.Soyad, d.TC, d.e_mail, b.BransAd, d.Sifre FROM tbl_doktorlar d  INNER JOIN tbl_branslar b ON d.BransID = b.ID";

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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();

                // Seçilen AltKategoriID'yi almak
                if (comboBox1.SelectedItem is KeyValuePair<int, string> selectedBrans)
                {
                    int BransID = selectedBrans.Key;

                    // Ürün ekleme sorgusu
                    string SORGU = @"
                INSERT INTO tbl_doktorlar (Ad, Soyad,BransID,TC,Sifre,e_mail )
                VALUES (@doktorAd, @DoktorSoyad,  @BransID, @tc, @sifre, @email )";

                    SqlCommand komut = new SqlCommand(SORGU, baglanti);

                    // Parametreleri atama
                    komut.Parameters.AddWithValue("@BransID", BransID);
                    komut.Parameters.AddWithValue("@doktorAd", textBox1.Text);
                    komut.Parameters.AddWithValue("@DoktorSoyad", textBox2.Text);
                    komut.Parameters.AddWithValue("@tc", textBox3.Text);
                    komut.Parameters.AddWithValue("@sifre", textBox4.Text);
                    komut.Parameters.AddWithValue("@email", textBox5.Text);

                    // Sorguyu çalıştır
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Kayıt Tamamlandı!");
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    comboBox1.Items.Clear();


                }
                else
                {
                    MessageBox.Show("Lütfen Branş Seçiniz !");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally { baglanti.Close(); }
        }

        private void BranslariYukle()
        {
            try
            {
                baglanti.Open();
                {


                    // Alt kategorileri getiren sorgu
                    string query = "SELECT ID, BransAd FROM tbl_branslar";
                    SqlCommand cmd = new SqlCommand(query, baglanti);

                    SqlDataReader reader = cmd.ExecuteReader();

                    // ComboBox'ı temizle
                    comboBox1.Items.Clear();

                    while (reader.Read())
                    {
                        // AltKategoriID ve AltKategoriAdi'ni KeyValuePair olarak ekle
                        comboBox1.Items.Add(new KeyValuePair<int, string>(
                            (int)reader["ID"],
                            reader["BransAd"].ToString()));
                    }

                    comboBox1.DisplayMember = "Value"; // ComboBox'ta AltKategoriAdi gösterilecek
                    comboBox1.ValueMember = "Key";    // Seçimden sonra AltKategoriID alınacak
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally { baglanti.Close(); }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            // DataGridView'de bir satır seçilip seçilmediğini kontrol edin
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Seçili satırın KullaniciID'sini alın
                int DoktorID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                try
                {

                    // Veritabanı bağlantısını aç
                    baglanti.Open();


                    // Silme işlemi için SQL sorgusu
                    string sorgu = "DELETE FROM tbl_doktorlar WHERE ID = @doktorID";
                    SqlCommand komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@doktorID", DoktorID);

                    // Sorguyu çalıştır ve etkilenen satır sayısını kontrol et
                    int sonuc = komut.ExecuteNonQuery();

                    if (sonuc > 0)
                    {
                        MessageBox.Show("Kayıt başarıyla silindi!");
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                        textBox5.Clear();
                        comboBox1.Items.Clear();


                    }
                    else
                    {
                        MessageBox.Show("Silme işlemi başarısız.");
                    }
                }
                finally { baglanti.Close(); }

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            VeriYukle();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {


                baglanti.Open();

                // Seçili satırdaki KullaniciID'yi alıyoruz
                int kullaniciID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
                // Öncelikle veritabanındaki mevcut verileri alıyoruz
                string kontrolSorgusu = "SELECT Ad, Soyad,TC, e_mail, Sifre FROM tbl_doktorlar WHERE ID = @ID";
                SqlCommand kontrolKomut = new SqlCommand(kontrolSorgusu, baglanti);
                kontrolKomut.Parameters.AddWithValue("@ID", kullaniciID);
                SqlDataReader reader = kontrolKomut.ExecuteReader();
                // SqlDataReader), veritabanından sorgu sonucu gelen verileri okuyan
                // ve bunlara erişim sağlayan bir veri okuma sınıfıdır.

                if (reader.Read())
                {
                    string mevcutAd = reader["Ad"].ToString();
                    string mevcutSoyad = reader["Soyad"].ToString();
                    string mevcutTC = reader["TC"].ToString();
                    string mevcutEmail = reader["e_mail"].ToString();
                    string mevcutSifre = reader["Sifre"].ToString();
                    // Eğer veriler değişmemişse işlem yapılmaz
                    if (mevcutAd == textBox1.Text &&
                        mevcutSoyad == textBox2.Text &&
                        mevcutTC == textBox3.Text &&
                        mevcutEmail == textBox5.Text &&
                        mevcutSifre == textBox4.Text)
                    {
                        MessageBox.Show("Herhangi bir değişiklik yapılmadı.");
                        reader.Close();
                        return; // İşlemi sonlandır
                    }
                }
                reader.Close();


                string sorgu = "UPDATE tbl_doktorlar SET Ad = @Ad, Soyad = @Soyad, " +
                               "TC = @tc, e_mail = @Email, Sifre = @Sifre WHERE ID = @ID";

                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                komut.Parameters.AddWithValue("@Ad", textBox1.Text);
                komut.Parameters.AddWithValue("@Soyad", textBox2.Text);
                komut.Parameters.AddWithValue("@tc", textBox3.Text);
                komut.Parameters.AddWithValue("@Email", textBox5.Text);
                komut.Parameters.AddWithValue("@Sifre", textBox4.Text);
                komut.Parameters.AddWithValue("@ID", kullaniciID);

                int sonuc = komut.ExecuteNonQuery();
                if (sonuc > 0)
                {
                    MessageBox.Show("Kayıt güncellendi!");
                    // Güncel bilgileri göstermek için DataGridView'i yenile
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    comboBox1.Items.Clear();
                }
                else
                {
                    MessageBox.Show("Güncelleme başarısız.");
                }

            }
            catch (Exception ex)
            {
                // Hata mesajını kullanıcıya göster
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }



            finally { baglanti.Close(); }

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            
            if (dataGridView1.CurrentRow != null) // Geçerli bir satır seçildiğinde
            {
                DataGridViewRow row = dataGridView1.CurrentRow;
                textBox1.Text = row.Cells["Ad"].Value?.ToString();
                textBox2.Text = row.Cells["Soyad"].Value?.ToString();
                comboBox1.Text = row.Cells["BransAd"].Value?.ToString();
                textBox3.Text = row.Cells["TC"].Value?.ToString();
                textBox4.Text = row.Cells["Sifre"].Value?.ToString();
                textBox5.Text = row.Cells["e_mail"].Value?.ToString();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

}


