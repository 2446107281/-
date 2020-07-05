using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace 超前进位加法器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TB_x = new TextBox[8] { x0, x1, x2, x3, x4, x5, x6, x7 };
            TB_y = new TextBox[8] { y0, y1, y2, y3, y4, y5, y6, y7 };
            TB_c = new TextBox[8] { c0, c1, c2, c3, c4, c5, c6, c7 };
            TB_s = new TextBox[8] { s0, s1, s2, s3, s4, s5, s6, s7 };
            TB_g = new TextBox[8] { g0, g1, g2, g3, g4, g5, g6, g7 };
            TB_p = new TextBox[8] { p0, p1, p2, p3, p4, p5, p6, p7 };
        }
        static TextBox[] TB_x ,TB_y,TB_c,TB_s,TB_g,TB_p;
        string x_two, y_two;
        int i=0,n=0;
        int[] G = new int[8];
        int[] P = new int[8];
        int[] X = new int[8];
        int[] Y = new int[8];
        int[] C = new int[8];
        int[] S = new int[8];

        private void btn_output_Click(object sender, EventArgs e)//数据的输出
        {
            output();
        }
        private void btn_finally_Click(object sender, EventArgs e)//最终执行
        {
            compare_finally();
        }
        private void btn_danbu_Click(object sender, EventArgs e)//单步执行
        {
            if (i<8)
            { 
                compare_danbu(i);
                i++;
            }
            else btn_danbu.Enabled = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form1 f2 = new Form1();//新建子窗体对象
            f2.Owner = this;
        }
        public int duijie(string a,string b)
        {
            //string a_16 = GetChsFromHex(a);
            int i = int.Parse(a,System.Globalization.NumberStyles.HexNumber);
            //intput_x.Text = Convert.ToString(Convert.ToInt32("i",16),10);//十六进制的字符串转十进制数
            intput_x.Text = Convert.ToString(i);//十六进制的字符串转十进制数
            intput_y.Text = Convert.ToString(Convert.ToInt32(b, 2));
            InputData();
            compare_finally();
            output();
            return Convert.ToInt32 (textbox_jieguo.Text);

        }

        /*private string GetChsFromHex(string hex)
        {
            if (hex == null)
            {
                //throw new ArgumentException("hex is null!");
            }

            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格
                            //throw new ArgumentException("hex is not a valid number!", "hex");
            }

            // 需要将 hex 转换成 byte 数组。
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }

            // 获得 GB2312，Chinese Simplified。
            Encoding chs = System.Text.Encoding.GetEncoding("GB2312");

            return chs.GetString(bytes);
        }*/

        private void btn_ok_Click(object sender, EventArgs e)//确定
         {
            InputData();
         }
        
        public void InputData()//输入数据，将转换完的二进制字符逐个输入到X[],Y[],textbox中
        {
            zhuanhuan_2();
            chushihua();
            char[] char_x = x_two.ToCharArray();
            char[] char_y = y_two.ToCharArray();                    
            for (int i = x_two.Length - 1; i >= 0; i--)//将转换完的二进制字符逐个输入到textbox中
            { 
                    X[i] = x_two[n]-48;
                    TB_x[i].Text = Convert.ToString(x_two[n]-48);
                if (n<x_two.Length)
                {
                    n++;
                }
            }
            n = 0;
            for (int i = y_two.Length - 1; i >= 0; i--)//将转换完的二进制字符逐个输入到textbox中
            {
                Y[i] = y_two[n] - 48;
                TB_y[i].Text = Convert.ToString(y_two[n] - 48);
                if (n < y_two.Length)
                {
                    n++;
                }
            }
        }    
        private void compare_danbu(int i)//单步执行
        {
            G[i] = X[i] * Y[i];//Gi进位产生信号
            P[i] = X[i] + Y[i];//Pi进位传递信号
            if (i < 7)
            {
                C[i + 1] = G[i] + P[i] * C[i];//计算C[i]
            }
            if (G[i] == 1 && P[i] != 2)//判断是否进位
            {
                C[i + 1] = 1;
            }
            else if (G[i] == 0 && P[i] == 1)
            {
                C[i + 1] = C[i];
            }
            else if (G[i] == 0 && P[i] == 0 && i < 7)
            {
                C[i + 1] = 0;
            }
            else if (P[i] == 2)
            {
                P[i]--;
            }
            S[i] = G[i] ^ P[i] ^ C[i];
            TB_g[i].Text = Convert.ToString(G[i]);//显示在各个方块内
            TB_p[i].Text = Convert.ToString(P[i]);
            TB_c[i].Text = Convert.ToString(C[i]);
            TB_s[i].Text = Convert.ToString(S[i]);
        }
        private void compare_finally()//最终计算
        {
            for (int i = 0; i < 8; i++)
            {
                G[i] = X[i] * Y[i];//Gi进位产生信号
                P[i] = X[i] + Y[i];//Pi进位传递信号
                if (P[i] == 2)
                {
                    P[i]--;
                }
                if (i<7)
                {
                    C[i+1] = G[i] + P[i] * C[i];//计算C[i]
                }
                if (G[i] == 1&&P[i]!=2)//判断是否进位
                {
                    C[i + 1] = 1;
                }
                else if (G[i] == 0 && P[i] == 1)
                {
                    C[i + 1] = C[i];
                }
                else if (G[i] == 0 && P[i] == 0 && i < 7)
                {
                    C[i + 1] = 0;
                }
                else if(P[i] == 2)
                {
                    P[i]--;
                }
                S[i] = G[i] ^ P[i] ^ C[i];
                TB_g[i].Text = Convert.ToString(G[i]);
                TB_p[i].Text = Convert.ToString(P[i]);
                TB_c[i].Text = Convert.ToString(C[i]);
                TB_s[i].Text = Convert.ToString(S[i]);
            }
        }
        private void output()//输出结果
        {
            for (int i = 7; i>=0; i--)
            {
                textbox_jieguo.Text = textbox_jieguo.Text + S[i];
            }
            string jieguo = textbox_jieguo.Text;
            textbox_jieguo.Text = Convert.ToString(Convert.ToInt32(jieguo,2));
        }
        public void zhuanhuan_2()//将输入的数字转换为二进制的数字
        {
            x_two = Convert.ToString(int.Parse(intput_x.Text), 2);//8==>1000
            y_two = Convert.ToString(int.Parse(intput_y.Text), 2);//20==>10100
        }
        private void chushihua()//控件初始化
        {
            for (int i = 0; i < 8; i++)
            {
                TB_x[i].Text = Convert.ToString(0);//初始值为0
                TB_y[i].Text = Convert.ToString(0);
            }
        }
    }
}
