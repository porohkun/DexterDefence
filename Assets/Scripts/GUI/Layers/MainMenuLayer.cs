using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MimiJson;
using System.IO;
using System.Collections.Generic;
using System.Collections;

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
        [SerializeField]
        private WavePanel _wavePanelPrefab;
        [SerializeField]
        private RectTransform _wavePanelRoot;
        [SerializeField]
        private InputField _startMoney;
        [SerializeField]
        private InputField _lives;

        private JsonValue _mapsConfig;
        private List<MapToggle> _mapList = new List<MapToggle>();
        private List<WavePanel> _waves = new List<WavePanel>();

        private void Start()
        {
            base.OnFloatUp();
            LoadMaps();
            InitializeWaves();
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

        private void InitializeWaves()
        {
            OnAddWave();
            _waves[0].CreateUnit("unit0");
            (_waves[0][0] as UnitWavePanel).Configure(10, 0.5f);
            _waves[0].CreateUnit("unit1");
            (_waves[0][1] as UnitWavePanel).Configure(13, 0.5f);
            _waves[0].CreatePause();
            (_waves[0][2] as PauseWavePanel).Configure(5f);
            _waves[0].CreateUnit("unit2");
            (_waves[0][3] as UnitWavePanel).Configure(10, 1f);
        }

        private void Wave_WantBeRemoved(WavePanel wave)
        {
            _waves.Remove(wave);
            Destroy(wave.gameObject);
            for (int i = 0; i < _waves.Count; i++)
                _waves[i].SetWaveIndex(i + 1);
        }

        public void OnPlay()
        {
            foreach (var map in _mapList)
                if (map.Selected)
                {
                    int startMoney;
                    if (!int.TryParse(_startMoney.text, out startMoney))
                        startMoney = 4000;
                    int lives;
                    if (!int.TryParse(_lives.text, out lives))
                        lives = 20;
                    LayersManager.FadeOut(0.5f, () =>
                    {
                        LayersManager.Push<GameLayer>().Initialize(map.Text, _waves.Select(w => w.GetWave()).ToArray(), startMoney, lives);
                        LayersManager.FadeIn(0.5f, () =>
                        {
                        });
                    });
                    break;
                }
        }

        public void OnAddWave()
        {
            var wave = Instantiate(_wavePanelPrefab, _wavePanelRoot);
            _waves.Add(wave);
            wave.WantBeRemoved += Wave_WantBeRemoved;
            wave.SetWaveIndex(_waves.Count);
        }
    }
}
