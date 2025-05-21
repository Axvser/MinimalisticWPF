# MinimalisticWPF

## English Documentation 📚  
[👉 Wiki](https://github.com/Axvser/MinimalisticWPF/wiki)

# 简介 📖
为WPF项目提供极简化的开发体验，以C#实现过渡、VM通信、MonoBehaviour等功能.

[![GitHub](https://img.shields.io/badge/GitHub-Repository-blue?logo=github)](https://github.com/Axvser/MinimalisticWPF)  
[![NuGet](https://img.shields.io/nuget/v/MinimalisticWPF?color=green&logo=nuget)](https://www.nuget.org/packages/MinimalisticWPF/)

> 自 V5.0.0 开始，MinimalisticWPF 正式进入稳定期

  - 已有 API 不再变更
  - 阶段性完成了基于弱引用的内存使用率优化
  - 下一阶段
    - 对 TransitionSystem 采取更多可能的性能优化
    - 对 DynamicTheme 采取更多可能的性能优化
    - 增加一些 InterpolationHandler 封装 ，以支持弹簧缓冲 、正弦波等过渡效果支持

---

# 核心功能导航 🚀
1. **[过渡](#过渡系统)** - 使用 `Fluent API` 构建针对`属性`而非`依赖属性`的插值过渡 
   - 不依赖Storyboard
   - 生命周期支持
   - 串行/并行支持
   - 线程安全的属性更新
   - 更新操作的优先级可调
2. **[源生成器](#源生成器)** - 增量生成器驱动类功能的快速扩展
   - 2.1. **[View 增强](#v增强)** - 声明 `Attribute` 以实现 `主题切换` `悬停交互` `热键组件` 效果
     - 基于已有/自定义依赖属性进行功能扩展
     - 视觉效果细化为对应的依赖属性,显著减少Trigger使用
     - 扩展为HotKey组件,它可在运行时自动注册/注销全局快捷方式
   - 2.2. **[ViewModel 增强](#vm增强)** - 声明 `Attribute` 以实现 `属性生成` `消息流` 效果
     - 隐去ViewModel接口实现
     - 隐去Model定义,直接从ViewModel内字段生成对应属性
     - 可在ViewModel间使用消息流批量发送通知
   - 2.3. **[View & ViewModel 通用增强](#v&vm增强)** - 声明 `Attribute` 以实现 `帧更新` `AOP` `构造器` 效果
     - 允许实例按照一定频率更新,类似于你在一些游戏引擎中所做的事情
     - 基于反向生成接口实现代理,允许在运行时拦截方法调用
     - 使用2.1/2.2中的功能后,类已被识别为`增强版`,其构造函数将自动生成且仍支持扩展
3. **[快捷方式](#快捷方式)** - 优雅实现全局快捷方式/局部快捷方式
   - 3.1. **[全局快捷方式](#全局快捷方式)** - 允许注册/注销全局快捷方式 
   - 3.2. **[局部快捷方式](#局部快捷方式)** - 允许对指定控件注册/注销局部快捷方式

# 完整功能 📚
[👉 Wiki](https://github.com/Axvser/MinimalisticWPF/wiki)

( 由于只有作者一个人在开发,Wiki需要逐步补全,有问题发邮箱 : `Axvser@outlook.com` )
   
---

# 快速开始 🚀

这里提供了一些简单的代码片段来帮助你快速上手MinimalisticWPF,如果你感兴趣,可以在Wiki看到详尽的文档。

### 过渡系统 🌈

```csharp
// 为控件添加背景色渐变动画
var grid = new Grid();
grid.Transition()
    .SetProperty(x => x.Background, Brushes.Red)
    .SetParams(p => p.Duration = 2)
    .Start();
```

---

### 源生成器 🔧
#### V增强
```csharp
    [Theme(nameof(Background), typeof(Dark), ["#1e1e1e"])] // 切换至暗色主题时,为属性加载过渡动画
    [Theme(nameof(Background), typeof(Light), ["White"])] // 切换至明亮主题时,为属性加载过渡动画
    [Hover(nameof(Background))] // 鼠标悬停时,为属性加载过渡动画
    [HotKeyComponent] // 该类为热键组件,可自动化热键的注册和注销
    public partial class MainWindow : Window
    {
        [Constructor] // 此函数将在无参构造函中执行
        private void SetDefaultValue()
        {

        }
        [Constructor] // 此函数将在带参构造函中执行
        private void SetDefaultValue(object value)
        {

        }
        // … 可添加更多构造函数,这取决于自定义函数的形参列表

        // … 源生成器会产生类似于DarkHoveredBackground的依赖属性,它表示暗色主题下,鼠标悬停在控件上时,控件的背景色应该表现出的颜色
        // … 源生成器会产生类似于DarkNoHoveredBackground的依赖属性,它表示暗色主题下,鼠标没有悬停在控件上时,控件的背景色应该表现出的颜色
        // … 当然,仅标记Theme特性的话,是不会有Dark前缀的;反之则不会有Hovered字样

        // … 此外,源生成器还会生成一些生命周期函数,这里是两个主题切换的生命周期函数
        // … 输入partial关键字,选择并按下Tab键,将会自动生成函数体
        partial void OnThemeChanging(Type? oldTheme, Type newTheme)
        {
            
        }
        partial void OnThemeChanged(Type? oldTheme, Type newTheme)
        {
            
        }
    }
```
```csharp
    public partial class App : Application
    {
        public App()
        {
            DynamicTheme.FollowSystem(typeof(Dark)); // 主题跟随系统,如果系统主题获取失败,则使用Dark主题
        }
    }
```

---

#### VM增强
  - 标记[Observable]即可从字段生成属性
  - 标记[SubscribeMessageFlows]即可订阅消息流

```csharp
    internal partial class Class1
    {
        public Class2 class2 = new();

        [Observable]
        private string name = "default";

        // 当Name更新后,发送[ Class1NameChanged ] 事件流
        // 可以标记为async
        async partial void OnNameChanged(string oldValue, string newValue)
        {
            await Task.Delay(2000);
            SendMessageFlow("Class1NameChanged", oldValue, newValue);
        }
    }
```
```csharp
    [SubscribeMessageFlows("Class1NameChanged")] // 订阅事件流
    internal partial class Class2
    {
        // 任何地方发送[ Class1NameChanged ]时,此函数被调用
        private partial void FlowClass1NameChanged(object sender, MessageFlowArgs e)
        {
            MessageBox.Show($"Class1NameChanged: {e.Messages[0]} -> {e.Messages[1]}");
        }
        // … 订阅多个事件流的话,这里就会自动产生更多partial函数,一个函数对应一个事件流的处理
    }
```

  - 在这个示例中,如果Class1的Name属性被更新,那么Class2会收到Class1NameChanged事件流,并在FlowClass1NameChanged函数中处理它.
  - 具体表现为Class1的Name属性发生更新后,Class2实例会弹出一个消息框,显示Class1的Name属性的旧值和新值.

---

#### V&VM增强
这里是一些通用的增强功能,它们可以在View和ViewModel中使用.
```csharp
    [MonoBehaviour(1000)] // 每隔1000ms更新一次
    public partial class MainWindow : Window
    {
        partial void Awake()
        {
            // 默认开启,但这里可以设置初始状态为关闭
            // CanMonoBehaviour = false;
        }
        partial void Start()
        {

        }
        partial void Update()
        {

        }
        partial void LateUpdate()
        {

        }
        partial void ExistMonoBehaviour()
        {
            
        }

        [AspectOriented] // 产生一个Proxy属性,Proxy.SaveData()可以触发自定义的拦截/扩展/覆盖,SaveData()则不触发
        public void SaveData()
        {

        }

        public void SetProxy() // 可以在运行时动态编辑Proxy的行为,注意,修改后的行为仅当通过Proxy访问才会生效,它不会破坏原始对象
        {
            Proxy.SetMethod(nameof(SaveData),
            (paras, prevs) =>
            {
                MessageBox.Show("拦截，发生在方法调用前");
                return null;
            },
            (paras, prevs) =>
            {
                MessageBox.Show("覆盖，不再使用方法的原始逻辑");
                return null;
            },
            (paras, prevs) =>
            {
                MessageBox.Show("回调，发生在方法调用后");
                return null;
            });

            // paras : 本次方法接收的参数
            // prevs : 上一个节点的返回值
        }
    }
```

---

### 快捷方式 🔑
- #### 全局快捷方式
```csharp
        [Constructor]
        private void SetHotKey()
        {
            GlobalHotKey.Register(VirtualModifiers.Ctrl | VirtualModifiers.Alt, VirtualKeys.F1, (s, e) =>
            {
                var modifiers = HotKeyHelper.GetModifiers(e.Modifiers); // 修饰符Ctrl+Alt
                var key = e.Keys; // 键F1 ( 全局快捷方式可以有多个修饰符,但具体的Key仅存在一个有效值 )
            });
        }
```
- #### 局部快捷方式
```csharp
        [Constructor]
        private void SetHotKey()
        {
            LocalHotKey.Register(this, [Key.LeftCtrl, Key.F1], (s, e) =>
            {
                // 本地快捷方式可以有多个修饰符,也可以有多个非修饰符,仅焦点位于控件时可被触发
            });
        }
```

- #### 注意事项
  - 全局快捷方式需要等待MainWindow加载完成后才会被注册,因为需要在MainWindow的事件中做一些处理
    - `SourceInitialized`事件中，调用`GlobalHotKey.Awake()`方法
    - `Closed`事件中，调用`GlobalHotKey.Dispose()`方法
    - 当然,如果你的`MainWindow`被识别为`增强版View`,那这些工作会自动完成
      - 使用过`Theme`特性特性的UIElement会被识别为增强版View
      - 使用过`HotKeyComponent`特性特性的UIElement会被识别为增强版View
      - 使用过`Hover`特性特性的UIElement会被识别为增强版View
      - 使用过`MonoBehaviour`的UIElement会被识别为增强版View
      - 使用过`AspectOriented`特性的UIElement会被识别为增强版View
    - 不过,你也不需要担心GlobalHotKey.Awake()方法没能及时调用,因为GlobalHotKey的注册行为具备缓冲机制,如果注册发生在Awake()之前,那么它会被暂存然后在Awake()成功后立即执行
  - 局部快捷方式需要保证UIElement可聚焦,即 `Focusable = true`,可通过为类标注`FocusModule`特性一键配置
