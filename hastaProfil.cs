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
    public partial class hastaProfil : Form
    {

        public string HastaTC;
        public hastaProfil()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void hastaProfil_Load(object sender, EventArgs e)
        {
            sehirlerilistele();
            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("SELECT * FROM tbl_hastalar WHERE TC = @tc", baglanti);
                komut.Parameters.AddWithValue("@tc", HastaTC); // Girişten gelen hastaTC'yi kullanıyoruz

                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    TxtAd.Text = dr["Ad"].ToString();
                    TxtSoyad.Text = dr["Soyad"].ToString();
                    TxtTC.Text = dr["TC"].ToString();
                    TxtEmail.Text = dr["e_mail"].ToString();
                    maskedTextBox1.Text = dr["Telefon"].ToString();
                    TxtSifre.Text = dr["Sifre"].ToString();
                    TxtBoy.Text = dr["Boy"].ToString();
                    TxtKilo.Text = dr["Kilo"].ToString();
                    cmbCinsiyet.SelectedItem = dr["Cinsiyet"].ToString();
                    dateTimePicker1.Value = Convert.ToDateTime(dr["dogum_tarihi"]);
                    
                    // CmbIl.SelectedItem = dr["il"].ToString();
                    // CmbIlce.SelectedItem = dr["ilçe"].ToString();
                }
                dr.Close();
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
        public void sehirlerilistele()
        {
            try
            {
                baglanti.Open();
                string sorgu = "Select * from tbl_iller";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    CmbIl.Items.Add(dr[1]);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message); // Hata mesajı
            }
            finally
            {
                baglanti.Close(); // Bağlantıyı kapatma

            }

        }


        private void CmbIlce_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbIl_SelectedIndexChanged(object sender, EventArgs e)
        {
            CmbIlce.Items.Clear();
            CmbIlce.Text = "";
            try
            {
                baglanti.Open();
                string sorgu = "Select ID,IlceAdi from tbl_ilceler where IlID = @ilID";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@ilID", CmbIl.SelectedIndex + 1);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    CmbIlce.Items.Add(dr[1]);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message); // Hata mesajı
            }
            finally
            {
                baglanti.Close(); // Bağlantıyı kapatma
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ad = TxtAd.Text;
            string soyad = TxtSoyad.Text;
            string Tc = TxtTC.Text;
            string telefon = maskedTextBox1.Text;
            string mail = TxtEmail.Text;
            string sifre = TxtSifre.Text;
            string boy = TxtBoy.Text;
            double kilo = Convert.ToDouble(TxtKilo.Text);
            string cinsiyet = cmbCinsiyet.Text;
           
            string il = CmbIl.Text;
            string ilçe = CmbIlce.Text;

            if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) || string.IsNullOrEmpty(Tc) || string.IsNullOrEmpty(telefon) || string.IsNullOrEmpty(mail) || string.IsNullOrEmpty(sifre) ||
                 string.IsNullOrEmpty(boy) ||  string.IsNullOrEmpty(cinsiyet)  || string.IsNullOrEmpty(il) || string.IsNullOrEmpty(ilçe))
            {
                MessageBox.Show("Lütfen Tüm Alanları Doldurunuz !");
                return;
            }


            try
            {
                baglanti.Open();

                string sorgu = "Update tbl_hastalar SET AD = @Ad, Soyad = @soyad, sifre = @sifre, cinsiyet = @cinsiyet, telefon = @tel, il = @il, ilçe = @ilçe, dogum_tarihi = @dogumtrh, boy = @boy, kilo = @kilo, e_mail = @mail Where TC = @Tc";
                SqlCommand guncellekomut = new SqlCommand(sorgu, baglanti);

                guncellekomut.Parameters.AddWithValue("@Tc", HastaTC);
                guncellekomut.Parameters.AddWithValue("@Ad", ad);
                guncellekomut.Parameters.AddWithValue("@soyad", soyad);
                guncellekomut.Parameters.AddWithValue("@sifre", sifre);
                guncellekomut.Parameters.AddWithValue("@cinsiyet", cinsiyet);
                guncellekomut.Parameters.AddWithValue("@tel", telefon);
                guncellekomut.Parameters.AddWithValue("@il", il);
                guncellekomut.Parameters.AddWithValue("@ilçe", ilçe);
                guncellekomut.Parameters.AddWithValue("@dogumtrh", dateTimePicker1.Value);
                guncellekomut.Parameters.AddWithValue("@boy", Convert.ToInt16(boy));
                guncellekomut.Parameters.AddWithValue("@kilo", kilo);
                guncellekomut.Parameters.AddWithValue("@mail", mail);

                int result = guncellekomut.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Bilgileriniz başarıyla güncellendi ! ");
                }
                else
                {
                    MessageBox.Show("Güncelleme Başarısız ! ");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message); // Hata mesajı
            }
            finally
            {
                baglanti.Close(); // Bağlantıyı kapatma
            }
            

        }


    }
}


