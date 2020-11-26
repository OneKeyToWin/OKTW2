using System;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Kindred : Base
    {
        public static Core.OKTWdash Dash;

        public Kindred()
        {
            Q = new Spell(SpellSlot.Q, 340);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 500);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Dash = new Core.OKTWdash(Q);

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("Use on:").AddItem(new MenuItem("Euse" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Renemy", "Don't R if x enemies", true).SetValue(new Slider(4, 5, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalker_AfterAttack;
        }


        public void Orbwalker_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target.Type != GameObjectType.obj_AI_Hero)
                return;

            if (Program.Combo && Player.Mana > RMANA + QMANA && Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
            {
                var t = target as Obj_AI_Hero;
                if (t.IsValidTarget())
                {
                    var dashPos = Dash.CastDash();
                    if (!dashPos.IsZero && dashPos.CountEnemiesInRange(500) > 0)
                    {
                        Q.Cast(dashPos);
                    }
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && Config.Item("autoE", true).GetValue<bool>())
                LogicE();

            if (Program.LagFree(2) && W.IsReady() && Config.Item("autoW", true).GetValue<bool>() )
                LogicW();

            if (Program.LagFree(3) && Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (R.IsReady() && Config.Item("autoR", true).GetValue<bool>())
                LogicR();
        }

        private void LogicQ()
        {
            if (Program.Combo && Player.Mana > RMANA + QMANA)
            {
                if (Orbwalker.GetTarget() != null)
                    return;
                var dashPos = Dash.CastDash();
                if (!dashPos.IsZero && dashPos.CountEnemiesInRange(500) > 0)
                {
                    Q.Cast(dashPos);
                }
            }
            if (FarmSpells && Config.Item("farmQ", true).GetValue<bool>())
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, 400);
                if (allMinionsQ.Count >= FarmMinions)
                    Q.Cast(Game.CursorPos);
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(650, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget() && !Q.IsReady())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA)
                    W.Cast();
                else if (Program.Harass && Config.Item("harassW", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + EMANA && Config.Item("Harass" + t.ChampionName).GetValue<bool>())
                    W.Cast();
            }
            var tks = TargetSelector.GetTarget(1600, TargetSelector.DamageType.Physical);
            if (tks.IsValidTarget())
            {
                if (W.GetDamage(tks) * 3 > tks.Health - OktwCommon.GetIncomingDamage(tks))
                    W.Cast();
            }

            if (FarmSpells && Config.Item("farmW", true).GetValue<bool>())
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, 600);
                if (allMinionsQ.Count >= FarmMinions)
                    W.Cast();
            }
        }

        private void LogicE()
        {
            var torb = Orbwalker.GetTarget();
            if (torb == null || torb.Type != GameObjectType.obj_AI_Hero)
                return;
            else
            {
                var t = torb as Obj_AI_Hero;

                if (t.IsValidTarget(E.Range))
                {
                    if (!Config.Item("Euse" + t.ChampionName, true).GetValue<bool>())
                        return;
                    if (Program.Combo && Player.Mana > RMANA + EMANA)
                        E.CastOnUnit(t);
                    else if (Program.Harass && Config.Item("harassE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + EMANA && Config.Item("Harass" + t.ChampionName).GetValue<bool>())
                        E.CastOnUnit(t);
                }
            }
        }

        private void LogicR()
        {
            var rEnemy = Config.Item("Renemy", true).GetValue<Slider>().Value;

            double dmg = OktwCommon.GetIncomingDamage(Player);

            if (dmg == 0 )
                return;

            if (Player.Health - dmg <  Player.Level * 10 && Player.CountEnemiesInRange(500) < rEnemy)
                R.Cast(Player);
            
        }

        private void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (E.IsReady() && Config.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast(mob);
                        return;
                    }
                    if (Q.IsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(Game.CursorPos);
                        return;
                    }
                    if (W.IsReady() && Config.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast();
                        return;
                    }
                }
            }
        }

        private void SetMana()
        {
            if ((Config.Item("manaDisable", true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20)
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

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
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
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
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
