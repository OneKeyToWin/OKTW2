using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class OKTWdash : Program
    {
        private static Spell DashSpell;

        public OKTWdash(Spell qwer)
        {
            DashSpell = qwer;
            
            Config.SubMenu(Player.ChampionName).SubMenu(qwer.Slot + " Config").AddItem(new MenuItem("DashMode", "Dash MODE", true).SetValue(new StringList(new[] { "Game Cursor", "Side", "Safe position" }, 2)));
            Config.SubMenu(Player.ChampionName).SubMenu(qwer.Slot + " Config").AddItem(new MenuItem("EnemyCheck", "Block dash in x enemies ", true).SetValue(new Slider(3, 5, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu(qwer.Slot + " Config").AddItem(new MenuItem("WallCheck", "Block dash in wall", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu(qwer.Slot + " Config").AddItem(new MenuItem("TurretCheck", "Block dash under turret", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu(qwer.Slot + " Config").AddItem(new MenuItem("AAcheck", "Dash only in AA range", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu(qwer.Slot + " Config").SubMenu("Gapcloser").AddItem(new MenuItem("GapcloserMode", "Gapcloser MODE", true).SetValue(new StringList(new[] { "Game Cursor", "Away - safe position", "Disable" }, 1)));
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu(qwer.Slot + " Config").SubMenu("Gapcloser").AddItem(new MenuItem("EGCchampion" + enemy.ChampionName, enemy.ChampionName, true).SetValue(true));

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (DashSpell.IsReady() && Config.Item("EGCchampion" + gapcloser.Sender.ChampionName, true).GetValue<bool>())
            {
                int GapcloserMode = Config.Item("GapcloserMode", true).GetValue<StringList>().SelectedIndex;
                if (GapcloserMode == 0)
                {
                    var bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range);
                    if (IsGoodPosition(bestpoint))
                        DashSpell.Cast(bestpoint);
                }
                else if (GapcloserMode == 1)
                {
                    var points = OktwCommon.CirclePoints(10, DashSpell.Range, Player.Position);
                    var bestpoint = Player.Position.Extend(gapcloser.Sender.Position, -DashSpell.Range);
                    int enemies = bestpoint.CountEnemiesInRange(DashSpell.Range);
                    foreach (var point in points)
                    {
                        int count = point.CountEnemiesInRange(DashSpell.Range);
                        if (count < enemies)
                        {
                            enemies = count;
                            bestpoint = point;
                        }
                        else if (count == enemies && Game.CursorPos.Distance(point) < Game.CursorPos.Distance(bestpoint))
                        {
                            enemies = count;
                            bestpoint = point;
                        }
                    }
                    if (IsGoodPosition(bestpoint))
                        DashSpell.Cast(bestpoint);
                }
            }
        }

        public Vector3 CastDash(bool asap = false)
        {
            int DashMode = Config.Item("DashMode", true).GetValue<StringList>().SelectedIndex;

            Vector3 bestpoint = Vector3.Zero;
            if (DashMode == 0)
            {
                bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range);
            }
            else if (DashMode == 1)
            {
                var orbT = Orbwalker.GetTarget();
                if(orbT != null && orbT is Obj_AI_Hero)
                {
                    Vector2 start = Player.Position.To2D();
                    Vector2 end = orbT.Position.To2D();
                    var dir = (end - start).Normalized();
                    var pDir = dir.Perpendicular();

                    var rightEndPos = end + pDir * Player.Distance(orbT);
                    var leftEndPos = end - pDir * Player.Distance(orbT);

                    var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, Player.Position.Z);
                    var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, Player.Position.Z);

                    if (Game.CursorPos.Distance(rEndPos) < Game.CursorPos.Distance(lEndPos))
                    {
                        bestpoint = Player.Position.Extend(rEndPos, DashSpell.Range);
                    }
                    else
                    {
                        bestpoint = Player.Position.Extend(lEndPos, DashSpell.Range);
                    }
                }
            }
            else if (DashMode == 2)
            {
                var points = OktwCommon.CirclePoints(15, DashSpell.Range, Player.Position);
                bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range);
                int enemies = bestpoint.CountEnemiesInRange(350);
                foreach (var point in points)
                {
                    int count = point.CountEnemiesInRange(350);
                    if (!InAARange(point))
                        continue;
                    if (point.UnderAllyTurret())
                    {
                        bestpoint = point;
                        enemies = count - 1;
                    }
                    else if (count < enemies)
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                    else if (count == enemies && Game.CursorPos.Distance(point) < Game.CursorPos.Distance(bestpoint))
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                }
            }

            if (bestpoint.IsZero)
                return Vector3.Zero;

            var isGoodPos = IsGoodPosition(bestpoint);

            if (asap && isGoodPos)
            {
                return bestpoint;
            }
            else if (isGoodPos && InAARange(bestpoint))
            {
                return bestpoint;
            }
            return Vector3.Zero;
        }

        public bool InAARange(Vector3 point)
        {
            if (!Config.Item("AAcheck", true).GetValue<bool>())
                return true;
            else if (Orbwalker.GetTarget() != null && Orbwalker.GetTarget().Type == GameObjectType.obj_AI_Hero)
            {
                return point.Distance(Orbwalker.GetTarget().Position) < Player.AttackRange;
            }
            else
            {
                return point.CountEnemiesInRange(Player.AttackRange) > 0;
            }
        }

        public bool IsGoodPosition(Vector3 dashPos)
        {
            if (Config.Item("WallCheck", true).GetValue<bool>())
            {
                float segment = DashSpell.Range / 5;
                for (int i = 1; i <= 5; i++)
                {
                    if (Player.Position.Extend(dashPos, i * segment).IsWall())
                        return false;
                }
            }

            if (Config.Item("TurretCheck", true).GetValue<bool>())
            {
                if(dashPos.UnderTurret(true))
                    return false;
            }

            var enemyCheck = Config.Item("EnemyCheck", true).GetValue<Slider>().Value;
            var enemyCountDashPos = dashPos.CountEnemiesInRange(600);
            
            if (enemyCheck > enemyCountDashPos)
                return true;

            var enemyCountPlayer = Player.CountEnemiesInRange(400);

            if(enemyCountDashPos <= enemyCountPlayer)
                return true;

            return false;
        }
    }
}
