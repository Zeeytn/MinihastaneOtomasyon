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
    public partial class DoktorEkranı : Form
    {
        public DoktorEkranı()
        {
            InitializeComponent();
        }
        public string DoktorTC;
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-HB4GCHL\SQLEXPRESS02;Initial Catalog=minihastaneotomasyonu;Integrated Security=True");

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void DoktorEkranı_Load(object sender, EventArgs e)
        {
            randevular();
            label3.Text = DoktorTC;
            try
            {


                baglanti.Open();
                string sorgu = "SELECT d.Ad, d.Soyad,b.BransAd FROM tbl_doktorlar d inner join tbl_branslar b on d.BransID = b.ID WHERE TC = @tcNo";
                using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                {
                    komut.Parameters.AddWithValue("@tcNo", label3.Text);

                    using (SqlDataReader dr = komut.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            label4.Text = dr["Ad"].ToString() + " " + dr["Soyad"].ToString();// Ad Soyad bilgisi alınıyor
                            label6.Text = dr["BransAd"].ToString();
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
        public void randevular()
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
            string query = @"
        
	           SELECT H.Ad, H.Soyad, R.RandevuTarihi 
               FROM tbl_randevular R
               INNER JOIN tbl_hastalar H ON R.HastaID = H.ID
               WHERE R.DoktorID = @ID";

            SqlCommand cmd = new SqlCommand(query, baglanti);
            cmd.Parameters.AddWithValue("@ID", doktorID);

            SqlDataAdapter da = new SqlDataAdapter(cmd); // Komutu SqlDataAdapter'e veriyoruz
            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
            baglanti.Close();
        }



        private void button6_Click(object sender, EventArgs e)
        {

            GirisPaneli gr = new GirisPaneli();
            gr.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoktorTahlil thl = new DoktorTahlil();
            thl.DoktorTC = label3.Text;
            thl.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DoktorReçete rct = new DoktorReçete();
            rct.DoktorTC = label3.Text;
            rct.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Duyurular dyr = new Duyurular();
            dyr.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
