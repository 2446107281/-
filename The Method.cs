using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.DataFormats;
using 超前进位加法器;



namespace 计算机原理__模型机
{
    public class The_Method
    {
       
        public string ir;//指令暂存器
        public static TextBox[] tbox_R;
        
        
        private static char[] numA;
        private static char[] numB;

        public static string shiliu_erjinzhi(string hexString)//十六进制转为二进制
        {
            string result = string.Empty;
            foreach (char c in hexString)
            {
                int v = Convert.ToInt32(c.ToString(), 16);
                int v2 = int.Parse(Convert.ToString(v, 2));
                result += string.Format("{0:d4}", v2);
            }
            return result;
        }
        
        public static string bucheng_16(string s)
        {
            while (s.Length < 17)
            {
                if (s.Length == 4 || s.Length == 8 || s.Length == 12)
                    s = " " + s;
                s = "0" + s;
            }
            return s;
        }
        public static void select_line(int ia, int line)//选中rtbox的一行
        {
            int length= Form2.rtbox_jqm[ia].Lines[line].Length;
            int a = Form2.rtbox_jqm[ia].GetFirstCharIndexFromLine(line);
            Form2.rtbox_jqm[ia].Select(a, length);
            Form2.rtbox_jqm[ia].SelectionBackColor = Color.Blue;
            
        }
        public static void select_line_cancel(int ia, int line)//取消选中rtbox的一行
        {
            int length = Form2.rtbox_jqm[ia].Lines[line].Length;
            int a = Form2.rtbox_jqm[ia].GetFirstCharIndexFromLine(line);
            Form2.rtbox_jqm[ia].Select(a, length);
            Form2.rtbox_jqm[ia].SelectionBackColor = Color.White;
        }
        public static string BinaryStrAdd(string str1, string str2)//加法与超前进位加法器进行对接
        {
            Form1 f1 = new Form1();
            int result = f1.duijie(str1, str2);
            return Convert.ToString(result, 16);

        }
        public static string BinaryStrAdd_one(string str1, string str2)//自增加一
        {
            int a = Convert.ToInt32(str1, 16);
            int b = Convert.ToInt32(str2, 2);
            int c = a + b;
            string d = Convert.ToString(c, 16);
            return d;
        }
        public static string BinaryStrSub(string str, string s)//减法
        {

            int a = Convert.ToInt32(str, 16);
            int b = Convert.ToInt32(s, 16);
            int c = a - b;
            string d = Convert.ToString(c, 16);
            return d;
        }
        public static string BinaryStrAnd(string a, string b)//逻辑乘：异或
        {

            char[] A = new char[8];
            numA = The_Method.zh(a);
            numB = The_Method.zh(b);
            for (int i = numA.Length - 1; i >= 0; i--)
            {
                for (int j = numB.Length - 1; j >= 0; j--)
                {
                    if (numA[i] == '1' && numB[i] == '1' || numA[i] == '0' && numB[i] == '0')
                    {
                        A[i] = '0';
                    }
                    else if (numA[i] == '1' && numB[i] == '0' || numA[i] == '0' && numB[i] == '1')
                    {
                        A[i] = '1';
                    }
                    else
                        MessageBox.Show("异或错误");
                }
            }
            return Convert.ToString(Convert.ToInt32(new string(A), 2), 16);
        }
        
        //数值转换
        public static char[] zh(string a)
        {
            int t = Convert.ToInt32(a, 16);
            a = Convert.ToString(t, 2);
            while (a.Length < 8)
            {
                a = "0" + a;
            }
            char[] c = a.ToCharArray();
            return c;
        }
    }
}
