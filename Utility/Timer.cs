using System;



/// <summary>
/// Abstract base class for a timer.
/// </summary>
public abstract class Timer {
    protected float initialTime;

    /// <summary>
    /// Current time of the timer.
    /// </summary>
    public float Time { get; set; }

    /// <summary>
    /// Indicates if the timer is currently running.
    /// </summary>
    public bool IsRunning { get; protected set; }

    /// <summary>
    /// Progress of the timer as a value between 0 and 1.
    /// </summary>
    public float Progress => Time / initialTime;

    /// <summary>
    /// Event invoked when the timer starts.
    /// </summary>
    public Action OnTimerStart = delegate { };

    /// <summary>
    /// Event invoked when the timer stops.
    /// </summary>
    public Action OnTimerStop = delegate { };

    /// <summary>
    /// Initializes a new instance of the Timer class.
    /// </summary>
    /// <param name="value">Initial value of the timer.</param>
    protected Timer(float value) {
        initialTime = value;
        IsRunning = false;
    }

    /// <summary>
    /// Starts the timer from the initial time.
    /// </summary>
    public void Start() {
        Time = initialTime;
        if (!IsRunning) {
            IsRunning = true;
            OnTimerStart.Invoke();
        }
    }

    /// <summary>
    /// Stops the timer if it's running.
    /// </summary>
    public void Stop() {
        if (IsRunning) {
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    /// <summary>
    /// Resumes the timer.
    /// </summary>
    public void Resume() => IsRunning = true;

    /// <summary>
    /// Pauses the timer.
    /// </summary>
    public void Pause() => IsRunning = false;

    /// <summary>
    /// Called every frame to update the timer.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last frame.</param>
    public abstract void Tick(float deltaTime);
}

/// <summary>
/// Timer that counts up from zero.
/// </summary>
public class StopwatchTimer : Timer {
    /// <summary>
    /// Initializes a new instance of the StopwatchTimer class.
    /// </summary>
    public StopwatchTimer() : base(0) { }

    /// <summary>
    /// Updates the timer by adding elapsed time.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last update.</param>
    public override void Tick(float deltaTime) {
        if (IsRunning) {
            Time += deltaTime;
        }
    }

    /// <summary>
    /// Resets the stopwatch timer to zero.
    /// </summary>
    public void Reset() => Time = 0;

    /// <summary>
    /// Retrieves the current elapsed time of the stopwatch timer.
    /// </summary>
    /// <returns>Current elapsed time.</returns>
    public float GetTime() => Time;
}

/// <summary>
/// Timer that counts down from an initial value to zero.
/// </summary>
public class CountdownTimer : Timer {
    /// <summary>
    /// Initializes a new instance of the CountdownTimer class.
    /// </summary>
    /// <param name="value">Initial countdown time.</param>
    public CountdownTimer(float value) : base(value) { }

    /// <summary>
    /// Updates the timer based on elapsed time.
    /// Stops the timer when it reaches zero.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last update.</param>
    public override void Tick(float deltaTime) {
        if (IsRunning && Time > 0) {
            Time -= deltaTime;
        }

        if (IsRunning && Time <= 0) {
            Stop();
        }
    }

    /// <summary>
    /// Indicates if the countdown timer has finished (reached zero).
    /// </summary>
    public bool IsFinished => Time <= 0;

    /// <summary>
    /// Resets the countdown timer to its initial time.
    /// </summary>
    public void Reset() => Time = initialTime;

    /// <summary>
    /// Resets the countdown timer to a new initial time.
    /// </summary>
    /// <param name="newTime">New initial time for the countdown.</param>
    public void Reset(float newTime) {
        initialTime = newTime;
        Reset();
    }
}