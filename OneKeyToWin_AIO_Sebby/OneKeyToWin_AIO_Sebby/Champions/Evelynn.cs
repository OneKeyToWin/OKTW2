using System;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Evelynn : Base
    {
        Spell Q2;

        bool isq2;

        public Evelynn()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            Q2 = new Spell(SpellSlot.Q, 500f);
            W = new Spell(SpellSlot.W, 1200f);
            E = new Spell(SpellSlot.E, 325f);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.3f, 60, 2400, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.35f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));

            HeroMenu.SubMenu("W config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W config").AddItem(new MenuItem("waitForW", "Wait for W", true).SetValue(true));

            HeroMenu.SubMenu("E config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));

            HeroMenu.SubMenu("R config").AddItem(new MenuItem("rCount", "Auto R x enemies", true).SetValue(new Slider(3, 0, 5)));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle E", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("laneQ", "Lane clear Q", true).SetValue(true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += GameOnOnUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (waitForW(args.Target as Obj_AI_Base))
            {
                args.Process = false;
            }
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            isq2 = Q.Instance.Name == "EvelynnQ2";

            if (MainMenu.Item("useR", true).GetValue<KeyBind>().Active)
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    R.CastIfWillHit(t, 2, true);
                    R.Cast(t, true, true);
                }
            }
            if (Program.Combo)
            {
                if (Program.LagFree(1) && Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                    LogicQ();
                if (Program.LagFree(2) && E.IsReady() && MainMenu.Item("autoE", true).GetValue<bool>())
                    LogicE();
                if (Program.LagFree(3) && W.IsReady())
                    LogicW();
                if (Program.LagFree(4) && R.IsReady())
                    LogicR();
            }
            else if (Program.LaneClear)
            {
                Jungle();
            }
        }

        private bool waitForW(Obj_AI_Base unit)
        {
            if (!MainMenu.Item("waitForW", true).GetValue<bool>())
                return false;

            if (unit == null)
                return false;

            var buff = unit.GetBuff("EvelynnW");

            return buff != null && buff.EndTime - Game.Time > 2.5f;
        }

        private void LogicQ()
        {
            Obj_AI_Hero t = null;

            if (!isq2)
                t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            else
                t = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Magical);

            if (t == null || waitForW(t))
                return;

            if (isq2)
                Q2.Cast();
            else
                Program.CastSpell(Q, t);
        }

        private void LogicE()
        {
           var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
           if (t.IsValidTarget() && !waitForW(t))
           {
               E.CastOnUnit(t);
           }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (t == null || (t.Position.UnderTurret(true) && !Player.Position.UnderTurret(true)))
                return;

            if (Q.IsReady())
            {
                if (isq2 && t.Distance(Player) < Q2.Range)
                    return;
                else if (!isq2 && t.Distance(Player) < 600)
                    return;
            }

            if (R.IsReady() && t.Health < R.GetDamage(t) && t.Distance(Player) < R.Range)
                return;

            if (E.IsReady() && t.Distance(Player) < E.Range)
                return;

            if (MainMenu.Item("autoW", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + QMANA)
                W.CastOnUnit(t);
        }

        private void LogicR()
        {
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                var poutput = R.GetPrediction(t, true);

                var aoeCount = poutput.AoeTargetsHitCount;

                aoeCount = (aoeCount == 0) ? 1 : aoeCount;

                if (MainMenu.Item("rCount", true).GetValue<Slider>().Value > 0 && MainMenu.Item("rCount", true).GetValue<Slider>().Value <= aoeCount)
                    R.Cast(poutput.CastPosition);

                if (t.Health < R.GetDamage(t) && 
                    OktwCommon.ValidUlt(t) && poutput.Hitchance >= HitChance.Medium)
                {
                    R.Cast(poutput.CastPosition);
                }

                if (Player.HealthPercent < 60)
                {
                    double dmg = OktwCommon.GetIncomingDamage(Player, 1);
                    var enemys = Player.CountEnemiesInRange(700);
                    if (dmg > 0 && (Player.Health - dmg) < (enemys * Player.Level * 15))
                        R.Cast(poutput.CastPosition);
                    else if ((Player.Health - dmg) < (Player.Level * 10) && dmg > 0)
                        R.Cast(poutput.CastPosition);
                }
            }      
        }

        private void Jungle()
        {
            if (Player.ManaPercent < MainMenu.Item("Mana", true).GetValue<Slider>().Value)
                return;
            var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (MainMenu.Item("jungleE", true).GetValue<bool>() && E.IsReady())
                    E.CastOnUnit(mob);
                if (MainMenu.Item("jungleQ", true).GetValue<bool>() && Q.IsReady())
                {
                    if (isq2)
                        Q.Cast();
                    else Q.Cast(mob.Position);
                }
            }

            if (MainMenu.Item("laneQ", true).GetValue<bool>() && Q.IsReady())
            {
                if (isq2)
                    Q.Cast();
                else
                {
                    var minion = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Enemy).MinOrDefault(x => x.Health);

                    if (minion != null)
                        Q.CastIfHitchanceEquals(minion, HitChance.Medium);
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("qRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (MainMenu.Item("wRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (MainMenu.Item("eRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (MainMenu.Item("rRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }

}
