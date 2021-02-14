using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Kalista : Base
    {
        private int wCount = 0;
        private float grabTime = Game.Time, lastecast = 0f;
        private static Obj_AI_Hero AllyR;

        public Kalista()
        {
            Q = new Spell(SpellSlot.Q, 1150);
            Q1 = new Spell(SpellSlot.Q, 1150);
            W = new Spell(SpellSlot.W, 5000);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 1200f);

            Q.SetSkillshot(0.1f, 40f, 2100f, true, SkillshotType.SkillshotLine);
            Q1.SetSkillshot(0.1f, 40f, 2100f, false, SkillshotType.SkillshotLine);

            LoadMenuOKTW();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void LoadMenuOKTW()
        {

            
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eDamage", "E damage %", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("qMana", "Q harass mana %", true).SetValue(new Slider(50, 100, 0)));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("qMode", "Q combo mode", true).SetValue(new StringList(new[] { "Always", "OKTW logic" }, 1)));

            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("countE", "Auto E if x stacks", true).SetValue(new Slider(10, 30, 0)));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("Edmg", "E % dmg adjust", true).SetValue(new Slider(100, 150, 50)));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("Edead", "Cast E before Kalista dead", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("Ekillmin", "Cast E minion kill + harras target", true).SetValue(true));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("Wdragon", "Auto W Dragon, Baron, Blue, Red", true).SetValue(true));
            HeroMenu.AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("Balista Config").AddItem(new MenuItem("balista", "Balista R", true).SetValue(true));
            HeroMenu.SubMenu("Balista Config").AddItem(new MenuItem("rangeBalista", "Balista min range", true).SetValue(new Slider(300, 1400, 0)));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmEcount", "Auto E if x minions", true).SetValue(new Slider(2, 10, 1)));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQcount", "Lane clear Q if x minions", true).SetValue(new Slider(2, 10, 1)));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("minionE", "Auto E big minion", true).SetValue(true));
            HeroMenu.SubMenu("Farm").SubMenu("E Config").AddItem(new MenuItem("jungleE", "Jungle ks E", true).SetValue(true));
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "KalistaExpungeWrapper")
                {
                    lastecast = Game.Time;
                }
                if (args.SData.Name == "kalistaw")
                {
                    wCount++;
                }
            }

            if (R.IsReady() && sender.IsAlly && !sender.IsMinion && args.Target == null && args.SData.Name == "RocketGrab" && Player.Distance(sender.Position) < R.Range && Player.Distance(sender.Position) > MainMenu.Item("rangeBalista", true).GetValue<Slider>().Value)
            {
                grabTime = Game.Time;
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.IsReady() && Player.Mana > QMANA + EMANA)
            {
                var t = gapcloser.Sender;
                if (t.IsValidTarget(Q.Range) )
                    Q.Cast(t);
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.HasBuff("Recall") || Player.IsRecalling())
                return;

            if (R.IsReady() && AllyR != null && MainMenu.Item("balista", true).GetValue<bool>() &&  AllyR.IsVisible && AllyR.Distance(Player.Position) < R.Range && AllyR.ChampionName == "Blitzcrank" && Player.Distance(AllyR.Position) > MainMenu.Item("rangeBalista", true).GetValue<Slider>().Value)
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget() && !enemy.IsDead && enemy.HasBuff("rocketgrab2")))
                {
                    R.Cast();
                }
                if (Game.Time - grabTime < 1)
                    return;
            }

            SurvivalLogic();

            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (E.IsReady())
            {
                LogicE();
                JungleE();
            }

            if (Program.LagFree(1) && Q.IsReady() && !Player.IsWindingUp && !Player.IsDashing())
                LogicQ();
            if (Program.LagFree(2) && Program.LaneClear && Q.IsReady() && !Player.IsWindingUp && !Player.IsDashing() && MainMenu.Item("farmQ", true).GetValue<bool>())
                FarmQ();
            if (Program.LagFree(4) && W.IsReady() && Program.None && !Player.IsWindingUp && !Player.IsDashing())
                LogicW();
            if (R.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                LogicR();
        }

        private void SurvivalLogic()
        {
            if (E.IsReady() && Player.HealthPercent < 50 && MainMenu.Item("Edead", true).GetValue<bool>() )
            {
                double dmg = OktwCommon.GetIncomingDamage(Player, 0.5F);
                if (dmg > 0)
                {
                    if (Player.Health - dmg < Player.CountEnemiesInRange(700) * Player.Level * 5)
                        CastE();
                    else if (Player.Health - dmg < Player.Level * 5)
                        CastE();
                }
            }

            if (R.IsReady())
            {
                if (R.IsReady() && AllyR != null && AllyR.IsVisible && AllyR.HealthPercent < 50 && AllyR.Distance(Player.Position) < R.Range)
                {
                    double dmg = OktwCommon.GetIncomingDamage(AllyR, 1);
                    if (dmg > 0)
                    {
                        if (AllyR.Health - dmg < AllyR.CountEnemiesInRange(700) * AllyR.Level * 10)
                            R.Cast();
                        else if (AllyR.Health - dmg < AllyR.Level * 10)
                            R.Cast();
                    }
                }
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget())
            {
                var poutput = Q.GetPrediction(t);
                var col = poutput.CollisionObjects;
                bool cast = true;
                foreach (var colobj in col)
                {
                    if (Q.GetDamage(colobj) < colobj.Health)
                        cast = false;
                }

                var qDmg = OktwCommon.GetKsDamage(t, Q)  + Player.GetAutoAttackDamage(t);
                var eDmg = GetEdmg(t);

                if (qDmg > t.Health && eDmg < t.Health && Player.Mana > QMANA + EMANA)
                    castQ(cast, t);
                else if ((qDmg * 1.1) + eDmg > t.Health && eDmg < t.Health && Player.Mana > QMANA + EMANA && Orbwalking.InAutoAttackRange(t))
                    castQ(cast, t);
                else if (Program.Combo && Player.Mana > RMANA +  QMANA + EMANA )
                {
                    if(MainMenu.Item("qMode", true).GetValue<StringList>().SelectedIndex == 0)
                        castQ(cast, t);
                    else if (!Orbwalking.InAutoAttackRange(t) || CountMeleeInRange(400) > 0)
                        castQ(cast, t);
                }
                else if (Program.Harass && !Orbwalking.InAutoAttackRange(t) && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && !Player.UnderTurret(true) && Player.ManaPercent > MainMenu.Item("qMana", true).GetValue<Slider>().Value)
                    castQ(cast, t);
                if ((Program.Combo || Program.Harass) && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        castQ(cast, t);
                }
            }
        }

        private void FarmQ()
        {
            var minions = Cache.GetMinions(Player.ServerPosition, Q.Range);
            int countMinion = 0;
            Obj_AI_Base bestMinion = null;

                foreach (var minion in minions.Where(minion => minion.HealthPercent < 95 && minion.IsValidTarget(Q.Range) && Q.GetDamage(minion) > minion.Health))
                {
                    var poutput = Q.GetPrediction(minion);
                    var col = poutput.CollisionObjects;

                    if(col.Count == 0)
                        continue;

                    foreach (var colobj in col)
                    {
                        if (Q.GetDamage(colobj) > colobj.Health)
                        {
                            countMinion++;
                            bestMinion = minion;
                        }
                        else
                        {
                            countMinion = 0;
                            bestMinion = null;
                            continue;
                        }
                    }
                countMinion = countMinion / 3;
                countMinion += 1;
            }
            if (bestMinion != null && countMinion >= MainMenu.Item("farmQcount", true).GetValue<Slider>().Value)
                Q1.Cast(bestMinion);
        
        }

        private void LogicE()
        {
            var countE = MainMenu.Item("countE", true).GetValue<Slider>().Value;
            bool eBigMinion = MainMenu.Item("minionE", true).GetValue<bool>();
            int count = 0;
            int outRange = 0;


            var minions = Cache.GetMinions(Player.ServerPosition, E.Range - 50);

            foreach (var minion in minions.Where(minion => minion.IsValidTarget(E.Range) && minion.HealthPercent < 80))
            {
                var eDmg = E.GetDamage(minion);
                if (minion.Health < eDmg - minion.HPRegenRate && eDmg > 0)
                {
                    if (GetPassiveTime(minion) > 0.5 && HealthPrediction.GetHealthPrediction(minion, 300) > minion.GetAutoAttackDamage(minion) && !minion.HasBuff("kindredrnodeathbuff"))
                    {
                        count++;
                        if (!Orbwalking.InAutoAttackRange(minion))
                        {
                            outRange++;
                        }
                        if (eBigMinion)
                        {
                            var minionName = minion.BaseSkinName.ToLower();
                            if (minionName.Contains("siege") || minionName.Contains("super"))
                            {
                                outRange++;
                            }
                        }
                    }
                }
            }

            bool near700 = Player.CountEnemiesInRange(700) == 0;

            foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(E.Range) && target.HasBuff("kalistaexpungemarker") && OktwCommon.ValidUlt(target)))
            {
                var eDmg = GetEdmg(target);
                if (target.Health < eDmg)
                {
                    CastE();
                }
                if (0 < eDmg && count > 0 && MainMenu.Item("Ekillmin", true).GetValue<bool>())
                {
                    CastE();

                }
                if (GetEStacks(target) >= countE && (GetPassiveTime(target) < 1 || near700) && Player.Mana > RMANA + EMANA)
                {
                    CastE();
                }
            }

            if (Program.Farm && count > 0 && MainMenu.Item("farmE", true).GetValue<bool>())
            {
                if (outRange > 0)
                {
                    CastE();
                }
                if ((count >= MainMenu.Item("farmEcount", true).GetValue<Slider>().Value || ((Player.UnderTurret(false) && !Player.UnderTurret(true)) && Player.Mana > RMANA + QMANA + EMANA)))
                {
                    CastE();
                }
            }
        }

        private void LogicW()
        {
            if (MainMenu.Item("Wdragon", true).GetValue<bool>() &&  !Orbwalker.GetTarget().IsValidTarget() && !Program.Combo && Player.CountEnemiesInRange(800)==0)
            {
                if (wCount > 0)
                {
                    Vector3 baronPos;
                    baronPos.X = 5232;
                    baronPos.Y = 10788;
                    baronPos.Z = 0;
                    if (Player.Distance(baronPos) < 5000)
                        W.Cast(baronPos);
                }
                if (wCount == 0)
                {
                    Vector3 dragonPos;
                    dragonPos.X = 9919f;
                    dragonPos.Y = 4475f;
                    dragonPos.Z = 0f;
                    if (Player.Distance(dragonPos) < 5000)
                        W.Cast(dragonPos);
                    else
                        wCount ++;
                    return;
                }

                if (wCount == 1)
                {
                    Vector3 redPos;
                    redPos.X = 8022;
                    redPos.Y = 4156;
                    redPos.Z = 0;
                    if (Player.Distance(redPos) < 5000)
                        W.Cast(redPos);
                    else
                        wCount++;
                    return;
                }
                if (wCount == 2)
                {
                    Vector3 bluePos;
                    bluePos.X = 11396;
                    bluePos.Y = 7076;
                    bluePos.Z = 0;
                    if (Player.Distance(bluePos) < 5000)
                        W.Cast(bluePos);
                    else
                        wCount++;
                    return;
                }
                if (wCount > 2)
                {
                    wCount = 0;
                }
            }
        }

        private void JungleE()
        {

            if (!MainMenu.Item("jungleE", true).GetValue<bool>())
                return;

            var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                var dmg = GetEdmg(mob);


                if (mob.Name.Contains("Baron") && Player.HasBuff("barontarget"))
                {
                    dmg = dmg * 0.5f;
                }
                if (mob.Name.Contains("Dragon") && Player.HasBuff("s5test_dragonslayerbuff"))
                {
                    dmg = dmg * (1 - (0.07f * ObjectManager.Player.GetBuffCount("s5test_dragonslayerbuff")));
                }

                if (mob.Health < dmg)
                    CastE();
            }
        }

        private float GetEdmg( Obj_AI_Base t)
        {

            var eDamage = E.GetDamage(t);

            if (Player.HasBuff("summonerexhaust"))
                eDamage = eDamage * 0.6f;

            if (t.HasBuff("ferocioushowl"))
                eDamage = eDamage * 0.7f;

            if (t is Obj_AI_Hero)
            {
                var champion = (Obj_AI_Hero)t;
                if (champion.ChampionName == "Blitzcrank" && !champion.HasBuff("BlitzcrankManaBarrierCD") && !champion.HasBuff("ManaBarrier"))
                {
                    eDamage -= champion.Mana / 2f;
                }
            }

            eDamage -= t.HPRegenRate;
            eDamage -= 0 * 0.005f * t.FlatPhysicalDamageMod;
            eDamage = eDamage * 0.01f * (float)MainMenu.Item("Edmg", true).GetValue<Slider>().Value;

            return eDamage;
        }

        private int CountMeleeInRange(float range)
        {
            int count = 0;
            foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(range) && target.IsMelee))
            {
                count++;
            }
            return count;
        }

        private void CastE()
        {
            if (Game.Time - lastecast < 0.4)
            {
                return;
            }
            else
            { 
                
                E.Cast();
            }
        }

        void castQ(bool cast, Obj_AI_Base t)
        {
            if (cast)
                Program.CastSpell(Q1, t);
            else
                Program.CastSpell(Q, t);
        }

        private float GetPassiveTime(Obj_AI_Base target)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Name == "kalistaexpungemarker")
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault() - Game.Time;
        }

        private int GetEStacks(Obj_AI_Base target)
        {
            foreach (var buff in target.Buffs)
            {
                if (buff.Name == "kalistaexpungemarker")
                    return buff.Count;
            }
            return 0;
        }

        private void LogicR()
        {

            if (AllyR == null)
            {
                foreach (var ally in HeroManager.Allies.Where(ally => !ally.IsDead && !ally.IsMe && ally.HasBuff("kalistacoopstrikeally")))
                {
                    AllyR = ally;
                    break;
                }
            }
            else if (AllyR.IsVisible && AllyR.Distance(Player.Position) < R.Range)
            {
                if (AllyR.Health < AllyR.CountEnemiesInRange(600) * AllyR.Level * 30)
                {
                    R.Cast();
                }
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

        public static void drawText(string msg, Obj_AI_Base Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - 200, color, msg);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("eDamage", true).GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(target => target.IsValidTarget(E.Range + 500) && target.IsEnemy))
                {
                    float hp = enemy.Health - E.GetDamage(enemy);
                    int stack = GetEStacks(enemy);
                    float dmg = (float)Player.GetAutoAttackDamage(enemy) * 2f;
                    if (stack > 0)
                        dmg = (float)Player.GetAutoAttackDamage(enemy) + (E.GetDamage(enemy) / (float)stack);

                    if (hp > 0)
                        drawText((int)((E.GetDamage(enemy) / enemy.Health) * 100) + " %", enemy, System.Drawing.Color.GreenYellow);
                    else
                        drawText("KILL E", enemy, System.Drawing.Color.Red);
                }
            }
            var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                drawText((int)((E.GetDamage(mob) / mob.Health) * 100) + " %", mob, System.Drawing.Color.GreenYellow);

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
