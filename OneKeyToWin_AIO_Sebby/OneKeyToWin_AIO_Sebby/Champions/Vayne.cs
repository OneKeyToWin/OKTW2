using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Vayne : Base
    {
        public static Core.OKTWdash Dash;

        public Vayne()
        {
            Q = new Spell(SpellSlot.Q, 300);
            E = new Spell(SpellSlot.E, 670);
            W = new Spell(SpellSlot.E, 670);
            R = new Spell(SpellSlot.R, 3000);

            E.SetTargetted(0.25f, 2200f);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange2", "E push position", true).SetValue(false));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("Qstack", "Q at X stack", true).SetValue(new Slider(2, 2, 1)));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("QE", "try Q + E ", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("Qonly", "Q only after AA", true).SetValue(false));
            Dash = new Core.OKTWdash(Q);

            HeroMenu.SubMenu("E Config").SubMenu("GapCloser").AddItem(new MenuItem("gapE", "Enable", true).SetValue(true));
            
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("E Config").SubMenu("GapCloser").AddItem(new MenuItem("gap" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("E Config").SubMenu("Use E ").AddItem(new MenuItem("stun" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("useE", "OneKeyToCast E closest person", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("Eks", "E KS", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("Ecombo", "E combo only", true).SetValue(false));

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("visibleR", "Unvisable block AA ", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoQR", "Auto Q when R active ", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "Q farm helper", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQjungle", "Q jungle", true).SetValue(true));


            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += afterAttack;
            Interrupter2.OnInterruptableTarget +=Interrupter2_OnInterruptableTarget;
            //Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(E.Range))
                E.Cast(sender);
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;

            if (E.IsReady() && target.IsValidTarget(E.Range) && MainMenu.Item("gapE", true).GetValue<bool>() && MainMenu.Item("gap" + target.ChampionName).GetValue<bool>())
                E.Cast(target);
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (MainMenu.Item("visibleR", true).GetValue<bool>() && Player.HasBuff("vaynetumblefade") && Player.CountEnemiesInRange(800) > 1)
                args.Process = false;

            if (args.Target.Type != GameObjectType.obj_AI_Hero)
                return;

            var t = args.Target as Obj_AI_Hero;

            if (GetWStacks(t) < 2 && args.Target.Health > 5 * Player.GetAutoAttackDamage(t))
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(800) && GetWStacks(target) == 2))
                {
                    if (Orbwalking.InAutoAttackRange(target) && args.Target.Health > 3 * Player.GetAutoAttackDamage(target))
                    {
                        args.Process = false;
                        Orbwalker.ForceTarget(target);
                    }
                }
            }
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var t = target as Obj_AI_Hero;
            if (t != null)
            {
                if (E.IsReady() && MainMenu.Item("Eks", true).GetValue<bool>())
                {
                    var incomingDMG = OktwCommon.GetIncomingDamage(t, 0.3f, false);
                    if (incomingDMG > t.Health)
                        return;

                    var dmgE = E.GetDamage(t) + incomingDMG;

                    if (GetWStacks(t) == 1)
                        dmgE += Wdmg(t);

                    if (dmgE > t.Health)
                    {
                        E.Cast(t);
                    }
                }

                if (Q.IsReady() && !Program.None && MainMenu.Item("autoQ", true).GetValue<bool>() && (GetWStacks(t) == MainMenu.Item("Qstack", true).GetValue<Slider>().Value - 1 || Player.HasBuff("vayneinquisition")))
                {
                    var dashPos = Dash.CastDash(true);
                    if (!dashPos.IsZero)
                    {
                        Q.Cast(dashPos);
                    }
                }
            }

            var m = target as Obj_AI_Minion;

            if (m != null && Q.IsReady() && Program.Farm && MainMenu.Item("farmQ", true).GetValue<bool>())
            {
                var dashPosition = Player.Position.Extend(Game.CursorPos, Q.Range);
                if (!Dash.IsGoodPosition(dashPosition))
                    return;
                
                if (MainMenu.Item("farmQjungle", true).GetValue<bool>() && m.Team == GameObjectTeam.Neutral)
                {
                    Q.Cast(dashPosition, true);
                }

                if (MainMenu.Item("farmQ", true).GetValue<bool>())
                {
                    foreach (var minion in Cache.GetMinions(dashPosition, 0).Where(minion => m.NetworkId != minion.NetworkId))
                    {
                        var time = (int)(Player.AttackCastDelay * 1000) + Game.Ping / 2 + 1000 * (int)Math.Max(0, Player.Distance(minion) - Player.BoundingRadius) / (int)Player.BasicAttack.MissileSpeed;
                        var predHealth = HealthPrediction.GetHealthPrediction(minion, time);
                        if (predHealth < Player.GetAutoAttackDamage(minion) + Q.GetDamage(minion) && predHealth > 0)
                            Q.Cast(dashPosition, true);
                    }
                }
            }
        }

        private double Wdmg(Obj_AI_Base target)
        {
            return target.MaxHealth * (4.5 + W.Level * 1.5) * 0.01;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            var dashPosition = Player.Position.Extend(Game.CursorPos, Q.Range);

            if (E.IsReady())
            {
                if (!MainMenu.Item("Ecombo", true).GetValue<bool>() || Program.Combo)
                    {
                    var ksTarget = Player;
                    foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(E.Range) && target.Path.Count() < 2))
                    {
                        if (CondemnCheck(Player.ServerPosition, target) && MainMenu.Item("stun" + target.ChampionName).GetValue<bool>())
                            E.Cast(target);
                        else if (Q.IsReady() && Dash.IsGoodPosition(dashPosition) && MainMenu.Item("QE", true).GetValue<bool>() && CondemnCheck(dashPosition, target))
                        {
                            Q.Cast(dashPosition);
                            Program.debug("Q + E");
                        }
                    }
                }
            }

            if (Program.LagFree(1) && Q.IsReady())
            {
                if (MainMenu.Item("autoQR", true).GetValue<bool>() && Player.HasBuff("vayneinquisition")  && Player.CountEnemiesInRange(1500) > 0 && Player.CountEnemiesInRange(670) != 1)
                {
                    var dashPos = Dash.CastDash();
                    if (!dashPos.IsZero)
                    {
                        Q.Cast(dashPos);
                    }
                }
                if (Program.Combo && MainMenu.Item("autoQ", true).GetValue<bool>() && !MainMenu.Item("Qonly", true).GetValue<bool>())
                {
                    var t = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

                    if (t.IsValidTarget() && !Orbwalking.InAutoAttackRange(t) && t.Position.Distance(Game.CursorPos) < t.Position.Distance(Player.Position) &&  !t.IsFacing(Player))
                    {
                        var dashPos = Dash.CastDash();
                        if (!dashPos.IsZero)
                        {
                            Q.Cast(dashPos);
                        }
                    }
                }
            }

            if (Program.LagFree(2))
            {
                Obj_AI_Hero bestEnemy = null;
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(E.Range)))
                {
                    if (target.IsValidTarget(250) && target.IsMelee)
                    {
                        if (Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                        {
                            var dashPos = Dash.CastDash(true);
                            if (!dashPos.IsZero)
                            {
                                Q.Cast(dashPos);
                            }
                        }
                        else if (E.IsReady() && Player.Health < Player.MaxHealth * 0.4)
                        {
                            E.Cast(target);
                            Program.debug("push");
                        }
                    }
                    if (bestEnemy == null)
                        bestEnemy = target;
                    else if (Player.Distance(target.Position) < Player.Distance(bestEnemy.Position))
                        bestEnemy = target;
                }
                if (MainMenu.Item("useE", true).GetValue<KeyBind>().Active && bestEnemy != null)
                {
                    E.Cast(bestEnemy);
                }
            }

            if (Program.LagFree(3) && R.IsReady() )
            {
                if ( MainMenu.Item("autoR", true).GetValue<bool>())
                {
                    if (Player.CountEnemiesInRange(700) > 2)
                        R.Cast();
                    else if (Program.Combo && Player.CountEnemiesInRange(600) > 1)
                        R.Cast();
                    else if (Player.Health < Player.MaxHealth * 0.5 && Player.CountEnemiesInRange(500) > 0)
                        R.Cast();
                }
            }
        }

        private bool CondemnCheck(Vector3 fromPosition, Obj_AI_Hero target)
        {
            var prepos = E.GetPrediction(target);

            float pushDistance = 470;

            if (Player.ServerPosition != fromPosition)
                pushDistance = 410 ;

            int radius = 250;
            var start2 = target.ServerPosition;
            var end2 = prepos.CastPosition.Extend(fromPosition, -pushDistance);

            Vector2 start = start2.To2D();
            Vector2 end = end2.To2D();
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;


            var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
            var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);


            var step = start2.Distance(rEndPos) / 10;
            for (var i = 0; i < 10; i++)
            {
                var pr = start2.Extend(rEndPos, step * i);
                var pl = start2.Extend(lEndPos, step * i);
                if (pr.IsWall() && pl.IsWall())
                    return true;
            }

            return false;
        }

        private int GetWStacks(Obj_AI_Base target)
        {
            foreach (var buff in target.Buffs)
            {
                if (buff.Name.ToLower() == "vaynesilvereddebuff")
                    return buff.Count;
            }
            return 0;
        }

        private List<Vector3> CirclePoint(float CircleLineSegmentN, float radius, Vector3 position)
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

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("qRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range + E.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range + E.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (E.IsReady() && MainMenu.Item("eRange2", true).GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(800)))
                {
                    var poutput = E.GetPrediction(target);

                    var pushDistance = 460;

                    var finalPosition = poutput.CastPosition.Extend(Player.ServerPosition, -pushDistance);
                    if (finalPosition.IsWall())
                        Render.Circle.DrawCircle(finalPosition, 100, System.Drawing.Color.Red);
                    else
                        Render.Circle.DrawCircle(finalPosition, 100, System.Drawing.Color.YellowGreen);
                }
            }
        }
    }
}
