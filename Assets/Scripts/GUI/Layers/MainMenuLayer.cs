using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MimiJson;
using System.IO;
using System.Collections.Generic;

namespace Layers
{
    public class MainMenuLayer : LayerBase
    {
        public override HidingType Hiding { get { return HidingType.HideAll; } }

        [SerializeField]
        private Transform _mapListRoot;
        [SerializeField]
        private ToggleGroup _mapListToggleGroup;
        [SerializeField]
        private MapToggle _mapListItemPrefab;

        private JsonValue _mapsConfig;
        private List<MapToggle> _mapList = new List<MapToggle>();

        internal override void OnFloatUp()
        {
            base.OnFloatUp();
            LoadMaps();
        }

        private void LoadMaps()
        {
            _mapsConfig = JsonValue.Parse(Resources.Load<TextAsset>("maps").text);

            foreach (var mapListItem in _mapList)
                Destroy(mapListItem.gameObject);
            _mapList.Clear();
            foreach (var mapname in _mapsConfig["maps"])
            {
                var mapListItem = Instantiate(_mapListItemPrefab);
                mapListItem.Text = mapname;
                mapListItem.SetGroup(_mapListToggleGroup);
                mapListItem.transform.SetParent(_mapListRoot);
                mapListItem.transform.localScale = Vector3.one;
                _mapList.Add(mapListItem);
            }
        }

        public void OnPlay()
        {
            LayersManager.FadeOut(0.5f, () =>
            {
                //LayersManager.Push<GameLayer>().Initialize();
                LayersManager.FadeIn(0.5f, () =>
                {
                });
            });
        }
    }
}
