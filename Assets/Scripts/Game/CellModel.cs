﻿using MimiJson;

namespace Game
{
    public class CellModel
    {
        public Point Position { get; private set; }

        public string Surface { get; set; }
        public string Obstacle { get; set; }
        public string Covering { get; set; }
        public Direction Waypoints { get; set; }

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
            if (!string.IsNullOrEmpty(Obstacle))
                result.Add("obstacle", Obstacle);
            if (Waypoints != Direction.None)
                result.Add("waypoints", Waypoints.Stringify());
            return result;
        }

    }
}
