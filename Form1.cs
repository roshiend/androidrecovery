using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaDevices;

namespace AndroidFileRecovery
{
public partial class Form1 : Form
{
        private bool isAutoDetectionEnabled = false;
        private List<string> lastKnownDevices = new List<string>();
        private DateTime lastDeviceNotification = DateTime.MinValue;
        private readonly TimeSpan notificationCooldown = TimeSpan.FromSeconds(5);

    public Form1()
    {
        InitializeComponent();
            
            // Initialize auto-detection as enabled by default
            isAutoDetectionEnabled = true;
            btnToggleMonitoring.Text = "Auto-Detect: ON";
            timer1.Enabled = true;
            
            // Load initial device list
            RefreshDevicesSilently();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void viewStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void viewLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void btnRefreshDevices_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void btnTestDetection_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void btnToggleMonitoring_Click(object sender, EventArgs e)
        {
            try
            {
                isAutoDetectionEnabled = !isAutoDetectionEnabled;
                
                if (isAutoDetectionEnabled)
                {
                    timer1.Enabled = true;
                    btnToggleMonitoring.Text = "Auto-Detect: ON";
                    btnToggleMonitoring.BackColor = Color.LightGreen;
                    
                    // Update status
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = "Auto-detection enabled - monitoring for device changes";
                    }
                    
                    // Refresh devices immediately
                    RefreshDevicesSilently();
                    
                    Console.WriteLine("Auto-detection enabled");
                }
                else
                {
                    timer1.Enabled = false;
                    btnToggleMonitoring.Text = "Auto-Detect: OFF";
                    btnToggleMonitoring.BackColor = Color.LightCoral;
                    
                    // Update status
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = "Auto-detection disabled - manual refresh required";
                    }
                    
                    Console.WriteLine("Auto-detection disabled");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling auto-detection: {ex.Message}", 
                              "Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                
                Console.WriteLine($"Toggle monitoring error: {ex.Message}");
            }
        }

        private void btnConnectDevice_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void lstDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void btnStopRecovery_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void btnStartRecovery_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void btnBrowsePath_Click(object sender, EventArgs e)
        {
            try
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select Recovery Directory";
                    folderDialog.ShowNewFolderButton = true;
                    
                    // Set initial directory if one is already selected
                    if (!string.IsNullOrEmpty(txtRecoveryPath.Text) && Directory.Exists(txtRecoveryPath.Text))
                    {
                        folderDialog.SelectedPath = txtRecoveryPath.Text;
                    }
                    else
                    {
                        // Default to Desktop or Documents
                        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        if (Directory.Exists(defaultPath))
                        {
                            folderDialog.SelectedPath = defaultPath;
                        }
                    }

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedPath = folderDialog.SelectedPath;
                        
                        // Validate the selected path
                        if (Directory.Exists(selectedPath))
                        {
                            txtRecoveryPath.Text = selectedPath;
                            
                            // Update status bar
                            if (statusStrip1.Items.Count > 0)
                            {
                                statusStrip1.Items[0].Text = $"Recovery path set to: {selectedPath}";
                            }
                            
                            // Log the selection
                            Console.WriteLine($"Recovery path selected: {selectedPath}");
                        }
                        else
                        {
                            MessageBox.Show("The selected path does not exist or is not accessible.", 
                                          "Invalid Path", 
                                          MessageBoxButtons.OK, 
                                          MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting recovery path: {ex.Message}", 
                              "Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                
                Console.WriteLine($"Browse path error: {ex.Message}");
            }
        }

        private void lstRecoveredFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void recoverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!isAutoDetectionEnabled)
                return;

            try
            {
                CheckForDeviceChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Auto-detection error: {ex.Message}");
            }
        }

        private void CheckForDeviceChanges()
        {
            try
            {
                var currentDevices = GetCurrentDeviceList();
                
                // Check if device list has changed
                if (!currentDevices.SequenceEqual(lastKnownDevices))
                {
                    var connectedDevices = currentDevices.Except(lastKnownDevices).ToList();
                    var disconnectedDevices = lastKnownDevices.Except(currentDevices).ToList();
                    
                    // Update device list silently
                    RefreshDevicesSilently();
                    
                    // Show notifications with cooldown and only if there are actual changes
                    if (DateTime.Now - lastDeviceNotification > notificationCooldown && 
                        (connectedDevices.Any() || disconnectedDevices.Any()))
                    {
                        if (connectedDevices.Any())
                        {
                            OnDevicesConnected(connectedDevices);
                        }
                        
                        if (disconnectedDevices.Any())
                        {
                            OnDevicesDisconnected(disconnectedDevices);
                        }
                        
                        lastDeviceNotification = DateTime.Now;
                    }
                    
                    lastKnownDevices = new List<string>(currentDevices);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CheckForDeviceChanges error: {ex.Message}");
            }
        }

        private List<string> GetCurrentDeviceList()
        {
            try
            {
                var devices = new List<string>();
                
                // Get MTP devices
                var mtpDevices = MediaDevice.GetDevices();
                foreach (var device in mtpDevices)
                {
                    // Use a more stable identifier to prevent false triggers
                    var deviceId = device.DeviceId ?? "unknown";
                    var friendlyName = device.FriendlyName ?? "Unknown Device";
                    devices.Add($"{friendlyName}|{deviceId}");
                }
                
                return devices;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCurrentDeviceList error: {ex.Message}");
                return new List<string>();
            }
        }

        private void RefreshDevicesSilently()
        {
            try
            {
                lstDevices.Items.Clear();
                var devices = GetCurrentDeviceList();
                
                foreach (var device in devices)
                {
                    // Display friendly name in list
                    var displayName = device.Split('|')[0];
                    lstDevices.Items.Add(displayName);
                }
                
                // Update device status
                if (devices.Any())
                {
                    lblDeviceStatus.Text = $"Found {devices.Count} device(s)";
                    btnConnectDevice.Enabled = true;
                }
                else
                {
                    lblDeviceStatus.Text = "No devices found";
                    btnConnectDevice.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RefreshDevicesSilently error: {ex.Message}");
                lblDeviceStatus.Text = "Error detecting devices";
                btnConnectDevice.Enabled = false;
            }
        }

        private void OnDevicesConnected(List<string> devices)
        {
            try
            {
                var deviceNames = string.Join(", ", devices.Select(d => 
                {
                    var parts = d.Split('|');
                    return parts.Length > 0 ? parts[0] : "Unknown Device";
                }));
                
                // Update status bar instead of showing message box to prevent loops
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = $"Device(s) connected: {deviceNames}";
                }
                
                Console.WriteLine($"Devices connected: {string.Join(", ", devices)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnDevicesConnected error: {ex.Message}");
            }
        }

        private void OnDevicesDisconnected(List<string> devices)
        {
            try
            {
                var deviceNames = string.Join(", ", devices.Select(d => 
                {
                    var parts = d.Split('|');
                    return parts.Length > 0 ? parts[0] : "Unknown Device";
                }));
                
                // Update status bar instead of showing message box to prevent loops
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = $"Device(s) disconnected: {deviceNames}";
                }
                
                Console.WriteLine($"Devices disconnected: {string.Join(", ", devices)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnDevicesDisconnected error: {ex.Message}");
            }
        }
    }
}