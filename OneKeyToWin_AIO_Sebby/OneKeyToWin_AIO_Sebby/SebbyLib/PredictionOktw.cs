using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;

namespace SebbyLib.Prediction
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
        public bool Aoe = false;
        public bool Collision = false;
        public CollisionableObjects[] CollisionObjects =
        {
            CollisionableObjects.Minions, CollisionableObjects.YasuoWall
        };
        public float Delay;
        public float Radius = 1f;
        public float Range = float.MaxValue;
        public float Speed = float.MaxValue;
        public SkillshotType Type = SkillshotType.SkillshotLine;
        public Obj_AI_Base Unit = ObjectManager.Player;
        public Obj_AI_Base Source = ObjectManager.Player;
        public bool UseBoundingRadius = true;

        public Vector3 From
        {
            get { return _from.To2D().IsValid() ? _from : ObjectManager.Player.ServerPosition; }
            set { _from = value; }
        }

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
        public List<Obj_AI_Hero> AoeTargetsHit = new List<Obj_AI_Hero>();
        public List<Obj_AI_Base> CollisionObjects = new List<Obj_AI_Base>();
        public HitChance Hitchance = HitChance.Impossible;
        internal PredictionInput Input;

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

        public int AoeTargetsHitCount
        {
            get { return Math.Max(_aoeTargetsHitCount, AoeTargetsHit.Count); }
        }

        public Vector3 UnitPosition
        {
            get { return _unitPosition.To2D().IsValid() ? _unitPosition.SetZ() : Input.Unit.ServerPosition; }
            set { _unitPosition = value; }
        }
    }

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
                result = GetPositionOnPath(input, input.Unit.GetWaypoints(), input.Unit.MoveSpeed);
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
                result = WayPointAnalysis(result, input);
                //.debug(input.Unit.BaseSkinName + result.Hitchance);
            }

            //Check for collision
            if (checkCollision && input.Collision && result.Hitchance > HitChance.Impossible)
            {
                var positions = new List<Vector3> { result.CastPosition };
                if (input.Delay > 0.3)
                    positions.Add(input.Unit.Position);

                var originalUnit = input.Unit;
                if (Collision.GetCollision(positions, input))
                    result.Hitchance = HitChance.Collision;
            }

            return result;
        }

        public static bool PointInLineSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 point)
        {
            var distanceStartEnd = segmentStart.Distance(segmentEnd, true);
            var distanceStartPoint = segmentStart.Distance(point, true);
            var distanceEndPoint = segmentEnd.Distance(point, true);
            return !(distanceEndPoint > distanceStartEnd || distanceStartPoint > distanceStartEnd);
        }

        public static float UnitIsSlowed(Obj_AI_Base unit)
        {
            var t = 0f;
            foreach (var buff in unit.Buffs)
                if (buff.Type == BuffType.Sleep)
                    t = buff.EndTime - Game.Time;

            return t;
        }

        internal static PredictionOutput WayPointAnalysis(PredictionOutput result, PredictionInput input)
        {
            if (!(input.Unit is Obj_AI_Hero) || input.Radius == 1)
            {
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }

            // CAN'T MOVE SPELLS ///////////////////////////////////////////////////////////////////////////////////

            if (UnitTracker.GetSpecialSpellEndTime(input.Unit) > 100 || input.Unit.HasBuff("Recall") || (UnitTracker.GetLastStopMoveTime(input.Unit) < 100 && input.Unit.IsRooted))
            {
                OktwCommon.debug("CAN'T MOVE SPELLS");
                result.Hitchance = HitChance.VeryHigh;
                result.CastPosition = input.Unit.Position;
                return result;
            }

            // PREPARE MATH ///////////////////////////////////////////////////////////////////////////////////
            var wayPoints = input.Unit.GetWaypoints();
            var lastWaypiont = wayPoints.Last().To3D();
            var distanceUnitToWaypoint = lastWaypiont.Distance(input.Unit.Position);
            var distanceFromToUnit = input.From.Distance(input.Unit.Position);
            var distanceFromToWaypoint = lastWaypiont.Distance(input.From);
            var speedDelay = distanceFromToUnit / input.Speed;

            if (Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
                speedDelay = 0;

            float totalDelay = speedDelay + input.Delay - (input.Radius / input.Unit.MoveSpeed);
            float moveArea = input.Unit.MoveSpeed * totalDelay;
            float fixRange = moveArea * 0.25f;

            var LastNewPathTime = UnitTracker.GetLastNewPathTime(input.Unit);
            Vector2 pos1 = lastWaypiont.To2D() - input.Unit.Position.To2D();
            Vector2 pos2 = input.From.To2D() - input.Unit.Position.To2D();
            var getAngle = pos1.AngleBetween(pos2);

            // NEW VISABLE ///////////////////////////////////////////////////////////////////////////////////
            if (UnitTracker.GetLastVisableTime(input.Unit) < 100 || input.Unit.Path.Count() == 0)
            {
                OktwCommon.debug("PRED M: NEW VISABLE");
                result.Hitchance = HitChance.Medium;
                return result;
            }

            // FIX RANGE ///////////////////////////////////////////////////////////////////////////////////
            if (distanceFromToUnit > input.Range - fixRange)
            {
                if (input.Type != SkillshotType.SkillshotCircle)
                {
                    if (wayPoints.Count() <= 1 || getAngle < 150)
                    {
                        OktwCommon.debug("PRED M: Fix Range");
                        result.Hitchance = HitChance.Medium;
                        return result;
                    }
                }
            }

            if (UnitTracker.IsSpamClick(input.Unit, input.Radius, input.Delay))
            {
                OktwCommon.debug("PRED VH: SPAM CLICK");
                result.Hitchance = HitChance.VeryHigh;
                result.CastPosition = input.Unit.Position;
                return result;
            }

            // SHORT CLICK DETECTION ///////////////////////////////////////////////////////////////////////////////////
            if (distanceUnitToWaypoint > 0 && distanceUnitToWaypoint < 200)
            {
                OktwCommon.debug("PRED M: SMALL WAYPOINT");
                result.Hitchance = HitChance.Medium;
                return result;
            }

            // INCOMING DAMAGE  ///////////////////////////////////////////////////////////////////////////////////
            var incoming = OktwCommon.GetIncomingDamage((Obj_AI_Hero)input.Unit);
            if (input.Unit.Health - incoming < HeroManager.Player.MaxHealth * 0.2f && incoming > 10)
            {
                OktwCommon.debug("PRED VH: LOW HP ENEMY");
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }

            if (HeroManager.Player.MaxHealth < 20 && HeroManager.Player.Health - OktwCommon.GetIncomingDamage(HeroManager.Player) < HeroManager.Player.MaxHealth * 0.2f)
            {
                OktwCommon.debug("PRED VH: LOW HP MY HERO");
                result.Hitchance = HitChance.VeryHigh;
            }

            // SPECIAL CASES ///////////////////////////////////////////////////////////////////////////////////
            if (distanceFromToUnit < 250)
            {
                OktwCommon.debug("PRED VH: Near");
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }
            else if (distanceFromToWaypoint < 250)
            {
                OktwCommon.debug("PRED VH: on way");
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }

            // SLOW LOGIC /////////////////////////////////////////////////////////////////////////////////// 
            var remainingSlowT = UnitIsSlowed(input.Unit);
            if (remainingSlowT > 0)
            {
                var timeToReachTargetPosition = input.Delay + input.Unit.Position.Distance(input.From) / input.Speed;

                if (timeToReachTargetPosition <= remainingSlowT + input.RealRadius / input.Unit.MoveSpeed)
                {
                    OktwCommon.debug("PRED VH: SLOW");
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }
                else
                {
                    OktwCommon.debug("PRED M: SLOW BAD");
                    result.Hitchance = HitChance.High;
                    return result;
                }
            }

            // DON'T MOVE
            if (wayPoints.Count() <= 1)
            {
                var stoptime = UnitTracker.GetLastStopMoveTime(input.Unit);
                if (stoptime < 1000)
                {
                    var acitveSpell = input.Unit.Spellbook;

                    if (acitveSpell.IsCastingSpell && totalDelay < acitveSpell.CastEndTime - Game.Time && !input.Unit.CanMove)
                    {
                        OktwCommon.debug("PRED VH: Spell Cast Detection");
                        result.Hitchance = HitChance.VeryHigh;
                        return result;
                    }
                    else if (!acitveSpell.IsAutoAttacking && acitveSpell.CastEndTime - Game.Time < 0.1 && !input.Unit.CanMove)
                    {
                        OktwCommon.debug("PRED M: STOP CAST SPELL SOON ");
                        result.Hitchance = HitChance.Medium;
                        return result;
                    }
                    else if (UnitTracker.GetLastAutoAttackTime(input.Unit) < 60 && input.Delay < 0.3)
                    {
                        OktwCommon.debug("PRED VH: AA detection");
                        result.Hitchance = HitChance.VeryHigh;
                        return result;
                    }
                    else
                    {
                        OktwCommon.debug("PRED M: STOP HIGH ");
                        result.Hitchance = HitChance.Medium;
                        return result;
                    }
                }
                else
                {
                    OktwCommon.debug("PRED VH: STOP LOGIC");
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }
            }
            else //MOVE 
            {
                //if (Game.IsFogOfWar(input.Unit.Position.Extend(wayPoints.back(), 100)))
                //{
                //    OktwCommon.debug( "PRED VH: FOW cast");
                //    result.Hitchance = HitChance.VeryHigh;
                //    return result;
                //}

                if (distanceUnitToWaypoint > 1200)
                {
                    OktwCommon.debug("PRED VH: LONG CLICK DETECTION");
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }

                //if (getAngle < 110 && getAngle > 40 && LastNewPathTime < 200)
                //{
                //	OktwCommon.debug( "PRED M: Bad Click");
                //	result.Hitchance = HitChance.Medium;
                //	return result;
                //}

                if (getAngle > 105 && getAngle < 150 && distanceUnitToWaypoint < 600)
                {
                    OktwCommon.debug("PRED M: Bad Click Angle");
                    result.Hitchance = HitChance.Medium;
                    return result;
                }

                if (distanceUnitToWaypoint > 500)
                {
                    if (getAngle > 170 || getAngle < 10)
                    {
                        OktwCommon.debug("PRED VH: Angle run long cast");
                        result.Hitchance = HitChance.VeryHigh;
                        return result;
                    }
                }

                if (input.Radius >= 90 && input.Speed > 3000 && input.Delay < 0.7)
                {
                    OktwCommon.debug("PRED VH: RADIUS GOOD");
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }

                if (distanceUnitToWaypoint > 500)
                {
                    // RUN IN LANE DETECTION /////////////////////////////////////////////////////////////////////////////////// 
                    foreach (var x in ObjectManager.Get<Obj_AI_Turret>())
                    {
                        if (x.Team == input.Unit.Team && !x.IsDead && x.Distance(input.From) < 2500 && x.Distance(input.Unit.Position) > 400)
                        {
                            var pos3 = x.Position - input.Unit.Position;
                            var getAngle2 = pos1.AngleBetween(pos3.To2D());
                            if (getAngle2 < 10)
                            {
                                OktwCommon.debug("PRED VH: Angle TURRET");
                                result.Hitchance = HitChance.VeryHigh;
                                return result;
                            }
                        }
                    }
                }
                var inSameDriection = OktwCommon.IsMovingInSameDirection(input.Unit, ObjectManager.Player);
                if (inSameDriection || distanceUnitToWaypoint > 600)
                {
                    if (getAngle > 130 || getAngle < 15)
                    {
                        OktwCommon.debug("PRED VH: Angle run");
                        result.Hitchance = HitChance.VeryHigh;
                        return result;
                    }
                }

                if (wayPoints.Count() == 2 && LastNewPathTime < 80 && totalDelay < 0.25)
                {
                    OktwCommon.debug("PRED VH: NEW PATH");
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }
            }
            result.Hitchance = HitChance.High;
            return result;
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

        internal static Vector3 GetWallPoint(Vector3 from, float range)
        {
            var count = 30;
            var points = OktwCommon.CirclePoints(count, range, from);
            Vector3 first = Vector3.Zero, last = Vector3.Zero;

            for (int i = 0; i < count; i++)
            {
                if (points[i].IsWall())
                {
                    if (first.IsZero)
                    {
                        if (i == count - 1)
                        {
                            if (!points[0].IsWall())
                                first = points[i];
                        }
                        else
                        {
                            if (!points[i + 1].IsWall())
                                first = points[i];
                        }
                    }
                    if (last.IsZero)
                    {
                        if (i == 0)
                        {
                            if (!points[count - 1].IsWall())
                                last = points[i];
                        }
                        else
                        {
                            if (!points[i - 1].IsWall())
                                last = points[i];
                        }
                    }
                }
            }
            if (!first.IsZero && !last.IsZero)
            {
                var finnaly = new Vector3((last.X + first.X) / 2, (last.Y + first.Y) / 2, (last.Z + first.Z) / 2);
                return finnaly;
            }
            else
                return Vector3.Zero;
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
            if (input.Unit.Distance(input.From, true) < 230 * 230)
            {
                input.Delay /= 2;
                speed /= 1.5f;
            }

            speed = (Math.Abs(speed - (-1)) < float.Epsilon) ? input.Unit.MoveSpeed : speed;

            if (path.Count <= 1 || (input.Unit.IsWindingUp && !input.Unit.IsDashing()))
            {
                return new PredictionOutput
                {
                    Input = input,
                    UnitPosition = input.Unit.ServerPosition,
                    CastPosition = input.Unit.ServerPosition,
                    Hitchance = HitChance.High
                };
            }

            var pLength = path.PathLength();

            //Skillshots with only a delay
            var tDistance = input.Delay * speed - input.RealRadius;
            if (pLength >= tDistance && Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
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
            if (pLength >= tDistance && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                var d = tDistance;
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

        private static bool MinionIsDead(PredictionInput input, Obj_AI_Base minion, float distance)
        {
            float delay = (distance / input.Speed) + input.Delay;

            if (Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
                delay = input.Delay;

            int convert = (int)(delay * 1000) - Game.Ping;

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

                                var distanceFromToUnit = minion.ServerPosition.Distance(input.From);
                                var bOffset = minion.BoundingRadius + input.Unit.BoundingRadius;
                                if (distanceFromToUnit < bOffset)
                                {
                                    if (MinionIsDead(input, minion, distanceFromToUnit))
                                        continue;
                                    else
                                        return true;
                                }
                                else if (minion.ServerPosition.Distance(position) < bOffset)
                                {
                                    if (MinionIsDead(input, minion, distanceFromToUnit))
                                        continue;
                                    else
                                        return true;
                                }
                                else if (minion.ServerPosition.Distance(input.Unit.Position) < bOffset)
                                {
                                    if (MinionIsDead(input, minion, distanceFromToUnit))
                                        continue;
                                    else
                                        return true;
                                }
                                else
                                {
                                    var minionPos = minion.ServerPosition;
                                    int bonusRadius = 15;
                                    if (minion.IsMoving)
                                    {
                                        var predInput2 = new PredictionInput
                                        {
                                            Collision = false,
                                            Speed = input.Speed,
                                            Delay = input.Delay,
                                            Range = input.Range,
                                            From = input.From,
                                            Radius = input.Radius,
                                            Unit = minion,
                                            Type = input.Type
                                        };
                                        minionPos = Prediction.GetPrediction(predInput2).CastPosition;
                                        bonusRadius = 50 + (int)input.Radius;
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
        public Vector3[] Position { get; set; }
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
        public float ClickAvarageTime { get; set; }
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
            if (sender is Obj_AI_Hero)
                UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).LastInvisableTick = Utils.TickCount;
        }

        private static void Obj_AI_Hero_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender is Obj_AI_Hero)
            {
                var x = UnitTrackerInfoList.Find(y => y.NetworkId == sender.NetworkId);
                if (x.PathBank.Count() > 2)
                {
                    float time0 = Utils.TickCount - x.PathBank[2].Time;
                    float time1 = x.PathBank[2].Time - x.PathBank[1].Time;
                    float time2 = x.PathBank[1].Time - x.PathBank[0].Time;
                    var i = 1;
                    float sum = 0;
                    if (time0 < 400 && args.Path.Count() > 1)
                    {
                        sum += time0;
                        i++;
                    }
                    if (time1 < 400 && x.PathBank[2].Position.Count() > 1)
                    {
                        sum += time1;
                        i++;
                    }
                    if (time2 < 400 && x.PathBank[1].Position.Count() > 1)
                    {
                        sum += time2;
                        i++;
                    }
                    x.ClickAvarageTime = (x.ClickAvarageTime + sum) / i;
                }

                x.PathBank.Add(new PathInfo() { Position = args.Path, Time = Utils.TickCount });

                if (x.PathBank.Count > 3)
                    x.PathBank.RemoveAt(0);

                if (args.Path.Count() > 1 && args.Path.Last().Distance(args.Path.First()) > 1) // STOP MOVE DETECTION
                    x.NewPathTick = Utils.TickCount;
                else
                    x.StopMoveTick = Utils.TickCount;

            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is Obj_AI_Hero)
            {
                if (args.SData.IsAutoAttack())
                    UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).AaTick = Utils.TickCount;
                else
                {

                    var foundSpell = spells.Find(x => args.SData.Name.ToLower() == x.name.ToLower());
                    if (foundSpell != null)
                    {
                        UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).SpecialSpellFinishTick = Utils.TickCount + (int)(foundSpell.duration * 1000);
                    }
                    else if (sender.IsWindingUp || sender.IsRooted || !sender.CanMove)
                    {
                        UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).SpecialSpellFinishTick = Utils.TickCount + 100;
                    }
                }
            }
        }

        public static bool IsSpamClick(Obj_AI_Base unit, float radius, float delay)
        {
            var x = UnitTrackerInfoList.Find(y => y.NetworkId == unit.NetworkId);
            if (x.PathBank.Count < 3)
                return false;

            var pathBank = x.PathBank;

            if (pathBank.Count < 3)
                return false;

            if (pathBank[0].Position.Count() < 2 || pathBank[1].Position.Count() < 2 || pathBank[2].Position.Count() < 2)
                return false;

            var fixDelay = radius * 2 - unit.MoveSpeed * delay;

            if (Utils.TickCount - pathBank[2].Time > 100)
                return false;

            if (Utils.TickCount - pathBank[0].Time > 900 + fixDelay)
                return false;

            var pos1 = pathBank[0].Position.Last().To2D() - pathBank[0].Position.First().To2D();
            var pos2 = pathBank[1].Position.Last().To2D() - pathBank[1].Position.First().To2D();
            var pos3 = pathBank[2].Position.Last().To2D() - pathBank[2].Position.First().To2D();

            var angle1 = pos1.AngleBetween(pos2);
            var angle2 = pos2.AngleBetween(pos3);
            if (angle1 > 130 && angle2 > 130)
                return true;

            return false;
        }

        public static bool SpamSamePlace(Obj_AI_Base unit)
        {
            //var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            //if (TrackerUnit.PathBank.Count < 3)
            //    return false;
            //if (TrackerUnit.PathBank[2].Time - TrackerUnit.PathBank[1].Time < 180 && Utils.TickCount - TrackerUnit.PathBank[2].Time < 90)
            //{
            //    var C = TrackerUnit.PathBank[1].Position;
            //    var A = TrackerUnit.PathBank[2].Position;
            //    var B = unit.Position.To2D();
            //    var AB = Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2);
            //    var BC = Math.Pow(B.X - C.X, 2) + Math.Pow(B.Y - C.Y, 2);
            //    var AC = Math.Pow(A.X - C.X, 2) + Math.Pow(A.Y - C.Y, 2);

            //    if (TrackerUnit.PathBank[1].Position.Distance(TrackerUnit.PathBank[2].Position) < 50)
            //    {
            //        Console.WriteLine("SPAM PLACE");
            //        return true;
            //    }
            //    else if (Math.Cos((AB + BC - AC) / (2 * Math.Sqrt(AB) * Math.Sqrt(BC))) * 180 / Math.PI < 31)
            //    {
            //        Console.WriteLine("SPAM ANGLE");
            //        return true;
            //    }
            //    else
            //        return false;
            //}
            //else
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
            return TrackerUnit.SpecialSpellFinishTick - Utils.TickCount;
        }

        public static double GetLastAutoAttackTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            return Utils.TickCount - TrackerUnit.AaTick;
        }

        public static double GetLastNewPathTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            return Utils.TickCount - TrackerUnit.NewPathTick;
        }

        public static double GetLastVisableTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            return Utils.TickCount - TrackerUnit.LastInvisableTick;
        }

        public static double GetLastStopMoveTime(Obj_AI_Base unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            return Utils.TickCount - TrackerUnit.StopMoveTick;
        }
    }
}