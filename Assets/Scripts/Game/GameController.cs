using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MimiJson;
using UnityEngine;
using Views;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private UnitView _unitViewPrefab;
        [SerializeField]
        private Transform _unitsRoot;

        private MapModel _map;

        private MapView _mapView;
        private Dictionary<UnitModel, UnitView> _unitViews;

        private void Awake()
        {
            _unitViews = new Dictionary<UnitModel, UnitView>();
            transform.position = Vector3.zero;
        }

        public void StartGame(JsonValue mapJson)
        {
            _map = new MapModel(mapJson);

            _mapView = new GameObject("MapView", typeof(MapView)).GetComponent<MapView>();
            _mapView.transform.SetParent(transform);
            _mapView.transform.position = Vector3.zero;
            _mapView.CreateChunks(_map);

            Camera.main.GetComponent<CameraMotor>().SetMapSize(_map.Width, _map.Height);

            StartWave();
        }

        private void Update()
        {
            _map.Update(Time.deltaTime);
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
            var unit = new UnitModel("unit1", 100, 1.0f);
            _map.AddUnit(unit);
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
    }
}
