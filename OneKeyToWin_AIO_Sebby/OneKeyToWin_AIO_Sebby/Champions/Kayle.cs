using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Kayle : Base
    {
        public Kayle()
        {
            Q = new Spell(SpellSlot.Q, 670);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 660);
            R = new Spell(SpellSlot.R, 900);

            Q.SetSkillshot(0.25f, 75f, 1600f, true, SkillshotType.SkillshotLine);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification & line", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoWspeed", "W speed-up", true).SetValue(true));
            foreach (var enemy in HeroManager.Allies)
                HeroMenu.SubMenu("W Config").SubMenu("W ally:").AddItem(new MenuItem("Wally" + enemy.ChampionName, enemy.ChampionName).SetValue(true));


            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(true));

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            foreach (var enemy in HeroManager.Allies)
                HeroMenu.SubMenu("R Config").SubMenu("R ally:").AddItem(new MenuItem("Rally" + enemy.ChampionName, enemy.ChampionName).SetValue(true));


            foreach (var enemy in HeroManager.Enemies)
            {
                for (int i = 0; i < 4; i++)
                {
                    var spell = enemy.Spellbook.Spells[i];
                    if (spell.SData.TargettingType != SpellDataTargetType.Self && spell.SData.TargettingType != SpellDataTargetType.SelfAndUnit)
                    {
                        HeroMenu.SubMenu("R Config").SubMenu("Spell Manager").SubMenu(enemy.ChampionName).AddItem(new MenuItem("spell" + spell.SData.Name, spell.Name,true).SetValue(false));
                    }
                }
            }

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!R.IsReady() || sender.IsMinion || !sender.IsEnemy || args.SData.IsAutoAttack() || !MainMenu.Item("autoR", true).GetValue<bool>()
                 || !sender.IsValid<Obj_AI_Hero>() || args.SData.Name.ToLower() == "tormentedsoil")
                return;

            if (MainMenu.Item("spell" + args.SData.Name,true) == null || !MainMenu.Item("spell" + args.SData.Name,true).GetValue<bool>())
                return;

            if (args.Target != null)
            {
                if (args.Target.IsAlly )
                {
                    var ally = args.Target as Obj_AI_Hero;
                    if(ally != null && MainMenu.Item("Rally" + ally.ChampionName).GetValue<bool>())
                        R.CastOnUnit(ally);
                }
            }
            else
            {
                foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 70 && Player.ServerPosition.Distance(ally.ServerPosition) < R.Range && MainMenu.Item("Rally" + ally.ChampionName).GetValue<bool>()))
                {
                    if(OktwCommon.CanHitSkillShot(ally, args.Start, args.End, args.SData))
                        R.CastOnUnit(ally);
                }
            }
        }
        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(1))
            {
                SetMana();
                Jungle();
            }

            if (R.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                LogicR();

            if (Program.LagFree(2) && W.IsReady() && !Player.IsWindingUp && MainMenu.Item("autoW", true).GetValue<bool>())
                LogicW();
            
            if (Program.LagFree(3) && E.IsReady() && MainMenu.Item("autoE", true).GetValue<bool>())
                LogicE();
            if (Program.LagFree(4) && Q.IsReady() && !Player.IsWindingUp && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();
        }

        private void LogicR()
        {
            foreach (var ally in HeroManager.Allies.OrderBy(x => x.Health).Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 70 && Player.ServerPosition.Distance(ally.ServerPosition) < R.Range && MainMenu.Item("Rally" + ally.ChampionName).GetValue<bool>() ))
            {
                double dmg = OktwCommon.GetIncomingDamage(ally);
                if (dmg == 0)
                    continue;

                if (ally.Health - dmg <  ally.Level * 10)
                    R.CastOnUnit(ally);
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (t.IsValidTarget())
            {
                var pred = Q.GetPrediction(t);

                if (pred.Hitchance < HitChance.High && (pred.CollisionObjects.Count != 0 && pred.CollisionObjects[0].Distance(pred.UnitPosition) > 350))
                    return;

                if (Program.Combo)
                    Q.Cast(pred.CastPosition);
                else if (Program.Harass && MainMenu.Item("harassQ", true).GetValue<bool>() &&  MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + WMANA + QMANA + QMANA)
                    Q.Cast(pred.CastPosition);
                else if (Player.Health < Player.Level * 40 && !W.IsReady() && !R.IsReady())
                    Q.Cast(pred.CastPosition);
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Q.Cast(pred.CastPosition);
            }
        }

        private void LogicW()
        {
            if (!Player.InFountain() && !Player.HasBuff("Recall") && !Player.IsRecalling())
            {
                Obj_AI_Hero lowest = Player;

                foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && MainMenu.Item("Wally" + ally.ChampionName).GetValue<bool>() && Player.Distance(ally.Position) < W.Range))
                {
                    if (ally.Health < lowest.Health)
                        lowest = ally;
                }
                
                if (Player.Mana > WMANA + QMANA && lowest.Health < lowest.Level * 40)
                    W.CastOnUnit(lowest);
                else if (Player.Mana > WMANA + EMANA + QMANA && lowest.Health < lowest.MaxHealth * 0.4 && lowest.Health < 1500)
                    W.CastOnUnit(lowest);
                else if (Player.Mana > Player.MaxMana * 0.5 && lowest.Health < lowest.MaxHealth * 0.7 && lowest.Health < 2000)
                    W.CastOnUnit(lowest);
                else if (Player.Mana > Player.MaxMana * 0.9 && lowest.Health < lowest.MaxHealth * 0.9)
                    W.CastOnUnit(lowest);
                else if (Player.Mana == Player.MaxMana && lowest.Health < lowest.MaxHealth * 0.9)
                    W.CastOnUnit(lowest);
                if (MainMenu.Item("autoWspeed", true).GetValue<bool>())
                {
                    var t = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
                    if (t.IsValidTarget())
                    {
                        if (Program.Combo && Player.Mana > WMANA + QMANA + EMANA && Player.Distance(t.Position) > Q.Range)
                            W.CastOnUnit(Player);
                    }
                }
            }
        }

        private void LogicE()
        {
            var orbTarget = Orbwalker.GetTarget() as Obj_AI_Hero;

            if (orbTarget != null)
            {
                if (!Orbwalking.CanAttack() && Orbwalking.CanMove(20))
                {
                    if (Program.Combo && Player.Mana > WMANA + EMANA)
                    {
                        E.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                    else if (Program.Harass && MainMenu.Item("harassE", true).GetValue<bool>() && Player.Mana > WMANA + EMANA + QMANA)
                    { 
                        E.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }
            else
            {
                if (Program.Combo && Player.Mana > WMANA + EMANA && Player.CountEnemiesInRange(600) > 0)
                {
                    E.Cast();
                    Orbwalking.ResetAutoAttackTimer();
                }
                else if (Program.Harass && MainMenu.Item("harassE", true).GetValue<bool>() && Player.Mana > WMANA + EMANA + QMANA && Player.CountEnemiesInRange(500) > 0)
                {  
                    E.Cast();
                    Orbwalking.ResetAutoAttackTimer();
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
                    if (E.IsReady() && MainMenu.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast();
                        Orbwalking.ResetAutoAttackTimer();
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

        private bool FarmE()
        {
            return (Cache.GetMinions(Player.ServerPosition, 600).Count > 0);
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
            RMANA = 0;

            if (!Q.IsReady())
                QMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;

        }
        private void Drawing_OnDraw(EventArgs args)
        {
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
