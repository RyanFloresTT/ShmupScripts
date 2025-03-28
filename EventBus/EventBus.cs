using System;
using System.Collections.Generic;
using Unity.VisualScripting;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Learn to Build an Advanced Event Bus | Unity Architecture] (https://www.youtube.com/watch?v=4_DTAnigmaQ)
// -----------------------------------------------------------------------


namespace _Project.Scripts.EventBus {
    public static class EventBus<T> where T : IEvent {
        static readonly HashSet<IEventBinding<T>> Bindings = new();
        static readonly Queue<Action> DeferredActions = new();
        static bool isRaisingEvent = false;

        public static void Register(EventBinding<T> binding) {
            if (isRaisingEvent)
                DeferredActions.Enqueue(() => Bindings.Add(binding));
            else
                Bindings.Add(binding);
        }

        public static void Deregister(EventBinding<T> binding) {
            if (isRaisingEvent)
                DeferredActions.Enqueue(() => Bindings.Remove(binding));
            else
                Bindings.Remove(binding);
        }

        public static void Raise(T @event) {
            isRaisingEvent = true;

            foreach (var binding in new List<IEventBinding<T>>(Bindings)) {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }

            isRaisingEvent = false;

            while (DeferredActions.Count > 0) {
                Action action = DeferredActions.Dequeue();
                action.Invoke();
            }
        }

        static void Clear() {
            Logger.Log($"Clearing {typeof(T).Name} bindings", Logger.LogCategory.EventBus);
            Bindings.Clear();
        }
    }
}