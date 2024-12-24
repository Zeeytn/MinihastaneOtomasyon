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
using System.Linq.Expressions;

namespace minihastaneotomasyonu
{
    public partial class HastaKayıt : Form
    {
        public HastaKayıt()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                string sorgu = "insert into tbl_Hastalar (AD,Soyad,TC,Telefon,Sifre,cinsiyet,dogum_tarihi,boy,kilo,e_mail,il,ilçe) values (@hastaad,@hastasoyad,@hastatc,@hastatelefon,@hastasifre,@hastacinsiyet,@hastayas,@hastaboy,@hastakilo,@hastaemail,@hastail,@hastailce)";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@hastaad", textBox1.Text);
                komut.Parameters.AddWithValue("@hastasoyad", textBox2.Text);
                komut.Parameters.AddWithValue("@hastatc", textBox3.Text);
                komut.Parameters.AddWithValue("@hastatelefon", maskedTextBox1.Text);
                komut.Parameters.AddWithValue("@hastasifre", textBox4.Text);
                komut.Parameters.AddWithValue("@hastacinsiyet", comboBox1.Text);
                komut.Parameters.AddWithValue("@hastayas", dateTimePicker1.Value);
                komut.Parameters.AddWithValue("@hastaboy", textBox6.Text);
                komut.Parameters.AddWithValue("@hastakilo", textBox5.Text);
                komut.Parameters.AddWithValue("@hastaemail", textBox7.Text);
                komut.Parameters.AddWithValue("@hastail", comboBox2.Text);
                komut.Parameters.AddWithValue("@hastailce", comboBox3.Text);

                // Formdaki tüm TextBox'ları al
                foreach (Control ctrl in this.Controls)
                {
                    // Eğer kontrol bir TextBox ise
                    if (ctrl is TextBox)
                    {
                        TextBox txtBox = (TextBox)ctrl;

                        // TextBox boş ise uyarı mesajı göster
                        if (string.IsNullOrWhiteSpace(txtBox.Text))
                        {
                            MessageBox.Show("Lütfen tüm alanları doldurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Eğer bir boş TextBox varsa, işlemi sonlandır
                        }
                    }
                }




                //  komut.ExecuteNonQuery();
                int sonuc = komut.ExecuteNonQuery();


                if (sonuc > 0)
                {
                    MessageBox.Show("Kayıt başarılı!");
                 

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

        private void HastaKayıt_Load(object sender, EventArgs e)
        {
            sehirlerilistele();
            textBox4.PasswordChar = '*';
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
                    comboBox2.Items.Add(dr[1]);
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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Text = "";
            try
            {
                baglanti.Open();
                string sorgu = "Select ID,IlceAdi from tbl_ilceler where IlID = @ilID";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@ilID", comboBox2.SelectedIndex + 1);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    comboBox3.Items.Add(dr[1]);
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

        private void button2_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }
    }
}
