using Game;
using Managers;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Views;

namespace Layers
{
    public class EditorLayer : LayerBase
    {
        [SerializeField]
        private Text _cursorPositionLabel;
        [SerializeField]
        private InputField _filenameField;

        private Camera _camera;
        private MapModel _map;
        private MapView _mapView;
        private CanvasGroup _canvasGroup;

        private string _brush = "grass";
        private string _brushType = "surface";
        private string _filename { get { return Path.Combine(Path.Combine("Assets", "Resources"), _filenameField.text) + ".json"; } }

        private void Start()
        {
            _camera = Camera.main;
            _canvasGroup = GetComponent<CanvasGroup>();
            LayersManager.FadeIn(0.5f, null);
        }

        private void Update()
        {
            var hide = Input.GetKey(KeyCode.H);
            _canvasGroup.alpha = hide ? 0 : 1;
            _canvasGroup.interactable = hide ? false : true;
            _canvasGroup.blocksRaycasts = hide ? false : true;

            var vectorCurPos = GraphicsManager.ScaleBack(_camera.ScreenToWorldPoint(Input.mousePosition));
            Point curPos = vectorCurPos;
            _cursorPositionLabel.text = curPos.ToString();

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && _map.CorrectPosition(curPos))
            {
                switch (_brushType)
                {
                    case "surface":
                        _map[curPos].Surface = _brush;
                        break;
                    case "objects":
                        _map[curPos].Obstacle = _brush;
                        break;
                }

                _mapView.RegenerateChunk((vectorCurPos + Vector2.one / 2f) / GraphicsManager.ChunkSize - Vector2.one / 2f);
            }
        }

        public void OnSave()
        {
            _map.ToJson().ToFile(_filename);
        }

        public void OnLoad()
        {
            _map = new MapModel(MimiJson.JsonValue.ParseFile(_filename));
            CreateMapView();
        }

        public void OnNew()
        {
            LayersManager.Push<NewMapLayer>();
        }

        internal override void OnFloatUp()
        {
            base.OnFloatUp();
            var newMapLayer = LayersManager.GetLayer<NewMapLayer>();
            if (!newMapLayer.OkPressed)
                return;
            _filenameField.text = newMapLayer.Filename;

            _map = new MapModel(newMapLayer.Width, newMapLayer.Height);
            CreateMapView();
        }

        private void CreateMapView()
        {
            if (_mapView != null)
                Destroy(_mapView);
            _mapView = new GameObject("MapView", typeof(MapView)).GetComponent<MapView>();
            _mapView.transform.position = Vector3.zero;
            _mapView.CreateMap(_map);
            _camera.GetComponent<CameraMotor>().SetMapSize(_map.Width, _map.Height);
        }

        public void SelectSurfaceBrush(string brush)
        {
            _brushType = "surface";
            _brush = brush;
        }

        public void SelectObjectsBrush(string brush)
        {
            _brushType = "objects";
            _brush = brush;
        }
    }
}
