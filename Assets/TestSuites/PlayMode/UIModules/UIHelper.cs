using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper {
    public Canvas canvas { get; }

    public UIHelper() {
        canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.gameObject.AddComponent<CanvasScaler>();
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        
    }
    
    public Image CreateImage(string imageName) {
        var imageGo = new GameObject("imageName");
        imageGo.transform.SetParent(canvas.transform);
        return imageGo.AddComponent<Image>();
    }

    public Transform CreateContentWithDefaultItem(string contentName) {
        var content = CreateTransform(contentName);
        var defaultItem = new GameObject("DefaultItem").AddComponent<TextMeshPro>();
        defaultItem.transform.SetParent(content.transform);
        return content;
    }
    
    public Transform CreateTransform(string viewName) {
        return new GameObject(viewName).transform;
    }
    
    public TMP_Dropdown CreateDropdown(string dropdownName) {
        var dropdown = new GameObject(dropdownName).AddComponent<TMP_Dropdown>();
        return dropdown;
    }

    public Toggle CreateToggle(string name, bool isOn = false)
    {
        var toggle = new GameObject(name).AddComponent<Toggle>();
        toggle.isOn = isOn;
        toggle.group = canvas.gameObject.AddComponent<ToggleGroup>();
        return toggle;
    }

    public TMP_InputField CreateInputField(string name)
    {
        var inputGo = new GameObject(name);
        inputGo.transform.SetParent(canvas.transform);
        return inputGo.AddComponent<TMP_InputField>();
    }
    
    public TextMeshPro CreateText(string name)
    {
        var inputGo = new GameObject(name);
        inputGo.transform.SetParent(canvas.transform);
        return inputGo.AddComponent<TextMeshPro>();
    }

    public Button CreateButton(string name)
    {
        var buttonGo = new GameObject(name);
        buttonGo.transform.SetParent(canvas.transform);
        var label = new GameObject("Label").AddComponent<TextMeshPro>();
        label.transform.SetParent(buttonGo.transform);
        return buttonGo.AddComponent<Button>();
    }
}