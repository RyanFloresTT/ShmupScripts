using System;
using _Project.Scripts.EventBus;
using Cysharp.Threading.Tasks;
using Febucci.UI.Core;
using KBCore.Refs;
using PrimeTween;
using UnityEngine;

namespace _Project.Scripts.UI {
    public class DeathUI : MonoBehaviour {
        [SerializeField] [Self] CanvasGroup deathUI;
        [SerializeField] TypewriterCore typewriter;
        EventBinding<OnPlayerDeath> onPlayerDeath;

        void Start() {
            this.onPlayerDeath = new EventBinding<OnPlayerDeath>(this.Handle_PlayerDeath);
            EventBus<OnPlayerDeath>.Register(this.onPlayerDeath);
        }

        async void Handle_PlayerDeath() {
            try {
                this.deathUI.interactable = true;
                await Tween.Alpha(this.deathUI, 1, 2f, Ease.Linear).ToUniTask();
                this.typewriter.StartShowingText();
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }

        void OnDisable() {
            EventBus<OnPlayerDeath>.Deregister(this.onPlayerDeath);
        }
    }
}