using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SebbyLib;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Anivia : Base
    {
        private float RCastTime = 0;
        private int Rwidth = 400;
        private static GameObject QMissile, RMissile;
        private static int QMissileCreateTick;
        private static SharpDX.Vector3 StartPosition, EndPosition;

        public Anivia()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 685);

            Q.SetSkillshot(0.25f, 110f, 870f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.6f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("AGCQ", "Q gapcloser", true).SetValue(false));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("AGCW", "AntiGapcloser W", true).SetValue(false));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("inter", "OnPossibleToInterrupt W", true)).SetValue(true);

            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(false));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmR", "Lane clear R", true).SetValue(false));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleR", "Jungle clear R", true).SetValue(true));


            HeroMenu.AddItem(new MenuItem("AACombo", "Disable AA if can use E", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (MainMenu.Item("inter", true).GetValue<bool>() && W.IsReady() && sender.IsValidTarget(W.Range))
                W.Cast(sender);
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var Target = gapcloser.Sender;
            if (Q.IsReady() && Q.Instance.ToggleState == 1 && MainMenu.Item("AGCQ", true).GetValue<bool>())
            {
                if (Target.IsValidTarget(300))
                {
                    Q.Cast(Target);
                    Program.debug("AGC Q");
                }
            }
            else if (W.IsReady() && MainMenu.Item("AGCW", true).GetValue<bool>())
            {
                if (Target.IsValidTarget(W.Range))
                {
                    W.Cast(ObjectManager.Player.Position.Extend(Target.Position, 50), true);
                }
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                if (obj.Name == "FlashFrostSpell")
                {
                    var missile = obj as MissileClient;

                    if (missile != null)
                    {
                        QMissile = obj;
                        QMissileCreateTick = Utils.TickCount + 25;
                        StartPosition = missile.Position;
                        EndPosition = missile.EndPosition;
                    }
                }
                if (obj.Name.StartsWith("Anivia") && obj.Name.EndsWith("R_indicator_ring"))
                {
                    RMissile = obj;
                    Program.debug("dupa");
                }
            }
        }

        private SharpDX.Vector3 MissilePosition()
        {
            if (QMissile != null)
            {
                return StartPosition.Extend(EndPosition, (Utils.TickCount - QMissileCreateTick) / 1000f * Q.Speed);
            }

            return SharpDX.Vector3.Zero;
        }

        private void Obj_AI_Base_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                if (obj.Name == "FlashFrostSpell")
                {
                    QMissile = null;
                    QMissileCreateTick = 0;
                    StartPosition = SharpDX.Vector3.Zero;
                    EndPosition = SharpDX.Vector3.Zero;
                }

                if (obj.Name.StartsWith("Anivia") && obj.Name.EndsWith("R_indicator_ring"))
                    RMissile = null;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.Combo && MainMenu.Item("AACombo", true).GetValue<bool>())
            {
                if (!E.IsReady())
                    Orbwalking.Attack = true;

                else
                    Orbwalking.Attack = false;
            }
            else
                Orbwalking.Attack = true;

            if (Q.IsReady() && Q.Instance.ToggleState == 2 && QMissile != null && MissilePosition().CountEnemiesInRange(230) > 0)
                Q.Cast();
            
            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (Program.LagFree(1) && R.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                LogicR();

            if (Program.LagFree(2) && W.IsReady() && MainMenu.Item("autoW", true).GetValue<bool>())
                LogicW();

            if (Program.LagFree(3) && Q.IsReady() && QMissile == null && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(4) )
            {
                if(E.IsReady() && MainMenu.Item("autoE", true).GetValue<bool>())
                    LogicE();

                Jungle();
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget() && Q.Instance.ToggleState == 1)
            {
                if (Program.Combo && Player.Mana > EMANA + QMANA - 10)
                    Program.CastSpell(Q, t);
                else if (Program.Harass && MainMenu.Item("harassQ", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                {
                    Program.CastSpell(Q, t);
                }
                else
                {
                    var qDmg = OktwCommon.GetKsDamage(t,Q);
                    var eDmg = E.GetDamage(t);
                    if (qDmg > t.Health)
                        Program.CastSpell(Q, t);
                    else if (qDmg + eDmg > t.Health && Player.Mana > QMANA + WMANA)
                        Program.CastSpell(Q, t);
                }
                if (!Program.None && Player.Mana > RMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                    {
                        Q.Cast(enemy, true);
                    }
                }
            }
        }

        private void LogicW()
        {
            if (Program.Combo && Player.Mana > RMANA + EMANA + WMANA)
            {
                var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget(W.Range) && W.GetPrediction(t).CastPosition.Distance(t.Position) > 100)
                {
                    if (Player.Position.Distance(t.ServerPosition) > Player.Position.Distance(t.Position))
                    {
                        if (t.Position.Distance(Player.ServerPosition) < t.Position.Distance(Player.Position))
                            Program.CastSpell(W, t);
                    }
                    else
                    {
                        if (t.Position.Distance(Player.ServerPosition) > t.Position.Distance(Player.Position) && t.Distance(Player) < R.Range)
                            Program.CastSpell(W, t);
                    }
                }
            }
        }
        
        private void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                var qCd = Q.Instance.CooldownExpires - Game.Time;
                var rCd = R.Instance.CooldownExpires - Game.Time;
                if (Player.Level < 7)
                    rCd = 10;
                //debug("Q " + qCd + "R " + rCd + "E now " + E.Instance.Cooldown);
                var eDmg = OktwCommon.GetKsDamage(t, E);

                if (eDmg > t.Health)
                    E.Cast(t, true);
                
                if (t.HasBuff("chilled") || qCd > E.Instance.Cooldown - 1 && rCd > E.Instance.Cooldown - 1)
                {
                    if (eDmg * 3 > t.Health)
                        E.Cast(t, true);
                    else if (Program.Combo && (t.HasBuff("chilled") || Player.Mana > RMANA + EMANA))
                    {
                        E.Cast(t, true);
                    }
                    else if ( Program.Harass && Player.Mana > RMANA + EMANA + QMANA + WMANA && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && !Player.UnderTurret(true) && QMissile == null)
                    {
                        E.Cast(t, true);
                    }
                }
                else if (Program.Combo && R.IsReady() && Player.Mana > RMANA + EMANA && QMissile == null )
                {
                    R.Cast(t, true, true);
                }
            }
            farmE();
        }

        private void farmE()
        {
            if (FarmSpells && MainMenu.Item("farmE", true).GetValue<bool>() && !Orbwalking.CanAttack())
            {
                var minions = Cache.GetMinions(Player.ServerPosition, E.Range);
                foreach (var minion in minions.Where(minion => minion.Health > Player.GetAutoAttackDamage(minion)))
                {
                    var eDmg = E.GetDamage(minion) * 2;
                    if (minion.Health < eDmg && minion.HasBuff("chilled"))
                        E.Cast(minion);
                }
            }
        }

        private void LogicR()
        {
            if (RMissile == null)
            {
                var t = TargetSelector.GetTarget(R.Range + 400, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (R.GetDamage(t) > t.Health)
                        R.Cast(t, true, true);
                    else if (Player.Mana > RMANA + EMANA && E.GetDamage(t) * 2 + R.GetDamage(t) > t.Health)
                        R.Cast(t, true, true);
                    if (Player.Mana > RMANA + EMANA + QMANA + WMANA && Program.Combo)
                        R.Cast(t, true, true);
                }
                if (FarmSpells && MainMenu.Item("farmR", true).GetValue<bool>())
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, R.Range);
                    var farmPos = R.GetCircularFarmLocation(allMinions, Rwidth);
                    if (farmPos.MinionsHit >= FarmMinions)
                        R.Cast(farmPos.Position);
                }
            }
            else
            {
                if (FarmSpells && MainMenu.Item("farmR", true).GetValue<bool>())
                {
                    var allMinions = Cache.GetMinions(RMissile.Position, Rwidth);
                    var mobs = Cache.GetMinions(RMissile.Position, Rwidth, MinionTeam.Neutral);
                    if (mobs.Count > 0)
                    {
                        if (!MainMenu.Item("jungleR", true).GetValue<bool>())
                        {
                            R.Cast();
                        }
                    }
                    else if (allMinions.Count > 0)
                    {
                        if (allMinions.Count < 2 || Player.ManaPercent < MainMenu.Item("Mana", true).GetValue<Slider>().Value)
                            R.Cast();
                        else if (Player.ManaPercent < MainMenu.Item("Mana", true).GetValue<Slider>().Value)
                            R.Cast();
                    }
                    else
                        R.Cast();

                }
                else if (!Program.None &&(RMissile.Position.CountEnemiesInRange(470) == 0 || Player.Mana < EMANA + QMANA))
                {
                    R.Cast();
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        if (QMissile != null)
                        {
                            if (Q.Instance.ToggleState == 2 && MissilePosition().Distance(mob.ServerPosition) < 230)
                                Q.Cast();
                        }
                        else if (Q.Instance.ToggleState == 1)
                        {
                            Q.Cast(mob.ServerPosition);
                        }
                        
                        return;
                    }
                    if (R.IsReady() && MainMenu.Item("jungleR", true).GetValue<bool>() && RMissile == null)
                    {
                        R.Cast(mob.ServerPosition);
                        return;
                    }
                    if (E.IsReady() && MainMenu.Item("jungleE", true).GetValue<bool>() && mob.HasBuff("chilled"))
                    {
                        E.Cast(mob);
                        return;
                    }
                    if (W.IsReady() && MainMenu.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob.Position.Extend(Player.Position , 100));
                        return;
                    }
                }
            }
        }

        private void SetMana()
        {
            if ((MainMenu.Item("manaDisable", true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20 )
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
