using UnityEngine;

/// <summary>
/// Interface for visiting components that implement <see cref="IVisitable"/>.
/// </summary>
public interface IVisitor {
    /// <summary>
    /// Visits a visitable component of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the visitable component that implements <see cref="IVisitable"/>.</typeparam>
    /// <param name="visitable">The visitable component to visit.</param>
    void Visit<T>(T visitable) where T : Component, IVisitable;
}

/// <summary>
/// Interface for components that can accept a visitor implementing <see cref="IVisitor"/>.
/// </summary>
public interface IVisitable {
    /// <summary>
    /// Accepts a visitor, allowing it to visit and interact with the implementing component.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    void Accept(IVisitor visitor);
}
