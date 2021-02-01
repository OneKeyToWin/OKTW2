#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

#endregion

namespace Evade
{
    internal static class SkillshotDetector
    {
        public class hiu_structure
        {
            public Vector2 position;
            public GameObject self;
            public int created_at;

            public hiu_structure(Vector2 p, GameObject o, int tick_count)
            {
                this.position = p;
                this.self = o;
                this.created_at = tick_count;
            }
        }

        public delegate void OnDeleteMissileH(Skillshot skillshot, MissileClient missile);
        public delegate void OnDetectSkillshotH(Skillshot skillshot);
        private static Vector2 LuxRPosition = Vector2.Zero;
        private static Vector2 LuxRPositionMiddle = Vector2.Zero;
        private static Obj_AI_Hero Jhin = null;
        private static Vector3 JhinLastRDirection;
        private static int JhinLastTimeR = 0;
        private static Obj_AI_Hero MissFortune = null;
        static SkillshotDetector()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (Config.TestOnAllies || hero.IsEnemy)
                {
                    if (hero.ChampionName == "Jhin")
                        Jhin = hero;
                    if (hero.ChampionName == "MissFortune")
                        MissFortune = hero;
                }
            }

            Obj_AI_Base.OnProcessSpellCast += ObjAiHeroOnOnProcessSpellCast;
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnBuffAdd += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnBuffRemove += OnBuffRemove;
            //Detect when projectiles collide.
            GameObject.OnDelete += ObjSpellMissileOnOnDelete;
            GameObject.OnCreate += ObjSpellMissileOnOnCreate;
            GameObject.OnCreate += GameObject_OnCreate; 
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffRemoveEventArgs args)
        {
           
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
           
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if(Jhin != null)
            {
                if (Jhin.Spellbook.GetSpell(SpellSlot.R).Name == "JhinRShot")
                {
                    if (!JhinLastRDirection.Equals(Jhin.Direction) && Utils.TickCount - JhinLastTimeR > 1500 )
                    {
                        var sd = SpellDatabase.GetByName("JhinRShot");  
                        if (sd != null)
                        {
                            var startTime = Utils.TickCount - Game.Ping / 2;
                            var startPos = Jhin.ServerPosition.To2D();
                            var endPos = Jhin.ServerPosition.To2D() + 3000 * Jhin.Direction.To2D().Rotated(-1.5707f).Perpendicular();
                            var direction = (endPos - startPos).Normalized();

                            if (sd.BehindStart != -1)
                                startPos = startPos - direction * sd.BehindStart;

                            if (sd.MinimalRange != -1 && startPos.Distance(endPos) < sd.MinimalRange)
                                endPos = startPos + direction * sd.MinimalRange;
                            
                            if (startPos.Distance(endPos) > sd.Range || sd.FixedRange)
                                endPos = startPos + direction * sd.Range;

                            if (sd.ExtraRange != -1)
                                endPos = endPos + Math.Min(sd.ExtraRange, sd.Range - endPos.Distance(startPos)) * direction;
                            
                            TriggerOnDetectSkillshot(DetectionType.ProcessSpell, sd, startTime, startPos, endPos, endPos, Jhin);
                        }
                    }
                    JhinLastRDirection = Jhin.Direction;
                }
                else
                {
                    JhinLastTimeR = Utils.TickCount;
                    JhinLastRDirection = new Vector3();
                }
            }
        
            //Get the skillshot data.
            var spellData = SpellDatabase.GetByName(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Name);

            if (Config.TestOnAllies && spellData != null && Program.DetectedSkillshots.Count == 0 )
            {
                //TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData,
                //    Utils.TickCount, Game.CursorPos.To2D(), ObjectManager.Player.Position.To2D(), ObjectManager.Player.Position.To2D(), ObjectManager.Player);
            }
        }

        private static void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (MissFortune != null && sender.NetworkId ==  MissFortune.NetworkId)
            {
                for (var i = Program.DetectedSkillshots.Count - 1; i >= 0; i--)
                {
                    var skillshot = Program.DetectedSkillshots[i];
                    if (skillshot.SpellData.Slot == SpellSlot.R)
                        Program.DetectedSkillshots.RemoveAt(i);
                }
            }

            if (args.IsDash && args.Path.Length >= 2)
            {
                var caster = sender as Obj_AI_Hero;

                if (caster == null)
                    return;

                var spellData = SpellDatabase.GetByDash(caster.ChampionName);

                if (spellData == null || spellData.CanDetectDash == null)
                    return;
                
                var startPos = args.Path[0].To2D();
                var endPos = args.Path.Last().To2D();
                var direction = (endPos - startPos).Normalized();

                if (spellData.BehindStart != -1)
                    startPos = startPos - direction * spellData.BehindStart;
                
                if (spellData.MinimalRange != -1 && startPos.Distance(endPos) < spellData.MinimalRange)
                    endPos = startPos + direction * spellData.MinimalRange;

                if (startPos.Distance(endPos) > spellData.Range || spellData.FixedRange)
                    endPos = startPos + direction * spellData.Range;

                if (spellData.ExtraRange != -1)
                    endPos = endPos + Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(startPos)) * direction;
                
                if (spellData.ExtraRange != -1)
                    endPos = endPos + Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(startPos)) * direction;

                if (spellData.DashDelayedAction == -1)
                {
                    if (spellData.CanDetectDash(sender, args))
                    {
                        TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2, startPos, endPos, sender.Position.To2D(), caster);
                    }
                }
                else
                {                    
                    Utility.DelayAction.Add(spellData.DashDelayedAction, () =>
                    {
                        if (spellData.CanDetectDash(sender, args))
                        {
                            TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2, startPos, endPos, sender.Position.To2D(), caster);
                        }
                    });
                }
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (Config.PrintSpellData && !(sender is MissileClient) && ObjectManager.Player.Distance(sender.Position) < 1000)
            {
                //var jhin = ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault();
                //if (jhin != null && sender.Name == "Jhin_Base_R_small_mis")
                //{
                //    Console.WriteLine(Utils.TickCount + " GameObject_OnCreate " + sender.Name + " " + sender.IsAlly + " " + sender.Type + " range : " + jhin.Distance(sender.Position));
                //    ObjectManager.Player.ForceIssueOrder(GameObjectOrder.MoveTo, sender.Position.Extend(jhin.Position, 1000));
                //    if(ObjectManager.Player.Spellbook.CastSpell(SpellSlot.E, sender.Position.Extend(jhin.Position, 1000)))
                //    {

                //    }
                //    else
                //        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Trinket, sender.Position.Extend(jhin.Position, 1000));
                //}
                Console.WriteLine(Utils.TickCount + " GameObject_OnCreate " + sender.Name + " " + sender.IsAlly + " " + sender.Type );
            }


            var spellData = SpellDatabase.GetBySourceObjectName(sender.Name);

            if (spellData != null)
            {
              

                if (Config.Menu.Item("Enabled" + spellData.MenuItemName) == null)
                    return;

                var caster = HeroManager.AllHeroes.Where(x => (x.IsEnemy || Config.TestOnAllies) && x.ChampionName == spellData.ChampionName)
                    .OrderByDescending(x => x.Position.Distance(sender.Position)).FirstOrDefault();

                if (caster == null)
                    return;

               if (spellData.SpellName == "KaynAssW")
                {
                    var clone = sender as Obj_AI_Minion;

                    if (clone != null)
                    {
                        Utility.DelayAction.Add(0, () =>
                        {
                            var startPos2 = clone.Position.To2D();
                            var endPos2 = startPos2 + clone.Direction.To2D() * spellData.Range;

                            TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2 - spellData.ParticleDetectDelay, startPos2, endPos2, sender.Position.To2D(), caster);
                        });
                    }

                    return;
                }
                else if (spellData.SpellName == "AzirQ")
                {
                    var azirSoldier = ObjectManager.Get<Obj_AI_Minion>().Where(x => (x.IsEnemy || Config.TestOnAllies) && x.Name == "AzirSoldier" && x.Health > 0).FirstOrDefault();

                    TriggerOnDetectSkillshot(DetectionType.RecvPacket, spellData, Utils.TickCount - Game.Ping / 2, azirSoldier.Position.To2D(), sender.Position.To2D(), sender.Position.To2D(), caster);
                    
                    return;
                }
                var startPos = caster.Position.To2D();
                var endPos = sender.Position.To2D();
                var direction = (endPos - startPos).Normalized();

                Obj_GeneralParticleEmitter objpartice = sender as Obj_GeneralParticleEmitter;

                if (objpartice != null && spellData.Type != SkillShotType.SkillshotCircle)
                {
                    direction = objpartice.DirectionEmitter.To2D();
                    startPos = sender.Position.To2D();

                    if (spellData.ParticleRotation > 0)
                        direction = direction.Rotated(Utils.ToRadians(spellData.ParticleRotation));
                }

                if (spellData.SpellName == "XerathArcanopulse")
                {
                    if (!Config.TestOnAllies)
                        foreach (var ally in HeroManager.Allies.Where(x => (x.ChampionName == spellData.ChampionName)))
                            if (ally.Distance(objpartice.Position) < 50)
                                return;

                    var buff = caster.GetBuff("XerathArcanopulseChargeUp");
                    if (buff != null)
                    {
                        var bufftime = Game.Time - buff.StartTime;
                        spellData.Range = 800f + 750f * Math.Min((bufftime / 1.4f), 1f);
                    }
                    else
                    {
                        spellData.Range = 1550;
                    }
                }

                if (spellData.BehindStart != -1)
                    startPos = startPos - direction * spellData.BehindStart;

                if (spellData.MinimalRange != -1 && startPos.Distance(endPos) < spellData.MinimalRange)
                    endPos = startPos + direction * spellData.MinimalRange;

                if (startPos.Distance(endPos) > spellData.Range || spellData.FixedRange)
                    endPos = startPos + direction * spellData.Range;

                if (spellData.ExtraRange != -1)
                    endPos = endPos + Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(startPos)) * direction;
               
                TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2 - spellData.ParticleDetectDelay, startPos, endPos, endPos, caster);
                return;
            }

            if (spellData == null)
                return;
            
            if (Config.Menu.Item("Enabled" + spellData.MenuItemName) == null)
                return;

            TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2, sender.Position.To2D(), sender.Position.To2D(), sender.Position.To2D(), HeroManager.AllHeroes.MinOrDefault(h => h.IsAlly ? 1 : 0));
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || !Config.TestOnAllies && sender.Team == ObjectManager.Player.Team)
            {
                return;
            }

            for (var i = Program.DetectedSkillshots.Count - 1; i >= 0; i--)
            {
                var skillshot = Program.DetectedSkillshots[i];
                if (skillshot.SpellData.ToggleParticleName != "" && new Regex(skillshot.SpellData.ToggleParticleName).IsMatch(sender.Name) && !skillshot.IsSafe(sender.Position.To2D()))
                {
                    Program.DetectedSkillshots.RemoveAt(i);
                }
                if (skillshot.SpellData.EndAtParticle != "" && new Regex(skillshot.SpellData.EndAtParticle, System.Text.RegularExpressions.RegexOptions.IgnoreCase).IsMatch(sender.Name) && !skillshot.IsSafe(sender.Position.To2D()))
                {
                    Program.DetectedSkillshots.RemoveAt(i);
                }
            }
        }

        private static void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            //var unit = missile.SpellCaster as Obj_AI_Hero;
            //
            //if (unit == null || !unit.IsValid || (unit.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
            //{
            //    return;
            //}
            //
            //Console.WriteLine(
            //        Utils.TickCount + " Projectile Created: " + missile.SData.Name + " distance: " +
            //        missile.SData.CastRange + "Radius: " +
            //        missile.SData.LineWidth + " Speed: " + missile.SData.MissileSpeed);  
            //
            //var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);
            //
            //if (spellData == null)
            //{
            //    return;
            //}

            Utility.DelayAction.Add(0, delegate
            {
                ObjSpellMissionOnOnCreateDelayed(sender, args);
            });
        }

        private static void ObjSpellMissionOnOnCreateDelayed(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
                return;
                       
            var unit = missile.SpellCaster as Obj_AI_Hero;

            if (unit == null || !unit.IsValid || (unit.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
                return;
            

            if (Config.TestOnAllies) Console.WriteLine(Utils.TickCount + " Projectile Created: " + missile.SData.Name + " distance: " + missile.SData.CastRange + "Radius: " +  missile.SData.LineWidth + " Speed: " + missile.SData.MissileSpeed);

            var missileName = missile.SData.Name;

            if(missileName == "ThreshQMissile")
            {
                for (var i = Program.DetectedSkillshots.Count - 1; i >= 0; i--)
                {
                    var skillshot = Program.DetectedSkillshots[i];
                    if (skillshot.SpellData.SpellName == "ThreshQInternal")
                    {
                        Program.DetectedSkillshots.RemoveAt(i);
                    }
                }
            }
            if (missileName.Contains("HowlingGaleSpell"))
                missileName = "HowlingGaleSpell";

            var spellData = SpellDatabase.GetByMissileName(missileName);

            if (spellData == null)
                return;

            if (missileName == "HowlingGaleSpell")
                spellData.MissileSpeed = (int)missile.SData.MissileSpeed;

            var missilePosition = missile.Position.To2D();
            var unitPosition = missile.StartPosition.To2D();
            var endPos = missile.EndPosition.To2D();

            //Calculate the real end Point:
            var direction = (endPos - unitPosition).Normalized();

            if (spellData.MinimalRange != -1 && unitPosition.Distance(endPos) < spellData.MinimalRange)
                    endPos = unitPosition + direction * spellData.MinimalRange;

            if (unitPosition.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = unitPosition + direction * spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos +  Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(unitPosition)) * direction;
            }

            var castTime = Utils.TickCount - Game.Ping / 2 - (spellData.MissileDelayed ? 0 : spellData.Delay) -
                           (int)(1000f * missilePosition.Distance(unitPosition) / spellData.MissileSpeed);
            
            //Trigger the skillshot detection callbacks.
            TriggerOnDetectSkillshot(DetectionType.RecvPacket, spellData, castTime, unitPosition, endPos, endPos, unit);
        }

        private static void ObjSpellMissileOnOnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var caster = missile.SpellCaster as Obj_AI_Hero;

            if (caster == null || !caster.IsValid || (caster.Team == ObjectManager.Player.Team && !Config.TestOnAllies))
            {
                return;
            }

            var spellName = missile.SData.Name;
            if (OnDeleteMissile != null)
            {
                foreach (var skillshot in Program.DetectedSkillshots)
                {
                    if (skillshot.SpellData.MissileSpellName.Equals(spellName, StringComparison.InvariantCultureIgnoreCase) &&
                        (skillshot.Unit.NetworkId == caster.NetworkId &&
                         (missile.EndPosition.To2D() - missile.StartPosition.To2D()).AngleBetween(skillshot.Direction) <
                         10) && skillshot.SpellData.CanBeRemoved)
                    {
                        OnDeleteMissile(skillshot, missile);
                        break;
                    }
                }
            }

#if DEBUG
           /* Console.WriteLine(
                "Missile deleted: " + missile.SData.Name + " D: " + missile.EndPosition.Distance(missile.Position)); */
#endif

            Program.DetectedSkillshots.RemoveAll(
                skillshot =>
                    (skillshot.SpellData.MissileSpellName.Equals(spellName, StringComparison.InvariantCultureIgnoreCase) ||
                     skillshot.SpellData.ExtraMissileNames.Contains(spellName, StringComparer.InvariantCultureIgnoreCase)) &&
                    (skillshot.Unit.NetworkId == caster.NetworkId &&
                     ((missile.EndPosition.To2D() - missile.StartPosition.To2D()).AngleBetween(skillshot.Direction) < 10) &&
                     skillshot.SpellData.CanBeRemoved || skillshot.SpellData.ForceRemove)); // 
        }

        public static event OnDetectSkillshotH OnDetectSkillshot;

        public static event OnDeleteMissileH OnDeleteMissile;

        internal static void TriggerOnDetectSkillshot(DetectionType detectionType,
            SpellData spellData,
            int startT,
            Vector2 start,
            Vector2 end,
            Vector2 originalEnd,
            Obj_AI_Base unit)
        {
            var skillshot = new Skillshot(detectionType, spellData, startT, start, end, unit)
            {
                OriginalEnd = originalEnd
            };

            if (OnDetectSkillshot != null)
            {
                OnDetectSkillshot(skillshot);
            }
        }

        private static void ObjAiHeroOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid)
                return;

            if (Config.PrintSpellData && sender is Obj_AI_Hero)
            {
                Game.PrintChat(Utils.TickCount + " ProcessSpellCast: " + args.SData.Name);
                Console.WriteLine(Utils.TickCount + " ProcessSpellCast: " + args.SData.Name);
            }

            if (args.SData.Name == "dravenrdoublecast")
            {
                Program.DetectedSkillshots.RemoveAll(
                    s => s.Unit.NetworkId == sender.NetworkId && s.SpellData.SpellName == "DravenRCast");
            }

            if (!sender.IsValid || sender.Team == ObjectManager.Player.Team && !Config.TestOnAllies)
            {
                return;
            }
            //Get the skillshot data.
            var spellData = SpellDatabase.GetByName(args.SData.Name);

            //Skillshot not added in the database.
            if (spellData == null)
                return;

            var startPos = new Vector2();

            if (spellData.FromObject != "")
            {
                foreach (var o in ObjectManager.Get<GameObject>())
                {
                    if (o.Name.Contains(spellData.FromObject))
                    {
                        startPos = o.Position.To2D();
                    }
                }
            }
            else
            {
                startPos = sender.ServerPosition.To2D();
            }

            //For now only zed support.
            if (spellData.FromObjects != null && spellData.FromObjects.Length > 0)
            {
                foreach (var obj in ObjectManager.Get<GameObject>())
                {
                    if (obj.IsEnemy && spellData.FromObjects.Contains(obj.Name))
                    {
                        var start = obj.Position.To2D();
                        var end = start + spellData.Range * (args.End.To2D() - obj.Position.To2D()).Normalized();
                        TriggerOnDetectSkillshot(
                            DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2, start, end, end,
                            sender);
                    }
                }
            }

            if (!startPos.IsValid())
            {
                return;
            }

            var endPos = args.End.To2D();

            if (spellData.SpellName == "RakanW" && args.Start.Distance(args.End) > 100)
            {
                return;
            }

            if ((spellData.SpellName == "LucianQ" || spellData.SpellName == "SennaQCast") && args.Target != null &&
                args.Target.NetworkId == ObjectManager.Player.NetworkId)
            {
                return;
            }

            if (spellData.SpellName == "WarwickR")
            {
                spellData.Range = (int)(2.1f * sender.MoveSpeed);
                spellData.MissileSpeed = (int)(4.0f * sender.MoveSpeed);
            }

            var startTime = Utils.TickCount - Game.Ping / 2;
            //Calculate the real end Point:
            var direction = (endPos - startPos).Normalized();

            if (spellData.SpellName == "YorickE")
            {
                startPos = endPos - direction * 120;

                startTime += (int)(sender.Position.Distance(startPos.To3D()) / 1.8f);
            }

            if (spellData.BehindStart != -1)
            {
                startPos = startPos - direction * spellData.BehindStart;
            }

            if (spellData.MinimalRange != -1)
            {
                if (startPos.Distance(endPos) < spellData.MinimalRange)
                {
                    endPos = startPos + direction * spellData.MinimalRange;
                }
            }

            if (startPos.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = startPos + direction * spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos +
                         Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(startPos)) * direction;
            }

            if (spellData.ChampionName == "Yone" &&
                spellData.Slot == SpellSlot.Q)
            {
                spellData.Delay = (int)(args.CastDelay * 1000);
            }

            if (spellData.SpellName == "ThreshQInternal")
            {
                var points = Utils.CirclePointsNormal(40, 1100, sender.Position.To2D());

                foreach (var point in points)
                {
                    if (ObjectManager.Player.Distance(point) < 600)
                    {
                        TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, startTime, startPos, point, point, sender);
                    }
                }
                return;
            }
            if (spellData.SpellName == "MissFortuneRicochetShot" && args.Target != null && !args.Target.IsMe)
            {
                startPos = args.Target.Position.To2D();
                endPos = startPos.Extend(sender.Position.To2D(), -300);
                TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, startTime, startPos, endPos, endPos, sender);
                return;
            }
            //Trigger the skillshot detection callbacks.
            TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, startTime, startPos, endPos, args.End.To2D(), sender);
        }

        

    }
}
