using System.Collections.Generic;
using UnityEngine;

namespace Texturing
{
    public class PreMesh
    {
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector2[] Uv;
        public Vector2[] Uv2;
        public Vector2[] Uv3;
        public int[] Triangles;

        public static Mesh ToMesh(IEnumerable<PreMesh> list)
        {
            List<Vector3> v = new List<Vector3>();
            List<Vector3> n = new List<Vector3>();
            List<Vector2> u = new List<Vector2>();
            List<Vector2> u2 = new List<Vector2>();
            List<Vector2> u3 = new List<Vector2>();
            List<int> t = new List<int>();
            int tr = 0;

            foreach (var item in list)
            {
                v.AddRange(item.Vertices);
                n.AddRange(item.Normals);
                u.AddRange(item.Uv);
                u2.AddRange(item.Uv2);
                u3.AddRange(item.Uv3);
                foreach (int triangle in item.Triangles)
                    t.Add(tr + triangle);
                tr = v.Count;
            }
            return new Mesh
            {
                vertices = v.ToArray(),
                normals = n.ToArray(),
                uv = u.ToArray(),
                uv2 = u2.ToArray(),
                uv3 = u3.ToArray(),
                triangles = t.ToArray()
            };
        }

        public Mesh ToMesh()
        {
            return ToMesh(new[] { this });
        }
    }
}