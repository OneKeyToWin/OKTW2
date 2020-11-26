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

    class AutoLvlUp : Program
    {
        int lvl1, lvl2, lvl3, lvl4;
        public void LoadOKTW()
        {
            Config.SubMenu("AutoLvlUp OKTW©").AddItem(new MenuItem("AutoLvl", "ENABLE").SetValue(true));
            Config.SubMenu("AutoLvlUp OKTW©").AddItem(new MenuItem("1", "1", true).SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 3)));
            Config.SubMenu("AutoLvlUp OKTW©").AddItem(new MenuItem("2", "2", true).SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
            Config.SubMenu("AutoLvlUp OKTW©").AddItem(new MenuItem("3", "3", true).SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
            Config.SubMenu("AutoLvlUp OKTW©").AddItem(new MenuItem("4", "4", true).SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
            Config.SubMenu("AutoLvlUp OKTW©").AddItem(new MenuItem("LvlStart", "Auto LVL start", true).SetValue(new Slider(2, 6, 1)));
            
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnLevelUp +=Obj_AI_Base_OnLevelUp;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!Program.LagFree(0) || !Config.Item("AutoLvl").GetValue<bool>())
                return;
            lvl1 = Config.Item("1", true).GetValue<StringList>().SelectedIndex;
            lvl2 = Config.Item("2", true).GetValue<StringList>().SelectedIndex;
            lvl3 = Config.Item("3", true).GetValue<StringList>().SelectedIndex;
            lvl4 = Config.Item("4", true).GetValue<StringList>().SelectedIndex;
        }

        private void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !Config.Item("AutoLvl").GetValue<bool>() || ObjectManager.Player.Level < Config.Item("LvlStart", true).GetValue<Slider>().Value)
                return;
            if (lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4)
                return;
            int delay = 700;
            Utility.DelayAction.Add(delay, () => Up(lvl1));
            Utility.DelayAction.Add(delay + 50, () => Up(lvl2));
            Utility.DelayAction.Add(delay + 100, () => Up(lvl3));
            Utility.DelayAction.Add(delay + 150, () => Up(lvl4));
        }


        private void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.Level == 1 && Config.Item("AutoLvl").GetValue<bool>() )
            {
                if ((lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4) && (int)Game.Time % 2 == 0)
                {
                    drawText("AutoLvlUp: PLEASE SET ABILITY SEQENCE", ObjectManager.Player.Position, System.Drawing.Color.OrangeRed, -200);
                }
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        private void Up(int indx)
        {
            if (ObjectManager.Player.Level < 4)
            {
                if (indx == 0 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (indx == 0 )
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2 )
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (indx == 3)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }
    }
}
