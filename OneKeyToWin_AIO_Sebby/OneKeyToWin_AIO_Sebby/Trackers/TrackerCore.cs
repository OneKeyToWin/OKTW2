using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using SharpDX.Direct3D9;
using static LeagueSharp.Common.Packet;

class TrackerCore
{
    public static IDictionary<int, HeroInfo> heroes_info = new Dictionary<int, HeroInfo>();
    public static Obj_SpawnPoint enemy_spawn_point, ally_spawn_point;
    private List<Tuple<Obj_AI_Minion, Obj_AI_Hero>> clone_objects = new List<Tuple<Obj_AI_Minion, Obj_AI_Hero>>();

    public TrackerCore()
    {
        foreach (var it in ObjectManager.Get<Obj_SpawnPoint>())
        {
            if (it.IsEnemy)
                enemy_spawn_point = it;
            else
                ally_spawn_point = it;
        }

        foreach (var it in ObjectManager.Get<Obj_AI_Hero>())
        {
            var hero = new HeroInfo();

            hero.org = it;
            hero.old_dead = it.IsDead;
            hero.last_visible_tick = Utils.TickCount;
            hero.last_visible_position = OktwCommon.EnemySpawnPoint.Position;

            if (it.GetSpell(SpellSlot.Summoner1).Name.Contains("Smite"))
                hero.is_jungler = true;
            else if (it.GetSpell(SpellSlot.Summoner2).Name.Contains("Smite"))
                hero.is_jungler = true;

            heroes_info[it.NetworkId] = hero;
        }

        Game.OnUpdate += OnUpdate;
        GameObject.OnCreate += OnCreate;
        GameObject.OnDelete += OnDelete;
        Obj_AI_Base.OnTeleport += OnTeleport;
    }

    private void OnDelete(GameObject obj, EventArgs args)
    {
        var minion = obj as Obj_AI_Minion;

        if (minion != null)
            clone_objects.Remove(clone_objects.SingleOrDefault(x => x.Item1.NetworkId == minion.NetworkId));
    }

    private void OnCreate(GameObject obj, EventArgs args)
    {
        if (obj.IsAlly || !obj.Position.IsValid())
            return;

        var missile = obj as MissileClient;
        var minion = obj as Obj_AI_Minion;

        if (missile != null)
        {
            var sender = missile.SpellCaster;

            if (sender != null && sender.IsValid && !sender.IsVisible && sender is Obj_AI_Hero && sender.IsEnemy && sender.Distance(obj.Position) > 600)
            {
                var info = heroes_info[sender.NetworkId];
                if (LeagueSharp.Common.Utils.TickCount - info.last_visible_tick > 5000)
                {
                    info.last_visible_position = missile.StartPosition;
                    info.last_visible_tick = LeagueSharp.Common.Utils.TickCount;
                }
            }
        }
        else if (minion == null)
        {
            foreach (var map in heroes_info)
            {
                var info = map.Value;
                var hero = info.org;

                if (hero.ChampionName == "Teemo")
                    return;

                if (hero.IsEnemy && !hero.IsVisible && !hero.IsDead && hero.Distance(obj.Position) > 1000)
                {
                    var cont = true;
                    foreach (var ally in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (!ally.IsAlly)
                            continue;

                        if (ally.IsTargetable && ally.Distance(obj.Position) < 1400)
                        {
                            cont = false;
                            continue;
                        }
                    }

                    if (!cont)
                        continue;

                    if (obj.Name.Contains(hero.BaseSkinName))
                    {
                        info.last_visible_position = obj.Position;
                        info.last_visible_tick = LeagueSharp.Common.Utils.TickCount;
                        return;
                    }

                    var ai_base = obj as Obj_AI_Base;

                    if (ai_base != null && ai_base.BaseSkinName.Contains(hero.BaseSkinName))
                    {
                        info.last_visible_position = obj.Position;
                        info.last_visible_tick = LeagueSharp.Common.Utils.TickCount;
                        return;
                    }
                }
            }
        }
        else
        {
            Obj_AI_Hero real_hero = null;
            foreach (var map in heroes_info)
            {
                if (obj.Name == map.Value.org.Name)
                {
                    real_hero = map.Value.org;
                    break;
                }
            }

            if (real_hero != null)
                clone_objects.Add(new Tuple<Obj_AI_Minion, Obj_AI_Hero>(obj as Obj_AI_Minion, real_hero));
        }
    }

    private void OnTeleport(GameObject sender, GameObjectTeleportEventArgs args)
    {
        var unit = sender as Obj_AI_Hero;

        if (unit == null)
            return;

        var hero = heroes_info[sender.NetworkId];
        var recall = S2C.Teleport.Decoded(unit, args);

        switch (recall.Status)
        {
            case S2C.Teleport.Status.Finish:
                if (sender.IsEnemy && recall.Type == S2C.Teleport.Type.Recall)
                {
                    hero.last_visible_position = OktwCommon.EnemySpawnPoint.Position;
                    hero.last_visible_tick = LeagueSharp.Common.Utils.TickCount;
                }
                hero.teleport_finish_tick = Game.Time;
                break;

            case S2C.Teleport.Status.Abort:
                hero.teleport_abort_tick = Game.Time;
                break;

            case S2C.Teleport.Status.Start:
                hero.teleport_type = recall.Type;
                hero.teleport_start_tick = Game.Time;
                hero.teleport_end_tick = Game.Time + recall.Duration / 1000f;
                break;
        }
    }

    private void OnUpdate(EventArgs args)
    {
        foreach (var map in heroes_info)
        {
            var hero = map.Value;
            var it = hero.org;

            if (it.IsAlly)
            {
                hero.last_visible_tick = LeagueSharp.Common.Utils.TickCount;
                hero.last_visible_position = it.Position;
            }
            else
            {
                if (it.IsVisible)
                {
                    hero.last_visible_tick = LeagueSharp.Common.Utils.TickCount;
                    hero.last_visible_position = it.Position;
                    hero.last_visible_real = Game.Time;
                    hero.detected_changes_in_row = 0;
                }
                else if (Game.Time - hero.last_visible_real > 0.1f)
                {
                    if (hero.last_position != it.Position)
                    {
                        hero.last_visible_tick = LeagueSharp.Common.Utils.TickCount;
                        hero.last_visible_position = it.Position;
                        hero.detected_changes_in_row++;
                    }
                    if (hero.detected_changes_in_row > 10 && !hero.is_fogofwar)
                    {
                        hero.is_fogofwar = true;
                    }
                }
                hero.last_position = it.Position;
            }

            if (hero.old_dead != it.IsDead && it.IsEnemy)
            {
                if (!it.IsDead)
                {
                    hero.last_visible_position = OktwCommon.EnemySpawnPoint.Position;
                    hero.last_visible_tick = LeagueSharp.Common.Utils.TickCount;
                }
                else
                {
                    float dead_timer = 0f;
                    var level = it.Level;

                    if (level <= 6)
                        dead_timer = level * 2 + 4;
                    else if (level == 7)
                        dead_timer = 21;
                    else if (level >= 8)
                        dead_timer = (float)level * 2.5f + 7.5f;

                    var time = Game.Time;

                    var current_minutes = (int)(time / 60f);

                    var BRW = 1f;
                    if (time > 3210f)
                        BRW = 1.5f;
                    else if (time > 2700f)
                        BRW += ((1f * (current_minutes - 15) * 2 * 0.425f) + (1f * (current_minutes - 30) * 2 * 0.30f) + (1f * (current_minutes - 45) * 2 * 1.45f)) / 100f;
                    else if (time > 1800f)
                        BRW += ((1f * (current_minutes - 15) * 2 * 0.425f) + (1f * (current_minutes - 30) * 2 * 0.30f)) / 100f;
                    else if (time > 900f)
                        BRW += (1f * (current_minutes - 15) * 2 * 0.425f) / 100f;

                    dead_timer *= BRW;

                    hero.respawn_time = time + (int)(dead_timer + 1f);
                }
            }

            hero.old_dead = it.IsDead;
        }
    }
}

