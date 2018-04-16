using System;
using MimiJson;
using System.Linq;

namespace Game
{
    public class CellModel
    {
        public Point Position { get; private set; }

        public string Surface { get; set; }
        public string Obstacle { get; set; }
        public string Covering { get; set; }
        public Direction Waypoints { get; set; }
        public TowerModel Tower { get; set; }
        public bool HaveObstacle { get { return !string.IsNullOrEmpty(Obstacle); } }
        public bool CanBuild { get { return !HaveObstacle && Surface == "grass" && Tower == null; } }

        private int _lastWaypoint = -1;

        public CellModel(Point position, string surface)
        {
            Position = position;

            Surface = surface;
            Obstacle = "";
            Covering = "";
        }

        public CellModel(Point position, JsonValue json)
        {
            Position = position;

            Surface = json["surface"];
            if (json.Object.ContainsKey("obstacle"))
                Obstacle = json["obstacle"];
            if (json.Object.ContainsKey("waypoints"))
                Waypoints = json["waypoints"].String.ToDirection();
            Covering = "";
        }

        public JsonValue ToJson()
        {
            var result = new JsonObject(
                new JOPair("position", Position.ToJson()),
                new JOPair("surface", Surface));
            if (HaveObstacle)
                result.Add("obstacle", Obstacle);
            if (Waypoints != Direction.None)
                result.Add("waypoints", Waypoints.Stringify());
            return result;
        }

        public Direction GetNextWaypoint()
        {
            var wps = Waypoints.Enumerate().ToArray();
            _lastWaypoint++;
            _lastWaypoint = _lastWaypoint % wps.Length;
            return wps[_lastWaypoint];
        }
    }
}
