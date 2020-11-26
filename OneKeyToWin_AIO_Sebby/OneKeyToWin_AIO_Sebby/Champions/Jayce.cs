using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Jayce : Base
    {
        private Spell  Qext, QextCol;
        private float  QMANA2 = 0, WMANA2 = 0, EMANA2 = 0;
        private float Qcd, Wcd, Ecd, Q2cd, W2cd, E2cd;
        private float Qcdt, Wcdt, Ecdt, Q2cdt, W2cdt, E2cdt;
        private Vector3 EcastPos;
        private int Etick = 0;
        public int Muramana = 3042;
        public int Tear = 3070;
        public int Manamune = 3004;


        public Jayce()
        {
            #region SET SKILLS
            Q = new Spell(SpellSlot.Q, 1030);
            Qext = new Spell(SpellSlot.Q, 1650);
            QextCol = new Spell(SpellSlot.Q, 1650);
            Q1 = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            W1 = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 650);
            E1 = new Spell(SpellSlot.E, 240);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 70, 1450, true, SkillshotType.SkillshotLine);
            Qext.SetSkillshot(0.30f, 80, 2000, false, SkillshotType.SkillshotLine);
            QextCol.SetSkillshot(0.30f, 100, 1600, true, SkillshotType.SkillshotLine);
            Q1.SetTargetted(0.25f, float.MaxValue);
            E.SetSkillshot(0.1f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E1.SetTargetted(0.25f, float.MaxValue);
            #endregion

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("showcd", "Show cooldown", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification & line", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q range", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQm", "Auto Q melee", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("QEforce", "force E + Q", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("QEsplash", "Q + E splash minion damage", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("QEsplashAdjust", "Q + E splash minion radius", true).SetValue(new Slider(150, 250, 50)));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("useQE", "Semi-manual Q + E near mouse key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W range", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoWm", "Auto W melee", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoWmove", "Disable move if W range active", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E range (Q + E)", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoEm", "Auto E melee", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoEks", "E melee ks only", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("gapE", "Gapcloser R + E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("intE", "Interrupt spells R + Q + E", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R range", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoRm", "Auto R melee", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("stack", "Stack Tear if full mana", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("Harass").AddItem(new MenuItem("harassMana", "Harass Mana", true).SetValue(new Slider(80, 100, 0)));

            Config.SubMenu(Player.ChampionName).AddItem(new MenuItem("flee", "FLEE MODE", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q + E range", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W range && mele", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleR", "Jungle clear R", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQm", "Jungle clear Q melee", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleWm", "Jungle clear W melee", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleEm", "Jungle clear E melee", true).SetValue(true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.Item("gapE", true).GetValue<bool>() || E2cd > 0.1)
                return;

            if(Range && !R.IsReady())
                return;

            var t = gapcloser.Sender;

            if (t.IsValidTarget(400))
            {
                if (Range)
                {
                    R.Cast();
                }
                else
                    E.Cast(t);
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.Item("intE", true).GetValue<bool>() || E2cd > 0.1)
                return;

            if (Range && !R.IsReady())
                return;

            if (t.IsValidTarget(300))
            {
                if (Range)
                {
                    R.Cast();
                }
                else 
                    E.Cast(t);

            }
            else if (Q2cd < 0.2 && t.IsValidTarget(Q1.Range))
            {
                if (Range)
                {
                    R.Cast();
                }
                else
                {
                    Q.Cast(t);
                    if(t.IsValidTarget(E1.Range))
                        E.Cast(t);
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                if (W.IsReady() && !Range && Player.Mana > 80)
                    W.Cast();
                if (E.IsReady() && Range && Config.Item("QEforce", true).GetValue<bool>())
                    E.Cast(Player.ServerPosition.Extend(args.EndPosition, 120));
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower() == "jayceshockblast" )
            {
                if (Range && E.IsReady() && Config.Item("autoE", true).GetValue<bool>())
                {
                    EcastPos = Player.ServerPosition.Extend(args.End, 130 + (Game.Ping /2));
                    Etick = Utils.TickCount;
                    E.Cast(EcastPos);

                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (W.IsReady() && Config.Item("autoW", true).GetValue<bool>() && Range && args.Target is Obj_AI_Hero)
            {
                if(Program.Combo)
                    W.Cast();
                else if (args.Target.Position.Distance(Player.Position)< 500)
                    W.Cast();
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Range && E.IsReady() && Utils.TickCount - Etick < 250 + Game.Ping)
            {
                E.Cast(EcastPos);
            }

            if (Config.Item("flee", true).GetValue<KeyBind>().Active)
            {
                FleeMode();
            }

            if (Range)
            {
                
                if (Config.Item("autoWmove", true).GetValue<bool>() && Orbwalker.GetTarget() != null && Player.HasBuff("jaycehyperchargevfx"))
                    Orbwalking.Move = false;
                else
                    Orbwalking.Move = true;

                if (Program.LagFree(1) && Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                    LogicQ();

                if (Program.LagFree(2) && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
                    LogicW();
            }
            else
            {
                Orbwalking.Move = true;
                if (Program.LagFree(1) && E1.IsReady() && Config.Item("autoEm", true).GetValue<bool>())
                    LogicE2();

                if (Program.LagFree(2) && Q1.IsReady() && Config.Item("autoQm", true).GetValue<bool>())
                    LogicQ2();
                if (Program.LagFree(3) && W1.IsReady() && Config.Item("autoWm", true).GetValue<bool>())
                    LogicW2();
            }

            if (Program.LagFree(4))
            {
                if (Program.None && Config.Item("stack", true).GetValue<bool>()  && !Player.HasBuff("Recall") && Player.Mana > Player.MaxMana * 0.90 &&  (Items.HasItem(Tear) || Items.HasItem(Manamune)))
                {
                    if(Utils.TickCount - Q.LastCastAttemptT > 4200 && Utils.TickCount - W.LastCastAttemptT > 4200 && Utils.TickCount - E.LastCastAttemptT > 4200)
                    {
                        if (Range)
                        {
                            if (W.IsReady())
                                W.Cast();
                            else if (E.IsReady() && (Player.InFountain() || Player.IsMoving))
                                E.Cast(Player.ServerPosition);
                            else if (Q.IsReady() && !E.IsReady())
                                Q.Cast(Player.Position.Extend(Game.CursorPos, 500));
                            else if (R.IsReady() && Player.InFountain())
                                R.Cast();
                        }
                        else
                        {
                            if (W.IsReady())
                                W.Cast();
                            else if (R.IsReady() && Player.InFountain())
                                R.Cast();
                        }
                    }
                }

                SetValue();
                if(R.IsReady())
                    LogicR();
            }

            Jungle();
            LaneClearLogic();
        }

        private void FleeMode()
        {
            if (Range)
            {
                if (E.IsReady())
                    E.Cast(Player.Position.Extend(Game.CursorPos, 150));
                else if (R.IsReady())
                    R.Cast();
            }
            else
            {
                if (Q1.IsReady())
                {
                    var mobs = Cache.GetMinions(Player.ServerPosition, Q1.Range);
                    

                    if (mobs.Count > 0)
                    {
                        Obj_AI_Base best;
                        best = mobs[0];

                        foreach (var mob in mobs.Where(mob => mob.IsValidTarget(Q1.Range)))
                        {
                            if (mob.Distance(Game.CursorPos) < best.Distance(Game.CursorPos) )
                                best = mob;
                        }
                        if(best.Distance(Game.CursorPos) + 200 < Player.Distance(Game.CursorPos))
                        Q1.Cast(best);
                    }
                    else if (R.IsReady())
                        R.Cast();
                }
                else if (R.IsReady())
                    R.Cast();
            }
            //Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        private void LogicQ()
        {
            var Qtype = Q;
            if (CanUseQE())
            {
                Qtype = Qext;

                if (Config.Item("useQE", true).GetValue<KeyBind>().Active)
                {
                    var mouseTarget = HeroManager.Enemies.Where(enemy =>
                        enemy.IsValidTarget(Qtype.Range)).OrderBy(enemy => enemy.Distance(Game.CursorPos)).FirstOrDefault();

                    if (mouseTarget != null)
                    {
                        CastQ(mouseTarget);
                        return;
                    }
                }
            }

            var t = TargetSelector.GetTarget(Qtype.Range, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget())
            {
                var qDmg = OktwCommon.GetKsDamage(t, Qtype);

                if (CanUseQE())
                {
                    qDmg = qDmg * 1.4f;
                }

                if (qDmg > t.Health)
                    CastQ(t);
                else if (Program.Combo && Player.Mana > EMANA + QMANA)
                    CastQ(t);
                else if (Program.Harass && Player.ManaPercent > Config.Item("harassMana", true).GetValue<Slider>().Value && OktwCommon.CanHarras())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Qtype.Range) && Config.Item("Harass" + enemy.ChampionName).GetValue<bool>()))
                    {
                        CastQ(t);
                    }
                }
                else if ((Program.Combo || Program.Harass) && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Qtype.Range) && !OktwCommon.CanMove(enemy)))
                        CastQ(t);
                }
            }
        }

        private void LogicW()
        {
            if (Program.Combo && R.IsReady() && Range && Orbwalker.GetTarget().IsValidTarget() && Orbwalker.GetTarget() is Obj_AI_Hero)
            {
                W.Cast();
            }
        }

        private void LogicE()
        {
            var t = TargetSelector.GetTarget(E1.Range, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget())
            {
                var qDmg = OktwCommon.GetKsDamage(t, E1);
                if (qDmg > t.Health)
                    E1.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    E1.Cast(t);
            }
        }

        private void LogicQ2()
        {
            var t = TargetSelector.GetTarget(Q1.Range, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, Q1) > t.Health)
                    Q1.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Q1.Cast(t);
            }
        }

        private void LogicW2()
        {
            if (Player.CountEnemiesInRange(300) > 0 && Player.Mana > 80)
                W.Cast();
        }

        private void LogicE2()
        {
            var t = TargetSelector.GetTarget(E1.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, E1) > t.Health)
                    E1.Cast(t);
                else if (Program.Combo && !Config.Item("autoEks", true).GetValue<bool>() && !Player.HasBuff("jaycehyperchargevfx"))
                    E1.Cast(t);
            }
        }

        private void LogicR()
        {
            if (Range && Config.Item("autoRm", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Q1.Range + 200, TargetSelector.DamageType.Physical);
                if (Program.Combo && Qcd > 0.5  && t.IsValidTarget() && ((!W.IsReady() && !t.IsMelee ) || (!W.IsReady() && !Player.HasBuff("jaycehyperchargevfx") && t.IsMelee)))
                {
                    if (Q2cd < 0.5 && t.CountEnemiesInRange(800) < 3)
                        R.Cast();
                    else if (Player.CountEnemiesInRange(300) > 0 && E2cd < 0.5)
                        R.Cast();
                }
            }
            else if (Program.Combo && Config.Item("autoR", true).GetValue<bool>())
            {

                var t = TargetSelector.GetTarget(1400, TargetSelector.DamageType.Physical);
                if(t.IsValidTarget()&& !t.IsValidTarget(Q1.Range + 200) && Q.GetDamage(t) * 1.4 > t.Health && Qcd < 0.5 && Ecd < 0.5)
                {
                    R.Cast();
                }

                if (!Q.IsReady() && (!E.IsReady() || Config.Item("autoEks", true).GetValue<bool>()))
                {
                    R.Cast();
                }   
            }
        }

        private void LaneClearLogic()
        {
            if (!Program.LaneClear)
                return;

            if (Range && Q.IsReady() && E.IsReady() && FarmSpells && Config.Item("farmQ", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = QextCol.GetCircularFarmLocation(minionList, 150);

                if (farmPosition.MinionsHit >= FarmMinions)
                    Q.Cast(farmPosition.Position);
            }

            if (W.IsReady() && FarmSpells && Config.Item("farmW", true).GetValue<bool>())
            {
                if (Range)
                {
                    Program.debug("csa");
                    var mobs = Cache.GetMinions(Player.ServerPosition, 550);
                    if (mobs.Count >= FarmMinions)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var mobs = Cache.GetMinions(Player.ServerPosition, 300);
                    if (mobs.Count >= FarmMinions)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 700, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Range)
                    {
                        if (Q.IsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                        {
                            Q.Cast(mob.ServerPosition);
                            return;
                        }
                        if (W.IsReady() && Config.Item("jungleE", true).GetValue<bool>())
                        {
                            if(Orbwalking.InAutoAttackRange(mob))
                                W.Cast();
                            return;
                        }
                        if (Config.Item("jungleR", true).GetValue<bool>())
                            R.Cast();
                    }
                    else
                    {
                        if (Q1.IsReady() && Config.Item("jungleQm", true).GetValue<bool>() && mob.IsValidTarget(Q1.Range))
                        {
                            Q1.Cast(mob);
                            return;
                        }

                        if (W1.IsReady() && Config.Item("jungleWm", true).GetValue<bool>() )
                        {
                            if(mob.IsValidTarget(300))
                                W.Cast();
                            return;
                        }
                        if (E1.IsReady() && Config.Item("jungleEm", true).GetValue<bool>() && mob.IsValidTarget(E1.Range))
                        {
                            if( mob.IsValidTarget(E1.Range))
                                E1.Cast(mob);
                            return;
                        }
                        if (Config.Item("jungleR", true).GetValue<bool>())
                            R.Cast();
                    }
                }
            }
        }

        private void CastQ(Obj_AI_Base t)
        {
            if (!CanUseQE())
            {
                Program.CastSpell(Q, t);
                return; 
            }

            bool cast = true;

            if (Config.Item("QEsplash", true).GetValue<bool>())
            {
                var poutput = QextCol.GetPrediction(t);

                foreach (var minion in poutput.CollisionObjects.Where(minion => minion.IsEnemy && minion.Distance(poutput.CastPosition) > Config.Item("QEsplashAdjust", true).GetValue<Slider>().Value))
                {
                    cast = false;
                    break;
                }
            }
            else
                cast = false;

            if (cast)
                Program.CastSpell(Qext, t);
            else
                Program.CastSpell(QextCol, t);

        }

        private float GetComboDMG(Obj_AI_Base t)
        {
            float comboDMG = 0;

            if (Qcd < 1 && Ecd < 1)
                comboDMG = Q.GetDamage(t) * 1.4f;
            else if (Qcd < 1)
                comboDMG = Q.GetDamage(t);

            if (Q2cd < 1)
                comboDMG = Q.GetDamage(t, 1);

            if (Wcd < 1)
                comboDMG += (float)Player.GetAutoAttackDamage(t) * 3;

            if (W2cd < 1)
                comboDMG += W.GetDamage(t) * 2;

            if (E2cd < 1)
                comboDMG += E.GetDamage(t) * 3;
            return comboDMG;
        }

        private bool CanUseQE()
        {
            if(E.IsReady() && Player.Mana > QMANA + EMANA && Config.Item("autoE", true).GetValue<bool>())
                return true;
            else
                return false;
        }

        private float SetPlus(float valus)
        {
            if (valus < 0)
                return 0;
            else
                return valus;
        }

        private void SetValue()
        {
            if (Range)
            {
                Qcdt = Q.Instance.CooldownExpires;
                Wcdt = W.Instance.CooldownExpires;
                Ecd = E.Instance.CooldownExpires;

                QMANA = Q.Instance.ManaCost;
                WMANA = W.Instance.ManaCost;
                EMANA = E.Instance.ManaCost;
            }
            else
            {
                Q2cdt = Q.Instance.CooldownExpires;
                W2cdt = W.Instance.CooldownExpires;
                E2cdt = E.Instance.CooldownExpires;

                QMANA2 = Q.Instance.ManaCost;
                WMANA2 = W.Instance.ManaCost;
                EMANA2 = E.Instance.ManaCost;
            }

            Qcd = SetPlus(Qcdt - Game.Time);
            Wcd = SetPlus(Wcdt - Game.Time);
            Ecd = SetPlus(Ecdt - Game.Time);
            Q2cd = SetPlus(Q2cdt - Game.Time);
            W2cd = SetPlus(W2cdt - Game.Time);
            E2cd = SetPlus(E2cdt - Game.Time);
        }

        private bool Range { get { return Q.Instance.Name.ToLower() == "jayceshockblast"; } }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("showcd", true).GetValue<bool>())
            {
                string msg = " ";

                if (Range)
                {
                    msg = "Q " + (int)Q2cd + "   W " + (int)W2cd + "   E " + (int)E2cd;
                    Drawing.DrawText(Drawing.Width * 0.5f - 50, Drawing.Height * 0.3f, System.Drawing.Color.Orange, msg);
                }
                else
                {
                    msg = "Q " + (int)Qcd + "   W " + (int)Wcd + "   E " + (int)Ecd;
                    Drawing.DrawText(Drawing.Width * 0.5f - 50, Drawing.Height * 0.3f, System.Drawing.Color.Aqua, msg);
                }
            }
            

            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                    {
                        if (Range)
                        {
                            if (CanUseQE())
                                Utility.DrawCircle(Player.Position, Qext.Range, System.Drawing.Color.Cyan, 1, 1);
                            else
                                Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                        }
                        else
                            Utility.DrawCircle(Player.Position, Q1.Range, System.Drawing.Color.Orange, 1, 1);
                    }
                }
                else
                {
                    if (Range)
                    {
                        if (CanUseQE())
                            Utility.DrawCircle(Player.Position, Qext.Range, System.Drawing.Color.Cyan, 1, 1);
                        else
                            Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                    }
                    else
                        Utility.DrawCircle(Player.Position, Q1.Range, System.Drawing.Color.Cyan, 1, 1);
                }
            }

            if (Config.Item("noti", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(1600, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var damageCombo = GetComboDMG(t);
                    if (damageCombo > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Combo deal  " + damageCombo + " to " + t.ChampionName);
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }

                }
            }
        }
    }
}
