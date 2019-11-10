namespace HeyADTS
{
    partial class girisPenceresi
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(girisPenceresi));
            this.cihaz = new System.IO.Ports.SerialPort(this.components);
            this.portSec_comboBox = new System.Windows.Forms.ComboBox();
            this.guncelle = new System.Windows.Forms.Timer(this.components);
            this.etiket1 = new System.Windows.Forms.Label();
            this.giris_ekran = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.durumSimge = new System.Windows.Forms.PictureBox();
            this.kartBekleniyor = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.giris_panel = new System.Windows.Forms.Panel();
            this.giris_buton = new System.Windows.Forms.Button();
            this.isim = new System.Windows.Forms.TextBox();
            this.girisDurum_etiket = new System.Windows.Forms.Label();
            this.sifre = new System.Windows.Forms.TextBox();
            this.bilgi1 = new System.Windows.Forms.Label();
            this.bilgi2 = new System.Windows.Forms.Label();
            this.cihazsizGiris_buton = new System.Windows.Forms.Button();
            this.klavuz_panel = new System.Windows.Forms.PictureBox();
            this.giris_ekran.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.durumSimge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kartBekleniyor)).BeginInit();
            this.giris_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.klavuz_panel)).BeginInit();
            this.SuspendLayout();
            // 
            // cihaz
            // 
            this.cihaz.ReadTimeout = 1000;
            // 
            // portSec_comboBox
            // 
            resources.ApplyResources(this.portSec_comboBox, "portSec_comboBox");
            this.portSec_comboBox.BackColor = System.Drawing.Color.White;
            this.portSec_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.portSec_comboBox.ForeColor = System.Drawing.Color.Black;
            this.portSec_comboBox.FormattingEnabled = true;
            this.portSec_comboBox.Name = "portSec_comboBox";
            this.portSec_comboBox.DropDown += new System.EventHandler(this.PortSec_comboBox_DropDown);
            this.portSec_comboBox.SelectedIndexChanged += new System.EventHandler(this.PortSec_comboBox_SelectedIndexChanged);
            // 
            // guncelle
            // 
            this.guncelle.Enabled = true;
            this.guncelle.Interval = 50;
            this.guncelle.Tick += new System.EventHandler(this.Guncelle_Tick);
            // 
            // etiket1
            // 
            resources.ApplyResources(this.etiket1, "etiket1");
            this.etiket1.Name = "etiket1";
            // 
            // giris_ekran
            // 
            resources.ApplyResources(this.giris_ekran, "giris_ekran");
            this.giris_ekran.Controls.Add(this.label2);
            this.giris_ekran.Controls.Add(this.durumSimge);
            this.giris_ekran.Controls.Add(this.kartBekleniyor);
            this.giris_ekran.Controls.Add(this.label1);
            this.giris_ekran.Controls.Add(this.giris_panel);
            this.giris_ekran.Name = "giris_ekran";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.label2.Name = "label2";
            this.label2.Tag = "";
            // 
            // durumSimge
            // 
            resources.ApplyResources(this.durumSimge, "durumSimge");
            this.durumSimge.Image = global::HeyADTS.Properties.Resources.Baglanildi;
            this.durumSimge.Name = "durumSimge";
            this.durumSimge.TabStop = false;
            // 
            // kartBekleniyor
            // 
            resources.ApplyResources(this.kartBekleniyor, "kartBekleniyor");
            this.kartBekleniyor.Name = "kartBekleniyor";
            this.kartBekleniyor.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(181)))), ((int)(((byte)(37)))));
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // giris_panel
            // 
            resources.ApplyResources(this.giris_panel, "giris_panel");
            this.giris_panel.Controls.Add(this.giris_buton);
            this.giris_panel.Controls.Add(this.isim);
            this.giris_panel.Controls.Add(this.girisDurum_etiket);
            this.giris_panel.Controls.Add(this.sifre);
            this.giris_panel.Controls.Add(this.bilgi1);
            this.giris_panel.Controls.Add(this.bilgi2);
            this.giris_panel.Name = "giris_panel";
            // 
            // giris_buton
            // 
            resources.ApplyResources(this.giris_buton, "giris_buton");
            this.giris_buton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.giris_buton.FlatAppearance.BorderSize = 2;
            this.giris_buton.ForeColor = System.Drawing.Color.White;
            this.giris_buton.Name = "giris_buton";
            this.giris_buton.UseVisualStyleBackColor = false;
            this.giris_buton.Click += new System.EventHandler(this.Giris_buton_Click);
            // 
            // isim
            // 
            resources.ApplyResources(this.isim, "isim");
            this.isim.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.isim.Name = "isim";
            // 
            // girisDurum_etiket
            // 
            resources.ApplyResources(this.girisDurum_etiket, "girisDurum_etiket");
            this.girisDurum_etiket.ForeColor = System.Drawing.Color.Red;
            this.girisDurum_etiket.Name = "girisDurum_etiket";
            // 
            // sifre
            // 
            resources.ApplyResources(this.sifre, "sifre");
            this.sifre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sifre.Name = "sifre";
            this.sifre.UseSystemPasswordChar = true;
            // 
            // bilgi1
            // 
            resources.ApplyResources(this.bilgi1, "bilgi1");
            this.bilgi1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.bilgi1.Name = "bilgi1";
            this.bilgi1.Tag = "";
            // 
            // bilgi2
            // 
            resources.ApplyResources(this.bilgi2, "bilgi2");
            this.bilgi2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.bilgi2.Name = "bilgi2";
            this.bilgi2.Tag = "";
            // 
            // cihazsizGiris_buton
            // 
            resources.ApplyResources(this.cihazsizGiris_buton, "cihazsizGiris_buton");
            this.cihazsizGiris_buton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.cihazsizGiris_buton.FlatAppearance.BorderSize = 2;
            this.cihazsizGiris_buton.ForeColor = System.Drawing.Color.White;
            this.cihazsizGiris_buton.Name = "cihazsizGiris_buton";
            this.cihazsizGiris_buton.UseVisualStyleBackColor = false;
            this.cihazsizGiris_buton.Click += new System.EventHandler(this.CihazsizGiris_buton_Click);
            // 
            // klavuz_panel
            // 
            resources.ApplyResources(this.klavuz_panel, "klavuz_panel");
            this.klavuz_panel.Image = global::HeyADTS.Properties.Resources.BaglantiKlavuzu;
            this.klavuz_panel.Name = "klavuz_panel";
            this.klavuz_panel.TabStop = false;
            // 
            // girisPenceresi
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.Controls.Add(this.giris_ekran);
            this.Controls.Add(this.cihazsizGiris_buton);
            this.Controls.Add(this.klavuz_panel);
            this.Controls.Add(this.etiket1);
            this.Controls.Add(this.portSec_comboBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "girisPenceresi";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HeyAdts_Baglanti_FormClosing);
            this.Load += new System.EventHandler(this.HeyAdts_Baglanti_Load);
            this.giris_ekran.ResumeLayout(false);
            this.giris_ekran.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.durumSimge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kartBekleniyor)).EndInit();
            this.giris_panel.ResumeLayout(false);
            this.giris_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.klavuz_panel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort cihaz;
        private System.Windows.Forms.ComboBox portSec_comboBox;
        private System.Windows.Forms.Timer guncelle;
        private System.Windows.Forms.Label etiket1;
        public System.Windows.Forms.PictureBox klavuz_panel;
        private System.Windows.Forms.Panel giris_ekran;
        private System.Windows.Forms.Label girisDurum_etiket;
        private System.Windows.Forms.PictureBox kartBekleniyor;
        private System.Windows.Forms.Button giris_buton;
        private System.Windows.Forms.Label bilgi2;
        private System.Windows.Forms.Label bilgi1;
        private System.Windows.Forms.TextBox sifre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox isim;
        public System.Windows.Forms.PictureBox durumSimge;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel giris_panel;
        private System.Windows.Forms.Button cihazsizGiris_buton;
    }
}

