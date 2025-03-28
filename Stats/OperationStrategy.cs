// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Learn to Build an Advanced Event Bus | Unity Architecture] (https://www.youtube.com/watch?v=4_DTAnigmaQ)
// -----------------------------------------------------------------------


/// <summary>
/// Interface for defining a strategy to perform a mathematical operation on a float value.
/// </summary>
public interface IOperationStrategy {
    /// <summary>
    /// Calculates the result of the operation on the given value.
    /// </summary>
    /// <param name="value">The input value to operate on.</param>
    /// <returns>The result of the operation.</returns>
    float Calculate(float value);
}

/// <summary>
/// Strategy that adds a specified value to the input value.
/// </summary>
public class AddOperation : IOperationStrategy {
    private readonly float value;

    /// <summary>
    /// Initializes an instance of AddOperation with the specified value to add.
    /// </summary>
    /// <param name="value">The value to add.</param>
    public AddOperation(float value) {
        this.value = value;
    }

    /// <summary>
    /// Adds the stored value to the provided input value.
    /// </summary>
    /// <param name="value">The input value to which the stored value will be added.</param>
    /// <returns>The result of the addition operation.</returns>
    public float Calculate(float value) => value + this.value;
}

/// <summary>
/// Strategy that multiplies a specified value with the input value.
/// </summary>
public class MultiplyOperation : IOperationStrategy {
    private readonly float value;

    /// <summary>
    /// Initializes an instance of MultiplyOperation with the specified value to multiply.
    /// </summary>
    /// <param name="value">The value to multiply.</param>
    public MultiplyOperation(float value) {
        this.value = value;
    }

    /// <summary>
    /// Multiplies the stored value with the provided input value.
    /// </summary>
    /// <param name="value">The input value to which the stored value will be multiplied.</param>
    /// <returns>The result of the multiplication operation.</returns>
    public float Calculate(float value) => value * this.value;
}