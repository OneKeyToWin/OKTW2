using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class TwistedFate : Base
    {
        private string temp = null;
        private bool cardok = true;
        private int FindCard = 0;
        //private string wName = "";

        public TwistedFate()
        {
            Q = new Spell(SpellSlot.Q, 1400);
            E = new Spell(SpellSlot.E, 700);
            W = new Spell(SpellSlot.W, 1200);
            R = new Spell(SpellSlot.R, 5500);

            Q.SetSkillshot(0.25f, 40f, 1000, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 40f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, 2000f);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRangeMini", "R range minimap", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("cardInfo", "Show card info", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("notR", "R info helper", true).SetValue(true));

            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("Wmode", "W mode", true).SetValue(new StringList(new[] { "Auto", "Manual" }, 0)));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("Wgold", "Gold key", true).SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press))); //32 == space 
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("Wblue", "Blue key", true).SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Press))); //32 == space 
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("Wred", "RED key", true).SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Press))); //32 == space 


            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("WblockAA", "Block AA if seeking GOLD card", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass GOLD low range", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("ignoreW", "Ignore first card", true).SetValue(true));

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space 
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("Renemy", "Don't R if enemy in x range", true).SetValue(new Slider(1000, 2000, 0)));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("RenemyA", "Don't R if ally in x range near target", true).SetValue(new Slider(800, 2000, 0)));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("turetR", "Don't R under turret ", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("WredFarm", "LaneClear red card above % mana", true).SetValue(new Slider(80, 100, 0)));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W Blue / Red card", true).SetValue(false));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Game.OnWndProc += Game_OnWndProc;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            // if (sender.IsValid<Obj_GeneralParticleEmitter>() && sender.Name.Contains("TwistedFate_Base_W_"))
            //     wName = sender.Name;
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.WParam == 16)
            {
                MainMenu.Item("Wgold", true).Show(MainMenu.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 1);
                MainMenu.Item("Wblue", true).Show(MainMenu.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 1);
                MainMenu.Item("Wred", true).Show(MainMenu.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 1);
                MainMenu.Item("harassW", true).Show(MainMenu.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 0);
            }
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Program.Combo && W.IsReady() && FindCard == 1 && W.Instance.Name != "PickACard" && MainMenu.Item("WblockAA", true).GetValue<bool>())
            {
                args.Process = false;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            foreach (var buf in Player.Buffs.Where(x => x.Name != "secondsight"))
            {

            }
            if (!MainMenu.Item("ignoreW", true).GetValue<bool>())
                cardok = true;

            if (W.IsReady())
            {
                if (MainMenu.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 0)
                    LogicW();
                else
                    LogicWmaunal();
            }
            else if (W.Instance.Name == "PickACard")
            {
                temp = null;
                cardok = false;
            }

            if (Program.LagFree(2) && Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(4) && Q.IsReady())
                Jungle();

            if (R.IsReady())
            {
                if (Program.LagFree(3) && W.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                    LogicR();

                if (MainMenu.Item("useR", true).GetValue<KeyBind>().Active)
                {
                    if (Player.HasBuff("destiny_marker"))
                    {
                        var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                        if (t.IsValidTarget())
                        {
                            R.Cast(t);
                        }
                    }
                    else
                    {
                        R.Cast();
                    }
                }
            }
            //Program.debug("" + (W.Instance.CooldownExpires - Game.Time));
        }

        private void LogicWmaunal()
        {

            if (!Player.HasBuff("pickacard_tracker"))
            {
                if (Utils.TickCount - W.LastCastAttemptT > 300)
                {
                    if (R.IsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    {
                        FindCard = 1;
                        W.Cast();
                    }
                    else if (MainMenu.Item("Wgold", true).GetValue<KeyBind>().Active)
                    {
                        FindCard = 1;
                        W.Cast();
                    }
                    else if (MainMenu.Item("Wblue", true).GetValue<KeyBind>().Active)
                    {
                        FindCard = 2;
                        W.Cast();
                    }
                    else if (MainMenu.Item("Wred", true).GetValue<KeyBind>().Active)
                    {
                        FindCard = 3;
                        W.Cast();
                    }
                }
            }
            else
            {
                if (temp == null)
                    temp = W.Instance.Name;
                else if (temp != W.Instance.Name)
                    cardok = true;

                if (cardok)
                {
                    if (R.IsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    {
                        FindCard = 1;
                        if (W.Instance.Name == "GoldCardLock")
                            W.Cast();
                    }
                    else if (FindCard == 1)
                    {
                        if (W.Instance.Name == "GoldCardLock")
                        {
                            Console.WriteLine("dupa" + Game.Time);
                            W.Cast();
                        }
                    }
                    else if (FindCard == 2)
                    {
                        if (W.Instance.Name == "BlueCardLock")
                            W.Cast();
                    }
                    else if (FindCard == 3)
                    {
                        if (W.Instance.Name == "RedCardLock")
                            W.Cast();
                    }
                }
            }
        }

        private void LogicW()
        {

            var t = TargetSelector.GetTarget(1100, TargetSelector.DamageType.Magical);
            if (!Player.HasBuff("pickacard_tracker"))
            {
                if (Utils.TickCount - W.LastCastAttemptT > 300)
                {
                    if (R.IsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                        W.Cast();
                    else if (t.IsValidTarget() && Program.Combo)
                        W.Cast();
                    else if (Orbwalker.GetTarget() != null)
                    {
                        if (Program.Harass && Orbwalker.GetTarget().Type == GameObjectType.obj_AI_Hero && MainMenu.Item("harassW", true).GetValue<bool>())
                            W.Cast();
                        else if (Program.LaneClear && (Orbwalker.GetTarget().Type == GameObjectType.obj_AI_Minion || Orbwalker.GetTarget().Type == GameObjectType.obj_AI_Turret) && MainMenu.Item("farmW", true).GetValue<bool>())
                            W.Cast();
                    }
                }
            }
            else
            {
                if (temp == null)
                    temp = W.Instance.Name;
                else if (temp != W.Instance.Name)
                    cardok = true;

                if (cardok)
                {
                    Obj_AI_Hero orbTarget = null;

                    var getTarget = Orbwalker.GetTarget();
                    if (getTarget != null && getTarget.Type == GameObjectType.obj_AI_Hero)
                    {
                        orbTarget = (Obj_AI_Hero)getTarget;
                    }

                    if (R.IsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    {
                        FindCard = 1;
                        if (W.Instance.Name == "GoldCardLock")
                            W.Cast();
                    }
                    else if (Program.Combo && orbTarget.IsValidTarget() && W.GetDamage(orbTarget) + Player.GetAutoAttackDamage(orbTarget) > orbTarget.Health)
                    {
                        W.Cast();
                        Program.debug("1" + W.Instance.Name);
                    }
                    else if (Player.Mana < RMANA + QMANA + WMANA)
                    {
                        FindCard = 2;
                        if (W.Instance.Name == "BlueCardLock")
                            W.Cast();
                    }
                    else if (Program.Harass && orbTarget.IsValidTarget())
                    {
                        FindCard = 1;
                        if (W.Instance.Name == "BlueCardLock")
                            W.Cast();
                    }
                    else if (Player.ManaPercent > MainMenu.Item("WredFarm", true).GetValue<Slider>().Value && FarmSpells && MainMenu.Item("farmW", true).GetValue<bool>())
                    {
                        FindCard = 3;
                        if (W.Instance.Name == "RedCardLock")
                            W.Cast();
                    }
                    else if ((Program.LaneClear || Player.Mana < RMANA + QMANA) && MainMenu.Item("farmW", true).GetValue<bool>())
                    {
                        FindCard = 2;
                        if (W.Instance.Name == "BlueCardLock")
                            W.Cast();
                    }
                    else if (Program.Combo)
                    {
                        FindCard = 1;
                        if (W.Instance.Name == "GoldCardLock")
                            W.Cast();
                    }
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 700, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob);
                        return;
                    }
                }
            }
        }

        private void LogicR()
        {
            if (Player.CountEnemiesInRange(MainMenu.Item("Renemy", true).GetValue<Slider>().Value) == 0)
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget() && t.Distance(Player.Position) > Q.Range && t.CountAlliesInRange(MainMenu.Item("RenemyA", true).GetValue<Slider>().Value) == 0)
                {
                    if (Q.GetDamage(t) + W.GetDamage(t) + Player.GetAutoAttackDamage(t) * 3 > t.Health && t.CountEnemiesInRange(1000) < 3)
                    {
                        var rPos = R.GetPrediction(t).CastPosition;
                        if (MainMenu.Item("turetR", true).GetValue<bool>())
                        {
                            if (!rPos.UnderTurret(true))
                                R.Cast(rPos);
                        }
                        else
                        {
                            R.Cast(rPos);
                        }
                    }
                }
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, Q) > t.Health && !Orbwalking.InAutoAttackRange(t))
                    Program.CastSpell(Q, t);

                if (W.Instance.CooldownExpires - Game.Time < W.Instance.Cooldown - 1.3 && !Player.HasBuff("pickacard_tracker") && (W.Instance.CooldownExpires - Game.Time > 3 || Player.CountEnemiesInRange(950) == 0))
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA)
                        Program.CastSpell(Q, t);
                    if (Program.Harass && Player.Mana > RMANA + QMANA + WMANA + EMANA && MainMenu.Item("harassQ", true).GetValue<bool>() && OktwCommon.CanHarras() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                        Program.CastSpell(Q, t);
                }

                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                    Q.Cast(enemy, true, true);

            }
            else if (FarmSpells && MainMenu.Item("farmQ", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit >= FarmMinions)
                    Q.Cast(farmPosition.Position);
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
                RMANA = WMANA - Player.PARRegenRate * W.Instance.Cooldown;
            else
                RMANA = R.Instance.ManaCost;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (R.IsReady() && MainMenu.Item("rRangeMini", true).GetValue<bool>())
            {
                Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
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

            if (MainMenu.Item("cardInfo", true).GetValue<bool>() && Player.HasBuff("pickacard_tracker"))
            {
                if (FindCard == 1)
                    drawText("SEEK YELLOW", Player.Position, System.Drawing.Color.Yellow, -70);
                if (FindCard == 2)
                    drawText("SEEK BLUE ", Player.Position, System.Drawing.Color.Aqua, -70);
                if (FindCard == 3)
                    drawText("SEEK RED ", Player.Position, System.Drawing.Color.OrangeRed, -70);

            }


            if (R.IsReady() && MainMenu.Item("notR", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    var comboDMG = Q.GetDamage(t) + W.GetDamage(t) + Player.GetAutoAttackDamage(t) * 3;
                    if (Player.HasBuff("destiny_marker"))
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Yellow, "AUTO R TARGET: " + t.ChampionName + " Heal " + t.Health + " My damage: " + comboDMG);
                    else if (comboDMG > t.Health)
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "You can kill: " + t.ChampionName + " Heal " + t.Health + " My damage: " + comboDMG + " PRESS semi-manual cast");
                }
            }
        }
    }
}
