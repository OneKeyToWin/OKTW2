namespace LeagueSharp.Common.Data
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ItemData
    {
        #region Static Fields

        public static Item Boots = new Item
        {
            Name = "Boots",
            Into = new[]
                        {
                                3158, 3006, 3009, 3020, 3047, 3111, 3117,
                        },
            GoldBase = 300,
            GoldPrice = 300,
            GoldSell = 210,
            Purchasable = true,
            Tags = new[]
                {
                        "Boots",
                        },
            FlatMovementSpeedMod = 25f,
            Id = 1001
        };

        public static Item Faerie_Charm = new Item
        {
            Name = "Faerie Charm",
            Into = new[]
                                {
                                2065, 3114, 4642,
                        },
            GoldBase = 250,
            GoldPrice = 250,
            GoldSell = 175,
            Purchasable = true,
            Tags = new[]
                        {
                        "ManaRegen",
                        },

            Id = 1004
        };

        public static Item Rejuvenation_Bead = new Item
        {
            Name = "Rejuvenation Bead",
            Into = new[]
                                {
                                3109, 3801,
                        },
            GoldBase = 150,
            GoldPrice = 150,
            GoldSell = 105,
            Purchasable = true,
            Tags = new[]
                        {
                        "HealthRegen",
                        },

            Id = 1006
        };

        public static Item Giants_Belt = new Item
        {
            Name = "Giant's Belt",
            From = new[]
                                {
                                1028,
                        },
            Into = new[]
                                {
                                3075, 3001, 3083, 3116, 3748, 4637,
                        },
            GoldBase = 500,
            GoldPrice = 900,
            GoldSell = 630,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health",
                        },
            FlatHPPoolMod = 350f,
            Depth = 2,
            Id = 1011
        };

        public static Item Cloak_of_Agility = new Item
        {
            Name = "Cloak of Agility",
            Into = new[]
                                {
                                3124, 6676, 3086, 3031, 3036, 3072, 3095, 3139, 3508, 6671, 6672, 6673, 6675,
                        },
            GoldBase = 600,
            GoldPrice = 600,
            GoldSell = 420,
            Purchasable = true,
            Tags = new[]
                        {
                        "CriticalStrike",
                        },
            FlatCritChanceMod = 0.15f,
            Id = 1018
        };

        public static Item Blasting_Wand = new Item
        {
            Name = "Blasting Wand",
            Into = new[]
                                {
                                6616, 3100, 3115, 3116, 6655, 3135, 3152, 3165, 3504, 4633, 4636, 4637, 6656,
                        },
            GoldBase = 850,
            GoldPrice = 850,
            GoldSell = 595,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage",
                        },
            FlatMagicDamageMod = 40f,
            Id = 1026
        };

        public static Item Sapphire_Crystal = new Item
        {
            Name = "Sapphire Crystal",
            Into = new[]
                                {
                                3004, 3003, 3024, 3802,
                        },
            GoldBase = 350,
            GoldPrice = 350,
            GoldSell = 245,
            Purchasable = true,
            Tags = new[]
                        {
                        "Mana",
                        },
            FlatMPPoolMod = 250f,
            Id = 1027
        };

        public static Item Ruby_Crystal = new Item
        {
            Name = "Ruby Crystal",
            Into = new[]
                                {
                                6035, 6609, 1011, 3044, 3053, 3066, 3067, 3211, 3814, 3152, 3165, 3742, 3748, 3801, 4401, 4635, 4636, 6660,
                        },
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 280,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health",
                        },
            FlatHPPoolMod = 150f,
            Id = 1028
        };

        public static Item Cloth_Armor = new Item
        {
            Name = "Cloth Armor",
            Into = new[]
                                {
                                1031, 3193, 3076, 3191, 3024, 3047, 3082, 3105, 6664, 3143,
                        },
            GoldBase = 300,
            GoldPrice = 300,
            GoldSell = 210,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor",
                        },
            FlatArmorMod = 15f,
            Id = 1029
        };

        public static Item Chain_Vest = new Item
        {
            Name = "Chain Vest",
            From = new[]
                                {
                                1029,
                        },
            Into = new[]
                                {
                                3026, 3742, 6333, 6662,
                        },
            GoldBase = 500,
            GoldPrice = 800,
            GoldSell = 560,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor",
                        },
            FlatArmorMod = 40f,
            Depth = 2,
            Id = 1031
        };

        public static Item NullMagic_Mantle = new Item
        {
            Name = "Null-Magic Mantle",
            Into = new[]
                                {
                                3193, 1057, 3105, 3211, 3111, 3140, 3155, 4632, 6662,
                        },
            GoldBase = 450,
            GoldPrice = 450,
            GoldSell = 315,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock",
                        },
            FlatSpellBlockMod = 25f,
            Id = 1033
        };

        public static Item Emberknife = new Item
        {
            Name = "Emberknife",
            GoldBase = 350,
            GoldPrice = 350,
            GoldSell = 140,
            Purchasable = true,
            Tags = new[]
                        {
                        "Jungle",
                        },

            Id = 1035
        };

        public static Item Long_Sword = new Item
        {
            Name = "Long Sword",
            Into = new[]
                                {
                                3071, 1053, 3035, 3044, 3051, 3814, 3123, 3133, 3134, 3142, 3155, 6670, 6692,
                        },
            GoldBase = 350,
            GoldPrice = 350,
            GoldSell = 245,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "Lane",
                        },
            FlatPhysicalDamageMod = 10f,
            Id = 1036
        };

        public static Item Pickaxe = new Item
        {
            Name = "Pickaxe",
            Into = new[]
                                {
                                6035, 3077, 6676, 3085, 3031, 3053, 3139, 3142, 3153, 6029, 6333, 6671, 6672, 6675, 6695,
                        },
            GoldBase = 875,
            GoldPrice = 875,
            GoldSell = 613,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage",
                        },
            FlatPhysicalDamageMod = 25f,
            Id = 1037
        };

        public static Item B_F_Sword = new Item
        {
            Name = "B. F. Sword",
            Into = new[]
                                {
                                3026, 3031, 3072, 3095, 4403,
                        },
            GoldBase = 1300,
            GoldPrice = 1300,
            GoldSell = 910,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage",
                        },
            FlatPhysicalDamageMod = 40f,
            Id = 1038
        };

        public static Item Hailblade = new Item
        {
            Name = "Hailblade",
            GoldBase = 350,
            GoldPrice = 350,
            GoldSell = 140,
            Purchasable = true,
            Tags = new[]
                        {
                        "Jungle",
                        },

            Id = 1039
        };

        public static Item Dagger = new Item
        {
            Name = "Dagger",
            Into = new[]
                                {
                                1043, 3091, 3124, 6677, 3085, 2015, 3086, 3006, 3046, 3051, 6670,
                        },
            GoldBase = 300,
            GoldPrice = 300,
            GoldSell = 210,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed",
                        },
            PercentAttackSpeedMod = 0.12f,
            Id = 1042
        };

        public static Item Recurve_Bow = new Item
        {
            Name = "Recurve Bow",
            From = new[]
                                {
                                1042, 1042,
                        },
            Into = new[]
                                {
                                3115, 3153,
                        },
            GoldBase = 400,
            GoldPrice = 1000,
            GoldSell = 700,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed", "OnHit",
                        },
            PercentAttackSpeedMod = 0.25f,
            Depth = 2,
            Id = 1043
        };

        public static Item Amplifying_Tome = new Item
        {
            Name = "Amplifying Tome",
            Into = new[]
                                {
                                3191, 3108, 3113, 3115, 3116, 3145, 3802, 4632, 3916, 4629, 4630, 4635, 4637, 4642,
                        },
            GoldBase = 435,
            GoldPrice = 435,
            GoldSell = 305,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage",
                        },
            FlatMagicDamageMod = 20f,
            Id = 1052
        };

        public static Item Vampiric_Scepter = new Item
        {
            Name = "Vampiric Scepter",
            From = new[]
                                {
                                1036,
                        },
            Into = new[]
                                {
                                3072, 3074, 3153, 3181, 4403, 6673, 6692,
                        },
            GoldBase = 550,
            GoldPrice = 900,
            GoldSell = 630,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "LifeSteal",
                        },
            FlatPhysicalDamageMod = 15f,
            PercentLifeStealMod = 0.1f,
            Depth = 2,
            Id = 1053
        };

        public static Item Dorans_Shield = new Item
        {
            Name = "Doran's Shield",
            GoldBase = 450,
            GoldPrice = 450,
            GoldSell = 180,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "Lane",
                        },
            FlatHPPoolMod = 80f,
            FlatHPRegenMod = 1.2f,
            Id = 1054
        };

        public static Item Dorans_Blade = new Item
        {
            Name = "Doran's Blade",
            GoldBase = 450,
            GoldPrice = 450,
            GoldSell = 180,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "SpellVamp", "Lane",
                        },
            FlatHPPoolMod = 80f,
            FlatPhysicalDamageMod = 8f,
            Id = 1055
        };

        public static Item Dorans_Ring = new Item
        {
            Name = "Doran's Ring",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Lane", "ManaRegen", "SpellDamage",
                        },
            FlatHPPoolMod = 70f,
            FlatMagicDamageMod = 15f,
            Id = 1056
        };

        public static Item Negatron_Cloak = new Item
        {
            Name = "Negatron Cloak",
            From = new[]
                                {
                                1033,
                        },
            Into = new[]
                                {
                                3091, 3001, 6664, 3222, 4401,
                        },
            GoldBase = 450,
            GoldPrice = 900,
            GoldSell = 630,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock",
                        },
            FlatSpellBlockMod = 50f,
            Depth = 2,
            Id = 1057
        };

        public static Item Needlessly_Large_Rod = new Item
        {
            Name = "Needlessly Large Rod",
            Into = new[]
                                {
                                3003, 3089, 4403, 4628,
                        },
            GoldBase = 1250,
            GoldPrice = 1250,
            GoldSell = 875,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage",
                        },
            FlatMagicDamageMod = 60f,
            Id = 1058
        };

        public static Item Dark_Seal = new Item
        {
            Name = "Dark Seal",
            Into = new[]
                                {
                                3041,
                        },
            GoldBase = 350,
            GoldPrice = 350,
            GoldSell = 140,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "Lane",
                        },
            FlatHPPoolMod = 40f,
            FlatMagicDamageMod = 15f,
            Id = 1082
        };

        public static Item Cull = new Item
        {
            Name = "Cull",
            GoldBase = 450,
            GoldPrice = 450,
            GoldSell = 180,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "LifeSteal", "Lane",
                        },
            FlatPhysicalDamageMod = 7f,
            Id = 1083
        };

        public static Item Health_Potion = new Item
        {
            Name = "Health Potion",
            GoldBase = 50,
            GoldPrice = 50,
            GoldSell = 20,
            Purchasable = true,
            Tags = new[]
                        {
                        "HealthRegen", "Consumable", "Lane", "Jungle",
                        },

            Stacks = 5,
            Consumed = true,
            Id = 2003
        };

        public static Item Total_Biscuit_of_Everlasting_Will = new Item
        {
            Name = "Total Biscuit of Everlasting Will",
            GoldBase = 75,
            GoldPrice = 75,
            GoldSell = 30,
            Purchasable = false,

            Stacks = 10,
            Consumed = true,
            InStore = false,
            HideFromAll = true,
            Id = 2010
        };

        public static Item Kircheis_Shard = new Item
        {
            Name = "Kircheis Shard",
            From = new[]
                                {
                                1042,
                        },
            Into = new[]
                                {
                                3094, 3095,
                        },
            GoldBase = 400,
            GoldPrice = 700,
            GoldSell = 490,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed",
                        },
            PercentAttackSpeedMod = 0.15f,
            Depth = 2,
            Id = 2015
        };

        public static Item Refillable_Potion = new Item
        {
            Name = "Refillable Potion",
            Into = new[]
                                {
                                2033,
                        },
            GoldBase = 150,
            GoldPrice = 150,
            GoldSell = 60,
            Purchasable = true,
            Tags = new[]
                        {
                        "HealthRegen", "Consumable", "Active", "Lane", "Jungle",
                        },

            Id = 2031
        };

        public static Item Corrupting_Potion = new Item
        {
            Name = "Corrupting Potion",
            From = new[]
                                {
                                2031,
                        },
            GoldBase = 350,
            GoldPrice = 500,
            GoldSell = 200,
            Purchasable = true,
            Tags = new[]
                        {
                        "Active", "Consumable", "HealthRegen", "Lane", "ManaRegen",
                        },

            Depth = 2,
            Id = 2033
        };

        public static Item Guardians_Horn = new Item
        {
            Name = "Guardian's Horn",
            GoldBase = 950,
            GoldPrice = 950,
            GoldSell = 665,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "Lane",
                        },
            FlatHPPoolMod = 150f,
            FlatHPRegenMod = 4f,
            Id = 2051
        };

        public static Item PoroSnax = new Item
        {
            Name = "Poro-Snax",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = false,

            Stacks = 2,
            Consumed = true,
            InStore = false,
            Id = 2052
        };

        public static Item Control_Ward = new Item
        {
            Name = "Control Ward",
            GoldBase = 75,
            GoldPrice = 75,
            GoldSell = 30,
            Purchasable = true,
            Tags = new[]
                        {
                        "Consumable", "Lane", "Stealth", "Vision",
                        },
            Range = 600f,
            Stacks = 2,
            Consumed = true,
            ConsumeOnFull = true,
            Id = 2055
        };

        public static Item Shurelyas_Battlesong = new Item
        {
            Name = "Shurelya's Battlesong",
            From = new[]
                                {
                                3067, 1004, 3066,
                        },
            GoldBase = 850,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Active", "CooldownReduction", "NonbootsMovement", "AbilityHaste",
                        },
            FlatHPPoolMod = 350f,
            PercentMovementSpeedMod = 0.05f,
            Depth = 3,
            Id = 2065
        };

        public static Item Elixir_of_Iron = new Item
        {
            Name = "Elixir of Iron",
            GoldBase = 500,
            GoldPrice = 500,
            GoldSell = 200,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Consumable", "NonbootsMovement", "Tenacity",
                        },

            Consumed = true,
            ConsumeOnFull = true,
            Id = 2138
        };

        public static Item Elixir_of_Sorcery = new Item
        {
            Name = "Elixir of Sorcery",
            GoldBase = 500,
            GoldPrice = 500,
            GoldSell = 200,
            Purchasable = true,
            Tags = new[]
                        {
                        "Consumable", "ManaRegen", "SpellDamage",
                        },

            Consumed = true,
            ConsumeOnFull = true,
            Id = 2139
        };

        public static Item Elixir_of_Wrath = new Item
        {
            Name = "Elixir of Wrath",
            GoldBase = 500,
            GoldPrice = 500,
            GoldSell = 200,
            Purchasable = true,
            Tags = new[]
                        {
                        "Consumable", "Damage", "LifeSteal", "SpellVamp",
                        },

            Consumed = true,
            ConsumeOnFull = true,
            Id = 2140
        };

        public static Item Minion_Dematerializer = new Item
        {
            Name = "Minion Dematerializer",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = false,

            Stacks = 10,
            Consumed = true,
            InStore = false,
            HideFromAll = true,
            Id = 2403
        };

        public static Item Commencing_Stopwatch = new Item
        {
            Name = "Commencing Stopwatch",
            Into = new[]
                                {
                                2420,
                        },
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = false,
            Tags = new[]
                        {
                        "Active",
                        },

            InStore = false,
            Id = 2419
        };

        public static Item Stopwatch = new Item
        {
            Name = "Stopwatch",
            Into = new[]
                                {
                                3026, 3157,
                        },
            GoldBase = 650,
            GoldPrice = 650,
            GoldSell = 260,
            Purchasable = true,
            Tags = new[]
                        {
                        "Active",
                        },

            Id = 2420
        };

        public static Item Broken_Stopwatch = new Item
        {
            Name = "Broken Stopwatch",
            Into = new[]
                                {
                                3157, 3026,
                        },
            GoldBase = 650,
            GoldPrice = 650,
            GoldSell = 260,
            Purchasable = true,

            HideFromAll = true,
            Id = 2421
        };

        public static Item Slightly_Magical_Footware = new Item
        {
            Name = "Slightly Magical Footware",
            Into = new[]
                                {
                                3006, 3047, 3020, 3158, 3111, 3117, 3009,
                        },
            GoldBase = 300,
            GoldPrice = 300,
            GoldSell = 210,
            Purchasable = false,
            Tags = new[]
                        {
                        "Boots",
                        },
            FlatMovementSpeedMod = 25f,
            InStore = false,
            Id = 2422
        };

        public static Item Perfectly_Timed_Stopwatch = new Item
        {
            Name = "Perfectly Timed Stopwatch",
            Into = new[]
                                {
                                3157, 3026,
                        },
            GoldBase = 650,
            GoldPrice = 650,
            GoldSell = 260,
            Purchasable = false,
            Tags = new[]
                        {
                        "Active",
                        },

            InStore = false,
            Id = 2423
        };

        public static Item Broken_Stopwatch2 = new Item
        {
            Name = "Broken Stopwatch",
            Into = new[]
                                {
                                3157, 3026,
                        },
            GoldBase = 650,
            GoldPrice = 650,
            GoldSell = 260,
            Purchasable = false,

            InStore = false,
            HideFromAll = true,
            Id = 2424
        };

        public static Item Abyssal_Mask = new Item
        {
            Name = "Abyssal Mask",
            From = new[]
                                {
                                1011, 1057,
                        },
            GoldBase = 900,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock",
                        },
            FlatHPPoolMod = 350f,
            FlatSpellBlockMod = 60f,
            Depth = 3,
            Id = 3001
        };

        public static Item Archangels_Staff = new Item
        {
            Name = "Archangel's Staff",
            From = new[]
                                {
                                3070, 1058, 1027,
                        },
            GoldBase = 1000,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "Mana", "Active",
                        },
            FlatMagicDamageMod = 60f,
            FlatMPPoolMod = 400f,
            Depth = 2,
            Id = 3003
        };

        public static Item Manamune = new Item
        {
            Name = "Manamune",
            From = new[]
                                {
                                3070, 3133, 1027,
                        },
            GoldBase = 750,
            GoldPrice = 2600,
            GoldSell = 1820,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "Mana", "ManaRegen", "CooldownReduction", "OnHit", "AbilityHaste",
                        },
            FlatMPPoolMod = 400f,
            FlatPhysicalDamageMod = 35f,
            Depth = 3,
            Id = 3004
        };

        public static Item Berserkers_Greaves = new Item
        {
            Name = "Berserker's Greaves",
            From = new[]
                                {
                                1001, 1042,
                        },
            GoldBase = 500,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed", "Boots",
                        },
            FlatMovementSpeedMod = 45f,
            PercentAttackSpeedMod = 0.35f,
            Depth = 2,
            Id = 3006
        };

        public static Item Boots_of_Swiftness = new Item
        {
            Name = "Boots of Swiftness",
            From = new[]
                                {
                                1001,
                        },
            GoldBase = 600,
            GoldPrice = 900,
            GoldSell = 630,
            Purchasable = true,
            Tags = new[]
                        {
                        "Boots",
                        },
            FlatMovementSpeedMod = 60f,
            Depth = 2,
            Id = 3009
        };

        public static Item Chemtech_Putrifier = new Item
        {
            Name = "Chemtech Putrifier",
            From = new[]
                                {
                                3916, 4642,
                        },
            GoldBase = 450,
            GoldPrice = 2300,
            GoldSell = 1610,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "ManaRegen", "CooldownReduction", "AbilityHaste",
                        },
            FlatMagicDamageMod = 45f,
            Depth = 3,
            Id = 3011
        };

        public static Item Sorcerers_Shoes = new Item
        {
            Name = "Sorcerer's Shoes",
            From = new[]
                                {
                                1001,
                        },
            GoldBase = 800,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "Boots", "MagicPenetration",
                        },
            FlatMovementSpeedMod = 45f,
            Depth = 2,
            Id = 3020
        };

        public static Item Glacial_Buckler = new Item
        {
            Name = "Glacial Buckler",
            From = new[]
                                {
                                1027, 1029,
                        },
            Into = new[]
                                {
                                3050, 3110,
                        },
            GoldBase = 250,
            GoldPrice = 900,
            GoldSell = 630,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor", "Mana", "CooldownReduction", "AbilityHaste",
                        },
            FlatArmorMod = 20f,
            FlatMPPoolMod = 250f,
            Depth = 2,
            Id = 3024
        };

        public static Item Guardian_Angel = new Item
        {
            Name = "Guardian Angel",
            From = new[]
                                {
                                1038, 1031, 2420,
                        },
            GoldBase = 50,
            GoldPrice = 2800,
            GoldSell = 1120,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor", "Damage",
                        },
            FlatArmorMod = 40f,
            FlatPhysicalDamageMod = 40f,
            Depth = 3,
            Id = 3026
        };

        public static Item Infinity_Edge = new Item
        {
            Name = "Infinity Edge",
            From = new[]
                                {
                                1038, 1037, 1018,
                        },
            GoldBase = 625,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "CriticalStrike", "Damage",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 70f,
            Depth = 2,
            Id = 3031
        };

        public static Item Mortal_Reminder = new Item
        {
            Name = "Mortal Reminder",
            From = new[]
                                {
                                3123, 3086,
                        },
            GoldBase = 700,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "AttackSpeed", "NonbootsMovement",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 20f,
            PercentAttackSpeedMod = 0.25f,
            PercentMovementSpeedMod = 0.07f,
            Depth = 3,
            Id = 3033
        };

        public static Item Last_Whisper = new Item
        {
            Name = "Last Whisper",
            From = new[]
                                {
                                1036, 1036,
                        },
            Into = new[]
                                {
                                3036, 6694,
                        },
            GoldBase = 750,
            GoldPrice = 1450,
            GoldSell = 1015,
            Purchasable = true,
            Tags = new[]
                        {
                        "ArmorPenetration", "Damage",
                        },
            FlatPhysicalDamageMod = 20f,
            Depth = 2,
            Id = 3035
        };

        public static Item Lord_Dominiks_Regards = new Item
        {
            Name = "Lord Dominik's Regards",
            From = new[]
                                {
                                3035, 1018,
                        },
            GoldBase = 850,
            GoldPrice = 2900,
            GoldSell = 2030,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "ArmorPenetration",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 30f,
            Depth = 3,
            Id = 3036
        };

        public static Item Seraphs_Embrace = new Item
        {
            Name = "Seraph's Embrace",
            GoldBase = 3000,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = false,
            Tags = new[]
                        {
                        "SpellDamage", "Mana",
                        },
            FlatMagicDamageMod = 60f,
            FlatMPPoolMod = 850f,
            SpecialRecipe = 3003,
            InStore = false,
            Id = 3040
        };

        public static Item Mejais_Soulstealer = new Item
        {
            Name = "Mejai's Soulstealer",
            From = new[]
                                {
                                1082,
                        },
            GoldBase = 1250,
            GoldPrice = 1600,
            GoldSell = 1120,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "NonbootsMovement",
                        },
            FlatHPPoolMod = 100f,
            FlatMagicDamageMod = 20f,
            Depth = 2,
            Id = 3041
        };

        public static Item Muramana = new Item
        {
            Name = "Muramana",
            GoldBase = 2600,
            GoldPrice = 2600,
            GoldSell = 1820,
            Purchasable = false,
            Tags = new[]
                        {
                        "Damage", "Mana", "CooldownReduction", "OnHit",
                        },
            FlatMPPoolMod = 850f,
            FlatPhysicalDamageMod = 35f,
            SpecialRecipe = 3004,
            InStore = false,
            Id = 3042
        };

        public static Item Muramana2 = new Item
        {
            Name = "Muramana",
            GoldBase = 2600,
            GoldPrice = 2600,
            GoldSell = 1820,
            Purchasable = false,
            Tags = new[]
                        {
                        "Damage", "Mana", "CooldownReduction", "OnHit",
                        },
            FlatMPPoolMod = 850f,
            FlatPhysicalDamageMod = 35f,
            SpecialRecipe = 3008,
            InStore = false,
            Id = 3043
        };

        public static Item Phage = new Item
        {
            Name = "Phage",
            From = new[]
                                {
                                1028, 1036,
                        },
            Into = new[]
                                {
                                6630, 3053, 6632,
                        },
            GoldBase = 350,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "Damage",
                        },
            FlatHPPoolMod = 200f,
            FlatPhysicalDamageMod = 15f,
            Depth = 2,
            Id = 3044
        };

        public static Item Phantom_Dancer = new Item
        {
            Name = "Phantom Dancer",
            From = new[]
                                {
                                1042, 3086, 1042,
                        },
            GoldBase = 1100,
            GoldPrice = 2900,
            GoldSell = 2030,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed", "CriticalStrike", "NonbootsMovement",
                        },
            FlatCritChanceMod = 0.2f,
            PercentAttackSpeedMod = 0.4f,
            PercentMovementSpeedMod = 0.07f,
            Depth = 3,
            Id = 3046
        };

        public static Item Plated_Steelcaps = new Item
        {
            Name = "Plated Steelcaps",
            From = new[]
                                {
                                1001, 1029,
                        },
            GoldBase = 500,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor", "Boots",
                        },
            FlatArmorMod = 20f,
            FlatMovementSpeedMod = 45f,
            Depth = 2,
            Id = 3047
        };

        public static Item Seraphs_Embrace2 = new Item
        {
            Name = "Seraph's Embrace",
            GoldBase = 3000,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = false,
            Tags = new[]
                        {
                        "SpellDamage", "Mana",
                        },
            FlatMagicDamageMod = 60f,
            FlatMPPoolMod = 850f,
            SpecialRecipe = 3007,
            InStore = false,
            Id = 3048
        };

        public static Item Zekes_Convergence = new Item
        {
            Name = "Zeke's Convergence",
            From = new[]
                                {
                                3067, 3024,
                        },
            GoldBase = 700,
            GoldPrice = 2400,
            GoldSell = 1680,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Armor", "Mana", "Active", "CooldownReduction", "OnHit", "AbilityHaste",
                        },
            FlatArmorMod = 30f,
            FlatHPPoolMod = 300f,
            FlatMPPoolMod = 250f,
            Depth = 3,
            Id = 3050
        };

        public static Item Hearthbound_Axe = new Item
        {
            Name = "Hearthbound Axe",
            From = new[]
                                {
                                1042, 1036,
                        },
            Into = new[]
                                {
                                3078, 6631, 3091,
                        },
            GoldBase = 450,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "AttackSpeed", "NonbootsMovement",
                        },
            FlatPhysicalDamageMod = 15f,
            PercentAttackSpeedMod = 0.15f,
            Depth = 2,
            Id = 3051
        };

        public static Item Steraks_Gage = new Item
        {
            Name = "Sterak's Gage",
            From = new[]
                                {
                                1037, 3044, 1028,
                        },
            GoldBase = 725,
            GoldPrice = 3100,
            GoldSell = 2170,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage",
                        },
            FlatHPPoolMod = 400f,
            FlatPhysicalDamageMod = 50f,
            Depth = 3,
            Id = 3053
        };

        public static Item Sheen = new Item
        {
            Name = "Sheen",
            Into = new[]
                                {
                                3078, 3100, 3508, 6632,
                        },
            GoldBase = 700,
            GoldPrice = 700,
            GoldSell = 490,
            Purchasable = true,

            Id = 3057
        };

        public static Item Spirit_Visage = new Item
        {
            Name = "Spirit Visage",
            From = new[]
                                {
                                3211, 3067,
                        },
            GoldBase = 850,
            GoldPrice = 2900,
            GoldSell = 2030,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock", "HealthRegen", "CooldownReduction", "AbilityHaste",
                        },
            FlatHPPoolMod = 450f,
            FlatSpellBlockMod = 40f,
            Depth = 3,
            Id = 3065
        };

        public static Item Winged_Moonplate = new Item
        {
            Name = "Winged Moonplate",
            From = new[]
                                {
                                1028,
                        },
            Into = new[]
                                {
                                2065, 3742, 4401,
                        },
            GoldBase = 400,
            GoldPrice = 800,
            GoldSell = 560,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "NonbootsMovement",
                        },
            FlatHPPoolMod = 150f,
            Depth = 2,
            Id = 3066
        };

        public static Item Kindlegem = new Item
        {
            Name = "Kindlegem",
            From = new[]
                                {
                                1028,
                        },
            Into = new[]
                                {
                                3065, 2065, 3190, 6617, 3071, 6630, 3050, 3078, 3083, 3107, 3109, 3143, 4005, 4403, 4629, 6631, 6632,
                        },
            GoldBase = 400,
            GoldPrice = 800,
            GoldSell = 560,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "CooldownReduction", "AbilityHaste",
                        },
            FlatHPPoolMod = 200f,
            Depth = 2,
            Id = 3067
        };

        public static Item Sunfire_Aegis = new Item
        {
            Name = "Sunfire Aegis",
            From = new[]
                                {
                                6660, 3105,
                        },
            GoldBase = 700,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock", "Armor", "Aura", "CooldownReduction", "AbilityHaste",
                        },
            FlatArmorMod = 30f,
            FlatHPPoolMod = 450f,
            FlatSpellBlockMod = 30f,
            Depth = 3,
            Id = 3068
        };

        public static Item Tear_of_the_Goddess = new Item
        {
            Name = "Tear of the Goddess",
            Into = new[]
                                {
                                3003, 3004,
                        },
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 280,
            Purchasable = true,
            Tags = new[]
                        {
                        "Mana", "ManaRegen",
                        },
            FlatMPPoolMod = 150f,
            Id = 3070
        };

        public static Item Black_Cleaver = new Item
        {
            Name = "Black Cleaver",
            From = new[]
                                {
                                3133, 3067, 1036,
                        },
            GoldBase = 1050,
            GoldPrice = 3300,
            GoldSell = 2310,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "CooldownReduction", "OnHit", "ArmorPenetration", "AbilityHaste",
                        },
            FlatHPPoolMod = 300f,
            FlatPhysicalDamageMod = 40f,
            Depth = 3,
            Id = 3071
        };

        public static Item Bloodthirster = new Item
        {
            Name = "Bloodthirster",
            From = new[]
                                {
                                1038, 1018, 1053,
                        },
            GoldBase = 600,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "LifeSteal",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 55f,
            PercentLifeStealMod = 0.2f,
            Depth = 3,
            Id = 3072
        };

        public static Item Ravenous_Hydra = new Item
        {
            Name = "Ravenous Hydra",
            From = new[]
                                {
                                3077, 1053, 3133,
                        },
            GoldBase = 100,
            GoldPrice = 3300,
            GoldSell = 2310,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "LifeSteal", "CooldownReduction", "SpellVamp", "AbilityHaste",
                        },
            FlatPhysicalDamageMod = 65f,
            Depth = 3,
            Id = 3074
        };

        public static Item Thornmail = new Item
        {
            Name = "Thornmail",
            From = new[]
                                {
                                3076, 1011,
                        },
            GoldBase = 1000,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Armor",
                        },
            FlatArmorMod = 60f,
            FlatHPPoolMod = 350f,
            Depth = 3,
            Id = 3075
        };

        public static Item Bramble_Vest = new Item
        {
            Name = "Bramble Vest",
            From = new[]
                                {
                                1029, 1029,
                        },
            Into = new[]
                                {
                                3075,
                        },
            GoldBase = 200,
            GoldPrice = 800,
            GoldSell = 560,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor",
                        },
            FlatArmorMod = 35f,
            Depth = 2,
            Id = 3076
        };

        public static Item Tiamat = new Item
        {
            Name = "Tiamat",
            From = new[]
                                {
                                1037,
                        },
            Into = new[]
                                {
                                3074, 3748,
                        },
            GoldBase = 325,
            GoldPrice = 1200,
            GoldSell = 840,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage",
                        },
            FlatPhysicalDamageMod = 25f,
            Depth = 2,
            Id = 3077
        };

        public static Item Trinity_Force = new Item
        {
            Name = "Trinity Force",
            From = new[]
                                {
                                3057, 3051, 3067,
                        },
            GoldBase = 733,
            GoldPrice = 3333,
            GoldSell = 2333,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "AttackSpeed", "CooldownReduction", "NonbootsMovement", "AbilityHaste",
                        },
            FlatHPPoolMod = 200f,
            FlatPhysicalDamageMod = 35f,
            PercentAttackSpeedMod = 0.35f,
            Depth = 3,
            Id = 3078
        };

        public static Item Wardens_Mail = new Item
        {
            Name = "Warden's Mail",
            From = new[]
                                {
                                1029, 1029,
                        },
            Into = new[]
                                {
                                3110, 3143,
                        },
            GoldBase = 400,
            GoldPrice = 1000,
            GoldSell = 700,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor",
                        },
            FlatArmorMod = 40f,
            Depth = 2,
            Id = 3082
        };

        public static Item Warmogs_Armor = new Item
        {
            Name = "Warmog's Armor",
            From = new[]
                                {
                                1011, 3067, 3801,
                        },
            GoldBase = 650,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "CooldownReduction", "AbilityHaste",
                        },
            FlatHPPoolMod = 800f,
            Depth = 3,
            Id = 3083
        };

        public static Item Runaans_Hurricane = new Item
        {
            Name = "Runaan's Hurricane",
            From = new[]
                                {
                                1037, 3086, 1042,
                        },
            GoldBase = 1025,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "AttackSpeed", "OnHit", "NonbootsMovement",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 25f,
            PercentAttackSpeedMod = 0.4f,
            PercentMovementSpeedMod = 0.07f,
            Depth = 3,
            Id = 3085
        };

        public static Item Zeal = new Item
        {
            Name = "Zeal",
            From = new[]
                                {
                                1018, 1042,
                        },
            Into = new[]
                                {
                                3085, 3033, 3046, 3094, 4403,
                        },
            GoldBase = 300,
            GoldPrice = 1200,
            GoldSell = 840,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed", "CriticalStrike", "NonbootsMovement",
                        },
            FlatCritChanceMod = 0.15f,
            PercentAttackSpeedMod = 0.18f,
            Depth = 2,
            Id = 3086
        };

        public static Item Rabadons_Deathcap = new Item
        {
            Name = "Rabadon's Deathcap",
            From = new[]
                                {
                                1058, 1058,
                        },
            GoldBase = 1300,
            GoldPrice = 3800,
            GoldSell = 2660,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage",
                        },
            FlatMagicDamageMod = 120f,
            Depth = 2,
            Id = 3089
        };

        public static Item Wits_End = new Item
        {
            Name = "Wit's End",
            From = new[]
                                {
                                3051, 1057, 1042,
                        },
            GoldBase = 800,
            GoldPrice = 3100,
            GoldSell = 2170,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock", "Damage", "AttackSpeed", "OnHit", "NonbootsMovement",
                        },
            FlatPhysicalDamageMod = 30f,
            FlatSpellBlockMod = 50f,
            PercentAttackSpeedMod = 0.4f,
            Depth = 3,
            Id = 3091
        };

        public static Item Rapid_Firecannon = new Item
        {
            Name = "Rapid Firecannon",
            From = new[]
                                {
                                3086, 2015,
                        },
            GoldBase = 800,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "CriticalStrike", "AttackSpeed", "NonbootsMovement",
                        },
            FlatCritChanceMod = 0.2f,
            PercentAttackSpeedMod = 0.35f,
            PercentMovementSpeedMod = 0.07f,
            Depth = 3,
            Id = 3094
        };

        public static Item Stormrazor = new Item
        {
            Name = "Stormrazor",
            From = new[]
                                {
                                1038, 1018, 2015,
                        },
            GoldBase = 100,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "AttackSpeed", "Slow",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 40f,
            PercentAttackSpeedMod = 0.15f,
            Depth = 3,
            Id = 3095
        };

        public static Item Lich_Bane = new Item
        {
            Name = "Lich Bane",
            From = new[]
                                {
                                3057, 3113, 1026,
                        },
            GoldBase = 600,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "NonbootsMovement",
                        },
            FlatMagicDamageMod = 80f,
            PercentMovementSpeedMod = 0.1f,
            Depth = 3,
            Id = 3100
        };

        public static Item Banshees_Veil = new Item
        {
            Name = "Banshee's Veil",
            From = new[]
                                {
                                3108, 4632,
                        },
            GoldBase = 400,
            GoldPrice = 2500,
            GoldSell = 1750,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock", "SpellDamage", "CooldownReduction", "AbilityHaste",
                        },
            FlatMagicDamageMod = 65f,
            FlatSpellBlockMod = 45f,
            Depth = 3,
            Id = 3102
        };

        public static Item Aegis_of_the_Legion = new Item
        {
            Name = "Aegis of the Legion",
            From = new[]
                                {
                                1033, 1029,
                        },
            Into = new[]
                                {
                                3190, 3193, 3068, 4403,
                        },
            GoldBase = 750,
            GoldPrice = 1500,
            GoldSell = 1050,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock", "Armor", "CooldownReduction", "AbilityHaste",
                        },
            FlatArmorMod = 30f,
            FlatSpellBlockMod = 30f,
            Depth = 2,
            Id = 3105
        };

        public static Item Redemption = new Item
        {
            Name = "Redemption",
            From = new[]
                                {
                                3067, 3114,
                        },
            GoldBase = 700,
            GoldPrice = 2300,
            GoldSell = 1610,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "ManaRegen", "CooldownReduction", "AbilityHaste",
                        },
            FlatHPPoolMod = 200f,
            Depth = 3,
            Id = 3107
        };

        public static Item Fiendish_Codex = new Item
        {
            Name = "Fiendish Codex",
            From = new[]
                                {
                                1052,
                        },
            Into = new[]
                                {
                                3102, 6653, 3157, 4629,
                        },
            GoldBase = 465,
            GoldPrice = 900,
            GoldSell = 630,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "CooldownReduction", "AbilityHaste",
                        },
            FlatMagicDamageMod = 35f,
            Depth = 2,
            Id = 3108
        };

        public static Item Knights_Vow = new Item
        {
            Name = "Knight's Vow",
            From = new[]
                                {
                                3801, 1006, 3067,
                        },
            GoldBase = 700,
            GoldPrice = 2300,
            GoldSell = 1610,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "Aura", "Active", "CooldownReduction", "NonbootsMovement", "AbilityHaste",
                        },
            FlatHPPoolMod = 400f,
            Depth = 3,
            Id = 3109
        };

        public static Item Frozen_Heart = new Item
        {
            Name = "Frozen Heart",
            From = new[]
                                {
                                3082, 3024,
                        },
            GoldBase = 800,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor", "Mana", "Aura", "CooldownReduction", "AbilityHaste",
                        },
            FlatArmorMod = 80f,
            FlatMPPoolMod = 400f,
            Depth = 3,
            Id = 3110
        };

        public static Item Mercurys_Treads = new Item
        {
            Name = "Mercury's Treads",
            From = new[]
                                {
                                1001, 1033,
                        },
            GoldBase = 350,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "Boots", "SpellBlock", "Tenacity",
                        },
            FlatMovementSpeedMod = 45f,
            FlatSpellBlockMod = 25f,
            Depth = 2,
            Id = 3111
        };

        public static Item Guardians_Orb = new Item
        {
            Name = "Guardian's Orb",
            GoldBase = 950,
            GoldPrice = 950,
            GoldSell = 665,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "ManaRegen", "Lane",
                        },
            FlatHPPoolMod = 150f,
            FlatMagicDamageMod = 40f,
            Id = 3112
        };

        public static Item Aether_Wisp = new Item
        {
            Name = "Aether Wisp",
            From = new[]
                                {
                                1052,
                        },
            Into = new[]
                                {
                                3100,
                        },
            GoldBase = 415,
            GoldPrice = 850,
            GoldSell = 595,
            Purchasable = true,
            Tags = new[]
                        {
                        "NonbootsMovement", "SpellDamage",
                        },
            FlatMagicDamageMod = 30f,
            Depth = 2,
            Id = 3113
        };

        public static Item Forbidden_Idol = new Item
        {
            Name = "Forbidden Idol",
            From = new[]
                                {
                                1004,
                        },
            Into = new[]
                                {
                                6616, 3107, 3222, 3504,
                        },
            GoldBase = 550,
            GoldPrice = 800,
            GoldSell = 560,
            Purchasable = true,
            Tags = new[]
                        {
                        "ManaRegen",
                        },

            Depth = 2,
            Id = 3114
        };

        public static Item Nashors_Tooth = new Item
        {
            Name = "Nashor's Tooth",
            From = new[]
                                {
                                1043, 1026, 1052,
                        },
            GoldBase = 715,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed", "SpellDamage", "OnHit",
                        },
            FlatMagicDamageMod = 100f,
            PercentAttackSpeedMod = 0.5f,
            Depth = 3,
            Id = 3115
        };

        public static Item Rylais_Crystal_Scepter = new Item
        {
            Name = "Rylai's Crystal Scepter",
            From = new[]
                                {
                                1026, 1011, 1052,
                        },
            GoldBase = 815,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "Slow",
                        },
            FlatHPPoolMod = 350f,
            FlatMagicDamageMod = 90f,
            Depth = 3,
            Id = 3116
        };

        public static Item Mobility_Boots = new Item
        {
            Name = "Mobility Boots",
            From = new[]
                                {
                                1001,
                        },
            GoldBase = 700,
            GoldPrice = 1000,
            GoldSell = 700,
            Purchasable = true,
            Tags = new[]
                        {
                        "Boots",
                        },
            FlatMovementSpeedMod = 115f,
            Depth = 2,
            Id = 3117
        };

        public static Item Executioners_Calling = new Item
        {
            Name = "Executioner's Calling",
            From = new[]
                                {
                                1036,
                        },
            Into = new[]
                                {
                                6609, 3033,
                        },
            GoldBase = 450,
            GoldPrice = 800,
            GoldSell = 560,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage",
                        },
            FlatPhysicalDamageMod = 15f,
            Depth = 2,
            Id = 3123
        };

        public static Item Guinsoos_Rageblade = new Item
        {
            Name = "Guinsoo's Rageblade",
            From = new[]
                                {
                                6677, 1018, 1042,
                        },
            GoldBase = 900,
            GoldPrice = 2600,
            GoldSell = 1820,
            Purchasable = true,
            Tags = new[]
                        {
                        "CriticalStrike", "AttackSpeed", "OnHit",
                        },
            FlatCritChanceMod = 0.2f,
            PercentAttackSpeedMod = 0.4f,
            Depth = 3,
            Id = 3124
        };

        public static Item Caulfields_Warhammer = new Item
        {
            Name = "Caulfield's Warhammer",
            From = new[]
                                {
                                1036, 1036,
                        },
            Into = new[]
                                {
                                6609, 3179, 3071, 3004, 3074, 3156, 3508, 6333, 6675, 6691, 6693, 6694,
                        },
            GoldBase = 400,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CooldownReduction", "AbilityHaste",
                        },
            FlatPhysicalDamageMod = 25f,
            Depth = 2,
            Stacks = 0,
            Id = 3133
        };

        public static Item Serrated_Dirk = new Item
        {
            Name = "Serrated Dirk",
            From = new[]
                                {
                                1036, 1036,
                        },
            Into = new[]
                                {
                                3142, 3179, 6676, 3814, 3181, 6691, 6692, 6693, 6695,
                        },
            GoldBase = 400,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "ArmorPenetration",
                        },
            FlatPhysicalDamageMod = 30f,
            Depth = 2,
            Stacks = 0,
            Id = 3134
        };

        public static Item Void_Staff = new Item
        {
            Name = "Void Staff",
            From = new[]
                                {
                                4630, 1026,
                        },
            GoldBase = 400,
            GoldPrice = 2500,
            GoldSell = 1750,
            Purchasable = true,
            Tags = new[]
                        {
                        "MagicPenetration", "SpellDamage",
                        },
            FlatMagicDamageMod = 65f,
            Depth = 3,
            Id = 3135
        };

        public static Item Mercurial_Scimitar = new Item
        {
            Name = "Mercurial Scimitar",
            From = new[]
                                {
                                3140, 1018, 1037,
                        },
            GoldBase = 125,
            GoldPrice = 2900,
            GoldSell = 2030,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock", "Damage", "CriticalStrike", "Active", "NonbootsMovement", "Tenacity",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 30f,
            FlatSpellBlockMod = 30f,
            Depth = 3,
            Id = 3139
        };

        public static Item Quicksilver_Sash = new Item
        {
            Name = "Quicksilver Sash",
            From = new[]
                                {
                                1033,
                        },
            Into = new[]
                                {
                                6035, 3139,
                        },
            GoldBase = 850,
            GoldPrice = 1300,
            GoldSell = 910,
            Purchasable = true,
            Tags = new[]
                        {
                        "Active", "SpellBlock",
                        },
            FlatSpellBlockMod = 30f,
            Depth = 2,
            Id = 3140
        };

        public static Item Youmuus_Ghostblade = new Item
        {
            Name = "Youmuu's Ghostblade",
            From = new[]
                                {
                                3134, 1037, 1036,
                        },
            GoldBase = 675,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "Active", "NonbootsMovement", "ArmorPenetration",
                        },
            FlatPhysicalDamageMod = 60f,
            Depth = 3,
            Id = 3142
        };

        public static Item Randuins_Omen = new Item
        {
            Name = "Randuin's Omen",
            From = new[]
                                {
                                3082, 1029, 3067,
                        },
            GoldBase = 600,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Armor", "Active", "CooldownReduction", "Slow", "AbilityHaste",
                        },
            Range = 500f,
            FlatArmorMod = 80f,
            FlatHPPoolMod = 250f,
            Depth = 3,
            Id = 3143
        };

        public static Item Hextech_Alternator = new Item
        {
            Name = "Hextech Alternator",
            From = new[]
                                {
                                1052, 1052,
                        },
            Into = new[]
                                {
                                3152, 4628, 4636,
                        },
            GoldBase = 180,
            GoldPrice = 1050,
            GoldSell = 735,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage",
                        },
            FlatMagicDamageMod = 40f,
            Depth = 2,
            Id = 3145
        };

        public static Item Hextech_Rocketbelt = new Item
        {
            Name = "Hextech Rocketbelt",
            From = new[]
                                {
                                3145, 1028, 1026,
                        },
            GoldBase = 900,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "Active", "CooldownReduction", "NonbootsMovement", "MagicPenetration", "AbilityHaste",
                        },
            FlatHPPoolMod = 250f,
            FlatMagicDamageMod = 80f,
            Depth = 3,
            Id = 3152
        };

        public static Item Blade_of_The_Ruined_King = new Item
        {
            Name = "Blade of The Ruined King",
            From = new[]
                                {
                                1053, 1043, 1037,
                        },
            GoldBase = 325,
            GoldPrice = 3100,
            GoldSell = 2170,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "AttackSpeed", "LifeSteal", "Slow", "OnHit",
                        },
            FlatPhysicalDamageMod = 40f,
            PercentAttackSpeedMod = 0.3f,
            PercentLifeStealMod = 0.12f,
            Depth = 3,
            Id = 3153
        };

        public static Item Hexdrinker = new Item
        {
            Name = "Hexdrinker",
            From = new[]
                                {
                                1036, 1033,
                        },
            Into = new[]
                                {
                                3156,
                        },
            GoldBase = 500,
            GoldPrice = 1300,
            GoldSell = 910,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "SpellBlock",
                        },
            FlatPhysicalDamageMod = 20f,
            FlatSpellBlockMod = 35f,
            Depth = 2,
            Stacks = 0,
            Id = 3155
        };

        public static Item Maw_of_Malmortius = new Item
        {
            Name = "Maw of Malmortius",
            From = new[]
                                {
                                3155, 3133,
                        },
            GoldBase = 700,
            GoldPrice = 3100,
            GoldSell = 2170,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock", "Damage", "CooldownReduction", "AbilityHaste",
                        },
            FlatPhysicalDamageMod = 50f,
            FlatSpellBlockMod = 50f,
            Depth = 3,
            Stacks = 0,
            Id = 3156
        };

        public static Item Zhonyas_Hourglass = new Item
        {
            Name = "Zhonya's Hourglass",
            From = new[]
                                {
                                3191, 2420, 3108,
                        },
            GoldBase = 50,
            GoldPrice = 2500,
            GoldSell = 1750,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor", "SpellDamage", "Active", "CooldownReduction", "AbilityHaste",
                        },
            FlatArmorMod = 45f,
            FlatMagicDamageMod = 65f,
            Depth = 3,
            Id = 3157
        };

        public static Item Ionian_Boots_of_Lucidity = new Item
        {
            Name = "Ionian Boots of Lucidity",
            From = new[]
                                {
                                1001,
                        },
            GoldBase = 600,
            GoldPrice = 900,
            GoldSell = 630,
            Purchasable = true,
            Tags = new[]
                        {
                        "Boots", "CooldownReduction",
                        },
            FlatMovementSpeedMod = 45f,
            Depth = 2,
            Id = 3158
        };

        public static Item Morellonomicon = new Item
        {
            Name = "Morellonomicon",
            From = new[]
                                {
                                1026, 3916, 1028,
                        },
            GoldBase = 450,
            GoldPrice = 2500,
            GoldSell = 1750,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage",
                        },
            FlatHPPoolMod = 250f,
            FlatMagicDamageMod = 70f,
            Depth = 3,
            Id = 3165
        };

        public static Item Guardians_Blade = new Item
        {
            Name = "Guardian's Blade",
            GoldBase = 950,
            GoldPrice = 950,
            GoldSell = 665,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "Lane", "AbilityHaste",
                        },
            FlatHPPoolMod = 150f,
            FlatPhysicalDamageMod = 30f,
            Id = 3177
        };

        public static Item Umbral_Glaive = new Item
        {
            Name = "Umbral Glaive",
            From = new[]
                                {
                                3134, 3133,
                        },
            GoldBase = 600,
            GoldPrice = 2800,
            GoldSell = 1960,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "Vision", "CooldownReduction", "ArmorPenetration", "AbilityHaste",
                        },
            FlatPhysicalDamageMod = 50f,
            Depth = 3,
            Id = 3179
        };

        public static Item Sanguine_Blade = new Item
        {
            Name = "Sanguine Blade",
            From = new[]
                                {
                                3134, 1053,
                        },
            GoldBase = 1000,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "AttackSpeed", "LifeSteal", "SpellVamp", "ArmorPenetration",
                        },
            FlatPhysicalDamageMod = 50f,
            Depth = 3,
            Id = 3181
        };

        public static Item Guardians_Hammer = new Item
        {
            Name = "Guardian's Hammer",
            GoldBase = 950,
            GoldPrice = 950,
            GoldSell = 665,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "LifeSteal", "Lane",
                        },
            FlatHPPoolMod = 150f,
            FlatPhysicalDamageMod = 25f,
            PercentLifeStealMod = 0.1f,
            Id = 3184
        };

        public static Item Locket_of_the_Iron_Solari = new Item
        {
            Name = "Locket of the Iron Solari",
            From = new[]
                                {
                                3067, 3105,
                        },
            GoldBase = 400,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock", "Armor", "Aura", "Active", "CooldownReduction", "AbilityHaste",
                        },
            Range = 600f,
            FlatArmorMod = 30f,
            FlatHPPoolMod = 200f,
            FlatSpellBlockMod = 30f,
            Depth = 3,
            Id = 3190
        };

        public static Item Seekers_Armguard = new Item
        {
            Name = "Seeker's Armguard",
            From = new[]
                                {
                                1052, 1029,
                        },
            Into = new[]
                                {
                                3157,
                        },
            GoldBase = 165,
            GoldPrice = 900,
            GoldSell = 630,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor", "SpellDamage",
                        },
            FlatArmorMod = 15f,
            FlatMagicDamageMod = 30f,
            Depth = 2,
            Id = 3191
        };

        public static Item Gargoyle_Stoneplate = new Item
        {
            Name = "Gargoyle Stoneplate",
            From = new[]
                                {
                                1029, 3105, 1033,
                        },
            GoldBase = 1050,
            GoldPrice = 3300,
            GoldSell = 2310,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock", "Armor", "Active", "CooldownReduction", "AbilityHaste",
                        },
            FlatArmorMod = 60f,
            FlatSpellBlockMod = 60f,
            Depth = 3,
            Id = 3193
        };

        public static Item Spectres_Cowl = new Item
        {
            Name = "Spectre's Cowl",
            From = new[]
                                {
                                1028, 1033,
                        },
            Into = new[]
                                {
                                3065,
                        },
            GoldBase = 400,
            GoldPrice = 1250,
            GoldSell = 875,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "SpellBlock",
                        },
            FlatHPPoolMod = 250f,
            FlatSpellBlockMod = 25f,
            Depth = 2,
            Id = 3211
        };

        public static Item Mikaels_Blessing = new Item
        {
            Name = "Mikael's Blessing",
            From = new[]
                                {
                                3114, 1057,
                        },
            GoldBase = 600,
            GoldPrice = 2300,
            GoldSell = 1610,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock", "ManaRegen", "Active", "CooldownReduction", "Tenacity", "AbilityHaste",
                        },
            FlatSpellBlockMod = 50f,
            Depth = 3,
            Id = 3222
        };

        public static Item Scarecrow_Effigy = new Item
        {
            Name = "Scarecrow Effigy",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = true,
            Tags = new[]
                        {
                        "Active", "Jungle", "Lane", "Trinket", "Vision",
                        },
            Range = 600f,
            RequiredChampion = "FiddleSticks",
            Id = 3330
        };

        public static Item Stealth_Ward = new Item
        {
            Name = "Stealth Ward",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = true,
            Tags = new[]
                        {
                        "Active", "Jungle", "Lane", "Trinket", "Vision",
                        },
            Range = 600f,
            Id = 3340
        };

        public static Item Farsight_Alteration = new Item
        {
            Name = "Farsight Alteration",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = true,
            Tags = new[]
                        {
                        "Active", "Trinket", "Vision",
                        },
            Range = 4000f,
            Id = 3363
        };

        public static Item Oracle_Lens = new Item
        {
            Name = "Oracle Lens",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = true,
            Tags = new[]
                        {
                        "Active", "Trinket", "Vision",
                        },
            Range = 600f,
            Id = 3364
        };

        public static Item Your_Cut = new Item
        {
            Name = "Your Cut",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = false,
            Tags = new[]
                        {
                        "Consumable", "GoldPer",
                        },

            Consumed = true,
            InStore = false,
            HideFromAll = true,
            Id = 3400
        };

        public static Item Ardent_Censer = new Item
        {
            Name = "Ardent Censer",
            From = new[]
                                {
                                3114, 1026,
                        },
            GoldBase = 650,
            GoldPrice = 2300,
            GoldSell = 1610,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed", "SpellDamage", "ManaRegen", "OnHit",
                        },
            FlatMagicDamageMod = 60f,
            Depth = 3,
            Id = 3504
        };

        public static Item Essence_Reaver = new Item
        {
            Name = "Essence Reaver",
            From = new[]
                                {
                                3133, 3057, 1018,
                        },
            GoldBase = 500,
            GoldPrice = 2900,
            GoldSell = 2030,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "ManaRegen", "CooldownReduction", "AbilityHaste",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 40f,
            Depth = 3,
            Id = 3508
        };

        public static Item Eye_of_the_Herald = new Item
        {
            Name = "Eye of the Herald",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = false,
            Tags = new[]
                        {
                        "Trinket", "Active",
                        },
            Range = 600,
            Consumed = true,
            InStore = false,
            Id = 3513
        };

        public static Item Kalistas_Black_Spear = new Item
        {
            Name = "Kalista's Black Spear",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = true,
            Tags = new[]
                        {
                        "Consumable",
                        },

            RequiredChampion = "Kalista",
            Id = 3599
        };

        public static Item Kalistas_Black_Spear_Sylas = new Item
        {
            Name = "Kalista's Black Spear",
            GoldBase = 0,
            GoldPrice = 0,
            GoldSell = 0,
            Purchasable = true,
            Tags = new[]
                        {
                        "Consumable",
                        },

            RequiredChampion = "Sylas",
            Id = 3600
        };

        public static Item Dead_Mans_Plate = new Item
        {
            Name = "Dead Man's Plate",
            From = new[]
                                {
                                3066, 1028, 1031,
                        },
            GoldBase = 900,
            GoldPrice = 2900,
            GoldSell = 2030,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Armor", "Slow", "NonbootsMovement",
                        },
            FlatArmorMod = 40f,
            FlatHPPoolMod = 475f,
            PercentMovementSpeedMod = 0.05f,
            Depth = 3,
            Id = 3742
        };

        public static Item Titanic_Hydra = new Item
        {
            Name = "Titanic Hydra",
            From = new[]
                                {
                                3077, 1028, 1011,
                        },
            GoldBase = 800,
            GoldPrice = 3300,
            GoldSell = 2310,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "Damage", "OnHit",
                        },
            FlatHPPoolMod = 500f,
            FlatPhysicalDamageMod = 30f,
            Depth = 3,
            Id = 3748
        };

        public static Item Crystalline_Bracer = new Item
        {
            Name = "Crystalline Bracer",
            From = new[]
                                {
                                1028, 1006,
                        },
            Into = new[]
                                {
                                3083, 3109,
                        },
            GoldBase = 100,
            GoldPrice = 650,
            GoldSell = 455,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen",
                        },
            FlatHPPoolMod = 200f,
            Depth = 2,
            Id = 3801
        };

        public static Item Lost_Chapter = new Item
        {
            Name = "Lost Chapter",
            From = new[]
                                {
                                1052, 1027, 1052,
                        },
            Into = new[]
                                {
                                6655, 6653, 6656,
                        },
            GoldBase = 80,
            GoldPrice = 1300,
            GoldSell = 910,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "Mana", "ManaRegen", "CooldownReduction", "AbilityHaste",
                        },
            FlatMagicDamageMod = 40f,
            FlatMPPoolMod = 300f,
            Depth = 2,
            Id = 3802
        };

        public static Item Edge_of_Night = new Item
        {
            Name = "Edge of Night",
            From = new[]
                                {
                                1036, 3134, 1028,
                        },
            GoldBase = 1050,
            GoldPrice = 2900,
            GoldSell = 2030,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "ArmorPenetration",
                        },
            FlatHPPoolMod = 325f,
            FlatPhysicalDamageMod = 50f,
            Depth = 3,
            Stacks = 0,
            Id = 3814
        };

        public static Item Spellthiefs_Edge = new Item
        {
            Name = "Spellthief's Edge",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "ManaRegen", "Vision", "GoldPer", "Lane",
                        },
            FlatHPPoolMod = 10f,
            FlatMagicDamageMod = 8f,
            Id = 3850
        };

        public static Item Frostfang = new Item
        {
            Name = "Frostfang",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = false,
            Tags = new[]
                        {
                        "GoldPer", "Lane", "ManaRegen", "SpellDamage",
                        },
            Range = 600f,
            FlatHPPoolMod = 70f,
            FlatMagicDamageMod = 15f,
            SpecialRecipe = 3850,
            InStore = false,
            Id = 3851
        };

        public static Item Shard_of_True_Ice = new Item
        {
            Name = "Shard of True Ice",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = false,
            Tags = new[]
                        {
                        "GoldPer", "Lane", "ManaRegen", "SpellDamage",
                        },
            Range = 600f,
            FlatHPPoolMod = 75f,
            FlatMagicDamageMod = 40f,
            SpecialRecipe = 3851,
            InStore = false,
            Id = 3853
        };

        public static Item Steel_Shoulderguards = new Item
        {
            Name = "Steel Shoulderguards",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "Damage", "Vision", "GoldPer", "Lane",
                        },
            FlatHPPoolMod = 30f,
            FlatPhysicalDamageMod = 3f,
            Id = 3854
        };

        public static Item Runesteel_Spaulders = new Item
        {
            Name = "Runesteel Spaulders",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = false,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "GoldPer", "Lane",
                        },
            Range = 600f,
            FlatHPPoolMod = 100f,
            FlatPhysicalDamageMod = 6f,
            SpecialRecipe = 3854,
            InStore = false,
            Id = 3855
        };

        public static Item Pauldrons_of_Whiterock = new Item
        {
            Name = "Pauldrons of Whiterock",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = false,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "GoldPer", "Lane",
                        },
            Range = 600f,
            FlatHPPoolMod = 250f,
            FlatPhysicalDamageMod = 15f,
            SpecialRecipe = 3855,
            InStore = false,
            Id = 3857
        };

        public static Item Relic_Shield = new Item
        {
            Name = "Relic Shield",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "SpellDamage", "Vision", "GoldPer", "Lane",
                        },
            FlatHPPoolMod = 30f,
            FlatMagicDamageMod = 5f,
            Id = 3858
        };

        public static Item Targons_Buckler = new Item
        {
            Name = "Targon's Buckler",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = false,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "GoldPer", "Lane",
                        },
            Range = 600f,
            FlatHPPoolMod = 100f,
            FlatMagicDamageMod = 10f,
            SpecialRecipe = 3858,
            InStore = false,
            Id = 3859
        };

        public static Item Bulwark_of_the_Mountain = new Item
        {
            Name = "Bulwark of the Mountain",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = false,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "GoldPer", "Lane",
                        },
            Range = 600f,
            FlatHPPoolMod = 250f,
            FlatMagicDamageMod = 20f,
            SpecialRecipe = 3859,
            InStore = false,
            Id = 3860
        };

        public static Item Spectral_Sickle = new Item
        {
            Name = "Spectral Sickle",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "ManaRegen", "Vision", "GoldPer", "Lane",
                        },
            FlatHPPoolMod = 10f,
            FlatPhysicalDamageMod = 5f,
            Id = 3862
        };

        public static Item Harrowing_Crescent = new Item
        {
            Name = "Harrowing Crescent",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = false,
            Tags = new[]
                        {
                        "Health", "ManaRegen", "GoldPer", "Lane",
                        },
            Range = 600f,
            FlatHPPoolMod = 60f,
            FlatPhysicalDamageMod = 10f,
            SpecialRecipe = 3862,
            InStore = false,
            Id = 3863
        };

        public static Item Black_Mist_Scythe = new Item
        {
            Name = "Black Mist Scythe",
            GoldBase = 400,
            GoldPrice = 400,
            GoldSell = 160,
            Purchasable = false,
            Tags = new[]
                        {
                        "Health", "ManaRegen", "GoldPer", "Lane",
                        },
            Range = 600f,
            FlatHPPoolMod = 75f,
            FlatPhysicalDamageMod = 20f,
            SpecialRecipe = 3863,
            InStore = false,
            Id = 3864
        };

        public static Item Oblivion_Orb = new Item
        {
            Name = "Oblivion Orb",
            From = new[]
                                {
                                1052,
                        },
            Into = new[]
                                {
                                3011, 3165,
                        },
            GoldBase = 365,
            GoldPrice = 800,
            GoldSell = 560,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage",
                        },
            FlatMagicDamageMod = 30f,
            Depth = 2,
            Stacks = 0,
            Id = 3916
        };

        public static Item Imperial_Mandate = new Item
        {
            Name = "Imperial Mandate",
            From = new[]
                                {
                                3067, 4642,
                        },
            GoldBase = 850,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "ManaRegen", "CooldownReduction", "NonbootsMovement",
                        },
            FlatHPPoolMod = 200f,
            FlatMagicDamageMod = 40f,
            Depth = 3,
            Id = 4005
        };

        public static Item Force_of_Nature = new Item
        {
            Name = "Force of Nature",
            From = new[]
                                {
                                1057, 1028, 3066,
                        },
            GoldBase = 800,
            GoldPrice = 2900,
            GoldSell = 2030,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock", "NonbootsMovement",
                        },
            FlatHPPoolMod = 350f,
            FlatSpellBlockMod = 60f,
            PercentMovementSpeedMod = 0.05f,
            Depth = 3,
            Id = 4401
        };

        public static Item The_Golden_Spatula = new Item
        {
            Name = "The Golden Spatula",
            From = new[]
                                {
                                1038, 1053, 3086, 1058, 3067, 3105,
                        },
            GoldBase = 687,
            GoldPrice = 7637,
            GoldSell = 5346,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock", "HealthRegen", "Armor", "Damage", "CriticalStrike", "AttackSpeed", "LifeSteal", "SpellDamage", "Mana", "ManaRegen", "CooldownReduction", "NonbootsMovement",
                        },
            FlatArmorMod = 30f,
            FlatCritChanceMod = 0.3f,
            FlatHPPoolMod = 250f,
            FlatMagicDamageMod = 120f,
            FlatMPPoolMod = 250f,
            FlatPhysicalDamageMod = 70f,
            FlatSpellBlockMod = 30f,
            PercentAttackSpeedMod = 0.5f,
            PercentLifeStealMod = 0.1f,
            PercentMovementSpeedMod = 0.1f,
            Depth = 3,
            Id = 4403
        };

        public static Item Horizon_Focus = new Item
        {
            Name = "Horizon Focus",
            From = new[]
                                {
                                1058, 3145,
                        },
            GoldBase = 700,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage",
                        },
            FlatMagicDamageMod = 100f,
            Depth = 3,
            Id = 4628
        };

        public static Item Cosmic_Drive = new Item
        {
            Name = "Cosmic Drive",
            From = new[]
                                {
                                3108, 3067, 1052,
                        },
            GoldBase = 865,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "CooldownReduction", "NonbootsMovement", "AbilityHaste",
                        },
            FlatHPPoolMod = 200f,
            FlatMagicDamageMod = 70f,
            Depth = 3,
            Id = 4629
        };

        public static Item Blighting_Jewel = new Item
        {
            Name = "Blighting Jewel",
            From = new[]
                                {
                                1052,
                        },
            Into = new[]
                                {
                                3135,
                        },
            GoldBase = 815,
            GoldPrice = 1250,
            GoldSell = 875,
            Purchasable = true,
            Tags = new[]
                        {
                        "MagicPenetration", "SpellDamage",
                        },
            FlatMagicDamageMod = 25f,
            Depth = 2,
            Id = 4630
        };

        public static Item Verdant_Barrier = new Item
        {
            Name = "Verdant Barrier",
            From = new[]
                                {
                                1033, 1052,
                        },
            Into = new[]
                                {
                                3102,
                        },
            GoldBase = 315,
            GoldPrice = 1200,
            GoldSell = 840,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellBlock", "SpellDamage",
                        },
            FlatMagicDamageMod = 25f,
            FlatSpellBlockMod = 25f,
            Depth = 2,
            Id = 4632
        };

        public static Item Riftmaker = new Item
        {
            Name = "Riftmaker",
            From = new[]
                                {
                                4635, 1026,
                        },
            GoldBase = 1050,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "CooldownReduction", "SpellVamp", "MagicPenetration", "AbilityHaste",
                        },
            FlatHPPoolMod = 150f,
            FlatMagicDamageMod = 80f,
            Depth = 3,
            Id = 4633
        };

        public static Item Leeching_Leer = new Item
        {
            Name = "Leeching Leer",
            From = new[]
                                {
                                1028, 1052,
                        },
            Into = new[]
                                {
                                4633,
                        },
            GoldBase = 465,
            GoldPrice = 1300,
            GoldSell = 910,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellVamp",
                        },
            FlatHPPoolMod = 150f,
            FlatMagicDamageMod = 20f,
            Depth = 2,
            Id = 4635
        };

        public static Item Night_Harvester = new Item
        {
            Name = "Night Harvester",
            From = new[]
                                {
                                3145, 1028, 1026,
                        },
            GoldBase = 900,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "CooldownReduction", "NonbootsMovement", "AbilityHaste",
                        },
            FlatHPPoolMod = 250f,
            FlatMagicDamageMod = 80f,
            Depth = 3,
            Id = 4636
        };

        public static Item Demonic_Embrace = new Item
        {
            Name = "Demonic Embrace",
            From = new[]
                                {
                                1026, 1011, 1052,
                        },
            GoldBase = 815,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage",
                        },
            FlatHPPoolMod = 350f,
            FlatMagicDamageMod = 70f,
            Depth = 3,
            Id = 4637
        };

        public static Item Watchful_Wardstone = new Item
        {
            Name = "Watchful Wardstone",
            From = new[]
                                {
                                4641,
                        },
            Into = new[]
                                {
                                4643,
                        },
            GoldBase = 0,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = false,
            Tags = new[]
                        {
                        "Vision", "Active", "CooldownReduction",
                        },
            Range = 600f,
            Depth = 2,
            InStore = false,
            Id = 4638
        };

        public static Item Stirring_Wardstone = new Item
        {
            Name = "Stirring Wardstone",
            Into = new[]
                                {
                                4638,
                        },
            GoldBase = 1100,
            GoldPrice = 1100,
            GoldSell = 770,
            Purchasable = true,
            Tags = new[]
                        {
                        "Active", "CooldownReduction", "AbilityHaste",
                        },

            Consumed = true,
            ConsumeOnFull = true,
            Id = 4641
        };

        public static Item Bandleglass_Mirror = new Item
        {
            Name = "Bandleglass Mirror",
            From = new[]
                                {
                                1004, 1052,
                        },
            Into = new[]
                                {
                                6617, 3011, 4005,
                        },
            GoldBase = 365,
            GoldPrice = 1050,
            GoldSell = 735,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "ManaRegen", "CooldownReduction", "AbilityHaste",
                        },
            FlatMagicDamageMod = 20f,
            Depth = 2,
            Id = 4642
        };

        public static Item Vigilant_Wardstone = new Item
        {
            Name = "Vigilant Wardstone",
            From = new[]
                                {
                                4638,
                        },
            GoldBase = 1200,
            GoldPrice = 2300,
            GoldSell = 1610,
            Range = 600f,
            Purchasable = true,
            Tags = new[]
                        {
                        "Vision", "Active", "CooldownReduction", "NonbootsMovement", "AbilityHaste",
                        },
            PercentMovementSpeedMod = 0.1f,
            Depth = 3,
            Id = 4643
        };

        public static Item Ironspike_Whip = new Item
        {
            Name = "Ironspike Whip",
            From = new[]
                                {
                                1037,
                        },
            Into = new[]
                                {
                                6630, 6631,
                        },
            GoldBase = 325,
            GoldPrice = 1200,
            GoldSell = 840,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "Active",
                        },
            Range = 400f,
            FlatPhysicalDamageMod = 30f,
            Depth = 2,
            Id = 6029
        };

        public static Item Silvermere_Dawn = new Item
        {
            Name = "Silvermere Dawn",
            From = new[]
                                {
                                3140, 1037, 1028,
                        },
            GoldBase = 425,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock", "Damage", "Active", "Tenacity",
                        },
            FlatHPPoolMod = 200f,
            FlatPhysicalDamageMod = 35f,
            FlatSpellBlockMod = 35f,
            PercentMovementSpeedMod = 0.07f,
            Depth = 3,
            Id = 6035
        };

        public static Item Deaths_Dance = new Item
        {
            Name = "Death's Dance",
            From = new[]
                                {
                                1031, 3133, 1037,
                        },
            GoldBase = 325,
            GoldPrice = 3100,
            GoldSell = 2170,
            Purchasable = true,
            Tags = new[]
                        {
                        "Armor", "Damage", "CooldownReduction", "NonbootsMovement", "AbilityHaste",
                        },
            FlatArmorMod = 40f,
            FlatPhysicalDamageMod = 50f,
            Depth = 3,
            Stacks = 0,
            Id = 6333
        };

        public static Item Chempunk_Chainsword = new Item
        {
            Name = "Chempunk Chainsword",
            From = new[]
                                {
                                3123, 1028, 3133,
                        },
            GoldBase = 400,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "CooldownReduction", "AbilityHaste",
                        },
            FlatHPPoolMod = 200f,
            FlatPhysicalDamageMod = 45f,
            Depth = 3,
            Id = 6609
        };

        public static Item Staff_of_Flowing_Water = new Item
        {
            Name = "Staff of Flowing Water",
            From = new[]
                                {
                                3114, 1026,
                        },
            GoldBase = 650,
            GoldPrice = 2300,
            GoldSell = 1610,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "NonbootsMovement",
                        },
            FlatMagicDamageMod = 60f,
            Depth = 3,
            Id = 6616
        };

        public static Item Moonstone_Renewer = new Item
        {
            Name = "Moonstone Renewer",
            From = new[]
                                {
                                3067, 4642,
                        },
            GoldBase = 850,
            GoldPrice = 2700,
            GoldSell = 1890,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "CooldownReduction",
                        },
            FlatHPPoolMod = 200f,
            FlatMagicDamageMod = 40f,
            Depth = 3,
            Id = 6617
        };

        public static Item Goredrinker = new Item
        {
            Name = "Goredrinker",
            From = new[]
                                {
                                6029, 3044, 3067,
                        },
            GoldBase = 200,
            GoldPrice = 3300,
            GoldSell = 2310,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "HealthRegen", "Damage", "Active", "CooldownReduction", "AbilityHaste",
                        },
            Range = 400f,
            FlatHPPoolMod = 400f,
            FlatPhysicalDamageMod = 45f,
            Depth = 3,
            Id = 6630
        };

        public static Item Stridebreaker = new Item
        {
            Name = "Stridebreaker",
            From = new[]
                                {
                                6029, 3051, 3067,
                        },
            GoldBase = 200,
            GoldPrice = 3300,
            GoldSell = 2310,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "AttackSpeed", "Active", "CooldownReduction", "Slow", "NonbootsMovement", "AbilityHaste",
                        },
            Range = 200f,
            FlatHPPoolMod = 300f,
            FlatPhysicalDamageMod = 50f,
            PercentAttackSpeedMod = 0.2f,
            Depth = 3,
            Id = 6631
        };

        public static Item Divine_Sunderer = new Item
        {
            Name = "Divine Sunderer",
            From = new[]
                                {
                                3044, 3057, 3067,
                        },
            GoldBase = 700,
            GoldPrice = 3300,
            GoldSell = 2310,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "LifeSteal", "CooldownReduction", "ArmorPenetration", "AbilityHaste",
                        },
            FlatHPPoolMod = 400f,
            FlatPhysicalDamageMod = 40f,
            Depth = 3,
            Id = 6632
        };

        public static Item Liandrys_Anguish = new Item
        {
            Name = "Liandry's Anguish",
            From = new[]
                                {
                                3802, 3108,
                        },
            GoldBase = 1200,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "Mana", "CooldownReduction", "MagicPenetration", "AbilityHaste",
                        },
            FlatMagicDamageMod = 80f,
            FlatMPPoolMod = 600f,
            Depth = 3,
            Id = 6653
        };

        public static Item Ludens_Tempest = new Item
        {
            Name = "Luden's Tempest",
            From = new[]
                                {
                                3802, 1026,
                        },
            GoldBase = 1250,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "SpellDamage", "Mana", "CooldownReduction", "MagicPenetration", "AbilityHaste",
                        },
            FlatMagicDamageMod = 80f,
            FlatMPPoolMod = 600f,
            Depth = 3,
            Id = 6655
        };

        public static Item Everfrost = new Item
        {
            Name = "Everfrost",
            From = new[]
                                {
                                3802, 1026,
                        },
            GoldBase = 1250,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellDamage", "Mana", "Active", "CooldownReduction", "Slow", "AbilityHaste",
                        },
            FlatHPPoolMod = 200f,
            FlatMagicDamageMod = 80f,
            FlatMPPoolMod = 600f,
            Depth = 3,
            Id = 6656
        };

        public static Item Bamis_Cinder = new Item
        {
            Name = "Bami's Cinder",
            From = new[]
                                {
                                1028, 1028,
                        },
            Into = new[]
                                {
                                3068, 6664, 6662,
                        },
            GoldBase = 200,
            GoldPrice = 1000,
            GoldSell = 700,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health",
                        },
            FlatHPPoolMod = 300f,
            Depth = 2,
            Id = 6660
        };

        public static Item Frostfire_Gauntlet = new Item
        {
            Name = "Frostfire Gauntlet",
            From = new[]
                                {
                                6660, 1033, 1031,
                        },
            GoldBase = 950,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock", "Armor", "Aura", "CooldownReduction", "Slow", "AbilityHaste",
                        },
            FlatArmorMod = 50f,
            FlatHPPoolMod = 350f,
            FlatSpellBlockMod = 25f,
            Depth = 3,
            Id = 6662
        };

        public static Item Turbo_Chemtank = new Item
        {
            Name = "Turbo Chemtank",
            From = new[]
                                {
                                6660, 1029, 1057,
                        },
            GoldBase = 1000,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "SpellBlock", "Armor", "Aura", "Active", "CooldownReduction", "Slow", "NonbootsMovement", "Tenacity", "AbilityHaste",
                        },
            FlatArmorMod = 25f,
            FlatHPPoolMod = 350f,
            FlatSpellBlockMod = 50f,
            Depth = 3,
            Id = 6664
        };

        public static Item Noonquiver = new Item
        {
            Name = "Noonquiver",
            From = new[]
                                {
                                1036, 1042, 1036,
                        },
            Into = new[]
                                {
                                6671, 6672, 6673,
                        },
            GoldBase = 300,
            GoldPrice = 1300,
            GoldSell = 910,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "AttackSpeed",
                        },
            FlatPhysicalDamageMod = 30f,
            PercentAttackSpeedMod = 0.15f,
            Depth = 2,
            Id = 6670
        };

        public static Item Galeforce = new Item
        {
            Name = "Galeforce",
            From = new[]
                                {
                                6670, 1018, 1037,
                        },
            GoldBase = 625,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "AttackSpeed", "Active", "NonbootsMovement",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 55f,
            PercentAttackSpeedMod = 0.2f,
            Depth = 3,
            Id = 6671
        };

        public static Item Kraken_Slayer = new Item
        {
            Name = "Kraken Slayer",
            From = new[]
                                {
                                6670, 1018, 1037,
                        },
            GoldBase = 625,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "AttackSpeed", "ArmorPenetration",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 60f,
            PercentAttackSpeedMod = 0.25f,
            Depth = 3,
            Id = 6672
        };

        public static Item Immortal_Shieldbow = new Item
        {
            Name = "Immortal Shieldbow",
            From = new[]
                                {
                                6670, 1018, 1053,
                        },
            GoldBase = 600,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "Health", "Damage", "CriticalStrike", "AttackSpeed", "LifeSteal",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 50f,
            PercentAttackSpeedMod = 0.15f,
            PercentLifeStealMod = 0.12f,
            Depth = 3,
            Id = 6673
        };

        public static Item Navori_Quickblades = new Item
        {
            Name = "Navori Quickblades",
            From = new[]
                                {
                                3133, 1037, 1018,
                        },
            GoldBase = 825,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "CooldownReduction", "AbilityHaste",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 60f,
            Depth = 3,
            Id = 6675
        };

        public static Item The_Collector = new Item
        {
            Name = "The Collector",
            From = new[]
                                {
                                3134, 1037, 1018,
                        },
            GoldBase = 425,
            GoldPrice = 3000,
            GoldSell = 2100,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CriticalStrike", "ArmorPenetration",
                        },
            FlatCritChanceMod = 0.2f,
            FlatPhysicalDamageMod = 45f,
            Depth = 3,
            Id = 6676
        };

        public static Item Rageknife = new Item
        {
            Name = "Rageknife",
            From = new[]
                                {
                                1042, 1042,
                        },
            Into = new[]
                                {
                                3124,
                        },
            GoldBase = 200,
            GoldPrice = 800,
            GoldSell = 560,
            Purchasable = true,
            Tags = new[]
                        {
                        "AttackSpeed", "OnHit",
                        },
            PercentAttackSpeedMod = 0.25f,
            Depth = 2,
            Id = 6677
        };

        public static Item Duskblade_of_Draktharr = new Item
        {
            Name = "Duskblade of Draktharr",
            From = new[]
                                {
                                3134, 3133,
                        },
            GoldBase = 1000,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "Stealth", "CooldownReduction", "Slow", "ArmorPenetration", "AbilityHaste",
                        },
            FlatPhysicalDamageMod = 55f,
            Depth = 3,
            Id = 6691
        };

        public static Item Eclipse = new Item
        {
            Name = "Eclipse",
            From = new[]
                                {
                                3134, 1036, 1053,
                        },
            GoldBase = 850,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "LifeSteal", "SpellVamp", "NonbootsMovement", "ArmorPenetration",
                        },
            FlatPhysicalDamageMod = 55f,
            Depth = 3,
            Id = 6692
        };

        public static Item Prowlers_Claw = new Item
        {
            Name = "Prowler's Claw",
            From = new[]
                                {
                                3134, 3133,
                        },
            GoldBase = 1000,
            GoldPrice = 3200,
            GoldSell = 2240,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "Active", "CooldownReduction", "NonbootsMovement", "ArmorPenetration", "AbilityHaste",
                        },
            FlatPhysicalDamageMod = 55f,
            Depth = 3,
            Id = 6693
        };

        public static Item Seryldas_Grudge = new Item
        {
            Name = "Serylda's Grudge",
            From = new[]
                                {
                                3133, 3035,
                        },
            GoldBase = 850,
            GoldPrice = 3400,
            GoldSell = 2380,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "CooldownReduction", "ArmorPenetration", "AbilityHaste",
                        },
            FlatPhysicalDamageMod = 45f,
            Depth = 3,
            Id = 6694
        };

        public static Item Serpents_Fang = new Item
        {
            Name = "Serpent's Fang",
            From = new[]
                                {
                                3134, 1037,
                        },
            GoldBase = 825,
            GoldPrice = 2800,
            GoldSell = 1960,
            Purchasable = true,
            Tags = new[]
                        {
                        "Damage", "ArmorPenetration",
                        },
            FlatPhysicalDamageMod = 60f,
            Depth = 3,
            Id = 6695
        };

        #endregion

        public struct Item
        {
            #region Fields

            /// <summary>
            ///     Item Consumed
            /// </summary>
            public bool Consumed;

            /// <summary>
            ///     Consume Item On Full
            /// </summary>
            public bool ConsumeOnFull;

            /// <summary>
            ///     Item Depth
            /// </summary>
            public int Depth;

            /// <summary>
            ///     Item Disabled Maps
            /// </summary>
            public int[] DisabledMaps;

            public float FlatArmorMod;

            public float FlatAttackSpeedMod;

            public float FlatBlockMod;

            public float FlatCritChanceMod;

            public float FlatCritDamageMod;

            public float FlatEnergyPoolMod;

            public float FlatEnergyRegenMod;

            public float FlatEXPBonus;

            public float FlatHPPoolMod;

            public float FlatHPRegenMod;

            public float FlatMagicDamageMod;

            public float FlatMovementSpeedMod;

            public float FlatMPPoolMod;

            public float FlatMPRegenMod;

            public float FlatPhysicalDamageMod;

            public float FlatSpellBlockMod;

            /// <summary>
            ///     Item From Item Ids
            /// </summary>
            public int[] From;

            /// <summary>
            ///     Gold Base Price
            /// </summary>
            public int GoldBase;

            /// <summary>
            ///     Gold Price
            /// </summary>
            public int GoldPrice;

            /// <summary>
            ///     Gold Sell Price
            /// </summary>
            public int GoldSell;

            /// <summary>
            ///     Item Group
            /// </summary>
            public string Group;

            /// <summary>
            ///     Item Hide from All
            /// </summary>
            public bool HideFromAll;

            /// <summary>
            ///     Item Id
            /// </summary>
            public int Id;

            /// <summary>
            ///     Item In Store
            /// </summary>
            public bool InStore;

            /// <summary>
            ///     Item into Items
            /// </summary>
            public int[] Into;

            /// <summary>
            ///     Item Name
            /// </summary>
            public string Name;

            public float PercentArmorMod;

            public float PercentAttackSpeedMod;

            public float PercentBlockMod;

            public float PercentCritChanceMod;

            public float PercentCritDamageMod;

            public float PercentDodgeMod;

            public float PercentEXPBonus;

            public float PercentHPPoolMod;

            public float PercentHPRegenMod;

            public float PercentLifeStealMod;

            public float PercentMagicDamageMod;

            public float PercentMovementSpeedMod;

            public float PercentMPPoolMod;

            public float PercentMPRegenMod;

            public float PercentPhysicalDamageMod;

            public float PercentSpellBlockMod;

            public float PercentSpellVampMod;

            /// <summary>
            ///     Item Purchasable
            /// </summary>
            public bool Purchasable;

            /// <summary>
            ///     Item Range
            /// </summary>
            public float Range;

            /// <summary>
            ///     Required Champion for Item
            /// </summary>
            public string RequiredChampion;

            public float rFlatArmorModPerLevel;

            public float rFlatArmorPenetrationMod;

            public float rFlatArmorPenetrationModPerLevel;

            public float rFlatCritChanceModPerLevel;

            public float rFlatCritDamageModPerLevel;

            public float rFlatDodgeMod;

            public float rFlatDodgeModPerLevel;

            public float rFlatEnergyModPerLevel;

            public float rFlatEnergyRegenModPerLevel;

            public float rFlatGoldPer10Mod;

            public float rFlatHPModPerLevel;

            public float rFlatHPRegenModPerLevel;

            public float rFlatMagicDamageModPerLevel;

            public float rFlatMagicPenetrationMod;

            public float rFlatMagicPenetrationModPerLevel;

            public float rFlatMovementSpeedModPerLevel;

            public float rFlatMPModPerLevel;

            public float rFlatMPRegenModPerLevel;

            public float rFlatPhysicalDamageModPerLevel;

            public float rFlatSpellBlockModPerLevel;

            public float rFlatTimeDeadMod;

            public float rFlatTimeDeadModPerLevel;

            public float rPercentArmorPenetrationMod;

            public float rPercentArmorPenetrationModPerLevel;

            public float rPercentAttackSpeedModPerLevel;

            public float rPercentCooldownMod;

            public float rPercentCooldownModPerLevel;

            public float rPercentMagicPenetrationMod;

            public float rPercentMagicPenetrationModPerLevel;

            public float rPercentMovementSpeedModPerLevel;

            public float rPercentTimeDeadMod;

            public float rPercentTimeDeadModPerLevel;

            /// <summary>
            ///     Special Recipe
            /// </summary>
            public int SpecialRecipe;

            /// <summary>
            ///     Item Stacks
            /// </summary>
            public int Stacks;

            /// <summary>
            ///     Item Tags
            /// </summary>
            public string[] Tags;

            #endregion

            #region Public Methods and Operators

            public Items.Item GetItem()
            {
                return new Items.Item(this.Id, this.Range);
            }

            #endregion
        }
    }
}