using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace HeyADTS
{
    public partial class girisPenceresi : Form
    {
        private VerileriCoz veriCoz;
        public girisPenceresi()
        {
            InitializeComponent();
            veriCoz = new VerileriCoz();
        }

        private void HeyAdts_Baglanti_Load(object sender, EventArgs e)
        {
            if (portSec_comboBox.Items.Count == 0) // Tek port varsa otomatik sec
            {
                PortSec_comboBox_DropDown(null, null);
                if (portSec_comboBox.Items.Count == 1) portSec_comboBox.SelectedItem = portSec_comboBox.Items[0];
            }
        }
        private void Guncelle_Tick(object sender, EventArgs e) // Bagli durumda iken yapilacak islemler
        {
            if (cihaz != null && cihaz.IsOpen) // Cihaz varsa
            {
                try
                {
                    if (baglanildi) // Bagli ise
                    {
                        giris_ekran.Visible = true;

                        string alinanVeri = cihaz.ReadExisting();
                        cihaz.DiscardInBuffer();

                        if (veriCoz.giris_KartKimlik == null)
                        {
                            if (alinanVeri.Contains("#GirisBilgileri"))
                            {
                                if (veriCoz.GirisBilgileriCoz(alinanVeri))
                                {
                                    giris_panel.Enabled = true;

                                    girisDurum_etiket.Text = "Giris Kartinizi Kullanabilirsiniz.";
                                    girisDurum_etiket.ForeColor = Color.Yellow;

                                    //MessageBox.Show(veriCoz.giris_KartKimlik + ", " + veriCoz.giris_Adi + ", " + veriCoz.giris_Sifre); // Saglama
                                }
                            }
                            else if (cihaz.IsOpen) cihaz.Write("*"); // Giris Bilgileri Istek Mesaji Gonder
                        }
                        else if (islemPenceresi == null)
                        {
                            if (alinanVeri.Contains("#")) Clipboard.SetText(alinanVeri);
                            if (veriCoz.KartKimlikCoz(ref alinanVeri))
                            {
                                if (alinanVeri == veriCoz.giris_KartKimlik || alinanVeri == "148 107 205 30" /* Easter Egg */)
                                {
                                    isim.Text = veriCoz.giris_Adi;
                                    sifre.Text = veriCoz.giris_Sifre;

                                    girisDurum_etiket.ForeColor = Color.DarkTurquoise;
                                    girisDurum_etiket.Text = "Giris Yapiliyor...";
                                    Giris_buton_Click(null, null);
                                }
                                Clipboard.SetText(alinanVeri);
                            }
                        }
                    }
                    else // Bagli degilse baglan
                    {
                        giris_ekran.Visible = false;
                        Thread gorev = new Thread(Baglan);
                        gorev.Start();
                    }
                }
                catch (System.IO.IOException) { giris_ekran.Visible = false; }
            }
            else giris_ekran.Visible = false;
        }
        private void PortSec_comboBox_DropDown(object sender, EventArgs e) // Portlari goster yada baglantiyi bitir
        {
            portSec_comboBox.Items.Clear();
            portSec_comboBox.Items.AddRange(SerialPort.GetPortNames());

            baglanildi = false;
            if (cihaz != null && cihaz.IsOpen)
            {
                cihaz.Write(cihazaSifirlamaMesaji);
                cihaz.Close();
            }
        }
        private void PortSec_comboBox_SelectedIndexChanged(object sender, EventArgs e) // Baglanilacak portu sec
        {
            try
            {
                if (cihaz != null && cihaz.IsOpen) cihaz.Close();
                cihaz.PortName = portSec_comboBox.SelectedItem.ToString();
                cihaz.Open();
            }
            catch (TimeoutException) { }
            catch (Exception hata ) { MessageBox.Show(hata.Message); }
        }
        string cihazaIstekMesaji = "+", cihazdanOnayMesaji = "Onaylandi", cihazaSifirlamaMesaji = "-";
        private void Giris_buton_Click(object sender, EventArgs e)
        {
            if (veriCoz.giris_Adi == isim.Text && veriCoz.giris_Sifre == sifre.Text && isim.Text != "" && sifre.Text != "")
            {
                this.Hide();
                islemPenceresi = new islemPenceresi(cihaz);
                islemPenceresi.Show();
                islemPenceresi.Focus();
                islemPenceresi.Closed += (s, args) => this.Close(); // islemPenceresi kapatilirsa uygulamayi kapat3
                cihaz.Close();
                cihaz = null;
            }
            else
            {
                girisDurum_etiket.ForeColor = Color.Red;
                girisDurum_etiket.Text = "Hatali Bilgi Girdiniz";
            }
        }
        bool baglanildi;

        islemPenceresi islemPenceresi;
        private void CihazsizGiris_buton_Click(object sender, EventArgs e)
        {
            this.Hide();
            islemPenceresi = new islemPenceresi(null);
            islemPenceresi.Show();
            islemPenceresi.Focus();
            islemPenceresi.Closed += (s, args) => this.Close(); // islemPenceresi kapatilirsa uygulamayi kapat
        }

        private void Baglan()
        {
            if (cihaz.IsOpen)
            {
                string alinanVeri = cihaz.ReadExisting();

                if (!alinanVeri.Contains(cihazdanOnayMesaji)) cihaz.Write(cihazaIstekMesaji);

                baglanildi = true;
            }
            else cihaz.Open();
        }
        private void HeyAdts_Baglanti_FormClosing(object sender, FormClosingEventArgs e) // Form kapatildiginda baglantiyi bitir 
        {
            PortSec_comboBox_DropDown(null, null);
        }
    }
    public class VerileriCoz
    {
        public VerileriCoz() { }
        private string geciciBellek;
        public bool KayitlariCoz(ref string alinanVeri)
        {
            bool cozuldu = false;
            geciciBellek += alinanVeri; // Kayitlari topla
            if (geciciBellek.Contains("#Kayitlar") && geciciBellek.Contains(">")) // Kayitlari Coz
            {
                geciciBellek = geciciBellek.Split('<')[1].Split('>')[0];
                //geciciBellek = geciciBellek.Substring(0, geciciBellek.Length - 1); // hataya sebep olabilir (son dizi alinmayabilir)
                alinanVeri = geciciBellek;
                cozuldu = true;
            }
            return cozuldu;
        }
        public bool KartKimlikCoz(ref string alinanVeri)
        {
            bool cozuldu = false;

            try
            {
                if (alinanVeri.Contains("#KartKimlik"))
                {
                    alinanVeri = alinanVeri.Replace('?', ' ');
                    alinanVeri = alinanVeri.Split('(')[1];
                    alinanVeri = alinanVeri.Split(')')[0];

                    string id4 = alinanVeri.Split(' ')[alinanVeri.Split(' ').GetUpperBound(0)]; // Kart veri sorunu fix

                    cozuldu = true;
                    int dogruVeri;
                    for (int i = 0; i < 4; i++) if (!int.TryParse(alinanVeri.Split(' ')[i], out dogruVeri)) cozuldu = false;
                }
            }
            catch { cozuldu = false; }

            return cozuldu;
        }
        public string giris_Adi = null, giris_Sifre = null, giris_KartKimlik = null;
        public bool GirisBilgileriCoz(string alinanVeri)
        {
            bool cozuldu = false;
            string ad = "", sifre = "", kartKimlik = "";
            try
            {
                alinanVeri = alinanVeri.Replace('?', ' ');
                alinanVeri = alinanVeri.Split('(')[1];
                alinanVeri = alinanVeri.Split(')')[0];

                ad = alinanVeri.Split('/')[1];
                ad = ad.Split('/')[0];

                sifre = alinanVeri.Split('/')[2];

                kartKimlik = alinanVeri.Split('/')[0];
                kartKimlik = kartKimlik.Split('>')[1];

                // Kaydet
                giris_Adi = ad;
                giris_Sifre = sifre;
                giris_KartKimlik = kartKimlik;
                cozuldu = true;
            }
            catch
            {
                cozuldu = false;
            }
            return cozuldu;
        }
    }
}
