using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using SharpDX.Direct3D9;

namespace OneKeyToWin_AIO_Sebby
{
    class Program
    {
        public enum AioMode
        {
            All = 0,
            ChampionOnly = 1,
            UtilityOnly = 2,
        }

        public static string[] SuportedChampions = { "Ahri", "Anivia", "Annie", "Ashe", "Blitzcrank", "Brand", "Braum", "Caitlyn", "Corki", "Darius", "Draven", "Ekko", "Evelynn", "Ezreal",
                                                    "Graves", "Jayce", "Jhin", "Jinx", "Kalista", "Karthus", "Kayle", "Kindred", "KogMaw", "Lucian", "Lux", "Malzahar", "MissFortune", "Morgana" ,
                                                    "Orianna", "Quinn", "Sivir", "Swain", "Syndra", "Thresh", "Tristana", "TwistedFate", "Twitch", "Urgot", "Varus", "Vayne", "Velkoz", "Xerath" };

        public static Spell Q, W, E, R, Q1, W1, E1, R1;

        private static string OktNews = "Prediction collision fixed, prediction improve";

        public static AioMode AioModeSet = AioMode.All;
        public static Menu Config;

        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Orbwalking.Orbwalker Orbwalker;

        public static Spell DrawSpell;

        public static float DrawSpellTime = 0;

        public static int HitChanceNum = 4, tickNum = 4, tickIndex = 0;

        public static SebbyLib.Prediction.PredictionOutput DrawSpellPos;

        public static bool SPredictionLoad = false;

        public static bool LaneClear = false, None = false, Harass = false, Combo = false, Farm = false;

        private static float dodgeTime = Game.Time;

        static void Main(string[] args) { CustomEvents.Game.OnGameLoad += GameOnOnGameLoad; }

        private static void GameOnOnGameLoad(EventArgs args)
        {

            Config = new Menu("OneKeyToWin AIO", "OneKeyToWin_AIO" + ObjectManager.Player.ChampionName, true).SetFontStyle(System.Drawing.FontStyle.Bold, Color.DeepSkyBlue);

            #region MENU ABOUT OKTW
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("debug", "Debug").SetValue(false));
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("debugOrb", "Debug orbwalker").SetValue(false));
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("debugChat", "Debug Chat").SetValue(false));
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("0", "OneKeyToWin© by Sebby"));
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("1", "visit joduska.me"));
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("2", "DONATE: kaczor.sebastian@gmail.com"));
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("print", "OKTW NEWS in chat").SetValue(true));
            #endregion

            Config.AddItem(new MenuItem("AIOmode", "AIO mode", true).SetValue(new StringList(new[] { "Utility and champion", "Only Champion", "Only Utility" }, 0))).ValueChanged += Program_ValueChanged;
           
            var aioModeMenu = Config.Item("AIOmode", true).GetValue<StringList>().SelectedIndex;

            if (aioModeMenu == 0)
                AioModeSet = AioMode.All;
            else if (aioModeMenu == 1)
                AioModeSet = AioMode.ChampionOnly;
            else if (aioModeMenu == 2)
                AioModeSet = AioMode.UtilityOnly;

            if (AioModeSet != AioMode.UtilityOnly)
            {
                Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
                #region LOAD CHAMPIONS

                switch (Player.ChampionName)
                {
                    case "Jinx":
                        new Champions.Jinx();
                        break;
                    case "Sivir":
                        new Champions.Sivir();
                        break;
                    case "Ezreal":
                        new Champions.Ezreal();
                        break;
                    case "KogMaw":
                        new Champions.KogMaw();
                        break;
                    case "Annie":
                        new Champions.Annie();
                        break;
                    case "Ashe":
                        new Champions.Ashe();
                        break;
                    case "MissFortune":
                        new Champions.MissFortune();
                        break;
                    case "Quinn":
                        new Champions.Quinn();
                        break;
                    case "Kalista":
                        new Champions.Kalista();
                        break;
                    case "Caitlyn":
                        new Champions.Caitlyn();
                        break;
                    case "Graves":
                        new Champions.Graves();
                        break;
                    case "Urgot":
                        new Champions.Urgot();
                        break;
                    case "Anivia":
                        new Champions.Anivia();
                        break;
                    case "Orianna":
                        new Champions.Orianna();
                        break;
                    case "Ekko":
                        new Champions.Ekko();
                        break;
                    case "Vayne":
                        new Champions.Vayne();
                        break;
                    case "Lucian":
                        new Champions.Lucian();
                        break;
                    case "Darius":
                        new Champions.Darius();
                        break;
                    case "Blitzcrank":
                        new Champions.Blitzcrank();
                        break;
                    case "Corki":
                        new Champions.Corki();
                        break;
                    case "Varus":
                        new Champions.Varus();
                        break;
                    case "Twitch":
                        new Champions.Twitch();
                        break;
                    case "Tristana":
                        new Champions.Tristana();
                        break;
                    case "Xerath":
                        new Champions.Xerath();
                        break;
                    case "Jayce":
                        new Champions.Jayce();
                        break;
                    case "Kayle":
                        new Champions.Kayle();
                        break;
                    case "Thresh":
                        new Champions.Thresh();
                        break;
                    case "Draven":
                        new Champions.Draven();
                        break;
                    case "Evelynn":
                        new Champions.Evelynn();
                        break;
                    case "Ahri":
                        new Champions.Ahri();
                        break;
                    case "Brand":
                        new Champions.Brand();
                        break;
                    case "Morgana":
                        new Champions.Morgana();
                        break;
                    case "Lux":
                        new Champions.Lux();
                        break;
                    case "Malzahar":
                        new Champions.Malzahar();
                        break;
                    case "Karthus":
                        new Champions.Karthus();
                        break;
                    case "Swain":
                        new Champions.Swain();
                        break;
                    case "TwistedFate":
                        new Champions.TwistedFate();
                        break;
                    case "Syndra":
                        new Champions.Syndra();
                        break;
                    case "Velkoz":
                        new Champions.Velkoz();
                        break;
                    case "Jhin":
                        new Champions.Jhin();
                        break;
                    case "Kindred":
                        new Champions.Kindred();
                        break;
                    case "Braum":
                        new Champions.Braum();
                        break;
                    case "Teemo":
                        new Champions.Teemo();
                        break;
                }
                #endregion

                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("Qpred", "Q Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION", "SPediction press F5 if not loaded", "SDK"}, 1)));
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("QHitChance", "Q Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("Wpred", "W Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION", "SPediction press F5 if not loaded", "SDK"}, 1)));
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("WHitChance", "W Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("Epred", "E Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION", "SPediction press F5 if not loaded", "SDK" }, 1)));
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("EHitChance", "E Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("Rpred", "R Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION", "SPediction press F5 if not loaded", "SDK" }, 1)));
                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("RHitChance", "R Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));

                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("debugPred", "Draw Aiming OKTW© PREDICTION").SetValue(false));

                Config.SubMenu("Prediction MODE").AddItem(new MenuItem("322", "SPREDICTION NOT LOADED"));
                new Core.OktwTs();
            }

            if (Config.Item("debug").GetValue<bool>())
            {
                new Core.OKTWlab().LoadOKTW();
            }

            if (AioModeSet != AioMode.ChampionOnly)
            {
                new Activator().LoadOKTW();
                new Core.OKTWward().LoadOKTW();
                new Core.AutoLvlUp().LoadOKTW();
                new Core.OKTWdraws().LoadOKTW();
            }

            new Core.OKTWtracker().LoadOKTW();

            Config.AddItem(new MenuItem("aiomodes", "!!! PRESS F5 TO RELOAD MODE !!!"));
            Config.Item("aiomodes").Show(false);
            Config.AddToMainMenu();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

            if (Config.Item("print").GetValue<bool>())
            {
                Game.PrintChat("<font size='30'>OneKeyToWin</font> <font color='#b756c5'>by Sebby</font>");
                Game.PrintChat("<font color='#b756c5'>OKTW NEWS: </font>" + OktNews);
            }
        }

        private static void Program_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            Config.Item("aiomodes").Show(true);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (AioModeSet == AioMode.UtilityOnly)
            {
                if (Player.IsMoving)
                    Combo = true;
                else
                    Combo = false;
            }
            else
            {
                Combo = Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;

                if(Config.Item("harassMixed").GetValue<bool>())
                    Harass = Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed;
                else
                    Harass = Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Freeze;

                Farm = Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Freeze;
                None = Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None;
                LaneClear = Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear;
            }

            tickIndex++;
            if (tickIndex > 4)
                tickIndex = 0;
        }


        public static bool LagFree(int offset)
        {
            if (tickIndex == offset)
                return true;
            else
                return false;
        }

        public static void CastSpell(Spell QWER, Obj_AI_Base target)
        {
            int predIndex = 0;
            HitChance hitchance = HitChance.Low;

            if (QWER.Slot == SpellSlot.Q)
            {
                predIndex = Config.Item("Qpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.W)
            {
                predIndex = Config.Item("Wpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.E)
            {
                predIndex = Config.Item("Epred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.R)
            {
                predIndex = Config.Item("Rpred", true).GetValue<StringList>().SelectedIndex;
                if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (Config.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }

            if (predIndex == 3)
            {
                SebbyLib.Movement.SkillshotType CoreType2 = SebbyLib.Movement.SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    //CoreType2 = SebbyLib.Movement.SkillshotType.SkillshotCircle;
                    //aoe2 = true;
                }

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new SebbyLib.Movement.PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = SebbyLib.Movement.Prediction.GetPrediction(predInput2);

                //var poutput2 = QWER.GetPrediction(target);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if ((int)hitchance == 6)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Movement.HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }

                }
                else if ((int)hitchance == 5)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.High)
                        QWER.Cast(poutput2.CastPosition);

                }
                else if ((int)hitchance == 4)
                {
                    if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.Medium)
                        QWER.Cast(poutput2.CastPosition);
                }
            }
            else if (predIndex == 1)
            {
                SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (QWER.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                if (QWER.Width > 80 && !QWER.Collision)
                    aoe2 = true;

                var predInput2 = new SebbyLib.Prediction.PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                //var poutput2 = QWER.GetPrediction(target);

                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if ((int)hitchance == 6)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                        QWER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }

                }
                else if ((int)hitchance == 5)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                        QWER.Cast(poutput2.CastPosition);

                }
                else if ((int)hitchance == 4)
                {
                    if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                        QWER.Cast(poutput2.CastPosition);
                }
                if (Game.Time - DrawSpellTime > 0.5)
                {
                    DrawSpell = QWER;
                    DrawSpellTime = Game.Time;

                }
                DrawSpellPos = poutput2;
            }
            else if (predIndex == 0)
            {
                QWER.CastIfHitchanceEquals(target, hitchance);
            }
            else if (predIndex == 2)
            {
                QWER.CastIfHitchanceEquals(target, HitChance.High);
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        public static void debug(string msg)
        {
            if (Config.Item("debug").GetValue<bool>())
            {
                Console.WriteLine(msg);
            }
            if (Config.Item("debugChat").GetValue<bool>())
            {
                Game.PrintChat(msg);
            }
        }


        private static void OnDraw(EventArgs args)
        {
           
                if (AioModeSet != AioMode.UtilityOnly && !SPredictionLoad && (int)Game.Time % 2 == 0 && (Config.Item("Qpred", true).GetValue<StringList>().SelectedIndex == 2 || Config.Item("Wpred", true).GetValue<StringList>().SelectedIndex == 2
                || Config.Item("Epred", true).GetValue<StringList>().SelectedIndex == 2 || Config.Item("Rpred", true).GetValue<StringList>().SelectedIndex == 2))
                drawText("PRESS F5 TO LOAD SPREDICTION", Player.Position, System.Drawing.Color.Yellow, -300);

            

        }
    }
}
