using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace wifi4
{
    public partial class FormWifiNetworks : Form
    {
        private Color defaultButtonColor = Color.FromArgb(0, 122, 204);
        private Color hoverButtonColor = Color.FromArgb(0, 102, 184);

        public FormWifiNetworks()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
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

        private void btnScanWifi_Click(object sender, EventArgs e)
        {
            listViewWifiNetworks.Items.Clear();  // ListView'i temizle
            labelNetworkCount.Text = "WiFi taraması yapılıyor...";

            try
            {
                string output = RunCommand("netsh", "wlan show networks mode=bssid");
                string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                List<string> networks = new List<string>();
                string ssid = "", signal = "", security = "";

                foreach (var raw in lines)
                {
                    string line = raw.Trim();
                    if (line.StartsWith("SSID"))
                    {
                        if (!string.IsNullOrEmpty(ssid))
                        {
                            networks.Add($"{ssid} - {signal} - {security}");
                        }

                        ssid = line.Split(new[] { ':' }, 2)[1].Trim();
                        signal = "";
                        security = "";
                    }
                    else if (line.StartsWith("Signal"))
                    {
                        signal = line.Split(':')[1].Trim();
                    }
                    else if (line.StartsWith("Authentication"))
                    {
                        security = line.Split(':')[1].Trim();
                    }
                }

                // Son SSID'yi de ekle
                if (!string.IsNullOrEmpty(ssid))
                    networks.Add($"{ssid} - {signal} - {security}");

                // ListView'e ekleme
                foreach (var net in networks.Distinct())
                {
                    var parts = net.Split('-');
                    ListViewItem item = new ListViewItem(parts[0].Trim()); // SSID
                    item.SubItems.Add(parts[1].Trim()); // Sinyal
                    item.SubItems.Add(parts[2].Trim()); // Güvenlik
                    listViewWifiNetworks.Items.Add(item);
                }

                labelNetworkCount.Text = $"Görünen Ağ Sayısı: {networks.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tarama hatası: " + ex.Message);
            }
        }

        private string RunCommand(string cmd, string args)
        {
            var psi = new ProcessStartInfo(cmd, args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var process = Process.Start(psi))
            {
                if (process == null)
                {
                    throw new Exception($"Failed to start process: {cmd} {args}");
                }
                return process.StandardOutput.ReadToEnd();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide();
        }
    }
}
