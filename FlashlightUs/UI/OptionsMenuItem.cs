using System;
using System.Collections.Generic;
using FlashlightUs.UI.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VentLib.Utilities.Extensions;


namespace FlashlightUs.UI;

// https://github.com/Hyz-sui/TownOfHost-H/blob/e30daf2f851eb0dc4f8b23556227dcd7406ac4dc/Modules/OptionsMenu.cs
// TODO: Re-create this or refactor in order to support more advanced mods
public class OptionsMenuItem
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(FlashlightUsPlugin));
    public ToggleButtonBehaviour ToggleButton;
    public SlideBar SlideBar;
    public string Label;
    
    private readonly Func<bool> getValue;
    private readonly Action<bool> setValue;
    
    private readonly Func<float> getFloatValue;
    private readonly Action<float> setFloatValue;
    private readonly FloatRange floatRange;
    
    private static OptionsMenuBehaviour behaviour;
    
    public static GenericPopup popup2;
    public static GenericPopup OptionsBaseMenu;
    public static PassiveButton MenuButton;
    
    public static PassiveButton modOptionsButtonV2;
    
    private static int numOptions = 0;
    
    private static OptionsMenuBehaviour builtFor;
    private static readonly List<GameObject> spawnedItems = new();

    // for buttons
    private OptionsMenuItem(string label, string objectName, Func<bool> getValue, Action<bool> setValue, OptionsMenuBehaviour optionsMenuBehaviour, Action additionalOnClickAction = null)    
    {
        try
        {
            this.getValue = getValue;
            this.setValue = setValue;
            Label = label;

            var mouseMoveToggle = optionsMenuBehaviour.DisableMouseMovement;
            
            CheckMenu(optionsMenuBehaviour);
            
            // Generate Buttons (google translated)
            ToggleButton = Object.Instantiate(mouseMoveToggle, OptionsBaseMenu.transform);
            ToggleButton.gameObject.SetActive(true);
            spawnedItems.Add(ToggleButton.gameObject);
            ToggleButton.transform.localPosition = new Vector3(
                // Calculate the position based on the current number of options. (google translated)
                numOptions % 2 == 0 ? -1.3f : 1.3f,
                2.2f - (0.5f * (numOptions / 2)),
                -6f);
            ToggleButton.name = objectName;
            ToggleButton.Text.text = label;
            var passiveButton = ToggleButton.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener(new Action(() =>
            {
                setValue(!getValue());
                UpdateToggle();
                additionalOnClickAction?.Invoke();
            }));
            UpdateToggle();
        }
        finally
        {
            numOptions++;
        }
    }
    
    // sliders
    private OptionsMenuItem(string label, string objectName, Func<float> getValue, Action<float> setValue, FloatRange range, 
        OptionsMenuBehaviour optionsMenuBehaviour, float? snapStep = null)
    {
        try
        {
            getFloatValue = getValue;
            setFloatValue = setValue;
            floatRange = range;
            Label = label;

            var mouseMoveToggle = optionsMenuBehaviour.DisableMouseMovement;
            var musicSlider = optionsMenuBehaviour.MusicSlider;
            
            CheckMenu(optionsMenuBehaviour);

            // sliders take a whole row, if there's only 1 button, we just skip
            if (numOptions % 2 != 0) numOptions++;

            SlideBar = Object.Instantiate(musicSlider, OptionsBaseMenu.transform);
            spawnedItems.Add(SlideBar.gameObject);

            SlideBar.GetComponentInChildren<TextTranslatorTMP>(true)?.DestroyImmediate();

            SlideBar.transform.localPosition = new Vector3(-2.1f, 2.2f - (0.5f * (numOptions / 2)), -6f);
            SlideBar.name = objectName;

            SlideBar.OnValueChange = new UnityEvent();
            SlideBar.Value = range.ReverseLerp(getValue());
            SlideBar.UpdateValue();
            UpdateSlider();

            SlideBar.OnValueChange.AddListener(new Action(() =>
            {
                float real = floatRange.Lerp(SlideBar.Value);

                // if snap then snap
                if (snapStep.HasValue && snapStep.Value > 0f)
                {
                    real = Mathf.Round(real / snapStep.Value) * snapStep.Value;
                    SlideBar.Value = floatRange.ReverseLerp(real);
                    SlideBar.UpdateValue();
                    UpdateSlider();
                }
                
                SetTitle(SlideBar, Label, real);
                setFloatValue(real);
            }));
            numOptions++;
        }
        finally
        {
            numOptions++;
        }
    }
    
    private static void CheckMenu(OptionsMenuBehaviour optionsMenuBehaviour)
    {
        log.Trace($"CheckMenu called. builtFor == optionsMenuBehaviour: {builtFor == optionsMenuBehaviour}");

        if (builtFor == optionsMenuBehaviour) return;

        foreach (var go in spawnedItems)
        {
            if (go != null) Object.Destroy(go);
        }
        spawnedItems.Clear();
        numOptions = 0;
        
        if (modOptionsButtonV2 != null) Object.Destroy(modOptionsButtonV2.gameObject);
        modOptionsButtonV2 = null;
        
        builtFor = optionsMenuBehaviour;

        CreateOptionsMenuV2(optionsMenuBehaviour);
    }
    
    public static void CreateOptionsMenuV2(OptionsMenuBehaviour optionsMenuBehaviour)
    {
        if (!OptionsBaseMenu)
        {
            log.Warn("OptionsBaseMenu is null, creating new one.");
            Utilities.RunWithLogging(
                () => OptionsBaseMenu = Object.Instantiate(popup2, optionsMenuBehaviour.transform), 
                "Instantiated OptionsBaseMenu from popup2");
        }

        behaviour = optionsMenuBehaviour;
        OptionsBaseMenu.name = "FlashlightUsOptionsMenu";
        
        var exitButton = OptionsBaseMenu.transform.Find("ExitGame");
        exitButton.transform.localPosition = new Vector3(1.8f, -0.6f, -1f);
        
        var bg = OptionsBaseMenu.transform.Find("Background");
        OptionsBaseMenu.gameObject.transform.SetParent(
            HudManager.InstanceExists ? HudManager.Instance.transform : optionsMenuBehaviour.transform, false);
        OptionsBaseMenu.transform.SetLocalZ(HudManager.InstanceExists ? -905f : -10f);
        bg.localScale = new Vector3(1.9f, 1.9f, 1f);
        bg.localPosition = new Vector3(0f, 0.8f, 1f);
        
        TryCreateModOptionsButton(optionsMenuBehaviour);

        if (FlashlightUsPlugin.ModUpdater.HasUpdate)
        {
            if (PlayerControl.LocalPlayer != null) return;
            
            var updateBtn = Object.Instantiate(exitButton, OptionsBaseMenu.transform);
            updateBtn.GetComponentInChildren<TextTranslatorTMP>().DestroyImmediate();
            updateBtn.GetComponentInChildren<TextMeshPro>().text = Translations.ModUpdater.UpdateMod;
            var actualBtn = updateBtn.GetComponent<PassiveButton>();
            
            exitButton.transform.localPosition = new Vector3(-1.8f, -0.6f, -1f);
            updateBtn.transform.localPosition = new Vector3(1.8f, -0.6f, -1f);

            actualBtn.OnClick = new Button.ButtonClickedEvent();
            actualBtn.OnClick.AddListener(new Action(() => ModUpdaterPatches.ModUpdateMenu.Open()));
        }
        
        UiElement[] selectableButtons = optionsMenuBehaviour.ControllerSelectable.ToArray();
        PassiveButton leaveButton = null;
        PassiveButton returnButton = null;
        
        selectableButtons.ForEach(button =>
        {
            if (button == null) return;
            if (button.name == "LeaveGameButton") leaveButton = button.GetComponent<PassiveButton>();
            else if (button.name == "ReturnToGameButton") returnButton = button.GetComponent<PassiveButton>();
        });
        if (leaveButton != null) leaveButton.transform.localPosition = new Vector3(-1.35f, -2.411f, -1f);
        if (returnButton != null) returnButton.transform.localPosition = new Vector3(1.35f, -2.411f, -1f);
    }

    public static void OpenMenu()
    {
        if (!OptionsBaseMenu) CheckMenu(behaviour);
        OptionsBaseMenu.Show();
    }

    public static void CloseMenu()
    {
        if (!OptionsBaseMenu) CheckMenu(behaviour);
        OptionsBaseMenu.Close();
    }

    public static void TryCreateModOptionsButton(OptionsMenuBehaviour optionsMenuBehaviour)
    {
        var startButton = MenuButton;
        if (modOptionsButtonV2 != null)
        {
            StaticLogger.Warn("ModOptionsButton already exists");
            return;
        }
        modOptionsButtonV2 = Object.Instantiate(startButton, optionsMenuBehaviour.transform);
        modOptionsButtonV2.gameObject.SetActive(true);
        
        Vector3 pos = Utilities.IsLotusLoaded() ? new Vector3(-3.37f, -1.75f, -10f) : new Vector3(4.15f, -2.65f, -10f);
        Vector3 scale = Utilities.IsLotusLoaded() ? new Vector3(0.5f, 0.5f, 1f) : new Vector3(0.6f, 0.6f, 1f);

        modOptionsButtonV2.transform.localPosition = pos;
        modOptionsButtonV2.transform.localScale = scale;
        modOptionsButtonV2.name = "ModOptionsButton";
        
        modOptionsButtonV2.GetComponentInChildren<TextTranslatorTMP>(true)?.DestroyImmediate();
        modOptionsButtonV2.buttonText.text = Translations.ModName;
        
        modOptionsButtonV2.inactiveSprites.FindChild<SpriteRenderer>("Shine", true)?.gameObject.SetActive(false);
        modOptionsButtonV2.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(1f, 0.750f, 0.500f, 1f);
        modOptionsButtonV2.inactiveSprites.FindChild<SpriteRenderer>("Icon").sprite = null;
        
        modOptionsButtonV2.activeSprites.FindChild<SpriteRenderer>("Shine", true)?.gameObject.SetActive(false);
        modOptionsButtonV2.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, 1f, 0.6f, 1f);
        modOptionsButtonV2.activeSprites.FindChild<SpriteRenderer>("Icon").sprite = null;

        var modOptionsPassiveButton = modOptionsButtonV2.GetComponent<PassiveButton>();
        modOptionsPassiveButton.OnClick = new();
        modOptionsPassiveButton.OnClick.AddListener(new Action(() =>
        {
            if (OptionsBaseMenu.gameObject.activeSelf) CloseMenu();
            else OpenMenu();
        }));
    }

    public static OptionsMenuItem Create(string label, string objectName, Func<bool> getValue, Action<bool> setValue,
        OptionsMenuBehaviour optionsMenuBehaviour, Action additionalOnClickAction = null)
    {
        return new(label, objectName, getValue, setValue, optionsMenuBehaviour, additionalOnClickAction);
    }
    
    public static OptionsMenuItem CreateSlider(string label, string objectName, Func<float> getValue, Action<float> setValue,
        FloatRange range, OptionsMenuBehaviour optionsMenuBehaviour, float? snapStep = null)
    {
        return new OptionsMenuItem(label, objectName, getValue, setValue, range, optionsMenuBehaviour, snapStep);
    }

    public void UpdateToggle()
    {
        if (ToggleButton == null) return;

        var color = getValue() ? Color.green : Color.red;
        ToggleButton.Background.color = color;
        if (ToggleButton.Rollover != null)
        {
            ToggleButton.Rollover.ChangeOutColor(color);
        }
    }
    
    public void UpdateSlider()
    {
        if (SlideBar == null) return;
        
        SetTitle(SlideBar, Label, getFloatValue());
        SlideBar.Value = floatRange.ReverseLerp(getFloatValue());
        SlideBar.UpdateValue();
    }

    private void SetTitle(SlideBar slider, string title, float value)
    {
        slider.gameObject.GetComponentInChildren<TextTranslatorTMP>(true)?.DestroyImmediate();
        var titleText = slider.gameObject.GetComponentInChildren<TextMeshPro>(true);
        if (titleText == null) return;

        titleText.text = $"{title}\n{value:0.00}x";
    }
}