using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMMProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            string ResultdirectoryPath = @"D:\\201604_CMProcess\\HMMResult\\VersionTest9\\log";

            string directoryPath_HWInfo = @"D:/201604_CMProcess/CMDProcessResult/version1/HWInfoResult/";//定义一个路径变量
            string directoryPath_STInfo = @"D:/201604_CMProcess/CMDProcessResult/version1/STDataResult_400_1Hour/";//定义一个路径变量

            if (!Directory.Exists(ResultdirectoryPath))//如果路径不存在
            {
                Directory.CreateDirectory(ResultdirectoryPath);//创建一个路径的文件夹
            }

            StreamWriter swlog = new StreamWriter(Path.Combine(ResultdirectoryPath, "log.txt"), true);

            Dictionary<int,List<int>> userhwinfo;

            //输入轨迹存储
            Dictionary<int, List<CellTra>> tempdata = new Dictionary<int, List<CellTra>>();

            Dictionary <int,double[]> B;
            Double[,] A;
            Double[,] PI;
            Dictionary<int, string[]> StationInfo;

            //原始数据读取
            DataInput input = new DataInput(9, 3956);
            //1、生成矩阵读取
            B = input.ConfusionMatInput();
            //2、转换矩阵读取
            A = input.TransMatInput();
            //3、初始矩阵读取
            PI = input.InitMatInput();
            //4、基站坐标读取
            StationInfo = input.GridCellIdInfoInput();

            //分163个文件读
            DirectoryInfo dir = new System.IO.DirectoryInfo("D:\\201604_CMProcess\\CMDProcessResult\\version1\\STDataResult_400_1Hour");
            
           // int n = 0;
            for(int n=0;n< dir.GetFiles().Count();n++)
            //foreach (FileInfo fi in )
            {

                //FileInfo fi = dir.(filePath_ST);
                string filePath_ST = "STTraByUser_" + n + ".txt";
               // if (fi.Extension == ".txt")
               // {
                    userhwinfo = new Dictionary<int, List<int>>();
                    StreamReader sr = new StreamReader(Path.Combine(directoryPath_STInfo + filePath_ST));
                    String line;
                    int m = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //文件顺序：userid,stationid, lat,lon, intimeindex, outtimeindex, NumST
                        string[] strArr = line.Split('\t');
                        DateTime indt = DateTime.ParseExact(strArr[4], "yyyy/M/dd H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime outdt = DateTime.ParseExact(strArr[5], "yyyy/M/dd H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        int userid = Convert.ToInt32(strArr[0]);
                        int cellid = Convert.ToInt32(strArr[1]);
                        int NumST = Convert.ToInt32(strArr[6]);
                        //添加到hash表中，key为用户id
                        if (tempdata.ContainsKey(userid))
                        {
                            tempdata[userid].Add(new CellTra(indt, outdt, userid, cellid, NumST));
                        }
                        else
                        {
                            tempdata.Add(userid, new List<CellTra>());
                            tempdata[userid].Add(new CellTra(indt, outdt, userid, cellid, NumST));
                        }
                        m++;
                    }
                    //读取对应文件的家和工作信息
                    string filePath_HW = "HWInfoByUser_" + n + ".txt";
                    int C = 0;
                    try
                    {
                        using (StreamReader srHW = new StreamReader(Path.Combine(directoryPath_HWInfo + filePath_HW)))
                        {
                            String HWline;
                            while ((HWline = srHW.ReadLine()) != null)
                            {
                                //字段：userid，home，work.若为0，则为空
                                string[] strArr = HWline.Split('\t');
                                int userid = Convert.ToInt32(strArr[0]);
                                int Home = Convert.ToInt32(strArr[1]);
                                int Work = Convert.ToInt32(strArr[2]);

                                userhwinfo.Add(userid,new List<int> { Home, Work });
                                C++;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("HW file could not be read:");
                        Console.WriteLine(e.Message);
                    }


                    TestHMM test = new TestHMM(n, A, B, PI);
                    int num = 0;
                    //得到某一文件原始数据序列，100000用户为一个文件,一个tt为一个用户数据
                    foreach (var tt in tempdata)
                    {
                        int isNotower = 1;
                        //判断是否有不在混淆矩阵的点。因为混淆矩阵只有5183维，有一些地方不会产生活动。
                        var q = from w in tt.Value select w.cellid;
                        foreach (var k in q)
                            if (!B.ContainsKey(k))
                                isNotower = 0;
                        if(isNotower==1)
                        {
                            test.CheckViterbi(tt.Value.ToArray(), userhwinfo[tt.Key], n);
                            num++;
                        }

                    }

                    int UserCount = tempdata.Keys.Count();
                    //LOG字段：文件序号，该文件数据条数，用户数，输入hmm的用户数，HWInfo条数
                    swlog.WriteLine(n + "\t" + m + "\t" + UserCount + "\t" + num + "\t" + C);
                    Console.WriteLine("文件序号：" + n + "数据条数：" + m);
                    Console.WriteLine("该文件用户数：" + UserCount);
                    Console.WriteLine("HWInfo数：" + C);

                    userhwinfo.Clear();
                    tempdata.Clear();
                    test.Close();
                   // n++;//文件循环163
                    sr.Close();
                    sr.Dispose();
                }

            swlog.Flush();
            swlog.Close();
            //}
        }

    }
}
