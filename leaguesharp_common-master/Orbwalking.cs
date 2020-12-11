namespace LeagueSharp.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SharpDX;
    using Color = System.Drawing.Color;

    public static class Orbwalking
    {
        public static int[] LastTargets = new int[] { 0, 0, 0 };
        public static bool Attack = true;
        public static bool DisableNextAttack;
        private static int _ApheliosChakramAATick;
        private static int _LastAATick;

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
                    if (Player.ChampionName == "Caitlyn")
                    {
                        if (aiBase.HasBuff("caitlynyordletrapinternal"))
                            return 1250;
                    }

                    if (Player.ChampionName == "Aphelios")
                    {
                        if (aiBase.HasBuff("aphelioscalibrumbonusrangedebuff"))
                            return 1800;
                    }

                    result -= Math.Min(Game.Ping / 9f, 10f);
                    result -= 7;
                    if (Player.IsMoving && aiBase.IsMoving)
                    {
                        if (!aiBase.IsFacing(Player))
                            result -= 10;
                        if (Player.IsFacing(aiBase))
                            result -= 5;
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

            if (Utils.GameTimeTickCount - LastMoveCommandT < 70 + Math.Min(60, Game.Ping) && !overrideTimer && angle < 60)
                return;

            if (angle >= 60 && Utils.GameTimeTickCount - LastMoveCommandT < 60)
                return;

            if (Player.Distance(point, true) > 500 * 500)
                point = playerPosition.Extend(position, randomizeMinDistance ? (_random.NextFloat(0.6f, 1)) * 800 : 800);

            Player.ForceIssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = point;
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
                            Console.WriteLine("ALL INN");
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SpellbookOnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && spellbook.Owner.IsMe && args.DestroyMissile && args.StopAnimation)
            {
                ResetAutoAttackTimer();
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
            private const float LaneClearWaitTimeMod = 2f;
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
                        var t = (int)(AttackCastDelay * 1000) - 100 + Game.Ping / 2
                                + 1000 * (int)Math.Max(0, this.Player.Distance(minion) - this.Player.BoundingRadius)
                                / (int)GetMyProjectileSpeed();

                        if (mode == OrbwalkingMode.Freeze)
                        {
                            t += 200 + Game.Ping / 2;
                        }

                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, this.FarmDelay);

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
                                    return minion;
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

                    if (closestTower != null && this.Player.Distance(closestTower, true) < 1500 * 1500)
                    {
                        Obj_AI_Minion farmUnderTurretMinion = null;
                        Obj_AI_Minion noneKillableMinion = null;
                        // return all the minions underturret in auto attack range
                        var minions =
                            MinionManager.GetMinions(this.Player.Position, this.Player.AttackRange + 200)
                                .Where(
                                    minion =>
                                    this.InAutoAttackRange(minion) && closestTower.Distance(minion, true) < 900 * 900)
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
                                    minion is Obj_AI_Minion && HealthPrediction.HasTurretAggro(minion as Obj_AI_Minion));

                            if (turretMinion != null)
                            {
                                var hpLeftBeforeDie = 0;
                                var hpLeft = 0;
                                var turretAttackCount = 0;
                                var turretStarTick = HealthPrediction.TurretAggroStartTick(
                                    turretMinion as Obj_AI_Minion);
                                // from healthprediction (don't blame me :S)
                                var turretLandTick = turretStarTick + (int)(closestTower.AttackCastDelay * 1000)
                                                     + 1000
                                                     * Math.Max(
                                                         0,
                                                         (int)
                                                         (turretMinion.Distance(closestTower)
                                                          - closestTower.BoundingRadius))
                                                     / (int)(closestTower.BasicAttack.MissileSpeed + 70);
                                // calculate the HP before try to balance it
                                for (float i = turretLandTick + 50;
                                     i < turretLandTick + 10 * closestTower.AttackDelay * 1000 + 50;
                                     i = i + closestTower.AttackDelay * 1000)
                                {
                                    var time = (int)i - Utils.GameTimeTickCount + Game.Ping / 2;
                                    var predHP =
                                        (int)
                                        HealthPrediction.LaneClearHealthPrediction(turretMinion, time > 0 ? time : 0);
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
                                    var damage = (int)this.Player.GetAutoAttackDamage(turretMinion, true);
                                    var hits = hpLeftBeforeDie / damage;
                                    var timeBeforeDie = turretLandTick
                                                        + (turretAttackCount + 1)
                                                        * (int)(closestTower.AttackDelay * 1000)
                                                        - Utils.GameTimeTickCount;
                                    var timeUntilAttackReady = LastAATick + (int)(this.Player.AttackDelay * 1000)
                                                               > Utils.GameTimeTickCount + Game.Ping / 2 + 25
                                                                   ? LastAATick + (int)(this.Player.AttackDelay * 1000)
                                                                     - (Utils.GameTimeTickCount + Game.Ping / 2 + 25)
                                                                   : 0;
                                    var timeToLandAttack = this.Player.IsMelee
                                                               ? AttackCastDelay * 1000
                                                               : AttackCastDelay * 1000
                                                                 + 1000
                                                                 * Math.Max(
                                                                     0,
                                                                     turretMinion.Distance(this.Player)
                                                                     - this.Player.BoundingRadius)
                                                                 / this.Player.BasicAttack.MissileSpeed;
                                    if (hits >= 1
                                        && hits * this.Player.AttackDelay * 1000 + timeUntilAttackReady
                                        + timeToLandAttack < timeBeforeDie)
                                    {
                                        farmUnderTurretMinion = turretMinion as Obj_AI_Minion;
                                    }
                                    else if (hits >= 1
                                             && hits * this.Player.AttackDelay * 1000 + timeUntilAttackReady
                                             + timeToLandAttack > timeBeforeDie)
                                    {
                                        noneKillableMinion = turretMinion as Obj_AI_Minion;
                                    }
                                }
                                else if (hpLeft == 0 && turretAttackCount == 0 && hpLeftBeforeDie == 0)
                                {
                                    noneKillableMinion = turretMinion as Obj_AI_Minion;
                                }
                                // should wait before attacking a minion.
                                if (this.ShouldWaitUnderTurret(noneKillableMinion))
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
                                        x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion
                                        && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion)))
                                {
                                    var playerDamage = (int)this.Player.GetAutoAttackDamage(minion);
                                    var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                    var leftHP = (int)minion.Health % turretDamage;
                                    if (leftHP > playerDamage)
                                    {
                                        return minion;
                                    }
                                }
                                // late game
                                var lastminion =
                                    minions.LastOrDefault(
                                        x =>
                                        x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion
                                        && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));
                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (1f / this.Player.AttackDelay >= 1f
                                        && (int)(turretAttackCount * closestTower.AttackDelay / this.Player.AttackDelay)
                                        * this.Player.GetAutoAttackDamage(lastminion) > lastminion.Health)
                                    {
                                        return lastminion;
                                    }
                                    if (minions.Count() >= 5 && 1f / this.Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            else
                            {
                                if (this.ShouldWaitUnderTurret(noneKillableMinion))
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
                                        var playerDamage = (int)this.Player.GetAutoAttackDamage(minion);
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
                                    minions.LastOrDefault(
                                        x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));
                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (minions.Count() >= 5 && 1f / this.Player.AttackDelay >= 1.2)
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
                    if (!this.ShouldWait())
                    {
                        if (this._prevMinion.IsValidTarget() && this.InAutoAttackRange(this._prevMinion))
                        {
                            var predHealth = HealthPrediction.LaneClearHealthPrediction(
                                this._prevMinion,
                                (int)(this.Player.AttackDelay * 1000 * LaneClearWaitTimeMod),
                                this.FarmDelay);
                            if (predHealth >= 2 * this.Player.GetAutoAttackDamage(this._prevMinion)
                                || Math.Abs(predHealth - this._prevMinion.Health) < float.Epsilon)
                            {
                                return this._prevMinion;
                            }
                        }

                        var results = (from minion in
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    minion =>
                                        minion.IsValidTarget() && this.InAutoAttackRange(minion)
                                        && this.ShouldAttackMinion(minion) && minion.MaxHealth != 1)
                                       let predHealth =
                                           HealthPrediction.LaneClearHealthPrediction(
                                               minion,
                                               (int)(this.Player.AttackDelay * 1000 * LaneClearWaitTimeMod),
                                               this.FarmDelay)
                                       where
                                           predHealth >= 2 * this.Player.GetAutoAttackDamage(minion)
                                           || Math.Abs(predHealth - minion.Health) < float.Epsilon
                                       select minion);

                        result = results.MaxOrDefault(m => !MinionManager.IsMinion(m, true) ? float.MaxValue : m.Health);

                        if (_config.Item("PrioritizeCasters").GetValue<bool>())
                        {
                            result =
                                results.OrderByDescending(
                                    m =>
                                        m.CharData.BaseSkinName.Contains("Ranged"))
                                    .FirstOrDefault();
                        }

                        if (result != null)
                        {
                            this._prevMinion = (Obj_AI_Minion)result;
                        }
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
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Any(
                            minion =>
                            minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral
                            && this.InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false)
                            && HealthPrediction.LaneClearHealthPrediction(
                                minion,
                                (int)(this.Player.AttackDelay * 1000 * LaneClearWaitTimeMod),
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
