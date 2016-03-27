using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMMProcess

{
    class DataInput
    {
            //基站号：2-5953，5934个
            private Dictionary<int, double[]> B;
            private Double[,] A;
            private Double[,] PI;
            /// <summary>  
            /// 隐藏状态数目 N  
            /// </summary>  
            private readonly Int32 N;

            /// <summary>  
            /// 观察符号数目 M  
            /// </summary>  
            private readonly Int32 M;

        private Dictionary<int,string[]> StationInfo;

        //private Hashtable StationInfo;
        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="StatesNum">隐藏状态数目</param>  
        /// <param name="ObservationSymbolsNum">观察符号数目</param>  
        public DataInput(Int32 StatesNum, Int32 ObservationSymbolsNum)  
        {  
            N = StatesNum;              // 隐藏状态数目  9
            M = ObservationSymbolsNum;  // 观察符号数目  5183

            A = new Double[N * 4, N * 4];   // 状态转移矩阵,分4个时段  
            B = new Dictionary<int, double[]>();   // 混淆矩阵   
            PI = new Double[N,4];     // 初始概率向量，分24个时段

            StationInfo = new Dictionary<int, string[]>();
        }


        public Dictionary<int, Double[]> ConfusionMatInput()
        {
            //混淆矩阵
            try
            {

                using (StreamReader sr = new StreamReader("D:\\201512_CMProcess\\POIConfusionResult\\2016_ConfusionMAT_9_5934_GTEsti.txt"))
                {
                    //输入文本为5183行，10列。
                    String line;
                    int i = 0, j = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] strArr = line.Split('\t');
                        int stationid = Convert.ToInt32(strArr[0]);
                        B.Add(stationid, new double[9]);
                        for (j = 1; j < strArr.Count()-1; j++)
                        {
                            //B[j, i] = Convert.ToDouble(strArr[j]) * 100;
                            B[stationid][j - 1] = Convert.ToDouble(strArr[j])*100;
                            //Console.WriteLine(matrix[i, j]);
                        }
                        i++;
                    }
                    Console.WriteLine(i + " rows, " + j + "  cols Confusion Matrix B has been read");
                    //WriteLogData(i + " rows, " + j + "  cols Confusion Matrix B has been read");
                    return B;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }


        public Double[,] TransMatInput()
        {
            //转换矩阵
            try
            {

                using (StreamReader sr = new StreamReader("D:\\201512_CMProcess\\CheckinTransitionResult\\2016TransitionMATPro_4_9.txt"))
                {

                    String line;
                    int i = 0, j = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        double Sum = 0;
                        string[] strArr = line.Split('\t');
                        for (j = 0; j < strArr.Count(); j++)
                        {
                            A[i, j] = Convert.ToDouble(strArr[j]) ;
                            Sum += A[i, j];
                            //Console.WriteLine(matrix[i, j]);
                        }
                        //for (j = 0; j < A.GetLength(0); j++)
                        //{
                        //    A[i, j] /= Sum;
                        //}

                        i++;
                    }
                    Console.WriteLine(i + " rows, " + j + "  cols Transition Matrix A has been read");
                    //WriteLogData(i  + " rows, " + j + "  cols Transition Matrix A has been read");
                    return A;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }


        public Double[,] InitMatInput()
        {
            //初始概率矩阵
            try
            {

                using (StreamReader sr = new StreamReader("D:\\201512_CMProcess\\POIConfusionResult\\IniMat_9_4 Version2.txt"))
                {

                    String line;
                    // the file is reached.从1开始存。
                    int i = 0,j=0;
                    while ((line = sr.ReadLine()) != null)
                    {//9行，4列
                        string[] strArr = line.Split('\t');
                        for (j = 0; j < strArr.Count() - 1; j++)
                        {
                            PI[i, j] = Convert.ToDouble(strArr[j])/10;

                            //Console.WriteLine(matrix[i, j]);
                        }
                        //PI[i] = Convert.ToDouble(line);
                        //PI[i] /= 27183;
                        i++;
                    }

                    Console.WriteLine(i + "  Init Matrix PI has been read");
                    //WriteLogData(i + "  Init Matrix PI has been read");
                    return PI;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public Dictionary<int, string[]> StationIdInfoInput()
        {
            //基站id数据
            try
            {

                using (StreamReader sr = new StreamReader("D:\\201512_CMProcess\\stationIdInfo.txt"))
                {

                    String line;
                    int i = 0;
                    int[] latlon = new int[2];
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] strArr = line.Split('\t');
                        string[] LatLon = strArr[0].Split(',');

                        StationInfo.Add(Convert.ToInt32(strArr[1]), LatLon);
                        i++;
                    }
                    Console.WriteLine(i + "  station id has been read");
                    //swlog.WriteLine("{0}\t{1}", i, " station");
                    return StationInfo;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
