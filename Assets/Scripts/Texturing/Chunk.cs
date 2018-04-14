using Game;
using Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Texturing
{
    public class Chunk : MonoBehaviour
    {
        private MapModel _map;
        private Point _startPoint;
        private MeshRenderer _renderer;
        private MeshFilter _filter;
        private int _chunkSize;

        private static readonly Vector3 _normal = new Vector3(0, 0, -1);

        public void Initialize(MapModel map, Point startPoint, int chunkSize)
        {
            _map = map;
            _startPoint = startPoint;
            _renderer = gameObject.AddComponent<MeshRenderer>();
            _renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _renderer.receiveShadows = false;
            _renderer.allowOcclusionWhenDynamic = false;
            _filter = gameObject.AddComponent<MeshFilter>();
            _chunkSize = chunkSize;
        }

        public void Regenerate()
        {
            _filter.mesh = PreMesh.ToMesh(GeneratePremeshes());
            _renderer.material = GraphicsManager.GetMaterial("tiles");
        }

        private IEnumerable<PreMesh> GeneratePremeshes()
        {
            for (int x = _startPoint.X; x < _startPoint.X + _chunkSize; x++)
                for (int y = _startPoint.Y; y < _startPoint.Y + _chunkSize; y++)
                {
                    if (_map.CorrectPosition(x, y))
                        foreach (var preMesh in GenerateCell(x, y))
                            yield return preMesh;
                }
        }

        protected virtual IEnumerable<PreMesh> GenerateCell(int x, int y)
        {
            var z = -GraphicsManager.ZIndexOffset;
            var manifest = GraphicsManager.GetManifest("tiles");

            var cell = _map[x, y];
            var sprite1 = manifest["surface." + cell.Surface];
            var sprite2 = manifest[string.IsNullOrEmpty(cell.Obstacle) ? "empty" : "objects." + cell.Obstacle];
            var sprite3 = manifest["misc.rect"];
            var center = GraphicsManager.Scale(new Vector2(x, y) - (Vector2)_startPoint);

            yield return GetSpriteMesh(sprite1, sprite2, sprite3, center, z, 1);
        }

        protected static PreMesh GetSpriteMesh(TextureManifest.Sprite sprite1, TextureManifest.Sprite sprite2, TextureManifest.Sprite sprite3, Vector2 anchor, float z, float mod)
        {
            var psize2 = sprite1.PixelSize * mod / 2f;
            return new PreMesh()
            {
                Vertices = new[]
                {
                    new Vector3(anchor.x - psize2.x, anchor.y + psize2.y, z), //top left
                    new Vector3(anchor.x + psize2.x, anchor.y + psize2.y, z), //top right
                    new Vector3(anchor.x - psize2.x, anchor.y - psize2.y, z), //bottom left
                    new Vector3(anchor.x + psize2.x, anchor.y - psize2.y, z)  //bottom right
                },
                Normals = new[] { _normal, _normal, _normal, _normal },
                Uv = new[]
                {
                    new Vector2(sprite1.Position.x                 , sprite1.Position.y + sprite1.Size.y),
                    new Vector2(sprite1.Position.x + sprite1.Size.x, sprite1.Position.y + sprite1.Size.y),
                    new Vector2(sprite1.Position.x                 , sprite1.Position.y                 ),
                    new Vector2(sprite1.Position.x + sprite1.Size.x, sprite1.Position.y                 )
                },
                Uv2 = new[]
                {
                    new Vector2(sprite2.Position.x                 , sprite2.Position.y + sprite2.Size.y),
                    new Vector2(sprite2.Position.x + sprite2.Size.x, sprite2.Position.y + sprite2.Size.y),
                    new Vector2(sprite2.Position.x                 , sprite2.Position.y                 ),
                    new Vector2(sprite2.Position.x + sprite2.Size.x, sprite2.Position.y                 )
                },
                Uv3 = new[]
                {
                    new Vector2(sprite3.Position.x                 , sprite3.Position.y + sprite3.Size.y),
                    new Vector2(sprite3.Position.x + sprite3.Size.x, sprite3.Position.y + sprite3.Size.y),
                    new Vector2(sprite3.Position.x                 , sprite3.Position.y                 ),
                    new Vector2(sprite3.Position.x + sprite3.Size.x, sprite3.Position.y                 )
                },
                Triangles = new[] { 0, 1, 2, 1, 3, 2 }
            };
        }
    }
}
