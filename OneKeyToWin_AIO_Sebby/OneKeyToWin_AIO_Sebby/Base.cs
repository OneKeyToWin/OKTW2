using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using SharpDX;
using SebbyLib;
using SharpDX.Direct3D9;

namespace OneKeyToWin_AIO_Sebby
{
    class Base : Program
    {
        public static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        private static Font TextBold;
        private static float spellFarmTimer = 0;
        public static bool FarmSpells
        {
            get
            {
                return MainMenu.Item("spellFarm").GetValue<bool>() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                    && Player.ManaPercent > MainMenu.Item("Mana", true).GetValue<Slider>().Value;
            }
        }

        public static int FarmMinions {get { return MainMenu.Item("LCminions", true).GetValue<Slider>().Value;}}

        static Base()
        {
            Console.WriteLine("BASE");
            TextBold = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Impact", Height = 30, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });
            MainMenu.SubMenu("Extra settings OKTW©").AddItem(new MenuItem("supportMode", "Support Mode", true).SetValue(false));
            MainMenu.SubMenu("Extra settings OKTW©").AddItem(new MenuItem("comboDisableMode", "Disable auto-attack in combo mode", true).SetValue(false));
            MainMenu.SubMenu("Extra settings OKTW©").AddItem(new MenuItem("manaDisable", "Disable mana manager in combo", true).SetValue(false));
            MainMenu.SubMenu("Extra settings OKTW©").AddItem(new MenuItem("collAA", "Disable auto-attack if Yasuo wall collision", true).SetValue(true));           
            MainMenu.SubMenu("Extra settings OKTW©").AddItem(new MenuItem("harassMixed", "Spell-harass only in mixed mode").SetValue(false));
            MainMenu.Item("supportMode", true).SetValue(false);

            foreach (var enemy in HeroManager.Enemies)
                HeroMenu.SubMenu("Harass").AddItem(new MenuItem("Harass" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("spellFarm", "OKTW spells farm").SetValue(true)).Show();
            HeroMenu.SubMenu("Farm").SubMenu("SPELLS FARM TOGGLE").AddItem(new MenuItem("spellFarmMode", "SPELLS FARM TOGGLE MODE").SetValue(new StringList(new[] { "Scroll down", "Scroll press", "Key toggle", "Disable" }, 1)));
            HeroMenu.SubMenu("Farm").SubMenu("SPELLS FARM TOGGLE").AddItem(new MenuItem("spellFarmKeyToggle", "Key toggle").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle))).ValueChanged += Base_ValueChanged; ;
            HeroMenu.SubMenu("Farm").SubMenu("SPELLS FARM TOGGLE").AddItem(new MenuItem("showNot", "Show notification").SetValue(true));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("LCminions", "Lane clear minimum minions", true).SetValue(new Slider(2, 10, 0)));
            HeroMenu.SubMenu("Farm").AddItem(new MenuItem("Mana", "LaneClear Mana", true).SetValue(new Slider(50, 100, 0)));

            MainMenu.Item("spellFarm").Permashow(true);
            MainMenu.Item("harassMixed").Permashow(true);

            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += OnDraw;
        }

        private static void Base_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (MainMenu.Item("spellFarmMode").GetValue<StringList>().SelectedIndex == 2)
            {
                MainMenu.Item("spellFarm").SetValue(!MainMenu.Item("spellFarmKeyToggle").GetValue<KeyBind>().Active);
                spellFarmTimer = Game.Time;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            
            if (AioModeSet != AioMode.UtilityOnly && MainMenu.Item("debugOrb").GetValue<bool>())
                DrawFontTextScreen(TextBold, "mode: " + Orbwalker.ActiveMode.ToString().ToUpper() , Drawing.Width * 0.1f, Drawing.Height * 0.1f, Color.White);

            if (Game.Time - DrawSpellTime < 0.5 && MainMenu.Item("debugPred").GetValue<bool>() && (MainMenu.Item("Qpred", true).GetValue<StringList>().SelectedIndex == 0 || MainMenu.Item("Wpred", true).GetValue<StringList>().SelectedIndex == 0
                || MainMenu.Item("Epred", true).GetValue<StringList>().SelectedIndex == 0 || MainMenu.Item("Rpred", true).GetValue<StringList>().SelectedIndex == 0))
            {
                if (DrawSpell.Type == SkillshotType.SkillshotLine)
                    OktwCommon.DrawLineRectangle(DrawSpellPos.CastPosition, Player.Position, (int)DrawSpell.Width, 1, System.Drawing.Color.DimGray);
                if (DrawSpell.Type == SkillshotType.SkillshotCircle)
                    Render.Circle.DrawCircle(DrawSpellPos.CastPosition, DrawSpell.Width, System.Drawing.Color.DimGray, 1);

                drawText("Aiming " + DrawSpellPos.Hitchance, Player.Position.Extend(DrawSpellPos.CastPosition, 400), System.Drawing.Color.Gray);
            }

            if (AioModeSet != AioMode.UtilityOnly && spellFarmTimer + 1 > Game.Time && MainMenu.Item("showNot").GetValue<bool>() && MainMenu.Item("spellFarm") != null)
            {
                if (MainMenu.Item("spellFarm").GetValue<bool>())
                    DrawFontTextScreen(TextBold, "SPELLS FARM ON", Drawing.Width * 0.5f, Drawing.Height * 0.4f, Color.GreenYellow);
                else
                    DrawFontTextScreen(TextBold, "SPELLS FARM OFF", Drawing.Width * 0.5f, Drawing.Height * 0.4f, Color.OrangeRed);
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {

            if (AioModeSet == AioMode.UtilityOnly)
                return;

            if (args.Msg == 0x20a && MainMenu.Item("spellFarmMode").GetValue<StringList>().SelectedIndex == 0 )
            {
                MainMenu.Item("spellFarm").SetValue(!MainMenu.Item("spellFarm").GetValue<bool>());
                spellFarmTimer = Game.Time;
            }
            if (args.Msg == 520 && MainMenu.Item("spellFarmMode").GetValue<StringList>().SelectedIndex == 1)
            {
                MainMenu.Item("spellFarm").SetValue(!MainMenu.Item("spellFarm").GetValue<bool>());
                spellFarmTimer = Game.Time;
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var t = args.Target as Obj_AI_Hero;
            if (t != null && Combo && MainMenu.Item("comboDisableMode", true).GetValue<bool>())
            {
                
                if (4 * Player.GetAutoAttackDamage(t) < t.Health - OktwCommon.GetIncomingDamage(t) && !t.HasBuff("luxilluminatingfraulein") && !Player.HasBuff("sheen") && !Player.HasBuff("Mastery6261"))
                    args.Process = false;
            }

            if (!Player.IsMelee && OktwCommon.CollisionYasuo(Player.ServerPosition, args.Target.Position) && MainMenu.Item("collAA", true).GetValue<bool>())
            {
                args.Process = false;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && MainMenu.Item("supportMode", true).GetValue<bool>())
            {
                if (args.Target.Type == GameObjectType.obj_AI_Minion) args.Process = false;
            }
        }
        private static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }
    }
}
