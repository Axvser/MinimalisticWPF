using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class TransitionMeta : IRecomputableTransitionMeta, IMergeableTransition, ITransitionMeta, IConvertibleTransitionMeta
    {
        internal TransitionMeta() { }
        public TransitionMeta(TransitionParams transitionParams, List<List<Tuple<PropertyInfo, List<object?>>>> frames)
        {
            TransitionParams = transitionParams;
            FrameSequence = frames;
        }
        public TransitionMeta(ITransitionMeta transitionMeta)
        {
            TransitionParams = transitionMeta.TransitionParams;
            FrameSequence = transitionMeta.FrameSequence;
        }
        public TransitionMeta(params TransitionMeta[] transitionMetas)
        {
            Merge(transitionMetas);
        }

        public TransitionParams TransitionParams { get; set; } = new();
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; set; } = [];

        public void Merge<T>(params T[] metas) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            Merge(metas);
        }
        public void Merge<T>(ICollection<T> metas) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            MergeSequence(metas);
        }
        public List<List<Tuple<PropertyInfo, List<object?>>>> RecomputeFrames(int fps)
        {
            for (int i = 0; i < FrameSequence.Count; i++)
            {
                for (int j = 0; j < FrameSequence[i].Count; j++)
                {
                    var start = FrameSequence[i][j].Item2.FirstOrDefault();
                    var end = FrameSequence[i][j].Item2.LastOrDefault();
                    if (FrameSequence[i][j].Item1.PropertyType == typeof(double))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.DoubleComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(Brush))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.BrushComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(Transform))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.TransformComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(Point))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.PointComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(Thickness))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.ThicknessComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(CornerRadius))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.CornerRadiusComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(IInterpolable))
                    {
                        if (start != null && end != null)
                        {
                            var ac0 = (IInterpolable)start;
                            if (ac0 != null)
                            {
                                FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, ac0.Interpolate(start, end, fps));
                            }
                        }
                    }
                }
            }
            return FrameSequence;
        }

        public TransitionMeta ToTransitionMeta()
        {
            return this;
        }
        public State ToState()
        {
            var state = new State();
            foreach (var frames in FrameSequence)
            {
                foreach (var frame in frames)
                {
                    state.AddProperty(frame.Item1.Name, frame.Item2.LastOrDefault());
                }
            }
            return state;
        }
        public TransitionBoard<T> ToTransitionBoard<T>() where T : class
        {
            var result = Transition.CreateBoardFromType<T>();
            result.TempState = ToState();
            result.ParamsInstance = TransitionParams;
            return result;
        }

        private void MergeSequence<T>(ICollection<T> metas) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            var list = metas.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].TransitionParams.FrameRate != TransitionParams.FrameRate)
                {
                    list[i].RecomputeFrames(TransitionParams.FrameRate);
                }
                MergeFrameSequences(list[i].FrameSequence);
            }
        }
        private void MergeFrameSequences(List<List<Tuple<PropertyInfo, List<object?>>>> source)
        {
            foreach (var propertyFrames in source)
            {
                if (!propertyFrames.Any())
                    continue;

                var propertyInfo = propertyFrames.First().Item1;
                var framesToAdd = propertyFrames.First().Item2;

                var existingPropertyFrames = FrameSequence.FirstOrDefault(pf => pf.First().Item1 == propertyInfo);

                if (existingPropertyFrames != null)
                {
                    existingPropertyFrames.Clear();
                    existingPropertyFrames.AddRange(propertyFrames);
                }
                else
                {
                    FrameSequence.Add(new List<Tuple<PropertyInfo, List<object?>>>(propertyFrames));
                }
            }
        }
    }
}
