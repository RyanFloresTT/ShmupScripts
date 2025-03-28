using AbilitySystem;
using System;
using System.Collections.Generic;
using _Project.Scripts.Player;

/// <summary>
/// Factory class for creating abilities.
/// </summary>
public static class AbilityFactory {
    /// <summary>
    /// Dictionary to store registered abilities.
    /// </summary>
    static readonly Dictionary<Type, Type> _registeredAbilities = new();

    /// <summary>
    /// Registers an ability type with the corresponding ability data type.
    /// </summary>
    /// <typeparam name="TData">The type of the ability data.</typeparam>
    /// <typeparam name="TAbility">The type of the ability.</typeparam>
    public static void Register<TData, TAbility>()
        where TData : AbilityData
        where TAbility : Ability<TData> {
        _registeredAbilities[typeof(TData)] = typeof(TAbility);
    }

    /// <summary>
    /// Creates an ability based on the provided data and character.
    /// </summary>
    /// <param name="data">The ability data.</param>
    /// <param name="character">The character associated with the ability.</param>
    /// <returns>The created ability, or null if creation fails.</returns>
    public static Ability Create(AbilityData data, Character character) {
        if (character == null) Logger.Log("Character is null", Logger.LogCategory.AbilitySystem);
        if (data == null) Logger.Log("Data is null", Logger.LogCategory.AbilitySystem);
        Type dataType = data.GetType();

        if (_registeredAbilities.TryGetValue(dataType, out Type abilityType))
            try {
                object abilityInstance = Activator.CreateInstance(abilityType, data, character);
                return abilityInstance as Ability;
            }
            catch (Exception ex) {
                Logger.Log(
                    $"Exception while creating ability: '{data.name}'. {ex.Message}. {ex.StackTrace}. {ex.InnerException?.Message}.",
                    Logger.LogCategory.AbilitySystem);
                return null;
            }
        else {
            Logger.Log($"No registered ability found for data type {dataType.Name}", Logger.LogCategory.AbilitySystem);
            return null;
        }
    }
}