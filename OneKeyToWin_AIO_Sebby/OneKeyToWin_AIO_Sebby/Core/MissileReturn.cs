using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using OneKeyToWin_AIO_Sebby;
using OneKeyToWin_AIO_Sebby.SebbyLib;

class MissileReturn
{
    public Obj_AI_Hero Target;
    private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
    private static Menu Config = Program.MainMenu;
    private static Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
    private string MissileName, MissileReturnName;
    private Spell QWER;
    public MissileClient Missile;
    private Vector3 MissileEndPos;

    public MissileReturn(string missile, string missileReturnName, Spell qwer)
    {
        Config.SubMenu(Player.ChampionName).SubMenu(qwer.Slot + " Config").SubMenu("Auto AIM OKTW system").AddItem(new MenuItem("aim", "Auto aim returned missile", true).SetValue(true));
        Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("drawHelper", "Show " + qwer.Slot + " helper", true).SetValue(true));

        MissileName = missile;
        MissileReturnName = missileReturnName;
        QWER = qwer;

        GameObject.OnCreate += SpellMissile_OnCreateOld;
        GameObject.OnDelete += Obj_SpellMissile_OnDelete;
        Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        Game.OnUpdate += Game_OnGameUpdate;
        Drawing.OnDraw += Drawing_OnDraw;
    }

    private void Drawing_OnDraw(EventArgs args)
    {
        if (Missile != null && Missile.IsValid && Config.Item("drawHelper", true).GetValue<bool>())
            OktwCommon.DrawLineRectangle(Missile.Position, Player.Position, (int)QWER.Width, 1, System.Drawing.Color.White);
    }

    private void Game_OnGameUpdate(EventArgs args)
    {
        if (Config.Item("aim", true).GetValue<bool>())
        {
            var posPred = CalculateReturnPos();
            if (posPred != Vector3.Zero)
                Orbwalker.SetOrbwalkingPoint(posPred);
            else
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
        }
        else
            Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
    }

    private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
    {
        if (sender.IsMe && args.Slot == QWER.Slot)
        {
            MissileEndPos = args.End;
        }
    }

    private void SpellMissile_OnCreateOld(GameObject sender, EventArgs args)
    {
        if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid<MissileClient>())
            return;

        MissileClient missile = (MissileClient)sender;

        if (missile.SData.Name != null)
        {
            if (missile.SData.Name.ToLower() == MissileName.ToLower() || missile.SData.Name.ToLower() == MissileReturnName.ToLower())
            {
                Missile = missile;
            }
        }
    }

    private void Obj_SpellMissile_OnDelete(GameObject sender, EventArgs args)
    {
        if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid<MissileClient>())
            return;

        MissileClient missile = (MissileClient)sender;

        if (missile.SData.Name != null)
        {
            if (missile.SData.Name.ToLower() == MissileReturnName.ToLower())
            {
                Missile = null;
            }
        }
    }

    public Vector3 CalculateReturnPos()
    {
        if (Missile != null && Missile.IsValid && Target.IsValidTarget())
        {
            var finishPosition = Missile.Position;
            if (Missile.SData.Name.ToLower() == MissileName.ToLower())
            {
                finishPosition = MissileEndPos;
            }

            var misToPlayer = Player.Distance(finishPosition);
            var tarToPlayer = Player.Distance(Target);

            if (misToPlayer > tarToPlayer)
            {
                var misToTarget = Target.Distance(finishPosition);

                if (misToTarget < QWER.Range && misToTarget > 50)
                {
                    var cursorToTarget = Target.Distance(Player.Position.Extend(Game.CursorPos, 100));
                    var ext = finishPosition.Extend(Target.ServerPosition, cursorToTarget + misToTarget);

                    if (ext.Distance(Player.Position) < 800 && ext.CountEnemiesInRange(400) < 2)
                    {
                        if (Config.Item("drawHelper", true).GetValue<bool>())
                            Utility.DrawCircle(ext, 100, System.Drawing.Color.White, 1, 1);
                        return ext;
                    }
                }
            }
        }
        return Vector3.Zero;
    }
}
