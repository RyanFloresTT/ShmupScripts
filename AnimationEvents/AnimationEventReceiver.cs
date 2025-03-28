using UnityEngine;
using System.Collections.Generic;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Using IMPROVED Animation Events in Unity] (https://www.youtube.com/watch?v=XEDi7fUCQos)
// -----------------------------------------------------------------------


public class AnimationEventReceiver : MonoBehaviour {
    [SerializeField] List<AnimationEvent> animationEvents = new();

    public void OnAnimationEventTriggered(string eventName) {
        AnimationEvent matchingEvent = animationEvents.Find(se => se.eventName == eventName);
        matchingEvent?.OnAnimationEvent?.Invoke();
    }
}