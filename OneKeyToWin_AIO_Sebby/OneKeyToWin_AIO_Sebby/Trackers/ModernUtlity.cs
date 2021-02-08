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

class ModernUtlity : TrackerCore
{
    public Menu Menu { get; set; }

    //elements
    private MenuItem hud_tracker_enabled;
    private MenuItem hud_scale_e;

    private MenuItem hud_x_offset;
    private MenuItem hud_y_offset;
    private MenuItem hud_position_mode;

    private MenuItem clone_tracker;
    private MenuItem predict_position;
    private MenuItem draw_portret;

    private MenuItem ht_enabled;
    private MenuItem ht_health_percent;
    private MenuItem ht_ultimate;
    private MenuItem ht_offset_text;
    private MenuItem ht_offset_top;
    private MenuItem ht_offset_right;
    private MenuItem ht_spacing;
    private MenuItem ht_font_size;
    private MenuItem ht_version;

    private MenuItem esp_lines_enabled;
    private MenuItem esp_distance;
    private MenuItem onlyjunglerss;
    private MenuItem show_only_enemy;

    private MenuItem show_recalls;
    private MenuItem show_teleports;
    private MenuItem show_shenteleport;
    private MenuItem show_tfteleport;

    private MenuItem remove_delay_recall;

    private MenuItem en_enabled;
    private MenuItem en_animated;
    private MenuItem en_thickness;

    private MenuItem draw_minimap_heores;
    private MenuItem draw_hero_range;

    private MenuItem spell_tracker_enemy;
    private MenuItem spell_tracker_ally;
    private MenuItem spell_tracker_me;

    public ModernUtlity()
    {
        var menu = MainMenu.AddSubMenu(new Menu("Modern Utility", "ModernUtility"));

        var ch_track = menu.AddSubMenu(new Menu("Champion tracker", "ch_track"));
        {
            hud_tracker_enabled = ch_track.AddItem(new MenuItem("hud_tracker_enabled", "Enabled").SetValue(true));
            hud_scale_e = ch_track.AddItem(new MenuItem("scale_hud", "Hud scale").SetValue(new Slider(10, 5, 25)));
            hud_position_mode = ch_track.AddItem(new MenuItem("hud_style", "Hud position").SetValue(new StringList(new string[] { "Right vertical", "Left vertical", "Right Horizontal", "Left Horizontal" }, 0)));
            hud_x_offset = ch_track.AddItem(new MenuItem("hud_x", "Hud X offset").SetValue(new Slider(0, 0, 1000)));
            hud_y_offset = ch_track.AddItem(new MenuItem("hud_y", "Hud Y offset").SetValue(new Slider(0, -1000, 1000)));
        };

        var clone_tracker_menu = menu.AddSubMenu(new Menu("Clone tracker", "esp_clones"));
        {
            clone_tracker = clone_tracker_menu.AddItem(new MenuItem("clone.tracker", "Draw clones").SetValue(true));
        };

        var health_tracker = menu.AddSubMenu(new Menu("Health tracker", "health_tracker"));
        {
            ht_enabled = health_tracker.AddItem(new MenuItem("ht_enabled", "Enabled").SetValue(false));
            ht_health_percent = health_tracker.AddItem(new MenuItem("ht_health_percent", "Champion health percent").SetValue(true));
            ht_ultimate = health_tracker.AddItem(new MenuItem("ht_ultimate", "Champion ultimate").SetValue(true));
            ht_offset_text = health_tracker.AddItem(new MenuItem("ht_offset_text", "Offset Text").SetValue(new Slider(30, -100, 100)));
            ht_offset_top = health_tracker.AddItem(new MenuItem("ht_offset_top", "Offset Top").SetValue(new Slider(75, 0, 1500)));
            ht_offset_right = health_tracker.AddItem(new MenuItem("ht_offset_right", "Offset Right").SetValue(new Slider(170, 0, 1500)));
            ht_spacing = health_tracker.AddItem(new MenuItem("ht_spacing", "Spacing").SetValue(new Slider(10, 0, 30)));
            ht_font_size = health_tracker.AddItem(new MenuItem("ht_font_size", "Font Size").SetValue(new Slider(14, 12, 25)));
            ht_version = health_tracker.AddItem(new MenuItem("ht_version", "Display options").SetValue(new StringList(new string[] { "Compact", "Clean" }, 0)));
        };

        var en_range = menu.AddSubMenu(new Menu("Enemy ranges", "en_range"));
        {
            en_enabled = en_range.AddItem(new MenuItem("er_enabled", "Show ranges").SetValue(true));
            en_animated = en_range.AddItem(new MenuItem("er_animated", "Animated fade").SetValue(true));
            en_thickness = en_range.AddItem(new MenuItem("er_thick", "Line thickness percentage").SetValue(new Slider(100, 0, 250)));
        };

        var sp_track = menu.AddSubMenu(new Menu("Spell tracker", "sp_track"));
        {
            spell_tracker_enemy = sp_track.AddItem(new MenuItem("sp_enemy", "Enemy").SetValue(true));
            spell_tracker_ally = sp_track.AddItem(new MenuItem("sp_ally", "Ally").SetValue(false));
            spell_tracker_me = sp_track.AddItem(new MenuItem("sp_ me", "Me").SetValue(false));
        };

        var minimap = menu.AddSubMenu(new Menu("Minimap tracker", "minimap"));
        {
            draw_minimap_heores = minimap.AddItem(new MenuItem("mini_show", "Show champion").SetValue(true));
            draw_hero_range = minimap.AddItem(new MenuItem("mini_range", "Show champion range").SetValue(true));
        };

        var esp_lines = menu.AddSubMenu(new Menu("ESP lines", "esp_lines"));
        {
            esp_lines_enabled = esp_lines.AddItem(new MenuItem("esp_enabled", "ESP lines").SetValue(true));
            onlyjunglerss = esp_lines.AddItem(new MenuItem("esp_enabled.j.only", "Only junglers").SetValue(false));
            show_only_enemy = esp_lines.AddItem(new MenuItem("esp_enabled.e.only", "Only enemies").SetValue(false));
            esp_distance = esp_lines.AddItem(new MenuItem("esp_enabled.dist", "ESP lines distance").SetValue(new Slider(4200, 0, 8000)));
        };

        var rcall_track = menu.AddSubMenu(new Menu("Recall tracker", "rcall_track"));
        {
            show_recalls = rcall_track.AddItem(new MenuItem("rc_recalls", "Show recalls").SetValue(true));
            show_teleports = rcall_track.AddItem(new MenuItem("rc_tel", "Show teleports").SetValue(true));
            show_shenteleport = rcall_track.AddItem(new MenuItem("rc_shen", "Show Shen").SetValue(true));
            show_tfteleport = rcall_track.AddItem(new MenuItem("rc_tf", "Show Twisted Fate").SetValue(true));

            remove_delay_recall = rcall_track.AddItem(new MenuItem("rc_delay", "Remove recall delay").SetValue(new Slider(20, 0, 50)));
        }

        var fog_tracker = menu.AddSubMenu(new Menu("Fog tracker", "champion.fog.tracker"));
        {
            predict_position = fog_tracker.AddItem(new MenuItem("fog.tracker.predict", "Enable fog tracker").SetValue(true));
            draw_portret = fog_tracker.AddItem(new MenuItem("fog.tracker.draw.portert", "Draw champ portert").SetValue(true));
        }

        Menu = menu;
        Drawing.OnDraw += Drawing_OnDraw;
    }

    private void Drawing_OnDraw(EventArgs args)
    {
        if (clone_tracker.GetValue<bool>())
        {
            foreach (var it in clone_objects)
            {
                if (it.Item1.IsDead || !it.Item1.IsVisibleOnScreen || !it.Item1.IsVisible)
                    continue;

                var clone_bounding = get_screen_bouding_rectangle(it.Item1);

                var size = Math.Min(clone_bounding.w, clone_bounding.h);
                var half_size_value = (float)(size / 2f);
                var half_size = new SharpDX.Vector3(half_size_value, half_size_value, 0f);
                var center = new SharpDX.Vector3(clone_bounding.x + (clone_bounding.w / 2), clone_bounding.y + (clone_bounding.h / 2), 0f);

                DrawingInternal.AddLineOnScreen(center - half_size, center + half_size, System.Drawing.Color.FromArgb(0xFF, 0x45, 0x00), 2f);
                DrawingInternal.AddLineOnScreen(center + new SharpDX.Vector3(-half_size_value, half_size_value, 0f), center + new SharpDX.Vector3(half_size_value, -half_size_value, 0f), System.Drawing.Color.FromArgb(0xFF, 0x45, 0x00), 2f);

                if (it.Item2.IsDead || !it.Item2.IsVisibleOnScreen || !it.Item2.IsVisible)
                    continue;

                var champ_bounding = get_screen_bouding_rectangle(it.Item2);

                size = Math.Min(champ_bounding.w, champ_bounding.h) / 2f;
                half_size_value = size / 2f;

                DrawingInternal.AddLineOnScreen(new SharpDX.Vector3(champ_bounding.x, champ_bounding.y, 0f), new SharpDX.Vector3(champ_bounding.x + half_size_value, champ_bounding.y, 0f), System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32), 2f);
                DrawingInternal.AddLineOnScreen(new SharpDX.Vector3(champ_bounding.x, champ_bounding.y, 0f), new SharpDX.Vector3(champ_bounding.x, champ_bounding.y + half_size_value, 0f), System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32), 2f);

                DrawingInternal.AddLineOnScreen(new SharpDX.Vector3(champ_bounding.x + champ_bounding.w, champ_bounding.y, 0f), new SharpDX.Vector3(champ_bounding.x + champ_bounding.w - half_size_value, champ_bounding.y, 0f), System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32), 2f);
                DrawingInternal.AddLineOnScreen(new SharpDX.Vector3(champ_bounding.x + champ_bounding.w, champ_bounding.y, 0f), new SharpDX.Vector3(champ_bounding.x + champ_bounding.w, champ_bounding.y + half_size_value, 0f), System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32), 2f);

                DrawingInternal.AddLineOnScreen(new SharpDX.Vector3(champ_bounding.x + champ_bounding.w, champ_bounding.y + champ_bounding.h, 0f), new SharpDX.Vector3(champ_bounding.x + champ_bounding.w - half_size_value, champ_bounding.y + champ_bounding.h, 0f), System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32), 2f);
                DrawingInternal.AddLineOnScreen(new SharpDX.Vector3(champ_bounding.x + champ_bounding.w, champ_bounding.y + champ_bounding.h, 0f), new SharpDX.Vector3(champ_bounding.x + champ_bounding.w, champ_bounding.y + champ_bounding.h - half_size_value, 0f), System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32), 2f);

                DrawingInternal.AddLineOnScreen(new SharpDX.Vector3(champ_bounding.x, champ_bounding.y + champ_bounding.h, 0f), new SharpDX.Vector3(champ_bounding.x + half_size_value, champ_bounding.y + champ_bounding.h, 0f), System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32), 2f);
                DrawingInternal.AddLineOnScreen(new SharpDX.Vector3(champ_bounding.x, champ_bounding.y + champ_bounding.h, 0f), new SharpDX.Vector3(champ_bounding.x, champ_bounding.y + champ_bounding.h - half_size_value, 0f), System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32), 2f);
            }
        }

        Vector3 champ_size = new SharpDX.Vector3(72, 70, 0);
        Vector3 avatar_size = new SharpDX.Vector3(43, 43, 0);
        Vector3 avatar_image_size = new SharpDX.Vector3(39, 39, 0);

        float scale = ((float)hud_scale_e.GetValue<Slider>().Value / 10f);
        var one_frame_champ = champ_size * scale;
        var base_pos = new Vector3();

        switch (hud_position_mode.GetValue<StringList>().SelectedIndex)
        {
            case 0:
                {
                    base_pos = new SharpDX.Vector3(Drawing.Width - 10 - one_frame_champ.Y - hud_x_offset.GetValue<Slider>().Value, 100 + hud_y_offset.GetValue<Slider>().Value, 0);
                    break;
                }
            case 1:
                {
                    base_pos = new SharpDX.Vector3(10 + hud_x_offset.GetValue<Slider>().Value, 100 + hud_y_offset.GetValue<Slider>().Value, 0);
                    break;
                }
            case 2:
                {
                    var enemy_count = 0;
                    foreach (var it in heroes_info)
                        if (it.Value.org.IsEnemy)
                            enemy_count++;

                    var totalsize_x = enemy_count * (10 + one_frame_champ.X);
                    base_pos = new SharpDX.Vector3(Drawing.Width - totalsize_x - hud_x_offset.GetValue<Slider>().Value, Drawing.Height - 600 + hud_y_offset.GetValue<Slider>().Value, 0);
                    break;
                }
            case 3:
                {
                    base_pos = new SharpDX.Vector3(10 + hud_x_offset.GetValue<Slider>().Value, Drawing.Height - 600 + hud_y_offset.GetValue<Slider>().Value, 0);
                    break;
                }
        }

        var ScreenPos = new SharpDX.Vector3(Drawing.Width * 0.5f, Drawing.Height * 0.736111111f - 70f, 0);
        float i = 0;

        foreach (var map in heroes_info)
        {
            var hero = map.Value;
            var obj = hero.org;
            bool is_enemy = obj.IsEnemy;
            var texture_hero = obj.SquareIconPortrait;

            if (draw_portret.GetValue<bool>() && is_enemy)
            {
                if (!obj.IsDead && !obj.IsVisible)
                {
                    var clone_bounding = get_screen_bouding_rectangle(obj);
                    var image_size = new SharpDX.Vector3(65, 65, 0);

                    var center = new SharpDX.Vector3(clone_bounding.x + (clone_bounding.w / 2) - (image_size.X / 2f), clone_bounding.y - (clone_bounding.h * 0.25f), 0);
                    var ss_time = (int)((float)(LeagueSharp.Common.Utils.TickCount - hero.last_visible_tick) / 1000f);
                    var dead_size = DrawingInternal.CalcTextSize(14, ss_time.ToString());
                    DrawingInternal.AddImage(texture_hero, center + new SharpDX.Vector3(2, 2, 0), image_size, 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.4f, 0.4f, 0.4f, 1), new SharpDX.Vector4(0, 0, 0, 0));
                    var dead_text_draw = new SharpDX.Vector3(center.X + 2 + (image_size.X / 2f), center.Y + 2 + (image_size.Y / 2f), 0);
                    DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(dead_text_draw.X - (dead_size.X / 2f), dead_text_draw.Y - (dead_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 255, 0), 14, ss_time.ToString());
                }
            }

            if (!obj.IsDead)
            {
                if (ht_enabled.GetValue<bool>() && obj.IsEnemy)
                {
                    var champion = obj.ChampionName;
                    if (champion.Length > 12)
                    {
                        champion = champion.Remove(7) + "...";
                    }

                    bool draw_ultimate = ht_ultimate.GetValue<bool>();
                    bool draw_health_percent = ht_health_percent.GetValue<bool>();
                    int version = ht_version.GetValue<StringList>().SelectedIndex;
                    float spacing = ht_spacing.GetValue<Slider>().Value;
                    float hud_offset_text = ht_offset_text.GetValue<Slider>().Value;
                    float hud_offset_top = ht_offset_top.GetValue<Slider>().Value;
                    float hud_offset_right = ht_offset_right.GetValue<Slider>().Value;
                    int font_size = ht_font_size.GetValue<Slider>().Value;

                    var championInfo = draw_health_percent
                                  ? $"{champion} ({(int)obj.HealthPercent}%)"
                                  : champion;

                    if (draw_ultimate)
                    {
                        var timeR = obj.Spellbook.GetSpell(SpellSlot.R).CooldownExpires - Game.Time;
                        var ultText = timeR <= 0
                                          ? "READY"
                                          : (timeR < 10 ? timeR.ToString("N1") : ((int)timeR).ToString()) + "s";

                        if (obj.Spellbook.GetSpell(SpellSlot.R).Level == 0)
                        {
                            ultText = "N/A";
                        }

                        championInfo += $" - R: {ultText}";
                    }

                    if (version == 1)
                    {
                        const int Height = 25;

                        DrawingInternal.AddRect(
                            new SharpDX.Vector3(Drawing.Width - hud_offset_right, hud_offset_top + i, 0),
                            new SharpDX.Vector3(Drawing.Width - hud_offset_right + 200, hud_offset_top + i + Height, 0), System.Drawing.Color.FromArgb(175, 51, 55, 51), 0, -1);

                        // Draws the rectangle
                        DrawRect(
                            Drawing.Width - hud_offset_right,
                            hud_offset_top + i,
                            200,
                            Height,
                            1,
                            System.Drawing.Color.FromArgb(175, 51, 55, 51));

                        DrawRect(
                           Drawing.Width - hud_offset_right + 2,
                            hud_offset_top + i - -2,
                            obj.HealthPercent <= 0 ? 100 : (int)obj.HealthPercent * 2 - 4,
                            Height - 4,
                            1,
                            obj.HealthPercent < 30 && obj.HealthPercent > 0
                            ? System.Drawing.Color.FromArgb(250, 0, 23)
                            : obj.HealthPercent < 50
                            ? System.Drawing.Color.FromArgb(230, 169, 14)
                            : System.Drawing.Color.FromArgb(2, 157, 10));

                        var text_size = DrawingInternal.CalcTextSize(font_size, championInfo);
                        SharpDX.Vector3 point = new SharpDX.Vector3((int)(Drawing.Width - hud_offset_right - text_size.X / 2f)
                        + 200 / 2,
                        (int)(hud_offset_top + i + 13 - text_size.Y / 2f), 0);

                        DrawingInternal.AddTextOnScreen(point, System.Drawing.Color.FromArgb(175, 255, 255, 255), font_size, championInfo);
                    }
                    else
                    {
                        int BarHeight = 10;

                        var text_size = DrawingInternal.CalcTextSize(font_size, championInfo);
                        SharpDX.Vector3 point = new SharpDX.Vector3(Drawing.Width - hud_offset_right - hud_offset_text
                            - text_size.X,
                        (int)(hud_offset_top + i + 6 - text_size.Y / 2f), 0);

                        DrawingInternal.AddTextOnScreen(point, obj.HealthPercent > 0 ? System.Drawing.Color.FromArgb(255, 255, 255, 255) : System.Drawing.Color.FromArgb(244, 8, 8), font_size, championInfo);

                        // Draws the rectangle
                        DrawRect(
                            Drawing.Width - hud_offset_right,
                            hud_offset_top + i,
                            100,
                            BarHeight,
                            1,
                            System.Drawing.Color.FromArgb(51, 55, 51));

                        // Fils the rectangle
                        DrawRect(
                            Drawing.Width - hud_offset_right,
                            hud_offset_top + i,
                            obj.HealthPercent <= 0 ? 100 : (int)obj.HealthPercent,
                            BarHeight,
                            1,
                            obj.HealthPercent < 30 && obj.HealthPercent > 0
                            ? System.Drawing.Color.FromArgb(250, 0, 23)
                            : obj.HealthPercent < 50
                            ? System.Drawing.Color.FromArgb(230, 169, 14)
                            : System.Drawing.Color.FromArgb(2, 157, 10));
                    }

                    i += 20 + (version == 1 ? 5 : spacing);
                }

                if (esp_lines_enabled.GetValue<bool>() && !obj.IsMe && (!obj.IsVisibleOnScreen || !obj.IsVisible) && (!show_only_enemy.GetValue<bool>() || obj.IsEnemy) && (!onlyjunglerss.GetValue<bool>() || hero.is_jungler) && LeagueSharp.Common.Utils.TickCount - hero.last_visible_tick < 10000)
                {
                    var width = (esp_distance.GetValue<Slider>().Value - ObjectManager.Player.Distance(hero.last_visible_position)) / 150;
                    if (width > 1)
                    {
                        var destination = hero.last_visible_position;
                        var alfa = 130;
                        if (hero.is_jungler)
                            alfa = 255;

                        if (obj.IsEnemy)
                            DrawingInternal.AddLine(destination, ObjectManager.Player.Position.Extend(destination, 330), System.Drawing.Color.FromArgb(alfa, 255, 63, 0), width);
                        else
                            DrawingInternal.AddLine(destination, ObjectManager.Player.Position.Extend(destination, 330), System.Drawing.Color.FromArgb(alfa, 124, 229, 4), width);

                        DrawingInternal.AddText(ObjectManager.Player.Position.Extend(destination, 400), System.Drawing.Color.FromArgb(255, 255, 255, 255), 15, obj.ChampionName);
                    }
                }

                /*recalls*/
                if (is_enemy)
                {
                    var teleport_is_active = Game.Time < hero.teleport_end_tick && Game.Time >= hero.teleport_start_tick && hero.teleport_abort_tick < hero.teleport_start_tick && hero.teleport_finish_tick < hero.teleport_start_tick;

                    var recall_has_finished = teleport_is_active ? false : Game.Time - hero.teleport_finish_tick < (remove_delay_recall.GetValue<Slider>().Value / 10); /*menu*/
                    var recall_has_aborted = teleport_is_active ? false : Game.Time - hero.teleport_abort_tick < (remove_delay_recall.GetValue<Slider>().Value / 10);

                    if (teleport_is_active || (recall_has_finished || recall_has_aborted))
                    {
                        var cur_time = recall_has_finished ? hero.teleport_finish_tick : recall_has_aborted ? hero.teleport_abort_tick : Game.Time;

                        var TpType = hero.teleport_type;
                        var duration = hero.teleport_end_tick - hero.teleport_start_tick;
                        var TpPercent = Math.Max(0f, Math.Min(1f, ((cur_time) - hero.teleport_start_tick) / (duration)));
                        var DrawColor = System.Drawing.Color.FromArgb(50, 160, 175); // Teleport color

                        if (
                            (show_recalls.GetValue<bool>() && (TpType == S2C.Teleport.Type.Recall)) ||
                            (show_teleports.GetValue<bool>() && TpType == S2C.Teleport.Type.Teleport) ||
                            (show_shenteleport.GetValue<bool>() && TpType == S2C.Teleport.Type.Shen) ||
                            (show_tfteleport.GetValue<bool>() && TpType == S2C.Teleport.Type.TwistedFate)
                            )
                        {
                            if (TpType == S2C.Teleport.Type.Teleport)
                                DrawColor = System.Drawing.Color.FromArgb(175, 50, 155);
                            else if (TpType == S2C.Teleport.Type.Shen || TpType == S2C.Teleport.Type.TwistedFate)
                                DrawColor = System.Drawing.Color.FromArgb(220, 100, 60);
                            DrawingInternal.AddRect(new SharpDX.Vector3(ScreenPos.X - 204f, ScreenPos.Y - 1f, 0), new SharpDX.Vector3(ScreenPos.X - 204f + 417f, ScreenPos.Y - 1f + 14f, 0), recall_has_finished ? System.Drawing.Color.FromArgb(51, 184, 87) : recall_has_aborted ? System.Drawing.Color.FromArgb(255, 0, 0) : System.Drawing.Color.FromArgb(82, 65, 33), 0, -1);
                            DrawingInternal.AddFilledRect(new SharpDX.Vector3(ScreenPos.X - 203f, ScreenPos.Y, 0), new SharpDX.Vector3(ScreenPos.X - 203f + 415f, ScreenPos.Y + 12f, 0), System.Drawing.Color.FromArgb(16, 28, 24), 0, -1);
                            DrawingInternal.AddRect(new SharpDX.Vector3(ScreenPos.X - 205f, ScreenPos.Y - 2f, 0), new SharpDX.Vector3(ScreenPos.X - 205f + 419f, ScreenPos.Y - 2f + 16f, 0), System.Drawing.Color.FromArgb(140, 0, 0, 0), 0, -1);
                            DrawingInternal.AddFilledRect(new SharpDX.Vector3(ScreenPos.X - 202f, ScreenPos.Y, 0), new SharpDX.Vector3(ScreenPos.X - 202f + 415f * (1f - TpPercent), ScreenPos.Y + 12f, 0), DrawColor, 0, -1);

                            if (hero.killable_with_baseult && (TpType == S2C.Teleport.Type.Recall))
                            {
                                var base_ult_percent = Math.Max(0f, Math.Min(1f, hero.travel_baseult_time / duration));

                                if ((1f - TpPercent) > base_ult_percent)
                                    DrawingInternal.AddFilledRect(new SharpDX.Vector3(ScreenPos.X - 202f, ScreenPos.Y, 0), new SharpDX.Vector3(ScreenPos.X - 202f + 415f * (base_ult_percent), ScreenPos.Y + 12f, 0), System.Drawing.Color.FromArgb(212, 62, 51), 0, -1);
                            }

                            for (var ii = 0f; ii < 4.5f; ii += 1f)
                                DrawingInternal.AddRect(new SharpDX.Vector3(ScreenPos.X - 203f + ii, ScreenPos.Y + ii, 0), new SharpDX.Vector3(ScreenPos.X - 203f + ii + 415f - ii * 2, ScreenPos.Y + ii + 12f - ii * 2, 0), System.Drawing.Color.FromArgb(255 / ((int)(ii) + 1), 0, 0, 0), 0, -1);

                            var text_size = DrawingInternal.CalcTextSize(19, obj.ChampionName);

                            DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(ScreenPos.X - (text_size.X / 2f), ScreenPos.Y - 16f - (text_size.Y / 2f), 0), DrawColor, 19,
                            obj.ChampionName);

                            var duration_real = (duration - (cur_time - hero.teleport_start_tick));

                            if (duration_real < 0)
                                duration_real = 0f;

                            var text_size_len = DrawingInternal.CalcTextSize(19, duration_real.ToString("n1"));

                            DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(ScreenPos.X - (text_size_len.X / 2f), ScreenPos.Y + 26f - (text_size_len.Y / 2f), 0), DrawColor, 19,
                            duration_real.ToString("n1"));

                            ScreenPos.Y -= 70f;
                        }
                    }

                    if (en_enabled.GetValue<bool>() && obj.IsVisible)
                        draw_entity_range(obj);
                }

                /*minimap*/

                if (!obj.IsVisible && is_enemy)
                {
                    if (draw_minimap_heores.GetValue<bool>())
                    {
                        var is_recalling = Game.Time < hero.teleport_end_tick && Game.Time >= hero.teleport_start_tick && hero.teleport_abort_tick < hero.teleport_start_tick && hero.teleport_finish_tick < hero.teleport_start_tick;

                        var map_pos = Drawing.WorldToMinimap(hero.last_visible_position).To3D();

                        if (draw_hero_range.GetValue<bool>())
                        {
                            var radius = (obj.MoveSpeed > 1 ? obj.MoveSpeed : 540) * ((LeagueSharp.Common.Utils.TickCount - hero.last_visible_tick) / 1000f);

                            if (radius < 8000)
                            {
                                if (is_recalling)
                                {
                                    var startTime = hero.teleport_start_tick;
                                    var duration = hero.teleport_end_tick - hero.teleport_start_tick;
                                    radius = (obj.MoveSpeed > 1 ? obj.MoveSpeed : 540) * (((LeagueSharp.Common.Utils.TickCount - hero.last_visible_tick) / 1000f) - (Game.Time - startTime));
                                    if (radius < 8000)
                                        draw_circle_on_minimap(hero.last_visible_position + new SharpDX.Vector3(-12, -12, 0), radius, System.Drawing.Color.FromArgb(255, 255, 0), 1, 30);
                                }
                                else
                                {
                                    draw_circle_on_minimap(hero.last_visible_position + new SharpDX.Vector3(-12, -12, 0), radius, System.Drawing.Color.FromArgb(255, 50, 0), 1, 30);
                                }
                            }
                        }

                        DrawingInternal.AddImage(texture_hero, map_pos - new SharpDX.Vector3(12, 12, 0), new SharpDX.Vector3(24, 24, 0), 90f, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.4f, 0.4f, 0.4f, 1), new SharpDX.Vector4(0, 0, 0, 0));

                        if (is_recalling)
                            DrawingInternal.AddCircleOnScreen(map_pos, 12, System.Drawing.Color.FromArgb(180, 255, 255, 0), 1, 90);
                        else
                            DrawingInternal.AddCircleOnScreen(map_pos, 12, System.Drawing.Color.FromArgb(180, 255, 0, 0), 1, 90);
                    }
                }
                else if ((spell_tracker_enemy.GetValue<bool>() && is_enemy) || (spell_tracker_ally.GetValue<bool>() && !is_enemy && !obj.IsMe) || (spell_tracker_me.GetValue<bool>() && obj.IsMe)) /*menu*/
                    draw_hp_bar_obj(obj);
            }
            
            /*hud*/
            if (is_enemy && hud_tracker_enabled.GetValue<bool>())
            {
                var end_pos = base_pos + (champ_size * scale);
                var start_avatar = base_pos + new SharpDX.Vector3(3, 3, 0);
                var end_avatar = start_avatar + (avatar_size * scale);
                var image_size = avatar_image_size * scale;
                var font_size = (int)((float)13 * scale);
                var text_level_size = DrawingInternal.CalcTextSize(font_size, obj.Level.ToString());
                
                /*frame & border*/
                DrawingInternal.AddRect(base_pos, end_pos, System.Drawing.Color.FromArgb(114, 104, 66), 0, -1);
                DrawingInternal.AddFilledRect(base_pos + new SharpDX.Vector3(1, 1, 0), end_pos - new SharpDX.Vector3(1, 1, 0), System.Drawing.Color.FromArgb(12, 24, 21), 0, -1);

                /*champion avatar*/
                DrawingInternal.AddRect(start_avatar, end_avatar, System.Drawing.Color.FromArgb(118, 107, 61), 0, -1);
                DrawingInternal.AddRect(start_avatar + new SharpDX.Vector3(1, 1, 0), end_avatar - new SharpDX.Vector3(1, 1, 0), System.Drawing.Color.FromArgb(118, 107, 61), 0, -1);

                if (obj.IsDead)
                {
                    var dead_timer = Math.Max(0, (int)(hero.respawn_time - Game.Time));
                    var dead_size = DrawingInternal.CalcTextSize(font_size, dead_timer.ToString());
                    DrawingInternal.AddImage(texture_hero, start_avatar + new SharpDX.Vector3(2, 2, 0), image_size, 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.3f, 0.3f, 0.3f, 1), new SharpDX.Vector4(0, 0, 0, 0));
                    var dead_text_draw = new SharpDX.Vector3(start_avatar.X + 2 + (image_size.X / 2f), start_avatar.Y + 2 + (image_size.Y / 2f), 0);
                    DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(dead_text_draw.X - (dead_size.X / 2f), dead_text_draw.Y - (dead_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 0, 0), font_size, dead_timer.ToString());
                }
                else if (!obj.IsVisible)
                {
                    var ss_time = (int)((float)(LeagueSharp.Common.Utils.TickCount - hero.last_visible_tick) / 1000f);
                    var dead_size = DrawingInternal.CalcTextSize(font_size, ss_time.ToString());
                    DrawingInternal.AddImage(texture_hero, start_avatar + new SharpDX.Vector3(2, 2, 0), image_size, 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.4f, 0.4f, 0.4f, 1), new SharpDX.Vector4(0, 0, 0, 0));
                    var dead_text_draw = new SharpDX.Vector3(start_avatar.X + 2 + (image_size.X / 2f), start_avatar.Y + 2 + (image_size.Y / 2f), 0);
                    DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(dead_text_draw.X - (dead_size.X / 2f), dead_text_draw.Y - (dead_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 255, 0), font_size, ss_time.ToString());
                }
                else
                    DrawingInternal.AddImage(texture_hero, start_avatar + new SharpDX.Vector3(2, 2, 0), image_size, 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(1, 1, 1, 1), new SharpDX.Vector4(0, 0, 0, 0));

                DrawingInternal.AddTextOnScreen(start_avatar + new SharpDX.Vector3(2, 2, 0) + image_size - new SharpDX.Vector3(3, 3, 0) - new SharpDX.Vector3(text_level_size.X, text_level_size.Y / 1.2f, 0), System.Drawing.Color.FromArgb(255, 255, 255, 255), font_size, obj.Level.ToString());

                /*healthbars*/
                var hp_bar_size = new SharpDX.Vector3(43, 19, 0);
                var hp_bar_start = base_pos + new SharpDX.Vector3(3, (avatar_size.Y * scale) + 6, 0);
                var hp_bar_end = new SharpDX.Vector3(hp_bar_start.X + (hp_bar_size.X * scale), end_pos.Y - 3, 0);

                float one = ((hp_bar_end.Y - hp_bar_start.Y) - 4) / 2f;

                DrawingInternal.AddRect(hp_bar_start, hp_bar_end, System.Drawing.Color.FromArgb(31, 68, 61), 0, -1);

                /*health*/
                var health_start = hp_bar_start + new SharpDX.Vector3(2, 2, 0);
                var health_end = hp_bar_end - new SharpDX.Vector3(2, 2 + one + 2, 0);
                DrawingInternal.AddFilledRect(health_start, health_end, System.Drawing.Color.FromArgb(19, 19, 19), 0, -1);
                DrawingInternal.AddFilledRect(health_start, new SharpDX.Vector3(health_start.X + ((health_end.X - health_start.X) * (obj.HealthPercent / 100f)), health_end.Y, 0), hp_color((int)obj.HealthPercent), 0, -1);

                /*mana*/
                var mana_start = hp_bar_start + new SharpDX.Vector3(2, 2 + one + 2, 0);
                var mana_end = hp_bar_end - new SharpDX.Vector3(2, 2, 0);
                DrawingInternal.AddFilledRect(mana_start, mana_end, System.Drawing.Color.FromArgb(19, 19, 19), 0, -1);
                DrawingInternal.AddFilledRect(mana_start, new SharpDX.Vector3(mana_start.X + ((mana_end.X - mana_start.X) * (obj.ManaPercent / 100f)), mana_end.Y, 0), System.Drawing.Color.FromArgb(49, 142, 234), 0, -1);

                /*spells*/
                var spell_frame = new SharpDX.Vector3(21, 21, 0);
                var real_size_frame_spell = spell_frame * scale;
                real_size_frame_spell.X = end_pos.X - 3 - (end_avatar.X + 3);

                var sum1 = obj.GetSpell(SpellSlot.Summoner1);
                var sum2 = obj.GetSpell(SpellSlot.Summoner2);

                var cool1 = sum1.CooldownExpiresEx;
                var cool2 = sum2.CooldownExpiresEx;

                var sum1_darw = new SharpDX.Vector3(end_avatar.X + 3, start_avatar.Y, 0);

                if (cool1 > 0f)
                {
                    var text_size = DrawingInternal.CalcTextSize(font_size, ((int)cool1).ToString());
                    DrawingInternal.AddImage(sum1.IconTexture, sum1_darw, real_size_frame_spell, 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.3f, 0.3f, 0.3f, 1), new SharpDX.Vector4(0, 0, 0, 0));

                    var sum1_text_draw = new SharpDX.Vector3(sum1_darw.X + (real_size_frame_spell.X / 2f), sum1_darw.Y + (real_size_frame_spell.Y / 2f), 0);

                    DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(sum1_text_draw.X - (text_size.X / 2f), sum1_text_draw.Y - (text_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 255, 255, 255), font_size, ((int)cool1).ToString());
                }
                else
                    DrawingInternal.AddImage(sum1.IconTexture, sum1_darw, real_size_frame_spell, 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(1, 1, 1, 1), new SharpDX.Vector4(0, 0, 0, 0));

                var sum2_darw = new SharpDX.Vector3(end_avatar.X + 3, start_avatar.Y + 2 + real_size_frame_spell.Y, 0);
                if (cool2 > 0f)
                {
                    var text_size = DrawingInternal.CalcTextSize(font_size, ((int)cool2).ToString());
                    DrawingInternal.AddImage(sum2.IconTexture, sum2_darw, real_size_frame_spell, 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.3f, 0.3f, 0.3f, 1), new SharpDX.Vector4(0, 0, 0, 0));

                    var sum2_text_draw = new SharpDX.Vector3(sum2_darw.X + (real_size_frame_spell.X / 2f), sum2_darw.Y + (real_size_frame_spell.Y / 2f), 0);

                    DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(sum2_text_draw.X - (text_size.X / 2f), sum2_text_draw.Y - (text_size.Y / 3f), 0), System.Drawing.Color.FromArgb(255, 255, 255, 255), font_size, ((int)cool2).ToString());
                }
                else
                    DrawingInternal.AddImage(sum2.IconTexture, sum2_darw, real_size_frame_spell, 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(1, 1, 1, 1), new SharpDX.Vector4(0, 0, 0, 0));

                var r_pos = new SharpDX.Vector3(end_avatar.X + 3, start_avatar.Y + 4 + (real_size_frame_spell.Y * 2f), 0);

                var r_spell = obj.GetSpell(SpellSlot.R);
                var r_cool = r_spell.Level > 0 ? r_spell.CooldownExpiresEx : 1f;
                if (r_cool > 0f)
                {
                    DrawingInternal.AddImage(r_spell.IconTexture, r_pos, new SharpDX.Vector3(real_size_frame_spell.X, hp_bar_end.Y - r_pos.Y, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(0.3f, 0.3f, 0.3f, 1), new SharpDX.Vector4(0, 0, 0, 0));

                    if (r_spell.Level > 0)
                    {
                        var text_size = DrawingInternal.CalcTextSize(font_size, ((int)r_cool).ToString());
                        var rcool_text_draw = new SharpDX.Vector3(r_pos.X + (real_size_frame_spell.X / 2f), r_pos.Y + ((hp_bar_end.Y - r_pos.Y) / 2f), 0);

                        DrawingInternal.AddTextOnScreen(new SharpDX.Vector3(rcool_text_draw.X - (text_size.X / 2f), rcool_text_draw.Y - (text_size.Y / 2.2f), 0), System.Drawing.Color.FromArgb(255, 255, 255, 255), font_size, ((int)r_cool).ToString());
                    }
                }
                else
                    DrawingInternal.AddImage(r_spell.IconTexture, r_pos, new SharpDX.Vector3(real_size_frame_spell.X, hp_bar_end.Y - r_pos.Y, 0), 0, new SharpDX.Vector3(0, 0, 0), new SharpDX.Vector3(1, 1, 1), new SharpDX.Vector4(1, 1, 1, 1), new SharpDX.Vector4(0, 0, 0, 0));

                if (hud_position_mode.GetValue<StringList>().SelectedIndex >= 2)
                    base_pos.X += 10 + one_frame_champ.X;
                else
                    base_pos.Y += 10 + one_frame_champ.Y;
            }
        }
    }

    public void draw_entity_range(Obj_AI_Hero obj)
    {
        var CircleQuality = 120;
        var StepMax = 3.14159265358979323846f * 2.0f * ((float)(CircleQuality) - 1.0f) / (float)(CircleQuality);

        var EnemyPos = obj.Position;
        var AARange = obj.AttackRange + obj.BoundingRadius;
        var DistanceToEnemy = ObjectManager.Player.Distance(obj);
        var IsInRange = DistanceToEnemy <= AARange;
        var LineThickness = (float)(en_thickness.GetValue<Slider>().Value) / 100f;

        List<SharpDX.Vector3> DrawPoints = new List<SharpDX.Vector3>();
        for (var i = 0; i <= CircleQuality; i++)
        {
            var Step = (float)(i) / (float)(CircleQuality) * StepMax;
            DrawPoints.Add(new SharpDX.Vector3(EnemyPos.X + (float)Math.Cos((double)Step) * AARange, EnemyPos.Y + (float)Math.Sin((double)Step) * AARange, EnemyPos.Z));
        }

        var my_pos = ObjectManager.Player.Position;

        for (var i = 1; i <= DrawPoints.Count; i++)
        {
            var Prev = DrawPoints.ElementAt(i - 1);
            var Cur = DrawPoints.ElementAt(i == DrawPoints.Count() ? 0 : i);

            var a_2d = Drawing.WorldToScreen(Prev);
            var b_2d = Drawing.WorldToScreen(Cur);

            if (!is_on_screen(a_2d) || !is_on_screen(b_2d))
                continue;

            if (en_animated.GetValue<bool>() && !IsInRange)
            {
                var AnimRange = 400f;
                var Mid = (Prev + Cur) * 0.5f;
                var Dist = my_pos.Distance(Mid);
                var AnimStep = 1f - Math.Max(Math.Min((Dist / AnimRange) * 0.55f, 1f), 0f);

                DrawingInternal.AddLineOnScreen(a_2d.To3D(), b_2d.To3D(), GetColorFade(System.Drawing.Color.FromArgb(155, 155, 155), System.Drawing.Color.FromArgb(255, 63, 0), AnimStep), LineThickness);
            }
            else
                DrawingInternal.AddLineOnScreen(a_2d.To3D(), b_2d.To3D(), IsInRange ? System.Drawing.Color.FromArgb(255, 63, 0) : System.Drawing.Color.FromArgb(155, 155, 155), LineThickness);
        }
    }
}

