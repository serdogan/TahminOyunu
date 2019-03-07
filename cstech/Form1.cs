using cstech.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cstech
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int bilgisayarTutulanSayi;   // Bilgisayarın tuttuğu rastgele değer
        public int bilgisayarTahminEttigiSayi;   // Kullanıcının tuttuğu sayıyı bulmak için ilk tahmin
        public int bilgisayarGonderilenArti = 0;
        public int bilgisayarGonderilenEksi = 0;
        public int artiEksiSayisi = -1;   // Toplam 4 olursa doğru sayıları buldum demek
        public int elemanKonumu = 0;   // Tahmin edilen sayıları bulmak için kullandım
        public int oncekiDeger = 0;
        public int degisen = 0;
        public int artiBilgisi = -1;

        public string deger;   // Sayının başına 0 değeri gelince int dönüşümünde almıyor bu yüzden değişken kullandım
        public List<Tahminler> Tahminler = new List<Tahminler>();   // Kullanıcının tahminlerini DataGridViewRow'da göstermek için

        public int kullaniciBulunanSayiKonum = 0;   // Kullanıcının tuttuğu sayının doğru rakamlarını bulduktan sonra sayıyı bulmak için kullanılan değişkenler
        public int kullaniciBulunanSayiKonumIlerle = 1;
        protected int olmayanSayiKontrol = 0;
        protected int eskiHali = 0;
        protected int kontrolDegistiMi = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            bilgisayarTutulanSayi = sayiUret();
            bilgisayarTahminEttigiSayi = sayiUret();
        }

        /// <summary>
        /// Rastgele sayı üretiliyor. Üretilen sayının rakamları farklı olana kadar sayı üretiyor ve rakamları farklı olan sayıyı bulunca gönderiyor
        /// </summary>
        /// <returns></returns>
        private int sayiUret()
        {
            Random rastgele = new Random();
            int uretilenSayi = 0;
            int kontrol = 1;   // Bilgisayarın ürettiği sayıda aynı rakam var mı kontrolü için kullanılıyor
            while (kontrol == 1)
            {
                kontrol = 0;
                uretilenSayi = rastgele.Next(1000, 9999);
                char[] sayi = uretilenSayi.ToString().ToCharArray();
                for (int i = 0; i < uretilenSayi.ToString().Length - 1; i++)
                {
                    if (uretilenSayi.ToString().Substring(i + 1, (uretilenSayi.ToString().Length - 1) - i).Contains(sayi[i]))
                    {
                        kontrol = 1;
                        break;
                    }
                }
            }
            return uretilenSayi;
        }

        /// <summary>
        /// Kullanıcı tahmininin kontrolleri yapılıyor ve ipucu veriliyor
        /// </summary>
        private void kullanici_Click(object sender, EventArgs e)   // Kullanıcının girdiği sayının kontrol kısmı
        {
            int ayni = 0;
            int farkli = 0;
            string kullaniciTahmini = textBox1.Text;
            if (kullaniciTahmini.Length != 4)
            {
                MessageBox.Show("Lütfen 4 basamaklı sayı giriniz!");
                textBox1.Focus();
                return;
            }
            char[] kullaniciTahminiKarakter = kullaniciTahmini.ToCharArray();
            char[] bilgisayarTutulanKarakter = bilgisayarTutulanSayi.ToString().ToCharArray();
            for (int i = 0; i < bilgisayarTutulanSayi.ToString().Length; i++)
            {
                if (kullaniciTahminiKarakter[i] == bilgisayarTutulanKarakter[i])
                    ayni++;
                else if (bilgisayarTutulanSayi.ToString().Contains(kullaniciTahminiKarakter[i]))
                    farkli++;
            }
            label2.Text = "Girilen Son Tahmin:" + textBox1.Text + "     +" + ayni + "   -" + farkli;

            Tahminler.Add(new Tahminler() { Deger = Convert.ToInt32(textBox1.Text), TahminSirasi = Tahminler.Count + 1 });
            grdTahminler.DataSource = Tahminler.ToList();

            textBox1.Clear();

            if (ayni == bilgisayarTutulanSayi.ToString().Length)
            {
                MessageBox.Show("Tebrikler Oyunu Kullanıcı Kazandı");
                Environment.Exit(0);
            }
            kullanici.Visible = false;
            bilgisayar.Visible = true;
            if (bilgisayarTahminEttigiSayi.ToString().Length == 3)
                label6.Text = "0" + bilgisayarTahminEttigiSayi.ToString();
            else
                label6.Text = bilgisayarTahminEttigiSayi.ToString();
        }

        // Kullanıcının tahminini girdiği textbox'a sadece sayı girilmesi için
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        // Kullanıcının bilgisayara bilgi(+) verdiği kısma sadece sayı girilmesi için
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        // Kullanıcının bilgisayara bilgi(-) verdiği kısma sadece sayı girilmesi için
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        /// <summary>
        /// Bilgisayar tahminine ve bilgisayara verilen ipuçlarına göre kontroller yapılıyor ve bilgisayarın bir sonraki tahmini hesaplanıyor 
        /// </summary>
        private void bilgisayar_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "4")
            {
                MessageBox.Show("Oyunu Bilgisayar Kazandı");
                Environment.Exit(0);
            }
            if (textBox2.Text != "")
                bilgisayarGonderilenArti = Convert.ToInt32(textBox2.Text);
            if (textBox3.Text != "")
                bilgisayarGonderilenEksi = Convert.ToInt32(textBox3.Text);

            textBox2.Clear();
            textBox3.Clear();

            int kontrol = bilgisayarGonderilenArti + bilgisayarGonderilenEksi;
            deger = bilgisayarTahminEttigiSayi.ToString();
            if (deger.Length == 3)
            {
                deger = "0" + deger;
            }
            char[] tahminDegeri = deger.ToCharArray();

            if (kontrol > 4 || kontrol < 0)
            {
                MessageBox.Show("Lütfen + ve - değerler toplamını 0 ile 4 arasında giriniz!");
                return;
            }

            if (artiEksiSayisi != 4 && kontrol != 4)
            {
                if (artiEksiSayisi != -1)
                {
                    if (artiEksiSayisi < kontrol)
                    {
                        if (elemanKonumu != 3)
                        {
                            elemanKonumu++;
                            ortak(2);
                            artiEksiSayisi = kontrol;
                        }
                    }
                    else if (artiEksiSayisi > kontrol)
                    {
                        tahminDegeri[elemanKonumu] = Convert.ToChar(oncekiDeger.ToString());
                        elemanKonumu++;

                        bilgisayarTahminiYazdırma(tahminDegeri);

                        ortak(3);
                    }
                    else if (artiEksiSayisi == kontrol)
                    {
                        ortak(4);
                    }
                }
                else
                {
                    ortak(1);
                    artiEksiSayisi = kontrol;
                }
            }
            else
            {
                if (artiBilgisi != -1)
                {
                    if (kullaniciBulunanSayiKonum == 2 && kullaniciBulunanSayiKonumIlerle == 4)
                    {
                        MessageBox.Show("Tebrikler Oyunu Kullanıcı Kazandı");
                        Environment.Exit(0);
                    }
                    if (artiBilgisi > bilgisayarGonderilenArti)
                    {
                        if (kontrolDegistiMi == 1)
                        {
                            tahminDegeri[kullaniciBulunanSayiKonum] = Convert.ToChar(oncekiDeger.ToString());
                            bilgisayarTahminiYazdırma(tahminDegeri);
                        }
                        kullaniciBulunanSayiKonum++;
                        kullaniciBulunanSayiKonumIlerle = kullaniciBulunanSayiKonumIlerle + 1;

                        oncekiDeger = tahminDegeri[kullaniciBulunanSayiKonum];
                        tahminDegeri[kullaniciBulunanSayiKonum] = Convert.ToChar(olmayanSayiKontrol.ToString());
                        bilgisayarTahminiYazdırma(tahminDegeri);
                        kontrolDegistiMi = 1;
                    }
                    else if (artiBilgisi < bilgisayarGonderilenArti)
                    {
                        if (kontrolDegistiMi == 1)
                        {
                            tahminDegeri[kullaniciBulunanSayiKonum] = Convert.ToChar(oncekiDeger.ToString());
                            bilgisayarTahminiYazdırma(tahminDegeri);
                        }
                        else
                        {
                            eskiHali = bilgisayarTahminEttigiSayi;

                        }
                        kullaniciBulunanSayiKonum++;
                        kullaniciBulunanSayiKonumIlerle = kullaniciBulunanSayiKonum + 1;

                        oncekiDeger = Convert.ToInt32(tahminDegeri[kullaniciBulunanSayiKonum].ToString());
                        tahminDegeri[kullaniciBulunanSayiKonum] = Convert.ToChar(olmayanSayiKontrol.ToString());
                        bilgisayarTahminiYazdırma(tahminDegeri);
                        kontrolDegistiMi = 1;

                        artiBilgisi = bilgisayarGonderilenArti;
                    }
                    else
                    {
                        if (kontrolDegistiMi == 1)
                        {
                            tahminDegeri[kullaniciBulunanSayiKonum] = Convert.ToChar(oncekiDeger.ToString());
                            bilgisayarTahminiYazdırma(tahminDegeri);
                        }
                        yerDegistirme();
                        kullaniciBulunanSayiKonumIlerle++;
                        if (kullaniciBulunanSayiKonumIlerle == 4)
                            kullaniciBulunanSayiKonumIlerle = kullaniciBulunanSayiKonum + 1;
                        kontrolDegistiMi = 0;
                    }
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (bilgisayarTahminEttigiSayi.ToString().Contains(olmayanSayiKontrol.ToString()))
                        {
                            olmayanSayiKontrol++;
                            if (olmayanSayiKontrol == 10)
                                olmayanSayiKontrol = 0;
                        }
                        else
                            break;
                    }
                    artiBilgisi = bilgisayarGonderilenArti;
                    eskiHali = bilgisayarTahminEttigiSayi;
                    if (bilgisayarGonderilenArti == 0)
                    {
                        yerDegistirme();
                        kullaniciBulunanSayiKonumIlerle++;
                    }
                    else
                    {
                        char[] bulunan = bilgisayarTahminEttigiSayi.ToString().ToCharArray();
                        oncekiDeger = bulunan[kullaniciBulunanSayiKonum];
                        bulunan[kullaniciBulunanSayiKonum] = Convert.ToChar(olmayanSayiKontrol.ToString());
                        kontrolDegistiMi = 1;
                        bilgisayarTahminiYazdırma(bulunan);
                    }
                }
            }
            label6.Text = "";
            kullanici.Visible = true;
            bilgisayar.Visible = false;
            textBox1.Focus();
        }

        /// <summary>
        /// Bilgisayar tahmininin hesaplandığı kısım
        /// </summary>
        /// <param name="geldigiYer">Büyük, küçük, eşit ve ilk tahmin sonucuna göre geldiği yer</param>
        private void ortak(int geldigiYer)
        {
            char[] tahminDegeri;
            if (bilgisayarTahminEttigiSayi.ToString().Length == 3)   // Tahmin edilen sayı 0 ile başlıyor mu kontrolü
            {
                deger = "0" + bilgisayarTahminEttigiSayi.ToString();
                tahminDegeri = deger.ToCharArray();
            }
            else
                tahminDegeri = bilgisayarTahminEttigiSayi.ToString().ToCharArray();

            if (geldigiYer == 1 || geldigiYer == 2 || geldigiYer == 3) // Büyük, küçük ve ilk tahmin durumu
            {
                oncekiDeger = Convert.ToInt32(tahminDegeri[elemanKonumu].ToString());
                degisen = oncekiDeger + 1;
            }
            else if (geldigiYer == 4)  // Eşitlik durumu
            {
                degisen = degisen + 1;
            }
            if (degisen == 10)
                degisen = 0;
            while (deger.Contains(degisen.ToString()))
            {
                degisen = degisen + 1;
                if (degisen == 10)
                    degisen = 0;
            }
            tahminDegeri[elemanKonumu] = Convert.ToChar(degisen.ToString());
            string sayi = "";
            for (int i = 0; i < 4; i++)
            {
                sayi = sayi + tahminDegeri[i].ToString();
            }
            bilgisayarTahminEttigiSayi = Convert.ToInt32(sayi);
        }

        /// <summary>
        /// Doğru rakamları bulduktan sonra yerlerini değiştirerek sayıyı bulmak için
        /// </summary>
        private void yerDegistirme()   // Swap işlemini burada yapıyor
        {
            char[] tahminDegeri = eskiHali.ToString().ToCharArray();
            char swap = tahminDegeri[kullaniciBulunanSayiKonum];
            tahminDegeri[kullaniciBulunanSayiKonum] = tahminDegeri[kullaniciBulunanSayiKonumIlerle];
            tahminDegeri[kullaniciBulunanSayiKonumIlerle] = swap;

            bilgisayarTahminiYazdırma(tahminDegeri);
        }

        /// <summary>
        /// Sayıyı char dizisine atarak yapılan işlemlerden sonra sayıyı birleştirip int'a çeviriyor
        /// </summary>
        /// <param name="tahminDegeri">Sayının char dizisi hali</param>
        private void bilgisayarTahminiYazdırma(char[] tahminDeger)
        {
            string sayi = "";
            for (int i = 0; i < 4; i++)
            {
                sayi = sayi + tahminDeger[i].ToString();
            }
            bilgisayarTahminEttigiSayi = Convert.ToInt32(sayi);
        }

    }
}
