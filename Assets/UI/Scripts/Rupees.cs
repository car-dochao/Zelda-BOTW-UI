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
        icon.style.width = 18;
        icon.style.height = 34;
        icon.style.flexGrow = 0;
        icon.style.marginRight = 10;
        icon.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
        icon.style.backgroundImage = Resources.Load<Texture2D>("Images/icons/rupee");

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
