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
        }

        [SerializeField]
        private TowerVisualData[] _towersBrushData;
        [SerializeField]
        private Text _cursorPositionLabel;
        [SerializeField]
        private GameController _gameControllerPrefab;
        [SerializeField]
        private ToggleGroup _towersToggleGroup;

        private Camera _camera;
        private GameController _controller;
        private Transform _towerBrush;
        private string _brushType = "none";
        private string _brush;



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

        public void Initialize(string mapName)
        {
            var json = JsonValue.Parse(Resources.Load<TextAsset>("Maps/" + mapName).text);

            _controller = Instantiate(_gameControllerPrefab);
            _controller.StartGame(json);
        }


        private void Update()
        {
            var vectorCurPos = GraphicsManager.ScaleBack(_camera.ScreenToWorldPoint(Input.mousePosition));
            Point curPos = vectorCurPos;
            _cursorPositionLabel.text = curPos.ToString();

            if (_towerBrush.gameObject.activeSelf)
                _towerBrush.position = GraphicsManager.Scale(curPos).AddZ(-9);

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && _controller.CorrectTowerPosition(curPos))
            {
                _controller.PlaceTower(_brush, curPos);
                DeselectTowers();
                //switch (_brushType)
                //{
                //    case "surface":
                //        _map[curPos].Surface = _brush;
                //        break;
                //    case "objects":
                //        _map[curPos].Obstacle = _brush;
                //        break;
                //    case "waypoints":
                //        _map[curPos].Waypoints = _wpBrush;
                //        break;
                //}

                //switch (_brushType)
                //{
                //    case "surface":
                //    case "objects":
                //        _mapView.RegenerateChunk((vectorCurPos + Vector2.one / 2f) / GraphicsManager.ChunkSize - Vector2.one / 2f);
                //        break;
                //    case "waypoints":
                //        _waypointsView.RegenerateChunk((vectorCurPos + Vector2.one / 2f) / GraphicsManager.ChunkSize - Vector2.one / 2f);
                //        break;
                //}
            }
            if (Input.GetMouseButtonDown(1))
                DeselectTowers();
        }

        private void DeselectTowers()
        {
            _towersToggleGroup.SetAllTogglesOff();
            _controller.ShowTowerGrid = false;
            _towerBrush.gameObject.SetActive(false);
        }

        public void OnSelectTower(string name)
        {
            _controller.ShowTowerGrid = true;
            _brushType = "tower";
            _brush = name;
            SwitchBrush();
        }

        private void SwitchBrush()
        {
            switch (_brushType)
            {
                case "tower":
                    foreach (var towerData in _towersBrushData)
                        if (towerData.Name == _brush)
                        {
                            towerData.ShowBrush();
                            _towerBrush.gameObject.SetActive(true);
                            break;
                        }
                    break;
                case "objects":
                    break;
                case "waypoints":
                    break;
            }
        }
    }
}
