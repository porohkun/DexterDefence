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
        [SerializeField]
        private TextAsset _towerAIs;
        [SerializeField]
        private BulletView[] _bulletViewPrefabs;
        [SerializeField]
        private Transform _bulletsRoot;

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
        public int Money { get; private set; }
        public int Wave { get; private set; }

        private bool _showTowerGrid = false;
        private MapView _mapView;
        private Dictionary<UnitModel, UnitView> _unitViews = new Dictionary<UnitModel, UnitView>();
        private Dictionary<TowerModel, TowerView> _towerViews = new Dictionary<TowerModel, TowerView>();
        private Dictionary<BulletModel, BulletView> _bulletViews = new Dictionary<BulletModel, BulletView>();

        private JsonValue _towerDatas;
        
        public void StartGame(JsonValue mapJson)
        {
            transform.position = Vector3.zero;
            _towerDatas = JsonValue.Parse(_towerAIs.text);

            Map = new MapModel(mapJson);
            Map.BulletCreated += Map_BulletCreated;

            _mapView = new GameObject("MapView", typeof(MapView)).GetComponent<MapView>();
            _mapView.transform.SetParent(transform);
            _mapView.transform.position = Vector3.zero;
            _mapView.CreateChunks(Map);

            Camera.main.GetComponent<CameraMotor>().SetMapSize(Map.Width, Map.Height);

            StartWave();
        }

        public void NextWave()
        {
            StartWave();
        }

        private void Map_BulletCreated(BulletModel bullet)
        {
            CreateBullet(bullet);
        }

        private void Update()
        {
            Map.Update(Time.deltaTime);
        }

        private void StartWave()
        {
            Wave++;
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

        public int GetTowerCost(string name, int level)
        {
            var data = _towerDatas[name];
            return data["levels"][level]["cost"];
        }

        private void Unit_Finished(UnitModel unit)
        {

        }

        private void Unit_Died(UnitModel unit)
        {
            if (_unitViews.ContainsKey(unit))
            {
                Destroy(_unitViews[unit].gameObject);
                _unitViews.Remove(unit);
            }
        }

        public bool HaveTowerAtPos(Point curPos)
        {
            return Map.CorrectPosition(curPos) && Map[curPos].Tower != null;
        }

        public TowerView GetTowerAtPos(Point curPos)
        {
            var tower = Map[curPos].Tower;
            if (tower == null)
                return null;
            return _towerViews[tower];
        }

        public bool CorrectTowerPosition(Point position)
        {
            return Map.CorrectPosition(position) && Map[position].CanBuild;
        }

        public void UpgradeTower(TowerView selectedTower)
        {
            if (selectedTower.CanBeUpgraded && TryChangeMoney(-selectedTower.UpgradeCost))
                selectedTower.UpgradeTower();
        }

        public void RemoveTower(TowerView selectedTower)
        {
            var towerModel = _towerViews.First(t => t.Value == selectedTower).Key;
            TryChangeMoney(towerModel.Cashback);
            Destroy(selectedTower.gameObject);
            _towerViews.Remove(towerModel);
            Map.RemoveTower(towerModel);
        }

        public void PlaceTower(string towerName, Point curPos)
        {
            foreach (var towerPrefab in _towerViewPrefabs)
                if (towerPrefab.name == towerName)
                {
                    ITowerAI ai = null;
                    var data = _towerDatas[towerName];
                    int cost = data["levels"][0]["cost"];
                    if (TryChangeMoney(-cost))
                    {
                        switch (data["ai"].String)
                        {
                            case "single_shot": ai = new SingleShotAI(data["levels"]); break;
                            case "multi_shot": ai = new MultiShotAI(data["levels"]); break;
                        }
                        var tower = new TowerModel(Shell.Bullet, ai, 1f);
                        Map.AddTower(tower, curPos);

                        var towerView = Instantiate(towerPrefab, _towersRoot);
                        towerView.AttachTo(tower);
                        _towerViews.Add(tower, towerView);

                        _mapView.RegenerateChunk((Vector2)curPos / GraphicsManager.ChunkSize - Vector2.one / 1.99f);
                    }
                    break;
                }
        }

        private void CreateBullet(BulletModel bullet)
        {
            foreach (var bulletPrefab in _bulletViewPrefabs)
                if (bulletPrefab.name == bullet.Visual)
                {
                    var bulletView = Instantiate(bulletPrefab, _bulletsRoot);
                    bullet.Hitted += Bullet_Hitted;
                    bulletView.AttachTo(bullet);
                    _bulletViews.Add(bullet, bulletView);
                    break;
                }
        }

        private void Bullet_Hitted(BulletModel bullet, UnitModel unit)
        {
            if (_bulletViews.ContainsKey(bullet))
            {
                Destroy(_bulletViews[bullet].gameObject);
                _bulletViews.Remove(bullet);
            }
        }

        public bool TryChangeMoney(int amount)
        {
            if (Money + amount > 0)
            {
                Money += amount;
                return true;
            }
            return false;
        }
    }
}
