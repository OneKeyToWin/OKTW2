using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Draven : Base
    {
        private int axeCatchRange;
        private static GameObject RMissile = null;
        public List<GameObject> axeList = new List<GameObject>();

        public Draven()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1050);
            R = new Spell(SpellSlot.R, 3000f);

            E.SetSkillshot(0.25f, 100, 1400, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.4f, 160, 2000, false, SkillshotType.SkillshotLine);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("noti", "Draw R helper", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qCatchRange", "Q catch range", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qAxePos", "Q axe position", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));

            HeroMenu.SubMenu("AXE option").AddItem(new MenuItem("axeCatchRange", "Axe catch range", true).SetValue(new Slider(500, 200, 2000)));
            HeroMenu.SubMenu("AXE option").AddItem(new MenuItem("axeTower", "Don't catch axe under enemy turret combo", true).SetValue(true));
            HeroMenu.SubMenu("AXE option").AddItem(new MenuItem("axeTower2", "Don't catch axe under enemy turret farm", true).SetValue(true));
            HeroMenu.SubMenu("AXE option").AddItem(new MenuItem("axeEnemy", "Don't catch axe in enemy grup", true).SetValue(true));
            HeroMenu.SubMenu("AXE option").AddItem(new MenuItem("axeKill", "Don't catch axe if can kill 2 AA", true).SetValue(true));
            HeroMenu.SubMenu("AXE option").AddItem(new MenuItem("axePro", "if axe timeout: force laneclear", true).SetValue(true));

            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("farmQ", "Farm Q", true).SetValue(true));

            HeroMenu.SubMenu("W config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W config").AddItem(new MenuItem("slowW", "Auto W slow", true).SetValue(true));

            HeroMenu.SubMenu("E config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E config").AddItem(new MenuItem("autoE2", "Harras E if can hit 2 targets", true).SetValue(true));
            HeroMenu.SubMenu("E config").AddItem(new MenuItem("agcE", "On Enemy Gapcloser", true).SetValue(true));
            HeroMenu.SubMenu("E config").AddItem(new MenuItem("intE", "On Interruptable Target", true).SetValue(true));

            HeroMenu.SubMenu("R config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Rdmg", "KS damage calculation", true).SetValue(new StringList(new[] { "X 1", "X 2" }, 1)));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("comboR", "Auto R in combo", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Rcc", "R cc", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Raoe", "R aoe combo", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("hitchanceR", "VeryHighHitChanceR", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space

            GameObject.OnCreate += SpellMissile_OnCreateOld;
            GameObject.OnDelete += Obj_SpellMissile_OnDelete;
            Orbwalking.BeforeAttack += BeforeAttack;
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObjectOnOnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += GameOnOnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (MainMenu.Item("intE", true).GetValue<bool>() && E.IsReady() && sender.IsValidTarget(E.Range))
            {
                E.Cast(sender);
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (MainMenu.Item("agcE", true).GetValue<bool>() && E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private void Obj_SpellMissile_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
                return;

            MissileClient missile = (MissileClient)sender;

            if (missile.IsValid && missile.IsAlly && missile.SData.Name != null && missile.SData.Name == "DravenR")
            {
                RMissile = null;
            }
        }

        private void SpellMissile_OnCreateOld(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
                return;

            MissileClient missile = (MissileClient)sender;

            if (missile.IsValid && missile.IsAlly && missile.SData.Name != null && missile.SData.Name == "DravenR")
            {
                RMissile = sender;
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            //Program.debug("" + OktwCommon.GetBuffCount(Player, "dravenspinningattack"));
            if (Q.IsReady())
            {
                var buffCount = OktwCommon.GetBuffCount(Player, "dravenspinningattack");
                if (MainMenu.Item("autoQ", true).GetValue<bool>() && args.Target.IsValid<Obj_AI_Hero>()  )
                {
                    if (buffCount + axeList.Count == 0)
                        Q.Cast();
                    else if (Player.Mana > RMANA + QMANA && buffCount == 0)
                        Q.Cast();
                }
                if (Program.Farm && MainMenu.Item("farmQ", true).GetValue<bool>() )
                {
                    if (buffCount + axeList.Count == 0 && Player.Mana > RMANA + EMANA + WMANA)
                        Q.Cast();
                    else if (Player.ManaPercent > 70 && buffCount == 0)
                        Q.Cast();
                }
            }
        }

        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("Q_reticle_self"))
            {
                axeList.Add(sender);
            }
        }

        private void GameObjectOnOnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("Q_reticle_self"))
            {
                axeList.Remove(sender);
            }
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            axeList.RemoveAll(x => !x.IsValid);
            if (ObjectManager.Player.HasBuff("Recall"))
                return;
            if (Program.LagFree(1))
            {
                axeCatchRange = MainMenu.Item("axeCatchRange", true).GetValue<Slider>().Value;
                SetMana();
                AxeLogic();
                if (MainMenu.Item("axePro", true).GetValue<bool>() && Player.HasBuff("dravenspinningattack"))
                {
                    var BuffTime = OktwCommon.GetPassiveTime(Player, "dravenspinningattack");
                    if (BuffTime < 1)
                    {
                        Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.LaneClear;
                    }
                    else
                    {
                        Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.None;
                    }
                }
                else
                {
                    Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.None;
                }
            }
            
            //Program.debug("" + OktwCommon.GetBuffCount(Player, "dravenspinningattack"));
            
            if (Program.LagFree(2) && E.IsReady() && MainMenu.Item("autoE", true).GetValue<bool>() )
                LogicE();

            if (Program.LagFree(3) && W.IsReady())
                LogicW();

            if (Program.LagFree(4) && R.IsReady() && !Player.IsWindingUp)
                LogicR();
        }

        private void LogicW()
        {
            if (MainMenu.Item("autoW", true).GetValue<bool>() && Program.Combo && Player.Mana > RMANA + EMANA + WMANA + QMANA && Player.CountEnemiesInRange(1000) > 0 && !Player.HasBuff("dravenfurybuff"))
                W.Cast();
            else if (MainMenu.Item("slowW", true).GetValue<bool>()&& Player.Mana > RMANA + EMANA + WMANA && Player.HasBuffOfType(BuffType.Slow))
                W.Cast();
        }

        private void LogicE()
        {
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !Orbwalking.InAutoAttackRange(enemy) && E.GetDamage(enemy) > enemy.Health))
            {
                Program.CastSpell(E, enemy);
                return;
            }

            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (Program.Combo )
                {
                    if (Player.Mana > RMANA + EMANA)
                    {
                        if (!Orbwalking.InAutoAttackRange(t))
                            Program.CastSpell(E, t);
                        if (Player.Health < Player.MaxHealth * 0.5)
                            Program.CastSpell(E, t);
                    }
                    
                    if(Player.Mana > RMANA + EMANA + QMANA)
                        E.CastIfWillHit(t, 2, true);
                }
                if (Program.Harass && MainMenu.Item("autoE2", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + QMANA)
                {
                    E.CastIfWillHit(t, 2, true);
                }
            }
            foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(E.Range)))
            {
                if (target.IsValidTarget(300) && target.IsMelee)
                {
                    Program.CastSpell(E, t);
                }
            }
        }

        private void LogicR()
        {
            if (MainMenu.Item("useR", true).GetValue<KeyBind>().Active)
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    R.CastIfWillHit(t, 2, true);
                    R.Cast(t, true, true);
                }
            }
            if (MainMenu.Item("autoR", true).GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target) && target.CountAlliesInRange(500) == 0))
                {
                    float predictedHealth = target.Health - (float)OktwCommon.GetIncomingDamage(target);
                    double Rdmg = CalculateR(target) ;

                    if (Rdmg * 2 > predictedHealth && MainMenu.Item("Rdmg", true).GetValue<StringList>().SelectedIndex == 1)
                        Rdmg = Rdmg + getRdmg(target);

                    var qDmg = Q.GetDamage(target);
                    var eDmg = E.GetDamage(target);
                    if (Rdmg > predictedHealth && !Orbwalking.InAutoAttackRange(target))
                    {
                        castR(target);
                        Program.debug("R normal");
                    }
                    else if (Program.Combo && MainMenu.Item("comboR", true).GetValue<bool>() && Orbwalking.InAutoAttackRange(target) && Rdmg * 2 + Player.GetAutoAttackDamage(target) > predictedHealth)
                    {
                        castR(target);
                        Program.debug("R normal");
                    }
                    else if (MainMenu.Item("Rcc", true).GetValue<bool>() && Rdmg * 2 > predictedHealth && !OktwCommon.CanMove(target) &&  target.IsValidTarget( E.Range))
                    {
                        R.Cast(target);
                        Program.debug("R normal");
                    }
                    else if (Program.Combo && MainMenu.Item("Raoe", true).GetValue<bool>())
                    {
                        R.CastIfWillHit(target, 3, true);
                    }
                    else if (target.IsValidTarget(E.Range) && Rdmg * 2 + qDmg + eDmg > predictedHealth && MainMenu.Item("Raoe", true).GetValue<bool>())
                    {
                        R.CastIfWillHit(target, 2, true);
                    }
                }
            }
        }

        private void castR(Obj_AI_Hero target)
        {
            if (MainMenu.Item("hitchanceR", true).GetValue<bool>())
            {
                List<Vector2> waypoints = target.GetWaypoints();
                if (target.Path.Count() < 2 && (Player.Distance(waypoints.Last<Vector2>().To3D()) - Player.Distance(target.Position)) > 300)
                {
                    Program.CastSpell(R, target);
                }
            }
            else
                Program.CastSpell(R, target);
        }

        private float CalculateR(Obj_AI_Base target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Physical, (75 + (100 * R.Level)) + Player.FlatPhysicalDamageMod * 1.1);
        }

        private double getRdmg(Obj_AI_Base target)
        {
            var rDmg = R.GetDamage(target);
            var dmg = 0;
            PredictionOutput output = R.GetPrediction(target);
            Vector2 direction = output.CastPosition.To2D() - Player.Position.To2D();
            direction.Normalize();
            List<Obj_AI_Hero> enemies = HeroManager.Enemies.Where(x => x.IsValidTarget()).ToList();
            foreach (var enemy in enemies)
            {
                PredictionOutput prediction = R.GetPrediction(enemy);
                Vector3 predictedPosition = prediction.CastPosition;
                Vector3 v = output.CastPosition - Player.ServerPosition;
                Vector3 w = predictedPosition - Player.ServerPosition;
                double c1 = Vector3.Dot(w, v);
                double c2 = Vector3.Dot(v, v);
                double b = c1 / c2;
                Vector3 pb = Player.ServerPosition + ((float)b * v);
                float length = Vector3.Distance(predictedPosition, pb);
                if (length < (R.Width + 100 + enemy.BoundingRadius / 2) && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                    dmg++;
            }
            var allMinionsR = Cache.GetMinions(Player.ServerPosition, R.Range);
            foreach (var minion in allMinionsR)
            {
                PredictionOutput prediction = R.GetPrediction(minion);
                Vector3 predictedPosition = prediction.CastPosition;
                Vector3 v = output.CastPosition - Player.ServerPosition;
                Vector3 w = predictedPosition - Player.ServerPosition;
                double c1 = Vector3.Dot(w, v);
                double c2 = Vector3.Dot(v, v);
                double b = c1 / c2;
                Vector3 pb = Player.ServerPosition + ((float)b * v);
                float length = Vector3.Distance(predictedPosition, pb);
                if (length < (R.Width + 100 + minion.BoundingRadius / 2) && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                    dmg++;
            }
            //if (Config.Item("debug", true).GetValue<bool>())
            //    Game.PrintChat("R collision" + dmg);

            if (dmg > 8)
                return rDmg * 0.6;
            else
                return rDmg - (rDmg * 0.08 * dmg);
        }

        private void AxeLogic()
        {

            var t = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);

            if (MainMenu.Item("axeKill", true).GetValue<bool>() && t.IsValidTarget() && Player.Distance(t.Position) > 400 && Player.GetAutoAttackDamage(t) * 2 > t.Health)
            {
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                return;
            }
            if (axeList.Count == 0)
            {
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                return;
            }
            
            if (axeList.Count == 1)
            {
                CatchAxe(axeList.First());
                return;
            }
            else
            {
                var bestAxe = axeList.First();
                foreach (var obj in axeList)
                {
                    if (Game.CursorPos.Distance(bestAxe.Position) > Game.CursorPos.Distance(obj.Position))
                        bestAxe = obj;
                }
                CatchAxe(bestAxe);
            }
        }

        private void CatchAxe(GameObject Axe)
        {
            if (Player.Distance(Axe.Position) < 100)
            {
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                return;
            }

            if (MainMenu.Item("axeTower", true).GetValue<bool>() && Program.Combo && Axe.Position.UnderTurret(true))
            {
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                return;
            }

            if (MainMenu.Item("axeTower2", true).GetValue<bool>() && Program.Harass && Axe.Position.UnderTurret(true))
            {
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                return;
            }

            if (MainMenu.Item("axeEnemy", true).GetValue<bool>() && Axe.Position.CountEnemiesInRange(500) > 2)
            {
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                return;
            }

            if (Game.CursorPos.Distance(Axe.Position) < axeCatchRange)
            {
                Orbwalker.SetOrbwalkingPoint(Axe.Position);
            }
            else
            {
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
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
                RMANA = EMANA - Player.PARRegenRate * E.Instance.Cooldown;
            else
                RMANA = R.Instance.ManaCost;
        }

        public static void drawText2(string msg, Vector3 Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - 200, color, msg);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("qAxePos", true).GetValue<bool>())
            {
                if (Player.HasBuff("dravenspinningattack"))
                {
                    var BuffTime = OktwCommon.GetPassiveTime(Player, "dravenspinningattack");
                    if (BuffTime < 2 )
                    {
                        if ((int)(Game.Time * 10) % 2 == 0)
                        {
                            drawText2("Q:  " + String.Format("{0:0.0}", BuffTime), Player.Position, System.Drawing.Color.Yellow);
                        }
                    }
                    else
                    {
                        drawText2("Q:  " + String.Format("{0:0.0}", BuffTime), Player.Position, System.Drawing.Color.GreenYellow);
                    }
                }
                foreach (var obj in axeList)
                {
                    if (Game.CursorPos.Distance(obj.Position) > axeCatchRange || obj.Position.UnderTurret(true))
                    {
                        Utility.DrawCircle(obj.Position, 150, System.Drawing.Color.OrangeRed, 1, 1);
                    }
                    else if (Player.Distance(obj.Position) > 120)
                    {
                        Utility.DrawCircle(obj.Position, 150, System.Drawing.Color.Yellow, 1, 1);
                    }
                    else if (Player.Distance(obj.Position) < 150)
                    {
                        Utility.DrawCircle(obj.Position, 150, System.Drawing.Color.YellowGreen, 1, 1);
                    }
                }
            }

            if (MainMenu.Item("qCatchRange", true).GetValue<bool>())
                Utility.DrawCircle(Game.CursorPos, axeCatchRange, System.Drawing.Color.LightSteelBlue, 1, 1);
            
            if (MainMenu.Item("noti", true).GetValue<bool>() && RMissile != null)
                OktwCommon.DrawLineRectangle(RMissile.Position, Player.Position, (int)R.Width, 1, System.Drawing.Color.White);

            if (MainMenu.Item("eRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
        }
    }
}
