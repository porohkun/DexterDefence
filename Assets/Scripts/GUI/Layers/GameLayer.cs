using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Game;
using MimiJson;

namespace Layers
{
    public class GameLayer : LayerBase
    {
        [SerializeField]
        private GameController _gameControllerPrefab;

        private GameController _controller;

        public void Initialize(string mapName)
        {
            var json = JsonValue.Parse(Resources.Load<TextAsset>("Maps/" + mapName).text);

            _controller = Instantiate(_gameControllerPrefab);
            _controller.StartGame(json);
        }
    }
}
