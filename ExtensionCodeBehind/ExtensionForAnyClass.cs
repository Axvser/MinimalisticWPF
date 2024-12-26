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

        public static IExecutableTransition BeginTransition<T>(this T source, TransitionBoard<T> transfer) where T : class
        {
            var result = Animator.Transition.Compile([transfer], transfer.TransitionParams, source);
            result.Start();
            return result;
        }
        public static IExecutableTransition BeginTransition<T>(this T source, TransitionBoard<T> transfer, Action<TransitionParams> set) where T : class
        {
            var param = new TransitionParams();
            set.Invoke(param);
            transfer.TransitionParams = param;
            var result = Animator.Transition.Compile([transfer], set, source);
            result.Start();
            return result;
        }
        public static IExecutableTransition BeginTransition<T>(this T source, TransitionBoard<T> transfer, TransitionParams set) where T : class
        {
            var result = Animator.Transition.Compile([transfer], set, source);
            result.Start();
            return result;
        }

        public static IExecutableTransition BeginTransition<T>(this T source, State state, Action<TransitionParams> set) where T : class
        {
            Animator.Transition.Dispose(source);
            var param = new TransitionParams();
            set.Invoke(param);
            var result = Animator.Transition.Compile([state], param, source);
            result.Start();
            return result;

        }
        public static IExecutableTransition BeginTransition<T>(this T source, State state, TransitionParams param) where T : class
        {
            Animator.Transition.Dispose(source);
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
