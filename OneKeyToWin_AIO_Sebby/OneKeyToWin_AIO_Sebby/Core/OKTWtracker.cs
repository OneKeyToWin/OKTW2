using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class ChampionInfo
    {
        public Obj_AI_Hero Hero { get; set; }

        public Vector3 LastVisablePos { get; set; }
        public float LastVisableTime { get; set; }
        public Vector3 PredictedPos { get; set; }
        public Vector3 LastWayPoint { get; set; }

        public float StartRecallTime { get; set; }
        public float AbortRecallTime { get; set; }
        public float FinishRecallTime { get; set; }
        public bool IsJungler { get; set; }

        public Render.Sprite NormalSprite;
        public Render.Sprite HudSprite;
        public Render.Sprite MinimapSprite;
        public Render.Sprite SquareSprite;

        public Render.Sprite RSprite;
        public Render.Sprite RSpriteS;
        public Render.Sprite Summoner1Sprite;
        public Render.Sprite Summoner2Sprite;
        public Render.Sprite Summoner1SpriteS;
        public Render.Sprite Summoner2SpriteS;


        public ChampionInfo(Obj_AI_Hero hero)
        {
            var texturePtr = hero.SquareIconPortrait;
            var sum1Tex = hero.GetSpell(SpellSlot.Summoner1).IconTexture;
            var sum2Tex = hero.GetSpell(SpellSlot.Summoner2).IconTexture;
            var rTex = hero.GetSpell(SpellSlot.R).IconTexture;

            Hero = hero;
            if (hero.IsEnemy)
                NormalSprite = ImageLoader.CreateRadrarIcon(hero, System.Drawing.Color.Red);
            else
                NormalSprite = ImageLoader.CreateRadrarIcon(hero, System.Drawing.Color.GreenYellow);

            if (texturePtr != IntPtr.Zero)
            {
                SquareSprite = new Render.Sprite(new BaseTexture(texturePtr), Vector2.Zero);
            }
            else
            {
                SquareSprite = ImageLoader.GetSprite("Default");
            }

            RSprite = ImageLoader.CreateSummonerSprite(hero.GetSpell(SpellSlot.R));
            Summoner1Sprite = ImageLoader.CreateSummonerSprite(hero.GetSpell(SpellSlot.Summoner1));
            Summoner2Sprite = ImageLoader.CreateSummonerSprite(hero.GetSpell(SpellSlot.Summoner2));

            if (sum1Tex != IntPtr.Zero)
            {
                Summoner1SpriteS = new Render.Sprite(new BaseTexture(sum1Tex), Vector2.Zero);
            }
            else
            {
                Summoner1SpriteS = ImageLoader.GetSprite("Default");
            }

            if (sum2Tex != IntPtr.Zero)
            {
                Summoner2SpriteS = new Render.Sprite(new BaseTexture(sum2Tex), Vector2.Zero);
            }
            else
            {
                Summoner2SpriteS = ImageLoader.GetSprite("Default");
            }

            if (rTex != IntPtr.Zero)
            {
                var bmp = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(BaseTexture.ToStream(new BaseTexture(hero.GetSpell(SpellSlot.R).IconTexture), ImageFileFormat.Bmp));

                if (bmp.Width > 64 || bmp.Height > 64)
                    bmp = ImageLoader.ResizeBitmap(bmp, 64, 64);

                RSpriteS = new Render.Sprite(bmp, Vector2.Zero);
            }
            else
            {
                RSpriteS = ImageLoader.GetSprite("Default");
            }

            HudSprite = ImageLoader.CreateRadrarIcon(hero, System.Drawing.Color.DarkGoldenrod, 100);
            MinimapSprite = ImageLoader.CreateMinimapSprite(hero);
            LastVisableTime = Game.Time;
            LastVisablePos = hero.Position;
            PredictedPos = hero.Position;
            IsJungler = hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite"));

            StartRecallTime = 0;
            AbortRecallTime = 0;
            FinishRecallTime = 0;
            Game.OnUpdate += OnUpdate;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
                return;
            NormalSprite.VisibleCondition = sender => !Hero.IsDead;
            HudSprite.VisibleCondition = sender => !Hero.IsDead;
            //MinimapSprite.VisibleCondition = sender => !Hero.IsDead && !Hero.IsVisible;
        }
    }

    class OKTWtracker
    {
        public static List<ChampionInfo> ChampionInfoList = new List<ChampionInfo>();

        private Vector3 EnemySpawn;

        public void LoadOKTW()
        {
            EnemySpawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy).Position;
            foreach (var hero in HeroManager.AllHeroes.Where(x => x.BaseSkinName != "PracticeTool_TargetDummy"))
            {
                ChampionInfoList.Add(new ChampionInfo(hero));
            }

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
        }

        private static void Obj_AI_Base_OnTeleport(GameObject sender, GameObjectTeleportEventArgs args)
        {
            var unit = sender as Obj_AI_Hero;

            if (unit == null || !unit.IsValid || unit.IsAlly)
                return;
            
            var ChampionInfoOne = ChampionInfoList.Find(x => x.Hero.NetworkId == sender.NetworkId);

            var recall = Packet.S2C.Teleport.Decoded(unit, args);

            if (recall.Type == Packet.S2C.Teleport.Type.Recall)
            {
                switch (recall.Status)
                {
                    case Packet.S2C.Teleport.Status.Start:
                        ChampionInfoOne.StartRecallTime = Game.Time;
                        break;
                    case Packet.S2C.Teleport.Status.Abort:
                        ChampionInfoOne.AbortRecallTime = Game.Time;
                        break;
                    case Packet.S2C.Teleport.Status.Finish:
                        ChampionInfoOne.FinishRecallTime = Game.Time;
                        var spawnPos = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy).Position;
                        ChampionInfoOne.LastVisablePos = spawnPos;
                        ChampionInfoOne.PredictedPos = spawnPos;
                        ChampionInfoOne.LastWayPoint = spawnPos;
                        ChampionInfoOne.LastVisableTime = Game.Time;
                        break;
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Program.LagFree(0))
                return;

            foreach (var extra in ChampionInfoList.Where( x => x.Hero.IsEnemy))
            {
                var enemy = extra.Hero;
                if (enemy.IsDead)
                {
                    extra.LastVisablePos = EnemySpawn;
                    extra.LastVisableTime = Game.Time;
                    extra.PredictedPos = EnemySpawn;
                    extra.LastWayPoint = EnemySpawn;
                }
                else if (enemy.IsVisible)
                {
                    extra.LastWayPoint = extra.Hero.GetWaypoints().Last().To3D();
                    extra.PredictedPos = enemy.Position.Extend(extra.LastWayPoint, 125);
                    extra.LastVisablePos = enemy.Position;
                    extra.LastVisableTime = Game.Time;
                }
            }
        }
    }
}
