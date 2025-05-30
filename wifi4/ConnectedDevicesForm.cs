using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace wifi4
{
    public partial class ConnectedDevicesForm : Form
    {
        private Dictionary<string, string> customDeviceNames = new Dictionary<string, string>();
        private string customNamesFilePath;
        private int deviceCount = 0;
        private CheckBox chkLocalOnly;
        private List<(string ip, string mac, string hostname, string vendor, string connectionType, string deviceType)> allDevices = new();
        private string mac = "";
        private SemaphoreSlim pingSemaphore = new SemaphoreSlim(20, 20); // Aynı anda max 20 ping

        public ConnectedDevicesForm()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(appData, "wifi4");
            Directory.CreateDirectory(dir);
            customNamesFilePath = Path.Combine(dir, "custom_device_names.txt");
            
            InitializeComponent();
            LoadCustomDeviceNames();
            LoadMacVendorCache();
            SetHoverBorder(btnScan);
            SetHoverBorder(btnBack);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Size = new Size(355, 618);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Başlık ve sayaç ortalanmış
            labelCount.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelCount.ForeColor = Color.FromArgb(0, 122, 204);
            labelCount.TextAlign = ContentAlignment.MiddleCenter;

            // Cihaz kartları paneli ortalanmış ve genişliği optimize edilmiş
            flowDevices.Location = new Point(12, 65);
            flowDevices.Size = new Size(310, 380);
            flowDevices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            flowDevices.AutoScroll = true;

            lblDevicesScan.Location = new Point(15, 470);
            lblHomePage.Location = new Point(235, 470);

            // Alt kısımda iki büyük buton: Cihazları Tara ve Ana Sayfa
            btnScan.Size = new Size(80, 60);
            btnScan.Location = new Point(12, 490);
            btnScan.Font = new Font("Segoe UI Symbol", 32F, FontStyle.Bold);
            btnScan.Text = "";
            btnScan.Image = null;
            btnScan.ImageAlign = ContentAlignment.MiddleCenter;
            btnScan.FlatStyle = FlatStyle.Flat;
            btnScan.FlatAppearance.BorderSize = 0;
            btnScan.FlatAppearance.BorderColor = this.BackColor;
            btnScan.TabStop = false;
            btnScan.UseVisualStyleBackColor = false;
            btnScan.BackColor = Color.FromArgb(240, 240, 240);
            btnScan.Cursor = Cursors.Hand;

            btnBack.Size = new Size(80, 60);
            btnBack.Location = new Point(230, 490);
            btnBack.Font = new Font("Segoe UI Symbol", 32F, FontStyle.Bold);
            btnBack.Text = "";
            btnBack.Image = null;
            btnBack.ImageAlign = ContentAlignment.MiddleCenter;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatAppearance.BorderColor = this.BackColor;
            btnBack.TabStop = false;
            btnBack.UseVisualStyleBackColor = false;
            btnBack.BackColor = Color.FromArgb(240, 240, 240);
            btnBack.Cursor = Cursors.Hand;

            chkLocalOnly = new CheckBox();
            chkLocalOnly.Text = "Sadece Yerel Ağ";
            chkLocalOnly.Location = new Point(12, 40);
            chkLocalOnly.AutoSize = true;
            chkLocalOnly.Font = new Font("Segoe UI", 10);
            chkLocalOnly.Checked = false;
            chkLocalOnly.CheckedChanged += (s, e) => ShowFilteredDevices();
            this.Controls.Add(chkLocalOnly);

            try
            {
                // Loading spinner ayarları
                loadingSpinner.Image = Image.FromFile(Path.Combine(Application.StartupPath, "Resources", "g2.gif"));
                loadingSpinner.SizeMode = PictureBoxSizeMode.Zoom;
                loadingSpinner.Visible = true;
                loadingSpinner.Size = new Size(100, 100);

                // Count spinner ayarları
                countSpinner.Image = Image.FromFile(Path.Combine(Application.StartupPath, "Resources", "g2.gif"));
                countSpinner.SizeMode = PictureBoxSizeMode.Zoom;
                countSpinner.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loading gif yüklenirken hata oluştu: " + ex.Message);
            }

            // Başlangıçta spinner'ları gizle
            HideLoadingSpinner();
            HideCountSpinner();
        }

        private void ShowLoadingSpinner()
        {
            loadingSpinner.Visible = true;
            loadingSpinner.Location = new Point((this.Width - loadingSpinner.Width) / 2, (this.Height - loadingSpinner.Height) / 2);
            loadingSpinner.BringToFront();
        }

        private void HideLoadingSpinner()
        {
            loadingSpinner.Visible = false;
        }

        private void ShowCountSpinner()
        {
            countSpinner.Visible = true;
            countSpinner.Location = new Point(labelCount.Right + 10, labelCount.Top + 2);
            countSpinner.BringToFront();
        }

        private void HideCountSpinner()
        {
            countSpinner.Visible = false;
        }
        private void SetHoverBorder(Button button)
        {
            button.MouseEnter += (s, e) =>
            {
                button.FlatAppearance.BorderSize = 2;
                button.FlatAppearance.BorderColor = Color.SteelBlue; // İstediğin rengi kullanabilirsin
            };

            button.MouseLeave += (s, e) =>
            {
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.BorderColor = this.BackColor;
            };
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            flowDevices.Controls.Clear();
            ShowLoadingSpinner();
            ShowCountSpinner();
            labelCount.Text = "Toplam Cihaz Sayısı: ";
            deviceCount = 0;

            btnScan.Enabled = false;
            btnBack.Enabled = false;

            try
            {
                // 1. Önce ARP tablosunu oku - bu çok hızlı
                var arpDevices = await GetArpTableAsync();
                var seenMacs = new HashSet<string>();

                // 2. ARP tablosundan gelen cihazları işle
                allDevices.Clear();
                var arpTasks = new List<Task>();

                foreach (var (ip, mac) in arpDevices)
                {
                    if (string.IsNullOrWhiteSpace(mac) || seenMacs.Contains(mac))
                        continue;
                    seenMacs.Add(mac);

                    // Her cihaz için ayrı task oluştur - paralel işlem
                    arpTasks.Add(ProcessDeviceAsync(ip, mac));
                }

                // 3. ARP cihazlarını paralel işle
                await Task.WhenAll(arpTasks);

                // 4. Her zaman ping taraması yap
                await PerformAdditionalPingScan(seenMacs);

                labelCount.Text = $"Toplam cihaz sayısı: {deviceCount}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (deviceCount == 0)
                    HideLoadingSpinner();
                HideCountSpinner();
                btnScan.Enabled = true;
                btnBack.Enabled = true;
            }
        }

        // ARP tablosunu asenkron olarak oku
        private async Task<List<(string ip, string mac)>> GetArpTableAsync()
        {
            return await Task.Run(() =>
            {
                var arpDevices = new List<(string ip, string mac)>();
                string arpOutput = RunCommand("arp", "-a");
                string[] arpLines = arpOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (string line in arpLines)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(line, @"(?<ip>\d+\.\d+\.\d+\.\d+)\s+([\-\w]+)?\s+(?<mac>([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2})");
                    if (match.Success)
                    {
                        string ip = match.Groups["ip"].Value;
                        string mac = match.Groups["mac"].Value.ToUpper().Replace(":", "-");
                        arpDevices.Add((ip, mac));
                    }
                }
                
                return arpDevices;
            });
        }

        // Her cihazı ayrı ayrı işle
        private async Task ProcessDeviceAsync(string ip, string mac)
        {
            try
            {
                string vendor = "Bilinmeyen Üretici";
                
                // Önce cache'den kontrol et
                if (macVendorCache.ContainsKey(mac))
                {
                    vendor = macVendorCache[mac];
                }
                else
                {
                    // Vendor bilgisini arka planda al - UI'ı bloklamayacak şekilde
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var vendorInfo = await GetVendorFromMacAsync(mac);
                            if (!string.IsNullOrEmpty(vendorInfo) && vendorInfo != "Bilinmeyen Üretici")
                            {
                                // UI'da vendor bilgisini güncelle
                                this.Invoke((MethodInvoker)delegate
                                {
                                    UpdateDeviceVendor(mac, vendorInfo);
                                });
                            }
                        }
                        catch { }
                    });
                }

                string connectionType = GetConnectionType(ip);
                string deviceType = GetDeviceType(vendor);

                var deviceTuple = (ip, mac, "Çözümlenemedi", vendor, connectionType, deviceType);
                
                lock (allDevices)
                {
                    allDevices.Add(deviceTuple);
                }

                // Filtreleme kontrolü
                if (!chkLocalOnly.Checked || (connectionType != null && connectionType.Trim().ToLower() == "yerel ağ"))
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        AddDeviceCard(ip, mac, "Çözümlenemedi", vendor, connectionType, deviceType);
                        deviceCount++;
                        if (deviceCount == 1)
                            HideLoadingSpinner();
                        countSpinner.Location = new Point(labelCount.Right + 10, labelCount.Top + 2);
                        labelCount.Text = $"Toplam cihaz sayısı: {deviceCount}";
                    });
                }
            }
            catch { }
        }

        // Ek ping taraması - sadece gerekirse
        private async Task PerformAdditionalPingScan(HashSet<string> seenMacs)
        {
            string localIp = GetLocalIPAddress();
            string baseIp = string.Join(".", localIp.Split('.').Take(3)) + ".";
            
            var pingTasks = new List<Task>();

            for (int i = 1; i <= 254; i++)
            {
                string ip = baseIp + i;
                pingTasks.Add(PingAndProcessAsync(ip, seenMacs));
            }

            await Task.WhenAll(pingTasks);
        }

        private async Task PingAndProcessAsync(string ip, HashSet<string> seenMacs)
        {
            await pingSemaphore.WaitAsync();
            try
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    var reply = await ping.SendPingAsync(ip, 500); // 500ms timeout
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        // ARP tablosunu tekrar kontrol et
                        await Task.Delay(100); // ARP tablosunun güncellenmesi için kısa bekleme
                        
                        string arpOutput = RunCommand("arp", $"-a {ip}");
                        var match = System.Text.RegularExpressions.Regex.Match(arpOutput, @"([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2}");
                        if (match.Success)
                        {
                            string mac = match.Value.ToUpper().Replace(":", "-");
                            lock (seenMacs)
                            {
                                if (!seenMacs.Contains(mac))
                                {
                                    seenMacs.Add(mac);
                                    _ = ProcessDeviceAsync(ip, mac);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            finally
            {
                pingSemaphore.Release();
            }
        }

        // Vendor bilgisini güncellemek için yardımcı method
        private void UpdateDeviceVendor(string mac, string vendor)
        {
            foreach (Panel panel in flowDevices.Controls.OfType<Panel>())
            {
                var macLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains(mac));
                if (macLabel != null)
                {
                    var vendorLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Text.StartsWith("🏭 Üretici:"));
                    if (vendorLabel != null)
                    {
                        vendorLabel.Text = $"🏭 Üretici: {vendor}";
                    }
                    
                    // Device type'ı da güncelle
                    string newDeviceType = GetDeviceType(vendor);
                    var typeLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("Cihazı") || l.Text.Contains("Diğer"));
                    if (typeLabel != null)
                    {
                        typeLabel.Text = newDeviceType;
                    }
                    
                    var iconLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Name == "Segoe UI Symbol");
                    if (iconLabel != null)
                    {
                        iconLabel.Text = GetDeviceIcon(newDeviceType);
                    }
                    break;
                }
            }
        }

        private async Task<string> GetVendorFromMacAsync(string mac)
        {
            mac = mac.Replace(":", "").Replace("-", "").ToUpper();

            if (macVendorCache.ContainsKey(mac))
                return macVendorCache[mac];

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(3); // 3 saniye timeout
                    client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");
                    
                    // Mevcut API'nizi kullanın ama timeout ekleyin
                    string url = $"https://macvendors.co/api/{mac}/json";
                    
                    var response = await client.GetStringAsync(url);
                    
                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        // Cache'e kaydet
                        lock (macVendorCache)
                        {
                            macVendorCache[mac] = response;
                        }
                        
                        // Dosyaya kaydet (arka planda)
                        _ = Task.Run(() =>
                        {
                            try
                            {
                                File.AppendAllLines(macVendorCacheFile, new[] { $"{mac}|{response}" });
                            }
                            catch { }
                        });
                        
                        return response;
                    }
                }
            }
            catch
            {
                return "Bilinmeyen Üretici";
            }

            return "Bilinmeyen Üretici";
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "192.168.1.1";
        }

        private string GetConnectionType(string ip)
        {
            string[] parts = ip.Split('.');
            if (parts.Length == 4)
            {
                int firstOctet = int.Parse(parts[0]);
                if (firstOctet == 192 && parts[1] == "168")
                    return "Yerel Ağ";
                else if (firstOctet == 10)
                    return "Yerel Ağ";
                else if (firstOctet == 172 && int.Parse(parts[1]) >= 16 && int.Parse(parts[1]) <= 31)
                    return "Yerel Ağ";
            }
            return "Harici Ağ";
        }

        private string GetDeviceType(string vendor)
        {
            vendor = vendor.ToLower();
            if (vendor.Contains("apple") || vendor.Contains("iphone") || vendor.Contains("ipad"))
                return "Apple Cihazı";
            else if (vendor.Contains("samsung") || vendor.Contains("android"))
                return "Android Cihazı";
            else if (vendor.Contains("microsoft") || vendor.Contains("windows"))
                return "Windows Cihazı";
            else if (vendor.Contains("router") || vendor.Contains("gateway"))
                return "Ağ Cihazı";
            else
                return "Diğer Cihaz";
        }

        private string RunCommand(string command, string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(command, args)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    return process.StandardOutput.ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
        }

        private Dictionary<string, string> macVendorCache = new Dictionary<string, string>();
        private string macVendorCacheFile = "mac_vendors.txt";

        private void LoadMacVendorCache()
        {
            if (File.Exists(macVendorCacheFile))
            {
                foreach (var line in File.ReadAllLines(macVendorCacheFile))
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                        macVendorCache[parts[0]] = parts[1];
                }
            }
        }

        private string GetWiFiInterface(string ip)
        {
            try
            {
                string output = RunCommand("netsh", "wlan show interfaces");
                string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    if (line.Contains("Name") && !line.Contains("Description"))
                    {
                        return line.Split(':')[1].Trim();
                    }
                }
            }
            catch { }
            return "Bilinmiyor";
        }

        private void AddDeviceCard(string ip, string mac, string hostname, string vendor, string connectionType, string deviceType)
        {
            var panel = new Panel
            {
                Width = 260,
                Height = 180,
                Margin = new Padding(10),
                Padding = new Padding(15),
                BackColor = Color.White,
                Tag = false
            };

            // Hover efekti için event handler'lar (sadece renk ve border)
            panel.MouseEnter += (s, e) =>
            {
                panel.BackColor = Color.FromArgb(245, 245, 245);
                panel.BorderStyle = BorderStyle.FixedSingle;
            };

            panel.MouseLeave += (s, e) =>
            {
                panel.BackColor = Color.White;
                panel.BorderStyle = BorderStyle.None;
            };

            // MAC adresini normalize et
            string normMac = mac.Replace(":", "-").ToUpper();
            string displayName = customDeviceNames.ContainsKey(normMac)
                ? customDeviceNames[normMac]
                : "Bilinmeyen Cihaz";

            var iconLabel = new Label
            {
                Text = GetDeviceIcon(deviceType),
                Font = new Font("Segoe UI Symbol", 32),
                Location = new Point(10, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var nameLabel = new Label
            {
                Text = displayName,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(iconLabel.Right + 5, 15),
                Size = new Size(panel.Width - (iconLabel.Right + 20), 25),
                AutoSize = false,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var typeLabel = new Label
            {
                Text = deviceType,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(iconLabel.Right + 5, 40),
                Size = new Size(panel.Width - (iconLabel.Right + 20), 20),
                AutoSize = false,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var connectionLabel = new Label
            {
                Text = $"🌐 {connectionType}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 80),
                Size = new Size(panel.Width - 30, 20),
                AutoSize = false,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var ipLabel = new Label
            {
                Text = $"📡 IP: {ip}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 105),
                Size = new Size(panel.Width - 30, 20),
                AutoSize = false,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var macLabel = new Label
            {
                Text = $"🔑 MAC: {mac}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 130),
                Size = new Size(panel.Width - 30, 20),
                AutoSize = false,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var vendorLabel = new Label
            {
                Text = $"🏭 Üretici: {vendor}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 155),
                Size = new Size(panel.Width - 30, 20),
                AutoSize = false,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            nameLabel.Cursor = Cursors.Default;

            panel.Controls.AddRange(new Control[] {
                iconLabel, nameLabel, typeLabel, connectionLabel,
                ipLabel, macLabel, vendorLabel
            });

            panel.DoubleClick += (sender, e) =>
            {
                var deviceInfoForm = new DeviceInfoForm(
                    displayName, ip, mac, vendor, hostname, connectionType, deviceType
                );
                deviceInfoForm.ShowDialog();
            };

            flowDevices.Controls.Add(panel);
        }


        private string GetDeviceIcon(string deviceType)
        {
            switch (deviceType)
            {
                case "Apple Cihazı":
                    return "🍎";
                case "Android Cihazı":
                    return "📱";
                case "Windows Cihazı":
                    return "💻";
                case "Ağ Cihazı":
                    return "🌐";
                default:
                    return "❓";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is MainMenuForm)
                {
                    form.Show();
                    break;
                }
            }
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form is MainMenuForm)
                    {
                        form.Show();
                        break;
                    }
                }
            }
            base.OnFormClosing(e);
        }

        // Form yeniden boyutlandırıldığında spinner'ları ortala
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (loadingSpinner != null && loadingSpinner.Visible)
            {
                loadingSpinner.Location = new Point((this.Width - loadingSpinner.Width) / 2, (this.Height - loadingSpinner.Height) / 2);
            }
            if (countSpinner != null && countSpinner.Visible)
            {
                countSpinner.Location = new Point(labelCount.Right + 8);
            }
        }

        public void ShowFilteredDevices()
        {
            flowDevices.Controls.Clear();
            deviceCount = 0;
            var filteredDevices = chkLocalOnly.Checked
                ? allDevices.Where(d => d.connectionType != null && d.connectionType.Trim().ToLower() == "yerel ağ").ToList()
                : allDevices;
            foreach (var device in filteredDevices)
            {
                AddDeviceCard(device.ip, device.mac, device.hostname, device.vendor, device.connectionType, device.deviceType);
                deviceCount++;
            }
            labelCount.Text = $"Toplam cihaz sayısı: {deviceCount}";
        }

        public void LoadCustomDeviceNames()
        {
            customDeviceNames.Clear();
            if (File.Exists(customNamesFilePath))
            {
                foreach (var line in File.ReadAllLines(customNamesFilePath))
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                        customDeviceNames[parts[0]] = parts[1];
                }
            }
        }

        public void SaveCustomDeviceNames()
        {
            var lines = customDeviceNames.Select(kvp => $"{kvp.Key}|{kvp.Value}");
            File.WriteAllLines(customNamesFilePath, lines);
        }

        public void ShowDeviceEditDialog(string mac)
        {
            string normMac = mac.Replace(":", "-").ToUpper();
            string currentName = customDeviceNames.ContainsKey(normMac) ? customDeviceNames[normMac] : "";
            Form dialog = new Form { Width = 300, Height = 120, Text = "Cihaz İsmini Değiştir" };
            TextBox txt = new TextBox { Text = currentName, Left = 10, Top = 10, Width = 260 };
            Button btn = new Button { Text = "Kaydet", Left = 100, Width = 80, Top = 40, DialogResult = DialogResult.OK };
            dialog.Controls.Add(txt);
            dialog.Controls.Add(btn);
            dialog.AcceptButton = btn;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string newName = txt.Text.Trim();
                if (!string.IsNullOrEmpty(newName))
                {
                    customDeviceNames[normMac] = newName;
                    SaveCustomDeviceNames();
                    LoadCustomDeviceNames();
                    ShowFilteredDevices();
                }
            }
        }
    }
}