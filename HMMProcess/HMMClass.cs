﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMMProcess
{
    class HMMClass
    {
        /// <summary>  
        /// 隐藏状态数目 N  
        /// </summary>  
        public readonly Int32 N;  
  
        /// <summary>  
        /// 观察符号数目 M  
        /// </summary>  
        public readonly Int32 M;   
  
        /// <summary>  
        /// 状态转移矩阵 A  
        /// </summary>  
        public Double[,]  A;  
  
        /// <summary>  
        /// 混淆矩阵（confusion matrix）B  
        /// </summary>  
        public Dictionary<int, double[]> B;  
  
        /// <summary>  
        /// 初始概率向量 PI  
        /// </summary>  
        public Double[,]   PI;  
  
        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="StatesNum">隐藏状态数目</param>  
        /// <param name="ObservationSymbolsNum">观察符号数目</param>  
        public HMMClass(Int32 StatesNum)  
        {  
            N = StatesNum;              // 隐藏状态数目  9
           // M = ObservationSymbolsNum;  // 观察符号数目  5934

            A = new Double[N*4, N*4];   // 状态转移矩阵  24小时分为4个时段。0-6，6-12，12-18，18-24
            B = new Dictionary<int, double[]>();   // 混淆矩阵   
            PI = new Double[N,4];     // 初始概率向量  
        }


        /// <summary>  
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列  
        /// Viterbi Algorithm: Finding most probable sequence of hidden states  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>  
        /// <returns>可能性最大的隐藏状态序列</returns>  
        /// <remarks>使用双精度运算，不输出中间结果</remarks>  
        public Int32[] Viterbi(CellTra[] OB, out Double[,] DELTA, out Double Probability, List<int> HWinfo)
        {
            //Double[,] DELTA;
            Int32[,] PSI;

            return Viterbi(OB, out DELTA, out PSI, out Probability, HWinfo);
        }
        public Int32[] Viterbi(CellTra[] OB, out Double[,] DELTA, out Double Probability)
        {
            //Double[,] DELTA;
            Int32[,] PSI;

            return Viterbi(OB, out DELTA, out PSI, out Probability);
        }


        /// <summary>  
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="DELTA">输出中间结果：局部最大概率</param>  
        /// <param name="PSI">输出中间结果：反向指针指示最可能路径</param>  
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>  
        /// <returns>可能性最大的隐藏状态序列</returns>   
        /// <remarks>使用双精度运算，且输出中间结果</remarks>  
        public Int32[] Viterbi(CellTra[] OB, out Double[,] DELTA, out Int32[,] PSI, out Double Probability, List<int> HWinfo)
        {

            DELTA = new Double[OB.Length, N];   // 局部概率  
            PSI = new Int32[OB.Length, N];      // 反向指针


            bool isPreHPoint = false;
            bool isPreWPoint = false;
            //初始概率矩阵为四段时间制
            //int InitHourIndex = (OB[0].intimeindex.Hour+OB[0].outtimeindex.Hour) > 24 ? (int)(OB[0].intimeindex.Hour + OB[0].outtimeindex.Hour-24)/12 :(int)(OB[0].intimeindex.Hour + OB[0].outtimeindex.Hour) / 12;
            int InitHourIndex=OB[0].intimeindex.Hour == 23? (int)(OB[0].intimeindex.Hour + OB[0].outtimeindex.Hour - 24) / 12: (int)(OB[0].intimeindex.Hour + OB[0].outtimeindex.Hour) / 12;

            // 1. 初始化  
            for (Int32 j = 0; j < N; j++)
            {
                //OB[0].towerindex是2841基站序列
                DELTA[0, j] = PI[j, InitHourIndex] * B[OB[0].cellid][j];
            }
            //初始为家
            if (HWinfo[0] == OB[0].cellid)
            {
                isPreHPoint = true;
            }
            //初始为工作
            if (HWinfo[1] == OB[0].cellid)
            {
                isPreWPoint = true;
            }

            // 2. 递归  
            for (Int32 t = 1; t < OB.Length; t++)
            {
                //获取时间索引（这里可采用4段制和24段制）,采用4段制。0-6，6-12，12-18，18-24
                int CHourIndex = OB[t].intimeindex.Hour == 23 ? (int)(OB[t].intimeindex.Hour + OB[t].outtimeindex.Hour - 24) / 12 : (int)(OB[t].intimeindex.Hour + OB[t].outtimeindex.Hour) / 12;
                int PHourIndex= OB[t - 1].intimeindex.Hour == 23? (int)(OB[t - 1].intimeindex.Hour + OB[t - 1].outtimeindex.Hour - 24) / 12: (int)(OB[t - 1].intimeindex.Hour + OB[t - 1].outtimeindex.Hour) / 12;


                //当前时刻每一个隐含状态j都遍历一边
                for (Int32 j = 0; j < N; j++)
                {
                    //如果前一状态确定为home或者work，则当前状态所记录的前一最大可能状态必须为home或者work
                    if (isPreHPoint == true&&isPreWPoint==false)
                    {
                        Double MaxValue = DELTA[t - 1, 5] * A[PHourIndex+5*4, CHourIndex + j * 4];
                        Int32 MaxValueIndex = 5;

                        DELTA[t, j] = MaxValue * B[OB[t].cellid][j];
                        PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态 
                    }
                    else if (isPreWPoint == true&&isPreHPoint==false)
                    {
                        Double MaxValue = DELTA[t - 1, 3] * A[PHourIndex+3*4, CHourIndex + j * 4];
                        Int32 MaxValueIndex = 3;

                        DELTA[t, j] = MaxValue * B[OB[t].cellid][j];
                        PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态 
                    }
                    else if(isPreHPoint==true&&isPreWPoint==true)
                    {
                        //看上一个点的所处时刻，决定是home还是work
                        if(PHourIndex==0||PHourIndex==3)
                        {
                            Double MaxValueH = DELTA[t - 1, 5] * A[PHourIndex + 5 * 4, CHourIndex + j * 4];
                            Int32 MaxValueIndex = 5;
                            DELTA[t, j] = MaxValueH * B[OB[t].cellid][j];
                            PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态 
                        }
                        else if(PHourIndex==1||PHourIndex==2)
                        {
                            Double MaxValueW = DELTA[t - 1, 3] * A[PHourIndex + 3 * 4, CHourIndex + j * 4];
                            Int32 MaxValueIndex = 3;
                            DELTA[t, j] = MaxValueW * B[OB[t].cellid][j];
                            PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态 
                        }
                    }
                    else //isPreHPoint和isPreWPoint都为false
                    {
                        Double MaxValue = DELTA[t - 1, 0] * A[PHourIndex+0*4, CHourIndex + j * 4];
                        Int32 MaxValueIndex = 0;
                        //当前时刻的某一j状态与前一时刻每一个状态连接，求得最大连接值
                        for (Int32 i = 1; i < N; i++)
                        {
                            //有pre-home info，且前一位置不为家，则不能将MaxValueIndex设为5,但如果没有pre-home info，
                            //则正常递归计算，若得到home location，则添加到HWinfo数组里，作为先验信息。
                            if (HWinfo[0] != 0 && i == 5)
                                continue;
                            //有pre-work info，且前一位置不为工作，则不能将MaxValueIndex设为3,但如果没有pre-work info，
                            //则正常递归计算，若得到work location，则添加到HWinfo数组里，作为先验信息。
                            if (HWinfo[1] != 0 && i == 3)
                                continue;
                            Double Value = DELTA[t - 1, i] * A[PHourIndex + i * 4, CHourIndex + j * 4];
                            if (Value > MaxValue)
                            {
                                MaxValue = Value;
                                MaxValueIndex = i;
                            }

                        }
                        //将探测出来的及时home work info添加进先验知识里
                        if (MaxValueIndex == 5)
                            HWinfo[0] = OB[t - 1].cellid;
                        if(MaxValueIndex==3)
                            HWinfo[1]= OB[t - 1].cellid;

                        DELTA[t, j] = MaxValue * B[OB[t].cellid][j];
                        PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态  

                    }
                }

                //先置为false
                isPreWPoint = false;
                isPreHPoint = false;
                
               //当前位置为工作或者家的时候，要保证下一个点的前溯最优点为这个点，否则会出现错误
               //有工作点
               if (HWinfo[1] == OB[t].cellid)
               {
                   isPreWPoint = true;
               }
               if (HWinfo[0] == OB[t].cellid)
               {
                    isPreHPoint = true;
               }
            }


            // 3. 终止  
            //终止条件适用于输入轨迹为1个点、2个点、多个点。
            //找到最大 的概率路径，即DELTA[OB.Length - 1, i]中最大的值
            Int32[] Q = new Int32[OB.Length];   // 最佳路径
            Q[OB.Length - 1] = 0;
            Probability = DELTA[OB.Length - 1, 0];
            //添加终止条件。
            if (isPreHPoint == true && isPreWPoint == false)
            {
                Probability = DELTA[OB.Length - 1, 5];
                Q[OB.Length - 1] = 5;
            }
            else if (isPreWPoint == true && isPreHPoint == false)
            {
                Probability = DELTA[OB.Length - 1, 3];
                Q[OB.Length - 1] = 3;
            }
            else if (isPreHPoint == true && isPreWPoint == true)
            {
                int PHourIndex= OB[0].intimeindex.Hour == 23? (int)(OB[OB.Count() - 1].intimeindex.Hour + OB[OB.Count() - 1].outtimeindex.Hour - 24) / 12: (int)(OB[OB.Count() - 1].intimeindex.Hour + OB[OB.Count() - 1].outtimeindex.Hour) / 12;
                if (PHourIndex == 0 || PHourIndex == 3)
                {
                    Probability = DELTA[OB.Length - 1, 5];
                    Q[OB.Length - 1] = 5;
                }
                else if (PHourIndex == 1 || PHourIndex == 2)
                {
                    Probability = DELTA[OB.Length - 1, 3];
                    Q[OB.Length - 1] = 3;
                }
            }
            else
            {
                for (Int32 i = 1; i < N; i++)
                {
                    //有pre-home info，且前一位置不为家，则不能将MaxValueIndex设为5,但如果没有pre-home info，
                    //则正常递归计算，若得到home location，则添加到HWinfo数组里，作为先验信息。
                    if (HWinfo[0] != 0 && i == 5)
                        continue;
                    //有pre-work info，且前一位置不为工作，则不能将MaxValueIndex设为3,但如果没有pre-work info，
                    //则正常递归计算，若得到work location，则添加到HWinfo数组里，作为先验信息。
                    if (HWinfo[1] != 0 && i == 3)
                        continue;

                    if (DELTA[OB.Length - 1, i] > Probability)
                    {
                        Probability = DELTA[OB.Length - 1, i];
                        Q[OB.Length - 1] = i;
                    }
                }
            }


            // 4. 路径回溯  
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                Q[t] = PSI[t + 1, Q[t + 1]];
            }

            return Q;
        }

        public Int32[] Viterbi(CellTra[] OB, out Double[,] DELTA, out Int32[,] PSI, out Double Probability)
        {

            DELTA = new Double[OB.Length, N];   // 局部概率  
            PSI = new Int32[OB.Length, N];      // 反向指针



            //24小时制的时间，转化成4个时间段。除以6
            int InitHourIndex = OB[0].intimeindex.Hour == 23 ? (int)(OB[0].intimeindex.Hour + OB[0].outtimeindex.Hour - 24) / 12 : (int)(OB[0].intimeindex.Hour + OB[0].outtimeindex.Hour) / 12;
            
            // 1. 初始化  
            for (Int32 j = 0; j < N; j++)
            {
                DELTA[0, j] = PI[j,InitHourIndex] * B[OB[0].cellid][j];
            }

            // 2. 递归  
            for (Int32 t = 1; t < OB.Length; t++)
            {
                int CHourIndex = OB[t].intimeindex.Hour == 23 ? (int)(OB[t].intimeindex.Hour + OB[t].outtimeindex.Hour - 24) / 12 : (int)(OB[t].intimeindex.Hour + OB[t].outtimeindex.Hour) / 12;
                int PHourIndex = OB[t - 1].intimeindex.Hour == 23 ? (int)(OB[t - 1].intimeindex.Hour + OB[t - 1].outtimeindex.Hour - 24) / 12 : (int)(OB[t - 1].intimeindex.Hour + OB[t - 1].outtimeindex.Hour) / 12;

                //当前时刻每一个隐含状态都遍历一边
                for (Int32 j = 0; j < N; j++)
                {

                    Double MaxValue = DELTA[t - 1, 0] * A[PHourIndex+0*4, CHourIndex + j * 4];
                    Int32 MaxValueIndex = 0;
                    //当前时刻的某一j状态与前一时刻每一个i状态连接，求得最大连接值
                    for (Int32 i = 1; i < N; i++)
                    {

                        Double Value = DELTA[t - 1, i] * A[PHourIndex + i * 4, CHourIndex + j * 4];
                        if (Value > MaxValue)
                        {
                            MaxValue = Value;
                            MaxValueIndex = i;
                        }

                    }

                    DELTA[t, j] = MaxValue * B[OB[t].cellid][j];
                    PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态  
                }
            }


            // 3. 终止  
            Int32[] Q = new Int32[OB.Length];   // 最佳路径
            Q[OB.Length - 1] = 0;
            Probability = DELTA[OB.Length - 1, 0];
            for (Int32 i = 1; i < N; i++)
            {
                if (DELTA[OB.Length - 1, i] > Probability)
                {
                    Probability = DELTA[OB.Length - 1, i];
                    Q[OB.Length - 1] = i;
                }
            }

            // 4. 路径回溯  
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                Q[t] = PSI[t + 1, Q[t + 1]];
            }

            return Q;
        }

        /// <summary>  
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>  
        /// <returns>可能性最大的隐藏状态序列</returns>  
        ///// <remarks>使用对数运算，不输出中间结果</remarks>  
        //public Int32[] ViterbiLog(UserData[] OB, out Double Probability)
        //{
        //    Double[,] DELTA;
        //    Int32[,] PSI;

        //    return ViterbiLog(OB, out DELTA, out PSI, out Probability);
        //}
        //public Int32[] ViterbiLog(UserData[] OB, out Double Probability, UserHWP HWinfo)
        //{
        //    Double[,] DELTA;
        //    Int32[,] PSI;

        //    return ViterbiLog(OB, out DELTA, out PSI, out Probability, HWinfo);
        //}

        ///// <summary>  
        ///// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列  
        ///// </summary>  
        ///// <param name="OB">已知的观察序列</param>  
        ///// <param name="DELTA">输出中间结果：局部最大概率。结果为自然对数值</param>  
        ///// <param name="PSI">输出中间结果：反向指针指示最可能路径</param>  
        ///// <param name="Probability">可能性最大的隐藏状态序列的概率</param>  
        ///// <returns>可能性最大的隐藏状态序列</returns>   
        ///// <remarks>使用对数运算，且输出中间结果</remarks>  
        //public Int32[] ViterbiLog(UserData[] OB, out Double[,] DELTA, out Int32[,] PSI, out Double Probability)
        //{
        //    DELTA = new Double[OB.Length, N];   // 局部概率  
        //    PSI = new Int32[OB.Length, N];      // 反向指针  

        //    // 0. 预处理  
        //    Double[,] LogA = new Double[N*24, N*24];
        //    for (Int32 i = 0; i < N*24; i++)
        //    {
        //        for (Int32 j = 0; j < N*24; j++)
        //        {
        //            LogA[i, j] = Math.Log(A[i, j]);
        //        }
        //    }

        //    Double[,] LogBIOT = new Double[N, OB.Length];
        //    for (Int32 i = 0; i < N; i++)
        //    {
        //        for (Int32 t = 0; t < OB.Length; t++)
        //        {
        //            LogBIOT[i, t] = Math.Log(B[i, OB[t].stationid-2]);
        //        }
        //    }

        //    Double[] LogPI = new Double[N];
        //    for (Int32 i = 0; i < N; i++)
        //    {
        //        LogPI[i] = Math.Log(PI[i]);
        //    }

        //    // 1. 初始化  
        //    for (Int32 j = 0; j < N; j++)
        //    {
        //        DELTA[0, j] = LogPI[j] + LogBIOT[j, 0];
        //    }

        //    // 2. 递归  
        //    for (Int32 t = 1; t < OB.Length; t++)
        //    {
        //        int CHourIndex = OB[t].timeindex / 3600;
        //        int PHourIndex = OB[t - 1].timeindex / 3600;
        //        for (Int32 j = 0; j < N; j++)
        //        {

        //            Double MaxValue = DELTA[t - 1, 0] + LogA[PHourIndex, CHourIndex + j * 24];
        //            Int32 MaxValueIndex = 0;
        //            for (Int32 i = 1; i < N; i++)
        //            {
        //                Double Value = DELTA[t - 1, i] + LogA[PHourIndex + i * 24, CHourIndex + j * 24];
        //                if (Value > MaxValue)
        //                {
        //                    MaxValue = Value;
        //                    MaxValueIndex = i;
        //                }
        //            }

        //            DELTA[t, j] = MaxValue + LogBIOT[j, t];
        //            PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态  
        //        }
        //    }

        //    // 3. 终止  
        //    Int32[] Q = new Int32[OB.Length];   // 定义最佳路径  
        //    Probability = DELTA[OB.Length - 1, 0];
        //    Q[OB.Length - 1] = 0;
        //    for (Int32 i = 1; i < N; i++)
        //    {
        //        if (DELTA[OB.Length - 1, i] > Probability)
        //        {
        //            Probability = DELTA[OB.Length - 1, i];
        //            Q[OB.Length - 1] = i;
        //        }
        //    }

        //    // 4. 路径回溯  
        //    Probability = Math.Exp(Probability);
        //    for (Int32 t = OB.Length - 2; t >= 0; t--)
        //    {
        //        Q[t] = PSI[t + 1, Q[t + 1]];
        //    }

        //    return Q;
        //}

        //public Int32[] ViterbiLog(UserData[] OB, out Double[,] DELTA, out Int32[,] PSI, out Double Probability, UserHWP HWinfo)
        //{
        //    DELTA = new Double[OB.Length, N];   // 局部概率  
        //    PSI = new Int32[OB.Length, N];      // 反向指针  

        //    // 0. 预处理  
        //    Double[,] LogA = new Double[N*24, N*24];
        //    for (Int32 i = 0; i < N*24; i++)
        //    {
        //        for (Int32 j = 0; j < N*24; j++)
        //        {
        //            LogA[i, j] = Math.Log(A[i, j]);
        //        }
        //    }

        //    Double[,] LogBIOT = new Double[N, OB.Length];
        //    for (Int32 i = 0; i < N; i++)
        //    {
        //        for (Int32 t = 0; t < OB.Length; t++)
        //        {
        //            LogBIOT[i, t] = Math.Log(B[i, OB[t].stationid-2]);
        //        }
        //    }

        //    Double[] LogPI = new Double[N];
        //    for (Int32 i = 0; i < N; i++)
        //    {
        //        LogPI[i] = Math.Log(PI[i]);
        //    }

        //    // 1. 初始化  
        //    for (Int32 j = 0; j < N; j++)
        //    {
        //        DELTA[0, j] = LogPI[j] + LogBIOT[j, 0];
        //    }

        //    // 2. 递归  
        //    for (Int32 t = 1; t < OB.Length; t++)
        //    {
        //        int CHourIndex = OB[t].timeindex / 3600;
        //        int PHourIndex = OB[t - 1].timeindex / 3600;
        //        for (Int32 j = 0; j < N; j++)
        //        {

        //            Double MaxValue = DELTA[t - 1, 0] + LogA[PHourIndex, CHourIndex + j * 24];
        //            Int32 MaxValueIndex = 0;
        //            for (Int32 i = 1; i < N; i++)
        //            {
        //                Double Value = DELTA[t - 1, i] + LogA[PHourIndex + i * 24, CHourIndex + j * 24];
        //                if (Value > MaxValue)
        //                {
        //                    MaxValue = Value;
        //                    MaxValueIndex = i;
        //                }
        //            }

        //            DELTA[t, j] = MaxValue + LogBIOT[j, t];
        //            PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态  
        //        }

        //        if (HWinfo.WorkIndex == OB[t].stationid)
        //        {
        //            Double MaxValue = DELTA[t - 1, 0] * LogA[PHourIndex, CHourIndex + 3 * 24];
        //            Int32 MaxValueIndex = 0;
        //            //当前时刻的某一j状态与前一时刻每一个状态连接，求得最大连接值
        //            for (Int32 i = 1; i < N; i++)
        //            {
        //                Double Value = DELTA[t - 1, i] * LogA[PHourIndex + i * 24, CHourIndex + 3 * 24];
        //                if (Value > MaxValue)
        //                {
        //                    MaxValue = Value;
        //                    MaxValueIndex = i;
        //                }
        //            }
        //            DELTA[t, 3] = MaxValue * 1;
        //            PSI[t, 3] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态  
        //        }

        //        if (HWinfo.WorkIndex == OB[t].stationid)
        //        {
        //            Double MaxValue = DELTA[t - 1, 0] * LogA[PHourIndex, CHourIndex + 5 * 24];
        //            Int32 MaxValueIndex = 0;
        //            //当前时刻的某一j状态与前一时刻每一个状态连接，求得最大连接值
        //            for (Int32 i = 1; i < N; i++)
        //            {
        //                Double Value = DELTA[t - 1, i] * LogA[PHourIndex + i * 24, CHourIndex + 5 * 24];
        //                if (Value > MaxValue)
        //                {
        //                    MaxValue = Value;
        //                    MaxValueIndex = i;
        //                }
        //            }
        //            DELTA[t, 5] = MaxValue * 1;
        //            PSI[t, 5] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态  
        //        }
        //    }

        //    // 3. 终止  
        //    Int32[] Q = new Int32[OB.Length];   // 定义最佳路径  
        //    Probability = DELTA[OB.Length - 1, 0];
        //    Q[OB.Length - 1] = 0;
        //    for (Int32 i = 1; i < N; i++)
        //    {
        //        if (DELTA[OB.Length - 1, i] > Probability)
        //        {
        //            Probability = DELTA[OB.Length - 1, i];
        //            Q[OB.Length - 1] = i;
        //        }
        //    }

        //    // 4. 路径回溯  
        //    Probability = Math.Exp(Probability);
        //    for (Int32 t = OB.Length - 2; t >= 0; t--)
        //    {
        //        Q[t] = PSI[t + 1, Q[t + 1]];
        //    }

        //    return Q;
        //}  
    }
}
