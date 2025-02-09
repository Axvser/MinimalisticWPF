# MinimalisticWPF

##### UserControl and Animation are the most important parts in WPF project. This library will allow you to speed up these parts by using C# & Source Generator.

- [github](https://github.com/Axvser/MinimalisticWPF) 
- [nuget](https://www.nuget.org/packages/MinimalisticWPF/)

---

## Directory
- Core
  - [Ⅰ Transition](#Transition)
    - [Quick](#Quick)
    - [Reusable](#Reusable)
    - [Shared](#Merge)
    - [Isolation](#Isolation)
    - [Compile](#Compile)
  - [Ⅱ ViewModel](#ViewModel)
    - [Hover](#（1）Hover)
    - [Theme](#（2）Theme)
    - [Dependency](#（3）Dependency)
  - [Ⅲ Aspect-Oriented Programming](#AOP)
  - [Ⅳ ObjectPool](#ObjectPool)
- Other
  - [StringValidator](#StringValidator)
  - [RGB](#RGB)

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

  control.BeginTransitions(transition1, transition2);
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
### （1）Hover
### （2）Theme
### （3）Dependency



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