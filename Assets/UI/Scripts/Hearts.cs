using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Hearts : VisualElement
{
    int current = 3;
    int max = 5;
    string path = "Images/icons/heart";

    List<VisualElement> hearts = new List<VisualElement>();

    [UxmlAttribute("current-lifes")]
    public int Current
    {
        get { return current; }
        set
        {
            current = value;

            if (current < 0) current = 0;
            else if (current > max) current = max;

            Refresh();
        }
    }

    [UxmlAttribute("max-lifes")]
    public int Max
    {
        get { return max; }
        set
        {
            max = value;

            if (max < 0) max = 0;
            else if (current > max) current = max;

            Refresh();
        }
    }

    [UxmlAttribute("icon-path")]
    public string Path
    {
        get { return path; }
        set
        {
            path = value;
            Refresh();
        }
    }

    public Hearts()
    {
        style.flexDirection = FlexDirection.Row;
        style.flexGrow = 0;
        Refresh();
    }

    public void Refresh()
    {
        if (hearts.Count != max)
        {
            Clear();
            hearts.Clear();

            for (int i = 0; i < max; i++)
            {
                VisualElement heart = new VisualElement();
                heart.style.width = 18;
                heart.style.height = 18;
                heart.style.marginRight = 3;
                heart.style.flexGrow = 0;
                heart.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                heart.style.backgroundImage = Resources.Load<Texture2D>(path);

                hearts.Add(heart);
                Add(heart);
            }
        }

        for (int i = 0; i < max; i++)
        {
            if (i < current) hearts[i].style.opacity = 1.0f;
            else hearts[i].style.opacity = 0.22f;
        }
    }
}
