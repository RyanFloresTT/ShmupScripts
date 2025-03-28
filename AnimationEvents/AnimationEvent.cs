using System;
using UnityEngine.Events;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Using IMPROVED Animation Events in Unity] (https://www.youtube.com/watch?v=XEDi7fUCQos)
// -----------------------------------------------------------------------

[Serializable]
public class AnimationEvent {
    public string eventName;
    public UnityEvent OnAnimationEvent;
}