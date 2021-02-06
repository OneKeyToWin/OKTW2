using System;
using System.Linq;
using LeagueSharp.Common;
using LeagueSharp;
using SebbyLib;
using OneKeyToWin_AIO_Sebby;

class Summoners : Program
{
    private SpellSlot heal, barrier, ignite, exhaust, flash, smite, teleport, cleanse;

    public Summoners()
    {
        teleport = Player.GetSpellSlot("SummonerTeleport");
        heal = Player.GetSpellSlot("summonerheal");
        barrier = Player.GetSpellSlot("summonerbarrier");
        ignite = Player.GetSpellSlot("summonerdot");
        exhaust = Player.GetSpellSlot("summonerexhaust");
        flash = Player.GetSpellSlot("summonerflash");
        cleanse = Player.GetSpellSlot("SummonerBoost");
        smite = Player.GetSpellSlot("summonersmite");

        if (smite == SpellSlot.Unknown) { smite = Player.GetSpellSlot("itemsmiteaoe"); }
        if (smite == SpellSlot.Unknown) { smite = Player.GetSpellSlot("s5_summonersmiteplayerganker"); }
        if (smite == SpellSlot.Unknown) { smite = Player.GetSpellSlot("s5_summonersmitequick"); }
        if (smite == SpellSlot.Unknown) { smite = Player.GetSpellSlot("s5_summonersmiteduel"); }

        if (smite != SpellSlot.Unknown)
        {
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("SmiteEnemy", "Auto Smite enemy under 50% hp").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("SmiteEnemyKS", "Auto Smite enemy KS").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Smite", "Auto Smite mobs OKTW").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rdragon", "Dragon", true).SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rbaron", "Baron", true).SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rherald", "Herald", true).SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rred", "Red", true).SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rblue", "Blue", true).SetValue(true));
            Config.Item("Smite").Permashow(true);
        }

        if (flash != SpellSlot.Unknown)
        {
            //Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Flash").AddItem(new MenuItem("Flash", "Flash max range").SetValue(true));

        }
        if (exhaust != SpellSlot.Unknown)
        {
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Exhaust").AddItem(new MenuItem("Exhaust", "Exhaust").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Exhaust").AddItem(new MenuItem("Exhaust1", "Exhaust if Channeling Important Spell ").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Exhaust").AddItem(new MenuItem("Exhaust2", "Always in combo").SetValue(false));
        }
        if (heal != SpellSlot.Unknown)
        {
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Heal").AddItem(new MenuItem("Heal", "Heal").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Heal").AddItem(new MenuItem("AllyHeal", "Ally Heal").SetValue(true));
        }
        if (barrier != SpellSlot.Unknown)
        {
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").AddItem(new MenuItem("Barrier", "Barrier").SetValue(true));

        }
        if (ignite != SpellSlot.Unknown)
        {
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").AddItem(new MenuItem("Ignite", "Ignite").SetValue(true));
        }
        if (cleanse != SpellSlot.Unknown)
        {
            Config.SubMenu("Activator OKTW©").SubMenu("Summoners").AddItem(new MenuItem("Cleanse", "Cleanse").SetValue(true));
        }

        Config.SubMenu("Activator OKTW©").AddItem(new MenuItem("pots", "Potion, Flask, Biscuit").SetValue(true));

        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("Botrk", "Botrk").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkKS", "Botrk KS").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkLS", "Botrk LifeSaver").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkCombo", "Botrk always in combo").SetValue(false));

        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("Cutlass", "Cutlass").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("CutlassKS", "Cutlass KS").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("CutlassCombo", "Cutlass always in combo").SetValue(true));

        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Hextech").AddItem(new MenuItem("Hextech", "Hextech").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Hextech").AddItem(new MenuItem("HextechKS", "Hextech KS").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Hextech").AddItem(new MenuItem("HextechCombo", "Hextech always in combo").SetValue(true));

        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Protobelt").AddItem(new MenuItem("Protobelt", "Protobelt").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Protobelt").AddItem(new MenuItem("ProtobeltKS", "Protobelt KS").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Protobelt").AddItem(new MenuItem("ProtobeltCombo", "Protobelt always in combo").SetValue(true));

        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("GLP800").AddItem(new MenuItem("GLP800", "GLP800").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("GLP800").AddItem(new MenuItem("GLP800KS", "GLP800 KS").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("GLP800").AddItem(new MenuItem("GLP800Combo", "GLP800 always in combo").SetValue(true));

        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("Youmuus", "Youmuus").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusR", "TwitchR, AsheQ").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusKS", "Youmuus KS").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusCombo", "Youmuus always in combo").SetValue(false));

        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Hydra").AddItem(new MenuItem("Hydra", "Hydra").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("HydraTitanic").AddItem(new MenuItem("HydraTitanic", "Hydra Titanic").SetValue(true));

        Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("FrostQueen").AddItem(new MenuItem("FrostQueen", "FrostQueen").SetValue(true));

        // DEF
        Config.SubMenu("Activator OKTW©").SubMenu("Defensives").AddItem(new MenuItem("Randuin", "Randuin").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Defensives").AddItem(new MenuItem("FaceOfTheMountain", "FaceOfTheMountain").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Defensives").SubMenu("Zhonya").AddItem(new MenuItem("Zhonya", "Zhonya").SetValue(true));

        foreach (var enemy in HeroManager.Enemies)
        {
            var spell = enemy.Spellbook.Spells[3];
            Config.SubMenu("Activator OKTW©").SubMenu("Defensives").SubMenu("Zhonya").AddItem(new MenuItem("spellZ" + spell.SData.Name, enemy.ChampionName + ": " + spell.Name).SetValue(spell.SData.TargettingType == SpellDataTargetType.Unit));
        }

        Config.SubMenu("Activator OKTW©").SubMenu("Defensives").AddItem(new MenuItem("Seraph", "Seraph").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Defensives").AddItem(new MenuItem("Solari", "Solari").SetValue(true));
        // CLEANSERS 

        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").AddItem(new MenuItem("Clean", "Quicksilver, Mikaels, Mercurial, Dervish").SetValue(true));

        foreach (var ally in HeroManager.Allies)
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Mikaels allys").AddItem(new MenuItem("MikaelsAlly" + ally.ChampionName, ally.ChampionName).SetValue(true));

        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").AddItem(new MenuItem("CSSdelay", "Delay x ms").SetValue(new Slider(0, 1000, 0)));
        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").AddItem(new MenuItem("cleanHP", "Use only under % HP").SetValue(new Slider(80, 100, 0)));
        //Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("CleanSpells", "ZedR FizzR MordekaiserR PoppyR VladimirR").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Stun", "Stun").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Snare", "Snare").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Charm", "Charm").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Fear", "Fear").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Suppression", "Suppression").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Taunt", "Taunt").SetValue(true));
        Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Blind", "Blind").SetValue(true));
        Game.OnUpdate += Game_OnGameUpdate;
        Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
    }

    private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
    {
        if (!(sender is Obj_AI_Hero) || !sender.IsEnemy)
            return;

        if (sender.Distance(Player.Position) > 1600)
            return;

        if (CanUse(exhaust) && Config.Item("Exhaust").GetValue<bool>())
        {
            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 51 && Player.Distance(ally.ServerPosition) < 700))
            {
                double dmg = 0;
                if (args.Target != null)
                {
                    if (args.Target.NetworkId == ally.NetworkId)
                        dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                }
                else
                {
                    if (OktwCommon.CanHitSkillShot(ally, args.Start, args.End, args.SData))
                        dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                    else
                        continue;
                }

                if (ally.Health - dmg < ally.CountEnemiesInRange(700) * ally.Level * 40)
                    Player.Spellbook.CastSpell(exhaust, sender);
            }
        }
    }

    private void Survival()
    {
        if (!CanUse(heal))
            return;

        foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 50 && Player.Distance(ally.ServerPosition) < 700))
        {
            double dmg = OktwCommon.GetIncomingDamage(ally);

            if (dmg == 0)
                continue;

            if (CanUse(heal) && Config.Item("Heal").GetValue<bool>())
            {
                if (!Config.Item("AllyHeal").GetValue<bool>() && !ally.IsMe)
                    return;

                if (ally.Health - dmg < ally.Level * 10)
                    Player.Spellbook.CastSpell(heal, ally);
                else if (ally.Health - dmg < ally.Level * 10)
                    Player.Spellbook.CastSpell(heal, ally);
            }

        }
    }

    private void Game_OnGameUpdate(EventArgs args)
    {
        if (Player.InFountain() || Player.IsRecalling() || Player.IsDead)
            return;

        Smite();
        Survival();

        if (!Program.LagFree(0))
            return;

        Ignite();
        Exhaust();
    }

    private void Smite()
    {
        if (CanUse(smite))
        {
            var mobs = Cache.GetMinions(Player.ServerPosition, 520, MinionTeam.Neutral);
            if (mobs.Count == 0 && (Player.GetSpellSlot("s5_summonersmiteplayerganker") != SpellSlot.Unknown || Player.GetSpellSlot("s5_summonersmiteduel") != SpellSlot.Unknown))
            {
                var enemy = TargetSelector.GetTarget(500, TargetSelector.DamageType.True);
                if (enemy.IsValidTarget())
                {
                    if (enemy.HealthPercent < 50 && Config.Item("SmiteEnemy").GetValue<bool>())
                        Player.Spellbook.CastSpell(smite, enemy);

                    var smiteDmg = Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Smite);

                    if (Config.Item("SmiteEnemyKS").GetValue<bool>() && enemy.Health - OktwCommon.GetIncomingDamage(enemy) < smiteDmg)
                        Player.Spellbook.CastSpell(smite, enemy);
                }
            }
            if (mobs.Count > 0 && Config.Item("Smite").GetValue<KeyBind>().Active)
            {
                foreach (var mob in mobs)
                {
                    if (((mob.BaseSkinName.ToLower().Contains("dragon") && Config.Item("Rdragon", true).GetValue<bool>())
                        || (mob.BaseSkinName == "SRU_Baron" && Config.Item("Rbaron", true).GetValue<bool>())
                        || (mob.BaseSkinName == "SRU_RiftHerald" && Config.Item("Rherald", true).GetValue<bool>())
                        || (mob.BaseSkinName == "SRU_Red" && Config.Item("Rred", true).GetValue<bool>())
                        || (mob.BaseSkinName == "SRU_Blue" && Config.Item("Rblue", true).GetValue<bool>()))
                        && mob.Health <= Player.GetSummonerSpellDamage(mob, Damage.SummonerSpell.Smite))
                    {
                        Player.Spellbook.CastSpell(smite, mob);
                    }
                }
            }
        }
    }

    private void Exhaust()
    {
        if (CanUse(exhaust) && Config.Item("Exhaust").GetValue<bool>())
        {
            if (Config.Item("Exhaust1").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(650) && enemy.IsChannelingImportantSpell()))
                {
                    Player.Spellbook.CastSpell(exhaust, enemy);
                }
            }

            if (Config.Item("Exhaust2").GetValue<bool>() && Program.Combo)
            {
                var t = TargetSelector.GetTarget(650, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget())
                {
                    Player.Spellbook.CastSpell(exhaust, t);
                }
            }
        }
    }

    private void Ignite()
    {
        if (CanUse(ignite) && Config.Item("Ignite").GetValue<bool>())
        {
            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(600)))
            {

                var pred = enemy.Health - OktwCommon.GetIncomingDamage(enemy);

                var IgnDmg = Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

                if (pred <= 2 * IgnDmg && OktwCommon.ValidUlt(enemy))
                {
                    if (pred <= IgnDmg && enemy.CountAlliesInRange(450) < 2)
                    {
                        var enemyPred = Prediction.GetPrediction(enemy, 0.1f).CastPosition;
                        if (Player.ServerPosition.Distance(enemyPred) > 500 || NavMesh.IsWallOfGrass(enemyPred, 0))
                            Player.Spellbook.CastSpell(ignite, enemy);
                    }

                    //if (enemy.PercentLifeStealMod > 10)
                    //    Player.Spellbook.CastSpell(ignite, enemy);

                    if (enemy.HasBuff("RegenerationPotion") || enemy.HasBuff("ItemMiniRegenPotion") || enemy.HasBuff("ItemCrystalFlask"))
                        Player.Spellbook.CastSpell(ignite, enemy);

                    if (enemy.Health > Player.Health)
                        Player.Spellbook.CastSpell(ignite, enemy);
                }
            }
        }
    }


    private bool CanUse(SpellSlot sum)
    {
        if (sum != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(sum) == SpellState.Ready)
            return true;
        else
            return false;
    }
}

