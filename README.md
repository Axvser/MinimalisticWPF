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

Running multiple transitions, some isolation mechanism can make the effect more flexible, but it is not thread-safe

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

Run multiple transitions at the same time. These effects are isolated

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



---

## ObjectPool



---

## StringValidator



---

## RGB