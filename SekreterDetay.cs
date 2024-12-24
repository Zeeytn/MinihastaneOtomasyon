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
using System.Net;
using System.Net.Mail;
using System.Web;

namespace minihastaneotomasyonu
{
    public partial class SekreterDetay : Form
    {
        public SekreterDetay()
        {
            InitializeComponent();

        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");
        public string TcNo;


        private void SekreterDetay_Load(object sender, EventArgs e)
        {
            BransYukle();
            DoktorlarıYukle();
            BranslarıGöster();
            label3.Text = TcNo;

            try
            {


                baglanti.Open();
                string sorgu = "SELECT Ad, Soyad FROM tbl_sekreter WHERE TC = @tcNo";
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


        private void button1_Click(object sender, EventArgs e)
        {

            string duyuruMetni = txtmetin.Text;

            baglanti.Open();

            string query = @"INSERT INTO tbl_duyurular (SekreterID, Duyuru, DuyuruTarihi) 
                         VALUES ((SELECT ID FROM tbl_sekreter WHERE TC = @Tc), @Duyuru, @DuyuruTarih)";
            SqlCommand cmd = new SqlCommand(query, baglanti);
            cmd.Parameters.AddWithValue("@Tc", TcNo); // Global değişken kullanımı
            cmd.Parameters.AddWithValue("@Duyuru", duyuruMetni);
            cmd.Parameters.AddWithValue("@DuyuruTarih", DateTime.Now);


            cmd.ExecuteNonQuery();
            baglanti.Close();

            MessageBox.Show("Duyuru başarıyla eklendi!");
            // DuyurulariGetir(); // DataGridView güncellenir



        }
        private void button6_Click(object sender, EventArgs e)
        {
            Duyurular duyuru = new Duyurular();
            duyuru.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DoktorEklemePaneli dktr = new DoktorEklemePaneli();
            dktr.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BransDüzenlemePaneli brn = new BransDüzenlemePaneli();
            brn.ShowDialog();
        }
        private void BransYukle()
        {


            try
            {
                // Veritabanı bağlantısını aç
                baglanti.Open();

                // Kullanıcıları getirmek için SQL sorgusu
                string sorgu = "Select * from tbl_branslar";

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
        private void DoktorlarıYukle()
        {


            try
            {
                // Veritabanı bağlantısını aç
                baglanti.Open();

                // Kullanıcıları getirmek için SQL sorgusu
                string sorgu = "SELECT  d.Ad, d.Soyad, d.TC, d.e_mail, b.BransAd FROM tbl_doktorlar d  INNER JOIN tbl_branslar b ON d.BransID = b.ID";

                // SqlDataAdapter: Veritabanından veri almayı veya veri yazmayı sağlayan bir nesnedir.
                SqlDataAdapter veriler = new SqlDataAdapter(sorgu, baglanti);
                //DataTable: Bellekte tablo yapısında veri saklamak için kullanılan bir sınıftır
                DataTable sakla = new DataTable();
                veriler.Fill(sakla);

                // DataGridView'i doldur
                dataGridView2.DataSource = sakla;

            }

            finally
            {
                // Veritabanı bağlantısını kapat
                baglanti.Close();
            }
            dataGridView2.ClearSelection();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            RandevuListesi randevuListesi = new RandevuListesi();
            randevuListesi.ShowDialog();
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

        private void button2_Click(object sender, EventArgs e)
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            HastaKayıt kyt = new HastaKayıt();
            kyt.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

            try
            {
                baglanti.Open();
                string query = @"  SELECT H.e_mail, H.Ad, H.Soyad, R.RandevuTarihi
    FROM tbl_hastalar H
    INNER JOIN tbl_randevular R ON H.ID = R.HastaID
    WHERE R.RandevuTarihi >= CAST(DATEADD(DAY, 1, GETDATE()) AS DATE)
      AND R.RandevuTarihi < CAST(DATEADD(DAY, 2, GETDATE()) AS DATE)";



                DataTable dt = new DataTable();


                SqlCommand command = new SqlCommand(query, baglanti);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);


                // E-posta gönderimi
                foreach (DataRow row in dt.Rows)
                {
                    string email = row["e_mail"].ToString();
                    string fullName = $"{row["Ad"]} {row["Soyad"]}";
                    string randevuTarih = Convert.ToDateTime(row["RandevuTarihi"]).ToString("dd/MM/yyyy");

                    SendEmail(email, fullName, randevuTarih);
                }

                MessageBox.Show("Randevular için hatırlatma mailleri başarıyla gönderildi.");

            }
            catch (SmtpException smtpEx)
            {
                MessageBox.Show($"SMTP Hatası: {smtpEx.Message}\nStatus Code: {smtpEx.StatusCode}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Genel Hata: {ex.Message}");
            }
            finally { baglanti.Close(); }

        }
        private void SendEmail(string toEmail, string fullName, string randevuTarih)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("zeynepturaaan@gmail.com");
                mail.To.Add(toEmail);
                mail.Subject = "Randevu Hatırlatması";
                mail.Body = $"Sayın {fullName},\n\n" +
                            $"{randevuTarih} tarihinde randevunuz bulunmaktadır. Lütfen uygulamamızdan randevu onayını yapınız.\n\n" +
                            "Sağlıklı günler dileriz,\nKorkmaz Clinic";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("zeynepturaaan@gmail.com", "jjgydzotujkyliua");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                MessageBox.Show($"Mail başarıyla {toEmail} adresine gönderildi.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mail gönderim hatası: {ex.Message}");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {

            GirisPaneli gr = new GirisPaneli();
            gr.Show();
            this.Close();
        }
    }
}


