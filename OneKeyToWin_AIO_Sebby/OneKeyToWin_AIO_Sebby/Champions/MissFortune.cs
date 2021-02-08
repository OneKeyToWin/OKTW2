using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class MissFortune : Base
    {
        private int LastAttackId = 0;
        private float RCastTime = 0;

        public MissFortune()
        {
            Q = new Spell(SpellSlot.Q, 655f);
            Q1 = new Spell(SpellSlot.Q, 1300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1350f);

            Q1.SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);
            Q.SetTargetted(0.25f, 1400f);
            E.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 50f, 3000f, false, SkillshotType.SkillshotCircle);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("QRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("ERange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("RRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification & line", true).SetValue(true));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").SubMenu("Minion config").AddItem(new MenuItem("harassQ", "Use Q on minion", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").SubMenu("Minion config").AddItem(new MenuItem("killQ", "Use Q only if can kill minion", true).SetValue(false));
            HeroMenu.SubMenu("Q Config").SubMenu("Minion config").AddItem(new MenuItem("qMinionMove", "Don't use if minions moving", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").SubMenu("Minion config").AddItem(new MenuItem("qMinionWidth", "secound Q angle", true).SetValue(new Slider(80, 100, 0)));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("AGC", "AntiGapcloserE", true).SetValue(true));

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("forceBlockMove", "Force block player", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("disableBlock", "Disable R key", true).SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press))); //32 == space
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("Rturrent", "Don't R under turret", true).SetValue(true));

            HeroMenu.AddItem(new MenuItem("newTarget", "Try change focus after attack ", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "LaneClear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "LaneClear W", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmE", "LaneClear E", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));


            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && MainMenu.Item("AGC", true).GetValue<bool>() &&  Player.Mana > RMANA + EMANA)
            {
                var Target = gapcloser.Sender;
                if (Target.IsValidTarget(E.Range))
                {
                    E.Cast(gapcloser.End);
                }
                return;
            }
            return;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "MissFortuneBulletTime")
            {
                RCastTime = Game.Time;
                Program.debug(args.SData.Name);
                Orbwalking.Attack = false;
                Orbwalking.Move = false;
                if (MainMenu.Item("forceBlockMove", true).GetValue<bool>())
                {
                    OktwCommon.blockMove = true;
                    OktwCommon.blockAttack = true;
                    OktwCommon.blockSpells = true;
                }
            }
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
                return;
            LastAttackId = target.NetworkId;

            var t = target as Obj_AI_Hero;
            if (t != null)
            {
                if (Q.IsReady())
                {
                    if (Q.GetDamage(t) + Player.GetAutoAttackDamage(t) * 3 > t.Health)
                        Q.Cast(t);
                    else if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                        Q.Cast(t);
                    else if (Program.Harass && Player.Mana > RMANA + QMANA + EMANA + WMANA && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                        Q.Cast(t);
                }
                if (W.IsReady())
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Player.Mana > RMANA + WMANA && MainMenu.Item("autoW", true).GetValue<bool>())
                        W.Cast();
                    else if (Player.Mana > RMANA + WMANA + QMANA && MainMenu.Item("harassW", true).GetValue<bool>())
                        W.Cast();
                }
            }
            else if (FarmSpells )
            {
                var minions = Cache.GetMinions(Player.ServerPosition, 600);

                if (minions.Count >= FarmMinions)
                {
                    if (Q.IsReady() && MainMenu.Item("farmQ", true).GetValue<bool>() && minions.Count > 1)
                        Q.Cast(minions.FirstOrDefault());
                    if (W.IsReady() && MainMenu.Item("farmW", true).GetValue<bool>() && minions.Count > 1)
                        W.Cast();
                }
            }


        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA )
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>() && !Orbwalking.CanAttack() && !Player.IsWindingUp)
                    {
                        Q.Cast(mob);
                        return;
                    }
                    if (W.IsReady() && MainMenu.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast();
                        return;
                    }
                    if (E.IsReady() && MainMenu.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (MainMenu.Item("disableBlock", true).GetValue<KeyBind>().Active)
            {
                Orbwalking.Attack = true;
                Orbwalking.Move = true;
                OktwCommon.blockSpells = false;
                OktwCommon.blockAttack = false;
                OktwCommon.blockMove = false;
                return;
            }
            else if (Player.IsChannelingImportantSpell() || Game.Time - RCastTime < 0.3)
            {
                if (MainMenu.Item("forceBlockMove", true).GetValue<bool>())
                {
                    OktwCommon.blockMove = true;
                    OktwCommon.blockAttack = true;
                    OktwCommon.blockSpells = true;
                }

                Orbwalking.Attack = false;
                Orbwalking.Move = false;
               
                Program.debug("cast R");
                return;
            }
            else
            {
                Orbwalking.Attack = true;
                Orbwalking.Move = true;
                if (MainMenu.Item("forceBlockMove", true).GetValue<bool>())
                {
                    OktwCommon.blockAttack = false;
                    OktwCommon.blockMove = false;
                    OktwCommon.blockSpells = false;
                }
                if (R.IsReady() && MainMenu.Item("useR", true).GetValue<KeyBind>().Active)
                {
                    var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget(R.Range))
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                        return;
                    }
                }
            }

            if (MainMenu.Item("newTarget", true).GetValue<bool>())
            {
                var orbT = Orbwalker.GetTarget();

                Obj_AI_Hero t2 = null;

                if (orbT != null && orbT is Obj_AI_Hero)
                    t2 = (Obj_AI_Hero)orbT;

                if (t2.IsValidTarget() && t2.NetworkId == LastAttackId)
                {
                    var ta = HeroManager.Enemies.Where(enemy => 
                        enemy.IsValidTarget() && Orbwalking.InAutoAttackRange(enemy) 
                            && (enemy.NetworkId != LastAttackId || enemy.Health < Player.GetAutoAttackDamage(enemy) * 2) ).FirstOrDefault();

                    if (ta!=null)
                        Orbwalker.ForceTarget(ta);
                }
            }

            if (Program.LagFree(1))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(2) && !Player.IsWindingUp && Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(3) && !Player.IsWindingUp && E.IsReady() && MainMenu.Item("autoE", true).GetValue<bool>())
                LogicE();

            if (Program.LagFree(4) && !Player.IsWindingUp && R.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                LogicR();
            
        }
        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var t1 = TargetSelector.GetTarget(Q1.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget(Q.Range) && Player.Distance(t.ServerPosition) > 500)
            {
                var qDmg = OktwCommon.GetKsDamage(t, Q);
                if (qDmg + Player.GetAutoAttackDamage(t) > t.Health)
                    Q.Cast(t);
                else if (qDmg + Player.GetAutoAttackDamage(t) * 3 > t.Health)
                    Q.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                    Q.Cast(t);
                else if (Program.Harass && Player.Mana > RMANA + QMANA + EMANA + WMANA && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                    Q.Cast(t);
            }
            else if (t1.IsValidTarget(Q1.Range) && MainMenu.Item("harassQ", true).GetValue<bool>() && Player.Distance(t1.ServerPosition) > Q.Range + 50)
            {
                var minions = Cache.GetMinions(Player.ServerPosition, Q1.Range);



                if (MainMenu.Item("qMinionMove", true).GetValue<bool>())
                {
                    if (minions.Exists(x => x.IsMoving))
                        return;
                }

                var enemyPredictionPos = Prediction.GetPrediction(t1, 0.2f).CastPosition;
                foreach(var minion in minions)
                {
                    if (MainMenu.Item("killQ", true).GetValue<bool>() && Q.GetDamage(minion) < minion.Health)
                        continue;

                    var posExt = Player.ServerPosition.Extend(minion.ServerPosition, 420 + Player.Distance(minion));
                    
                    if (InCone(enemyPredictionPos, posExt, minion.ServerPosition, MainMenu.Item("qMinionWidth", true).GetValue<Slider>().Value))
                    {
                        Program.debug("dupa");
                        if (minions.Exists(x => 
                        InCone(x.Position, posExt, minion.ServerPosition, MainMenu.Item("qMinionWidth", true).GetValue<Slider>().Value)
                        ))
                            continue;
                        Q.Cast(minion);
                        return;
                    }
                }
            }
        }


        private bool InCone(Vector3 Position, Vector3 finishPos, Vector3 firstPos, int angleSet)
        {
            var range = 420;
            var angle = angleSet * (float)Math.PI / 180;
            var end2 = finishPos.To2D() - firstPos.To2D();
            var edge1 = end2.Rotated(-angle / 2);
            var edge2 = edge1.Rotated(angle);

            var point = Position.To2D() - firstPos.To2D();
            if (point.Distance(new Vector2(), true) < range * range && edge1.CrossProduct(point) > 0 && point.CrossProduct(edge2) > 0)
                return true;

            return false;
        }

        private void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                var eDmg = OktwCommon.GetKsDamage(t, E);
                if (eDmg > t.Health)
                    Program.CastSpell(E, t);
                else if (eDmg + Q.GetDamage(t) > t.Health && Player.Mana > QMANA + EMANA + RMANA)
                    Program.CastSpell(E, t);
                else if (Program.Combo && Player.Mana > RMANA + WMANA + QMANA + EMANA)
                {
                    if (!Orbwalking.InAutoAttackRange(t) || Player.CountEnemiesInRange(300) > 0 || t.CountEnemiesInRange(250) > 1)
                        Program.CastSpell(E, t);
                    else 
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                            E.Cast(enemy, true, true);
                    }
                }
            }
            if (FarmSpells && MainMenu.Item("farmE", true).GetValue<bool>())
            {
                var minions = Cache.GetMinions(Player.ServerPosition, E.Range);
                var farmPos = E.GetCircularFarmLocation(minions, E.Width);
                if (farmPos.MinionsHit >= FarmMinions)
                {
                    E.Cast(farmPos.Position);
                }
            }
        }

        private void LogicR()
        {
            if (Player.UnderTurret(true) && MainMenu.Item("Rturrent", true).GetValue<bool>())
                return;

            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget(R.Range) && OktwCommon.ValidUlt(t))
            {
                var rDmg = R.GetDamage(t) * new double[] { 0.5, 0.75, 1 }[R.Level - 1];

                if (Player.CountEnemiesInRange(700) == 0 && t.CountAlliesInRange(400) == 0)
                {
                    var tDis = Player.Distance(t.ServerPosition);
                    if (rDmg * 7 > t.Health && tDis < 800)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 6 > t.Health && tDis < 900)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 5 > t.Health && tDis < 1000)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 4 > t.Health && tDis < 1100)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 3 > t.Health && tDis < 1200)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg > t.Health && tDis < 1300)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    return;
                }
                if (rDmg * 8 > t.Health - OktwCommon.GetIncomingDamage(t) && rDmg * 2 < t.Health && Player.CountEnemiesInRange(300) == 0 && !OktwCommon.CanMove(t))
                {
                    R.Cast(t, true, true);
                    RCastTime = Game.Time;
                    return;
                }
            }

        }

        private void SetMana()
        {
            if ((MainMenu.Item("manaDisable", true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20)
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

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("noti", true).GetValue<bool>() && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var rDamage = R.GetDamage(t) + (W.GetDamage(t) * 10);
                    if (rDamage * 8 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.GreenYellow, "8 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.GreenYellow);
                    }
                    else if (rDamage * 5 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Orange, "5 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Orange);
                    }
                    else if (rDamage * 3 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Yellow, "3 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                    else if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "1 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Red);
                    }
                }
            }

            if (MainMenu.Item("QRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (MainMenu.Item("ERange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (MainMenu.Item("RRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }

        public static void drawText(string msg, Obj_AI_Base Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1], color, msg);

        }
    }
}