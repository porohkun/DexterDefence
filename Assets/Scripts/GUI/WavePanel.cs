using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WavePanel : MonoBehaviour
{
    [SerializeField]
    private Text _waveName;
    [SerializeField]
    private UnitWavePanel _unitPanelPrefab;
    [SerializeField]
    private PauseWavePanel _pausePanelPrefab;
    [SerializeField]
    private RectTransform _panelsRoot;

    public event Action<WavePanel> WantBeRemoved;

    private List<IWaveContentPanel> _panels = new List<IWaveContentPanel>();

    public IWaveContentPanel this[int i] { get { return _panels[i]; } }

    public void SetWaveIndex(int i)
    {
        _waveName.text = "Wave " + i;
    }

    public Game.Wave GetWave()
    {
        var wave = new Game.Wave();
        foreach (var panel in _panels)
            wave.Items.Add(panel.GetWaveItem());
        return wave;
    }

    public void CreateUnit(string unit)
    {
        var panel = Instantiate(_unitPanelPrefab, _panelsRoot);
        panel.SetUnit(unit);
        _panels.Add(panel);
        panel.WantBeRemoved += Panel_WantBeRemoved;
    }

    public void CreatePause()
    {
        var panel = Instantiate(_pausePanelPrefab, _panelsRoot);
        _panels.Add(panel);
        panel.WantBeRemoved += Panel_WantBeRemoved;
    }

    private void Panel_WantBeRemoved(IWaveContentPanel panel)
    {
        _panels.Remove(panel);
        Destroy(panel.gameObject);
    }

    public void Remove()
    {
        if (WantBeRemoved != null)
            WantBeRemoved(this);
    }
}
