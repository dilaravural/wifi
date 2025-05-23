using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wifi4
{
    public partial class MainMenuForm : Form
    {
        private Color defaultButtonColor = Color.FromArgb(0, 122, 204);
        private Color hoverButtonColor = Color.FromArgb(0, 102, 184);

        public MainMenuForm()
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

        // WiFi ağ taraması için butona tıklama olayı
        private void btnWifiScan_Click(object sender, EventArgs e)
        {
            // WiFi ağlarını gösterecek formu başlat
            FormWifiNetworks wifiNetworksForm = new FormWifiNetworks(); // FormWifiNetworks yerine uygun formu kullanın
            wifiNetworksForm.Show();
            this.Hide(); // Ana menüyü gizle
        }

        // Bağlı cihazları göstermek için butona tıklama olayı
        private void btnConnectedDevices_Click(object sender, EventArgs e)
        {
            ConnectedDevicesForm devicesForm = new ConnectedDevicesForm(); // Bağlı cihazları gösterecek form
            devicesForm.Show();
            this.Hide(); // Ana menüyü gizle
        }
    }
}
