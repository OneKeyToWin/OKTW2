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
    class Teemo : Base
    {
        public Teemo()
        {
            Q = new Spell(SpellSlot.Q, 680);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 400);

            Q.SetTargetted(0.5f, 1500f);
            R.SetSkillshot(1.7f, 130f, 1000f, false, SkillshotType.SkillshotCircle);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification & line", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("Qgap", "Auto Q Gapcloser", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("QafterAA", "Q after AA only", true).SetValue(true));

            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("Q Config").SubMenu("Q on").AddItem(new MenuItem("qUseOn" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoWout", "Auto W if target outrange", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoWnear", "Auto W if enemy near", true).SetValue(true));

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("comboR", "Run", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("Raoe", "AOE", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("Rgap", "Gapcloser", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoRslow", "On slow", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoRcc", "On CC", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoRdash", "On dash", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("telR", "On zhonya, teleport, spells", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("bushR2", "Bush above 1 ammo", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("bushR", "Auto W bush after enemy enter", true).SetValue(true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Q.IsReady())
            {
                var t = target as Obj_AI_Hero;
                if (t != null)
                {
                    if (Program.Harass && OktwCommon.CanHarras() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                        Q.Cast(t);

                    if (Program.Combo)
                        Q.Cast(t);
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;
            if (R.IsReady() && MainMenu.Item("Rgap", true).GetValue<bool>())
            {
                if(Player.Position.Distance(gapcloser.End) < R.Range)
                {
                    R.Cast(gapcloser.End);
                }
                else
                {
                    R.Cast(Player.ServerPosition);
                }
            }
            else if (Q.IsReady() && MainMenu.Item("Qgap", true).GetValue<bool>() && t.IsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }
        }

        public bool ShouldUseE(string SpellName)
        {
            switch (SpellName)
            {
                case "ThreshQ":
                    return true;
                case "KatarinaR":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "GalioIdolOfDurand":
                    return true;
                case "LuxMaliceCannon":
                    return true;
                case "MissFortuneBulletTime":
                    return true;
                case "RocketGrabMissile":
                    return true;
                case "CaitlynPiltoverPeacemaker":
                    return true;
                case "EzrealTrueshotBarrage":
                    return true;
                case "InfiniteDuress":
                    return true;
                case "VelkozR":
                    return true;
            }
            return false;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                if (ObjectManager.Get<Obj_AI_Base>().Any(obj => obj.IsValid && obj.Health > 0 && obj.Position.Distance(args.EndPosition) < 250 && obj.Name.ToLower().Contains("noxious trap".ToLower())))
                    args.Process = false;
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
            RMANA = R.Instance.ManaCost;
        }
        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsRecalling())
                return;

            if (Program.LagFree(0))
            {
                SetMana();
                R.Range = 150 + 250 * R.Level;
            }
            
            if (Q.IsReady() && Orbwalking.CanMove(50) && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(4) && W.IsReady() && Player.IsMoving)
                LogicW();

            if (R.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                LogicR();
        }

        private void LogicW()
        {
            if (Player.Mana < RMANA + WMANA)
                return;

            if (HeroManager.Enemies.Any(enemy => enemy.IsValidTarget(350) && enemy.IsMoving && enemy.IsFacing(Player) && MainMenu.Item("autoWnear", true).GetValue<bool>()))
                W.Cast();

            if (Program.Combo && MainMenu.Item("autoWout", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(800, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget() && !Orbwalker.InAutoAttackRange(t) && !t.IsFacing(Player))
                    W.Cast();
            }
        }

        private void LogicR()
        {
            if (Player.Mana > RMANA + QMANA && (Player.IsMoving || !Program.None))
            {
                if (Program.LagFree(1) && Orbwalking.CanMove(50))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range + 100)))
                    {
                        if(MainMenu.Item("autoRcc", true).GetValue<bool>() && !OktwCommon.CanMove(enemy))
                            R.Cast(enemy);
                        if (MainMenu.Item("autoRdash", true).GetValue<bool>())
                            R.CastIfHitchanceEquals(enemy, HitChance.Dashing);
                        if (MainMenu.Item("autoRslow", true).GetValue<bool>() && enemy.HasBuffOfType(BuffType.Slow))
                            Program.CastSpell(R, enemy);
                        if (MainMenu.Item("Raoe", true).GetValue<bool>())
                            R.CastIfWillHit(enemy, 2);
                        if (MainMenu.Item("comboR", true).GetValue<bool>() && OktwCommon.IsMovingInSameDirection(Player, enemy))
                        {
                            var predPos = R.GetPrediction(enemy,true);
                            if (predPos.CastPosition.Distance(enemy.Position) > 300 && predPos.Hitchance >= HitChance.Low)
                            {
                                if (!OktwCommon.CirclePoints(8, 120, predPos.CastPosition).Any(x => x.IsWall()))
                                {
                                    R.Cast(predPos.CastPosition);
                                    return;
                                }  
                            }
                        }
                    }
                }

                if (Program.LagFree(2) && MainMenu.Item("telR", true).GetValue<bool>())
                {
                    var trapPos = OktwCommon.GetTrapPos(R.Range);
                    if (!trapPos.IsZero)
                        R.Cast(trapPos);
                }

                if (Program.LagFree(3) && Orbwalking.CanMove(50) && Player.Mana > RMANA + QMANA + WMANA && MainMenu.Item("bushR2", true).GetValue<bool>() && Utils.TickCount - R.LastCastAttemptT > 2000)
                {
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Ammo > 1 + Player.CountEnemiesInRange(1200) && Player.CountEnemiesInRange(800) == 0)
                    {
                        var points = OktwCommon.CirclePoints(12, R.Range, Player.Position);
                        points.Add(Player.Position);
                        foreach (var point in points)
                        {
                            if (NavMesh.IsWallOfGrass(point, 10))
                            {
                                if (!OktwCommon.CirclePoints(8, 110, point).Any(x => x.IsWall()) && !ObjectManager.Get<Obj_AI_Base>().Any(obj => obj.IsValid && obj.Mana > 40 && obj.Health > 0 && obj.Position.Distance(point) < 500 && obj.Name.ToLower().Contains("noxious trap".ToLower()) ))
                                {
                                    R.Cast(point);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LogicQ()
        {
            var hpPred = Player.Health - OktwCommon.GetIncomingDamage(Player) > Player.MaxHealth * 0.3;
            foreach (var t in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range)).OrderBy(enemy => enemy.Health))
            {
                if (OktwCommon.GetKsDamage(t, Q) + Player.GetAutoAttackDamage(t) > t.Health)
                    Q.Cast(t);

                if (!MainMenu.Item("qUseOn" + t.ChampionName).GetValue<bool>() && hpPred)
                    continue;

                if(MainMenu.Item("QafterAA", true).GetValue<bool>())
                    continue;
                
                if (Program.Harass && OktwCommon.CanHarras() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + WMANA + QMANA)
                    Q.Cast(t);

                if (t.IsMelee && t.IsFacing(Player) && t.ServerPosition.Distance(Player.ServerPosition) > 300)
                    continue;

                if (Program.Combo)
                    Q.Cast(t);
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (R.IsReady() && MainMenu.Item("telR", true).GetValue<bool>())
            {
                if (sender.IsEnemy  && sender.IsValidTarget(R.Range) && ShouldUseE(args.SData.Name))
                {
                    R.Cast(sender.ServerPosition, true);
                }
            }
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
