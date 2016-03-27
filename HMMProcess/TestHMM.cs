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

        enum Activity { Shopping, Eating, Transportation, work, Social, home, study, Recreation,Entertainment };  // 隐藏状态（活动）,9类
        string ResultdirectoryPath = @"D:\\201512_CMProcess\\HMMResult_Version3";
        string directoryPath = @"D:\\201512_CMProcess\\HMMResult_Version3\\Log";//定义一个路径变量
        HMMClass hmm;

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
            swlog = new StreamWriter(Path.Combine(directoryPath, filePath2), true);
        }

        public void CheckViterbi(CellTra[] Obeservation,List<int> userhwinfo,int n)
        {
            //一条用户停留轨迹
            

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
                 Q = hmm.Viterbi(Obeservation, out DELTA, out Probability);

            swlog.WriteLine(Probability.ToString("0.###E+0"));

            //Console.WriteLine("Probability =" + Probability.ToString("0.###E+0"));

            for (int t = 0; t < Q.Count(); t++)
            {
                sw.WriteLine(Obeservation[t].userid+"\t"+((Activity)Q[t]).ToString() + "\t" + Obeservation[t].intimeindex + "\t" + Obeservation[t].outtimeindex + "\t" + Obeservation[t].NumST + "\t" + Obeservation[t].stationid);
               // Console.WriteLine(((Activity)Q[t]).ToString());
            }
           // Console.WriteLine();
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
