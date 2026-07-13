#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using FlashlightUs.UI.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VentLib;
using VentLib.Utilities;
using VentLib.Utilities.Attributes;
using VentLib.Utilities.Extensions;

namespace FlashlightUs.UI;

// based on: https://github.com/Lotus-AU/LotusContinued/blob/8a00694ae082339c9abad299945ce1f1522be74c/src/GUI/Menus/ModUpdateMenu.cs
// im starting to realize how complicated lotus is - theta, 5:12 am
[RegisterInIl2Cpp]
public class ModUpdateMenu: MonoBehaviour
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(ModUpdateMenu));
    
    public GameObject AnchorObj;
    public GameObject ItemContainer;

    public GenericPopup UpdateBackground;
    
    public PassiveButton UpdateButton;
    public PassiveButton ExitButton;

    public GameObject cloneme;

    private bool isOpen;

    private bool DoneUpdating;
    
    private static List<UpdateItem> _updateItems = new();
    
    public ModUpdateMenu(IntPtr intPtr) : base(intPtr)
    {
        var a = new GameObject("ModUpdateMenu");
        a.transform.SetParent(gameObject.transform);
        AnchorObj = a;
        
        AnchorObj.SetActive(false);
        
        UpdateBackground = Instantiate(OptionsMenuItem.popup2, AnchorObj.transform);
        
        UpdateBackground.gameObject.SetActive(true);
        UpdateBackground.transform.SetLocalZ(-740);
        UpdateBackground.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
        UpdateBackground.name = "Updater";
        UpdateBackground.TextAreaTMP.transform.localPosition = new Vector3(-0.725f, 0.75f, -1f);
        UpdateBackground.TextAreaTMP.text = "FlashlightUs Updater";
        
        ItemContainer = new GameObject("ItemContainer");
        ItemContainer.transform.SetParent(UpdateBackground.transform, false);
        ItemContainer.transform.localPosition = new Vector3(-0.6125f, 0.8f, -1f);
        ItemContainer.transform.localScale = new Vector3(1f, 1f, 1f);

        var exitTransform = UpdateBackground.transform.Find("ExitGame");
        ExitButton = exitTransform.GetComponent<PassiveButton>();
        
        ExitButton.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        ExitButton.transform.localPosition = new Vector3(0.9f, -0.7f, -1f);
        

        UpdateButton = Instantiate(ExitButton, UpdateBackground.transform);
        UpdateButton.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        UpdateButton.transform.localPosition = new Vector3(0.9f, -0.45f, -1f);
        UpdateButton.name = "UpdateButton";
        UpdateButton.OnClick = new Button.ButtonClickedEvent();
        UpdateButton.OnClick.AddListener(new Action(() =>
        {
            if (DoneUpdating)
            {
                Application.Quit(0);
                return;
            }
            
            log.Info("Updating mods");
            foreach (UpdateItem item in _updateItems)
            {
                log.Info($"Updating {item.Name}");
                if (item.UpdateState == UpdateState.Updating) return;
                item.UpdateFunction();
            }
            log.Info("Mods updated");
            DoneUpdating = true;
            UpdateButton.GetComponentInChildren<TextMeshPro>().text = "Exit Game";
        }));
        
        ExitButton.GetComponentInChildren<TextTranslatorTMP>().DestroyImmediate();
        UpdateButton.GetComponentInChildren<TextTranslatorTMP>().DestroyImmediate();

        cloneme = UpdateBackground.transform.Find("Text_TMP").gameObject;
        
        _updateItems.ForEach(CreateRow);
    }
    
    internal void CreateRow(UpdateItem item)
    {
        GameObject rowObj = Instantiate(cloneme, ItemContainer.transform);
        rowObj.transform.SetParent(ItemContainer.transform, false);
        rowObj.transform.localPosition = new Vector3(0f, -0.3f * ItemContainer.transform.childCount, 0f);

        TextMeshPro label = rowObj.GetComponent<TextMeshPro>();
        label.alignment = TextAlignmentOptions.BaselineLeft;
        label.fontSize = 1.5f;
        label.text = $"- <u>{item.Name}: {item.Version ?? "?"} - Pending</u>";

        item.Label = label;
    }
    
    public void Open()
    {
        AnchorObj.SetActive(isOpen = true);
        UpdateBackground.gameObject.SetActive(true);
        
        UpdateButton.GetComponentInChildren<TextMeshPro>().text = "Update";
        ExitButton.GetComponentInChildren<TextMeshPro>().text = "Exit";
    }
    
    public void Close()
    {
        AnchorObj.SetActive(isOpen = false);
        UpdateBackground.gameObject.SetActive(false);
    }
    
    public static void AddUpdateItem(string name, string? version, UpdateDelegate updateDelegate, bool autoStartUpdate = false)
    {
        UpdateItem item = new() { Name = name, Version = version, AutoStartUpdate = autoStartUpdate };
        _updateItems.Add(item.BindUpdateFunction(updateDelegate));
        
        ModUpdaterPatches.ModUpdateMenu?.CreateRow(item);
        
        if (autoStartUpdate) item.UpdateFunction();
    }
    
    internal class UpdateItem
    {
        public string Name = null!;
        public string? Version;
        public Func<Progress<float>> UpdateFunction = null!;
        public UpdateState UpdateState = UpdateState.None;
        public bool AutoStartUpdate;
        public TextMeshPro? Label;

        public UpdateItem BindUpdateFunction(UpdateDelegate updateDelegate)
        {
            UpdateFunction = () =>
            {
                Progress<float> bar = updateDelegate(HandleException);
                UpdateState = UpdateState.Updating;
                RefreshLabel(); 
                bar.ProgressChanged += (_, f) =>
                {
                    if (f >= 1) UpdateState = UpdateState.Complete;
                    RefreshLabel();
                };
                return bar;
            };
            return this;
        }

        public void RefreshLabel()
        {
            if (Label == null) return;
            string stateText = UpdateState switch
            {
                UpdateState.Updating => "Updating...",
                UpdateState.Complete => "Done",
                _ => "Pending"
            };
            Color color = UpdateState switch
            {
                UpdateState.Updating => Color.yellow,
                UpdateState.Complete => Color.green,
                _ => Color.white
            };
            Label.text = color.Colorize($"- <u>{Name}: {Version ?? "?"} - {stateText}</u>");
        }

        public void HandleException(Exception exception)
        {
            UpdateState = UpdateState.None;
            RefreshLabel();
            log.Exception($"Update failed: {exception}");
        }
    }

    internal enum UpdateState
    {
        None,
        Updating,
        Complete
    }
    
    public delegate Progress<float> UpdateDelegate(Action<Exception> exception);
}