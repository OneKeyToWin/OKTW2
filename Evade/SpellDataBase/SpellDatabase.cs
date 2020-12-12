// Copyright 2014 - 2014 Esk0r
// SpellDatabase.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;

#endregion

namespace Evade
{
    public static class SpellDatabase
    {
        public static List<SpellData> Spells = new List<SpellData>();

        static SpellDatabase()
        {
            //Add spells to the database 

            #region Test

            if (Config.TestOnAllies)
            {
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = ObjectManager.Player.ChampionName,
                        SpellName = "TestCircleSkillShot",
                        Slot = SpellSlot.R,
                        Type = SkillShotType.SkillshotCircle,
                        Delay = 6000,
                        Range = 650,
                        Radius = 50,
                        MissileSpeed = int.MaxValue,
                        FixedRange = false,
                        AddHitbox = true,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "TestSkillShot",
                        DontCross = true
                    });

                Spells.Add(
                    new SpellData
                    {
                        ChampionName = ObjectManager.Player.ChampionName,
                        SpellName = "TestLineSkillShot",
                        Slot = SpellSlot.R,
                        Type = SkillShotType.SkillshotMissileLine,
                        Delay = 1000,
                        Range = 1200,
                        Radius = 70,
                        MissileSpeed = 400,
                        FixedRange = true,
                        AddHitbox = true,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "TestLineSkillShot",
                        DontCross = true
                    });
            }

            #endregion Test

            Spells.Add(
            new SpellData
            {
                ChampionName = "AllChampions",
                DangerValue = 4,
                MissileSpeed = 1600,
                Radius = 80,
                Range = 900,
                Delay = 350,
                FixedRange = true,
                IsDangerous = true,
                AddHitbox = true,
                Slot = SpellSlot.Q,
                SpellName = "6656Cast",
                Type = SkillShotType.SkillshotMissileLine
            });

            #region Aatrox

            //Spells.Add(
            //    new SpellData
            //    {
            //        ChampionName = "Aatrox",
            //        SpellName = "AatroxW",
            //        Slot = SpellSlot.W,
            //        Type = SkillShotType.SkillshotMissileLine,
            //        Delay = 250,
            //        Range = 825,
            //        Radius = 60,
            //        MissileSpeed = 1600,
            //        FixedRange = true,
            //        AddHitbox = true,
            //        DangerValue = 3,
            //        IsDangerous = true,
            //        MissileSpellName = "AatroxW",
            //        CanBeRemoved = true,
            //        CollisionObjects = new [] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Minion, CollisionObjectTypes.Champions }
            //    });

            #endregion Aatrox

            #region Ahri

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriOrbofDeception",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    MissileSpeed = 2500,
                    MissileAccel = -3200,
                    MissileMaxSpeed = 2500,
                    MissileMinSpeed = 400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "AhriOrbMissile",
                    CanBeRemoved = true,
                    ForceRemove = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriOrbReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    MissileSpeed = 60,
                    MissileAccel = 1900,
                    MissileMinSpeed = 60,
                    MissileMaxSpeed = 2600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileFollowsUnit = true,
                    CanBeRemoved = true,
                    ForceRemove = true,
                    MissileSpellName = "AhriOrbReturn",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriSeduce",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 60,
                    MissileSpeed = 1550,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "AhriSeduceMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            #endregion Ahri

            #region Amumu

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Amumu",
                    SpellName = "BandageToss",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 80,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "SadMummyBandageToss",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Amumu",
                    SpellName = "CurseoftheSadMummy",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 550,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = false,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion Amumu

            #region Anivia

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Anivia",
                    SpellName = "FlashFrost",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 110,
                    MissileSpeed = 850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "FlashFrostSpell",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            #endregion Anivia

            #region Annie

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    SpellName = "Incinerate",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 825,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    SpellName = "InfernalGuardian",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 600,
                    Radius = 251,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion Annie

            #region Ashe

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ashe",
                    SpellName = "Volley",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 60,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VolleyAttack",
                    MultipleNumber = 9,
                    MultipleAngle = 4.62f*(float) Math.PI/180,
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Minion}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ashe",
                    SpellName = "EnchantedCrystalArrow",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 20000,
                    Radius = 130,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "EnchantedCrystalArrow",
                    EarlyEvade = new[] {EarlyObjects.Allies},
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall}
                });

            #endregion Ashe

            #region Aurelion Sol

            Spells.Add(
                new SpellData
                {
                    ChampionName = "AurelionSol",
                    SpellName = "AurelionSolQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 180,
                    MissileSpeed = 850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "AurelionSolQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "AurelionSol",
                    SpellName = "AurelionSolR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 300,
                    Range = 1420,
                    Radius = 120,
                    MissileSpeed = 4500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "AurelionSolRBeamMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionObjectTypes.YasuoWall}
                });

            #endregion Aurelion Sol

            #region Bard

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Bard",
                    SpellName = "BardQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 65,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BardQMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Bard",
                    SpellName = "BardR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 3400,
                    Radius = 350,
                    MissileSpeed = 2100,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "BardR",
                });

            #endregion

            #region Blatzcrank

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "RocketGrab",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1150,
                    Radius = 70,
                    MissileSpeed = 1800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "RocketGrabMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "StaticField",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 600,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = false,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            #endregion Blatzcrink

            #region Brand

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellName = "BrandQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 60,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BrandQMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellName = "BrandW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 950,
                    ParticleDetectDelay = 250,
                    Range = 900,
                    Radius = 260,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                    EndAtParticle = "brand_.+_w.+tar_red"
                });

            #endregion Brand

            #region Braum

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Braum",
                    SpellName = "BraumQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BraumQMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Braum",
                    SpellName = "BraumRWrapper",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1200,
                    Radius = 115,
                    MissileSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "braumrmissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            #endregion Braum

            #region Caitlyn

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellName = "CaitlynPiltoverPeacemaker",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 625,
                    Range = 1300,
                    Radius = 60,
                    MissileSpeed = 2200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "CaitlynPiltoverPeacemaker",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellName = "CaitlynYordleTrap",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1350,
                    Range = 800,
                    Radius = 70,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DontCross = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    ToggleParticleName = "caitlyn_.+_yordleTrap_trigger_sound"
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellName = "CaitlynEntrapment",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 125,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 1,
                    IsDangerous = false,
                    MissileSpellName = "CaitlynEntrapmentMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            #endregion Caitlyn

            #region Cassiopeia

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Cassiopeia",
                    SpellName = "CassiopeiaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 750,
                    Range = 850,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "CassiopeiaQ",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Cassiopeia",
                    SpellName = "CassiopeiaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 600,
                    Range = 825,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "CassiopeiaR",
                });

            #endregion Cassiopeia

            #region Chogath

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Chogath",
                    SpellName = "Rupture",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1200,
                    ParticleDetectDelay = 500,
                    Range = 950,
                    Radius = 250,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "Rupture",
                    EndAtParticle = "chogath_.+_q_enemy_team"
                });

            #endregion Chogath

            #region Corki

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Corki",
                    SpellName = "PhosphorusBomb",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 300,
                    Range = 825,
                    Radius = 250,
                    MissileSpeed = 1000,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "PhosphorusBombMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Corki",
                    SpellName = "MissileBarrage",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1300,
                    Radius = 40,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "MissileBarrageMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Corki",
                    SpellName = "MissileBarrage2",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1500,
                    Radius = 40,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "MissileBarrageMissile2",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            #endregion Corki

            #region Darius

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Darius",
                    SpellName = "DariusCleave",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 750,
                    Range = 0,
                    Radius = 425 - 50,
                    MissileSpeed = int.MaxValue, 
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "DariusCleave",
                    FollowCaster = true,
                    DisabledByDefault = true,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Darius",
                    SpellName = "DariusAxeGrabCone",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 550,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = false,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DariusAxeGrabCone",
                });

            #endregion Darius

            #region Diana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Diana",
                    SpellName = "DianaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 895,
                    Radius = 195,
                    MissileSpeed = 1400,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DianaQ",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.AllyObjects},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Diana",
                    SpellName = "DianaArcArc",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotArc,
                    Delay = 250,
                    Range = 895,
                    Radius = 195,
                    DontCross = true,
                    MissileSpeed = 1400,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DianaArcArc",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.AllyObjects},
                    TakeClosestPath = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            #endregion Diana

            #region DrMundo

            Spells.Add(
                new SpellData
                {
                    ChampionName = "DrMundo",
                    SpellName = "InfectedCleaverMissileCast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "InfectedCleaverMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            #endregion DrMundo

            #region Draven

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Draven",
                    SpellName = "DravenDoubleShot",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 130,
                    MissileSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DravenDoubleShotMissile",
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Draven",
                    SpellName = "DravenRCast",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 400,
                    Range = 20000,
                    Radius = 160,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "DravenR",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            #endregion Draven

            #region Ekko

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1650,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "ekkoqmis",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 3750,
                    Range = 1600,
                    Radius = 375,
                    MissileSpeed = 1650,
                    FixedRange = false,
                    DisabledByDefault = true,
                    AddHitbox = false,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "EkkoW",
                    CanBeRemoved = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1600,
                    Radius = 375,
                    MissileSpeed = 1650,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "EkkoR",
                    CanBeRemoved = true,
                    FromObjects = new[] {"Ekko_Base_R_TrailEnd.troy"}
                });

            #endregion Ekko

            #region Elise

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Elise",
                    SpellName = "EliseHumanE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 55,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "EliseHumanE",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            #endregion Elise

            #region Evelynn

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Evelynn",
                    SpellName = "EvelynnQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 70,
                    MissileSpeed = 2400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "EvelynnQ",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Evelynn",
                    SpellName = "EvelynnR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 450,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "EvelynnR",
                });

            #endregion Evelynn

            #region Ezreal

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 60,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "EzrealQ",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                    Id = 229,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 80,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    CanBeRemoved = true,
                    IsDangerous = false,
                    MissileSpellName = "EzrealW",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 1000,
                    Range = 20000,
                    Radius = 160,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "EzrealR",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                    Id = 245,
                });

            #endregion Ezreal

            #region Fiora

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Fiora",
                    SpellName = "FioraW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 800,
                    Radius = 70,
                    MissileSpeed = 3200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "FioraWMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Fiora

            #region Fizz

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Fizz",
                    SpellName = "FizzR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1275,
                    Radius = 110,
                    MissileSpeed = 1300,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "FizzRMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies},
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                    CanBeRemoved = true,
                });

            #endregion Fizz

            #region Galio

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Galio",
                    SpellName = "GalioQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 825,
                    Radius = 200,
                    MissileSpeed = 1300,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CollisionObjects = new [] { CollisionObjectTypes.YasuoWall }
                });
            
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Galio",
                    SpellName = "GalioE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 350,
                    Range = 1000,
                    Radius = 120,
                    ExtraRange = 80,
                    MissileSpeed = 2300,
                    BehindStart = 300,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true
                });

            #endregion Galio

            #region Gnar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1125,
                    Radius = 60,
                    MissileSpeed = 2500,
                    MissileAccel = -3000,
                    MissileMaxSpeed = 2500,
                    MissileMinSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    CanBeRemoved = true,
                    ForceRemove = true,
                    MissileSpellName = "gnarqmissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarQReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 2500,
                    Radius = 75,
                    MissileSpeed = 60,
                    MissileAccel = 800,
                    MissileMaxSpeed = 2600,
                    MissileMinSpeed = 60,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    CanBeRemoved = true,
                    ForceRemove = true,
                    MissileSpellName = "GnarQMissileReturn",
                    DisableFowDetection = false,
                    DisabledByDefault = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1150,
                    Radius = 90,
                    MissileSpeed = 2100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GnarBigQMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 600,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GnarBigW",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 0,
                    Range = 473,
                    Radius = 150,
                    MissileSpeed = 903,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GnarE",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 475,
                    Radius = 200,
                    MissileSpeed = 1000,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GnarBigE",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 500,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = false,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion

            #region Gragas

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1100,
                    Radius = 275,
                    MissileSpeed = 1300,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GragasQMissile",
                    ExtraDuration = 4500,
                    ToggleParticleName = "Gragas_.+_Q_(Enemy|Ally)",
                    DontCross = true,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 950,
                    Radius = 200,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GragasE",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    ExtraRange = 300,
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1050,
                    Radius = 375,
                    MissileSpeed = 1800,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "GragasRBoom",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Gragas

            #region Graves

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Graves",
                    SpellName = "GravesQLineSpell",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 808,
                    Radius = 40,
                    MissileSpeed = 3000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GravesQLineMis",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Graves",
                    SpellName = "GravesChargeShot",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 100,
                    MissileSpeed = 2100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "GravesChargeShotShot",
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Graves

            #region Heimerdinger

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Heimerdinger",
                    SpellName = "Heimerdingerwm",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 70,
                    MissileSpeed = 1800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "HeimerdingerWAttack2",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Heimerdinger",
                    SpellName = "HeimerdingerE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 925,
                    Radius = 100,
                    MissileSpeed = 1200,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "heimerdingerespell",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Heimerdinger

            #region Illaoi

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 750,
                    Range = 850,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "illaoiemis",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 50,
                    MissileSpeed = 1900,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "illaoiemis",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 0,
                    Radius = 450,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = false,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion Illaoi

            #region Irelia

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Irelia",
                    SpellName = "IreliaE2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 5000,
                    Radius = 70,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 4,
                    MissileDelayed=true,
                    IsDangerous = true,
                    MissileSpellName = "IreliaEMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Irelia",
                    SpellName = "IreliaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1050,
                    Radius = 160,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "IreliaR",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions},
                });

            #endregion Irelia

            #region Ivern

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ivern",
                    SpellName = "IvernQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 65,
                    MissileSpeed = 1300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "IvernQ",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Minion, CollisionObjectTypes.Champions,  },
                });

            #endregion Ivern

            #region Janna

                Spells.Add(
                new SpellData
                {
                    ChampionName = "Janna",
                    SpellName = "JannaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1700,
                    Radius = 120,
                    MissileSpeed = 900,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "HowlingGaleSpell",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Janna

            #region JarvanIV

            Spells.Add(
                new SpellData
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVDragonStrike",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 770,
                    Radius = 70,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = false,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVEQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 880,
                    Radius = 70,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVDemacianStandard",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 860,
                    Radius = 175,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "JarvanIVDemacianStandard",
                });

            #endregion JarvanIV

            #region Jayce

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jayce",
                    SpellName = "jayceshockblast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 70,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "JayceShockBlastMis",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jayce",
                    SpellName = "JayceQAccel",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 70,
                    MissileSpeed = 2350,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "JayceShockBlastWallMis",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Jayce

            #region Jhin

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jhin",
                    SpellName = "JhinW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 750,
                    Range = 2550,
                    Radius = 40,
                    MissileSpeed = 5000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JhinWMissile",
                    EarlyEvade = new[] { EarlyObjects.Allies, EarlyObjects.AllyObjects },
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jhin",
                    SpellName = "JhinRShot",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 3500,
                    Radius = 80,
                    MissileSpeed = 5000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JhinRShotMis",
                    ExtraMissileNames = new[] { "JhinRShotMis4" },
                    EarlyEvade = new[] { EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects },
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                });

            #endregion Jhin

            #region Jinx

            //TODO: Detect the animation from fow instead of the missile.
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jinx",
                    SpellName = "JinxWMissile",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 600,
                    Range = 1500,
                    Radius = 60,
                    MissileSpeed = 3300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JinxWMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jinx",
                    SpellName = "JinxR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 600,
                    Range = 20000,
                    Radius = 140,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "JinxR",
                    EarlyEvade = new[] {EarlyObjects.Allies},
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });

            #endregion Jinx

            #region Kalista

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kalista",
                    SpellName = "KalistaMysticShot",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 40,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "kalistamysticshotmis",
                    ExtraMissileNames = new[] {"kalistamysticshotmistrue"},
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Kalista

            #region Karma

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karma",
                    SpellName = "KarmaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KarmaQMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            //TODO: add the circle at the end.
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karma",
                    SpellName = "KarmaQMantra",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 80,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KarmaQMissileMantra",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Karma

            #region Karthus

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karthus",
                    SpellName = "KarthusLayWasteA2",
                    ExtraSpellNames =
                        new[]
                        {
                            "karthuslaywastea3", "karthuslaywastea1", "karthuslaywastedeada1", "karthuslaywastedeada2",
                            "karthuslaywastedeada3"
                        },
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1150,
                    Range = 875,
                    Radius = 160,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            #endregion Karthus

            #region Kassadin

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kassadin",
                    SpellName = "RiftWalk",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 450,
                    Radius = 270,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "RiftWalk",
                });

            #endregion Kassadin

            #region Kennen

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kennen",
                    SpellName = "KennenShurikenHurlMissile1",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 125,
                    Range = 1050,
                    Radius = 50,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KennenShurikenHurlMissile1",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Kennen

            #region Khazix

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Khazix",
                    SpellName = "KhazixW",
                    ExtraSpellNames = new[] {"khazixwlong"},
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1025,
                    Radius = 73,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KhazixWMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    MultipleNumber = 3,
                    MultipleAngle = 22f*(float) Math.PI/180,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Khazix",
                    SpellName = "KhazixE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 600,
                    Radius = 300,
                    MissileSpeed = 1500,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KhazixE",
                });

            #endregion Khazix

            #region Kled

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kled",
                    SpellName = "KledQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 45,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "KledQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kled",
                    SpellName = "KledE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 750,
                    Radius = 125,
                    MissileSpeed = 945,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kled",
                    SpellName = "KledRiderQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 700,
                    Radius = 40,
                    MissileSpeed = 3000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KledRiderQMissile",
                    MultipleNumber = 5,
                    MultipleAngle = 5 * (float)Math.PI / 180,
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });
           
            #endregion Kled

            #region Kogmaw

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kogmaw",
                    SpellName = "KogMawQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 70,
                    MissileSpeed = 1650,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KogMawQ",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kogmaw",
                    SpellName = "KogMawVoidOoze",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1360,
                    Radius = 120,
                    MissileSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KogMawVoidOozeMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kogmaw",
                    SpellName = "KogMawLivingArtillery",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1200,
                    Range = 1800,
                    Radius = 225,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KogMawLivingArtillery",
                });

            #endregion Kogmaw

            #region Leblanc

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSlide",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 0,
                    Range = 600,
                    Radius = 220,
                    MissileSpeed = 1450,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LeblancSlide",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 0,
                    Range = 600,
                    Radius = 220,
                    MissileSpeed = 1450,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LeblancSlideM",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 55,
                    MissileSpeed = 1750,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LeblancSoulShackle",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSoulShackleM",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 55,
                    MissileSpeed = 1750,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LeblancSoulShackleM",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Leblanc

            #region LeeSin

            Spells.Add(
                new SpellData
                {
                    ChampionName = "LeeSin",
                    SpellName = "BlindMonkQOne",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 65,
                    MissileSpeed = 1800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BlindMonkQOne",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion LeeSin

            #region Leona

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaZenithBlade",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 905,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    TakeClosestPath = true,
                    MissileSpellName = "LeonaZenithBladeMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaSolarFlare",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1000,
                    Range = 1200,
                    Radius = 300,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "LeonaSolarFlare",
                });

            #endregion Leona

            #region Lissandra

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 825,
                    Radius = 75,
                    MissileSpeed = 2200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LissandraQMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraQShards",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 825,
                    Radius = 90,
                    MissileSpeed = 2200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "lissandraqshards",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1025,
                    Radius = 125,
                    MissileSpeed = 850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LissandraEMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Lulu

            #region Lucian

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    Range = 1300,
                    Radius = 65,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LucianQ",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 55,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "lucianwmissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianRMis",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1400,
                    Radius = 110,
                    MissileSpeed = 2800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "lucianrmissileoffhand",
                    ExtraMissileNames = new[] {"lucianrmissile"},
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    DontCheckForDuplicates = true,
                    DisabledByDefault = true,
                });

            #endregion Lucian

            #region Lulu

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LuluQMissile",  
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluQPix",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LuluQMissileTwo",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Lulu

            #region Lux

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lux",
                    SpellName = "LuxLightBinding",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 70,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LuxLightBindingMis",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    //CanBeRemoved = true,
                    //CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall, },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lux",
                    SpellName = "LuxLightStrikeKugel",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 300,
                    ExtraDuration = 5500,
                    Range = 1100,
                    Radius = 275,
                    MissileSpeed = 1300,
                    FixedRange = false,
                    AddHitbox = true,
                    IsDangerous = false,
                    DontCross = true,
                    MissileSpellName = "LuxLightStrikeKugel",
                    ToggleParticleName = "Lux_.+_E_tar_aoe_",
                    DangerValue = 2,
                    CanBeRemoved = false,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lux",
                    SpellName = "LuxMaliceCannonMis",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 1000,
                    Range = 3500,
                    Radius = 190,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "LuxMaliceCannonMis",
                    EndAtParticle = "lux_.+_r_mis_beam"
                });

            #endregion Lux

            #region Malphite

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Malphite",
                    SpellName = "UFSlash",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 0,
                    Range = 1000,
                    Radius = 270,
                    MissileSpeed = 1500,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    IsDash = true,
                    CanDetectDash = (sender, args) =>
                    {
                        return args.Speed > sender.MoveSpeed + 1450 && Utils.ImmobileTime(sender) == -1;
                    },
                    MissileSpellName = "UFSlash",
                });

            #endregion Malphite

            #region Malzahar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Malzahar",
                    SpellName = "MalzaharQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 750,
                    Range = 900,
                    Radius = 85,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    DontCross = true,
                    MissileSpellName = "MalzaharQ",
                });

            #endregion Malzahar

            #region Morgana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Morgana",
                    SpellName = "MorganaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 80,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "MorganaQ",
                    EarlyEvade = new[] { EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects },
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Morgana

            #region Mordekaiser

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Mordekaiser",
                    SpellName = "MordekaiserQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 600,
                    Range = 645,
                    Radius = 120,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Mordekaiser",
                    SpellName = "MordekaiserE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 120,
                    MissileSpeed = 2500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true
                });

            #endregion Morgana

            #region Nami

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellName = "NamiQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 950,
                    Range = 1625,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "namiqmissile",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellName = "NamiR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 2750,
                    Radius = 260,
                    MissileSpeed = 850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "NamiRMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Nami

            #region Nautilus

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nautilus",
                    SpellName = "NautilusAnchorDragMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 80,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "NautilusAnchorDragMissile",
                    //EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects, EarlyObjects.Wall },
                    //CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                    //walls?
                });

            #endregion Nautilus

            #region Nocturne

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nocturne",
                    SpellName = "NocturneDuskbringer",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1125,
                    Radius = 60,
                    MissileSpeed = 1400,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "NocturneDuskbringer",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            #endregion Nocturne

            #region Nidalee

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nidalee",
                    SpellName = "JavelinToss",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 40,
                    MissileSpeed = 1300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JavelinToss",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Nidalee

            #region Olaf

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Olaf",
                    SpellName = "OlafAxeThrowCast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    ExtraRange = 150,
                    Radius = 105,
                    MissileSpeed = 1600,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "olafaxethrow",
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            #endregion Olaf

            #region Orianna

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannasQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 1500,
                    Radius = 80,
                    MissileSpeed = 1200,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "orianaizuna",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannaQend",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 0,
                    Range = 1500,
                    Radius = 90,
                    MissileSpeed = 1200,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OrianaDissonanceCommand-",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 255,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "OrianaDissonanceCommand-",
                    FromObject = "yomu_ring_",
                    SourceObjectName = "w_dissonance_ball" //Orianna_Base_W_Dissonance_ball_green.troy & Orianna_Base_W_Dissonance_cas_green.troy
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannasE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 1500,
                    Radius = 85,
                    MissileSpeed = 1850,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "orianaredact",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OrianaDetonateCommand-",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 700,
                    Range = 0,
                    Radius = 410,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "OrianaDetonateCommand-",
                    FromObject = "yomu_ring_",
                    SourceObjectName = "r_vacuumindicator", //Orianna_Base_R_VacuumIndicator.troy
                });

            #endregion Orianna

            #region Quinn

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Quinn",
                    SpellName = "QuinnQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 313,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1550,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "QuinnQ",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Quinn

            #region Poppy

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    Range = 430,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "PoppyQ",
                    EarlyEvade = new[] { EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyRSpell",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 300,
                    Range = 1200,
                    Radius = 100,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "PoppyRMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                });

            #endregion Poppy

            #region Rengar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rengar",
                    SpellName = "RengarE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "RengarEFinal",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion Rengar

            #region RekSai

            Spells.Add(
                new SpellData
                {
                    ChampionName = "RekSai",
                    SpellName = "reksaiqburrowed",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1625,
                    Radius = 60,
                    MissileSpeed = 1950,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "RekSaiQBurrowedMis",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion RekSai

            #region Riven

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Riven",
                    SpellName = "rivenizunablade",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 125,
                    MissileSpeed = 1600,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 5,
                    IsDangerous = true,
                    MultipleNumber = 3,
                    MultipleAngle = 15*(float) Math.PI/180,
                    MissileSpellName = "RivenLightsaberMissile",
                    ExtraMissileNames = new[] {"RivenLightsaberMissileSide"}
                });

            #endregion Riven

            #region Rumble

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rumble",
                    SpellName = "RumbleGrenade",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "RumbleGrenade",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rumble",
                    SpellName = "RumbleCarpetBombM",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 400,
                    MissileDelayed = true,
                    Range = 1200,
                    Radius = 200,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = false,
                    MissileSpellName = "RumbleCarpetBombMissile",
                    CanBeRemoved = false,
                    CollisionObjects = new CollisionObjectTypes[] {},
                });

            #endregion Rumble

            #region Ryze

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ryze",
                    SpellName = "RyzeQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 55,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "RyzeQ",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            #endregion

            #region Sejuani
         
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sejuani",
                    SpellName = "SejuaniR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 110,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "SejuaniRMissile",
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });

            #endregion Sejuani

            #region Sion

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sion",
                    SpellName = "SionQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 900,
                    ExtraDuration = 1000,
                    Range = 780,
                    Radius = 230,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    DontCross = true,
                    MissileSpellName = "SionQHitParticleMissile2",
                    ToggleParticleName = "SionQHitParticleMissile2",
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Sion",
                   SpellName = "SionE",
                   Slot = SpellSlot.E,
                   Type = SkillShotType.SkillshotMissileLine,
                   Delay = 250,
                   Range = 1400,
                   Radius = 80,
                   MissileSpeed = 1800,
                   FixedRange = true,
                   AddHitbox = true,
                   DangerValue = 3,
                   IsDangerous = true,
                   MissileSpellName = "SionEMissile",
                   CollisionObjects =
                       new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
               });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sion",
                    SpellName = "SionR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 750,
                    Radius = 120,
                    MissileSpeed = 1000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    EarlyEvade = new[] {EarlyObjects.Allies},
                    CollisionObjects =
                        new[] {CollisionObjectTypes.Champions},
                });

            #endregion Sion

            #region Soraka

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Soraka",
                    SpellName = "SorakaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 950,
                    Radius = 300,
                    MissileSpeed = 1750,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Soraka

            #region Shen

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Shen",
                    SpellName = "ShenE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 650,
                    Radius = 50,
                    MissileSpeed = 1300,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ShenE",
                    ExtraRange = 200,
                    IsDash = true,
                    CanDetectDash = (sender, args) =>
                    {
                        var expected = sender.MoveSpeed + 815;

                        return args.Speed > expected - 50 && args.Speed < expected + 50 && Utils.ImmobileTime(sender) == -1;
                    },
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Minion, CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });

            #endregion Shen

            #region Shyvana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Shyvana",
                    SpellName = "ShyvanaFireball",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ShyvanaFireballMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Minion, CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Shyvana",
                    SpellName = "ShyvanaTransformCast",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 150,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ShyvanaTransformCast",
                    ExtraRange = 200,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Shyvana",
                    SpellName = "shyvanafireballdragon2",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 850,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "ShyvanaFireballDragonFxMissile",
                    ExtraRange = 200,
                    MultipleNumber = 5,
                    MultipleAngle = 10 * (float)Math.PI / 180
                });

            #endregion Shyvana

            #region Sivir

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sivir",
                    SpellName = "SivirQReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 1250,
                    Radius = 100,
                    MissileSpeed = 1350,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SivirQMissileReturn",
                    DisableFowDetection = false,
                    MissileFollowsUnit = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sivir",
                    SpellName = "SivirQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1250,
                    Radius = 90,
                    MissileSpeed = 1350,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SivirQMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Sivir

            #region Skarner

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Skarner",
                    SpellName = "SkarnerFracture",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SkarnerFractureMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Skarner

            #region Sona

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sona",
                    SpellName = "SonaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 140,
                    MissileSpeed = 2400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "SonaR",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Sona

            #region Swain

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Swain",
                    SpellName = "SwainW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1500,
                    Range = 3500,
                    Radius = 300,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Swain",
                    SpellName = "SwainE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 90,
                    ExtraRange = 150,
                    MissileSpeed = 1500,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    CollisionObjects = new [] { CollisionObjectTypes.YasuoWall },
                    MissileSpellName = "SwainE"
                });

            #endregion Swain

            #region Syndra

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "SyndraQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 700,
                    Range = 800,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileDelayed = true,
                    MissileSpellName = "SyndraQSpell",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "syndrawcast",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 950,
                    Radius = 210,
                    MissileSpeed = 1450,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "syndrawcast",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "syndrae5",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 950,
                    Radius = 100,
                    MissileSpeed = 2000,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "syndrae5",
                    DisableFowDetection = true,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "SyndraE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 950,
                    Radius = 100,
                    MissileSpeed = 2000,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    DisableFowDetection = true,
                    MissileSpellName = "SyndraE",
                });

            #endregion Syndra

            #region Taliyah

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Taliyah",
                    SpellName = "TaliyahQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    MissileSpeed = 3600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "TaliyahQMis",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Minion, CollisionObjectTypes.Champions },
                    DisabledByDefault = true,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Taliyah",
                    SpellName = "TaliyahW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 600,
                    Range = 900,
                    Radius = 200,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "TaliyahW",
                });

            #endregion Taliyah

            #region Talon

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Talon",
                    SpellName = "TalonRake",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 80,
                    MissileSpeed = 2300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MultipleNumber = 3,
                    MultipleAngle = 20*(float) Math.PI/180,
                    MissileSpellName = "talonrakemissileone",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Talon",
                    SpellName = "TalonRakeReturn",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 80,
                    MissileSpeed = 1850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MultipleNumber = 3,
                    MultipleAngle = 20*(float) Math.PI/180,
                    MissileSpellName = "talonrakemissiletwo",
                });

            #endregion Riven

            #region Tahm Kench

            Spells.Add(
                new SpellData
                {
                    ChampionName = "TahmKench",
                    SpellName = "TahmKenchQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 951,
                    Radius = 90,
                    MissileSpeed = 2800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "tahmkenchqmissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Minion, CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });

            #endregion Tahm Kench

            #region Taric

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Taric",
                    SpellName = "TaricE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 1000,
                    Range = 750,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "TaricE"
                });

            #endregion Taric

            #region Thresh
            //Spells.Add(
            //new SpellData
            //{
            //    ChampionName = "Thresh",
            //    SpellName = "ThreshQInternal",
            //    Slot = SpellSlot.Q,
            //    Type = SkillShotType.SkillshotCircle,
            //    Delay = 1000,
            //    Range = 0,
            //    Radius = 1100,
            //    MissileSpeed = int.MaxValue,
            //    FixedRange = true,
            //    AddHitbox = true,
            //    DangerValue = 3,
            //    IsDangerous = false,
            //    FollowCaster = true,
            //    DisabledByDefault = true,
            //});
            Spells.Add(

                new SpellData
                {
                    ChampionName = "Thresh",
                    SpellName = "ThreshQInternal",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1100,
                    Radius = 70,
                    MissileSpeed = 1900,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Minion, CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });
            Spells.Add(

               new SpellData
               {
                   ChampionName = "Thresh",
                   SpellName = "ThreshQ",
                   Slot = SpellSlot.Q,
                   Type = SkillShotType.SkillshotMissileLine,
                   Delay = 500,
                   Range = 1100,
                   Radius = 50,
                   MissileSpeed = 1900,
                   FixedRange = true,
                   AddHitbox = true,
                   DangerValue = 3,
                   IsDangerous = true,
                   MissileSpellName = "ThreshQMissile",
                   EarlyEvade = new[] { EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects },
                   CanBeRemoved = true,
                   CollisionObjects =
                       new[]
                       {CollisionObjectTypes.Minion, CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
               });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Thresh",
                    SpellName = "ThreshEFlay",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 125,
                    Range = 1075,
                    Radius = 110,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    Centered = true,
                    MissileSpellName = "ThreshEMissile1",
                });

            #endregion Thresh

            #region Tristana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Tristana",
                    SpellName = "RocketJump",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 900,
                    Radius = 270,
                    MissileSpeed = 1500,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "RocketJump",
                });

            #endregion Tristana

            #region Tryndamere

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Tryndamere",
                    SpellName = "slashCast",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 660,
                    Radius = 93,
                    MissileSpeed = 1300,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "slashCast",
                });

            #endregion Tryndamere

            #region TwistedFate

            Spells.Add(
                new SpellData
                {
                    ChampionName = "TwistedFate",
                    SpellName = "WildCards",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1450,
                    Radius = 40,
                    MissileSpeed = 1000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SealFateMissile",
                    MultipleNumber = 3,
                    MultipleAngle = 28*(float) Math.PI/180,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion TwistedFate

            #region Twitch

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Twitch",
                    SpellName = "TwitchVenomCask",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 900,
                    Radius = 275,
                    MissileSpeed = 1400,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "TwitchVenomCaskMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Twitch

            #region Urgot

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellName = "UrgotQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 525,
                    Range = 800,
                    Radius = 150,
                    MissileSpeed = 5000,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "UrgotQMissile",
                    CollisionObjects = new [] { CollisionObjectTypes.YasuoWall },
                    CanBeRemoved = false,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellName = "UrgotE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 500,
                    Radius = 100,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellName = "UrgotR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 400,
                    Range = 500,
                    Radius = 80,
                    MissileSpeed = 3200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "UrgotR",
                    CollisionObjects = new [] { CollisionObjectTypes.YasuoWall }
                });

            #endregion Urgot

            #region Varus

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Varus",
                    SpellName = "VarusQMissilee",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1800,
                    Radius = 70,
                    MissileSpeed = 1900,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VarusQMissile",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Varus",
                    SpellName = "VarusE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1000,
                    Range = 925,
                    Radius = 235,
                    MissileSpeed = 1500,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VarusE",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Varus",
                    SpellName = "VarusR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 120,
                    MissileSpeed = 1950,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "VarusRMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies},
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });

            #endregion Varus

            #region Veigar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarBalefulStrike",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 70,
                    MissileSpeed = 2200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VeigarBalefulStrikeMis",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarDarkMatterCastLockout",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1350,
                    Range = 900,
                    Radius = 225,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarEventHorizon",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotRing,
                    Delay = 500,
                    Range = 700,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 3,
                    IsDangerous = true,
                    DontAddExtraDuration = true,
                    RingRadius = 350,
                    ExtraDuration = 3300,
                    DontCross = true,
                    MissileSpellName = "",
                });

            #endregion Veigar

            #region Velkoz

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 50,
                    MissileSpeed = 1300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VelkozQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Minion, CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozQSplit",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 55,
                    MissileSpeed = 2100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VelkozQMissileSplit",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Minion, CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 88,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VelkozWMissile",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 800,
                    Radius = 225,
                    MissileSpeed = 1500,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VelkozEMissile",
                });

            #endregion Velkoz

            #region Vi

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Vi",
                    SpellName = "Vi-q",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 90,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ViQMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies},
                });

            #endregion Vi

            #region Viktor

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Viktor",
                    SpellName = "Laser",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 80,
                    MissileSpeed = 1050,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ViktorDeathRayMissile",
                    ExtraMissileNames = new[] {"viktoreaugmissile"},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Viktor

            #region Xerath

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathArcanopulse2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 1600,
                    Radius = 95,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "xeratharcanopulse2",
                    EndAtParticle = "xerath_.+_q_aoe_reticle_red"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathArcaneBarrage2",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 700,
                    Range = 1000,
                    Radius = 200,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "XerathArcaneBarrage2",
                    EndAtParticle = "xerath_.+_w_aoe_red"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathMageSpear",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1150,
                    Radius = 60,
                    MissileSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "XerathMageSpearMissile",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Minion, CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall}
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathRMissileWrapper",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 700,
                    Range = 5600,
                    Radius = 130,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileDelayed = true,
                    MissileSpellName = "XerathLocusPulse",
                    EndAtParticle = "xerath_.+_r_aoe_reticle_red"
                });

            #endregion Xerath

            #region Yasuo 

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yasuo",
                    SpellName = "YasuoQ3",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1150,
                    Radius = 90,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    Invert = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "yasuoq3mis",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            #endregion Yasuo

            #region Zac

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zac",
                    SpellName = "ZacQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 120,
                    MissileSpeed = 2800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zac",
                    SpellName = "ZacE2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1800,
                    Radius = 250,
                    MissileSpeed = 1500,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    EndAtParticle = "zac_.+_e_tar_red"
                });

            #endregion Zac

            #region Zed

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zed",
                    SpellName = "ZedQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 925,
                    Radius = 50,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZedQMissile",
                    //FromObjects = new[] { "Zed_Clone_idle.troy", "Zed_Clone_Idle.troy" },
                    FromObjects = new[] {"Zed_Base_W_tar.troy", "Zed_Base_W_cloneswap_buf.troy"},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Zed

            #region Ziggs

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 140,
                    MissileSpeed = 1700,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsQSpell",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = false,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQBounce1",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 140,
                    MissileSpeed = 1700,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsQSpell2",
                    ExtraMissileNames = new[] {"ZiggsQSpell2"},
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = false,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQBounce2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 160,
                    MissileSpeed = 1700,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsQSpell3",
                    ExtraMissileNames = new[] {"ZiggsQSpell3"},
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CanBeRemoved = false,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    ExtraDuration = 5000,
                    Range = 1000,
                    Radius = 275,
                    MissileSpeed = 1750,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsW",
                    ToggleParticleName = "Ziggs.+_W_aoe",
                    DontCross = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 900,
                    Radius = 235,
                    MissileSpeed = 1750,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    DontCross = true,
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Ziggs",
                   SpellName = "ZiggsE",
                   MissileSpellName = "ZiggsE3",
                   Slot = SpellSlot.E,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 50,
                   ExtraDuration = 11000,
                   ToggleParticleName = "Ziggs.+_E_aoe",
                   Range = 900,
                   Radius = 20,
                   MissileSpeed = 1750,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 2,
                   IsDangerous = false,
                   DontCross = true,
               });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 0,
                    Range = 5300,
                    Radius = 500,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsR",
                    DisableFowDetection = true,
                });

            #endregion Ziggs

            #region Zilean

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zilean",
                    SpellName = "ZileanQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250 + 450,
                    ExtraDuration = 3050,
                    Range = 900,
                    Radius = 140,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DontCross = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "ZileanQMissile",
                    ToggleParticleName = "Zilean_.+_Q.+attach_mis",
                    //EndAtParticle = "Zilean_.+_Q.+TimeBombFixGreen",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });

            #endregion Zilean

            #region Zyra

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "ZyraQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 850,
                    Range = 800,
                    Radius = 140,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZyraQ",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "ZyraE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1150,
                    Radius = 70,
                    MissileSpeed = 1150,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ZyraE",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "zyrapassivedeathmanager",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1474,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "zyrapassivedeathmanager",
                    EarlyEvade = new[] {EarlyObjects.Allies, EarlyObjects.Minions, EarlyObjects.AllyObjects},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            #endregion Zyra

            #region Senna

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Senna",
                    SpellName = "SennaQCast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 310,
                    Range = 1350,
                    Radius = 75,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Senna",
                    SpellName = "SennaW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1250,
                    Radius = 85,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    CanBeRemoved = true,
                    MissileSpellName = "SennaW",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Senna",
                    SpellName = "SennaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 1000,
                    Range = 18000,
                    Radius = 180,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            #endregion Senna

            #region Aphelios

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Aphelios",
                    SpellName = "ApheliosCalibrumQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 350,
                    Range = 1500,
                    Radius = 65,
                    MissileSpeed = 1850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    CanBeRemoved = true,
                    IsDangerous = false,
                    MissileSpellName = "ApheliosCalibrumQ",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Aphelios",
                    SpellName = "ApheliosR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 450,
                    Range = 1650,
                    Radius = 125,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "ApheliosR",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            #endregion Aphelios

            #region Sylas

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sylas",
                    SpellName = "SylasE2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 850,
                    Radius = 65,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    CanBeRemoved = true,
                    IsDangerous = true,
                    MissileSpellName = "SylasE2",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sylas",
                    SpellName = "SylasQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1150,
                    Range = 725,
                    Radius = 150,
                    MinimalRange = 175,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sylas",
                    SpellName = "SylasQLine",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 400,
                    Range = 725,
                    Radius = 55,
                    MinimalRange = 175,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                });

            #endregion Sylas

            #region Sett

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sett",
                    SpellName = "SettW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 750,
                    Range = 785,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "SettW",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sett",
                    SpellName = "SettE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    Range = 490,
                    Radius = 160,
                    Centered = true,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true
                });

            #endregion Sett

            #region Pyke

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Pyke",
                    SpellName = "PykeQRange",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 80,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "PykeQRange",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Pyke",
                    SpellName = "PykeE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 1000,
                    Range = 1100,
                    Radius = 110,
                    MissileSpeed = 2500,
                    AddHitbox = true,
                    DangerValue = 3,
                    MissileFollowsUnit = true,
                    IsDangerous = true,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Pyke",
                    SpellName = "PykeR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    Range = 750,
                    Radius = 85,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true
                });

            #endregion Pyke

            #region Warwick

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Warwick",
                    SpellName = "WarwickR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 100,
                    Range = 2000,
                    Radius = 150,
                    ExtraRange = 100,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "PykeQRange",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions },
                });

            #endregion Warwick

            #region Yorick

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yorick",
                    SpellName = "YorickW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 750,
                    Range = 600,
                    Radius = 250,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = false,
                    DangerValue = 2,
                    IsDangerous = false,
                    DisabledByDefault = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yorick",
                    SpellName = "YorickE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 330,
                    Range = 550,
                    Radius = 120,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = true
                });

            #endregion Yorick

            #region Neeko

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Neeko",
                    SpellName = "NeekoQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 800,
                    Radius = 225,
                    MissileSpeed = 1800,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "NeekoQ"
                });
            
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Neeko",
                    SpellName = "NeekoE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 85,
                    MissileSpeed = 1400,
                    AddHitbox = true,
                    FixedRange = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "NeekoQ",
                    CollisionObjects = new [] { CollisionObjectTypes.YasuoWall }
                });

            #endregion Neeko

            #region Zoe

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zoe",
                    SpellName = "ZoeQMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 875,
                    Radius = 60,
                    MissileSpeed = 1200,
                    AddHitbox = true,
                    FixedRange = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    CanBeRemoved = true,
                    MissileSpellName = "ZoeQMissile",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zoe",
                    SpellName = "ZoeQ2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 2500,
                    Radius = 70,
                    ExtraRange = 100,
                    MissileSpeed = 2500,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    CanBeRemoved = true,
                    MissileSpellName = "ZoeQMis2",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zoe",
                    SpellName = "ZoeE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    ExtraRange = 100,
                    Radius = 60,
                    MissileSpeed = 1700,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "ZoeEMis",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zoe",
                    SpellName = "ZoeEBubble",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 900,
                    Range = 900,
                    ExtraDuration = 4500,
                    Radius = 250,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileDelayed = true,
                    MissileSpellName = "ZoeEc"
                });

            #endregion Zoe

            #region Qiyana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Qiyana",
                    SpellName = "QiyanaQ_Rock",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 875,
                    Radius = 85,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "QiyanaQ_Rock",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Qiyana",
                    SpellName = "QiyanaQ_Grass",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 875,
                    Radius = 85,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "QiyanaQ_Grass",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Qiyana",
                    SpellName = "QiyanaQ_Water",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 875,
                    Radius = 85,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "QiyanaQ_Water",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Qiyana",
                    SpellName = "QiyanaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 875,
                    Radius = 200,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "QiyanaRMis",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });

            #endregion Qiyana

            #region Kaisa

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kaisa",
                    SpellName = "KaisaW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 450,
                    Range = 3000,
                    Radius = 80,
                    MissileSpeed = 1750,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "KaisaW",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion }
                });

            #endregion Kaisa

            #region Ornn

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ornn",
                    SpellName = "OrnnQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 65,
                    MissileSpeed = 1800,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "OrnnQMissile",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ornn",
                    SpellName = "OrnnRWave",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 3500,
                    Radius = 200,
                    MissileSpeed = 1650,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = false,
                    FixedRange = true,
                    CanBeRemoved = false,
                    MissileSpellName = "OrnnRWave2",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion }
                });

            #endregion Ornn

            #region Kayn

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kayn",
                    SpellName = "KaynW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    Range = 700,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FixedRange = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kayn",
                    SpellName = "KaynAssW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 875,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FixedRange = true,
                    EndAtParticle = "shadowclone"
                });

            #endregion Kayn

            #region Rakan

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rakan",
                    SpellName = "RakanQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 850,
                    Radius = 65,
                    MissileSpeed = 1850,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    CanBeRemoved = true,
                    FixedRange = true,
                    MissileSpellName = "RakanQMis",
                    CollisionObjects = new [] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rakan",
                    SpellName = "RakanW",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 650,
                    Radius = 275,
                    MissileSpeed = 1700,
                    CanDetectDash = (sender, args) =>
                    {
                        return args.Speed == 1700 && Utils.ImmobileTime(sender) == -1;
                    },
                    IsDash = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true
                });
            #endregion Rakan

            #region Xayah

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xayah",
                    SpellName = "XayahQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 600,
                    Range = 1100,
                    Radius = 50,
                    MissileSpeed = 2400,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileDelayed = true,
                    CanBeRemoved = true,
                    FixedRange = true,
                    MissileSpellName = "XayahQMissile1",
                    ExtraMissileNames = new [] { "XayahQMissile2" },
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });

            #endregion Xayah

            #region Pantheon

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Pantheon",
                    SpellName = "PantheonQTap",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 300,
                    Range = 600,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Pantheon",
                    SpellName = "PantheonQMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 60,
                    MissileSpeed = 2500,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "PantheonQMissile",
                });
            #endregion Pantheon

            #region Akali

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Akali",
                    SpellName = "AkaliE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 900,
                    Radius = 70,
                    MissileSpeed = 1800,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "AkaliEMis",
                    CollisionObjects = new [] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall }
                });

            #endregion Akali

            #region Maokai

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Maokai",
                    SpellName = "MaokaiQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 350,
                    Range = 650,
                    Radius = 110,
                    MissileSpeed = 1600,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = true,
                    CanBeRemoved = true,
                    MissileSpellName = "MaokaiQMissile",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall }
                });

            #endregion Maokai

            #region Yone

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yone",
                    SpellName = "YoneQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 350,
                    Range = 475,
                    Radius = 50,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = true,
                    DisabledByDefault = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yone",
                    SpellName = "YoneQ3",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 350,
                    Range = 950,
                    Radius = 80,
                    MissileSpeed = 1500,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    FixedRange = true,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yone",
                    SpellName = "YoneR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 750,
                    Range = 950,
                    Radius = 140,
                    MissileSpeed = 1500,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    FixedRange = true,
                });

            #endregion Yone

            #region Lillia

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lillia",
                    SpellName = "LilliaW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 750,
                    Range = 500,
                    Radius = 250,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = false,
                    FixedRange = false
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lillia",
                    SpellName = "LilliaE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 700,
                    Radius = 150,
                    MissileSpeed = 1400,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    FixedRange = false,
                    MissileSpellName = "LilliaE",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lillia",
                    SpellName = "LilliaE2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 18000,
                    Radius = 60,
                    MissileSpeed = 1150,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    FixedRange = false,
                    MissileSpellName = "LilliaERollingMissile",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });

            #endregion Lillia

            #region Samira

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Samira",
                    SpellName = "SamiraQGun",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 2600,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = true,
                    MissileSpellName = "SamiraQGun",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Champions, CollisionObjectTypes.Minion }
                });

            #endregion Samira

            #region Seraphine

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Seraphine",
                    SpellName = "SeraphineQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 900,
                    Radius = 350,
                    MissileSpeed = 1300,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = false,
                    FixedRange = false,
                    MissileSpellName = "SeraphineQInitialMissile"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Seraphine",
                    SpellName = "SeraphineE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 70,
                    MissileSpeed = 1200,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FixedRange = true,
                    MissileSpellName = "SeraphineEMissile"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Seraphine",
                    SpellName = "SeraphineR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1250,
                    Radius = 160,
                    MissileSpeed = 1600,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    FixedRange = true,
                    MissileSpellName = "SeraphineR"
                });

            #endregion Seraphine

            #region Volibear

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Volibear",
                    SpellName = "VolibearE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 2000,
                    Range = 1250,
                    Radius = 325,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    EndAtParticle = "volibear_.+_e_aoe_warning"
                });

            #endregion

            #region Kayle

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kayle",
                    SpellName = "KayleQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 300,
                    Range = 900,
                    Radius = 60,
                    MissileSpeed = 2000,
                    DangerValue = 2,
                    FixedRange = true,
                    AddHitbox = true,
                    MissileSpellName = "KayleQMis"
                });

            #endregion
        }

        public static SpellData GetByDash(string championName)
        {
            foreach (var spellData in Spells)
            {
                if (!spellData.IsDash)
                {
                    continue;
                }

                if (championName == spellData.ChampionName)
                {
                    return spellData;
                }
            }

            return null;
        }

        public static SpellData GetByEndAtParticle(string objectName)
        {
            objectName = objectName.ToLowerInvariant();
            foreach (var spellData in Spells)
            {
                if (spellData.EndAtParticle.Length == 0)
                {
                    continue;
                }
                               
                var reg = new System.Text.RegularExpressions.Regex(spellData.EndAtParticle);

                if (reg.IsMatch(objectName))
                {
                    return spellData;
                }
            }

            return null;
        }

        public static SpellData GetBySourceObjectName(string objectName)
        {
            objectName = objectName.ToLowerInvariant();
            foreach (var spellData in Spells)
            {
                if (spellData.SourceObjectName.Length == 0)
                {
                    continue;
                }

                if (objectName.Contains(spellData.SourceObjectName))
                {
                    return spellData;
                }
            }

            return null;
        }

        public static SpellData GetByName(string spellName)
        {
            spellName = spellName.ToLower();
            foreach (var spellData in Spells)
            {
                if (spellData.SpellName.ToLower() == spellName || spellData.ExtraSpellNames.Contains(spellName))
                {
                    return spellData;
                }
            }

            return null;
        }

        public static SpellData GetByMissileName(string missileSpellName)
        {
            missileSpellName = missileSpellName.ToLower();
            foreach (var spellData in Spells)
            {
                if (spellData.MissileSpellName != null && spellData.MissileSpellName.ToLower() == missileSpellName ||
                    spellData.ExtraMissileNames.Contains(missileSpellName))
                {
                    return spellData;
                }
            }

            return null;
        }

        public static SpellData GetBySpeed(string ChampionName, int speed, int id = -1)
        {
            foreach (var spellData in Spells)
            {
                if (spellData.ChampionName == ChampionName && spellData.MissileSpeed == speed &&
                    (spellData.Id == -1 || id == spellData.Id))
                {
                    return spellData;
                }
            }

            return null;
        }
    }
}
