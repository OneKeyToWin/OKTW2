using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Lucian : Base
    {
        private bool passRdy = false;
        private float castR = Game.Time;
        public static Core.OKTWdash Dash;

        public Lucian()
        {
            Q = new Spell(SpellSlot.Q, 575f);
            Q1 = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1200f);
            R1 = new Spell(SpellSlot.R, 1200f);

            Q1.SetSkillshot(0.40f, 10f, float.MaxValue, true, SkillshotType.SkillshotLine);
            Q.SetTargetted(0.25f, 1600f);
            W.SetSkillshot(0.30f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.1f, 110, 2800, true, SkillshotType.SkillshotLine);
            R1.SetSkillshot(0.1f, 110, 2800, false, SkillshotType.SkillshotLine);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Use Q on minion", true).SetValue(true));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("ignoreCol", "Ignore collision", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("wInAaRange", "Cast only in AA range", true).SetValue(true));

            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("slowE", "Auto SlowBuff E", true).SetValue(true));
            Dash = new Core.OKTWdash(E);

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "LaneClear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "LaneClear W", true).SetValue(true));

            
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell +=Spellbook_OnCastSpell;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                passRdy = true;
            }
        }
       
        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
                return;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "LucianW" || args.SData.Name == "LucianE" || args.SData.Name == "LucianQ")
                {
                    passRdy = true;
                }
                else
                    passRdy = false;

                if (args.SData.Name == "LucianR")
                    castR = Game.Time;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsChannelingImportantSpell() && (int)(Game.Time * 10) % 2 == 0)
            {
                Console.WriteLine("chaneling");
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (R1.IsReady() && Game.Time - castR > 5 && MainMenu.Item("useR", true).GetValue<KeyBind>().Active)
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget(R1.Range))
                {
                    R1.Cast(t);
                    return;
                }
            }
            if (Program.LagFree(0))
            {
                SetMana();
                
            }
            if (Program.LagFree(1) && Q.IsReady() && !passRdy && !SpellLock )
                LogicQ();
            if (Program.LagFree(2) && W.IsReady() && !passRdy && !SpellLock && MainMenu.Item("autoW", true).GetValue<bool>())
                LogicW();
            if (Program.LagFree(3) && E.IsReady() )
                LogicE();
            if (Program.LagFree(4))
            {
                if (R.IsReady() && Game.Time - castR > 5 && MainMenu.Item("autoR", true).GetValue<bool>())
                    LogicR();

                if (!passRdy && !SpellLock)
                    farm();
            }
        }

        private double AaDamage(Obj_AI_Hero target)
        {
            if (Player.Level > 12)
                return Player.GetAutoAttackDamage(target) * 1.3;
            else if (Player.Level > 6)
                return Player.GetAutoAttackDamage(target) * 1.4;
            else if (Player.Level > 0)
                return Player.GetAutoAttackDamage(target) * 1.5;
            return 0;
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var t1 = TargetSelector.GetTarget(Q1.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget(Q.Range))
            {
                if (OktwCommon.GetKsDamage(t, Q) + AaDamage(t) > t.Health)
                    Q.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Q.Cast(t);
                else if (Program.Harass && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + QMANA + EMANA + WMANA)
                    Q.Cast(t);
            }
            else if ((Program.Harass || Program.Combo) && MainMenu.Item("harassQ", true).GetValue<bool>() && t1.IsValidTarget(Q1.Range) && MainMenu.Item("Harass" + t1.ChampionName).GetValue<bool>() && Player.Distance(t1.ServerPosition) > Q.Range + 100)
            {
                if (Program.Combo && Player.Mana < RMANA + QMANA)
                    return;
                if (Program.Harass && Player.Mana < RMANA + QMANA + EMANA + WMANA )
                    return;
                if (!OktwCommon.CanHarras())
                    return;
                var prepos = Prediction.GetPrediction(t1, Q1.Delay); 
                if ((int)prepos.Hitchance < 5)
                    return;
                var distance = Player.Distance(prepos.CastPosition);
                var minions = Cache.GetMinions(Player.ServerPosition, Q.Range);
                
                foreach (var minion in minions.Where(minion => minion.IsValidTarget(Q.Range)))
                {
                    if (prepos.CastPosition.Distance(Player.Position.Extend(minion.Position, distance)) < 25)
                    {
                        Q.Cast(minion);
                        return;
                    }
                }
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (MainMenu.Item("ignoreCol", true).GetValue<bool>() && Orbwalking.InAutoAttackRange(t))
                    W.Collision=false;
                else
                    W.Collision=true;

                var qDmg = Q.GetDamage(t);
                var wDmg = OktwCommon.GetKsDamage(t, W);

                if (Orbwalking.InAutoAttackRange(t))
                {
                    qDmg += (float)AaDamage(t);
                    wDmg += (float)AaDamage(t);
                }

                if (wDmg > t.Health) 
                    Program.CastSpell(W, t);
                else if (wDmg + qDmg > t.Health && Q.IsReady() && Player.Mana > RMANA + WMANA + QMANA)
                    Program.CastSpell(W, t);

                var orbT = Orbwalker.GetTarget() as Obj_AI_Hero;
                if (orbT == null)
                {
                    if (MainMenu.Item("wInAaRange", true).GetValue<bool>())
                    {
                        return;
                    }
                }
                else if (orbT.IsValidTarget())
                {
                    t = orbT;
                }

                
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                    Program.CastSpell(W, t);
                else if (Program.Harass && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && !Player.UnderTurret(true) && Player.Mana > Player.MaxMana * 0.8 && Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA)
                    Program.CastSpell(W, t);
                else if ((Program.Combo || Program.Harass) && Player.Mana > RMANA + WMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
        }
        
        private void LogicR()
        {
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget(R.Range) && t.CountAlliesInRange(500) == 0 && OktwCommon.ValidUlt(t) && !Orbwalking.InAutoAttackRange(t))
            {
                var rDmg = R.GetDamage(t,1) * (10 + 5 * R.Level);

                var tDis = Player.Distance(t.ServerPosition);
                if (rDmg * 0.8 > t.Health && tDis < 700 && !Q.IsReady())
                    R.Cast(t, true, true);
                else if (rDmg * 0.7 > t.Health && tDis < 800)
                    R.Cast(t, true, true);
                else if (rDmg * 0.6 > t.Health && tDis < 900)
                    R.Cast(t, true, true);
                else if (rDmg * 0.5 > t.Health && tDis < 1000)
                    R.Cast(t, true, true);
                else if (rDmg * 0.4 > t.Health && tDis < 1100)
                    R.Cast(t, true, true);
                else if (rDmg * 0.3 > t.Health && tDis < 1200)
                    R.Cast(t, true, true);
                return;
            }
        }

        private void LogicE()
        {
            if (Player.Mana < RMANA + EMANA || !MainMenu.Item("autoE", true).GetValue<bool>())
                return;

            if (HeroManager.Enemies.Any(target => target.IsValidTarget(270) && target.IsMelee))
            {
                var dashPos = Dash.CastDash(true);
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
            else
            {
                if (!Program.Combo || passRdy || SpellLock)
                    return;

                var dashPos = Dash.CastDash();
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
        }

        public void farm()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0 )
                {
                    var mob = mobs[0];
                    if (Q.IsReady())
                    {
                        Q.Cast(mob);
                        return;
                    }

                    if (W.IsReady())
                    {
                        W.Cast(mob);
                        return;
                    }
                }

                if (FarmSpells)
                {
                    
                    if (Q.IsReady() && MainMenu.Item("farmQ", true).GetValue<bool>())
                    {
                        var minions = Cache.GetMinions(Player.ServerPosition, Q1.Range);
                        foreach (var minion in minions)
                        {
                            var poutput = Q1.GetPrediction(minion);
                            var col = poutput.CollisionObjects;
                            
                            if (col.Count() > 2)
                            {
                                var minionQ = col.First();
                                if (minionQ.IsValidTarget(Q.Range))
                                {
                                    Q.Cast(minion);
                                    return;
                                }
                            }
                        }
                    }
                    if (W.IsReady() && MainMenu.Item("farmW", true).GetValue<bool>())
                    {
                        var minions = Cache.GetMinions(Player.ServerPosition, Q1.Range);
                        var Wfarm = W.GetCircularFarmLocation(minions, 150);
                        if (Wfarm.MinionsHit > 3 )
                            W.Cast(Wfarm.Position);
                    }
                }
            }
        }


        private bool SpellLock
        {
            get
            {
                if (Player.HasBuff("lucianpassivebuff"))
                    return true;
                else
                    return false;
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

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - 200, color, msg);
        }

        private void Drawing_OnDraw(EventArgs args)
        {

            if (MainMenu.Item("qRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q1.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q1.Range, System.Drawing.Color.Cyan, 1, 1);
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
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
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
