using System;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Hecarim : Base
    {
        public Hecarim()
        {
            Q = new Spell(SpellSlot.Q, 350);
            W = new Spell(SpellSlot.W, 575);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 1000);
            R1 = new Spell(SpellSlot.R, 1000);

            // TODO check width
            R.SetSkillshot(0.0f, 250f, 1100, false, SkillshotType.SkillshotCircle);
            R1.SetSkillshot(0.0f, 250f, 1100, false, SkillshotType.SkillshotLine);

            EHeroMenu();
            RHeroMenu();
            DrawHeroMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
        }
        
        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && EMANA + RMANA < Player.Mana
                && HeroMenu.Item("inter", true).GetValue<bool>() && sender.IsValidTarget(W.Range))
            {
                E.Cast();
            }
        }
        
        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && E.IsReady() && EMANA + RMANA < Player.Mana
                && HeroMenu.Item("Gap", true).GetValue<bool>() && gapcloser.Sender.IsValidTarget(W.Range))
            {
                E.Cast();
            }
        }
        
        private void Drawing_OnDraw(EventArgs args)
        {
            foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range + R.Width)))
            {
                var predictionCircle = R.GetPrediction(target, true);
                var predictionLine = R1.GetPrediction(target, true);
                Utility.DrawCircle(target.Position, 60, System.Drawing.Color.Pink, 1, 1);
                if (predictionCircle.AoeTargetsHitCount > 0 || predictionLine.AoeTargetsHitCount > 0)
                {
                    if (predictionCircle.AoeTargetsHitCount > 0)
                        Utility.DrawCircle(predictionCircle.CastPosition, 50, System.Drawing.Color.GreenYellow, 1, 1);

                    if (predictionLine.AoeTargetsHitCount > 0)
                        Utility.DrawCircle(predictionLine.CastPosition, 70, System.Drawing.Color.OrangeRed, 1, 1);
                }
            }


            if (HeroMenu.Item("qRange", true).GetValue<bool>())
            {
                if (HeroMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Gray, 1, 1);
            }
            
            if (HeroMenu.Item("wRange", true).GetValue<bool>())
            {
                if (HeroMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Gray, 1, 1);
            }

            if (HeroMenu.Item("rRange", true).GetValue<bool>())
            {
                if (HeroMenu.Item("onlyRdy", true).GetValue<bool>())
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
            SetMana();

            if(R.IsReady())
                LogicR();

            LogicQ();
            LogicW();
            LogicE();
            
        }
        
        private void SetMana()
        {
            if (Program.Combo || Player.HealthPercent < 20)
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
                RMANA = 0;
            else 
                RMANA = R.Instance.ManaCost;
        }

        private void LogicQ()
        {
            // always save mana for R
            if (Q.IsReady() && QMANA + RMANA < Player.Mana)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
                
                if (target != null || (minions.Count > 0 && Program.LaneClear))
                {
                    Q.Cast();
                }
            }
        }
        
        private void LogicW()
        {
            // always save mana for R
            if (W.IsReady() && WMANA + RMANA < Player.Mana)
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
                
                if (Player.HealthPercent < 95 && (target != null || (minions.Count > 0 && Program.LaneClear)))
                {
                    W.Cast();
                }
            }
        }

        private void LogicE()
        {
            // always save mana for R
            if (E.IsReady() && EMANA + RMANA < Player.Mana)
            {
                if (Program.Combo)
               {
                   E.Cast();
               }
               else if (Program.LaneClear)
                {
                    var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                        MinionTypes.All, MinionTeam.NotAlly);
            
                    // do not use when near minons
                    if (!(minions.Count > 0))
                    {
                        E.Cast();
                    }
                }
            }
        }

        private void LogicR()
        {
            bool rKs = HeroMenu.Item("rKs", true).GetValue<bool>();
            var ts = HeroMenu.Item("ts", true).GetValue<bool>();

            if (!Program.Combo)
                return;

            Vector3 bestRposition = new Vector3();
            int aoeCount = 0;

            foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range)))
            {
                var predictionCircle = R.GetPrediction(target, true);
                var predictionLine = R1.GetPrediction(target, true);

                if(predictionLine.AoeTargetsHitCount > 0 && aoeCount > predictionLine.AoeTargetsHitCount && predictionLine.Hitchance >= HitChance.High)
                {
                    bestRposition = predictionLine.CastPosition;
                    aoeCount = predictionLine.AoeTargetsHitCount;
                }
                if (predictionCircle.AoeTargetsHitCount > 0 && aoeCount >= predictionLine.AoeTargetsHitCount && predictionCircle.Hitchance >= HitChance.High)
                {
                    bestRposition = predictionCircle.CastPosition;
                    aoeCount = predictionCircle.AoeTargetsHitCount;
                }
            }

            if (aoeCount + 1 >= HeroMenu.Item("Raoe", true).GetValue<int>())
            {
                if(bestRposition.CountAlliesInRange(1000) > 0)
                    R.Cast(bestRposition);
            }

            return;    

            if (R.IsReady() && Program.Combo)
            {
                if (rKs)
                {
                    foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range)))
                    {
                        if (R.GetDamage(target) > target.Health * 1.8)
                        {
                            // TODO recalculate, use behind
                            Program.CastSpell(R, target);
                        }
                    }
                }
                if (ts && QMANA + WMANA + EMANA + RMANA < Player.Mana)
                {
                    var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(R.Range)
                        && Player.Distance(target.ServerPosition) > Player.AttackRange)
                    {
                        Program.CastSpell(R, target);
                    }
                }
            }
        }

        private void EHeroMenu()
        {
            HeroMenu.SubMenu("E option")
                .AddItem(new MenuItem("inter", "OnPossibleToInterrupt", true)).SetValue(true);
            HeroMenu.SubMenu("E option")
                .AddItem(new MenuItem("Gap", "OnEnemyGapcloser", true)).SetValue(true);
        }

        private void RHeroMenu()
        {
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("ts", "Use common TargetSelector", true).SetValue(true));
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("ts1", "ON - only one target"));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("R option").SubMenu("Use R on")
                    .AddItem(new MenuItem("grab" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("rKs", "R ks", true).SetValue(true));
            HeroMenu.SubMenu("R option").
                AddItem(new MenuItem("Raoe", "R aoe", true).SetValue(new Slider(0, 5, 2)));
        }

        private void DrawHeroMenu()
        {
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));
        }
    }
}