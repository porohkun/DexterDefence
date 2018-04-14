using System;
using System.Collections;
using UnityEngine;

namespace Layers
{
    public class StartLayer : LayerBase
    {
        void Start()
        {
            LayersManager.Push<MainMenuLayer>();
            LayersManager.FadeIn(0.5f, null);
        }

    }
}
