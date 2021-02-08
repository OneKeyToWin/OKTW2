using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby;
using SebbyLib;
using SharpDX;
using SharpDX.Direct3D9;
using static LeagueSharp.Common.Packet;

public class rectangle_f
{
    public float x;
    public float y;
    public float w;
    public float h;
};

class TrackerCore : Program
{
    public static IDictionary<int, HeroInfo> heroes_info = new Dictionary<int, HeroInfo>();
    public static Obj_SpawnPoint enemy_spawn_point, ally_spawn_point;
    public List<Tuple<Obj_AI_Minion, Obj_AI_Hero>> clone_objects = new List<Tuple<Obj_AI_Minion, Obj_AI_Hero>>();

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

    public void DrawRect(float x, float y, int width, float height, float thickness, System.Drawing.Color color)
    {
        DrawingInternal.AddFilledRect
        (
            new SharpDX.Vector3(x, y, 0),
            new SharpDX.Vector3(x + width, y + height, 0), color, 0, -1
        );
    }

    public rectangle_f get_screen_bouding_rectangle(GameObject obj)
    {
        float minX = 0, maxX = 0, minY = 0, maxY = 0;

        var bbox_min = obj.BBox.Minimum;
        var bbox_max = obj.BBox.Maximum;

        List<SharpDX.Vector3> corners = new List<SharpDX.Vector3>
            {
            new SharpDX.Vector3(bbox_min.X, bbox_max.Y, bbox_max.Z),
            new SharpDX.Vector3(bbox_max.X, bbox_max.Y, bbox_max.Z),
            new SharpDX.Vector3(bbox_max.X, bbox_min.Y, bbox_max.Z),
            new SharpDX.Vector3(bbox_min.X, bbox_min.Y, bbox_max.Z),
            new SharpDX.Vector3(bbox_min.X, bbox_max.Y, bbox_min.Z),
            new SharpDX.Vector3(bbox_max.X, bbox_max.Y, bbox_min.Z),
            new SharpDX.Vector3(bbox_max.X, bbox_min.Y, bbox_min.Z),
            new SharpDX.Vector3(bbox_min.X, bbox_min.Y, bbox_min.Z)
        };

        foreach (var corner in corners)
        {
            var pos = Drawing.WorldToScreen(corner);

            var x = (int)Math.Round(pos.X);
            var y = (int)Math.Round(pos.Y);

            // Compare current with existing X
            if (minX == 0 || x < minX)
            {
                minX = x;
            }
            else if (maxX == 0 || x > maxX)
            {
                maxX = x;
            }

            // Compare current with existing Y
            if (minY == 0 || y < minY)
            {
                minY = y;
            }
            else if (maxY == 0 || y > maxY)
            {
                maxY = y;
            }
        }

        return new rectangle_f { x = minX, y = minY, w = maxX - minX, h = maxY - minY };
    }

    public bool is_on_screen(SharpDX.Vector2 pos)
    {
        return pos.X > 0 && pos.X <= Drawing.Width && pos.Y > 0 && pos.Y <= Drawing.Height;
    }

    public System.Drawing.Color GetColorFade(System.Drawing.Color ColorA, System.Drawing.Color ColorB, float Percentage)
    {
        var R1 = ColorA.R;
        var R2 = ColorB.R;
        var G1 = ColorA.G;
        var G2 = ColorB.G;
        var B1 = ColorA.B;
        var B2 = ColorB.B;
        var A1 = ColorA.A;
        var A2 = ColorB.A;

        return System.Drawing.Color.FromArgb((int)((1f - Percentage) * A1 + Percentage * A2), (int)((1f - Percentage) * R1 + Percentage * R2), (int)((1f - Percentage) * G1 + Percentage * G2), (int)((1f - Percentage) * B1 + Percentage * B2));
    }

    public void draw_circle_on_minimap(SharpDX.Vector3 center, float radius, System.Drawing.Color color, float thickness, int quality)
    {
        Geometry.Polygon result = new Geometry.Polygon();

        result.Add(new SharpDX.Vector3(0, 0, 0));
        result.Add(new SharpDX.Vector3(0, 14800, 0));
        result.Add(new SharpDX.Vector3(14800, 14800, 0));
        result.Add(new SharpDX.Vector3(14800, 0, 0));

        List<SharpDX.Vector3> points = new List<SharpDX.Vector3>();
        for (var i = 1; i <= quality; ++i)
        {
            var angle = i * 2 * Math.PI / quality;
            points.Add(new SharpDX.Vector3(center.X + radius * (float)(Math.Cos(angle)), center.Y + radius * (float)(Math.Sin(angle)), 0));
        }

        for (var i = 0; i < points.Count; ++i)
        {
            var start = points[i];
            var end = points[points.Count - 1 == i ? 0 : i + 1];
            if (!result.IsInside(start) && !result.IsInside(end))
                continue;

            DrawingInternal.AddLineOnScreen(Drawing.WorldToMinimap(start).To3D(), Drawing.WorldToMinimap(end).To3D(), color, thickness);
        }
    }

    public void color_convert_hsv_to_rgb(float h, float s, float v, ref float out_r, ref float out_g, ref float out_b)
    {
        if (s == 0.0f)
        {
            out_r = out_g = out_b = v;
            return;
        }

        h = (h % 1f) / (60.0f / 360.0f);
        int i = (int)h;
        float f = h - (float)i;
        float p = v * (1.0f - s);
        float q = v * (1.0f - s * f);
        float t = v * (1.0f - s * (1.0f - f));

        switch (i)
        {
            case 0: out_r = v; out_g = t; out_b = p; break;
            case 1: out_r = q; out_g = v; out_b = p; break;
            case 2: out_r = p; out_g = v; out_b = t; break;
            case 3: out_r = p; out_g = q; out_b = v; break;
            case 4: out_r = t; out_g = p; out_b = v; break;
            case 5: default: out_r = v; out_g = p; out_b = q; break;
        }
    }

    public System.Drawing.Color hp_color(int percetn_hp)
    {
        float hue = ((6.0f * percetn_hp) / 5.0f) / 360.0f;
        float r = 0, g = 0, b = 0, a = 0;
        color_convert_hsv_to_rgb(hue, 1.0f, 1.0f, ref r, ref g, ref b);

        r *= 255f;
        g *= 255f;
        b *= 255f;

        if (percetn_hp < 30)
            a = LeagueSharp.Common.Utils.TickCount % 255;
        else
            a = 255;

        return System.Drawing.Color.FromArgb((int)a, (int)r, (int)g, (int)b);
    }

    public void draw_hp_bar_obj(Obj_AI_Hero obj)
    {
        if (!obj.IsVisibleOnScreen)
            return;

        var hp_bar_pos = obj.HPBarPosition + new SharpDX.Vector2(10, 31);

        if (!is_on_screen(hp_bar_pos))
            return;

        bool is_real_1440 = Drawing.Height >= 1440;
        bool is_scale_1440 = Drawing.Height > 1300;
        var hp_bar_w = is_scale_1440 ? 126f : 112f;
        if (is_scale_1440)
            hp_bar_w = 134f;

        var passive_size = is_scale_1440 ? 24f : 21f;

        if (obj.PassiveCooldownTotalTime != 0)
        {
            var end_time = obj.PassiveCooldownEndTime - Game.Time;
            var cur_spell = hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(7, 19, 0) : new SharpDX.Vector3(-1, 11, 0));
            cur_spell.X -= is_scale_1440 ? 26 : 23;
            cur_spell.Y += is_scale_1440 ? 4 : 3;

            DrawingInternal.AddRect(cur_spell, cur_spell + new SharpDX.Vector3(passive_size, passive_size, 0), System.Drawing.Color.FromArgb(114, 104, 66), 0, -1);

            if (end_time > 0f)
            {
                var cool_int = (int)(end_time + 1f);
                var draw_spell = cur_spell + new SharpDX.Vector3(1, 1, 1);

                DrawingInternal.AddImage(obj.PassiveIconTexture, draw_spell, new SharpDX.Vector3((float)passive_size - 2, (float)passive_size - 2, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.3f, 0.3f, 0.3f, 1), new SharpDX.Vector4(0, 0, 0, 0));

                var half = ((float)passive_size - 2) / 2f;
                var text_size = DrawingInternal.CalcTextSize(10, cool_int.ToString());
                var rcool_text_draw = new SharpDX.Vector3(draw_spell.X + half, draw_spell.Y + half, 0);

                DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(rcool_text_draw.X - (text_size.X / 2f), rcool_text_draw.Y - (text_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 255, 255, 255), 10, cool_int.ToString());

            }
            else
                DrawingInternal.AddImage(obj.PassiveIconTexture, cur_spell + new SharpDX.Vector3(1, 1, 0), new SharpDX.Vector3((float)passive_size - 2, (float)passive_size - 2, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(1, 1, 1, 1), new SharpDX.Vector4(0, 0, 0, 0));
        }

        var per_one = (int)(hp_bar_w / 4f);

        for (var i = (int)(SpellSlot.Q); i <= (int)(SpellSlot.R); i++)
        {
            var spell = obj.GetSpell((SpellSlot)(i));
            var spell_img = spell.IconTexture;

            var cur_spell = hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(7, 19, 0) : new SharpDX.Vector3(-1, 11, 0));
            DrawingInternal.AddRect(cur_spell, cur_spell + new SharpDX.Vector3(per_one, per_one, 0), System.Drawing.Color.FromArgb(114, 104, 66), 0, -1);

            var spell_cool = spell.Level > 0 ? spell.CooldownExpiresEx : 1f;
            if (spell_cool > 0f)
            {
                var cool_int = (int)(spell_cool + 1f);

                var draw_spell = hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(8, 20, 0) : new SharpDX.Vector3(0, 12, 0));
                DrawingInternal.AddImage(spell_img, draw_spell, new SharpDX.Vector3((float)per_one - 2, (float)per_one - 2, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.3f, 0.3f, 0.3f, 1), new SharpDX.Vector4(0, 0, 0, 0));
                if (spell.Level > 0)
                {
                    var half = ((float)per_one - 2) / 2f;
                    var text_size = DrawingInternal.CalcTextSize(13, cool_int.ToString());
                    var rcool_text_draw = new SharpDX.Vector3(draw_spell.X + half, draw_spell.Y + half, 0);

                    DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(rcool_text_draw.X - (text_size.X / 2f), rcool_text_draw.Y - (text_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 255, 255, 255), 13, cool_int.ToString());
                }
            }
            else
                DrawingInternal.AddImage(spell_img, hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(8, 20, 0) : new SharpDX.Vector3(0, 12, 0)), new SharpDX.Vector3((float)per_one - 2, (float)per_one - 2, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(1, 1, 1, 1), new SharpDX.Vector4(0, 0, 0, 0));

            hp_bar_pos.X += per_one;
        }

        {
            var spell = obj.GetSpell(SpellSlot.Summoner2);
            var spell_img = spell.IconTexture;

            var cur_spell = hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(7, 19, 0) : new SharpDX.Vector3(-1, 11, 0));
            DrawingInternal.AddRect(cur_spell, cur_spell + new SharpDX.Vector3(per_one, per_one, 0), System.Drawing.Color.FromArgb(114, 104, 66), 0, -1);

            var spell_cool = spell.Level > 0 ? spell.CooldownExpiresEx : 1f;
            if (spell_cool > 0f)
            {
                var cool_int = (int)(spell_cool + 1f);

                var draw_spell = hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(8, 20, 0) : new SharpDX.Vector3(0, 12, 0));
                DrawingInternal.AddImage(spell_img, draw_spell, new SharpDX.Vector3((float)per_one - 2, (float)per_one - 2, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.3f, 0.3f, 0.3f, 1), new SharpDX.Vector4(0, 0, 0, 0));
                if (spell.Level > 0)
                {
                    var half = ((float)per_one - 2) / 2f;
                    var text_size = DrawingInternal.CalcTextSize(13, cool_int.ToString());
                    var rcool_text_draw = new SharpDX.Vector3(draw_spell.X + half, draw_spell.Y + half, 0);

                    DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(rcool_text_draw.X - (text_size.X / 2f), rcool_text_draw.Y - (text_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 255, 255, 255), 13, cool_int.ToString());
                }
            }
            else
                DrawingInternal.AddImage(spell_img, hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(8, 20, 0) : new SharpDX.Vector3(0, 12, 0)), new SharpDX.Vector3((float)per_one - 2, (float)per_one - 2, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(1, 1, 1, 1), new SharpDX.Vector4(0, 0, 0, 0));

            hp_bar_pos.Y -= per_one;
        }

        {
            var spell = obj.GetSpell(SpellSlot.Summoner1);
            var spell_img = spell.IconTexture;

            var cur_spell = hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(7, 19, 0) : new SharpDX.Vector3(-1, 11, 0));
            DrawingInternal.AddRect(cur_spell, cur_spell + new SharpDX.Vector3(per_one, per_one, 0), System.Drawing.Color.FromArgb(114, 104, 66), 0, -1);

            var spell_cool = spell.Level > 0 ? spell.CooldownExpiresEx : 1f;
            if (spell_cool > 0f)
            {
                var cool_int = (int)(spell_cool + 1f);

                var draw_spell = hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(8, 20, 0) : new SharpDX.Vector3(0, 12, 0));
                DrawingInternal.AddImage(spell_img, draw_spell, new SharpDX.Vector3((float)per_one - 2, (float)per_one - 2, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.3f, 0.3f, 0.3f, 1), new SharpDX.Vector4(0, 0, 0, 0));
                if (spell.Level > 0)
                {
                    var half = ((float)per_one - 2) / 2f;
                    var text_size = DrawingInternal.CalcTextSize(13, cool_int.ToString());
                    var rcool_text_draw = new SharpDX.Vector3(draw_spell.X + half, draw_spell.Y + half, 0);

                    DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(rcool_text_draw.X - (text_size.X / 2f), rcool_text_draw.Y - (text_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 255, 255, 255), 13, cool_int.ToString());
                }
            }
            else
                DrawingInternal.AddImage(spell_img, hp_bar_pos.To3D() + (is_real_1440 ? new SharpDX.Vector3(8, 20, 0) : new SharpDX.Vector3(0, 12, 0)), new SharpDX.Vector3((float)per_one - 2, (float)per_one - 2, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(1, 1, 1, 1), new SharpDX.Vector4(0, 0, 0, 0));
        }
    }
}

