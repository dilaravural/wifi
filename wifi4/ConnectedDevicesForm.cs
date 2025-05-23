using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace wifi4
{
    public partial class ConnectedDevicesForm : Form
    {
        private Dictionary<string, string> customDeviceNames = new Dictionary<string, string>();
        private string customNamesFilePath = "custom_device_names.txt";
        private Color defaultButtonColor = Color.FromArgb(0, 122, 204);
        private Color hoverButtonColor = Color.FromArgb(0, 102, 184);
        private CancellationTokenSource? cancellationTokenSource;
        private List<DeviceInfo> discoveredDevices = new List<DeviceInfo>();
        private ImageList deviceIcons;

        private class DeviceInfo
        {
            public string Name { get; set; } = "Bilinmeyen Cihaz";
            public string IPAddress { get; set; } = "0.0.0.0";
            public long PingTime { get; set; }
            public List<int> OpenPorts { get; set; } = new List<int>();
            public Panel DevicePanel { get; set; } = null!;
            public int DeviceType { get; set; } = 0; // 0: Unknown, 1: Computer, 2: Router, 3: Printer, 4: Mobile
            public string MacAddress { get; set; } = "Bilinmiyor";
            public string HostName { get; set; } = "Bilinmiyor";
            public string Manufacturer { get; set; } = "Bilinmiyor";
        }

        public ConnectedDevicesForm()
        {
            InitializeComponent();
            InitializeDeviceIcons();
            LoadCustomDeviceNames();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeDeviceIcons()
        {
            deviceIcons = new ImageList();
            deviceIcons.ImageSize = new Size(32, 32);
            deviceIcons.ColorDepth = ColorDepth.Depth32Bit;
            
            // Sadece mevcut ikonları ekle
            deviceIcons.Images.Add("computer", Properties.Resources.computer);
            deviceIcons.Images.Add("router", Properties.Resources.router);
            deviceIcons.Images.Add("printer", Properties.Resources.printer);
            deviceIcons.Images.Add("mobile", Properties.Resources.mobile);
            deviceIcons.Images.Add("unknown_device", Properties.Resources.unknown_device);
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            if (btnScan.Text == "Durdur")
            {
                cancellationTokenSource?.Cancel();
                return;
            }

            btnScan.Text = "Durdur";
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;
            labelStatus.Text = "Cihazlar taranıyor...";
            flowDevices.Controls.Clear();
            discoveredDevices.Clear();
            labelCount.Text = "Bağlı Cihaz Sayısı: 0";
            panelDeviceInfo.Visible = false;

            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await ScanNetworkAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                labelStatus.Text = "Tarama durduruldu.";
            }
            finally
            {
                btnScan.Text = "Cihazları Tara";
                progressBar.Visible = false;
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        private async Task ScanNetworkAsync(CancellationToken cancellationToken)
        {
            string localIP = GetLocalIPAddress();
            string[] ipParts = localIP.Split('.');
            string baseIP = $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.";

            labelStatus.Text = "Ping taraması başlatılıyor...";

            // Paralel ping taraması - daha hızlı tarama için batch işleme
            var pingTasks = new List<Task<(string ip, bool isActive)>>();
            const int batchSize = 50; // Her seferde 50 IP adresi tara

            for (int i = 1; i <= 254 && !cancellationToken.IsCancellationRequested; i += batchSize)
                {
                var batchTasks = new List<Task<(string ip, bool isActive)>>();
                for (int j = 0; j < batchSize && i + j <= 254; j++)
                {
                    string ipAddress = baseIP + (i + j).ToString();
                    batchTasks.Add(PingHostAsync(ipAddress));
                }
                pingTasks.AddRange(batchTasks);
                await Task.WhenAll(batchTasks);
            }

            var results = await Task.WhenAll(pingTasks);
            var activeIPs = results.Where(r => r.isActive).Select(r => r.ip).ToList();

            if (cancellationToken.IsCancellationRequested) return;

            int totalDevices = activeIPs.Count;
            discoveredDevices.Clear();

            if (totalDevices == 0)
            {
                labelStatus.Text = "Aktif cihaz bulunamadı.";
                progressBar.Visible = false;
                return;
            }

            // Aktif cihazlar için detaylı tarama - daha hızlı tarama için batch işleme
            var scanTasks = new List<Task>();
            int completedScans = 0;
            const int scanBatchSize = 10; // Her seferde 10 cihaz tara

            foreach (var ipAddress in activeIPs)
            {
                if (cancellationToken.IsCancellationRequested) break;

                var scanTask = Task.Run(async () =>
                {
                    try
                    {
                        var deviceInfo = new DeviceInfo { IPAddress = ipAddress };

                        // Ping süresini ölç - timeout süresini 300ms'ye düşürdük
                        using (Ping ping = new Ping())
                        {
                            PingReply reply = await ping.SendPingAsync(ipAddress, 300);
                            deviceInfo.PingTime = reply.RoundtripTime;
                        }

                        // Cihaz adını al
                        try
                        {
                            IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddress);
                            deviceInfo.Name = hostEntry.HostName;
                        }
                        catch
                        {
                            deviceInfo.Name = "Bilinmeyen Cihaz";
                        }

                        // Sadece önemli portları tara (80, 443, 8080)
                        deviceInfo.OpenPorts = await ScanImportantPortsAsync(ipAddress, cancellationToken);

                        // Örnek: device.MacAddress = "00:11:22:33:44:55"; // Gerçekte arp -a çıktısından alınır.
                        // Örnek: device.HostName = "host1"; // Gerçekte hostname komutundan alınır.
                        // Örnek: device.Manufacturer = "Üretici A"; // Gerçekte (örneğin SNMP veya başka bir yöntemle) alınabilir.

                        lock (discoveredDevices)
                        {
                            discoveredDevices.Add(deviceInfo);
                            completedScans++;
                            this.Invoke((MethodInvoker)delegate
                            {
                                labelStatus.Text = $"Cihaz Taranıyor ({completedScans}/{totalDevices})";
                                UpdateDeviceList();
                                labelCount.Text = $"Bağlı Cihaz Sayısı: {discoveredDevices.Count}";
                            });
                        }
            }
                    catch { }
                });

                scanTasks.Add(scanTask);
            }

            await Task.WhenAll(scanTasks);

            if (!cancellationToken.IsCancellationRequested)
            {
                labelStatus.Text = $"Tarama tamamlandı. {discoveredDevices.Count} cihaz bulundu.";
                await Task.Delay(300); // Bekleme süresini 300ms'ye düşürdük
                progressBar.Visible = false;
            }
        }

        private async Task<(string ip, bool isActive)> PingHostAsync(string ipAddress)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(ipAddress, 500); // Timeout süresini 500ms'ye düşürdük
                    return (ipAddress, reply.Status == IPStatus.Success);
                }
            }
            catch
            {
                return (ipAddress, false);
            }
        }

        private async Task<List<int>> ScanImportantPortsAsync(string ipAddress, CancellationToken cancellationToken)
        {
            var openPorts = new List<int>();
            var importantPorts = new[] { 80, 443, 8080 }; // Sadece önemli portları tara
            var tasks = importantPorts.Select(port => Task.Run(async () =>
        {
            try
            {
                    using (var tcpClient = new TcpClient())
                {
                        var connectTask = tcpClient.ConnectAsync(ipAddress, port);
                        if (await Task.WhenAny(connectTask, Task.Delay(300, cancellationToken)) == connectTask)
                        {
                            await connectTask;
                            openPorts.Add(port);
                        }
                }
            }
                catch { }
            }));

            await Task.WhenAll(tasks);
            return openPorts;
        }

        private void UpdateDeviceList()
        {
            flowDevices.Controls.Clear();
            foreach (var device in discoveredDevices.OrderBy(d => d.Name))
            {
                var panel = CreateDevicePanel(device);
                device.DevicePanel = panel;
                flowDevices.Controls.Add(panel);
            }
        }

        private Panel CreateDevicePanel(DeviceInfo device)
        {
            var panel = new Panel
            {
                Width = flowDevices.Width - 40,
                Height = 70,
                Margin = new Padding(5),
                BackColor = Color.FromArgb(45, 45, 48),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Device type icon
            var deviceIcon = new PictureBox
            {
                Image = deviceIcons.Images[GetDeviceTypeIcon(device)],
                Size = new Size(32, 32),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            var nameLabel = new Label
            {
                Text = customDeviceNames.ContainsKey(device.IPAddress) 
                    ? customDeviceNames[device.IPAddress] 
                    : device.Name,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(50, 5),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            nameLabel.Click += (s, e) => ShowDeviceEditDialog(device, nameLabel);

            var ipLabel = new Label
            {
                Text = $"IP: {device.IPAddress}",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, 30),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { deviceIcon, nameLabel, ipLabel });
            panel.Click += (s, e) => ShowDeviceDetails(device);
            deviceIcon.Click += (s, e) => ShowDeviceDetails(device);
            ipLabel.Click += (s, e) => ShowDeviceDetails(device);
            panel.Cursor = Cursors.Hand;
            return panel;
        }

        private string GetDeviceTypeIcon(DeviceInfo device)
        {
            if (device.Name.ToLower().Contains("router") || device.Name.ToLower().Contains("gateway"))
                return "router";
            if (device.Name.ToLower().Contains("printer"))
                return "printer";
            if (device.Name.ToLower().Contains("android") || device.Name.ToLower().Contains("iphone"))
                return "mobile";
            if (device.Name.ToLower().Contains("pc") || device.Name.ToLower().Contains("computer"))
                return "computer";
            return "unknown_device";
        }

        private void ShowDeviceEditDialog(DeviceInfo device, Label nameLabel)
        {
            using (var form = new Form())
            {
                form.Text = "Cihaz Adını Düzenle";
                form.Size = new Size(300, 150);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = Color.FromArgb(37, 37, 38);

                var textBox = new TextBox
                {
                    Text = customDeviceNames.ContainsKey(device.IPAddress) 
                        ? customDeviceNames[device.IPAddress] 
                        : device.Name,
                    Location = new Point(20, 20),
                    Size = new Size(240, 25),
                    Font = new Font("Segoe UI", 10)
                };

                var saveButton = new Button
                {
                    Text = "Kaydet",
                    DialogResult = DialogResult.OK,
                    Location = new Point(100, 60),
                    Size = new Size(80, 30),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                form.Controls.AddRange(new Control[] { textBox, saveButton });
                form.AcceptButton = saveButton;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string newName = textBox.Text.Trim();
                    if (!string.IsNullOrEmpty(newName))
                    {
                        customDeviceNames[device.IPAddress] = newName;
                        nameLabel.Text = newName;
                        SaveCustomDeviceNames();
                    }
                }
            }
        }

        private void SaveCustomDeviceNames()
        {
            var lines = customDeviceNames.Select(kvp => $"{kvp.Key}|{kvp.Value}");
            System.IO.File.WriteAllLines(customNamesFilePath, lines);
        }

        private void ShowDeviceDetails(DeviceInfo device)
        {
            labelDeviceName.Text = $"Cihaz Adı (Kısa Ad): {(customDeviceNames.ContainsKey(device.IPAddress) ? customDeviceNames[device.IPAddress] : device.Name)}";
            labelIPAddress.Text = $"IP Adresi: {device.IPAddress}";
            labelPingStatus.Text = $"Ping: {device.PingTime}ms";
            labelOpenPorts.Text = $"Açık Portlar: {string.Join(", ", device.OpenPorts)}";
            pictureBoxDevice.Image = deviceIcons.Images[GetDeviceTypeIcon(device)];
            labelMac.Text = $"MAC Adresi: {device.MacAddress}";
            labelHost.Text = $"Host (Biliniyorsa): {device.HostName}";
            labelManufacturer.Text = $"Üretici (Biliniyorsa): {device.Manufacturer}";
            panelDeviceInfo.Visible = true;
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
        }
            }
            return "127.0.0.1";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide();
        }

        private void LoadCustomDeviceNames()
        {
            if (!System.IO.File.Exists(customNamesFilePath)) return;

            foreach (var line in System.IO.File.ReadAllLines(customNamesFilePath))
            {
                var parts = line.Split('|');
                if (parts.Length == 2)
                    customDeviceNames[parts[0]] = parts[1];
            }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = hoverButtonColor;
                button.Cursor = Cursors.Hand;
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = defaultButtonColor;
                button.Cursor = Cursors.Default;
            }
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {

        }
    }
}