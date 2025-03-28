using UnityEngine;
using UnityServiceLocator;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// -----------------------------------------------------------------------


public class Bootstrapper : MonoBehaviour {
    void Awake() {
        DontDestroyOnLoad(gameObject);
        ServiceLocator.Global.Register<IStatModifierFactory>(new StatModifierFactory());
    }
}