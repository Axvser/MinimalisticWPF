using MinimalisticWPF.StructuralDesign.Animator;
using System.Reflection;
using System.Windows;

namespace MinimalisticWPF.TransitionSystem
{
    public sealed class TransitionInterpreter : IExecutableTransition
    {
        internal TransitionInterpreter(TransitionScheduler machine, TransitionParams transitionParams)
        {
            TransitionScheduler = machine;
            TransitionParams = transitionParams;
        }

        public TransitionParams TransitionParams { get; set; }
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; set; } = [];

        public TransitionScheduler TransitionScheduler { get; internal set; }
        internal int DeltaTime { get; set; } = 0;

        private bool IsRunning { get; set; } = false;
        private bool IsStop { get; set; } = false;
        private int LoopDepth { get; set; } = 0;
        private int FrameDepth { get; set; } = 0;

        public async Task Start(object? target = null)
        {
            if (IsStop || IsRunning) { WhileEnded(); return; }
            IsRunning = true;

            var accTimes = GetAccDeltaTime((int)TransitionScheduler.FrameCount);

            for (int x = LoopDepth; TransitionParams.LoopTime == int.MaxValue || x <= TransitionParams.LoopTime; x++, LoopDepth++)
            {
                for (int i = FrameDepth; i < TransitionScheduler.FrameCount; i++, FrameDepth++)
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
                    await Task.Delay(TransitionParams.Acceleration == 0 ? DeltaTime : i < accTimes.Count & accTimes.Count > 0 ? accTimes[i] : DeltaTime);
                }

                if (TransitionParams.IsAutoReverse)
                {
                    FrameDepth = TransitionScheduler.FrameCount > 1 ? (int)TransitionScheduler.FrameCount - 1 : 0;
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
                        await Task.Delay(TransitionParams.Acceleration == 0 ? DeltaTime : i < accTimes.Count & accTimes.Count > 0 ? accTimes[i] : DeltaTime);
                    }
                }
            }

            WhileEnded();
        }
        public void Stop()
        {
            IsStop = IsRunning;
            LoopDepth = 0;
            FrameDepth = 0;
        }
        public void StartAtNewTask()
        {
            if (IsStop || IsRunning) { WhileEnded(); return; }
            IsRunning = true;

            var accTimes = GetAccDeltaTime((int)TransitionScheduler.FrameCount);

            for (int x = LoopDepth; TransitionParams.LoopTime == int.MaxValue || x <= TransitionParams.LoopTime; x++, LoopDepth++)
            {
                for (int i = FrameDepth; i < TransitionScheduler.FrameCount; i++, FrameDepth++)
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
                    FrameDepth = TransitionScheduler.FrameCount > 1 ? (int)TransitionScheduler.FrameCount - 1 : 0;
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

        private bool EndConditionCheck()
        {
            if (IsStop || Application.Current == null || (TransitionScheduler.IsReSet || TransitionScheduler.Interpreter != this))
            {
                WhileEnded();
                return true;
            }
            return false;
        }
        private void FrameStart()
        {
            TransitionParams.UpdateInvoke();
        }
        private async void FrameUpdate(int i, int j, int k)
        {
            if (IsFrameIndexRight(i, j, k) && Application.Current != null)
            {
                if (TransitionParams.IsBeginInvoke)
                {
                    try
                    {
                        await Application.Current.Dispatcher.BeginInvoke(TransitionParams.Priority, () =>
                        {
                            FrameSequence[j][k].Item1.SetValue(TransitionScheduler.TransitionApplied, FrameSequence[j][k].Item2[i]);
                        });
                    }
                    catch { }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(TransitionParams.Priority, () =>
                    {
                        FrameSequence[j][k].Item1.SetValue(TransitionScheduler.TransitionApplied, FrameSequence[j][k].Item2[i]);
                    });
                }
            }
        }
        private void FrameEnd()
        {
            TransitionParams.LateUpdateInvoke();
        }
        private void WhileEnded()
        {
            if (TransitionScheduler.IsReSet)
            {
                return;
            }

            TransitionParams.CompletedInvoke();

            IsRunning = false;
            IsStop = false;

            if (TransitionScheduler.Interpreter == this)
            {
                TransitionScheduler.Interpreter = null;
                if (TransitionParams.IsLast)
                {
                    TransitionScheduler.Interpreters.Clear();
                }
                if (TransitionScheduler.Interpreters.TryDequeue(out var source))
                {
                    TransitionScheduler.InterpreterScheduler(source.Item1, source.Item2.TransitionParams, source.Item2.FrameSequence);
                }
                TransitionScheduler.CurrentState = null;
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
