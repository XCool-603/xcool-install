# xcool-install

**一个把整个程序目录打成单个 `.exe` 的 Windows 安装包制作工具。**
双语文案、按清单精确卸载、出错自动回滚。

> 这是对一个 2013 年的 WinForms + Delphi 安装包制作项目（`Installation` / `UnInstall` / `InstalltionEdit`）
> 的重写。旧项目仍在仓库里作为对照，见 [遗留项目](#遗留项目)。

---

## 它解决什么问题

旧工具能用，但有几个必须绕开的坑。下面是**实际代码位置**，不是泛泛而谈：

| 旧项目的问题 | 位置 |
|---|---|
| 用**按钮文字**当状态机，界面一翻译就失效 | `Installation\FrmInstallation.cs:173,218,78`、`UnInstall\FrmUnInstall.cs:117` |
| 解压时不校验路径，可被 `..\..\` 写到安装目录外 | `Installation\FrmInstallation.cs:725` |
| 卸载时**递归删光安装目录**，用户数据一起陪葬 | `UnInstall\FrmUnInstall.cs:232-257` |
| COM 注册先把 DLL 复制到 `System32`，参数还不加引号 | `Installation\FrmInstallation.cs:486-492` |
| 异常被空 `catch` 吞掉后照样打印"注册成功" | 同上 |
| 卸载键指向根本不存在的 `UnInstallation.exe` | `Installation\FrmInstallation.cs:587` |
| 制作机路径被打进发布包 | 实测真实包里的 `InstallSetupPath` / `InstallationExePath` |
| zip 条目名按当时区域 ANSI 代码页编码，跨语言乱码 | `CsharpZipLib\Zip\ZipConstants.cs:434` |

这些都在 [`项目分析报告.md`](docs/项目分析报告.md) 里逐条列出了证据。

---

## 特性

### 打包端
- **单文件交付** —— 产物就是一个 `.exe`，没有附属目录、没有 `install.resources`
- **可重现构建** —— 同样的输入 + 同样的时间戳 = 同样的字节（e2e 里断言了两次打包哈希相同）
- **自校验容器** —— stub 哈希 + payload 哈希 + CRC32，回读验证失败就拒绝出包
- **图标补丁** —— 直接改 PE 资源目录，不依赖 `UpdateResource`
- **命令行可进 CI** —— `pack` / `verify` / `inspect` / `extract` / `diff`

### 安装端
- **十一步管线** —— 预检 → 结束占用进程 → 解压 → COM 注册 → 快捷方式 → 自启动 → 清理残留卸载项 → 标准卸载键 → 部署卸载器 → 写安装记录 → 装完运行
- **写前日志 + 整机回滚** —— 任何一步失败都回到安装前状态，不是"尽力而为"
- **按清单卸载** —— 只删自己装的文件，用户放进安装目录的东西保留
- **修复安装** —— `--repair` 按哈希逐个比对，只补缺失/损坏的，幂等
- **重复安装不留垃圾** —— 自动清理"同目录、不同 productCode"的残留卸载项
- **降级卸载** —— 安装记录丢了也能按卸载键定位并清理干净
- **COM 注册** —— 不复制到 `System32`、参数加引号、按目标位数选对 `regsvr32`、失败必须报错

### 界面
- **向导中英双语是默认行为** —— 内置 40+ 条中英文案，清单可逐条覆盖
- **制作端一屏三步** —— 只有 3 项必填：文件夹、产品名称、版本号
- **AntdUI 界面** —— 浅色/深色、主题色可配

---

## 技术栈

**运行环境**：.NET Framework **4.6.1**。产物 exe 是普通的 .NET Framework 程序 ——
目标机器有 4.6.1 就能跑（Win7 SP1 起可随系统或单独安装），**不需要额外运行时**，
也不需要把运行时打进包里。

| 层 | 选型 | 版本 | 说明 |
|---|---|---|---|
| 目标框架 | .NET Framework | **4.6.1** | 产品全部 6 个项目 |
| 测试目标框架 | .NET Framework | 4.6.2 | 唯一的 xunit 运行器最低只支持到 4.6.2 |
| 语言 | C# | 7.3 | 兼容 VS2017 |
| 界面 | WinForms + **AntdUI** | 2.4.3 | 自绘控件、圆角卡片、SVG 图标 |
| JSON | **Newtonsoft.Json** | 13.0.3 | 工程文件 / 清单 / 安装记录 |
| 单元测试 | **xUnit** | 2.9.3 | + Microsoft.NET.Test.Sdk 17.14.1 |
| 构建 | .NET SDK | 10.0.400-preview | SDK 风格 csproj，`dotnet build` 即可 |

**没有用任何第三方安装框架。** 下面这些全是自己实现的：

| 能力 | 实现方式 |
|---|---|
| 单文件容器 | 自定义 footer 布局（stub + payload + manifest） |
| 图标补丁 | 自己解析并重建 PE 资源目录（`PeIconPatcher`） |
| 事务回滚 | 写前日志（`InstallJournal`）+ 逆序 undo |
| 快捷方式 | `IShellLink` COM 互操作 |
| COM 注册 | 调 `regsvr32`，按目标位数选对版本 |
| 路径防护 | `SafePath` 统一把关，拒绝 `..` 逃逸 |

为什么不用现成框架：要的「单文件 + 可重现 + 按清单精确卸载」这几条，
现成框架要么给不了，要么得写同样多的胶水代码。

---

## 与现有安装包方案的关系

安装包是个成熟领域，下面这些方案都比本项目久经考验。
列出来是为了说清**什么场景该用哪个** —— 本项目不是要取代谁。

| 方案 | 形态 | 强项 | 代价 |
|---|---|---|---|
| **NSIS** | 脚本 + 小体积 stub | 体积极小、插件生态大、脚本灵活 | 自有脚本语言；界面要自己搭；无事务回滚 |
| **Inno Setup** | Pascal 脚本 | 文档好、上手快、向导开箱即用 | 脚本语言；卸载粒度较粗 |
| **WiX / MSI** | 声明式 XML → MSI | 企业级：组策略分发、修复、回滚、日志 | 学习曲线陡；ICE 校验与升级规则绕 |
| **InstallShield / Advanced Installer** | 商业 GUI | 功能最全、可视化 | 收费；工程文件不透明 |
| **MSIX** | 容器化 | 卸载干净、现代 | 需 Win10 1809+，且**必须签名** |
| **7-Zip / WinRAR 自解压** | SFX | 最简单 | 无卸载集成、无回滚、无快捷方式/注册表 |
| **本项目** | 单文件 exe + JSON 清单 | 单文件、可重现构建、按清单精确卸载、写前日志回滚、中英双语 | 只面向 Windows + .NET Framework；没有生态 |

**什么时候用本项目**

- 交付的是**中文 WinForms / .NET 桌面程序**，希望「一个 exe 发出去就能装」
- 需要**可重现构建**（同输入 → 同字节），便于审计或归档
- 需要**精确卸载**：只删自己装的文件，用户放进安装目录的数据保留
- 团队不想为发一个安装包去学 NSIS 脚本或 WiX XML

**什么时候该用别人**

- 要企业组策略分发、补丁升级、多语言 MSI 特性 → **WiX / MSI**
- 要极小体积或很复杂的安装逻辑 → **NSIS / Inno Setup**
- 要 MSIX 那种容器化沙箱 → **MSIX**（但要接受强制签名）

> 注：Windows 安装包本身就与平台绑定，上述方案**没有一个跨平台** ——
> 这不是本项目的短板，是这类工具的共性。

---

## 快速上手

### 用现成的制作工具

```
制作工具\安装包制作助手.exe
```

> 该目录不在仓库里（是构建产物）。用 `tools\stage-tool.ps1` 生成，或见下面的"从源码构建"。

界面是**一屏三步的卡片布局**：

1. **拖文件夹进虚线框** —— 选好之后卡片会显示路径、文件数、总大小
2. **填名称和版本** —— 产品名**自动取文件夹名**；中英文名可以只填一个
3. **勾快捷方式** —— 点右下角「生成安装包」

模板 stub、输出路径、入口程序、安装目录、快捷方式名**全部自动推导**，用户看不到也不需要知道。
许可协议、安装范围、图标、COM 注册这些收在「高级选项」里。

顶部有「打开 / 保存」可以存取 `.wmpkg.json` 工程文件；
制作端还会**自动记住上次的工程**（含 productCode），下次打开接着用。

### 从源码构建

需要 .NET SDK（能编译 `net461`）与 .NET Framework 4.6.1 Targeting Pack。

```powershell
dotnet build Installer.sln -c Release
powershell -ExecutionPolicy Bypass -File tools\e2e.ps1
```

### 命令行打包

```powershell
installer-cli pack --project app.wmpkg.json --source .\publish --stub Installer.Runtime.exe --out Setup.exe
installer-cli verify Setup.exe
installer-cli inspect Setup.exe
```

工程文件就是可读、可 diff 的 JSON：

```jsonc
{
  "schemaVersion": 2,
  "productCode": "{A4402741-6A0D-420B-93FC-2E1B8059E5E8}",
  "product": {
    "name": { "zh-Hans": "汽车小镇", "en": "Auto Town" },
    "version": "2.4.1"
  },
  "defaultInstallDir": "%LOCALAPPDATA%\\Programs\\AutoTown",
  "license": { "required": true, "text": { "zh-Hans": "…", "en": "…" } }
}
```

---

## 项目结构

```
Installer.sln
├── src/
│   ├── Installer.Abstractions/   契约层：清单模型、容器格式、步骤接口、平台接口
│   ├── Installer.Core/           实现层：打包、安装引擎、卸载引擎、PE 补丁、路径防护
│   ├── Installer.UI/             向导界面（AntdUI）
│   ├── Installer.Builder/        制作端界面（AntdUI）
│   ├── Installer.Cli/            命令行
│   └── Installer.Runtime/        安装端 stub（自包含单文件）
├── tests/Installer.Core.Tests/   198 项单元测试
├── tools/                        构建与验收脚本（**索引见 tools/README.md**）
└── docs/界面截图/                 界面渲染图
```

**分层规则**：`Abstractions ← Core ← {Cli, UI, Builder, Runtime}`，只允许单向依赖。
`Models/` 与 `Presenters/` 里没有 `using System.Windows.Forms;`，所以业务逻辑都能单测。

---

## 容器格式

```
[ PE stub ][ payload.zip ][ manifest.json ][ footer 128B ]
```

footer 放在**文件末尾**，PE 加载器会忽略尾部多余数据 —— 所以打包只需要"追加"，
不用 `UpdateResource`、不用回退 seek。

| 偏移 | 字段 |
|---|---|
| 0 | 魔数 `WMINS2\0\0` |
| 8 | 格式版本 |
| 16 / 24 | payload 偏移 / 长度 |
| 32 / 40 | manifest 偏移 / 长度 |
| 48 | payload SHA256 |
| 80 | **stub SHA256**（只哈希 payload 会漏掉代码篡改） |
| 112 | footer CRC32 |

规格的单一事实来源是 `Installer.Abstractions\Packaging\ContainerFormatSpec.cs`。

---

## 验收

| 项 | 结果 |
|---|---|
| 全解决方案构建（Debug / Release） | 0 警告 |
| 单元测试 | 198 / 198 |
| 端到端断言 | 169 / 169 |
| 界面校验 | 50 / 50 |
| 真实系统残留 | 零（e2e 有专门的回归护栏） |

端到端覆盖：可重现构建、容器篡改检测、Zip Slip 防护、完整安装/卸载、
向导与制作端渲染、修复安装、v1 旧包迁移、指定安装位置、重复安装去重。

---

## 遗留项目

`legacy/Installation/` `legacy/UnInstall/` `legacy/InstalltionEdit/` `legacy/CsharpZipLib/` `legacy/CheckDotNet/` `legacy/DoNetFramework/`
是**旧项目原样保留**，只作对照，不参与构建。分析见：

- [`项目分析报告.md`](docs/项目分析报告.md) —— 逐条列出旧代码的问题与证据
- [`打包逻辑对照.md`](docs/打包逻辑对照.md) —— 新旧打包流程逐步对照
- [`重构方案.md`](docs/重构方案.md) —— 重构设计
- [`重构说明.md`](docs/重构说明.md) —— 已落地的实现与踩过的坑
- [`设计文档.md`](docs/设计文档.md) —— **当前设计的完整说明（改设计看这份）**

---

## 界面

![制作端](docs/界面截图/制作端/主界面.png)

![安装向导](docs/界面截图/安装向导-中文/Path.png)

---

## 状态

已完成：打包 / 安装 / 卸载 / 向导 / 制作端 / COM 注册 / 修复安装 / v1 迁移。

未做：免注册 COM（明确报错，不假装成功）、标准静默参数 `/S` `/D=`、
外置 payload（差分更新）、制作端"最近使用"列表。
