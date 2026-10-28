unit frmMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs,Registry, StdCtrls,Tlhelp32;

type
  TForm1 = class(TForm)
    Label1: TLabel;
    Button1: TButton;
    lbl2: TLabel;
    Label3: TLabel;
    lbl3: TLabel;
    Label5: TLabel;
    lbl35: TLabel;
    Label7: TLabel;
    lbl4: TLabel;
    Label9: TLabel;
    lbl45: TLabel;
    Label11: TLabel;
    lbl46: TLabel;
    Button2: TButton;
    Button3: TButton;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }

  end;

var
  Form1: TForm1;
  path:string;
  lppe:TProcessEntry32;
  found:boolean;
  handle:THandle;
  ProcessStr,ExeName:string;
  WinDir:pchar;
  const
  MySize=64000; {根据编译或压缩后的文件大小进行修改}
implementation

{$R *.dfm}
function CheckNetFrameWork(sVersion:string): Boolean;
var
  ff:boolean;
  sqlstr,DBServerName,DBName,DBID,DBPwd:string;
  reg:TRegistry;
begin
  Result := False;
  try
    Reg:= TRegistry.Create;
    try
      Reg.RootKey := HKEY_LOCAL_MACHINE ;
      if Reg.OpenKeyReadOnly('\Software\Microsoft\NET Framework Setup\NDP\'+sVersion) then
      begin
        Result := True;
        Reg.CloseKey;
      end
    finally
    Reg.Free;
  end;
  except on e:exception do
    ShowMessage(E.message);
  end;
end;

//释放EXE资源文件
function ExtractRes(ResType, ResName, ResNewName: string): boolean;
var
  Res: TResourceStream;
begin
  try
    Res := TResourceStream.Create(Hinstance, Resname, Pchar(ResType));
    try
      Res.SavetoFile(ResNewName);
      Result := true;
    finally
      Res.Free;
    end;
  except
    Result := false;
  end;
end;

procedure TForm1.Button1Click(Sender: TObject);
begin
if CheckNetFrameWork('v2') or CheckNetFrameWork('v2.0.50727') then
begin
  lbl2.Caption:= '.NET FrameWork 2.0已安装';
//ShowMessage('.NET FrameWork 4.0已安装');
end;
if CheckNetFrameWork('v3') or CheckNetFrameWork('v3.0') then
begin
  lbl3.Caption:= '.NET FrameWork 3.0已安装';
end;
if CheckNetFrameWork('v3') or CheckNetFrameWork('v3.5') then
begin
  lbl35.Caption:= '.NET FrameWork 3.5已安装';
end;
if CheckNetFrameWork('v4.0') or CheckNetFrameWork('v4.0') then
begin
  lbl4.Caption:= '.NET FrameWork 4.0已安装';
end;
if CheckNetFrameWork('v4') or CheckNetFrameWork('v4') then
begin
  lbl45.Caption:= '.NET FrameWork 4.5已安装';
end;
end;

procedure TForm1.Button2Click(Sender: TObject);
var
  strmSource,strmDest:TMemoryStream;
begin
  try
  
  //读exe
  strmSource:=TMemoryStream.Create;
  strmSource.loadfromfile('Stuep.exe');
  //拷贝到strmdest
  strmDest:=TMemoryStream.Create;
  strmDest.copyfrom(strmSource,strmSource.size);

  //合并CSharpZip.dll
  strmSource.clear;
  strmSource.loadfromfile('CSharpZip.dll');
  //拷贝到strmdest
  strmDest.seek(strmDest.size,soFromBeginning);
  strmDest.copyfrom(strmSource,strmSource.size);


  //合并DSkin.dll
  strmSource.clear;
  strmSource.loadfromfile('DSkin.dll');
  //拷贝到strmdest
  strmDest.seek(strmDest.size,soFromBeginning);
  strmDest.copyfrom(strmSource,strmSource.size);
  strmSource.clear;

  //合并insatll.resources
  strmSource.loadfromfile('insatll.resources');
  //拷贝到strmdest
  strmDest.seek(strmDest.size,soFromBeginning);
  strmDest.copyfrom(strmSource,strmSource.size);
  strmSource.free;

  //生成新的exe文件
  strmDest.SaveToFile('Build.exe');

  strmDest.free;
  finally
  end;
end;

procedure TForm1.Button3Click(Sender: TObject);
begin
ExtractRes('exefile','app','tmp.exe');//这里的APP要同刚才制作的文本里的APP同(资源名相同)
end;

end.
