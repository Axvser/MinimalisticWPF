# MinimalisticWPF 🎨

[中文](#中文) | [English](#english)

<a name="中文"></a>
## 中文版

### 简介 📖
为WPF项目提供极简化的开发体验，通过C#代码直接实现前端动画、MVVM模式、路径运动等高级功能。支持源码生成器、独立过渡系统、主题切换等特性，大幅降低XAML学习成本。

[![GitHub](https://img.shields.io/badge/GitHub-Repository-blue?logo=github)](https://github.com/Axvser/MinimalisticWPF)  
[![NuGet](https://img.shields.io/nuget/v/MinimalisticWPF?color=green&logo=nuget)](https://www.nuget.org/packages/MinimalisticWPF/)

---

### 核心功能导航 🚀
1. **[过渡系统](#过渡系统)** - 属性动画与复合过渡  
2. **[MVVM增强](#mvvm增强)** - 源码生成器驱动的ViewModel  
3. **[路径运动](#路径运动)** - 可视化轨迹设计与控件运动  
4. **[粒子特效](#粒子特效)** - 可视化粒子发射器与消散边界
5. **[扩展工具](#扩展工具)** - 字符串处理/颜色管理/AOP 

### 完整文档 📚
[查看Wiki获取完整API参考](https://github.com/Axvser/MinimalisticWPF/wiki)

---

### 版本 📦
| 版本   | 类型 | 目标框架                   |
|--------|------|----------------------------|
| 4.x    | Limited Support Release  | .NET 5 / .NET Framework 4.6.2 |

### 更新日志 📜 

| 版本 | 亮点 | 更新时间 |
|---------|------------|--------------|
| [v4.5.3](https://github.com/Axvser/MinimalisticWPF/wiki/Changes-v4.5.3)  | 系统主题跟随 | 2025-03-27 |

---

### 快速开始 🚀

#### 1. 安装NuGet包
```bash
dotnet add package MinimalisticWPF --version 4.4.0-pre
```

#### 2. 基础过渡动画
```csharp
// 为控件添加背景色渐变动画
var grid = new Grid();
grid.Transition()
    .SetProperty(x => x.Background, Brushes.Red)
    .SetParams(p => p.Duration = 2)
    .Start();
```

#### 3. MVVM数据绑定
```csharp
// 自动生成属性与构造函数
[Observable]
private string _text = "Hello";
```

---

<a name="过渡系统"></a>
## 过渡系统 ⏳

### 核心特性
- **链式配置** - 流畅的API设计
- **复合动画** - 多属性并行/串行动画
- **性能可控** - 帧率/优先级/线程模式可调

### 代码示例
```csharp
// 创建复用动画模板
var template = Transition.Create<Grid>()
    .SetProperty(x => x.Width, 100)
    .SetProperty(x => x.Opacity, 0.5);

// 应用动画到多个控件
grid1.BeginTransition(template);
grid2.BeginTransition(template);
```

---

<a name="mvvm增强"></a>
## MVVM增强 🧩

### 特性亮点
- **源码生成器** - 自动生成属性/构造函数/依赖属性
- **主题切换** - 一键切换Light/Dark模式
- **悬停交互** - 内置鼠标悬停动画支持

### 示例：主题切换
```csharp
// 定义支持主题的属性
[Observable]
[Dark("#1E1E1E")]
[Light("White")]
private Brush _background;

// 切换全局主题
DynamicTheme.Apply(typeof(Light));
```

---

<a name="路径运动"></a>
## 路径运动 🛤️

拖拽设计器中的锚点，即可以指定路径控制元素移动 ！

![](https://s3.bmp.ovh/imgs/2025/03/25/1454ad276b2e2ad3.png)

---

<a name="粒子特效"></a>
## 粒子特效 ✨

用 Shape 作为粒子发射器 OR 粒子消散边界 , 你可轻松创建简单的粒子特效 !

![](https://s3.bmp.ovh/imgs/2025/03/25/90b1c4fc66056138.png)

---

<a name="扩展工具"></a>
## 扩展工具 🧰

| 模块               | 功能描述                     |
|--------------------|------------------------------|
| **StringValidator** | 链式字符串验证（正则/长度/格式） |
| **StringCatcher**   | 结构化文本提取（中英文/数字/层级） |
| **RGB**             | 颜色值转换与管理              |
| **AOP**             | 动态方法拦截与扩展            |

```csharp
// 示例：中文提取
var text = "Hello世界";
var result = StringCatcher.Chinese(text); // ["世界"]
```

---

### 完整文档 📚
[查看Wiki获取完整API参考](https://github.com/Axvser/MinimalisticWPF/wiki)

---
---
---

<a name="english"></a>  
## English Version  

### Introduction 📖  
A minimalist development experience for WPF projects, enabling advanced frontend animations, MVVM patterns, path-based motion, and particle effects through pure C# code. Features include source generators, an independent transition system, theme switching, and more—significantly reducing the learning curve for XAML.  

[![GitHub](https://img.shields.io/badge/GitHub-Repository-blue?logo=github)](https://github.com/Axvser/MinimalisticWPF)  
[![NuGet](https://img.shields.io/nuget/v/MinimalisticWPF?color=green&logo=nuget)](https://www.nuget.org/packages/MinimalisticWPF/)  

---

### Core Features Navigation 🚀  
1. **[Transition System](#transition-system)** - Property animations & composite transitions  
2. **[MVVM Enhancements](#mvvm-enhancements)** - Source generator-driven ViewModel  
3. **[Path Animation](#path-animation)** - Visual trajectory design & control motion  
4. **[Particle Effects](#particle-effects)** - Shape-based emitters & dissipation boundaries  
5. **[Utilities](#utilities)** - String processing/color management/AOP  

---

### Version 📦  
| Version | Type | Target Frameworks             |
|---------|------|-------------------------------|
| 4.x    | Limited Support Release  | .NET 5 / .NET Framework 4.6.2 |

### 📜 History

| Version | Highlights | Release Date |
|---------|------------|--------------|
| [v4.5.3](https://github.com/Axvser/MinimalisticWPF/wiki/Changes-v4.5.3)  | System topic following | 2025-03-27 |

---

### Quick Start 🚀  

#### 1. Install NuGet Package  
```bash  
dotnet add package MinimalisticWPF --version 4.4.0-pre  
```  

#### 2. Basic Transition Animation  
```csharp  
// Add background color transition animation  
var grid = new Grid();  
grid.Transition()  
    .SetProperty(x => x.Background, Brushes.Red)  
    .SetParams(p => p.Duration = 2)  
    .Start();  
```  

#### 3. MVVM Data Binding  
```csharp  
// Auto-generated properties and constructors  
[Observable]  
private string _text = "Hello";  
```  

---

<a name="transition-system"></a>  
## Transition System ⏳  

### Core Features  
- **Fluent API** - Chainable configuration  
- **Composite Animations** - Parallel/sequential multi-property animations  
- **Performance Control** - Adjustable frame rate/priority/threading modes  

### Code Example  
```csharp  
// Create reusable animation template  
var template = Transition.Create<Grid>()  
    .SetProperty(x => x.Width, 100)  
    .SetProperty(x => x.Opacity, 0.5);  

// Apply template to multiple controls  
grid1.BeginTransition(template);  
grid2.BeginTransition(template);  
```  

---

<a name="mvvm-enhancements"></a>  
## MVVM Enhancements 🧩  

### Key Highlights  
- **Source Generators** - Auto-generate properties/constructors/dependency properties  
- **Theme Switching** - One-click Light/Dark mode toggle  
- **Hover Interaction** - Built-in mouse hover animations  

### Example: Theme Switching  
```csharp  
// Define theme-aware property  
[Observable]  
[Dark("#1E1E1E")]  
[Light("White")]  
private Brush _background;  

// Apply global theme  
DynamicTheme.Apply(typeof(Light));  
```  

---

<a name="path-animation"></a>  
## Path Animation 🛤️  

Drag anchors in the designer to define motion paths for elements!  

![](https://s3.bmp.ovh/imgs/2025/03/25/1454ad276b2e2ad3.png)  

---

<a name="particle-effects"></a>  
## Particle Effects ✨  

Use Shapes as particle emitters OR dissipation boundaries to create stunning visual effects with ease!  

![](https://s3.bmp.ovh/imgs/2025/03/25/90b1c4fc66056138.png)  

---

<a name="utilities"></a>  
## Utilities 🧰  

| Module            | Functionality                  |  
|--------------------|---------------------------------|  
| **StringValidator**| Chainable validation (regex/length/format) |  
| **StringCatcher**  | Structured text extraction (CN/EN/digits/hierarchy) |  
| **RGB**            | Color conversion & management  |  
| **AOP**            | Dynamic method interception & extension |  

```csharp  
// Example: Chinese text extraction  
var text = "Hello世界";  
var result = StringCatcher.Chinese(text); // ["世界"]  
```  

---

### Full Documentation 📚  
[View Complete API Reference on Wiki](https://github.com/Axvser/MinimalisticWPF/wiki)