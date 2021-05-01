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
                        AddHitbox = false,
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

            Spells.Add(
            new SpellData
            {
                ChampionName = "AllChampions",
                DangerValue = 2,
                MissileSpeed = int.MaxValue,
                Radius = 160,
                Range = 2500,
                Delay = 1000,
                FixedRange = false,
                IsDangerous = true,
                AddHitbox = true,
                Slot = SpellSlot.Q,
                SpellName = "ArcaneComet",
                MissileDelayed = true,
                MissileSpellName = "Perks_ArcaneComet_Mis",
                ExtraMissileNames = new[] { "Perks_ArcaneComet_Mis_Arc" },
                EndAtParticle = "Perks_Meteor_AOE_Tar",
                Type = SkillShotType.SkillshotCircle
            });
            //Perks_ArcaneComet_Mis
            #region Azir
            Spells.Add(
             new SpellData
             {
                 ChampionName = "Azir",
                 SpellName = "AzirQ",
                 Slot = SpellSlot.Q,
                 Type = SkillShotType.SkillshotMissileLine,
                 Delay = 0,
                 DontCross = true,
                 Range = 1500,
                 Radius = 60,
                 MissileSpeed = 1600,
                 FixedRange = false,
                 AddHitbox = true,
                 DangerValue = 2,
                 MissileSpellName = "",
                 ToggleParticleName = "",
                 SourceObjectName = "Azir_Base_Q_SoldierMoveIndicator",
                 EndAtParticle = "",

             });
            Spells.Add(
              new SpellData
              {
                  ChampionName = "Azir",
                  SpellName = "AzirR",
                  Slot = SpellSlot.W,
                  Type = SkillShotType.SkillshotMissileLine,
                  Delay = 250,
                  Range = 900,
                  Radius = 350,
                  MissileSpeed = 1600,
                  FixedRange = true,
                  BehindStart = 300,
                  AddHitbox = true,
                  DangerValue = 4,
              });
            #endregion

            #region Aatrox

            Spells.Add(
                new SpellData
                {
                    ChampionName = "XinZhao",
                    SpellName = "XinZhaoR",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 0,
                    Radius = 500,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    SourceObjectName = "Aatrox_Base_Q_Indicator_01",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "XinZhao",
                    SpellName = "XinZhaoW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 900,
                    Radius = 60,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    SourceObjectName = "XinZhao.+W_IndicatorProjection",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Minion, CollisionObjectTypes.Champions }
                });

            #endregion Aatrox

            #region Aatrox

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Aatrox",
                    SpellName = "AatroxQ1",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    Range = 700,
                    Radius = 90,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    SourceObjectName = "Aatrox_Base_Q_Indicator_01",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Aatrox",
                    SpellName = "AatroxW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 825,
                    Radius = 60,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "AatroxW",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Minion, CollisionObjectTypes.Champions }
                });

            #endregion Aatrox

            #region Camille

            Spells.Add(
               new SpellData
               {
                   ChampionName = "Camille",
                   SpellName = "CamilleE",
                   Slot = SpellSlot.E,
                   Type = SkillShotType.SkillshotMissileLine,
                   Delay = 0,
                   Range = 900,
                   Radius = 80,
                   MissileSpeed = 2300,
                   FixedRange = true,
                   AddHitbox = true,
                   DangerValue = 3,
                   IsDangerous = true,
                   SourceObjectName = "Camille_.+_E_indicator_dash_.+",
                   CanBeRemoved = true,
                   CollisionObjects =
                       new[]
                       {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
               });


            #endregion Camille
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    Radius = 70,
                    MissileSpeed = 1550,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "AhriSeduceMissile",
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
                    SpellName = "FlashFrostSpell",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 350,
                    Range = 1350,
                    Radius = 200,
                    MissileSpeed = 950,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "FlashFrostSpell",
                    CanBeRemoved = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall}
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Anivia",
                   SpellName = "AniviaR",
                   Slot = SpellSlot.R,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 1000,
                   //ExtraDuration = 5500,
                   //DontAddExtraDuration = true,
                   Range = 1100,
                   Radius = 275,
                   MissileSpeed = 1300,
                   FixedRange = false,
                   AddHitbox = true,
                   IsDangerous = true,
                   DontCross = true,
                   ToggleParticleName = "anivia_.+_r_full",
                   EndAtParticle = "anivia_.+_r_aoe_",
                   SourceObjectName = "anivia_.+_r_aoe_",
                   DangerValue = 2,
 
               });
            Spells.Add(
              new SpellData
              {
                  ChampionName = "Anivia",
                  SpellName = "AniviaR2",
                  Slot = SpellSlot.R,
                  Type = SkillShotType.SkillshotCircle,
                  Delay = 1000,
                  ExtraDuration = 400,
                   //DontAddExtraDuration = true,
                  Range = 1100,
                  Radius = 350,
                  MissileSpeed = 1300,
                  FixedRange = false,
                  AddHitbox = true,
                  IsDangerous = true,
                  DontCross = true,
                  ToggleParticleName = "anivia_.+_r_full",
                  SourceObjectName = "anivia_.+_r_full",
                  DangerValue = 2,
              });
            #endregion Anivia

            #region Annie

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    SpellName = "AnnieW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 625,
                    Radius = 50,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    SpellName = "AnnieR",
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
                    IsDangerous = true,
                    MissileSpellName = "VolleyAttack",
                    MultipleNumber = 9,
                    MultipleAngle = 4.62f * (float) Math.PI/180,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    TakeClosestPath = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "",
                    SourceObjectName = "brand_.+_w.+tar_red"
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

            #region Gankplank

          
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gangplank",  
                    SpellName = "GangplanakE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1350,
                    ExtraDuration = 23000,
                    DontAddExtraDuration = true,
                    Range = 800,
                    Radius = 250,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DontCross = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    DontCheckForDuplicates = true,
                    SourceObjectName = "Barrel",
                    EndAtParticle = "_Tar",
                });

            #endregion Gankplank

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
                    IsDangerous = true,
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
                    ExtraDuration = 30000,
                    DontAddExtraDuration = true,
                    Range = 800,
                    Radius = 60,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DontCross = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    DontCheckForDuplicates = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "MissileBarrageMissile",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "MissileBarrageMissile2",
                    
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall}
                });

            #endregion Corki

            //#region Jax

            //Spells.Add(
            //    new SpellData
            //    {
            //        ChampionName = "Jax",
            //        SpellName = "JaxE",
            //        Slot = SpellSlot.E,
            //        Type = SkillShotType.SkillshotCircle,
            //        Delay = 750,
            //        Range = 0,
            //        Radius = 425 - 50,
            //        MissileSpeed = int.MaxValue,
            //        FixedRange = true,
            //        AddHitbox = true,
            //        DangerValue = 3,
            //        IsDangerous = true,
            //        FollowCaster = true,
            //        DisabledByDefault = true,
            //        ToggleParticleName = "Jax_base_E_buf"
            //    });

            //#endregion Jax

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
                    Radius = 425,
                    MissileSpeed = int.MaxValue, 
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    FollowCaster = true,
                    SourceObjectName = "Darius.+Q_Ring_Windup"
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
                    Radius = 60,
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
                    Range = 800,
                    Radius = 210,
                    MissileSpeed = 1400,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DianaQ",
                    
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
                    Range = 1300,
                    Radius = 195,
                    DontCross = true,   
                    MissileSpeed = 1400,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DianaArcArc",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "InfectedCleaverMissile",
                    
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
                    MissileSpellName = "EkkoQMis",
                    CanBeRemoved = true,
                    DontCross = true,
                    CollisionObjects =
                        new[] {CollisionObjectTypes.YasuoWall}
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Ekko",
                   SpellName = "EkkoQReturn",
                   Slot = SpellSlot.Q,
                   Type = SkillShotType.SkillshotMissileLine,
                   Delay = 250,
                   Range = 950,
                   Radius = 60,
                   MissileSpeed = 2300,
                   FixedRange = true,
                   AddHitbox = true,
                   DangerValue = 4,
                   IsDangerous = true,
                   MissileSpellName = "EkkoQReturn",
                   CanBeRemoved = true,
                   DontCross = true,
                   CollisionObjects =
                       new[] { CollisionObjectTypes.YasuoWall }
               });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoQball",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 950,
                    ExtraDuration = 2200,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "EkkoQMis",
                    CanBeRemoved = true,
                    DontCross = true,
                    CollisionObjects =
                        new[] { CollisionObjectTypes.YasuoWall }
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 3500,
                    Range = 1600,
                    Radius = 350,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 3,
                    IsDangerous = true,
                    ParticleDetectDelay = 300,
                    SourceObjectName = "Ekko.+W_Indicator",
                    CanBeRemoved = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 800,
                    Range = 3000,
                    Radius = 375,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    CanBeRemoved = true,
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
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 470,
                    Radius = 180,
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
                    Delay = 260,
                    Range = 1250,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "EzrealQ",
                    
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
                    Delay = 260,
                    Range = 1250,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    CanBeRemoved = true,
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "FioraWMissile",
                    //SourceObjectName = "Fiora_base_w_cas",
                    //EndAtParticle = "Fiora_base_w_cas",
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
                    DontCheckForDuplicates = true,
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall},
                    CanBeRemoved = true,
                });

            Spells.Add(
               new SpellData
               {
                   ChampionName = "Fizz",
                   SpellName = "FizzR",
                   Slot = SpellSlot.R,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 950,
                   ParticleDetectDelay = 250,
                   ExtraDuration = 2000,
                   DontAddExtraDuration = true,
                   Range = 1275,
                   Radius = 250,
                   MissileSpeed = int.MaxValue,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 5,
                   IsDangerous = true,
                   SourceObjectName = "Fizz_Base_R_DeadFish",
                   DontCheckForDuplicates = true,
                   DontCross = true,
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
                    IsDangerous = true,
                    
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "GnarBigQMissile",
                    
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    Range = 800,
                    Radius = 200,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "GragasE",
                    
                    CanBeRemoved = true,
                    ExtraRange = 300,
                    IsDash = true,

                    CanDetectDash = (sender, args) =>
                    {
                        return Utils.ImmobileTime(sender) == -1;
                    },
                    CollisionObjects = new[] {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1000,
                    Range = 1050,
                    Radius = 375,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasRfow",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1000,
                    Range = 1050,
                    Radius = 375,
                    MissileSpeed = 1300,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "GragasRBoom",
                    EndAtParticle = "Gragas_Base_R_End",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
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
                    IsDangerous = true,
                    MissileSpellName = "GravesQLineMis",

                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });
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
                   IsDangerous = true,
                   MissileSpellName = "GravesQReturn",
                   CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
               });
            Spells.Add(
              new SpellData
              {
                  ChampionName = "Graves",
                  SpellName = "GravesSmokeGrenade",
                  Slot = SpellSlot.W,
                  Type = SkillShotType.SkillshotCircle,
                  Delay = 350,
                  Range = 950,
                  Radius = 220,
                  ExtraDuration = 3700,
                  DontAddExtraDuration = true,
                  DontCross = true,
                  MissileSpeed = 1500,
                  FixedRange = false,
                  AddHitbox = true,
                  DangerValue = 4,
                  IsDangerous = true,
                  MissileSpellName = "GravesSmokeGrenadeBoom",
                  CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
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
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Graves",
                    SpellName = "GravesChargeShotFxMissile2",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 800,
                    Radius = 10,
                    MissileSpeed = 2100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "GravesChargeShotFxMissile2",
                    
                });
            #endregion Graves

            #region Heimerdinger

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Heimerdinger",
                    SpellName = "HeimerdingerWM",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 70,
                    MissileSpeed = 2200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "HeimerdingerWAttack2",
                    DontCheckForDuplicates = true,
                    ExtraMissileNames = new[]{ "HeimerdingerWAttack2Ult" },
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    SourceObjectName = "JarvanIV_.+_E_tar_red",
                });

            #endregion JarvanIV

            #region Jayce

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jayce",
                    SpellName = "JayceShockBlast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 80,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "JayceShockBlastMis",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "JayceShockBlastWallMis",
                    
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
                    Radius = 50,
                    MissileSpeed = 5000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JhinWMissile",
                    SourceObjectName = "Jhin_.+_W_indicator",
                    
                    DontCross = true,
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Jhin",
                   SpellName = "JhinE",
                   Slot = SpellSlot.E,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 500,
                   ExtraDuration = 180000,
                   DontAddExtraDuration = true,
                   Range = 750,
                   Radius = 110,
                   MissileSpeed = 1600,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 2,
                   MissileSpellName = "JhinETrap",
                   ToggleParticleName = "Noxious Trap",
                   DontCross = true,
                   CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
               });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jhin",
                    SpellName = "JhinRShot",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 350,
                    Range = 3500,
                    Radius = 80,
                    MissileSpeed = 5000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "JhinRShotMis",
                    ExtraMissileNames = new[] { "JhinRShotMis4" },
                    //
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
                    SourceObjectName = "Jinx_.+_W_Beam",
                    
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Jinx",
                   SpellName = "JinxE",
                   Slot = SpellSlot.E,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 1200,
                   ExtraDuration = 5500,
                   DontAddExtraDuration = true,
                   Range = 900,
                   Radius = 60,
                   MissileSpeed = 1750,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 3,
                   IsDangerous = true,
                   DontCross = true,
                   MissileSpellName = "JinxEHit",
                   DontCheckForDuplicates = true,
                   CollisionObjects =new[] {CollisionObjectTypes.YasuoWall},
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
                    IsDangerous = true,
                    MissileSpellName = "kalistamysticshotmis",
                    ExtraMissileNames = new[] {"kalistamysticshotmistrue"},
                    
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
                    Radius = 80,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "KarmaQMissile",
                    ExtraMissileNames = new string[] { "KarmaQMissileMantra"},
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
               new SpellData
               {
                   ChampionName = "Karma",
                   SpellName = "KarmaQMantraCircle",
                   Slot = SpellSlot.Q,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 1800,
                   Range = 1000,
                   Radius = 240,
                   MissileSpeed = int.MaxValue,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 3,
                   IsDangerous = true,
                   SourceObjectName = "Karm.+Q_impact_R_01",
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "RiftWalk",
                });

            Spells.Add(
               new SpellData
               {
                   ChampionName = "Kassadin",
                   SpellName = "ForcePulse",
                   Slot = SpellSlot.E,
                   Type = SkillShotType.SkillshotCone,
                   Delay = 350,
                   Range = 625,
                   Radius = 80,
                   MissileSpeed = int.MaxValue,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 3,
                   IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "KennenShurikenHurlMissile1",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "KhazixWMissile",
                    
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "KogMawQ",
                    
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    Radius = 75,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    TakeClosestPath = true,
                    MissileSpellName = "LeonaZenithBladeMissile",

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
                    Radius = 140,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    SourceObjectName = "Leona.+R_hit_aoe_",
                });

            //Spells.Add(
            //   new SpellData
            //   {
            //       ChampionName = "Leona",
            //       SpellName = "LeonaR2",
            //       ExtraSpellNames = new string[] {  },
            //       Slot = SpellSlot.R,
            //       Type = SkillShotType.SkillshotCircle,
            //       Delay = 1000,
            //       Range = 1200,
            //       Radius = 290,
            //       MissileSpeed = int.MaxValue,
            //       FixedRange = false,
            //       AddHitbox = true,
            //       DangerValue = 5,
            //       IsDangerous = true,
            //       SourceObjectName = "Leona.+R_hit_aoe_",
            //       DontCheckForDuplicates = true,
            //   });

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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    Delay = 400,
                    Range = 1000,
                    Radius = 50,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
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
                    Range = 1350,
                    Radius = 80,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "lucianwmissile",
                    
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianRMis",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 0,
                    ExtraDuration = 800,
                    Range = 1400,
                    Radius = 120,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "lucianrmissileoffhand",
                    ExtraMissileNames = new[] {"lucianrmissile"},
                    
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "LuluQMissileTwo",
                    
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
                    Radius = 80,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LuxLightBindingMis",
                    
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
                    DontAddExtraDuration = true,
                    Range = 1100,
                    Radius = 275,
                    MissileSpeed = 1300,
                    FixedRange = false,
                    AddHitbox = true,
                    IsDangerous = true,
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
                    Radius = 110,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    ParticleRotation = 180,
                    SourceObjectName = "Lux_.+_R_cas", 
                    MissileSpellName = "LuxRVfxMis",
                    EndAtParticle = "lux_.+_r_mis_beam"
                });

            #endregion Lux

            #region Malphite

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Malphite",
                    SpellName = "MalphiteR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 0,
                    ExtraDuration = 1000,
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
                        return Utils.ImmobileTime(sender) == -1;
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
                    IsDangerous = true,
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
                    
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[]
                        {CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall},
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Morgana",
                    SpellName = "MorganaW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 950,
                    ParticleDetectDelay = 250,
                    Range = 1300,
                    Radius = 280,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                    SourceObjectName = "morgana.+w_cas",
                    CanBeRemoved = true,
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
                    SourceObjectName = "Nami.+Q_indicator_red",
                    ParticleDetectDelay = 150
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
                    IsDangerous = true,
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
                    Radius = 85,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OrianaW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1500,
                    Radius = 255,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "OrianaDissonanceCommand-",
                    FromObject = "yomu_ring_",
                    SourceObjectName = "Orianna_.+_W_Dissonance_cas_red", //Orianna_Base_W_Dissonance_ball_green.troy & Orianna_Base_W_Dissonance_cas_green.troy 
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Orianna",
                   SpellName = "OriannaW2",
                   Slot = SpellSlot.W,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 250,
                   Range = 1500,
                   Radius = 255,
                   MissileSpeed = int.MaxValue,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 2,
                   IsDangerous = true,
                   MissileSpellName = "OrianaDissonanceCommand-",
                   FromObject = "yomu_ring_",
                   SourceObjectName = "Orianna_.+_W_Dissonance_ball_red", //Orianna_Base_W_Dissonance_ball_green.troy & Orianna_Base_W_Dissonance_cas_green.troy 
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
                    IsDangerous = true,
                    MissileSpellName = "orianaredact",
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OrianaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 700,
                    Range = 1500,
                    Radius = 400,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    SourceObjectName = "Orianna.+R_VacuumIndicator", //Orianna_Base_R_VacuumIndicator.troy
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
                    IsDangerous = true,
                    MissileSpellName = "QuinnQ",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "PoppyQ",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "RekSaiQBurrowedMis",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "RumbleGrenade",
                    
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "RyzeQ",
                    
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
                    Delay = 250,
                    Range = 950,
                    Radius = 240,
                    MissileSpeed = 1100,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "SorakaQMissile",
                    
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Soraka",
                    SpellName = "SorakaE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1600,
                    Range = 1200,
                    Radius = 240,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "",
                    DontCross = true,
                    SourceObjectName = "soraka_.+e_beam",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
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
                    IsDangerous = true,
                    MissileSpellName = "ShyvanaFireballMissile",
                    
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    SourceObjectName = "Swain_.+AOE_Initial"

                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Swain",
                    SpellName = "SwainE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1150,
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
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Swain",
                   SpellName = "SwainEReturn",
                   Slot = SpellSlot.E,
                   Type = SkillShotType.SkillshotMissileLine,
                   Delay = 250,
                   Range = 1000,
                   Radius = 90,
                   ExtraRange = 150,
                   MissileSpeed = 1200,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 3,
                   IsDangerous = true,
                   CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                   MissileSpellName = "SwainEReturnMissile"
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
                    IsDangerous = true,
                    MissileDelayed = true,
                    MissileSpellName = "SyndraQSpell",
                });

            //Spells.Add(
            //    new SpellData
            //    {
            //        ChampionName = "Syndra",
            //        SpellName = "syndrawcast",
            //        Slot = SpellSlot.W,
            //        Type = SkillShotType.SkillshotCircle,
            //        Delay = 250,
            //        Range = 950,
            //        Radius = 210,
            //        MissileSpeed = 1450,
            //        FixedRange = false,
            //        AddHitbox = true,
            //        DangerValue = 2,
            //        IsDangerous = true,
            //        MissileSpellName = "syndrawcast",
            //        SourceObjectName = "Syndra_Skin01_W_fling_01"
            //    });

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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "TaliyahQMis",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall, CollisionObjectTypes.Minion, CollisionObjectTypes.Champions },
                    DisabledByDefault = true,
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Taliyah",
                    SpellName = "TaliyahWVC",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 800,
                    Range = 900,
                    Radius = 190,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    SourceObjectName = "OffsetMinion",
                });

            #endregion Taliyah

            #region Talon

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Talon",
                    SpellName = "TalonW",
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
                    MissileSpellName = "TalonWMissileOne",
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
                    MissileSpellName = "TalonWMissileTwo",
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
            //    IsDangerous = true,
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
                    Radius = 60,
                    MissileSpeed = 1900,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    TakeClosestPath = true,
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
                   Radius = 60,
                   MissileSpeed = 1900,
                   FixedRange = true,
                   AddHitbox = true,
                   DangerValue = 3,
                   IsDangerous = true,
                   MissileSpellName = "ThreshQMissile",
                   
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
                    SpellName = "TristanaW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 900,
                    Radius = 270,
                    MissileSpeed = 1500,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    Radius = 75,
                    MissileSpeed = 1900,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
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
                    Radius = 250,
                    MissileSpeed = 1500,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "VeigarBalefulStrikeMis",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    SpellName = " ",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotRing,
                    Delay = 1000,
                    Range = 700,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 3,
                    IsDangerous = true,
                    RingRadius = 350,
                    ExtraDuration = 2800,
                    DontAddExtraDuration = true,
                    DontCross = true,
                    //DontCheckForDuplicates = true,
                    SourceObjectName = "veigar_base_e_warning_red"
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarEventHorizon",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotRing,
                    Delay = 1000,
                    Range = 700,
                    Radius = 140,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = false,
                    DangerValue = 3,
                    IsDangerous = true,
                    RingRadius = 240,
                    DontCross = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "VelkozQMissileSplit",
                    
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileSpellName = "ViktorDeathRayMissile",
                    ExtraMissileNames = new[] {"viktoreaugmissile"},
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });
            Spells.Add(
             new SpellData
             {
                 ChampionName = "Viktor",
                 SpellName = "ViktorGravitonField",
                 Slot = SpellSlot.W,
                 Type = SkillShotType.SkillshotCircle,
                 Delay = 1500,
                 Range = 800,
                 Radius = 270,
                 ExtraDuration = 2000,
                 DontAddExtraDuration = true,
                 DontCross = true,
                 MissileSpeed = int.MaxValue,
                 FixedRange = false,
                 AddHitbox = true,
                 DangerValue = 4,
                 IsDangerous = true,
                 SourceObjectName = "Viktor_Catalyst",
 
             });

            #endregion Viktor

            #region Xerath

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathArcanopulse",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 1550,
                    Radius = 95,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    SourceObjectName = "^Xerath_.+_Q_cas$"
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
                    IsDangerous = true,
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
                    MissileSpellName = "YasuoQ3Mis",
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
                    IsDangerous = true,
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
                    MissileSpeed = 1200,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    SourceObjectName = "Zac.+E_Tar"
                });

            #endregion Zac

            #region Zed

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zed",
                    SpellName = "ZedW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 750,
                    Radius = 240,
                    MissileSpeed = 2500,
                    ExtraDuration = 5000,
                    DontAddExtraDuration = false,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    DontCross = true,
                    DontCheckForDuplicates = true,
                    MissileSpellName = "ZedWMissile",
                    ToggleParticleName = "zed_.+_cloneswap",
                    SourceObjectName = "zed_.+_cloneswap"
                });

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
                   IsDangerous = true,
                   MissileSpellName = "ZedQMissile",
                    //FromObjects = new[] { "Zed_Clone_idle.troy", "Zed_Clone_Idle.troy" },
                    FromObjects = new[] { "Zed_Base_W_tar.troy", "Zed_Base_W_cloneswap_buf.troy" },
                   CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
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
                    IsDangerous = true,
                    MissileSpellName = "ZiggsQSpell",
                    
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
                    IsDangerous = true,
                    MissileSpellName = "ZiggsQSpell2",
                    ExtraMissileNames = new[] {"ZiggsQSpell2"},
                    
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
                    IsDangerous = true,
                    MissileSpellName = "ZiggsQSpell3",
                    ExtraMissileNames = new[] {"ZiggsQSpell3"},
                    
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
                    
                    Range = 1000,
                    Radius = 275,
                    MissileSpeed = 1750,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "ZiggsW",
                    DontCross = true,
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });

            Spells.Add(
              new SpellData
              {
                  ChampionName = "Ziggs",
                  SpellName = "ZiggsWtrap",
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
                  IsDangerous = true,
                  SourceObjectName = "Ziggs.+_W_aoe",
                  ToggleParticleName = "Ziggs.+_W_aoe",
                  DontCross = true,
                  CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
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
                    IsDangerous = true,
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
                   IsDangerous = true,
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
                    IsDangerous = true,
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
                    Delay = 700,
                    ExtraDuration = 3050,
                    Range = 900,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DontCross = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    DontAddExtraDuration = true,
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
                    IsDangerous = true,
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
                    CollisionObjects = new[] {CollisionObjectTypes.YasuoWall},
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "ZyraR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 2250,
                    Range = 700,
                    Radius = 500,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    MissileSpellName = "",
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    Radius = 70,
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
                    BehindStart = 200,
                    ExtraRange = 200,
                    IsDangerous = true,
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Pyke",
                    SpellName = "PykessR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    ExtraDuration = 500,
                    Range = 750,
                    Radius = 85,
                    ToggleParticleName = "Pyke.+R_GroundIndicator",
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true
                });

            #endregion Pyke

            #region Hecarim

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Hecarim",
                    SpellName = "HecarimR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 1450,
                    Radius = 50,
                    MissileSpeed = 1100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    DontCheckForDuplicates = true,
                    MissileSpellName = "HecarimUltMissile",
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Hecarim",
                   SpellName = "HecarimRcircle",
                   Slot = SpellSlot.R,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 0,
                   Range = 1000,
                   Radius = 250,
                   MissileSpeed = 1100,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 5,
                   IsDangerous = true,
                   IsDash = true,
                   DontCheckForDuplicates = true,
                   CanDetectDash = (sender, args) =>
                   {
                       return args.Speed == 1100 && Utils.ImmobileTime(sender) == -1;
                   },
               });

            #endregion Warwick
            #region Wrwick

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Warwick",
                    SpellName = "WarwickR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 100,
                    Range = 2000,
                    Radius = 100,
                    ExtraRange = 100,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "",
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    Range = 900,
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
                    Delay = 1000,
                    Range = 900,
                    //ExtraDuration = 4500,
                    DontAddExtraDuration = true,
                    DontCross = true,
                    Radius = 250,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileDelayed = true,
                    MissileSpellName = "ZoeEc",
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
                    MissileDelayed = true,
                    CanBeRemoved = true,
                    FixedRange = true,
                    MissileSpellName = "XayahQMissile1",
                    ExtraMissileNames = new [] { "XayahQMissile2" },
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall }
                });


            Spells.Add(
               new SpellData
               {
                   ChampionName = "Xayah",
                   SpellName = "XayahR",
                   Slot = SpellSlot.R,
                   Type = SkillShotType.SkillshotCone,
                   Delay = 1000,
                   Range = 1200,
                   Radius = 50,
                   MissileSpeed = 2400,
                   AddHitbox = true,
                   DangerValue = 2,
                   IsDangerous = true,
                   MissileDelayed = true,
                   CanBeRemoved = true,
                   FixedRange = true,
                   SourceObjectName = "Xayah_Base_R_Ground_Cas",

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
                    IsDangerous = true,
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
                    Range = 1200,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    IsDangerous = true,
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
                    SpellName = "SeraphineQCast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 600,
                    Range = 900,
                    Radius = 290,
                    MissileSpeed = 1300,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    FixedRange = false,
                    //MissileSpellName = "SeraphineQInitialMissile",
                    SourceObjectName = "Seraphine_Base_Q_AOE_Indicator",
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Seraphine",
                    SpellName = "SeraphineQCastEcho",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 600,
                    ExtraDuration = 200,
                    Range = 900,
                    Radius = 290,
                    MissileSpeed = 1300,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    FixedRange = false,
                    //MissileSpellName = "SeraphineQInitialMissile",
                    SourceObjectName = "Seraphine_Base_Q_AOE_Indicator",
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Seraphine",
                    SpellName = "SeraphineECast",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1350,
                    Radius = 80,
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
                   SpellName = "SeraphineECastEcho",
                   Slot = SpellSlot.E,
                   Type = SkillShotType.SkillshotMissileLine,
                   Delay = 250,
                   Range = 1350,
                   Radius = 80,
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
                    Range = 2500,
                    Radius = 160,
                    MissileSpeed = 1600,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    FixedRange = true,
                    MissileSpellName = "SeraphineR"
                });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Seraphine",
                   SpellName = "SeraphineRfow",
                   Slot = SpellSlot.R,
                   Type = SkillShotType.SkillshotLine,
                   Delay = 500,
                   Range = 1300,
                   Radius = 160,
                   MissileSpeed = int.MaxValue,
                   AddHitbox = true,
                   DangerValue = 5,
                   IsDangerous = true,
                   FixedRange = true,
                   SourceObjectName = "Seraphine.+R_Mis_Warning"
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

            #region Viego

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Viego",
                    SpellName = "ViegoQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 400,
                    Range = 600,
                    Radius = 70,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    FixedRange = true,
                    AddHitbox = true,
                });
            Spells.Add(
              new SpellData
              {
                  ChampionName = "Viego",
                  SpellName = "ViegoWCast",
                  Slot = SpellSlot.W,
                  Type = SkillShotType.SkillshotMissileLine,
                  Delay = 0,
                  Range = 1000,
                  Radius = 70,
                  MissileSpeed = 1500,
                  DangerValue = 3,
                  FixedRange = true,
                  AddHitbox = true,
                  MissileSpellName = "ViegoWMis"
              });
            Spells.Add(
               new SpellData
               {
                   ChampionName = "Viego",
                   SpellName = "ViegoR",
                   Slot = SpellSlot.R,
                   Type = SkillShotType.SkillshotCircle,
                   Delay = 600,
                   Range = 500,
                   Radius = 270,
                   DontCross = true,
                   MissileSpeed = int.MaxValue,
                   FixedRange = false,
                   AddHitbox = true,
                   DangerValue = 5,
                   IsDangerous = true,
                   SourceObjectName = "Viego.+R_Tell"
               });
            #endregion

            #region Rell
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rell",
                    SpellName = "RellQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 350,
                    Range = 700,
                    Radius = 70,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    FixedRange = true,
                    AddHitbox = true,
                    MissileSpellName = "RellQ_VFXMis"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rell",
                    SpellName = "RellR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 400,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                });
            Spells.Add(
              new SpellData
              {
                  ChampionName = "Rell",
                  SpellName = "RellR",
                  Slot = SpellSlot.W,
                  Type = SkillShotType.SkillshotLine,
                  Delay = 450,
                  Range = 900,
                  Radius = 220,
                  MissileSpeed = 5000,
                  FixedRange = true,
                  AddHitbox = true,
                  DangerValue = 3,
                  IsDangerous = true,
                  IsDash = true,
                  CanDetectDash = (sender, args) =>
                  {
                      return Utils.ImmobileTime(sender) == -1;
                  },
              });
            #endregion

            #region MissFortune
            Spells.Add(
               new SpellData
               {
                   ChampionName = "MissFortune",
                   SpellName = "MissFortuneRicochetShot",
                   Slot = SpellSlot.Q,
                   Type = SkillShotType.SkillshotCone,
                   Delay = 250,
                   Range = 600,
                   Radius = 70,
                   DontCross = true,
                   MissileSpeed = 500,
                   DangerValue = 2,
                   FixedRange = false,
                   AddHitbox = true,
                   IsDangerous = true,
                   MissileSpellName = "MissFortuneRicochetShot"
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "MissFortune",
                    SpellName = "MissFortuneScattershot",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1000,
                    ExtraDuration = 2000,
                    Range = 1000,
                    Radius = 250,
                    DontAddExtraDuration = true,
                    DontCross = true,
                    MissileSpeed = int.MaxValue,
                    FixedRange = false,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "",
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "MissFortune",
                    SpellName = "MissFortuneBulletTime",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 1500,
                    ExtraDuration = 1500,
                    DontAddExtraDuration = true,
                    Range = 1500,
                    Radius = 35,
                    BehindStart = 100,
                    DontCross = true,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    FixedRange = false,
                    AddHitbox = true,
                    IsDangerous = true,
                });
           
            #endregion

        }

        public static SpellData GetByDash(string championName)
        {
            foreach (var spellData in Spells)
            {
                if (!spellData.IsDash)
                    continue;

                if (championName == spellData.ChampionName)
                    return spellData;
            }
            return null;
        }

        public static SpellData GetByEndAtParticle(string objectName)
        {
            foreach (var spellData in Spells)
            {
                if (spellData.EndAtParticle.Length == 0)
                    continue;

                var reg = new System.Text.RegularExpressions.Regex(spellData.EndAtParticle, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (reg.IsMatch(objectName))
                    return spellData;
            }
            return null;
        }

        public static SpellData GetBySourceObjectName(string objectName)
        {
            foreach (var spellData in Spells)
            {
                if (spellData.SourceObjectName.Length == 0)
                    continue;

                var reg = new System.Text.RegularExpressions.Regex(spellData.SourceObjectName, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (reg.IsMatch(objectName))
                    return spellData;
            }
            return null;
        }

        public static SpellData GetByName(string spellName)
        {
            spellName = spellName.ToLowerInvariant();
            foreach (var spellData in Spells)
            {
                if (spellData.SpellName.ToLowerInvariant() == spellName || spellData.ExtraSpellNames.Contains(spellName))
                    return spellData;
            }
            return null;
        }

        public static SpellData GetByMissileName(string missileSpellName)
        {
            missileSpellName = missileSpellName.ToLower();
            foreach (var spellData in Spells)
            {
                if (spellData.MissileSpellName != null)
                {
                    if(spellData.MissileSpellName.ToLower() == missileSpellName)
                        return spellData;

                    foreach (var name in spellData.ExtraMissileNames)
                    {
                        if (missileSpellName == name.ToLower())
                            return spellData;
                    }
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
