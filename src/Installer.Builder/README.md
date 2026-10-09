# Installer.Builder —— 制作端

安装包制作助手的界面层。遵循 `winforms` 技能的分层与 Designer 契约。

## 目录结构

```
Builder/
├── Forms/          BuilderForm（外壳）+ AdvancedForm（高级选项对话框）
├── Views/          6 个 UserControl
│                   FolderStepView / InfoStepView / ShortcutStepView / OutputBar
│                   AdvancedLicenseView / AdvancedInstallView
├── Presenters/     BuilderPresenter —— 加载/保存/校验/打包（不引用 WinForms）
├── Models/         BuilderDocument —— 工程文档状态 + 输出路径推导
├── Services/       BuildService —— 包装 PackageBuilder，可注入替身
└── Common/         BuilderStrings（内置中英文案）/ StubLocator（自动找模板 stub）
```

## 与 `winforms` 技能校验脚本的两处有意偏离

`tools\check-structure.ps1` 会报 2 个错误。**这两处是刻意的**，理由如下：

### 1. `Data/` 缺失

脚本要求有 `Data/` 层。本项目的**数据访问在契约层**：
工程文件的读写是 `Installer.Core.Packaging.ProjectSerializer`，
`BuilderPresenter` 只是调用它。

为满足脚本而建一个空的 `Data/` 文件夹没有意义 —— 技能原文也写着
"用不到的可省，但不能没有结构"。

### 2. `Models/` 引用 `Installer.Abstractions.Model`

脚本认为 "Models 不应依赖任何其他层"。

但 `Installer.Abstractions` **就是契约层**，不是"另一层业务实现"：
它只包含清单模型、容器格式常量、步骤接口、平台接口。
`BuilderDocument` 包装 `InstallerProject` 属于**依赖倒置**，是正确方向。

真正的红线是技能 R6 那条：**`Models/` 和 `Services/` 里不允许出现
`using System.Windows.Forms;`** —— 这一条脚本报 `OK 业务层零 WinForms 依赖`，我们满足。

## Designer 契约

`check-designer.ps1` 通过（8 个文件，无问题）。三条自我约束：

1. 控件只在 `*.Designer.cs` 的 `InitializeComponent()` 里创建
2. 该函数内无 lambda / 循环 / 条件 / 局部变量 / 对象初始化器
3. 业务逻辑只在 `*.cs`

## AntdUI 2.4.3 的实测坑

本项目锁定 AntdUI **2.4.3**，技能正文按另一个版本写成。已实测的差异：

| 项 | 2.4.3 实际情况 |
|---|---|
| `Button.IconGap` | `float`，**字号倍数**（默认 0.25），不是像素 |
| `Label.IconGap` | `int`，**像素** |
| `Label.PrefixSvg` | 文本为空时**不绘制**，需要给个空格 |
| `Label.PrefixSvg` 尺寸 | 固定约 20px，**不跟随 `Font`** |
| `SvgDb` | 只有 35 个 `Ico*` 常量，**没有**官方图标集 → 手写 SVG 字符串 |
| `Button.ImageSvg` | 属性名是 `IconSvg` |
| `TMode` | 只有 `Light` / `Dark`，**没有 `Auto`** |
| `Modal.Config` | **有** `OkText`/`CancelText`/`OkType`/`Icon` |
| 禁用态 | `Enabled=false` 渲染成浅灰底浅灰字，几乎读不出 → 用 `Visible` 代替 |
| `TableLayoutPanel` | 调了 `SuspendLayout()` 就必须调 `ResumeLayout()`，否则子控件不显示 |

## 界面规格

见仓库根目录的 [`设计文档.md`](../../设计文档.md) 与 Calicat 里的
「安装包制作助手 · 界面改版原型 v2」。
