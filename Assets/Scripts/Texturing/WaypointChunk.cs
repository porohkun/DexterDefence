using Game;
using Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Texturing
{
    public class WaypointChunk : Chunk
    {
        protected override string _texture { get { return "waypoints"; } }

        protected override IEnumerable<PreMesh> GenerateCell(int x, int y)
        {
            var z = -GraphicsManager.ZIndexOffset;

            var cell = _map[x, y];
            var waypoints = cell.Waypoints.Stringify();
            var sprite1 = _manifest[string.IsNullOrEmpty(waypoints) ? "empty" : "waypoints." + waypoints];
            var sprite2 = _manifest["empty"];
            var sprite3 = _manifest["empty"];
            var center = GraphicsManager.Scale(new Vector2(x, y) - (Vector2)_startPoint);

            yield return GetSpriteMesh(sprite1, sprite2, sprite3, center, z, 1);
        }
    }
}
