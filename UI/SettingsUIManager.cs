using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.EventBus;
using Saving;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

// -----------------------------------------------------------------------
// This script was written by my friend who helped with the project early on
// -----------------------------------------------------------------------

public class SettingsUIManager : MonoBehaviour {
    #region Classnames

    const string WrapperCls = "menu";
    const string WrapperActiveCls = "menu--active";
    const string ContentCls = "menu__content";
    const string ContentTitleCls = "menu__content-title";
    const string OptionCls = "menu-option";
    const string OptionSelectedCls = "menu-option--selected";
    const string OptionLabelCls = "menu-option__label";
    const string OptionOptionsCls = "menu-option__options";
    const string OptionLeftCls = "menu-option__left";
    const string OptionValueCls = "menu-option__value";
    const string OptionValueSelectedCls = "menu-option__value--selected";
    const string OptionRightCls = "menu-option__right";

    #endregion

    #region Components and Elements

    // Components
    [SerializeField] SaveManager saveManager;
    [SerializeField] InputManagerSO inputManager;
    [SerializeField] UIDocument uiDoc;

    // UI Elements
    VisualElement _rootEl;
    VisualElement _wrapperEl;
    VisualElement _contentEl;

    #endregion

    #region Settings / State

    readonly List<FullScreenMode> _screenModes = new() {
        FullScreenMode.Windowed,
        FullScreenMode.MaximizedWindow,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.ExclusiveFullScreen
    };

    List<Resolution> _resolutions = new();

    int _optionIndex = 0;
    int _screenModeIndex = -1;
    int _resolutionIndex = -1;
    int _optionsCount = -1;

    #endregion

    #region Events

    EventBinding<OnMenuUpEvent> _upBinding;
    EventBinding<OnMenuDownEvent> _downBinding;
    EventBinding<OnMenuLeftEvent> _leftBinding;
    EventBinding<OnMenuRightEvent> _rightBinding;
    EventBinding<OnMenuConfirmEvent> _confirmBinding;

    #endregion

    #region Lifecycle

    void OnEnable() {
        this.GetElements();
        this.SubsribeToInput();
    }

    void Start() {
        // Dev only
        this.inputManager.ChangeInputMap(ActionMap.Menu);

        // Get the saved settings
        SaveData gameData = this.saveManager.SaveData;
        SaveData.Settings settings = gameData.GameSettings;

        // Add screen mode option
        this._screenModeIndex = this._screenModes.IndexOf(gameData.GameSettings.ScreenMode);

        if (this._screenModeIndex > -1) {
            this._contentEl.Add(this.BuildOption("Screen mode", this._screenModes[this._screenModeIndex].ToString(),
                true));
            this._optionsCount++;
        }

        // Add resolution
        this._resolutions = Screen.resolutions.ToList();

        // See if saved resolution in list
        int resolutionIndex = this._resolutions.FindIndex(r =>
            Mathf.Approximately((float)r.refreshRateRatio.value, (float)settings.Resolution.RefreshRate) &&
            r.width == settings.Resolution.Width &&
            r.height == settings.Resolution.Height);

        // If it is set the index
        if (resolutionIndex > -1)
            this._resolutionIndex = resolutionIndex;
        // If not add the resolution
        else {
            this._resolutions.Add(Screen.currentResolution);
            this._resolutionIndex = this._resolutions.IndexOf(Screen.currentResolution);
        }

        Resolution currentResolution = this._resolutions[this._resolutionIndex];

        this._contentEl.Add(this.BuildOption("Resolution", this.FormatResolutionString(currentResolution)));
        this._optionsCount++;
    }

    void OnDisable() {
        this.UnsubsribeToInput();
    }

    #endregion

    #region Input subsriptions

    void SubsribeToInput() {
        this._upBinding = new EventBinding<OnMenuUpEvent>(this.HandleUp);
        this._downBinding = new EventBinding<OnMenuDownEvent>(this.HandleDown);
        this._leftBinding = new EventBinding<OnMenuLeftEvent>(this.HandleLeft);
        this._rightBinding = new EventBinding<OnMenuRightEvent>(this.HandleRight);
        this._confirmBinding = new EventBinding<OnMenuConfirmEvent>(this.HandleConfirm);


        EventBus<OnMenuUpEvent>.Register(this._upBinding);
        EventBus<OnMenuDownEvent>.Register(this._downBinding);
        EventBus<OnMenuLeftEvent>.Register(this._leftBinding);
        EventBus<OnMenuRightEvent>.Register(this._rightBinding);
        EventBus<OnMenuConfirmEvent>.Register(this._confirmBinding);
    }

    void UnsubsribeToInput() {
        EventBus<OnMenuUpEvent>.Deregister(this._upBinding);
        EventBus<OnMenuDownEvent>.Deregister(this._downBinding);
        EventBus<OnMenuLeftEvent>.Deregister(this._leftBinding);
        EventBus<OnMenuRightEvent>.Deregister(this._rightBinding);
        EventBus<OnMenuConfirmEvent>.Deregister(this._confirmBinding);
    }

    #endregion

    #region Helpers

    void GetElements() {
        this._rootEl = this.uiDoc.rootVisualElement;
        this._wrapperEl = this._rootEl.Q(className: WrapperCls);
        this._contentEl = this._rootEl.Q(className: ContentCls);
    }

    string FormatResolutionString(Resolution resolution) {
        // Access the width, height, and refresh rate directly from the resolution object
        int width = resolution.width;
        int height = resolution.height;
        double refreshRate = resolution.refreshRateRatio.value;

        // Format the refresh rate to two decimal places
        string refreshRateStr = refreshRate.ToString("F2");

        return $"{width} x {height} @ {refreshRateStr}Hz";
    }

    VisualElement BuildOption(string label, string value, bool selected = false) {
        // Root
        VisualElement el = new();
        el.AddToClassList(OptionCls);
        if (selected) el.AddToClassList(OptionSelectedCls);

        // Label
        Label optionLabel = new(label);
        optionLabel.AddToClassList(OptionLabelCls);
        el.Add(optionLabel);

        // Options
        VisualElement options = new();
        options.AddToClassList(OptionOptionsCls);
        el.Add(options);

        // Left
        VisualElement optionsLeft = new();
        optionsLeft.AddToClassList(OptionLeftCls);
        options.Add(optionsLeft);

        // Value
        Label optionsValue = new(value);
        optionsValue.AddToClassList(OptionValueCls);
        options.Add(optionsValue);

        // Right
        VisualElement optionsRight = new();
        optionsRight.AddToClassList(OptionRightCls);
        options.Add(optionsRight);

        return el;
    }

    void ChangeSelectedOption(int newIndex) {
        // Get all options
        var options = this._rootEl.Query(className: OptionCls);

        // Unselect current
        options.AtIndex(this._optionIndex).RemoveFromClassList(OptionSelectedCls);

        // Select new
        options.AtIndex(newIndex).AddToClassList(OptionSelectedCls);

        // Set state
        this._optionIndex = newIndex;
    }

    #endregion

    #region Event Handlers

    void HandleUp(OnMenuUpEvent obj) {
        if (this._optionIndex > 0)
            this.ChangeSelectedOption(this._optionIndex - 1);
        else
            this.ChangeSelectedOption(this._optionsCount);
    }

    void HandleDown(OnMenuDownEvent obj) {
        if (this._optionIndex < this._optionsCount)
            this.ChangeSelectedOption(this._optionIndex + 1);
        else
            this.ChangeSelectedOption(0);
    }

    void HandleLeft(OnMenuLeftEvent obj) {
        // Screen Mode
        if (this._optionIndex == 0) {
            if (this._screenModeIndex == 0)
                this._screenModeIndex = this._screenModes.Count - 1;
            else
                this._screenModeIndex--;

            this.UpdateScreenMode();
        }
        // Resolution
        else if (this._optionIndex == 1) {
            if (this._resolutionIndex == 0)
                this._resolutionIndex = this._resolutions.Count - 1;
            else
                this._resolutionIndex--;

            this.UpdateResolution();
        }
    }

    void HandleRight(OnMenuRightEvent obj) {
        // Screen Mode
        if (this._optionIndex == 0) {
            if (this._screenModes.Count - 1 == this._screenModeIndex)
                this._screenModeIndex = 0;
            else
                this._screenModeIndex++;

            this.UpdateScreenMode();
        }
        // Resolution
        else if (this._optionIndex == 1) {
            if (this._resolutions.Count - 1 == this._resolutionIndex)
                this._resolutionIndex = 0;
            else
                this._resolutionIndex++;

            this.UpdateResolution();
        }
    }

    void UpdateScreenMode() {
        // Find new mode
        FullScreenMode newMode = this._screenModes[this._screenModeIndex];

        // Update text
        this._rootEl.Query<Label>(classes: OptionValueCls).AtIndex(0).text = newMode.ToString();

        // Update screen mode
        Screen.fullScreenMode = newMode;

        // Save change
        this.saveManager.UpdateSingleField(SaveField.ScreenMode, newMode);
        this.saveManager.SaveGameData();
    }

    void UpdateResolution() {
        // Find new mode
        Resolution newResolution = this._resolutions[this._resolutionIndex];

        // Update text
        this._rootEl.Query<Label>(classes: OptionValueCls).AtIndex(1).text = this.FormatResolutionString(newResolution);

        // Update screen mode
        Screen.SetResolution(
            newResolution.width,
            newResolution.height,
            Screen.fullScreenMode,
            newResolution.refreshRateRatio
        );

        // Save change
        SaveData.SerializableResolution serializableResolution = new(newResolution);
        this.saveManager.UpdateSingleField(SaveField.Resolution, serializableResolution);
        this.saveManager.SaveGameData();
    }

    void HandleConfirm(OnMenuConfirmEvent obj) { }

    #endregion
}