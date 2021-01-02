
#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;
#endregion

namespace Evade
{
    internal class Program
    {
        public static SpellList<Skillshot> DetectedSkillshots = new SpellList<Skillshot>();
        private static bool _evading;
        private static Vector2 _evadePoint;
        public static bool NoSolutionFound = false;
        public static Vector2 EvadeToPoint = new Vector2();
        public static int LastWardJumpAttempt = 0;
        public static Vector2 PreviousTickPosition = new Vector2();
        public static Vector2 PlayerPosition = new Vector2();
        public static string PlayerChampionName;
        private static readonly Random RandomN = new Random();
        public static int LastSentMovePacketT = 0;
        public static int LastSentMovePacketT2 = 0;

        public static bool Evading
        {
            get { return _evading; }
            set
            {
                if (value == true)
                {
                    LastSentMovePacketT = 0;
                    ObjectManager.Player.SendMovePacket(EvadePoint);
                }

                _evading = value;
            }
        }

        public static Vector2 EvadePoint
        {
            get { return _evadePoint; }
            set
            {
                _evadePoint = value;
            }
        }

        private static Font WarningMsg { get; } = new Font(Drawing.Direct3DDevice,
        new FontDescription
        {
            FaceName = "Courier New",
            Height = 17,
            OutputPrecision = FontPrecision.Default,
            Quality = FontQuality.Default,
            Weight = FontWeight.Normal
        });

        private static int startT;

        private static void Main(string[] args)
        {
            if (Game.Mode == GameMode.Running)
                Game_OnGameStart(new EventArgs());
            else
                Game.OnStart += Game_OnGameStart;
        }

        private static void Game_OnGameStart(EventArgs args)
        {
            startT = Utils.TickCount;
            PlayerChampionName = ObjectManager.Player.ChampionName;

            //Create the menu to allow the user to change the config.
            Config.CreateMenu();

            //Add the game events.
            Game.OnUpdate += Game_OnOnGameUpdate;
            Obj_AI_Hero.OnIssueOrder += ObjAiHeroOnOnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;

            //Set up the OnDetectSkillshot Event.
            SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
            SkillshotDetector.OnDeleteMissile += SkillshotDetectorOnOnDeleteMissile;

            //For skillshot drawing.
            Drawing.OnDraw += Drawing_OnDraw;

            //Ondash event.
            CustomEvents.Unit.OnDash += UnitOnOnDash;

            DetectedSkillshots.OnAdd += DetectedSkillshots_OnAdd;
            Drawing.OnPreReset += dummyArgs => { WarningMsg.OnLostDevice(); };
            Drawing.OnPostReset += dummyArgs => { WarningMsg.OnResetDevice(); };

            //Initialze the collision
            Collision.Init();

            if (Config.PrintSpellData)
            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
                {
                    foreach (var spell in hero.Spellbook.Spells.Where(s => s.SData.Name != "BaseSpell"))
                    {
                        if(Config.Debug) Console.WriteLine("\n\n");
                        if(Config.Debug) Console.WriteLine("SpellSlot: {0} Spell: {1}", spell.Slot, spell.SData.Name);
                        if(Config.Debug) Console.WriteLine("=================================================================");
                        foreach (var prop in spell.SData.GetType().GetProperties())
                        {
                            if (prop.Name != "Entries")
                                if(Config.Debug) Console.WriteLine("\t{0} => '{1}'", prop.Name, prop.GetValue(spell.SData, null));
                        }

                    }
                }
                Console.WriteLine(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name);
            }

            if (Config.TestOnAllies)
                Benchmarking.Benchmark.Initialize();
        }

        private static void DetectedSkillshots_OnAdd(object sender, EventArgs e)
        {
            if(Config.Debug) Console.WriteLine("evading false3 ");
            Evading = false;
        }

        private static void SkillshotDetectorOnOnDeleteMissile(Skillshot skillshot, MissileClient missile)
        {
            if (skillshot.SpellData.SpellName == "VelkozQ")
            {
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                var direction = skillshot.Direction.Perpendicular();
                if (DetectedSkillshots.Count(s => s.SpellData.SpellName == "VelkozQSplit") == 0)
                {
                    for (var i = -1; i <= 1; i = i + 2)
                    {
                        var pos = skillshot.GetMissilePosition(-25);

                        var skillshotToAdd = new Skillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount, pos,
                            pos + i * direction * spellData.Range, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                }
            }
        }

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            //Check if the skillshot is already added.
            var alreadyAdded = false;

            if (Config.Menu.Item("DisableFow").GetValue<bool>() && !skillshot.Unit.IsVisible)
                return;

            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    (item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                     (skillshot.Direction).AngleBetween(item.Direction) < 5 &&
                     (skillshot.Start.Distance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0)))
                {
                    alreadyAdded = true;
                    break;
                }
            }

            //Check if the skillshot is from an ally.
            if (skillshot.Unit.Team == ObjectManager.Player.Team && !Config.TestOnAllies)
                return;

            //Check if the skillshot is too far away.
            if (skillshot.Start.Distance(PlayerPosition) > (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
                return;

            //Add the skillshot to the detected skillshot list.
            if (!alreadyAdded || skillshot.SpellData.DontCheckForDuplicates)
            {
                //Multiple skillshots like twisted fate Q.
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;

                        for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                            i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                            i++)
                        {
                            var end = skillshot.Start +
                                      skillshot.SpellData.Range *
                                      originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "BardR" && skillshot.End.Distance(skillshot.Start) < 850)
                    {
                        skillshot.StartTick = Utils.TickCount - skillshot.SpellData.Delay + 800;
                    }

                    if (skillshot.SpellData.SpellName == "MordekaiserE")
                    {
                        var end = skillshot.OriginalEnd;
                        if (skillshot.Start.Distance(skillshot.OriginalEnd) > 700)
                        {
                            end = skillshot.Start + skillshot.Direction * 700;
                        }

                        skillshot.End = end + skillshot.Direction * 275;
                        skillshot.Start = skillshot.End;
                        skillshot.End = skillshot.End - skillshot.Direction * 900;
                        skillshot.SpellData.Delay = 200 + 250 + (int)((skillshot.Start.Distance(skillshot.End) / skillshot.SpellData.MissileSpeed) * 1000);

                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, skillshot.End,
                            skillshot.Unit);

                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "SennaQCast" && skillshot.Unit != null)
                    {
                        skillshot.SpellData.Delay = (int)(skillshot.Unit.AttackCastDelay * 1000);
                    }

                    if (skillshot.SpellData.SpellName == "UFSlash")
                    {
                        skillshot.SpellData.MissileSpeed = 1600 + (int)skillshot.Unit.MoveSpeed;
                    }

                    if (skillshot.SpellData.SpellName == "SionR")
                    {
                        skillshot.SpellData.MissileSpeed = (int)skillshot.Unit.MoveSpeed;
                    }

                    if (skillshot.SpellData.Invert)
                    {
                        var newDirection = -(skillshot.End - skillshot.Start).Normalized();
                        var end = skillshot.Start + newDirection * skillshot.Start.Distance(skillshot.End);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.Centered)
                    {
                        var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range;
                        var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "TaricE" && (skillshot.Unit as Obj_AI_Hero).ChampionName == "Taric")
                    {
                        var target = HeroManager.AllHeroes.FirstOrDefault(h => h.Team == skillshot.Unit.Team && h.IsVisible && h.HasBuff("taricwleashactive"));
                        if (target != null)
                        {
                            var start = target.ServerPosition.To2D();
                            var direction = (skillshot.OriginalEnd - start).Normalized();
                            var end = start + direction * skillshot.SpellData.Range;
                            var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick,
                                    start, end, target)
                            {
                                OriginalEnd = skillshot.OriginalEnd
                            };
                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                    }

                    if (skillshot.SpellData.SpellName == "SylasQ")
                    {
                        var sylasQLine = SpellDatabase.GetByName("SylasQLine");

                        if (sylasQLine != null)
                        {
                            var dir = skillshot.Direction.Perpendicular();
                            var leftStart = skillshot.Start + dir * 125;
                            var leftEnd = leftStart.Extend(skillshot.End, sylasQLine.Range);

                            var rightStart = skillshot.Start - dir * 125;
                            var rightEnd = rightStart.Extend(skillshot.End, sylasQLine.Range);

                            DetectedSkillshots.Add(new Skillshot(skillshot.DetectionType, sylasQLine, skillshot.StartTick, leftStart, leftEnd, skillshot.Unit));
                            DetectedSkillshots.Add(new Skillshot(skillshot.DetectionType, sylasQLine, skillshot.StartTick, rightStart, rightEnd, skillshot.Unit));
                        }
                    }

                    if (skillshot.SpellData.SpellName == "PykeR")
                    {
                        var start2 = skillshot.End + new Vector2(250, -250);
                        var end2 = skillshot.End + new Vector2(-250, 250);

                        skillshot.Start = skillshot.End - new Vector2(250, 250);
                        skillshot.End = skillshot.End + new Vector2(250, 250);

                        DetectedSkillshots.Add(new Skillshot(skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start2, end2, skillshot.Unit));
                        DetectedSkillshots.Add(new Skillshot(skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, skillshot.End, skillshot.Unit));
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                    {
                        var angle = 60;
                        var edge1 =
                            (skillshot.End - skillshot.Unit.ServerPosition.To2D()).Rotated(
                                -angle / 2 * (float)Math.PI / 180);
                        var edge2 = edge1.Rotated(angle * (float)Math.PI / 180);

                        var positions = new List<Vector2>();

                        var explodingQ = DetectedSkillshots.FirstOrDefault(s => s.SpellData.SpellName == "SyndraQ");

                        if (explodingQ != null)
                        {
                            var position = explodingQ.End;
                            var v = position - skillshot.Unit.ServerPosition.To2D();
                            if (edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                                position.Distance(skillshot.Unit) < 800)
                            {
                                var start = position;
                                var end = skillshot.Unit.ServerPosition.To2D()
                                    .Extend(position, skillshot.Unit.Distance(position) > 200 ? 1300 : 1000);

                                var startTime = skillshot.StartTick;

                                startTime += (int)(150 + Math.Min(250, explodingQ.StartTick + explodingQ.SpellData.Delay - 150 - Utils.TickCount) + skillshot.Unit.Distance(position) / 2.5f);
                                var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, startTime, start, end,
                                    skillshot.Unit);
                                DetectedSkillshots.Add(skillshotToAdd);
                            }
                        }

                        foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            if (minion.Name == "Seed" && !minion.IsDead && (minion.Team != ObjectManager.Player.Team || Config.TestOnAllies))
                            {
                                positions.Add(minion.ServerPosition.To2D());
                            }
                        }

                        foreach (var position in positions)
                        {
                            var v = position - skillshot.Unit.ServerPosition.To2D();
                            if (edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                                position.Distance(skillshot.Unit) < 800)
                            {
                                var start = position;
                                var end = skillshot.Unit.ServerPosition.To2D()
                                    .Extend(position, skillshot.Unit.Distance(position) > 200 ? 1300 : 1000);

                                var startTime = skillshot.StartTick;

                                startTime += (int)(150 + skillshot.Unit.Distance(position) / 2.5f);
                                var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, startTime, start, end,
                                    skillshot.Unit);
                                DetectedSkillshots.Add(skillshotToAdd);
                            }
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "ZoeE")
                    {
                        Vector2 wall_start = Vector2.Zero;
                        int range_left = 0;
                        int range_max = skillshot.SpellData.RawRange + skillshot.SpellData.ExtraRange;

                        for (int i = 0; i < range_max; i += 10)
                        {
                            var curr_pos = skillshot.Start + skillshot.Direction * i;

                            if (curr_pos.IsWall())
                            {
                                wall_start = curr_pos;
                                range_left = range_max - i;
                                break;
                            }
                        }

                        int max = 70;
                        while (wall_start.IsWall() && max > 0)
                        {
                            wall_start = wall_start + skillshot.Direction * 35;
                            max--;
                        }

                        for (int i = 0; i < range_left; i += 10)
                        {
                            var curr_pos = wall_start + skillshot.Direction * i;

                            if (curr_pos.IsWall())
                            {
                                range_left = i;
                                break;
                            }
                        }



                        if (range_left > 0)
                        {
                            skillshot.End = wall_start + skillshot.Direction * range_left;

                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, skillshot.End,
                                skillshot.Unit);
                            DetectedSkillshots.Add(skillshotToAdd);
                            return;
                        }
                    }

                    if (skillshot.SpellData.SpellName == "MalzaharQ")
                    {
                        var start = skillshot.End - skillshot.Direction.Perpendicular() * 400;
                        var end = skillshot.End + skillshot.Direction.Perpendicular() * 400;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "ZyraQ")
                    {
                        var start = skillshot.End - skillshot.Direction.Perpendicular() * 450;
                        var end = skillshot.End + skillshot.Direction.Perpendicular() * 450;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "DianaQ")
                    {
                        var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("DianaArcArc"), skillshot.StartTick, skillshot.Start, skillshot.End,
                        skillshot.Unit);

                        DetectedSkillshots.Add(skillshotToAdd);
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.Distance(skillshot.End);
                        var d2 = d1 * 0.4f;
                        var d3 = d2 * 0.69f;


                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");

                        var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction * d3;

                        bounce1SpellData.Delay =
                            (int)(skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                        bounce2SpellData.Delay =
                            (int)(bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);

                        var bounce1 = new Skillshot(
                            skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.End, bounce1Pos,
                            skillshot.Unit);
                        var bounce2 = new Skillshot(
                            skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                            skillshot.Unit);

                        DetectedSkillshots.Add(bounce1);
                        DetectedSkillshots.Add(bounce2);
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsR")
                    {
                        skillshot.SpellData.Delay =
                            (int)(1500 + 1500 * skillshot.End.Distance(skillshot.Start) / skillshot.SpellData.Range);
                    }

                    if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                    {
                        var endPos = new Vector2();

                        foreach (var s in DetectedSkillshots)
                        {
                            if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                            {
                                var extendedE = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start,
                                    skillshot.End + skillshot.Direction * 100, skillshot.Unit);
                                if (!extendedE.IsSafe(s.End))
                                {
                                    endPos = s.End;
                                }
                                break;
                            }
                        }

                        foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            if (m.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team)
                            {

                                var extendedE = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start,
                                    skillshot.End + skillshot.Direction * 100, skillshot.Unit);
                                if (!extendedE.IsSafe(m.Position.To2D()))
                                {
                                    endPos = m.Position.To2D();
                                }
                                break;
                            }
                        }

                        if (endPos.IsValid())
                        {
                            skillshot = new Skillshot(DetectionType.ProcessSpell, SpellDatabase.GetByName("JarvanIVEQ"), Utils.TickCount, skillshot.Start, endPos, skillshot.Unit);
                            skillshot.End = endPos + 200 * (endPos - skillshot.Start).Normalized();
                            skillshot.Direction = (skillshot.End - skillshot.Start).Normalized();
                        }
                    }
                }

                if (skillshot.SpellData.SpellName == "OriannasQ")
                {
                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, SpellDatabase.GetByName("OriannaQend"), skillshot.StartTick, skillshot.Start, skillshot.End,
                        skillshot.Unit);

                    DetectedSkillshots.Add(skillshotToAdd);
                }

                if (skillshot.SpellData.SpellName == "XerathArcanopulse2")
                {
                    if (SkillshotDetector.GetHiuLine(skillshot.End, ref skillshot.Start, ref skillshot.End))
                    {
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, skillshot.End,
                            skillshot.Unit);

                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                    else
                    {
                        if(Config.Debug) Console.WriteLine("NOT FIND LINE");
                    }

                    return;
                }


                if (skillshot.SpellData.SpellName == "IreliaE2")
                {
                    var reg = new System.Text.RegularExpressions.Regex("Irelia_.+_E_.+_Indicator");
                    var firstE = ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(x => x.IsValid && reg.IsMatch(x.Name)).
                        OrderByDescending(x => x.Position.Distance(skillshot.Start.To3D())).FirstOrDefault();

                    if (firstE == null)
                    {
                        var firstEMissile = ObjectManager.Get<MissileClient>().Where(x =>
                            x.IsValid && x.EndPosition.To2D().Distance(skillshot.End) > 5 && x.SData.Name == skillshot.SpellData.MissileSpellName).
                            OrderByDescending(x => x.Position.Distance(skillshot.Start.To3D())).FirstOrDefault();

                        if (firstEMissile != null)
                        {
                            if(Config.Debug) Console.WriteLine("Adding from missile");

                            var skillshotToAdd = new Skillshot(skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, firstEMissile.EndPosition.To2D(), skillshot.End,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }

                    if (firstE != null)
                    {
                        if(Config.Debug) Console.WriteLine("Adding from particle");

                        var skillshotToAdd = new Skillshot(skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, firstE.Position.To2D(), skillshot.End,
                            skillshot.Unit);

                        DetectedSkillshots.Add(skillshotToAdd);
                    }

                    return;
                }


                //Dont allow fow detection.
                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }
#if DEBUG
                Console.WriteLine(Utils.TickCount + "Adding new skillshot: " + skillshot.SpellData.SpellName);
#endif

                DetectedSkillshots.Add(skillshot);
            }
        }

        private static void Game_OnOnGameUpdate(EventArgs args)
        {
            PlayerPosition = ObjectManager.Player.ServerPosition.To2D();

            //Set evading to false after blinking
            if (PreviousTickPosition.IsValid() && PlayerPosition.Distance(PreviousTickPosition) > 200)
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
            }

            PreviousTickPosition = PlayerPosition;

            //Remove the detected skillshots that have expired.
            DetectedSkillshots.RemoveAll(skillshot => !skillshot.IsActive());

            //Trigger OnGameUpdate on each skillshot.
            foreach (var skillshot in DetectedSkillshots)
            {
                skillshot.Game_OnGameUpdate();
            }

            //Evading disabled
            if (!Config.Menu.Item("Enabled").GetValue<KeyBind>().Active)
            {
                Evading = false;
                return;
            }

            if (PlayerChampionName == "Olaf" && Config.Menu.Item("DisableEvadeForOlafR").GetValue<bool>() && ObjectManager.Player.HasBuff("OlafRagnarok"))
            {
                Evading = false;
                return;
            }

            //Avoid sending move/cast packets while dead.
            if (ObjectManager.Player.IsDead)
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
                return;
            }

            //Avoid sending move/cast packets while channeling interruptable spells that cause hero not being able to move.
            if (ObjectManager.Player.IsCastingInterruptableSpell(true))
            {
                Evading = false;
                EvadeToPoint = Vector2.Zero;
                return;
            }


            //if (ObjectManager.Player.IsWindingUp && !Orbwalking.IsAutoAttack(ObjectManager.Player.LastCastedSpellName()))
            //{
            //    Evading = false;
            //    return;
            //}

            /*Avoid evading while stunned or immobile.*/
            if (Utils.ImmobileTime(ObjectManager.Player) - Utils.TickCount > Game.Ping / 2 + 70)
            {
                Evading = false;
                return;
            }

            /*Avoid evading while dashing.*/
            if (ObjectManager.Player.IsDashing())
            {
                Evading = false;
                return;
            }

            //Don't evade while casting R as sion
            if (PlayerChampionName == "Sion" && ObjectManager.Player.HasBuff("SionR"))
                return;

            //Shield allies.
            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (ally.IsValidTarget(1000, false))
                {
                    var shieldAlly = Config.Menu.Item("shield" + ally.ChampionName);
                    if (shieldAlly != null && shieldAlly.GetValue<bool>())
                    {
                        var allySafeResult = IsSafe(ally.ServerPosition.To2D());

                        if (!allySafeResult.IsSafe)
                        {
                            var dangerLevel = 0;

                            foreach (var skillshot in allySafeResult.SkillshotList)
                            {
                                dangerLevel = Math.Max(dangerLevel, skillshot.GetValue<Slider>("DangerLevel").Value);
                            }

                            foreach (var evadeSpell in EvadeSpellDatabase.Spells)
                            {
                                if (evadeSpell.IsShield && evadeSpell.CanShieldAllies &&
                                    ally.Distance(ObjectManager.Player.ServerPosition) < evadeSpell.MaxRange &&
                                    dangerLevel >= evadeSpell.DangerLevel &&
                                    ObjectManager.Player.Spellbook.CanUseSpell(evadeSpell.Slot) == SpellState.Ready &&
                                    IsAboutToHit(ally, evadeSpell.Delay))
                                {
                                    ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ally);
                                }
                            }
                        }
                    }
                }
            }

            //Spell Shielded
            if (ObjectManager.Player.IsSpellShielded())
                return;

            var currentPath = ObjectManager.Player.GetWaypoints();
            var safeResult = IsSafe(PlayerPosition);
            var safePath = IsSafePath(currentPath, 100);

            NoSolutionFound = false;

            //Continue evading
            if (Evading && IsSafe(EvadePoint).IsSafe)
            {
                if (safeResult.IsSafe)
                {
                    if(Config.Debug) Console.WriteLine("evading false1 ");
                    //We are safe, stop evading.
                    Evading = false;
                }
                else
                {
                    if (!Orbwalking.CanMove(50) && CanAttackInSkillshot())
                    {
                        return;
                    }

                    if (Utils.TickCount - LastSentMovePacketT > 1000 / 3)
                    {
                        LastSentMovePacketT = Utils.TickCount;
                        ObjectManager.Player.SendMovePacket(EvadePoint);
                    }
                    return;
                }
            }
            //Stop evading if the point is not safe.
            else if (Evading)
            {
                if(Config.Debug) Console.WriteLine("evading false2 ");
                Evading = false;
            }

           
            //The path is not safe.
            if (!safePath.IsSafe)
            {
                //Inside the danger polygon.
                if (!safeResult.IsSafe)
                {
                    

                    //Search for an evade point:
                    TryToEvade(safeResult.SkillshotList, EvadeToPoint.IsValid() ? EvadeToPoint : Game.CursorPos.To2D());
                }
            }
        }

        static bool CanAttackInSkillshot()
        {
            var canAttack = true;
            foreach (var skillshot in DetectedSkillshots)
            {
                var dangerValue = skillshot.SpellData.DangerValue;
                var dangerSkillMenu = Config.Menu.Item("DangerLevel" + skillshot.SpellData.MenuItemName);

                if (dangerSkillMenu != null)
                    dangerValue = dangerSkillMenu.GetValue<Slider>().Value;

                if (skillshot.Evade() && skillshot.IsDanger(PlayerPosition) && dangerValue > Config.Menu.Item("AllowAaLevel").GetValue<Slider>().Value)
                    canAttack = false;
            }
            return canAttack;
        }

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsValid && sender.Owner.IsMe)
            {
                //if (args.Slot == SpellSlot.Recall)
                //    EvadeToPoint = new Vector2();
                var pointToCheck = HeroManager.Player.Position - HeroManager.Player.Direction * 100;

                if (Evading || !IsSafe(PlayerPosition).IsSafe || !IsSafe(pointToCheck.To2D()).IsSafe)
                {
                    var blockLevel = Config.Menu.Item("BlockSpells").GetValue<StringList>().SelectedIndex;

                    if (blockLevel == 0)
                        return;

                    var isDangerous = false;
                    foreach (var skillshot in DetectedSkillshots)
                    {
                        if (skillshot.Evade() && skillshot.IsDanger(PlayerPosition) )
                        {
                            isDangerous = skillshot.GetValue<bool>("IsDangerous");  
                        }
                    }

                    if (blockLevel == 1 && !isDangerous)
                        return;

                    args.Process = !SpellBlocker.ShouldBlock(args.Slot);
                }
            }
        }

        /// Used to block the movement to avoid entering in dangerous areas.
        private static void ObjAiHeroOnOnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (args.Order == GameObjectOrder.MoveTo || args.Order == GameObjectOrder.AttackTo)
            {
                EvadeToPoint.X = args.TargetPosition.X;
                EvadeToPoint.Y = args.TargetPosition.Y;
            }
            else
            {
                EvadeToPoint = Vector2.Zero;
            }

            //Don't block the movement packets if cant find an evade point.
            if (NoSolutionFound)
            {
                if(Config.Debug) Console.WriteLine("NoSolutionFound");
                return;
            }

            //Evading disabled
            if (!Config.Menu.Item("Enabled").GetValue<KeyBind>().Active)
                return;

            if (EvadeSpellDatabase.Spells.Any(evadeSpell => evadeSpell.Name == "Walking" && !evadeSpell.Enabled))
                return;

            //Spell Shielded
            if (ObjectManager.Player.IsSpellShielded())
                return;

            if (PlayerChampionName == "Olaf" && Config.Menu.Item("DisableEvadeForOlafR").GetValue<bool>() && ObjectManager.Player.HasBuff("OlafRagnarok"))
                return;

            var myPath = Orbwalking.GetPath(new Vector3(args.TargetPosition.X, args.TargetPosition.Y, ObjectManager.Player.ServerPosition.Z)).To2DList();
            var safeResult = IsSafe(PlayerPosition);

            if (args.Order == GameObjectOrder.AttackUnit)
            {
                var target = args.Target;
                if (target != null && target.IsValid<Obj_AI_Base>() && target.IsVisible)
                {
                    if(CanAttackInSkillshot())
                        return;
                }
            }
            //If we are evading:
            if (Evading || !safeResult.IsSafe)
            {
                var rcSafePath = IsSafePath(myPath, Config.EvadingRouteChangeTimeOffset);
                if (args.Order == GameObjectOrder.MoveTo)
                {
                    if(Config.Debug) Console.WriteLine("Evading issue order " + Evading);

                    if (Evading && Utils.TickCount - Config.LastEvadePointChangeT > Config.EvadePointChangeInterval)
                    {
                        if(Config.Debug) Console.WriteLine("update point first ");
                        //Update the evade point to the closest one:
                        var points = Evader.GetEvadePoints(-1, 0, false, true);
                        if (points.Count > 0)
                        {
                            if(Config.Debug) Console.WriteLine("update point");
                            var to = new Vector2(args.TargetPosition.X, args.TargetPosition.Y);
                            EvadePoint = to.Closest(points);
                            Evading = true;
                            Config.LastEvadePointChangeT = Utils.TickCount;
                        }
                    }

                    //If the path is safe let the user follow it.
                    if (rcSafePath.IsSafe && IsSafe(myPath[myPath.Count - 1]).IsSafe && args.Order == GameObjectOrder.MoveTo)
                    {
                        if(Config.Debug) Console.WriteLine("update path");
                        EvadePoint = myPath[myPath.Count - 1];
                        Evading = true;
                    }
                }

                //Block the packets if we are evading or not safe.
                args.Process = false;
                return;
            }

            var safePath = IsSafePath(myPath, Config.CrossingTimeOffset);

            //Not evading, outside the skillshots.
            if (!safePath.IsSafe && args.Order != GameObjectOrder.AttackUnit)
            {
                if(Config.Debug) Console.WriteLine("block move");
                Vector2 pathfinderPoint = GetPathFinderPoint();
                if (pathfinderPoint.IsValid())
                {
                    if(Config.Debug) Console.WriteLine("FOUND POINT");
                    if (Utils.TickCount - LastSentMovePacketT > 1000 / 3)
                    {
                        LastSentMovePacketT = Utils.TickCount;
                        ObjectManager.Player.SendMovePacket(pathfinderPoint, true);
                    }
                }
                args.Process = false;
                return;
            }

            //AutoAttacks.
            if (!safePath.IsSafe && args.Order == GameObjectOrder.AttackUnit)
            {
                var target = args.Target;
                if (target != null && target.IsValid<Obj_AI_Base>() && target.IsVisible)
                {
                    //Out of attack range.
                    if (PlayerPosition.Distance(((Obj_AI_Base)target).ServerPosition) >
                        ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius +
                        target.BoundingRadius)
                    {
                        if (safePath.Intersection.Valid)
                        {
                            ObjectManager.Player.SendMovePacket(safePath.Intersection.Point);
                        }
                        args.Process = false;
                        return;
                    }
                }
            }
            if(Config.Debug) Console.WriteLine("move accept");
        }

        public static Vector2 GetPathFinderPoint()
        {
            var gameCursorVec2 = Game.CursorPos.To2D();
            var Points = Utils.CirclePoints(36, 300, ObjectManager.Player.Position.To2D());
            Points = Points.OrderBy(x => x.Distance(gameCursorVec2, true)).ToArray();

            foreach (var vector2 in Points)
            {
                var truePosition = Utils.CutVector(PlayerPosition,vector2);

                if (!IsSafe(truePosition).IsSafe)
                    continue;

                if (ObjectManager.Player.Distance(truePosition, true) < 100 * 100)
                    continue;

                var safeResult = IsSafePath(Orbwalking.GetPath(truePosition.To3D()).To2DList(), Config.CrossingTimeOffset);
                if (!safeResult.IsSafe || safeResult.Intersection.Valid)
                    continue;
                
                

                if (ObjectManager.Player.Direction.To2D().AngleBetween(truePosition - PlayerPosition) < 120)
                    return truePosition;
            }

            return Vector2.Zero;
        }

        private static void UnitOnOnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsMe)
            {
                if (Config.PrintSpellData)
                {
                    Console.WriteLine(Utils.TickCount + "DASH: Speed: " + args.Speed + " Width:" + args.EndPos.Distance(args.StartPos));
                }

                EvadeToPoint = args.EndPos;
                //Utility.DelayAction.Add(args.Duration, delegate { Evading = false; });
            }
        }

        /// Returns true if the point is not inside the detected skillshots.
        public static IsSafeResult IsSafe(Vector2 point)
        {
            var result = new IsSafeResult();
            result.SkillshotList = new List<Skillshot>();

            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade() && skillshot.IsDanger(point) )
                {
                    result.SkillshotList.Add(skillshot);
                }
            }

            result.IsSafe = (result.SkillshotList.Count == 0);
            return result;
        }

        /// Returns if the unit will get hit by skillshots taking the path.
        public static SafePathResult IsSafePath(GamePath path, int timeOffset, int speed = -1, int delay = 0, Obj_AI_Base unit = null)
        {
            var IsSafe = true;
            var intersections = new List<FoundIntersection>();
            var intersection = new FoundIntersection();

            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade())
                {
                    var sResult = skillshot.IsSafePath(path, timeOffset, speed, delay, unit);
                    IsSafe = (IsSafe) ? sResult.IsSafe : false;

                    if (sResult.Intersection.Valid)
                        intersections.Add(sResult.Intersection);
                }
            }

            //Return the first intersection
            if (!IsSafe)
            {
                var intersetion = intersections.MinOrDefault(o => o.Distance);
                return new SafePathResult(false, intersetion.Valid ? intersetion : intersection);
            }

            return new SafePathResult(true, intersection);
        }

        /// Returns if you can blink to the point without being hit.
        public static bool IsSafeToBlink(Vector2 point, int timeOffset, int delay)
        {
            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade() && !skillshot.IsSafeToBlink(point, timeOffset, delay))
                    return false;
            }
            return true;
        }

        /// Returns true if some detected skillshot is about to hit the unit.
        public static bool IsAboutToHit(Obj_AI_Base unit, int time)
        {
            time += 150;
            foreach (var skillshot in DetectedSkillshots)
            {
                if (skillshot.Evade() && skillshot.IsAboutToHit(time, unit))
                    return true;
            }
            return false;
        }

        private static void TryToEvade(List<Skillshot> HitBy, Vector2 to)
        {
            var dangerLevel = 0;

            foreach (var skillshot in HitBy)
            {
                dangerLevel = Math.Max(dangerLevel, skillshot.GetValue<Slider>("DangerLevel").Value);
            }

            foreach (var evadeSpell in EvadeSpellDatabase.Spells)
            {
                if (evadeSpell.Enabled && evadeSpell.DangerLevel <= dangerLevel)
                {
                    //SpellShields
                    if (evadeSpell.IsSpellShield && ObjectManager.Player.Spellbook.CanUseSpell(evadeSpell.Slot) == SpellState.Ready)
                    {
                        if (evadeSpell.Name == "Samira W")
                            if (!HitBy.Exists(x => x.SpellData.MissileSpeed > 0 && x.SpellData.MissileSpeed != int.MaxValue))
                                continue;

                        if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);

                        //Let the user move freely inside the skillshot.
                        NoSolutionFound = true;
                        return;
                    }

                    //Walking
                    if (evadeSpell.Name == "Walking")
                    {
                        var points = Evader.GetEvadePoints();
                        if (points.Count > 0)
                        {
                            EvadePoint = to.Closest(points);
                            var nEvadePoint = EvadePoint.Extend(PlayerPosition, -100);
                            if (Program.IsSafePath(Orbwalking.GetPath(nEvadePoint.To3D()).To2DList(), Config.EvadingSecondTimeOffset, (int)ObjectManager.Player.MoveSpeed, 100).IsSafe)
                            {
                                EvadePoint = nEvadePoint;
                            }

                            Evading = true;
                            return;
                        }
                    }

                    if (evadeSpell.IsReady())
                    {
                        //MovementSpeed Buff
                        if (evadeSpell.IsMovementSpeedBuff)
                        {
                            var points = Evader.GetEvadePoints((int)evadeSpell.MoveSpeedTotalAmount());

                            if (points.Count > 0)
                            {
                                EvadePoint = to.Closest(points);
                                Evading = true;

                                if (evadeSpell.IsSummonerSpell)
                                    ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);
                                else
                                    ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);

                                return;
                            }
                        }

                        //Dashes
                        if (evadeSpell.IsDash)
                        {
                            //Targetted dashes
                            if (evadeSpell.IsTargetted) //Lesinga W.
                            {
                                var targets = Evader.GetEvadeTargets(
                                    evadeSpell.ValidTargets, evadeSpell.Speed, evadeSpell.Delay, evadeSpell.MaxRange,
                                    false, false);

                                if (targets.Count > 0)
                                {
                                    var closestTarget = Utils.Closest(targets, to);
                                    EvadePoint = closestTarget.ServerPosition.To2D();
                                    Evading = true;

                                    if (evadeSpell.IsSummonerSpell)
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                    else
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);

                                    return;
                                }
                                if (Utils.TickCount - LastWardJumpAttempt < 250)
                                {
                                    //Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }

                                if (evadeSpell.IsTargetted && evadeSpell.ValidTargets.Contains(SpellValidTargets.AllyWards) && Config.Menu.Item("WardJump" + evadeSpell.Name).GetValue<bool>())
                                {
                                    var wardSlot = Items.GetWardSlot();
                                    if (wardSlot != null)
                                    {
                                        var points = Evader.GetEvadePoints(evadeSpell.Speed, evadeSpell.Delay, false);

                                        // Remove the points out of range
                                        points.RemoveAll(item => item.Distance(ObjectManager.Player.ServerPosition) > 600);

                                        if (points.Count > 0)
                                        {
                                            //Dont dash just to the edge:
                                            for (var i = 0; i < points.Count; i++)
                                            {
                                                var k = (int)(600 - PlayerPosition.Distance(points[i]));
                                                k = k - new Random(Utils.TickCount).Next(k);
                                                var extended = points[i] + k * (points[i] - PlayerPosition).Normalized();

                                                if (IsSafe(extended).IsSafe)
                                                    points[i] = extended;
                                            }

                                            var ePoint = to.Closest(points);
                                            ObjectManager.Player.Spellbook.CastSpell(wardSlot.SpellSlot, ePoint.To3D());
                                            LastWardJumpAttempt = Utils.TickCount;
                                            //Let the user move freely inside the skillshot.
                                            NoSolutionFound = true;
                                            return;
                                        }
                                    }
                                }
                            }
                            //Skillshot type dashes.
                            else
                            {
                                var points = Evader.GetEvadePoints(evadeSpell.Speed, evadeSpell.Delay, false);

                                // Remove the points out of range
                                points.RemoveAll(item => item.Distance(ObjectManager.Player.ServerPosition) > evadeSpell.MaxRange);

                                //If the spell has a fixed range (Vaynes Q), calculate the real dashing location. TODO: take into account walls in the future.
                                if (evadeSpell.FixedRange)
                                {
                                    for (var i = 0; i < points.Count; i++)
                                        points[i] = PlayerPosition.Extend(points[i], evadeSpell.MaxRange);

                                    for (var i = points.Count - 1; i > 0; i--)
                                        if (!IsSafe(points[i]).IsSafe)
                                            points.RemoveAt(i);
                                }
                                else
                                {
                                    for (var i = 0; i < points.Count; i++)
                                    {
                                        var k = (int)(evadeSpell.MaxRange - PlayerPosition.Distance(points[i]));
                                        k -= Math.Max(RandomN.Next(k) - 100, 0);
                                        var extended = points[i] + k * (points[i] - PlayerPosition).Normalized();

                                        if (IsSafe(extended).IsSafe)
                                            points[i] = extended;
                                    }
                                }

                                if (points.Count > 0)
                                {
                                    EvadePoint = to.Closest(points);
                                    Evading = true;

                                    if (!evadeSpell.Invert)
                                    {
                                        if (evadeSpell.RequiresPreMove)
                                        {
                                            ObjectManager.Player.SendMovePacket(EvadePoint);
                                            var theSpell = evadeSpell;
                                            Utility.DelayAction.Add(
                                                Game.Ping / 2 + 100,
                                                delegate
                                                {
                                                    ObjectManager.Player.Spellbook.CastSpell(
                                                        theSpell.Slot, EvadePoint.To3D());
                                                });
                                        }
                                        else
                                        {
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, EvadePoint.To3D());
                                        }
                                    }
                                    else
                                    {
                                        var castPoint = PlayerPosition - (EvadePoint - PlayerPosition);
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, castPoint.To3D());
                                    }

                                    return;
                                }
                            }
                        }

                        //Blinks
                        if (evadeSpell.IsBlink)
                        {
                            //Targetted blinks
                            if (evadeSpell.IsTargetted)
                            {
                                var targets = Evader.GetEvadeTargets(evadeSpell.ValidTargets, int.MaxValue, evadeSpell.Delay, evadeSpell.MaxRange, true, false);

                                if (targets.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        var closestTarget = Utils.Closest(targets, to);
                                        EvadePoint = closestTarget.ServerPosition.To2D();
                                        Evading = true;

                                        if (evadeSpell.IsSummonerSpell)
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                        else
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                    }

                                    //Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }
                                if (Utils.TickCount - LastWardJumpAttempt < 250)
                                {
                                    //Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }

                                if (evadeSpell.IsTargetted && evadeSpell.ValidTargets.Contains(SpellValidTargets.AllyWards) && Config.Menu.Item("WardJump" + evadeSpell.Name).GetValue<bool>())
                                {
                                    var wardSlot = Items.GetWardSlot();
                                    if (wardSlot != null)
                                    {
                                        var points = Evader.GetEvadePoints(int.MaxValue, evadeSpell.Delay, true);

                                        // Remove the points out of range
                                        points.RemoveAll(item => item.Distance(ObjectManager.Player.ServerPosition) > 600);

                                        if (points.Count > 0)
                                        {
                                            //Dont blink just to the edge:
                                            for (var i = 0; i < points.Count; i++)
                                            {
                                                var k = (int)(600 - PlayerPosition.Distance(points[i]));
                                                k = k - new Random(Utils.TickCount).Next(k);
                                                var extended = points[i] + k * (points[i] - PlayerPosition).Normalized();
                                                if (IsSafe(extended).IsSafe)
                                                    points[i] = extended;
                                            }

                                            var ePoint = to.Closest(points);
                                            ObjectManager.Player.Spellbook.CastSpell(wardSlot.SpellSlot, ePoint.To3D());
                                            LastWardJumpAttempt = Utils.TickCount;
                                            //Let the user move freely inside the skillshot.
                                            NoSolutionFound = true;
                                            return;
                                        }
                                    }
                                }
                            }
                            //Skillshot type blinks.
                            else
                            {
                                var points = Evader.GetEvadePoints(int.MaxValue, evadeSpell.Delay, true);

                                // Remove the points out of range
                                points.RemoveAll(item => item.Distance(ObjectManager.Player.ServerPosition) > evadeSpell.MaxRange);

                                //Dont blink just to the edge:
                                for (var i = 0; i < points.Count; i++)
                                {
                                    var k = (int)(evadeSpell.MaxRange - PlayerPosition.Distance(points[i]));
                                    k = k - new Random(Utils.TickCount).Next(k);
                                    var extended = points[i] + k * (points[i] - PlayerPosition).Normalized();
                                    if (IsSafe(extended).IsSafe)
                                        points[i] = extended;
                                }

                                if (points.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        EvadePoint = to.Closest(points);
                                        Evading = true;
                                        if (evadeSpell.IsSummonerSpell)
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, EvadePoint.To3D());
                                        else
                                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, EvadePoint.To3D());
                                    }

                                    //Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }
                            }
                        }

                        //Invulnerabilities, like Fizz's E
                        if (evadeSpell.IsInvulnerability)
                        {
                            if (evadeSpell.IsTargetted)
                            {
                                var targets = Evader.GetEvadeTargets(evadeSpell.ValidTargets, int.MaxValue, 0, evadeSpell.MaxRange, true, false, true);

                                if (targets.Count > 0)
                                {
                                    if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                    {
                                        var closestTarget = Utils.Closest(targets, to);
                                        EvadePoint = closestTarget.ServerPosition.To2D();
                                        Evading = true;
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                                    }

                                    //Let the user move freely inside the skillshot.
                                    NoSolutionFound = true;
                                    return;
                                }
                            }
                            else
                            {
                                if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                                {
                                    if (evadeSpell.SelfCast)
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot);
                                    else
                                        ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player.ServerPosition);
                                }
                            }

                            //Let the user move freely inside the skillshot.
                            NoSolutionFound = true;
                            return;
                        }
                    }

                    //Zhonyas
                    if (evadeSpell.Name == "Zhonyas" && (Items.CanUseItem("ZhonyasHourglass")))
                    {
                        if (IsAboutToHit(ObjectManager.Player, 100))
                        {
                            //Let the user move freely inside the skillshot.
                            NoSolutionFound = true;
                            Items.UseItem("ZhonyasHourglass");
                            return;
                        }
                    }

                    //Shields
                    if (evadeSpell.IsShield && ObjectManager.Player.Spellbook.CanUseSpell(evadeSpell.Slot) == SpellState.Ready)
                    {
                        if (IsAboutToHit(ObjectManager.Player, evadeSpell.Delay))
                        {
                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, ObjectManager.Player);
                            NoSolutionFound = true;
                            return;
                        }

                        //Let the user move freely inside the skillshot.
                       
                    }
                }
            }

            NoSolutionFound = true;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Utils.TickCount < startT + 20000 && Config.Menu.Item("DrawWarningMsg").GetValue<bool>())
            {
                WarningMsg.DrawText(
                    null,
                    "To use \"Evade#\" you need to unbind Right Mouse Button in Game Settings\n要使用“Evade#”，您需要在游戏设置中取消绑定鼠标右键",
                    75,
                    100,
                    new ColorBGRA(33, 227, 252, 180));
            }

            if (!Config.Menu.Item("EnableDrawings").GetValue<bool>())
                return;

            if (Config.Menu.Item("ShowEvadeStatus").GetValue<bool>())
            {
                var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
                if (Config.Menu.Item("Enabled").GetValue<KeyBind>().Active)
                    Drawing.DrawText(heropos.X, heropos.Y, Color.Red, "Evade: ON");
            }

            var Border = Config.Menu.Item("Border").GetValue<Slider>().Value;
            var missileColor = Config.Menu.Item("MissileColor").GetValue<Color>();

            //Draw the polygon for each skillshot.
            foreach (var skillshot in DetectedSkillshots)
            {
                skillshot.Draw((skillshot.Evade() && Config.Menu.Item("Enabled").GetValue<KeyBind>().Active)
                        ? Config.Menu.Item("EnabledColor").GetValue<Color>()
                        : Config.Menu.Item("DisabledColor").GetValue<Color>(), missileColor, Border);
            }

            if (Config.TestOnAllies)
            {
                var myPath = ObjectManager.Player.GetWaypoints();

                for (var i = 0; i < myPath.Count - 1; i++)
                {
                    var A = myPath[i];
                    var B = myPath[i + 1];
                    var SA = Drawing.WorldToScreen(A.To3D());
                    var SB = Drawing.WorldToScreen(B.To3D());
                    Drawing.DrawLine(SA.X, SA.Y, SB.X, SB.Y, 1, Color.White);
                }

                //var evadePath = Pathfinding.Pathfinding.PathFind(PlayerPosition, Game.CursorPos.To2D());

                //for (var i = 0; i < evadePath.Count - 1; i++)
                //{
                //    var A = evadePath[i];
                //    var B = evadePath[i + 1];
                //    var SA = Drawing.WorldToScreen(A.To3D());
                //    var SB = Drawing.WorldToScreen(B.To3D());
                //    Drawing.DrawLine(SA.X, SA.Y, SB.X, SB.Y, 1, Color.Red);
                //}



                //Drawing.DrawCircle(EvadePoint.To3D(), 300, Color.White);
                //Drawing.DrawCircle(EvadeToPoint.To3D(), 300, Color.Red);
            }
        }

        public struct IsSafeResult
        {
            public bool IsSafe;
            public List<Skillshot> SkillshotList;
        }
    }
}
