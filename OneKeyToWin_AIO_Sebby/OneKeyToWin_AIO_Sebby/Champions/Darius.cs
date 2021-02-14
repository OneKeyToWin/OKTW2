using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Darius : Base
    {
        public Darius()
        {
            Q = new Spell(SpellSlot.Q, 430);
            W = new Spell(SpellSlot.W, 145);
            E = new Spell(SpellSlot.E, 530);
            R = new Spell(SpellSlot.R, 460);

            E.SetSkillshot(0.01f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));

            HeroMenu.SubMenu("Q option").AddItem(new MenuItem("Harass", "Harass Q", true).SetValue(true));
            HeroMenu.SubMenu("Q option").AddItem(new MenuItem("qOutRange", "Auto Q only out range AA", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("E Config").SubMenu("Use E on").AddItem(new MenuItem("Eon" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            HeroMenu.SubMenu("R option").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("R option").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            HeroMenu.SubMenu("R option").AddItem(new MenuItem("autoRbuff", "Auto R if darius execute multi cast time out ", true).SetValue(true));
            HeroMenu.SubMenu("R option").AddItem(new MenuItem("autoRdeath", "Auto R if darius execute multi cast and under 10 % hp", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "Farm W", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "Farm Q", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += afterAttack;
            Interrupter.OnPossibleToInterrupt += OnInterruptableSpell;
        }

        private void OnInterruptableSpell(Obj_AI_Hero unit, InterruptableSpell spell)
        {
            if (E.IsReady()  && unit.IsValidTarget(E.Range))
                E.Cast(unit);
        }


        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Player.Mana < RMANA + WMANA || !W.IsReady() || !unit.IsMe)
                return;

            var t = target as Obj_AI_Hero;

            if (t.IsValidTarget())
                W.Cast();

        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {


        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (R.IsReady() && MainMenu.Item("useR", true).GetValue<KeyBind>().Active )
            {
                var targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.True);
                if (targetR.IsValidTarget())
                    R.Cast(targetR, true);
            }

            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (Program.LagFree(1) && W.IsReady())
                LogicW();
            if (Program.LagFree(2) && Q.IsReady() && Orbwalking.CanMove(50) && !Player.IsWindingUp)
                LogicQ();
            if (Program.LagFree(3) && E.IsReady())
                LogicE();
            if (Program.LagFree(4) && R.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                LogicR();
        }

        private void LogicW()
        {
            if (!Player.IsWindingUp && MainMenu.Item("farmW", true).GetValue<bool>() && Program.Farm)
            {
                var minions = Cache.GetMinions(Player.Position, Player.AttackRange);

                int countMinions = 0;

                foreach (var minion in minions.Where(minion => minion.Health < W.GetDamage(minion)))
                {
                    countMinions++;
                }

                if (countMinions > 0)
                    W.Cast();
            }
        }

        private void LogicE()
        {
            if (Player.Mana > RMANA + EMANA )
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && MainMenu.Item("Eon" + target.ChampionName, true).GetValue<bool>() && ((Player.UnderTurret(false) && !Player.UnderTurret(true)) || Program.Combo) )
                {
                    if (!Orbwalking.InAutoAttackRange(target))
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (!MainMenu.Item("qOutRange", true).GetValue<bool>() || Orbwalking.InAutoAttackRange(t))
                {
                    if (Player.Mana > RMANA + QMANA && Program.Combo)
                        Q.Cast();
                    else if (Program.Harass && Player.Mana > RMANA + QMANA + EMANA + WMANA && MainMenu.Item("Harass", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                        Q.Cast();
                }

                if (!R.IsReady() && OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Q.Cast();
            }
            
            else if (MainMenu.Item("farmQ", true).GetValue<bool>() && FarmSpells)
            {
                var minionsList = Cache.GetMinions(Player.ServerPosition, Q.Range);

                if (minionsList.Any(x => Player.Distance(x.ServerPosition) > 300 && x.Health < Q.GetDamage(x) * 0.6))
                    Q.Cast();
                        
            }
        }

        private void LogicR()
        {
            var targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.True);
            if (targetR.IsValidTarget() && OktwCommon.ValidUlt(targetR) && MainMenu.Item("autoRbuff", true).GetValue<bool>())
            {
                var buffTime = OktwCommon.GetPassiveTime(Player, "dariusexecutemulticast");
                if((buffTime < 2 || (Player.HealthPercent < 10 && MainMenu.Item("autoRdeath", true).GetValue<bool>())) && buffTime > 0)
                    R.Cast(targetR, true);
            }

            foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
            {
                var dmgR = OktwCommon.GetKsDamage(target, R, false);

                if (dmgR > target.Health)
                {
                    R.Cast(target);
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("qRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>() && Q.IsReady())
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                    else
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (MainMenu.Item("eRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>() && E.IsReady())
                    if (E.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
                    else
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
            } 
            if (MainMenu.Item("rRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>() && R.IsReady())
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Red, 1, 1);
                    else
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Red, 1, 1);
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
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.ManaCost;
        }
        
    }
}
