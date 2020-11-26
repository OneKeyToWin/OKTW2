using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class HiddenObj
    {
        public int type;
        //0 - missile
        //1 - normal
        //2 - pink
        //3 - teemo trap
        public float endTime { get; set; }
        public Vector3 pos { get; set; }
    }

    class OKTWward : Program
    {
        private bool rengar = false;
        Obj_AI_Hero Vayne = null;

        public static List<HiddenObj> HiddenObjList = new List<HiddenObj>();

        private Items.Item
            VisionWard = new Items.Item(2055, 550f),
            OracleLens = new Items.Item(3364, 550f),
            TrinketN = new Items.Item(3340, 600f),
            SightStone = new Items.Item(3851, 600f),
            WardN = new Items.Item(3853, 600f),
            RunesteelSpaulders = new Items.Item(3855, 600f),
            PauldronsofWhiterock = new Items.Item(3857, 600f),
            EOTOasis = new Items.Item(3859, 600f),
            EOTEquinox = new Items.Item(3860, 600f),
            HarrowingCrescent = new Items.Item(3863, 600f),
            BlackMistScythe = new Items.Item(3864, 600f),
            FarsightOrb = new Items.Item(3363, 4000f);

        public void LoadOKTW()
        {
            Config.SubMenu("AutoWard OKTW©").AddItem(new MenuItem("AutoWard", "Auto Ward").SetValue(true));
            //Config.SubMenu("AutoWard OKTW©").AddItem(new MenuItem("autoBuy", "Auto buy blue trinket after lvl 9").SetValue(false)); Not working
            Config.SubMenu("AutoWard OKTW©").AddItem(new MenuItem("AutoWardBlue", "Auto Blue Trinket").SetValue(true));
            Config.SubMenu("AutoWard OKTW©").AddItem(new MenuItem("AutoWardCombo", "Only combo mode").SetValue(true));
            Config.SubMenu("AutoWard OKTW©").AddItem(new MenuItem("AutoWardPink", "Auto VisionWard, OracleLens").SetValue(true));

            foreach (var hero in HeroManager.Enemies)
            {
                if (hero.ChampionName == "Rengar")
                    rengar = true;
                if (hero.ChampionName == "Vayne")
                    Vayne = hero;
            }
            
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate +=GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!Program.LagFree(0) || Player.IsRecalling() || Player.IsDead)
                return;

            foreach (var obj in HiddenObjList)
            {
                if (obj.endTime < Game.Time)
                {
                    HiddenObjList.Remove(obj);
                    return;
                }
            }

            //if (Config.Item("autoBuy").GetValue<bool>() && Player.InFountain() && !FarsightOrb.IsOwned() && Player.Level >= 9 && MenuGUI.IsShopOpen)
            //    Player.BuyItem(ItemId.Farsight_Orb_Trinket);

            if(rengar && Player.HasBuff("rengarralertsound"))
                CastVisionWards(Player.ServerPosition);
            
            AutoWardLogic();
        }

        private void AutoWardLogic()
        {
            foreach (var need in OKTWtracker.ChampionInfoList.Where(x => x.Hero.IsValid && x.PredictedPos != null && !x.Hero.IsVisible && !x.Hero.IsDead))
            {
                //var need = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);

                var PPDistance = need.PredictedPos.Distance(Player.Position);

                if(PPDistance > 1400)
                    continue;

                var timer = Game.Time - need.LastVisableTime;

                if (timer > 1 && timer < 3 && AioModeSet != AioMode.UtilityOnly)
                {
                    if (Program.Combo && PPDistance < 1500 && Player.ChampionName == "Quinn" && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
                    {
                        W.Cast();
                    }

                    if (Program.Combo && PPDistance < 900 && Player.ChampionName == "Karhus" && Q.IsReady() && Player.CountEnemiesInRange(900) == 0)
                    {
                        Q.Cast(need.PredictedPos);
                    }

                    if (Program.Combo && PPDistance < 1400 && Player.ChampionName == "Ashe" && E.IsReady() && Player.CountEnemiesInRange(800) == 0 && Config.Item("autoE", true).GetValue<bool>())
                    {
                        E.Cast(Player.Position.Extend(need.PredictedPos, 5000));
                    }

                    if (PPDistance < 800 && Player.ChampionName == "MissFortune" && E.IsReady() && Program.Combo && Player.Mana > 200)
                    {
                        E.Cast(Player.Position.Extend(need.PredictedPos, 800));
                    }

                    if (Player.ChampionName == "Caitlyn" && !Player.IsWindingUp && PPDistance < 800 && W.IsReady() && Player.Mana > 200f && Config.Item("bushW", true).GetValue<bool>() && Utils.TickCount - W.LastCastAttemptT > 2000)
                    {
                        W.Cast(need.PredictedPos);
                    }
                    if (Player.ChampionName == "Teemo" && !Player.IsWindingUp &&  PPDistance < 150 + R.Level * 250 && R.IsReady() && Player.Mana > 200f && Config.Item("bushR", true).GetValue<bool>() && Utils.TickCount - W.LastCastAttemptT > 2000)
                    {
                        R.Cast(need.PredictedPos);
                    }
                    if (Player.ChampionName == "Jhin" && !Player.IsWindingUp && PPDistance < 760 && E.IsReady() && Player.Mana > 200f && Config.Item("bushE", true).GetValue<bool>() && Utils.TickCount - E.LastCastAttemptT > 2000)
                    {
                        E.Cast(need.PredictedPos);
                    }
                }

                if (timer < 4)
                {
                    if (Config.Item("AutoWardCombo").GetValue<bool>() && Program.AioModeSet != Program.AioMode.ChampionOnly && !Program.Combo)
                        return;

                    if (NavMesh.IsWallOfGrass(need.PredictedPos, 0) ||
                        NavMesh.IsWallOfGrass(need.Hero.Position, 0))
                    {
                        if (PPDistance < 600 && Config.Item("AutoWard").GetValue<bool>())
                        {
                            if (TrinketN.IsReady())
                            {
                                TrinketN.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (SightStone.IsReady())
                            {
                                SightStone.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (WardN.IsReady())
                            {
                                WardN.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (EOTOasis.IsReady())
                            {
                                EOTOasis.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (EOTEquinox.IsReady())
                            {
                                EOTEquinox.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (RunesteelSpaulders.IsReady())
                            {
                                RunesteelSpaulders.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (PauldronsofWhiterock.IsReady())
                            {
                                PauldronsofWhiterock.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (HarrowingCrescent.IsReady())
                            {
                                HarrowingCrescent.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (BlackMistScythe.IsReady())
                            {
                                BlackMistScythe.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                        }

                        if (Config.Item("AutoWardBlue").GetValue<bool>())
                        {
                            if (FarsightOrb.IsReady())
                            {
                                FarsightOrb.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                        }
                    }
                }
            } 
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsEnemy || sender.IsAlly )
                return;
            var missile = sender as MissileClient;
            if (missile != null)
            {
                if (missile.SpellCaster != null && !missile.SpellCaster.IsVisible)
                {
                    if ((missile.SData.Name == "BantamTrapShort" || missile.SData.Name == "BantamTrapBounceSpell") && !HiddenObjList.Exists(x => missile.EndPosition == x.pos))
                        AddWard("teemorcast", missile.EndPosition);
                }
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                if ((sender.Name.ToLower() == "jammerdevice" || sender.Name.ToLower() == "sightward") && !HiddenObjList.Exists(x => x.pos.Distance(sender.Position) < 100))
                {
                    foreach (var obj in HiddenObjList)
                    {
                        if (obj.pos.Distance(sender.Position) < 400)
                        {
                            if (obj.type == 0)
                            {
                                HiddenObjList.Remove(obj);
                                return;
                            }
                        }
                    }

                    var dupa = (Obj_AI_Minion)sender;
                    if (dupa.Mana == 0)
                        HiddenObjList.Add(new HiddenObj() { type = 2, pos = sender.Position, endTime = float.MaxValue });
                    else
                        HiddenObjList.Add(new HiddenObj() { type = 1, pos = sender.Position, endTime = Game.Time + dupa.Mana });
                }
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && minion.MaxHealth < 100)
            {

                foreach (var obj in HiddenObjList)
                {
                    if (obj.pos == sender.Position)
                    {
                        HiddenObjList.Remove(obj);
                        return;
                    }
                    else if (obj.type == 3 && obj.pos.Distance(sender.Position) < 100)
                    {
                        HiddenObjList.Remove(obj);
                        return;
                    }
                    else if (obj.pos.Distance(sender.Position) < 400 && minion.MaxHealth < 5)
                    {
                        if (obj.type == 2 )
                        {
                            HiddenObjList.Remove(obj);
                            return;
                        }
                        else if ((obj.type == 0 || obj.type == 1) && sender.Name.ToLower() == "sightward")
                        {
                            HiddenObjList.Remove(obj);
                            return;
                        }
                    }
                }
            }
        }
       
        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is Obj_AI_Hero && sender.IsEnemy)
            {
                if(args.Target == null)
                    AddWard(args.SData.Name.ToLower(), args.End);

                if ((OracleLens.IsReady() || VisionWard.IsReady()) && sender.Distance(Player.Position) < 1200)
                {
                    switch (args.SData.Name.ToLower())
                    {
                        case "pykew":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "sennae":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "rengarr":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "twitchhideinshadows":
                            CastVisionWards(sender.ServerPosition);
                            break;
                    }
                }
            }
        }

        private void AddWard(string name, Vector3 posCast)
        {
            switch (name)
            {
                //PINKS
                case "jammerdevice":
                    HiddenObjList.Add(new HiddenObj() { type = 2, pos = posCast, endTime = float.MaxValue });
                    break;
                case "trinkettotemlvl3B":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                //SIGH WARD
                case "itemghostward":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 150 });
                    break;
                case "wrigglelantern":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 150 });
                    break;
                case "sightward":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 150 });
                    break;
                case "itemferalflare":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 150 });
                    break;
                //TRINKET
                case "trinkettotemlvl1":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 60 + Player.Level * 3.3f });
                    break;
                case "trinkettotemlvl2":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 120 });
                    break;
                case "trinkettotemlvl3":
                    HiddenObjList.Add(new HiddenObj() { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                //others
                case "teemorcast":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 300 });
                    break;
                case "noxious trap":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 300 });
                    break;
                case "JackInTheBox":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 100 });
                    break;
                case "Jack In The Box":
                    HiddenObjList.Add(new HiddenObj() { type = 3, pos = posCast, endTime = Game.Time + 100 });
                    break;
            }
        }

        private void CastVisionWards(Vector3 position)
        {
            if (Config.Item("AutoWardPink").GetValue<bool>())
            {
                if (OracleLens.IsReady())
                    OracleLens.Cast(Player.Position.Extend(position, OracleLens.Range));
                else if (VisionWard.IsReady())
                    VisionWard.Cast(Player.Position.Extend(position, VisionWard.Range));
            }
        }
    }
}
