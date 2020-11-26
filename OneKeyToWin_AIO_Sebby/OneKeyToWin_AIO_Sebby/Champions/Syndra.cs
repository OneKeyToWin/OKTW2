using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Syndra : Base
    {
        private Spell EQ, Eany;
        private static List<Obj_AI_Minion> BallsList = new List<Obj_AI_Minion>();
        private bool EQcastNow = false;

        public Syndra()
        {
            Q = new Spell(SpellSlot.Q, 790);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 700);
            EQ = new Spell(SpellSlot.Q, Q.Range + 500);
            Eany = new Spell(SpellSlot.Q, Q.Range + 500);
            R = new Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 100, 2500f, false, SkillshotType.SkillshotLine);
            EQ.SetSkillshot(0.6f, 100f, 2500f, false, SkillshotType.SkillshotLine);
            Eany.SetSkillshot(0.30f, 50f, 2500f, false, SkillshotType.SkillshotLine);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("QHarassMana", "Harass Mana", true).SetValue(new Slider(30, 100, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto Q + E combo, ks", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass Q + E", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("EInterrupter", "Auto Q + E Interrupter", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("useQE", "Semi-manual Q + E near mouse key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("Auto Q + E Gapcloser").AddItem(new MenuItem("Egapcloser" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("Use Q + E on").AddItem(new MenuItem("Eon" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R KS", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rcombo", "Extra combo dmg calculation", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("Use on").AddItem(new MenuItem("Rmode" + enemy.ChampionName, enemy.ChampionName, true).SetValue(new StringList(new[] { "KS ", "Always ", "Never " }, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQout", "Last hit Q minion out range AA", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.Q && EQcastNow && E.IsReady())
            {
                var customeDelay = Q.Delay - (E.Delay + ((Player.Distance(args.End)) / E.Speed));
                Utility.DelayAction.Add((int)(customeDelay * 1000), () => E.Cast(args.End));
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && Config.Item("EInterrupter", true).GetValue<bool>())
            {
                if(sender.IsValidTarget(E.Range))
                {
                    E.Cast(sender.Position);
                }
                else if (Q.IsReady() && sender.IsValidTarget(EQ.Range))
                {
                    TryBallE(sender);
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && Config.Item("Egapcloser" + gapcloser.Sender.ChampionName, true).GetValue<bool>())
            {
                if (Q.IsReady())
                {
                    EQcastNow = true;
                    Q.Cast(gapcloser.Sender);
                }
                else if(gapcloser.Sender.IsValidTarget(E.Range))
                {
                    E.Cast(gapcloser.Sender);
                }
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Type == GameObjectType.obj_AI_Minion && sender.Name == "Seed")
            {
                var ball = sender as Obj_AI_Minion;
                BallsList.Add(ball);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!E.IsReady())
                EQcastNow = false;

            if (Program.LagFree(0))
            { 
                SetMana();
                BallCleaner();
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && Config.Item("autoE", true).GetValue<bool>())
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
                LogicW();

            if (Program.LagFree(4) && R.IsReady() && Config.Item("autoR", true).GetValue<bool>())
                LogicR();
        }

        private void TryBallE(Obj_AI_Hero t)
        {
            if (Q.IsReady())
            {
                CastQE(t);
            }
            if(!EQcastNow)
            {
                var ePred = Eany.GetPrediction(t);
                if (ePred.Hitchance >= HitChance.VeryHigh)
                {
                    var playerToCP = Player.Distance(ePred.CastPosition);
                    foreach (var ball in BallsList.Where(ball => Player.Distance(ball.Position) < E.Range))
                    {
                        var ballFinalPos = Player.ServerPosition.Extend(ball.Position, playerToCP);
                        if (ballFinalPos.Distance(ePred.CastPosition) < 50)
                            E.Cast(ball.Position);
                    }
                }
            }
        }

        private void LogicE()
        {
            if(Config.Item("useQE", true).GetValue<KeyBind>().Active)
            {
                var mouseTarget = HeroManager.Enemies.Where(enemy => 
                    enemy.IsValidTarget(Eany.Range)).OrderBy(enemy => enemy.Distance(Game.CursorPos)).FirstOrDefault();

                if (mouseTarget != null)
                {
                    TryBallE(mouseTarget);
                    return;
                }
            }

            var t = TargetSelector.GetTarget(Eany.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, E) + Q.GetDamage(t)> t.Health)
                    TryBallE(t);
                if (Program.Combo && Player.Mana > RMANA + EMANA + QMANA && Config.Item("Eon" + t.ChampionName, true).GetValue<bool>())
                    TryBallE(t);
                if (Program.Harass && Player.Mana > RMANA + EMANA + QMANA + WMANA && Config.Item("harassE", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>())
                    TryBallE(t);
            }
        }

        private void LogicR()
        {
            R.Range = R.Level == 3 ? 750 : 675;

            bool Rcombo = Config.Item("Rcombo", true).GetValue<bool>();

            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && OktwCommon.ValidUlt(enemy)))
            {
                int Rmode = Config.Item("Rmode" + enemy.ChampionName, true).GetValue<StringList>().SelectedIndex;

                if (Rmode == 2)
                    continue;
                else if (Rmode == 1)
                    R.Cast(enemy);

                var comboDMG = OktwCommon.GetKsDamage(enemy, R) ;
                comboDMG += R.GetDamage(enemy, 1) * (R.Instance.Ammo - 3);
                Console.WriteLine(comboDMG + " " + R.Instance.Ammo);
                if (Rcombo)
                {
                    if (Q.IsReady() && enemy.IsValidTarget(600))
                        comboDMG += Q.GetDamage(enemy);

                    if (E.IsReady())
                        comboDMG += E.GetDamage(enemy);

                    if (W.IsReady())
                        comboDMG += W.GetDamage(enemy);
                }

                if (enemy.Health < comboDMG)
                {
                    R.Cast(enemy);
                }
            }
        }

        private void LogicW()
        {
            if (W.Instance.ToggleState == 1)
            {
                var t = TargetSelector.GetTarget(W.Range - 150, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                        CatchW(t);
                    else if (Program.Harass && Config.Item("harassW", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>() 
                        && Player.ManaPercent > Config.Item("QHarassMana", true).GetValue<Slider>().Value && OktwCommon.CanHarras())
                    {
                        CatchW(t);
                    }
                    else if (OktwCommon.GetKsDamage(t, W) > t.Health)
                        CatchW(t);
                    else if (Player.Mana > RMANA + WMANA)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                            CatchW(t);
                    }
                }
                else if (Program.LaneClear && !Q.IsReady() && FarmSpells && Config.Item("farmW", true).GetValue<bool>())
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                    var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);

                    if (farmPos.MinionsHit >= FarmMinions)
                        CatchW(allMinions.FirstOrDefault());
                }
            }
            else
            {
                var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    Program.CastSpell(W, t);
                }
                else if (FarmSpells && Config.Item("farmW", true).GetValue<bool>())
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                    var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);

                    if (farmPos.MinionsHit > 1)
                        W.Cast(farmPos.Position);
                }
            }
        }   

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA + EMANA && !E.IsReady())
                    Program.CastSpell(Q, t);
                else if (Program.Harass && Config.Item("harassQ", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.ManaPercent > Config.Item("QHarassMana", true).GetValue<Slider>().Value && OktwCommon.CanHarras())
                    Program.CastSpell(Q, t);
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Program.CastSpell(Q, t);
                else if (Player.Mana > RMANA + QMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Program.CastSpell(Q, t);
                }
            }

            if (Player.IsWindingUp)
                return;

            if (!Program.None && !Program.Combo )
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);

                if (Config.Item("farmQout", true).GetValue<bool>() && Player.Mana > RMANA + QMANA + EMANA + WMANA)
                {
                    foreach (var minion in allMinions.Where(minion => minion.IsValidTarget(Q.Range) && (!Orbwalker.InAutoAttackRange(minion) || (!minion.UnderTurret(true) && minion.UnderTurret()))))
                    {
                        var hpPred = SebbyLib.HealthPrediction.GetHealthPrediction(minion, 600);
                        if (hpPred < Q.GetDamage(minion)  && hpPred > minion.Health - hpPred * 2)
                        {
                            Q.Cast(minion);
                            return;
                        }
                    }
                }
                if (FarmSpells && Config.Item("farmQ", true).GetValue<bool>())
                {
                    var farmPos = Q.GetCircularFarmLocation(allMinions, Q.Width);
                    if (farmPos.MinionsHit >= FarmMinions)
                        Q.Cast(farmPos.Position);
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }
                    else if (W.IsReady() && Config.Item("jungleW", true).GetValue<bool>() && Utils.TickCount - Q.LastCastAttemptT > 900)
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private void CastQE(Obj_AI_Base target)
        {
            SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;

            var predInput2 = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = false,
                Collision = EQ.Collision,
                Speed = EQ.Speed,
                Delay = EQ.Delay,
                Range = EQ.Range,
                From = Player.ServerPosition,
                Radius = EQ.Width,
                Unit = target,
                Type = CoreType2
            };

            var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            Vector3 castQpos = poutput2.CastPosition;

            if (Player.Distance(castQpos) > Q.Range)
                castQpos = Player.Position.Extend(castQpos, Q.Range);

            if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 0)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }

            }
            else if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 1)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }

            }
            else if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 2)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                {
                    EQcastNow = true;
                    Q.Cast(castQpos);
                }
            }
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
                        Utility.DrawCircle(ObjectManager.Player.Position, EQ.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, EQ.Range, System.Drawing.Color.Yellow, 1, 1);
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

        private void BallCleaner()
        {
            if (BallsList.Count > 0)
            {
                BallsList.RemoveAll(ball => !ball.IsValid || ball.Mana == 19);
            }
        }

        private void CatchW(Obj_AI_Base t, bool onlyMinin = false)
        {

            if (Utils.TickCount - W.LastCastAttemptT < 150)
                return;

            var catchRange = 925;
            Obj_AI_Base obj = null;
            if (BallsList.Count > 0 && !onlyMinin)
            {
                obj = BallsList.Find(ball => ball.Distance(Player) < catchRange);
            }
            if (obj == null)
            {
                obj = MinionManager.GetMinions(Player.ServerPosition, catchRange, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth).FirstOrDefault();
            }

            if (obj != null)
            {
                foreach (var minion in MinionManager.GetMinions(Player.ServerPosition, catchRange, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth))
                {
                    if (t.Distance(minion) < t.Distance(obj))
                        obj = minion;
                }
                
                W.Cast(obj.Position);
            }
        }
    }
}
