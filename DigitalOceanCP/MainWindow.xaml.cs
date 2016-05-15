using DigitalOceanCP.Properties;
using DigitalOceanCP.ProfileSetting;
using DigitalOceanCP.SaveWindowsState;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DigitalOceanCP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProfileLists profileLists = null;

        public MainWindow()
        {
            InitializeComponent();
            
            profileLists = new ProfileLists();
            profileLists.Reload(); // Refresh (get) data from persisted storage.
            RefreshProfile();
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            CreateProfile(profileName.Text, apiKey.Text);
            profileName.Clear();
            apiKey.Clear();
            comboBox1.SelectedIndex = 0;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteProfile(comboBox1.Text);
            profileName.Clear();
            comboBox1.SelectedIndex = 0;
        }

        private void DeleteProfile(string profileName)
        {
            profileLists.ProfileList.Remove(profileName);
            profileLists.Save();
        }

        private void CreateProfile(string profileName, string apiKey)
        {
            profileLists.ProfileList.Add(profileName, apiKey);
            profileLists.Save();
        }

        public void RefreshProfile()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add(new KeyValuePair<string, string>("---- Select Profile ----", null));
            foreach (KeyValuePair<string, string> profile in profileLists.ProfileList)
            {
                comboBox1.Items.Add(profile);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_DropDownOpened(object sender, EventArgs e)
        {
            RefreshProfile();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            try
            {
                // Load window placement details for previous application session from application settings
                // Note - if window was closed on a monitor that is now disconnected from the computer,
                //        SetWindowPlacement will place the window onto a visible monitor.
                var wp = Settings.Default.WindowPlacement;
                wp.length = Marshal.SizeOf(typeof(WindowPlacement));
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == SwShowminimized ? SwShownormal : wp.showCmd);
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowPlacement(hwnd, ref wp);
            }
            catch
            {
                // ignored
            }
        }

        // WARNING - Not fired when Application.SessionEnding is fired
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // Persist window placement details to application settings
            WindowPlacement wp;
            var hwnd = new WindowInteropHelper(this).Handle;
            GetWindowPlacement(hwnd, out wp);
            Settings.Default.WindowPlacement = wp;
            Settings.Default.Save();
        }

        #region Win32 API declarations to set and get window placement

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

        private const int SwShownormal = 1;
        private const int SwShowminimized = 2;

        #endregion
    }
}
