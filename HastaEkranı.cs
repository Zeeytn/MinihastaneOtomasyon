using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms.VisualStyles;

namespace minihastaneotomasyonu
{
    public partial class HastaEkranı : Form
    {
        public HastaEkranı()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");

        public string HastaTC;

        private void HastaEkranı_Load(object sender, EventArgs e)
        {
            RandevuyuOnay();
            RandevularımıGöster();
            BranslarıGöster();
            label3.Text = HastaTC;
            try
            {


                baglanti.Open();
                string sorgu = "SELECT Ad, Soyad FROM tbl_hastalar WHERE TC = @tcNo";
                using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                {
                    komut.Parameters.AddWithValue("@tcNo", label3.Text);

                    using (SqlDataReader dr = komut.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            label4.Text = dr["Ad"].ToString() + " " + dr["Soyad"].ToString();// Ad Soyad bilgisi alınıyor
                        }
                        else
                        {
                            MessageBox.Show("Kayıt bulunamadı!");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally { baglanti.Close(); }



        }

        private void RandevuyuOnay()
        {
            try
            {
                baglanti.Open();
                string hastaIDBulSorgusu = "SELECT ID FROM tbl_hastalar WHERE TC = @hastaTC";
                SqlCommand hastaIDBulKomut = new SqlCommand(hastaIDBulSorgusu, baglanti);
                hastaIDBulKomut.Parameters.AddWithValue("@hastaTC", HastaTC);

                object hastaIDObj = hastaIDBulKomut.ExecuteScalar();
                if (hastaIDObj == null)
                {
                    MessageBox.Show("Girilen TC'ye ait bir hasta bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int hastaID = Convert.ToInt32(hastaIDObj);

                string sorgu = "SELECT ID, RandevuTarihi,Durum FROM tbl_randevular WHERE HastaID = @hastaID AND RandevuTarihi >= CAST(DATEADD(DAY, 1, GETDATE()) AS DATE)    AND RandevuTarihi < CAST(DATEADD(DAY, 2, GETDATE()) AS DATE)";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@hastaID", hastaID);  // Hasta ID'si burada alınacak
                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    string durum = dr["Durum"].ToString();

                    // Duruma göre mesajı ayarlıyoruz
                    if (durum == "Onaylandı")
                    {
                        label12.Text = "Randevunuz onaylanmış. Onay gerektiren randevunuz bulunmamaktadır.";
                        button6.Enabled = false;
                        button7.Enabled = false;
                    }
                    else if (durum == "Onaylanmadı")
                    {
                        label12.Text = "Randevu onaylanmamış. Onay gerektiren randevunuz bulunmamaktadır.";
                        button6.Enabled = false;
                        button7.Enabled = false;
                    }
                    else
                    {
                        label12.Text = dr["RandevuTarihi"].ToString() + " tarihindeki randevunuzu onaylıyor musunuz?";
                    }
                }
                else
                {
                    label12.Text = "Yaklaşan randevunuz bulunmamaktadır.";
                    button6.Enabled = false;
                    button7.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.Close();
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void RandevularımıGöster()
        {
            try
            {
                baglanti.Open();

                string sorgu = @"
            SELECT 
                r.ID, 
                d.Ad,
                 d.Soyad, 
                b.BransAd, 
                r.RandevuTarihi, 
                r.Durum,
                CASE 
                    WHEN r.RandevuTarihi < GETDATE() THEN 'Geçmiş' 
                    ELSE 'Aktif' 
                END AS RandevuDurum
            FROM tbl_randevular r
            JOIN tbl_doktorlar d ON r.DoktorID = d.ID
            JOIN tbl_branslar b ON r.BransID = b.ID
            JOIN tbl_hastalar h ON r.HastaID = h.ID
            WHERE h.TC = @hastaTC
            ORDER BY r.RandevuTarihi";



                SqlCommand cmd = new SqlCommand(sorgu, baglanti);
                cmd.Parameters.AddWithValue("@hastaTC", HastaTC);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // DataGridView'e veri aktarımı
                dataGridView1.DataSource = dt;
                dataGridView1.ClearSelection();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }

            finally
            {
                baglanti.Close();
            }
        }



        public void BranslarıGöster()
        {
            string sorgu = "SELECT ID, BransAd FROM tbl_branslar";
            SqlDataAdapter da = new SqlDataAdapter(sorgu, baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "BransAd";  // Görünen metin
            comboBox1.ValueMember = "ID";   // Arka plandaki değer
            comboBox1.SelectedIndex = -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue != null && int.TryParse(comboBox1.SelectedValue.ToString(), out int bransID))
                {
                    string sorgu = "SELECT ID, Ad, Soyad FROM tbl_doktorlar WHERE BransID = @BransID";
                    SqlCommand komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@BransID", bransID);

                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    comboBox2.DataSource = null;
                    comboBox2.SelectedIndex = -1;

                    // DoktorAdSoyad alanını birleştirme işlemiyle manuel olarak oluşturuyoruz
                    List<KeyValuePair<int, string>> doktorListesi = new List<KeyValuePair<int, string>>();
                    foreach (DataRow row in dt.Rows)
                    {
                        int doktorID = Convert.ToInt32(row["ID"]);
                        string doktorAdSoyad = $"{row["Ad"]} {row["Soyad"]}";
                        doktorListesi.Add(new KeyValuePair<int, string>(doktorID, doktorAdSoyad));
                    }

                    comboBox2.DataSource = new BindingSource(doktorListesi, null);
                    comboBox2.DisplayMember = "Value";  // Ad ve Soyad görünür
                    comboBox2.ValueMember = "Key";     // Doktor ID saklanır

                    comboBox2.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                baglanti.Open();
                // Hasta TC'yi al ve ilgili ID'yi bul
                string hastaTc = textBox2.Text.Trim();
                if (string.IsNullOrEmpty(hastaTc))
                {
                    MessageBox.Show("Lütfen hasta TC'sini giriniz.");
                    return;
                }

                // Branş ve doktor ID'lerini al
                int bransID = Convert.ToInt32(comboBox1.SelectedValue);
                int doktorID = Convert.ToInt32(comboBox2.SelectedValue);

                // DateTimePicker ile seçilen tarihi al
                DateTime randevuTarihi = dateTimePicker1.Value.Date;

                // Saat ComboBox'ından seçilen saati al
                string selectedTime = comboBox3.SelectedItem.ToString();
                DateTime randevuSaati;
                if (DateTime.TryParse(selectedTime, out randevuSaati))
                {
                    // Tarihe saati ekle
                    randevuTarihi = randevuTarihi.AddHours(randevuSaati.Hour).AddMinutes(randevuSaati.Minute);
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir saat seçiniz.", "Saat Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (randevuTarihi < DateTime.Now)
                {
                    MessageBox.Show("Geçmiş tarihe randevu verilemez.", "Geçmiş Tarih Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (bransID == -1 || doktorID == -1)
                {
                    MessageBox.Show("Lütfen bir branş ve doktor seçiniz.");
                    return;
                }

                // Hasta ID'yi çek
                string hastaSorgu = "SELECT ID FROM tbl_hastalar WHERE TC = @TcNo";
                int hastaID;
                using (SqlCommand hastaKomut = new SqlCommand(hastaSorgu, baglanti))
                {
                    hastaKomut.Parameters.AddWithValue("@TcNo", hastaTc);
                    object result = hastaKomut.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Hasta bulunamadı. Lütfen geçerli bir TC giriniz.");
                        return;
                    }
                    hastaID = Convert.ToInt32(result);
                }

                // Seçilen randevu saatiyle çakışan bir randevu olup olmadığını kontrol et
                string kontrolSorgu = "SELECT COUNT(*) FROM tbl_randevular WHERE DoktorID = @DoktorID AND RandevuTarihi = @RandevuTarihi";
                using (SqlCommand kontrolKomut = new SqlCommand(kontrolSorgu, baglanti))
                {
                    kontrolKomut.Parameters.AddWithValue("@DoktorID", doktorID);
                    kontrolKomut.Parameters.AddWithValue("@RandevuTarihi", randevuTarihi);
                    int randevuSayisi = (int)kontrolKomut.ExecuteScalar();
                    if (randevuSayisi > 0)
                    {
                        MessageBox.Show("Seçilen tarihte ve saatte randevu mevcut değil. Lütfen farklı bir saat seçiniz.", "Randevu Çakışması", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Randevuyu kaydet
                string randevuSorgu = @"
        INSERT INTO tbl_randevular (HastaID, DoktorID, BransID, RandevuTarihi, Durum)
        VALUES (@HastaID, @DoktorID, @BransID, @RandevuTarihi, 'OnayBekliyor')";

                using (SqlCommand randevuKomut = new SqlCommand(randevuSorgu, baglanti))
                {
                    randevuKomut.Parameters.AddWithValue("@HastaID", hastaID);
                    randevuKomut.Parameters.AddWithValue("@DoktorID", doktorID);
                    randevuKomut.Parameters.AddWithValue("@BransID", bransID);
                    randevuKomut.Parameters.AddWithValue("@RandevuTarihi", randevuTarihi);

                    int rowsAffected = randevuKomut.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Randevu başarıyla oluşturuldu. Onay bekleniyor.");
                     


                    }
                    else
                    {
                        MessageBox.Show("Randevu oluşturulamadı. Lütfen tekrar deneyiniz.");
                       

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            GirisPaneli gr = new GirisPaneli();
            gr.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            hastaProfil prf = new hastaProfil();
            prf.HastaTC = label3.Text;
            prf.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Tahlillerim thl = new Tahlillerim();
            thl.HastaTC = label3.Text;
            thl.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Reçetelerim r = new Reçetelerim();
            r.HastaTC = label3.Text;
            r.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                string sorgu = "UPDATE tbl_randevular SET Durum = 'Onaylandı' WHERE  RandevuTarihi >= CAST(DATEADD(DAY, 1, GETDATE()) AS DATE)    AND RandevuTarihi < CAST(DATEADD(DAY, 2, GETDATE()) AS DATE)";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                komut.ExecuteNonQuery();
                MessageBox.Show("Randevu onaylandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                string sorgu = "UPDATE tbl_randevular SET Durum = 'Onaylanmadı' WHERE  RandevuTarihi >= CAST(DATEADD(DAY, 1, GETDATE()) AS DATE)    AND RandevuTarihi < CAST(DATEADD(DAY, 2, GETDATE()) AS DATE)";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                komut.ExecuteNonQuery();
                MessageBox.Show("Randevu onaylandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.Close();
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            RandevularımıGöster();
        }
    }
}










