# MinimalisticWPF
###### Use C# to get everything done ~

[Document](#Document) 

## What can this project do for you ?

- （1）use C# entirely to create [ Transition ] effects → [ TransitionParams ](#TransitionParams)

- （2）Make your [ ViewModel ] support aspect-oriented programming, theme switching, object pooling, and hover effects

- （3）Generate simple [ DependentProperties ] for your control based on DataContext

- （4）Other small components are used to speed up WPF project building, such as [ FluentStringValidator ]

[github √](https://github.com/Axvser/MinimalisticWPF) 

[nuget √](https://www.nuget.org/packages/MinimalisticWPF/)

---

## Important Notice

2025 - 1 - 16 :

Update ( V2.6.6 )

- (1) Remove unnecessary transition parameters, especially deprecating Unsafe
- (2) Instead of creating threads for each animation, we now use [ async Task ] to reduce pressure on the thread pool
- (3) [Object Pool](#ObjectPool) now support source generation

2025 - 1 - 17 :

Update ( V2.6.8 )

(1) Normally the source generator will automatically implement the necessary interfaces, which in the old version you had to explicitly implement in the ViewModel because they were partial methods with modifiers, which is addressed in the new version.
- Theme/Pool → [ partial void ] → Optional
- Pool       → [ CanRelease() ] → Necessary
```csharp
        private partial bool CanRelease()
        {
            return true;
        }
```

(2) Fixed a failed trigger for object pool life cycle function

---

## Document 

Feature Directory
- Core << [ source generator √ / × ]
  - [Transition](#Transition) × 
  - [Mvvm](#Mvvm) √ 
  - [Aspect-Oriented Programming](#AOP) √ 
  - [Object Pool](#ObjectPool) √ 
  - [Dynamic Theme](#Theme) √ 
- Other
  - [FluentStringValidator](#StringValidator)
  - [Xml / Json Operation](#Xml&Json)
  - [Folder Creation](#folder)
  - [string Convertor](#stringConvertor)
  - [string Matcher](#stringMatcher)
  - [RGB Convertor](#RGB)
---

## Transition

<h4 style="color:white">Take the following two controls as examples to demonstrate some animation operations</h4>

```xml
        <Grid>
            <Grid x:Name="c1" Width="200" Height="200" Background="Lime"/>
            <Grid x:Name="c2" Width="100" Height="100" Background="Gray" HorizontalAlignment="Right" VerticalAlignment="Bottom"/>
        </Grid>
```

A description of the animation parameters is in the Details directory [TransitionParams](#TransitionParams)

<h4 style="color:White">[ 1 - 1 ] Warm Start</h4>

```csharp 
        public void LoadAnimation_Warm()
        {
            var param = new TransitionParams()
            {
                Duration = 1,
                IsAutoReverse = true,
                LoopTime = 1
            };

            c1.Transition()
                .SetProperty(x => x.Width, 500)
                .SetProperty(x => x.Height, 300)
                .SetProperty(x => x.Background, Brushes.Cyan)
                .SetProperty(x => x.Opacity, 0.5)
                .SetParams(param)
                .Start();

            // .SetParams((p)=>{})
            // .SetParams(TransitionParams.Theme)
            // .SetParams(TransitionParams.Hover)
        }
```

<h4 style="color:White">[ 1 - 2 ] Cold Start</h4>

```csharp
        public void LoadAnimation_Cold()
        {
            var param = new TransitionParams()
            {
                Duration = 1,
                IsAutoReverse = true,
                LoopTime = 1
            };

            var animation = Transition.Create<Grid>()
                .SetProperty(x => x.Width, 500)
                .SetProperty(x => x.Height, 300)
                .SetProperty(x => x.Background, Brushes.Cyan)
                .SetProperty(x => x.Opacity, 0.5)
                .SetParams(param);

            c1.BeginTransition(animation);
            c2.BeginTransition(animation);
        }
```

<h4 style="color:White">[ 1 - 3 ] Merge Transitions</h4>

```csharp 
        public void LoadAnimation_Merge()
        {
            var param = new TransitionParams()
            {
                Duration = 1,
                IsAutoReverse = true,
                LoopTime = 1
            };

            var animation1 = Transition.Create<Grid>()
                .SetProperty(x => x.Width, 500)
                .SetProperty(x => x.Height, 300);
            var animation2 = Transition.Create<Grid>()
                .SetProperty(x => x.Background, Brushes.Cyan)
                .SetProperty(x => x.Opacity, 0.5);

            var transition = Transition.Create([animation1, animation2], param, null); 
            //The last argument indicates that the effect is applied to a specific instance, so if you specify an instance instead of null, you don't need to specify it again when calling Start()

            transition.Start(c1);
            transition.Start(c2);
        }
```

<h4 style="color:White">[ 1 - 4 ] Make custom properties participate in the transition</h4>

<h5 style="color:white">An interface is provided to allow types to participate in transitions</h5>Please describe how this type computes interpolation

```csharp
    internal class ComplexValue : IInterpolable
    {
        public double DoubleValue { get; set; } = 0;
        public Brush BrushValue { get; set; } = Brushes.Transparent;
        public ComplexValue() { Current = this; }

        public object Current { get; set; }

        public List<object?> Interpolate(object? current, object? target, int steps)
        {
            List<object?> result = new List<object?>(steps);

            ComplexValue old = current as ComplexValue ?? new ComplexValue();
            ComplexValue tar = target as ComplexValue ?? new ComplexValue();

            var doublelinear = IInterpolable.DoubleComputing(old.DoubleValue, tar.DoubleValue, steps);
            var brushlinear = IInterpolable.BrushComputing(old.BrushValue, tar.BrushValue, steps);

            for (int i = 0; i < steps; i++)
            {
                var dou = (double?)doublelinear[i];
                var bru = (Brush?)brushlinear[i];
                var newValue = new ComplexValue();
                newValue.DoubleValue = dou ?? 0;
                newValue.BrushValue = bru ?? Brushes.Transparent;
                result.Add(newValue);
            }

            return result;
        }
```

<h4 style="color:White">[ 1 - 5 ] Stop Transition</h4>

```csharp
Transition.DisposeAll();          // All transitions
Transition.Dispose(c1,c2);        // Only transitions of the selected object
```

<h4 style="color:White">[ 1 - 6 ] Compile Transition</h4>

<h5 style="color:white">Compile() outputs a controller that can only be used to start or terminate a transition.Modifying the original data will not affect the controller's transition</h5>

```csharp
        public void LoadAnimation_Merge()
        {
            var param = new TransitionParams()
            {
                Duration = 1,
                IsAutoReverse = true,
                LoopTime = 1
            };

            var animation1 = Transition.Create<Grid>()
                .SetProperty(x => x.Width, 500)
                .SetProperty(x => x.Height, 300);
            var animation2 = c1.Transition()
                .SetProperty(x => x.Background, Brushes.Cyan)
                .SetProperty(x => x.Opacity, 0.5);

            var transition = Transition.Create([animation1, animation2], param, null);
            var compiled = Transition.Compile([animation1, animation2], param, null);

            param.LoopTime = 3;

            transition.Start(c1); // LoopTime = 3
            compiled.Start(c2);   // LoopTime = 1
        }
```

Compile() is supported on any instance created with Transition.Create(), as well as on instances created directly with object.Transition()

---

## Mvvm

### [ 2 - 1 ] Automatically generate ViewModel for a partial class

```csharp
    internal partial class Class1
    {
        [Observable]
        private Brush _textBrush = Brushes.White;
        partial void OnTextBrushChanging(Brush oldValue, Brush newValue)
        {
            
        }
        partial void OnTextBrushChanged(Brush oldValue, Brush newValue)
        {
            
        }

        [Observable(SetterValidations.Custom)]
        private Brush _borderBrush = Brushes.White;
        private partial bool BorderBrushIntercepting(Brush oldValue, Brush newValue)
        {
            throw new NotImplementedException(); // Returning true will cancel the update
        }
        partial void OnBorderBrushChanging(Brush oldValue, Brush newValue)
        {
            
        }
        partial void OnBorderBrushChanged(Brush oldValue, Brush newValue)
        {
            
        }
    }
```

### [ 2 - 2 ] Declaration of multiple constructors

```csharp
        [Constructor]
        private void SetDefaultTheme()// [no-argument constructor] When initialized, set to dark theme
        {
            
        }      

        [Constructor]
        private void SetDefaultTheme(Type themeType)// [constructor with a Type argument] When initialized, set to custom theme
        {
            
        }
        [Constructor]
        private void LoadThemeAnimation(Type themeType)// Methods with the same parameters are executed in the same constructor
        {
            
        }
```

The source generator analyzes the parameter list and generates different constructors based on the parameter list ↓

```csharp
      // <auto-generated/>

      public Class1 ()
      {
         SetDefaultTheme();
      }

      public Class1 (Type themeType)
      {
         SetDefaultTheme(themeType);
         LoadThemeAnimation(themeType);
      }
```

You can define multiple methods with the same parameter list, and they will all go into the same constructor

---

## AOP

### [ 3 - 1 ] Make class aspect-oriented
- public Property/Method
- [ Observable & AspectOriented ] Field

```csharp
    internal partial class Class1
    {
        [Observable]
        [AspectOriented]
        private int _id = 0;

        [AspectOriented]
        public string Property { get; set; } = string.Empty;

        [AspectOriented]
        public void Action()
        {

        }
    }
```

### [ 3 - 2 ] Make an interception/coverage
- You can execute custom logic before or after calling an operation [ param1 & param3 ]
- you can override the original execution logic [ param2 ]
- [ para ] refers to the value passed in for this method call
- [ last ] represents the return value of the previous step
- If you don't need to return a value when defining an event, you can simply return null

```csharp
            var c1 = new Class1();

            c1.Proxy.SetMethod(nameof(c1.Action),
                (para, last) => { MessageBox.Show("Intercept method"); return null; },
                null,
                null);
            c1.Proxy.SetPropertyGetter(nameof(c1.Property),
                (para, last) => { MessageBox.Show("Intercept getter"); return null; },
                null,
                null);
            c1.Proxy.SetPropertySetter(nameof(c1.Property),
                (para, last) => { MessageBox.Show("Intercept setter"); return null; },
                null,
                null);

            c1.Proxy.Action();
            var a = c1.Proxy.Property;
            // Members are accessed through Proxy
```

---

## ObjectPool

### [ 4 - 1 ] Describe a ViewModel with a timer

- (1) When Time reaches the threshold, this instance will be released
  - CanRelease()
- (2) When an instance is reused or released, relevant attributes need to be modified to reasonably control the effect
  - OnReusing()
  - OnReused()
  - OnReleasing()
  - OnReleased()
```csharp
using MinimalisticWPF;

namespace WpfApp5
{
    internal partial class Class1
    {
        private bool cantimeadd = true;

        [Observable(CanInvokeRelease: true, SetterValidation: SetterValidations.Custom)]
        private int time = 0;
        private partial bool TimeIntercepting(int oldValue, int newValue)
        {
            return oldValue != newValue && cantimeadd;
        }

        [Observable]
        private double opacity = 1;

        partial void OnReusing()
        {
            Time = 0;
            Opacity = 1;
        }
        partial void OnReused()
        {
            cantimeadd = true;
        }

        private partial bool CanRelease()
        {
            return Time > 1000;
        }

        partial void OnReleasing()
        {
            cantimeadd = false;
        }
        partial void OnReleased()
        {
            Time = 0;
            Opacity = 0;
        }
    }
}
```

### [ 4 - 2 ] Get ViewModel from the object pool and use it for datacontext

- (1) Generally, the object pool in this project only takes and never lets go, and the release and reuse logic is included in the ViewModel

- (2) When resources are insufficient, the library will attempt to build a new instance based on the parameters received by Dequeue

```csharp
using MinimalisticWPF;
using MinimalisticWPF.ObjectPool;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    [DataContextConfig(nameof(Class1), "WpfApp1")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var context = Pool.Reuse(typeof(Class1));
            DataContext = context;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            ((Class1)DataContext).BorderBrush = Brushes.Violet;

            Pool.Reuse(typeof(Class1));
        }
    }
}
```

---

## Theme

### [ 5 - 1 ] Marking Theme Properties

[ Brush | double | CornerRadius | Thickness ] are supported.

Values are set with only the most basic constructors, such as [( 0,0,0,0 )] => [
( new Thickness(0,0,0,0) )]

```csharp
        [Observable]
        [Light(1)]
        [Dark(0)]
        private double _opacity = 1;

        [Observable]
        [Light("#1e1e1e")]
        [Dark(nameof(System.Windows.Media.Brushes.Cyan))] // Special data requires a full namespace
        public Brush _brush = Brushes.Transparent;
```

Do something before/after the theme changed

```csharp
        partial void OnThemeChanging(Type? oldTheme, Type newTheme)// [DynamicTheme] allows you to do something before theme changed
        {
            var state = IsThemeChanging; // Whether the theme switch animation is loading
            var current = CurrentTheme;
        }
        partial void OnThemeChanged(Type? oldTheme, Type newTheme)// [DynamicTheme] allows you to do something after theme changed
        {
            
        }
```

<h5 style="color:white">Apply Theme</h5>

```csharp
   this.ApplyTheme(typeof(WhenLight),default); // Started by the instance itself
   DynamicTheme.Apply(typeof(WhenLight),default); // Global usage
```

### [ 5 - 2 ] Theme Customization

Make your attribute implement the [ IThemeAttribute ] interface so that it can be used just like [ Light ] / [ Dark ]

|Property|Description|
|-------|---------|
|Parameters|These parameters will be used to dynamically construct values for a given theme|

### [ 5 - 3 ] Code Practices

Starting from version 2.6.1, the library enables you to implement theme switching, control hover effects, and more with less code.

(1) Using

```csharp
using MinimalisticWPF;
using System.Windows.Media;
using MinimalisticWPF.Animator;
```

(2) ViewModel

Theoretically, you can use multiple partial classes to build your view model, but there is one point to note: only one partial class of the same type can use the source code generation provided by this project. Usually, it is helpful for front-end and back-end separation to place business data in one partial class and animation effects in another. 

The front-end and back-end separation in this project is not reflected in [ C# + XAML ] but in [ C# + C# +... + XAML ].

```csharp
    public partial class WindowViewModel
    {
        [Constructor]
        private void SetDefualtHover()
        {
            CurrentTheme = typeof(Dark);

            HoveredTransition.SetParams(TransitionParams.Hover);
            NoHoveredTransition.SetParams(TransitionParams.Hover);
        }

        [Observable(CanHover: true)]
        [Dark("White")]
        [Light("#1e1e1e")]
        private Brush _textBrush = Brushes.White;

        [Observable(CanHover: true)]
        private Brush _borderBrush = Brushes.White;

        partial void OnThemeChanging(Type? oldTheme, Type newTheme)
        {
            Transition.DisposeSafe(this);
        }
        partial void OnThemeChanged(Type? oldTheme, Type newTheme)
        {
            UpdateTransitionBoard();
        }
    }
```

(3) Style

Suppose we use the ViewModel for a Window
- "WindowViewModel" is the name of ViewModel
- "Wpf3" is the namespace of ViewModel ( Optional )

```csharp
    [DataContextConfig(nameof(WindowViewModel),nameof(WpfApp3))]
    public partial class MainWindow : Window
```

The source generator will automatically generate these dependency properties for controlling the hover effect
- Just set the property value to achieve the function

```xml
        <Style TargetType="local:MainWindow" x:Key="WindowWithTheme">
            <!--Dark-->
            <Setter Property="DarkHoveredTextBrush" Value="Cyan"/>
            <Setter Property="DarkNoHoveredTextBrush" Value="#1e1e1e"/>
            <!--Light-->
            <Setter Property="LightHoveredTextBrush" Value="Violet"/>
            <Setter Property="LightNoHoveredTextBrush" Value="White"/>
            <!--NoTheme-->
            <Setter Property="HoveredBorderBrush" Value="Red"/>
            <Setter Property="NoHoveredBorderBrush" Value="Lime"/>
        </Style>
```

---

## FluentStringValidator

```csharp
        static void Main(string[] args)
        {
            var validator = new FluentStringValidator()
                .StartWith("Hello")
                .EndWith("World")
                .VarLength(10,22)
                .Include("beautiful")
                .Exclude("bad")
                .Regex(@"^[A-Za-z\s]+$");

            string testString1 = "Hello beautiful World";
            string testString2 = "Hello bad World";

            MessageBox.Show($"{validator.validate(testString1)} | {validator.validate(testString2)}");
        }
```

## Xml&Json

### (1)  deserialization

```csharp
   string folderName = "Data";

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";

   string AbsPathA = Path.Combine(folderName.CreatFolder(), $"{fileName1}.xml");
   string AbsPathB = Path.Combine(folderName.CreatFolder(), $"{fileName2}.json");
   var dataA = File.ReadAllText(AbsPathA);
   var dataB = File.ReadAllText(AbsPathB);

   var result1 = dataA.XmlParse<Person>();
   var result2 = dataB.JsonParse<Person>();
```

### (2) serialization

```csharp
   string folderName = "Data";

   string fileName1 = "firstPersondata";
   string fileName2 = "secondPersondata";

   var target = new Person();

   var result1 = fileName1.CreatXmlFile(folderName.CreatFolder(), target);
   var result2 = fileName2.CreatJsonFile(folderName.CreatFolder(), target);
```

---

## folder

```csharp
   string folderNameA = "FF1";
   string folderNameB = "FF2";
   string folderNameC = "FF3";
   //The folder name

   var result1 = folderNameA.CreatFolder();
   //From the.exe location, create a folder named "FF1"

   var result2 = folderNameC.CreatFolder(folderNameA,folderNameB);
   //From the.exe location, create a folder named "FF1/FF2/FF3"
```

---

## stringConvertor

```csharp
   string valueC = "#1e1e1e";

   var result5 = valueC.ToBrush();
   var result6 = valueC.ToColor();
   var result7 = valueC.ToRGB();
```

---

## stringMatcher

```csharp
   string sourceA = "[1]wkhdkjhk[a][F3]https:awijdioj.mp3fwafw";
   string sourceB = "awdhttps://aiowdjoajfo.comawd&*(&d)*dhttps://tn.comdawd";
   
   var resultA = sourceA.CaptureBetween("https:", ".mp3");

   var resultB = sourceB.CaptureLike("https://", "com");
```

---

## RGB

### (1) Properties

| Property | Type   | Description                                             |
|----------|--------|---------------------------------------------------------|
| R        | int    | Gets or sets the red component of the color, ranging from 0 to 255. |
| G        | int    | Gets or sets the green component of the color, ranging from 0 to 255. |
| B        | int    | Gets or sets the blue component of the color, ranging from 0 to 255. |
| A        | int    | Gets or sets the alpha (transparency) component of the color, ranging from 0 to 255. |

### (2) Methods

| Method Name     | Return Type            | Parameters                                      | Description                                                         |
|-----------------|------------------------|-------------------------------------------------|---------------------------------------------------------------------|
| Color           | System.Windows.Media.Color | None                                        | Converts the current `RGB` structure to a WPF `Color` object.       |
| SolidColorBrush | System.Windows.Media.SolidColorBrush | None                                        | Creates a `SolidColorBrush` based on the current color.             |
| Brush           | System.Windows.Media.Brush | None                                        | Creates a `Brush` based on the current color; internally calls `SolidColorBrush`. |
| FromColor       | RGB                    | color: System.Windows.Media.Color              | Static method that creates a new `RGB` structure from a given WPF `Color` object. |
| FromBrush       | RGB                    | brush: System.Windows.Media.Brush             | Static method that attempts to create a new `RGB` structure from a given brush. |
| Scale           | RGB                    | rateR, rateG, rateB, rateA: double            | Scales each color component according to the given ratios.          |
| Scale           | RGB                    | rateRGB, rateA: double                        | Scales the RGB components and transparency of the color by the given ratios. |
| Scale           | RGB                    | rateRGBA: double                             | Uniformly scales all color components by the given ratio.           |
| Delta           | RGB                    | deltaR, deltaG, deltaB, deltaA: int           | Changes each color component by the specified deltas.               |
| Delta           | RGB                    | deltaRGB, deltaA: int                         | Changes the RGB components and transparency of the color by the specified deltas. |
| Delta           | RGB                    | deltaRGBA: int                               | Uniformly changes all color components by the specified delta.      |
| SubA            | RGB                    | newValue: int                                 | Sets the alpha (transparency) component of the color.               |
| SubR            | RGB                    | newValue: int                                 | Sets the red component of the color.                                |
| SubG            | RGB                    | newValue: int                                 | Sets the green component of the color.                              |
| SubB            | RGB                    | newValue: int                                 | Sets the blue component of the color.                               |
| ToString        | string                 | None                                        | Returns a string representation of the current color in the format "RGBA [R,G,B,A]". |

---

## TransitionParams

| Property Name     | Description                                                                                      | Default Value         |
|-------------------|--------------------------------------------------------------------------------------------------|-----------------------|
| Start             | event that execute before transition starts                                                       | null                  |
| Update            | event that execute at the start of each frame                                                     | null                  |
| LateUpdate        | event that execute at the end of each frame                                                       | null                  |
| Completed         | event that execute after animation completes                                                      | null                  |
| IsAutoReverse     | Whether to automatically reverse                                                                 | false                 |
| LoopTime          | Number of loops                                                                                  | 0                     |
| Duration          | Duration of the transition (unit: s)                                                             | 0                     |
| FrameRate         | Transition frame rate (default: 60)                                                              | DefaultFrameRate (60) |
| Acceleration      | Acceleration (default: 0)                                                                        | 0                     |
| Priority          | UI update priority                                                                               | DefaultPriority     |
| IsBeginInvoke     | Whether to use BeginInvoke when updating properties                                              | DefaultIsBeginInvoke  |

---