using MimiJson;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CustomImporter
{
    private static int[] textureSizes = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096 };

    [MenuItem("Assets/Convert To Json")]
    public static void ReimportTexturesCorrect()
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var importer = TextureImporter.GetAtPath(path) as TextureImporter;

            int width, height;
            GetImageSize(AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D, importer, out width, out height);

            var json = new JsonValue(new JsonObject(
                new JOPair("texture", Path.GetFileNameWithoutExtension(path)),
                new JOPair("size", new JsonObject(
                    new JOPair("x", width),
                    new JOPair("y", height))),
                new JOPair("sprites", new JsonObject())));
            var sprites = json["sprites"].Object;

            var meta = File.ReadAllLines(path + ".meta");

            for (int i = 0; i < meta.Length; i++)
            {
                var line = meta[i];
                if (line.Length > 12 && line.Substring(0, 12) == "      name: ")
                {
                    var name = line.Substring(12);
                    int x = int.Parse(meta[i + 3].Substring(11));
                    int y = int.Parse(meta[i + 4].Substring(11));
                    int w = int.Parse(meta[i + 5].Substring(15));
                    int h = int.Parse(meta[i + 6].Substring(16));
                    var j = JsonValue.Parse(meta[i + 8].Substring(13).Replace("x", "\"x\"").Replace("y", "\"y\""));

                    sprites.Add(name, new JsonObject(
                        new JOPair("position", new JsonObject(
                            new JOPair("x", x),
                            new JOPair("y", y))),
                        new JOPair("size", new JsonObject(
                            new JOPair("x", w),
                            new JOPair("y", h))),
                        new JOPair("anchor", new JsonObject(
                            new JOPair("x", (int)(j["x"] * (double)w)),
                            new JOPair("y", (int)(j["y"] * (double)h))))));
                }
            }

            var fullpath = Path.GetFullPath(path);
            json.ToFile(Path.ChangeExtension(fullpath, "json"));
        }
    }

    static MethodInfo GetWidthAndHeight = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);

    public static bool GetImageSize(Texture2D asset, TextureImporter importer, out int width, out int height)
    {
        if (asset != null && importer != null)
        {
            object[] args = new object[2] { 0, 0 };
            GetWidthAndHeight.Invoke(importer, args);

            width = (int)args[0];
            height = (int)args[1];

            return true;
        }

        height = width = 0;
        return false;
    }
}
