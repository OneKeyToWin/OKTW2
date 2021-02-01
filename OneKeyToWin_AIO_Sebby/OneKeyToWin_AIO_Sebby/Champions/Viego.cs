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

            W.SetCharged("ViegoW", "ViegoW", 400,850, 1f);

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoQ", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassQ", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));


            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("ComboInfo", "R killable info", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("sheen", "Sheen logic", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("AApriority", "AA priority over spell", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "LaneClear W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmE", "LaneClear E", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));


            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += afterAttack;
            Obj_AI_Base.OnBuffAdd += OnBuffAdd;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            if(sender.IsMe)
            {

                Console.WriteLine("buffname: " +    args.Buff.Name);
            }
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
            if (Player.ChampionName != "Viego")
            {
               if(Player.InventoryItems.Count() == 2)
                {
                    Console.WriteLine("EMPTY");
                }

                return;
            }

           
          
            {
                
            }
            var viegosoul = ObjectManager.Get<Obj_AI_Minion>().Where(
                                minion =>
                                minion.Team == GameObjectTeam.Neutral
                                && minion.CharData.BaseSkinName == "ViegoSoul" && minion.IsHPBarRendered
                                && minion.IsValidTarget(600)).FirstOrDefault();
            if(viegosoul != null)
            {
                //Console.WriteLine("VIEGO SOUL");
                Orbwalker.SetOrbwalkingPoint(viegosoul.Position);
                Orbwalker.ForceTarget(viegosoul);
            }
            else
            {
                Orbwalker.SetOrbwalkingPoint(new SharpDX.Vector3());
                Orbwalker.ForceTarget(null);
            }


            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }
            //viegopassivecasting
            //buffname: viegopassivetransform
            //Barrel
            if (Program.Combo)
            {

                //Console.WriteLine("viegopassivecasting " + Player.HasBuff("viegopassivecasting") + " viegopassivetransform " + Player.HasBuff("viegopassivetransform"));
               
            }

            //if (Program.LagFree(1) && E.IsReady() && Config.Item("autoE", true).GetValue<bool>())
            //    LogicE();

            if (!Orbwalking.CanMove(50))
                return;

            if (Program.LagFree(2) && Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(4) && R.IsReady())
                LogicR();

            if (Program.LagFree(3) && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
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
                else if (Program.Combo && Player.Mana > RMANA + QMANA * 2 + EMANA)
                    Program.CastSpell(Q, t);
                else if ((Program.Harass && Player.Mana > RMANA + EMANA + QMANA * 2 + WMANA) && Config.Item("harassQ", true).GetValue<bool>() && !Player.UnderTurret(true))
                    Program.CastSpell(Q, t);
                else if ((Program.Combo || Program.Harass) && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true);

                }
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                var wDmg = OktwCommon.GetKsDamage(t, W);
                if (wDmg + Q.GetDamage(t) > t.Health )
                {
                    Program.CastSpell(W, t);
                }
                else if (Program.Combo && Player.Mana > RMANA + WMANA )
                {
                    Program.CastSpell(W, t);
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 650, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (E.IsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob);
                        return;
                    }
                    else if (W.IsReady() && Config.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob);
                        return;
                    }

                }
            }
        }

        private void LogicR()
        {
            if (Config.Item("autoR", true).GetValue<bool>() )
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range + R.Width) && OktwCommon.ValidUlt(target)))
                {
                    var dmgR = OktwCommon.GetKsDamage(target, R, true) + 3 * Player.GetAutoAttackDamage(target);

                    if (dmgR > target.Health)
                    {
                        Program.CastSpell(R, target);
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

        private void drawText(string msg, Obj_AI_Hero Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1], color, msg);
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
