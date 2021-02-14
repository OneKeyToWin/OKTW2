using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class OKTWlab
    {
        private GameObject obj;
        private float time = 0;
        private Vector3 from;
        private float castTime;
        private Vector3 endPosG = Game.CursorPos;

        public void LoadOKTW()
        {
            Obj_AI_Base.OnDelete += Obj_AI_Base_OnDelete;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnBuffAdd += OnBuffAdd;
        }

        private void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            if(sender.IsMe)
            Console.WriteLine("sender " + args.Buff.Name );
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            return;
            Program.debug("CAST");
        }

        private void Game_OnProcessPacket(GamePacketEventArgs args)
        {
            
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if(ObjectManager.Player.IsWindingUp || !ObjectManager.Player.CanMove || ObjectManager.Player.IsRooted)
                Console.WriteLine(ObjectManager.Player.IsWindingUp + " " + ObjectManager.Player.CanMove  + " " + ObjectManager.Player.IsRooted);


            foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(400)))
            {
                if(!SebbyLib.OktwCommon.CanMove(enemy))
                {
                    Console.WriteLine("cant move");
                }
            }
            return;
            //Program.debug("count minions: " + ObjectManager.Player.FlatPhysicalDamageMod);
            //return;
            foreach (var buff in ObjectManager.Player.Buffs)
            {
                if ("masterylordsdecreecooldown" == buff.Name)
                    Program.debug(buff.Name);
            }
        }

        private void GetConeTarget(Vector2 end)
        {
            var angle = 60f * (float)Math.PI / 180;
            var range = 3500;
            var end2 = end - ObjectManager.Player.Position.To2D();
            var edge1 = end2.Rotated(-angle / 2);
            var edge2 = edge1.Rotated(angle);


            var point = Game.CursorPos.To2D() - ObjectManager.Player.Position.To2D();
            if (point.Distance(new Vector2(), true) < range * range)
            {
                
                if (edge1.CrossProduct(point) > 0)
                {
                    if (point.CrossProduct(edge2) > 0)
                    {
                        Program.debug("dupa " + edge1);
                        Render.Circle.DrawCircle(Game.CursorPos, 50, System.Drawing.Color.Orange, 1);
                    }
                }
            }
        }


        private void Drawing_OnDraw(EventArgs args)
        {


            var obj = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.Distance(Game.CursorPos) < 100);
            if (obj != null)
            {
                var wts = Drawing.WorldToScreen(Game.CursorPos);
                Drawing.DrawText(wts[0], wts[1], System.Drawing.Color.Aqua, obj.Name);
            }


            return;
            GetConeTarget(endPosG.To2D());
            Render.Circle.DrawCircle(endPosG, 50, System.Drawing.Color.Red, 1);
            //OktwCommon.DrawLineRectangle(Game.CursorPos, ObjectManager.Player.Position, 75, 1, System.Drawing.Color.DimGray);
            //drawText("Range " + ObjectManager.Player.Distance(Game.CursorPos), ObjectManager.Player.Position.Extend(Game.CursorPos, 400), System.Drawing.Color.Gray);


            int radius = 100;
            var start2 = ObjectManager.Player.ServerPosition;
            var end2 = Game.CursorPos;


            Vector2 start = start2.To2D();
            Vector2 end = end2.To2D();
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;

            
            var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
            var lEndPos  =new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);


            var step = start2.Distance(rEndPos) / 10;
            for (var i = 0; i < 10; i++)
            {
                var pr = start2.Extend(rEndPos, step * i);
                var pl = start2.Extend(lEndPos, step * i);
                Render.Circle.DrawCircle(pr, 50, System.Drawing.Color.Orange, 1);
                Render.Circle.DrawCircle(pl, 50, System.Drawing.Color.Orange, 1);
            }

            


            if (obj != null &&  obj.IsValid)
            {
                //Utility.DrawCircle(obj.Position, 100, System.Drawing.Color.Orange, 1, 1);
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            return;
            if (sender.IsMe && !args.SData.IsAutoAttack())
            {
                castTime = Game.Time;
                //Program.debug("speed: " +args.SData.MissileSpeed);
                Program.debug("name: " + args.SData.Name);
                //Program.debug("" + args.SData.DelayTotalTimePercent);
                //time = Game.Time;
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            return;
            var minion = sender as Obj_AI_Minion;

            if (minion != null )
                Program.debug(" render: " + minion.IsHPBarRendered+" " + minion.Name +  " type "  + minion.Team + " mana " + minion.MaxMana + " hp "+ minion.Health + " attack "+ minion.IsMinion + minion.AttackSpeedMod  );
            return;
            if (sender.IsValid && sender.IsAlly )
            {
                //obj = sender;
                
                if (!sender.IsValid<MissileClient>())
                    return;

                MissileClient missile = (MissileClient)sender;
                if (missile.SData.LineWidth == 0)
                    return;
                Program.debug(missile.SData.Name + " " + missile.SData.LineWidth + " " + missile.SData.MissileSpeed + " " + (Game.Time - castTime));
                if (missile.IsValid && missile.IsAlly && missile.SData.Name != null && (missile.SData.Name == "SivirQMissile" || missile.SData.Name == "SivirQMissileReturn"))
                {
                    
                }
                //Program.debug(""+);
                //Program.debug("cast time" +(time - Game.Time));
            }
        }

        private void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            return;
            if (sender.IsValid)
            {
                
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }
    }
}
