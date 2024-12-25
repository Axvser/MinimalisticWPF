using MinimalisticWPF.StructuralDesign.Animator;
using System.Reflection;
using System.Windows;

namespace MinimalisticWPF.Animator
{
    public sealed class TransitionInterpreter : IExecutableTransition
    {
        internal TransitionInterpreter(StateMachine machine, TransitionParams transitionParams)
        {
            Machine = machine;
            TransitionParams = transitionParams;
        }

        public TransitionParams TransitionParams { get; set; }
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; set; } = [];

        public StateMachine Machine { get; internal set; }
        internal int DeltaTime { get; set; } = 0;

        private bool IsRunning { get; set; } = false;
        private bool IsStop { get; set; } = false;
        private int LoopDepth { get; set; } = 0;
        private int FrameDepth { get; set; } = 0;

        public void Start(object? target = null)
        {
            if (IsStop || IsRunning) { WhileEnded(); return; }
            IsRunning = true;

            var accTimes = GetAccDeltaTime((int)Machine.FrameCount);

            for (int x = LoopDepth; TransitionParams.LoopTime == int.MaxValue || x <= TransitionParams.LoopTime; x++, LoopDepth++)
            {
                for (int i = FrameDepth; i < Machine.FrameCount; i++, FrameDepth++)
                {
                    if (EndConditionCheck()) return;
                    FrameStart();
                    for (int j = 0; j < FrameSequence.Count; j++)
                    {
                        for (int k = 0; k < FrameSequence[j].Count; k++)
                        {
                            FrameUpdate(i, j, k);
                        }
                    }
                    FrameEnd();
                    Thread.Sleep(TransitionParams.Acceleration == 0 ? DeltaTime : i < accTimes.Count & accTimes.Count > 0 ? accTimes[i] : DeltaTime);
                }

                if (TransitionParams.IsAutoReverse)
                {
                    FrameDepth = Machine.FrameCount > 1 ? (int)Machine.FrameCount - 1 : 0;
                    for (int i = FrameDepth; i > -1; i--, FrameDepth--)
                    {
                        if (EndConditionCheck()) return;
                        FrameStart();
                        for (int j = 0; j < FrameSequence.Count; j++)
                        {
                            for (int k = 0; k < FrameSequence[j].Count; k++)
                            {
                                FrameUpdate(i, j, k);
                            }
                        }
                        FrameEnd();
                        Thread.Sleep(TransitionParams.Acceleration == 0 ? DeltaTime : i < accTimes.Count & accTimes.Count > 0 ? accTimes[i] : DeltaTime);
                    }
                }
            }

            WhileEnded();
        }
        public void Stop(bool IsUnsafeStoped = false)
        {
            if (TransitionParams.IsUnSafe && !IsUnsafeStoped) return;
            IsStop = IsRunning;
            LoopDepth = 0;
            FrameDepth = 0;
        }

        private bool EndConditionCheck()
        {
            if (IsStop || Application.Current == null || !TransitionParams.IsUnSafe && (Machine.IsReSet || Machine.Interpreter != this))
            {
                WhileEnded();
                return true;
            }
            return false;
        }
        private async void FrameStart()
        {
            if (Application.Current != null)
            {
                if (TransitionParams.Update != null)
                {
                    Application.Current.Dispatcher.Invoke(TransitionParams.Update);
                }
                if (TransitionParams.UpdateAsync != null)
                {
                    await TransitionParams.UpdateAsync.Invoke();
                }
            }
        }
        private async void FrameUpdate(int i, int j, int k)
        {
            if (IsFrameIndexRight(i, j, k) && Application.Current != null)
            {
                if (TransitionParams.IsBeginInvoke)
                {
                    try
                    {
                        await Application.Current.Dispatcher.BeginInvoke(TransitionParams.UIPriority, () =>
                        {
                            FrameSequence[j][k].Item1.SetValue(Machine.Target, FrameSequence[j][k].Item2[i]);
                        });
                    }
                    catch { }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(TransitionParams.UIPriority, () =>
                    {
                        FrameSequence[j][k].Item1.SetValue(Machine.Target, FrameSequence[j][k].Item2[i]);
                    });
                }
            }
        }
        private async void FrameEnd()
        {
            if (Application.Current != null)
            {
                if (TransitionParams.LateUpdate != null)
                {
                    Application.Current.Dispatcher.Invoke(TransitionParams.LateUpdate);
                }
                if (TransitionParams.LateUpdateAsync != null)
                {
                    await TransitionParams.LateUpdateAsync.Invoke();
                }
            }
        }
        private async void WhileEnded()
        {
            if (TransitionParams.IsUnSafe)
            {
                Machine.UnSafeInterpreters.Remove(this);
                return;
            }

            if (Machine.IsReSet)
            {
                return;
            }

            if (Application.Current != null)
            {
                if (TransitionParams.Completed != null)
                {
                    Application.Current.Dispatcher.Invoke(TransitionParams.Completed);
                }
                if (TransitionParams.CompletedAsync != null)
                {
                    await TransitionParams.CompletedAsync.Invoke();
                }
            }
            IsRunning = false;
            IsStop = false;

            if (Machine.Interpreter == this)
            {
                Machine.Interpreter = null;
                if (TransitionParams.IsLast)
                {
                    Machine.Interpreters.Clear();
                }
                if (Machine.Interpreters.TryDequeue(out var source))
                {
                    Machine.InterpreterScheduler(source.Item1, source.Item2.TransitionParams, source.Item2.FrameSequence);
                }
                Machine.CurrentState = null;
            }
        }
        private List<int> GetAccDeltaTime(int Steps)
        {
            List<int> result = [];
            if (TransitionParams.Acceleration == 0) return result;

            var acc = Math.Clamp(TransitionParams.Acceleration, -1, 1);
            var start = DeltaTime * (1 + acc);
            var end = DeltaTime * (1 - acc);
            var delta = end - start;
            for (int i = 0; i < Steps; i++)
            {
                var t = (double)(i + 1) / Steps;
                result.Add((int)(start + t * delta));
            }

            return result;
        }
        private bool IsFrameIndexRight(int i, int j, int k)
        {
            if (FrameSequence.Count > 0 && j >= 0 && j < FrameSequence.Count)
            {
                if (FrameSequence[j].Count > 0 && k >= 0 && k < FrameSequence[j].Count)
                {
                    if (FrameSequence[j][k].Item2.Count > 0 && i >= 0 && i < FrameSequence[j][k].Item2.Count)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
