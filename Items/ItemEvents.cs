using UnityEngine;

public struct OnItemRegister : IEvent { public GameObject Item; }

public struct OnItemDropped : IEvent { public GameObject Item; public Vector3 Position; }
public struct OnPlayerEnteredStore : IEvent { 
    public bool IsInStore; 
    public OnPlayerEnteredStore(bool isIn) => IsInStore = isIn;
}