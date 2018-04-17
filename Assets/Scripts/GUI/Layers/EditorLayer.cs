using Game;
using Managers;
using System.IO;
using System.Linq;
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
        [SerializeField]
        private Image _brushVisual;

        private Camera _camera;
        private MapModel _map;
        private MapView _mapView;
        private WaypointsView _waypointsView;
        private CanvasGroup _canvasGroup;

        private string _brushType = "surface";
        private string _brush = "grass";
        private Direction _wpBrush = Direction.None;
        private string _filename { get { return Path.Combine(Path.Combine("Assets", Path.Combine("Resources", "Maps")), _filenameField.text) + ".json"; } }
        private string _mapsFilename { get { return Path.Combine("Assets", Path.Combine("Resources", "maps.json")); } }

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
                    case "waypoints":
                        _map[curPos].Waypoints = _wpBrush;
                        break;
                }

                switch (_brushType)
                {
                    case "surface":
                    case "objects":
                        _mapView.RegenerateChunk((vectorCurPos + Vector2.one / 2f) / GraphicsManager.ChunkSize - Vector2.one / 2f);
                        break;
                    case "waypoints":
                        _waypointsView.RegenerateChunk((vectorCurPos + Vector2.one / 2f) / GraphicsManager.ChunkSize - Vector2.one / 2f);
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.E)) //rotate waypoint cw
            {
                Direction newBrush = Direction.None;
                foreach (var dir in _wpBrush.Enumerate())
                    switch (dir)
                    {
                        case Direction.North: newBrush = newBrush.AddFlag(Direction.East); break;
                        case Direction.East: newBrush = newBrush.AddFlag(Direction.South); break;
                        case Direction.South: newBrush = newBrush.AddFlag(Direction.West); break;
                        case Direction.West: newBrush = newBrush.AddFlag(Direction.North); break;
                    }
                _wpBrush = newBrush;
                ShowBrush();
            }
            if (Input.GetKeyDown(KeyCode.Q)) //rotate waypoint ccw
            {
                Direction newBrush = Direction.None;
                foreach (var dir in _wpBrush.Enumerate())
                    switch (dir)
                    {
                        case Direction.North: newBrush = newBrush.AddFlag(Direction.West); break;
                        case Direction.West: newBrush = newBrush.AddFlag(Direction.South); break;
                        case Direction.South: newBrush = newBrush.AddFlag(Direction.East); break;
                        case Direction.East: newBrush = newBrush.AddFlag(Direction.North); break;
                    }
                _wpBrush = newBrush;
                ShowBrush();
            }
        }

        public void OnSave()
        {
            var filename = _filenameField.text;
            _map.ToJson().ToFile(_filename);
            var maps = MimiJson.JsonValue.ParseFile(_mapsFilename);
            if (maps["maps"].Array.Count(item => item == filename) == 0)
                maps["maps"].Array.Add(filename);
            maps.ToFile(_mapsFilename);
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
                Destroy(_mapView.gameObject);
            _mapView = new GameObject("MapView", typeof(MapView)).GetComponent<MapView>();
            _mapView.transform.position = Vector3.zero;
            _mapView.CreateChunks(_map);

            if (_waypointsView != null)
                Destroy(_waypointsView.gameObject);
            _waypointsView = new GameObject("WaypointsView", typeof(WaypointsView)).GetComponent<WaypointsView>();
            _waypointsView.transform.position = Vector3.back;
            _waypointsView.CreateChunks(_map);

            _camera.GetComponent<CameraMotor>().SetMapSize(_map.Width, _map.Height);
        }

        public void SelectSurfaceBrush(string brush)
        {
            _brushType = "surface";
            _brush = brush;
            ShowBrush();
        }

        public void SelectObjectsBrush(string brush)
        {
            _brushType = "objects";
            _brush = brush;
            ShowBrush();
        }

        public void SelectWaypointsBrush(string brush)
        {
            _brushType = "waypoints";
            _wpBrush = brush.ToDirection();
            ShowBrush();
        }

        private void ShowBrush()
        {
            switch (_brushType)
            {
                case "surface":
                    _brushVisual.sprite = GraphicsManager.GetSprite(string.IsNullOrEmpty(_brush) ? "empty" : "surface." + _brush);
                    break;
                case "objects":
                    _brushVisual.sprite = GraphicsManager.GetSprite(string.IsNullOrEmpty(_brush) ? "empty" : "objects." + _brush);
                    break;
                case "waypoints":
                    _brushVisual.sprite = GraphicsManager.GetSprite(_wpBrush == Direction.None ? "empty" : "waypoints." + _wpBrush.Stringify());
                    break;
            }
        }
    }
}
