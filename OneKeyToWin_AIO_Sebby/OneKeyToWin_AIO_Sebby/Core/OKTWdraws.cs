using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using SebbyLib;
using System.Collections.Generic;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class OktwNotification
    {
        public Render.Sprite Hero;
        public Render.Sprite SpellIco;
        public bool Lost;
        public int TimeRec = 0;
        public OktwNotification(Render.Sprite hero, Render.Sprite spellIco, bool lost)
        {
            Hero = hero;
            SpellIco = spellIco;
            Lost = lost;
        }
    }

    class OKTWdraws : Program
    {
        public static List<OktwNotification> NotificationsList = new List<OktwNotification>();

        public static Font Tahoma13, Tahoma13B, TextBold, HudLevel, HudCd, RecFont, HudLevel2;
        public static Vector2 centerScreen = new Vector2(Drawing.Width / 2 - 20, Drawing.Height / 2 - 90);
        private float IntroTimer = Game.Time;
        private Render.Sprite Intro;
        private static Render.Sprite Pink, Ward, PinkMM, WardMM, Not;
        private static Render.Sprite Isready, Lost;
        private bool MenuOpen = false;
        private static int NotTimer = Utils.TickCount + 1500;

        public static string[] IgnoreR = { "Corki", "Jayce", "Kassadin", "KogMaw", "Leblanc", "Teemo", "Swain", "Shyvana", "Nidalee", "Anivia", "Quinn", "Gnar" };


        public void LoadOKTW()
        {
            Not = ImageLoader.GetSprite("not");
            Isready = ImageLoader.GetSprite("isready");
            Lost = ImageLoader.GetSprite("lost");
            Pink = ImageLoader.CreateSummonerSprite("pink");
            Pink.Scale = new Vector2(0.15f, 0.15f);
            Ward = ImageLoader.CreateSummonerSprite("ward");
            Ward.Scale = new Vector2(0.15f, 0.15f);
            PinkMM = ImageLoader.CreateSummonerSprite("WT_Pink");
            WardMM = ImageLoader.CreateSummonerSprite("WT_Green");

            Config.SubMenu("About OKTW©").AddItem(new MenuItem("logo", "Intro logo OKTW").SetValue(true));

            if (Config.Item("logo").GetValue<bool>())
            {
                Intro = new Render.Sprite(LoadImg("intro"), new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
                Intro.Add(0);
                Intro.OnDraw();
            }

            Utility.DelayAction.Add(7000, () => Intro.Remove());

            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Hud").AddItem(new MenuItem("championInfo", "Show enemy avatars").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Hud").AddItem(new MenuItem("championInfoHD", "Full HD screen size").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Hud").AddItem(new MenuItem("posX", "Avatars posX").SetValue(new Slider(839, 1000, 0)));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Hud").AddItem(new MenuItem("posY", "Avatars posY").SetValue(new Slider(591, 1000, 0)));

            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Hud").AddItem(new MenuItem("gankalert", "Gank alert").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Hud").AddItem(new MenuItem("posXj", "Alert posX").SetValue(new Slider(639, 1000, 0)));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Hud").AddItem(new MenuItem("posYj", "Alert posY").SetValue(new Slider(591, 1000, 0)));

            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Notification").AddItem(new MenuItem("Notification", "Enable").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Notification").AddItem(new MenuItem("NotificationR", "Ultimate").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Notification").AddItem(new MenuItem("NotificationS", "Summoners").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Notification").AddItem(new MenuItem("posXn", "Notifications posX ").SetValue(new Slider(400, 1000, 0)));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Notification").AddItem(new MenuItem("posYn", "Notifications posY").SetValue(new Slider(50, 1000, 0)));

            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Awareness radar").AddItem(new MenuItem("ScreenRadar", "Enable").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Awareness radar").AddItem(new MenuItem("ScreenRadarEnemy", "Only enemy").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Awareness radar").AddItem(new MenuItem("ScreenRadarJungler", "Only jungler").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").AddItem(new MenuItem("HpBar", "Damage indicators").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Spell tracker").AddItem(new MenuItem("SpellTrackerEnemy", "Enemy").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Spell tracker").AddItem(new MenuItem("SpellTrackerAlly", "Ally").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Spell tracker").AddItem(new MenuItem("SpellTrackerMe", "Me").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").SubMenu("Spell tracker").AddItem(new MenuItem("SpellTrackerLvl", "Show spell lvl (can drop fps)").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").AddItem(new MenuItem("ShowClicks", "Show enemy clicks").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Screen").AddItem(new MenuItem("showWards", "Show hidden objects, wards").SetValue(true));

            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Minimap").AddItem(new MenuItem("minimap", "Mini-map hack").SetValue(true));

            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("disableDraws", "DISABLE UTILITY DRAWS").SetValue(false));

            Tahoma13B = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 14, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });

            Tahoma13 = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 14, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });

            TextBold = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Impact", Height = 30, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });

            HudLevel = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 17, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });

            HudLevel2 = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 12, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });

            RecFont = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 12, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });

            HudCd = new Font(Drawing.Direct3DDevice, new FontDescription
            { FaceName = "Tahoma", Height = 11, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });

            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == 256)
                MenuOpen = true;
            if (args.Msg == 257)
                MenuOpen = false;
            
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (NotTimer + 1000 < Utils.TickCount)
            {
                foreach (var hero in OKTWtracker.ChampionInfoList.Where(x => x.Hero.IsEnemy))
                {
                    var rSlot = hero.Hero.Spellbook.Spells[3];
                    var sum1 = hero.Hero.Spellbook.Spells[4];
                    var sum2 = hero.Hero.Spellbook.Spells[5];
                    if (Config.Item("NotificationR").GetValue<bool>() && rSlot != null && !IgnoreR.Any(x => x == hero.Hero.ChampionName))
                    {
                        var time = rSlot.CooldownExpires - Game.Time;
                        if (time < 1 && time >= 0)
                            NotificationsList.Add(new OktwNotification(hero.SquareSprite, hero.RSpriteS, false));
                        else if (rSlot.Cooldown - time < 1 && rSlot.Cooldown - time >= 0)
                            NotificationsList.Add(new OktwNotification(hero.SquareSprite, hero.RSpriteS, true));
                    }
                    if (Config.Item("NotificationS").GetValue<bool>())
                    {
                        if (sum1 != null)
                        {
                            var time = sum1.CooldownExpires - Game.Time;
                            if (time < 1 && time >= 0)
                                NotificationsList.Add(new OktwNotification(hero.SquareSprite, hero.Summoner1SpriteS, false));
                            else if (sum1.Cooldown - time < 1 && sum1.Cooldown - time >= 0)
                                NotificationsList.Add(new OktwNotification(hero.SquareSprite, hero.Summoner1SpriteS, true));
                        }
                        if (sum2 != null)
                        {
                            var time = sum2.CooldownExpires - Game.Time;
                            if (time < 1 && time >= 0)
                                NotificationsList.Add(new OktwNotification(hero.SquareSprite, hero.Summoner2SpriteS, false));
                            else if (sum2.Cooldown - time < 1 && sum2.Cooldown - time >= 0)
                                NotificationsList.Add(new OktwNotification(hero.SquareSprite, hero.Summoner2SpriteS, true));
                        }
                    }
                }
                NotTimer = Utils.TickCount;
            }
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (Config.Item("disableDraws").GetValue<bool>())
                return;

            bool blink = true;

            if ((int)(Game.Time * 10) % 2 == 0)
                blink = false;

            var minimap = Config.Item("minimap").GetValue<bool>();
            var HpBar = Config.Item("HpBar").GetValue<bool>();
            var championInfo = Config.Item("championInfo").GetValue<bool>();
            var ScreenRadar = Config.Item("ScreenRadar").GetValue<bool>();
            var ScreenRadarEnemy = Config.Item("ScreenRadarEnemy").GetValue<bool>();
            var ScreenRadarJungler = Config.Item("ScreenRadarJungler").GetValue<bool>();
            var SpellTrackerEnemy = Config.Item("SpellTrackerEnemy").GetValue<bool>();
            var SpellTrackerAlly = Config.Item("SpellTrackerAlly").GetValue<bool>();
            var SpellTrackerMe = Config.Item("SpellTrackerMe").GetValue<bool>();
            var SpellTrackerLvl = Config.Item("SpellTrackerLvl").GetValue<bool>();
            var ShowClicks = Config.Item("ShowClicks").GetValue<bool>();
            var championInfoHD = Config.Item("championInfoHD").GetValue<bool>();
            float posY = (Config.Item("posY").GetValue<Slider>().Value * 0.001f) * Drawing.Height;
            float posX = (Config.Item("posX").GetValue<Slider>().Value * 0.001f) * Drawing.Width;

            float posYn = (Config.Item("posYn").GetValue<Slider>().Value * 0.001f) * Drawing.Height;
            float posXn = (Config.Item("posXn").GetValue<Slider>().Value * 0.001f) * Drawing.Width;

            int Width = 105;
            int Height = 11;
            int XOffset = 10;
            int YOffset = 20;

            if (Drawing.Height >= 1440)
            {
                Width = 126;
                Height = 13;
                XOffset = 20;
                YOffset = 26;
            }


            var FillColor = System.Drawing.Color.GreenYellow;
            var Color = System.Drawing.Color.Azure;

            var hudSpace = 0;
            var area = 5000;
            var notPos = new Vector2(posXn, posYn);

            var centerScreenWorld = Drawing.ScreenToWorld(centerScreen);
            if (Config.Item("gankalert").GetValue<bool>())
            {
                var jungler = OKTWtracker.ChampionInfoList.FirstOrDefault(x => x.Hero.IsEnemy && x.IsJungler);
                var stringg = "Jungler not detected";
                float posXj = (Config.Item("posXj").GetValue<Slider>().Value * 0.001f) * Drawing.Width;
                float posYj = (Config.Item("posYj").GetValue<Slider>().Value * 0.001f) * Drawing.Height;
                

                var jungleAlertPos = new Vector2(posXj, posYj);
                if (jungler != null)
                {
                    Drawing.DrawLine(jungleAlertPos, jungleAlertPos + new Vector2(120, 0), 20, System.Drawing.Color.Black);
                    Drawing.DrawLine(jungleAlertPos + new Vector2(1, 1), jungleAlertPos + new Vector2(119, 1), 18, System.Drawing.Color.DarkGreen);
                    float percent = 0;
                    var distance = jungler.LastVisablePos.Distance(Player.Position);
                    if (Game.Time - jungler.FinishRecallTime < 4)
                    {
                        stringg = "Jungler in base";
                        percent = 0;
                    }
                    else if (jungler.Hero.IsDead)
                    {
                        stringg = "Jungler dead";
                        percent = 0;
                    }
                    else if (distance < 3500)
                    {
                        stringg = "Jungler NEAR you";
                        percent = 1;
                    }
                    else if (jungler.Hero.IsVisible)
                    {
                        stringg = "Jungler visable";
                        percent = 0;
                    }
                    else
                    {
                        var timer = jungler.LastVisablePos.Distance(Player.Position) / 330;
                        var time2 = timer - (Game.Time - jungler.LastVisableTime);
                        stringg = "Jungler in jungle " + (int)time2;
                        time2 = time2 - 10;
                        if (time2 > 0)
                            percent = 0;
                        else
                            percent = (-time2) * 0.05f;
                        //Console.WriteLine(timer + " " + time2);
                        percent = Math.Min(percent, 1);
                    }

                    if (percent != 0)
                        Drawing.DrawLine(jungleAlertPos + new Vector2(1, 1), jungleAlertPos + new Vector2(1 + 118 * percent, 1), 18, System.Drawing.Color.OrangeRed);
                }
                DrawFontTextScreen(RecFont, stringg, jungleAlertPos.X + 3, jungleAlertPos.Y + 3, SharpDX.Color.White);
            }

            if (Config.Item("Notification").GetValue<bool>())
            {
                if (MenuGlobals.DrawMenu)
                {
                    Not.Position = notPos;
                    Not.Color = new ColorBGRA(0, 0.5f, 1f, 0.6f * 1);
                    Not.OnEndScene();
                }
                var noti = NotificationsList.FirstOrDefault();
                if (noti != null)
                {
                    if (noti.TimeRec == 0)
                    {
                        noti.TimeRec = Utils.TickCount;
                    }
                    else if (Utils.TickCount - noti.TimeRec > 3000)
                    {
                        NotificationsList.Remove(noti);
                    }
                    else
                    {
                        float time = Utils.TickCount - noti.TimeRec;
                        float calcOpacity = 1;

                        if (time < 500)
                            calcOpacity = time / 500;
                        else if (time > 2500)
                            calcOpacity = (3000 - time) / 500;

                        var opacity = new ColorBGRA(256, 256, 256, 0.9f * calcOpacity);
                        Not.Position = notPos;

                        if (noti.Lost)
                            Not.Color = new ColorBGRA(0, 0.5f, 1f, 0.6f * calcOpacity);

                        else
                            Not.Color = new ColorBGRA(0.5f, 0, 0, 0.6f * calcOpacity);

                        Not.OnEndScene();

                        if (noti.Lost)
                        {
                            Lost.Position = notPos + new Vector2(81, 8);
                            Lost.Color = opacity;
                            Lost.OnEndScene();
                        }
                        else
                        {
                            Isready.Position = notPos + new Vector2(81, 8);
                            Isready.Color = opacity;
                            Isready.OnEndScene();
                        }

                        noti.SpellIco.Position = notPos + new Vector2(8, 8);
                        noti.SpellIco.Color = opacity;
                        noti.SpellIco.OnEndScene();

                        noti.Hero.Position = notPos + new Vector2(152, 8);
                        noti.Hero.Color = opacity;
                        noti.Hero.Scale = new Vector2(0.547f, 0.547f);
                        noti.Hero.OnEndScene();

                    }
                }
            }
            if (Config.Item("showWards").GetValue<bool>())
            {
                #region showWards
                var circleSize = 30;
                foreach (var obj in OKTWward.HiddenObjList)
                {
                    if (obj.pos.IsOnScreen())
                    {
                        var pos = Drawing.WorldToScreen(obj.pos) + new Vector2(-50, -50); ;
                        if (obj.type == 1)
                        {
                            Drawing.DrawText(pos.X - 15, pos.Y - 13, System.Drawing.Color.YellowGreen, MakeNiceNumber(obj.endTime - Game.Time));
                            Ward.Position = pos + new Vector2(10, 10);

                            Ward.OnEndScene();
                            DrawFontTextMap(Tahoma13, ((int)(obj.endTime - Game.Time)).ToString(), obj.pos + new Vector3(-18, 18, 0), SharpDX.Color.White);
                        }
                        else if (obj.type == 2)
                        {
                            Pink.Position = pos + new Vector2(10, 10);
                            Pink.OnEndScene();

                        }
                        else if (obj.type == 3)
                        {
                            OktwCommon.DrawTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Orange);
                            DrawFontTextMap(Tahoma13, "! " + (int)(obj.endTime - Game.Time), obj.pos, SharpDX.Color.Orange);
                        }
                    }

                    {
                        var pos = Drawing.WorldToMinimap(obj.pos);
                        if (obj.type == 1)
                        {
                            WardMM.Position = pos;
                            WardMM.OnEndScene();
                        }
                        else if (obj.type == 2)
                        {
                            PinkMM.Position = pos;
                            PinkMM.OnEndScene();
                        }
                        else if (obj.type == 3)
                        {

                        }
                    }
                }
                #endregion
            }

            foreach (var hero in OKTWtracker.ChampionInfoList.Where(x => !x.Hero.IsDead))
            {
                var barPos = hero.Hero.HPBarPosition;
                var q = hero.Hero.Spellbook.GetSpell(SpellSlot.Q);
                var w = hero.Hero.Spellbook.GetSpell(SpellSlot.W);
                var e = hero.Hero.Spellbook.GetSpell(SpellSlot.E);
                var r = hero.Hero.Spellbook.GetSpell(SpellSlot.R);

                if (hero.Hero.IsVisible && ((SpellTrackerAlly && (hero.Hero.IsAlly && !hero.Hero.IsMe)) || (SpellTrackerEnemy && hero.Hero.IsEnemy) || (SpellTrackerMe && hero.Hero.IsMe)))
                {
                    bool is1440p = Drawing.Height >= 1440;
                    var scale = new Vector2(8, 7);

                    if (is1440p)
                    {
                        scale.X = 18;
                        scale.Y = 15;
                    }

                    var _w = Width + 7;
                    var _h = 34;

                    Drawing.DrawLine(barPos + scale + new Vector2(0, _h), barPos + scale + new Vector2(_w, _h), 9, System.Drawing.Color.DimGray);
                    Drawing.DrawLine(barPos + scale + new Vector2(2, _h + 1), barPos + scale + new Vector2(_w - 2, _h + 1), 7, System.Drawing.Color.Black);

                    var qCal = Math.Max(Math.Min((q.CooldownExpires - Game.Time) / q.Cooldown, 1), 0);
                    var wCal = Math.Max(Math.Min((w.CooldownExpires - Game.Time) / w.Cooldown, 1), 0);
                    var eCal = Math.Max(Math.Min((e.CooldownExpires - Game.Time) / e.Cooldown, 1), 0);
                    var rCal = Math.Max(Math.Min((r.CooldownExpires - Game.Time) / r.Cooldown, 1), 0);

                    float dotWidth = 2;
                    int dotSpacing = 4;
                    var barWidth = 27;

                    if (is1440p)
                    {
                        barWidth = 31;
                        dotSpacing = 5;
                        dotWidth = 3;
                    }

                    Vector2 start, end;

                    if (q.Level > 0)
                    {
                        start = barPos + scale + new Vector2(1 + (_w / 4 * 0), _h + 2);
                        end = barPos + scale + new Vector2(barWidth + (_w / 4 * 0) - ((barWidth - 1) * qCal), _h + 2);

                        Drawing.DrawLine(start, end, 
                            5, qCal > 0 ? System.Drawing.Color.Orange : System.Drawing.Color.YellowGreen);
                        
                        int g = barWidth / 5;

                        if (SpellTrackerLvl)
                            for (int i = 0 ; i < Math.Min(q.Level, 5) ; i++)
                                Drawing.DrawLine(
                                    start + new Vector2(3 + i * dotSpacing, 1),
                                    start + new Vector2(3 + dotWidth + i * dotSpacing, 1), 3, System.Drawing.Color.Black);
                    }
                    if (w.Level > 0)
                    {
                        start = barPos + scale + new Vector2(1 + (_w / 4 * 1), _h + 2);
                        end = barPos + scale + new Vector2(barWidth + (_w / 4 * 1) - ((barWidth - 1) * wCal), _h + 2);

                        Drawing.DrawLine(start, end,
                            5, wCal > 0 ? System.Drawing.Color.Orange : System.Drawing.Color.YellowGreen);

                        if (SpellTrackerLvl)
                            for (int i = 0; i < Math.Min(w.Level, 5); i++)
                                Drawing.DrawLine(
                                    start + new Vector2(3 + i * dotSpacing, 1),
                                    start + new Vector2(3 + dotWidth + i * dotSpacing, 1), 3, System.Drawing.Color.Black);
                    }
                    if (e.Level > 0)
                    {
                        start = barPos + scale + new Vector2(1 + (_w / 4 * 2), _h + 2);
                        end = barPos + scale + new Vector2(barWidth + (_w / 4 * 2) - ((barWidth-1) * eCal), _h + 2);

                        Drawing.DrawLine(start, end,
                            5, eCal > 0 ? System.Drawing.Color.Orange : System.Drawing.Color.YellowGreen);
                        
                        if (SpellTrackerLvl)
                            for (int i = 0 ; i < Math.Min(e.Level, 5) ; i++)
                                Drawing.DrawLine(
                                    start + new Vector2(3 + i * dotSpacing, 1),
                                    start + new Vector2(3 + dotWidth + i * dotSpacing, 1), 3, System.Drawing.Color.Black);
                    }
                    if (r.Level > 0)
                    {
                        start = barPos + scale + new Vector2(1 + (_w / 4 * 3), _h + 2);
                        end = barPos + scale + new Vector2(barWidth + (_w / 4 * 3) - ((barWidth - 1) * rCal), _h + 2);

                        Drawing.DrawLine(start, end,
                            5, rCal > 0 ? System.Drawing.Color.Orange : System.Drawing.Color.YellowGreen);

                        if (SpellTrackerLvl)
                            for (int i = 0 ; i < Math.Min(r.Level, 5) ; i++)
                                Drawing.DrawLine(
                                    start + new Vector2(3 + i * dotSpacing, 1),
                                    start + new Vector2(3 + dotWidth + i * dotSpacing, 1), 3, System.Drawing.Color.Black);
                    }
                }

                if (hero.Hero.IsMe)
                    continue;

                if (hero.Hero.IsEnemy)
                {
                    if (ShowClicks && hero.Hero.IsValidTarget() && hero.LastWayPoint.IsValid() && hero.Hero.Position.Distance(hero.LastWayPoint) > 100)
                    {
                        drawLine(hero.Hero.Position, hero.LastWayPoint, 1, System.Drawing.Color.Red);
                        DrawFontTextMap(Tahoma13, hero.Hero.ChampionName, hero.LastWayPoint, SharpDX.Color.WhiteSmoke);
                    }
                    #region Damage indicators
                    if (HpBar && hero.Hero.IsHPBarRendered && hero.Hero.Position.IsOnScreen())
                    {
                        
                        if (HpBar)
                        {
                            float QdmgDraw = 0, WdmgDraw = 0, EdmgDraw = 0, RdmgDraw = 0, damage = 0;
                            bool qRdy = Player.Spellbook.GetSpell(SpellSlot.Q).IsReady();
                            bool wRdy = Player.Spellbook.GetSpell(SpellSlot.W).IsReady();
                            bool eRdy = Player.Spellbook.GetSpell(SpellSlot.E).IsReady();
                            bool rRdy = Player.Spellbook.GetSpell(SpellSlot.R).IsReady();

                            float qDmg = 0;
                            float wDmg = 0;
                            float eDmg = 0;
                            float rDmg = 0;

                            if (qRdy)
                                qDmg = (float)Player.GetSpellDamage(hero.Hero, SpellSlot.Q);
                            if (wRdy)
                                wDmg = (float)Player.GetSpellDamage(hero.Hero, SpellSlot.W);
                            if (eRdy)
                                eDmg = (float)Player.GetSpellDamage(hero.Hero, SpellSlot.E);
                            if (rRdy)
                                rDmg = (float)Player.GetSpellDamage(hero.Hero, SpellSlot.R);

                            damage = qDmg + wDmg + eDmg + rDmg;

                            if (qRdy)
                                QdmgDraw = (qDmg / damage);

                            if (wRdy && Player.ChampionName != "Kalista")
                                WdmgDraw = (wDmg / damage);

                            if (eRdy)
                                EdmgDraw = (eDmg / damage);

                            if (rRdy)
                                RdmgDraw = (rDmg / damage);

                            var percentHealthAfterDamage = Math.Max(0, hero.Hero.Health - damage) / hero.Hero.MaxHealth;

                            var yPos = barPos.Y + YOffset;
                            var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                            var xPosCurrentHp = barPos.X + XOffset + Width * hero.Hero.Health / hero.Hero.MaxHealth;

                            float differenceInHP = xPosCurrentHp - xPosDamage;
                            var pos1 = barPos.X + XOffset + (Width * percentHealthAfterDamage);

                            for (int i = 0; i < differenceInHP; i++)
                            {
                                if (qRdy && i < QdmgDraw * differenceInHP)
                                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Cyan);
                                else if (wRdy && i < (QdmgDraw + WdmgDraw) * differenceInHP)
                                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Orange);
                                else if (eRdy && i < (QdmgDraw + WdmgDraw + EdmgDraw) * differenceInHP)
                                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Yellow);
                                else if (rRdy && i < (QdmgDraw + WdmgDraw + EdmgDraw + RdmgDraw) * differenceInHP)
                                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.YellowGreen);
                            }
                        }
                    }
                    #endregion
                    if (minimap)
                    {
                        if (!hero.Hero.IsVisible)
                        {
                            var minimapSprite = hero.MinimapSprite;

                            var lastWP = hero.LastWayPoint;
                            var heroPos = hero.PredictedPos;

                            if (!hero.LastWayPoint.IsZero)
                            {
                                if (heroPos.Distance(lastWP) < 1200)
                                    lastWP = heroPos.Extend(lastWP, 1200);
                                Drawing.DrawLine(Drawing.WorldToMinimap(heroPos), Drawing.WorldToMinimap(lastWP), 2, System.Drawing.Color.OrangeRed);
                            }

                            minimapSprite.Position = Drawing.WorldToMinimap(heroPos) - new Vector2(10, 10);
                            minimapSprite.OnEndScene();
                        }
                    }

                    if (championInfo && championInfoHD)
                    {

                        var hudSprite = hero.HudSprite;

                        if (!hero.Hero.IsVisible)
                            hudSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                        else
                            hudSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());

                        Vector2 hudPos = new Vector2(posX + hudSpace, posY);
                        float scale = 0.5f;
                        hudSprite.Scale = new Vector2(scale, scale);
                        hudSprite.Position = hudPos - new Vector2(12, 10);
                        hudSprite.OnEndScene();

                        var vec1manaB = new Vector2(hudPos.X - 9, hudPos.Y + 48);
                        var vec2manaB = new Vector2(hudPos.X - 8 + 50 + 3, hudPos.Y + 48);
                        Drawing.DrawLine(vec1manaB, vec2manaB, 18, System.Drawing.Color.DarkGoldenrod);

                        var vec1hpB = new Vector2(hudPos.X - 8, hudPos.Y + 49);
                        var vec2hpB = new Vector2(hudPos.X - 8 + 50 + 2, hudPos.Y + 49);
                        Drawing.DrawLine(vec1hpB, vec2hpB, 16, System.Drawing.Color.Black);

                        System.Drawing.Color color = System.Drawing.Color.LimeGreen;
                        if (hero.Hero.HealthPercent < 30)
                            color = System.Drawing.Color.OrangeRed;
                        else if (hero.Hero.HealthPercent < 50)
                            color = System.Drawing.Color.DarkOrange;
                        var vec1hp = new Vector2(hudPos.X - 7, hudPos.Y + 50);
                        var vec2hp = new Vector2(hudPos.X - 7 + hero.Hero.HealthPercent / 2, hudPos.Y + 50);
                        Drawing.DrawLine(vec1hp, vec2hp, 7, color);

                        var vec1mana = new Vector2(hudPos.X - 7, hudPos.Y + 59);
                        var vec2mana = new Vector2(hudPos.X - 7 + hero.Hero.ManaPercent / 2, hudPos.Y + 59);
                        Drawing.DrawLine(vec1mana, vec2mana, 5, System.Drawing.Color.DodgerBlue);
                        var vecHudLevel = new Vector2(hudPos.X + 30, hudPos.Y + 33);

                        DrawFontTextScreen(HudLevel, hero.Hero.Level.ToString(), vecHudLevel.X, vecHudLevel.Y, SharpDX.Color.White);

                        {
                            if (Game.Time - hero.FinishRecallTime < 4)
                            {
                                DrawFontTextScreen(RecFont, "    FINISH", hudPos.X - 10, hudPos.Y + 18, SharpDX.Color.YellowGreen);
                            }
                            else if (hero.StartRecallTime <= hero.AbortRecallTime && Game.Time - hero.AbortRecallTime < 4)
                            {
                                DrawFontTextScreen(RecFont, "    ABORT", hudPos.X - 10, hudPos.Y + 18, SharpDX.Color.Yellow);
                            }
                            else if (Game.Time - hero.StartRecallTime < 8)
                            {
                                var recallPercent = (Game.Time - hero.StartRecallTime) / 8;
                                var vec1rec = new Vector2(hudPos.X - 9, hudPos.Y + 35);
                                var vec2rec = new Vector2(hudPos.X - 8 + 50 + 3, hudPos.Y + 35);
                                Drawing.DrawLine(vec1rec, vec2rec, 14, System.Drawing.Color.DarkGoldenrod);

                                vec1rec = new Vector2(hudPos.X - 8, hudPos.Y + 36);
                                vec2rec = new Vector2(hudPos.X - 8 + 50 + 2, hudPos.Y + 36);
                                Drawing.DrawLine(vec1rec, vec2rec, 12, System.Drawing.Color.Black);

                                vec1rec = new Vector2(hudPos.X - 7, hudPos.Y + 37);
                                vec2rec = new Vector2(hudPos.X - 7 + 100 * recallPercent / 2, hudPos.Y + 37);
                                Drawing.DrawLine(vec1rec, vec2rec, 10, System.Drawing.Color.Yellow);

                                if (blink)
                                    DrawFontTextScreen(RecFont, "RECALLING", hudPos.X - 10, hudPos.Y + 18, SharpDX.Color.White);
                            }
                            else if (!hero.Hero.IsVisible)
                            {
                                DrawFontTextScreen(RecFont, "SS  " + (int)(Game.Time - hero.LastVisableTime), hudPos.X, hudPos.Y + 18, SharpDX.Color.White);
                            }
                        }

                        var ult = hero.Hero.Spellbook.Spells[3];
                        var sum1 = hero.Hero.Spellbook.Spells[4];
                        var sum2 = hero.Hero.Spellbook.Spells[5];

                        if (ult != null)
                        {
                            var sumTime = ult.CooldownExpires - Game.Time;

                            var spritePos = new Vector2(hudPos.X + 3, hudPos.Y - 53);
                            var vecHudCd = new Vector2(hudPos.X + 10, hudPos.Y - 46);
                            var sumSprite = hero.RSprite;
                            sumSprite.Position = spritePos;

                            sumSprite.Scale = new Vector2(0.4f, 0.4f);
                            if (hero.Hero.Level < 6)
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                                sumSprite.OnEndScene();
                            }
                            else if (sumTime < 0)
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());
                                sumSprite.OnEndScene();
                            }
                            else
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                                sumSprite.OnEndScene();
                                DrawFontTextScreen(HudCd, MakeNiceNumber(sumTime), vecHudCd.X, vecHudCd.Y, SharpDX.Color.White);
                            }
                        }

                        if (sum1 != null)
                        {
                            var sumTime = sum1.CooldownExpires - Game.Time;

                            var vecFlashPos = new Vector2(hudPos.X - 10, hudPos.Y - 30);
                            var vecHudCd = new Vector2(hudPos.X - 4, hudPos.Y - 24);
                            var sumSprite = hero.Summoner1Sprite;
                            sumSprite.Position = vecFlashPos;
                            sumSprite.Scale = new Vector2(0.4f, 0.4f);

                            if (sumTime < 0)
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());
                                sumSprite.OnEndScene();
                            }
                            else
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                                sumSprite.OnEndScene();
                                DrawFontTextScreen(HudCd, MakeNiceNumber(sumTime), vecHudCd.X, vecHudCd.Y, SharpDX.Color.White);
                            }
                        }

                        if (sum2 != null)
                        {
                            var sumTime = sum2.CooldownExpires - Game.Time;

                            var vecHealPos = new Vector2(hudPos.X + 15, hudPos.Y - 30);
                            var vecHudCd = new Vector2(hudPos.X + 22, hudPos.Y - 24);
                            var sumSprite = hero.Summoner2Sprite;
                            sumSprite.Position = vecHealPos;
                            sumSprite.Scale = new Vector2(0.4f, 0.4f);

                            if (sumTime < 0)
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());
                                sumSprite.OnEndScene();
                            }
                            else
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                                sumSprite.OnEndScene();
                                DrawFontTextScreen(HudCd, MakeNiceNumber(sumTime), vecHudCd.X, vecHudCd.Y, SharpDX.Color.White);
                            }
                        }
                        hudSpace += 65;
                    }
                    else if (championInfo)
                    {
                        var hudSprite = hero.HudSprite;
                        if (!hero.Hero.IsVisible)
                            hudSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                        else
                            hudSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());

                        Vector2 hudPos = new Vector2(posX + hudSpace, posY);
                        float scale = 0.33f;
                        hudSprite.Scale = new Vector2(scale, scale);
                        hudSprite.Position = hudPos - new Vector2(11, -8);
                        hudSprite.OnEndScene();

                        var vec1manaB = new Vector2(hudPos.X - 9, hudPos.Y + 48);
                        var vec2manaB = new Vector2(hudPos.X - 8 + 33 + 3, hudPos.Y + 48);
                        Drawing.DrawLine(vec1manaB, vec2manaB, 18, System.Drawing.Color.DarkGoldenrod);

                        var vec1hpB = new Vector2(hudPos.X - 8, hudPos.Y + 49);
                        var vec2hpB = new Vector2(hudPos.X - 8 + 33 + 2, hudPos.Y + 49);
                        Drawing.DrawLine(vec1hpB, vec2hpB, 16, System.Drawing.Color.Black);

                        System.Drawing.Color color = System.Drawing.Color.LimeGreen;
                        if (hero.Hero.HealthPercent < 30)
                            color = System.Drawing.Color.OrangeRed;
                        else if (hero.Hero.HealthPercent < 50)
                            color = System.Drawing.Color.DarkOrange;
                        var vec1hp = new Vector2(hudPos.X - 7, hudPos.Y + 50);
                        var vec2hp = new Vector2(hudPos.X - 7 +  33 * hero.Hero.HealthPercent * 0.01f, hudPos.Y + 50);
                        Drawing.DrawLine(vec1hp, vec2hp, 7, color);

                        var vec1mana = new Vector2(hudPos.X - 7, hudPos.Y + 59);
                        var vec2mana = new Vector2(hudPos.X - 7 + 33 * hero.Hero.ManaPercent * 0.01f, hudPos.Y + 59);
                        Drawing.DrawLine(vec1mana, vec2mana, 5, System.Drawing.Color.DodgerBlue);
                        var vecHudLevel = new Vector2(hudPos.X + 15, hudPos.Y + 36);
                        DrawFontTextScreen(HudLevel2, hero.Hero.Level.ToString(), vecHudLevel.X, vecHudLevel.Y, SharpDX.Color.White);
                        {
                            if (Game.Time - hero.FinishRecallTime < 4)
                            {
                                DrawFontTextScreen(HudLevel2, "FINISH", hudPos.X - 9, hudPos.Y + 18, SharpDX.Color.YellowGreen);
                            }
                            else if (hero.StartRecallTime <= hero.AbortRecallTime && Game.Time - hero.AbortRecallTime < 4)
                            {
                                DrawFontTextScreen(HudLevel2, "ABORT", hudPos.X - 9, hudPos.Y + 18, SharpDX.Color.Yellow);
                            }
                            else if (Game.Time - hero.StartRecallTime < 8)
                            {
                                var recallPercent = (Game.Time - hero.StartRecallTime) / 8;
                                var vec1rec = new Vector2(hudPos.X - 9, hudPos.Y + 35);
                                var vec2rec = new Vector2(hudPos.X - 8 + 33 + 3, hudPos.Y + 35);
                                Drawing.DrawLine(vec1rec, vec2rec, 14, System.Drawing.Color.DarkGoldenrod);

                                vec1rec = new Vector2(hudPos.X - 8, hudPos.Y + 36);
                                vec2rec = new Vector2(hudPos.X - 8 + 33 + 2, hudPos.Y + 36);
                                Drawing.DrawLine(vec1rec, vec2rec, 12, System.Drawing.Color.Black);

                                vec1rec = new Vector2(hudPos.X - 7, hudPos.Y + 37);
                                vec2rec = new Vector2(hudPos.X - 7 + 33 * recallPercent, hudPos.Y + 37);
                                Drawing.DrawLine(vec1rec, vec2rec, 10, System.Drawing.Color.Yellow);

                                if (blink)
                                    DrawFontTextScreen(HudLevel2, "RECALL", hudPos.X - 9, hudPos.Y + 18, SharpDX.Color.White);

                            }
                            else if (!hero.Hero.IsVisible)
                            {
                                DrawFontTextScreen(HudLevel2, "SS  " + (int)(Game.Time - hero.LastVisableTime), hudPos.X - 9, hudPos.Y + 21, SharpDX.Color.White);
                            }
                        }

                        var ult = hero.Hero.Spellbook.Spells[3];
                        var sum1 = hero.Hero.Spellbook.Spells[4];
                        var sum2 = hero.Hero.Spellbook.Spells[5];

                        if (ult != null)
                        {
                            var sumTime = ult.CooldownExpires - Game.Time;

                            var spritePos = new Vector2(hudPos.X - 2, hudPos.Y - 30);
                            var vecHudCd = new Vector2(hudPos.X + 2, hudPos.Y - 24);
                            var sumSprite = hero.RSprite;
                            sumSprite.Position = spritePos;

                            sumSprite.Scale = new Vector2(0.35f, 0.35f);
                            if (hero.Hero.Level < 6)
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                                sumSprite.OnEndScene();
                            }
                            else if (sumTime < 0)
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());
                                sumSprite.OnEndScene();
                            }
                            else
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                                sumSprite.OnEndScene();
                                DrawFontTextScreen(HudCd, MakeNiceNumber(sumTime), vecHudCd.X, vecHudCd.Y, SharpDX.Color.White);
                            }
                        }

                        if (sum1 != null)
                        {
                            var sumTime = sum1.CooldownExpires - Game.Time;

                            var vecFlashPos = new Vector2(hudPos.X - 13, hudPos.Y - 10);
                            var vecHudCd = new Vector2(hudPos.X - 8, hudPos.Y - 5);
                            var sumSprite = hero.Summoner1Sprite;
                            sumSprite.Position = vecFlashPos;
                            sumSprite.Scale = new Vector2(0.35f, 0.35f);
                            
                            if (sumTime < 0)
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());
                                sumSprite.OnEndScene();
                            }
                            else
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                                sumSprite.OnEndScene();
                                DrawFontTextScreen(HudCd, MakeNiceNumber(sumTime), vecHudCd.X, vecHudCd.Y, SharpDX.Color.White);
                            }
                        }

                        if (sum2 != null)
                        {
                            var sumTime = sum2.CooldownExpires - Game.Time;

                            var vecHealPos = new Vector2(hudPos.X + 9, hudPos.Y - 10);
                            var vecHudCd = new Vector2(hudPos.X + 15, hudPos.Y - 5);
                            var sumSprite = hero.Summoner2Sprite;
                            sumSprite.Position = vecHealPos;
                            sumSprite.Scale = new Vector2(0.35f, 0.35f);

                            if (sumTime < 0)
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());
                                sumSprite.OnEndScene();
                            }
                            else
                            {
                                sumSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                                sumSprite.OnEndScene();
                                DrawFontTextScreen(HudCd, MakeNiceNumber(sumTime), vecHudCd.X, vecHudCd.Y, SharpDX.Color.White);
                            }
                        }
                        hudSpace += 45;
                    }
                }

                if (ScreenRadar && !hero.Hero.Position.IsOnScreen() && (!ScreenRadarEnemy || hero.Hero.IsEnemy) && (!ScreenRadarJungler || hero.IsJungler))
                {
                    var dis = centerScreenWorld.Distance(hero.Hero.Position);
                    if (dis < area)
                    {
                        float scale = 1.1f;

                        if (dis < area)
                            scale -= dis / area;

                        var normalSprite = hero.NormalSprite;

                        Vector2 dupa2 = centerScreen.Extend(Drawing.WorldToScreen(hero.Hero.Position), Drawing.Height / 2 - 120);

                        normalSprite.Scale = new Vector2(scale, scale);

                        if (hero.Hero.IsEnemy)
                        {
                            if (!hero.Hero.IsHPBarRendered)
                            {
                                dupa2 = centerScreen.Extend(Drawing.WorldToScreen(hero.LastWayPoint), Drawing.Height / 2 - 120);
                                normalSprite.Color = new ColorBGRA(System.Drawing.Color.DimGray.ToArgb());
                            }
                            else
                                normalSprite.Color = new ColorBGRA(System.Drawing.Color.White.ToArgb());
                        }
                        normalSprite.Scale = new Vector2(scale, scale);
                        normalSprite.Position = dupa2;
                        normalSprite.OnEndScene();
                    }
                }
            }
        }

        private string MakeNiceNumber(float number)
        {
            int num = (int)number;
            if (num < 10)
                return " " + num.ToString();
            else
                return num.ToString();

        }

        private static System.Drawing.Bitmap LoadImg(string imgName)
        {
            var bitmap = Resource1.ResourceManager.GetObject(imgName) as System.Drawing.Bitmap;
            if (bitmap == null)
            {
                Console.WriteLine(imgName + ".png not found.");
            }
            return bitmap;
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        public static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        public static void DrawFontTextMap(Font vFont, string vText, Vector3 Pos, ColorBGRA vColor)
        {
            var wts = Drawing.WorldToScreen(Pos);
            vFont.DrawText(null, vText, (int)wts[0], (int)wts[1], vColor);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }
    }
}
