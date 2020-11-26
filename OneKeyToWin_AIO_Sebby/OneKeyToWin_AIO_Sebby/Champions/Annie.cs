using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Annie : Base
    {
        private SpellSlot flash;
        public Obj_AI_Base Tibbers;
        public float TibbersTimer = 0;
        private bool HaveStun = false;
        private Spell FR;

        public Annie()
        {
            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 550f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 625f);
            FR = new Spell(SpellSlot.R, 1000f );

            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.3f, 80f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 180f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            FR.SetSkillshot(0.25f, 180f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            flash = Player.GetSpellSlot("summonerflash");

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E stack stun", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("Ultimate Manager").AddItem(new MenuItem("UM" + enemy.ChampionName, enemy.ChampionName, true).SetValue(new StringList(new[] { "Normal", "Always", "Never", "Always Stun"}, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoRks", "Auto R KS", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoRcombo", "Auto R Combo if stun is ready", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("rCount", "Auto R x enemies", true).SetValue(new Slider(3, 2, 5)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("tibers", "Tibbers Auto Pilot", true).SetValue(true));

            if (flash != SpellSlot.Unknown)
            {
                Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("rCountFlash", "Auto flash + R stun x enemies", true).SetValue(new Slider(4, 2, 5)));
            }

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Farm Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(false));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid && obj.IsAlly && obj is Obj_AI_Minion && obj.Name.ToLower() == "tibbers")
            {
                Tibbers = obj as Obj_AI_Base ;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.HasBuff("Recall"))
                return;

            HaveStun = Player.HasBuff("pyromania_particle");

            SetMana();

            if (R.IsReady() && (Program.LagFree(1) || Program.LagFree(3)) && !HaveTibers)
            {
                var realRange = R.Range;

                if (flash.IsReady())
                    realRange = FR.Range;

                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(realRange) && OktwCommon.ValidUlt(enemy)))
                {
                    if (enemy.IsValidTarget(R.Range))
                    {
                        int Rmode = Config.Item("UM" + enemy.ChampionName, true).GetValue<StringList>().SelectedIndex;

                        if (Rmode == 2)
                            continue;

                        var poutput = R.GetPrediction(enemy, true);
                        var aoeCount = poutput.AoeTargetsHitCount;

                        if (Rmode == 1)
                            R.Cast(poutput.CastPosition);

                        if (Rmode == 3 && HaveStun)
                            R.Cast(poutput.CastPosition);

                        if (aoeCount >= Config.Item("rCount", true).GetValue<Slider>().Value && Config.Item("rCount", true).GetValue<Slider>().Value > 0)
                            R.Cast(poutput.CastPosition);
                        else if (Program.Combo && HaveStun && Config.Item("autoRcombo", true).GetValue<bool>())
                            R.Cast(poutput.CastPosition);
                        else if (Config.Item("autoRks", true).GetValue<bool>())
                        {
                            var comboDmg = OktwCommon.GetKsDamage(enemy, R);

                            if (W.IsReady() && RMANA + WMANA < Player.Mana)
                                comboDmg += W.GetDamage(enemy);

                            if (Q.IsReady() && RMANA + WMANA + QMANA < Player.Mana)
                                comboDmg += Q.GetDamage(enemy);

                            if (enemy.Health < comboDmg)
                                R.Cast(poutput.CastPosition);
                        }
                    }
                    else if(HaveStun && flash.IsReady())
                    {
                        var poutputFlas = FR.GetPrediction(enemy, true);
                        var aoeCountFlash = poutputFlas.AoeTargetsHitCount;
                        if (HaveStun && aoeCountFlash >= Config.Item("rCountFlash", true).GetValue<Slider>().Value && Config.Item("rCountFlash", true).GetValue<Slider>().Value > 0)
                        {
                            Player.Spellbook.CastSpell(flash, poutputFlas.CastPosition);
                            R.Cast(poutputFlas.CastPosition);
                        }
                    }
                }
            }

            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget() && Program.LagFree(2))
            {
                if (Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                {
                    if (Program.Combo && RMANA + WMANA < Player.Mana)
                        Q.Cast(t);
                    else if (Program.Harass && RMANA + WMANA + QMANA < Player.Mana && Config.Item("harassQ", true).GetValue<bool>() && Config.Item("Harass" + t.ChampionName).GetValue<bool>())
                        Q.Cast(t);
                    else
                    {
                        var qDmg = OktwCommon.GetKsDamage(t, Q);
                        var wDmg = W.GetDamage(t);
                        if (qDmg > t.Health)
                            Q.Cast(t);
                        else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                            Q.Cast(t);
                    }
                }
                if (W.IsReady() && Config.Item("autoW", true).GetValue<bool>() && t.IsValidTarget(W.Range))
                {
                    var poutput = W.GetPrediction(t, true);
                    var aoeCount = poutput.AoeTargetsHitCount;

                    if (Program.Combo && RMANA + WMANA < Player.Mana)
                        W.Cast(poutput.CastPosition);
                    else if (Program.Harass && RMANA + WMANA + QMANA < Player.Mana && Config.Item("harassW", true).GetValue<bool>())
                        W.Cast(poutput.CastPosition);
                    else
                    {
                        var wDmg = OktwCommon.GetKsDamage(t, W);
                        var qDmg = Q.GetDamage(t);
                        if (wDmg > t.Health)
                            W.Cast(poutput.CastPosition);
                        else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                            W.Cast(poutput.CastPosition);
                    }
                }
            }
            else if(Q.IsReady() || W.IsReady())
            {
                if (Config.Item("farmQ", true).GetValue<bool>())
                {
                    if (Config.Item("supportMode", true).GetValue<bool>())
                    {
                        if (Program.LaneClear && Player.Mana > RMANA + QMANA)
                            farm();
                    }
                    else
                    {
                        if ((!HaveStun || Program.LaneClear) && Program.Harass)
                            farm();
                    }
                }
            }

            if (Program.LagFree(3))
            {
                if (!HaveStun)
                {
                    if (E.IsReady() && !Program.LaneClear && Config.Item("autoE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + QMANA + WMANA)
                        E.Cast();
                    else if (W.IsReady() && Player.InFountain())
                        W.Cast(Player.Position);
                }
                if (R.IsReady())
                {
                    if (Config.Item("tibers", true).GetValue<bool>() && HaveTibers && Tibbers != null && Tibbers.IsValid)
                    {
                        var enemy = HeroManager.Enemies.Where(x => x.IsValidTarget() && Tibbers.Distance(x.Position) < 1000 && !x.UnderTurret(true)).OrderBy(x => x.Distance(Tibbers)).FirstOrDefault();
                        if(enemy != null)
                        {

                            if (Tibbers.Distance(enemy.Position) > 200)
                                Player.IssueOrder(GameObjectOrder.MovePet, enemy);
                            else
                                Tibbers.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                        }
                        else
                        {
                            var annieTarget = Orbwalker.GetTarget() as Obj_AI_Base;
                            if (annieTarget != null)
                            {
                                if (Tibbers.Distance(annieTarget.Position) > 200)
                                    Player.IssueOrder(GameObjectOrder.MovePet, annieTarget);
                                else
                                    Tibbers.IssueOrder(GameObjectOrder.AttackUnit, annieTarget);
                            }
                            else if (Tibbers.UnderTurret(true))
                            {
                                Player.IssueOrder(GameObjectOrder.MovePet, Player);
                            }
                        }
                    }
                    else
                    {
                        Tibbers = null;
                    }
                }
            }
        }

        private void farm()
        {
            if(Program.LaneClear)
            { 
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady())
                        W.Cast(mob);
                    else if (Q.IsReady())
                        Q.Cast(mob);
                }
            }

            var minionsList = Cache.GetMinions(Player.ServerPosition, Q.Range);
            if (Q.IsReady())
            {
                var minion = minionsList.Where(x => SebbyLib.HealthPrediction.LaneClearHealthPrediction(x, 250, 50) < Q.GetDamage(x) && x.Health > Player.GetAutoAttackDamage(x)).FirstOrDefault();
                Q.Cast(minion);
            }
            else if (FarmSpells && W.IsReady() && Config.Item("farmW", true).GetValue<bool>())
            {
                var farmLocation = W.GetCircularFarmLocation(minionsList, W.Width);
                if (farmLocation.MinionsHit >= FarmMinions)
                    W.Cast(farmLocation.Position);
            }
        }

        private bool HaveTibers
        {
            get { return Player.HasBuff("infernalguardiantimer"); }
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

            if (!R.IsReady() || HaveTibers)
                RMANA = 0;
            else 
                RMANA = R.Instance.ManaCost;
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

            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range + R.Width / 2, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range + R.Width / 2, System.Drawing.Color.Gray, 1, 1);
            }

        }
    }
}
