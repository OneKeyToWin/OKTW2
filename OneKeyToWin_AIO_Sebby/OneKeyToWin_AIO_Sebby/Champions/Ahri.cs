using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Ahri : Base
    {
        private static GameObject QMissile = null, EMissile = null;
        public Obj_AI_Hero Qtarget = null;
        public static Core.MissileReturn missileManager;

        public Ahri()
        {
            Q = new Spell(SpellSlot.Q, 870);
            W = new Spell(SpellSlot.W, 580);
            E = new Spell(SpellSlot.E, 965);
            R = new Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0.25f, 90, 1700, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60, 1550, true, SkillshotType.SkillshotLine);

            missileManager = new Core.MissileReturn("AhriOrbMissile", "AhriOrbReturn", Q);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification & line", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("Qhelp", "Show Q helper", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("aimQ", "Auto aim Q missile", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("Use E on").AddItem(new MenuItem("Eon" + enemy.ChampionName, enemy.ChampionName ,true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("Gapcloser").AddItem(new MenuItem("Egapcloser" + enemy.ChampionName, enemy.ChampionName ,true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "R KS ", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR2", "auto R fight logic + aim Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += SpellMissile_OnCreateOld;
            GameObject.OnDelete += Obj_SpellMissile_OnDelete;
        }

        private void Obj_SpellMissile_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid<MissileClient>())
                return;

            MissileClient missile = (MissileClient)sender;

            if ( missile.SData.Name != null)
            {
                if(missile.SData.Name == "AhriOrbReturn")
                    QMissile = null;
                if (missile.SData.Name == "AhriSeduceMissile")
                    EMissile = null;
            }
        }

        private void SpellMissile_OnCreateOld(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid<MissileClient>())
                return;

            MissileClient missile = (MissileClient)sender;

            if (missile.SData.Name != null )
            {
                if (missile.SData.Name == "AhriOrbMissile" || missile.SData.Name == "AhriOrbReturn")
                {
                    QMissile = sender;
                }
                if (missile.SData.Name == "AhriSeduceMissile")
                {
                    EMissile = sender;
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range) && Config.Item("Egapcloser" + gapcloser.Sender.ChampionName, true).GetValue<bool>())
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && Player.Distance(sender.ServerPosition) < E.Range)
            {
                E.Cast(sender);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }
            
            if (E.IsReady() && Config.Item("autoE", true).GetValue<bool>())
                LogicE();
            if (Program.LagFree(2) && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
                LogicW();
            if (Program.LagFree(3) && Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();
            if (Program.LagFree(4) && R.IsReady() && Program.Combo)
                LogicR();
        }

        private void LogicR()
        {
            var dashPosition = Player.Position.Extend(Game.CursorPos, 450);

            if (Player.Distance(Game.CursorPos) < 450)
                dashPosition = Game.CursorPos;

            if (dashPosition.CountEnemiesInRange(800) > 2)
                return;

            if (Config.Item("autoR2", true).GetValue<bool>())
            {
                if (Player.HasBuff("AhriTumble"))
                {
                    var BuffTime = OktwCommon.GetPassiveTime(Player, "AhriTumble");
                    if (BuffTime < 3)
                    {
                        R.Cast(dashPosition);
                    }

                    var posPred = missileManager.CalculateReturnPos();
                    if (posPred != Vector3.Zero)
                    {

                        if (missileManager.Missile.SData.Name == "AhriOrbReturn" && Player.Distance(posPred) > 200)
                        {
                            R.Cast(posPred);
                            Program.debug("AIMMMM");
                        }
                    }
                }
            }

            if (Config.Item("autoR", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(450 + R.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    var comboDmg = R.GetDamage(t) * 3;
                    if (Q.IsReady())
                    {
                        comboDmg += Q.GetDamage(t) * 2;
                    }
                    if (W.IsReady())
                    {
                        comboDmg += W.GetDamage(t) + W.GetDamage(t, 1);
                    }
                    if (t.CountAlliesInRange(600) < 2 && comboDmg > t.Health && t.Position.Distance(Game.CursorPos) < t.Position.Distance(Player.Position) && dashPosition.Distance(t.ServerPosition) < 500)
                    {
                        R.Cast(dashPosition);
                    }

                    foreach (var target in HeroManager.Enemies.Where(target => target.IsMelee && target.IsValidTarget(300)))
                    {
                        R.Cast(dashPosition);
                    }
                }
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA)
                    W.Cast();
                else if (Program.Harass && Player.Mana > RMANA + QMANA + WMANA && Config.Item("harassW", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>())
                    W.Cast();
                else if (W.GetDamage(t) + W.GetDamage(t, 1) + Q.GetDamage(t) * 2 > t.Health - OktwCommon.GetIncomingDamage(t))
                    W.Cast();
            }
            else if (FarmSpells && QMissile == null && Config.Item("farmW", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range, MinionTeam.Enemy);
                if (minionList.Count >= FarmMinions && minionList.Any(minion => minion.Health < W.GetDamage(minion)))
                     W.Cast();
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                missileManager.Target = t;
                if (EMissile == null || !EMissile.IsValid)
                {
                    
                    if (Program.Combo && Player.Mana > RMANA + QMANA)
                        Program.CastSpell(Q, t);
                    else if (Program.Harass  && Player.Mana > RMANA + WMANA + QMANA + QMANA && Config.Item("harassQ", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>() && OktwCommon.CanHarras())
                        Program.CastSpell(Q, t);
                    else if (Q.GetDamage(t) * 2 + OktwCommon.GetEchoLudenDamage(t) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Q.Cast(t, true);
                }
                if (!Program.None && Player.Mana > RMANA + WMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true);
                }
            }
            else if (FarmSpells && Config.Item("farmQ", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit >= FarmMinions)
                    Q.Cast(farmPosition.Position);
            }
        }

        private void LogicE()
        {
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && OktwCommon.GetKsDamage(enemy,E) + Q.GetDamage(enemy) + W.GetDamage(enemy) > enemy.Health))
                Program.CastSpell(E, enemy);
            
            var t = Orbwalker.GetTarget() as Obj_AI_Hero;
            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget() )
            {
                if (Program.Combo && Player.Mana > RMANA + EMANA && Config.Item("Eon" + t.ChampionName, true).GetValue<bool>())
                    Program.CastSpell(E, t);
                else if (Program.Harass && Player.Mana > RMANA + EMANA + WMANA + EMANA && Config.Item("harassE", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>() )
                    Program.CastSpell(E, t);
                if (!Program.None && Player.Mana > RMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy) && Config.Item("Eon" + enemy.ChampionName, true).GetValue<bool>()))
                        E.Cast(enemy);
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > QMANA + RMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && Config.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast();
                        return;
                    }
                    if (Q.IsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob.Position);
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

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
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
                        Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
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

            if (Config.Item("noti", true).GetValue<bool>())
            {

                var t = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var comboDmg = 0f;
                    if (R.IsReady())
                    {
                        comboDmg += R.GetDamage(t) * 3;
                    }
                    if (Q.IsReady())
                    {
                        comboDmg += Q.GetDamage(t) * 2;
                    }
                    if (W.IsReady())
                    {
                        comboDmg += W.GetDamage(t) + W.GetDamage(t, 1);
                    }
                    if (comboDmg > t.Health)
                    {

                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "COMBO KILL " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                }
            }
        }
    }
}
