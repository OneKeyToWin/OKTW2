using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Sivir : Base
    {

        public static MissileReturn missileManager;

        public Sivir()
        {
            Q = new Spell(SpellSlot.Q, 1200f);
            Q1 = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W, float.MaxValue);
            E = new Spell(SpellSlot.E, float.MaxValue);
            R = new Spell(SpellSlot.R, 25000f);

            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);
            Q1.SetSkillshot(0.25f, 90f, 1350f, true, SkillshotType.SkillshotLine);

            missileManager = new MissileReturn("SivirQMissile", "SivirQMissileReturn", Q);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("notif", "Notification (timers)", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("noti", "Show KS notification", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));

            HeroMenu.AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
            {
                for (int i = 0; i < 4; i++)
                {
                    var spell = enemy.Spellbook.Spells[i];
                    if (spell.SData.TargettingType != SpellDataTargetType.Self && spell.SData.TargettingType != SpellDataTargetType.SelfAndUnit)
                    {
                        if (spell.SData.TargettingType == SpellDataTargetType.Unit)
                            HeroMenu.SubMenu("E Shield Config").SubMenu("Spell Manager").SubMenu(enemy.ChampionName).AddItem(new MenuItem("spell" + spell.SData.Name, spell.Name).SetValue(true));
                        else
                            HeroMenu.SubMenu("E Shield Config").SubMenu("Spell Manager").SubMenu(enemy.ChampionName).AddItem(new MenuItem("spell" + spell.SData.Name, spell.Name).SetValue(false));
                    }
                }
            }

            HeroMenu.AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));

            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("autoEmissile", "Block unknown missile", true).SetValue(true));
            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("AGC", "AntiGapcloserE", true).SetValue(true));
            HeroMenu.SubMenu("E Shield Config").AddItem(new MenuItem("Edmg", "Block under % hp", true).SetValue(new Slider(90, 100, 0)));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += Orbwalker_AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if(missile != null && MainMenu.Item("autoEmissile", true).GetValue<bool>())
            {
                if(!missile.SData.IsAutoAttack() && missile.Target == Player && missile.SpellCaster.IsEnemy && missile.SpellCaster.IsChampion())
                {
                    E.Cast();
                }
            }
        }

        public void Orbwalker_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (W.IsReady() && !Program.None && target.IsValidTarget())
            {
                var t = target as Obj_AI_Hero;
                if (t != null)
                {
                    if(Player.GetAutoAttackDamage(t) * 3 > t.Health - OktwCommon.GetIncomingDamage(t))
                        W.Cast();
                    if (Program.Combo && Player.Mana > RMANA + WMANA)
                        W.Cast();
                    else if (MainMenu.Item("harassW", true).GetValue<bool>() && !Player.UnderTurret(true) && Player.Mana > RMANA + WMANA + QMANA && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var t2 = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);
                    if (t2.IsValidTarget() && MainMenu.Item("harassW", true).GetValue<bool>() && MainMenu.Item("Harass" + t2.ChampionName).GetValue<bool>() && !Player.UnderTurret(true) && Player.Mana > RMANA + WMANA + QMANA && t2.Distance(target.Position) < 500)
                    {
                        W.Cast();
                    }

                    if (target is Obj_AI_Minion && FarmSpells && MainMenu.Item("farmW", true).GetValue<bool>() && FarmSpells && !Player.UnderTurret(true))
                    {
                        var minions = Cache.GetMinions(target.Position, 500);
                        if (minions.Count >= FarmMinions)
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!E.IsReady() || args.SData.IsAutoAttack() || Player.HealthPercent > MainMenu.Item("Edmg", true).GetValue<Slider>().Value || !MainMenu.Item("autoE", true).GetValue<bool>()
                || !sender.IsEnemy || sender.IsMinion || !sender.IsValid<Obj_AI_Hero>() || args.SData.Name.ToLower() == "tormentedsoil")
                return;

            if (MainMenu.Item("spell" + args.SData.Name) != null && !MainMenu.Item("spell" + args.SData.Name).GetValue<bool>())
                return;

            if (args.Target != null)
            {
                if (args.Target.IsMe)
                    E.Cast();
            }
            else if (OktwCommon.CanHitSkillShot(Player, args.Start, args.End, args.SData))
            {
                E.Cast();
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var Target = gapcloser.Sender;
            if (MainMenu.Item("AGC", true).GetValue<bool>() && E.IsReady() && Target.IsValidTarget(5000))
                E.Cast();
            return;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
            }
           
            if (Program.LagFree(1) && Q.IsReady() && !Player.IsWindingUp)
            {
                LogicQ();
            }

            if (Program.LagFree(2) && R.IsReady() && Program.Combo && MainMenu.Item("autoR", true).GetValue<bool>())
            {
                LogicR();
            }

            if (Program.LagFree(3) && Program.LaneClear)
            {
                Jungle();
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                missileManager.Target = t;
                var qDmg = OktwCommon.GetKsDamage(t,Q) * 1.9;
                if (Orbwalking.InAutoAttackRange(t))
                    qDmg = qDmg + Player.GetAutoAttackDamage(t) * 3;
                if (qDmg > t.Health)
                    Q.Cast(t, true);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Harass && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && !Player.UnderTurret(true))
                {
                     if (Player.Mana > Player.MaxMana * 0.9)
                        Program.CastSpell(Q, t);
                     else if (ObjectManager.Player.Mana > RMANA + WMANA + QMANA + QMANA)
                        Program.CastSpell(Q1, t);
                     else if (Player.Mana > RMANA + WMANA + QMANA + QMANA)
                     {
                         Q.CastIfWillHit(t, 2, true);
                         if(Program.LaneClear)
                             Program.CastSpell(Q, t);
                     }
                }
                if (Player.Mana > RMANA + WMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
            else if (FarmSpells && MainMenu.Item("farmQ", true).GetValue<bool>() )
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit >= FarmMinions)
                    Q.Cast(farmPosition.Position);
            }
        }
        private void LogicR()
        {
            var t = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            if (Player.CountEnemiesInRange(800f) > 2)
                R.Cast();
            else if (t.IsValidTarget() && Orbwalker.GetTarget() == null && Program.Combo && Player.GetAutoAttackDamage(t) * 2 > t.Health && !Q.IsReady() && t.CountEnemiesInRange(800) < 3)
                R.Cast();
        }

        private void Jungle()
        {
            if ( Player.Mana > RMANA  + WMANA + RMANA )
            {
                var mobs = Cache.GetMinions(ObjectManager.Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && MainMenu.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast();
                        return;
                    }
                    if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob);
                        return;
                    }
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

        public static void drawText2(string msg, Vector3 Hero, int high, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - high, color, msg);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MainMenu.Item("notif", true).GetValue<bool>())
            {
                if (Player.HasBuff("sivirwmarker"))
                {
                    var color = System.Drawing.Color.Yellow;
                    var buffTime = OktwCommon.GetPassiveTime(Player, "sivirwmarker");
                    if (buffTime<1)
                        color = System.Drawing.Color.Red;
                    drawText2("W:  " + String.Format("{0:0.0}", buffTime), Player.Position, 175, color);
                }
                if (Player.HasBuff("SivirE"))
                {
                    var color = System.Drawing.Color.Aqua;
                    var buffTime = OktwCommon.GetPassiveTime(Player, "SivirE");
                    if (buffTime < 1)
                        color = System.Drawing.Color.Red;
                    drawText2("E:  " + String.Format("{0:0.0}", buffTime), Player.Position, 200, color);
                }
                if (Player.HasBuff("SivirR"))
                {
                    var color = System.Drawing.Color.GreenYellow;
                    var buffTime = OktwCommon.GetPassiveTime(Player, "SivirR");
                    if (buffTime < 1)
                        color = System.Drawing.Color.Red;
                    drawText2("R:  " + String.Format("{0:0.0}", buffTime), Player.Position, 225, color);
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

            if (MainMenu.Item("noti", true).GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget())
                {
                    if (Q.GetDamage(target) * 2 > target.Health)
                    {
                        Render.Circle.DrawCircle(target.ServerPosition, 200, System.Drawing.Color.Red);
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "Q kill: " + target.ChampionName + " have: " + target.Health + "hp");
                    }
                }
            }
        }
    }
}
