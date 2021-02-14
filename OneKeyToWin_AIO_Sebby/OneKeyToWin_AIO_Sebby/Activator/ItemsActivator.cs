using System;
using System.Linq;
using LeagueSharp.Common;
using LeagueSharp;
using SebbyLib;
using OneKeyToWin_AIO_Sebby;
using OneKeyToWin_AIO_Sebby.SebbyLib;

class ItemsActivator : Program
{
    public Items.Item

    //Cleans
    Mikaels = new Items.Item(3222, 600f),
    Quicksilver = new Items.Item(3140, 0),
    Mercurial = new Items.Item(3139, 0),
    Dervish = new Items.Item(3137, 0),
    //REGEN
    Potion = new Items.Item(2003, 0),
    ManaPotion = new Items.Item(2004, 0),
    Flask = new Items.Item(2041, 0),
    Biscuit = new Items.Item(2010, 0),
    Refillable = new Items.Item(2031, 0),
    Hunter = new Items.Item(2032, 0),
    Corrupting = new Items.Item(2033, 0),
    //attack
    Botrk = new Items.Item(3153, 550f),
    Cutlass = new Items.Item(3144, 550f),
    Youmuus = new Items.Item(3142, 650f),
    Hydra = new Items.Item(3074, 440f),
    Hydra2 = new Items.Item(3077, 440f),
    HydraTitanic = new Items.Item(3748, 150f),
    Hextech = new Items.Item(3146, 700f),
    FrostQueen = new Items.Item(3092, 850f),
    Protobelt = new Items.Item(3152, 850f),
    GLP800 = new Items.Item(3030, 800f),

    //def
    FaceOfTheMountain = new Items.Item(3401, 600f),
    Zhonya = new Items.Item(3157, 0),
    Seraph = new Items.Item(3040, 0),
    Solari = new Items.Item(3190, 600f),
    Randuin = new Items.Item(3143, 400f);

    public ItemsActivator()
    {
        var activatorMenu = MainMenu.SubMenu("Activator OKTW©");

        activatorMenu.AddItem(new MenuItem("pots", "Potion, Flask, Biscuit").SetValue(true));

        activatorMenu.SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("Botrk", "Botrk").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkKS", "Botrk KS").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkLS", "Botrk LifeSaver").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkCombo", "Botrk always in combo").SetValue(false));

        activatorMenu.SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("Cutlass", "Cutlass").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("CutlassKS", "Cutlass KS").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("CutlassCombo", "Cutlass always in combo").SetValue(true));

        activatorMenu.SubMenu("Offensives").SubMenu("Hextech").AddItem(new MenuItem("Hextech", "Hextech").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Hextech").AddItem(new MenuItem("HextechKS", "Hextech KS").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Hextech").AddItem(new MenuItem("HextechCombo", "Hextech always in combo").SetValue(true));

        activatorMenu.SubMenu("Offensives").SubMenu("Protobelt").AddItem(new MenuItem("Protobelt", "Protobelt").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Protobelt").AddItem(new MenuItem("ProtobeltKS", "Protobelt KS").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Protobelt").AddItem(new MenuItem("ProtobeltCombo", "Protobelt always in combo").SetValue(true));

        activatorMenu.SubMenu("Offensives").SubMenu("GLP800").AddItem(new MenuItem("GLP800", "GLP800").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("GLP800").AddItem(new MenuItem("GLP800KS", "GLP800 KS").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("GLP800").AddItem(new MenuItem("GLP800Combo", "GLP800 always in combo").SetValue(true));

        activatorMenu.SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("Youmuus", "Youmuus").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusR", "TwitchR, AsheQ").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusKS", "Youmuus KS").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusCombo", "Youmuus always in combo").SetValue(false));

        activatorMenu.SubMenu("Offensives").SubMenu("Hydra").AddItem(new MenuItem("Hydra", "Hydra").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("HydraTitanic").AddItem(new MenuItem("HydraTitanic", "Hydra Titanic").SetValue(true));
        activatorMenu.SubMenu("Offensives").SubMenu("FrostQueen").AddItem(new MenuItem("FrostQueen", "FrostQueen").SetValue(true));

        // DEF
        activatorMenu.SubMenu("Defensives").AddItem(new MenuItem("Randuin", "Randuin").SetValue(true));
        activatorMenu.SubMenu("Defensives").AddItem(new MenuItem("FaceOfTheMountain", "FaceOfTheMountain").SetValue(true));
        activatorMenu.SubMenu("Defensives").SubMenu("Zhonya").AddItem(new MenuItem("Zhonya", "Zhonya").SetValue(true));

        foreach (var enemy in HeroManager.Enemies)
        {
            var spell = enemy.Spellbook.Spells[3];
            activatorMenu.SubMenu("Defensives").SubMenu("Zhonya").AddItem(new MenuItem("spellZ" + spell.SData.Name, enemy.ChampionName + ": " + spell.Name).SetValue(spell.SData.TargettingType == SpellDataTargetType.Unit));
        }

        activatorMenu.SubMenu("Defensives").AddItem(new MenuItem("Seraph", "Seraph").SetValue(true));
        activatorMenu.SubMenu("Defensives").AddItem(new MenuItem("Solari", "Solari").SetValue(true));
        // CLEANSERS 

        activatorMenu.SubMenu("Cleansers").AddItem(new MenuItem("Clean", "Quicksilver, Mikaels, Mercurial, Dervish").SetValue(true));

        foreach (var ally in HeroManager.Allies)
            activatorMenu.SubMenu("Cleansers").SubMenu("Mikaels allys").AddItem(new MenuItem("MikaelsAlly" + ally.ChampionName, ally.ChampionName).SetValue(true));

        activatorMenu.SubMenu("Cleansers").AddItem(new MenuItem("CSSdelay", "Delay x ms").SetValue(new Slider(0, 1000, 0)));
        activatorMenu.SubMenu("Cleansers").AddItem(new MenuItem("cleanHP", "Use only under % HP").SetValue(new Slider(80, 100, 0)));
        //Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("CleanSpells", "ZedR FizzR MordekaiserR PoppyR VladimirR").SetValue(true));
        activatorMenu.SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Stun", "Stun").SetValue(true));
        activatorMenu.SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Snare", "Snare").SetValue(true));
        activatorMenu.SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Charm", "Charm").SetValue(true));
        activatorMenu.SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Fear", "Fear").SetValue(true));
        activatorMenu.SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Suppression", "Suppression").SetValue(true));
        activatorMenu.SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Taunt", "Taunt").SetValue(true));
        activatorMenu.SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Blind", "Blind").SetValue(true));
        Game.OnUpdate += Game_OnGameUpdate;
        Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        //Drawing.OnDraw += Drawing_OnDraw;
    }

    private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
    {
        if (target is Obj_AI_Hero && MainMenu.Item("HydraTitanic").GetValue<bool>() && HydraTitanic.IsReady() && target.IsValid<Obj_AI_Hero>())
        {
            HydraTitanic.Cast();
        }
    }

    private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
    {
        if (!(sender is Obj_AI_Hero) || !sender.IsEnemy)
            return;

        if (sender.Distance(Player.Position) > 1600)
            return;

        if (Zhonya.IsReady() && MainMenu.Item("Zhonya").GetValue<bool>())
        {
            if (MainMenu.Item("spellZ" + args.SData.Name) != null && MainMenu.Item("spellZ" + args.SData.Name).GetValue<bool>())
            {
                if (args.Target != null)
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                        ZhonyaTryCast();
                }
                else
                {

                    if (OktwCommon.CanHitSkillShot(Player, args.Start, args.End, args.SData))
                        ZhonyaTryCast();
                }
            }
        }
    }

    private void ZhonyaTryCast()
    {
        if (Player.HasBuffOfType(BuffType.PhysicalImmunity) || Player.HasBuffOfType(BuffType.SpellImmunity) || (!Player.Spellbook.Spells[3].IsReady() && Player.ChampionName == "Kayle")
           || Player.IsZombie || Player.IsInvulnerable || Player.HasBuffOfType(BuffType.Invulnerability) || Player.HasBuff("kindredrnodeathbuff")
           || Player.HasBuffOfType(BuffType.SpellShield) || Player.AllShield > OktwCommon.GetIncomingDamage(Player))
        {

        }
        else
        {
            Zhonya.Cast();
        }
    }

    private void Survival()
    {
        if (Player.HealthPercent < 60 && (Seraph.IsReady() || Zhonya.IsReady()))
        {
            double dmg = OktwCommon.GetIncomingDamage(Player, 1);
            var enemys = Player.CountEnemiesInRange(800);
            if (dmg > 0 || enemys > 0)
            {
                if (Seraph.IsReady() && MainMenu.Item("Seraph").GetValue<bool>())
                {
                    var value = Player.Mana * 0.2 + 150;
                    if (dmg > value && Player.HealthPercent < 50)
                        Seraph.Cast();
                    else if (Player.Health - dmg < enemys * Player.Level * 20)
                        Seraph.Cast();
                    else if (Player.Health - dmg < Player.Level * 10)
                        Seraph.Cast();
                }

                if (Zhonya.IsReady() && MainMenu.Item("Zhonya").GetValue<bool>())
                {
                    if (dmg > Player.Level * 35)
                    {
                        ZhonyaTryCast();
                    }
                    else if (Player.Health - dmg < enemys * Player.Level * 20)
                    {
                        ZhonyaTryCast();

                    }
                    else if (Player.Health - dmg < Player.Level * 10)
                    {
                        ZhonyaTryCast();

                    }
                }
            }
        }


        if (!Solari.IsReady() && !FaceOfTheMountain.IsReady() )
            return;

        foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 50 && Player.Distance(ally.ServerPosition) < 700))
        {
            double dmg = OktwCommon.GetIncomingDamage(ally);

            if (dmg == 0)
                continue;

            if (MainMenu.Item("Solari").GetValue<bool>() && Solari.IsReady() && Player.Distance(ally.ServerPosition) < Solari.Range)
            {
                var value = 75 + (15 * Player.Level);
                if (dmg > value && Player.HealthPercent < 50)
                    Solari.Cast();
                else if (ally.Health - dmg < ally.Level * 10)
                    Solari.Cast();
                else if (ally.Health - dmg < ally.Level * 10)
                    Solari.Cast();
            }

            if (MainMenu.Item("FaceOfTheMountain").GetValue<bool>() && FaceOfTheMountain.IsReady() && Player.Distance(ally.ServerPosition) < FaceOfTheMountain.Range)
            {
                var value = 0.1 * Player.MaxHealth;
                if (dmg > value && Player.HealthPercent < 50)
                    FaceOfTheMountain.Cast(ally);
                else if (ally.Health - dmg < ally.Level * 10)
                    FaceOfTheMountain.Cast(ally);
                else if (ally.Health - dmg < ally.Level * 10)
                    FaceOfTheMountain.Cast(ally);
            }
        }
    }


    private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
    {
        if (!Youmuus.IsReady() || !MainMenu.Item("YoumuusR").GetValue<bool>())
            return;
        if (args.Slot == SpellSlot.R && (Player.ChampionName == "Twitch"))
        {
            Youmuus.Cast();
        }
        if (args.Slot == SpellSlot.Q && (Player.ChampionName == "Ashe"))
        {
            Youmuus.Cast();
        }
    }

    private void Game_OnGameUpdate(EventArgs args)
    {
        if (Player.InFountain() || Player.IsRecalling() || Player.IsDead)
        {
            return;
        }

        Cleansers();
        Survival();

        if (!Program.LagFree(0))
            return;

        if (MainMenu.Item("pots").GetValue<bool>())
            PotionManagement();


        Offensive();
        Defensive();
        ZhonyaCast();
    }


    private void ZhonyaCast()
    {
        if (MainMenu.Item("Zhonya").GetValue<bool>() && Zhonya.IsReady())
        {
            float time = 10;
            if (Player.HasBuff("zedrdeathmark"))
            {
                time = OktwCommon.GetPassiveTime(Player, "zedulttargetmark");
            }
            if (Player.HasBuff("FizzMarinerDoom"))
            {
                time = OktwCommon.GetPassiveTime(Player, "FizzMarinerDoom");
            }
            if (Player.HasBuff("MordekaiserChildrenOfTheGrave"))
            {
                time = OktwCommon.GetPassiveTime(Player, "MordekaiserChildrenOfTheGrave");
            }
            if (Player.HasBuff("VladimirHemoplague"))
            {
                time = OktwCommon.GetPassiveTime(Player, "VladimirHemoplague");
            }
            if (time < 1 && time > 0)
                ZhonyaTryCast();
        }
    }

    private void Cleansers()
    {
        if (!Quicksilver.IsReady() && !Mikaels.IsReady() && !Mercurial.IsReady() && !Dervish.IsReady())
            return;

        if (Player.HealthPercent >= (float)MainMenu.Item("cleanHP").GetValue<Slider>().Value || !MainMenu.Item("Clean").GetValue<bool>())
            return;

        if (Mikaels.IsReady())
        {
            foreach (var ally in HeroManager.Allies.Where(
                ally => ally.IsValid && !ally.IsDead && MainMenu.Item("MikaelsAlly" + ally.ChampionName).GetValue<bool>() && Player.Distance(ally.Position) < Mikaels.Range
                && ally.HealthPercent < (float)MainMenu.Item("cleanHP").GetValue<Slider>().Value))
            {
                if (ally.HasBuff("zedrdeathmark") || ally.HasBuff("FizzMarinerDoom") || ally.HasBuff("MordekaiserChildrenOfTheGrave") || ally.HasBuff("PoppyDiplomaticImmunity") || ally.HasBuff("VladimirHemoplague"))
                    Mikaels.Cast(ally);
                if (ally.HasBuffOfType(BuffType.Stun) && MainMenu.Item("Stun").GetValue<bool>())
                    Mikaels.Cast(ally);
                if (ally.HasBuffOfType(BuffType.Snare) && MainMenu.Item("Snare").GetValue<bool>())
                    Mikaels.Cast(ally);
                if (ally.HasBuffOfType(BuffType.Charm) && MainMenu.Item("Charm").GetValue<bool>())
                    Mikaels.Cast(ally);
                if (ally.HasBuffOfType(BuffType.Fear) && MainMenu.Item("Fear").GetValue<bool>())
                    Mikaels.Cast(ally);
                if (ally.HasBuffOfType(BuffType.Stun) && MainMenu.Item("Stun").GetValue<bool>())
                    Mikaels.Cast(ally);
                if (ally.HasBuffOfType(BuffType.Taunt) && MainMenu.Item("Taunt").GetValue<bool>())
                    Mikaels.Cast(ally);
                if (ally.HasBuffOfType(BuffType.Suppression) && MainMenu.Item("Suppression").GetValue<bool>())
                    Mikaels.Cast(ally);
                if (ally.HasBuffOfType(BuffType.Blind) && MainMenu.Item("Blind").GetValue<bool>())
                    Mikaels.Cast(ally);
            }
        }

        if (Player.HasBuffOfType(BuffType.Stun) && MainMenu.Item("Stun").GetValue<bool>())
            Clean();
        if (Player.HasBuffOfType(BuffType.Snare) && MainMenu.Item("Snare").GetValue<bool>())
            Clean();
        if (Player.HasBuffOfType(BuffType.Charm) && MainMenu.Item("Charm").GetValue<bool>())
            Clean();
        if (Player.HasBuffOfType(BuffType.Fear) && MainMenu.Item("Fear").GetValue<bool>())
            Clean();
        if (Player.HasBuffOfType(BuffType.Stun) && MainMenu.Item("Stun").GetValue<bool>())
            Clean();
        if (Player.HasBuffOfType(BuffType.Taunt) && MainMenu.Item("Taunt").GetValue<bool>())
            Clean();
        if (Player.HasBuffOfType(BuffType.Suppression) && MainMenu.Item("Suppression").GetValue<bool>())
            Clean();
        if (Player.HasBuffOfType(BuffType.Blind) && MainMenu.Item("Blind").GetValue<bool>())
            Clean();
    }

    private void Clean()
    {
        if (Quicksilver.IsReady())
            Utility.DelayAction.Add(MainMenu.Item("CSSdelay").GetValue<Slider>().Value, () => Quicksilver.Cast());
        else if (Mercurial.IsReady())
            Utility.DelayAction.Add(MainMenu.Item("CSSdelay").GetValue<Slider>().Value, () => Mercurial.Cast());
        else if (Dervish.IsReady())
            Utility.DelayAction.Add(MainMenu.Item("CSSdelay").GetValue<Slider>().Value, () => Dervish.Cast());
    }

    private void Defensive()
    {
        if (Randuin.IsReady() && MainMenu.Item("Randuin").GetValue<bool>() && Player.CountEnemiesInRange(Randuin.Range) > 0)
        {
            Randuin.Cast();
        }
    }

    private void Offensive()
    {
        if (Botrk.IsReady() && MainMenu.Item("Botrk").GetValue<bool>())
        {
            var t = TargetSelector.GetTarget(Botrk.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (MainMenu.Item("BotrkKS").GetValue<bool>() && Player.CalcDamage(t, Damage.DamageType.Physical, t.MaxHealth * 0.1) > t.Health - OktwCommon.GetIncomingDamage(t))
                    Botrk.Cast(t);
                if (MainMenu.Item("BotrkLS").GetValue<bool>() && Player.Health < Player.MaxHealth * 0.5 - OktwCommon.GetIncomingDamage(Player))
                    Botrk.Cast(t);
                if (MainMenu.Item("BotrkCombo").GetValue<bool>() && Program.Combo)
                    Botrk.Cast(t);
            }
        }

        if (GLP800.IsReady() && MainMenu.Item("GLP800").GetValue<bool>())
        {
            var t = TargetSelector.GetTarget(GLP800.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (MainMenu.Item("GLP800KS").GetValue<bool>() && Player.CalcDamage(t, Damage.DamageType.Magical, 200 + Player.FlatMagicDamageMod * 0.35) > t.Health - OktwCommon.GetIncomingDamage(t))
                    GLP800.Cast(Prediction.GetPrediction(t, 0.5f).CastPosition);
                if (MainMenu.Item("GLP800Combo").GetValue<bool>() && Program.Combo)
                {
                    Program.debug("PRO");
                    GLP800.Cast(Prediction.GetPrediction(t, 0.5f).CastPosition);
                }
            }
        }

        if (Protobelt.IsReady() && MainMenu.Item("Protobelt").GetValue<bool>())
        {
            var t = TargetSelector.GetTarget(Protobelt.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (MainMenu.Item("ProtobeltKS").GetValue<bool>() && Player.CalcDamage(t, Damage.DamageType.Magical, 150 + Player.FlatMagicDamageMod * 0.35) > t.Health - OktwCommon.GetIncomingDamage(t))
                    Protobelt.Cast(Prediction.GetPrediction(t, 0.5f).CastPosition);
                if (MainMenu.Item("ProtobeltCombo").GetValue<bool>() && Program.Combo)
                {
                    Program.debug("PRO");
                    Protobelt.Cast(Prediction.GetPrediction(t, 0.5f).CastPosition);
                }
            }
        }

        if (Hextech.IsReady() && MainMenu.Item("Hextech").GetValue<bool>())
        {
            var t = TargetSelector.GetTarget(Hextech.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (MainMenu.Item("HextechKS").GetValue<bool>() && Player.CalcDamage(t, Damage.DamageType.Magical, 150 + Player.FlatMagicDamageMod * 0.4) > t.Health - OktwCommon.GetIncomingDamage(t))
                    Hextech.Cast(t);
                if (MainMenu.Item("HextechCombo").GetValue<bool>() && Program.Combo)
                    Hextech.Cast(t);
            }
        }

        if (Program.Combo && FrostQueen.IsReady() && MainMenu.Item("FrostQueen").GetValue<bool>() && Player.CountEnemiesInRange(800) > 0)
        {
            FrostQueen.Cast();
        }

        if (Cutlass.IsReady() && MainMenu.Item("Cutlass").GetValue<bool>())
        {
            var t = TargetSelector.GetTarget(Cutlass.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (MainMenu.Item("CutlassKS").GetValue<bool>() && Player.CalcDamage(t, Damage.DamageType.Magical, 100) > t.Health - OktwCommon.GetIncomingDamage(t))
                    Cutlass.Cast(t);
                if (MainMenu.Item("CutlassCombo").GetValue<bool>() && Program.Combo)
                    Cutlass.Cast(t);
            }
        }

        if (Youmuus.IsReady() && MainMenu.Item("Youmuus").GetValue<bool>() && Program.Combo)
        {
            var t = Orbwalker.GetTarget();

            if (t.IsValidTarget() && t is Obj_AI_Hero)
            {
                if (MainMenu.Item("YoumuusKS").GetValue<bool>() && t.Health < Player.MaxHealth)
                    Youmuus.Cast();
                if (MainMenu.Item("YoumuusCombo").GetValue<bool>())
                    Youmuus.Cast();
            }
        }

        if (MainMenu.Item("Hydra").GetValue<bool>())
        {
            if (Hydra.IsReady() && Player.CountEnemiesInRange(Hydra.Range) > 0)
                Hydra.Cast();
            else if (Hydra2.IsReady() && Player.CountEnemiesInRange(Hydra2.Range) > 0)
                Hydra2.Cast();
        }
    }

    private void PotionManagement()
    {
        if (Player.Health - OktwCommon.GetIncomingDamage(Player) + 250 > Player.MaxHealth)
            return;

        if (Player.HealthPercent > 50 && Player.CountEnemiesInRange(700) == 0)
            return;

        if (Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemMiniRegenPotion") || Player.HasBuff("ItemCrystalFlaskJungle") || Player.HasBuff("ItemDarkCrystalFlask") || Player.HasBuff("ItemCrystalFlask"))
            return;

        if (Potion.IsReady())
            Potion.Cast();
        else if (Biscuit.IsReady())
            Biscuit.Cast();
        else if (Hunter.IsReady())
            Hunter.Cast();
        else if (Corrupting.IsReady())
            Corrupting.Cast();
        else if (Refillable.IsReady())
            Refillable.Cast();
    }

    private bool CanUse(SpellSlot sum)
    {
        if (sum != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(sum) == SpellState.Ready)
            return true;
        else
            return false;
    }
}

