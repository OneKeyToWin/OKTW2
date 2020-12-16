using System;
using System.Collections.Generic;
using System.IO;
using Keras.Models;
using LeagueSharp;
using Numpy;

namespace SebbyLib
{
    public class AIPredictionInput
    {
        public float Delay;
        public Obj_AI_Hero Source;
        public Obj_AI_Hero Target;
        public float MoveArea;
        public float GameTime;
        public bool IsDash;
        public float Health;
        public float LastPathTime;
        public float CurrentPathLength;
        public float AngleBetweenLastPathAndPosition;
        public float LengthFromCasterToUnit;
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