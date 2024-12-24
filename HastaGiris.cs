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

namespace minihastaneotomasyonu
{
    public partial class HastaGiris : Form
    {
        public HastaGiris()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HastaKayıt kyt = new HastaKayıt();
            kyt.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string hastatc = textBox1.Text;
            string yastasifre = textBox2.Text;
            try
            {
                baglanti.Open();
                string sorgu = "Select * From tbl_hastalar Where TC=@hastatc and sifre=@hastasifre";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@hastatc", textBox1.Text);
                komut.Parameters.AddWithValue("@hastasifre", textBox2.Text);
                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    HastaEkranı fr = new HastaEkranı();
                    fr.HastaTC = textBox1.Text;
                    fr.Show();
                    this.Close();
                  
                }
                else if (textBox1.Text == "" || textBox2.Text == "")  // kullanıcı adı veya şifre boş ise kullanıcıya uyarı gönderdik.
                {
                    MessageBox.Show("Lütfen Boş Alan Bırakmayınız ! ");
                }
                else  // kuallnıcı veri tabanında bulunamazsa bu mesajı veriyoruz.
                {
                    MessageBox.Show("Hatalı Kullanıcı Adı veya Şifre !");

                }
                dr.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }

            finally
            {
                baglanti.Close();
            }
        }

        private void HastaGiris_Load(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
