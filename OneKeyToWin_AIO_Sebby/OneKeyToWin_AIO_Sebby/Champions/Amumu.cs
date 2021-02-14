using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Amumu : Base
    {
        private int grab = 0, grabS = 0;
        private float grabW = 0;

        public Amumu()
        {
            Q = new Spell(SpellSlot.Q, 920);
            W = new Spell(SpellSlot.W, 300);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 550);
            Q.SetSkillshot(0.25f, 100f, 2000f, true, SkillshotType.SkillshotLine);

           

            HeroMenu.AddItem(new MenuItem("showBandageStats", "Show statistics", true).SetValue(true));

            QConfig();
            WConfig();
            EConfig();
            RConfig();
            DrawConfig();

            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }
        
        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && MainMenu.Item("inter", true).GetValue<bool>() && sender.IsValidTarget(R.Range))
            {
                R.Cast();
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.Q)
            {
                // do not count grabs used during farm
                if (!Program.LaneClear)
                {
                    grab++;
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("showBandageStats", true).GetValue<bool>())
            {
                var percent = 0f;
                if (grab > 0)
                    percent = ((float) grabS / (float) grab) * 100f;
                Drawing.DrawText(Drawing.Width * 0f, Drawing.Height * 0.4f, System.Drawing.Color.YellowGreen,
                    " grab: " + grab + " grab successful: " + grabS + " grab successful % : " + percent + "%");
            }

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

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && MainMenu.Item("Gap", true).GetValue<bool>() && gapcloser.Sender.IsValidTarget(R.Range))
            {
                R.Cast();
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            LogicQ();
            LogicW();
            LogicE();
            LogicR();

            if (!Q.IsReady() && Game.Time - grabW > 2)
            {
                foreach (var enemy in HeroManager.Enemies.Where(SuccessfulBandage))
                {
                    grabS++;
                    grabW = Game.Time;
                }
            }
        }

        private void LogicQ()
        {
            float maxGrab = MainMenu.Item("maxGrab", true).GetValue<Slider>().Value;
            float minGrab = MainMenu.Item("minGrab", true).GetValue<Slider>().Value;
            var ts = MainMenu.Item("ts", true).GetValue<bool>();
            var targetUnderAllyTurret = Player.UnderAllyTurret() && MainMenu.Item("qTur", true).GetValue<bool>();
            var qCC = MainMenu.Item("qCC", true).GetValue<bool>();

            if (Q.IsReady())
            {
                if (Program.Combo && ts)
                {
                    var target = TargetSelector.GetTarget(maxGrab, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(maxGrab) && !target.HasBuffOfType(BuffType.SpellImmunity) &&
                        !target.HasBuffOfType(BuffType.SpellShield) &&
                        MainMenu.Item("grab" + target.ChampionName).GetValue<bool>() &&
                        Player.Distance(target.ServerPosition) > minGrab)
                        Program.CastSpell(Q, target);
                }
                else if (Program.LaneClear)
                {
                    foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                        MinionTypes.All,MinionTeam.NotAlly)
                        .Where(minion => minion.Team == GameObjectTeam.Neutral
                            && minion.IsValidTarget(maxGrab) 
                            && Player.Distance(minion.ServerPosition) > Q.Range / 2))
                    {
                        Q.Cast(minion.ServerPosition);
                    }
                }

                foreach (var t in HeroManager.Enemies.Where(t =>
                    t.IsValidTarget(maxGrab) && (MainMenu.Item("grab" + t.ChampionName).GetValue<bool>())))
                {
                    if (!t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield) &&
                        Player.Distance(t.ServerPosition) > minGrab)
                    {
                        if (Program.Combo && !ts)
                            Program.CastSpell(Q, t);
                        else if (targetUnderAllyTurret)
                            Program.CastSpell(Q, t);

                        if (qCC)
                        {
                            if (!OktwCommon.CanMove(t))
                                Q.Cast(t, true);
                            Q.CastIfHitchanceEquals(t, HitChance.Dashing);
                            Q.CastIfHitchanceEquals(t, HitChance.Immobile);
                        }
                    }
                }
            }
        }

        private void LogicW()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            var enoughMana = true;

            if (target != null || (minions.Count > 0 && Program.LaneClear))
            {
                if (target != null)
                {
                    enoughMana = Player.ManaPercent >= MainMenu.Item("wComboUntil", true).GetValue<Slider>().Value;
                }
                else
                {
                    enoughMana = Player.ManaPercent >= MainMenu.Item("wFarmUntil", true).GetValue<Slider>().Value;
                }

                if (IsWTurnedOff() && enoughMana)
                {
                    TurnOnW();
                }
                else if (!enoughMana)
                {
                    TurnOffW();
                }
            }
            else
            {
                TurnOffW();
            }
        }

        private void LogicE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            
            var enoughMana = true;
            
            if (target != null || (minions.Count > 0 && Program.LaneClear))
            {
                if (target != null)
                {
                    enoughMana = Player.ManaPercent >= MainMenu.Item("eComboUntil", true).GetValue<Slider>().Value;
                }
                else
                {
                    enoughMana = Player.ManaPercent >= MainMenu.Item("eFarmUntil", true).GetValue<Slider>().Value;
                }

                if (enoughMana)
                    E.Cast();
            }
        }

        private void LogicR()
        {
            bool rKs = MainMenu.Item("rKs", true).GetValue<bool>();

            if (R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range)))
                {
                    if (rKs && R.GetDamage(target) > target.Health * 1.1)
                    {
                        R.Cast();
                    }
                }

                if (Player.CountEnemiesInRange(R.Range) >= MainMenu.Item("rCount", true).GetValue<Slider>().Value &&
                    MainMenu.Item("rCount", true).GetValue<Slider>().Value > 0)
                {
                    R.Cast();
                }
            }
        }

        private bool SuccessfulBandage(Obj_AI_Hero enemy)
        {
            return enemy.IsChampion()
                   && enemy.HasBuff("Stun")
                   && !enemy.HasBuff("CurseoftheSadMummy")
                   && enemy.GetBuff("Stun").Caster.NetworkId == Player.NetworkId;
        }

        private void TurnOnW()
        {
            if (IsWTurnedOff())
            {
                W.Cast();
            }
        }

        private void TurnOffW()
        {
            if (!IsWTurnedOff())
            {
                W.Cast();
            }
        }

        private bool IsWTurnedOff()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1;
        }

        private void QConfig()
        {
            HeroMenu.SubMenu("Q option")
                .AddItem(new MenuItem("ts", "Use common TargetSelector", true).SetValue(true));
            HeroMenu.SubMenu("Q option")
                .AddItem(new MenuItem("ts1", "ON - only one target"));
            HeroMenu.SubMenu("Q option")
                .AddItem(new MenuItem("ts2", "OFF - all grab-able targets"));
            HeroMenu.SubMenu("Q option")
                .AddItem(new MenuItem("qTur", "Auto Q under turret", true).SetValue(true));
            HeroMenu.SubMenu("Q option")
                .AddItem(new MenuItem("qCC", "Auto Q cc & dash enemy", true).SetValue(true));
            HeroMenu.SubMenu("Q option").AddItem(
                new MenuItem("minGrab", "Min range grab", true).SetValue(new Slider(300, 125, (int) Q.Range)));
            HeroMenu.SubMenu("Q option").AddItem(
                new MenuItem("maxGrab", "Max range grab", true).SetValue(new Slider((int) Q.Range, 125,
                    (int) Q.Range)));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("Q option").SubMenu("Grab")
                    .AddItem(new MenuItem("grab" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
        }

        private void WConfig()
        {
            HeroMenu.SubMenu("W option")
                .AddItem(new MenuItem("wFarmUntil", "[Farm] Use W until Mana %", true)).SetValue(new Slider(20));
            HeroMenu.SubMenu("W option")
                .AddItem(new MenuItem("wComboUntil", "[Combo] Use W until Mana %", true)).SetValue(new Slider(10));
        }

        private void EConfig()
        {
            HeroMenu.SubMenu("E option")
                .AddItem(new MenuItem("eFarmUntil", "[Farm] Use E until Mana %", true)).SetValue(new Slider(20));
            HeroMenu.SubMenu("E option")
                .AddItem(new MenuItem("eComboUntil", "[Combo] Use E until Mana %", true)).SetValue(new Slider(10));
        }

        private void RConfig()
        {
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("rCount", "Auto R if enemies in range", true).SetValue(new Slider(3, 0, 5)));
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("rKs", "R ks", true).SetValue(true));
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("inter", "OnPossibleToInterrupt", true)).SetValue(true);
            HeroMenu.SubMenu("R option")
                .AddItem(new MenuItem("Gap", "OnEnemyGapcloser", true)).SetValue(true);
        }

        private void DrawConfig()
        {
            HeroMenu.SubMenu("Draw")
                .AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
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