using Managers;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMotor : MonoBehaviour
{
    [SerializeField]
    private float _speed = 100f;
    [SerializeField]
    private float _scale = 1f;

    private float _realScale = 1f;

    private Camera _camera;
    private int _mapWidth;
    private int _mapHeight;

    private float _left { get { return Screen.width / _realScale - GraphicsManager.Offset.x / 2f; } }
    private float _right { get { return _mapWidth * GraphicsManager.Offset.x - Screen.width / _realScale - GraphicsManager.Offset.x / 2f; } }
    private float _bottom { get { return Screen.height / _realScale - GraphicsManager.Offset.y / 2f; } }
    private float _top { get { return _mapHeight * GraphicsManager.Offset.y - Screen.height / _realScale - GraphicsManager.Offset.y / 2f; } }

    void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = Screen.height / _realScale;
    }

    public void SetMapSize(int width, int height)
    {
        _mapWidth = width;
        _mapHeight = height;
    }

    [SerializeField]
    float _lastWheel = 0f;

    void Update()
    {
        if (_mapWidth == 0 || _mapHeight == 0)
            return;

        var _wheel = Input.GetAxis("Mouse ScrollWheel");
        if (_wheel < 0 && _lastWheel >= 0 && Mathf.Abs(_scale - _realScale) < 0.00001f)
        {
            _scale = _scale / 1.2f;
            _realScale = _scale;
        }
        if (_wheel > 0 && _lastWheel <= 0 && Screen.height / _realScale / 8f > GraphicsManager.CellSize.y)
        {
            _scale = _scale * 1.2f;
            _realScale = _scale;
        }
        _lastWheel = _wheel;

        var ort = Screen.height / _realScale;
        var ver = _bottom - _top;
        if (ver > 0)
            ort = Mathf.Min(ort, Screen.height / _realScale - ver / 2f);
        var hor = _left - _right;
        if (hor > 0)
            ort = Mathf.Min(ort, Screen.height / _realScale - hor / 2f / Screen.width * Screen.height);
        _realScale = Screen.height / ort;
        ort = Screen.height / _realScale;
        _camera.orthographicSize = ort;
        var newPos = _camera.transform.localPosition + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * _speed * Time.deltaTime;

        if (newPos.x < _left)
            newPos.x = _left;
        if (newPos.x > _right)
            newPos.x = _right;
        if (newPos.y < _bottom)
            newPos.y = _bottom;
        if (newPos.y > _top)
            newPos.y = _top;

        _camera.transform.localPosition = newPos;
    }

}