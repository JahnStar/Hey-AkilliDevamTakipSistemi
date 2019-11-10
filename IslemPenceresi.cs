using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using Point = System.Drawing.Point;
namespace HeyADTS
{
    public partial class islemPenceresi : Form
    {
        private readonly SerialPort cihaz;
        public islemPenceresi(SerialPort cihaz)
        {
            InitializeComponent();
            this.cihaz = cihaz;

            islemPenceresi_SizeChanged(null,null);
            excelS_ornek = excelSS.Image;

            if (cihaz != null) okunanKart.Enabled = false;
        }

        private void IslemPenceresi_Load(object sender, EventArgs e)
        {
            TumPanelleriKapat_Click(null, null);
            hizliBaslangic.Visible = true;

            if (hizliBaslangic.Visible)
                projeKaydet_buton.Enabled = disariAktar_buton.Enabled = kayitlariAl_buton.Enabled = duzenle_menu.Enabled = false;
            else projeKaydet_buton.Enabled = disariAktar_buton.Enabled = kayitlariAl_buton.Enabled = duzenle_menu.Enabled = true;
        }

        private void ProjeYeni_buton_Click(object sender, EventArgs e)
        {
            projeYeni_panel.Visible = true;
            projeYeni_panel.Location = new Point((this.Width - projeYeni_panel.Width) / 2, (this.Height - projeYeni_panel.Height) / 2);

            cozulenKayitlar = tumUyeler = null;
            kayitTarihleri.Items.Clear();
        }

        private void TumPanelleriKapat_Click(object sender, EventArgs e)
        {
            hizliBaslangic.Visible = false;
            if (!projeKaydet_buton.Enabled && projeYeni_panel.Visible) hizliBaslangic.Visible = true;
            projeYeni_panel.Visible = false;
            iceriAktar_panel.Visible = false;
            sutunSil_panel.Visible = false;
            arama_panel.Visible = false;
            excelDosyaYolu = "";
            ssOnizleme.Checked = false;
            excelSS.Image = excelS_ornek;
            kartEsleme_panel.Visible = false;
            siradakiUye = 0;
            oncekiSiralamaOlcutu = "";
            siralamaSadeceKartsizlar.Checked = false;
            siralaUyeAdi.Text = "Uye: Boyle bir uye bulunmuyor";
            okunanKart.Text = "Kart Bekleniyor...";
            sonBulunanSatir = 0;
            bulunanUye_label.Text = "Uye:";
            aranacakDeger_text.Text = "";
            arananSurun_combo.SelectedItem = null;

            if (sender != null && sender.ToString() == "UyeListeGuncelle") UyeListeGuncelle(null);

            if (hizliBaslangic.Visible)
                projeKaydet_buton.Enabled = disariAktar_buton.Enabled = kayitlariAl_buton.Enabled = duzenle_menu.Enabled = false;
            else projeKaydet_buton.Enabled = disariAktar_buton.Enabled = kayitlariAl_buton.Enabled = duzenle_menu.Enabled = true;
            foreach (DataGridViewColumn baslik in uyelerVeriListesi.Columns) // Siralamayi Kapat
                baslik.SortMode = DataGridViewColumnSortMode.NotSortable;

            guncelle.Start();
        }
        private void UyeListeGuncelle(string kayitliTarihFiltrele)
        {
            if (tumUyeler != null) 
            {
                if (kayitliTarihFiltrele != null && kayitliTarihFiltrele != "" && cozulenKayitlar != null) // kayitlari listele
                {
                    uyelerVeriListesi.Rows.Clear();
                    uyelerVeriListesi.ReadOnly = true;
                    List<Uye> cozulenUyeler = new List<Uye>();
                    for (int i = 0; i < cozulenKayitlar.Length; i++) // kayit birlestir
                    {
                        string uyeTumSaatler = "";
                        for (int s = 0; s < cozulenKayitlar.Length; s++)
                        {
                            if (cozulenKayitlar[i].cozulenKart == cozulenKayitlar[s].cozulenKart &&
                                cozulenKayitlar[i].cozulenTarih == cozulenKayitlar[s].cozulenTarih)
                            {
                                string saat = cozulenKayitlar[s].cozulenSaat.ToString().Split(':')[0].PadLeft(2, '0');
                                string dakika = cozulenKayitlar[s].cozulenSaat.ToString().Split(':')[1].PadLeft(2, '0');

                                uyeTumSaatler += saat + ":" + dakika + ", ";
                            }
                        }
                        cozulenUyeler.Add(new Uye(cozulenKayitlar[i].cozulenKart, cozulenKayitlar[i].cozulenTarih, uyeTumSaatler));
                    }

                    if (!kayitSirasi_checkBox.Checked) // Zaten var olanlari sil // sorun-hataya sebep olabilir - testi gecti 
                    {
                        List<Uye> cozulenUyeler_tekKisili = cozulenUyeler;
                        for (int i = 0; i < cozulenUyeler.Count; i++)
                        {
                            bool ilkDegeriAtla = false;
                            for (int j = 0; j < cozulenUyeler.Count; j++)
                            {
                                if (cozulenUyeler[i].cozulenKart == cozulenUyeler_tekKisili[j].cozulenKart &&
                                    cozulenUyeler[i].cozulenTarih == cozulenUyeler_tekKisili[j].cozulenTarih &&
                                    cozulenUyeler[i].cozulenSaat == cozulenUyeler_tekKisili[j].cozulenSaat)
                                {
                                    if (!ilkDegeriAtla) ilkDegeriAtla = true;
                                    else cozulenUyeler_tekKisili.RemoveAt(j);
                                }
                            }
                        }
                        cozulenUyeler = cozulenUyeler_tekKisili;
                    }

                    for (int i = 0; i < cozulenUyeler.Count; i++) // kayit goster
                    {
                        if (cozulenUyeler[i].cozulenTarih.Contains(kayitliTarihFiltrele))
                        {
                            for (int s = 0; s < tumUyeler.Length; s++)
                                if (tumUyeler[s].uyeVeri[kart_baslikSirasi] == cozulenUyeler[i].cozulenKart)
                                {
                                    string[] veri = new string[tumUyeler[s].uyeVeri.Length];
                                    Array.Copy(tumUyeler[s].uyeVeri, veri, tumUyeler[s].uyeVeri.Length);
                                    string saatDakika = cozulenUyeler[i].cozulenSaat.Remove(cozulenUyeler[i].cozulenSaat.Length - 2);
                                    veri[kart_baslikSirasi + 1] = saatDakika;

                                    uyelerVeriListesi.Rows.Add(veri);
                                }
                        }
                    }
                }
                else // listeyi sifirla
                {
                    kayitTarihleri.SelectedItem = null;
                    if (uyelerVeriListesi.Rows != null) uyelerVeriListesi.Rows.Clear();
                    for (int i = 0; i < tumUyeler.Length; i++) uyelerVeriListesi.Rows.Add(tumUyeler[i].uyeVeri);
                    uyelerVeriListesi.ReadOnly = false;
                }
                yuklenen_progressBar.Value = 0;
            }
        }

        private void SutunEkle_buton_Click(object sender, EventArgs e)
        {
            if (sutunEkle_adi.Text != "" && sutunEkle_adi.Text.Split(' ').Length - 1 != sutunEkle_adi.Text.Length) sutunlar.Nodes.Add(sutunEkle_adi.Text);

            listeOnIzleme.Columns.Clear();
            for (int i = 0; i < sutunlar.Nodes.Count; i++)
            {
                TreeNode sutun = sutunlar.Nodes[i];

                listeOnIzleme.Columns.Add(sutun.Text, sutun.Text);
                listeOnIzleme.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            }
            sutunEkle_adi.Text = "";
        }
        private void Sutunlar_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            DialogResult kartTanimla = MessageBox.Show("\"" + e.Node.Text + "\" sutunu silinsin mi?", "Islem Onayi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (kartTanimla != DialogResult.Yes) return;
            sutunlar.Nodes.Remove(e.Node);

            sutunEkle_adi.Text = "";
            SutunEkle_buton_Click(null,null);
        }
        private void Sutunlar_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            bilgiKutusu.Show("Silmek icin cift tiklayin", sutunlar);
        }
        private void SutunEkleYadaSil(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) SutunEkle_buton_Click(null, null);
            else if (e.KeyChar == (char)Keys.Delete && sutunlar.SelectedNode != null)
            {
                MessageBox.Show("\"" + sutunlar.SelectedNode.Text + "\" silindi");
                sutunlar.Nodes.Remove(sutunlar.SelectedNode);

                sutunEkle_adi.Text = "";
                SutunEkle_buton_Click(null, null);
            }
        }
        private void SutunEkleYadaSil(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) SutunEkleYadaSil(null, new KeyPressEventArgs((char)Keys.Delete));
        }
        private void ProjeOlustur_Click(object sender, EventArgs e)
        {
            if (listeOnIzleme.Columns.Count > 0)
            {
                projeYeni_panel.Visible = false;
                TumPanelleriKapat_Click(null, null);
                uyelerVeriListesi.Columns.Clear();
                for (int i = 0; i < listeOnIzleme.Columns.Count; i++)
                {
                    string sutunAdi = listeOnIzleme.Columns[i].HeaderText;
                    uyelerVeriListesi.Columns.Add(sutunAdi, sutunAdi);
                }
                uyelerVeriListesi.Columns.Add("Kart No", "Kart No");
                uyelerVeriListesi.Columns[uyelerVeriListesi.Columns.Count - 1].ReadOnly = true;
                uyelerVeriListesi.Columns.Add("Geldigi Saatlar", "Geldigi Saatlar");
                uyelerVeriListesi.Columns[uyelerVeriListesi.Columns.Count - 1].ReadOnly = true;

                sutunlar.Nodes.Clear();
                SutunEkle_buton_Click(null, null);
            }
            else MessageBox.Show("\"Sutunlar\" bos olamaz", "Hata");
        }

        private string excelDosyaYolu = "";
        private void IceriAktar_DosyaSec_Click(object sender, EventArgs e)
        {
            guncelle.Stop();
            MessageBox.Show("Her satirin (uyenin) ilk degerinin farkli oldugundan emin olun. ilk sutun degeri uyenin kimligini belirtir ve her uyenin kimligi farkli olmalidir.", "Onemli",MessageBoxButtons.OK, MessageBoxIcon.Information);
            using (OpenFileDialog file = new OpenFileDialog() { Filter = "Excel Dosyasi |*.xlsx| Excel Dosyasi|*.xls", Title = "Excel Dosyasi Seciniz..", FilterIndex = 2, RestoreDirectory = true, })
            {
                if (file.ShowDialog() == DialogResult.OK)
                {
                    TumPanelleriKapat_Click(null, null);
                    excelDosyaYolu = file.FileName;
                    icerAktar_dosyaAdi.Text = "Iceri Aktar: "  + file.SafeFileName;

                    iceriAktar_panel.Visible = true;
                }
                else 
                {
                    guncelle.Start();
                    return;
                }
            }
            guncelle.Start();        
        }
        private void IceriAktar_buton_Click(object sender, EventArgs e)
        {
            guncelle.Stop();
            int sutun = 0;
            try
            {
                if (excelDosyaYolu != "" && iceriAktar_sutun.Text != "")
                {
                    sutun = int.Parse(iceriAktar_sutun.Text);

                    Application uygulama = new Application();
                    Workbook kitap = uygulama.Workbooks.Open(excelDosyaYolu);
                    Worksheet sayfa = new Worksheet(); // Sayfa acma butomnu koy
                    try { sayfa = kitap.Application.Sheets[iceriAktar_sayfa.Text]; }
                    catch { sayfa = kitap.Application.Sheets[1]; }
                    Range kullanilanAralik = sayfa.UsedRange;

                    int satirSayisi = kullanilanAralik.Rows.Count;
                    int sutunSayisi = kullanilanAralik.Columns.Count;

                    if (tumUyeler == null) projeBirlestir_checkBox.Checked = false;
                    if (projeBirlestir_checkBox.Checked)
                    {
                        for (int s = 0; s < sutunSayisi; s++)
                        {
                            Range aralik = sayfa.Cells[sutun, s + 1];

                            bool baslikVar = false;
                            for (int j = 0; j < uyelerVeriListesi.Columns.Count - 2; j++)
                                if (uyelerVeriListesi.Columns[j].HeaderText == aralik.Value + "") 
                                {
                                    baslikVar = true;
                                    break;
                                }

                            if (sutunSayisi != uyelerVeriListesi.Columns.Count - 2) baslikVar = false;
                            if (!baslikVar) 
                            {
                                MessageBox.Show("Basliklar mevcut basliklar ile ayni degil. 'Baslik satiri' degerini dogru girdiginizden veya " +
                                "basliklarin ayni oldugundan emin olduktan sonra tekrar deneyin.", "Basliklar uyumsuz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                TumPanelleriKapat_Click(null, null);
                                return;
                            }
                        }
                    }
                    if (!projeBirlestir_checkBox.Checked) tumUyeler = new Uye[satirSayisi - sutun];
                    uyelerVeriListesi.Columns.Clear();
                    uyelerVeriListesi.Rows.Clear();
                    // Sutun ekle
                    for (int s = 0; s < sutunSayisi; s++)
                    {
                        Range aralik = sayfa.Cells[sutun, s + 1];
                        uyelerVeriListesi.Columns.Add(aralik.Value + "", aralik.Value2 + "");
                    }

                    yuklenen_progressBar.Maximum = satirSayisi;
                    if (iceriAktar_veriyle.Checked && !projeBirlestir_checkBox.Checked) for (int i = sutun; i < satirSayisi; i++) // Yeni proje
                        {
                            List<string> tekSatir = new List<string>();
                            for (int s = 0; s < sutunSayisi; s++)
                            {
                                Range aralik = sayfa.Cells[i + 1, s + 1];
                                tekSatir.Add(aralik.Value + "");
                            }

                            tumUyeler[i - sutun] = new Uye(tekSatir.ToArray());

                            yuklenen_progressBar.Value = i;
                            bilgiKutusu1.Text = "Uyeler iceri aktariliyor... %" + 100 * i / yuklenen_progressBar.Maximum;
                        }
                    if (iceriAktar_veriyle.Checked && projeBirlestir_checkBox.Checked) for (int i = sutun; i < satirSayisi; i++) // Projeye ekle
                        {
                            List<string> tekSatir = new List<string>();
                            for (int s = 0; s < sutunSayisi; s++)
                            {
                                Range aralik = sayfa.Cells[i + 1, s + 1];
                                tekSatir.Add(aralik.Value + "");
                            }

                            bool uyeZatenVar = false;
                            for (int j = 0; j < tumUyeler.Length; j++) if (tumUyeler[j].uyeVeri[0] == tekSatir[0])
                                {
                                    uyeZatenVar = true;
                                    break;
                                }

                            if (!uyeZatenVar)
                            {
                                Array.Resize(ref tumUyeler, tumUyeler.Length + 1);
                                tumUyeler[tumUyeler.GetUpperBound(0)] = new Uye(tekSatir.ToArray());
                                //MessageBox.Show(tekSatir[1] + " eklendi"); // duzelt
                            }

                            yuklenen_progressBar.Value = i;
                            bilgiKutusu1.Text = "Uyeler iceri aktariliyor (Birlestiriliyor)... %" + 100 * i / yuklenen_progressBar.Maximum;
                        }
                    uyelerVeriListesi.Columns.Add("Kart No", "Kart No");
                    uyelerVeriListesi.Columns[uyelerVeriListesi.Columns.Count - 1].ReadOnly = true;
                    uyelerVeriListesi.Columns.Add("Geldigi Saatlar", "Geldigi Saatlar");
                    uyelerVeriListesi.Columns[uyelerVeriListesi.Columns.Count - 1].ReadOnly = true;
                    kart_baslikSirasi = uyelerVeriListesi.Columns.Count - 2;

                    TumPanelleriKapat_Click("UyeListeGuncelle", null);

                    kitap.Close();
                    uygulama.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(uygulama);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(kitap);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sayfa);

                    bilgiKutusu1.Text = "Uyeler iceri aktarildi! %100";
                }
                else MessageBox.Show("\"Baslik satiri\" bos olamaz", "Hata");
            }
            catch (Exception hata) 
            {
                MessageBox.Show(hata.Message);
            }
            guncelle.Start();
        }
        private void iceriAktar_sutun_MouseHover(object sender, EventArgs e)
        {
            if (!ssOnizleme.Checked) 
            {
                iceriAktar_anim.Visible = true;
                iceriAktar_anim2.Visible = false;
            }
        }
        private void iceriAktar_sayda_MouseHover(object sender, EventArgs e)
        {
            if (!ssOnizleme.Checked) 
            {
                iceriAktar_anim2.Visible = true;
                iceriAktar_anim.Visible = false;
            }
        }
        private void iceriAktar_sayda_MouseLeave(object sender, EventArgs e)
        {
            iceriAktar_anim2.Visible = false;
            iceriAktar_anim.Visible = false;
        }
        private void SayisalKarakterDenetim(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)Keys.Back) e.Handled = true;

            if (iceriAktar_sutun.Text.Length == 0 && e.KeyChar == (char)Keys.D0) e.Handled = true;
        }

        private Image excelS_ornek;
        private void ssOnizleme_CheckedChanged(object sender, EventArgs e)
        {
            if (excelDosyaYolu != "" && ssOnizleme.Checked)
            {
                Application geciciUygulama = new Application();
                Workbook kitap = geciciUygulama.Workbooks.Open(excelDosyaYolu);
                try
                {
                    Worksheet sayfa = kitap.Sheets[iceriAktar_sayfa.Text];
                    Range r = sayfa.Range["A1:D20"];
                    r.CopyPicture(XlPictureAppearance.xlScreen, XlCopyPictureFormat.xlBitmap);
                    if (Clipboard.GetDataObject() != null) excelSS.Image = (Image)Clipboard.GetDataObject().GetData(DataFormats.Bitmap, true);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sayfa);
                }
                catch 
                {
                    Worksheet sayfa = kitap.Sheets[1];
                    Range r = sayfa.Range["A1:D20"];
                    r.CopyPicture(XlPictureAppearance.xlScreen, XlCopyPictureFormat.xlBitmap);
                    if (Clipboard.GetDataObject() != null) excelSS.Image = (Image)Clipboard.GetDataObject().GetData(DataFormats.Bitmap, true);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sayfa);
                }
                kitap.Close();
                geciciUygulama.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(geciciUygulama);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(kitap);
            }
            else excelSS.Image = excelS_ornek;
        }
        private Uye[] tumUyeler, cozulenKayitlar;
        private string kayitDosyaYolu = "";
        private void KayitlariAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog file = new OpenFileDialog() { Filter = "Hey Adts Dosyasi|*.txt", Title = "Hey Adts Dosyasi Seciniz...", FilterIndex = 2, RestoreDirectory = true, })
            {
                if (file.ShowDialog() == DialogResult.OK) kayitDosyaYolu = file.FileName;
                else
                {
                    guncelle.Start();
                    return;
                }
            }
            if (kayitDosyaYolu != "") 
            {
                TumPanelleriKapat_Click(null, null);
                string alinanVeri = File.ReadAllText(kayitDosyaYolu);
                // Tarihleri Ekle
                if (kayitTarihleri.Items.Count > 0) kayitTarihleri.Items.Clear();
                string[] kayitlar = new string[alinanVeri.Split('(').Length];
                int kayitMiktari = kayitlar.Length;
                cozulenKayitlar = new Uye[kayitMiktari - 1];
                for (int i = 0; i < kayitMiktari - 1; i++) // Kayitlari ayikla
                {
                    int s = i + 1 < kayitMiktari ? i + 1 : i;

                    string kart = alinanVeri.Split('(')[s].Split(')')[0].Replace('?', ' '); // karti ayikla ve coz
                    string tarih = alinanVeri.Split('(')[s].Split(')')[1]; // tarihi ve saati ayikla
                    string saat = tarih.Split('?')[1].Split(')')[0]; // saati ayikla
                    tarih = tarih.Split('?')[0]; // tarihi ayikla
                    cozulenKayitlar[i] = new Uye(kart, tarih, saat);

                    string tarihVerisi = tarih;
                    for (int t = 0; t < kayitTarihleri.Items.Count; t++)
                        if (tarihVerisi == kayitTarihleri.Items[t] + "") tarihVerisi = null;
                    if (tarihVerisi != null && tarihVerisi.Split('.').Length == 3) kayitTarihleri.Items.Add(tarihVerisi);
                    kayitDosyaYolu = "";
                }

                bilgiKutusu1.Text = "<= Kayitlar Yuklendi! %100 Devamsizliklari gormek icin bir tarih secin.";
            }
        }
        private int kart_baslikSirasi;
        private void KartlariDegistir_Click(object sender, EventArgs e)
        {
            kartEsleme_panel.Visible = true;
            okunanKart.Text = "Kart Bekleniyor...";

            islemPenceresi_SizeChanged(null, null);

            siralaSiradaki_buton.Enabled = false;
            siralamaOlcutu.Items.Clear();
            for (int i = 0; i < uyelerVeriListesi.Columns.Count - 2; i++)
            {
                string baslik = uyelerVeriListesi.Columns[i].HeaderText;
                siralamaOlcutu.Items.Add(baslik);
            }

            kart_baslikSirasi = uyelerVeriListesi.Columns.Count - 2;
        }
        string oncekiSiralamaOlcutu = "";
        bool terstenSirala = false;
        int siradakiUye = 0;

        private void siralamaOlcutu_SelectedIndexChanged(object tiklamaOlayiDegil, EventArgs e)
        {
            try
            {
                if (siralamaOlcutu.Text != null) 
                {
                    bool tiklamaOlayiDegil1 = false;
                    bool.TryParse(tiklamaOlayiDegil + "", out tiklamaOlayiDegil1);
                    if (siralamaOlcutu.Text == oncekiSiralamaOlcutu && !tiklamaOlayiDegil1) terstenSirala = !terstenSirala;
                    //uyelerVeriListesi.Sort(uyelerVeriListesi.Columns[siralamaOlcutu.SelectedIndex], terstenSirala ? ListSortDirection.Descending : ListSortDirection.Ascending);
                    oncekiSiralamaOlcutu = siralamaOlcutu.Text;

                    siradakiUye = 0;
                    if (!siralamaSadeceKartsizlar.Checked) siralaUyeAdi.Text = uyelerVeriListesi.Rows[siradakiUye].Cells[siralamaOlcutu.SelectedIndex].Value + "";
                    else siralamaSadeceKartsizlar_CheckedChanged(null, null);

                    okunanKart_TextChanged(null,null);
                }
            }
            catch { }
        }
        private void siralaSiradaki_buton_Click(object sender, EventArgs e)
        {
            try 
            {
                bool yazmaIzniVar = true;
                if (ozelKart.Checked == true) for (int i = 0; i < uyelerVeriListesi.Rows.Count; i++)
                    {
                        if (uyelerVeriListesi.Rows[i].Cells[kart_baslikSirasi].Value + "" == okunanKart.Text)
                        {
                            yazmaIzniVar = false;
                            MessageBox.Show("Ayni karti farkli uyelere de tanimlamak istiyorsaniz \"Her uye icin ozel kart\"i devre disi birakin.", "Kart Tanimlanmadi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }

                if (yazmaIzniVar)
                {
                    if (okunanKart.Text != "Kart Bekleniyor..." && okunanKart.Text.Length > 6) for (int i = 0; i < tumUyeler.Length; i++)
                        if (siralaUyeAdi.Text == tumUyeler[i].uyeVeri[siralamaOlcutu.SelectedIndex] + "")
                        {
                            tumUyeler[i].uyeVeri[kart_baslikSirasi] = okunanKart.Text;
                            siradakiUye = i + 1;
                        }
                }

                if (siradakiUye < uyelerVeriListesi.Rows.Count - 1) siralaUyeAdi.Text = uyelerVeriListesi.Rows[siradakiUye].Cells[siralamaOlcutu.SelectedIndex].Value + "";
                else TumPanelleriKapat_Click(null, null);

                UyeListeGuncelle(null);
                okunanKart.Text = "Kart Bekleniyor...";
                siralaSiradaki_buton.Enabled = false;
            }
            catch { }
        }
        private void okunanKart_TextChanged(object sender, EventArgs e)
        {
            if (okunanKart.Text != "Kart Bekleniyor..." && okunanKart.Text.Length > 6 && 
                siralamaOlcutu.SelectedItem != null && !siralaUyeAdi.Text.Contains("Uye: Boyle bir uye bulunmuyor")) siralaSiradaki_buton.Enabled = true;
        }
        private void okunanKart_Click(object sender, MouseEventArgs e)
        {
            if (okunanKart.Text == "Kart Bekleniyor...") okunanKart.Text = "";
        }
        private void kayitTarihleri_SelectedIndexChanged(object sender, EventArgs e)
        {
            UyeListeGuncelle(kayitTarihleri.Text);
        }
        private async void ProjeAc_buton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog pencere = new OpenFileDialog() { Filter = "HeyDbFile|*.heydb", ValidateNames = true, Multiselect = false })
            {
                guncelle.Stop();
                if (pencere.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader oku = new StreamReader(pencere.FileName))
                    {
                        cozulenKayitlar = tumUyeler = null;
                        kayitTarihleri.Items.Clear();
                        uyelerVeriListesi.Columns.Clear();
                        uyelerVeriListesi.Rows.Clear();
                        var sifreliVeri = Convert.FromBase64String(await oku.ReadToEndAsync());
                        string veri = Encoding.UTF8.GetString(sifreliVeri);
                        for (int i = 0; i < veri.Split('\n').Length; i++) 
                        {
                            if (i == 0) 
                            {
                                for (int s = 0; s < veri.Split('\n')[i].Split(',').Length - 1; s++)
                                    uyelerVeriListesi.Columns.Add(veri.Split('\n')[i].Split(',')[s] + "", veri.Split('\n')[i].Split(',')[s] + "");

                                kart_baslikSirasi = uyelerVeriListesi.Columns.Count - 2;
                                uyelerVeriListesi.Columns[kart_baslikSirasi].ReadOnly = true;
                                uyelerVeriListesi.Columns[kart_baslikSirasi+1].ReadOnly = true;
                            }
                            if (i == 1)
                            {
                                string[] uyelerVeri = new string[veri.Split('\n')[i].Split(',').Length - 1];
                                for (int s = 0; s < veri.Split('\n')[i].Split(',').Length - 1; s++) uyelerVeri[s] = veri.Split('\n')[i].Split(',')[s] + "";

                                tumUyeler = new Uye[uyelerVeri.Length / uyelerVeriListesi.Columns.Count];
                                for (int s = 0; s < tumUyeler.Length; s++)
                                {
                                    List<string> tekSatir = new List<string>();
                                    for (int d = 0; d < uyelerVeriListesi.Columns.Count; d++)
                                        tekSatir.Add(uyelerVeri[d + (s * uyelerVeriListesi.Columns.Count)]);
                                    tumUyeler[s] = new Uye(tekSatir.ToArray());
                                }
                                TumPanelleriKapat_Click("UyeListeGuncelle", null);
                            }
                            if (i == 2 && veri.Split('\n')[i].Split(',').Length > 0)
                            {
                                if (kayitTarihleri.Items.Count > 0) kayitTarihleri.Items.Clear();
                                cozulenKayitlar = new Uye[veri.Split('\n')[i].Split(',').Length - 1];
                                for (int s = 0; s < cozulenKayitlar.Length; s++) 
                                {
                                    cozulenKayitlar[s] = new Uye(
                                        veri.Split('\n')[i].Split(',')[s].Split('-')[0], 
                                        veri.Split('\n')[i].Split(',')[s].Split('-')[1], 
                                        veri.Split('\n')[i].Split(',')[s].Split('-')[2]);

                                    string tarihVerisi = cozulenKayitlar[s].cozulenTarih;
                                    for (int t = 0; t < kayitTarihleri.Items.Count; t++)
                                        if (tarihVerisi == kayitTarihleri.Items[t] + "") tarihVerisi = null;
                                    if (tarihVerisi != null) kayitTarihleri.Items.Add(cozulenKayitlar[s].cozulenTarih);
                                }
                            }
                        }
                    }
                }
                else
                {
                    guncelle.Start();
                    return;
                }
                bilgiKutusu1.Text = "Proje yuklendi! %100";
                guncelle.Start();
            }
        }
        private async void ProjeKaydet_buton_Click(object sender, EventArgs e)
        {
            guncelle.Stop();
            DialogResult kayitlariKaydet = MessageBox.Show("Devamsizlik kayitlari da kaydedilsin mi?", "Islem Onayi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            using (SaveFileDialog pencere = new SaveFileDialog()
            { FileName = (DateTime.Now.Day + "0" + DateTime.Now.Month + "0" + DateTime.Now.Year) + "",  Filter = "HeyDbFile|*.heydb", ValidateNames = true })
            {
                if (pencere.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter yaz = new StreamWriter(pencere.FileName))
                    {
                        if (tumUyeler != null)
                        {
                            string veriler = "";
                            for (int i = 0; i < uyelerVeriListesi.Columns.Count; i++) veriler += uyelerVeriListesi.Columns[i].HeaderText + ",";
                            veriler += "\n";
                        
                            for (int i = 0; i < tumUyeler.Length; i++) for (int s = 0; s < uyelerVeriListesi.Columns.Count; s++)
                                    veriler += tumUyeler[i].uyeVeri[s] + ",";
                        
                            veriler += "\n";
                            if (cozulenKayitlar != null && kayitlariKaydet == DialogResult.Yes)
                            {
                                for (int i = 0; i < cozulenKayitlar.Length; i++) 
                                    veriler += cozulenKayitlar[i].cozulenKart + "-" + cozulenKayitlar[i].cozulenTarih + "-" + cozulenKayitlar[i].cozulenSaat + ",";
                            }
                            else veriler += " ";
                            var sifreliVeri = Encoding.UTF8.GetBytes(veriler);
                            await Task.Run(() => yaz.WriteLineAsync(Convert.ToBase64String(sifreliVeri)));
                        }
                    }
                }
                else
                {
                    guncelle.Start();
                    return;
                }
            }
            bilgiKutusu1.Text = "Proje kaydedildi! %100";
            guncelle.Start();
        }
        private void filtreKaldir_buton_Click(object sender, EventArgs e)
        {
            kayitTarihleri.SelectedItem = null;
            bilgiKutusu1.Text = "Tum Uyeler (Tanimlanan)";
            UyeListeGuncelle(null);
        }

        private void guncelle_Tick(object sender, EventArgs e)
        {
            if (cihaz != null)
            {
                if (!cihaz.IsOpen) cihaz.Open();

                string alinanVeri = cihaz.ReadExisting();
                cihaz.DiscardInBuffer();

                if (alinanVeri != null && alinanVeri != "") 
                {
                    VerileriCoz veriCoz = new VerileriCoz();
                    if (alinanVeri.Contains("#") && veriCoz.KartKimlikCoz(ref alinanVeri))
                    {
                        if (kartEsleme_panel.Visible) okunanKart.Text = alinanVeri;
                        else Clipboard.SetText(alinanVeri);
                        try
                        {
                            if (uyelerVeriListesi.CurrentCell.ColumnIndex == kart_baslikSirasi && !kartEsleme_panel.Visible)
                            {
                                bool uzerineYaz = false;
                                bool zatenMevcut = false;

                                string seciliHucreDeger = uyelerVeriListesi.CurrentCell.Value + "";
                                for (int i = 0; i < uyelerVeriListesi.Rows.Count; i++)
                                    if (uyelerVeriListesi.Rows[i].Cells[kart_baslikSirasi].Value + "" == alinanVeri)
                                    {
                                        zatenMevcut = true;
                                        guncelle.Stop();
                                        DialogResult kartTanimla = MessageBox.Show("\"" + alinanVeri + "\" karti baska bir uyede tanimli. Yinede bu uyeler icin de tanimlamak istermisiniz?", "Islem Onayi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (kartTanimla == DialogResult.Yes)
                                        {
                                            uzerineYaz = true;
                                            guncelle.Start();
                                            break;
                                        }
                                        else if (kartTanimla == DialogResult.No || kartTanimla == DialogResult.Cancel)
                                        {
                                            guncelle.Start();
                                            return;
                                        }
                                    }

                                if (seciliHucreDeger == "") uzerineYaz = true;

                                if (uzerineYaz || (!uzerineYaz && !zatenMevcut))
                                {
                                    DialogResult kartTanimla = MessageBox.Show("\"" + seciliHucreDeger + "\" > " + "\"" + alinanVeri + "\"" + " karti olarak degistirilsin mi?", "Islem Onayi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (kartTanimla == DialogResult.Yes)
                                    {
                                        if (seciliHucreDeger != "" && !tekHucreKartDegistir)
                                        {
                                            for (int i = 0; i < uyelerVeriListesi.Rows.Count; i++)
                                                if (uyelerVeriListesi.Rows[i].Cells[kart_baslikSirasi].Value + "" == seciliHucreDeger)
                                                    uyelerVeriListesi.Rows[i].Cells[kart_baslikSirasi].Value = alinanVeri;
                                        }
                                        else
                                        {
                                            uyelerVeriListesi.CurrentCell.Value = alinanVeri;
                                            tekHucreKartDegistir = false;
                                        }

                                        uyelerVeriListesi_Kaydet(null, null);
                                    }
                                    else return;
                                }
                            }
                            else if (!kartEsleme_panel.Visible) for (int i = 0; i < uyelerVeriListesi.Rows.Count; i++)
                                    if (uyelerVeriListesi.Rows[i].Cells[kart_baslikSirasi].Value + "" == alinanVeri) 
                                    {
                                        uyelerVeriListesi.ClearSelection();
                                        uyelerVeriListesi.Rows[i].Selected = true;
                                        uyelerVeriListesi.Rows[i].Cells[0].Selected = true;
                                        break;
                                    }
                        }
                        catch { }
                    }
                }
            }
            if (kayitTarihleri.SelectedItem != null)
            {
                if (kayitSirasi_checkBox.Checked) bilgiKutusu1.Text = "Okutulan Kart:" + (uyelerVeriListesi.Rows.Count - 1) + " / Tum Uyeler:" + tumUyeler.Length;
                else bilgiKutusu1.Text = "Gelen Uyeler:" + (uyelerVeriListesi.Rows.Count - 1) + " / Tum Uyeler:" + tumUyeler.Length;
            }
            else if (tumUyeler != null) 
            { 
                if (cihaz != null && uyelerVeriListesi.CurrentCell.ColumnIndex == kart_baslikSirasi && !kartEsleme_panel.Visible)
                        bilgiKutusu1.Text = "Tanimlama icin kart bekleniyor... (Tek bir hucreye uygulamak icin 'Ctrl' basili iken karti okutun)";
                else bilgiKutusu1.Text = "Tum Uyeler:" + tumUyeler.Length;
            }
        }

        private void islemPenceresi_SizeChanged(object sender, EventArgs e)
        {
            projeYeni_panel.Location = new Point((this.Width - projeYeni_panel.Width) / 2, (this.Height - projeYeni_panel.Height) / 2);
            iceriAktar_panel.Location = new Point((this.Width - iceriAktar_panel.Width) / 2, (this.Height - iceriAktar_panel.Height) / 2);
            hizliBaslangic.Location = new Point((this.Width - hizliBaslangic.Width) / 2, (this.Height - hizliBaslangic.Height) / 2);
            kartEsleme_panel.Location = new Point((this.Width - kartEsleme_panel.Width) / 2, (this.Height - kartEsleme_panel.Height) / 2);
            arama_panel.Location = new Point((this.Width - arama_panel.Width) / 2, (this.Height - arama_panel.Height) / 2);
            sutunSil_panel.Location = new Point((this.Width - sutunSil_panel.Width) / 2, (this.Height - sutunSil_panel.Height) / 2);
        }
        private void uyelerVeriListesi_Kaydet(object sender, DataGridViewCellEventArgs e) // Uyeleri guncelle
        {
            if (kayitTarihleri.SelectedItem == null) 
            {
                kart_baslikSirasi = uyelerVeriListesi.Columns.Count - 2;
                int satirSayisi = uyelerVeriListesi.Rows.Count - 1;
                int sutunSayisi = uyelerVeriListesi.Columns.Count - 1;
                tumUyeler = new Uye[satirSayisi];

                DataGridViewRowCollection sutunlar = uyelerVeriListesi.Rows;
                for (int i = 0; i < satirSayisi; i++)
                {
                    List<string> tekSatir = new List<string>();
                    for (int s = 0; s < sutunSayisi; s++) tekSatir.Add(sutunlar[i].Cells[s].Value + "");
                    tumUyeler[i] = new Uye(tekSatir.ToArray());
                }
            }
        }

        private void ByHalilEmreYildiz_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/jahn_star/");
        }
        bool tekHucreKartDegistir;
        private void uyelerVeriListesi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "ControlKey") tekHucreKartDegistir = true;
        }

        private void uyelerVeriListesi_KeyUp(object sender, KeyEventArgs e)
        {
            tekHucreKartDegistir = false;
        }

        private void tumTarihler_disariAktar_Click(object sender, EventArgs e)
        {
            DisariAktar_Excel(true);
        }

        private void DisariAktar_Excel(bool tumunuDisariAktar)
        {
            try
            {
                object misValue = System.Reflection.Missing.Value;

                Application uygulama = new Application();
                Workbook kitap = uygulama.Workbooks.Add(misValue);
                Worksheet sayfa = (Worksheet)kitap.Application.Sheets[1];

                uygulama.DisplayAlerts = false;
                kitap.CheckCompatibility = false;
                kitap.DoNotPromptForConvert = true;

                DataGridView mevcutListe = uyelerVeriListesi;

                if (mevcutListe.Rows.Count > 0 && kayitTarihleri.Items.Count > 0)
                {
                    int dosyaSayisi = tumunuDisariAktar ? kayitTarihleri.Items.Count : 1;
                    if (dosyaSayisi == 1)
                    {
                        if (kayitTarihleri.SelectedItem == null)
                        {
                            MessageBox.Show("'Tarihler' bolumunden bir tarih secin ve tekrar deneyin.", "Tarih Bulunamadi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        if (mevcutListe.Rows.Count < 2)
                        {
                            MessageBox.Show("Boyle bir uye bulunmuyor", "Uyeler Bulunamadi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    
                    string kayitYolu = "";
                    guncelle.Stop();
                    using (var pencere = new FolderBrowserDialog())
                    {
                        DialogResult sonuc = pencere.ShowDialog();

                        if (sonuc == DialogResult.OK && !string.IsNullOrWhiteSpace(pencere.SelectedPath))
                        {
                            kayitYolu = pencere.SelectedPath;
                            guncelle.Start();
                        }
                        else
                        {
                            guncelle.Start();
                            return;
                        }
                    }

                    for (int a = 0; a < dosyaSayisi; a++)
                    {
                        if (dosyaSayisi != 1) 
                        {
                            kayitTarihleri.SelectedItem = kayitTarihleri.Items[a];
                            if (mevcutListe.Rows.Count < 2 && a + 1 < kayitTarihleri.Items.Count)
                            {
                                a++;
                                kayitTarihleri.SelectedItem = kayitTarihleri.Items[a];
                            }
                            else if (a + 1 > kayitTarihleri.Items.Count)
                            {
                                uygulama.ActiveWorkbook.Close();
                                uygulama.Quit();
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(uygulama);
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(kitap);
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(sayfa);
                                return;
                            }
                        }

                        for (int i = 0; i < mevcutListe.Columns.Count; i++)
                        {
                            Range baslik = (Range)sayfa.Cells[1, i + 1];
                            baslik.Value = mevcutListe.Columns[i].HeaderText.ToUpper();
                            baslik.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(61, 158, 239));
                            baslik.Font.Name = "Arial";
                            baslik.Font.Bold = true;
                            baslik.Font.Color = Color.White;
                            baslik.Borders.Color = System.Drawing.Color.Black.ToArgb();
                        }

                        for (int s = 0; s < tumUyeler.Length; s++)
                        {
                            bool uyeVar = false;
                            for (int i = 0; i < uyelerVeriListesi.Rows.Count; i++)
                                if (uyelerVeriListesi.Rows[i].Cells[kart_baslikSirasi].Value + "" == tumUyeler[s].uyeVeri[kart_baslikSirasi])
                                {
                                    uyeVar = true;
                                    break;
                                }

                            if (!uyeVar && tumUyeler[s].uyeVeri[kart_baslikSirasi] != null && tumUyeler[s].uyeVeri[kart_baslikSirasi] != "")
                            {
                                string[] uyeVeri = tumUyeler[s].uyeVeri;
                                uyeVeri[kart_baslikSirasi + 1] = "Gelmedi";
                                uyelerVeriListesi.Rows.Add(uyeVeri);
                            }
                            else if (!uyeVar)
                            {
                                string[] uyeVeri = tumUyeler[s].uyeVeri;
                                uyeVeri[kart_baslikSirasi + 1] = "Kart/Kayit Yok";
                                uyelerVeriListesi.Rows.Add(uyeVeri);
                            }
                        }
                        yuklenen_progressBar.Maximum = mevcutListe.Rows.Count;

                        if (mevcutListe.Rows.Count > 1) for (int i = 0; i < mevcutListe.Rows.Count; i++)
                            {
                                for (int j = 0; j < mevcutListe.Columns.Count; j++) 
                                { 
                                    Range hucre = (Range)sayfa.Cells[i + 2, j + 1];
                                    hucre.Value = mevcutListe.Rows[i].Cells[j].Value + "";
                                    hucre.Borders.Color = System.Drawing.Color.Black.ToArgb();
                                }
                                bilgiKutusu1.Text = "Devamsizlik kayitlari disari aktariliyor... %" + (100 * i / yuklenen_progressBar.Maximum);
                                yuklenen_progressBar.Value = i;
                            }

                        uygulama.Columns.AutoFit();

                        kitap.SaveAs(kayitYolu + "\\" + kayitTarihleri.Text + ".xls", XlFileFormat.xlWorkbookNormal, misValue, misValue,
                        misValue, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);

                        System.Diagnostics.Process.Start(kayitYolu);

                        for (int s = 0; s < tumUyeler.Length; s++) tumUyeler[s].uyeVeri[kart_baslikSirasi + 1] = "Tarih Secilmemis";
                    }

                    uygulama.ActiveWorkbook.Close();
                    uygulama.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(uygulama);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(kitap);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sayfa);
                }
                else MessageBox.Show("Cihaz bagliyken 'Devamsizlik Kayitlarini Yukle' butonuna tiklayin ve tekrar deneyin.", "Kayit Bulunamadi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                uyelerVeriListesi.Rows.Clear();
                UyeListeGuncelle(null);
                bilgiKutusu1.Text = "Kayitlar disari aktarildi! %100";
            }
            catch (Exception hata) { MessageBox.Show(hata + ""  , hata.Message, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void seciliTarih_disariAktar_Click(object sender, EventArgs e)
        {
            DisariAktar_Excel(false);
        }

        private void araToolStripMenuItem_Click(object sender, EventArgs e)
        {
            arama_panel.Visible = true;
        }

        private void kapatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void arananSurun_combo_MouseClick(object sender, EventArgs e)
        {
            arananSurun_combo.Items.Clear();
            for (int i = 0; i < uyelerVeriListesi.Columns.Count - 2; i++)
            {
                string baslik = uyelerVeriListesi.Columns[i].HeaderText;
                arananSurun_combo.Items.Add(baslik);
            }
            arananSurun_combo.Items.Add("Kart No");
        }
        int sonBulunanSatir;
        private void aranacakDeger_TextChanged(object sender, EventArgs e)
        {
            try 
            {
                for (int i = 0; i < uyelerVeriListesi.Rows.Count; i++) 
                {
                    string arananUye = uyelerVeriListesi.Rows[i].Cells[arananSurun_combo.SelectedIndex].Value.ToString().ToLower();
                    if (arananUye.ToLower().Contains(aranacakDeger_text.Text.ToLower()))
                    {
                        bulunanUye_label.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(arananUye);
                        sonBulunanSatir = i;

                        uyelerVeriListesi.ClearSelection();
                        uyelerVeriListesi.Rows[sonBulunanSatir].Selected = true;

                        return;
                    }
                }
            }
            catch {}
        }

        private void sonrakiniAra_buton_Click(object sender, EventArgs e)
        {
            try
            {
                bool dondur = false;
                for (int i = 0; i < uyelerVeriListesi.Rows.Count; i++)
                {
                    string arananUye = uyelerVeriListesi.Rows[i].Cells[arananSurun_combo.SelectedIndex].Value.ToString().ToLower();
                    if (arananUye.ToLower().Contains(aranacakDeger_text.Text.ToLower()))
                    {
                        if (bulunanUye_label.Text != arananUye && sonBulunanSatir < i && !dondur)
                        {
                            bulunanUye_label.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(arananUye);
                            sonBulunanSatir = i;
                            dondur = true;

                            uyelerVeriListesi.ClearSelection();
                            uyelerVeriListesi.Rows[sonBulunanSatir].Selected = true;
                        }
                    }
                    if (!dondur && sonBulunanSatir < i) 
                    {
                        sonBulunanSatir = 0; // Basa don
                        string sonUye = bulunanUye_label.Text;
                        if (!bulunanUye_label.Text.Contains("Uye:")) bulunanUye_label.Text = "Son Uye: " + sonUye;
                    }
                }
            }
            catch { }
        }

        private void arananSurun_combo_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            aranacakDeger_TextChanged(null, null);
        }

        private void bulunanUye_label_Click(object sender, EventArgs e)
        {
            if (sonBulunanSatir != 0) 
            {
                uyelerVeriListesi.ClearSelection();
                uyelerVeriListesi.Rows[sonBulunanSatir].Selected = true;
                uyelerVeriListesi.Rows[sonBulunanSatir].Cells[arananSurun_combo.SelectedIndex].Selected = true;
            }
        }

        private void uyelerVeriListesi_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (uyelerVeriListesi.Rows.Count > 0 && !uyelerVeriListesi.ReadOnly) 
            {
                MessageBox.Show("\"" + e.RowIndex + "\" sutunu silindi.");
                uyelerVeriListesi_Kaydet(null, null);
                uyelerVeriListesi.ClearSelection();
                uyelerVeriListesi.Rows[e.RowIndex].Selected = true;
            }
        }

        private void islemPenceresi_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!hizliBaslangic.Visible && tumUyeler != null) 
            {
                DialogResult projeKaydetme = MessageBox.Show("Projeyi kaydedip cikmak istermisiniz?", "Kaydet ve cik", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (projeKaydetme == DialogResult.Yes)
                {
                    UyeListeGuncelle(null);
                    uyelerVeriListesi_Kaydet(null, null);
                    ProjeKaydet_buton_Click(null, null);
                }
            }
        }

        private void kayitSirasi_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            filtreKaldir_buton_Click(null, null);
        }

        private void sutunlariSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sutunSil_panel.Visible = true;
        }

        private void sutunSil_sutunlar_comboBox_Click(object sender, EventArgs e)
        {
            sutunSil_sutunlar_comboBox.Items.Clear();
            for (int i = 0; i < uyelerVeriListesi.Columns.Count; i++) 
                sutunSil_sutunlar_comboBox.Items.Add(uyelerVeriListesi.Columns[i].HeaderText);
        }

        private void sutunuSil_Click(object sender, EventArgs e)
        {
            if (sutunSil_sutunlar_comboBox.SelectedItem != null && uyelerVeriListesi.Columns != null)
            {
                TumPanelleriKapat_Click("UyeListeGuncelle", null);
                uyelerVeriListesi.Columns.RemoveAt(sutunSil_sutunlar_comboBox.SelectedIndex);
                uyelerVeriListesi_Kaydet(null, null);
            }
        }

        private void siralamaSadeceKartsizlar_CheckedChanged(object sender, EventArgs e)
        {
            try 
            {
                if (siralamaOlcutu.Text != null && siralamaSadeceKartsizlar.Checked)
                {
                    siralaUyeAdi.Text = "Uye: Boyle bir uye bulunmuyor";
                    for (int i = 0; i < uyelerVeriListesi.Rows.Count - 1; i++)
                        if (uyelerVeriListesi.Rows[i].Cells[kart_baslikSirasi].Value + "" == "")
                        {
                            siralaUyeAdi.Text = uyelerVeriListesi.Rows[i].Cells[siralamaOlcutu.SelectedIndex].Value + "";
                            return;
                        }
                }
                else if (siralamaOlcutu.Text != null) siralamaOlcutu_SelectedIndexChanged("True", null);
                okunanKart_TextChanged(null, null);
            }
            catch { }
        }
    }
    public class Uye
    {
        public string[] uyeVeri;
            public string cozulenKart = "", cozulenTarih = "", cozulenSaat = "";
        public Uye(string[] uyeVeri)
        {
            this.uyeVeri = new string[uyeVeri.Length + 2];
            for (int i = 0; i < this.uyeVeri.Length; i++) 
                if (i < uyeVeri.Length) this.uyeVeri[i] = uyeVeri[i];
        }
        public Uye(string cozulenKart, string cozulenTarih, string cozulenSaat) 
        {
            this.cozulenKart = cozulenKart;
            this.cozulenTarih = cozulenTarih;
            this.cozulenSaat = cozulenSaat;
        }
    }
}