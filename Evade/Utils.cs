
#region

using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

#endregion

namespace Evade
{
    public static class Utils
    {
        public static int TickCount
        {
            get { return (int)(Game.Time * 1000f); }
        }

        public static Vector3[] GetPath2(this Obj_AI_Hero hero, Vector3 end)
        {
            List<Vector3> result = new List<Vector3>();

            result.Add(hero.Position);

            return result.ToArray();
        }

        public static Vector2 CutVector(Vector2 from, Vector2 to, int step = 20)
        {
            float distance = from.Distance(to);
            Vector2 output = to;
            var array = new List<Vector2>();

            for (float i = 0; i <= distance; i += step)
            {
                Vector2 vec = from.Extend(to, i);
                array.Add(vec);
            }

            for (int i = 0; i < array.Count; i++)
            {
                if (!array[i].IsWall())
                    continue;

                Vector2 result = i - 1 >= 0 ? array[i - 1] : array[i];
                return result.Extend(from, ObjectManager.Player.BoundingRadius);
            }

            return output;
        }

        public static float ToRadians(this float val)
        {
            return ((float)Math.PI / 180f) * val;
        }

        public static Vector2[] CirclePointsNormal(float CircleLineSegmentN, float radius, Vector2 position)
        {
            var points = new List<Vector2>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector2(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle));
                points.Add(point);
            }
            return points.ToArray();
        }

        public static Vector2[] CirclePoints(float CircleLineSegmentN, float radius, Vector2 position)
        {
            var points = new List<Vector2>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector2(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle));
                points.Add(point);
            }
            radius = radius / 3;
            for (var i = 1; i <= CircleLineSegmentN / 2; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector2(position.X + radius * (float)Math.Cos(angle), position.Y + (radius) * (float)Math.Sin(angle));
                points.Add(point);
            }
            return points.ToArray();
        }
        public static List<Vector2> To2DList(this Vector3[] v)
        {
            var result = new List<Vector2>();
            foreach (var point in v)
                result.Add(point.To2D());
            return result;
        }

        public static void SendMovePacket(this Obj_AI_Base v, Vector2 point, bool triggerEvents = false)
        {
            if (ObjectManager.Player.CanMove)
                ObjectManager.Player.ForceIssueOrder(GameObjectOrder.MoveTo, point.To3D(), triggerEvents);
        }

        public static Obj_AI_Base Closest(List<Obj_AI_Base> targetList, Vector2 from)
        {
            var dist = float.MaxValue;
            Obj_AI_Base result = null;

            foreach (var target in targetList)
            {
                var distance = Vector2.DistanceSquared(from, target.ServerPosition.To2D());
                if (distance < dist)
                {
                    dist = distance;
                    result = target;
                }
            }

            return result;
        }

        public static bool LineSegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            var denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));

            if (denominator == 0)
            {
                return false;
            }

            var numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));

            var numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            if (numerator1 == 0 || numerator2 == 0)
            {
                return false;
            }

            var r = numerator1 / denominator;
            var s = numerator2 / denominator;

            return (r > 0 && r < 1) && (s > 0 && s < 1);
        }

        /// <summary>
        /// Returns when the unit will be able to move again
        /// </summary>
        public static int ImmobileTime(Obj_AI_Base unit)
        {
            var result = 0f;

            foreach (var buff in unit.Buffs)
            {
                if (buff.IsActive && Game.Time <= buff.EndTime &&
                    (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                     buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                {
                    result = Math.Max(result, buff.EndTime);
                }
            }

            return (result == 0f) ? -1 : (int) (Utils.TickCount + (result - Game.Time) * 1000);
        }


        public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, Color color)
        {
            var from = Drawing.WorldToScreen(start);
            var to = Drawing.WorldToScreen(end);
            Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            //Drawing.DrawLine(from.X, from.Y, to.X, to.Y, width, color);
        }

        public static Vector2 CutVectorWall(this Vector2 position, Vector2 from, out bool foundWall)
        {
            foundWall = false;

            Vector2 result = from;
            Vector2 direction = (position - from).Normalized();

            for (int i = 0; i < (int)from.Distance(position); i += 10)
            {
                result = from + direction * i;

                if (result.IsWall())
                {
                    foundWall = true;
                    result = result - direction * ObjectManager.Player.BoundingRadius;
                    break;
                }
            }

            return result;
        }

        public static List<Vector3> CirclePoints(float CircleLineSegmentN, float radius, Vector3 position)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
                points.Add(point);
            }
            return points;
        }
    }

    internal class SpellList<T> : List<T>
    {
        public event EventHandler OnAdd;

        public new void Add(T item)
        {
            if (OnAdd != null)
            {
                OnAdd(this, null);
            }

            base.Add(item);
        }
    }
}