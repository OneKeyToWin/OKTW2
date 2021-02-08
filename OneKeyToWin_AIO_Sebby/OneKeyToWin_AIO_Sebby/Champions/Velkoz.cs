using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Velkoz : Base
    {
        private Spell  QSplit, QDummy;
        private MissileClient QMissile = null;
        private List<Vector3> pointList;
        private static int QMissileCreateTick;
        private static SharpDX.Vector3 StartPosition, EndPosition;

        public Velkoz()
        {
            Q = new Spell(SpellSlot.Q, 1180);
            QSplit = new Spell(SpellSlot.Q, 1000);
            QDummy = new Spell(SpellSlot.Q, (float)Math.Sqrt(Math.Pow(Q.Range, 2) + Math.Pow(QSplit.Range, 2)));
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 1500);

            Q.SetSkillshot(0.25f, 70f, 1300f, true, SkillshotType.SkillshotLine);
            QSplit.SetSkillshot(0.1f, 70f, 2100f, true, SkillshotType.SkillshotLine);
            QDummy.SetSkillshot(0.5f, 55f, 1200, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 85f, 1700f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(1f, 180f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.1f, 80f, float.MaxValue, false, SkillshotType.SkillshotLine);

            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            HeroMenu.SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw when skill rdy", true).SetValue(true));

            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Harass Q", true).SetValue(true));
            HeroMenu.SubMenu("Q Config").AddItem(new MenuItem("QHarassMana", "Harass Mana", true).SetValue(new Slider(30, 100, 0)));

            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            HeroMenu.SubMenu("W Config").AddItem(new MenuItem("harassW", "Harass W", true).SetValue(true));

            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("harassE", "Harass E", true).SetValue(false));
            HeroMenu.SubMenu("E Config").AddItem(new MenuItem("EInterrupter", "Auto E Interrupter", true).SetValue(true));
            HeroMenu.SubMenu("E Config").SubMenu("E Gap Closer").AddItem(new MenuItem("EmodeGC", "Gap Closer position mode", true).SetValue(new StringList(new[] { "Dash end position", "Player position", "Prediction" }, 0)));
            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("E Config").SubMenu("E Gap Closer").SubMenu("Cast on enemy:").AddItem(new MenuItem("EGCchampion" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            HeroMenu.SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R KS", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;
            if (E.IsReady() && t.IsValidTarget(E.Range) && MainMenu.Item("EGCchampion" + t.ChampionName, true).GetValue<bool>())
            {
                if (MainMenu.Item("EmodeGC", true).GetValue<StringList>().SelectedIndex == 0)
                    E.Cast(gapcloser.End);
                else if (MainMenu.Item("EmodeGC", true).GetValue<StringList>().SelectedIndex == 1)
                    E.Cast(Player.Position);
                else
                    E.Cast(t.ServerPosition);
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(E.Range) && MainMenu.Item("EInterrupter", true).GetValue<bool>())
            {
                E.Cast(sender);
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid<MissileClient>() && sender.IsAlly)
            {
                MissileClient missile = (MissileClient)sender;
                if (missile.SData.Name != null && missile.SData.Name == "VelkozQMissile")
                {
                    QMissile = missile;
                    QMissileCreateTick = Utils.TickCount + 25;
                    StartPosition = missile.Position;
                    EndPosition = missile.EndPosition;
                }
            }
        }

        private SharpDX.Vector3 MissilePosition()
        {
            if (QMissile != null)
            {
                return StartPosition.Extend(EndPosition, (Utils.TickCount - QMissileCreateTick) / 1000f * Q.Speed);
            }

            return SharpDX.Vector3.Zero;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsChannelingImportantSpell() && R.IsReady() && R.Instance.ToggleState == 1)
            {
                if (MainMenu.Item("autoR", true).GetValue<bool>())
                {
                    var t = TargetSelector.GetTarget(R.Range + 150, TargetSelector.DamageType.Magical);
                    if (t.IsValidTarget() && OktwCommon.ValidUlt(t))
                    {
                        Player.Spellbook.UpdateChargedSpell(SpellSlot.R, R.GetPrediction(t, true).CastPosition, true);
                    }
                }
                OktwCommon.blockMove = true;
                OktwCommon.blockAttack = true;
                Orbwalking.Attack = false;
                Orbwalking.Move = false;
            }
            else 
            {
                if (R.IsReady() && MainMenu.Item("autoR", true).GetValue<bool>())
                    LogicR();
                OktwCommon.blockMove = false;
                OktwCommon.blockAttack = false;
                Orbwalking.Attack = true;
                Orbwalking.Move = true;
            }

            if (Program.LagFree(4))
            {
                //R.Cast(Game.CursorPos);
            }

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Q.IsReady() && MainMenu.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(3) && E.IsReady() && MainMenu.Item("autoE", true).GetValue<bool>())
                LogicE();

            if (Program.LagFree(4))
            {
                if(W.IsReady() && MainMenu.Item("autoW", true).GetValue<bool>())
                    LogicW();
            }
        }

        private void LogicR()
        {            
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget()  && Player.CountEnemiesInRange(400) == 0 && !Player.UnderTurret(true))
            {
                //900 - 100%
                //1500 - 10 %
                var rDmg = OktwCommon.GetKsDamage(t, R);
                var distance = Player.Distance(t);

                if (distance > 900 && OktwCommon.CanMove(t))
                {
                    float adjust = (R.Range - distance) / 600;
                    rDmg = rDmg * adjust;
                }

                if(rDmg > t.Health && OktwCommon.ValidUlt(t))
                {
                    R.Cast(t);
                }
            }
        }

        private void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    Program.CastSpell(E, t);
                else if (Program.Harass && OktwCommon.CanHarras() && MainMenu.Item("harassE", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(E, t);
                else
                {
                    var eDmg = OktwCommon.GetKsDamage(t, E);
                    var qDmg = Q.GetDamage(t);
                    if (qDmg + eDmg > t.Health)
                    {
                        if(eDmg > t.Health)
                            Program.CastSpell(E, t);
                        else if (Player.Mana > QMANA + EMANA)
                            Program.CastSpell(E, t);
                        return;
                    }
                }
                if (!Program.None && Player.Mana > RMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                        E.Cast(enemy);
                }
            }
            else if (FarmSpells && MainMenu.Item("farmE", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, E.Range);
                var farmPosition = E.GetCircularFarmLocation(minionList, E.Width);

                if (farmPosition.MinionsHit >= FarmMinions)
                    E.Cast(farmPosition.Position);
            }
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA)
                    Program.CastSpell(W, t);
                else if (Program.Harass && MainMenu.Item("harassW", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() 
                    && Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                    Program.CastSpell(W, t);
                else
                {
                    var wDmg = OktwCommon.GetKsDamage(t, W);
                    var qDmg = Q.GetDamage(t);
                    if (wDmg > t.Health)
                        Program.CastSpell(W, t);
                    else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                        Program.CastSpell(W, t);
                }
                if (!Program.None && Player.Mana > RMANA + WMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
            else if (FarmSpells && MainMenu.Item("farmW", true).GetValue<bool>())
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetLineFarmLocation(minionList, W.Width);

                if (farmPosition.MinionsHit >= FarmMinions)
                    W.Cast(farmPosition.Position);
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(QDummy.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Q.Instance.ToggleState == 0 && Utils.TickCount - Q.LastCastAttemptT > 150)
                {
                    if (Program.LagFree(1) || Program.LagFree(2))
                    {
                        QSplit.Collision = true;
                        if (Program.Combo && Player.Mana > RMANA + QMANA )
                            CastQ(t);
                        else if (Program.Harass && OktwCommon.CanHarras() && MainMenu.Item("harassQ", true).GetValue<bool>() && MainMenu.Item("Harass" + t.ChampionName).GetValue<bool>() && Player.ManaPercent > MainMenu.Item("QHarassMana", true).GetValue<Slider>().Value)
                            CastQ(t);
                        else
                        {
                            var qDmg = OktwCommon.GetKsDamage(t, Q);
                            var wDmg = W.GetDamage(t);
                            if (qDmg > t.Health )
                                CastQ(t);
                            else if(qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                                CastQ(t);
                        }
                        if (!Program.None && Player.Mana > RMANA + QMANA)
                        {

                            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(QDummy.Range) && !OktwCommon.CanMove(enemy)))
                                CastQ(t);
                        }
                    }
                }
                else
                {
                    DetonateQ(t);
                }
            }
        }

        private void CastQ(Obj_AI_Base t)
        {
            if (Q.Instance.ToggleState != 0)
                return;

            var Qpred = Q.GetPrediction(t);
            if (Qpred.Hitchance >= HitChance.High)
            {
                Program.CastSpell(Q, t);
            }
            else
            {
                var pred = QDummy.GetPrediction(t);
                if (pred.Hitchance >= HitChance.High)
                {
                    if (Program.LagFree(1))
                        pointList = AimQ(pred.CastPosition);
                    if (Program.LagFree(2))
                        BestAim(pred.CastPosition);
                }
            }
        }

        private void DetonateQ(Obj_AI_Base t)
        {
            if (QMissile != null && QMissile.IsValid && Q.Instance.ToggleState == 1)
            {
                QSplit.Collision = false;
                var realPosition = QMissile.StartPosition.Extend(QMissile.EndPosition, QMissile.StartPosition.Distance(MissilePosition()) + Game.Ping / 2 + 60);
                //Q.Cast();

                QSplit.UpdateSourcePosition(realPosition, realPosition);

                Vector2 start = QMissile.StartPosition.To2D();
                Vector2 end = realPosition.To2D();
                var radius = QSplit.Range;

                var dir = (end - start).Normalized();
                var pDir = dir.Perpendicular();

                var rightEndPos = end + pDir * radius;
                var leftEndPos = end - pDir * radius;

                var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
                var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);

                if (QSplit.WillHit(t, rEndPos) || QSplit.WillHit(t, lEndPos))
                    Q.Cast();
            }

        }

        private List<Vector3> AimQ(Vector3 finalPos)
        {
            var CircleLineSegmentN = 36;
            var radius = 500;
            var position = Player.Position;

            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
                if (point.Distance(Player.Position.Extend(finalPos, radius)) < 430)
                {
                    points.Add(point);
                    //Utility.DrawCircle(point, 20, System.Drawing.Color.Aqua, 1, 1);
                }
            }

            var point2 = points.OrderBy(x => x.Distance(finalPos));
            points = point2.ToList();
            points.RemoveAt(0);
            points.RemoveAt(1);
            return points;
        }

        private void BestAim(Vector3 predictionPos)
        {
            Vector2 start = Player.Position.To2D();
            var c1 = predictionPos.Distance(Player.Position);
            var playerPos2d = Player.Position.To2D();

            foreach ( var point in pointList )
            {
                for (var j = 400; j <= 1100; j = j + 50)
                {
                    var posExtend = Player.Position.Extend(point, j);

                    var a1 = Player.Distance(posExtend);
                    float b1 = (float)Math.Sqrt((c1 * c1) - (a1 * a1));

                    if (b1 > QSplit.Range)
                        continue;
                    
                    var pointA = Player.Position.Extend(point, a1);

                    Vector2 end = pointA.To2D();
                    var dir = (end - start).Normalized();
                    var pDir = dir.Perpendicular();

                    var rightEndPos = end + pDir * b1;
                    var leftEndPos = end - pDir * b1;

                    var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
                    var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);

                    if (lEndPos.Distance(predictionPos) < QSplit.Width)
                    {
                        var collision = Q.GetCollision(playerPos2d, new List<Vector2> { posExtend.To2D() });
                        if (collision.Count > 0)
                            break;

                        var collisionS = QSplit.GetCollision(pointA.To2D(), new List<Vector2> { lEndPos.To2D() });
                        if (collisionS.Count > 0)
                            break;

                        Q.Cast(pointA);
                        return;
                    }
                    if ( rEndPos.Distance(predictionPos) < QSplit.Width)
                    {
                        var collision = Q.GetCollision(playerPos2d, new List<Vector2> { posExtend.To2D() });
                        if (collision.Count > 0)
                            break;

                        var collisionR = QSplit.GetCollision(pointA.To2D(), new List<Vector2> { rEndPos.To2D() });
                        if (collisionR.Count > 0)
                            break;

                        Q.Cast(pointA);
                        return;
                    }
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && MainMenu.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    else if (Q.IsReady() && MainMenu.Item("jungleQ", true).GetValue<bool>())
                    {
                        if (Q.Instance.ToggleState == 0)
                        {
                            Q.Cast(mob.ServerPosition);
                            return;
                        }
                    }

                    else if (E.IsReady() && MainMenu.Item("jungleE", true).GetValue<bool>())
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            //Utility.DrawCircle(Game.CursorPos, E.Width, System.Drawing.Color.Cyan, 1, 1);
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
    }
}
