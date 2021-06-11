using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thinger.cn.DataConvertHelper;

namespace SocketLib
{
    public class SocketSend
    {
        /// <summary>
        /// 用来存储同来发送的二进制的压力信息
        /// </summary>
        public bool[] boolmpar;
        public bool[] BoolMpar
        {
            get { return boolmpar; }
            set { boolmpar = value; }
        }

        public static byte[] Sendcontext(int ma) {

            byte[] by = ByteArrayLib.GetByteArrayFromInt(ma);
            bool[] bl = BitLib.GetBitArrayFromByteArray(by, true);
            bool[] rb = BitLib.GetBitArray(bl, bl.Length - 10, 10);
            List<bool[]> boollist = new List<bool[]>();
            for (int i = 0; i < 8; i++)
            {
                bool[] b = new bool[] { false, false, false, false, false, false, false, false };
                boollist.Add(b);
            }
            boollist[5][7] = rb[0];
            boollist[5][6] = rb[1];
            boollist[5][5] = rb[2];
            boollist[5][4] = rb[3];
            boollist[5][3] = rb[4];
            boollist[5][2] = rb[5];
            boollist[5][1] = rb[6];
            boollist[5][0] = rb[7];
            boollist[6][7] = rb[8];
            boollist[6][6] = rb[9];
            byte[] sendby = new byte[8];
            for (int i = 0; i < boollist.Count; i++)
            {
                byte[] byzhuan = ByteArrayLib.GetByteArrayFromBoolArray(boollist[i]);
                byzhuan.CopyTo(sendby, i);
            }
            return sendby;
        }

        public static double GetContext(byte[] b) {

            //解析返回的数据
            List<bool[]> boolist = new List<bool[]>();
            for (int i = 0; i < b.Length; i++)
            {
                boolist.Add(BitLib.GetBitArrayFromByte(b[i]));
            }
            bool[] res = new bool[10];
            res[9] = boolist[5][7];
            res[8] = boolist[5][6];
            res[7] = boolist[5][5];
            res[6] = boolist[5][4];
            res[5] = boolist[5][3];
            res[4] = boolist[5][2];
            res[3] = boolist[5][1];
            res[2] = boolist[5][0];
            res[1] = boolist[6][7];
            res[0] = boolist[6][6];
            double deres = 0;
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i])
                {
                    deres += 1 * Math.Pow(2, i);
                }
                else
                {
                    deres += 0 * Math.Pow(2, i);
                }
            }
            return deres;
        }

    }
}
