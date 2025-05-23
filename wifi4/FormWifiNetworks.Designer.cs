namespace wifi4
{
    partial class FormWifiNetworks
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnWifiScan = new Button();
            btnback = new Button();
            labelNetworkCount = new Label();
            listViewWifiNetworks = new ListView();
            ssıd = new ColumnHeader();
            signal = new ColumnHeader();
            security = new ColumnHeader();
            panel1 = new Panel();
            labelTitle = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnWifiScan
            // 
            btnWifiScan.BackColor = Color.FromArgb(0, 122, 204);
            btnWifiScan.FlatAppearance.BorderSize = 0;
            btnWifiScan.FlatStyle = FlatStyle.Flat;
            btnWifiScan.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnWifiScan.ForeColor = Color.White;
            btnWifiScan.Location = new Point(22, 416);
            btnWifiScan.Name = "btnWifiScan";
            btnWifiScan.Size = new Size(200, 40);
            btnWifiScan.TabIndex = 0;
            btnWifiScan.Text = "WiFi Ağlarını Tara";
            btnWifiScan.UseVisualStyleBackColor = false;
            btnWifiScan.Click += btnScanWifi_Click;
            btnWifiScan.MouseEnter += Button_MouseEnter;
            btnWifiScan.MouseLeave += Button_MouseLeave;
            // 
            // btnback
            // 
            btnback.BackColor = Color.FromArgb(0, 122, 204);
            btnback.FlatAppearance.BorderSize = 0;
            btnback.FlatStyle = FlatStyle.Flat;
            btnback.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnback.ForeColor = Color.White;
            btnback.Location = new Point(274, 416);
            btnback.Name = "btnback";
            btnback.Size = new Size(200, 40);
            btnback.TabIndex = 1;
            btnback.Text = "Ana Menüye Dön";
            btnback.UseVisualStyleBackColor = false;
            btnback.Click += btnBack_Click;
            btnback.MouseEnter += Button_MouseEnter;
            btnback.MouseLeave += Button_MouseLeave;
            // 
            // labelNetworkCount
            // 
            labelNetworkCount.AutoSize = true;
            labelNetworkCount.Font = new Font("Segoe UI", 10F);
            labelNetworkCount.ForeColor = Color.White;
            labelNetworkCount.Location = new Point(22, 87);
            labelNetworkCount.Name = "labelNetworkCount";
            labelNetworkCount.Size = new Size(117, 23);
            labelNetworkCount.TabIndex = 3;
            labelNetworkCount.Text = "Bulunan Ağ: 0";
            // 
            // listViewWifiNetworks
            // 
            listViewWifiNetworks.BackColor = Color.FromArgb(45, 45, 48);
            listViewWifiNetworks.BorderStyle = BorderStyle.None;
            listViewWifiNetworks.Columns.AddRange(new ColumnHeader[] { ssıd, signal, security });
            listViewWifiNetworks.Font = new Font("Segoe UI", 10F);
            listViewWifiNetworks.ForeColor = Color.White;
            listViewWifiNetworks.FullRowSelect = true;
            listViewWifiNetworks.GridLines = true;
            listViewWifiNetworks.Location = new Point(22, 119);
            listViewWifiNetworks.Name = "listViewWifiNetworks";
            listViewWifiNetworks.Size = new Size(452, 264);
            listViewWifiNetworks.TabIndex = 4;
            listViewWifiNetworks.UseCompatibleStateImageBehavior = false;
            listViewWifiNetworks.View = View.Details;
            // 
            // ssıd
            // 
            ssıd.Text = "Ağ Adı (SSID)";
            ssıd.Width = 150;
            // 
            // signal
            // 
            signal.Text = "Sinyal Gücü";
            signal.Width = 150;
            // 
            // security
            // 
            security.Text = "Güvenlik";
            security.Width = 150;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(45, 45, 48);
            panel1.Controls.Add(labelTitle);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(500, 60);
            panel1.TabIndex = 7;
            // 
            // labelTitle
            // 
            labelTitle.Dock = DockStyle.Fill;
            labelTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            labelTitle.ForeColor = Color.White;
            labelTitle.Location = new Point(0, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(500, 60);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "WiFi Ağları";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormWifiNetworks
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(37, 37, 38);
            ClientSize = new Size(500, 495);
            Controls.Add(panel1);
            Controls.Add(listViewWifiNetworks);
            Controls.Add(labelNetworkCount);
            Controls.Add(btnback);
            Controls.Add(btnWifiScan);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormWifiNetworks";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WiFi Ağları";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnWifiScan;
        private Button btnback;
        private Label labelNetworkCount;
        private ListView listViewWifiNetworks;
        private ColumnHeader ssıd;
        private ColumnHeader signal;
        private ColumnHeader security;
        private Panel panel1;
        private Label labelTitle;
    }
}