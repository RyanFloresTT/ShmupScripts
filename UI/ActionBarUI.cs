using System;
using _Project.Scripts.EventBus;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;

// -----------------------------------------------------------------------
// This script was written by my friend who helped with the project early on
// -----------------------------------------------------------------------


namespace _Project.Scripts.UI {
    public class ActionBarUI : MonoBehaviour {
        [SerializeField] [Self] UIDocument actionBar;

        EventBinding<OnPlayerEnabled> onPlayerEnabled;

        void Awake() {
            this.onPlayerEnabled = new EventBinding<OnPlayerEnabled>(this.Handle_PlayerEnabled);
            EventBus<OnPlayerEnabled>.Register(this.onPlayerEnabled);
        }

        void Handle_PlayerEnabled() {
            EventBus<OnActionBarUISet>.Raise(new OnActionBarUISet { ActionBar = this.actionBar });
        }

        void OnDisable() {
            EventBus<OnPlayerEnabled>.Deregister(this.onPlayerEnabled);
        }
    }

    public struct OnActionBarUISet : IEvent {
        public UIDocument ActionBar;
    }
}