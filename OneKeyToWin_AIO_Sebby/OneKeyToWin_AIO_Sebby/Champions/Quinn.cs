using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Quinn : Base
    {
        public Quinn()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            E = new Spell(SpellSlot.E, 700);
            W = new Spell(SpellSlot.W, 2100);
            R = new Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 90f, 1550, true, SkillshotType.SkillshotLine);
            E.SetTargetted(0.25f, 2000f);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("AGC", "AntiGapcloser E", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("Int", "Interrupter E", true).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("E Config").SubMenu("GapCloser").AddItem(new MenuItem("gap" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));
            HeroMenu.AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.AddItem(new MenuItem("autoR", "Auto R in shop", true).SetValue(true));

            HeroMenu.AddItem(new MenuItem("focusP", "Focus marked enemy", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmP", "Attack marked minion first", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "Farm Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += afterAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if(args.Target.Type == GameObjectType.obj_AI_Hero && MainMenu.Item("focusP", true).GetValue<bool>() && args.Target.HealthPercent > 40)
            {
                var orbTarget = args.Target as Obj_AI_Hero;
                if (!orbTarget.HasBuff("quinnw"))
                {
                    var best = HeroManager.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget() && Orbwalking.InAutoAttackRange(enemy) && enemy.HasBuff("quinnw"));
                    if(best != null)
                        Orbwalker.ForceTarget(best);
                }
            }
            else if(Program.LaneClear && args.Target.Type == GameObjectType.obj_AI_Minion && MainMenu.Item("farmP", true).GetValue<bool>())
            {
                var bestMinion = Cache.GetMinions(Player.Position, Player.AttackRange).FirstOrDefault(minion => minion.IsValidTarget() && Orbwalking.InAutoAttackRange(minion) && minion.HasBuff("quinnw"));

                if (bestMinion != null)
                    Orbwalker.ForceTarget(bestMinion);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(1))
                SetMana();
            if (Program.LagFree(2) && Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(4) && R.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                LogicR();
        }
        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 700, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (mob.HasBuff("QuinnW"))
                        return;

                    if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }

                    if (E.IsReady() && MainMenu.Item("jungleE", true).GetValue<bool>())
                    {
                        E.CastOnUnit(mob);
                        return;
                    }
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && MainMenu.Item("Int", true).GetValue<bool>() && sender.IsValidTarget(E.Range))
                E.CastOnUnit(sender);
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if ( target.Type == GameObjectType.obj_AI_Hero)
            {
                var t = target as Obj_AI_Hero;
                if (E.IsReady() && MainMenu.Item("autoE", true).GetValue<bool>() && t.IsValidTarget(E.Range) && t.CountEnemiesInRange(800) < 3)
                {
                    if (Program.Combo && Player.Mana > RMANA + EMANA)
                        E.Cast(t);
                    else if (Program.Harass && Player.Mana > RMANA + EMANA + QMANA + WMANA && MainMenu.Item("harassE", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && OktwCommon.CanHarras())
                    {
                        E.Cast(t);
                    }
                    else if (OktwCommon.GetKsDamage(t, E) > t.Health)
                        E.Cast(t);
                }
                if (Q.IsReady() && t.IsValidTarget(Q.Range))
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA)
                        Program.CastSpell(Q, t);
                    else if (Program.Harass && Player.Mana > RMANA + EMANA + QMANA + WMANA && MainMenu.Item("harassQ", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && OktwCommon.CanHarras())
                    {
                        Program.CastSpell(Q, t);
                    }
                    else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                        Program.CastSpell(Q, t);

                    if (!Program.None && Player.Mana > RMANA + QMANA + EMANA)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                            Q.Cast(enemy);
                    }
                }
            }
            Jungle();
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && MainMenu.Item("AGC", true).GetValue<bool>() && MainMenu.Item("gap" + gapcloser.Sender.ChampionName,true).GetValue<bool>())
            {
                var t = gapcloser.Sender;
                if (t.IsValidTarget(E.Range))
                {
                    E.Cast(t);
                }
            }
        }

        private void LogicR()
        {
            if (Player.InFountain() && R.Instance.Name == "QuinnR")
            {
                R.Cast();
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (Orbwalking.InAutoAttackRange(t) && t.HasBuff("quinnw"))
                    return;
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Harass && Player.Mana > RMANA + EMANA + QMANA + WMANA && MainMenu.Item("harassQ", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && OktwCommon.CanHarras())
                {
                    Program.CastSpell(Q, t);
                }
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Program.CastSpell(Q, t);

                if (!Program.None && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
            else if (FarmSpells && MainMenu.Item("farmQ", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range - 150);
                var farmPosition = Q.GetCircularFarmLocation(minionList, 150);
                if (farmPosition.MinionsHit >= FarmMinions)
                    Q.Cast(farmPosition.Position);
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
                RMANA = WMANA - Player.PARRegenRate * W.Instance.Cooldown;
            else
                RMANA = R.Instance.ManaCost;
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
        }
    }
}
