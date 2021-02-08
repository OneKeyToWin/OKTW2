
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Jhin : Base
    {
        private bool Ractive = false;
        private Vector3 rPosLast;
        private Obj_AI_Hero rTargetLast;
        private Vector3 rPosCast;

        private Items.Item
                    FarsightOrb = new Items.Item(3342, 4000f),
                    ScryingOrb = new Items.Item(3363, 3500f);

        private static string[] Spells =
        {
            "katarinar","drain","consume","absolutezero", "staticfield","reapthewhirlwind","jinxw","jinxr","shenstandunited","threshe","threshrpenta","threshq","meditate","caitlynpiltoverpeacemaker", "volibearqattack",
            "cassiopeiapetrifyinggaze","ezrealtrueshotbarrage","galioidolofdurand","luxmalicecannon", "missfortunebullettime","infiniteduress","alzaharnethergrasp","lucianq","velkozr","rocketgrabmissile"
        };

        public Jhin()
        {
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 2900);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 3500);

            W.SetSkillshot(0.75f, 40, 10000, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(1f, 120, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.24f, 80, 5000, false, SkillshotType.SkillshotLine);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRangeMini", "R range minimap", true).SetValue(true));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("Qminion", "Q on minion", true).SetValue(true));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoWcombo", "Auto W only in combo", true).SetValue(false));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("Wmark", "W marked only (main target)", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("Wmarkall", "W marked (all enemys)", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("Waoe", "W aoe (above 2 enemy)", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoWcc", "Auto W CC enemy", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("MaxRangeW", "Max W range", true).SetValue(new Slider(2500, 2500, 0)));

            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E on hard CC", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("bushE", "Auto E bush", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("Espell", "E on special spell detection", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("EmodeCombo", "E combo mode", true).SetValue(new StringList(new[] { "always", "run - cheese", "disable" }, 1)));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("Eaoe", "Auto E x enemies", true).SetValue(new Slider(3, 5, 0)));
            HeroMenu.SubMenu("E Config").SubMenu("E Gap Closer").AddItem(new MenuItem("EmodeGC", "Gap Closer position mode", true).SetValue(new StringList(new[] { "Dash end position", "My hero position" }, 0)));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("E Config").SubMenu("E Gap Closer").AddItem(new MenuItem("EGCchampion" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Enable R", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("Rvisable", "Don't shot if enemy is not visable", true).SetValue(false));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("Rks", "Auto R if can kill in 3 hits", true).SetValue(true));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("MaxRangeR", "Max R range", true).SetValue(new Slider(3000, 3500, 0)));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("MinRangeR", "Min R range", true).SetValue(new Slider(1000, 3500, 0)));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("Rsafe", "R safe area", true).SetValue(new Slider(1000, 2000, 0)));
            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("trinkiet", "Auto blue trinkiet", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                if (MainMenu.Item("trinkiet", true).GetValue<bool>() && !IsCastingR)
                {
                    if (Player.Level < 9)
                        ScryingOrb.Range = 2500;
                    else
                        ScryingOrb.Range = 3500;

                    if (ScryingOrb.IsReady())
                        ScryingOrb.Cast(rPosLast);
                    if (FarsightOrb.IsReady())
                        FarsightOrb.Cast(rPosLast);
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(sender.IsMe && args.SData.Name.ToLower() == "jhinr")
            {
                rPosCast = args.End;
            }
            if (!E.IsReady() || sender.IsMinion || !sender.IsEnemy || !MainMenu.Item("Espell", true).GetValue<bool>() || !sender.IsValid<Obj_AI_Hero>() || !sender.IsValidTarget(E.Range))
                return;

            var foundSpell = Spells.Find(x => args.SData.Name.ToLower() == x);
            if (foundSpell != null)
            {
                E.Cast(sender.Position);
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && Player.Mana > RMANA + WMANA)
            {
                var t = gapcloser.Sender;
                if (t.IsValidTarget(W.Range) && MainMenu.Item("EGCchampion" + t.ChampionName, true).GetValue<bool>())
                {
                    if (MainMenu.Item("EmodeGC", true).GetValue<StringList>().SelectedIndex == 0)
                        E.Cast(gapcloser.End);
                    else
                        E.Cast(Player.ServerPosition);
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(1) && R.IsReady() )
                LogicR();

            if (IsCastingR)
            {
                OktwCommon.blockMove = true;
                OktwCommon.blockAttack = true;
                Orbwalking.Attack = false;
                Orbwalking.Move = false;
                return;
            }
            else
            {
                OktwCommon.blockMove = false;
                OktwCommon.blockAttack = false;
                Orbwalking.Attack = true;
                Orbwalking.Move = true;
            }


            if (Program.LagFree(4) && E.IsReady() && Orbwalking.CanMove(50))
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && !Player.IsWindingUp && MainMenu.Item("autoW", true).GetValue<bool>())
                LogicW();
        }

        private void LogicR()
        {
            if (!IsCastingR)
                R.Range = MainMenu.Item("MaxRangeR", true).GetValue<Slider>().Value;
            else
                R.Range = 3500;

            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                rPosLast = R.GetPrediction(t).CastPosition;
                if (MainMenu.Item("useR", true).GetValue<KeyBind>().Active && !IsCastingR)
                {
                    R.Cast(rPosLast);
                    rTargetLast = t;
                }
                
                if (!IsCastingR && MainMenu.Item("Rks", true).GetValue<bool>() && MainMenu.Item("autoR", true).GetValue<bool>()
                    && GetRdmg(t) * 4 > t.Health && t.CountAlliesInRange(700) == 0 && Player.CountEnemiesInRange(MainMenu.Item("Rsafe", true).GetValue<Slider>().Value) == 0 
                    && Player.Distance(t) > MainMenu.Item("MinRangeR", true).GetValue<Slider>().Value
                    && !Player.UnderTurret(true) && OktwCommon.ValidUlt(t) && !OktwCommon.IsSpellHeroCollision(t, R))
                {
                    R.Cast(rPosLast);
                    rTargetLast = t;
                }
                if (IsCastingR)
                {
                    if(InCone(t.ServerPosition))
                        R.Cast(t);
                    else
                    {
                        foreach(var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && InCone(t.ServerPosition)).OrderBy(enemy => enemy.Health))
                        {
                            R.Cast(t);
                            rPosLast = R.GetPrediction(enemy).CastPosition;
                            rTargetLast = enemy;
                        }
                    }
                }
            }
            else if (IsCastingR && rTargetLast != null && !rTargetLast.IsDead)
            {
                if(!MainMenu.Item("Rvisable", true).GetValue<bool>() && InCone(rTargetLast.Position) && InCone(rPosLast))
                    R.Cast(rPosLast);
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                var wDmg = GetWdmg(t);
                if (wDmg > t.Health - OktwCommon.GetIncomingDamage(t))
                    Program.CastSpell(W, t);

                if (MainMenu.Item("autoWcombo", true).GetValue<bool>() && !Program.Combo)
                    return;

                if (Player.CountEnemiesInRange(400) > 1 || Player.CountEnemiesInRange(250) > 0)
                    return;

                if (t.HasBuff("jhinespotteddebuff") || !MainMenu.Item("Wmark", true).GetValue<bool>() )
                {
                    if (Player.Distance(t) < MainMenu.Item("MaxRangeW", true).GetValue<Slider>().Value)
                    {
                        if (Program.Combo && Player.Mana > RMANA + WMANA)
                            Program.CastSpell(W, t);
                        else if (Program.Harass && MainMenu.Item("harassW", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>()
                            && Player.Mana > RMANA + WMANA + QMANA + WMANA && OktwCommon.CanHarras())
                            Program.CastSpell(W, t);
                    }
                }

                if (!Program.None && Player.Mana > RMANA + WMANA)
                {
                    if(MainMenu.Item("Waoe", true).GetValue<bool>())
                        W.CastIfWillHit(t, 2);

                    if (MainMenu.Item("autoWcc", true).GetValue<bool>())
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                            Program.CastSpell(W, enemy);
                    }
                    if (MainMenu.Item("Wmarkall", true).GetValue<bool>())
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && enemy.HasBuff("jhinespotteddebuff")))
                            Program.CastSpell(W, enemy);
                    }
                }
            }
            if (FarmSpells && MainMenu.Item("farmW", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetLineFarmLocation(minionList, W.Width);

                if (farmPosition.MinionsHit >= FarmMinions)
                    W.Cast(farmPosition.Position);
            }
        }

        private void LogicE()
        {
            if (MainMenu.Item("autoE", true).GetValue<bool>())
            {
                var trapPos = OktwCommon.GetTrapPos(E.Range);
                if (!trapPos.IsZero)
                    E.Cast(trapPos);

                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                    E.Cast(enemy);
            }

            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget() && MainMenu.Item("EmodeCombo", true).GetValue<StringList>().SelectedIndex != 2)
            {
                if (Program.Combo && !Player.IsWindingUp)
                {
                    if (MainMenu.Item("EmodeCombo", true).GetValue<StringList>().SelectedIndex == 1)
                    {
                        if (E.GetPrediction(t).CastPosition.Distance(t.Position) > 100)
                        {
                            if (Player.Position.Distance(t.ServerPosition) > Player.Position.Distance(t.Position))
                            {
                                if (t.Position.Distance(Player.ServerPosition) < t.Position.Distance(Player.Position))
                                    Program.CastSpell(E, t);
                            }
                            else
                            {
                                if (t.Position.Distance(Player.ServerPosition) > t.Position.Distance(Player.Position))
                                    Program.CastSpell(E, t);
                            }
                        }
                    }
                    else
                    {
                        Program.CastSpell(E, t);
                    }
                }

                E.CastIfWillHit(t, MainMenu.Item("Eaoe", true).GetValue<Slider>().Value);
            }
            else if (FarmSpells && MainMenu.Item("farmE", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, E.Range);
                var farmPosition = E.GetCircularFarmLocation(minionList, E.Width);

                if (farmPosition.MinionsHit >= FarmMinions)
                    E.Cast(farmPosition.Position);
            }
        }

        private void LogicQ()
        {
            var torb = Orbwalker.GetTarget();

            if (torb == null || torb.Type != GameObjectType.obj_AI_Hero)
            {
                if (MainMenu.Item("Qminion", true).GetValue<bool>())
                {
                    var t = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget() )
                    {
                        
                        var minion = Cache.GetMinions(Prediction.GetPrediction(t, 0.1f).CastPosition, 300).Where(minion2 => minion2.IsValidTarget(Q.Range)).OrderBy(x => x.Distance(t)).FirstOrDefault();
                        if (minion.IsValidTarget())
                        {
                            if (t.Health < GetQdmg(t))
                                Q.CastOnUnit(minion);
                            if (Program.Combo && Player.Mana > RMANA + EMANA)
                                Q.CastOnUnit(minion);
                            else if (Program.Harass && MainMenu.Item("harassQ", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + EMANA && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                                Q.CastOnUnit(minion);
                        }
                    }
                }

            }
            else if(!Orbwalking.CanAttack() && !Player.IsWindingUp)
            {
                var t = torb as Obj_AI_Hero;
                if (t.Health < GetQdmg(t) + GetWdmg(t))
                    Q.CastOnUnit(t);
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Q.CastOnUnit(t);
                else if (Program.Harass && MainMenu.Item("harassQ", true).GetValue<bool>() && Player.Mana > RMANA + QMANA + WMANA + EMANA && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>())
                    Q.CastOnUnit(t);
            }
            if (FarmSpells && MainMenu.Item("farmQ", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                
                if (minionList.Count >= FarmMinions)
                {
                    var minionAttack = minionList.FirstOrDefault(x => Q.GetDamage(x) > HealthPrediction.GetHealthPrediction(x, 300));
                    if(minionAttack.IsValidTarget())
                        Q.CastOnUnit(minionAttack);
                }
                    
            }
        }


        private bool InCone(Vector3 Position)
        {
            var range = R.Range;
            var angle = 70f * (float)Math.PI / 180;
            var end2 = rPosCast.To2D() - Player.Position.To2D();
            var edge1 = end2.Rotated(-angle / 2);
            var edge2 = edge1.Rotated(angle);

            var point = Position.To2D() - Player.Position.To2D();
            if (point.Distance(new Vector2(), true) < range * range && edge1.CrossProduct(point) > 0 && point.CrossProduct(edge2) > 0)
                return true;

            return false;
        }

        private void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (W.IsReady() && MainMenu.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    if (E.IsReady() && MainMenu.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                    if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.CastOnUnit(mob);
                        return;
                    }
                }
            }
        }

        private bool IsCastingR { get { return R.Instance.Name == "JhinRShot"; } }

        private double GetRdmg(Obj_AI_Base target)
        {
            var damage = ( -25 + 75 * R.Level + 0.2 * Player.FlatPhysicalDamageMod) * (1 + (100 - target.HealthPercent) * 0.02);

            return Player.CalcDamage(target, Damage.DamageType.Physical, damage);
        }

        private double GetWdmg(Obj_AI_Base target)
        {
            var damage = 55 + W.Level * 35 + 0.7 * Player.FlatPhysicalDamageMod;

            return Player.CalcDamage(target, Damage.DamageType.Physical, damage);
        }

        private double GetQdmg(Obj_AI_Base target)
        {
            var damage = 35 + Q.Level * 25 + 0.4 * Player.FlatPhysicalDamageMod;

            return Player.CalcDamage(target, Damage.DamageType.Physical, damage);
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
                if (MainMenu.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);
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
