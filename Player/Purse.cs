using Saving;
using System;

/// <summary>
/// Keeps track of a currency.
/// </summary>
[Serializable]
public class Purse {
    /// <summary>
    /// The current amount of currency in the purse.
    /// </summary>
    public int Amount;

    /// <summary>
    /// Event triggered when the amount of currency changes.
    /// </summary>
    [NonSerialized] public Action<Purse> OnAmountChanged;

    [NonSerialized] readonly SaveManager saveManager;

    /// <summary>
    /// The maximum amount of currency the purse can hold.
    /// </summary>
    public int MaxAmount;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Purse() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Purse"/> class with a starting amount and a maximum amount.
    /// </summary>
    /// <param name="startingAmount">The initial amount of currency.</param>
    /// <param name="maxAmount">The maximum amount of currency the purse can hold.</param>
    public Purse(int startingAmount, int maxAmount) {
        this.Amount = startingAmount;
        this.MaxAmount = maxAmount;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Purse"/> class with a starting amount, a maximum amount, and a save manager.
    /// </summary>
    /// <param name="startingAmount">The initial amount of currency.</param>
    /// <param name="maxAmount">The maximum amount of currency the purse can hold.</param>
    /// <param name="saveManager">The save manager to handle saving the currency data.</param>
    public Purse(int startingAmount, int maxAmount, SaveManager saveManager) {
        this.saveManager = saveManager;
        this.Amount = startingAmount;
        this.MaxAmount = maxAmount;
    }

    /// <summary>
    /// Adds the specified amount to the current amount of currency.
    /// </summary>
    /// <param name="amount">The amount to add to the currency.</param>
    public void Add(int amount) {
        this.Amount += amount;
        this.MaxAmount += amount;
        this.OnAmountChanged?.Invoke(this);
        this.SaveCoins();
    }

    /// <summary>
    /// Attempts to buy an item with the specified amount. Returns true if the purchase was successful, otherwise false.
    /// </summary>
    /// <param name="purchaseAmount">The amount to deduct for the purchase.</param>
    /// <returns>True if the purchase was successful; otherwise, false.</returns>
    public bool Buy(int purchaseAmount) {
        if (purchaseAmount <= this.Amount) {
            this.Amount -= purchaseAmount;
            this.SaveCoins();
            this.OnAmountChanged?.Invoke(this);
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Saves the current amount of currency to the save manager.
    /// </summary>
    void SaveCoins() {
        if (this.saveManager != null) this.saveManager.UpdateSingleField(SaveField.PermanentPurse, this);
        this.saveManager.SaveGameData();
    }
}