# MinimalisticWPF

[Document](#Document) 

## What can this project do for you ?

（1）Backend, simple, flexible animation implementation

（2）Easy design pattern implementation

（3）Other small components that speed up WPF development

[github √](https://github.com/Axvser/MinimalisticWPF) 

[nuget √](https://www.nuget.org/packages/MinimalisticWPF/)

---

## Important Notice

2025 - 1 - 1 :

Update (V2.5.8) 

(1) Classes no longer need to declare the Theme attribute, as this is analyzed by the source generator and added automatically. 

(2) The AOP feature is no longer applied to classes but is used separately for each unit that requires aspect-oriented programming.

(3) Optimize the source generation operation when CanHover is set to true. The automatically generated NoHovered property will retain its initial value. If a Theme is involved, it will be set to the target value under the specified theme.
- [ HoveredTransition & NoHoveredTransition ] are used to dynamically describe the hover animation effect

(4) Add two globally shared animation parameters
- TransitionParams.Theme
- TransitionParams.Hover

★ Code Practices In [Aspect-Oriented Programming](#AOP)[ 3 - 1 ] Has Been Changed

★ Code Practices In [Dynamic Theme](#Theme)[ 5 - 3 ] Has Been Changed

Update (V2.5.9) 

When the source is generated, add an additional MinimalisticWPF reference

( In fact, until now, the project has not entered a real stable phase, there have been a lot of breaking updates, so many thanks to all the friends who keep updating, I will do my best to keep improving it )

---

## Document 

Feature Directory
- Core
  - [Animation](#Animation)
  - [Mvvm](#Mvvm) [ source generator √ ]
  - [Aspect-Oriented Programming](#AOP) [ source generator √ ]
  - [Object Pool](#ObjectPool)
  - [Dynamic Theme](#Theme) [ source generator √ ]
- Other
  - [Xml / Json Operation](#Xml&Json)
  - [Folder Creation](#folder)
  - [string Convertor](#stringConvertor)
  - [string Matcher](#stringMatcher)
  - [RGB Convertor](#RGB)

Details
- [TransitionParams](#TransitionParams)
- [Unsafe Transition](#UnsafeTransition)
- [Roslyn](#Generator)
---

## Animation

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

            var animation = c1.Transition()
                .SetProperty(x => x.Width, 500)
                .SetProperty(x => x.Height, 300)
                .SetProperty(x => x.Background, Brushes.Cyan)
                .SetProperty(x => x.Opacity, 0.5)
                .SetParams(param);

            animation.Start();
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
Transition.DisposeSafe(c1,c2);    // Only Safe transitions
Transition.DisposeUnSafe(c1,c2);  // Only UnSafe transitions
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

<h4 style="color:White">[ 4 - 1 ] Use attributes to configure a class</h4>

```csharp
  [Pool(5, 10)] //The initial number of units is 5. If resources are insufficient, the system automatically expands to a maximum of 10 units
  public class Unit
  {
      public static int count = 1;

      public Unit() { Value = count; count++; }

      public int Value { get; set; } = 1;

      [PoolFetch] //Triggered when the object is removed from the pool
      public void WhileFetch()
      {
          MessageBox.Show($"Fetch {Value}");
      }

      [PoolDispose] //Triggered when the object pool is automatically reclaimed
      public void WhileDispose()
      {
          MessageBox.Show($"Dispose {Value}");
      }

      [PoolDisposeCondition] //If the return value is true, the resource can be reclaimed automatically
      public bool CanDisposed()
      {
          return Value % 2 == 0 ? true : false;
      }
  }
```

<h4 style="color:White">[ 4 - 2 ] Use object pool to manage instances</h4>

(1) Get Instance

```csharp
var unit = Pool.Fetch(typeof(Unit)) as Unit;
if (Pool.TryFetch(typeof(Unit), out var result))
{
    var unit = result as Unit;
}
```

(2) Start Auto Dispose

```csharp
Pool.RunAutoDispose(typeof(Unit), 5000);
```

(3) End Auto Dispose

```csharp
Pool.StopAutoDispose(typeof(Unit));
```

(4) Force resource release

```csharp
var unit = Pool.Fetch(typeof(Unit)) as Unit;
Pool.ForceDispose(unit);
// Pool.ForceDispose(typeof(Unit));
```

It is common to have one thread per class to intermittently reclaim objects, which can lead to too many threads, and you won't see this message if there are optimizations in future versions

---

## Theme

### [ 5 - 1 ] Marking Theme Properties

(1) Marking Class

```csharp
[DynamicTheme]
public partial class MyPage
{
    // The constructor is automatically generated by the source generator
}
```

(2) Marking Field / Property

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
        public partial void OnThemeChanging(Type? oldTheme, Type newTheme)// [DynamicTheme] allows you to do something before theme changed
        {
            var state = IsThemeChanging; // Whether the theme switch animation is loading
            var current = CurrentTheme;
        }
        public partial void OnThemeChanged(Type? oldTheme, Type newTheme)// [DynamicTheme] allows you to do something after theme changed
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

Take text-color as an example. Let's implement the following effect
- Light themes default to "Black" and turn "Violet" when hovered
- Dark themes default to "White" and turn "Cyan" when hovered

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
    public partial class ButtonViewModel
    {
        [Constructor]
        private void SetDefualtHover()
        {
            CurrentTheme = typeof(Dark);
            DarkHoveredTextBrush = Brushes.Cyan;
            DarkNoHoveredTextBrush = Brushes.White;

            LightHoveredTextBrush = Brushes.Violet;
            LightNoHoveredTextBrush = Brushes.Black;
        }

        [Observable(CanHover: true)]
        [Dark("White")]
        [Light("#1e1e1e")]
        private Brush _textBrush = Brushes.White;
        partial void OnDarkHoveredTextBrushChanged(Brush oldValue, Brush newValue)
        {
            if (CurrentTheme == typeof(Dark))
            {
                HoveredTransition.SetProperty(control => control.TextBrush, newValue);
            }
        }
        partial void OnDarkNoHoveredTextBrushChanged(Brush oldValue, Brush newValue)
        {
            if (CurrentTheme == typeof(Dark))
            {
                NoHoveredTransition.SetProperty(control => control.TextBrush, newValue);
            }
        }
        partial void OnLightHoveredTextBrushChanged(Brush oldValue, Brush newValue)
        {
            if (CurrentTheme == typeof(Light))
            {
                HoveredTransition.SetProperty(control => control.TextBrush, newValue);
            }
        }
        partial void OnLightNoHoveredTextBrushChanged(Brush oldValue, Brush newValue)
        {
            if (CurrentTheme == typeof(Light))
            {
                NoHoveredTransition.SetProperty(control => control.TextBrush, newValue);
            }
        }

        [Observable]
        private bool _isHover = false;
        partial void OnIsHoverChanged(bool oldValue, bool newValue)
        {
            if (!IsThemeChanging)
            {
                this.BeginTransition(newValue ? HoveredTransition : NoHoveredTransition, TransitionParams.Theme);
            }
        }

        public partial void OnThemeChanging(Type? oldTheme, Type newTheme)
        {
            Transition.DisposeSafe(this);
        }
        public partial void OnThemeChanged(Type? oldTheme, Type newTheme)
        {
            if (CurrentTheme != null)
            {
                NoHoveredTransition.SetProperty(x => x.TextBrush, newTheme == typeof(Dark) ? DarkNoHoveredTextBrush : LightNoHoveredTextBrush);
                HoveredTransition.SetProperty(x => x.TextBrush, newTheme == typeof(Dark) ? DarkHoveredTextBrush : LightHoveredTextBrush);
            }
            this.BeginTransition(IsHover ? HoveredTransition : NoHoveredTransition, TransitionParams.Theme);
        }
    }
```

The main point of this example is to dynamically change the animation effect during the Theme change cycle by using SetProperty().

---

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
   string valueA = "-123.7";
   string valueB = "TrUE";
   string valueC = "#1e1e1e";
   
   var result1 = valueA.ToInt();
   var result2 = valueA.ToDouble();
   var result3 = valueA.ToFloat();

   var result4 = valueB.ToBool();

   var result5 = valueC.ToBrush();
   var result6 = valueC.ToColor();
   var result7 = valueC.ToRGB();
```

---

## stringMatcher

### (1) Regular

```csharp
   string sourceA = "[1]wkhdkjhk[a][F3]https:awijdioj.mp3fwafw";
   string sourceB = "awdhttps://aiowdjoajfo.comawd&*(&d)*dhttps://tn.comdawd";
   
   var resultA = sourceA.CaptureBetween("https:", ".mp3");

   var resultB = sourceB.CaptureLike("https://", "com");
```

### (2) Fuzzy Match

```csharp
   string template = "abcdefg";

   string sourceA = "abc";
   List<string> sourceB = new List<string>()
   {
       "abcdegf",
       "cbdgafe"
   };

   var similarity1 = sourceA.LevenshteinDistance(template)
   //Returns the shortest edit distance

   var similarity2 = sourceA.JaroWinklerDistance(template)
   //Returns approximation

   var result3 = template.BestMatch(sourceB, 3);
   //Edit the result with a minimum distance of less than 3

   var result4 = template.BestMatch(sourceB, 0.5);
   //The result with the approximation degree greater than 0.5 and the largest
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
| Start             | Action to execute before transition starts                                                       | null                  |
| Update            | Action to execute at the start of each frame                                                     | null                  |
| LateUpdate        | Action to execute at the end of each frame                                                       | null                  |
| Completed         | Action to execute after animation completes                                                      | null                  |
| StartAsync        | Asynchronous action to execute before transition starts                                          | null                  |
| UpdateAsync       | Asynchronous action to execute at the start of each frame                                        | null                  |
| LateUpdateAsync   | Asynchronous action to execute at the end of each frame                                          | null                  |
| CompletedAsync    | Asynchronous action to execute after animation completes                                         | null                  |
| IsAutoReverse     | Whether to automatically reverse                                                                 | false                 |
| LoopTime          | Number of loops                                                                                  | 0                     |
| Duration          | Duration of the transition (unit: s)                                                             | 0                     |
| FrameRate         | Transition frame rate (default: 60)                                                              | DefaultFrameRate (60) |
| IsQueue           | Whether to queue execution (default: not queued)                                                 | false                 |
| IsLast            | Whether to clear other queued transitions after completion (default: do not clear)               | false                 |
| IsUnique          | Whether to add to the queue if a similar transition is already queued (default: do not add)      | true                  |
| Acceleration      | Acceleration (default: 0)                                                                        | 0                     |
| IsUnSafe          | Unsafe operation flag indicating unconditional and immediate execution (default: false)          | false                 |
| UIPriority        | UI update priority                                                                               | DefaultUIPriority     |
| IsBeginInvoke     | Whether to use BeginInvoke when updating properties                                              | DefaultIsBeginInvoke  |

---

## UnsafeTransition

If an animation is set to Unsafe, it will normally execute on its own without being interrupted by other animations

In some cases, you can use this feature to create animations like building blocks

Of course, the Transition class provides methods to terminate such transitions

---

## Generator

(1) Only for partial classes

(2) Contains at least one of the AOP, Theme, and VMProperty attributes for the source generator to work

(3) The source generator has built the constructors

(4) You'll need to regenerate after installing the project or if the content changes

(5) If regenerating does not make the source generator work, you need to restart the editor, and if this still does not work, you need to check for the [.NET Compliler Platform SDK] and [Visual Basic Roslyn] components.