using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MimiJson;
using UnityEngine;
using Views;
using Managers;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private UnitView _unitViewPrefab;
        [SerializeField]
        private Transform _unitsRoot;
        [SerializeField]
        private TowerView[] _towerViewPrefabs;
        [SerializeField]
        private Transform _towersRoot;

        public MapModel Map { get; private set; }
        public bool ShowTowerGrid
        {
            get { return _showTowerGrid; }
            set
            {
                if (_showTowerGrid != value)
                {
                    _showTowerGrid = value;
                    _mapView.SetOverlayAlpha(value ? 1f : 0f);
                }
            }
        }

        private bool _showTowerGrid = false;
        private MapView _mapView;
        private Dictionary<UnitModel, UnitView> _unitViews;
        private Dictionary<TowerModel, TowerView> _towerViews;

        private void Awake()
        {
            _unitViews = new Dictionary<UnitModel, UnitView>();
            _towerViews = new Dictionary<TowerModel, TowerView>();
            transform.position = Vector3.zero;
        }

        public void StartGame(JsonValue mapJson)
        {
            Map = new MapModel(mapJson);

            _mapView = new GameObject("MapView", typeof(MapView)).GetComponent<MapView>();
            _mapView.transform.SetParent(transform);
            _mapView.transform.position = Vector3.zero;
            _mapView.CreateChunks(Map);

            Camera.main.GetComponent<CameraMotor>().SetMapSize(Map.Width, Map.Height);

            StartWave();
        }

        private void Update()
        {
            Map.Update(Time.deltaTime);
        }

        private void StartWave()
        {
            StartCoroutine(WaveRoutine());
        }

        private IEnumerator WaveRoutine()
        {
            for (int i = 0; i < 100; i++)
            {
                CreateUnit();
                yield return new WaitForSeconds(0.4f);
            }
        }

        private void CreateUnit()
        {
            var unit = new UnitModel("unit1", 100, 1.0f, 0f);
            Map.AddUnit(unit);
            unit.Died += Unit_Died;
            unit.Finished += Unit_Finished;
            var unitView = Instantiate(_unitViewPrefab, _unitsRoot);
            unitView.AttachTo(unit);
            _unitViews.Add(unit, unitView);
        }

        private void Unit_Finished(UnitModel unit)
        {

        }

        private void Unit_Died(UnitModel unit)
        {

        }

        public bool CorrectTowerPosition(Point position)
        {
            return Map.CorrectPosition(position) && Map[position].CanBuild;
        }

        public void PlaceTower(string towerName, Point curPos)
        {
            foreach (var towerPrefab in _towerViewPrefabs)
                if (towerPrefab.name == towerName)
                {
                    var tower = new TowerModel(Shell.Bullet, null, 1f);
                    Map.AddTower(tower, curPos);

                    var towerView = Instantiate(towerPrefab, _towersRoot);
                    towerView.AttachTo(tower);
                    _towerViews.Add(tower, towerView);

                    _mapView.RegenerateChunk((Vector2)curPos / GraphicsManager.ChunkSize - Vector2.one / 1.99f);
                    break;
                }
        }
    }
}
