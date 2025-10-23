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
    private List<string> selectedDirectories = new List<string>();

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

        private async void btnConnectDevice_Click(object sender, EventArgs e)
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
                    
                    // Load directory tree for the connected device
                    await LoadDirectoryTree();
                    
                    // Enable start recovery button if recovery path is set
                    UpdateRecoveryButtonStates();
                    
                    MessageBox.Show($"Successfully connected to {selectedDeviceName}!\n\nYou can now select directories to scan and start file recovery.", 
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

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            if (lstRecoveredFiles.Items.Count == 0)
            {
                MessageBox.Show("No files to copy. Please scan for files first.", "No Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Get recovery path
                string recoveryPath = GetRecoveryPath();
                if (string.IsNullOrEmpty(recoveryPath))
                {
                    MessageBox.Show("Please select a recovery directory first.", "No Recovery Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Copy all files
                int copiedCount = 0;
                int errorCount = 0;

                foreach (ListViewItem item in lstRecoveredFiles.Items)
                {
                    var fileInfo = item.Tag as RecoveredFileInfo;
                    if (fileInfo != null && fileInfo.IsRealFile)
                    {
                        try
                        {
                            // Copy file from device to recovery directory
                            CopyFileFromDevice(fileInfo, recoveryPath);
                            copiedCount++;
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            Console.WriteLine($"Error copying {fileInfo.FileName}: {ex.Message}");
                        }
                    }
                }

                MessageBox.Show($"Copy completed!\n\nCopied: {copiedCount} files\nErrors: {errorCount} files", 
                              "Copy Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during copy operation: {ex.Message}", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCopySelected_Click(object sender, EventArgs e)
        {
            if (lstRecoveredFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select files to copy first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Get recovery path
                string recoveryPath = GetRecoveryPath();
                if (string.IsNullOrEmpty(recoveryPath))
                {
                    MessageBox.Show("Please select a recovery directory first.", "No Recovery Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Copy selected files
                int copiedCount = 0;
                int errorCount = 0;

                foreach (ListViewItem item in lstRecoveredFiles.SelectedItems)
                {
                    var fileInfo = item.Tag as RecoveredFileInfo;
                    if (fileInfo != null && fileInfo.IsRealFile)
                    {
                        try
                        {
                            // Copy file from device to recovery directory
                            CopyFileFromDevice(fileInfo, recoveryPath);
                            copiedCount++;
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            Console.WriteLine($"Error copying {fileInfo.FileName}: {ex.Message}");
                        }
                    }
                }

                MessageBox.Show($"Copy completed!\n\nCopied: {copiedCount} files\nErrors: {errorCount} files", 
                              "Copy Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during copy operation: {ex.Message}", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            if (lstRecoveredFiles.Items.Count == 0)
            {
                MessageBox.Show("The file list is already empty.", "Empty List", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to clear the file list?\n\nThis will remove {lstRecoveredFiles.Items.Count} files from the list.", 
                                       "Clear File List", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                lstRecoveredFiles.Items.Clear();
                pictureBox1.Image = null;
                lblPreview.Text = "Scanning Information:\r\n\r\nReady to scan device storage...\r\n\r\nSelect a file to preview";
                UpdateRecoveryButtonStates();
            }
        }

        private void treeViewDirectories_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                // Update selected directories list
                UpdateSelectedDirectories();
                
                // Update button states based on selection
                UpdateRecoveryButtonStates();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"treeViewDirectories_AfterCheck error: {ex.Message}");
            }
        }

        private async void PreviewFile(RecoveredFileInfo fileInfo)
        {
            try
            {
                if (connectedDevice == null)
                {
                    lblPreview.Text = "No device connected - Cannot preview file";
                    pictureBox1.Image = null;
                    return;
                }

                // Update preview info
                lblPreview.Text = $"Loading Preview...\n\nFile: {fileInfo.FileName}\nSize: {FormatFileSize(fileInfo.FileSize)}\nType: {fileInfo.FileType}\n\nPlease wait...";

                // Load preview based on file type
                Image? previewImage = null;
                
                if (IsImageFile(fileInfo.FileType))
                {
                    previewImage = await LoadImageFromDevice(fileInfo);
                }
                else if (IsVideoFile(fileInfo.FileType))
                {
                    previewImage = await LoadVideoPreview(fileInfo);
                }
                else
                {
                    previewImage = await LoadFileTypePreview(fileInfo);
                }

                // Display the preview
                if (previewImage != null)
                {
                    pictureBox1.Image = previewImage;
                    lblPreview.Text = $"File Preview\n\nFile: {fileInfo.FileName}\nSize: {FormatFileSize(fileInfo.FileSize)}\nType: {fileInfo.FileType}\nStatus: {fileInfo.Status}";
                }
                else
                {
                    pictureBox1.Image = null;
                    lblPreview.Text = $"Preview Unavailable\n\nFile: {fileInfo.FileName}\nSize: {FormatFileSize(fileInfo.FileSize)}\nType: {fileInfo.FileType}\n\nPreview could not be loaded";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PreviewFile error: {ex.Message}");
                pictureBox1.Image = null;
                lblPreview.Text = $"Preview Error\n\nFile: {fileInfo.FileName}\nError: {ex.Message}\n\nPlease try again";
            }
        }

        private bool IsImageFile(string fileType)
        {
            var imageTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".heic", ".raw" };
            return imageTypes.Contains(fileType.ToLower());
        }

        private bool IsVideoFile(string fileType)
        {
            var videoTypes = new[] { ".mp4", ".avi", ".mov", ".mkv", ".wmv", ".flv", ".webm", ".3gp", ".m4v" };
            return videoTypes.Contains(fileType.ToLower());
        }

        private async Task<Image?> LoadImageFromDevice(RecoveredFileInfo fileInfo)
        {
            try
            {
                if (connectedDevice == null) return null;

                // Find the file on the device
                var rootDir = connectedDevice.GetDirectoryInfo("/");
                var file = FindFileInDevice(rootDir, fileInfo.FileName);
                
                if (file != null)
                {
                    // Load image from device
                    using (var stream = file.OpenRead())
                    {
                        var image = Image.FromStream(stream);
                        return new Bitmap(image); // Create a copy to avoid stream disposal issues
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadImageFromDevice error: {ex.Message}");
                return null;
            }
        }

        private async Task<Image?> LoadVideoPreview(RecoveredFileInfo fileInfo)
        {
            try
            {
                // For videos, create a placeholder image with video icon
                var bitmap = new Bitmap(400, 300);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.DarkGray);
                    
                    // Draw video icon
                    var font = new Font("Arial", 48, FontStyle.Bold);
                    var brush = new SolidBrush(Color.White);
                    var text = "🎥";
                    var textSize = g.MeasureString(text, font);
                    var x = (bitmap.Width - textSize.Width) / 2;
                    var y = (bitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                    
                    // Draw file info
                    var infoFont = new Font("Arial", 12, FontStyle.Regular);
                    var infoText = $"Video File\n{fileInfo.FileName}\n{FormatFileSize(fileInfo.FileSize)}";
                    var infoSize = g.MeasureString(infoText, infoFont);
                    var infoX = (bitmap.Width - infoSize.Width) / 2;
                    var infoY = y + textSize.Height + 10;
                    g.DrawString(infoText, infoFont, brush, infoX, infoY);
                }
                
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadVideoPreview error: {ex.Message}");
                return null;
            }
        }

        private async Task<Image?> LoadFileTypePreview(RecoveredFileInfo fileInfo)
        {
            try
            {
                // Create a generic file type preview
                var bitmap = new Bitmap(400, 300);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.LightGray);
                    
                    // Draw file icon
                    var font = new Font("Arial", 48, FontStyle.Bold);
                    var brush = new SolidBrush(Color.DarkBlue);
                    var text = "📄";
                    var textSize = g.MeasureString(text, font);
                    var x = (bitmap.Width - textSize.Width) / 2;
                    var y = (bitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                    
                    // Draw file info
                    var infoFont = new Font("Arial", 12, FontStyle.Regular);
                    var infoText = $"{fileInfo.FileType.ToUpper()} File\n{fileInfo.FileName}\n{FormatFileSize(fileInfo.FileSize)}";
                    var infoSize = g.MeasureString(infoText, infoFont);
                    var infoX = (bitmap.Width - infoSize.Width) / 2;
                    var infoY = y + textSize.Height + 10;
                    g.DrawString(infoText, infoFont, brush, infoX, infoY);
                }
                
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadFileTypePreview error: {ex.Message}");
                return null;
            }
        }

        private string GetRecoveryPath()
        {
            // Get recovery path from the text box
            string path = txtRecoveryPath.Text.Trim();
            
            // If it's the default message, return empty
            if (path.Contains("Ready to scan") || path.Contains("Current scanning path"))
            {
                return string.Empty;
            }
            
            return path;
        }

        private void CopyFileFromDevice(RecoveredFileInfo fileInfo, string recoveryPath)
        {
            if (connectedDevice == null)
            {
                throw new InvalidOperationException("No device connected");
            }

            try
            {
                // Create recovery directory if it doesn't exist
                if (!Directory.Exists(recoveryPath))
                {
                    Directory.CreateDirectory(recoveryPath);
                }

                // Get the file from device
                var rootDir = connectedDevice.GetDirectoryInfo("/");
                var file = FindFileInDevice(rootDir, fileInfo.FileName);
                
                if (file != null)
                {
                    var destinationPath = Path.Combine(recoveryPath, fileInfo.FileName);
                    
                    // Copy file from device to local storage
                    using (var sourceStream = file.OpenRead())
                    using (var destinationStream = File.Create(destinationPath))
                    {
                        sourceStream.CopyTo(destinationStream);
                    }
                    
                    Console.WriteLine($"Copied: {fileInfo.FileName} to {destinationPath}");
                }
                else
                {
                    throw new FileNotFoundException($"File {fileInfo.FileName} not found on device");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying {fileInfo.FileName}: {ex.Message}");
                throw;
            }
        }

        private MediaFileInfo? FindFileInDevice(MediaDirectoryInfo directory, string fileName)
        {
            try
            {
                // Search in current directory
                var files = directory.EnumerateFiles();
                foreach (var file in files)
                {
                    if (file.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        return file;
                    }
                }

                // Search in subdirectories
                var subdirs = directory.EnumerateDirectories();
                foreach (var subdir in subdirs)
                {
                    var foundFile = FindFileInDevice(subdir, fileName);
                    if (foundFile != null)
                    {
                        return foundFile;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching in directory {directory.Name}: {ex.Message}");
            }

            return null;
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
            try
            {
                // Update button states when selection changes
                UpdateRecoveryButtonStates();
                
                // Show preview of selected file
                if (lstRecoveredFiles.SelectedItems.Count > 0)
                {
                    var selectedItem = lstRecoveredFiles.SelectedItems[0];
                    var fileInfo = selectedItem.Tag as RecoveredFileInfo;
                    
                    if (fileInfo != null)
                    {
                        PreviewFile(fileInfo);
                    }
                }
                else
                {
                    // Clear preview when no selection
                    pictureBox1.Image = null;
                    lblPreview.Text = "Scanning Information:\r\n\r\nReady to scan device storage...\r\n\r\nSelect a file to preview";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"lstRecoveredFiles_SelectedIndexChanged error: {ex.Message}");
            }
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
                
                // Check if directories are selected
                if (selectedDirectories.Count == 0)
                {
                    MessageBox.Show("Please select at least one directory to scan from the directory tree.", 
                                  "No Directories Selected", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    isRecoveryRunning = false;
                    UpdateRecoveryButtonStates();
                    return;
                }
                
                // Update info box
                UpdateScanningInfo($"Starting File Recovery Scan\n\nFile Types: {string.Join(", ", fileTypes)}\n\nSelected Directories: {string.Join(", ", selectedDirectories)}\n\nScanning selected directories for recoverable files...");
                
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
                    statusStrip1.Items[0].Text = $"Scan completed - Found {recoveredCount} files";
                }
                currentScanningPath = null;
                txtRecoveryPath.Text = "Scan completed - All directories scanned";
                UpdateScanningInfo($"Scan Complete\n\nTotal Files Found: {recoveredCount}\n\nFiles are ready for copying. Use 'Copy All' or 'Copy Selected' buttons to copy files to your computer.");
                
                // Show completion message
                MessageBox.Show($"File scan completed!\n\nFound {recoveredCount} recoverable files.\n\nUse the 'Copy All' or 'Copy Selected' buttons to copy files to your computer.", 
                              "Scan Complete", 
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
                
                // Scan selected directories
                if (connectedDevice != null)
                {
                    ScanSelectedDirectories(connectedDevice, fileTypes, recoveredFiles, recoveryPath, cancellationToken);
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

        private void ScanSelectedDirectories(MediaDevice device, List<string> fileTypes, List<RecoveredFileInfo> recoveredFiles, string recoveryPath, CancellationToken cancellationToken)
        {
            try
            {
                // Get device root directory
                var rootDir = device.GetDirectoryInfo("/");
                
                if (rootDir != null)
                {
                    // Scan each selected directory
                    foreach (var selectedDir in selectedDirectories)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;
                            
                        try
                        {
                            MediaDirectoryInfo? targetDir = null;
                            
                            if (selectedDir == "/" || selectedDir == "Device Root")
                            {
                                // Scan root directory
                                targetDir = rootDir;
                            }
                            else
                            {
                                // Find the specific directory
                                targetDir = FindDirectoryInDevice(rootDir, selectedDir);
                            }
                            
                            if (targetDir != null)
                            {
                                // Update status and info box
                                this.Invoke(new Action(() => {
                                    if (statusStrip1.Items.Count > 0)
                                    {
                                        statusStrip1.Items[0].Text = $"Scanning selected directory: {targetDir.Name}...";
                                    }
                                    currentScanningPath = $"/{targetDir.Name}";
                                    txtRecoveryPath.Text = $"Scanning: {currentScanningPath}";
                                    UpdateScanningInfo($"Scanning Selected Directory\n\n📁 {targetDir.Name}\n\n🔍 Scanning for recoverable files...");
                                }));
                                
                                // Scan the directory recursively
                                ScanDirectoryRecursive(targetDir, fileTypes, recoveredFiles, recoveryPath, cancellationToken);
                            }
                            else
                            {
                                Console.WriteLine($"Directory not found: {selectedDir}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error scanning directory {selectedDir}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ScanSelectedDirectories error: {ex.Message}");
            }
        }

        private MediaDirectoryInfo? FindDirectoryInDevice(MediaDirectoryInfo rootDir, string directoryName)
        {
            try
            {
                // First try to find in root directories
                var subdirs = rootDir.EnumerateDirectories();
                foreach (var subdir in subdirs)
                {
                    if (subdir.Name.Equals(directoryName, StringComparison.OrdinalIgnoreCase))
                    {
                        return subdir;
                    }
                }
                
                // If not found, search recursively (limited depth)
                return FindDirectoryRecursive(rootDir, directoryName, 0, 3); // Max depth of 3
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FindDirectoryInDevice error: {ex.Message}");
                return null;
            }
        }

        private MediaDirectoryInfo? FindDirectoryRecursive(MediaDirectoryInfo parentDir, string directoryName, int currentDepth, int maxDepth)
        {
            try
            {
                if (currentDepth >= maxDepth)
                    return null;
                    
                var subdirs = parentDir.EnumerateDirectories();
                foreach (var subdir in subdirs)
                {
                    if (subdir.Name.Equals(directoryName, StringComparison.OrdinalIgnoreCase))
                    {
                        return subdir;
                    }
                    
                    // Search in subdirectories
                    var found = FindDirectoryRecursive(subdir, directoryName, currentDepth + 1, maxDepth);
                    if (found != null)
                        return found;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FindDirectoryRecursive error: {ex.Message}");
                return null;
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
                    UpdateScanningInfo($"Current Scanning Directory\n\n📁 {currentScanningPath}\n\n🔍 Checking files for recovery...");
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
                            UpdateScanningInfo($"Scanning Progress\n\n📁 {currentScanningPath}\nFiles Checked: {fileCount}\nRecoverable Found: {recoverableCount}\n\nContinuing scan...");
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
                            UpdateScanningInfo($"Found Recoverable File\n\n📁 {currentScanningPath}\n📄 {file.Name}\nSize: {FormatFileSize((long)file.Length)}\nType: {fileExtension}\n\nPreparing to recover...");
                        }));
                        
                        // Found a recoverable file
                        var recoveredFile = new RecoveredFileInfo
                        {
                            FileName = file.Name,
                            FileSize = (long)file.Length,
                            FileType = fileExtension,
                            RecoveryDate = DateTime.Now,
                            Status = "Found",
                            IsRealFile = true
                        };
                        
                        // Add file to list (no copying yet)
                        try
                        {
                            // Update status and info box for found file
                            this.Invoke(new Action(() => {
                                if (statusStrip1.Items.Count > 0)
                                {
                                    statusStrip1.Items[0].Text = $"Found: {file.Name} - Total: {recoveredFiles.Count + 1} files";
                                }
                                UpdateScanningInfo($"File Found\n\n📁 {currentScanningPath}\n📄 {file.Name}\nSize: {FormatFileSize((long)file.Length)}\nTotal Found: {recoveredFiles.Count + 1} files\n\nContinuing scan...");
                            }));
                            
                            Console.WriteLine($"Found: {file.Name} ({file.Length} bytes)");
                        }
                        catch (Exception ex)
                        {
                            recoveredFile.Status = "Error";
                            Console.WriteLine($"Error processing {file.Name}: {ex.Message}");
                            
                            // Update status and info box for error
                            this.Invoke(new Action(() => {
                                if (statusStrip1.Items.Count > 0)
                                {
                                    statusStrip1.Items[0].Text = $"Error processing: {file.Name}";
                                }
                                UpdateScanningInfo($"Processing Error\n\n📁 {currentScanningPath}\n📄 {file.Name}\nError: {ex.Message}\n\nContinuing scan...");
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
                    UpdateScanningInfo($"Directory Scan Complete\n\n📁 {currentScanningPath}\nFiles Scanned: {fileCount}\nRecoverable Found: {recoverableCount}\n\nScanning subdirectories...");
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
                        UpdateScanningInfo($"Current Scanning Directory\n\n📁 {currentScanningPath}\n\n🔍 Scanning subdirectory...");
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
                    UpdateScanningInfo($"Scanning Error\n\n📁 {currentScanningPath}\nError: {ex.Message}\n\nContinuing scan...");
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
                
                // Check if there are files in the list
                bool hasFiles = lstRecoveredFiles.Items.Count > 0;
                bool hasSelection = lstRecoveredFiles.SelectedItems.Count > 0;
                
                // Update start recovery button
                btnStartRecovery.Enabled = canStartRecovery;
                
                // Update stop recovery button
                btnStopRecovery.Enabled = isRecoveryRunning;
                
                // Update copy buttons
                btnCopyAll.Enabled = hasFiles && !isRecoveryRunning;
                btnCopySelected.Enabled = hasSelection && !isRecoveryRunning;
                btnClearList.Enabled = hasFiles && !isRecoveryRunning;
                
                Console.WriteLine($"Recovery buttons updated - Start: {canStartRecovery}, Stop: {isRecoveryRunning}, CopyAll: {btnCopyAll.Enabled}, CopySelected: {btnCopySelected.Enabled}, ClearList: {btnClearList.Enabled}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateRecoveryButtonStates error: {ex.Message}");
            }
        }

        private void UpdateSelectedDirectories()
        {
            try
            {
                selectedDirectories.Clear();
                
                foreach (TreeNode node in treeViewDirectories.Nodes)
                {
                    if (node.Checked)
                    {
                        selectedDirectories.Add(node.Text);
                    }
                    
                    // Check child nodes recursively
                    AddCheckedNodes(node, selectedDirectories);
                }
                
                Console.WriteLine($"Selected directories: {string.Join(", ", selectedDirectories)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateSelectedDirectories error: {ex.Message}");
            }
        }

        private void AddCheckedNodes(TreeNode parentNode, List<string> selectedDirs)
        {
            try
            {
                foreach (TreeNode childNode in parentNode.Nodes)
                {
                    if (childNode.Checked)
                    {
                        selectedDirs.Add(childNode.Text);
                    }
                    
                    // Recursively check child nodes
                    AddCheckedNodes(childNode, selectedDirs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddCheckedNodes error: {ex.Message}");
            }
        }

        private async Task LoadDirectoryTree()
        {
            try
            {
                if (connectedDevice == null)
                {
                    treeViewDirectories.Nodes.Clear();
                    return;
                }

                treeViewDirectories.Nodes.Clear();
                selectedDirectories.Clear();

                // Get root directory
                var rootDir = connectedDevice.GetDirectoryInfo("/");
                if (rootDir != null)
                {
                    // Create root node
                    var rootNode = new TreeNode("Device Root")
                    {
                        Tag = "/",
                        Checked = true // Check root by default
                    };
                    treeViewDirectories.Nodes.Add(rootNode);

                    // Load subdirectories
                    await LoadDirectoryNodes(rootDir, rootNode);
                    
                    // Expand root node
                    rootNode.Expand();
                    
                    // Add root to selected directories
                    selectedDirectories.Add("/");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadDirectoryTree error: {ex.Message}");
                MessageBox.Show($"Error loading directory tree: {ex.Message}", "Directory Tree Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadDirectoryNodes(MediaDirectoryInfo directory, TreeNode parentNode)
        {
            try
            {
                var subdirs = directory.EnumerateDirectories();
                int count = 0;
                
                foreach (var subdir in subdirs)
                {
                    if (count >= 20) // Limit to prevent UI freezing
                        break;
                        
                    var node = new TreeNode(subdir.Name)
                    {
                        Tag = subdir.FullName,
                        Checked = false // Don't check subdirectories by default
                    };
                    
                    parentNode.Nodes.Add(node);
                    
                    // Load subdirectories of this directory (limited depth)
                    try
                    {
                        var subSubdirs = subdir.EnumerateDirectories();
                        if (subSubdirs.Any())
                        {
                            // Add a placeholder node to indicate there are subdirectories
                            var placeholderNode = new TreeNode("...")
                            {
                                Tag = "placeholder",
                                Checked = false
                            };
                            node.Nodes.Add(placeholderNode);
                        }
                    }
                    catch
                    {
                        // Ignore errors when accessing subdirectories
                    }
                    
                    count++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadDirectoryNodes error: {ex.Message}");
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