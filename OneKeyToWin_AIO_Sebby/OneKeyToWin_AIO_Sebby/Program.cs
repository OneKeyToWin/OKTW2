using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.SebbyLib;
using PROdiction;
using SharpDX;
using SebbyLib;

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

        public static string[] SuportedChampions = { "Ahri", "Amumu", "Anivia", "Annie", "Ashe", "Blitzcrank", "Brand", "Braum", "Caitlyn", "Corki", "Darius", "Draven", "Ekko", "Evelynn", "Ezreal",
                                                    "Graves", "Jayce", "Jhin", "Jinx", "Kalista", "Karthus", "Kayle", "Kindred", "KogMaw", "Lucian", "Lux", "Malzahar", "MissFortune", "Morgana" ,
                                                    "Orianna", "Quinn", "Sivir", "Swain", "Syndra", "Thresh", "Tristana", "TwistedFate", "Twitch", "Urgot", "Varus", "Vayne", "Velkoz", "Xerath" };

        public static Menu MainMenu;
        public static Menu HeroMenu;

        public static Spell Q, W, E, R, Q1, W1, E1, R1;
        public static AioMode AioModeSet = AioMode.All;
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell DrawSpell;
        public static float DrawSpellTime = 0;
        public static int HitChanceNum = 4, tickNum = 4, tickIndex = 0;
        public static PredictionOutput DrawSpellPos;
        public static bool LaneClear = false, None = false, Harass = false, Combo = false, Farm = false;
        private static float dodgeTime = Game.Time;

        static void Main(string[] args) { CustomEvents.Game.OnGameLoad += GameOnOnGameLoad; }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            MainMenu = new Menu("OneKeyToWin AIO", "OneKeyToWin_AIO" + ObjectManager.Player.ChampionName, true).SetFontStyle(System.Drawing.FontStyle.Bold, Color.DeepSkyBlue);

            #region MENU ABOUT OKTW
            MainMenu.SubMenu("About OKTW©").AddItem(new MenuItem("debug", "Debug").SetValue(false));
            MainMenu.SubMenu("About OKTW©").AddItem(new MenuItem("debugOrb", "Debug orbwalker").SetValue(false));
            MainMenu.SubMenu("About OKTW©").AddItem(new MenuItem("debugChat", "Debug Chat").SetValue(false));
            MainMenu.SubMenu("About OKTW©").AddItem(new MenuItem("print", "OKTW NEWS in chat").SetValue(true));
            #endregion

            MainMenu.AddItem(new MenuItem("AIOmode", "AIO mode", true).SetValue(new StringList(new[] { "Utility and champion", "Only Champion", "Only Utility" }, 0))).ValueChanged += Program_ValueChanged;
           
            var aioModeMenu = MainMenu.Item("AIOmode", true).GetValue<StringList>().SelectedIndex;

            if (aioModeMenu == 0)
                AioModeSet = AioMode.All;
            else if (aioModeMenu == 1)
                AioModeSet = AioMode.ChampionOnly;
            else if (aioModeMenu == 2)
                AioModeSet = AioMode.UtilityOnly;

            if (AioModeSet != AioMode.UtilityOnly)
            {
                Orbwalker = new Orbwalking.Orbwalker(MainMenu.AddSubMenu(new Menu("Orbwalking", "Orbwalking")));
                HeroMenu = MainMenu.SubMenu(Player.ChampionName).SetFontStyle(System.Drawing.FontStyle.Bold, Color.OrangeRed);

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
                    case "Garen":
                        new Champions.Garen();
                        break;
                    case "Amumu":
                        new Champions.Amumu();
                        break;
                    case "Viego":
                        new Champions.Viego();
                        break;
                    case "Leona":
                        new Champions.Leona();
                        break;
                    case "Hecarim":
                        new Champions.Hecarim();
                        break;
                    case "Nunu":
                        new Champions.Nunu();
                        break;
                }
                #endregion

                AddPredictionSpellMenuItem("Q");
                AddPredictionSpellMenuItem("W");
                AddPredictionSpellMenuItem("E");
                AddPredictionSpellMenuItem("R");
                
                MainMenu.SubMenu("Prediction MODE").AddItem(new MenuItem("debugPred", "Draw Aiming OKTW© PREDICTION").SetValue(false));
            }

            if (MainMenu.Item("debug").GetValue<bool>())
            {
                new Core.OKTWlab().LoadOKTW();
            }

            if (AioModeSet != AioMode.ChampionOnly)
            {
                Summoners summonners = new Summoners();
                ItemsActivator itemsActivator = new ItemsActivator();
                TrackerCore trackerCore = new TrackerCore();
                BaseUlt baseUlt = new BaseUlt();
                AutoLvlUp autoLvlUp = new AutoLvlUp();
                ModernUtlity modernUtlity = new ModernUtlity();
                new Core.OKTWward().LoadOKTW();
                //new Core.OKTWdraws().LoadOKTW();
            }

            MainMenu.AddItem(new MenuItem("aiomodes", "!!! PRESS F5 TO RELOAD MODE !!!"));
            MainMenu.Item("aiomodes").Show(false);
            MainMenu.AddToMainMenu();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

            if (MainMenu.Item("print").GetValue<bool>())
            {
                Game.PrintChat("<font size='30'>OneKeyToWin</font> <font color='#b756c5'>by Sebby</font>");
            }
        }

        private static void AddPredictionSpellMenuItem(string spellSlot)
        {
            var predType = MainMenu.SubMenu("Prediction MODE")
                .AddItem(new MenuItem($"{spellSlot}pred", $"{spellSlot} Prediction MODE", true).SetValue(
                    new StringList(new[] {"Common prediction", "PROdiction"}, 1)));
                
            var isAiPrediction = predType.GetValue<StringList>().SelectedIndex == 1;
            var regularPredictionMenu = MainMenu.SubMenu("Prediction MODE")
                .AddItem(new MenuItem($"{spellSlot}HitChance", $"{spellSlot} Hit Chance", true)
                    .SetValue(new StringList(new[] {"Very High", "High", "Medium"}, 0))).Show(!isAiPrediction);
            var aiPredictionMenu = MainMenu.SubMenu("Prediction MODE")
                .AddItem(new MenuItem($"{spellSlot}HitChanceAI", $"{spellSlot} Hit Chance AI", true).SetValue(new Slider(50, 0, 100)))
                .Show(isAiPrediction);

            predType.ValueChanged += (sender, args) =>
            {
                if (args.GetNewValue<StringList>().SelectedIndex == 1)
                {
                    aiPredictionMenu.Show(true);
                    regularPredictionMenu.Show(false);
                }
                else
                {
                    aiPredictionMenu.Show(false);
                    regularPredictionMenu.Show(true);
                }
            };
        }
        
        private static void Program_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            MainMenu.Item("aiomodes").Show(true);
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

                if(MainMenu.Item("harassMixed") != null && MainMenu.Item("harassMixed").GetValue<bool>())
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
                predIndex = MainMenu.Item("Qpred", true).GetValue<StringList>().SelectedIndex;
                if (MainMenu.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (MainMenu.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (MainMenu.Item("QHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.W)
            {
                predIndex = MainMenu.Item("Wpred", true).GetValue<StringList>().SelectedIndex;
                if (MainMenu.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (MainMenu.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (MainMenu.Item("WHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.E)
            {
                predIndex = MainMenu.Item("Epred", true).GetValue<StringList>().SelectedIndex;
                if (MainMenu.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (MainMenu.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (MainMenu.Item("EHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }
            else if (QWER.Slot == SpellSlot.R)
            {
                predIndex = MainMenu.Item("Rpred", true).GetValue<StringList>().SelectedIndex;
                if (MainMenu.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 0)
                    hitchance = HitChance.VeryHigh;
                else if (MainMenu.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 1)
                    hitchance = HitChance.High;
                else if (MainMenu.Item("RHitChance", true).GetValue<StringList>().SelectedIndex == 2)
                    hitchance = HitChance.Medium;
            }

            if (predIndex == 0)
            {
                var predInput2 = new PredictionInput
                {
                    Aoe = false,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = QWER.Type
                };
                
                var poutput2 = Prediction.GetPrediction(predInput2);
                
                if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if ((int)hitchance == 6)
                {
                    if (poutput2.Hitchance >= HitChance.VeryHigh)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }

                }
                else if ((int)hitchance == 5)
                {
                    if (poutput2.Hitchance >= HitChance.High)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }

                }
                else if ((int)hitchance == 4)
                {
                    if (poutput2.Hitchance >= HitChance.Medium)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }
                }
                if (Game.Time - DrawSpellTime > 0.5)
                {
                    DrawSpell = QWER;
                    DrawSpellTime = Game.Time;

                }
                DrawSpellPos = poutput2;
            }
            else if (predIndex == 1)
            {
                var predInput2 = new PredictionInput
                {
                    Aoe = false,
                    Collision = QWER.Collision,
                    Speed = QWER.Speed,
                    Delay = QWER.Delay,
                    Range = QWER.Range,
                    Slot = QWER.Slot,
                    From = Player.ServerPosition,
                    Radius = QWER.Width,
                    Unit = target,
                    Type = QWER.Type
                };

                var poutput2 = Prediction.GetPrediction(predInput2);
                
                if (poutput2.Hitchance >= HitChance.Low && poutput2.Hitchance <= HitChance.VeryHigh)
                {
                    var prediction = AIPrediction.GetPrediction(predInput2);

                    if (Game.Time - DrawSpellTime > 0.5)
                    {
                        DrawSpell = QWER;
                        DrawSpellTime = Game.Time;
                    }

                    DrawSpellPos = poutput2;

                    switch (QWER.Slot)
                    {
                        case SpellSlot.Q:
                        {
                            var targetPrediction = MainMenu.Item("QHitChanceAI", true).GetValue<Slider>().Value / 100.0f;
                            if (prediction.HitchanceFloat > targetPrediction)
                            {
                                QWER.Cast(prediction.CastPosition);
                                return;
                            }

                            break;
                        }

                        case SpellSlot.W:
                        {
                            var targetPrediction = MainMenu.Item("WHitChanceAI", true).GetValue<Slider>().Value / 100.0f;
                            if (prediction.HitchanceFloat > targetPrediction)
                            {
                                QWER.Cast(prediction.CastPosition);
                                return;
                            }

                            break;
                        }

                        case SpellSlot.E:
                        {
                            var targetPrediction = MainMenu.Item("EHitChanceAI", true).GetValue<Slider>().Value / 100.0f;
                            if (prediction.HitchanceFloat > targetPrediction)
                            {
                                QWER.Cast(prediction.CastPosition);
                                return;
                            }

                            break;
                        }

                        case SpellSlot.R:
                        {
                            var targetPrediction = MainMenu.Item("RHitChanceAI", true).GetValue<Slider>().Value / 100.0f;
                            if (prediction.HitchanceFloat > targetPrediction)
                            {
                                QWER.Cast(prediction.CastPosition);
                                return;
                            }
                            
                            break;
                        }

                        default:
                        {
                            return;
                        }
                    }

                }

                if (poutput2.Hitchance == HitChance.Immobile || poutput2.Hitchance == HitChance.Dashing)
                {
                    QWER.Cast(poutput2.CastPosition);
                }
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        public static void debug(string msg)
        {
            if (MainMenu.Item("debug").GetValue<bool>())
            {
                Console.WriteLine(msg);
            }
            if (MainMenu.Item("debugChat").GetValue<bool>())
            {
                Game.PrintChat(msg);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (AioModeSet != AioMode.UtilityOnly && (int)Game.Time % 2 == 0 && (MainMenu.Item("Qpred", true).GetValue<StringList>().SelectedIndex == 2 || MainMenu.Item("Wpred", true).GetValue<StringList>().SelectedIndex == 2
            || MainMenu.Item("Epred", true).GetValue<StringList>().SelectedIndex == 2 || MainMenu.Item("Rpred", true).GetValue<StringList>().SelectedIndex == 2))
            drawText("PRESS F5 TO LOAD SPREDICTION", Player.Position, System.Drawing.Color.Yellow, -300);
        }
    }
}
