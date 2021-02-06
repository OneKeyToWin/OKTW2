using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Tristana : Base
    {
        public Tristana()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 620);
            R = new Spell(SpellSlot.R, 620);

            W.SetSkillshot(0.35f, 250f, 1400f, false, SkillshotType.SkillshotCircle);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eInfo", "E info", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("nktdE", "NoKeyToDash", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("Wks", "W KS logic (W+E+R calculation)", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("smartW", "SmartCast W key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("Eturet", "E on turrent laneclear", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("focusE", "Focus target with E", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("Use E on").AddItem(new MenuItem("useEon" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R KS (E+R calculation)", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("turrentR", "Try R under turrent", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("allyR", "Try R under ally", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("OnInterruptableSpell", "OnInterruptableSpell", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "OneKeyToCast R closest person", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("GapCloser & anti-meele").AddItem(new MenuItem("GapCloser" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("GapCloser & anti-meele").AddItem(new MenuItem("RgapHP", " use gapcloser only under % hp", true).SetValue(new Slider(40, 100, 0)));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungle", "Jungle Farm", true).SetValue(true));
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += afterAttack;
            Interrupter2.OnInterruptableTarget +=Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target is Obj_AI_Hero)
            {
                var t = (Obj_AI_Hero)target;
                if (t.IsValidTarget())
                {
                    if (Program.Combo)
                        Q.Cast();
                    else if (Program.Harass && Config.Item("harassQ", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>())
                        Q.Cast();
                }
            }
            else if (target is Obj_AI_Minion && FarmSpells && Config.Item("farmQ", true).GetValue<bool>() && OktwCommon.CountEnemyMinions(Player, 700) > 2)
                Q.Cast();
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!E.IsReady())
                return;
            //Eturet

            var t = args.Target as Obj_AI_Hero;
            if (t != null && t.IsValidTarget())
            {
                if (t.IsValidTarget())
                {
                    if (E.GetDamage(t) > t.Health)
                    {
                        E.Cast(t);
                        if(!Player.HasBuff("itemstatikshankcharge"))
                            args.Process = false;
                    }
                    else if (R.IsReady() && E.GetDamage(t) + R.GetDamage(t) > t.Health && Player.Mana > RMANA + EMANA)
                    {
                        E.Cast(t);
                        if (!Player.HasBuff("itemstatikshankcharge"))
                            args.Process = false;
                    }
                    else if (Program.Combo && Player.Mana > RMANA + EMANA && Config.Item("useEon" + t.ChampionName).GetValue<bool>())
                    {
                        E.Cast(t);
                        if (!Player.HasBuff("itemstatikshankcharge"))
                            args.Process = false;
                    }
                    else if (Program.Harass && Player.Mana > RMANA + EMANA + WMANA + RMANA && Config.Item("harassE", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>())
                    {
                        E.Cast(t);
                        if (!Player.HasBuff("itemstatikshankcharge"))
                            args.Process = false;
                    }
                }
            }

            var tur = args.Target as Obj_AI_Turret;
            if (LaneClear && tur != null && tur.IsValidTarget() && Player.Mana > RMANA + EMANA + WMANA + RMANA)
            {
                E.Cast(tur);
            }
    }

        private void Game_OnUpdate(EventArgs args)
        {

            if (Config.Item("focusE", true).GetValue<bool>())
            {
                var eTarget = HeroManager.Enemies.FirstOrDefault(target => target.IsValidTarget() && Orbwalking.InAutoAttackRange(target) && target.HasBuff("tristanaechargesound"));
                if(eTarget != null)
                {
                    Orbwalker.ForceTarget(eTarget);
                }
            }

            if (W.IsReady() && Config.Item("smartW", true).GetValue<KeyBind>().Active)
            {
                W.Cast(Game.CursorPos);
            }

            if (Program.LagFree(1))
            {
                var lvl = 7 * (Player.Level - 1);

                E.Range = 620 + lvl;
                R.Range = 620 + lvl;

                SetMana();
                Jungle();
            }

            if ((Program.LagFree(4) || Program.LagFree(2)) && R.IsReady() )
                LogicR();
            if (Program.LagFree(3) && W.IsReady() && !Player.IsWindingUp)
                LogicW();
        }

        private void LogicW()
        {
            if (Config.Item("Wks", true).GetValue<bool>() && Player.Mana > RMANA + WMANA)
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && OktwCommon.ValidUlt(enemy) && enemy.CountEnemiesInRange(800) < 2 && enemy.CountAlliesInRange(400) == 0  && enemy.Health > enemy.Level * 2))
                {
                    var playerAaDmg = Player.GetAutoAttackDamage(enemy);
                    var dmgCombo = playerAaDmg + OktwCommon.GetKsDamage(enemy, W) + GetEDmg(enemy);
                    
                    if (dmgCombo > enemy.Health)
                    {
                        if (Orbwalking.InAutoAttackRange(enemy))
                        {
                            if (playerAaDmg * 2 + GetEDmg(enemy) < HealthPrediction.GetHealthPrediction(enemy,700))
                                Program.CastSpell(W, enemy);
                        }
                        else
                        {
                            if (playerAaDmg + GetEDmg(enemy) < HealthPrediction.GetHealthPrediction(enemy, 700))
                                Program.CastSpell(W, enemy);
                        }
                    }
                    else if (R.IsReady() && R.GetDamage(enemy) + dmgCombo > enemy.Health && Player.Mana > RMANA + WMANA)
                        Program.CastSpell(W, enemy);
                }
            }
        }

        private void LogicR()
        {
            Obj_AI_Hero bestEnemy = null;
            float pushDistance = 400 + (R.Level * 200);
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && OktwCommon.ValidUlt(enemy)))
            {
                if (bestEnemy == null)
                    bestEnemy = enemy;
                else if (Player.Distance(enemy.Position) < Player.Distance(bestEnemy.Position))
                    bestEnemy = enemy;

                if (OktwCommon.GetKsDamage(enemy,R) + GetEDmg(enemy) > enemy.Health  && GetEDmg(enemy) < enemy.Health && Config.Item("autoR", true).GetValue<bool>())
                {
                    R.Cast(enemy);
                    Program.debug("R ks");
                }
                
                var prepos = Prediction.GetPrediction(enemy, 0.4f);
                var finalPosition = prepos.CastPosition.Extend(Player.Position, -pushDistance);

                if (Config.Item("turrentR", true).GetValue<bool>())
                {
                    if (!finalPosition.UnderTurret(true) && finalPosition.UnderTurret(false) && !Player.UnderTurret(false))
                    {
                        R.Cast(enemy);
                        Program.debug("R turrent");
                    }
                }

                if (Config.Item("allyR", true).GetValue<bool>() && finalPosition.CountAlliesInRange(500) > 1 && enemy.CountAlliesInRange(350) == 0)
                {
                    Program.debug("R ally");

                    R.Cast(enemy);
                }

                if (Player.HealthPercent < Config.Item("RgapHP", true).GetValue<Slider>().Value && enemy.IsValidTarget(270) && enemy.IsMelee && Config.Item("GapCloser" + enemy.ChampionName).GetValue<bool>())
                {
                    R.Cast(enemy);
                    Program.debug("R Meele");
                }

            }
            if (Config.Item("useR", true).GetValue<KeyBind>().Active && bestEnemy!=null)
            {
                R.Cast(bestEnemy);
            }
        }

        private void Jungle()
        {
            if (!Config.Item("jungle", true).GetValue<bool>() || !Program.LaneClear)
                return;
            var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (Player.Mana > RMANA + EMANA + WMANA + RMANA)
                    E.Cast(mob);
                if (Q.IsReady())
                    Q.Cast();
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && Config.Item("OnInterruptableSpell", true).GetValue<bool>())
            {
                if (sender.IsValidTarget(R.Range))
                {
                    R.Cast(sender);
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && Player.HealthPercent < Config.Item("RgapHP", true).GetValue<Slider>().Value)
            {
                var t = gapcloser.Sender;
                if (t.IsValidTarget(R.Range) && Config.Item("GapCloser" + t.ChampionName).GetValue<bool>())
                {
                    R.Cast(t);
                }
            }
        }

        private float GetEDmg(Obj_AI_Base target)
        {
            if (!target.HasBuff("tristanaechargesound"))
                return 0;
            var dmg = E.GetDamage(target);
            var buffCount = OktwCommon.GetBuffCount(target, "tristanaecharge");
            dmg += (dmg * 0.3f * (buffCount - 1));
            return dmg - (target.HPRegenRate * 4);
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
                RMANA = EMANA - Player.PARRegenRate * E.Instance.Cooldown;
            else
                RMANA = R.Instance.ManaCost;
        }

        public static void drawText2(string msg, Vector3 Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - 200, color, msg);
        }

        public static void drawText(string msg, Obj_AI_Hero Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1], color, msg);
        }

        private void Drawing_OnDraw(EventArgs args)
        {

            if (Config.Item("eInfo", true).GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(2000)))
                {
                    if (GetEDmg(enemy) > enemy.Health)
                        drawText("IS DEAD", enemy, System.Drawing.Color.Yellow);
                    if (enemy.HasBuff("tristanaechargesound"))
                        drawText2("E:  " + String.Format("{0:0.0}", OktwCommon.GetPassiveTime(enemy, "tristanaechargesound")), enemy.Position, System.Drawing.Color.Yellow);
                }
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
