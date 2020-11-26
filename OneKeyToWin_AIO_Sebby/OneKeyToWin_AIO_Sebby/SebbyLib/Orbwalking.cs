using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace SebbyLib
{
    public static class Orbwalking
    {
        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);

        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
 
        public delegate void OnAttackEvenH(AttackableUnit unit, AttackableUnit target);

        public delegate void OnNonKillableMinionH(AttackableUnit minion);

        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);

        public enum OrbwalkingMode
        {
            LastHit,Mixed,LaneClear,Combo,Freeze,CustomMode,None
        }

        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "gravesmove", "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "gangplankqwrapper", "poppypassiveattack", "powerfist", "renektonpreexecute", "rengarq",
            "shyvanadoubleattack", "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble",
            "vie", "volibearq", "xenzhaocombotarget", "yorickspectral", "reksaiq", "itemtitanichydracleave", "masochism",
            "illaoiw", "elisespiderw", "fiorae", "meditate", "sejuaninorthernwinds","asheq"
        };

        private static readonly string[] NoAttacks =
        {
            "volleyattack", "volleyattackwithsound",
            "jarvanivcataclysmattack", "monkeykingdoubleattack", "shyvanadoubleattack", "shyvanadoubleattackdragon",
            "zyragraspingplantattack", "zyragraspingplantattack2", "zyragraspingplantattackfire",
            "zyragraspingplantattack2fire", "viktorpowertransfer", "sivirwattackbounce", "asheqattacknoonhit",
            "elisespiderlingbasicattack", "heimertyellowbasicattack", "heimertyellowbasicattack2",
            "heimertbluebasicattack", "annietibbersbasicattack", "annietibbersbasicattack2",
            "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack", "yorickspectralghoulbasicattack",
            "malzaharvoidlingbasicattack", "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
            "kindredwolfbasicattack", "gravesautoattackrecoil"
        };

        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced", "renektonexecute", "renektonsuperexecute",
            "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff"
        };

        private static readonly string[] NoCancelChamps = { "Kalista" };

        public static List<Obj_AI_Base> MinionListAA = new List<Obj_AI_Base>();

        public static int LastAATick;

        private static int DelayOnFire = 0;

        private static int DelayOnFireId = 0;
        private static int TimeAdjust = 0 ;

        private static int BrainFarmInt = -100;

        public static bool Attack = true;

        public static bool DisableNextAttack;

        public static bool Move = true;

        public static int LastAttackCommandT;

        public static int LastMoveCommandT;

        public static Vector3 LastMoveCommandPosition = Vector3.Zero;

        private static AttackableUnit _lastTarget;

        private static readonly Obj_AI_Hero Player;

        private static int _delay;

        private static float _minDistance = 400;

        private static bool _missileLaunched;

        private static readonly string _championName;

        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        private static int _autoattackCounter;

        static Orbwalking()
        {
            Player = ObjectManager.Player;
            _championName = Player.ChampionName;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnDoCast += Obj_AI_Base_OnDoCast;
            Spellbook.OnStopCast += SpellbookOnStopCast;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
        }

        private static void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if(DelayOnFire != 0 && missile != null && Player.AttackDelay > 1 / 2f)
            {
                if(missile.SpellCaster.IsMe && missile.SData.IsAutoAttack() &&  DelayOnFireId == missile.Target.NetworkId)
                {
                    var x = Utils.TickCount - DelayOnFire;

                    if (x < 110 - Game.Ping / 2)
                    {
                        BrainFarmInt -= 2;
                    }
                    else if (x > 130 - Game.Ping / 2)
                    {
                        BrainFarmInt += 2;
                    }

                    Console.WriteLine(BrainFarmInt + " ADJ " + x + " " + TimeAdjust);
                    //Console.WriteLine(missile.Target.BoundingRadius + " dis2 " + (missile.Position.Distance(missile.Target.Position)));
                }
            }
        }


        public static event BeforeAttackEvenH BeforeAttack;

        public static event OnAttackEvenH OnAttack;

        public static event AfterAttackEvenH AfterAttack;

        public static event OnTargetChangeH OnTargetChange;

        public static event OnNonKillableMinionH OnNonKillableMinion;

        private static void FireBeforeAttack(AttackableUnit target)
        {
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs { Target = target });
            }
            else
            {
                DisableNextAttack = false;
            }
        }

        private static void FireOnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (OnAttack != null)
            {
                OnAttack(unit, target);
            }
        }

        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            {
                AfterAttack(unit, target);
            }
        }

        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!_lastTarget.IsValidTarget() || _lastTarget != newTarget))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            if (OnNonKillableMinion != null)
            {
                OnNonKillableMinion(minion);
            }
        }

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static bool IsMelee(this Obj_AI_Base unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
                   Attacks.Contains(name.ToLower());
        }

        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = Player.AttackRange + Player.BoundingRadius;
            if (target.IsValidTarget())
            {
                var aiBase = target as Obj_AI_Base;
                if (aiBase != null && Player.ChampionName == "Caitlyn")
                {
                    if (aiBase.HasBuff("caitlynyordletrapinternal"))
                    {
                        result += 650;
                    }
                }

                return result + target.BoundingRadius;
            }

            return result;
        }

        public static float GetAttackRange(Obj_AI_Hero target)
        {
            var result = target.AttackRange + target.BoundingRadius;
            return result;
        }

        public static bool InAutoAttackRange(AttackableUnit target, float extraRange = 0)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }

            var myRange = GetRealAutoAttackRange(target) + extraRange;
            var hero = target as Obj_AI_Hero;
            if(hero!= null)
            {
                return
                Vector2.DistanceSquared(
                   Prediction.Prediction.GetPrediction(hero, 0).CastPosition.To2D(),  Player.Position.To2D()) <= myRange * myRange;
            }


            return
                Vector2.DistanceSquared(
                    target is Obj_AI_Base ? ((Obj_AI_Base)target).ServerPosition.To2D() : target.Position.To2D(),
                    Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        public static float GetMyProjectileSpeed()
        {
            if (_championName == "Kayle")
            {
                if (Player.AttackRange < 525)
                {
                    return float.MaxValue;
                }

                return 2250;
            }

            return IsMelee(Player) || _championName == "Azir" || _championName == "Thresh" || _championName == "Velkoz" ||
                   _championName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn")
                ? float.MaxValue
                : Player.BasicAttack.MissileSpeed;
        }

        public static bool CanAttack()
        {
            if (Player.ChampionName == "Graves")
            {
                var attackDelay = 1.0740296828d * 1000 * Player.AttackDelay - 716.2381256175d;
                if (Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + attackDelay &&
                    Player.HasBuff("GravesBasicAttackAmmo1"))
                {
                    return true;
                }

                return false;
            }

            if (Player.ChampionName == "Kayle")
            {
                if (Player.HasBuff("KayleR"))
                {
                    return false;
                }
            }

            if (Player.ChampionName == "Jhin")
            {
                if (Player.HasBuff("JhinPassiveReload"))
                {
                    return false;
                }
            }

            return Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + Player.AttackDelay * 1000;
        }

        public static bool CanMove(float extraWindup, bool disableMissileCheck = false)
        {
            if (_missileLaunched && Orbwalker.MissileCheck && !disableMissileCheck)
            {
                return true;
            }

            var localExtraWindup = 0;
            if (_championName == "Rengar" && (Player.HasBuff("rengarqbase") || Player.HasBuff("rengarqemp")))
            {
                localExtraWindup = 200;
            }

            return NoCancelChamps.Contains(_championName) ||
                   (Utils.GameTimeTickCount + Game.Ping / 2 >=
                    LastAATick + Player.AttackCastDelay * 1000 + extraWindup + localExtraWindup);
        }

        public static void SetMovementDelay(int delay)
        {
            _delay = delay;
        }

        public static void SetMinimumOrbwalkDistance(float d)
        {
            _minDistance = d;
        }

        public static float GetLastMoveTime()
        {
            return LastMoveCommandT;
        }

        public static Vector3 GetLastMovePosition()
        {
            return LastMoveCommandPosition;
        }

        public static void MoveTo(Vector3 position,
            float holdAreaRadius = 0,
            bool overrideTimer = false,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            var playerPosition = Player.ServerPosition;

            if (playerPosition.Distance(position, true) < holdAreaRadius * holdAreaRadius)
            {
                if (Player.Path.Length > 0)
                {
                    Player.ForceIssueOrder(GameObjectOrder.Stop, playerPosition);
                    LastMoveCommandPosition = playerPosition;
                    LastMoveCommandT = Utils.GameTimeTickCount - 70;
                }
                return;
            }

            var point = position;

            if (Player.Distance(point, true) < 150 * 150)
            {
                point = playerPosition.Extend(
                    position, randomizeMinDistance ? (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance : _minDistance);
            }
            var angle = 0f;
            var currentPath = Player.GetWaypoints();
            if (currentPath.Count > 1 && currentPath.PathLength() > 100)
            {
                var movePath = Player.GetPath(point);

                if (movePath.Length > 1)
                {
                    var v1 = currentPath[1] - currentPath[0];
                    var v2 = movePath[1] - movePath[0];
                    angle = v1.AngleBetween(v2.To2D());
                    var distance = movePath.Last().To2D().Distance(currentPath.Last(), true);

                    if ((angle < 10 && distance < 500 * 500) || distance < 50 * 50)
                    {
                        return;
                    }
                }
            }

            if (Utils.GameTimeTickCount - LastMoveCommandT < 70 + Math.Min(60, Game.Ping) && !overrideTimer &&
                angle < 60)
            {
                return;
            }

            if (angle >= 60 && Utils.GameTimeTickCount - LastMoveCommandT < 60)
            {
                return;
            }

            Player.ForceIssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = point;
            LastMoveCommandT = Utils.GameTimeTickCount;
        }

        public static void Orbwalk(AttackableUnit target,
            Vector3 position,
            float extraWindup = 90,
            float holdAreaRadius = 0,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            if (Utils.GameTimeTickCount - LastAttackCommandT < 70 + Math.Min(60, Game.Ping))
            {
                return;
            }

            try
            {
                if (target.IsValidTarget() && CanAttack() && Attack)
                {
                    DisableNextAttack = false;
                    FireBeforeAttack(target);

                    if (!DisableNextAttack)
                    {
                        if (!NoCancelChamps.Contains(_championName))
                        {
                            _missileLaunched = false;
                        }

                        if (Player.ForceIssueOrder(GameObjectOrder.AttackUnit, target))
                        {
                            LastAttackCommandT = Utils.GameTimeTickCount;
                            _lastTarget = target;
                        }

                        return;
                    }
                }

                if (CanMove(extraWindup) && Move)
                {
                    if (Orbwalker.LimitAttackSpeed && (Player.AttackDelay < 1 / 2.6f) && _autoattackCounter % 3 != 0 &&
                        !CanMove(500, true))
                    {
                        return;
                    }

                    MoveTo(position, Math.Max(holdAreaRadius, 30), false, useFixedDistance, randomizeMinDistance);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ResetAutoAttackTimer()
        {
            LastAATick = 0;
        }

        private static void SpellbookOnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && spellbook.Owner.IsMe && args.DestroyMissile && args.StopAnimation)
            {
                ResetAutoAttackTimer();
            }
        }

        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                var ping = Game.Ping;
                if (ping <= 30) //First world problems kappa
                {
                    Utility.DelayAction.Add(30 - ping, () => Obj_AI_Base_OnDoCast_Delayed(sender, args));
                    return;
                }

                Obj_AI_Base_OnDoCast_Delayed(sender, args);
            }
        }

        private static void Obj_AI_Base_OnDoCast_Delayed(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (IsAutoAttackReset(args.SData.Name))
            {
                ResetAutoAttackTimer();
            }

            if (IsAutoAttack(args.SData.Name))
            {
                FireAfterAttack(sender, args.Target as AttackableUnit);
                _missileLaunched = true;
            }
        }

        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs Spell)
        {
            try
            {
                var spellName = Spell.SData.Name;

                if (unit.IsMe && IsAutoAttackReset(spellName) && Spell.SData.SpellCastTime == 0)
                {
                    ResetAutoAttackTimer();
                }

                if (!IsAutoAttack(spellName))
                {
                    return;
                }

                if (unit.IsMe &&
                    (Spell.Target is Obj_AI_Base || Spell.Target is Obj_BarracksDampener || Spell.Target is Obj_HQ))
                {
                    LastAATick = Utils.GameTimeTickCount - Game.Ping / 2;
                    _missileLaunched = false;
                    LastMoveCommandT = 0;
                    _autoattackCounter++;

                    if (Spell.Target is Obj_AI_Base)
                    {
                        var target = (Obj_AI_Base)Spell.Target;
                        if (target.IsValid)
                        {
                            FireOnTargetSwitch(target);
                            _lastTarget = target;
                        }
                    }
                }

                FireOnAttack(unit, _lastTarget);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public class BeforeAttackEventArgs : EventArgs
        {
            private bool _process = true;

            public AttackableUnit Target;

            public Obj_AI_Base Unit = ObjectManager.Player;

            public bool Process
            {
                get { return _process; }
                set
                {
                    DisableNextAttack = !value;
                    _process = value;
                }
            }
        }

        public class Orbwalker
        {
            private double LaneClearWaitTimeMod
            {
                get { return 1.8 - 0.01 * _config.Item("LaneClearSpeed").GetValue<Slider>().Value; }
            }
           

            /// <summary>
            ///     The configuration
            /// </summary>
            private static Menu _config;

            /// <summary>
            ///     The instances of the orbwalker.
            /// </summary>
            public static List<Orbwalker> Instances = new List<Orbwalker>();

            /// <summary>
            ///     The player
            /// </summary>
            private readonly Obj_AI_Hero Player;

            /// <summary>
            ///     The forced target
            /// </summary>
            private Obj_AI_Base _forcedTarget;

            /// <summary>
            ///     The orbalker mode
            /// </summary>
            private OrbwalkingMode _mode = OrbwalkingMode.None;

            /// <summary>
            ///     The orbwalking point
            /// </summary>
            private Vector3 _orbwalkingPoint;

            /// <summary>
            ///     The previous minion the orbwalker was targeting.
            /// </summary>
            private Obj_AI_Minion _prevMinion;

            /// <summary>
            ///     The name of the CustomMode if it is set.
            /// </summary>
            private string CustomModeName;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Orbwalker" /> class.
            /// </summary>
            /// <param name="attachToMenu">The menu the orbwalker should attach to.</param>
            public Orbwalker(Menu attachToMenu)
            {
                _config = attachToMenu;
                /* Drawings submenu */
                var drawings = new Menu("Drawings", "drawings");
                drawings.AddItem(
                    new MenuItem("AACircle", "AACircle").SetShared()
                        .SetValue(new Circle(true, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(
                    new MenuItem("AACircle2", "Enemy AA circle").SetShared()
                        .SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(
                    new MenuItem("HoldZone", "HoldZone").SetShared()
                        .SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(new MenuItem("AALineWidth", "Line Width")).SetShared().SetValue(new Slider(2, 1, 6));
                drawings.AddItem(new MenuItem("LastHitHelper", "Last Hit Helper").SetShared().SetValue(false));
                _config.AddSubMenu(drawings);

                /* Misc options */
                var misc = new Menu("Misc", "Misc");
                misc.AddItem(
                    new MenuItem("HoldPosRadius", "Hold Position Radius").SetShared().SetValue(new Slider(0, 0, 250)));
                misc.AddItem(new MenuItem("PriorizeFarm", "Priorize farm over harass").SetShared().SetValue(true));

                misc.SubMenu("Auto attack wards").AddItem(new MenuItem("AttackWardsCombo", "Combo").SetShared().SetValue(false));
                misc.SubMenu("Auto attack wards").AddItem(new MenuItem("AttackWardsMixed", "Mixed").SetShared().SetValue(true));
                misc.SubMenu("Auto attack wards").AddItem(new MenuItem("AttackWardsLaneClear", "Lane Clear").SetShared().SetValue(true));
                misc.SubMenu("Auto attack wards").AddItem(new MenuItem("AttackWardsLastHit", "Last Hit").SetShared().SetValue(true));
                misc.SubMenu("Auto attack wards").AddItem(new MenuItem("AttackWardsFreeze", "Freeze").SetShared().SetValue(true));

                misc.SubMenu("Auto attack pets & traps").AddItem(new MenuItem("AttackBarrel", "Auto attack gangplank barrel").SetShared().SetValue(true));
                misc.SubMenu("Auto attack pets & traps").AddItem(new MenuItem("AttackPetsnTrapsCombo", "Combo").SetShared().SetValue(false));
                misc.SubMenu("Auto attack pets & traps").AddItem(new MenuItem("AttackPetsnTrapsMixed", "Mixed").SetShared().SetValue(true));
                misc.SubMenu("Auto attack pets & traps").AddItem(new MenuItem("AttackPetsnTrapsLaneClear", "Lane Clear").SetShared().SetValue(true));
                misc.SubMenu("Auto attack pets & traps").AddItem(new MenuItem("AttackPetsnTrapsLastHit", "Last Hit").SetShared().SetValue(true));
                misc.SubMenu("Auto attack pets & traps").AddItem(new MenuItem("AttackPetsnTrapsFreeze", "Freeze").SetShared().SetValue(true));

                misc.SubMenu("Auto attack buildings").AddItem(new MenuItem("BuildingsCombo", "Combo").SetShared().SetValue(false));
                misc.SubMenu("Auto attack buildings").AddItem(new MenuItem("BuildingsMixed", "Mixed").SetShared().SetValue(true));
                misc.SubMenu("Auto attack buildings").AddItem(new MenuItem("BuildingsLaneClear", "Lane Clear").SetShared().SetValue(true));
                misc.SubMenu("Auto attack buildings").AddItem(new MenuItem("BuildingsLastHit", "Last Hit").SetShared().SetValue(true));
                misc.SubMenu("Auto attack buildings").AddItem(new MenuItem("BuildingsFreeze", "Freeze").SetShared().SetValue(true));

                misc.AddItem(new MenuItem("Smallminionsprio", "Jungle clear small first").SetShared().SetValue(false));
                misc.AddItem(
                    new MenuItem("LimitAttackSpeed", "Don't kite if Attack Speed > 2.5").SetShared().SetValue(false));

                /* Missile check */
                misc.AddItem(new MenuItem("MissileCheck", "Use Missile Check").SetShared().SetValue(true));

                /* Delay sliders */
                misc.AddItem(
                    new MenuItem("ExtraWindup", "Extra windup time").SetShared().SetValue(new Slider(80, 0, 200)));
                misc.AddItem(new MenuItem("FarmDelay", "Farm delay").SetShared().SetValue(new Slider(0, 0, 200)));
                misc.AddItem(new MenuItem("LaneClearSpeed", "LaneClear speed").SetShared().SetValue(new Slider(0, -100, 100))).SetTooltip("higher number = faster");


                _config.AddSubMenu(misc);


                var sebbyFix = new Menu("Sebby FIX [ADVANCE]", "Sebby FIX [ADVANCE]");

                sebbyFix.AddItem(new MenuItem("DamageAdjust", "Last hit auto attack damage [0 default]").SetShared().SetValue(new Slider(0,-100, 100)));
                sebbyFix.AddItem(new MenuItem("AutoTimeAdjust", "Auto lasthit time adjust", true).SetShared().SetValue(false));
                sebbyFix.AddItem(new MenuItem("TimeAdjust", "FASTER    Last hit time adjust    LATER").SetShared().SetValue(new Slider(0, -100, 100))).SetTooltip("0 defaul");
                sebbyFix.AddItem(new MenuItem("PassiveDmg", "Last hit include passive damage", true).SetShared().SetValue(true));

                _config.AddSubMenu(sebbyFix);

                
                /*Load the menu*/
                _config.AddItem(
                    new MenuItem("LastHit", "Last hit").SetShared().SetValue(new KeyBind('X', KeyBindType.Press)));

                _config.AddItem(new MenuItem("Farm", "Mixed").SetShared().SetValue(new KeyBind('C', KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("Freeze", "Freeze").SetShared().SetValue(new KeyBind('N', KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("LaneClear", "LaneClear").SetShared().SetValue(new KeyBind('V', KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("Orbwalk", "Combo").SetShared().SetValue(new KeyBind(32, KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("StillCombo", "Combo without moving").SetShared()
                        .SetValue(new KeyBind('N', KeyBindType.Press)));
                _config.Item("StillCombo").ValueChanged +=
                    (sender, args) => { Move = !args.GetNewValue<KeyBind>().Active; };


                Player = ObjectManager.Player;
                Game.OnUpdate += GameOnOnGameUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Instances.Add(this);
            }

            private int FarmDelay
            {
                get { return _config.Item("FarmDelay").GetValue<Slider>().Value; }
            }

            public static bool MissileCheck
            {
                get { return _config.Item("MissileCheck").GetValue<bool>(); }
            }

            public static bool LimitAttackSpeed
            {
                get { return _config.Item("LimitAttackSpeed").GetValue<bool>(); }
            }

            public OrbwalkingMode ActiveMode
            {
                get
                {
                    if (_mode != OrbwalkingMode.None)
                    {
                        return _mode;
                    }

                    if (_config.Item("Orbwalk").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (_config.Item("StillCombo").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (_config.Item("LaneClear").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LaneClear;
                    }

                    if (_config.Item("Farm").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Mixed;
                    }

                    if (_config.Item("Freeze").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Freeze;
                    }

                    if (_config.Item("LastHit").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LastHit;
                    }

                    if (_config.Item(CustomModeName) != null && _config.Item(CustomModeName).GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.CustomMode;
                    }

                    return OrbwalkingMode.None;
                }
                set { _mode = value; }
            }

            public virtual bool InAutoAttackRange(AttackableUnit target)
            {
                return Orbwalking.InAutoAttackRange(target);
            }

            public virtual void RegisterCustomMode(string name, string displayname, uint key)
            {
                CustomModeName = name;
                if (_config.Item(name) == null)
                {
                    _config.AddItem(
                        new MenuItem(name, displayname).SetShared().SetValue(new KeyBind(key, KeyBindType.Press)));
                }
            }

            public void SetAttack(bool b)
            {
                Attack = b;
            }

            public void SetMovement(bool b)
            {
                Move = b;
            }

            public void ForceTarget(Obj_AI_Base target)
            {
                _forcedTarget = target;
            }

            public void SetOrbwalkingPoint(Vector3 point)
            {
                _orbwalkingPoint = point;
            }

            public bool ShouldWait()
            {
                if(Player.Level > 16)
                    return false;
                var attackCalc = (int)(Player.AttackDelay * 1000 * LaneClearWaitTimeMod) + (int)(Player.AttackCastDelay * 1000) + BrainFarmInt + Game.Ping / 2 + 1000 * 500 / (int)GetMyProjectileSpeed() ;
                return
                    MinionListAA.Any( 
                        minion =>HealthPrediction.LaneClearHealthPrediction(minion, attackCalc, FarmDelay) <= Player.GetAutoAttackDamage(minion) * 1.2);
            }

            private bool ShouldWaitUnderTurret(Obj_AI_Minion noneKillableMinion)
            {
                var attackCalc = (int)(Player.AttackDelay * 1000 + (Player.IsMelee ? Player.AttackCastDelay * 1000 : Player.AttackCastDelay * 1000 +
                                               1000 * (Player.AttackRange + 2 * Player.BoundingRadius) / Player.BasicAttack.MissileSpeed));
                return
                    MinionListAA.Any( minion =>
                                (noneKillableMinion != null ? noneKillableMinion.NetworkId != minion.NetworkId : true) &&
                                HealthPrediction.LaneClearHealthPrediction( minion, attackCalc , FarmDelay) <= Player.GetAutoAttackDamage(minion));
            }

            public virtual AttackableUnit GetTarget()
            {
                AttackableUnit result = null;
                var mode = ActiveMode;
                //Forced target
                if (_forcedTarget.IsValidTarget() && InAutoAttackRange(_forcedTarget))
                {
                    return _forcedTarget;
                }

                if ((mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LaneClear) &&
                    !_config.Item("PriorizeFarm").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                    if (target != null && InAutoAttackRange(target))
                    {
                        return target;
                    }
                }

                var MinionList = Cache.GetMinions(Player.Position, 0, MinionTeam.NotAlly);
                List<Obj_AI_Base> minionsFiltered = new List<Obj_AI_Base>();
                List<Obj_AI_Base> wards = new List<Obj_AI_Base>();
                List<Obj_AI_Base> other = new List<Obj_AI_Base>();

                if (!_config.Item("AutoTimeAdjust" , true).GetValue<bool>())
                    BrainFarmInt = -TimeAdjust - 100;

                var firstT = (int)(Player.AttackCastDelay * 1000) + BrainFarmInt + Game.Ping / 2;
                var projectileSpeed = (int)GetMyProjectileSpeed();

                if (mode != OrbwalkingMode.None)
                {
                    foreach (var minion in MinionList.Where(x => x.IsValidTarget() && x.IsHPBarRendered ))
                    {
                        var minionObj = minion as Obj_AI_Minion;

                        if (minion != null)
                        {
                            if (MinionManager.IsMinion(minionObj))
                                minionsFiltered.Add(minion);
                            else if (MinionManager.IsWard(minionObj))
                                wards.Add(minion);
                            else if (minion.CharData.BaseSkinName != "gangplankbarrel" && !minion.Name.ToLower().Contains("sru"))
                                other.Add(minion);
                        }
                    }
                }

                /*Killable Minion*/
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)
                {
                    var LastHitList = minionsFiltered
                        .Where(minion => minion.Team != GameObjectTeam.Neutral)
                            .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Super"))
                                .ThenByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                                    .ThenBy(minion => HealthPrediction.GetHealthPrediction(minion, 1500))
                                     .ThenByDescending(minion => minion.MaxHealth);

                    foreach (var minion in LastHitList)
                    {
                        var t = firstT + 1000 * (int)Math.Max(0, Player.ServerPosition.Distance(minion.ServerPosition)- Player.BoundingRadius) / projectileSpeed;

                        if (mode == OrbwalkingMode.Freeze)
                            t += 200 + Game.Ping / 2;
                        
                            
                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay);
                        var damage = Player.GetAutoAttackDamage(minion, _config.Item("PassiveDmg", true).GetValue<bool>()) + _config.Item("DamageAdjust").GetValue<Slider>().Value;
                        var killable = predHealth <= damage;

                        if (mode == OrbwalkingMode.Freeze)
                        {
                            if (minion.Health < 50 || predHealth <= 50)
                            {
                                return minion;
                            }
                        }
                        else
                        {
                            if (CanAttack())
                            {
                                DelayOnFire = t + Utils.TickCount;
                                DelayOnFireId = minion.NetworkId;
                            }

                            if (predHealth <= 0 )
                            {
                                if (HealthPrediction.GetHealthPrediction(minion, t - 50, FarmDelay) > 0)
                                {
                                    FireOnNonKillableMinion(minion);
                                    return minion;
                                }
                            }
                            else if (killable)
                            {
                                return minion;
                            }
                        }
                    }
                }
                
                if (CanAttack())
                {
                    DelayOnFire = 0;
                }

                /*Champions*/
                if (mode != OrbwalkingMode.LastHit)
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget() && InAutoAttackRange(target))
                    {
                        if(!ObjectManager.Player.UnderTurret(true) || mode == OrbwalkingMode.Combo)
                            return target;
                    }
                }

                /*Jungle minions*/
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed)
                {
                    var jminions = Cache.GetMinions(Player.Position, 0, MinionTeam.Neutral);

                    result = _config.Item("Smallminionsprio").GetValue<bool>()
                        ? jminions.MinOrDefault(mob => mob.MaxHealth)
                        : jminions.MaxOrDefault(mob => mob.MaxHealth);

                    if (result != null)
                    {
                        return result;
                    }
                }

                /*WARDS*/

                if ((mode == OrbwalkingMode.Combo && _config.Item("AttackWardsCombo").GetValue<bool>())
                    || (mode == OrbwalkingMode.Mixed && _config.Item("AttackWardsMixed").GetValue<bool>())
                    || (mode == OrbwalkingMode.LaneClear && _config.Item("AttackWardsLaneClear").GetValue<bool>())
                    || (mode == OrbwalkingMode.LastHit && _config.Item("AttackWardsLastHit").GetValue<bool>())
                    || (mode == OrbwalkingMode.Freeze && _config.Item("AttackWardsFreeze").GetValue<bool>())
                    )
                {
                    var obj = wards.FirstOrDefault();
                    if (obj != null)
                        return obj;
                }

                /*PETS*/

                if ((mode == OrbwalkingMode.Combo && _config.Item("AttackPetsnTrapsCombo").GetValue<bool>())
                    || (mode == OrbwalkingMode.Mixed && _config.Item("AttackPetsnTrapsMixed").GetValue<bool>())
                    || (mode == OrbwalkingMode.LaneClear && _config.Item("AttackPetsnTrapsLaneClear").GetValue<bool>())
                    || (mode == OrbwalkingMode.LastHit && _config.Item("AttackPetsnTrapsLastHit").GetValue<bool>())
                    || (mode == OrbwalkingMode.Freeze && _config.Item("AttackPetsnTrapsFreeze").GetValue<bool>())
                    )
                {
                    if (_config.Item("AttackBarrel").GetValue<bool>())
                    {
                        var enemyGangPlank = HeroManager.Enemies.FirstOrDefault(e => e.ChampionName.Equals("gangplank", StringComparison.InvariantCultureIgnoreCase));

                        if (enemyGangPlank != null)
                        {
                            var barrels = Cache.GetMinions(Player.Position, 0, MinionTeam.NotAlly).Where(minion => minion.Team == GameObjectTeam.Neutral && minion.CharData.BaseSkinName == "gangplankbarrel" && minion.IsHPBarRendered && minion.IsValidTarget() && InAutoAttackRange(minion));

                            foreach (var barrel in barrels)
                            {
                                if (barrel.Health <= 1f)
                                    return barrel;

                                var t = (int)(Player.AttackCastDelay * 1000) + Game.Ping / 2 + 1000 * (int)Math.Max(0, Player.Distance(barrel) - Player.BoundingRadius) / projectileSpeed;

                                var barrelBuff = barrel.Buffs.FirstOrDefault(b => b.Name.Equals("gangplankebarrelactive", StringComparison.InvariantCultureIgnoreCase));

                                if (barrelBuff != null && barrel.Health <= 2f)
                                {
                                    var healthDecayRate = enemyGangPlank.Level >= 13 ? 0.5f : (enemyGangPlank.Level >= 7 ? 1f : 2f);
                                    var nextHealthDecayTime = Game.Time < barrelBuff.StartTime + healthDecayRate ? barrelBuff.StartTime + healthDecayRate : barrelBuff.StartTime + healthDecayRate * 2;

                                    if (nextHealthDecayTime <= Game.Time + t / 1000f && ObjectManager.Get<Obj_GeneralParticleEmitter>().Any(x => x.Name == "Gangplank_Base_E_AoE_Red.troy" && barrel.Distance(x.Position) < 10))
                                        return barrel;
                                }
                            }
                            if (barrels.Any())
                                return null;
                        }
                    }

                    var obj = other.FirstOrDefault();
                    if (obj != null)
                        return obj;
                }

                /* turrets / inhibitors / nexus */
                if ((mode == OrbwalkingMode.Combo && _config.Item("BuildingsCombo").GetValue<bool>())
                     || (mode == OrbwalkingMode.Mixed && _config.Item("BuildingsMixed").GetValue<bool>())
                     || (mode == OrbwalkingMode.LaneClear && _config.Item("BuildingsLaneClear").GetValue<bool>())
                     || (mode == OrbwalkingMode.LastHit && _config.Item("BuildingsLastHit").GetValue<bool>())
                     || (mode == OrbwalkingMode.Freeze && _config.Item("BuildingsFreeze").GetValue<bool>())
                     )
                {
                    /* turrets */
                    foreach (var turret in
                        Cache.TurretList.Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    /* inhibitor */
                    foreach (var turret in
                        Cache.InhiList.Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    /* nexus */
                    foreach (var nexus in
                        Cache.NexusList.Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return nexus;
                    }
                }


                /* UnderTurret Farming */
                if ((mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit ||
                    mode == OrbwalkingMode.Freeze) && CanAttack() && Player.Level < 17)
                {
                    var closestTower =
                        ObjectManager.Get<Obj_AI_Turret>().MinOrDefault(t => t.IsAlly && !t.IsDead ? Player.Distance(t, true) : float.MaxValue);

                    if (closestTower != null && Player.Distance(closestTower, true) < 1500 * 1500)
                    {
                        Obj_AI_Minion farmUnderTurretMinion = null;
                        Obj_AI_Minion noneKillableMinion = null;
                        // return all the minions underturret in auto attack range
                        var minions = MinionListAA.Where(minion => 
                            closestTower.Distance(minion, true) < 900 * 900)
                            .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                            .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                            .ThenByDescending(minion => minion.MaxHealth)
                            .ThenByDescending(minion => minion.Health);

                        if (minions.Any())
                        {
                            // get the turret aggro minion
                            var turretMinion =
                                minions.FirstOrDefault(
                                    minion =>
                                        minion is Obj_AI_Minion &&
                                        HealthPrediction.HasTurretAggro(minion as Obj_AI_Minion));

                            if (turretMinion != null)
                            {
                                var hpLeftBeforeDie = 0;
                                var hpLeft = 0;
                                var turretAttackCount = 0;
                                var turretStarTick = HealthPrediction.TurretAggroStartTick(
                                    turretMinion as Obj_AI_Minion);
                                // from healthprediction (don't blame me :S)
                                var turretLandTick = turretStarTick + (int)(closestTower.AttackCastDelay * 1000) +
                                                     1000 *
                                                     Math.Max(
                                                         0,
                                                         (int)
                                                             (turretMinion.Distance(closestTower) -
                                                              closestTower.BoundingRadius)) /
                                                     (int)(closestTower.BasicAttack.MissileSpeed + 70);
                                // calculate the HP before try to balance it
                                for (float i = turretLandTick + 50;
                                    i < turretLandTick + 10 * closestTower.AttackDelay * 1000 + 50;
                                    i = i + closestTower.AttackDelay * 1000)
                                {
                                    var time = (int)i - Utils.GameTimeTickCount + Game.Ping / 2;
                                    var predHP =
                                        (int)
                                            HealthPrediction.LaneClearHealthPrediction(
                                                turretMinion, time > 0 ? time : 0);
                                    if (predHP > 0)
                                    {
                                        hpLeft = predHP;
                                        turretAttackCount += 1;
                                        continue;
                                    }
                                    hpLeftBeforeDie = hpLeft;
                                    hpLeft = 0;
                                    break;
                                }
                                // calculate the hits is needed and possibilty to balance
                                if (hpLeft == 0 && turretAttackCount != 0 && hpLeftBeforeDie != 0)
                                {
                                    var damage = (int)Player.GetAutoAttackDamage(turretMinion, true);
                                    var hits = hpLeftBeforeDie / damage;
                                    var timeBeforeDie = turretLandTick +
                                                        (turretAttackCount + 1) *
                                                        (int)(closestTower.AttackDelay * 1000) -
                                                        Utils.GameTimeTickCount;
                                    var timeUntilAttackReady = LastAATick + (int)(Player.AttackDelay * 1000) >
                                                               Utils.GameTimeTickCount + Game.Ping / 2 + 25
                                        ? LastAATick + (int)(Player.AttackDelay * 1000) -
                                          (Utils.GameTimeTickCount + Game.Ping / 2 + 25)
                                        : 0;
                                    var timeToLandAttack = Player.IsMelee
                                        ? Player.AttackCastDelay * 1000
                                        : Player.AttackCastDelay * 1000 +
                                          1000 * Math.Max(0, turretMinion.Distance(Player) - Player.BoundingRadius) /
                                          Player.BasicAttack.MissileSpeed;
                                    if (hits >= 1 &&
                                        hits * Player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack <
                                        timeBeforeDie)
                                    {
                                        farmUnderTurretMinion = turretMinion as Obj_AI_Minion;
                                    }
                                    else if (hits >= 1 &&
                                             hits * Player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack >
                                             timeBeforeDie)
                                    {
                                        noneKillableMinion = turretMinion as Obj_AI_Minion;
                                    }
                                }
                                else if (hpLeft == 0 && turretAttackCount == 0 && hpLeftBeforeDie == 0)
                                {
                                    noneKillableMinion = turretMinion as Obj_AI_Minion;
                                }
                                // should wait before attacking a minion.
                                if (ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }
                                if (farmUnderTurretMinion != null)
                                {
                                    return farmUnderTurretMinion;
                                }
                                // balance other minions
                                foreach (var minion in
                                    minions.Where(
                                        x =>
                                            x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion &&
                                            !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion)))
                                {
                                    var playerDamage = (int)Player.GetAutoAttackDamage(minion);
                                    var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                    var leftHP = (int)minion.Health % turretDamage;
                                    if (leftHP > playerDamage)
                                    {
                                        return minion;
                                    }
                                }
                                // late game
                                var lastminion =
                                    minions.LastOrDefault(x => x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion &&
                                            !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));
                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (1f / Player.AttackDelay >= 1f &&
                                        (int)(turretAttackCount * closestTower.AttackDelay / Player.AttackDelay) *
                                        Player.GetAutoAttackDamage(lastminion) > lastminion.Health)
                                    {
                                        return lastminion;
                                    }
                                    if (minions.Count() >= 5 && 1f / Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            else
                            {
                                if (ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }
                                // balance other minions
                                foreach (var minion in
                                    minions.Where(
                                        x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion))
                                    )
                                {
                                    if (closestTower != null)
                                    {
                                        var playerDamage = (int)Player.GetAutoAttackDamage(minion);
                                        var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                        var leftHP = (int)minion.Health % turretDamage;
                                        if (leftHP > playerDamage)
                                        {
                                            return minion;
                                        }
                                    }
                                }
                                //late game
                                var lastminion =
                                    minions
                                        .LastOrDefault(x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));
                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (minions.Count() >= 5 && 1f / Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            return null;
                        }
                    }
                }

                /*Lane Clear minions*/
                if (mode == OrbwalkingMode.LaneClear)
                {
                    if (!ShouldWait())
                    {
                        var firstT2 = (int)(Player.AttackDelay * 1000 * LaneClearWaitTimeMod) + (int)(Player.AttackCastDelay * 1000) + BrainFarmInt + Game.Ping / 2;
                        foreach (var minion in minionsFiltered.OrderBy(minion => minion.Health))
                        {
                            var t = firstT2 + 1000 * (int)Math.Max(0, Player.ServerPosition.Distance(minion.ServerPosition) - Player.BoundingRadius) / projectileSpeed;

                            var predHealth = HealthPrediction.LaneClearHealthPrediction(minion, t, FarmDelay);
                            if(Math.Abs(predHealth - minion.Health) < float.Epsilon)
                                return minion;
                            var damage = Player.GetAutoAttackDamage(minion);
                            if (predHealth >= (2 - 0.01 * _config.Item("LaneClearSpeed").GetValue<Slider>().Value) * damage)
                                return minion;
                        }
                    }

                }
                return result;
            }

            /// <summary>
            ///     Fired when the game is updated.
            /// </summary>
            /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
            private void GameOnOnGameUpdate(EventArgs args)
            {
                try
                {
                    TimeAdjust = _config.Item("TimeAdjust").GetValue<Slider>().Value;
                    if (ActiveMode == OrbwalkingMode.None)
                    {
                        return;
                    }

                    //Prevent canceling important spells
                    if (Player.IsCastingInterruptableSpell(true))
                    {
                        return;
                    }
                    MinionListAA = Cache.GetMinions(Player.Position, 0, MinionTeam.NotAlly);
                    var target = GetTarget();
                     
                    Orbwalk(
                        target, _orbwalkingPoint.To2D().IsValid() ? _orbwalkingPoint : Game.CursorPos,
                        _config.Item("ExtraWindup").GetValue<Slider>().Value,
                        Math.Max(_config.Item("HoldPosRadius").GetValue<Slider>().Value, 30));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private void DrawingOnOnDraw(EventArgs args)
            {
                if (_config.Item("AACircle").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(
                        Player.Position, GetRealAutoAttackRange(null) + 65,
                        _config.Item("AACircle").GetValue<Circle>().Color,
                        _config.Item("AALineWidth").GetValue<Slider>().Value);
                }
                if (_config.Item("AACircle2").GetValue<Circle>().Active)
                {
                    foreach (var target in
                        HeroManager.Enemies.FindAll(target => target.IsValidTarget(1175)))
                    {
                        Render.Circle.DrawCircle(
                            target.Position, GetAttackRange(target), _config.Item("AACircle2").GetValue<Circle>().Color,
                            _config.Item("AALineWidth").GetValue<Slider>().Value);
                    }
                }

                if (_config.Item("HoldZone").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(
                        Player.Position, _config.Item("HoldPosRadius").GetValue<Slider>().Value,
                        _config.Item("HoldZone").GetValue<Circle>().Color,
                        _config.Item("AALineWidth").GetValue<Slider>().Value, true);
                }

                if (_config.Item("LastHitHelper").GetValue<bool>())
                {
                    foreach (var minion in
                        Cache.MinionsListEnemy
                            .Where(
                                x => x.Name.ToLower().Contains("minion") && x.IsHPBarRendered && x.IsValidTarget(1000)))
                    {
                        if (minion.Health < ObjectManager.Player.GetAutoAttackDamage(minion, true))
                        {
                            Render.Circle.DrawCircle(minion.Position, 50, Color.LimeGreen);
                        }
                    }
                }
            }
        }
    }
}
