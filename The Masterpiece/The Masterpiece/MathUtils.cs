using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace The_Masterpiece
{
    internal class MathUtils
    {
        public class CircInter
        {
            public bool none;
            public bool one;
            public Vector2 inter1;
            public Vector2 inter2;

            public CircInter()
            {
                one = false;
                none = false;
                inter1 = new Vector2();
                inter2 = new Vector2();
            }

            public Vector2 getBestInter(Obj_AI_Base target)
            {
                if (none)
                    return new Vector2(0, 0);
                if (one)
                    return inter1;
                var dist1 = target.Distance(inter1, true);
                var dist2 = target.Distance(inter2, true);

                return dist1 > dist2 ? inter2 : inter1;
            }
        }

        public static CircInter getCicleLineInteraction(Vector2 from, Vector2 to, Vector2 cPos, float radius)
        {
            var res = new CircInter();

            var dx = from.X - to.X;
            var dy = from.Y - to.Y;

            var A = dx * dx + dy * dy;
            var B = 2 * (dx * (to.X - cPos.X) + dy * (to.Y - cPos.Y));
            var C = (to.X - cPos.X) * (to.X - cPos.X) +
                (to.Y - cPos.Y) * (to.Y - cPos.Y) -
                radius * radius;

            var det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                res.none = true;
                // No real solutions.
            }
            else if (det == 0)
            {
                res.one = true;
                // One solution.
                var t = -B / (2 * A);
                res.inter1 =
                    new Vector2(to.X + t * dx, to.Y + t * dy);
            }
            else
            {
                // Two solutions.
                var t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                res.inter1 =
                    new Vector2(to.X + t * dx, to.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                res.inter2 =
                    new Vector2(to.X + t * dx, to.Y + t * dy);
            }


            return res;
        }

        internal class Polygon
        {
            public List<Vector2> Points = new List<Vector2>();

            public Polygon()
            {
            }

            public Polygon(List<Vector2> P)
            {
                Points = P;
            }

            public void add(Vector2 vec)
            {
                Points.Add(vec);
            }

            public int Count()
            {
                return Points.Count;
            }

            public Vector2 getProjOnPolygon(Vector2 vec)
            {
                Vector2 closest = new Vector2(-1000, -1000);
                Vector2 start = Points[Count() - 1];
                foreach (Vector2 vecPol in Points)
                {
                    Vector2 proj = projOnLine(start, vecPol, vec);
                    closest = ClosestVec(proj, closest, vec);
                    start = vecPol;
                }
                return closest;
            }

            public Vector2 ClosestVec(Vector2 vec1, Vector2 vec2, Vector2 to)
            {
                float dist1 = Vector2.DistanceSquared(vec1, to);//133
                float dist2 = Vector2.DistanceSquared(vec2, to);//12
                return (dist1 > dist2) ? vec2 : vec1;
            }

            public void Draw(System.Drawing.Color color, int width = 1)
            {
                for (var i = 0; i <= Points.Count - 1; i++)
                {
                    if (Points[i].Distance(ObjectManager.Player.Position) < 1500)
                    {
                        var nextIndex = (Points.Count - 1 == i) ? 0 : (i + 1);
                        var from = Drawing.WorldToScreen(Points[i].To3D());
                        var to = Drawing.WorldToScreen(Points[nextIndex].To3D());
                        Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
                    }
                }
            }

            private Vector2 projOnLine(Vector2 v, Vector2 w, Vector2 p)
            {
                Vector2 nullVec = new Vector2(-1, -1);
                // Return minimum distance between line segment vw and point p
                float l2 = Vector2.DistanceSquared(v, w);  // i.e. |w-v|^2 -  avoid a sqrt
                if (l2 == 0.0)
                    return nullVec;   // v == w case
                // Consider the line extending the segment, parameterized as v + t (w - v).
                // We find projection of point p onto the line. 
                // It falls where t = [(p-v) . (w-v)] / |w-v|^2
                float t = Vector2.Dot(p - v, w - v) / l2;
                if (t < 0.0)
                    return nullVec;       // Beyond the 'v' end of the segment
                else if (t > 1.0)
                    return nullVec;  // Beyond the 'w' end of the segment
                Vector2 projection = v + t * (w - v);  // Projection falls on the segment
                return projection;
            }

            public bool pointInside(Vector2 testPoint)
            {
                bool result = false;
                int j = Count() - 1;
                for (int i = 0; i < Count(); i++)
                {
                    if (Points[i].Y < testPoint.Y && Points[j].Y >= testPoint.Y || Points[j].Y < testPoint.Y && Points[i].Y >= testPoint.Y)
                    {
                        if (Points[i].X + (testPoint.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) * (Points[j].X - Points[i].X) < testPoint.X)
                        {
                            result = !result;
                        }
                    }
                    j = i;
                }
                return result;
            }



        }
    }
}
