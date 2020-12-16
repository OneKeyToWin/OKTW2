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

        public float Delay;
        public Obj_AI_Hero Source;
        public int SourceSpellSlot;
        public Obj_AI_Hero Target;
        public float MoveArea;
        public float GameTime;
        public bool IsDash;
        public float Health;
        public float LastPathTime;
        public float CurrentPathLength;
        public float AngleBetweenLastPathAndPosition;
        public float LengthFromCasterToUnit;

        private static float SpeedFromVelocity(Vector3 velocity)
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

        private void PutHistoryPath(ICollection<float> target, Vector3[] path)
        {
            target.Add(path.Select(vector => new Vector2
            {
                X = vector.X,
                Y = vector.Y
            }).ToList().PathLength()); // length
            target.Add(path.Length == 0 ? 0.0f : Source.Direction.To2D().AngleBetween(path[0].To2D()));
            target.Add(Game.Time - 0.0f); // TODO: time ago
        }

        private void PutBuffData(ICollection<float> target, BuffInstance[] buffs)
        {
            var gameTime = Game.TimePrec;
            foreach (var buffType in (BuffType[]) Enum.GetValues(typeof(BuffType)))
            {
                var maxBuff = buffs.Where(buff => buffType == buff.Type)
                    .OrderByDescending(buff => gameTime - buff.EndTime).First();

                if (maxBuff != null)
                {
                    target.Add((float) gameTime - maxBuff.EndTime);
                }
                else
                {
                    target.Add(0.0f);
                }
            }
        }

        public float[] GetValues()
        {
            var pathLength = Target.Path.Select(path => new Vector2
            {
                X = path.X,
                Y = path.Y
            }).ToList().PathLength();

            var values = new List<float>();
            values.Add(ChampionToId[Source.ChampionName]); // source champ
            values.Add(ChampionToId[Target.ChampionName]); // target champ
            values.Add(SourceSpellSlot); // source spell slot
            values.Add(Target.Direction.X); // target direction x
            values.Add(Target.Direction.Y); // target direction y
            values.Add(Target.Direction.Z); // target direction z
            values.Add(Delay); // delay
            values.Add(SpeedFromVelocity(Target.Velocity)); // speed
            values.Add(Delay * SpeedFromVelocity(Target.Velocity)); // move area
            values.Add((float) Game.TimePrec); // game time
            values.Add(-1.0f); // is dash TODO
            values.Add(Target.Health / Target.MaxHealth); // health
            values.Add(0.0f); // last path time TODO
            values.Add(pathLength); // current path length
            values.Add(Target.Direction.To2D()
                .AngleBetween(Source.ServerPosition.To2D())); // angle_between_last_path_and_position
            values.Add(Target.ServerPosition.Distance(Source.ServerPosition));

            PutSpellData(values, Target.GetSpell(SpellSlot.Q));
            PutSpellData(values, Target.GetSpell(SpellSlot.W));
            PutSpellData(values, Target.GetSpell(SpellSlot.E));
            PutSpellData(values, Target.GetSpell(SpellSlot.R));
            PutSpellData(values, Target.GetSpell(SpellSlot.Summoner1));
            PutSpellData(values, Target.GetSpell(SpellSlot.Summoner2));
            PutHistoryPath(values, Target.Path);
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

        public AIPredictionOutput GetPrediction(AIPredictionInput input)
        {
            NDarray x = np.array(new[,]
            {
                {
                    4.00000000e+00f, 1.31000000e+02f, 0.00000000e+00f, 4.63506880e-02f,
                    0.00000000e+00f, 9.98925100e-01f, 5.95336914e-01f, 0.00000000e+00f,
                    0.00000000e+00f, 7.75109863e+02f, -1.00000000e+00f, 9.72130300e-01f,
                    7.74878784e+02f, 0.00000000e+00f, 4.08477480e+01f, 6.68253400e+02f,
                    5.00000000e+00f, 3.00000000e+00f, 0.00000000e+00f, 3.00000000e+00f,
                    8.40000000e+01f, 5.00000000e+00f, 1.79626460e+00f, 2.00000000e+00f, 0.00000000e+00f,
                    1.00000000e+00f, 0.00000000e+00f, 1.00000000e+00f, 0.00000000e+00f, 1.00000000e+00f,
                    1.96012820e+02f, 1.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 1.36349780e+02f,
                    2.31079102e-01f, 0.00000000e+00f, 1.36349780e+02f, 2.64099121e-01f, 8.62927550e+02f,
                    1.37484570e+02f, 8.58459473e-01f, 8.48942900e+02f, 1.37812590e+02f, 1.02355957e+00f,
                    4.87372070e+02f, 1.38885160e+02f, 1.48681641e+00f, 6.67186300e+02f, 1.39394850e+02f,
                    1.65191650e+00f, 7.09529400e+02f, 1.41762190e+02f, 2.44433594e+00f, 7.38335940e+02f,
                    1.39717530e+02f, 2.97460938e+00f, 3.18056600e+02f, 1.40078920e+02f, 3.27178955e+00f,
                    9.95389400e+01f, 1.40255980e+02f, 3.43688965e+00f, 2.48040060e+04f, 1.24202880e+01f,
                    0.00000000e+00f, 2.28680420e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f,
                    0.00000000e+00f, 1.07281500e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f,
                    0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f,
                    0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f,
                    0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f,
                    0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f, 0.00000000e+00f,
                    0.00000000e+00f
                }
            });

            var output = Model.Predict(new List<NDarray> {x}, verbose: 0);

            return new AIPredictionOutput
            {
                Hitchance = output[0].GetData<float>()[0]
            };
        }
    }
}