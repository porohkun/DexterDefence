using MimiJson;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class MapModel
    {
        private readonly CellModel[,] _cells;
        private readonly List<TowerModel> _towers;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public CellModel this[int x, int y]
        {
            get { return _cells[x, y]; }
            set { _cells[x, y] = value; }
        }
        public CellModel this[Point p]
        {
            get { return _cells[p.X, p.Y]; }
            set { _cells[p.X, p.Y] = value; }
        }
        public IList<TowerModel> Towers { get { return _towers; } }

        public MapModel(int width, int height)
        {
            Width = width;
            Height = height;

            _cells = new CellModel[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _cells[x, y] = new CellModel(new Point(x, y), "grass");
            
            _towers = new List<TowerModel>();
        }

        public MapModel(JsonValue json)
        {
            Width = json["width"];
            Height = json["height"];

            _cells = new CellModel[Width, Height];
            var enumerator = json["cells"].GetEnumerator();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    enumerator.MoveNext();
                    _cells[x, y] = new CellModel(new Point(x, y), enumerator.Current);
                }

            _towers = new List<TowerModel>();
        }

        public bool CorrectPosition(int x, int y)
        {
            return x >= 0 && y >= 0 &&
                x < Width && y < Height;
        }

        public bool CorrectPosition(Point position)
        {
            return CorrectPosition(position.X, position.Y);
        }

        public JsonValue ToJson()
        {
            return new JsonObject(
                new JOPair("width", Width),
                new JOPair("height", Height),
                new JOPair("cells", new JsonArray(_cells.Enumerate().Select(c => c.ToJson()))));
        }
    }
}
