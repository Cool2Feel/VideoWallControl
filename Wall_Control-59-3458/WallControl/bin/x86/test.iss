; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "�ҵĳ���"
#define MyAppVersion "v3.0.6"
#define MyAppPublisher "ktc"
#define MyAppExeName "Wall_Control.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
AppId={{E148D88C-9B8E-4E45-A549-2CB178978D1A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=F:\test
OutputBaseFilename=setup
SetupIconFile=F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\tv_screen.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Wall_Control.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\3458Help_ch.pdf"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\3458Help_en.pdf"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\59Help_ch.pdf"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\59Help_en.pdf"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\DevComponents.DotNetBar2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Help.pdf"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Interop.MSScriptControl.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\IrisSkin2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\LogFile.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\MatrixCode.mdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\my_log-file.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\setting.ini"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\skin.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Splicing Control Software.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Splicing Control Software.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\tv_screen.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Uninstall.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\unvell.ReoGrid.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\wall_contrl.db"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Wall_Control.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Wall_Control.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Wall_Control.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Wall_Control.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Wall_Control.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Wall_Control.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\Wall_Control.XmlSerializers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\WallControl.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\WallControl.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\WallControl.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\weather"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\zcm.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\1000_Application_Softwarewc\59_3458\WallControl\bin\x86\Release\�����ĵ�.chm"; DestDir: "{app}"; Flags: ignoreversion
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

