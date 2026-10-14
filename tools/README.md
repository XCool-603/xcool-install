# tools/ —— 构建、验收与诊断脚本

这个目录放的是**不参与产品构建**的辅助脚本。产品本身只有 `Installer.sln` 那 7 个项目。

统一入口是 [`ci.ps1`](#cipS1)：本地和 CI 跑同一份，避免"CI 上绿、本地红"。

> ⚠️ **本机只有 Windows PowerShell 5.1**（没有 pwsh / PowerShell 7）。
> 它会把**无 BOM 的 UTF-8 `.ps1` 按 ANSI 读**，中文直接变乱码、甚至解析失败。
> 编辑器在写回文件时还会吃掉首行的 `#`。
> 改完脚本**务必跑 `check-scripts.ps1`** —— 这两个坑都造成过真实事故（见下）。

---

## 验收类

### `ci.ps1`

统一 CI 入口：构建（Debug + Release）→ 单元测试 → 端到端 → 界面校验。

```powershell
powershell -ExecutionPolicy Bypass -File tools\ci.ps1
powershell -ExecutionPolicy Bypass -File tools\ci.ps1 -Only build,test
```

**每一步做三重断言**：退出码 == 0、成功摘要行命中、项数 ≥ 下限。

只看退出码是不够的 —— `e2e.ps1` 曾经因为文件头被吞而**解析失败、根本没执行**，
但输出里没有 `[FAIL]`，只看退出码的 CI 会变绿。

日志落在 `artifacts/ci-logs/`（已 gitignore）。

### `e2e.ps1`

端到端验收：打包 → 校验 → 查看 → 隔离运行安装 → 断言逐字节一致 → 可重现构建 →
反解/差异 → 篡改检测 → Zip Slip 防护 → 完整安装/卸载 → 修复安装 → v1 迁移。

全程沙箱，不碰真实系统（第 13 节有专门的回归护栏）。

```powershell
powershell -ExecutionPolicy Bypass -File tools\e2e.ps1
powershell -ExecutionPolicy Bypass -File tools\e2e.ps1 -KeepWorkDir   # 保留工作目录排查
```

### `verify-ui.ps1`

界面与设计稿的自动比对：主窗口结构不变量、与设计稿逐点比色、
向导 8 页与设计稿的一致度、文案逐字比对、图标定义、事件订阅完整性。

需要先构建 Release（它会去 `src\Installer.Builder\bin\Release\` 拿 exe）。

**跳过不等于通过**：设计稿或渲染图缺失时会记一次 `[跳过]`，最后以非零退出。

### `check-scripts.ps1`

校验 `tools/` 下所有 `.ps1` 的**文件头完整性**。改完脚本必跑。

```powershell
powershell -ExecutionPolicy Bypass -File tools\check-scripts.ps1
powershell -ExecutionPolicy Bypass -File tools\check-scripts.ps1 -Fix   # 自动补 BOM / 首行井号
```

检查五项：UTF-8 BOM、首行是注释、块注释起止符配对、可被解析器无错解析、`param` 之前无语句。

**它防的是两类真实事故**：

| 事故 | 后果 |
|---|---|
| `e2e.ps1` 的 `#Requires` 与 `<#` 两行被吞 | 注释块未闭合 → 解析失败、**根本没执行**，但外层只看到"没有 FAIL"而误判通过 |
| `verify-ui.ps1` 首行变成裸命令 | `param()` 被当成命令调用 → **参数全部失效** → 9 条设计稿颜色断言被静默跳过，却照样报"全部通过" |

> PowerShell 的块注释**不嵌套**：注释里写一个结束符就会提前闭合。
> 所以别在块注释里写那两个字符的字面量。

---

## 打包与产物类

### `stage-tool.ps1`

构建并把制作端组装成可直接双击的 `制作工具\` 目录。

```powershell
powershell -ExecutionPolicy Bypass -File tools\stage-tool.ps1
```

产物里 `安装包制作助手.exe` 是中文名（对 .NET Framework 没问题），
还包含 stub 模板与 5 个依赖 DLL。

### `check-package.ps1`

判断一个安装包里固化的逻辑**是不是最新的**。

```powershell
powershell -ExecutionPolicy Bypass -File tools\check-package.ps1 -Package D:\out\Setup.exe
```

**安装包在打包那一刻就把安装逻辑固化了**，改源码不会改变已经生成的 Setup.exe。
这个脚本在包里搜修复引入的标识符，判断它是不是修复前打的包。

### `make-v1-fixture.ps1`

生成 v1 旧包的测试夹具（`install.resources`）。e2e 需要它，缺失时会自动调用。

---

## 诊断类

### `check-window-icon.py`

**直接读运行中窗口的图标**，用来判断"换了图标但任务栏还是旧的"到底是代码问题还是系统缓存。

```powershell
python tools\check-window-icon.py "路径\某程序.exe"
```

启动目标 exe → 枚举它的窗口 → 读 `WM_GETICON` / `GetClassLongPtr` →
把 HICON 取成像素算 SHA-256，与 `ExtractIconEx` 取出的 exe 资源图标对比。

**两个哈希一致 = 代码没问题，是系统缓存**（Explorer 对同一路径的图标有缓存，
`ie4uinit -show` + 重启 Explorer 可清）。

> 为什么用 Python：本机 `Add-Type` 是坏的（`cannot seek in file`），
> 没法在 PowerShell 里现场编译 C# 做 P/Invoke。

---

## 图标生成

两个脚本产出的都是**多尺寸 `.ico`**（16/24/32/48/64/128/256，≤128 用 BMP、256 用 PNG）。
项目当前的 `app.ico` 是 7 帧、168,977 B，由下面第二个脚本生成。

### 该用哪个

| 场景 | 用哪个 |
|---|---|
| 已经有设计好的图片（PNG / JPG），要做成图标 | **`make-icon-from-image.py`** |
| 没有素材，要一个简洁的矢量风格图标 | `make-icon.ps1` |

### `make-icon-from-image.py`

从图片生成，带**去背景 + 裁边 + 预乘缩放**。

```powershell
python tools\make-icon-from-image.py --src setup.jpg --out src\Installer.Runtime\app.ico --threshold 90
python tools\make-icon-from-image.py --src setup.jpg --out x.ico --report   # 只出统计，调容差用
```

四步：四角洪水填充去背景（不是"全图白色→透明"，否则会把图标内部的白色也挖空）→
按 **alpha 通道**算包围盒裁边补方 → **预乘 alpha** 再缩放（否则透明像素的 RGB 会混进边缘形成白边）→
逐尺寸重采样出帧。

调容差时用 `--report` 看"背景占比"和"物体包围盒"：容差太大包围盒会突然缩小，说明把图形也吃掉了。

### `make-icon.ps1`

按品牌色 `#1677FF` 与主界面同一个盾牌图形，**逐尺寸矢量重绘**（不是把小图放大）。

```powershell
powershell -ExecutionPolicy Bypass -File tools\make-icon.ps1 -AlsoBuilder
```

改完之后记得把 `src\Installer.Runtime\app.ico` 同步一份到 `src\Installer.Builder\app.ico`
（`-AlsoBuilder` 会自动做）。

### 为什么必须多尺寸

`app_install.ico` 原先只有 **64×64 一帧**。Windows 在「大图标 / 超大图标」视图下找不到
256×256 帧，会退回显示**通用图标** —— 看起来完全就是"图标没生效"。

---

## 依赖关系

```
check-scripts.ps1 ──┐
                    ├──> ci.ps1 ──> GitHub Actions / Gitee 流水线
e2e.ps1 ────────────┤
verify-ui.ps1 ──────┤
                    └── 都需要先 dotnet build
stage-tool.ps1 ──> 制作工具\ ──> check-package.ps1
make-icon-from-image.py ──> src\*\app.ico ──> 构建时嵌入 exe
check-window-icon.py ── 独立诊断，随时可用
```
