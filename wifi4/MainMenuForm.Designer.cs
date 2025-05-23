namespace wifi4
{
    partial class MainMenuForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenuForm));
            btnWifiScan = new Button();
            btnConnectedDevices = new Button();
            labelWifi = new Label();
            labelDevices = new Label();
            labelTitle = new Label();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnWifiScan
            // 
            btnWifiScan.BackColor = Color.FromArgb(0, 122, 204);
            btnWifiScan.FlatAppearance.BorderSize = 0;
            btnWifiScan.FlatStyle = FlatStyle.Flat;
            btnWifiScan.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnWifiScan.ForeColor = Color.White;
            btnWifiScan.Location = new Point(50, 150);
            btnWifiScan.Name = "btnWifiScan";
            btnWifiScan.Size = new Size(400, 80);
            btnWifiScan.TabIndex = 0;
            btnWifiScan.Text = "WiFi Ağlarını Tara";
            btnWifiScan.UseVisualStyleBackColor = false;
            btnWifiScan.Click += btnWifiScan_Click;
            btnWifiScan.MouseEnter += Button_MouseEnter;
            btnWifiScan.MouseLeave += Button_MouseLeave;
            // 
            // btnConnectedDevices
            // 
            btnConnectedDevices.BackColor = Color.FromArgb(0, 122, 204);
            btnConnectedDevices.FlatAppearance.BorderSize = 0;
            btnConnectedDevices.FlatStyle = FlatStyle.Flat;
            btnConnectedDevices.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnConnectedDevices.ForeColor = Color.White;
            btnConnectedDevices.Location = new Point(50, 280);
            btnConnectedDevices.Name = "btnConnectedDevices";
            btnConnectedDevices.Size = new Size(400, 80);
            btnConnectedDevices.TabIndex = 1;
            btnConnectedDevices.Text = "Bağlı Cihazları Görüntüle";
            btnConnectedDevices.UseVisualStyleBackColor = false;
            btnConnectedDevices.Click += btnConnectedDevices_Click;
            btnConnectedDevices.MouseEnter += Button_MouseEnter;
            btnConnectedDevices.MouseLeave += Button_MouseLeave;
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = false;
            labelTitle.Dock = DockStyle.Top;
            labelTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            labelTitle.ForeColor = Color.White;
            labelTitle.Location = new Point(0, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(500, 60);
            labelTitle.TabIndex = 4;
            labelTitle.Text = "WiFi Ağ Tarayıcı";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(45, 45, 48);
            panel1.Controls.Add(labelTitle);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(500, 60);
            panel1.TabIndex = 5;
            // 
            // MainMenuForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(37, 37, 38);
            ClientSize = new Size(500, 400);
            Controls.Add(panel1);
            Controls.Add(btnConnectedDevices);
            Controls.Add(btnWifiScan);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainMenuForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WiFi Ağ Tarayıcı";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnWifiScan;
        private Button btnConnectedDevices;
        private Label labelWifi;
        private Label labelDevices;
        private Label labelTitle;
        private Panel panel1;
    }
}