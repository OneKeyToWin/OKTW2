using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class KogMaw : Base
    {
        public bool attackNow = true;

        public KogMaw()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 1800);

            Q.SetSkillshot(0.25f, 70f, 1650f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 125f, 1280f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.2f, 175f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("AGC", "AntiGapcloserE", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W config").AddItem(new MenuItem("harassW", "Harass W on max range", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("RmaxHp", "Target max % HP", true).SetValue(new Slider(50, 100, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("comboStack", "Max combo stack R", true).SetValue(new Slider(2, 10, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("harasStack", "Max haras stack R", true).SetValue(new Slider(1, 10, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("Rcc", "R cc", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("Rslow", "R slow", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("Raoe", "R aoe", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("Raa", "R only out off AA range", true).SetValue(false));

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
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += afterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("AGC", true).GetValue<bool>() && E.IsReady() && Player.Mana > RMANA + EMANA)
            {
                var Target = (Obj_AI_Hero)gapcloser.Sender;
                if (Target.IsValidTarget(E.Range))
                {
                    E.Cast(Target, true);
                    Program.debug("E AGC");
                }
            }
            return;
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            attackNow = true;
            if (Program.LaneClear && W.IsReady() && FarmSpells)
            {
                var minions = Cache.GetMinions(Player.ServerPosition, 650);

                if (minions.Count >= FarmMinions)
                {
                    if (Config.Item("farmW", true).GetValue<bool>() && minions.Count > 1)
                        W.Cast();
                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            attackNow = false;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                R.Range = 870 + 300 * Player.Spellbook.GetSpell(SpellSlot.R).Level;
                W.Range = 650 + 30 * Player.Spellbook.GetSpell(SpellSlot.W).Level;
                SetMana();
                Jungle();

            }
            if (Program.LagFree(1) && E.IsReady() && !Player.IsWindingUp && Config.Item("autoE", true).GetValue<bool>())
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && !Player.IsWindingUp && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
                LogicW();

            if (Program.LagFree(4) && R.IsReady() && !Player.IsWindingUp)
                LogicR();
            
        }
        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 650, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (E.IsReady() && Config.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                    else if (W.IsReady() && Config.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast();
                        return;
                    }
                    
                }
            }
        }

        private void LogicR()
        {
            if (Config.Item("autoR", true).GetValue<bool>() && Sheen())
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                if (target.IsValidTarget(R.Range) && target.HealthPercent < Config.Item("RmaxHp", true).GetValue<Slider>().Value && OktwCommon.ValidUlt(target))
                {
                    

                    if (Config.Item("Raa", true).GetValue<bool>() && Orbwalking.InAutoAttackRange(target))
                        return;

                    var harasStack = Config.Item("harasStack", true).GetValue<Slider>().Value;
                    var comboStack = Config.Item("comboStack", true).GetValue<Slider>().Value;
                    var countR = GetRStacks();

                    var Rdmg = R.GetDamage(target);
                    Rdmg = Rdmg + target.CountAlliesInRange(500) * Rdmg;

                    if (R.GetDamage(target) > target.Health - OktwCommon.GetIncomingDamage(target))
                        Program.CastSpell(R, target);
                    else if (Program.Combo && Rdmg * 2 > target.Health && Player.Mana > RMANA * 3)
                        Program.CastSpell(R, target);
                    else if (countR < comboStack + 2 && Player.Mana > RMANA * 3)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && !OktwCommon.CanMove(enemy)))
                        {
                                R.Cast(enemy, true);
                        }
                    }

                    if (target.HasBuffOfType(BuffType.Slow) && Config.Item("Rslow", true).GetValue<bool>() && countR < comboStack + 1 && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(R, target);
                    else if (Program.Combo && countR < comboStack && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(R, target);
                    else if (Program.Harass && countR < harasStack && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(R, target);
                }
            }
        }

        private void LogicW()
        {
            if (Player.CountEnemiesInRange(W.Range) > 0 && Sheen())
            {
                if (Program.Combo)
                    W.Cast();
                else if (Program.Harass && Config.Item("harassW", true).GetValue<bool>() && Player.CountEnemiesInRange(Player.AttackRange) > 0)
                    W.Cast();
            }
        }

        private void LogicQ()
        {
            if (Sheen())
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
        }

        private void LogicE()
        {
            if ( Sheen())
            {
                var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var qDmg = Q.GetDamage(t);
                    var eDmg = OktwCommon.GetKsDamage(t, E);
                    if (eDmg > t.Health)
                        Program.CastSpell(E, t);
                    else if (eDmg + qDmg > t.Health && Q.IsReady())
                        Program.CastSpell(E, t);
                    else if (Program.Combo && ObjectManager.Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(E, t);
                    else if (Program.Harass && Config.Item("harassE", true).GetValue<bool>() && Player.Mana > RMANA + WMANA + EMANA + QMANA + EMANA)
                        Program.CastSpell(E, t);
                    else if ((Program.Combo || Program.Harass) && ObjectManager.Player.Mana > RMANA + WMANA + EMANA)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                                E.Cast(enemy, true);
                    }
                }
                else if (FarmSpells && Config.Item("farmE", true).GetValue<bool>())
                {
                    var minionList = Cache.GetMinions(Player.ServerPosition, E.Range);
                    var farmPosition = E.GetLineFarmLocation(minionList, E.Width);

                    if (farmPosition.MinionsHit >= FarmMinions)
                        E.Cast(farmPosition.Position);
                }
            }
        }

        private bool Sheen()
        {
            var target = Orbwalker.GetTarget();
            if (!(target is Obj_AI_Hero))
                attackNow = true;
            if (target.IsValidTarget() && Player.HasBuff("sheen") && Config.Item("sheen", true).GetValue<bool>() && target is Obj_AI_Hero)
            {
                Program.debug("shen true");
                return false;
            }
            else if (target.IsValidTarget() && Config.Item("AApriority", true).GetValue<bool>() && target is Obj_AI_Hero && !attackNow)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private int GetRStacks()
        {
            foreach (var buff in ObjectManager.Player.Buffs)
            {
                if (buff.Name == "kogmawlivingartillerycost")
                    return buff.Count;
            }
            return 0;
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

            if (Config.Item("ComboInfo", true).GetValue<bool>())
            {
                var combo = "Harass";
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget()))
                {
                    if (R.GetDamage(enemy) > enemy.Health)
                    {
                        combo = "KILL R";
                        drawText(combo, enemy, System.Drawing.Color.GreenYellow);
                    }
                    else
                    {
                        combo = (int)(enemy.Health / R.GetDamage(enemy)) + " R";
                        drawText(combo, enemy, System.Drawing.Color.Red);
                    }
                }
            }
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
