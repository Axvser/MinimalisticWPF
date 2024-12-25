using MinimalisticWPF.Animator;
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

        public static void BeginTransition<T>(this T source, TransitionBoard<T> transfer) where T : class
        {
            transfer.Start(source);
        }
        public static void BeginTransition<T>(this T source, TransitionBoard<T> transfer, Action<TransitionParams> set) where T : class
        {
            var param = new TransitionParams();
            set.Invoke(param);
            transfer.TransitionParams = param;
            transfer.Start(source);
        }
        public static void BeginTransition<T>(this T source, TransitionBoard<T> transfer, TransitionParams set) where T : class
        {
            transfer.TransitionParams = set;
            transfer.Start(source);
        }

        public static void BeginTransition<T>(this T source, State state, Action<TransitionParams> set) where T : class
        {
            if (state == null) return;
            var machine = source.FindStateMachine();
            if (machine == null)
            {
                var newMachine = StateMachine.Create(source, state);
                newMachine.Transition(state.StateName, set);
            }
            else
            {
                machine.Interrupt();
                machine.States.Add(state);
                machine.Transition(state.StateName, set);
            }
        }
        public static void BeginTransition<T>(this T source, State state, TransitionParams set) where T : class
        {
            if (state == null) return;
            var machine = source.FindStateMachine();
            if (machine == null)
            {
                var newMachine = StateMachine.Create(source, state);
                newMachine.Transition(state.StateName, set);
            }
            else
            {
                machine.Interrupt();
                machine.States.Add(state);
                machine.Transition(state.StateName, set);
            }
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
