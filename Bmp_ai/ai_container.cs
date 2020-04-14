using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Bmp_ai
{
    class NeiroWeb
    {

        public int countTrainig = 0;
        public const int neironInArrayWidth = 50;
        public const int neironInArrayHeight = 50;
        private const string memory = "train_bmp_ai_memory.txt";
        private List<Neiron> neironArray = null;

        public NeiroWeb()
        {
            neironArray = InitWeb();
        }
        private static List<Neiron> InitWeb()
        {
            if (!File.Exists(memory)) return new List<Neiron>();
            string[] lines = File.ReadAllLines(memory);
            if (lines.Length == 0) return new List<Neiron>();
            string jStr = lines[0];
            JavaScriptSerializer json = new JavaScriptSerializer();
            List<Object> objects = json.Deserialize<List<Object>>(jStr);
            List<Neiron> res = new List<Neiron>();
            foreach (var o in objects) res.Add(NeironCreate((Dictionary<string, Object>)o));
            return res;
        }

        private static Neiron NeironCreate(Dictionary<string, object> o)
        {
            Neiron res = new Neiron
            {
                name = (string)o["name"],
                countTrainig = (int)o["countTrainig"]
            };
            Object[] veightData = (Object[])o["veight"];
            int arrSize = (int)Math.Sqrt(veightData.Length);
            res.veight = new double[arrSize, arrSize];
            int index = 0;
            for (int n = 0; n < res.veight.GetLength(0); n++)
                for (int m = 0; m < res.veight.GetLength(1); m++)
                {
                    res.veight[n, m] = Double.Parse(veightData[index].ToString());
                    index++;
                }
            return res;
        }

        public string CheckLitera(int[,] arr)
        {
            string res = null;
            double max = 0;
            foreach (var n in neironArray)
            {
                double d = n.GetRes(arr);
                if (d > max)
                {
                    max = d;
                    res = n.GetName();
                }
            }
            return res;
        }

        public void SaveState()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            string jStr = json.Serialize(neironArray);
            System.IO.StreamWriter file = new System.IO.StreamWriter(memory);
            file.WriteLine(jStr);
            file.Close();
        }

        public string[] GetLiteras()
        {
            var res = new List<string>();
            for (int i = 0; i < neironArray.Count; i++) res.Add(neironArray[i].GetName());
            res.Sort();

            return res.ToArray();
        }

        public void SetTraining(string trainingName, int[,] data)
        {
            Neiron neiron = neironArray.Find(v => v.name.Equals(trainingName));
            if (neiron == null)
            {
                neiron = new Neiron();
                neiron.Clear(trainingName, neironInArrayWidth, neironInArrayHeight);
                neironArray.Add(neiron);
            }
            countTrainig = neiron.Training(data);
        }
    }
}
