[Setup]
AppName=Mirada Finanza Control Central
; Wir nutzen hier ein Makro {#AppVersion}, das wir von GitHub aus befüllen
AppVersion={#AppVersion}
DefaultDirName={autopf}\Mirada-Finanza-Control-Central
DefaultGroupName=Mirada Finanza Control Central
OutputDir=.\InstallerOutput
OutputBaseFilename=Mirada-Finanza-Control-Central
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "mirada-finanza-control-central\bin\Release\mirada-finanza-control-central.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "mirada-finanza-control-central\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "mirada-finanza-control-central\bin\Release\*.config"; DestDir: "{app}"; Flags: ignoreversion

[Dirs]
Name: "{userappdata}\Mirada-Finanza-Control-Central"

[Icons]
Name: "{group}\Mirada Finanza Control Central"; Filename: "{app}\mirada-finanza-control-central.exe"
; Hier wurde der Task 'desktopicon' verknüpft:
Name: "{commondesktop}\Mirada Finanza Control Central"; Filename: "{app}\mirada-finanza-control-central.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\mirada-finanza-control-central.exe"; Description: "Mirada Finanza Control Central jetzt starten"; Flags: nowait postinstall skipifsilent