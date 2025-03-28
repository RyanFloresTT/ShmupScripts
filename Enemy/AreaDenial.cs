using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Player;

public class AreaDenial : MonoBehaviour {
    [SerializeField] LayerMask layerToAffect;
    public float TickRateSeconds = 1f;
    public int Damage = 10;
    public float Radius = 5f;
    bool isActive = false;
    readonly List<Character> charactersInField = new();

    void OnTriggerEnter(Collider other) {
        if (((1 << other.gameObject.layer) & this.layerToAffect.value) != 0) {
            Character character = other.GetComponent<Character>();
            if (character != null && !this.charactersInField.Contains(character)) {
                this.charactersInField.Add(character);
                if (!this.isActive) {
                    this.isActive = true;
                    this.StartCoroutine(this.TickDamage());
                }
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (((1 << other.gameObject.layer) & this.layerToAffect.value) != 0) {
            Character character = other.GetComponent<Character>();
            if (character != null && this.charactersInField.Contains(character)) {
                this.charactersInField.Remove(character);
                if (this.charactersInField.Count == 0) {
                    this.isActive = false;
                    this.StopCoroutine(this.TickDamage());
                }
            }
        }
    }

    IEnumerator TickDamage() {
        while (this.isActive) {
            foreach (Character character in this.charactersInField)
                if (character != null)
                    character.Health.TakeDamage(this.Damage);

            yield return new WaitForSeconds(this.TickRateSeconds);
        }
    }
}