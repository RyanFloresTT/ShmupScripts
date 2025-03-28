﻿using System;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Learn to Build an Advanced Event Bus | Unity Architecture] (https://www.youtube.com/watch?v=4_DTAnigmaQ)
// -----------------------------------------------------------------------

public interface IEventBinding<T> {
    public Action<T> OnEvent { get; set; }
    public Action OnEventNoArgs { get; set; }
}

public class EventBinding<T> : IEventBinding<T> where T : IEvent {
    Action<T> onEvent = _ => { };
    Action onEventNoArgs = () => { };

    Action<T> IEventBinding<T>.OnEvent {
        get => onEvent;
        set => onEvent = value;
    }

    Action IEventBinding<T>.OnEventNoArgs {
        get => onEventNoArgs;
        set => onEventNoArgs = value;
    }

    public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;
    public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;

    public void Add(Action onEvent) => onEventNoArgs += onEvent;
    public void Remove(Action onEvent) => onEventNoArgs -= onEvent;
    
    public void Add(Action<T> onEvent) => this.onEvent += onEvent;
    public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
}