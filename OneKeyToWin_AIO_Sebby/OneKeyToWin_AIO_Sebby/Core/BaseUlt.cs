using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneKeyToWin_AIO_Sebby;
using SebbyLib;
using static LeagueSharp.Common.Packet;

class BaseUlt : Program
{
    Menu Menu;
    Menu TeamUlt;
    Menu DisabledChampions;

    Spell Ultimate;
    int LastUltCastT;

    List<Obj_AI_Hero> Heroes;
    List<Obj_AI_Hero> Enemies;
    List<Obj_AI_Hero> Allies;

    bool compatibleChamp;

    struct UltSpellDataS
    {
        public int SpellStage;
        public float DamageMultiplicator;
        public float Width;
        public float Delay;
        public float Speed;
        public bool Collision;
    }

    Dictionary<String, UltSpellDataS> UltSpellData = new Dictionary<string, UltSpellDataS>
        {
            {"Jinx",    new UltSpellDataS { SpellStage = 1, DamageMultiplicator = 1.0f, Width = 140f, Delay = 0600f/1000f, Speed = 1700f, Collision = true}},
            {"Ashe",    new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 1.0f, Width = 130f, Delay = 0250f/1000f, Speed = 1600f, Collision = true}},
            {"Draven",  new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 0.7f, Width = 160f, Delay = 0400f/1000f, Speed = 2000f, Collision = true}},
            {"Ezreal",  new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 0.7f, Width = 160f, Delay = 1000f/1000f, Speed = 2000f, Collision = false}},
            {"Karthus", new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 1.0f, Width = 000f, Delay = 3125f/1000f, Speed = 0000f, Collision = false}}
        };

    public BaseUlt()
    {
        compatibleChamp = IsCompatibleChamp(ObjectManager.Player.ChampionName);

        if (!compatibleChamp)
            return;

        Ultimate = new Spell(SpellSlot.R);

        Menu = MainMenu.AddSubMenu(new Menu("Base Ult", "Base Ult")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.Yellow);
        Menu.AddItem(new MenuItem("baseUlt", "Base Ult").SetValue(true));
        Menu.AddItem(new MenuItem("panicKey", "No Ult while SBTW").SetValue(new KeyBind(32, KeyBindType.Press))); //32 == space
        Menu.AddItem(new MenuItem("regardlessKey", "No timelimit (hold)").SetValue(new KeyBind(17, KeyBindType.Press))); //17 == ctrl

        Heroes = ObjectManager.Get<Obj_AI_Hero>().ToList();
        Enemies = Heroes.Where(x => x.IsEnemy).ToList();
        Allies = Heroes.Where(x => x.IsAlly).ToList();

        if (compatibleChamp)
        {
            TeamUlt = Menu.AddSubMenu(new Menu("Team Baseult Friends", "TeamUlt"));

            foreach (Obj_AI_Hero champ in Allies.Where(x => !x.IsMe && IsCompatibleChamp(x.ChampionName)))
                TeamUlt.AddItem(new MenuItem(champ.ChampionName, "Ally with baseult: " + champ.ChampionName).SetValue(false).DontSave());

            DisabledChampions = Menu.AddSubMenu(new Menu("Disabled Champion targets", "DisabledChampions"));

            foreach (Obj_AI_Hero champ in Enemies)
                DisabledChampions.AddItem(new MenuItem(champ.ChampionName, "Don't shoot: " + champ.ChampionName).SetValue(false).DontSave());
        }

        Game.OnUpdate += Game_OnGameUpdate;
    }

    bool CanUseUlt(Obj_AI_Hero hero) //use for allies when fixed: champ.Spellbook.GetSpell(SpellSlot.R) = Ready
    {
        return hero.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready ||
            (hero.Spellbook.GetSpell(SpellSlot.R).Level > 0 && hero.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Surpressed && hero.Mana >= hero.Spellbook.GetSpell(SpellSlot.R).ManaCost);
    }

    public bool IsCompatibleChamp(String championName)
    {
        return UltSpellData.Keys.Any(x => x == championName);
    }

    bool IsCollidingWithChamps(Obj_AI_Hero source, SharpDX.Vector3 targetpos, float width)
    {
        var input = new PredictionInput
        {
            Radius = width,
            Unit = source,
        };

        input.CollisionObjects[0] = CollisionableObjects.Heroes;

        return LeagueSharp.Common.Collision.GetCollision(new List<SharpDX.Vector3> { targetpos }, input).Any(); //x => x.NetworkId != targetnetid, hard to realize with teamult
    }

    float GetUltTravelTime(Obj_AI_Hero source, float speed, float delay, SharpDX.Vector3 targetpos)
    {
        if (source.ChampionName == "Karthus")
            return delay;

        float distance = SharpDX.Vector3.Distance(source.ServerPosition, targetpos);
        float missilespeed = speed;

        if (source.ChampionName == "Jinx" && distance > 1350)
        {
            const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second
            var acceldifference = distance - 1350f;

            if (acceldifference > 150f) //it only accelerates 150 units
                acceldifference = 150f;

            var difference = distance - 1500f;

            missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) + difference * 2200f) / distance;
        }

        return (distance / missilespeed + delay);
    }

    void HandleUltTarget(HeroInfo enemyInfo)
    {
        bool ultNow = false;
        bool me = false;
        float me_timeneed = 0f;
        float incoming_damage = 0f;

        foreach (Obj_AI_Hero champ in Allies.Where(x => //gathering the damage from allies should probably be done once only with timers
                           x.IsValid && !x.IsDead && ((x.IsMe && !x.IsStunned) || TeamUlt.Items.Any(item => item.GetValue<bool>() && item.Name == x.ChampionName)) && CanUseUlt(x)))
        {
            if (UltSpellData[champ.ChampionName].Collision && IsCollidingWithChamps(champ, OktwCommon.EnemySpawnPoint.Position, UltSpellData[champ.ChampionName].Width))
            {
                continue;
            }

            var timeneeded = GetUltTravelTime(champ, UltSpellData[champ.ChampionName].Speed, UltSpellData[champ.ChampionName].Delay, OktwCommon.EnemySpawnPoint.Position) - 0.065f;
            float recall_cooldown = Math.Max(0, enemyInfo.teleport_end_tick - Game.Time);

            if (recall_cooldown >= timeneeded)
                incoming_damage += (float)Damage.GetSpellDamage(champ, enemyInfo.org, SpellSlot.R, UltSpellData[champ.ChampionName].SpellStage) * UltSpellData[champ.ChampionName].DamageMultiplicator;

            if (champ.IsMe)
            {
                me = true;

                me_timeneed = timeneeded;

                if (recall_cooldown - timeneeded < 0.06f)
                    ultNow = true;
            }
        }

        if (me)
        {
            float totalUltDamage = incoming_damage;
            float recall_cooldown = Math.Max(0, enemyInfo.teleport_end_tick - Game.Time);
            float targetHealth = GetTargetHealth(enemyInfo, (int)(recall_cooldown * 1000));
            int time = LeagueSharp.Common.Utils.TickCount;

            if (time - enemyInfo.last_visible_tick > 20000 && !Menu.Item("regardlessKey").GetValue<KeyBind>().Active)
            {
                if (totalUltDamage < enemyInfo.org.MaxHealth)
                {
                    enemyInfo.killable_with_baseult = false;
                    return;
                }
            }
            else if (totalUltDamage < targetHealth)
            {
                enemyInfo.killable_with_baseult = false;
                return;
            }

            enemyInfo.killable_with_baseult = true;
            enemyInfo.travel_baseult_time = me_timeneed;

            if (!ultNow || Menu.Item("panicKey").GetValue<KeyBind>().Active)
                return;

            Ultimate.Cast(OktwCommon.EnemySpawnPoint.Position, true);
            LastUltCastT = time;
        }
        else
        {
            enemyInfo.killable_with_baseult = false;
            enemyInfo.travel_baseult_time = 0;
        }
    }

    float GetTargetHealth(HeroInfo enemyInfo, int additionalTime)
    {
        if (enemyInfo.org.IsVisible)
            return enemyInfo.org.Health;

        float predictedHealth = enemyInfo.org.Health + enemyInfo.org.HPRegenRate * ((LeagueSharp.Common.Utils.TickCount - enemyInfo.last_visible_tick + additionalTime) / 1000f);

        return predictedHealth > enemyInfo.org.MaxHealth ? enemyInfo.org.MaxHealth : predictedHealth;
    }

    public void Game_OnGameUpdate(EventArgs args)
    {
        if (!Menu.Item("baseUlt").GetValue<bool>() || ObjectManager.Player.IsDead || ObjectManager.Player.IsDead)
            return;

        foreach (var enemyInfo in TrackerCore.heroes_info.Where(x =>
            x.Value.org.IsValid && x.Value.org.IsEnemy && !x.Value.org.IsDead &&
            !DisabledChampions.Item(x.Value.org.ChampionName).GetValue<bool>() &&
             x.Value.teleport_start_tick < x.Value.teleport_end_tick &&
              x.Value.teleport_start_tick > x.Value.teleport_abort_tick &&
            x.Value.teleport_type == Packet.S2C.Teleport.Type.Recall))
        {
            if (Environment.TickCount - LastUltCastT > 150)
                HandleUltTarget(enemyInfo.Value);
        }
    }

  
}

