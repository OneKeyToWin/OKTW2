namespace LeagueSharp.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HealthPrediction
    {

        private static readonly Dictionary<int, PredictedDamage> ActiveAttacks = new Dictionary<int, PredictedDamage>();


        #region Constructors and Destructors

        static HealthPrediction()
        {
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            Spellbook.OnStopCast += SpellbookOnStopCast;
            GameObject.OnDelete += MissileClient_OnDelete;
            Obj_AI_Base.OnDoCast += Obj_AI_Base_OnDoCast;
        }

        #endregion


        public static Obj_AI_Base GetAggroTurret(Obj_AI_Minion minion)
        {
            var ActiveTurret =
                ActiveAttacks.Values.FirstOrDefault(
                    m => (m.Source is Obj_AI_Turret) && m.Target.NetworkId == minion.NetworkId);
            return ActiveTurret != null ? ActiveTurret.Source : null;
        }

        public static float GetHealthPrediction(Obj_AI_Base unit, int time, int delay = 0)
        {
            var predictedDamage = 0f;
            foreach (var attack in ActiveAttacks.Values)
            {
                if (!attack.Processed && attack.Source.IsValidTarget(float.MaxValue, false)
                    && attack.Target.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var landTime = attack.StartTick + attack.Delay
                                   + 1000f * Math.Max(0f, unit.Distance(attack.Source) - attack.Source.BoundingRadius)
                                   / attack.ProjectileSpeed + 10f;

                    if (landTime < Utils.GameTimeTickCount + time)
                    {
                        predictedDamage += attack.Damage;
                    }
                }
            }
            return unit.Health - predictedDamage;
        }
        public static bool HasMinionAggro(Obj_AI_Minion minion)
        {
            return ActiveAttacks.Values.Any(m => (m.Source is Obj_AI_Minion) && m.Target.NetworkId == minion.NetworkId);
        }

        public static bool HasTurretAggro(Obj_AI_Minion minion)
        {
            return ActiveAttacks.Values.Any(m => (m.Source is Obj_AI_Turret) && m.Target.NetworkId == minion.NetworkId);
        }

        public static float LaneClearHealthPrediction(Obj_AI_Base unit, int time, int delay = 0)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                if (Utils.GameTimeTickCount - 100 <= attack.StartTick + attack.AnimationTime
                    && attack.Target.IsValidTarget(float.MaxValue, false)
                    && attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var n = 1;
                    var fromT = attack.StartTick;
                    var toT = Utils.GameTimeTickCount + time;

                    while (fromT < toT)
                    {
                        var travelTime = fromT + attack.Delay + 1000f * Math.Max(0, unit.Distance(attack.Source) - attack.Source.BoundingRadius) / attack.ProjectileSpeed + 10f;
                        if (fromT >= Utils.GameTimeTickCount && travelTime < toT)
                        {
                            n++;
                        }

                        fromT += (int)attack.AnimationTime;
                    }
                    predictedDamage += n * attack.Damage;
                }
            }
            return unit.Health - predictedDamage;
        }

        public static int TurretAggroStartTick(Obj_AI_Minion minion)
        {
            var ActiveTurret =
                ActiveAttacks.Values.FirstOrDefault(
                    m => (m.Source is Obj_AI_Turret) && m.Target.NetworkId == minion.NetworkId);
            return ActiveTurret != null ? ActiveTurret.StartTick : 0;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < Utils.GameTimeTickCount - 3000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));
        }


        static void MissileClient_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null && missile.SpellCaster != null)
            {
                var casterNetworkId = missile.SpellCaster.NetworkId;
                foreach (var activeAttack in ActiveAttacks)
                {
                    if (activeAttack.Key == casterNetworkId)
                    {
                        ActiveAttacks[casterNetworkId].Processed = true;
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (ActiveAttacks.ContainsKey(sender.NetworkId) && sender.IsMelee)
            {
                ActiveAttacks[sender.NetworkId].Processed = true;
            }
        }

        private static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValidTarget(3000, false) || sender.Team != ObjectManager.Player.Team || sender.IsMe
                || !Orbwalking.IsAutoAttack(args.SData.Name) || !(args.Target is Obj_AI_Base))
            {
                return;
            }

            var target = (Obj_AI_Base)args.Target;
            ActiveAttacks.Remove(sender.NetworkId);

            var attackData = new PredictedDamage(
                sender,
                target,
                Utils.GameTimeTickCount,
                sender.AttackCastDelay * 1000f,
                sender.AttackDelay * 1000f + (sender is Obj_AI_Turret ? 50 : 0),
                sender.IsMelee() ? int.MaxValue : (int)args.SData.MissileSpeed,
                (float)sender.GetAutoAttackDamage(target, true));
            ActiveAttacks.Add(sender.NetworkId, attackData);
        }


        private static void SpellbookOnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && args.StopAnimation)
            {
                if (ActiveAttacks.ContainsKey(spellbook.Owner.NetworkId))
                {
                    ActiveAttacks.Remove(spellbook.Owner.NetworkId);
                }
            }
        }

        private class PredictedDamage
        {
            #region Fields

            /// <summary>
            ///     The animation time
            /// </summary>
            public readonly float AnimationTime;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PredictedDamage" /> class.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="target">The target.</param>
            /// <param name="startTick">The start tick.</param>
            /// <param name="delay">The delay.</param>
            /// <param name="animationTime">The animation time.</param>
            /// <param name="projectileSpeed">The projectile speed.</param>
            /// <param name="damage">The damage.</param>
            public PredictedDamage(
                Obj_AI_Base source,
                Obj_AI_Base target,
                int startTick,
                float delay,
                float animationTime,
                int projectileSpeed,
                float damage)
            {
                this.Source = source;
                this.Target = target;
                this.StartTick = startTick;
                this.Delay = delay;
                this.ProjectileSpeed = projectileSpeed;
                this.Damage = damage;
                this.AnimationTime = animationTime;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the damage.
            /// </summary>
            /// <value>
            ///     The damage.
            /// </value>
            public float Damage { get; private set; }

            /// <summary>
            ///     Gets or sets the delay.
            /// </summary>
            /// <value>
            ///     The delay.
            /// </value>
            public float Delay { get; private set; }

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="PredictedDamage" /> is processed.
            /// </summary>
            /// <value>
            ///     <c>true</c> if processed; otherwise, <c>false</c>.
            /// </value>
            public bool Processed { get; internal set; }

            /// <summary>
            ///     Gets or sets the projectile speed.
            /// </summary>
            /// <value>
            ///     The projectile speed.
            /// </value>
            public int ProjectileSpeed { get; private set; }

            /// <summary>
            ///     Gets or sets the source.
            /// </summary>
            /// <value>
            ///     The source.
            /// </value>
            public Obj_AI_Base Source { get; private set; }

            /// <summary>
            ///     Gets or sets the start tick.
            /// </summary>
            /// <value>
            ///     The start tick.
            /// </value>
            public int StartTick { get; internal set; }

            /// <summary>
            ///     Gets or sets the target.
            /// </summary>
            /// <value>
            ///     The target.
            /// </value>
            public Obj_AI_Base Target { get; private set; }

            #endregion
        }
    }
}