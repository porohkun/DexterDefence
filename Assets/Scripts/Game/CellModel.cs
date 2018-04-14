using MimiJson;

namespace Game
{
    public class CellModel
    {
        public Point Position { get; private set; }
        
        public string Surface { get; set; }
        public string Obstacle { get; set; }
        public string Covering { get; set; }
        
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
            Covering = "";
        }
        
        public JsonValue ToJson()
        {
            var result = new JsonObject(
                new JOPair("position", Position.ToJson()),
                new JOPair("surface", Surface));
            if (!string.IsNullOrEmpty(Obstacle))
                result.Add("obstacle", Obstacle);
            return result;
        }

    }
}
