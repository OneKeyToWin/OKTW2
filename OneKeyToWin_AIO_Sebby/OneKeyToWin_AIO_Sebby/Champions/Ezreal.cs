using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Ezreal : Base
    {
        Vector3 CursorPosition = Vector3.Zero;
        public double lag = 0;
        public double WCastTime = 0;
        public double QCastTime = 0;
        public float DragonDmg = 0;
        public double DragonTime = 0;
        public bool Esmart = false;
        public double OverKill = 0;
        public double OverFarm = 0;
        public double diag = 0;
        public double diagF = 0;
        public int Muramana = 3042;
        public int Tear = 3070;
        public int Manamune = 3004;
        public double NotTime = 0;

        public static Core.OKTWdash Dash;

        public Ezreal()
        {
            Q = new Spell(SpellSlot.Q, 1180);
            W = new Spell(SpellSlot.W, 1180);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 3000f);
            
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 60f, 1700f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("wPush", "W on towers", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("smartE", "SmartCast E key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("smartEW", "SmartCast E + W key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("EKsCombo", "E ks combo", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("EAntiMelee", "E anti-melee", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoEgrab", "Auto E anti grab", true).SetValue(true));
            Dash = new Core.OKTWdash(E);

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rcc", "R cc", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Raoe", "R AOE", true).SetValue(new Slider(3, 5, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rjungle", "R Jungle stealer", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rdragon", "Dragon", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rbaron", "Baron", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rred", "Red", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rblue", "Blue", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rally", "Ally stealer", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rturrent", "Don't R under turret", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("MaxRangeR", "Max R range", true).SetValue(new Slider(3000, 5000, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("MinRangeR", "Min R range", true).SetValue(new Slider(900, 5000, 0)));

            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("HarassMana", "Harass Mana", true).SetValue(new Slider(30, 100, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "LaneClear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("FQ", "Farm Q out range", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("LCP", "FAST LaneClear", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("debug", "Debug", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("stack", "Stack Tear if full mana", true).SetValue(false));

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Obj_AI_Base.OnBuffAdd += Obj_AI_Base_OnBuffAdd;
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Config.Item("wPush", true).IsActive() && args.Target.Type == GameObjectType.obj_AI_Turret)
            {
                if (W.IsReady() && Player.Mana > RMANA + WMANA + EMANA)
                {
                    if (W.Cast(args.Target.Position))
                    {
                        args.Process = false;
                        return;
                    }
                }
            }

            if (W.IsReady() && Player.Mana > RMANA + WMANA + EMANA)
            {
                var target = args.Target as Obj_AI_Hero;
                if (target != null)
                {
                    var prediction = W.GetPrediction(target);

                    if (prediction.Hitchance < HitChance.Medium || target.Distance(Player) - Player.BoundingRadius > Player.AttackRange - 50)
                        return;

                    if (Program.Combo && Player.Mana > RMANA + WMANA + EMANA)
                    {
                        if (W.Cast(prediction.CastPosition))
                        {
                            args.Process = false;
                            return;
                        }
                    }
                    else if (Program.Harass && Config.Item("harassW", true).GetValue<bool>() && Config.Item("Harass" + target.ChampionName).GetValue<bool>() && Player.Mana > Player.MaxMana * 0.8 && Player.ManaPercent > Config.Item("HarassMana", true).GetValue<Slider>().Value && OktwCommon.CanHarras())
                    {
                        if (W.Cast(prediction.CastPosition))
                        {
                            args.Process = false;
                            return;
                        }
                    }
                }
            }
        }

        private void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            if(sender.IsMe && Config.Item("autoEgrab", true).GetValue<bool>() && E.IsReady())
            {
                if(args.Buff.Name == "ThreshQ" || args.Buff.Name == "rocketgrab2")
                {
                    var dashPos = Dash.CastDash(true);
                    if (!dashPos.IsZero)
                    {
                        E.Cast(dashPos);
                    }
                    else
                    {
                        E.Cast(Game.CursorPos);
                    }
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
            }
            if (R.IsReady() && Config.Item("Rjungle", true).GetValue<bool>())
            {
                KsJungle();
            }
            else
                DragonTime = 0;

            if (E.IsReady())
            {
                if (Program.LagFree(0))
                    LogicE();

                if (Config.Item("smartE", true).GetValue<KeyBind>().Active)
                    Esmart = true;
                if (Config.Item("smartEW", true).GetValue<KeyBind>().Active && W.IsReady())
                {
                    CursorPosition = Game.CursorPos;
                    W.Cast(CursorPosition);
                }
                if (Esmart && Player.Position.Extend(Game.CursorPos, E.Range).CountEnemiesInRange(500) < 4)
                    E.Cast(Player.Position.Extend(Game.CursorPos, E.Range), true);
                
                if (!CursorPosition.IsZero)
                    E.Cast(Player.Position.Extend(CursorPosition, E.Range), true);
            }
            else
            {
                CursorPosition = Vector3.Zero;
                Esmart = false;
            }

            if (Q.IsReady())
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
                LogicW();

            if (R.IsReady())
            {
                if (Config.Item("useR", true).GetValue<KeyBind>().Active)
                {
                    var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget())
                        R.Cast(t, true, true);
                }

                if (Program.LagFree(4))
                    LogicR();
            }
        }

        private void LogicQ()
        {
            if (Utils.TickCount - W.LastCastAttemptT < 125)
            {
                return;
            }

            if (Program.LagFree(1))
            {
                if (!Orbwalking.CanMove(50) )
                    return;
                bool cc = !Program.None && Player.Mana > RMANA + QMANA + EMANA;
                bool harass = Program.Harass && Player.ManaPercent > Config.Item("HarassMana", true).GetValue<Slider>().Value && OktwCommon.CanHarras();

                if (Program.Combo && Player.Mana > RMANA + QMANA)
                {
                    var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                    if (t.IsValidTarget() && (!W.IsReady() || !Config.Item("autoW", true).GetValue<bool>() || !CanCastSpellPred(W, t)))
                    {
                        Program.CastSpell(Q, t);
                    }
                }

                foreach (var t in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range)).OrderBy(t => t.Health))
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var wDmg = W.GetDamage(t);
                    if (qDmg + wDmg > t.Health)
                    {
                        Program.CastSpell(Q, t);
                        OverKill = Game.Time;
                        return;
                    }

                    if (cc && (!W.IsReady() || !Config.Item("autoW", true).GetValue<bool>() || !CanCastSpellPred(W, t)) && !OktwCommon.CanMove(t))
                        Q.Cast(t);

                    if (harass && (!W.IsReady() || !Config.Item("autoW", true).GetValue<bool>() || !CanCastSpellPred(W, t)) && Config.Item("Harass" + t.ChampionName).GetValue<bool>())
                        Program.CastSpell(Q, t);
                }
            }
            else if (Program.LagFree(2))
            {
                if (Player.Mana > QMANA && Farm)
                {
                    farmQ();
                    lag = Game.Time;
                }
                else if (Config.Item("stack", true).GetValue<bool>() && Utils.TickCount - Q.LastCastAttemptT > 4000 && !Player.HasBuff("Recall") && Player.Mana > Player.MaxMana * 0.95 && Program.None && (Items.HasItem(Tear) || Items.HasItem(Manamune)))
                {
                    Q.Cast(Player.Position.Extend(Game.CursorPos, 500));
                }
            }
        }

        private void LogicW()
        {
            if (Utils.TickCount - Q.LastCastAttemptT < 125)
            {
                return;
            }

            if (!W.IsReady() || !Config.Item("autoW", true).GetValue<bool>())
                return;

            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (!((Q.IsReady() && CanCastSpellPred(Q, t)) || Orbwalking.InAutoAttackRange(t)))
                    return;

                if (Program.Combo && Player.Mana > RMANA + WMANA + EMANA)
                    Program.CastSpell(W, t);
                else if (Program.Harass && Config.Item("harassW", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.Mana > Player.MaxMana * 0.8 && Player.ManaPercent > Config.Item("HarassMana", true).GetValue<Slider>().Value && OktwCommon.CanHarras())
                    Program.CastSpell(W, t);
                else
                {
                    var qDmg = Q.GetDamage(t);
                    var wDmg = OktwCommon.GetKsDamage(t, W);
                    if (wDmg > t.Health)
                    {
                        Program.CastSpell(W, t);
                        OverKill = Game.Time;
                    }
                    else if (wDmg + qDmg > t.Health && Q.IsReady())
                    {
                        Program.CastSpell(W, t);
                    }
                }

                if (!Program.None && Player.Mana > RMANA + WMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
        }

        private void LogicE()
        {
            var t = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);
            
            if (Config.Item("EAntiMelee", true).GetValue<bool>())
            { 
                if (HeroManager.Enemies.Any(target => target.IsValidTarget(1000) && target.IsMelee && Player.Distance(Prediction.GetPrediction(target, 0.2f).CastPosition) < 250))
                {
                    var dashPos = Dash.CastDash(true);
                    if (!dashPos.IsZero)
                    {
                        E.Cast(dashPos);
                    }
                }
            }

            if (t.IsValidTarget() && Program.Combo && Config.Item("EKsCombo", true).GetValue<bool>() && Player.HealthPercent > 40 && t.Distance(Game.CursorPos) + 300 < t.Position.Distance(Player.Position) && !Orbwalking.InAutoAttackRange(t) && !Player.UnderTurret(true) && (Game.Time - OverKill > 0.3) )
            {
                var dashPosition = Player.Position.Extend(Game.CursorPos, E.Range);

                if (dashPosition.CountEnemiesInRange(900) < 3)
                {
                    var dmgCombo = 0f;
                    
                    if (t.IsValidTarget(950))
                    {
                        dmgCombo = (float)Player.GetAutoAttackDamage(t) + E.GetDamage(t);
                    }

                    if (Q.IsReady() && Player.Mana > QMANA + EMANA && Q.WillHit(dashPosition, Q.GetPrediction(t).UnitPosition))
                        dmgCombo = Q.GetDamage(t);

                    if (W.IsReady() && Player.Mana > QMANA + EMANA + WMANA )
                    {
                        dmgCombo += W.GetDamage(t);
                    }

                    if (dmgCombo > t.Health && OktwCommon.ValidUlt(t))
                    {
                        E.Cast(dashPosition);
                        OverKill = Game.Time;
                        Program.debug("E ks combo");
                    }
                }
            }
        }

        private void LogicR()
        {
            if (Player.UnderTurret(true) && Config.Item("Rturrent", true).GetValue<bool>())
                return;

            if (Config.Item("autoR", true).GetValue<bool>() && Player.CountEnemiesInRange(800) == 0 && Game.Time - OverKill > 0.6)
            {
                R.Range = Config.Item("MaxRangeR", true).GetValue<Slider>().Value;
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    double predictedHealth = target.Health - OktwCommon.GetIncomingDamage(target);

                    if (Config.Item("Rcc", true).GetValue<bool>() && target.IsValidTarget(Q.Range + E.Range) && target.Health < Player.MaxHealth && !OktwCommon.CanMove(target))
                    {
                        R.Cast(target, true, true);
                    }

                    double Rdmg = R.GetDamage(target);

                    if (Rdmg > predictedHealth)
                        Rdmg = getRdmg(target);

                    if (Rdmg > predictedHealth && target.CountAlliesInRange(500) == 0 && Player.Distance(target) > Config.Item("MinRangeR", true).GetValue<Slider>().Value)
                    {
                        Program.CastSpell(R,target);
                        Program.debug("R normal");
                    }
                    if (Program.Combo && Player.CountEnemiesInRange(1200) == 0)
                    {
                        R.CastIfWillHit(target, Config.Item("Raoe", true).GetValue<Slider>().Value, true);
                    }
                }
            }
        }

        private bool DashCheck(Vector3 dash)
        {
            if (!dash.UnderTurret(true) || Program.Combo)
                return true;
            else
                return false;
        }

        private double getRdmg(Obj_AI_Base target)
        {
            var rDmg = R.GetDamage(target);
            var dmg = 0;
            PredictionOutput output = R.GetPrediction(target);
            Vector2 direction = output.CastPosition.To2D() - Player.Position.To2D();
            direction.Normalize();
            List<Obj_AI_Hero> enemies = HeroManager.Enemies.Where(x =>x.IsValidTarget()).ToList();
            foreach (var enemy in enemies)
            {
                PredictionOutput prediction = R.GetPrediction(enemy);
                Vector3 predictedPosition = prediction.CastPosition;
                Vector3 v = output.CastPosition - Player.ServerPosition;
                Vector3 w = predictedPosition - Player.ServerPosition;
                double c1 = Vector3.Dot(w, v);
                double c2 = Vector3.Dot(v, v);
                double b = c1 / c2;
                Vector3 pb = Player.ServerPosition + ((float)b * v);
                float length = Vector3.Distance(predictedPosition, pb);
                if (length < (R.Width + 100 + enemy.BoundingRadius / 2) && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                    dmg++;
            }
            var allMinionsR = Cache.GetMinions(ObjectManager.Player.ServerPosition, R.Range);
            foreach (var minion in allMinionsR)
            {
                PredictionOutput prediction = R.GetPrediction(minion);
                Vector3 predictedPosition = prediction.CastPosition;
                Vector3 v = output.CastPosition - Player.ServerPosition;
                Vector3 w = predictedPosition - Player.ServerPosition;
                double c1 = Vector3.Dot(w, v);
                double c2 = Vector3.Dot(v, v);
                double b = c1 / c2;
                Vector3 pb = Player.ServerPosition + ((float)b * v);
                float length = Vector3.Distance(predictedPosition, pb);
                if (length < (R.Width + 100 + minion.BoundingRadius / 2) && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                    dmg++;
            }
            //if (Config.Item("debug", true).GetValue<bool>())
            //    Game.PrintChat("R collision" + dmg);
            if (dmg == 0)
                return rDmg;
            else if (dmg > 7)
                return rDmg * 0.7;
            else
                return rDmg - (rDmg * 0.1 * dmg);

        }

        private float GetPassiveTime()
        {
            return
                ObjectManager.Player.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Name == "ezrealrisingspellforce")
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
        }

        public void farmQ()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 800, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    Q.Cast(mob.Position);
                }
            }

            if (!Orbwalking.CanMove(50) || (Orbwalker.ShouldWait() && Orbwalking.CanAttack()))
            {
                return;
            }

            var minions = Cache.GetMinions(Player.ServerPosition, Q.Range);
            int orbTarget = 0;

            if (Orbwalker.GetTarget() != null)
                orbTarget = Orbwalker.GetTarget().NetworkId;

            if (Config.Item("FQ", true).GetValue<bool>())
            {
                foreach (var minion in minions.Where(minion => minion.IsValidTarget() && orbTarget != minion.NetworkId && !Orbwalker.InAutoAttackRange(minion)))
                {
                    int delay = (int)((minion.Distance(Player) / Q.Speed + Q.Delay) * 1000);
                    var hpPred = HealthPrediction.GetHealthPrediction(minion, delay);
                    if (hpPred > 0 && hpPred < Q.GetDamage(minion))
                    {
                        if (Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                            return;
                    }
                }
            }

            if (Config.Item("farmQ", true).GetValue<bool>() && !Orbwalking.CanAttack() && FarmSpells)
            {
                var LCP = Config.Item("LCP", true).GetValue<bool>();
                var PT = Game.Time - GetPassiveTime() > -1.5 || !E.IsReady();

                foreach (var minion in minions.Where(minion => Orbwalker.InAutoAttackRange(minion)))
                {
                    int delay = (int)((minion.Distance(Player) / Q.Speed + Q.Delay) * 1000);
                    var hpPred = HealthPrediction.GetHealthPrediction(minion, delay);
                    if (hpPred < 20)
                        continue;
                    
                    var qDmg = Q.GetDamage(minion);
                    if (hpPred < qDmg && orbTarget != minion.NetworkId)
                    {
                        if (Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                            return; 
                    }
                    else if (PT || LCP)
                    {
                        if (minion.HealthPercent > 80)
                        {
                            if (Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                return;
                        }
                    }
                }
            }
        }

        private void KsJungle()
        {
            var mobs = Cache.GetMinions(Player.ServerPosition, float.MaxValue, MinionTeam.Neutral);
            foreach (var mob in mobs)
            {
                if (mob.Health == mob.MaxHealth)
                    continue;
                if (((mob.SkinName.ToLower().Contains("dragon") && Config.Item("Rdragon", true).GetValue<bool>())
                    || (mob.SkinName == "SRU_Baron" && Config.Item("Rbaron", true).GetValue<bool>())
                    || (mob.SkinName == "SRU_Red" && Config.Item("Rred", true).GetValue<bool>())
                    || (mob.SkinName == "SRU_Blue" && Config.Item("Rblue", true).GetValue<bool>()))
                    && (mob.CountAlliesInRange(1000) == 0 || Config.Item("Rally", true).GetValue<bool>())
                    && mob.Distance(Player.Position) > 1000
                    )
                {
                    if (DragonDmg == 0)
                        DragonDmg = mob.Health;

                    if (Game.Time - DragonTime > 3)
                    {
                        if (DragonDmg - mob.Health > 0)
                        {
                            DragonDmg = mob.Health;
                        }
                        DragonTime = Game.Time;
                    }
                    else
                    {
                        var DmgSec = (DragonDmg - mob.Health) * (Math.Abs(DragonTime - Game.Time) / 3);
                        //Program.debug("DS  " + DmgSec);
                        if (DragonDmg - mob.Health > 0)
                        {
                            
                            var timeTravel = GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position);
                            var timeR = (mob.Health - R.GetDamage(mob)) / (DmgSec / 3);
                            //Program.debug("timeTravel " + timeTravel + "timeR " + timeR + "d " + R.GetDamage(mob));
                            if (timeTravel > timeR)
                                R.Cast(mob.Position);
                        }
                        else
                            DragonDmg = mob.Health;
                        //Program.debug("" + GetUltTravelTime(ObjectManager.Player, R.Speed, R.Delay, mob.Position));
                    }
                }
            }
        }

        private float GetUltTravelTime(Obj_AI_Hero source, float speed, float delay, Vector3 targetpos)
        {
            float distance = Vector3.Distance(source.ServerPosition, targetpos);
            float missilespeed = speed;

            return (distance / missilespeed + delay);
        }

        private void SetMana()
        {
            if ((Config.Item("manaDisable" ,true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.ManaCost;
            WMANA = W.Instance.ManaCost;
            EMANA = E.Instance.ManaCost;

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.ManaCost;
        }

        public static void drawText(string msg, Obj_AI_Hero Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1], color, msg);
        }


        public static bool CanCastSpellPred(Spell QWER, Obj_AI_Base target)
        {
            int predIndex = 0;
            HitChance hitchance = HitChance.Low;

            if (QWER.Slot == SpellSlot.Q)
            {
                predIndex = Config.Item("Qpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.W)
            {
                predIndex = Config.Item("Wpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.E)
            {
                predIndex = Config.Item("Epred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.R)
            {
                predIndex = Config.Item("Rpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }

            if (predIndex == 3)
            {
                SkillshotType CoreType2 = SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = Prediction.GetPrediction(predInput2);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return false;

                if ((int)hitchance == 6)
                {
                    if (poutput2.Hitchance >= HitChance.VeryHigh)
                        return true;
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= HitChance.High)
                    {
                        return true;
                    }

                }
                else if ((int)hitchance == 5)
                {
                    if (poutput2.Hitchance >= HitChance.High)
                        return true;

                }
                else if ((int)hitchance == 4)
                {
                    if (poutput2.Hitchance >= HitChance.Medium)
                        return true;
                }
            }
            else if (predIndex == 1)
            {
                SkillshotType CoreType2 = SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = Prediction.GetPrediction(predInput2);

                //var poutput2 = QWER.GetPrediction(target);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return false;

                if ((int)hitchance == 6)
                {
                    if (poutput2.Hitchance >= HitChance.VeryHigh)
                        return true;
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= HitChance.High)
                    {
                        return true;
                    }

                }
                else if ((int)hitchance == 5)
                {
                    if (poutput2.Hitchance >= HitChance.High)
                        return true;

                }
                else if ((int)hitchance == 4)
                {
                    if (poutput2.Hitchance >= HitChance.Medium)
                        return true;
                }
                if (Game.Time - DrawSpellTime > 0.5)
                {
                    DrawSpell = QWER;
                    DrawSpellTime = Game.Time;

                }
                DrawSpellPos = poutput2;
            }
            else if (predIndex == 0)
            {
                return QWER.GetPrediction(target).Hitchance >= hitchance;
            }
            else if (predIndex == 2)
            {
                return QWER.GetPrediction(target).Hitchance >= HitChance.High;
            }
            return false;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (Config.Item("wRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (Config.Item("eRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }


            if (Config.Item("noti", true).GetValue<bool>())
            {

                var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget())
                {

                    var poutput = Q.GetPrediction(target);
                    if ((int)poutput.Hitchance == 5)
                        Render.Circle.DrawCircle(poutput.CastPosition, 50, System.Drawing.Color.YellowGreen);
                    if (Q.GetDamage(target) > target.Health)
                    {
                        Render.Circle.DrawCircle(target.ServerPosition, 200, System.Drawing.Color.Red);
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "Q kill: " + target.ChampionName + " have: " + target.Health + "hp");
                    }
                    else if (Q.GetDamage(target) + W.GetDamage(target) > target.Health)
                    {
                        Render.Circle.DrawCircle(target.ServerPosition, 200, System.Drawing.Color.Red);
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "Q + W kill: " + target.ChampionName + " have: " + target.Health + "hp");
                    }
                    else if (Q.GetDamage(target) + W.GetDamage(target) + E.GetDamage(target) > target.Health)
                    {
                        Render.Circle.DrawCircle(target.ServerPosition, 200, System.Drawing.Color.Red);
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "Q + W + E kill: " + target.ChampionName + " have: " + target.Health + "hp");
                    }
                }
            }
        }
    }
}
