using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;

namespace SebbyLib.Movement
{
    public enum HitChance
    {
        Immobile = 8,
        Dashing = 7,
        VeryHigh = 6,
        High = 5,
        Medium = 4,
        Low = 3,
        Impossible = 2,
        OutOfRange = 1,
        Collision = 0
    }

    public enum SkillshotType
    {
        SkillshotLine,
        SkillshotCircle,
        SkillshotCone
    }

    public enum CollisionableObjects
    {
        Minions,
        Heroes,
        YasuoWall,
        Walls
    }

    public class PredictionInput
    {

        private Vector3 _from;
        private Vector3 _rangeCheckFrom;

        /// <summary>
        ///     Set to true make the prediction hit as many enemy heroes as posible.
        /// </summary>
        public bool Aoe = false;

        /// <summary>
        ///     Set to true if the unit collides with units.
        /// </summary>
        public bool Collision = false;

        /// <summary>
        ///     Array that contains the unit types that the skillshot can collide with.
        /// </summary>
        public CollisionableObjects[] CollisionObjects =
        {
            CollisionableObjects.Minions, CollisionableObjects.YasuoWall
        };

        /// <summary>
        ///     The skillshot delay in seconds.
        /// </summary>
        public float Delay;

        /// <summary>
        ///     The skillshot width's radius or the angle in case of the cone skillshots.
        /// </summary>
        public float Radius = 1f;

        /// <summary>
        ///     The skillshot range in units.
        /// </summary>
        public float Range = float.MaxValue;

        /// <summary>
        ///     The skillshot speed in units per second.
        /// </summary>
        public float Speed = float.MaxValue;

        /// <summary>
        ///     The skillshot type.
        /// </summary>
        public SkillshotType Type = SkillshotType.SkillshotLine;

        /// <summary>
        ///     The unit that the prediction will made for.
        /// </summary>
        public Obj_AI_Base Unit = ObjectManager.Player;

        /// <summary>
        ///     Source unit for the prediction 
        /// </summary>
        public Obj_AI_Base Source = ObjectManager.Player;

        /// <summary>
        ///     Set to true to increase the prediction radius by the unit bounding radius.
        /// </summary>
        public bool UseBoundingRadius = true;

        /// <summary>
        ///     The position from where the skillshot missile gets fired.
        /// </summary>
        public Vector3 From
        {
            get { return _from.To2D().IsValid() ? _from : ObjectManager.Player.ServerPosition; }
            set { _from = value; }
        }

        /// <summary>
        ///     The position from where the range is checked.
        /// </summary>
        public Vector3 RangeCheckFrom
        {
            get
            {
                return _rangeCheckFrom.To2D().IsValid()
                    ? _rangeCheckFrom
                    : (From.To2D().IsValid() ? From : ObjectManager.Player.ServerPosition);
            }
            set { _rangeCheckFrom = value; }
        }

        internal float RealRadius
        {
            get { return UseBoundingRadius ? Radius + Unit.BoundingRadius : Radius; }
        }
    }

    public class PredictionOutput
    {
        internal int _aoeTargetsHitCount;
        private Vector3 _castPosition;
        private Vector3 _unitPosition;

        /// <summary>
        ///     The list of the targets that the spell will hit (only if aoe was enabled).
        /// </summary>
        public List<Obj_AI_Hero> AoeTargetsHit = new List<Obj_AI_Hero>();

        /// <summary>
        ///     The list of the units that the skillshot will collide with.
        /// </summary>
        public List<Obj_AI_Base> CollisionObjects = new List<Obj_AI_Base>();

        /// <summary>
        ///     Returns the hitchance.
        /// </summary>
        public HitChance Hitchance = HitChance.Impossible;

        internal PredictionInput Input;

        /// <summary>
        ///     The position where the skillshot should be casted to increase the accuracy.
        /// </summary>
        public Vector3 CastPosition
        {
            get
            {
                return _castPosition.IsValid() && _castPosition.To2D().IsValid()
                    ? _castPosition.SetZ()
                    : Input.Unit.ServerPosition;
            }
            set { _castPosition = value; }
        }

        /// <summary>
        ///     The number of targets the skillshot will hit (only if aoe was enabled).
        /// </summary>
        public int AoeTargetsHitCount
        {
            get { return Math.Max(_aoeTargetsHitCount, AoeTargetsHit.Count); }
        }

        /// <summary>
        ///     The position where the unit is going to be when the skillshot reaches his position.
        /// </summary>
        public Vector3 UnitPosition
        {
            get { return _unitPosition.To2D().IsValid() ? _unitPosition.SetZ() : Input.Unit.ServerPosition; }
            set { _unitPosition = value; }
        }
    }

    /// <summary>
    ///     Class used for calculating the position of the given unit after a delay.
    /// </summary>
    public static class Prediction
    {
        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay });
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius });
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit, float delay, float radius, float speed)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius, Speed = speed });
        }

        public static PredictionOutput GetPrediction(Obj_AI_Base unit,
            float delay,
            float radius,
            float speed,
            CollisionableObjects[] collisionable)
        {
            return
                GetPrediction(
                    new PredictionInput
                    {
                        Unit = unit,
                        Delay = delay,
                        Radius = radius,
                        Speed = speed,
                        CollisionObjects = collisionable
                    });
        }

        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return GetPrediction(input, true, true);
        }

        internal static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision)
        {
            PredictionOutput result = null;

            if (!input.Unit.IsValidTarget(float.MaxValue, false))
            {
                return new PredictionOutput();
            }

            if (ft)
            {
                //Increase the delay due to the latency and server tick:
                input.Delay += Game.Ping / 2000f + 0.06f;

                if (input.Aoe)
                {
                    return AoePrediction.GetPrediction(input);
                }
            }

            //Target too far away.
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon &&
                input.Unit.Distance(input.RangeCheckFrom, true) > Math.Pow(input.Range * 1.5, 2))
            {
                return new PredictionOutput { Input = input };
            }

            //Unit is dashing.
            if (input.Unit.IsDashing())
            {
                result = GetDashingPrediction(input);
            }
            else
            {
                //Unit is immobile.
                var remainingImmobileT = UnitIsImmobileUntil(input.Unit);
                if (remainingImmobileT >= 0d)
                {
                    result = GetImmobilePrediction(input, remainingImmobileT);
                }
            }

            //Normal prediction
            if (result == null)
            {
                result = GetStandardPrediction(input);
            }

            //Check if the unit position is in range
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance >= HitChance.High &&
                    input.RangeCheckFrom.Distance(input.Unit.Position, true) >
                    Math.Pow(input.Range + input.RealRadius * 3 / 4, 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.Distance(result.UnitPosition, true) >
                    Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.RealRadius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }

                /* This does not need to be handled for the updated predictions, but left as a reference.*/
                if (input.RangeCheckFrom.Distance(result.CastPosition, true) > Math.Pow(input.Range, 2))
                {
                    if (result.Hitchance != HitChance.OutOfRange)
                    {
                        result.CastPosition = input.RangeCheckFrom +
                                              input.Range *
                                              (result.UnitPosition - input.RangeCheckFrom).To2D().Normalized().To3D();
                    }
                    else
                    {
                        result.Hitchance = HitChance.OutOfRange;
                    }
                }
            }

            //Set hit chance
            if (result.Hitchance == HitChance.High)
            {
                result.Hitchance = GetHitChance(input);
                //.debug(input.Unit.BaseSkinName + result.Hitchance);
            }

            //Check for collision
            if (checkCollision && input.Collision && result.Hitchance > HitChance.Impossible)
            {
                var positions = new List<Vector3> { result.CastPosition };
                var originalUnit = input.Unit;
                if (Collision.GetCollision(positions, input))
                    result.Hitchance = HitChance.Collision;
            }
            return result;
        }

        internal static HitChance GetHitChance(PredictionInput input)
        {
            var hero = input.Unit as Obj_AI_Hero;

            
            if (hero == null || input.Radius <= 1f)
            {
                return HitChance.VeryHigh;
            }

            if (hero.IsCastingInterruptableSpell(true) || hero.IsRecalling())
            {
                return HitChance.VeryHigh;
            }

            if (hero.Path.Length > 0 != hero.IsMoving)
            {
                return HitChance.Medium;
            }

            var wayPoint = input.Unit.GetWaypoints().Last().To3D();
            var delay = input.Delay
                        + (Math.Abs(input.Speed - float.MaxValue) > float.Epsilon
                               ? hero.Distance(input.From) / input.Speed
                               : 0);
            var moveArea = hero.MoveSpeed * delay;
            var fixRange = moveArea * 0.4f;
            var minPath = 900 + moveArea;
            var moveAngle = 31d;

            if (UnitTracker.GetLastNewPathTime(input.Unit) < 0.1d && hero.Distance(wayPoint) > 100)
            {
                return HitChance.VeryHigh;
            }

            if (input.Radius > 70)
            {
                moveAngle++;
            }
            else if (input.Radius <= 60)
            {
                moveAngle--;
            }

            if (input.Delay < 0.3)
            {
                moveAngle++;
            }

            if (UnitTracker.GetLastNewPathTime(input.Unit) < 0.1d)
            {

                fixRange = moveArea * 0.3f;
                minPath = 700 + moveArea;
                moveAngle += 1.5;
            }

            if (input.Type == SkillshotType.SkillshotCircle)
            {
                fixRange -= input.Radius / 2;
            }

            if (input.From.Distance(wayPoint) <= hero.Distance(input.From))
            {
                if (hero.Distance(input.From) > input.Range - fixRange)
                {
                    return HitChance.Medium;
                }
            }
            else if (hero.Distance(wayPoint) > 350)
            {
                moveAngle += 1.5;
            }

            if (hero.Distance(input.From) < 250 || hero.MoveSpeed < 250 || input.From.Distance(wayPoint) < 250)
            {
                return HitChance.VeryHigh;
            }

            if (hero.Distance(wayPoint) > minPath)
            {
                return HitChance.VeryHigh;
            }

            if (hero.HealthPercent < 20 || ObjectManager.Player.HealthPercent < 20)
            {
                return HitChance.VeryHigh;
            }

            if (GetAngle(input.From, input.Unit) < moveAngle
                && hero.Distance(wayPoint) > 260)
            {
                return HitChance.VeryHigh;
            }

            if (input.Type == SkillshotType.SkillshotCircle
                && UnitTracker.GetLastNewPathTime(input.Unit) < 0.1d && hero.Distance(wayPoint) > fixRange)
            {
                return HitChance.VeryHigh;
            }

            return HitChance.High;
        }

        internal static PredictionOutput GetDashingPrediction(PredictionInput input)
        {
            var dashData = input.Unit.GetDashInfo();
            var result = new PredictionOutput { Input = input };
            //Normal dashes.
            if (!dashData.IsBlink)
            {
                //Mid air:
                var endP = dashData.Path.Last();
                var dashPred = GetPositionOnPath(
                    input, new List<Vector2> { input.Unit.ServerPosition.To2D(), endP }, dashData.Speed);
                if (dashPred.Hitchance >= HitChance.High && dashPred.UnitPosition.To2D().Distance(input.Unit.Position.To2D(), endP, true) < 200)
                {
                    dashPred.CastPosition = dashPred.UnitPosition;
                    dashPred.Hitchance = HitChance.Dashing;
                    return dashPred;
                }

                //At the end of the dash:
                if (dashData.Path.PathLength() > 200)
                {
                    var timeToPoint = input.Delay / 2f + input.From.To2D().Distance(endP) / input.Speed - 0.25f;
                    if (timeToPoint <=
                        input.Unit.Distance(endP) / dashData.Speed + input.RealRadius / input.Unit.MoveSpeed)
                    {
                        return new PredictionOutput
                        {
                            CastPosition = endP.To3D(),
                            UnitPosition = endP.To3D(),
                            Hitchance = HitChance.Dashing
                        };
                    }
                }
                result.CastPosition = dashData.Path.Last().To3D();
                result.UnitPosition = result.CastPosition;

                //Figure out where the unit is going.
            }

            return result;
        }

        internal static PredictionOutput GetImmobilePrediction(PredictionInput input, double remainingImmobileT)
        {
            var timeToReachTargetPosition = input.Delay + input.Unit.Distance(input.From) / input.Speed;

            if (timeToReachTargetPosition <= remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed)
            {
                return new PredictionOutput
                {
                    CastPosition = input.Unit.ServerPosition,
                    UnitPosition = input.Unit.Position,
                    Hitchance = HitChance.Immobile
                };
            }

            return new PredictionOutput
            {
                Input = input,
                CastPosition = input.Unit.ServerPosition,
                UnitPosition = input.Unit.ServerPosition,
                Hitchance = HitChance.High
                /*timeToReachTargetPosition - remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed < 0.4d ? HitChance.High : HitChance.Medium*/
            };
        }

        internal static PredictionOutput GetStandardPrediction(PredictionInput input)
        {
            var speed = input.Unit.MoveSpeed;

            if (input.Unit.Distance(input.From, true) < 200 * 200)
            {
                //input.Delay /= 2;
                speed /= 1.5f;
            }
            return GetPositionOnPath(input, input.Unit.GetWaypoints(), speed);
        }

        internal static double GetAngle(Vector3 from, Obj_AI_Base target)
        {
            var C = target.ServerPosition.To2D();
            var A = target.GetWaypoints().Last();

            if (C == A)
                return 60;

            var B = from.To2D();

            var AB = Math.Pow((double)A.X - (double)B.X, 2) + Math.Pow((double)A.Y - (double)B.Y, 2);
            var BC = Math.Pow((double)B.X - (double)C.X, 2) + Math.Pow((double)B.Y - (double)C.Y, 2);
            var AC = Math.Pow((double)A.X - (double)C.X, 2) + Math.Pow((double)A.Y - (double)C.Y, 2);

            return Math.Cos((AB + BC - AC) / (2 * Math.Sqrt(AB) * Math.Sqrt(BC))) * 180 / Math.PI;
        }

        internal static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare || buff.Type == BuffType.Fear
                         || buff.Type == BuffType.Taunt || buff.Type == BuffType.Knockback))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        internal static PredictionOutput GetPositionOnPath(PredictionInput input, List<Vector2> path, float speed = -1)
        {
            speed = (Math.Abs(speed - (-1)) < float.Epsilon) ? input.Unit.MoveSpeed : speed;

            if (path.Count <= 1)
            {
                return new PredictionOutput
                {
                    Input = input,
                    UnitPosition = input.Unit.ServerPosition,
                    CastPosition = input.Unit.ServerPosition,
                    Hitchance = HitChance.VeryHigh
                };
            }

            var pLength = path.PathLength();

            //Skillshots with only a delay
            if (pLength >= input.Delay * speed - input.RealRadius && Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
                var tDistance = input.Delay * speed - input.RealRadius;

                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var d = a.Distance(b);

                    if (d >= tDistance)
                    {
                        var direction = (b - a).Normalized();

                        var cp = a + direction * tDistance;
                        var p = a +
                                direction *
                                ((i == path.Count - 2)
                                    ? Math.Min(tDistance + input.RealRadius, d)
                                    : (tDistance + input.RealRadius));

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = cp.To3D(),
                            UnitPosition = p.To3D(),
                            Hitchance = HitChance.High
                        };
                    }

                    tDistance -= d;
                }
            }

            //Skillshot with a delay and speed.
            if (pLength >= input.Delay * speed - input.RealRadius &&
                Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                var d = input.Delay * speed - input.RealRadius;
                if (input.Type == SkillshotType.SkillshotLine || input.Type == SkillshotType.SkillshotCone)
                {
                    if (input.From.Distance(input.Unit.ServerPosition, true) < 200 * 200)
                    {
                        d = input.Delay * speed;
                    }
                }

                path = path.CutPath(d);
                var tT = 0f;
                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var tB = a.Distance(b) / speed;
                    var direction = (b - a).Normalized();
                    a = a - speed * tT * direction;
                    var sol = Geometry.VectorMovementCollision(a, b, speed, input.From.To2D(), input.Speed, tT);
                    var t = (float)sol[0];
                    var pos = (Vector2)sol[1];

                    if (pos.IsValid() && t >= tT && t <= tT + tB)
                    {
                        if (pos.Distance(b, true) < 20)
                            break;
                        var p = pos + input.RealRadius * direction;

                        if (input.Type == SkillshotType.SkillshotLine && false)
                        {
                            var alpha = (input.From.To2D() - p).AngleBetween(a - b);
                            if (alpha > 30 && alpha < 180 - 30)
                            {
                                var beta = (float)Math.Asin(input.RealRadius / p.Distance(input.From));
                                var cp1 = input.From.To2D() + (p - input.From.To2D()).Rotated(beta);
                                var cp2 = input.From.To2D() + (p - input.From.To2D()).Rotated(-beta);

                                pos = cp1.Distance(pos, true) < cp2.Distance(pos, true) ? cp1 : cp2;
                            }
                        }

                        return new PredictionOutput
                        {
                            
                            Input = input,
                            CastPosition = pos.To3D(),
                            UnitPosition = p.To3D(),
                            Hitchance = HitChance.High
                        };
                    }
                    tT += tB;
                }
            }

            var position = path.Last();
            return new PredictionOutput
            {
                Input = input,
                CastPosition = position.To3D(),
                UnitPosition = position.To3D(),
                Hitchance = HitChance.Medium
            };
        }


    }

    internal static class AoePrediction
    {
        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            switch (input.Type)
            {
                case SkillshotType.SkillshotCircle:
                    return Circle.GetPrediction(input);
                case SkillshotType.SkillshotCone:
                    return Cone.GetPrediction(input);
                case SkillshotType.SkillshotLine:
                    return Line.GetPrediction(input);
            }
            return new PredictionOutput();
        }

        internal static List<PossibleTarget> GetPossibleTargets(PredictionInput input)
        {
            var result = new List<PossibleTarget>();
            var originalUnit = input.Unit;
            foreach (var enemy in
                HeroManager.Enemies.FindAll(
                    h =>
                        h.NetworkId != originalUnit.NetworkId &&
                        h.IsValidTarget((input.Range + 200 + input.RealRadius), true, input.RangeCheckFrom)))
            {
                input.Unit = enemy;
                var prediction = Prediction.GetPrediction(input, false, false);
                if (prediction.Hitchance >= HitChance.High)
                {
                    result.Add(new PossibleTarget { Position = prediction.UnitPosition.To2D(), Unit = enemy });
                }
            }
            return result;
        }

        public static class Circle
        {
            public static PredictionOutput GetPrediction(PredictionInput input)
            {
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                {
                    new PossibleTarget { Position = mainTargetPrediction.UnitPosition.To2D(), Unit = input.Unit }
                };

                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    //Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                while (posibleTargets.Count > 1)
                {
                    var mecCircle = MEC.GetMec(posibleTargets.Select(h => h.Position).ToList());

                    if (mecCircle.Radius <= input.RealRadius - 10 &&
                        Vector2.DistanceSquared(mecCircle.Center, input.RangeCheckFrom.To2D()) <
                        input.Range * input.Range)
                    {
                        return new PredictionOutput
                        {
                            AoeTargetsHit = posibleTargets.Select(h => (Obj_AI_Hero)h.Unit).ToList(),
                            CastPosition = mecCircle.Center.To3D(),
                            UnitPosition = mainTargetPrediction.UnitPosition,
                            Hitchance = mainTargetPrediction.Hitchance,
                            Input = input,
                            _aoeTargetsHitCount = posibleTargets.Count
                        };
                    }

                    float maxdist = -1;
                    var maxdistindex = 1;
                    for (var i = 1; i < posibleTargets.Count; i++)
                    {
                        var distance = Vector2.DistanceSquared(posibleTargets[i].Position, posibleTargets[0].Position);
                        if (distance > maxdist || maxdist.CompareTo(-1) == 0)
                        {
                            maxdistindex = i;
                            maxdist = distance;
                        }
                    }
                    posibleTargets.RemoveAt(maxdistindex);
                }

                return mainTargetPrediction;
            }
        }

        public static class Cone
        {
            internal static int GetHits(Vector2 end, double range, float angle, List<Vector2> points)
            {
                return (from point in points
                        let edge1 = end.Rotated(-angle / 2)
                        let edge2 = edge1.Rotated(angle)
                        where
                            point.Distance(new Vector2(), true) < range * range && edge1.CrossProduct(point) > 0 &&
                            point.CrossProduct(edge2) > 0
                        select point).Count();
            }

            public static PredictionOutput GetPrediction(PredictionInput input)
            {
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                {
                    new PossibleTarget { Position = mainTargetPrediction.UnitPosition.To2D(), Unit = input.Unit }
                };

                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    //Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                if (posibleTargets.Count > 1)
                {
                    var candidates = new List<Vector2>();

                    foreach (var target in posibleTargets)
                    {
                        target.Position = target.Position - input.From.To2D();
                    }

                    for (var i = 0; i < posibleTargets.Count; i++)
                    {
                        for (var j = 0; j < posibleTargets.Count; j++)
                        {
                            if (i != j)
                            {
                                var p = (posibleTargets[i].Position + posibleTargets[j].Position) * 0.5f;
                                if (!candidates.Contains(p))
                                {
                                    candidates.Add(p);
                                }
                            }
                        }
                    }

                    var bestCandidateHits = -1;
                    var bestCandidate = new Vector2();
                    var positionsList = posibleTargets.Select(t => t.Position).ToList();

                    foreach (var candidate in candidates)
                    {
                        var hits = GetHits(candidate, input.Range, input.Radius, positionsList);
                        if (hits > bestCandidateHits)
                        {
                            bestCandidate = candidate;
                            bestCandidateHits = hits;
                        }
                    }

                    bestCandidate = bestCandidate + input.From.To2D();

                    if (bestCandidateHits > 1 && input.From.To2D().Distance(bestCandidate, true) > 50 * 50)
                    {
                        return new PredictionOutput
                        {
                            Hitchance = mainTargetPrediction.Hitchance,
                            _aoeTargetsHitCount = bestCandidateHits,
                            UnitPosition = mainTargetPrediction.UnitPosition,
                            CastPosition = bestCandidate.To3D(),
                            Input = input
                        };
                    }
                }
                return mainTargetPrediction;
            }
        }

        public static class Line
        {
            internal static IEnumerable<Vector2> GetHits(Vector2 start, Vector2 end, double radius, List<Vector2> points)
            {
                return points.Where(p => p.Distance(start, end, true, true) <= radius * radius);
            }

            internal static Vector2[] GetCandidates(Vector2 from, Vector2 to, float radius, float range)
            {
                var middlePoint = (from + to) / 2;
                var intersections = Geometry.CircleCircleIntersection(
                    from, middlePoint, radius, from.Distance(middlePoint));

                if (intersections.Length > 1)
                {
                    var c1 = intersections[0];
                    var c2 = intersections[1];

                    c1 = from + range * (to - c1).Normalized();
                    c2 = from + range * (to - c2).Normalized();

                    return new[] { c1, c2 };
                }

                return new Vector2[] { };
            }

            public static PredictionOutput GetPrediction(PredictionInput input)
            {
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                {
                    new PossibleTarget { Position = mainTargetPrediction.UnitPosition.To2D(), Unit = input.Unit }
                };
                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    //Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                if (posibleTargets.Count > 1)
                {
                    var candidates = new List<Vector2>();
                    foreach (var target in posibleTargets)
                    {
                        var targetCandidates = GetCandidates(
                            input.From.To2D(), target.Position, (input.Radius), input.Range);
                        candidates.AddRange(targetCandidates);
                    }

                    var bestCandidateHits = -1;
                    var bestCandidate = new Vector2();
                    var bestCandidateHitPoints = new List<Vector2>();
                    var positionsList = posibleTargets.Select(t => t.Position).ToList();

                    foreach (var candidate in candidates)
                    {
                        if (
                            GetHits(
                                input.From.To2D(), candidate, (input.Radius + input.Unit.BoundingRadius / 3 - 10),
                                new List<Vector2> { posibleTargets[0].Position }).Count() == 1)
                        {
                            var hits = GetHits(input.From.To2D(), candidate, input.Radius, positionsList).ToList();
                            var hitsCount = hits.Count;
                            if (hitsCount >= bestCandidateHits)
                            {
                                bestCandidateHits = hitsCount;
                                bestCandidate = candidate;
                                bestCandidateHitPoints = hits.ToList();
                            }
                        }
                    }

                    if (bestCandidateHits > 1)
                    {
                        float maxDistance = -1;
                        Vector2 p1 = new Vector2(), p2 = new Vector2();

                        //Center the position
                        for (var i = 0; i < bestCandidateHitPoints.Count; i++)
                        {
                            for (var j = 0; j < bestCandidateHitPoints.Count; j++)
                            {
                                var startP = input.From.To2D();
                                var endP = bestCandidate;
                                var proj1 = positionsList[i].ProjectOn(startP, endP);
                                var proj2 = positionsList[j].ProjectOn(startP, endP);
                                var dist = Vector2.DistanceSquared(bestCandidateHitPoints[i], proj1.LinePoint) +
                                           Vector2.DistanceSquared(bestCandidateHitPoints[j], proj2.LinePoint);
                                if (dist >= maxDistance &&
                                    (proj1.LinePoint - positionsList[i]).AngleBetween(
                                        proj2.LinePoint - positionsList[j]) > 90)
                                {
                                    maxDistance = dist;
                                    p1 = positionsList[i];
                                    p2 = positionsList[j];
                                }
                            }
                        }

                        return new PredictionOutput
                        {
                            Hitchance = mainTargetPrediction.Hitchance,
                            _aoeTargetsHitCount = bestCandidateHits,
                            UnitPosition = mainTargetPrediction.UnitPosition,
                            CastPosition = ((p1 + p2) * 0.5f).To3D(),
                            Input = input
                        };
                    }
                }

                return mainTargetPrediction;
            }
        }

        internal class PossibleTarget
        {
            public Vector2 Position;
            public Obj_AI_Base Unit;
        }
    }

    public static class Collision
    {
        static Collision()
        {

        }

        /// <summary>
        ///     Returns the list of the units that the skillshot will hit before reaching the set positions.
        /// </summary>
        /// 
        private static bool MinionIsDead(PredictionInput input, Obj_AI_Base minion, float distance)
        {
            float delay = (distance / input.Speed) + input.Delay;

            if (Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
                delay = input.Delay;

            int convert = (int)(delay * 1000);

            if (HealthPrediction.LaneClearHealthPrediction(minion, convert, 0) <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool GetCollision(List<Vector3> positions, PredictionInput input)
        {

            foreach (var position in positions)
            {
                foreach (var objectType in input.CollisionObjects)
                {
                    switch (objectType)
                    {
                        case CollisionableObjects.Minions:
                            foreach (var minion in Cache.GetMinions(input.From, Math.Min(input.Range + input.Radius + 100, 2000)))
                            {
                                input.Unit = minion;

                                var distanceFromToUnit = minion.ServerPosition.Distance(input.From);

                                if (distanceFromToUnit < input.Radius + minion.BoundingRadius)
                                {
                                    if (MinionIsDead(input, minion, distanceFromToUnit))
                                        continue;
                                    else
                                        return true;
                                }
                                else if (minion.ServerPosition.Distance(position) < input.Radius + minion.BoundingRadius)
                                {
                                    if (MinionIsDead(input, minion, distanceFromToUnit))
                                        continue;
                                    else
                                        return true;
                                }
                                else
                                {
                                    var minionPos = minion.ServerPosition;
                                    int bonusRadius = 20;
                                    if (minion.IsMoving)
                                    {
                                        minionPos = Prediction.GetPrediction(input, false, false).CastPosition;
                                        bonusRadius = 60 + (int)input.Radius;
                                    }

                                    if (minionPos.To2D().Distance(input.From.To2D(), position.To2D(), true, true) <= Math.Pow((input.Radius + bonusRadius + minion.BoundingRadius), 2))
                                    {
                                        if (MinionIsDead(input, minion, distanceFromToUnit))
                                            continue;
                                        else
                                            return true;
                                    }
                                }
                            }
                            break;
                        case CollisionableObjects.Heroes:
                            foreach (var hero in
                                HeroManager.Enemies.FindAll(
                                    hero =>
                                        hero.IsValidTarget(
                                            Math.Min(input.Range + input.Radius + 100, 2000), true, input.RangeCheckFrom))
                                )
                            {
                                input.Unit = hero;
                                var prediction = Prediction.GetPrediction(input, false, false);
                                if (
                                    prediction.UnitPosition.To2D()
                                        .Distance(input.From.To2D(), position.To2D(), true, true) <=
                                    Math.Pow((input.Radius + 50 + hero.BoundingRadius), 2))
                                {
                                    return true;
                                }
                            }
                            break;

                        case CollisionableObjects.Walls:
                            var step = position.Distance(input.From) / 20;
                            for (var i = 0; i < 20; i++)
                            {
                                var p = input.From.To2D().Extend(position.To2D(), step * i);
                                if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(CollisionFlags.Wall))
                                {
                                    return true;
                                }
                            }
                            break;
                    }
                }
            }
            return false;
        }
    }

    internal class PathInfo
    {
        public Vector2 Position { get; set; }
        public float Time { get; set; }
    }

    internal class Spells
    {
        public string name { get; set; }
        public double duration { get; set; }
    }

    internal class UnitTrackerInfo
    {
        public int NetworkId { get; set; }
        public int AaTick { get; set; }
        public int NewPathTick { get; set; }
        public int StopMoveTick { get; set; }
        public int LastInvisableTick { get; set; }
        public int SpecialSpellFinishTick { get; set; }
        public List<PathInfo> PathBank = new List<PathInfo>();
    }

    internal static class UnitTracker
    {
        public static List<UnitTrackerInfo> UnitTrackerInfoList = new List<UnitTrackerInfo>();
        private static List<Obj_AI_Hero> Champion = new List<Obj_AI_Hero>();
        private static List<Spells> spells = new List<Spells>();
        private static List<PathInfo> PathBank = new List<PathInfo>();
        static UnitTracker()
        {
            spells.Add(new Spells() { name = "katarinar", duration = 1 }); //Katarinas R
            spells.Add(new Spells() { name = "drain", duration = 1 }); //Fiddle W
            spells.Add(new Spells() { name = "crowstorm", duration = 1 }); //Fiddle R
            spells.Add(new Spells() { name = "consume", duration = 0.5 }); //Nunu Q
            spells.Add(new Spells() { name = "absolutezero", duration = 1 }); //Nunu R
            spells.Add(new Spells() { name = "staticfield", duration = 0.5 }); //Blitzcrank R
            spells.Add(new Spells() { name = "cassiopeiapetrifyinggaze", duration = 0.5 }); //Cassio's R
            spells.Add(new Spells() { name = "ezrealtrueshotbarrage", duration = 1 }); //Ezreal's R
            spells.Add(new Spells() { name = "galioidolofdurand", duration = 1 }); //Ezreal's R                                                                   
            spells.Add(new Spells() { name = "luxmalicecannon", duration = 1 }); //Lux R
            spells.Add(new Spells() { name = "reapthewhirlwind", duration = 1 }); //Jannas R
            spells.Add(new Spells() { name = "jinxw", duration = 0.6 }); //jinxW
            spells.Add(new Spells() { name = "jinxr", duration = 0.6 }); //jinxR
            spells.Add(new Spells() { name = "missfortunebullettime", duration = 1 }); //MissFortuneR
            spells.Add(new Spells() { name = "shenstandunited", duration = 1 }); //ShenR
            spells.Add(new Spells() { name = "threshe", duration = 0.4 }); //ThreshE
            spells.Add(new Spells() { name = "threshrpenta", duration = 0.75 }); //ThreshR
            spells.Add(new Spells() { name = "threshq", duration = 0.75 }); //ThreshQ
            spells.Add(new Spells() { name = "infiniteduress", duration = 1 }); //Warwick R
            spells.Add(new Spells() { name = "meditate", duration = 1 }); //yi W
            spells.Add(new Spells() { name = "alzaharnethergrasp", duration = 1 }); //Malza R
            spells.Add(new Spells() { name = "lucianq", duration = 0.5 }); //Lucian Q
            spells.Add(new Spells() { name = "caitlynpiltoverpeacemaker", duration = 0.5 }); //Caitlyn Q
            spells.Add(new Spells() { name = "velkozr", duration = 0.5 }); //Velkoz R 
            spells.Add(new Spells() { name = "jhinr", duration = 2 }); //Velkoz R 

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                Champion.Add(hero);
                UnitTrackerInfoList.Add(new UnitTrackerInfo() { NetworkId = hero.NetworkId, AaTick = Utils.TickCount, StopMoveTick = Utils.TickCount, NewPathTick = Utils.TickCount, SpecialSpellFinishTick = Utils.TickCount, LastInvisableTick = Utils.TickCount });
            }

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnNewPath += Obj_AI_Hero_OnNewPath;
            AttackableUnit.OnEnterVisiblityClient += Obj_AI_Base_OnEnterLocalVisiblityClient;
        }

        private static void Obj_AI_Base_OnEnterLocalVisiblityClient(GameObject sender, EventArgs args)
        {
            if (sender.Type != GameObjectType.obj_AI_Hero) return;

            UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).LastInvisableTick = Utils.TickCount;
        }

        private static void Obj_AI_Hero_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.Type != GameObjectType.obj_AI_Hero) return;

            var info = UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId);

            info.NewPathTick = Utils.TickCount;

            if (args.Path.Count() == 1 && !sender.IsMoving) // STOP MOVE DETECTION
                UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).StopMoveTick = Utils.TickCount;
            else // SPAM CLICK LOGIC
                info.PathBank.Add(new PathInfo() { Position = args.Path.Last().To2D(), Time = Game.Time });

            if (info.PathBank.Count > 3)
                info.PathBank.Remove(info.PathBank.First());
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Hero)) return;

            if (args.SData.IsAutoAttack())
                UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).AaTick = Utils.TickCount;
            else
            {
                var foundSpell = spells.Find(x => args.SData.Name.ToLower() == x.name.ToLower());
                if (foundSpell != null)
                {
                    UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).SpecialSpellFinishTick = Utils.TickCount + (int)(foundSpell.duration * 1000);
                }
            }
        }

        public static bool SpamSamePlace(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            if (TrackerUnit.PathBank.Count < 3)
                return false;

            if (TrackerUnit.PathBank[2].Time - TrackerUnit.PathBank[1].Time < 0.2f
                && TrackerUnit.PathBank[2].Time + 0.1f < Game.Time
                && TrackerUnit.PathBank[1].Position.Distance(TrackerUnit.PathBank[2].Position) < 100)
            {
                return true;
            }
            else
                return false;
        }

        public static bool PathCalc(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            if (TrackerUnit.PathBank.Count < 3)
                return false;

            if (TrackerUnit.PathBank[2].Time - TrackerUnit.PathBank[0].Time < 0.4f && Game.Time - TrackerUnit.PathBank[2].Time < 0.1
                && TrackerUnit.PathBank[2].Position.Distance(unit.Position) < 300
                && TrackerUnit.PathBank[1].Position.Distance(unit.Position) < 300
                && TrackerUnit.PathBank[0].Position.Distance(unit.Position) < 300)
            {
                var dis = unit.Distance(TrackerUnit.PathBank[2].Position);
                if (TrackerUnit.PathBank[1].Position.Distance(TrackerUnit.PathBank[2].Position) > dis && TrackerUnit.PathBank[0].Position.Distance(TrackerUnit.PathBank[1].Position) > dis)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static List<Vector2> GetPathWayCalc(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            List<Vector2> points = new List<Vector2>();
            points.Add(unit.ServerPosition.To2D());
            return points;
        }

        public static double GetSpecialSpellEndTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            return (TrackerUnit.SpecialSpellFinishTick - Utils.TickCount) / 1000d;
        }

        public static double GetLastAutoAttackTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            return (Utils.TickCount - TrackerUnit.AaTick) / 1000d;
        }

        public static double GetLastNewPathTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            return (Utils.TickCount - TrackerUnit.NewPathTick) / 1000d;
        }

        public static double GetLastVisableTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            return (Utils.TickCount - TrackerUnit.LastInvisableTick) / 1000d;
        }

        public static double GetLastStopMoveTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            return (Utils.TickCount - TrackerUnit.StopMoveTick) / 1000d;
        }
    }

}