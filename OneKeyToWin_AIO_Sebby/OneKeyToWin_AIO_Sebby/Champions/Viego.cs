using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Viego : Base
    {
        public bool attackNow = true;

        public Viego()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 500);

            Q.SetSkillshot(0.4f, 70f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0f, 70f, 1500f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.6f, 270f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            W.SetCharged("ViegoW", "ViegoW", 400, 850, 1f);

            HeroMenu.SubMenu("Q Config")
                .AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config")
                .AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            HeroMenu.SubMenu("W Config")
                .AddItem(new MenuItem("autoQ", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W Config")
                .AddItem(new MenuItem("harassQ", "Harass W", true).SetValue(true));

            HeroMenu.SubMenu("W config")
                .AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W config")
                .AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu
                .AddItem(new MenuItem("PassiveCast", "Passive cast spells", true).SetValue(true));

            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("ComboInfo", "R killable info", true).SetValue(true));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            HeroMenu.AddItem(new MenuItem("sheen", "Sheen logic", true).SetValue(true));
            HeroMenu
                .AddItem(new MenuItem("AApriority", "AA priority over spell", true).SetValue(true));

            HeroMenu.SubMenu("Farm")
                .AddItem(new MenuItem("farmW", "LaneClear W", true).SetValue(true));
            HeroMenu.SubMenu("Farm")
                .AddItem(new MenuItem("farmE", "LaneClear E", true).SetValue(true));

            HeroMenu.SubMenu("Farm")
                .AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm")
                .AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));


            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += afterAttack;
            Obj_AI_Base.OnBuffAdd += OnBuffAdd;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            //if (sender.IsMe)
            //{

            //    Console.WriteLine("buffname: " + args.Buff.Name);
            //}
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            //if (Config.Item("AGC", true).GetValue<bool>() && E.IsReady() && Player.Mana > RMANA + EMANA)
            //{
            //    var Target = (Obj_AI_Hero)gapcloser.Sender;
            //    if (Target.IsValidTarget(E.Range))
            //    {
            //        E.Cast(Target, true);
            //        Program.debug("E AGC");
            //    }
            //}
            //return;
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            //attackNow = true;
            //if (Program.LaneClear && W.IsReady() && FarmSpells)
            //{
            //    var minions = Cache.GetMinions(Player.ServerPosition, 650);

            //    if (minions.Count >= FarmMinions)
            //    {
            //        if (Config.Item("farmW", true).GetValue<bool>() && minions.Count > 1)
            //            W.Cast();
            //    }
            //}
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            //attackNow = false;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            var viegosoul = ObjectManager.Get<Obj_AI_Minion>()
                .FirstOrDefault(minion => minion.Team == GameObjectTeam.Neutral
                                          && minion.CharData.BaseSkinName == "ViegoSoul" && minion.IsHPBarRendered
                                          && minion.IsValidTarget(600));
            if (viegosoul != null && (Player.ServerPosition.CountEnemiesInRange(1000) == 0 ||
                                      Player.HealthPercent < 25))
            {
                Orbwalker.SetOrbwalkingPoint(viegosoul.Position);
                Orbwalker.ForceTarget(viegosoul);
            }
            else
            {
                Orbwalker.SetOrbwalkingPoint(new SharpDX.Vector3());
                Orbwalker.ForceTarget(null);
            }

            if (!Player.CharData.BaseSkinName.Contains("Viego"))
            {
                if (!Program.LagFree(0))
                    return;
                var t = TargetSelector.GetTarget(R.Range + R.Width, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var pred = Prediction.GetPrediction(t, 0.4f).CastPosition;
                    if (Player.InventoryItems.Count() == 2) // empty
                    {
                        Program.CastSpell(R, t);
                    }
                    else if (MainMenu.Item("PassiveCast", true).GetValue<bool>())
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            var spell = Player.Spellbook.Spells[i];
                            if (spell.SData != null && ObjectManager.Player.Spellbook.CanUseSpell(spell.Slot) ==
                                SpellState.Ready)
                            {
                                if (spell.SData.TargettingType == SpellDataTargetType.Unit ||
                                    spell.SData.TargettingType == SpellDataTargetType.SelfAndUnit)
                                {
                                    ObjectManager.Player.Spellbook.CastSpell(spell.Slot, t, true);
                                }
                                else if (spell.SData.TargettingType != SpellDataTargetType.Self &&
                                         spell.SData.TargettingType != SpellDataTargetType.SelfAoe)
                                {
                                    if (spell.SData.LineWidth > 0 || spell.SData.LineWidth > 0)
                                    {
                                        ObjectManager.Player.Spellbook.CastSpell(spell.Slot, pred, true);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Player.InventoryItems.Count() == 2) // empty
                    {
                        R.Cast(Game.CursorPos);
                    }
                }

                return;
            }


            if (Program.LagFree(0))
            {
                Jungle();
            }

            if (!Orbwalking.CanMove(50))
                return;

            if (Program.LagFree(2) && Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(4) && R.IsReady())
                LogicR();

            if (Program.LagFree(3) && W.IsReady() && MainMenu.Item("autoW", true).GetValue<bool>())
                LogicW();
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                var qDmg = OktwCommon.GetKsDamage(t, Q);
                var eDmg = E.GetDamage(t);
                if (t.IsValidTarget(W.Range) && qDmg + eDmg > t.Health)
                    Program.CastSpell(Q, t);
                else if (Program.Combo)
                    Program.CastSpell(Q, t);
                else if ((Program.Harass) && MainMenu.Item("harassQ", true).GetValue<bool>() && !Player.UnderTurret(true))
                    Program.CastSpell(Q, t);
                else if ((Program.Combo || Program.Harass))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy =>
                        enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true);
                }
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.ChargedMaxRange, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (!Player.HasBuff("ViegoW") && Program.Combo)
                {
                    var prediction = W.GetPrediction(t);

                    if (prediction.Hitchance >= HitChance.Low && prediction.Hitchance <= HitChance.VeryHigh)
                    {
                        W.StartCharging();
                    }

                    return;
                }

                var wDmg = OktwCommon.GetKsDamage(t, W);
                if (wDmg + Q.GetDamage(t) > t.Health)
                {
                    Program.CastSpell(W, t);
                }
                else if (Program.Combo)
                {
                    Program.CastSpell(W, t);
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 650, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (E.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob);
                        return;
                    }
                    else if (W.IsReady() && MainMenu.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob);
                        return;
                    }
                }
            }
        }

        private void LogicR()
        {
            if (MainMenu.Item("autoR", true).GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(target =>
                    target.IsValidTarget(R.Range + R.Width) && OktwCommon.ValidUlt(target)))
                {
                    var dmgR = OktwCommon.GetKsDamage(target, R, true) + 3 * Player.GetAutoAttackDamage(target);

                    if (dmgR > target.Health)
                    {
                        Program.CastSpell(R, target);
                    }
                }
            }
        }

        private void drawText(string msg, Obj_AI_Hero Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1], color, msg);
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