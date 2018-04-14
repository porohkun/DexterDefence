using MimiJson;
using System;
using System.Collections.Generic;
using Texturing;
using UnityEngine;

namespace Managers
{
    public class GraphicsManager : BaseManager<GraphicsManager>
    {
        [Serializable]
        private class TextureData
        {
            public Material Material;
            public TextAsset Manifest;
        }

        [SerializeField] private TextureData[] _textureDatas;
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private Vector2 _cellSize = new Vector2(64, 64);
        [SerializeField] private Vector2 _offset = new Vector2(64, 64);
        [SerializeField] private float _zIndexOffset = 0.01f;
        [SerializeField] private int _chunkSize = 16;

        public Texture2D CursorTexture;
        public Vector2 CursorPoint;
        public CursorMode CursorMode = CursorMode.ForceSoftware;


        public static Vector2 CellSize { get { return Instance._cellSize; } }
        public static Vector2 Offset { get { return Instance._offset; } }
        public static float ZIndexOffset { get { return Instance._zIndexOffset; } }
        public static int ChunkSize { get { return Instance._chunkSize; } }

        private Dictionary<string, TextureManifest> _manifests;
        private Dictionary<string, Material> _materials;
        private static Dictionary<string, Sprite> _spritesDict = new Dictionary<string, Sprite>();

        protected override void Initialize()
        {
            base.Initialize();

            _manifests = new Dictionary<string, TextureManifest>();
            _materials = new Dictionary<string, Material>();
            foreach (var data in _textureDatas)
            {
                var manifest = new TextureManifest(JsonValue.Parse(data.Manifest.text));
                _manifests.Add(manifest.TextureName, manifest);
                _materials.Add(manifest.TextureName, data.Material);
            }
            foreach (var sprite in _sprites)
            {
                string key = sprite.name;
                if (_spritesDict.ContainsKey(key))
                    _spritesDict[key] = sprite;
                else
                    _spritesDict.Add(key, sprite);
            }
        }

        public static TextureManifest GetManifest(string texture)
        {
            return Instance._manifests[texture];
        }

        public static Material GetMaterial(string texture)
        {
            return Instance._materials[texture];
        }

        public static Sprite GetSprite(string key)
        {
            if (_spritesDict.ContainsKey(key))
                return _spritesDict[key];
            else
                return _spritesDict["error"];
        }

        public static Vector2 Scale(Vector2 vector)
        {
            return Vector2.Scale(vector, Offset);
        }

        public static Vector2 Scale(float x, float y)
        {
            return Scale(new Vector2(x, y));
        }

        public static Vector2 ScaleBack(Vector2 vector)
        {
            return new Vector2(vector.x / Offset.x, vector.y / Offset.y);
        }

        public static Vector3 ScaleWithZ(Vector2 vector)
        {
            return OnlyAddZ(Scale(vector));
        }

        public static Vector3 ScaleWithZ(float x, float y)
        {
            return OnlyAddZ(Scale(x, y));
        }

        public static Vector3 OnlyAddZ(Vector2 vector)
        {
            return vector.AddZ(-100 + (vector.x + vector.y) * ZIndexOffset);
        }

        #region Cursor

        public static bool CursorVisible
        {
            get { return Cursor.visible; }
            set { Cursor.visible = value; }
        }

        public void SetCursor(Texture2D curTex)
        {
            if (curTex == null || (!Application.isEditor && Application.isMobilePlatform))
                return;
            CursorTexture = curTex;
            Cursor.SetCursor(CursorTexture, CursorPoint, CursorMode);
        }

        #endregion

    }
}