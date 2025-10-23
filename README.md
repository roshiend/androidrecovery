# Android File Recovery

A C# Windows Forms application for recovering deleted files from Android devices.

## Features

- **Device Detection**: Automatically detect connected Android devices
- **File Recovery**: Recover deleted images and videos from Android devices
- **File Preview**: Preview recovered files before saving
- **Multiple File Types**: Support for images (JPG, PNG, GIF, etc.) and videos (MP4, AVI, MOV, etc.)
- **User-Friendly Interface**: Clean and intuitive Windows Forms GUI

## Requirements

- .NET 9.0 or later
- Windows 10/11
- Android device with USB debugging enabled

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/yourusername/AndroidFileRecovery.git
   cd AndroidFileRecovery
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the application:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

## Usage

1. **Connect Android Device**: Connect your Android device via USB
2. **Enable USB Debugging**: On your Android device, enable Developer Options and USB Debugging
3. **Launch Application**: Run the Android File Recovery application
4. **Detect Device**: Click "Refresh Devices" to detect your connected device
5. **Connect Device**: Select your device and click "Connect Device"
6. **Start Recovery**: Click "Start Recovery" to begin scanning for deleted files
7. **Preview Files**: Select files from the list to preview them
8. **Recover Files**: Right-click on files to recover them to your computer

## Project Structure

```
AndroidFileRecovery/
├── Form1.cs                 # Main form code
├── Form1.Designer.cs        # UI designer code
├── FileRecoveryEngine.cs    # File recovery logic
├── RecoveryLogger.cs        # Logging functionality
├── Program.cs              # Application entry point
├── AndroidFileRecovery.csproj # Project file
└── README.md               # This file
```

## Dependencies

- **MediaDevices**: For MTP (Media Transfer Protocol) communication with Android devices
- **System.Windows.Forms**: For the Windows Forms GUI
- **System.Drawing**: For image processing and thumbnails

## Development

This project is built with:
- **C#** - Programming language
- **.NET 9.0** - Framework
- **Windows Forms** - GUI framework
- **Visual Studio** - Development environment

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Disclaimer

This software is for educational and personal use only. Always ensure you have permission to recover files from devices you don't own. The authors are not responsible for any misuse of this software.

## Support

If you encounter any issues or have questions, please open an issue on GitHub.

## Roadmap

- [ ] Add support for more file types
- [ ] Implement file carving for truly deleted files
- [ ] Add batch recovery operations
- [ ] Improve device compatibility
- [ ] Add recovery statistics and reporting