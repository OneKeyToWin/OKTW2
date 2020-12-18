using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Keras.Models;
using LeagueSharp;
using LeagueSharp.Common;
using Numpy;
using SharpDX;

namespace SebbyLib
{
    public class AIPredictionInput
    {
        private static readonly Dictionary<string, int> ChampionToId = new Dictionary<string, int>
        {
            ["Aatrox"] = 266,
            ["Ahri"] = 103,
            ["Akali"] = 84,
            ["Alistar"] = 12,
            ["Amumu"] = 32,
            ["Anivia"] = 34,
            ["Annie"] = 1,
            ["Aphelios"] = 523,
            ["Ashe"] = 22,
            ["AurelionSol"] = 136,
            ["Azir"] = 268,
            ["Bard"] = 432,
            ["Blitzcrank"] = 53,
            ["Brand"] = 63,
            ["Braum"] = 201,
            ["Caitlyn"] = 51,
            ["Camille"] = 164,
            ["Cassiopeia"] = 69,
            ["Chogath"] = 31,
            ["Corki"] = 42,
            ["Darius"] = 122,
            ["Diana"] = 131,
            ["DrMundo"] = 36,
            ["Draven"] = 119,
            ["Ekko"] = 245,
            ["Elise"] = 60,
            ["Evelynn"] = 28,
            ["Ezreal"] = 81,
            ["FiddleSticks"] = 9,
            ["Fiora"] = 114,
            ["Fizz"] = 105,
            ["Galio"] = 3,
            ["Gangplank"] = 41,
            ["Garen"] = 86,
            ["Gnar"] = 150,
            ["Gragas"] = 79,
            ["Graves"] = 104,
            ["Hecarim"] = 120,
            ["Heimerdinger"] = 74,
            ["Illaoi"] = 420,
            ["Irelia"] = 39,
            ["Ivern"] = 427,
            ["Janna"] = 40,
            ["JarvanIV"] = 59,
            ["Jax"] = 24,
            ["Jayce"] = 126,
            ["Jhin"] = 202,
            ["Jinx"] = 222,
            ["Kaisa"] = 145,
            ["Kalista"] = 429,
            ["Karma"] = 43,
            ["Karthus"] = 30,
            ["Kassadin"] = 38,
            ["Katarina"] = 55,
            ["Kayle"] = 10,
            ["Kayn"] = 141,
            ["Kennen"] = 85,
            ["Khazix"] = 121,
            ["Kindred"] = 203,
            ["Kled"] = 240,
            ["KogMaw"] = 96,
            ["Leblanc"] = 7,
            ["LeeSin"] = 64,
            ["Leona"] = 89,
            ["Lillia"] = 876,
            ["Lissandra"] = 127,
            ["Lucian"] = 236,
            ["Lulu"] = 117,
            ["Lux"] = 99,
            ["Malphite"] = 54,
            ["Malzahar"] = 90,
            ["Maokai"] = 57,
            ["MasterYi"] = 11,
            ["MissFortune"] = 21,
            ["Mordekaiser"] = 82,
            ["Morgana"] = 25,
            ["Nami"] = 267,
            ["Nasus"] = 75,
            ["Nautilus"] = 111,
            ["Neeko"] = 518,
            ["Nidalee"] = 76,
            ["Nocturne"] = 56,
            ["Nunu"] = 20,
            ["Olaf"] = 2,
            ["Orianna"] = 61,
            ["Ornn"] = 516,
            ["Pantheon"] = 80,
            ["Poppy"] = 78,
            ["Pyke"] = 555,
            ["Qiyana"] = 246,
            ["Quinn"] = 133,
            ["Rakan"] = 497,
            ["Rammus"] = 33,
            ["RekSai"] = 421,
            ["Renekton"] = 58,
            ["Rengar"] = 107,
            ["Riven"] = 92,
            ["Rumble"] = 68,
            ["Ryze"] = 13,
            ["Samira"] = 360,
            ["Sejuani"] = 113,
            ["Senna"] = 235,
            ["Seraphine"] = 147,
            ["Sett"] = 875,
            ["Shaco"] = 35,
            ["Shen"] = 98,
            ["Shyvana"] = 102,
            ["Singed"] = 27,
            ["Sion"] = 14,
            ["Sivir"] = 15,
            ["Skarner"] = 72,
            ["Sona"] = 37,
            ["Soraka"] = 16,
            ["Swain"] = 50,
            ["Sylas"] = 517,
            ["Syndra"] = 134,
            ["TahmKench"] = 223,
            ["Taliyah"] = 163,
            ["Talon"] = 91,
            ["Taric"] = 44,
            ["Teemo"] = 17,
            ["Thresh"] = 412,
            ["Tristana"] = 18,
            ["Trundle"] = 48,
            ["Tryndamere"] = 23,
            ["TwistedFate"] = 4,
            ["Twitch"] = 29,
            ["Udyr"] = 77,
            ["Urgot"] = 6,
            ["Varus"] = 110,
            ["Vayne"] = 67,
            ["Veigar"] = 45,
            ["Velkoz"] = 161,
            ["Vi"] = 254,
            ["Viktor"] = 112,
            ["Vladimir"] = 8,
            ["Volibear"] = 106,
            ["Warwick"] = 19,
            ["MonkeyKing"] = 62,
            ["Xayah"] = 498,
            ["Xerath"] = 101,
            ["XinZhao"] = 5,
            ["Yasuo"] = 157,
            ["Yone"] = 777,
            ["Yorick"] = 83,
            ["Yuumi"] = 350,
            ["Zac"] = 154,
            ["Zed"] = 238,
            ["Ziggs"] = 115,
            ["Zilean"] = 26,
            ["Zoe"] = 142,
            ["Zyra"] = 143,
        };

        private static int[] BuffTypes = new[]
        {
            1,
            3,
            26,
            2,
            10,
            13,
            14,
            12,
            5,
            29,
            27,
            30,
            21,
            28,
            11,
            22,
            17,
            19,
            4,
            15,
            9,
            31,
            7,
            34,
            18,
            32,
            23,
            24,
            6,
            33,
            8,
            25,
            20
        };

        public float Delay;
        public Obj_AI_Hero Source;
        public int SourceSpellSlot;
        public Obj_AI_Hero Target;

        public static float SpeedFromVelocity(Vector3 velocity)
        {
            var realVelocity = new Vector3
            {
                X = velocity.X * 20,
                Y = velocity.Y * 20,
                Z = velocity.Z * 20
            };

            return new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0
            }.Distance(realVelocity);
        }

        private void PutSpellData(ICollection<float> target, SpellDataInst spell)
        {
            target.Add(spell.Level);
            target.Add(spell.CooldownExpiresEx);
        }

        private void PutHistoryPath(ICollection<float> target, OnNewPathEvent @event, Obj_AI_Hero hero)
        {
            var pathLength = @event.Path.Select(vector => vector.To2D()).ToList().PathLength();
            target.Add(pathLength); // length
            target.Add(pathLength == 0 ? 0.0f : Source.Direction.To2D().AngleBetween(@event.Path[0].To2D()));
            target.Add((float) (Game.TimePrec - @event.GameTime)); // time ago
            target.Add(@event.Path.Length > 0 ? hero.Distance(@event.Path[0]) : 0.0f);
        }

        private void PutBuffData(ICollection<float> target, BuffInstance[] buffs)
        {
            var gameTime = Game.TimePrec;
            foreach (var buffType in BuffTypes)
            {
                var maxBuff = buffs.Where(buff => (BuffType) buffType == buff.Type)
                    .OrderByDescending(buff => buff.EndTime - gameTime).FirstOrDefault();

                if (maxBuff != null)
                {
                    target.Add((float) (maxBuff.EndTime - gameTime));
                }
                else
                {
                    target.Add(0.0f);
                }
            }
        }

        public float[] GetValues()
        {
            var values = new List<float>();
            values.Add(ChampionToId[Source.ChampionName]); // source champ
            values.Add(ChampionToId[Target.ChampionName]); // target champ
            values.Add(SourceSpellSlot); // source spell slot
            values.Add(Target.Direction.X); // target direction x
            values.Add(Target.Direction.Y); // target direction y
            values.Add(Source.Direction.X); // source direction x
            values.Add(Source.Direction.Y); // source direction y
            values.Add(Delay); // delay
            values.Add(SpeedFromVelocity(Target.Velocity)); // speed
            values.Add(Delay * SpeedFromVelocity(Target.Velocity)); // move area
            values.Add((float) Game.TimePrec); // game time
            values.Add(Target.IsDashing() ? 1.0f : -1.0f); // is dash
            values.Add(Target.Health / Target.MaxHealth); // health
            values.Add(Target.Path.Select(vector => vector.To2D()).ToList().PathLength()); // current path length
            values.Add(Target.Direction.To2D()
                .AngleBetween(Source.Direction.To2D())); // angle_between_last_path_and_position
            values.Add(Source.ServerPosition.Distance(Target.ServerPosition));
            values.Add(Target.Mana);

            PutSpellData(values, Target.GetSpell(SpellSlot.Q));
            PutSpellData(values, Target.GetSpell(SpellSlot.W));
            PutSpellData(values, Target.GetSpell(SpellSlot.E));
            PutSpellData(values, Target.GetSpell(SpellSlot.R));
            PutSpellData(values, Target.GetSpell(SpellSlot.Summoner1));
            PutSpellData(values, Target.GetSpell(SpellSlot.Summoner2));

            var amountOfPaths = 0;

            try
            {
                var lastPathes = OktwCommon.LastNewPathes[Target.NetworkId];
                amountOfPaths = lastPathes.Count;
                IEnumerable<OnNewPathEvent> lastNewPathesEnumerable = lastPathes;

                foreach (var path in lastNewPathesEnumerable.Reverse().Take(10))
                {
                    PutHistoryPath(values, path, Target);
                }
            }
            catch (KeyNotFoundException)
            {
            }

            for (var i = 0; i < 10 - amountOfPaths; i++)
            {
                values.Add(0.0f);
                values.Add(0.0f);
                values.Add(0.0f);
                values.Add(0.0f);
            }

            PutBuffData(values, Target.Buffs);

            return values.ToArray();
        }
    }

    public class AIPredictionOutput
    {
        public float Hitchance;
    }

    public class AIPrediction
    {
        private static readonly BaseModel Model;

        static AIPrediction()
        {
            Model = BaseModel.ModelFromJson(File.ReadAllText(@"c://test/model.json"));
            Model.LoadWeight("c://test/model.h5");
        }

        public static AIPredictionOutput GetPrediction(AIPredictionInput input)
        {
            var values = input.GetValues();

            var firstLevel = new float[1, 102];
            for (var i = 0; i < 102; i++)
            {
                // Console.Write(values[i] + ", ");
                firstLevel[0, i] = values[i];
            }

            // Console.WriteLine();

            var x = np.array(firstLevel);

            var output = Model.Predict(new List<NDarray> {x}, verbose: 0);

            return new AIPredictionOutput
            {
                Hitchance = output[0].GetData<float>()[0]
            };
        }
    }
}