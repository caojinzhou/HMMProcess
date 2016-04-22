using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMMProcess
{
    class UserData
    {
            public DateTime timeindex;
            public int userid;
            public int cellid ;
            

            public UserData()
            {

            }

            public UserData(DateTime p1,int p2,int p3)
            {
                // TODO: Complete member initialization
                timeindex = p1;
                userid = p2;
                cellid = p3;
            }
    }

    class CellTra
    {
        public DateTime intimeindex;
        public DateTime outtimeindex;
        public int userid;
        public int cellid;
        public int NumST;


        public CellTra()
        {

        }

        public CellTra(DateTime p0, DateTime p1, int p2, int p3, int p5)
        {
            // TODO: Complete member initialization
            intimeindex = p0;
            outtimeindex = p1;
            userid = p2;
            cellid = p3;
            NumST = p5;
        }
    }
}
