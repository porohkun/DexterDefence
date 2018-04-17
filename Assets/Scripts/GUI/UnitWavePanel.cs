using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UnitWavePanel : MonoBehaviour, IWaveContentPanel
{
    [Serializable]
    public class UnitImageData
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private Sprite _sprite;

        public string Name { get { return _name; } }
        public Sprite Sprite { get { return _sprite; } }
    }

    [SerializeField]
    private Image _unitImage;
    [SerializeField]
    private UnitImageData[] _imageData;
    [SerializeField]
    private InputField _countInput;
    [SerializeField]
    private InputField _intervalInput;

    public event Action<IWaveContentPanel> WantBeRemoved;

    private string _unitName;

    public void Configure(int count, float interval)
    {
        _countInput.text = count.ToString();
        _intervalInput.text = interval.ToString();
    }

    public Game.IWaveItem GetWaveItem()
    {
        int count;
        if (!int.TryParse(_countInput.text, out count))
            count = 1;
        float interval;
        if (!float.TryParse(_intervalInput.text, out interval))
            interval = 0.5f;
        return new Game.UnitWaveItem(_unitName, count, interval);
    }

    public void Remove()
    {
        if (WantBeRemoved != null)
            WantBeRemoved(this);
    }

    internal void SetUnit(string unit)
    {
        _unitName = unit;
        foreach (var image in _imageData)
            if (image.Name == unit)
                _unitImage.sprite = image.Sprite;
    }
}
