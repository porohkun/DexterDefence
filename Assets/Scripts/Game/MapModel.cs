using MimiJson;
using System.Collections.Generic;
using System.Linq;
using StartPosition = System.Collections.Generic.KeyValuePair<Game.Point, Game.Direction>;
using System;

namespace Game
{
    public class MapModel
    {
        private readonly CellModel[,] _cells;
        private readonly List<UnitModel> _units = new List<UnitModel>();
        private readonly List<TowerModel> _towers = new List<TowerModel>();
        private readonly List<BulletModel> _bullets = new List<BulletModel>();
        private readonly List<StartPosition> _startPositions = new List<StartPosition>();

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
        public IList<UnitModel> Units { get { return _units; } }
        public IList<TowerModel> Towers { get { return _towers; } }
        public event Action<BulletModel> BulletCreated;

        private int _lastStartPosition = -1;

        public MapModel(int width, int height)
        {
            Width = width;
            Height = height;

            _cells = new CellModel[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _cells[x, y] = new CellModel(new Point(x, y), "grass");
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
                    var cell = new CellModel(new Point(x, y), enumerator.Current);
                    _cells[x, y] = cell;
                    if (cell.Waypoints != Direction.None && (x == 0 || x == Width - 1 || y == 0 || y == Height - 1))
                    {
                        var back = cell.Waypoints.Mirror();
                        var backStep = cell.Position.AddDirection(back);
                        if (!CorrectPosition(backStep))
                            _startPositions.Add(new StartPosition(backStep, cell.Waypoints));
                    }
                }
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

        public void AddUnit(UnitModel unit)
        {
            _units.Add(unit);

            _lastStartPosition++;
            _lastStartPosition = _lastStartPosition % _startPositions.Count;

            unit.Initialize(this, _startPositions[_lastStartPosition]);
        }

        public void AddTower(TowerModel tower, Point position)
        {
            _towers.Add(tower);
            tower.Initialize(this, position);
            this[position].Tower = tower;
            tower.BulletShoot += Tower_BulletShoot;
        }

        private void Tower_BulletShoot(BulletModel bullet)
        {
            _bullets.Add(bullet);
            if (BulletCreated != null)
                BulletCreated(bullet);
        }

        public void Update(float deltaTime)
        {
            foreach (var unit in _units)
                unit.Update(deltaTime);
            foreach (var tower in _towers)
                tower.Update(deltaTime);
            foreach (var bullet in _bullets)
                bullet.Update(deltaTime);
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
