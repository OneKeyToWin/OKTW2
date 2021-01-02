using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Xerath : Base
    {
        private Vector3 Rtarget;
        private float lastR = 0;

        private Items.Item
            FarsightOrb = new Items.Item(3342, 4000f),
            ScryingOrb = new Items.Item(3363, 3500f);

        public Xerath()
        {
            Q = new Spell(SpellSlot.Q, 1510);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 1050);
            R = new Spell(SpellSlot.R, 5000);

            Q.SetSkillshot(0.54f, 95f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.7f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 130f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 600, 1510, 1.8f);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification & line", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRangeMini", "R range minimap", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "harass Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R 2 x dmg R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoRlast", "Cast last position if no target", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("trinkiet", "Auto blue trinkiet", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("delayR", "custome R delay ms (1000ms = 1 sec)", true).SetValue(new Slider(0, 3000, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("MaxRangeR", "Max R adjustment (R range - slider)", true).SetValue(new Slider(0, 5000, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("separate", "Separate laneclear from harras", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));
           
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));
            
            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("force", "Force passive use in combo on minion", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene +=Drawing_OnEndScene;
            Orbwalking.BeforeAttack +=Orbwalking_BeforeAttack;
            Orbwalking.AfterAttack +=Orbwalking_AfterAttack;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
        }

        private void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (args.Order == GameObjectOrder.AttackUnit && Q.IsCharging)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                args.Process = false;
            }
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
                Orbwalker.ForceTarget(null);
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                if (Config.Item("trinkiet", true).GetValue<bool>() && !IsCastingR)
                {
                    if (Player.Level < 9)
                        ScryingOrb.Range = 2500;
                    else
                        ScryingOrb.Range = 3500;

                    if (ScryingOrb.IsReady())
                        ScryingOrb.Cast(Rtarget);
                    if(FarsightOrb.IsReady())
                        FarsightOrb.Cast(Rtarget);                  
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "xerathlocuspulse")
                {
                    lastR = Game.Time;
                }
            }
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target.IsValid<Obj_AI_Minion>() && !Player.HasBuff("xerathascended2onhit") && Program.Combo)
            {
                args.Process = false;
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.Distance(gapcloser.Sender.ServerPosition) < E.Range)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.Distance(sender.ServerPosition) < E.Range)
            {
                E.Cast(sender);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(3) && R.IsReady())
                LogicR();

            //Program.debug(""+OktwCommon.GetPassiveTime(Player, "XerathArcanopulseChargeUp"));
            if (IsCastingR || Player.IsChannelingImportantSpell())
            {
                OktwCommon.blockMove = true;
                OktwCommon.blockAttack = true;
                OktwCommon.blockAttack = true;
                OktwCommon.blockMove = true;
                Orbwalking.Attack = false;
                Orbwalking.Move = false;
                return;
            }
            else
            {
                OktwCommon.blockMove = false;
                OktwCommon.blockAttack = false;
                OktwCommon.blockAttack = false;
                OktwCommon.blockMove = false;
                Orbwalking.Attack = true;
                Orbwalking.Move = true;
            }

            if (Q.IsCharging && (int)(Game.Time * 10) % 2 == 0)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (Program.LagFree(1))
            {
                SetMana();
                Jungle();
                int[] mana = new int[] { 0, 30, 33, 36, 42, 48, 54, 63, 72, 81, 90, 102, 114, 126, 138, 150, 165, 180, 195 };
                if (!Player.HasBuff("xerathascended2onhit") || Player.Mana + mana[Player.Level] > Player.MaxMana)
                    Orbwalker.ForceTarget(null);
                else if ((Program.Combo || Program.Harass) && Config.Item("force", true).GetValue<bool>() && Orbwalker.GetTarget() == null)
                {
                    var minion = Cache.GetMinions(Player.ServerPosition, Player.AttackRange + Player.BoundingRadius * 2).OrderByDescending(x => x.Health).FirstOrDefault();

                    if(minion != null && OktwCommon.CanHarras())
                        Orbwalker.ForceTarget(minion);
                }
            }

            if (E.IsReady() && Config.Item("autoE", true).GetValue<bool>())
                LogicE();
            if (Program.LagFree(2) && W.IsReady() && !Player.IsWindingUp && Config.Item("autoW", true).GetValue<bool>())
                LogicW();
            if (Program.LagFree(4) && Q.IsReady() && !Player.IsWindingUp && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();
        }

        private void LogicR()
        {
            R.Range = 5000;
            if (!IsCastingR)
                R.Range = R.Range - Config.Item("MaxRangeR", true).GetValue<Slider>().Value;
            
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget() )
            {
                if (Config.Item("useR", true).GetValue<KeyBind>().Active && !IsCastingR)
                {
                    R.Cast();
                }
                if (!t.IsValidTarget(W.Range) && Config.Item("autoR", true).GetValue<bool>() && !IsCastingR && t.CountAlliesInRange(500) == 0 && Player.CountEnemiesInRange(1100) == 0)
                {
                    if (OktwCommon.GetKsDamage(t, R) + (R.GetDamage(t) * R.Level)  > t.Health)
                    {
                        R.Cast();
                    }
                }
                if (Game.Time - lastR > 0.001 * (float)Config.Item("delayR", true).GetValue<Slider>().Value && IsCastingR)
                {
                    Program.CastSpell(R, t);
                     
                }
                Rtarget = R.GetPrediction(t).CastPosition;
            }
            else if (Config.Item("autoRlast", true).GetValue<bool>() && Game.Time - lastR > 0.001 * (float)Config.Item("delayR", true).GetValue<Slider>().Value && IsCastingR)
            {
                R.Cast(Rtarget);
            } 
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                var qDmg = Q.GetDamage(t);
                var wDmg = OktwCommon.GetKsDamage(t, W);
                
                if (wDmg > t.Health)
                {
                    Program.CastSpell(W, t);
                }
                else if (wDmg + qDmg > t.Health && Player.Mana > WMANA  + QMANA)
                    Program.CastSpell(W, t);
                else if (Program.Combo && Player.Mana > RMANA + WMANA )
                    Program.CastSpell(W, t);
                else if (Program.Harass && OktwCommon.CanHarras() && Config.Item("harassW", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA)
                    Program.CastSpell(W, t);
                else if ((Program.Combo || Program.Harass))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
            else if (FarmSpells && Config.Item("farmW", true).GetValue<bool>() && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetCircularFarmLocation(minionList, W.Width);
                
                if (farmPosition.MinionsHit >= FarmMinions)
                    W.Cast(farmPosition.Position);
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var t2 = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
            
            if (t.IsValidTarget() && t2.IsValidTarget() && t == t2 && !(Config.Item("separate", true).GetValue<bool>() && Program.LaneClear))
            {
                if (Q.IsCharging)
                {
                    Program.CastSpell(Q, t);
                    if (OktwCommon.GetPassiveTime(Player, "XerathArcanopulseChargeUp") < 1 || (Player.CountEnemiesInRange(800) > 0) || Player.Distance(t) > 1450)
                        Q.Cast(Q.GetPrediction(t).CastPosition);
                    else if(OktwCommon.GetPassiveTime(Player, "XerathArcanopulseChargeUp") < 2 || (Player.CountEnemiesInRange(1000) > 0))
                        Q.CastIfHitchanceEquals(t, HitChance.VeryHigh);

                    return;
                }
                else if (t.IsValidTarget(Q.Range - 300))
                {
                    if(t.Health < OktwCommon.GetKsDamage(t, Q))
                        Q.StartCharging();
                    else if (Program.Combo && Player.Mana > EMANA + QMANA)
                    {
                        Q.StartCharging();
                    }
                    else if (Program.Harass &&  t.IsValidTarget(1200) &&  Config.Item("harassQ", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + QMANA + QMANA && Config.Item("Harass" + t.ChampionName).GetValue<bool>() && !Player.UnderTurret(true) && OktwCommon.CanHarras())
                    {
                        Q.StartCharging();
                    }
                    else if ((Program.Combo || Program.Harass) && Player.Mana > RMANA + WMANA)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                            Q.StartCharging();
                    }
                }
            }
            else if (Program.LaneClear && Q.Range > 1000 && Player.CountEnemiesInRange(1450) == 0 && (Q.IsCharging || (FarmSpells && Config.Item("farmQ", true).GetValue<bool>())))
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var Qfarm = Q.GetLineFarmLocation(allMinionsQ, Q.Width);
                if (Qfarm.MinionsHit >= FarmMinions || (Q.IsCharging && Qfarm.MinionsHit > 0))
                    Q.Cast(Qfarm.Position);
            }
        }

        private void LogicE()
        {
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && E.GetDamage(enemy) + OktwCommon.GetKsDamage(enemy, Q) + W.GetDamage(enemy) + OktwCommon.GetEchoLudenDamage(enemy) > enemy.Health))
            {
                Program.CastSpell(E, enemy);
            }
            var t = Orbwalker.GetTarget() as Obj_AI_Hero;
            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    Program.CastSpell(E, t);
                if (Program.Harass && OktwCommon.CanHarras() && Config.Item("harassE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(E, t);
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                    E.Cast(enemy);
            }
        }

        private bool IsCastingR
        {
            get
            {
                return Player.HasBuff("XerathLocusOfPower2") ||
                       (ObjectManager.Player.LastCastedSpellName() == "XerathLocusOfPower2" &&
                        Utils.TickCount - ObjectManager.Player.LastCastedSpellT() < 500);
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (E.IsReady() && Config.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                    if (W.IsReady() && Config.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    if (Q.IsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private void SetMana()
        {
            if ((Config.Item("manaDisable", true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20 || Q.IsCharging)
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
                RMANA = RMANA - (30 + Player.Level * 3 + Player.Level);
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (Config.Item("rRangeMini", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20,true);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20,true);
            }
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

            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }

            if (Config.Item("noti", true).GetValue<bool>() && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage * 3 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "3 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                    else if (rDamage * 2 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "2 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                    else if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "1 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                }
            }
        }
    }
}
