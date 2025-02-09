# MinimalisticWPF

##### UserControl and Animation are the most important parts in WPF project. This library will allow you to speed up these parts by using C# & Source Generator.

- [github](https://github.com/Axvser/MinimalisticWPF) 
- [nuget](https://www.nuget.org/packages/MinimalisticWPF/)

---

## Directory
- Core
  - [Transition](#Transition)
    - [Start - fastest](#（1）Start)
    - [Start - reusable](#（2）Start)
    - [Merge](#（3）Merge)
    - [Compile](#（4）Compile)
    - [Interrupt](#（5）Interrupt)
    - [Non-Unique](#（6）Independent)
  - [ViewModel](#ViewModel)
    - [Hover](#（1）Hover)
    - [Theme](#（2）Theme)
    - [Dependency](#（3）Dependency)
  - [Aspect-Oriented Programming](#AOP)
  - [ObjectPool](#ObjectPool)
- Other
  - [StringValidator](#StringValidator)
  - [RGB](#RGB)

---

## Transition

```csharp
  using MinimalisticWPF.TransitionSystem;
```

### （1）Start

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

### （2）Start

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

### （3）Merge

For several transitions of the same class, you can combine them and apply them according to the transition parameters specified

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
```

### （4）Compile

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

### （5）Interrupt

Interrupt an executing transition

```csharp
  var control = new Grid();

  Transition.DisposeAll();     // all instances

  Transition.Dispose(control); // specified instances
```

### （6）Independent

In general, an object is not allowed to run multiple transitions at the same time, but libraries provide mechanisms to do so
- Each transition must contain explicit effect parameters in advance
- Use the scheduler to terminate the transition

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