using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Rupees : VisualElement
{
    int value = 0;
    Label text;

    [UxmlAttribute("value")]
    public int Value
    {
        get { return value; }
        set
        {
            this.value = value;
            Refresh();
        }
    }

    public Rupees()
    {
        style.flexDirection = FlexDirection.Row;
        style.flexGrow = 0;
        style.alignItems = Align.Center;

        VisualElement icon = new VisualElement();
        icon.style.width = 17;
        icon.style.height = 28;
        icon.style.flexGrow = 0;
        icon.style.marginRight = 10;
        icon.style.backgroundColor = new Color(0.14f, 0.78f, 0.29f);
        icon.style.borderTopLeftRadius = 4;
        icon.style.borderTopRightRadius = 4;
        icon.style.borderBottomLeftRadius = 10;
        icon.style.borderBottomRightRadius = 10;
        icon.style.borderTopWidth = 1;
        icon.style.borderRightWidth = 1;
        icon.style.borderBottomWidth = 1;
        icon.style.borderLeftWidth = 1;
        icon.style.borderTopColor = new Color(0.7f, 1.0f, 0.72f, 0.7f);
        icon.style.borderRightColor = new Color(0.7f, 1.0f, 0.72f, 0.7f);
        icon.style.borderBottomColor = new Color(0.7f, 1.0f, 0.72f, 0.7f);
        icon.style.borderLeftColor = new Color(0.7f, 1.0f, 0.72f, 0.7f);

        text = new Label();
        text.style.color = new Color(0.96f, 0.97f, 0.88f);
        text.style.fontSize = 26;
        text.style.unityFontStyleAndWeight = FontStyle.Bold;
        text.style.flexGrow = 0;

        Add(icon);
        Add(text);
        Refresh();
    }

    void Refresh()
    {
        text.text = value.ToString("N0", CultureInfo.InvariantCulture);
    }
}
