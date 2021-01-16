namespace LeagueSharp.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SharpDX;
    using Color = System.Drawing.Color;

    class YasuoWall
    {
        public Vector3 YasuoPosition { get; set; }
        public float CastTime { get; set; }
        public Vector3 CastPosition { get; set; }
        public float WallLvl { get; set; }
        
        public YasuoWall()
        {
            CastTime = 0;
        }
    }

    public static class Orbwalking
    {
        public static int[] LastTargets = new int[] { 0, 0, 0 };
        public static bool Attack = true;
        public static bool DisableNextAttack;
        private static int _ApheliosChakramAATick;
        private static int _LastAATick;
        private static YasuoWall yasuoWall = new YasuoWall();
        public static bool YasuoInGame = false;
        public static int LastAATick
        {
            get
            {
                if (Player.ChampionName == "Aphelios")
                {
                    if (Player.HasBuff("ApheliosCrescendumManager"))
                        return _ApheliosChakramAATick;
                }

                return _LastAATick;
            }

            set
            {
                if (Player.ChampionName == "Aphelios")
                {
                    if (Player.HasBuff("ApheliosCrescendumManager"))
                    {
                        _ApheliosChakramAATick = value;

                        if (_ApheliosChakramAATick == 0)
                            return;
                    }
                }
                _LastAATick = value;
            }
        }

        public static float _sennaAttackCastDelay;

        public static float AttackCastDelay
        {
            get
            {
                if (_championName == "Senna")
                {
                    return _sennaAttackCastDelay;
                }

                return Player.AttackCastDelay;
            }
        }

        public static int LastAttackCommandT;
        public static Vector3 LastMoveCommandPosition = Vector3.Zero;
        public static int LastMoveCommandT;
        public static bool Move = true;
        private static readonly string _championName;
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);
        private static readonly string[] AttackResets =
            {
                "dariusnoxiantacticsonh", "fiorae", "garenq", "gravesmove",
                "jaxempowertwo", "jaycehypercharge",
                "leonashieldofdaybreak", "luciane", "monkeykingdoubleattack",
                "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze",
                "netherblade", "gangplankqwrapper", "powerfist",
                "renektonpreexecute", "rengarq", "shyvanadoubleattack",
                "sivirw", "takedown", "talonnoxiandiplomacy",
                "trundletrollsmash", "vaynetumble", "vie", "volibearq",
                "xenzhaocombotarget", "yorickspectral", "reksaiq",
                "itemtitanichydracleave", "masochism", "illaoiw",
                "elisespiderw", "fiorae", "meditate", "sejuaninorthernwinds",
                "asheq", "camilleq", "camilleq2"
            };

        private static readonly string[] Attacks =
            {
                "caitlynheadshotmissile", "frostarrow", "garenslash2",
                "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced",
                "renektonexecute", "renektonsuperexecute",
                "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust",
                "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff",
                "lucianpassiveshot"
            };

        private static readonly string[] NoAttacks =
            {
                "volleyattack", "volleyattackwithsound",
                "jarvanivcataclysmattack", "monkeykingdoubleattack",
                "shyvanadoubleattack", "shyvanadoubleattackdragon",
                "zyragraspingplantattack", "zyragraspingplantattack2",
                "zyragraspingplantattackfire", "zyragraspingplantattack2fire",
                "viktorpowertransfer", "sivirwattackbounce", "asheqattacknoonhit",
                "elisespiderlingbasicattack", "heimertyellowbasicattack",
                "heimertyellowbasicattack2", "heimertbluebasicattack",
                "annietibbersbasicattack", "annietibbersbasicattack2",
                "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack",
                "yorickspectralghoulbasicattack", "malzaharvoidlingbasicattack",
                "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
                "kindredwolfbasicattack", "gravesautoattackrecoil"
            };

        private static readonly string[] NoCancelChamps = { "Kalista" };
        private static readonly Obj_AI_Hero Player;
        private static int _autoattackCounter;
        private static int _delay;
        private static AttackableUnit _lastTarget;
        private static float _minDistance = 100;

        static Orbwalking()
        {
            Player = ObjectManager.Player;
            _championName = Player.ChampionName;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnDoCast += Obj_AI_Base_OnDoCast;
            Spellbook.OnStopCast += SpellbookOnStopCast;

            if (Player.ChampionName == "Aphelios")
            {
                GameObject.OnDelete += GameObject_OnDelete;
            }
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy && hero.ChampionName == "Yasuo")
                    YasuoInGame = true;
            }

            /*
            if (_championName == "Rengar")
            {
                Obj_AI_Base.OnPlayAnimation += delegate(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
                    {
                        if (sender.IsMe && args.Animation == "Spell5")
                        {
                            var t = 0;

                            if (_lastTarget != null && _lastTarget.IsValid)
                            {
                                t += (int)Math.Min(ObjectManager.Player.Distance(_lastTarget) / 1.5f, 0.6f);
                            }

                            LastAATick = Utils.GameTimeTickCount - Game.Ping / 2 + t;
                        }
                    };
            }*/
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null && missile.Name == "ApheliosCrescendumAttackMisIn")
            {
                if (missile.SpellCaster != null && missile.SpellCaster.IsMe)
                    ResetAutoAttackTimer();
            }
        }

        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);
        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
        public delegate void OnAttackEvenH(AttackableUnit unit, AttackableUnit target);
        public delegate void OnNonKillableMinionH(AttackableUnit minion);
        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);

        public static event AfterAttackEvenH AfterAttack;
        public static event BeforeAttackEvenH BeforeAttack;
        public static event OnAttackEvenH OnAttack;
        public static event OnNonKillableMinionH OnNonKillableMinion;
        public static event OnTargetChangeH OnTargetChange;
        private static IDictionary<Vector3, float> minionsWithBounding = new Dictionary<Vector3, float>();
        public enum OrbwalkingMode
        {
            LastHit,
            Mixed,
            LaneClear,
            Combo,
            Freeze,
            CustomMode,
            None
        }

        public static Vector3[] GetPath(Vector3 end)
        {

            List<Vector3> path = new List<Vector3>();

            var playerPosition = Player.ServerPosition;
            var playerBoundingRadius = Player.BoundingRadius;
            path.Add(playerPosition);
            int step = 50;
            int lastWaypointIndex = 0;

            for (int i = 1; i * step <= playerPosition.Distance(end); i++)
            {
                var point = path[lastWaypointIndex].Extend(end, i * step);

                foreach (var minion in minionsWithBounding)
                {
                    var minionRange = playerBoundingRadius + minion.Value;
                    if (point.Distance(minion.Key) < minionRange)
                    {
                        path.Add(Geometry.CirclePoints(30, minionRange, point).Where(x => x.Distance(minion.Key) > minionRange).OrderBy(x => x.Distance(end)).FirstOrDefault());

                        lastWaypointIndex++;
                        break;
                    }
                }

                if (point.IsWall())
                {
                    if (playerPosition.Distance(point) < 150)
                    {
                        var pointToWall = point.Extend(path[lastWaypointIndex], playerBoundingRadius);
                        path.Add(Geometry.CirclePoints(8, 150, pointToWall)
                            .Where(x => !x.IsWall())
                            .OrderBy(x => x.Distance(end)).FirstOrDefault());
                    }
                    else
                    {
                        path.Add(point);
                    }

                    return path.ToArray();
                }

            }
            path.Add(end);
            return path.ToArray();
        }

        public static bool CanAttack()
        {
            if (Player.IsCastingInterruptableSpell())
                return false;


            if (!Player.CanAttack)
            {
                if (_championName == "Aphelios" || Player.Spellbook.IsChanneling)
                    return false;
            }

            foreach (var buff in Player.Buffs)
            {
                if (buff.Type == BuffType.Disarm || buff.Type == BuffType.Blind && Player.CharData.BaseSkinName != "Kalista")
                    return false;

                if (_championName == "Kayle" && buff.Name == "KayleR")
                    return false;

                if (_championName == "Samira")
                {
                    switch (buff.Name)
                    {
                        case "SamiraW":
                        case "SamiraR":
                            return false;
                    }
                }

                switch (buff.Name)
                {
                    case "JhinPassiveReload":
                    case "XayahR":
                    case "KaisaE":
                        return false;
                }
            }

            if (Player.ChampionName == "Graves")
            {
                var attackDelay = 1.0740296828d * 1000 * Player.AttackDelay - 716.2381256175d;
                if (Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + attackDelay && Player.HasBuff("GravesBasicAttackAmmo1"))
                    return true;

                return false;
            }

            if (!Player.IsDashing())
            {
                if (!Player.CanAttack)
                    return false;
                //if (Player.Spellbook.IsCastingSpell)
                //    return false;
            }

            return Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + Player.AttackDelay * 1000;
        }

        public static bool CanMove(float extraWindup, bool disableMissileCheck = false)
        {
            var localExtraWindup = 0;
            if (_championName == "Rengar" && (Player.HasBuff("rengarqbase") || Player.HasBuff("rengarqemp")))
                localExtraWindup = 200;

            return NoCancelChamps.Contains(_championName) || (Utils.GameTimeTickCount + Game.Ping / 2
                       >= LastAATick + AttackCastDelay * 1000 - 120 + extraWindup + localExtraWindup);
        }

        public static float GetAttackRange(Obj_AI_Hero target)
        {
            var result = target.AttackRange + target.BoundingRadius;
            return result;
        }

        public static Vector3 GetLastMovePosition()
        {
            return LastMoveCommandPosition;
        }

        public static float GetLastMoveTime()
        {
            return LastMoveCommandT;
        }

        public static float GetMyProjectileSpeed()
        {
            if (_championName == "Aphelios")
            {
                foreach (var buff in Player.Buffs)
                {
                    switch (buff.Name)
                    {
                        case "ApheliosCalibrumManager":
                            return 2500;

                        case "ApheliosSeverumManager":
                            return float.MaxValue;

                        case "ApheliosGravitumManager":
                            return 1500;

                        case "ApheliosInfernumManager":
                            return 1500;

                        case "ApheliosCrescendumManager":
                            return 4000;
                    }
                }

                return 1500;
            }

            if (_championName == "Kayle")
            {
                if (Player.AttackRange < 525)
                    return float.MaxValue;

                return 2250;
            }

            return IsMelee(Player) || _championName == "Azir" || _championName == "Velkoz" || _championName == "Senna"
                   || _championName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn")
                       ? float.MaxValue
                       : Player.BasicAttack.MissileSpeed;
        }



        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = Player.AttackRange + Player.BoundingRadius;
            if (target.IsValidTarget())
            {
                var aiBase = target as Obj_AI_Base;
                if (aiBase != null)
                {
                    if (Player.ChampionName == "Caitlyn" && aiBase.HasBuff("caitlynyordletrapinternal"))
                        return 1250;

                    if (Player.ChampionName == "Aphelios" && aiBase.HasBuff("aphelioscalibrumbonusrangedebuff"))
                        return 1800;

                    result -= Math.Min((Game.Ping - 40) / 3f, 10f);
                    result -= 11;
                    if (Player.IsMoving && aiBase.IsMoving)
                    {
                        if (!aiBase.IsFacing(Player))
                            result -= 8;
                        if (Player.IsFacing(aiBase))
                            result -= 8;
                    }
                }
                return result + target.BoundingRadius;
            }
            return result;
        }

        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.IsValidTarget())
                return false;

            var myRange = GetRealAutoAttackRange(target);
            return
                Vector2.DistanceSquared(
                    target is Obj_AI_Base ? ((Obj_AI_Base)target).ServerPosition.To2D() : target.Position.To2D(),
                    Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) || Attacks.Contains(name.ToLower());
        }

        public static bool CollisionYasuo(Vector3 from, Vector3 to)
        {
            if (!YasuoInGame)
                return false;

            if (Game.Time - yasuoWall.CastTime > 4)
                return false;

            var level = yasuoWall.WallLvl;
            var wallWidth = (350 + 50 * level);
            var wallDirection = (yasuoWall.CastPosition.To2D() - yasuoWall.YasuoPosition.To2D()).Normalized().Perpendicular();
            var wallStart = yasuoWall.CastPosition.To2D() + wallWidth / 2f * wallDirection;
            var wallEnd = wallStart - wallWidth * wallDirection;

            if (wallStart.Intersection(wallEnd, to.To2D(), from.To2D()).Intersects)
            {
                return true;
            }
            return false;
        }

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static bool IsMelee(this Obj_AI_Base unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        public static void MoveTo(Vector3 position, float holdAreaRadius = 0, bool overrideTimer = false, bool useFixedDistance = true, bool randomizeMinDistance = true)
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

            if (Player.Distance(point, true) > 500 * 500)
                point = playerPosition.Extend(position, randomizeMinDistance ? (_random.NextFloat(0.6f, 1)) * 800 : 800);

            var angle = 0f;
            var currentPath = Player.GetWaypoints();
            var movePath = GetPath(point);

            if (currentPath.Count > 1 && currentPath.PathLength() > 100)
            {
                if (movePath.Length > 1)
                {
                    var v1 = currentPath[1] - currentPath[0];
                    var v2 = movePath[1] - movePath[0];
                    angle = v1.AngleBetween(v2.To2D());
                    var distance = movePath.Last().To2D().Distance(currentPath.Last(), true);

                    if (angle < 10 || distance < 100 * 100)
                        return;
                }
            }

            if (Utils.GameTimeTickCount - LastMoveCommandT < 70 + Math.Min(60, Game.Ping) && !overrideTimer && angle < 60)
                return;

            if (angle >= 60 && Utils.GameTimeTickCount - LastMoveCommandT < 60)
                return;

            Player.ForceIssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = movePath[1];
            LastMoveCommandT = Utils.GameTimeTickCount;
        }

        public static void Orbwalk(AttackableUnit target, Vector3 position, float extraWindup = 90, float holdAreaRadius = 0, bool useFixedDistance = true, bool randomizeMinDistance = true)
        {
            if (Utils.GameTimeTickCount - LastAttackCommandT < 70 + Math.Min(60, Game.Ping))
            {
                return;
            }

            try
            {
                if (target.IsValidTarget()  && Attack)
                {
                    if (CanAttack())
                    {
                        DisableNextAttack = false;
                        FireBeforeAttack(target);

                        if (!DisableNextAttack)
                        {
                            if (Player.ForceIssueOrder(GameObjectOrder.AttackUnit, target))
                            {
                                LastAttackCommandT = Utils.GameTimeTickCount;
                                _lastTarget = target;
                            }
                            return;
                        }
                    }
                    else if (Player.ChampionName == "Caitlyn" && target.Type == GameObjectType.obj_AI_Hero)
                    {
                        var targetHero = (Obj_AI_Hero)target;
                        if (targetHero != null && targetHero.HasBuff("caitlynyordletrapinternal") && Player.ForceIssueOrder(GameObjectOrder.AttackTo, Game.CursorPos))
                        {
                            LastAttackCommandT = Utils.GameTimeTickCount;
                            _lastTarget = target;
                        }
                    }
                }

                if (CanMove(extraWindup) && Move)
                {
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

        public static void SetMinimumOrbwalkDistance(float d)
        {
            _minDistance = d;
        }

        public static void SetMovementDelay(int delay)
        {
            _delay = delay;
        }

        private static void PushLastTargets(int networkId)
        {
            LastTargets[2] = LastTargets[1];
            LastTargets[1] = LastTargets[0];
            LastTargets[0] = networkId;
        }

        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            {
                AfterAttack(unit, target);
            }
        }

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

        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            if (OnNonKillableMinion != null)
            {
                OnNonKillableMinion(minion);
            }
        }

        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!_lastTarget.IsValidTarget() || _lastTarget != newTarget))
            {
                OnTargetChange(_lastTarget, newTarget);
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
            }
        }

        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs Spell)
        {
            try
            {
                if (unit.IsMe)
                {
                    var spellName = Spell.SData.Name;

                    if (IsAutoAttackReset(spellName) && Spell.SData.SpellCastTime == 0)
                        ResetAutoAttackTimer();

                    if (!IsAutoAttack(spellName))
                        return;

                    if (_championName == "Senna")
                    {
                        _sennaAttackCastDelay = Spell.CastDelay;
                    }

                    if (Spell.Target is Obj_AI_Base || Spell.Target is Obj_BarracksDampener || Spell.Target is Obj_HQ)
                    {
                        PushLastTargets(Spell.Target.NetworkId);

                        LastAATick = Utils.GameTimeTickCount - Game.Ping / 2;

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
                }
                FireOnAttack(unit, _lastTarget);

                if (Spell.SData == null)
                {
                    return;
                }

                var targed = Spell.Target as Obj_AI_Base;

                if (targed == null)
                {
                    if (!YasuoInGame)
                        return;

                    if (!YasuoInGame || !unit.IsEnemy || unit.IsMinion || Spell.SData.IsAutoAttack() || unit.Type != GameObjectType.obj_AI_Hero)
                        return;

                    if (Spell.SData.Name.Contains("YasuoW"))
                    {
                        yasuoWall.CastTime = Game.Time;
                        yasuoWall.CastPosition = unit.Position.Extend(Spell.End, 400);
                        yasuoWall.YasuoPosition = unit.Position;
                        yasuoWall.WallLvl = unit.Spellbook.Spells[1].Level;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SpellbookOnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && spellbook.Owner.IsMe && args.DestroyMissile && args.StopAnimation )
            {
                ResetAutoAttackTimer();
                Console.WriteLine("Basic Attack was cancelled!");
            }
        }

        public class BeforeAttackEventArgs : EventArgs
        {
            public AttackableUnit Target;
            public Obj_AI_Base Unit = ObjectManager.Player;
            private bool _process = true;
            public bool Process
            {
                get
                {
                    return this._process;
                }
                set
                {
                    DisableNextAttack = !value;
                    this._process = value;
                }
            }
        }

        public class Orbwalker : IDisposable
        {
            private const float LaneClearWaitTimeMod = 2.0f;
            public static List<Orbwalker> Instances = new List<Orbwalker>();
            private static Menu _config;
            private readonly Obj_AI_Hero Player;
            private Obj_AI_Base _forcedTarget;
            private OrbwalkingMode _mode = OrbwalkingMode.None;
            private Vector3 _orbwalkingPoint;
            private Obj_AI_Minion _prevMinion;
            private string CustomModeName;

            public Orbwalker(Menu attachToMenu)
            {
                _config = attachToMenu;
                /* Drawings submenu */
                var drawings = new Menu("Drawings", "drawings");
                drawings.AddItem(new MenuItem("AACircle", "AACircle").SetShared().SetValue(new Circle(true, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(new MenuItem("AACircle2", "Enemy AA circle").SetShared().SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(new MenuItem("HoldZone", "HoldZone").SetShared().SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(new MenuItem("AALineWidth", "Line Width")).SetShared().SetValue(new Slider(2, 1, 6));
                drawings.AddItem(new MenuItem("LastHitHelper", "Last Hit Helper").SetShared().SetValue(false));
                _config.AddSubMenu(drawings);

                /* Misc options */
                var misc = new Menu("Misc", "Misc");
                misc.AddItem(new MenuItem("HoldPosRadius", "Hold Position Radius").SetShared().SetValue(new Slider(0, 50, 250)));
                misc.AddItem(new MenuItem("PriorizeFarm", "Prioritize farm over harass").SetShared().SetValue(true));
                misc.AddItem(new MenuItem("PrioritizeCasters", "Attack caster minions first").SetShared().SetValue(false));
                misc.AddItem(new MenuItem("AttackWards", "Auto attack wards").SetShared().SetValue(false));
                misc.AddItem(new MenuItem("AttackPetsnTraps", "Auto attack pets & traps").SetShared().SetValue(true));
                misc.AddItem(new MenuItem("AttackGPBarrel", "Auto attack gangplank barrel").SetShared().SetValue(new StringList(new[] { "Combo and Farming", "Farming", "No" }, 1)));
                misc.AddItem(new MenuItem("Smallminionsprio", "Jungle clear small first").SetShared().SetValue(false));
                misc.AddItem(new MenuItem("FocusMinionsOverTurrets", "Focus minions over objectives").SetShared().SetValue(new KeyBind('M', KeyBindType.Toggle)));
                _config.AddSubMenu(misc);

                /* Delay sliders */
                _config.AddItem(new MenuItem("ExtraWindup", "Extra windup time").SetShared().SetValue(new Slider(80, 0, 200)));
                _config.AddItem(new MenuItem("FarmDelay", "Farm delay").SetShared().SetValue(new Slider(0, 0, 200)));

                /*Load the menu*/
                _config.AddItem(new MenuItem("LastHit", "Last hit").SetShared().SetValue(new KeyBind('X', KeyBindType.Press)));
                _config.AddItem(new MenuItem("Farm", "Mixed").SetShared().SetValue(new KeyBind('C', KeyBindType.Press)));
                _config.AddItem(new MenuItem("Freeze", "Freeze").SetShared().SetValue(new KeyBind('N', KeyBindType.Press)));
                _config.AddItem(new MenuItem("LaneClear", "LaneClear").SetShared().SetValue(new KeyBind('V', KeyBindType.Press)));
                _config.AddItem(new MenuItem("Orbwalk", "Combo").SetShared().SetValue(new KeyBind(32, KeyBindType.Press)));
                _config.AddItem(new MenuItem("StillCombo", "Combo without moving").SetShared().SetValue(new KeyBind('N', KeyBindType.Press)));
                _config.Item("StillCombo").ValueChanged += (sender, args) => { Move = !args.GetNewValue<KeyBind>().Active; };

                Player = ObjectManager.Player;
                Game.OnUpdate += this.GameOnOnGameUpdate;
                Drawing.OnDraw += this.DrawingOnOnDraw;
                Instances.Add(this);
            }

            public OrbwalkingMode ActiveMode
            {
                get
                {
                    if (this._mode != OrbwalkingMode.None)
                    {
                        return this._mode;
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

                    if (_config.Item(this.CustomModeName) != null && _config.Item(this.CustomModeName).GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.CustomMode;
                    }

                    return OrbwalkingMode.None;
                }
                set
                {
                    this._mode = value;
                }
            }

            private int FarmDelay
            {
                get
                {
                    return _config.Item("FarmDelay").GetValue<Slider>().Value;
                }
            }

            public void Dispose()
            {
                Menu.Remove(_config);
                Game.OnUpdate -= this.GameOnOnGameUpdate;
                Drawing.OnDraw -= this.DrawingOnOnDraw;
                Instances.Remove(this);
            }

            public void ForceTarget(Obj_AI_Base target)
            {
                this._forcedTarget = target;
            }

            public virtual AttackableUnit GetTarget()
            {
                AttackableUnit result = null;
                var mode = this.ActiveMode;

                //Forced target
                if (this._forcedTarget.IsValidTarget() && this.InAutoAttackRange(this._forcedTarget))
                    return this._forcedTarget;

                /*Champions*/
                if (mode == OrbwalkingMode.Combo
                    || (!_config.Item("PriorizeFarm").GetValue<bool>() && mode == OrbwalkingMode.Mixed)
                    || (!Player.UnderTurret(true) && mode == OrbwalkingMode.LaneClear))
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                    if (target != null && this.InAutoAttackRange(target))
                        return target;
                }
                if (mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LaneClear)
                {
                    foreach (var nexus in ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                        return nexus;

                    if (_config.Item("AttackWards").IsActive())
                    {
                        var wardToAttack = ObjectManager.Get<Obj_AI_Minion>().Where(ward => ward.IsValidTarget() && InAutoAttackRange(ward) && ward.IsEnemy && MinionManager.IsWard(ward)
                                  && ward.CharData.BaseSkinName != "gangplankbarrel").FirstOrDefault();

                        if (wardToAttack != null)
                            return wardToAttack;
                    }
                }

                //GankPlank barrels
                var attackGankPlankBarrels = _config.Item("AttackGPBarrel").GetValue<StringList>().SelectedIndex;
                if (attackGankPlankBarrels != 2 && (attackGankPlankBarrels == 0
                        || (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed
                            || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)))
                {
                    var enemyGangPlank = HeroManager.Enemies.FirstOrDefault(e => e.ChampionName.Equals("gangplank", StringComparison.InvariantCultureIgnoreCase));

                    if (enemyGangPlank != null)
                    {
                        var barrels =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    minion =>
                                    minion.Team == GameObjectTeam.Neutral
                                    && minion.CharData.BaseSkinName == "gangplankbarrel" && minion.IsHPBarRendered
                                    && minion.IsValidTarget() && this.InAutoAttackRange(minion));

                        foreach (var barrel in barrels)
                        {
                            if (barrel.Health <= 1f)
                                return barrel;

                            var t = (int)(AttackCastDelay * 1000) + Game.Ping / 2
                                    + 1000 * (int)Math.Max(0, this.Player.Distance(barrel) - this.Player.BoundingRadius)
                                    / (int)GetMyProjectileSpeed();

                            var barrelBuff =
                                barrel.Buffs.FirstOrDefault(
                                    b =>
                                    b.Name.Equals("gangplankebarrelactive", StringComparison.InvariantCultureIgnoreCase));

                            if (barrelBuff != null && barrel.Health <= 2f)
                            {
                                var healthDecayRate = enemyGangPlank.Level >= 13 ? 0.5f : (enemyGangPlank.Level >= 7 ? 1f : 2f);
                                var nextHealthDecayTime = Game.Time < barrelBuff.StartTime + healthDecayRate ? barrelBuff.StartTime + healthDecayRate
                                                              : barrelBuff.StartTime + healthDecayRate * 2;

                                if (nextHealthDecayTime <= Game.Time + t / 1000f)
                                    return barrel;
                            }
                        }

                        if (barrels.Any())
                            return null;
                    }
                }

                /*Killable Minion*/
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)
                {
                    var MinionList =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(minion => minion.IsValidTarget() && this.InAutoAttackRange(minion))
                            .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                            .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                            .ThenBy(minion => minion.Health)
                            .ThenByDescending(minion => minion.MaxHealth);

                    foreach (var minion in MinionList)
                    {
                        var t = (int)(AttackCastDelay * 1000) - 20  /*+ Game.Ping / 2*/
                                + 1000 * (int)Math.Max(0, this.Player.Distance(minion) - this.Player.BoundingRadius)
                                / (int)GetMyProjectileSpeed();

                        if (mode == OrbwalkingMode.Freeze)
                        {
                            t += 200 + Game.Ping / 2;
                        }

                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, 0);

                        if (minion.Team != GameObjectTeam.Neutral && this.ShouldAttackMinion(minion))
                        {
                            var damage = this.Player.GetAutoAttackDamage(minion, true);
                            var killable = predHealth <= damage;

                            if (mode == OrbwalkingMode.Freeze)
                            {
                                if (minion.Health < 50 || predHealth <= 50)
                                    return minion;
                            }
                            else
                            {
                                if (predHealth <= 0)
                                    FireOnNonKillableMinion(minion);

                                if (killable)
                                {
                                    if (HealthPrediction.HasTurretAggro(minion as Obj_AI_Minion))
                                    {
                                       if(predHealth > 0)
                                            return minion;
                                    }
                                    else
                                    {
                                        return minion;
                                    }
                                }
                                  
                            }
                        }
                    }
                }


                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed)
                {
                    //if (!_config.Item("FocusMinionsOverTurrets").GetValue<KeyBind>().Active)
                    {
                        foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                            return turret;

                        foreach (var inhi in ObjectManager.Get<Obj_BarracksDampener>().Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                            return inhi;
                    }
                }

                /*Champions*/
                if (mode == OrbwalkingMode.Combo
                     || (_config.Item("PriorizeFarm").GetValue<bool>() && mode == OrbwalkingMode.Mixed)
                     || (!Player.UnderTurret(true) && mode == OrbwalkingMode.LaneClear))
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget() && this.InAutoAttackRange(target))
                    {
                        return target;
                    }
                }

                /*Jungle minions*/
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed)
                {
                    var jminions =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(mob => mob.IsValidTarget() && mob.Team == GameObjectTeam.Neutral && this.InAutoAttackRange(mob)
                               && mob.CharData.BaseSkinName != "gangplankbarrel" && mob.MaxHealth != 1);

                    result = _config.Item("Smallminionsprio").GetValue<bool>()
                                 ? jminions.MinOrDefault(mob => mob.MaxHealth)
                                 : jminions.MaxOrDefault(mob => mob.MaxHealth);

                    if (result != null)
                        return result;
                }

                /* UnderTurret Farming */
                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)
                {
                    var closestTower = ObjectManager.Get<Obj_AI_Turret>().MinOrDefault(t => t.IsAlly && !t.IsDead ? this.Player.Distance(t, true) : float.MaxValue);

                    if (closestTower != null && this.Player.Distance(closestTower, true) < 1500 * 1500 && Player.Level < 12)
                    {
                        // return all the minions underturret in auto attack range
                        var minions =
                            MinionManager.GetMinions(this.Player.Position, this.Player.AttackRange + 200)
                                .Where(
                                    minion =>
                                    this.InAutoAttackRange(minion) && closestTower.Distance(minion, true) < 900 * 900)
                                .OrderBy(minion => minion.Distance(closestTower));

                        if (minions.Any())
                        {
                            // get the turret aggro minion
                            var turretMinion =
                                minions.FirstOrDefault(
                                    minion =>
                                    minion is Obj_AI_Minion && HealthPrediction.HasTurretAggro(minion as Obj_AI_Minion));

                            if (turretMinion != null)
                            {
                                var damageOnMinion = closestTower.GetAutoAttackDamage(turretMinion, true);

                                var minionHpPred = HealthPrediction.GetHealthPrediction(turretMinion, 1500) - damageOnMinion;
                                if (minionHpPred > Player.GetAutoAttackDamage(turretMinion, true) && minionHpPred < damageOnMinion)
                                    return turretMinion;
                            }

                            var nextMinion = minions.FirstOrDefault(
                                   minion =>
                                   minion is Obj_AI_Minion && !HealthPrediction.HasTurretAggro(minion as Obj_AI_Minion));

                            if (nextMinion != null)
                            {
                                var damageOnMinion = closestTower.GetAutoAttackDamage(nextMinion, true);
                                var minionHpPred = HealthPrediction.GetHealthPrediction(nextMinion, 1500) - damageOnMinion;
                                if (minionHpPred > Player.GetAutoAttackDamage(nextMinion, true) && minionHpPred < damageOnMinion)
                                    return nextMinion;
                            }
                           
                                var minionToTryKill = minions.Where(x => closestTower.GetAutoAttackDamage(x, true) > x.Health 
                                                                                    && Player.GetAutoAttackDamage(nextMinion, true) < x.Health ).Last();
                                if (minionToTryKill != null)
                                    return minions.Last();
                            
                            if (mode == OrbwalkingMode.LaneClear && minions.Count() > 3)
                            {
                                var lastMinion = minions.Where(x =>!HealthPrediction.HasMinionAggro(x as Obj_AI_Minion)).Last();
                                if(lastMinion != null)
                                return minions.Last();
                            }
                            return null;
                        }
                    }
                }

                /*Lane Clear minions*/
                if (mode == OrbwalkingMode.LaneClear)
                {
                    if (!this.ShouldWait())
                    {
                        //if (this._prevMinion.IsValidTarget() && this.InAutoAttackRange(this._prevMinion))
                        //{
                        //    var predHealth = HealthPrediction.LaneClearHealthPrediction(
                        //        this._prevMinion,
                        //        (int)(this.Player.AttackDelay * 1000 * LaneClearWaitTimeMod),
                        //        this.FarmDelay);
                        //    if (predHealth >= 2 * this.Player.GetAutoAttackDamage(this._prevMinion)
                        //        || Math.Abs(predHealth - this._prevMinion.Health) < float.Epsilon)
                        //    {
                        //        return this._prevMinion;
                        //    }
                        //}

                        var t = (int)(AttackCastDelay * 1000) - 20 + 1000 * (int)Math.Max(0, 500)/ (int)GetMyProjectileSpeed();
                        float laneClearDelay = Player.AttackDelay * 1000 * LaneClearWaitTimeMod + t;

                        var results = (from minion in
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    minion =>
                                        minion.IsValidTarget() && this.InAutoAttackRange(minion)
                                        && this.ShouldAttackMinion(minion) && minion.MaxHealth != 1)
                                       let predHealth =
                                           HealthPrediction.LaneClearHealthPrediction(
                                               minion,
                                               (int)(laneClearDelay),
                                               this.FarmDelay)
                                       where
                                           predHealth >= 2 * this.Player.GetAutoAttackDamage(minion)
                                           || Math.Abs(predHealth - minion.Health) < float.Epsilon
                                       select minion);



                        result = results.OrderBy(m => m.Health - HealthPrediction.LaneClearHealthPrediction(m, (int)(laneClearDelay), this.FarmDelay)).FirstOrDefault();

                    }
                }

                return result;
            }

            public virtual bool InAutoAttackRange(AttackableUnit target)
            {
                return Orbwalking.InAutoAttackRange(target);
            }

            public virtual void RegisterCustomMode(string name, string displayname, uint key)
            {
                this.CustomModeName = name;
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

            public void SetOrbwalkingPoint(Vector3 point)
            {
                this._orbwalkingPoint = point;
            }



            public bool ShouldWait()
            {
                if (Player.Level > 13)
                    return false;

                var minionListAA = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget()
                            && minion.Team != GameObjectTeam.Neutral
                             && this.InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false));

                if (minionListAA.Any(minion => minion.BaseSkinName.Contains("super")))
                    return false;

                var minionsAlly = ObjectManager.Get<Obj_AI_Minion>().Where(minion => !minion.IsDead
                            && minion.IsAlly && minion.Distance(Player) < 600 && MinionManager.IsMinion(minion, false));

                int countAlly = minionsAlly.Count();

                if (minionListAA.Count() == 1 && countAlly > 3 && minionListAA.Any(x=> x.Health < Player.TotalAttackDamage * 2))
		            return true;

                if (countAlly > 2 && minionListAA.Any(x => x.IsMoving && x.Health < Player.TotalAttackDamage * 2))
		            return true;

                var t = (int)(AttackCastDelay * 1000) - 20 + 1000 * (int)Math.Max(0, 500) / (int)GetMyProjectileSpeed();
                float laneClearDelay = Player.AttackDelay * 1000 * LaneClearWaitTimeMod + t;
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Any(
                            minion =>
                            minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral
                            && this.InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false)
                            && HealthPrediction.LaneClearHealthPrediction(
                                minion,
                                (int)(laneClearDelay),
                                this.FarmDelay) <= this.Player.GetAutoAttackDamage(minion));
            }

            private void DrawingOnOnDraw(EventArgs args)
            {
                if (_config.Item("AACircle").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(this.Player.Position,
                        GetRealAutoAttackRange(null),
                        _config.Item("AACircle").GetValue<Circle>().Color,
                        _config.Item("AALineWidth").GetValue<Slider>().Value);
                }
                if (_config.Item("AACircle2").GetValue<Circle>().Active)
                {
                    foreach (var target in
                        HeroManager.Enemies.FindAll(target => target.IsValidTarget(1175)))
                    {
                        Render.Circle.DrawCircle(
                            target.Position,
                            GetAttackRange(target),
                            _config.Item("AACircle2").GetValue<Circle>().Color,
                            _config.Item("AALineWidth").GetValue<Slider>().Value);
                    }
                }

                if (_config.Item("HoldZone").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(
                        this.Player.Position,
                        _config.Item("HoldPosRadius").GetValue<Slider>().Value,
                        _config.Item("HoldZone").GetValue<Circle>().Color,
                        _config.Item("AALineWidth").GetValue<Slider>().Value,
                        true);
                }
                _config.Item("FocusMinionsOverTurrets")
                    .Permashow(_config.Item("FocusMinionsOverTurrets").GetValue<KeyBind>().Active);

                if (_config.Item("LastHitHelper").GetValue<bool>())
                {
                    foreach (var minion in
                        ObjectManager.Get<Obj_AI_Minion>()
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

            private void GameOnOnGameUpdate(EventArgs args)
            {
                try
                {
                    minionsWithBounding.Clear();
                    
                    foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.Health > 0 && minion.MaxHealth > 250 && minion.IsVisible && minion.Distance(Player.ServerPosition) < 500))
                    {
                        if(!minionsWithBounding.Any(x => x.Key.Distance(minion.Position ) < 1))
                            minionsWithBounding.Add(minion.Position, minion.BoundingRadius);
                    }

                    if (this.ActiveMode == OrbwalkingMode.None)
                    {
                        return;
                    }

                    //Prevent canceling important spells
                    if (this.Player.IsCastingInterruptableSpell(true))
                    {
                        return;
                    }

                    var target = this.GetTarget();
                    Orbwalk(target, this._orbwalkingPoint.To2D().IsValid() ? this._orbwalkingPoint : Game.CursorPos,
                        _config.Item("ExtraWindup").GetValue<Slider>().Value,
                        Math.Max(_config.Item("HoldPosRadius").GetValue<Slider>().Value, 30));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private bool ShouldAttackMinion(Obj_AI_Minion minion)
            {
                if (minion.Name == "WardCorpse" || minion.CharData.BaseSkinName == "jarvanivstandard")
                    return false;

                if (MinionManager.IsWard(minion))
                    return _config.Item("AttackWards").IsActive();

                return (_config.Item("AttackPetsnTraps").GetValue<bool>() || MinionManager.IsMinion(minion))
                       && minion.CharData.BaseSkinName != "gangplankbarrel";
            }

            private bool ShouldWaitUnderTurret(Obj_AI_Minion noneKillableMinion)
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Any(
                            minion =>
                            (noneKillableMinion != null ? noneKillableMinion.NetworkId != minion.NetworkId : true)
                            && minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral
                            && this.InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false)
                            && HealthPrediction.LaneClearHealthPrediction(
                                minion,
                                (int)
                                (this.Player.AttackDelay * 1000
                                 + (this.Player.IsMelee
                                        ? AttackCastDelay * 1000
                                        : AttackCastDelay * 1000
                                          + 1000 * (this.Player.AttackRange + 2 * this.Player.BoundingRadius)
                                          / this.Player.BasicAttack.MissileSpeed)),
                                this.FarmDelay) <= this.Player.GetAutoAttackDamage(minion));
            }
            
        }
    }
}
