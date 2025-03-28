using System.Collections.Generic;
using UnityEngine.UIElements;

namespace AbilitySystem {
    /// <summary>
    /// Manages the ability view UI.
    /// </summary>
    public class AbilityViewManager {
        // UI Classes
        private const string WrapperCls = "ability-bar";
        private const string SlotsCls = "ability-bar__slots";
        private const string SlotCls = "ability-bar-slot";
        private const string SlotActiveCls = "ability-bar-slot--active";
        private const string SlotFirstCls = "ability-bar-slot--first";
        private const string SlotKeyCls = "ability-bar-slot__key";
        private const string SlotIconCls = "ability-bar-slot__icon";
        private const string SlotCooldownCls = "ability-bar-slot__cooldown";

        // UI Elements
        private VisualElement _rootEl;
        private VisualElement _wrapperEl;
        private VisualElement _slotsEl;

        // Components
        private UIDocument _uiDoc;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbilityViewManager"/> class.
        /// </summary>
        /// <param name="uiDoc">The UI document containing the ability view.</param>
        public AbilityViewManager(UIDocument uiDoc) {
            _uiDoc = uiDoc;

            _rootEl = _uiDoc.rootVisualElement;
            _wrapperEl = _rootEl.Q(className: WrapperCls);
            _slotsEl = _wrapperEl.Q(className: SlotsCls);
        }

        /// <summary>
        /// Adds abilities to the view.
        /// </summary>
        /// <param name="abilities">The list of abilities to add.</param>
        public void AddAbilitiesToView(List<Ability> abilities) {
            for (int i = 0; i < abilities.Count; i++) {
                Ability ability = abilities[i];

                VisualElement slotEl = BuildSlotEl(
                    ability,
                    (i + 1).ToString(),
                    i == 0
                );
                _slotsEl.Add(slotEl);
            }
        }

        /// <summary>
        /// Gets the list of slot elements.
        /// </summary>
        /// <returns>A list of slot elements.</returns>
        public List<VisualElement> GetSlotEls() {
            return _rootEl.Query(className: SlotCls).ToList();
        }

        /// <summary>
        /// Updates the cooldown state of the slots.
        /// </summary>
        /// <param name="abilities">The list of abilities.</param>
        public void UpdateSlotsBusy(List<Ability> abilities) {
            List<VisualElement> cooldownEls = _rootEl.Query(className: SlotCooldownCls).ToList();

            for (int i = 0; i < cooldownEls.Count; i++) {
                if (i < abilities.Count) {
                    if (abilities[i].IsOnCooldown()) {
                        float abilityProgress = abilities[i].GetCooldownProgress();
                        cooldownEls[i].style.height = new Length(100 - (abilityProgress * 100), LengthUnit.Percent);
                    } else {
                        cooldownEls[i].style.height = new Length(0, LengthUnit.Percent);
                    }
                } else {
                    cooldownEls[i].style.height = new Length(0, LengthUnit.Percent);
                }
            }
        }

        /// <summary>
        /// Visually unlocks the ability at the specified index.
        /// </summary>
        /// <param name="abilityIndex">The index of the ability to unlock.</param>
        public void UnlockAbility(int abilityIndex) {
            VisualElement slot = _slotsEl.Query(className: SlotCls).AtIndex(abilityIndex);
            slot?.AddToClassList(SlotActiveCls);
        }

        /// <summary>
        /// Builds a slot element for the ability.
        /// </summary>
        /// <param name="ability">The ability for which to build the slot element.</param>
        /// <param name="key">The key to display in the slot element.</param>
        /// <param name="first">Indicates whether this is the first slot.</param>
        /// <returns>The built slot element.</returns>
        private VisualElement BuildSlotEl(Ability ability, string key, bool first) {
            VisualElement el = new();
            el.AddToClassList(SlotCls);
            if (first) {
                el.AddToClassList(SlotFirstCls);
            }

            if (ability.Unlocked) {
                el.AddToClassList(SlotActiveCls);
            }

            VisualElement iconEl = new();
            iconEl.AddToClassList(SlotIconCls);
            iconEl.style.backgroundImage = new StyleBackground(ability.AbilityData.Icon);
            el.Add(iconEl);

            VisualElement cooldownEl = new();
            cooldownEl.AddToClassList(SlotCooldownCls);
            el.Add(cooldownEl);

            Label keyEl = new(key);
            keyEl.AddToClassList(SlotKeyCls);
            el.Add(keyEl);

            return el;
        }
    }
}