using System;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VentLib.Utilities.Extensions;
using Object = UnityEngine.Object;

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

    public static SpriteRenderer CustomBackground;
    private static int numOptions = 0;

    // for buttons
    private OptionsMenuItem(string label, string objectName, Func<bool> getValue, Action<bool> setValue, OptionsMenuBehaviour optionsMenuBehaviour, Action additionalOnClickAction = null)    
    {
        try
        {
            this.getValue = getValue;
            this.setValue = setValue;
            Label = label;

            var mouseMoveToggle = optionsMenuBehaviour.DisableMouseMovement;
            

            // Create menu if null
            if (CustomBackground == null) CreateOptionsMenu(optionsMenuBehaviour, mouseMoveToggle);
            
            // Generate Buttons (google translated)
            ToggleButton = Object.Instantiate(mouseMoveToggle, CustomBackground.transform);
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

            // Create menu if null
            if (CustomBackground == null) CreateOptionsMenu(optionsMenuBehaviour, mouseMoveToggle);

            // sliders take a whole row, if there's only 1 button, we just skip
            if (numOptions % 2 != 0) numOptions++;

            SlideBar = Object.Instantiate(musicSlider, CustomBackground.transform);

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

    public static void CreateOptionsMenu(OptionsMenuBehaviour optionsMenuBehaviour, ToggleButtonBehaviour mouseMoveToggle)
    {
        behaviour = optionsMenuBehaviour;
        numOptions = 0;
        CustomBackground = Object.Instantiate(optionsMenuBehaviour.Background, optionsMenuBehaviour.transform);
        CustomBackground.name = "FlashlightUsOptionsMenu";
        CustomBackground.transform.localScale = new(0.9f, 0.9f, 1f);
        CustomBackground.transform.localPosition += Vector3.back * 8;
        CustomBackground.gameObject.SetActive(false);

        var closeButton = Object.Instantiate(mouseMoveToggle, CustomBackground.transform);
        closeButton.transform.localPosition = new(1.3f, -2.3f, -6f);
        closeButton.name = "Close";
        closeButton.Text.text = Translations.OptionsMenu.Close;
        closeButton.Background.color = Palette.DisabledGrey;
        var closePassiveButton = closeButton.GetComponent<PassiveButton>();
        closePassiveButton.OnClick = new();
        closePassiveButton.OnClick.AddListener(new Action(() => { CustomBackground.gameObject.SetActive(false); }));

        UiElement[] selectableButtons = optionsMenuBehaviour.ControllerSelectable.ToArray();
        PassiveButton leaveButton = null;
        PassiveButton returnButton = null;
        for (int i = 0; i < selectableButtons.Length; i++)
        {
            var button = selectableButtons[i];
            if (button == null)
            {
                continue;
            }

            if (button.name == "LeaveGameButton")
            {
                leaveButton = button.GetComponent<PassiveButton>();
            }
            else if (button.name == "ReturnToGameButton")
            {
                returnButton = button.GetComponent<PassiveButton>();
            }
        }

        var generalTab = mouseMoveToggle.transform.parent.parent.parent;

        var modOptionsButton = Object.Instantiate(mouseMoveToggle, generalTab);
        modOptionsButton.transform.localPosition = new(-4f, 2f, -6f);
        modOptionsButton.name = "ModOptionsButton";
        modOptionsButton.Text.text = Translations.ModName;
        modOptionsButton.Background.color = Color.yellow;
        var modOptionsPassiveButton = modOptionsButton.GetComponent<PassiveButton>();
        modOptionsPassiveButton.OnClick = new();
        modOptionsPassiveButton.OnClick.AddListener(new Action(() => { CustomBackground.gameObject.SetActive(true); }));

        if (leaveButton != null)
        {
            leaveButton.transform.localPosition = new(-1.35f, -2.411f, -1f);
        }

        if (returnButton != null)
        {
            returnButton.transform.localPosition = new(1.35f, -2.411f, -1f);
        }
    }
    
    public static void OpenMenu()
    {
        if (behaviour == null)
        {
            log.Warn("Behaviour is null, not opening menu.");
            return;
        }
        
        if (CustomBackground == null)
        {
            log.Exception("CustomBackground is null, not opening menu.");
            return;
        };
        behaviour.gameObject.SetActive(true);
        CustomBackground.gameObject.SetActive(true);
    }
    
    public static void CloseMenu()
    {
        if (behaviour == null)
        {
            log.Warn("Behaviour is null, can't close menu.");
            return;
        }
        
        if (CustomBackground == null)
        {
            log.Exception("CustomBackground is null, can't clos menu.");
            return;
        };
        behaviour.gameObject.SetActive(false);
        CustomBackground.gameObject.SetActive(false);
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