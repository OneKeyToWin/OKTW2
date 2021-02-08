using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Nunu : Base
    {
        private String nunuW = "nunuW";
        private String nunuE = "nunuesnowballfightbuff";
        private String nunuR = "nunurshield";
        public Nunu()
        {
            Q = new Spell(SpellSlot.Q, 125);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 690);
            R = new Spell(SpellSlot.R, 300);

            // Nunu swiftly throws a volley of 3 snowballs in the target direction over 0.4 seconds,
            // each snowball shattering upon hitting an enemy,
            // dealing magic damage to enemies in a cone.
            E.SetSkillshot(0.0f, 60f, float.MaxValue, false, SkillshotType.SkillshotLine);
            
            W.SetCharged(nunuW, nunuW, 600, 1510, 1.8f);
            R.SetCharged(nunuR, nunuR, 600, 600, 1.8f);

            DrawMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("qRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(Player.Position,
                            (float) MainMenu.Item("maxGrab", true).GetValue<Slider>().Value, System.Drawing.Color.Cyan, 1,
                            1);
                }
                else
                    Utility.DrawCircle(Player.Position, (float) MainMenu.Item("maxGrab", true).GetValue<Slider>().Value,
                        System.Drawing.Color.Cyan, 1, 1);
            }

            if (MainMenu.Item("wRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Gray, 1, 1);
            }

            if (MainMenu.Item("eRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Gray, 1, 1);
            }

            if (MainMenu.Item("rRange", true).GetValue<bool>())
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

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead || !CanCast())
                return;

            LogicQ();
            LogicW();
            LogicE();
            LogicR();
        }

        private void LogicQ()
        {
            if (Q.IsReady() && CanCast() )
            {
                foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly).OrderByDescending(min => min.HealthPercent))
                {
                    Q.Cast(minion);
                }
                
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
        }

        private void LogicW()
        {
            if (W.IsReady() && CanCast())
            {
                // W cancels slows
                if (Player.HasBuffOfType(BuffType.Slow))
                {
                    W.StartCharging(Player.ServerPosition.Extend(Game.CursorPos, 100));
                }
                else if (Program.Combo)
                {
                    W.StartCharging(Player.ServerPosition.Extend(Game.CursorPos, 100));
                }
            }
        }

        private void LogicE()
        {
            if (E.IsReady() && CanCast())
            {
                if (Program.LaneClear)
                {
                    foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range,
                        MinionTypes.All, MinionTeam.NotAlly))
                    {
                        E.Cast(minion.ServerPosition);
                    }
                }
                else if (Program.Combo)
                {
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                    if (target != null)
                    {
                        Program.CastSpell(E, target);
                    }
                }
            }
        }

        private void LogicR()
        {
            if (R.IsReady() && CanCast())
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range)))
                {
                    R.StartCharging();
                }
            }
        }

        private void DrawMainMenu()
        {
            MainMenu.SubMenu(Player.ChampionName).SubMenu("Draw")
                .AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            MainMenu.SubMenu(Player.ChampionName).SubMenu("Draw")
                .AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            MainMenu.SubMenu(Player.ChampionName).SubMenu("Draw")
                .AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            MainMenu.SubMenu(Player.ChampionName).SubMenu("Draw")
                .AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            MainMenu.SubMenu(Player.ChampionName).SubMenu("Draw")
                .AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));
        }

        private bool CanCast()
        {
            return !Player.HasBuff(nunuW) && !Player.HasBuff(nunuR);
        }
    }
}