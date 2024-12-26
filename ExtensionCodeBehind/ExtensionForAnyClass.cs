using MinimalisticWPF.Animator;
using MinimalisticWPF.StructuralDesign.Animator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class ExtensionForAnyClass
    {
        public static TransitionBoard<T> Transition<T>(this T element) where T : class
        {
            TransitionBoard<T> tempStoryBoard = new() { Target = element };
            return tempStoryBoard;
        }

        public static IExecutableTransition BeginTransition<T1, T2>(this T1 source, T2 transfer) where T1 : class where T2 : class, ITransitionMeta
        {
            var result = Animator.Transition.Compile([transfer], transfer.TransitionParams, source);
            result.Start();
            return result;
        }
        public static IExecutableTransition BeginTransition<T1, T2>(this T1 source, T2 state, Action<TransitionParams> set) where T1 : class where T2 : class, ITransitionMeta
        {
            Animator.Transition.DisposeSafe(source);
            var param = new TransitionParams();
            set.Invoke(param);
            var result = Animator.Transition.Compile([state], param, source);
            result.Start();
            return result;

        }
        public static IExecutableTransition BeginTransition<T1, T2>(this T1 source, T2 state, TransitionParams param) where T1 : class where T2 : class, ITransitionMeta
        {
            Animator.Transition.DisposeSafe(source);
            var result = Animator.Transition.Compile([state], param, source);
            result.Start();
            return result;
        }

        public static StateMachine? FindStateMachine<T>(this T source) where T : class
        {
            if (StateMachine.TryGetMachine(source, out var machineA))
            {
                return machineA;
            }
            return null;
        }
    }
}
