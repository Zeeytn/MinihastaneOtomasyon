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
    public partial class DoktorGiris : Form
    {
        public DoktorGiris()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                baglanti.Open();
                string sorgu = "Select * From tbl_doktorlar Where TC=@doktortc and sifre=@doktorsifre";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@doktortc", textBox1.Text);
                komut.Parameters.AddWithValue("@doktorsifre", textBox2.Text);
                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    DoktorEkranı fr = new DoktorEkranı();
                    fr.DoktorTC = textBox1.Text;
                    fr.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Hatalı Kullanıcı adı ve Şifresi ");
                }

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

        private void DoktorGiris_Load(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
