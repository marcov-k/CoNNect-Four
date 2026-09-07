#define GameName "CoNNect-Four"
#define Publisher "FlyingArthur"
#define ExecutableName "CoNNect-Four.exe"

[Setup]
AppId={{395caaeb-6fd2-41f6-b23f-0b9748d7049a}}
AppName={#GameName}
AppVersion={#GameVersion}
AppPublisher={#Publisher}

DefaultDirName={autopf}\{#GameName}
DefaultGroupName={#GameName}

OutputDir=output
OutputBaseFilename=CoNNect-Four-v{#GameVersion}

ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

PrivilegesRequired=admin

Compression=lzma
SolidCompression=yes

WizardStyle=modern

UninstallDisplayName={#GameName}
UninstallDisplayIcon={app}\{#ExecutableName}

[Files]
Source: "{#BuildDir}\{#ExecutableName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#BuildDir}\UnityPlayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#BuildDir}\MonoBleedingEdge\*"; DestDir: "{app}\MonoBleedingEdge"; Flags: recursesubdirs createallsubdirs ignoreversion
Source: "{#BuildDir}\UnityCrashHandler64.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#BuildDir}\CoNNect-Four_Data\*"; DestDir: "{app}\CoNNect-Four_Data"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{autoprograms}\{#GameName}"; Filename: "{app}\{#ExecutableName}"; WorkingDir: "{app}"
Name: "{autodesktop}\{#GameName}"; Filename: "{app}\{#ExecutableName}"; WorkingDir: "{app}"

[Run]
Filename: "{app}\{#ExecutableName}"; WorkingDir: "{app}"; Description: "Launch {#GameName}"; Flags: nowait postinstall skipifsilent
