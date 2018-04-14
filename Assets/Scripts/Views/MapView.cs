using Game;
using Managers;
using Texturing;
using UnityEngine;

namespace Views
{
    public class MapView : MonoBehaviour
    {
        private MapModel _map;
        private Chunk[,] _chunks;

        public int Width { get { return _map.Width; } }
        public int Height { get { return _map.Height; } }

        public MapModel Map { get { return _map; } }

        public MapModel CreateMap(MapModel map)
        {
            _map = map;

            CreateChunks();
            RegenerateChunks();

            return _map;
        }

        private void CreateChunks()
        {
            if (_chunks != null)
                foreach (var chunk in _chunks)
                    DestroyObject(chunk.gameObject);

            _chunks = new Chunk[
                Mathf.RoundToInt(Mathf.Ceil((float)_map.Width / GraphicsManager.ChunkSize)),
                Mathf.RoundToInt(Mathf.Ceil((float)_map.Height / GraphicsManager.ChunkSize))
                ];
            for (int x = 0; x < _map.Width; x += GraphicsManager.ChunkSize)
                for (int y = 0; y < _map.Height; y += GraphicsManager.ChunkSize)
                {
                    var chunk = new GameObject(string.Format("chunk_{0}:{1}", x, y)).AddComponent<Chunk>();
                    chunk.transform.SetParent(transform);
                    var startPoint = new Point(x, y);
                    chunk.transform.localPosition = GraphicsManager.Scale(startPoint);
                    chunk.Initialize(_map, startPoint, GraphicsManager.ChunkSize);
                    _chunks[x / GraphicsManager.ChunkSize, y / GraphicsManager.ChunkSize] = chunk;
                }
        }

        public void RegenerateChunks()
        {
            foreach (var chunk in _chunks)
                chunk.Regenerate();
        }

        public void RegenerateChunk(Point pos)
        {
            if (pos.X >= 0 && pos.Y >= 0 && pos.X < _chunks.GetLength(0) && pos.Y < _chunks.GetLength(1))
                _chunks[pos.X, pos.Y].Regenerate();
        }

    }
}
