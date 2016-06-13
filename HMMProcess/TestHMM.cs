using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace   HMMProcess
{
    class TestHMM
    {
        StreamWriter swlog;
        StreamWriter sw;
        StreamWriter swFra;

        enum Activity { Shopping,Eating,Transportation, work, Social, home, study, Recreation,Entertainment };  // 隐藏状态（活动）,9类
        string ResultdirectoryPath = @"D:\\201604_CMProcess\\HMMResult\\VersionTest6";
        string directoryPath = @"D:\\201604_CMProcess\\HMMResult\\VersionTest6\\Log";//定义一个路径变量
        HMMClass hmm;
        static int[] FractionOne=new int[3];
        static int[] FractionTwo=new int[7];
        static int[] FractionThree=new int[11];


        public TestHMM(int n,Double[,] A, Dictionary<int, double[]> B, Double[,] PI)
        {

            hmm = new HMMClass(9);
            hmm.A = A;
            hmm.B = B;
            hmm.PI = PI;
            string filePath = "HMMResult_" + n + ".txt";//定义一个文件路径变量

            if (!Directory.Exists(ResultdirectoryPath))//如果路径不存在
            {
                Directory.CreateDirectory(ResultdirectoryPath);//创建一个路径的文件夹
            }
            //续写文件
            sw = new StreamWriter(Path.Combine(ResultdirectoryPath, filePath));

            string filePath2 = "HMMResult_log_" + n + ".txt";//定义一个文件路径变量
            if (!Directory.Exists(directoryPath))//如果路径不存在
            {
                Directory.CreateDirectory(directoryPath);//创建一个路径的文件夹
            }
            //续写log文件
            swlog = new StreamWriter(Path.Combine(directoryPath, filePath2), false);


        }

        public void CheckViterbi(CellTra[] Obeservation,List<int> userhwinfo,int n)
        {
            //一条用户停留轨迹，先判断它的唯一活动位置数


            
            //Console.WriteLine(Obeservation[0].userid);
            swlog.WriteLine(Obeservation[0].userid + "\t");
            swlog.WriteLine(Obeservation.Count() + "\t");


            // 找出最有可能的隐藏状态序列
            Double Probability;
            Double[,] DELTA;

           // Console.WriteLine("------------维特比算法：双精度运算-----------------");
            //活动结果序列
            Int32[] Q;
            //Home 和work至少一个不为0
            if (userhwinfo[0] != 0 || userhwinfo[1] != 0)
                Q = hmm.Viterbi(Obeservation, out DELTA, out Probability, userhwinfo);
            else
            {
                List<int> HWInfo = new List<int> { 0,0};
                Q = hmm.Viterbi(Obeservation, out DELTA, out Probability, HWInfo);
            }
                 //Q = hmm.Viterbi(Obeservation, out DELTA, out Probability);

            swlog.WriteLine(Probability.ToString("0.###E+0"));

            var qq = from w in Obeservation select w.cellid;
            int diffloc = qq.Distinct().Count();
            //初步统计
            //if (diffloc == 1)
            //{
            //    FractionOne[0]++;
            //    if (Q[0] == 5)
            //        FractionOne[1]++;
            //    else
            //        FractionOne[2]++;
            //}
            //else if (diffloc == 2)
            //{
            //    FractionTwo[0]++;
            //    if(Q[0]==5&&Q[1]==3)
            //}
            //else 
            if (diffloc == 3)
            {
                FractionThree[0]++;

                Dictionary<int, int> temp = new Dictionary<int, int>();
                bool index = false;
                for (int t = 0; t < Q.Count(); t++)
                {
                    sw.WriteLine(Obeservation[t].userid + "\t" + ((Activity)Q[t]).ToString() + "\t" + Obeservation[t].intimeindex + "\t" + Obeservation[t].outtimeindex + "\t" + Obeservation[t].NumST + "\t" + Obeservation[t].cellid);

                    if (!temp.ContainsKey(Obeservation[t].cellid))
                    {
                        temp.Add(Obeservation[t].cellid,Q[t]);
                    }
                    else
                    {
                        if(temp[Obeservation[t].cellid]!=Q[t])
                        {
                            index = true;
                        }
                    }
                }

                int Hcnt = temp.Values.Count(x => x.ToString().Contains("5"));
                int Wcnt= temp.Values.Count(x => x.ToString().Contains("3"));
                if (Hcnt == 1 && Wcnt == 1)
                    FractionThree[1]++;
                else if (Hcnt == 2 && Wcnt == 1)
                    FractionThree[2]++;
                else if (Hcnt == 1 && Wcnt == 2)
                    FractionThree[3]++;
                else if (Hcnt == 1 && Wcnt == 0)
                    FractionThree[4]++;
                else if (Hcnt == 0 && Wcnt == 2)
                    FractionThree[5]++;
                else if (Hcnt == 0 && Wcnt == 1)
                    FractionThree[6]++;
                else if (Hcnt == 0 && Wcnt == 0)
                    FractionThree[7]++;
                else if (Hcnt == 3)
                    FractionThree[8]++;
                else if (Wcnt == 3)
                    FractionThree[9]++;
                else if (Hcnt == 2 && Wcnt == 0)
                    FractionThree[10]++;
            }
            else if (diffloc == 2)
            {
                FractionTwo[0]++;

                Dictionary<int, int> temp = new Dictionary<int, int>();
                bool index = false;
                for (int t = 0; t < Q.Count(); t++)
                {
                    sw.WriteLine(Obeservation[t].userid + "\t" + ((Activity)Q[t]).ToString() + "\t" + Obeservation[t].intimeindex + "\t" + Obeservation[t].outtimeindex + "\t" + Obeservation[t].NumST + "\t" + Obeservation[t].cellid);

                    if (!temp.ContainsKey(Obeservation[t].cellid))
                    {
                        temp.Add(Obeservation[t].cellid, Q[t]);
                    }
                    else
                    {
                        if (temp[Obeservation[t].cellid] != Q[t])
                        {
                            index = true;
                        }
                    }
                }

                int Hcnt = temp.Values.Count(x => x.ToString().Contains("5"));
                int Wcnt = temp.Values.Count(x => x.ToString().Contains("3"));
                if (Hcnt == 1 && Wcnt == 1)
                    FractionTwo[1]++;
                else if (Hcnt == 1 && Wcnt == 0)
                    FractionTwo[2]++;
                else if (Hcnt == 0 && Wcnt == 1)
                    FractionTwo[3]++;
                else if (Hcnt == 2 && Wcnt == 0)
                    FractionTwo[4]++;
                else if (Hcnt == 0 && Wcnt == 2)
                    FractionTwo[5]++;
                else if (Hcnt == 0 && Wcnt == 0)
                    FractionTwo[6]++;
            }
            else
            {
                for (int t = 0; t < Q.Count(); t++)
                {
                    sw.WriteLine(Obeservation[t].userid + "\t" + ((Activity)Q[t]).ToString() + "\t" + Obeservation[t].intimeindex + "\t" + Obeservation[t].outtimeindex + "\t" + Obeservation[t].NumST + "\t" + Obeservation[t].cellid);
                    // Console.WriteLine(((Activity)Q[t]).ToString());
                }
            }
           // Console.WriteLine();
        }

        public void WriteFration()
        {
            string filePath3 = "HMMResult_log_Fration.txt";//定义一个文件路径变量
            swFra = new StreamWriter(Path.Combine(directoryPath, filePath3), true);
            swFra.WriteLine("DistinctLoc=3");
            foreach (var oput in FractionThree)
                swFra.WriteLine(oput);
            swFra.WriteLine("DistinctLoc=2");
            foreach (var oput in FractionTwo)
                swFra.WriteLine(oput);
            swFra.Flush();
            swFra.Close();
        }

        public void Close()
        {
            sw.Flush();
            sw.Close();
            swlog.Flush();
            swlog.Close();
        }
    }
}
