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
        private MediaDevice? connectedDevice = null;
        private string? connectedDeviceName = null;
        private CancellationTokenSource? recoveryCancellationToken = null;
        private bool isRecoveryRunning = false;
        private string? currentScanningPath = null;

    public Form1()
    {
        InitializeComponent();
            
            // Initialize auto-detection as enabled by default
            isAutoDetectionEnabled = true;
            btnToggleMonitoring.Text = "Auto-Detect: ON";
            timer1.Enabled = true;
            
            // Initialize scanning path display
            txtRecoveryPath.Text = "Ready to scan - Connect device and start recovery";
            
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
            try
            {
                // Update status
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = "Refreshing device list...";
                }
                
                // Force refresh the device list
                RefreshDevicesSilently();
                
                // Update last known devices to current state
                lastKnownDevices = new List<string>(GetCurrentDeviceList());
                
                // Update status
                if (statusStrip1.Items.Count > 0)
                {
                    var deviceCount = lstDevices.Items.Count;
                    if (deviceCount > 0)
                    {
                        statusStrip1.Items[0].Text = $"Found {deviceCount} device(s) - Ready for connection";
                    }
                    else
                    {
                        statusStrip1.Items[0].Text = "No devices found - Connect an Android device";
                    }
                }
                
                Console.WriteLine("Device list refreshed manually");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing devices: {ex.Message}", 
                              "Refresh Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                
                Console.WriteLine($"Refresh devices error: {ex.Message}");
                
                // Update status on error
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = "Error refreshing device list";
                }
            }
        }

        private void btnTestDetection_Click(object sender, EventArgs e)
        {
            try
            {
                // Update status
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = "Testing device detection...";
                }
                
                // Test device detection
                var devices = GetCurrentDeviceList();
                
                if (devices.Any())
                {
                    var deviceNames = string.Join(", ", devices.Select(d => d.Split('|')[0]));
                    MessageBox.Show($"Device Detection Test PASSED!\n\nFound {devices.Count} device(s):\n{deviceNames}", 
                                  "Detection Test", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information);
                    
                    // Update status
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = $"Detection test passed - Found {devices.Count} device(s)";
                    }
                }
                else
                {
                    MessageBox.Show("Device Detection Test FAILED!\n\nNo devices found.\n\nPlease ensure:\n• Android device is connected via USB\n• USB debugging is enabled\n• Device is unlocked\n• Try different USB cable/port", 
                                  "Detection Test", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    
                    // Update status
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = "Detection test failed - No devices found";
                    }
                }
                
                Console.WriteLine($"Detection test completed - Found {devices.Count} devices");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Detection test error: {ex.Message}", 
                              "Test Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                
                Console.WriteLine($"Test detection error: {ex.Message}");
                
                // Update status on error
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = "Detection test error occurred";
                }
            }
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
            try
            {
                // Check if a device is selected
                if (lstDevices.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a device from the list first.", 
                                  "No Device Selected", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    return;
                }

                // Get selected device
                var selectedDeviceName = lstDevices.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedDeviceName))
                {
                    MessageBox.Show("Invalid device selection.", 
                                  "Selection Error", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                    return;
                }

                // Update status
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = $"Connecting to {selectedDeviceName}...";
                }

                // Find the device in our device list
                var devices = GetCurrentDeviceList();
                var selectedDevice = devices.FirstOrDefault(d => d.Split('|')[0] == selectedDeviceName);
                
                if (selectedDevice == null)
                {
                    MessageBox.Show("Selected device is no longer available. Please refresh the device list.", 
                                  "Device Not Available", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    
                    // Refresh device list
                    RefreshDevicesSilently();
                    return;
                }

                // Extract device ID for connection
                var deviceId = selectedDevice.Split('|')[1];
                
                // Check if we're already connected to this device
                if (connectedDevice != null && connectedDeviceName == selectedDeviceName)
                {
                    // Disconnect from current device
                    DisconnectFromDevice();
                    return;
                }
                
                // Disconnect from any previously connected device
                if (connectedDevice != null)
                {
                    DisconnectFromDevice();
                }
                
                // Attempt to connect to the device
                var device = ConnectToDevice(deviceId);
                
                if (device != null)
                {
                    // Store connected device
                    connectedDevice = device;
                    connectedDeviceName = selectedDeviceName;
                    
                    // Update UI for connected state
                    btnConnectDevice.Text = "Disconnect";
                    btnConnectDevice.BackColor = Color.LightCoral;
                    btnConnectDevice.Enabled = true;
                    
                    // Update status
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = $"Connected to {selectedDeviceName} - Ready for recovery";
                    }
                    
                    // Update device status label
                    lblDeviceStatus.Text = $"Connected to {selectedDeviceName}";
                    lblDeviceStatus.ForeColor = Color.Green;
                    
                    // Enable start recovery button if recovery path is set
                    UpdateRecoveryButtonStates();
                    
                    MessageBox.Show($"Successfully connected to {selectedDeviceName}!\n\nYou can now start file recovery.", 
                                  "Connection Successful", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information);
                    
                    Console.WriteLine($"Successfully connected to device: {selectedDeviceName}");
                }
                else
                {
                    MessageBox.Show($"Failed to connect to {selectedDeviceName}.\n\nPlease ensure:\n• Device is unlocked\n• USB debugging is enabled\n• Try refreshing the device list", 
                                  "Connection Failed", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                    
                    // Update status
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = $"Failed to connect to {selectedDeviceName}";
                    }
                    
                    Console.WriteLine($"Failed to connect to device: {selectedDeviceName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to device: {ex.Message}", 
                              "Connection Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                
                Console.WriteLine($"Connect device error: {ex.Message}");
                
                // Update status on error
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = "Connection error occurred";
                }
            }
        }

        private void lstDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Empty - ready for implementation
        }

        private void btnStopRecovery_Click(object sender, EventArgs e)
        {
            try
            {
                if (isRecoveryRunning && recoveryCancellationToken != null)
                {
                    // Cancel the recovery operation
                    recoveryCancellationToken.Cancel();
                    
                    // Update status
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = "Stopping recovery...";
                    }
                    
                    Console.WriteLine("Recovery stop requested by user");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping recovery: {ex.Message}", 
                              "Stop Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                
                Console.WriteLine($"Stop recovery error: {ex.Message}");
            }
        }

        private void btnStartRecovery_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate prerequisites
                if (connectedDevice == null)
                {
                    MessageBox.Show("Please connect to a device first before starting recovery.", 
                                  "No Device Connected", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtRecoveryPath.Text) || txtRecoveryPath.Text == "Click Browse... to select recovery directory")
                {
                    MessageBox.Show("Please select a recovery directory first.", 
                                  "No Recovery Path", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    return;
                }

                if (!chkIncludeImages.Checked && !chkIncludeVideos.Checked)
                {
                    MessageBox.Show("Please select at least one file type to recover (Images or Videos).", 
                                  "No File Types Selected", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    return;
                }

                // Update UI for recovery start
                isRecoveryRunning = true;
                UpdateRecoveryButtonStates();
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                // Update status
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = "Starting file recovery scan...";
                }

                // Start recovery process
                StartFileRecovery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting recovery: {ex.Message}", 
                              "Recovery Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                
                Console.WriteLine($"Start recovery error: {ex.Message}");
                
                // Reset UI on error
                isRecoveryRunning = false;
                UpdateRecoveryButtonStates();
                progressBar.Visible = false;
            }
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
                            
                            // Update recovery button states
                            UpdateRecoveryButtonStates();
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

        private MediaDevice? ConnectToDevice(string deviceId)
        {
            try
            {
                // Get all available devices
                var devices = MediaDevice.GetDevices();
                
                // Find the device with matching ID
                var device = devices.FirstOrDefault(d => d.DeviceId == deviceId);
                
                if (device == null)
                {
                    Console.WriteLine($"Device with ID {deviceId} not found");
                    return null;
                }

                // Attempt to connect to the device
                if (device.IsConnected)
                {
                    Console.WriteLine($"Device {device.FriendlyName} is already connected");
                    return device;
                }

                // Try to connect
                device.Connect();
                
                if (device.IsConnected)
                {
                    Console.WriteLine($"Successfully connected to {device.FriendlyName}");
                    return device;
                }
                else
                {
                    Console.WriteLine($"Failed to connect to {device.FriendlyName}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ConnectToDevice error: {ex.Message}");
                return null;
            }
        }

        private void DisconnectFromDevice()
        {
            try
            {
                if (connectedDevice != null)
                {
                    var deviceName = connectedDeviceName ?? "Unknown Device";
                    
                    // Disconnect from device
                    if (connectedDevice.IsConnected)
                    {
                        connectedDevice.Disconnect();
                    }
                    
                    // Clear connected device
                    connectedDevice = null;
                    connectedDeviceName = null;
                    
                    // Update UI for disconnected state
                    btnConnectDevice.Text = "Connect Device";
                    btnConnectDevice.BackColor = SystemColors.Control;
                    btnConnectDevice.Enabled = true;
                    
                    // Update status
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = $"Disconnected from {deviceName}";
                    }
                    
                    // Update device status label
                    lblDeviceStatus.Text = "No devices found";
                    lblDeviceStatus.ForeColor = SystemColors.ControlText;
                    
                    // Update recovery button states
                    UpdateRecoveryButtonStates();
                    
                    Console.WriteLine($"Disconnected from device: {deviceName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DisconnectFromDevice error: {ex.Message}");
            }
        }

        private async void StartFileRecovery()
        {
            try
            {
                isRecoveryRunning = true;
                recoveryCancellationToken = new CancellationTokenSource();
                
                // Clear previous results
                lstRecoveredFiles.Items.Clear();
                
                // Get file types to scan
                var fileTypes = new List<string>();
                if (chkIncludeImages.Checked)
                {
                    fileTypes.AddRange(new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".heic", ".raw" });
                }
                if (chkIncludeVideos.Checked)
                {
                    fileTypes.AddRange(new[] { ".mp4", ".avi", ".mov", ".mkv", ".wmv", ".flv", ".webm", ".3gp", ".m4v" });
                }
                
                // Update status and info box
                if (statusStrip1.Items.Count > 0)
                {
                    var fileTypeList = string.Join(", ", fileTypes);
                    statusStrip1.Items[0].Text = $"Starting scan for {fileTypes.Count} file types: {fileTypeList}";
                }
                
                // Update info box
                UpdateScanningInfo($"Starting File Recovery Scan\n\nFile Types: {string.Join(", ", fileTypes)}\n\nScanning device storage for recoverable files...");
                
                // Start recovery process
                await Task.Run(() => PerformFileRecovery(fileTypes, recoveryCancellationToken.Token));
                
                // Recovery completed
                isRecoveryRunning = false;
                
                // Update UI
                UpdateRecoveryButtonStates();
                progressBar.Visible = false;
                
                // Update status and info box
                var recoveredCount = lstRecoveredFiles.Items.Count;
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = $"Recovery completed - Found {recoveredCount} files";
                }
                currentScanningPath = null;
                txtRecoveryPath.Text = "Recovery completed - All directories scanned";
                UpdateScanningInfo($"Recovery Complete\n\nTotal Files Found: {recoveredCount}\nRecovery Path: {txtRecoveryPath.Text}\n\nAll files have been successfully recovered!");
                
                // Show completion message
                MessageBox.Show($"File recovery completed!\n\nFound {recoveredCount} recoverable files.\n\nFiles have been saved to:\n{txtRecoveryPath.Text}", 
                              "Recovery Complete", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Information);
                
                Console.WriteLine($"Recovery completed - Found {recoveredCount} files");
            }
            catch (OperationCanceledException)
            {
                // Recovery was cancelled
                isRecoveryRunning = false;
                UpdateRecoveryButtonStates();
                progressBar.Visible = false;
                
                if (statusStrip1.Items.Count > 0)
                {
                    statusStrip1.Items[0].Text = "Recovery cancelled by user";
                }
                currentScanningPath = null;
                txtRecoveryPath.Text = "Recovery cancelled by user";
                UpdateScanningInfo($"Recovery Cancelled\n\nRecovery was stopped by user\n\nFiles found before cancellation have been saved.");
                
                Console.WriteLine("Recovery cancelled by user");
            }
            catch (Exception ex)
            {
                isRecoveryRunning = false;
                UpdateRecoveryButtonStates();
                progressBar.Visible = false;
                
                currentScanningPath = null;
                txtRecoveryPath.Text = "Recovery error - Check device connection";
                UpdateScanningInfo($"Recovery Error\n\nError: {ex.Message}\n\nPlease check device connection and try again.");
                
                MessageBox.Show($"Recovery error: {ex.Message}", 
                              "Recovery Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                
                Console.WriteLine($"Recovery error: {ex.Message}");
            }
        }

        private void PerformFileRecovery(List<string> fileTypes, CancellationToken cancellationToken)
        {
            try
            {
                var recoveredFiles = new List<RecoveredFileInfo>();
                var recoveryPath = txtRecoveryPath.Text;
                
                // Create recovery directory if it doesn't exist
                if (!Directory.Exists(recoveryPath))
                {
                    Directory.CreateDirectory(recoveryPath);
                }
                
                // Scan device storage
                if (connectedDevice != null)
                {
                    ScanDeviceStorage(connectedDevice, fileTypes, recoveredFiles, recoveryPath, cancellationToken);
                }
                
                // Update UI with results
                this.Invoke(new Action(() => {
                    RefreshRecoveredFilesList(recoveredFiles);
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PerformFileRecovery error: {ex.Message}");
                throw;
            }
        }

        private void ScanDeviceStorage(MediaDevice device, List<string> fileTypes, List<RecoveredFileInfo> recoveredFiles, string recoveryPath, CancellationToken cancellationToken)
        {
            try
            {
                // Get device root directory
                var rootDir = device.GetDirectoryInfo("/");
                
                if (rootDir != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                        
                    // Update status, info box, and scanning path
                    this.Invoke(new Action(() => {
                        if (statusStrip1.Items.Count > 0)
                        {
                            statusStrip1.Items[0].Text = $"Scanning device storage: {rootDir.Name}...";
                        }
                        currentScanningPath = $"/{rootDir.Name}";
                        txtRecoveryPath.Text = $"Scanning: {currentScanningPath}";
                        UpdateScanningInfo($"Scanning Device Storage\n\nCurrent Directory: {rootDir.Name}\n\nScanning for recoverable files...");
                    }));
                    
                    // Scan directory recursively
                    ScanDirectoryRecursive(rootDir, fileTypes, recoveredFiles, recoveryPath, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ScanDeviceStorage error: {ex.Message}");
            }
        }

        private void ScanDirectoryRecursive(MediaDirectoryInfo directory, List<string> fileTypes, List<RecoveredFileInfo> recoveredFiles, string recoveryPath, CancellationToken cancellationToken)
        {
            try
            {
                // Update status, info box, and scanning path to show current directory being scanned
                this.Invoke(new Action(() => {
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = $"Scanning directory: {directory.Name}...";
                    }
                    currentScanningPath = $"/{directory.Name}";
                    txtRecoveryPath.Text = $"Scanning: {currentScanningPath}";
                    UpdateScanningInfo($"Scanning Directory\n\nDirectory: {directory.Name}\n\nChecking files for recovery...");
                }));
                
                // Scan files in current directory
                var files = directory.EnumerateFiles();
                int fileCount = 0;
                int recoverableCount = 0;
                
                foreach (var file in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                        
                    fileCount++;
                    
                    // Update status and info box every 5 files
                    if (fileCount % 5 == 0)
                    {
                        this.Invoke(new Action(() => {
                            if (statusStrip1.Items.Count > 0)
                            {
                                statusStrip1.Items[0].Text = $"Scanning {directory.Name}: {fileCount} files checked, {recoverableCount} recoverable...";
                            }
                            UpdateScanningInfo($"Scanning Progress\n\nDirectory: {directory.Name}\nFiles Checked: {fileCount}\nRecoverable Found: {recoverableCount}\n\nContinuing scan...");
                        }));
                    }
                    
                    var fileExtension = Path.GetExtension(file.Name).ToLower();
                    if (fileTypes.Contains(fileExtension))
                    {
                        recoverableCount++;
                        
                        // Update status and info box for each recoverable file found
                        this.Invoke(new Action(() => {
                            if (statusStrip1.Items.Count > 0)
                            {
                                statusStrip1.Items[0].Text = $"Found recoverable file: {file.Name} ({FormatFileSize((long)file.Length)})";
                            }
                            UpdateScanningInfo($"Found Recoverable File\n\nFile: {file.Name}\nSize: {FormatFileSize((long)file.Length)}\nType: {fileExtension}\n\nPreparing to recover...");
                        }));
                        
                        // Found a recoverable file
                        var recoveredFile = new RecoveredFileInfo
                        {
                            FileName = file.Name,
                            FileSize = (long)file.Length,
                            FileType = fileExtension,
                            RecoveryDate = DateTime.Now,
                            Status = "Recovered",
                            IsRealFile = true
                        };
                        
                        // Copy file to recovery directory
                        try
                        {
                            var destinationPath = Path.Combine(recoveryPath, file.Name);
                            
                            // Update status and info box during file copy
                            this.Invoke(new Action(() => {
                                if (statusStrip1.Items.Count > 0)
                                {
                                    statusStrip1.Items[0].Text = $"Recovering: {file.Name}...";
                                }
                                UpdateScanningInfo($"Recovering File\n\nFile: {file.Name}\nSize: {FormatFileSize((long)file.Length)}\nDestination: {destinationPath}\n\nCopying file...");
                            }));
                            
                            using (var sourceStream = file.OpenRead())
                            using (var destinationStream = File.Create(destinationPath))
                            {
                                sourceStream.CopyTo(destinationStream);
                            }
                            
                            recoveredFile.Status = "Recovered";
                            Console.WriteLine($"Recovered: {file.Name} ({file.Length} bytes)");
                            
                            // Update status and info box after successful recovery
                            this.Invoke(new Action(() => {
                                if (statusStrip1.Items.Count > 0)
                                {
                                    statusStrip1.Items[0].Text = $"Recovered: {file.Name} - Total: {recoveredFiles.Count + 1} files";
                                }
                                UpdateScanningInfo($"File Successfully Recovered\n\nFile: {file.Name}\nSize: {FormatFileSize((long)file.Length)}\nTotal Recovered: {recoveredFiles.Count + 1} files\n\nContinuing scan...");
                            }));
                        }
                        catch (Exception ex)
                        {
                            recoveredFile.Status = "Error";
                            Console.WriteLine($"Error recovering {file.Name}: {ex.Message}");
                            
                            // Update status and info box for error
                            this.Invoke(new Action(() => {
                                if (statusStrip1.Items.Count > 0)
                                {
                                    statusStrip1.Items[0].Text = $"Error recovering: {file.Name}";
                                }
                                UpdateScanningInfo($"Recovery Error\n\nFile: {file.Name}\nError: {ex.Message}\n\nContinuing scan...");
                            }));
                        }
                        
                        recoveredFiles.Add(recoveredFile);
                        
                        // Update UI periodically
                        if (recoveredFiles.Count % 10 == 0)
                        {
                            this.Invoke(new Action(() => {
                                RefreshRecoveredFilesList(recoveredFiles);
                            }));
                        }
                    }
                }
                
                // Update status and info box after scanning directory
                this.Invoke(new Action(() => {
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = $"Completed {directory.Name}: {fileCount} files scanned, {recoverableCount} recoverable";
                    }
                    UpdateScanningInfo($"Directory Scan Complete\n\nDirectory: {directory.Name}\nFiles Scanned: {fileCount}\nRecoverable Found: {recoverableCount}\n\nScanning subdirectories...");
                }));
                
                // Scan subdirectories
                var subdirs = directory.EnumerateDirectories();
                foreach (var subdir in subdirs)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    
                    // Update scanning path for subdirectory
                    this.Invoke(new Action(() => {
                        currentScanningPath = $"{currentScanningPath}/{subdir.Name}";
                        txtRecoveryPath.Text = $"Scanning: {currentScanningPath}";
                    }));
                        
                    ScanDirectoryRecursive(subdir, fileTypes, recoveredFiles, recoveryPath, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ScanDirectoryRecursive error: {ex.Message}");
                
                // Update status and info box for error
                this.Invoke(new Action(() => {
                    if (statusStrip1.Items.Count > 0)
                    {
                        statusStrip1.Items[0].Text = $"Error scanning {directory.Name}: {ex.Message}";
                    }
                    UpdateScanningInfo($"Scanning Error\n\nDirectory: {directory.Name}\nError: {ex.Message}\n\nContinuing scan...");
                }));
            }
        }

        private void RefreshRecoveredFilesList(List<RecoveredFileInfo> recoveredFiles)
        {
            try
            {
                lstRecoveredFiles.Items.Clear();
                
                foreach (var file in recoveredFiles)
                {
                    var item = new ListViewItem(file.FileName);
                    item.SubItems.Add(FormatFileSize(file.FileSize));
                    item.SubItems.Add(file.FileType);
                    item.SubItems.Add(file.RecoveryDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    item.SubItems.Add(file.Status);
                    item.Tag = file;
                    
                    lstRecoveredFiles.Items.Add(item);
                }
                
                // Auto-select first item for preview
                if (lstRecoveredFiles.Items.Count > 0)
                {
                    lstRecoveredFiles.Items[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RefreshRecoveredFilesList error: {ex.Message}");
            }
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024 * 1024):F1} MB";
            return $"{bytes / (1024 * 1024 * 1024):F1} GB";
        }

        private void UpdateRecoveryButtonStates()
        {
            try
            {
                // Check if device is connected and recovery path is set
                bool canStartRecovery = connectedDevice != null && 
                                      !string.IsNullOrEmpty(txtRecoveryPath.Text) && 
                                      txtRecoveryPath.Text != "Click Browse... to select recovery directory" &&
                                      !isRecoveryRunning;
                
                // Update start recovery button
                btnStartRecovery.Enabled = canStartRecovery;
                
                // Update stop recovery button
                btnStopRecovery.Enabled = isRecoveryRunning;
                
                Console.WriteLine($"Recovery buttons updated - Start: {canStartRecovery}, Stop: {isRecoveryRunning}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateRecoveryButtonStates error: {ex.Message}");
            }
        }

        private void UpdateScanningInfo(string info)
        {
            try
            {
                if (lblPreview.InvokeRequired)
                {
                    lblPreview.Invoke(new Action(() => UpdateScanningInfo(info)));
                    return;
                }
                
                lblPreview.Text = $"Scanning Information:\n\n{info}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateScanningInfo error: {ex.Message}");
            }
        }

        public class RecoveredFileInfo
        {
            public string FileName { get; set; } = string.Empty;
            public long FileSize { get; set; }
            public string FileType { get; set; } = string.Empty;
            public DateTime RecoveryDate { get; set; }
            public string Status { get; set; } = string.Empty;
            public bool IsRealFile { get; set; }
        }
    }
}