# MinimalisticWPF

##### UserControl and Animation are the most important parts in WPF project. This library will allow you to speed up these parts by using C# & Source Generator.

Get →

- [github](https://github.com/Axvser/MinimalisticWPF) 
- [nuget](https://www.nuget.org/packages/MinimalisticWPF/)

Practice →

- [MinimalisticWPF.Controls](https://github.com/Axvser/MinimalisticWPF.Controls)

---

## Features
- Core
  - [Ⅰ Transition](#Transition)
    - [Quick](#Quick)
    - [Reusable](#Reusable)
    - [Shared](#Shared)
    - [Isolation](#Isolation)
    - [Compile](#Compile)
    - [TransitionParams](#TransitionParams)
  - [Ⅱ ViewModel](#ViewModel)
    - [Field](#Field)
    - [Constructor](#Constructor)
    - [Hover](#Hover)
    - [Theme](#Theme)
    - [Dependency Properties](#Dependency)
  - [Ⅲ Aspect-Oriented Programming](#AOP)
  - [Ⅳ ObjectPool](#ObjectPool)
- Other
  - [StringValidator](#StringValidator)
  - [RGB](RGB)
  - [Custom Theme](#CustomTheme)
  - [Custom Interpolable Property](#CustomInterpolableProperty)

---

## Transition

namespace

```csharp
  using MinimalisticWPF.TransitionSystem;
```

### Quick

Load the transition on the instance by using the extension method

```csharp
  var control = new Grid();
  control.Transition()
      .SetProperty(x=>x.Background,Brushes.Red)
      .SetParams((p) =>
      {
          p.Duration = 3;
          p.IsAutoReverse = true;
          p.LoopTime = 2;
      })
      .Start();
```

### Reusable

The transition is described beforehand and then applied for multiple instances

```csharp
  var control1 = new Grid();
  var control2 = new Grid();
  var transition = Transition.Create<Grid>()
      .SetProperty(x=>x.Background,Brushes.Red)
      .SetParams((p) =>
      {
          p.Duration = 3;
          p.IsAutoReverse = true;
          p.LoopTime = 2;
      });
  control1.BeginTransition(transition);
  control2.BeginTransition(transition);
```

### Shared

Running multiple transitions, some shared mechanism makes them thread-safe

- Share a transition parameter
- A transition can either be terminated manually or a new transition can be started to override the current one
- Can use the methods provided by Transition to terminate the transition

```csharp
  var control = new Grid();

  var param = new TransitionParams()
  {
      Duration = 3,
      IsAutoReverse = true,
      LoopTime = 2,
  };
  var transition1 = Transition.Create<Grid>()
      .SetProperty(x => x.Background, Brushes.Red)
      .SetParams(param);
  var transition2 = Transition.Create<Grid>()
      .SetProperty(x => x.Width, 100)
      .SetParams(param);

  var transition = Transition.Create([transition1, transition2]);

  control.BeginTransition(transition);

  // Transition.DisposeAll();
  // Transition.Dispose(control);
```

### Isolation

Running multiple transitions, some isolation mechanism can make the effect more flexible, but it is not thread-safe

- Each transition must contain explicit effect parameters in advance
- Transitions in progress can only be terminated manually
- Can use the scheduler to terminate the transition

```csharp
  var control = new Grid();

  var param1 = new TransitionParams()
  {
      Duration = 3,
      IsAutoReverse = true,
  };
  var param2 = new TransitionParams()
  {
      Duration = 1,
      LoopTime = 15
  };
  var transition1 = Transition.Create<Grid>()
      .SetProperty(x => x.Background, Brushes.Red)
      .SetParams(param1);
  var transition2 = Transition.Create<Grid>()
      .SetProperty(x => x.Width, 100)
      .SetProperty(x => x.Height, 100)
      .SetParams(param2);

  var schedulers = control.BeginTransitions(transition1, transition2);

  // schedulers[0].Dispose();
  // schedulers[1].Dispose();
```

### Compile

You can make the transition immutable with the Compile operation

```csharp
  var control1 = new Grid();
  var control2 = new Grid();

  var param = new TransitionParams()
  {
      Duration = 3,
      IsAutoReverse = true,
      LoopTime = 2,
  };
  var transition1 = Transition.Create<Grid>()
      .SetProperty(x => x.Background, Brushes.Red)
      .SetParams(param);
  var transition2 = Transition.Create<Grid>()
      .SetProperty(x => x.Width, 100)
      .SetParams(param);

  var transition = Transition.Create([transition1, transition2]);
  var compile = Transition.Compile([transition1, transition2], param, null);

  param.LoopTime = 10;

  control1.BeginTransition(transition);
  control2.BeginTransition(compile);
```

---

## ViewModel

This is the heart of the library. You will build awesome user controls in Mvvm mode using clean C# code

★ Ultimately, everything is abstracted to data, and changing the data changes the functionality, and all you need in XAML is data binding ！

### Field

Automatically generate properties for fields

```csharp
  [Observable]
  private int _id = 0;
  partial void OnIdChanged(int oldValue, int newValue)
  {
      
  }
  partial void OnIdChanging(int oldValue, int newValue)
  {
      
  }
```

### Constructor

Multiple constructors are generated automatically.Functions with the same parameter list will be called from within the same constructor

```csharp
  [Constructor]
  private void SetDefaultValue()
  {

  }

  [Constructor]
  private void SetDefaultValues(int id, int age)
  {

  }
```

### Hover

Controls need to animate in response to your mouse hovering over them

```csharp
  [Observable(CanHover: true)]
  private Brush background = Brushes.White;

  [Constructor]
  private void SetDefaultValue()
  {
      // The default values are the same as the initial values of the fields, and you can change the values of these properties to achieve the mouse hover animation
      HoveredBackground = Brushes.Cyan;
      NoHoveredBackground = Brushes.White;

      // You can change the transition details
      HoveredTransition.SetParams(TransitionParams.Hover);
      NoHoveredTransition.SetParams(TransitionParams.Hover);

      // This change will automatically enable the hover transition
      IsHovered = true;

      // You can see if the hover transition is loading
      if (IsHoverChanging)
      {

      }
  }
```

### Theme

- Easily realize light and dark theme switch or other custom theme

```csharp
  [Observable]
  [Dark("#1e1e1e")]
  [Light("White")]
  private Brush background = Brushes.White;

  [Constructor]
  private void SetDefaultTheme()
  {
      // Setting the current theme
      CurrentTheme = typeof(Light);

      // You can see if you're switching topics
      if (IsThemeChanging)
      {

      }
  }

  partial void OnThemeChanging(Type? oldTheme, Type newTheme)
  {
      // For example, stopping the current transition
      Transition.Dispose(this);
  }
  partial void OnThemeChanged(Type? oldTheme, Type newTheme)
  {
      // This method is automatically generated by the library
      // It handles targets that apply both [CanHover:true] and [Theme]
      // If no target uses [CanHover:true], this method will not be generated even if [Theme] is used
      UpdateTransitionBoard();

      // The essence is to update [No/HoveredTransition] and decide which transition to load based on [IsHovered]
  }
```

- Switching between themes

```csharp
   this.ApplyTheme(typeof(Light),TransitionParams.Theme); // Local usage

   DynamicTheme.Apply(typeof(Light),TransitionParams.Theme); // Global usage
```

### Dependency

You can easily make user controls have dependency properties with the same name as properties in the ViewModel
- param1 → DataContext type name
- param2 → The name of the namespace in which the ViewModel is defined. If no class with the same name exists, omit namespace validation

```csharp
  [DataContextConfig(nameof(Class1), "TestForMWpf")]
  public partial class MainWindow : Window
```

- Class1

```csharp
    public partial class Class1
    {
        [Observable(CanDependency:true)]
        [Dark("#1e1e1e")]
        [Light("White")]
        private Brush background = Brushes.White;
    }
```

- Once configured, the control already contains dependency properties with the same name as properties in the ViewModel.

Here are some options 

```csharp
  partial void OnBackgroundChanged(Brush oldValue, Brush newValue)
  {
      
  }
  partial void OnDarkBackgroundChanged(Brush oldValue, Brush newValue)
  {
      
  }
  partial void OnLightBackgroundChanged(Brush oldValue, Brush newValue)
  {
      
  }
```

### TransitionParams

### Static ↓

| Property Name     | Type                 | Description                                                                 |
|-------------------|----------------------|-----------------------------------------------------------------------------|
| DefaultFrameRate  | int                  | Gets or sets the default frame rate for transitions.                      |
| DefaultPriority   | DispatcherPriority   | Gets or sets the default priority level for transition operations.        |
| DefaultIsBeginInvoke | bool              | Gets or sets whether the default invocation method is BeginInvoke.        |
| Theme             | TransitionParams     | Gets or sets the theme-specific transition parameters.                    |
| Hover             | TransitionParams     | Gets or sets the hover-specific transition parameters.                    |

### Effect parameter ↓

| Property Name     | Type                 | Description                                                                 |
|-------------------|----------------------|-----------------------------------------------------------------------------|
| IsAutoReverse     | bool                 | Gets or sets a value indicating whether the transition should auto-reverse. |
| LoopTime          | int                  | Gets or sets the number of times the transition should loop.              |
| Duration          | double               | Gets or sets the duration of the transition.                              |
| FrameRate         | int                  | Gets or sets the frame rate for the transition.                           |
| Acceleration      | double               | Gets or sets the acceleration factor for the transition.                |
| Priority          | DispatcherPriority   | Gets or sets the priority level for the transition operation.           |
| IsBeginInvoke     | bool                 | Gets or sets whether the invocation method is BeginInvoke.              |

### Life cycle event ↓

| Event Name    | Delegate Type | Description                                             |
|---------------|---------------|---------------------------------------------------------|
| Start         | Action        | Occurs when the transition starts.                    |
| Update        | Action        | Occurs during each update tick of the transition.       |
| LateUpdate    | Action        | Occurs after each update tick of the transition.        |
| Completed     | Action        | Occurs when the transition completes.                   |


---

## AOP

Allows you to dynamically override, extend, and intercept methods without modifying the source code of a class
- Mark properties, methods
```csharp
    internal partial class Class1
    {
        [AspectOriented]
        public string Property { get; set; } = string.Empty;

        [AspectOriented]
        public void Action()
        {

        }
    }
```
- Set up custom logic
  - Method parameters
    - first → Before the method is called
    - second → Original logic, passing null means no coverage
    - third → After the method is called
  - Delegate parameters
    - para → The arguments received when the method is called
    - last → The return value of the previous step
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

In the implementation of some visual effects, using object pooling makes it easier to avoid performance issues

```csharp
    [ObjectPool]
    internal partial class Class2
    {
        public Class2()
        {
            Pool.Record(this);
        }

        private double _counter = 0;
        public double Counter
        {
            get => _counter;
            set
            {
                _counter = value;
                if (CanRelease())
                {
                    Pool.Release(this);
                }
            }
        }

        private bool _isfinished = false;
        public bool IsFinished
        {
            get => _isfinished;
            set
            {
                _isfinished = value;
                if (CanRelease())
                {
                    Pool.Release(this);
                }
            }
        }

        public double Opacity { get; set; } = 1;

        private partial bool CanRelease()
        {
            return Counter > 100 && IsFinished;
        }
        partial void OnReleased()
        {
            _counter = 0;
            _isfinished = false;
            Opacity = 0;
        }
        partial void OnReused()
        {
            Opacity = 1;
        }
    }
```

---

## StringValidator

Minimal code → reusable string validator

```csharp
  var validator = new StringValidator()
      .StartWith("Hello")
      .EndWith("World")
      .VarLength(10,22)
      .Include("beautiful")
      .Exclude("bad")
      .Regex(@"^[A-Za-z\s]+$");

  string testString1 = "Hello beautiful World";
  string testString2 = "Hello bad World";

  MessageBox.Show($"{validator.validate(testString1)} | {validator.validate(testString2)}");
```

---

## RGB

Provides a reference class RGB to describe a color

- Supporting transition system
- Edit the values of R, G, B, and A directly
- Good for color class conversion

```csharp
  // Some raw data
  string colorText = "Red";
  Color color = Color.FromArgb(0, 0, 0, 0);
  Brush brush = Brushes.Red;

  // Can be converted to RGB
  RGB rgb1 = RGB.FromString(colorText);
  RGB rgb2 = RGB.FromColor(color);
  RGB rgb3 = RGB.FromBrush(brush);

  // RGB is a reference class, but you can use Equals to determine if RGBA is the same
  bool result = rgb1.Equals(rgb3);
  // The hash value is obtained based on RGBA
  int hash = rgb1.GetHashCode();

  // Modify the RGBA value directly
  rgb2.A = rgb1.A;
  rgb2.R = rgb1.R;
  rgb2.G = rgb1.G;
  rgb2.B = rgb1.B;

  // Converting from RGB to other common classes to represent colors
  color = rgb2.Color;
  brush = rgb2.Brush;
```

---

### Custom Theme

In addition to light and dark themes, you can also add custom themes that are also global and handled by the source generator

```csharp
using MinimalisticWPF.StructuralDesign.Theme;

namespace TestForMWpf
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class Glass(params object?[] objects) : Attribute, IThemeAttribute
    {
        public object?[] Parameters => objects;
    }
}
```

---

### Custom Interpolable Property

Make custom classes also participate in the transition system

```csharp
    internal class ComplexValue : IInterpolable
    {
        public ComplexValue() { }

        public double DoubleValue { get; set; } = 0;
        public Brush BrushValue { get; set; } = Brushes.Transparent;        

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