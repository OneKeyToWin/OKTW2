using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Orianna : Base
    {
        private Spell QR;
        private float RCastTime = 0;
        private Vector3 BallPos;
        private int FarmId;
        private bool Rsmart = false;
        private Obj_AI_Hero best;

        public Orianna()
        {
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 210);
            E = new Spell(SpellSlot.E, 1095);
            R = new Spell(SpellSlot.R, 360);
            QR = new Spell(SpellSlot.Q, 825);

            Q.SetSkillshot(0.05f, 70f, 1400f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 210f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 100f, 1700f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.4f, 325f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            QR.SetSkillshot(0.5f, 400f, 100f, false, SkillshotType.SkillshotCircle);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("autoW", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("hadrCC", "Auto E hard CC", true).SetValue(true));
            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("poison", "Auto E poison", true).SetValue(true));
            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("Wdmg", "E dmg % hp", true).SetValue(new Slider(10, 100, 0)));
            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("AGC", "AntiGapcloserE", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQout", "Farm Q out range aa minion", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "LaneClear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "LaneClear W", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmE", "LaneClear E", true).SetValue(false));

            HeroMenu.SubMenu("R config").AddItem(new MenuItem("rCount", "Auto R x enemies", true).SetValue(new Slider(3, 0, 5)));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("smartR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("OPTI", "OnPossibleToInterrupt R", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Rturrent", "auto R under turrent", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Rks", "R ks", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Rlifesaver", "auto R life saver", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Rblock", "Block R if 0 hit ", true).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("R config").SubMenu("Always R").AddItem(new MenuItem("Ralways" + enemy.ChampionName, enemy.ChampionName,true).SetValue(false));

            HeroMenu.AddItem(new MenuItem("W", "Auto W SpeedUp logic", true).SetValue(false));
            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget +=Interrupter2_OnInterruptableTarget;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!MainMenu.Item("OPTI", true).GetValue<bool>())
                return;
            if (R.IsReady() && sender.Distance(BallPos) < R.Range)
            {
                R.Cast();
                Program.debug("interupt");
            }
            else if (Q.IsReady() && Player.Mana > RMANA + QMANA && sender.IsValidTarget(Q.Range))
                Q.Cast(sender.ServerPosition);
        }


        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R && MainMenu.Item("Rblock", true).GetValue<bool>() &&  CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) == 0)
                args.Process = false;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var Target = gapcloser.Sender;
            if (MainMenu.Item("AGC", true).GetValue<bool>() && E.IsReady() && Target.IsValidTarget(800) && Player.Mana > RMANA + EMANA)
                E.CastOnUnit(Player);
            return;
        }
        
        private void Game_OnGameUpdate(EventArgs args)
        {
            //Program.debug(""+BallPos.Distance(Player.Position));
            if (Player.HasBuff("Recall") || Player.IsDead)
                return;

            if (R.IsReady())
                LogicR();

            bool hadrCC = true, poison = true;
            if (Program.LagFree(0))
            {
                SetMana();
                hadrCC = MainMenu.Item("hadrCC", true).GetValue<bool>();
                poison = MainMenu.Item("poison", true).GetValue<bool>();
            }

            best = Player;

            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead))
            {
                if (ally.HasBuff("orianaghostself") || ally.HasBuff("orianaghost"))
                    BallPos = ally.ServerPosition;

                if (Program.LagFree(3) )
                {
                    if (E.IsReady() && Player.Mana > RMANA + EMANA && ally.Distance(Player.Position) < E.Range)
                    {
                        var countEnemy = ally.CountEnemiesInRange(800);
                        if (ally.Health < countEnemy * ally.Level * 25)
                        {
                            E.CastOnUnit(ally);
                        }
                        else if (HardCC(ally) && hadrCC && countEnemy > 0)
                        {
                            E.CastOnUnit(ally);
                        }
                        else if (ally.HasBuffOfType(BuffType.Poison))
                        {
                            E.CastOnUnit(ally);
                        }
                    }
                    if (W.IsReady() && Player.Mana > RMANA + WMANA && BallPos.Distance(ally.ServerPosition) < 240 && ally.Health < ally.CountEnemiesInRange(600) * ally.Level * 20)
                        W.Cast();

                    if ((ally.Health < best.Health || ally.CountEnemiesInRange(300) > 0) && ally.Distance(Player.Position) < E.Range && ally.CountEnemiesInRange(700) > 0)
                        best = ally;
                }
                if (Program.LagFree(1) && E.IsReady() && Player.Mana > RMANA + EMANA && ally.Distance(Player.Position) < E.Range && ally.CountEnemiesInRange(R.Width) >= MainMenu.Item("rCount", true).GetValue<Slider>().Value && 0 != MainMenu.Item("rCount", true).GetValue<Slider>().Value)
                {
                    E.CastOnUnit(ally);
                }
            }
            /*
            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && ally.Distance(Player.Position) < 1000))
            {
                foreach (var buff in ally.Buffs)
                {
                        Program.debug(buff.Name);
                }

            }
            */
            if ((MainMenu.Item("smartR", true).GetValue<KeyBind>().Active || Rsmart) && R.IsReady())
            {
                Rsmart = true;
                var target = TargetSelector.GetTarget(Q.Range + 100, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget())
                {
                    if (CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) > 1)
                        R.Cast();
                    else if (Q.IsReady())
                        QR.Cast(target, true, true);
                    else if (CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) > 0)
                        R.Cast();
                }
                else
                    Rsmart = false;
            }
            else
                Rsmart = false;

            if (Program.LagFree(1))
            {
                LogicQ();
                LogicFarm();
            }

            if (Program.LagFree(2) && W.IsReady() )
                LogicW();

            if (Program.LagFree(4) && E.IsReady())
                LogicE(best); 
        }

        private void LogicE(Obj_AI_Hero best)
        {
            var ta = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Magical);

            if (Program.Combo && ta.IsValidTarget() && !W.IsReady() && Player.Mana > RMANA + EMANA)
            {
                if (CountEnemiesInRangeDeley(BallPos, 100, 0.1f) > 0)
                    E.CastOnUnit(best);
                var castArea = ta.Distance(best.ServerPosition) * (best.ServerPosition - ta.ServerPosition).Normalized() + ta.ServerPosition;
                if (castArea.Distance(ta.ServerPosition) < ta.BoundingRadius / 2)
                    E.CastOnUnit(best);
            }
        }

        private void LogicR()
        {            
            foreach (var t in HeroManager.Enemies.Where(t => t.IsValidTarget() && BallPos.Distance(Prediction.GetPrediction(t, R.Delay).CastPosition) < R.Width && BallPos.Distance(t.ServerPosition) < R.Width))
            {
                if (Program.Combo && MainMenu.Item("Ralways" + t.ChampionName, true).GetValue<bool>())
                {
                    R.Cast();
                }

                if (MainMenu.Item("Rks", true).GetValue<bool>())
                {
                    var comboDmg = OktwCommon.GetKsDamage(t, R);

                    if (t.IsValidTarget(Q.Range))
                        comboDmg += Q.GetDamage(t);
                    if (W.IsReady())
                        comboDmg += W.GetDamage(t);
                    if (Orbwalker.InAutoAttackRange(t))
                        comboDmg += (float)Player.GetAutoAttackDamage(t) * 2;
                    if (t.Health < comboDmg)
                        R.Cast();
                    Program.debug("ks");
                }
                if (MainMenu.Item("Rturrent", true).GetValue<bool>() && BallPos.UnderTurret(false) && !BallPos.UnderTurret(true))
                {
                    R.Cast();
                    Program.debug("Rturrent");
                }
                if (MainMenu.Item("Rlifesaver", true).GetValue<bool>() && Player.Health < Player.CountEnemiesInRange(800) * Player.Level * 20 && Player.Distance(BallPos) > t.Distance(Player.Position))
                {
                    R.Cast();
                    Program.debug("ls");
                }
            }

            int countEnemies=CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay);

            if (countEnemies >= MainMenu.Item("rCount", true).GetValue<Slider>().Value && BallPos.CountEnemiesInRange(R.Width) == countEnemies)
                R.Cast();
        }

        private void LogicW()
        {
            foreach (var t in HeroManager.Enemies.Where(t => t.IsValidTarget() && BallPos.Distance(t.ServerPosition) < 250 && t.Health < W.GetDamage(t)))
            {
                W.Cast();
                return;
            }
            if (CountEnemiesInRangeDeley(BallPos, W.Width, 0f) > 0 && Player.Mana > RMANA + WMANA)
            {
                W.Cast();
                return;
            }
            if (MainMenu.Item("W", true).GetValue<bool>() && !Program.Harass && !Program.Combo && ObjectManager.Player.Mana > Player.MaxMana * 0.95 && Player.HasBuff("orianaghostself"))
                W.Cast();
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget() && Q.IsReady())
            {
                if (Q.GetDamage(t) + W.GetDamage(t) > t.Health)
                    CastQ(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA - 10)
                    CastQ(t);
                else if (Program.Harass && Player.Mana > RMANA + QMANA + WMANA + EMANA && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                    CastQ(t);
            }
            if (MainMenu.Item("W", true).GetValue<bool>() && !t.IsValidTarget() && Program.Combo && Player.Mana > RMANA + 3 * QMANA + WMANA + EMANA + WMANA)
            {
                if (W.IsReady() && Player.HasBuff("orianaghostself"))
                {
                    W.Cast();
                }
                else if (E.IsReady() && !Player.HasBuff("orianaghostself"))
                {
                    E.CastOnUnit(Player);
                }
            }
        }

        private void LogicFarm()
        {
            if (!Program.Harass)
                return;

            var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);
            if (MainMenu.Item("farmQout", true).GetValue<bool>() && Player.Mana > RMANA + QMANA + WMANA + EMANA)
            {
                foreach (var minion in allMinions.Where(minion => minion.IsValidTarget(Q.Range) && !Orbwalker.InAutoAttackRange(minion) && minion.Health < Q.GetDamage(minion) && minion.Health > minion.FlatPhysicalDamageMod))
                {
                    Q.Cast(minion);
                }
            }

            if (!Program.LaneClear || Player.Mana < RMANA + QMANA)
                return;

            var mobs = Cache.GetMinions(Player.ServerPosition, 800, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (Q.IsReady())
                    Q.Cast(mob.Position);
                if (W.IsReady() && BallPos.Distance(mob.Position) < W.Width)
                    W.Cast();
                else if (E.IsReady())
                    E.CastOnUnit(best);
                return;
            }
            

            if ((FarmSpells || (Player.UnderTurret(false) && !Player.UnderTurret(true))))
            {
                var Qfarm = Q.GetCircularFarmLocation(allMinions, 100);
                var QWfarm = Q.GetCircularFarmLocation(allMinions, W.Width);

                if (Qfarm.MinionsHit + QWfarm.MinionsHit == 0)
                    return;
                if (MainMenu.Item("farmQ", true).GetValue<bool>())
                {
                    if (Qfarm.MinionsHit >= FarmMinions && !W.IsReady() && Q.IsReady())
                    {
                        Q.Cast(Qfarm.Position);
                    }
                    else if (QWfarm.MinionsHit > 2 && Q.IsReady())
                        Q.Cast(QWfarm.Position);
                }

                foreach (var minion in allMinions)
                {
                    if (W.IsReady() && minion.Distance(BallPos) < W.Range && minion.Health < W.GetDamage(minion) && MainMenu.Item("farmW", true).GetValue<bool>())
                        W.Cast();
                    if (!W.IsReady() && E.IsReady() && minion.Distance(BallPos) < E.Width && MainMenu.Item("farmE", true).GetValue<bool>())
                        E.CastOnUnit(Player);
                }
            }
        }

        private void CastQ(Obj_AI_Hero target)
        {
            float distance = Vector3.Distance(BallPos, target.ServerPosition);
            
            if (E.IsReady() && Player.Mana > RMANA + QMANA + WMANA + EMANA && distance > Player.Distance(target.ServerPosition) + 300)
            {
                E.CastOnUnit(Player);
                return;
            }

            if (MainMenu.Item("Qpred", true).GetValue<StringList>().SelectedIndex == 1)
            {
                //var prepos5 = Core.Prediction.GetPrediction(target, delay, Q.Width);

                var predInput2 = new PredictionInput
                {
                    Aoe = true,
                    Collision = Q.Collision,
                    Speed = Q.Speed,
                    Delay = Q.Delay,
                    Range = float.MaxValue,
                    From = BallPos,
                    Radius = Q.Width,
                    Unit = target,
                    Type = SkillshotType.SkillshotCircle
                };
                var prepos5 = Prediction.GetPrediction(predInput2);

                if ((int)prepos5.Hitchance > 5 - MainMenu.Item("QHitChance", true).GetValue<StringList>().SelectedIndex)
                {
                    if (prepos5.CastPosition.Distance(prepos5.CastPosition) < Q.Range)
                    {
                        Q.Cast(prepos5.CastPosition);
                    }
                }
            }
            else
            {
                float delay = (distance / Q.Speed + Q.Delay);
                var prepos = Prediction.GetPrediction(target, delay, Q.Width);

                if ((int)prepos.Hitchance > 5 - MainMenu.Item("QHitChance", true).GetValue<StringList>().SelectedIndex)
                {
                    if (prepos.CastPosition.Distance(prepos.CastPosition) < Q.Range)
                    {
                        Q.Cast(prepos.CastPosition);
                    }
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if (sender.IsMe && args.SData.Name == "OrianaIzunaCommand")
                BallPos = args.End;

             if (!E.IsReady() || !sender.IsEnemy || !MainMenu.Item("autoW", true).GetValue<bool>() || Player.Mana < EMANA + RMANA || sender.Distance(Player.Position) > 1600)
                return;

            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && Player.Distance(ally.ServerPosition) < E.Range))
            {
                double dmg = 0;
                if (args.Target != null && args.Target.NetworkId == ally.NetworkId)
                {
                    dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                }
                else
                {
                    var castArea = ally.Distance(args.End) * (args.End - ally.ServerPosition).Normalized() + ally.ServerPosition;
                    if (castArea.Distance(ally.ServerPosition) < ally.BoundingRadius / 2)
                        dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                    else
                        continue;
                }

                double HpLeft = ally.Health - dmg;
                double HpPercentage = (dmg * 100) / ally.Health;
                double shieldValue = 60 + E.Level * 40 + 0.4 * Player.FlatMagicDamageMod;

                if (HpPercentage >= MainMenu.Item("Wdmg", true).GetValue<Slider>().Value || dmg > shieldValue)
                    E.CastOnUnit(ally);
            }   
        }

        private int CountEnemiesInRangeDeley(Vector3 position, float range, float delay)
        {
            int count = 0;
            foreach (var t in HeroManager.Enemies.Where(t => t.IsValidTarget()))
            {
                Vector3 prepos = Prediction.GetPrediction(t, delay).CastPosition;
                if (position.Distance(prepos) < range)
                    count++;
            }
            return count;
        }

        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid && obj.IsAlly && obj.Name == "TheDoomBall")
            {
                BallPos = obj.Position;
            }
        }

        private bool HardCC(Obj_AI_Hero target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned )
            {
                return true;

            }
            else
                return false;
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

        private void Drawing_OnDraw(EventArgs args)
        {
            if (BallPos.IsValid())
            {
                if (MainMenu.Item("wRange", true).GetValue<bool>())
                {
                    if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                    {
                        if (W.IsReady())
                            Utility.DrawCircle(BallPos, W.Range, System.Drawing.Color.Orange, 1, 1);
                    }
                    else
                        Utility.DrawCircle(BallPos, W.Range, System.Drawing.Color.Orange, 1, 1);
                }

                if (MainMenu.Item("rRange", true).GetValue<bool>())
                {
                    if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                    {
                        if (R.IsReady())
                            Utility.DrawCircle(BallPos, R.Range, System.Drawing.Color.Gray, 1, 1);
                    }
                    else
                        Utility.DrawCircle(BallPos, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
            }

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
        }
    }
}
