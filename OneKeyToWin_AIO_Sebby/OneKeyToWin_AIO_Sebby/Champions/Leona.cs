using System;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Leona : Base
    {
        private int grab = 0, grabS = 0;
        private float grabW = 0;

        public Leona()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 450);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 1100);
            
            E.SetSkillshot(0.25f, 100f, 2000f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            
            HeroMenu
                .AddItem(new MenuItem("showStats", "Show statistics", true).SetValue(true));
            
            EConfig();
            RConfig();
            DrawConfig();

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (MainMenu.Item("afterAA", true).GetValue<bool>() && Q.IsReady() && target is Obj_AI_Hero)
            {
                foreach (var buff in ((Obj_AI_Hero) target).Buffs)
                {
                    if (buff.Type == BuffType.Stun && (buff.EndTime - Game.Time) < 0.3)
                    {
                        Q.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                    else
                    {
                        Q.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && sender.IsValidTarget(R.Range) && MainMenu.Item("inter", true).GetValue<bool>())
            {
                Program.CastSpell(R, sender); 
            }
        }
        
        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (MainMenu.Item("Gap", true).GetValue<bool>())
            {
                if (Q.IsReady() && gapcloser.Sender.IsValidTarget(Player.AttackRange))
                {
                    Q.Cast();
                    Orbwalking.ResetAutoAttackTimer();
                }
                else if (R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range))
                {
                    Program.CastSpell(R, gapcloser.Sender); 
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.E)
            {
                grab++;
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("showStats", true).GetValue<bool>())
            {
                var percent = 0f;
                if (grab > 0)
                    percent = ((float) grabS / (float) grab) * 100f;
                Drawing.DrawText(Drawing.Width * 0f, Drawing.Height * 0.4f, System.Drawing.Color.YellowGreen,
                    " grab: " + grab + " grab successful: " + grabS + " grab successful % : " + percent + "%");
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
            SetMana();
            LogicW();
            LogicE();
            LogicR();

            if (!E.IsReady() && Game.Time - grabW > 2)
            {
                foreach (var t in HeroManager.Enemies.Where(t => t.HasBuff("leonazenithbladeroot")))
                {
                    grabS++;
                    grabW = Game.Time;
                    Program.debug("GRAB!!!!");
                }
            }
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
        
        private void LogicW()
        {
            double dmg = OktwCommon.GetIncomingDamage(Player);

            int nearEnemys = Player.CountEnemiesInRange(800);

            int sensitivity = 20;

            double hpPercentage = (dmg * 100) / Player.Health;

            if (Player.HasBuffOfType(BuffType.Poison))
            {
                W.Cast();
            }

            nearEnemys = (nearEnemys == 0) ? 1 : nearEnemys;

            if (dmg > 100 + Player.Level * sensitivity)
                W.Cast();
            else if (Player.Health - dmg < nearEnemys * Player.Level * sensitivity)
                W.Cast();
            else if (hpPercentage >= 5)
                W.Cast();
        }

        private void LogicE()
        {
            float maxGrab = MainMenu.Item("maxGrab", true).GetValue<Slider>().Value;
            float minGrab = MainMenu.Item("minGrab", true).GetValue<Slider>().Value;
            var ts = MainMenu.Item("ts", true).GetValue<bool>();
            
            if (E.IsReady() && Program.Combo && (QMANA + WMANA + EMANA + RMANA) < Player.Mana)
            {
                if (ts)
                {
                    var target = TargetSelector.GetTarget(maxGrab, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(maxGrab) && !target.HasBuffOfType(BuffType.SpellImmunity) &&
                        !target.HasBuffOfType(BuffType.SpellShield) &&
                        MainMenu.Item("grab" + target.ChampionName).GetValue<bool>() &&
                        Player.Distance(target.ServerPosition) >= minGrab)
                        Program.CastSpell(E, target);
                }

                foreach (var t in HeroManager.Enemies.Where(t =>
                    t.IsValidTarget(maxGrab) && (MainMenu.Item("grab" + t.ChampionName).GetValue<bool>())))
                {
                    if (!t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield) &&
                        Player.Distance(t.ServerPosition) >= minGrab)
                    {
                        if (Program.Combo && !ts)
                            Program.CastSpell(E, t);
                    }
                }
            }
        }

        private void LogicR()
        {
            bool enemyLowHp = MainMenu.Item("enemyLowHp", true).GetValue<bool>();
            var rCount = MainMenu.Item("rCount", true).GetValue<Slider>().Value;

            if (R.IsReady() && Program.Combo)
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && OktwCommon.ValidUlt(enemy)))
                {
                    if (enemy.IsValidTarget(R.Range))
                    {
                        var poutput = R.GetPrediction(enemy, true);
                        
                        if (enemyLowHp && enemy.HealthPercent <= 20)
                        {
                            Program.CastSpell(R, enemy);
                        }
                        
                        
                        var aoeCount = poutput.AoeTargetsHitCount;

                        if (aoeCount >= rCount && rCount > 0)
                        {
                            R.Cast(poutput.CastPosition);
                        }
                    }
                }
            }
        }

        private void EConfig()
        {
            HeroMenu.SubMenu("E option")
                .AddItem(new MenuItem("ts", "Use common TargetSelector", true).SetValue(true));
            HeroMenu.SubMenu("E option")
                .AddItem(new MenuItem("ts1", "ON - only one target"));
            HeroMenu.SubMenu("E option")
                .AddItem(new MenuItem("ts2", "OFF - all grab-able targets"));
            HeroMenu.SubMenu("E option")
                .AddItem(new MenuItem("qCC", "Auto E cc & dash enemy", true).SetValue(true));
            HeroMenu.SubMenu("E option").AddItem(
                new MenuItem("minGrab", "Min range grab", true).SetValue(new Slider(200, 125, (int) E.Range)));
            HeroMenu.SubMenu("E option").AddItem(
                new MenuItem("maxGrab", "Max range grab", true).SetValue(new Slider((int) E.Range, 125,
                    (int) E.Range)));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("E option").SubMenu("Grab")
                    .AddItem(new MenuItem("grab" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
        }

        private void RConfig()
        {
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("rCount", "Auto R if enemies in range", true).SetValue(new Slider(2, 1, 5)));
            HeroMenu.SubMenu("R option").AddItem(new MenuItem("afterAA", "Auto R after AA", true).SetValue(true));
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("inter", "OnPossibleToInterrupt", true)).SetValue(true);
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("Gap", "OnEnemyGapcloser", true)).SetValue(true);
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("enemyLowHp", "R on low HP", true).SetValue(true));
        }

        private void DrawConfig()
        {
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));
        }
    }
}