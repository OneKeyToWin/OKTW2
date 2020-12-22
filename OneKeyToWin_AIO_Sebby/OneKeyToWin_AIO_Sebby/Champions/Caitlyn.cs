using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Caitlyn : Base
    {
        private float QCastTime = 0;
        public Obj_AI_Hero LastW = ObjectManager.Player;

        private static string[] Spells =
        {
            "katarinar","drain","consume","absolutezero", "staticfield","reapthewhirlwind","jinxw","jinxr","shenstandunited","threshe","threshrpenta","threshqinternal", "threshq","meditate","caitlynpiltoverpeacemaker", "volibearqattack",
            "cassiopeiapetrifyinggaze","ezrealtrueshotbarrage","galioidolofdurand","luxmalicecannon", "missfortunebullettime","infiniteduress","alzaharnethergrasp","lucianq","velkozr","rocketgrabmissile"
        };

        public Caitlyn()
        {
            Q = new Spell(SpellSlot.Q, 1250f);
            Q1 = new Spell(SpellSlot.Q, 1250f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 850f);
            R = new Spell(SpellSlot.R, 3000f);

            Q.SetSkillshot(0.65f, 60f, 2200f, false, SkillshotType.SkillshotLine);
            Q1.SetSkillshot(0.65f, 60f, 2200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.5f, 20f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.30f, 70f, 2000f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 200f, 1500f, false, SkillshotType.SkillshotCircle);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification & line", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ2", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Reduce Q use", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("Qaoe", "Q aoe", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("Qslow", "Q slow", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W on hard CC", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("telE", "Auto W teleport", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("forceW", "Force W before E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("bushW", "Auto W bush after enemy enter", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("bushW2", "Auto W bush and turret if full ammo", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("Wspell", "W on special spell detection", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").SubMenu("W Gap Closer").AddItem(new MenuItem("WmodeGC", "Gap Closer position mode", true).SetValue(new StringList(new[] { "Dash end position", "My hero position" }, 0)));
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("W Config").SubMenu("W Gap Closer").SubMenu("Cast on enemy:").AddItem(new MenuItem("WGCchampion" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("Ehitchance", "Auto E dash and immobile target", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harrasEQ", "TRY E + Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("EQks", "Ks E + Q + AA", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("useE", "Dash E HotKeySmartcast", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("E Gap Closer").AddItem(new MenuItem("EmodeGC", "Gap Closer position mode", true).SetValue(new StringList(new[] { "Dash end position", "Cursor position", "Enemy position" }, 2)));
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("E Config").SubMenu("E Gap Closer").SubMenu("Cast on enemy:").AddItem(new MenuItem("EGCchampion" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R KS", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rcol", "R collision width [400]", true).SetValue(new Slider(400, 1000, 1)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rrange", "R minimum range [1000]", true).SetValue(new Slider(1000, 1500, 1)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rturrent", "Don't R under turret", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            //Orbwalking.BeforeAttack += BeforeAttack;
            //Orbwalking.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        static Vector3 wCastPos;
        static int wCastTimeMax;
        
        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W)
            {
                foreach (var particle in ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(x => x.IsValid && x.Position.Distance(args.EndPosition) < 300))
                {
                    var name = particle.Name.ToLower();

                    if (name.StartsWith("caitlyn") && name.Contains("yordletrap") && name.Contains("green"))
                    {
                        args.Process = false;
                        return;
                    }
                }
            }

            if (args.Slot == SpellSlot.E && Player.Mana > RMANA + WMANA && Config.Item("forceW", true).GetValue<bool>())
            {
                wCastPos = Player.Position.Extend(args.EndPosition, Player.Distance(args.EndPosition) + 50);
                wCastTimeMax = Utils.TickCount + 500;
                W.Cast(wCastPos);
                //W.Cast(Player.Position.Extend(args.EndPosition, Player.Distance(args.EndPosition) + 50));
                //Utility.DelayAction.Add(10, () => E.Cast(args.EndPosition));
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && (args.SData.Name == "CaitlynPiltoverPeacemaker" || args.SData.Name == "CaitlynEntrapment"))
            {
                QCastTime = Game.Time;
            }

            if (!W.IsReady() || sender.IsMinion || !sender.IsEnemy || !Config.Item("Wspell", true).GetValue<bool>() || !sender.IsValid<Obj_AI_Hero>() || !sender.IsValidTarget(W.Range))
                return;

            var foundSpell = Spells.Find(x => args.SData.Name.ToLower() == x);
            if (foundSpell != null)
            {
                W.Cast(sender.Position);
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.Mana > RMANA + WMANA)
            {
                var t = gapcloser.Sender;
                if (E.IsReady() && t.IsValidTarget(E.Range) && Config.Item("EGCchampion" + t.ChampionName, true).GetValue<bool>())
                {
                    if (Config.Item("EmodeGC", true).GetValue<StringList>().SelectedIndex == 0)
                        E.Cast(gapcloser.End);
                    else if (Config.Item("EmodeGC", true).GetValue<StringList>().SelectedIndex == 1)
                        E.Cast(Game.CursorPos);
                    else
                        E.Cast(t.ServerPosition);
                }
                else if (W.IsReady() && t.IsValidTarget(W.Range) && Config.Item("WGCchampion" + t.ChampionName, true).GetValue<bool>())
                {
                    if (Config.Item("WmodeGC", true).GetValue<StringList>().SelectedIndex == 0)
                        W.Cast(gapcloser.End);
                    else
                        W.Cast(Player.ServerPosition);
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsRecalling())
                return;

            if (/*!W.IsReady() ||*/ Utils.TickCount > wCastTimeMax)
            {
                wCastPos = Vector3.Zero;
            }
            else if (wCastPos != Vector3.Zero)
            {
                //Player.Spellbook.CastSpell(SpellSlot.Item1, wCastPos);
                if (wCastPos.CountEnemiesInRange(800) < 3)
                {
                    var galeforce = new Items.Item(6671, 800);
                    galeforce.Cast(wCastPos);
                    W.Cast(wCastPos);
                }
            }

            if (Config.Item("useR", true).GetValue<KeyBind>().Active && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget())
                    R.CastOnUnit(t);
            }

            if (Program.LagFree(0))
            {
                SetMana();
                R.Range = (500 * R.Level) + 1500;
                //debug("" + ObjectManager.Player.AttackRange);
            }

            if (Program.LagFree(1) && E.IsReady() && Orbwalking.CanMove(40))
                LogicE();
            var orbT = Orbwalker.GetTarget() as Obj_AI_Hero;
            if (orbT != null)
            {
                if (Player.GetAutoAttackDamage(orbT) * 2 > orbT.Health)
                    return;
            }

            if (Program.LagFree(2) && W.IsReady())
                LogicW();
            if (Program.LagFree(3) && Q.IsReady() && Orbwalking.CanMove(50) && Config.Item("autoQ2", true).GetValue<bool>())
                LogicQ();
            if (Program.LagFree(4) && R.IsReady() && Config.Item("autoR", true).GetValue<bool>() && !ObjectManager.Player.UnderTurret(true) && Game.Time - QCastTime > 1)
                LogicR();
        }

        private void LogicR()
        {
            bool cast = false;

            if (Player.UnderTurret(true) && Config.Item("Rturrent", true).GetValue<bool>())
                return;

            foreach (var target in HeroManager.Enemies.Where(target => target.IsValidTarget(R.Range) && Player.Distance(target.Position) > Config.Item("Rrange", true).GetValue<Slider>().Value && target.CountEnemiesInRange(Config.Item("Rcol", true).GetValue<Slider>().Value) == 1 && target.CountAlliesInRange(500) == 0 && OktwCommon.ValidUlt(target)))
            {
                if (OktwCommon.GetKsDamage(target, R) > target.Health)
                {
                    cast = true;
                    PredictionOutput output = R.GetPrediction(target);
                    Vector2 direction = output.CastPosition.To2D() - Player.Position.To2D();
                    direction.Normalize();
                    List<Obj_AI_Hero> enemies = HeroManager.Enemies.Where(x => x.IsValidTarget()).ToList();
                    foreach (var enemy in enemies)
                    {
                        if (enemy.SkinName == target.SkinName || !cast)
                            continue;
                        PredictionOutput prediction = R.GetPrediction(enemy);
                        Vector3 predictedPosition = prediction.CastPosition;
                        Vector3 v = output.CastPosition - Player.ServerPosition;
                        Vector3 w = predictedPosition - Player.ServerPosition;
                        double c1 = Vector3.Dot(w, v);
                        double c2 = Vector3.Dot(v, v);
                        double b = c1 / c2;
                        Vector3 pb = Player.ServerPosition + ((float)b * v);
                        float length = Vector3.Distance(predictedPosition, pb);
                        if (length < (Config.Item("Rcol", true).GetValue<Slider>().Value + enemy.BoundingRadius) && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                            cast = false;
                    }
                    if (cast)
                        R.CastOnUnit(target);
                }
            }
        }

        private void LogicW()
        {
            if (Player.Mana > RMANA + WMANA && Orbwalking.CanMove(50) && !Orbwalking.CanAttack())
            {
                if (Config.Item("autoW", true).GetValue<bool>())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy) && !enemy.HasBuff("caitlynyordletrapinternal")))
                    {
                        W.Cast(enemy);
                    }
                }

                if (Config.Item("telE", true).GetValue<bool>())
                {
                    var trapPos = OktwCommon.GetTrapPos(W.Range);
                    if (!trapPos.IsZero)
                        W.Cast(trapPos);
                }

                if ((int)(Game.Time * 10) % 2 == 0 && Config.Item("bushW2", true).GetValue<bool>() && !Orbwalker.ShouldWait())
                {
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Ammo == new int[] { 0, 3, 3, 4, 4, 5 }[W.Level] && Player.CountEnemiesInRange(1000) == 0)
                    {
                        var points = OktwCommon.CirclePoints(8, W.Range, Player.Position);
                        foreach (var point in points)
                        {
                            if (NavMesh.IsWallOfGrass(point, 0) || point.UnderTurret(true))
                            {
                                if (!OktwCommon.CirclePoints(8, 150, point).Any(x => x.IsWall()))
                                {
                                    W.Cast(point);
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
            if (Program.Combo && Player.IsWindingUp)
                return;
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget(Q.Range))
            {
                if (GetRealDistance(t) > bonusRange() + 250 && !Orbwalking.InAutoAttackRange(t) && OktwCommon.GetKsDamage(t, Q) > t.Health && Player.CountEnemiesInRange(400) == 0)
                {
                    Program.CastSpell(Q, t);
                    Program.debug("Q KS");
                }
                else if (Program.Combo && Player.Mana > RMANA + QMANA + EMANA + 10 && Player.CountEnemiesInRange(bonusRange() + 100 + t.BoundingRadius) == 0 && !Config.Item("autoQ", true).GetValue<bool>())
                    Program.CastSpell(Q, t);
                if ((Program.Combo || Program.Harass) && Player.Mana > RMANA + QMANA && Player.CountEnemiesInRange(400) == 0)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && (!OktwCommon.CanMove(enemy) || enemy.HasBuff("caitlynyordletrapinternal"))))
                        Q.Cast(enemy, true);
                    if (Player.CountEnemiesInRange(bonusRange()) == 0 && OktwCommon.CanHarras())
                    {
                        if (t.HasBuffOfType(BuffType.Slow) && Config.Item("Qslow", true).GetValue<bool>())
                            Q.Cast(t);
                        if (Config.Item("Qaoe", true).GetValue<bool>())
                            Q.CastIfWillHit(t, 2, true);
                    }
                }
            }
            else if (FarmSpells && Config.Item("farmQ", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit >= FarmMinions)
                    Q.Cast(farmPosition.Position);
            }
        }

        private void LogicE()
        {
            if (Program.Combo && Player.IsWindingUp)
                return;
            if (Config.Item("autoE", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var positionT = Player.ServerPosition - (t.Position - Player.ServerPosition);

                    if (Player.Position.Extend(positionT, 400).CountEnemiesInRange(700) < 2 && Orbwalking.CanMove(0) && !Orbwalking.CanAttack())
                    {
                        var eDmg = E.GetDamage(t);
                        var qDmg = Q.GetDamage(t);
                        if (Config.Item("EQks", true).GetValue<bool>() && qDmg + eDmg + Player.GetAutoAttackDamage(t) > t.Health && Player.Mana > EMANA + QMANA)
                        {
                            Program.CastSpell(E, t);
                            Program.debug("E + Q FINISH");
                        }
                        else if ((Program.Harass || Program.Combo) && Config.Item("harrasEQ", true).GetValue<bool>() && Player.Mana > EMANA + QMANA + RMANA)
                        {
                            Program.CastSpell(E, t);
                            Program.debug("E + Q Harras");
                        }
                    }

                    if (Player.Mana > RMANA + EMANA)
                    {
                        if (Config.Item("Ehitchance", true).GetValue<bool>())
                        {
                            if((Orbwalking.CanMove(0) && !Orbwalking.CanAttack()) || t.IsDashing())
                                E.CastIfHitchanceEquals(t, HitChance.Dashing);
                        }
                        if (Player.Health < Player.MaxHealth * 0.3)
                        {
                            if (GetRealDistance(t) < 500)
                                E.Cast(t, true);
                            if (Player.CountEnemiesInRange(250) > 0)
                                E.Cast(t, true);
                        }
                    }

                }
            }
            if (Config.Item("useE", true).GetValue<KeyBind>().Active)
            {
                var position = Player.ServerPosition - (Game.CursorPos - Player.ServerPosition);
                E.Cast(position, true);
            }
        }

        private float GetRealRange(GameObject target)
        {
            return 680f + Player.BoundingRadius + target.BoundingRadius;
        }

        private float GetRealDistance(GameObject target)
        {
            return Player.ServerPosition.Distance(target.Position) + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }
        public float bonusRange()
        {
            return 720f + Player.BoundingRadius;
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
            if (Config.Item("noti", true).GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget() && R.IsReady())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                }

                var tw = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (tw.IsValidTarget())
                {
                    if (Q.GetDamage(tw) > tw.Health)
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "Q can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                }
            }
        }
    }
}
