namespace LeagueSharp.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    /// <summary>
    ///     Helps in decoding packets. This is not currently updated past 4.21!
    /// </summary>
    [Obsolete("Jodus wears dirty socks", false)]
    public static class Packet
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Packet" /> class.
        /// </summary>
        static Packet()
        {
            Console.WriteLine(
                @"LeagueSharp.Common.Packet will be removed soon, use LeagueSharp.Network.Packets instead");
        }

        #endregion

        #region Enums

        /// <summary>
        ///     The states of actions.
        /// </summary>
        public enum ActionStates
        {
            /// <summary>
            ///     The begin recall state
            /// </summary>
            BeginRecall = 111207118,

            /// <summary>
            ///     The finish recall state
            /// </summary>
            FinishRecall = 97690254,
        }

        /// <summary>
        ///     The type of attack.
        /// </summary>
        public enum AttackTypePacket
        {
            /// <summary>
            ///     Circular skillshots.
            /// </summary>
            Circular = 0,

            /// <summary>
            ///     Cone and Skillshot spells
            /// </summary>
            ConeSkillShot = 1,

            /// <summary>
            ///     Targeted spells and AAs
            /// </summary>
            TargetedAA = 2,
        }

        /// <summary>
        ///     The type of damage.
        /// </summary>
        public enum DamageTypePacket
        {
            /// <summary>
            ///     Magical Damage (AP)
            /// </summary>
            Magical = 4,

            /// <summary>
            ///     A Critical Attack
            /// </summary>
            CriticalAttack = 11,

            /// <summary>
            ///     Physical Damage (AD)
            /// </summary>
            Physical = 12,

            /// <summary>
            ///     True Damage
            /// </summary>
            True = 36,
        }

        /// <summary>
        ///     Types of emotes.
        /// </summary>
        public enum Emotes
        {
            /// <summary>
            ///     Dance
            /// </summary>
            Dance = 0x00,

            /// <summary>
            ///     Joke
            /// </summary>
            Joke = 0x03,

            /// <summary>
            ///     Taunt
            /// </summary>
            Taunt = 0x01,

            /// <summary>
            ///     Laugh
            /// </summary>
            Laugh = 0x02,
        }

        /// <summary>
        ///     Type of floating text on a hero.
        /// </summary>
        public enum FloatTextPacket
        {
            /// <summary>
            ///     Invulnerable
            /// </summary>
            Invulnerable,

            /// <summary>
            ///     Special
            /// </summary>
            Special,

            /// <summary>
            ///     Heal
            /// </summary>
            Heal,

            /// <summary>
            ///     Mana heal
            /// </summary>
            ManaHeal,

            /// <summary>
            ///     Mana Damage
            /// </summary>
            ManaDmg,

            /// <summary>
            ///     Dodge
            /// </summary>
            Dodge,

            /// <summary>
            ///     Critical
            /// </summary>
            Critical,

            /// <summary>
            ///     Experience
            /// </summary>
            Experience,

            /// <summary>
            ///     Gold
            /// </summary>
            Gold,

            /// <summary>
            ///     Level
            /// </summary>
            Level,

            /// <summary>
            ///     Disable
            /// </summary>
            Disable,

            /// <summary>
            ///     Quest Received
            /// </summary>
            QuestRecv,

            /// <summary>
            ///     Quest Done
            /// </summary>
            QuestDone,

            /// <summary>
            ///     Score
            /// </summary>
            Score,

            /// <summary>
            ///     Physical Damage
            /// </summary>
            PhysDmg,

            /// <summary>
            ///     Magic Damage
            /// </summary>
            MagicDmg,

            /// <summary>
            ///     True Damage
            /// </summary>
            TrueDmg,

            /// <summary>
            ///     Enemy Physical Damage
            /// </summary>
            EnemyPhysDmg,

            /// <summary>
            ///     Enemy Magic Damage
            /// </summary>
            EnemyMagicDmg,

            /// <summary>
            ///     Enemy True Damage
            /// </summary>
            EnemyTrueDmg,

            /// <summary>
            ///     Enemy Critical
            /// </summary>
            EnemyCritical,

            /// <summary>
            ///     Countdown
            /// </summary>
            Countdown,

            /// <summary>
            ///     Legacy
            /// </summary>
            Legacy,

            /// <summary>
            ///     Legacy critical
            /// </summary>
            LegacyCritical,

            /// <summary>
            ///     Debug
            /// </summary>
            Debug
        }

        /// <summary>
        ///     Because riot has run out of headers because they used byte headers, packets have 2 byte headers. This Enum
        ///     represnets them.
        /// </summary>
        public enum MultiPacketType
        {
            /* Confirmed in IDA */

            /// <summary>
            ///     The unknown100
            /// </summary>
            Unknown100 = 0x00,

            /// <summary>
            ///     The unknown101
            /// </summary>
            Unknown101 = 0x01,

            /// <summary>
            ///     The unknown102
            /// </summary>
            Unknown102 = 0x02,

            /// <summary>
            ///     The unknown115
            /// </summary>
            Unknown115 = 0x15,

            /// <summary>
            ///     The unknown116
            /// </summary>
            Unknown116 = 0x16,

            /// <summary>
            ///     The unknown124
            /// </summary>
            Unknown124 = 0x24,

            /// <summary>
            ///     The unknown11 a
            /// </summary>
            Unknown11A = 0x1A,

            /// <summary>
            ///     The unknown11 e (Currently Empty)
            /// </summary>
            Unknown11E = 0x1E,

            /* These others could be packets with a handler */

            /// <summary>
            ///     The unknown104. Somehow related to spell slots.
            /// </summary>
            Unknown104 = 0x04,

            /// <summary>
            ///     The unknown118. (Sion Ult)
            /// </summary>
            Unknown118 = 0x08,

            /// <summary>
            ///     The unknown120
            /// </summary>
            Unknown120 = 0x20, // confirmed in game

            /// <summary>
            ///     The spawn turret packet.
            /// </summary>
            SpawnTurret = 0x23, // confirmed in ida

            /* New List: Confirmed in Game */
            //FE 19 00 00 40 07 01 00 01 00 00 00 02 00 00 00 FF FF FF FF 00 00 00 00 00 00 00 00
            /// <summary>
            ///     The initialize spell pack. Could also be the stack count for stackables.
            /// </summary>
            InitSpell = 0x07,

            /// <summary>
            ///     The unknown10 c.
            /// </summary>
            Unknown10C = 0x0C, //this packet is like 0x127

            /// <summary>
            ///     The unknown122
            /// </summary>
            Unknown122 = 0x22,

            /// <summary>
            ///     The unknown125. (Sion Ult)
            /// </summary>
            Unknown125 = 0x25, // sion ult, other stuff
            //FE 05 00 00 40 25 01 03 EC 06 00 00 00 01 <== sion
            //            FE 19 00 00 40 25 01 00 00 07 00 00 00 06 FB 16 00 40 56 04 00 40 B2 04 00 40 B2 04 00 40 56 05 00 40 FB 16 00 40
            //FE 19 00 00 40 25 01 00 00 07 00 00 00 06 FB 16 00 40 56 04 00 40 B2 04 00 40 B2 04 00 40 56 05 00 40 FB 16 00 40
            //FE 19 00 00 40 25 01 00 00 07 00 00 00 06 FB 16 00 40 56 04 00 40 B2 04 00 40 B2 04 00 40 56 05 00 40 FB 16 00 40
            //FE 19 00 00 40 25 01 00 00 07 00 00 00 06 FB 16 00 40 56 04 00 40 B2 04 00 40 B2 04 00 40 56 05 00 40 FB 16 00 40
            //FE 19 00 00 40 25 01 00 00 07 00 00 00 06 56 04 00 40 B2 04 00 40 B2 04 00 40 56 05 00 40 56 05 00 40 FB 16 00 40

            /// <summary>
            ///     The NPC death packet
            /// </summary>
            NPCDeath = 0x26, //confirmed in ida, struct from intwars/ida

            /// <summary>
            ///     The unknown129. Related to spells.
            /// </summary>
            Unknown129 = 0x29, //related to spells (kalista ally unit), add?

            /// <summary>
            ///     The unknown12A. Related to  spells.
            /// </summary>
            Unknown12A = 0x2A, //related to spells (kalist ally unit after 0x129), maybe delete?

            //FE 06 00 00 40 2A 01 3C 00 00 00
            /// <summary>
            ///     The unknown12 c
            /// </summary>
            Unknown12C = 0x2C,

            //FE 00 00 00 00 2C 01 81 00 00 00 00 FF FF FF FF 
            //FE 00 00 00 00 2C 01 80 00 00 00 00 FF FF FF FF 
            /// <summary>
            ///     The unknown12 e
            /// </summary>
            Unknown12E = 0x2E, //confirmed in ida

            /// <summary>
            ///     The unknown12 f
            /// </summary>
            Unknown12F = 0x2F, //FE 05 00 00 40 2F 01 00

            /// <summary>
            ///     The add buff packet.
            /// </summary>
            AddBuff = 0x09, // buff added by towers in new SR

            /// <summary>
            ///     The undo token packet
            /// </summary>
            UndoToken = 0x0B,

            /// <summary>
            ///     The object creation packet. Used for Azir's ult.
            /// </summary>
            ObjectCreation = 0x0D, // azir ult

            /// <summary>
            ///     The surrender state packet
            /// </summary>
            SurrenderState = 0x0E,

            /// <summary>
            ///     The on attack packet.
            /// </summary>
            OnAttack = 0x0F,

            /// <summary>
            ///     The death timer packet.
            /// </summary>
            DeathTimer = 0x17,

            /// <summary>
            ///     The change item packet. (EX: Health Potion to Biscuit)
            /// </summary>
            ChangeItem = 0x1C, //like hpp=>biscuit

            /// <summary>
            ///     The action state packet. Triggers on recall.
            /// </summary>
            ActionState = 0x21, // ?? triggers on recall

            /// <summary>
            ///     The undo confirmation packet.
            /// </summary>
            UndoConfirm = 0x27,

            /// <summary>
            ///     The lock camera packet for Sion's Ult.
            /// </summary>
            LockCamera = 0x2B, // Sion Ult

            /// <summary>
            ///     An unkown packet.
            /// </summary>
            Unknown = 0xFF, // Default, not real packet
        }

        /// <summary>
        ///     The type of ping.
        /// </summary>
        public enum PingType
        {
            /// <summary>
            ///     A normal ping.
            /// </summary>
            Normal = 0,

            /// <summary>
            ///     A fallback ping.
            /// </summary>
            Fallback = 5,

            /// <summary>
            ///     An enemy missing ping.
            /// </summary>
            EnemyMissing = 3,

            /// <summary>
            ///     A danagr ping.
            /// </summary>
            Danger = 2,

            /// <summary>
            ///     An on my way ping.
            /// </summary>
            OnMyWay = 4,

            /// <summary>
            ///     An assist me ping.
            /// </summary>
            AssistMe = 6,
        }
        #endregion


        /// <summary>
        ///     Contains packet that are sent from the server to the client (the game).
        /// </summary>
        public static class S2C
        {
            /// <summary>
            ///     Gets received when a unit starts, aborts or finishes a teleport (such as recall, teleport, twisted fate ulti, shen
            ///     ulti,...)
            /// </summary>
            public static class Teleport
            {
                #region Constants

                /// <summary>
                ///     The error gap in ticks.
                /// </summary>
                private const int ErrorGap = 100; //in ticks

                #endregion

                #region Static Fields

                /// <summary>
                ///     The header
                /// </summary>
                public static byte Header = 0x44;

                /// <summary>
                ///     The recall data by network identifier
                /// </summary>
                private static readonly IDictionary<int, TeleportData> RecallDataByNetworkId =
                    new Dictionary<int, TeleportData>();

                /// <summary>
                ///     The type by string
                /// </summary>
                private static readonly IDictionary<string, ITeleport> TypeByString =
                    new Dictionary<string, ITeleport>
                        {
                            { "Recall", new RecallTeleport() }, { "Teleport", new TeleportTeleport() },
                            { "Gate", new TwistedFateTeleport() }, { "Shen", new ShenTeleport() },
                        };

                #endregion

                #region Enums

                /// <summary>
                ///     The status of the teleport.
                /// </summary>
                public enum Status
                {
                    /// <summary>
                    ///     The teleport has been started.
                    /// </summary>
                    Start,

                    /// <summary>
                    ///     The teleport has been aborted.
                    /// </summary>
                    Abort,

                    /// <summary>
                    ///     The teleport has finished.
                    /// </summary>
                    Finish,

                    /// <summary>
                    ///     The status of the teleport is unknown.
                    /// </summary>
                    Unknown
                }

                /// <summary>
                ///     The type of teleport.
                /// </summary>
                public enum Type
                {
                    /// <summary>
                    ///     The unit is recalling back to base.
                    /// </summary>
                    Recall,

                    /// <summary>
                    ///     The unit is teleporting to another unit with the Teleport summoner spell.
                    /// </summary>
                    Teleport,

                    /// <summary>
                    ///     The unit is teleporting to a location with Twisted Fate's Ultimate.
                    /// </summary>
                    TwistedFate,

                    /// <summary>
                    ///     The unit is teleporting to a unit with Shen's Ultimate.
                    /// </summary>
                    Shen,

                    /// <summary>
                    ///     The type of teleportation is unknown.
                    /// </summary>
                    Unknown
                }

                #endregion

                #region Interfaces

                /// <summary>
                ///     An interface for different types of teleports to get durations and type of teleport.
                /// </summary>
                internal interface ITeleport
                {
                    #region Public Properties

                    /// <summary>
                    ///     Gets the type.
                    /// </summary>
                    /// <value>The type.</value>
                    Type Type { get; }

                    #endregion

                    #region Public Methods and Operators

                    /// <summary>
                    ///     Gets the duration.
                    /// </summary>
                    /// <param name="packetData">The <see cref="GameObjectTeleportEventArgs" /> instance containing the event data.</param>
                    /// <returns>System.Int32.</returns>
                    int GetDuration(GameObjectTeleportEventArgs packetData);

                    #endregion
                }

                #endregion

                #region Public Methods and Operators

                /// <summary>
                ///     Decodes the specified sender.
                /// </summary>
                /// <param name="sender">The sender.</param>
                /// <param name="args">The <see cref="GameObjectTeleportEventArgs" /> instance containing the event data.</param>
                /// <returns>Struct.</returns>
                public static Struct Decoded(GameObject sender, GameObjectTeleportEventArgs args) //
                {
                    var result = new Struct { Status = Status.Unknown, Type = Type.Unknown };

                    if (sender == null || !sender.IsValid || !(sender is Obj_AI_Hero))
                    {
                        return result;
                    }

                    result.UnitNetworkId = sender.NetworkId;

                    var hero = sender as Obj_AI_Hero;

                    if (!RecallDataByNetworkId.ContainsKey(result.UnitNetworkId))
                    {
                        RecallDataByNetworkId[result.UnitNetworkId] = new TeleportData { Type = Type.Unknown };
                    }

                    if (!string.IsNullOrEmpty(args.RecallType))
                    {
                        if (TypeByString.ContainsKey(args.RecallType))
                        {
                            ITeleport teleportMethod = TypeByString[args.RecallType];

                            var duration = teleportMethod.GetDuration(args);
                            var type = teleportMethod.Type;
                            var time = Utils.TickCount;

                            RecallDataByNetworkId[result.UnitNetworkId] = new TeleportData
                            {
                                Duration = duration,
                                Type = type,
                                Start = time
                            };

                            result.Status = Status.Start;
                            result.Duration = duration;
                            result.Type = type;
                            result.Start = time;
                        }
                    }
                    else
                    {
                        var shorter = Utils.TickCount - RecallDataByNetworkId[result.UnitNetworkId].Start
                                      < RecallDataByNetworkId[result.UnitNetworkId].Duration - ErrorGap;
                        result.Status = shorter ? Status.Abort : Status.Finish;
                        result.Type = RecallDataByNetworkId[result.UnitNetworkId].Type;
                        result.Duration = 0;
                        result.Start = 0;
                    }
                    return result;
                }

                /// <summary>
                ///     Encodes the specified packet structure.
                /// </summary>
                /// <param name="packetStruct">The packet structure.</param>
                /// <returns>GamePacket.</returns>
                public static GamePacket Encoded(Struct packetStruct)
                {
                    //TODO when the packet is fully decoded.
                    return new GamePacket(Header);
                }

                #endregion

                /// <summary>
                ///     Represents the packet received when a unit teleports.
                /// </summary>
                public struct Struct
                {
                    #region Fields

                    /// <summary>
                    ///     The duration
                    /// </summary>
                    public int Duration;

                    /// <summary>
                    ///     The start
                    /// </summary>
                    public int Start;

                    /// <summary>
                    ///     The status
                    /// </summary>
                    public Status Status;

                    /// <summary>
                    ///     The type
                    /// </summary>
                    public Type Type;

                    /// <summary>
                    ///     The unit network identifier
                    /// </summary>
                    public int UnitNetworkId;

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    ///     Initializes a new instance of the <see cref="Struct" /> struct.
                    /// </summary>
                    /// <param name="unitNetworkId">The unit network identifier.</param>
                    /// <param name="status">The status.</param>
                    /// <param name="type">The type.</param>
                    /// <param name="duration">The duration.</param>
                    /// <param name="start">The start.</param>
                    public Struct(int unitNetworkId, Status status, Type type, int duration, int start = 0)
                    {
                        this.UnitNetworkId = unitNetworkId;
                        this.Status = status;
                        this.Type = type;
                        this.Duration = duration;
                        this.Start = start;
                    }

                    #endregion
                }

                /// <summary>
                ///     Contains data about the teleport.
                /// </summary>
                internal struct TeleportData
                {
                    #region Public Properties

                    /// <summary>
                    ///     Gets or sets the duration.
                    /// </summary>
                    /// <value>The duration.</value>
                    public int Duration { get; set; }

                    /// <summary>
                    ///     Gets or sets the start.
                    /// </summary>
                    /// <value>The start.</value>
                    public int Start { get; set; }

                    /// <summary>
                    ///     Gets or sets the type.
                    /// </summary>
                    /// <value>The type.</value>
                    public Type Type { get; set; }

                    #endregion
                }

                /// <summary>
                ///     A recall teleport.
                /// </summary>
                internal class RecallTeleport : ITeleport
                {
                    #region Public Properties

                    /// <summary>
                    ///     Gets the type.
                    /// </summary>
                    /// <value>The type.</value>
                    public Type Type
                    {
                        get
                        {
                            return Type.Recall;
                        }
                    }

                    #endregion

                    #region Public Methods and Operators

                    /// <summary>
                    ///     Gets the duration.
                    /// </summary>
                    /// <param name="args">The <see cref="GameObjectTeleportEventArgs" /> instance containing the event data.</param>
                    /// <returns>System.Int32.</returns>
                    public int GetDuration(GameObjectTeleportEventArgs args)
                    {
                        return Utility.GetRecallTime(args.RecallName);
                    }

                    #endregion
                }

                /// <summary>
                ///     A Shen's Ultimate teleport.
                /// </summary>
                internal class ShenTeleport : ITeleport
                {
                    #region Public Properties

                    /// <summary>
                    ///     Gets the type.
                    /// </summary>
                    /// <value>The type.</value>
                    public Type Type
                    {
                        get
                        {
                            return Type.Shen;
                        }
                    }

                    #endregion

                    #region Public Methods and Operators

                    /// <summary>
                    ///     Gets the duration.
                    /// </summary>
                    /// <param name="args">The <see cref="GameObjectTeleportEventArgs" /> instance containing the event data.</param>
                    /// <returns>System.Int32.</returns>
                    public int GetDuration(GameObjectTeleportEventArgs args)
                    {
                        return 3000;
                    }

                    #endregion
                }

                /// <summary>
                ///     A teleport summoner spell teleport.
                /// </summary>
                internal class TeleportTeleport : ITeleport
                {
                    #region Public Properties

                    /// <summary>
                    ///     Gets the type.
                    /// </summary>
                    /// <value>The type.</value>
                    public Type Type
                    {
                        get
                        {
                            return Type.Teleport;
                        }
                    }

                    #endregion

                    #region Public Methods and Operators

                    /// <summary>
                    ///     Gets the duration.
                    /// </summary>
                    /// <param name="args">The <see cref="GameObjectTeleportEventArgs" /> instance containing the event data.</param>
                    /// <returns>System.Int32.</returns>
                    public int GetDuration(GameObjectTeleportEventArgs args)
                    {
                        return 3500;
                    }

                    #endregion
                }

                /// <summary>
                ///     A Twisted Fate's Ultimate Teleport.
                /// </summary>
                internal class TwistedFateTeleport : ITeleport
                {
                    #region Public Properties

                    /// <summary>
                    ///     Gets the type.
                    /// </summary>
                    /// <value>The type.</value>
                    public Type Type
                    {
                        get
                        {
                            return Type.TwistedFate;
                        }
                    }

                    #endregion

                    #region Public Methods and Operators

                    /// <summary>
                    ///     Gets the duration.
                    /// </summary>
                    /// <param name="args">The <see cref="GameObjectTeleportEventArgs" /> instance containing the event data.</param>
                    /// <returns>System.Int32.</returns>
                    public int GetDuration(GameObjectTeleportEventArgs args)
                    {
                        return 1500;
                    }

                    #endregion
                }
            }
        }
    }
}