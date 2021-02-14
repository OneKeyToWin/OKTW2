using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Morgana : Base
    {
        public Morgana()
        {
            Q = new Spell(SpellSlot.Q, 1150);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0.25f, 70f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.50f, 200f, 2200f, false, SkillshotType.SkillshotCircle);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));

            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("ts", "Use common TargetSelector", true).SetValue(true));
            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("ts1", "ON - only one target"));
            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("ts2", "OFF - all targets"));
            HeroMenu.SubMenu("Q config").AddItem(new MenuItem("qCC", "Auto Q cc & dash enemy", true).SetValue(true));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("Q config").SubMenu("Use on").AddItem(new MenuItem("grab" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            HeroMenu.SubMenu("W config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W config").AddItem(new MenuItem("autoWcc", "Auto W only CC enemy", true).SetValue(false));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
            {
                for (int i = 0; i < 4; i++)
                {
                    var spell2 = enemy.Spellbook.Spells[i];
                    if (spell2 != null && spell2.SData != null && spell2.SData.TargettingType != SpellDataTargetType.Self && spell2.SData.TargettingType != SpellDataTargetType.SelfAndUnit)
                    {
                        if (Damage.Spells.ContainsKey(enemy.ChampionName))
                        {
                            var spell = Damage.Spells[enemy.ChampionName].FirstOrDefault(s => s.Slot == enemy.Spellbook.Spells[i].Slot);
                            if (spell != null)
                            {
                                if (spell.DamageType == Damage.DamageType.Physical || spell.DamageType == Damage.DamageType.True)
                                    HeroMenu.SubMenu("E Shield Config").SubMenu("Spell Manager").SubMenu(enemy.ChampionName).AddItem(new MenuItem("spell" + spell2.SData.Name, spell2.Name, true).SetValue(false));
                                else
                                    HeroMenu.SubMenu("E Shield Config").SubMenu("Spell Manager").SubMenu(enemy.ChampionName).AddItem(new MenuItem("spell" + spell2.SData.Name, spell2.Name, true).SetValue(true));
                            }
                            else
                                HeroMenu.SubMenu("E Shield Config").SubMenu("Spell Manager").SubMenu(enemy.ChampionName).AddItem(new MenuItem("spell" + spell2.SData.Name, spell2.Name, true).SetValue(true));
                        }
                    }
                }
            }

            foreach (var ally in HeroManager.Allies)
            {
                HeroMenu.SubMenu("E Shield Config").SubMenu("Shield ally").SubMenu(ally.ChampionName).AddItem(new MenuItem("skillshot" + ally.ChampionName, "skillshot", true).SetValue(true));
                HeroMenu.SubMenu("E Shield Config").SubMenu("Shield ally").SubMenu(ally.ChampionName).AddItem(new MenuItem("targeted" + ally.ChampionName, "targeted", true).SetValue(true));
                HeroMenu.SubMenu("E Shield Config").SubMenu("Shield ally").SubMenu(ally.ChampionName).AddItem(new MenuItem("HardCC" + ally.ChampionName, "Hard CC", true).SetValue(true));
                HeroMenu.SubMenu("E Shield Config").SubMenu("Shield ally").SubMenu(ally.ChampionName).AddItem(new MenuItem("Poison" + ally.ChampionName, "Poison", true).SetValue(true));
            }

            HeroMenu.SubMenu("R config").AddItem(new MenuItem("rCount", "Auto R if enemies in range", true).SetValue(new Slider(3, 0, 5)));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("rKs", "R ks", true).SetValue(false));
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("inter", "OnPossibleToInterrupt", true)).SetValue(true);
            HeroMenu.SubMenu("R config").AddItem(new MenuItem("Gap", "OnEnemyGapcloser", true)).SetValue(true);    

            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && MainMenu.Item("inter", true).GetValue<bool>() && sender.IsValidTarget(R.Range))
                R.Cast();
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!E.IsReady() || !sender.IsEnemy || sender.IsMinion || args.SData.IsAutoAttack() || !sender.IsValid<Obj_AI_Hero>() || Player.Distance(sender.ServerPosition) > 2000)
                return;

            if (MainMenu.Item("spell" + args.SData.Name, true) != null && !MainMenu.Item("spell" + args.SData.Name, true).GetValue<bool>())
                return;
            
            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid  && Player.Distance(ally.ServerPosition) < E.Range))
            {
                //double dmg = 0;

                if (MainMenu.Item("targeted" + ally.ChampionName, true).GetValue<bool>() && args.Target != null && args.Target.NetworkId == ally.NetworkId)
                {
                    E.CastOnUnit(ally);
                    return;
                    //dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                }
                else if (args.Target == null && MainMenu.Item("skillshot" + ally.ChampionName ,true).GetValue<bool>())
                {
                    if (!OktwCommon.CanHitSkillShot(ally, args.Start, args.End, args.SData))
                        continue;
                    //dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                    E.CastOnUnit(ally);
                    return;
                }
            }   
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && MainMenu.Item("Gap", true).GetValue<bool>() && gapcloser.Sender.IsValidTarget(R.Range))
                R.Cast();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }
            if (Program.LagFree(1) && Q.IsReady())
                LogicQ();
            if (Program.LagFree(2) && R.IsReady())
                LogicR();
            if (Program.LagFree(3) && W.IsReady() && MainMenu.Item("autoW", true).GetValue<bool>())
                LogicW();
            if (Program.LagFree(4) && E.IsReady())
                LogicE();
        }

        private void LogicE()
        {
            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && ally.Distance(Player.Position) < E.Range))
            {
                if (MainMenu.Item("Poison" + ally.ChampionName, true).GetValue<bool>() && ally.HasBuffOfType(BuffType.Poison))
                {
                    E.CastOnUnit(ally);
                }
            }
        }

        private void LogicQ()
        {
            if (Program.Combo && MainMenu.Item("ts", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget(Q.Range) && !t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield) && MainMenu.Item("grab" + t.ChampionName).GetValue<bool>())
                    Program.CastSpell(Q, t);
            }
            foreach (var t in HeroManager.Enemies.Where(t => t.IsValidTarget(Q.Range) && MainMenu.Item("grab" + t.ChampionName).GetValue<bool>()))
            {
                if (!t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield))
                {
                    if (Program.Combo && !MainMenu.Item("ts", true).GetValue<bool>())
                        Program.CastSpell(Q, t);

                    if (MainMenu.Item("qCC", true).GetValue<bool>())
                    {
                        if (!OktwCommon.CanMove(t))
                            Q.Cast(t, true);
                        Q.CastIfHitchanceEquals(t, HitChance.Dashing);
                        Q.CastIfHitchanceEquals(t, HitChance.Immobile);
                    }
                }
            }
        }

        private void LogicR()
        {
            bool rKs = MainMenu.Item("rKs", true).GetValue<bool>();
            foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range) && target.HasBuff("rocketgrab2")))
            {
                if (rKs && R.GetDamage(target) > target.Health)
                    R.Cast();
            }
            if (Player.CountEnemiesInRange(R.Range) >= MainMenu.Item("rCount", true).GetValue<Slider>().Value && MainMenu.Item("rCount", true).GetValue<Slider>().Value > 0)
                R.Cast();
        }
        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget() )
            {
                if (!MainMenu.Item("autoWcc", true).GetValue<bool>() && !Q.IsReady())
                {
                    if (W.GetDamage(t) > t.Health)
                        Program.CastSpell(W, t);
                    else if (Program.Combo && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(W, t);
                }

                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                    W.Cast(enemy, true);
            }
            else if (FarmSpells && MainMenu.Item("farmW", true).GetValue<bool>() && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetCircularFarmLocation(minionList, W.Width);

                if (farmPosition.MinionsHit >= FarmMinions)
                    W.Cast(farmPosition.Position);
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
                    if (W.IsReady() && MainMenu.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
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
        }
    }
}
