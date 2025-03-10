using MinimalisticWPF.StructuralDesign.Move;
using MinimalisticWPF.TransitionSystem;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.MoveBehavior
{
    public static class Move
    {
        public static IExecutableMove Create(FrameworkElement target, TransitionParams transitionParams, params IMoveMeta[] moves)
        {
            return new MoveAggregator(target, transitionParams.DeepCopy(), moves);
        }
    }
}
