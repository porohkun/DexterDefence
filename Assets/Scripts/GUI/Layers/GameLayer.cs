using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Game;
using MimiJson;
using Managers;
using UnityEngine.EventSystems;
using Views;

namespace Layers
{
    public class GameLayer : LayerBase
    {
        [Serializable]
        public class TowerVisualData
        {
            [SerializeField]
            private string _name;
            [SerializeField]
            private Sprite _base;
            [SerializeField]
            private Sprite _turret;
            [SerializeField]
            private Text _cost;

            public string Name { get { return _name; } }

            private Transform _towerBrush;
            private SpriteRenderer _baseRenderer;
            private SpriteRenderer _turretRenderer;

            public void AddBrush(Transform towerBrush)
            {
                _towerBrush = towerBrush;
                _baseRenderer = _towerBrush.GetComponent<SpriteRenderer>();
                _turretRenderer = _towerBrush.GetChild(0).GetComponent<SpriteRenderer>();
            }

            public void ShowBrush()
            {
                _baseRenderer.sprite = _base;
                _turretRenderer.sprite = _turret;
            }

            public void SetCost(int amount)
            {
                _cost.text = amount.ToString();
            }
        }
        
        [SerializeField]
        private TowerVisualData[] _towersBrushData;
        [SerializeField]
        private Text _cursorPositionLabel;
        [SerializeField]
        private GameController _gameControllerPrefab;
        [SerializeField]
        private ToggleGroup _towersToggleGroup;
        [SerializeField]
        private RectTransform _selectedTowerPanel;
        [SerializeField]
        private Button _selectedTowerUpgradeButton;
        [SerializeField]
        private Text _selectedTowerUpgradeCost;
        [SerializeField]
        private Text _selectedTowerRemoveCost;
        [SerializeField]
        private Text _moneyLabel;
        [SerializeField]
        private Text _waveLabel;
        [SerializeField]
        private Text _healthLabel;
        [SerializeField]
        private GameObject _nextWaveButton;

        private Camera _camera;
        private GameController _controller;
        private Transform _towerBrush;
        private string _brushType = "none";
        private string _brush;
        private TowerView _selectedTower;
        private string _mapName;
        private Wave[] _waves;
        private int _startMoney;
        private int _lives;

        private void Start()
        {
            _camera = Camera.main;
            _towerBrush = new GameObject("TowerBrush", typeof(SpriteRenderer)).transform;
            _towerBrush.position = Vector3.zero;
            _towerBrush.localScale = Vector3.one;
            var turret = Instantiate(_towerBrush, _towerBrush);
            turret.localPosition = Vector3.back * 0.1f;
            _towerBrush.gameObject.SetActive(false);
            foreach (var towerData in _towersBrushData)
                towerData.AddBrush(_towerBrush);
        }

        public void Clear()
        {
            if (_controller != null)
                Destroy(_controller.gameObject);
        }

        public void Restart()
        {
            Initialize(_mapName, _waves, _startMoney, _lives);
        }

        public void Initialize(string mapName, Wave[] waves, int startMoney, int lives)
        {
            Clear();

            var json = JsonValue.Parse(Resources.Load<TextAsset>("Maps/" + mapName).text);
            _mapName = mapName;
            _waves = waves;
            _startMoney = startMoney;
            _lives = lives;

            _controller = Instantiate(_gameControllerPrefab);
            _controller.TryChangeMoney(_startMoney);
            _controller.StartGame(json, _waves, _lives);
            _controller.WaveEnded += _controller_WaveEnded;
            foreach (var towerData in _towersBrushData)
                towerData.SetCost(_controller.GetTowerCost(towerData.Name, 0));
        }

        private void _controller_WaveEnded()
        {
            _nextWaveButton.SetActive(true);
            if (_controller.Wave == _waves.Length || _controller.Health <= 0)
                LayersManager.Push<EndGameLayer>().Initialize(_controller.Health > 0);
        }

        public void OnNextWave()
        {
            _nextWaveButton.SetActive(false);
            _controller.NextWave();
        }

        private void Update()
        {
            _moneyLabel.text = _controller.Money.ToString();
            _waveLabel.text = string.Format("{0}/{1}", _controller.Wave.ToString(), _waves.Length);
            _healthLabel.text = _controller.Health.ToString();

            var vectorCurPos = GraphicsManager.ScaleBack(_camera.ScreenToWorldPoint(Input.mousePosition));
            Point curPos = vectorCurPos;
            _cursorPositionLabel.text = curPos.ToString();

            if (_towerBrush.gameObject.activeSelf)
                _towerBrush.position = GraphicsManager.Scale(curPos).AddZ(-9);

            if (_selectedTower != null)
            {
                _selectedTowerPanel.position = Camera.main.WorldToScreenPoint(_selectedTower.transform.position);
                _selectedTowerUpgradeButton.interactable = _selectedTower.CanBeUpgraded;
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                switch (_brushType)
                {
                    case "tower":
                        if (_controller.CorrectTowerPosition(curPos))
                            _controller.PlaceTower(_brush, curPos);
                        DropBrush();
                        break;
                    case "tower_interface":
                        DropBrush();
                        break;
                    case "none":
                        if (_controller.HaveTowerAtPos(curPos))
                        {
                            var towerView = _controller.GetTowerAtPos(curPos);
                            if (towerView != null)
                            {
                                ShowTowerInterface(towerView);
                            }
                        }
                        break;
                }
            }
            else if (Input.GetMouseButtonDown(1))
                DropBrush();
        }

        private void ShowTowerInterface(TowerView towerView)
        {
            DropBrush();
            _brushType = "tower_interface";
            _selectedTower = towerView;
            _selectedTowerPanel.gameObject.SetActive(true);
            _selectedTowerUpgradeCost.text = towerView.CanBeUpgraded ? towerView.UpgradeCost.ToString() : "";
            _selectedTowerRemoveCost.text = towerView.RemoveCashback.ToString();
        }

        public void OnUpgradeSelectedTower()
        {
            _controller.UpgradeTower(_selectedTower);
            _selectedTowerUpgradeCost.text = _selectedTower.CanBeUpgraded ? _selectedTower.UpgradeCost.ToString() : "";
            _selectedTowerRemoveCost.text = _selectedTower.RemoveCashback.ToString();
        }

        public void OnRemoveSelectedTower()
        {
            _controller.RemoveTower(_selectedTower);
            DropBrush();
        }

        public void OnSelectTower(string name)
        {
            DropBrush();
            _controller.ShowTowerGrid = true;
            _brushType = "tower";
            _brush = name;

            foreach (var towerData in _towersBrushData)
                if (towerData.Name == _brush)
                {
                    towerData.ShowBrush();
                    _towerBrush.gameObject.SetActive(true);
                    break;
                }
        }

        private void DropBrush()
        {
            switch (_brushType)
            {
                case "tower":
                    _towersToggleGroup.SetAllTogglesOff();
                    _controller.ShowTowerGrid = false;
                    _towerBrush.gameObject.SetActive(false);
                    break;
                case "tower_interface":
                    _selectedTower = null;
                    _selectedTowerPanel.gameObject.SetActive(false);
                    break;
            }
            _brushType = "none";
        }
    }
}
