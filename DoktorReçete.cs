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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace minihastaneotomasyonu
{
    public partial class DoktorReçete : Form
    {
        public DoktorReçete()
        {
            InitializeComponent();
        }
        public string DoktorTC;
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");


        private void DoktorReçete_Load(object sender, EventArgs e)
        {
            ReceteleriYukle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                string sorgu = "SELECT dogum_tarihi, Cinsiyet, Kilo FROM tbl_hastalar WHERE TC = @hastaTC";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@hastaTC", textBox1.Text);

                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    // Doğum tarihini al
                    DateTime dogumTarihi = Convert.ToDateTime(dr["dogum_tarihi"]);

                    // Yaşı hesapla
                    int yas = DateTime.Now.Year - dogumTarihi.Year;
                    if (DateTime.Now < dogumTarihi.AddYears(yas)) // Eğer doğum günü bu yıl henüz gelmediyse yaşı 1 azalt
                    {
                        yas--;
                    }

                    label5.Text = yas.ToString(); // Yaşı label'a yaz
                    label6.Text = dr["Cinsiyet"].ToString();
                    label7.Text = dr["Kilo"].ToString();
                }
                else
                {
                    MessageBox.Show("Girilen TC'ye ait hasta bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                dr.Close();
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

        private void button2_Click(object sender, EventArgs e)
        {
            string hastaTC = textBox1.Text.Trim(); // Kullanıcının girdiği hasta TC
            string recete = textBox2.Text.Trim();
            string doktorTC = DoktorTC; // Giriş ekranından alınan doktor TC

            if (string.IsNullOrEmpty(hastaTC) || string.IsNullOrEmpty(recete))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Veritabanı bağlantısını aç
                baglanti.Open();

                // 1. Adım: Girilen HastaTC'den HastaID'yi çek
                string hastaIDBulSorgusu = "SELECT ID FROM tbl_hastalar WHERE TC = @hastaTC";
                SqlCommand hastaIDBulKomut = new SqlCommand(hastaIDBulSorgusu, baglanti);
                hastaIDBulKomut.Parameters.AddWithValue("@hastaTC", hastaTC);

                object hastaIDObj = hastaIDBulKomut.ExecuteScalar();
                if (hastaIDObj == null)
                {
                    MessageBox.Show("Girilen TC'ye ait bir hasta bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int hastaID = Convert.ToInt32(hastaIDObj);

                // 2. Adım: Girilen DoktorTC'den DoktorID'yi çek
                string doktorIDBulSorgusu = "SELECT ID FROM tbl_doktorlar WHERE TC = @doktorTC";
                SqlCommand doktorIDBulKomut = new SqlCommand(doktorIDBulSorgusu, baglanti);
                doktorIDBulKomut.Parameters.AddWithValue("@doktorTC", doktorTC);

                object doktorIDObj = doktorIDBulKomut.ExecuteScalar();
                if (doktorIDObj == null)
                {
                    MessageBox.Show("Girilen TC'ye ait bir doktor bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int doktorID = Convert.ToInt32(doktorIDObj);

                // 3. Adım: Tahlil tablosuna ekleme işlemi
                string ekleReceteSorgusu = @"
            INSERT INTO tbl_receteler (HastaID, DoktorID, ReceteTarih, ReceteSonuc)
            VALUES (@hastaID, @doktorID, GETDATE(), @receteSonuc)";
                SqlCommand ekleReceteKomut = new SqlCommand(ekleReceteSorgusu, baglanti);
                ekleReceteKomut.Parameters.AddWithValue("@hastaID", hastaID);
                ekleReceteKomut.Parameters.AddWithValue("@doktorID", doktorID);
                ekleReceteKomut.Parameters.AddWithValue("@receteSonuc", recete);

                ekleReceteKomut.ExecuteNonQuery();
                MessageBox.Show("Reçete başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Clear();
                textBox2.Clear();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.Close(); // Bağlantıyı kapat
            }

        }
        private void ReceteleriYukle()
        {
            try
            {
                baglanti.Open();

                // Doktor ID'sini TC'ye göre al
                string doktorIDBulSorgusu = "SELECT ID FROM tbl_doktorlar WHERE TC = @doktorTC";
                SqlCommand doktorIDBulKomut = new SqlCommand(doktorIDBulSorgusu, baglanti);
                doktorIDBulKomut.Parameters.AddWithValue("@doktorTC", DoktorTC);

                object doktorIDObj = doktorIDBulKomut.ExecuteScalar();
                if (doktorIDObj == null)
                {
                    MessageBox.Show("Girilen TC'ye ait bir doktor bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int doktorID = Convert.ToInt32(doktorIDObj);

                // Doktorun tahlillerini getir
                string sorgu = @"
            SELECT 
                h.Ad + ' ' + h.Soyad AS 'Hasta Adı Soyadı',
                h.TC AS 'Hasta TC',
                r.ReceteTarih AS 'Reçete Tarihi',
                r.ReceteSonuc AS 'Reçete',
                d.Ad + ' ' + d.Soyad AS 'Doktor Adı Soyadı'
            FROM tbl_receteler r
            INNER JOIN tbl_hastalar h ON r.HastaID = h.ID
            INNER JOIN tbl_doktorlar d ON r.DoktorID = d.ID
            WHERE r.DoktorID = @doktorID";

                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@doktorID", doktorID);

                SqlDataAdapter da = new SqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // DataGridView'e verileri aktar
                dataGridView1.DataSource = dt;

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

        private void button4_Click(object sender, EventArgs e)
        {
            ReceteleriYukle();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string hastaTC = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(hastaTC))
            {
                MessageBox.Show("Lütfen bir hasta TC girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                baglanti.Open();

                // Hasta TC'ye göre tahlilleri getir
                string sorgu = @"
            SELECT 
                h.Ad + ' ' + h.Soyad AS 'Hasta Adı Soyadı',
                h.TC AS 'Hasta TC',
                r.ReceteTarih AS 'Reçete Tarihi',
                r.ReceteSonuc AS 'Reçete',
                d.Ad + ' ' + d.Soyad AS 'Doktor Adı Soyadı'
            FROM tbl_receteler r
            INNER JOIN tbl_hastalar h ON r.HastaID = h.ID
            INNER JOIN tbl_doktorlar d ON r.DoktorID = d.ID
            WHERE h.TC = @hastaTC";

                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@hastaTC", hastaTC);

                SqlDataAdapter da = new SqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // DataGridView'e verileri aktar
                dataGridView1.DataSource = dt;

                // DataGridView'deki ilk seçimi temizle
                dataGridView1.ClearSelection();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Bu TC'ye ait Reçete bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
