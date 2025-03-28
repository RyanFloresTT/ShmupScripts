using UnityEngine;

/// <summary>
/// Base abstract class for audio events
/// </summary>
public abstract class AudioEvent : ScriptableObject
{
    public abstract void Play();
    public abstract void Play(Vector3 position);
} 