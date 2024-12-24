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
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace minihastaneotomasyonu
{
    public partial class DoktorTahlil : Form
    {
        public DoktorTahlil()
        {
            InitializeComponent();
        }
        public string DoktorTC;
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");

        private void button1_Click(object sender, EventArgs e)
        {
            string hastaTC = textBox1.Text.Trim(); // Kullanıcının girdiği hasta TC
            string tahlilTuru = comboBox1.Text.Trim(); // Seçilen tahlil türü
            string doktorTC = DoktorTC; // Giriş ekranından alınan doktor TC

            if (string.IsNullOrEmpty(hastaTC) || string.IsNullOrEmpty(tahlilTuru))
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
                string ekleTahlilSorgusu = @"
            INSERT INTO tbl_tahliller (HastaID, DoktorID, TahlilTarihi, TahlilTürü)
            VALUES (@hastaID, @doktorID, GETDATE(), @tahlilTuru)";
                SqlCommand ekleTahlilKomut = new SqlCommand(ekleTahlilSorgusu, baglanti);
                ekleTahlilKomut.Parameters.AddWithValue("@hastaID", hastaID);
                ekleTahlilKomut.Parameters.AddWithValue("@doktorID", doktorID);
                ekleTahlilKomut.Parameters.AddWithValue("@tahlilTuru", tahlilTuru);

                ekleTahlilKomut.ExecuteNonQuery();
                MessageBox.Show("Tahlil başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Clear();
                comboBox1.SelectedIndex = -1;


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

        private void DoktorTahlil_Load(object sender, EventArgs e)
        {
            TahlilleriYukle();
        }
        private void TahlilleriYukle()
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
                t.TahlilTürü AS 'Tahlil Türü',
                t.TahlilTarihi AS 'Tahlil Tarihi',
                d.Ad + ' ' + d.Soyad AS 'Doktor Adı Soyadı'
            FROM tbl_tahliller t
            INNER JOIN tbl_hastalar h ON t.HastaID = h.ID
            INNER JOIN tbl_doktorlar d ON t.DoktorID = d.ID
            WHERE t.DoktorID = @doktorID";

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

        private void button3_Click(object sender, EventArgs e)
        {
            TahlilleriYukle();
        }

        private void textBox2_MEnter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string hastaTC = textBox2.Text.Trim();

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
                t.TahlilTürü AS 'Tahlil Türü',
                t.TahlilTarihi AS 'Tahlil Tarihi',
                d.Ad + ' ' + d.Soyad AS 'Doktor Adı Soyadı'
            FROM tbl_tahliller t
            INNER JOIN tbl_hastalar h ON t.HastaID = h.ID
            INNER JOIN tbl_doktorlar d ON t.DoktorID = d.ID
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
                    MessageBox.Show("Bu TC'ye ait tahlil bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
