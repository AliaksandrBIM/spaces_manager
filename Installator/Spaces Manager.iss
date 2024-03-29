; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
#define RawDllRoot "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\bin\Release R21\Eneca.SpacesManager.dll"

#define MyAppName GetStringFileInfo(RawDllRoot, "FileDescription")
#define MyAppId GetStringFileInfo(RawDllRoot, "ProductName")
#define MyAppVersion GetVersionNumbersString(RawDllRoot)
#define MyAppPublisher GetStringFileInfo(RawDllRoot, "CompanyName")
#define MyAppURL GetStringFileInfo(RawDllRoot, "AuthorRights")
#define MyAppVersion GetStringFileInfo(RawDllRoot, "FileVersion")

#define InstallerDir "C:\Users\Public\SoftwareDevelopment\Eneca\Updates\Eneca\SpacesManager\"
///#define InstallerDir "C:\Users\Public\SoftwareDevelopment\Eneca\revit-plugin-spaces-manager\Installator\"

#define DestinationDir "C:\ProgramData\Autodesk\Revit\Addins\" 

//��������� �������� ��� VersionUpdater.exe
#define Params  MyAppId + " " + MyAppVersion + " " +  InstallerDir + MyAppId + "Installer.exe"
//�������, ����������� ������ � ��� � update.xml
///#expr Exec("C:\Users\Public\SoftwareDevelopment\ENECA\windows-library-version-updater\IntermediateStarter\bin\Debug\IntermediateStarter.exe", Params)

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{B9407ADF-08DE-4B00-9CF8-57BBC53E9B14}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
CreateAppDir=no
InfoBeforeFile={#InstallerDir + "UpdateInfo.txt"}
; Uncomment the following line to run in non administrative install mode (install for current user only.)
OutputDir={#InstallerDir}
OutputBaseFilename={#MyAppId + "Installer"}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallFilesDir={autoappdata}\{#MyAppName}
SetupIconFile=SpacesManager.ico

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
//Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\Eneca.SpacesManager.addin"; DestDir: {#DestinationDir + "2019\"}; Flags: ignoreversion
Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\bin\Release R19\*"; Excludes: "*.BIN,*.pdb,*.dll.config"; DestDir: {#DestinationDir + "2019\Eneca\SpacesManager"}; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\Eneca.SpacesManager.addin"; DestDir: {#DestinationDir + "2020\"}; Flags: ignoreversion
Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\bin\Release R20\*"; Excludes: "*.BIN,*.pdb,*.dll.config"; DestDir: {#DestinationDir + "2020\Eneca\SpacesManager"}; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\Eneca.SpacesManager.addin"; DestDir: {#DestinationDir + "2021\"}; Flags: ignoreversion
Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\bin\Release R21\*"; Excludes: "*.BIN,*.pdb,*.dll.config"; DestDir: {#DestinationDir + "2021\Eneca\SpacesManager"}; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\Eneca.SpacesManager.addin"; DestDir: {#DestinationDir + "2022\"}; Flags: ignoreversion
Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\bin\Release R22\*"; Excludes: "*.BIN,*.pdb,*.dll.config"; DestDir: {#DestinationDir + "2022\Eneca\SpacesManager"}; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\Eneca.SpacesManager.addin"; DestDir: {#DestinationDir + "2023\"}; Flags: ignoreversion
Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\bin\Release R23\*"; Excludes: "*.BIN,*.pdb,*.dll.config"; DestDir: {#DestinationDir + "2023\Eneca\SpacesManager"}; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\Eneca.SpacesManager.addin"; DestDir: {#DestinationDir + "2024\"}; Flags: ignoreversion
Source: "C:\Users\Public\SoftwareDevelopment\ENECA\revit-plugin-spaces-manager\SpacesManager\bin\Release R24\*"; Excludes: "*.BIN,*.pdb,*.dll.config"; DestDir: {#DestinationDir + "2024\Eneca\SpacesManager"}; Flags: ignoreversion recursesubdirs createallsubdirs