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
            string customName = !string.IsNullOrEmpty(mac) && customDeviceNames.TryGetValue(mac, out var foundName)
                ? foundName
                : "Bilinmeyen";

            try
            {
                

                // 1. Ağ aralığını bul
                string localIp = GetLocalIPAddress();
                string baseIp = string.Join(".", localIp.Split('.').Take(3)) + ".";
                List<string> activeIps = new List<string>();
                HashSet<string> seenMacs = new HashSet<string>();

                // 2. Tüm IP'lere paralel ping at (daha hızlı ve sınırlı sayıda paralel işlem)
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
                await Task.Run(() =>
                {
                    Parallel.For(1, 255, parallelOptions, (i) =>
                    {
                        string ip = baseIp + i;
                        using (System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping())
                        {
                            try
                            {
                                var reply = ping.Send(ip, 70); // Daha kısa timeout
                                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                                {
                                    lock (activeIps)
                                        activeIps.Add(ip);
                                }
                            }
                            catch { }
                        }
                    });
                });

                // 3. ARP tablosunu oku
                string arpOutput = RunCommand("arp", "-a");
                string[] arpLines = arpOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var arpDevices = new List<(string ip, string mac)>();
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

                // 4. Ping ve ARP sonuçlarını birleştir
                var allIps = activeIps.Union(arpDevices.Select(d => d.ip)).Distinct().ToList();

                allDevices.Clear();
                foreach (string ip in allIps)
                {
                    string mac = arpDevices.FirstOrDefault(d => d.ip == ip).mac ?? "";
                    mac = mac.Replace(":", "-").ToUpper();
                    if (string.IsNullOrWhiteSpace(mac) || seenMacs.Contains(mac))
                        continue;
                    seenMacs.Add(mac);

                
                    string hostname = "Çözümlenemedi";


                    string vendor = "Bilinmeyen Üretici";
                    if (!macVendorCache.ContainsKey(mac))
                    {
                        try
                        {
                            vendor = await GetVendorFromMacAsync(mac);
                        }
                        catch { }
                    }
                    else
                    {
                        vendor = macVendorCache[mac];
                    }

                    string connectionType = GetConnectionType(ip);
                    string deviceType = GetDeviceType(vendor);

                    var deviceTuple = (ip, mac, hostname, vendor, connectionType, deviceType);
                    allDevices.Add(deviceTuple);

                    if (!chkLocalOnly.Checked || (connectionType != null && connectionType.Trim().ToLower() == "yerel ağ"))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            AddDeviceCard(ip, mac, hostname, vendor, connectionType, deviceType);
                            deviceCount++;
                            if (deviceCount == 1)
                                HideLoadingSpinner();
                            countSpinner.Location = new Point(labelCount.Right + 10, labelCount.Top + 2);
                            labelCount.Text = $"Toplam cihaz sayısı: {deviceCount}";
                        });
                    }
                }
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

        private async Task<string> GetVendorFromMacAsync(string mac)
        {
            // Clean up the MAC address
            mac = mac.Replace(":", "").Replace("-", "").ToUpper();

            // Check the cache first
            if (macVendorCache.ContainsKey(mac))
                return macVendorCache[mac];

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");
                    string url = $"https://macvendors.co/api/{mac}/json";
                    var response = await client.GetStringAsync(url);

                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        // Cache the result to reduce API calls
                        macVendorCache[mac] = response;
                        File.AppendAllLines(macVendorCacheFile, new[] { $"{mac}|{response}" });
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
                Width = 280,
                Height = 180,
                Margin = new Padding(10),
                Padding = new Padding(15),
                BackColor = Color.White,
                Tag = false
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
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MainMenuForm mainMenu = new MainMenuForm();
                mainMenu.Show();
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