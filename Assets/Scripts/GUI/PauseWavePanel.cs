using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PauseWavePanel : MonoBehaviour, IWaveContentPanel
{
    [SerializeField]
    private InputField _pauseInput;

    public event Action<IWaveContentPanel> WantBeRemoved;

    public void Configure(float interval)
    {
        _pauseInput.text = interval.ToString();
    }

    public Game.IWaveItem GetWaveItem()
    {
        float interval;
        if (!float.TryParse(_pauseInput.text, out interval))
            interval = 0.5f;
        return new Game.PauseWaveItem(interval);
    }

    public void Remove()
    {
        if (WantBeRemoved != null)
            WantBeRemoved(this);
    }
}
