using MimiJson;
using System.Collections.Generic;
using UnityEngine;

namespace Texturing
{
    public class TextureManifest
    {
        public class Sprite
        {
            private Vector2 _texSize;

            private Vector2 _pixelPosition;
            private Vector2 _pixelSize;
            private Vector2 _pixelAnchor;

            private Vector2 _position;
            private Vector2 _size;
            private Vector2 _anchor;

            public Vector2 PixelPosition { get { return _pixelPosition; } }
            public Vector2 PixelSize { get { return _pixelSize; } }
            public Vector2 PixelAnchor { get { return _pixelAnchor; } }

            public Vector2 Position { get { return _position; } }
            public Vector2 Size { get { return _size; } }
            public Vector2 Anchor { get { return _anchor; } }

            internal Sprite(JsonValue json, Vector2 size)
            {
                _texSize = size;
                _pixelPosition = VectorFromJson(json["position"]);
                _pixelSize = VectorFromJson(json["size"]);
                _pixelAnchor = VectorFromJson(json["anchor"]);

                _position = _pixelPosition.DivideBy(size);
                _size = _pixelSize.DivideBy(size);
                _anchor = _pixelAnchor.DivideBy(size);
            }
        }

        private string _textureName;
        private Vector2 _size;
        private Dictionary<string, TextureManifest.Sprite> _sprites;

        public string TextureName { get { return _textureName; } }
        public Vector2 TextureSize { get { return _size; } }
        public TextureManifest.Sprite this[string name]
        {
            get
            {
                TextureManifest.Sprite sprite;
                if (_sprites.TryGetValue(name, out sprite))
                    return sprite;
                if (_sprites.TryGetValue("error", out sprite))
                    return sprite;
                return null;
            }
        }

        public TextureManifest(JsonValue json)
        {
            _textureName = json["texture"];
            _size = VectorFromJson(json["size"]);
            _sprites = new Dictionary<string, Sprite>();
            foreach (var sprite in json["sprites"].Object)
                _sprites.Add(sprite.Key, new Sprite(sprite.Value, _size));
        }

        private static Vector2 VectorFromJson(JsonValue json)
        {
            return new Vector2(json["x"], json["y"]);
        }
    }
}
