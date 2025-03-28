using System;
using _Project.Scripts.EventBus;
using UnityEngine;

public class ItemDropManager : MonoBehaviour {
    [SerializeField] bool dropItems = true;
    GameObjectPoolManager poolManager;
    EventBinding<OnItemRegister> itemRegisterBinding;
    EventBinding<OnItemDropped> itemDroppedBinding;

    void Awake() {
        this.poolManager = new GameObjectPoolManager(this.transform);
    }

    void OnEnable() {
        this.itemRegisterBinding = new EventBinding<OnItemRegister>(this.Handle_ItemRegister);
        this.itemDroppedBinding = new EventBinding<OnItemDropped>(this.Handle_ItemDropped);
        EventBus<OnItemRegister>.Register(this.itemRegisterBinding);
        EventBus<OnItemDropped>.Register(this.itemDroppedBinding);
    }

    void OnDisable() {
        EventBus<OnItemRegister>.Deregister(this.itemRegisterBinding);
        EventBus<OnItemDropped>.Deregister(this.itemDroppedBinding);
    }

    void Handle_ItemDropped(OnItemDropped data) {
        if (!this.dropItems) return;
        GameObject item = this.poolManager.Get(data.Item.name);
        item.transform.position = data.Position;
    }

    void Handle_ItemRegister(OnItemRegister data) {
        this.poolManager.CreatePool(data.Item.name, data.Item);
    }
}