using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MapToggle : MonoBehaviour
{
    [SerializeField]
    private Toggle _toggle;
    [SerializeField]
    private Text _content;

    public bool Selected { get { return _toggle.isOn; } }
    public string Text
    {
        get { return _content.text; }
        set { _content.text = value; }
    }

    public void SetGroup(ToggleGroup group)
    {
        _toggle.group = group;
    }
}
