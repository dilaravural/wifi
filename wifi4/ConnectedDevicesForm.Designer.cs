namespace wifi4
{
    partial class ConnectedDevicesForm
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
            btnScan = new Button();
            flowDevices = new FlowLayoutPanel();
            labelCount = new Label();
            btnBack = new Button();
            panel1 = new Panel();
            labelTitle = new Label();
            progressBar = new ProgressBar();
            labelStatus = new Label();
            panelDeviceInfo = new Panel();
            pictureBoxDevice = new PictureBox();
            labelOpenPorts = new Label();
            labelPingStatus = new Label();
            labelIPAddress = new Label();
            labelDeviceName = new Label();
            labelMac = new Label();
            labelHost = new Label();
            labelManufacturer = new Label();
            panel1.SuspendLayout();
            panelDeviceInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDevice).BeginInit();
            SuspendLayout();
            // 
            // btnScan
            // 
            btnScan.BackColor = Color.FromArgb(0, 122, 204);
            btnScan.FlatAppearance.BorderSize = 0;
            btnScan.FlatStyle = FlatStyle.Flat;
            btnScan.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnScan.ForeColor = Color.White;
            btnScan.Location = new Point(47, 618);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(158, 47);
            btnScan.TabIndex = 0;
            btnScan.Text = "Cihazları Tara";
            btnScan.UseVisualStyleBackColor = false;
            btnScan.Click += btnScan_Click;
            btnScan.MouseEnter += Button_MouseEnter;
            btnScan.MouseLeave += Button_MouseLeave;
            // 
            // flowDevices
            // 
            flowDevices.AutoScroll = true;
            flowDevices.BackColor = Color.FromArgb(45, 45, 48);
            flowDevices.Location = new Point(47, 87);
            flowDevices.Name = "flowDevices";
            flowDevices.Padding = new Padding(10);
            flowDevices.Size = new Size(427, 336);
            flowDevices.TabIndex = 1;
            // 
            // labelCount
            // 
            labelCount.AutoSize = true;
            labelCount.Font = new Font("Segoe UI", 10F);
            labelCount.ForeColor = Color.White;
            labelCount.Location = new Point(47, 51);
            labelCount.Name = "labelCount";
            labelCount.Size = new Size(158, 23);
            labelCount.TabIndex = 2;
            labelCount.Text = "Bağlı Cihaz Sayısı: 0";
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.FromArgb(0, 122, 204);
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(313, 618);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(161, 47);
            btnBack.TabIndex = 3;
            btnBack.Text = "Ana Menüye Dön";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += btnBack_Click;
            btnBack.MouseEnter += Button_MouseEnter;
            btnBack.MouseLeave += Button_MouseLeave;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(45, 45, 48);
            panel1.Controls.Add(labelTitle);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(527, 38);
            panel1.TabIndex = 6;
            // 
            // labelTitle
            // 
            labelTitle.Dock = DockStyle.Fill;
            labelTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            labelTitle.ForeColor = Color.White;
            labelTitle.Location = new Point(0, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(527, 38);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Bağlı Cihazlar";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            labelTitle.Click += labelTitle_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(47, 442);
            progressBar.MarqueeAnimationSpeed = 30;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(427, 19);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 10;
            progressBar.Visible = false;
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Font = new Font("Segoe UI", 9F);
            labelStatus.ForeColor = Color.White;
            labelStatus.Location = new Point(47, 426);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(0, 20);
            labelStatus.TabIndex = 8;
            // 
            // panelDeviceInfo
            // 
            panelDeviceInfo.BackColor = Color.FromArgb(45, 45, 48);
            panelDeviceInfo.BorderStyle = BorderStyle.FixedSingle;
            panelDeviceInfo.Controls.Add(pictureBoxDevice);
            panelDeviceInfo.Controls.Add(labelOpenPorts);
            panelDeviceInfo.Controls.Add(labelPingStatus);
            panelDeviceInfo.Controls.Add(labelIPAddress);
            panelDeviceInfo.Controls.Add(labelDeviceName);
            panelDeviceInfo.Controls.Add(labelMac);
            panelDeviceInfo.Controls.Add(labelHost);
            panelDeviceInfo.Controls.Add(labelManufacturer);
            panelDeviceInfo.Location = new Point(47, 477);
            panelDeviceInfo.Name = "panelDeviceInfo";
            panelDeviceInfo.Size = new Size(427, 121);
            panelDeviceInfo.TabIndex = 9;
            panelDeviceInfo.Visible = false;
            // 
            // pictureBoxDevice
            // 
            pictureBoxDevice.Location = new Point(10, 10);
            pictureBoxDevice.Name = "pictureBoxDevice";
            pictureBoxDevice.Size = new Size(40, 40);
            pictureBoxDevice.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxDevice.TabIndex = 4;
            pictureBoxDevice.TabStop = false;
            // 
            // labelOpenPorts
            // 
            labelOpenPorts.AutoSize = true;
            labelOpenPorts.Font = new Font("Segoe UI", 9F);
            labelOpenPorts.ForeColor = Color.White;
            labelOpenPorts.Location = new Point(300, 33);
            labelOpenPorts.Name = "labelOpenPorts";
            labelOpenPorts.Size = new Size(87, 20);
            labelOpenPorts.TabIndex = 3;
            labelOpenPorts.Text = "Açık Portlar:";
            // 
            // labelPingStatus
            // 
            labelPingStatus.AutoSize = true;
            labelPingStatus.Font = new Font("Segoe UI", 9F);
            labelPingStatus.ForeColor = Color.Lime;
            labelPingStatus.Location = new Point(300, 10);
            labelPingStatus.Name = "labelPingStatus";
            labelPingStatus.Size = new Size(72, 20);
            labelPingStatus.TabIndex = 2;
            labelPingStatus.Text = "Ping: 5ms";
            // 
            // labelIPAddress
            // 
            labelIPAddress.AutoSize = true;
            labelIPAddress.Font = new Font("Segoe UI", 9F);
            labelIPAddress.ForeColor = Color.White;
            labelIPAddress.Location = new Point(60, 33);
            labelIPAddress.Name = "labelIPAddress";
            labelIPAddress.Size = new Size(70, 20);
            labelIPAddress.TabIndex = 1;
            labelIPAddress.Text = "IP Adresi:";
            // 
            // labelDeviceName
            // 
            labelDeviceName.AutoSize = true;
            labelDeviceName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelDeviceName.ForeColor = Color.White;
            labelDeviceName.Location = new Point(60, 10);
            labelDeviceName.Name = "labelDeviceName";
            labelDeviceName.Size = new Size(91, 23);
            labelDeviceName.TabIndex = 0;
            labelDeviceName.Text = "Cihaz Adı:";
            // 
            // labelMac
            // 
            labelMac.AutoSize = true;
            labelMac.Font = new Font("Segoe UI", 9F);
            labelMac.ForeColor = Color.White;
            labelMac.Location = new Point(60, 50);
            labelMac.Name = "labelMac";
            labelMac.Size = new Size(90, 20);
            labelMac.TabIndex = 5;
            labelMac.Text = "MAC Adresi:";
            // 
            // labelHost
            // 
            labelHost.AutoSize = true;
            labelHost.Font = new Font("Segoe UI", 9F);
            labelHost.ForeColor = Color.White;
            labelHost.Location = new Point(60, 70);
            labelHost.Name = "labelHost";
            labelHost.Size = new Size(125, 20);
            labelHost.TabIndex = 6;
            labelHost.Text = "Host (Biliniyorsa):";
            // 
            // labelManufacturer
            // 
            labelManufacturer.AutoSize = true;
            labelManufacturer.Font = new Font("Segoe UI", 9F);
            labelManufacturer.ForeColor = Color.White;
            labelManufacturer.Location = new Point(60, 90);
            labelManufacturer.Name = "labelManufacturer";
            labelManufacturer.Size = new Size(137, 20);
            labelManufacturer.TabIndex = 7;
            labelManufacturer.Text = "Üretici (Biliniyorsa):";
            // 
            // ConnectedDevicesForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(37, 37, 38);
            ClientSize = new Size(527, 698);
            Controls.Add(panelDeviceInfo);
            Controls.Add(labelStatus);
            Controls.Add(progressBar);
            Controls.Add(panel1);
            Controls.Add(btnScan);
            Controls.Add(labelCount);
            Controls.Add(btnBack);
            Controls.Add(flowDevices);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ConnectedDevicesForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bağlı Cihazlar";
            panel1.ResumeLayout(false);
            panelDeviceInfo.ResumeLayout(false);
            panelDeviceInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxDevice).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnScan;
        private FlowLayoutPanel flowDevices;
        private Label labelCount;
        private Button btnBack;
        private Panel panel1;
        private Label labelTitle;
        private ProgressBar progressBar;
        private Label labelStatus;
        private Panel panelDeviceInfo;
        private Label labelDeviceName;
        private Label labelIPAddress;
        private Label labelPingStatus;
        private Label labelOpenPorts;
        private PictureBox pictureBoxDevice;
        private Label labelMac;
        private Label labelHost;
        private Label labelManufacturer;
    }
}