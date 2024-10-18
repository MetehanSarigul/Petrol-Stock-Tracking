using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

namespace PetrolTakip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection("Data Source=Metehan\\SQLEXPRESS;Initial Catalog=PetrolStok;Integrated Security=True;Encrypt=False");
        
        // Petrol Stoklarını Sql'den yazdırma.
        void petrolstok()
        {
            //Kurşunsuz 95'i Sql'den ProgressBar'a yazdırma.
            con.Open();
            SqlCommand komut = new SqlCommand("Select * From TblBenzin Where PetrolTur = @p1", con);
            komut.Parameters.AddWithValue("@p1", "Kurşunsuz95");
            SqlDataReader dr= komut.ExecuteReader();
            if (dr.Read())
            {
                progressKurşunsuz.Value = Convert.ToInt32(dr[4]);
                lblKurşunsuz.Text = progressKurşunsuz.Value.ToString();
            }
            con.Close();

            // Dizel'i Sql'den ProgressBar'a yazdırma.
            con.Open();
            SqlCommand komut1 = new SqlCommand("Select * From TblBenzin Where PetrolTur = @p1", con);
            komut1.Parameters.AddWithValue("@p1", "Diesel");
            SqlDataReader dr2 = komut1.ExecuteReader();
            if (dr2.Read())
            {
                progressDizel.Value = Convert.ToInt32(dr2[4]);
                lblDizel.Text = progressDizel.Value.ToString();
            }
            con.Close();

            // Otogaz'ı Sql'den ProgressBar'a yazdırma.
            con.Open();
            SqlCommand komut2 = new SqlCommand("Select * From TblBenzin Where PetrolTur = @p1", con);
            komut2.Parameters.AddWithValue("@p1", "Otogaz");
            SqlDataReader dr3 = komut2.ExecuteReader();
            if (dr3.Read())
            {
                progressOtogaz.Value = Convert.ToInt32(dr3[4]);
                lblOtogaz.Text = progressOtogaz.Value.ToString();
            }
            con.Close();
        }


        // Kasadaki parayı Sqlden çekmek için.
        void kasa()
        {
            con.Open();
            SqlCommand com = new SqlCommand("Select * From TblKasa",con);
            SqlDataReader dr = com.ExecuteReader();
            while(dr.Read())
            {
                lblkasa.Text = dr[0].ToString();
            }
            con.Close();
        }

        //Kurşunsuz 95 fiyatını hesaplamak için.
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        { 
            double litre, kurşunsuzfiyat, tutar;
            con.Open();
            SqlCommand com = new SqlCommand("Select * From TblBenzin Where PetrolTur = @p1", con);
            com.Parameters.AddWithValue("@p1", "Kurşunsuz95");
            SqlDataReader dr = com.ExecuteReader();
            if (dr.Read())
            {
                litre = Convert.ToDouble(numerickurşunsuz.Value);
                kurşunsuzfiyat = Convert.ToDouble(dr[3]);
                tutar = kurşunsuzfiyat * litre;
                txtKurşunsuzSatış.Text = tutar.ToString();
            }
            con.Close();
        }

        //Dizel fiyatını hesaplamak için.
        private void numericDizel_ValueChanged(object sender, EventArgs e)
        {
            double litre, dizelfiyat, tutar;
            con.Open();
            SqlCommand com = new SqlCommand("Select * From TblBenzin Where PetrolTur = @p1",con);
            com.Parameters.AddWithValue("@p1", "Diesel");
            SqlDataReader dr = com.ExecuteReader();
            if(dr.Read())
            {
                litre = Convert.ToDouble(numericDizel.Value);
                dizelfiyat = Convert.ToDouble(dr[3]);
                tutar = dizelfiyat * litre;
                txtDizelSatış.Text = tutar.ToString();
            }
            con.Close();
          
        }

        // Otogaz fiyatını hesaplamak için.
        private void numericOtogaz_ValueChanged(object sender, EventArgs e)
        {
            double litre, otogazfiyat, tutar;
            con.Open();
            SqlCommand com = new SqlCommand("Select * From TblBenzin Where PetrolTur = @p1", con);
            com.Parameters.AddWithValue("@p1", "Otogaz");
            SqlDataReader dr = com.ExecuteReader();
            if (dr.Read())
            {
                litre = Convert.ToDouble(numericOtogaz.Value);
                otogazfiyat = Convert.ToDouble(dr[3]);
                tutar = otogazfiyat * litre;
                txtOtogazSatış.Text = tutar.ToString();
            }
            con.Close();
        }

        //  Satılan benzinlerle alakalı gerekli tüm işlemler. (decimal.Parse kullanımı!!!!)
        private void button2_Click(object sender, EventArgs e)
        {
            if(numerickurşunsuz.Value!=0)
            {
                // Satılan benzinin parasını kasaya kaydetmek için yapılan matematiksel işlemler.
                double toplam, kasa1, satış;
                kasa1 = Convert.ToDouble(lblkasa.Text);
                satış = Convert.ToDouble(txtKurşunsuzSatış.Text);
                toplam = Convert.ToDouble(kasa1 + satış);

                // Satılan benzinden sonra kalan benzini kaydetmek için yapılan matematiksel işlemler.
                int stok, satılan,sonuç;
                stok = Convert.ToInt16(lblKurşunsuz.Text);
                satılan = Convert.ToInt16(numerickurşunsuz.Value);
                sonuç = Convert.ToInt16(stok - satılan);

                // Satılan benzini Sql'e kaydetmek için. 
                con.Open();
                SqlCommand com = new SqlCommand("insert into TblHareket (Plaka,BenzinTur,Litre,Fiyat) values (@p1,@p2,@p3,@p4)", con);
                com.Parameters.AddWithValue("@p1", txtPlaka.Text);
                com.Parameters.AddWithValue("@p2", "Kurşunsuz 95");
                com.Parameters.AddWithValue("@p3", decimal.Parse(numerickurşunsuz.Value.ToString()));
                com.Parameters.AddWithValue("@p4", decimal.Parse(txtKurşunsuzSatış.Text.ToString()));
                com.ExecuteNonQuery();
                con.Close(); MessageBox.Show("Satış yapıldı.","Bilgi",MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Satılan benzinden sonra kasadaki parayı güncellemek için.
                con.Open();
                SqlCommand com2 = new SqlCommand("update TblKasa set Kasa=@a1",con);
                com2.Parameters.AddWithValue("@a1", decimal.Parse(Convert.ToString(toplam)));
                com2.ExecuteNonQuery();
                con.Close();

                // Satılan benzinden sonra benzin stoğunu güncellemek için.
                con.Open();
                SqlCommand com3 = new SqlCommand("Update TblBenzin set Stok=@a1 Where Id=@a2 ", con);
                com3.Parameters.AddWithValue("@a1", Convert.ToInt16(sonuç));
                com3.Parameters.AddWithValue("@a2", 1);
                com3.ExecuteNonQuery();
                con.Close();
                kasa();
                petrolstok();
            }
            // Alınan ve Satılan benzinleri Sql'e kaydetmek için.(decimal.Parse kullanımı!!!!)
            else if (numericDizel.Value!=0)
            {
                // Satılan benzinin parasını kasaya kaydetmek için yapılan matematiksel işlemler.
                double toplam, kasa1, satış;
                kasa1 = Convert.ToDouble(lblkasa.Text);
                satış = Convert.ToDouble(txtDizelSatış.Text);
                toplam = Convert.ToDouble(kasa1 + satış);

                // Satılan benzinden sonra kalan benzini kaydetmek için yapılan matematiksel işlemler.
                int stok, satılan, sonuç;
                stok = Convert.ToInt16(lblDizel.Text);
                satılan = Convert.ToInt16(numericDizel.Value);
                sonuç = Convert.ToInt16(stok - satılan);

                // Satılan benzini Sql'e kaydetmek için. 
                con.Open();
                SqlCommand com = new SqlCommand("insert into TblHareket (Plaka,BenzinTur,Litre,Fiyat) values (@p1,@p2,@p3,@p4)", con);
                com.Parameters.AddWithValue("@p1", txtPlaka.Text);
                com.Parameters.AddWithValue("@p2", "Dizel");
                com.Parameters.AddWithValue("@p3", decimal.Parse(numericDizel.Value.ToString()));
                com.Parameters.AddWithValue("@p4", decimal.Parse(txtDizelSatış.Text.ToString()));
                com.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Satış yapıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Satılan benzinden sonra kasadaki parayı güncellemek için.
                con.Open();
                SqlCommand com2 = new SqlCommand("update TblKasa set Kasa=@a1", con);
                com2.Parameters.AddWithValue("@a1", decimal.Parse(Convert.ToString(toplam)));
                com2.ExecuteNonQuery();
                con.Close();

                // Satılan benzinden sonra benzin stoğunu güncellemek için.
                con.Open();
                SqlCommand com3 = new SqlCommand("Update TblBenzin set Stok=@a1 Where Id=@a2 ", con);
                com3.Parameters.AddWithValue("@a1", Convert.ToInt16(sonuç));
                com3.Parameters.AddWithValue("@a2", 2);
                com3.ExecuteNonQuery();
                con.Close();
                kasa();
                petrolstok();
            }

            // Alınan ve Satılan benzinleri Sql'e kaydetmek için.(decimal.Parse kullanımı!!!!)
            else if (numericOtogaz.Value!=0)
            {
                // Satılan benzinin parasını kasaya kaydetmek için yapılan matematiksel işlemler.
                double toplam, satış, kasa1;
                satış = Convert.ToDouble(txtOtogazSatış.Text);
                kasa1 = Convert.ToDouble(lblkasa.Text);
                toplam = Convert.ToDouble(satış + kasa1);

                // Satılan benzinden sonra kalan benzini kaydetmek için yapılan matematiksel işlemler.
                int stok, satılan, sonuç;
                stok = Convert.ToInt16(lblOtogaz.Text);
                satılan = Convert.ToInt16(numericOtogaz.Value);
                sonuç = Convert.ToInt16(stok - satılan);

                // Satılan benzini Sql'e kaydetmek için. 
                con.Open();
                SqlCommand com = new SqlCommand("insert into TblHareket (Plaka,BenzinTur,Litre,Fiyat) values (@p1,@p2,@p3,@p4)", con);
                com.Parameters.AddWithValue("@p1", txtPlaka.Text);
                com.Parameters.AddWithValue("@p2", "Otogaz");
                com.Parameters.AddWithValue("@p3", decimal.Parse(numericOtogaz.Value.ToString()));
                com.Parameters.AddWithValue("@p4", decimal.Parse(txtOtogazSatış.Text.ToString()));
                com.ExecuteNonQuery();
                con.Close();

                // Satılan benzinden sonra kasadaki parayı güncellemek için.
                MessageBox.Show("Satış yapıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                con.Open();
                SqlCommand com2 = new SqlCommand("Update TblKasa set Kasa=@a1",con);
                com2.Parameters.AddWithValue("@a1",decimal.Parse(Convert.ToString(toplam)));
                com2.ExecuteNonQuery();
                con.Close();

                // Satılan benzinden sonra benzin stoğunu güncellemek için.
                con.Open();
                SqlCommand com3 = new SqlCommand("Update TblBenzin set Stok=@a1 Where Id=@a2 ", con);
                com3.Parameters.AddWithValue("@a1", Convert.ToInt16(sonuç));
                com3.Parameters.AddWithValue("@a2", 3);
                com3.ExecuteNonQuery();
                con.Close();
                kasa();
                petrolstok();
            }
        }
        
        // Program başlar başlamaz verilerin girili olması için fonksiyonlarımızı çağırmalıyız.
        private void Form1_Load(object sender, EventArgs e)
        {
            petrolstok();
            kasa();
        }

        // Alınan benzinlerle alakalı tüm gerekli işlemler.
        private void button1_Click(object sender, EventArgs e)
        {
            int miktar, alış, sonuç,kasa1,label;
            int stok, alınan, sonuç1;
          
            // Kurşunsuz 95 alışın kasaya yansıması.
            if (txtKurşunsuzAlış.Text != "")
            {
                stok = Convert.ToInt16(lblKurşunsuz.Text);
                alınan = Convert.ToInt16(txtKurşunsuzAlış.Text);
                sonuç1 = Convert.ToInt16(stok + alınan);
                miktar = Convert.ToInt16(txtKurşunsuzAlış.Text);


                con.Open();
                SqlCommand com2 = new SqlCommand("Select * From TblBenzin Where Id = '1'", con);
                SqlDataReader dr = com2.ExecuteReader();
                while (dr.Read())
                {
                    alış = Convert.ToInt16(dr[2]);
                    label = Convert.ToInt32(alış * miktar);
                    kasa1 = Convert.ToInt16(lblkasa.Text);
                    sonuç = Convert.ToInt32(kasa1 - label);
                    label15.Text = sonuç.ToString();
                }
                con.Close();

                // Alış sonrası kasa güncellemesi.
                con.Open();
                SqlCommand com = new SqlCommand("Update TblKasa set Kasa=@a1", con);
                com.Parameters.AddWithValue("@a1", Convert.ToInt16(label15.Text));
                com.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Petrol alımı gerçekleştirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Alış sonrası stok güncellemesi.
                con.Open();
                SqlCommand com3 = new SqlCommand("Update TblBenzin set Stok=@a1 Where Id=@a2 ", con);
                com3.Parameters.AddWithValue("@a1", Convert.ToInt16(sonuç1));
                com3.Parameters.AddWithValue("@a2", 1);
                com3.ExecuteNonQuery();
                con.Close();
            }

            // Dizel alışın kasaya yansıması.
            else if (txtDizelAlış.Text != "")
            {
                stok = Convert.ToInt16(lblDizel.Text);
                alınan = Convert.ToInt16(txtDizelAlış.Text);
                sonuç1 = Convert.ToInt16(stok + alınan);
                miktar = Convert.ToInt16(txtDizelAlış.Text);

                con.Open();
                SqlCommand com3 = new SqlCommand("Select * From TblBenzin Where Id = '2'", con);
                SqlDataReader dr2 = com3.ExecuteReader();
                while (dr2.Read())
                {
                    alış = Convert.ToInt16(dr2[2]);
                    label = Convert.ToInt32(alış * miktar);
                    kasa1 = Convert.ToInt16(lblkasa.Text);
                    sonuç = Convert.ToInt32(kasa1 - label);
                    label15.Text = sonuç.ToString();
                }
                con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("Update TblKasa set Kasa=@a1", con);
                com.Parameters.AddWithValue("@a1", Convert.ToInt16(label15.Text));
                com.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Petrol alımı gerçekleştirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                con.Open();
                SqlCommand com4 = new SqlCommand("Update TblBenzin set Stok=@a1 Where Id=@a2 ", con);
                com4.Parameters.AddWithValue("@a1", Convert.ToInt16(sonuç1));
                com4.Parameters.AddWithValue("@a2", 2);
                com4.ExecuteNonQuery();
                con.Close();
            }
            // Otogaz alışın kasaya yansıması.
            else if (txtOtogazAlış.Text != "")
            {
                stok = Convert.ToInt16(lblOtogaz.Text);
                alınan = Convert.ToInt16(txtOtogazAlış.Text);
                sonuç1 = Convert.ToInt16(stok + alınan);
                miktar = Convert.ToInt16(txtOtogazAlış.Text);

                con.Open();
                SqlCommand com4 = new SqlCommand("Select * From TblBenzin Where Id = '3'", con);
                SqlDataReader dr3 = com4.ExecuteReader();
                while (dr3.Read())
                {
                    alış = Convert.ToInt16(dr3[2]);
                    label = Convert.ToInt32(alış * miktar);
                    kasa1 = Convert.ToInt16(lblkasa.Text);
                    sonuç = Convert.ToInt32(kasa1 - label);
                    label15.Text = sonuç.ToString();
                }
                con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("Update TblKasa set Kasa=@a1", con);
                com.Parameters.AddWithValue("@a1", Convert.ToInt16(label15.Text));
                com.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Petrol alımı gerçekleştirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                con.Open();
                SqlCommand com3 = new SqlCommand("Update TblBenzin set Stok=@a1 Where Id=@a2 ", con);
                com3.Parameters.AddWithValue("@a1", Convert.ToInt16(sonuç1));
                com3.Parameters.AddWithValue("@a2", 3);
                com3.ExecuteNonQuery();
                con.Close();

            }
            kasa();
            petrolstok();
        }
    }
}
