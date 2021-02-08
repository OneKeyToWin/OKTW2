using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Lux : Base
    {
        private Vector3 Epos = Vector3.Zero;
        private float DragonDmg = 0;
        private double DragonTime = 0;

        public Lux()
        {
            Q = new Spell(SpellSlot.Q, 1175);
            Q1 = new Spell(SpellSlot.Q, 1175);
            W = new Spell(SpellSlot.W, 1175);
            E = new Spell(SpellSlot.E, 1090);
            R = new Spell(SpellSlot.R, 3400);

            Q1.SetSkillshot(0.25f, 70f, 1200f, true, SkillshotType.SkillshotLine);
            Q.SetSkillshot(0.25f, 70f, 1200f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 110f, 1200f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 150f, 1300f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRangeMini", "R range minimap", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true)); 
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            HeroMenu.SubMenu("Q Config").SubMenu("Q Gap Closer").AddItem(new MenuItem("gapQ", "Auto Q Gap Closer", true).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("Q Config").SubMenu("Q Gap Closer").SubMenu("Use on:").AddItem(new MenuItem("Qgap" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("Q Config").SubMenu("Use on:").AddItem(new MenuItem("Qon" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            HeroMenu.SubMenu("E config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(false));
            HeroMenu.SubMenu("E config").AddItem(new MenuItem("autoEcc", "Auto E only CC enemy", true).SetValue(false));
            HeroMenu.SubMenu("E config").AddItem(new MenuItem("autoEslow", "Auto E slow logic detonate", true).SetValue(true));
            HeroMenu.SubMenu("E config").AddItem(new MenuItem("autoEdet", "Only detonate if target in E ", true).SetValue(false));
            
            HeroMenu.SubMenu("W Shield Config").AddItem(new MenuItem("Wdmg", "W dmg % hp", true).SetValue(new Slider(10, 100, 0)));
            foreach (var ally in HeroManager.Allies)
            {
                HeroMenu.SubMenu("W Shield Config").SubMenu("Shield ally").SubMenu(ally.ChampionName).AddItem(new MenuItem("damage" + ally.ChampionName, "Damage incoming", true).SetValue(true));
                HeroMenu.SubMenu("W Shield Config").SubMenu("Shield ally").SubMenu(ally.ChampionName).AddItem(new MenuItem("HardCC" + ally.ChampionName, "Hard CC", true).SetValue(true));
                HeroMenu.SubMenu("W Shield Config").SubMenu("Shield ally").SubMenu(ally.ChampionName).AddItem(new MenuItem("Poison" + ally.ChampionName, "Poison", true).SetValue(true));
            }

            HeroMenu.SubMenu("R config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Rcc", "R fast KS combo", true).SetValue(true));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("RaoeCount", "R x enemies in combo [0 == off]", true).SetValue(new Slider(3, 5, 0)));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("hitchanceR", "Hit Chance R", true).SetValue(new Slider(2, 3, 0)));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space   

            HeroMenu.SubMenu("R config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rjungle", "R Jungle stealer", true).SetValue(true));
            HeroMenu.SubMenu("R config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rdragon", "Dragon", true).SetValue(true));
            HeroMenu.SubMenu("R config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rbaron", "Baron", true).SetValue(true));
            HeroMenu.SubMenu("R config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rred", "Red", true).SetValue(true));
            HeroMenu.SubMenu("R config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rblue", "Blue", true).SetValue(true));
            HeroMenu.SubMenu("R config").SubMenu("R Jungle stealer").AddItem(new MenuItem("Rally", "Ally stealer", true).SetValue(false));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.IsReady() && gapcloser.Sender.IsValidTarget(Q.Range) && MainMenu.Item("gapQ", true).GetValue<bool>() && MainMenu.Item("Qgap" + gapcloser.Sender.ChampionName).GetValue<bool>())
                Q.Cast(gapcloser.Sender);
        }


        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "LuxLightStrikeKugel")
            {
                Epos = args.End;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (R.IsReady() )
            {
                if (MainMenu.Item("Rjungle", true).GetValue<bool>())
                {
                    KsJungle();
                }
                
                if (MainMenu.Item("useR", true).GetValue<KeyBind>().Active)
                {
                    var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                    if (t.IsValidTarget())
                        R.Cast(t, true, true);
                }
            }
            else
                DragonTime = 0; 


            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if ((Program.LagFree(4) || Program.LagFree(1) || Program.LagFree(3)) && W.IsReady() && !Player.IsRecalling())
                LogicW();
            if (Program.LagFree(1) && Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();
            if (Program.LagFree(2) && E.IsReady() && MainMenu.Item("autoE", true).GetValue<bool>())
                LogicE();
            if (Program.LagFree(3) && R.IsReady())
                LogicR();
        }
        private void LogicW()
        {
            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead  && MainMenu.Item("damage" + ally.ChampionName, true).GetValue<bool>() && Player.ServerPosition.Distance(ally.ServerPosition) < W.Range))
            {
                double dmg = OktwCommon.GetIncomingDamage(ally);


                int nearEnemys = ally.CountEnemiesInRange(800);

                if (dmg == 0 && nearEnemys == 0)
                    continue;

                int sensitivity = 20;
                
                double HpPercentage = (dmg * 100) / ally.Health;
                double shieldValue = 65 + W.Level * 25 + 0.35 * Player.FlatMagicDamageMod;

                if (MainMenu.Item("HardCC" + ally.ChampionName, true).GetValue<bool>() && nearEnemys > 0 && HardCC(ally))
                {
                    W.Cast(ally.Position);
                }
                else if (MainMenu.Item("Poison" + ally.ChampionName, true).GetValue<bool>() && ally.HasBuffOfType(BuffType.Poison))
                {
                    W.Cast(W.GetPrediction(ally).CastPosition);
                }

                nearEnemys = (nearEnemys == 0) ? 1 : nearEnemys;

                if (dmg > shieldValue)
                    W.Cast(W.GetPrediction(ally).CastPosition);
                else if (dmg > 100 + Player.Level * sensitivity)
                    W.Cast(W.GetPrediction(ally).CastPosition);
                else if (ally.Health - dmg < nearEnemys * ally.Level * sensitivity)
                    W.Cast(W.GetPrediction(ally).CastPosition);
                else if (HpPercentage >= MainMenu.Item("Wdmg", true).GetValue<Slider>().Value)
                    W.Cast(W.GetPrediction(ally).CastPosition);
            }
        }

        private void LogicQ()
        {
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && E.GetDamage(enemy) + Q.GetDamage(enemy) + BonusDmg(enemy) > enemy.Health))
            {
                CastQ(enemy);
                return;
            }

            var t = Orbwalker.GetTarget() as Obj_AI_Hero;
            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget() && MainMenu.Item("Qon" + t.ChampionName).GetValue<bool>())
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    CastQ(t);
                else if (Program.Harass  && MainMenu.Item("harassQ", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    CastQ(t);
                else if(OktwCommon.GetKsDamage(t,Q) > t.Health)
                    CastQ(t);

                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                    CastQ(enemy);
            }
        }
        
        private void CastQ(Obj_AI_Base t)
        {
            var poutput = Q1.GetPrediction(t);
            var col = poutput.CollisionObjects.Count(ColObj => ColObj.IsEnemy && ColObj.IsMinion && !ColObj.IsDead);

            if (col < 3)
            {

                Program.CastSpell(Q, t);
            }
        }

        private void LogicE()
        {
            if (E.Instance.ToggleState >= 1)
            {
                int eBig = Epos.CountEnemiesInRange(350);
                if (MainMenu.Item("autoEslow", true).GetValue<bool>())
                {
                    int detonate = eBig - Epos.CountEnemiesInRange(160);

                    if (detonate > 0 || eBig > 1)
                        E.Cast();
                }
                else if (MainMenu.Item("autoEdet", true).GetValue<bool>())
                {
                    if (eBig > 0)
                        E.Cast();
                }
                else
                {
                    E.Cast();
                }
            }
            else if (E.Instance.ToggleState == 0)
            {
                var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget() )
                {
                    if (!MainMenu.Item("autoEcc", true).GetValue<bool>())
                    {
                        if (Program.Combo && Player.Mana > RMANA + EMANA)
                            Program.CastSpell(E, t);
                        else if (Program.Harass && OktwCommon.CanHarras() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && MainMenu.Item("harassE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + EMANA + RMANA)
                            Program.CastSpell(E, t);
                        else if (OktwCommon.GetKsDamage(t, E) > t.Health)
                                Program.CastSpell(E, t);
                    }

                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                        E.Cast(enemy, true);
                }
                else if (FarmSpells && MainMenu.Item("farmE", true).GetValue<bool>() && Player.Mana > RMANA + WMANA)
                {
                    var minionList = Cache.GetMinions(Player.ServerPosition, E.Range);
                    var farmPosition = E.GetCircularFarmLocation(minionList, E.Width);

                    if (farmPosition.MinionsHit >= FarmMinions)
                        E.Cast(farmPosition.Position);
                }
            }
        }

        private void LogicR()
        {
            if (MainMenu.Item("autoR", true).GetValue<bool>() )
            {
                foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range) && target.CountAlliesInRange(600) < 2 && OktwCommon.ValidUlt(target)))
                {
                    float predictedHealth = target.Health + target.HPRegenRate * 2;
                    float Rdmg = OktwCommon.GetKsDamage(target, R);

                    if (Items.HasItem(3155, target))
                    {
                        Rdmg = Rdmg - 250;
                    }

                    if (Items.HasItem(3156, target))
                    {
                        Rdmg = Rdmg - 400;
                    }

                    if (target.HasBuff("luxilluminatingfraulein"))
                    {
                        Rdmg +=  (float)Player.CalcDamage(target, Damage.DamageType.Magical,10 + (8 * Player.Level) + 0.2 * Player.FlatMagicDamageMod);
                    }

                    if (Player.HasBuff("itemmagicshankcharge"))
                    {
                        if (Player.GetBuff("itemmagicshankcharge").Count == 100)
                        {
                            Rdmg += (float)Player.CalcDamage(target, Damage.DamageType.Magical, 100 + 0.1 * Player.FlatMagicDamageMod);
                        }
                    }

                    if (Rdmg > predictedHealth )
                    {
                        castR(target);
                        Program.debug("R normal");
                    }
                    else if (!OktwCommon.CanMove(target) && MainMenu.Item("Rcc", true).GetValue<bool>() && target.IsValidTarget(E.Range))
                    {
                        float dmgCombo = Rdmg;

                        if (E.IsReady())
                        {
                            var eDmg = E.GetDamage(target);
                            
                            if (eDmg > predictedHealth)
                                return;
                            else
                                dmgCombo += eDmg;
                        }

                        if (target.IsValidTarget(800))
                            dmgCombo += BonusDmg(target);

                        if (dmgCombo > predictedHealth)
                        {
                            R.CastIfWillHit(target, 2);
                            R.Cast(target);
                        }

                    }
                    else if (Program.Combo && MainMenu.Item("RaoeCount", true).GetValue<Slider>().Value > 0)
                    {
                        R.CastIfWillHit(target, MainMenu.Item("RaoeCount", true).GetValue<Slider>().Value);
                    }
                }
            }
        }

        private float BonusDmg(Obj_AI_Hero target)
        {
            float damage = 10 + (Player.Level) * 8 + 0.2f * Player.FlatMagicDamageMod;
            if (Player.HasBuff("lichbane"))
            {
                damage += (Player.BaseAttackDamage * 0.75f) + ((Player.BaseAbilityDamage + Player.FlatMagicDamageMod) * 0.5f);
            }

            return (float)(Player.GetAutoAttackDamage(target) + Player.CalcDamage(target, Damage.DamageType.Magical, damage));
        }

        private void castR(Obj_AI_Hero target)
        {
            var inx = MainMenu.Item("hitchanceR", true).GetValue<Slider>().Value;
            if (inx == 0)
            {
                R.Cast(R.GetPrediction(target).CastPosition);
            }
            else if (inx == 1)
            {
                R.Cast(target);
            }
            else if (inx == 2)
            {
                Program.CastSpell(R, target);
            }
            else if (inx == 3)
            {
                List<Vector2> waypoints = target.GetWaypoints();
                if ((Player.Distance(waypoints.Last<Vector2>().To3D()) - Player.Distance(target.Position)) > 400)
                {
                    Program.CastSpell(R, target);
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }
                    if (E.IsReady() && MainMenu.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private void KsJungle()
        {
            var mobs = Cache.GetMinions(Player.ServerPosition, R.Range, MinionTeam.Neutral);
            foreach (var mob in mobs)
            {
                //debug(mob.SkinName);
                if (((mob.SkinName == "SRU_Dragon" && MainMenu.Item("Rdragon", true).GetValue<bool>())
                    || (mob.SkinName == "SRU_Baron" && MainMenu.Item("Rbaron", true).GetValue<bool>())
                    || (mob.SkinName == "SRU_Red" && MainMenu.Item("Rred", true).GetValue<bool>())
                    || (mob.SkinName == "SRU_Blue" && MainMenu.Item("Rblue", true).GetValue<bool>()))
                    && (mob.CountAlliesInRange(1000) == 0 || MainMenu.Item("Rally", true).GetValue<bool>())
                    && mob.Health < mob.MaxHealth
                    && mob.Distance(Player.Position) > 1000
                    )
                {
                    if (DragonDmg == 0)
                        DragonDmg = mob.Health;

                    if (Game.Time - DragonTime > 3)
                    {
                        if (DragonDmg - mob.Health > 0)
                        {
                            DragonDmg = mob.Health;
                        }
                        DragonTime = Game.Time;
                    }
                    else
                    {
                        var DmgSec = (DragonDmg - mob.Health) * (Math.Abs(DragonTime - Game.Time) / 3);
                        //Program.debug("DS  " + DmgSec);
                        if (DragonDmg - mob.Health > 0)
                        {
                            var timeTravel = R.Delay;
                            var timeR = (mob.Health - R.GetDamage(mob)) / (DmgSec / 3);
                            //Program.debug("timeTravel " + timeTravel + "timeR " + timeR + "d " + R.GetDamage(mob));
                            if (timeTravel > timeR)
                                R.Cast(mob.Position);
                        }
                        else
                            DragonDmg = mob.Health;

                        //Program.debug("" + GetUltTravelTime(ObjectManager.Player, R.Speed, R.Delay, mob.Position));
                    }
                }
            }
        }


        private bool HardCC(Obj_AI_Hero target)
        {
            
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned)
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

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private void Drawing_OnEndScene(EventArgs args)
        {

            if (MainMenu.Item("rRangeMini", true).GetValue<bool>())
            {
                if (R.IsReady())
                    Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);
            }
            else
                Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);


        }

        private void Drawing_OnDraw(EventArgs args)
        {

            if (MainMenu.Item("qRange", true).GetValue<bool>())
            {
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
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
                        Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
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
            if (R.IsReady() && MainMenu.Item("noti", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if ( t.IsValidTarget() && R.GetDamage(t) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                    drawLine(t.Position, Player.Position, 5, System.Drawing.Color.Red);
                }
            }
        }
    }
}
