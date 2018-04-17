using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;

public interface IWaveContentPanel
{
    GameObject gameObject { get; }

    IWaveItem GetWaveItem();
}
