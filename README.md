# Windows Reboot Preventer
This application prevents the reboot of the Windows operating system.

## Why use this application?

Windows OS automatically reboots(restarts) after Windows Update.

There is an 'Active hours' setting to control reboots, but the active hours range is up to 18 hours.
It is not possible to set the active hours range to 24 hours.

And if there is no **active** logon user, there is no setting to suppress the restart after Windows Update.  
[(Microsoft Learn) Manage device restarts after updates](https://learn.microsoft.com/en-US/windows/deployment/update/waas-restart#delay-automatic-reboot)

The application prevents Windows reboots in a hackish way by displaying a '**Save As dialog**' in a hard-to-see location.

## Requirements

.Net Framework >= 4.7.2

**.Net Framework** is already installed in the following versions of Windows.

✔ Windows 11  
✔ Windows 10  
✔ Windows Server 2022  
✔ Windows Server 2019  

For more information, see the following link.  
[(Microsoft Learn) .NET Framework versions and dependencies](https://learn.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies)

## Usage

To prevent a reboot, run this exe file.

To exit the application, right-click on the icon in the task tray and select "Exit".


## License

MIT License

## Contact

Contact me on [GitHub Issues](https://github.com/sklab/windows-reboot-preventer/issues)
