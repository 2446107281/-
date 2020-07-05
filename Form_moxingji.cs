using System;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.DataFormats;
using 超前进位加法器;

namespace 计算机原理__模型机
{
    public partial class Form2 : Form
    {
      
            public static RichTextBox[] rtbox_jqm;
        public static TextBox[] tbox_R;
        
        int i = 0;
        int[] M = new int[65536];//主存储器
        int pc = 0x1000;//存储下一条指令(十六进制)
        int pc_line;
        string ir;//指令暂存器
        string Y_j, M_j;//源操作数寄存器，目的操作数寄存器
        int y_id, m_id;//源寄存器和目的寄存器的编号
        int single_caozuoqi;//单操作目的寄存器

        public Form2()
        {
            InitializeComponent();
            rtbox_jqm = new RichTextBox[2] { richTextBox_input, richTextBox_jiqima }; //0,1
            tbox_R = new TextBox[8] { R0, R1, R2, R3, R4, R5, R6, R7 };
        }

        private void btn_input_Click(object sender, EventArgs e)//导入文件
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "文本文件(*.txt;*.data)|*.txt;*.data";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFile.FileNames)
                {
                    richTextBox_input.Text = File.ReadAllText(file);
                }
            }
            zhuanhuan();
        }
        public void zhuanhuan()//转换为二进制机器码
        {
            string[] codes = richTextBox_input.Text.ToLower().Split('\n');
            for (int i = 0; i < codes.Length; i++)
            {
                richTextBox_jiqima.Text += Class_zhuanjiqima.jiqima_2(codes[i]);
            }
        }
        public void one_begin()
        {
            initMemory();
            
                string[] Arr = richTextBox_input.Text.ToLower().Split('\n');//将汇编指令一行行分到arr中
                pc = Convert.ToInt32(textBox_PC.Text.Replace(" ", ""), 16);//将空格换掉并写入pc 
                ir = richTextBox_jiqima.Lines[Convert.ToInt32(pc - i) - 4096];//从pc写入ir
                if (i != Arr.Length)
                {
                    The_Method.select_line(0, pc - i - 4096);
                    The_Method.select_line(1, pc - i - 4096);
                    if (i != 0)
                    {
                        The_Method.select_line_cancel(0, pc - i - 4097);
                        The_Method.select_line_cancel(1, pc - i - 4097);
                    }
                }
                pc= pc+2;
                i++;
                textBox_PC.Text = Convert.ToString(pc, 16);//pc的显示
                string[] fenge_4 = ir.Split(' ');//分成4段
                if (fenge_4.Length == 5)
                {
                    y_id = Convert.ToInt32(fenge_4[2], 2);//寄存器编号
                    m_id = Convert.ToInt32(fenge_4[4], 2);
                }
                if (fenge_4.Length == 3) single_caozuoqi = Convert.ToInt32(fenge_4[2], 2);//如果是单操作数
                if (ir == "") MessageBox.Show("" + "指令执行完毕");
                //*****************************************取指   ***************************************************************************
                /********************加法           减法                与                   MOV数据传送 ********************************************************************/
                if (fenge_4[0] == "0001" || fenge_4[0] == "0010" || fenge_4[0] == "0011" || fenge_4[0] == "1010")
                {
                    if (fenge_4[1] == "000")//直接寻址
                    {
                        Y_j = tbox_R[y_id].Text;
                    }
                    if (fenge_4[1] == "001")//间接寻址 
                    {
                        Y_j = Convert.ToString(readMemory(Convert.ToInt32(tbox_R[y_id].Text.Replace(" ", " "), 2)), 2);//用寄存器的地址去访问主存，再从主存中取出数送Y_j
                    }
                    if (fenge_4[1] == "010")//自增型间接寻址
                    {
                        Y_j = Convert.ToString(readMemory(Convert.ToInt32(tbox_R[y_id].Text.Replace(" ", " "), 2)), 2);//在间接的基础上，修改RS的内容，经运算器加一在送回rs当前操作的寄存器
                        tbox_R[y_id].Text = Convert.ToString(Convert.ToInt32(tbox_R[y_id].Text) + 1, 2);
                    }
                    if (fenge_4[1] == "011")//自增型双间址
                    {
                        Y_j = Convert.ToString(readMemory(readMemory(Convert.ToInt32(tbox_R[y_id].Text.Replace(" ", " "), 2))), 2);//第一次访问主存取出操作数的地址送往地址寄存器
                        tbox_R[y_id].Text = Convert.ToString(Convert.ToInt32(tbox_R[y_id].Text) + 1, 2);//第二次以此地址访问存储器取出操作数送入Y_j  //RS的内容加一
                    }
                    if (fenge_4[1] == "100")//变址寻址
                    {
                        Y_j = Convert.ToString(readMemory(Convert.ToInt32(richTextBox_jiqima.Lines[Convert.ToInt32(textBox_PC.Text) - i - 4096].Replace(" ", ""), 2)
                            + Convert.ToInt32(tbox_R[y_id].Text.Replace(" ", ""), 2)), 2); //以PC现在的地址从主存中取出位移量，在与RS的内容相加
                        pc = pc + 2; //以相加结果的地址取出操作数送入Y_j  //pc的内容加一
                    }
                }
                //***************************************取目的数******************************************************************************
                /********************加法           减法                与                   MOV数据传送       LDI载入立即数       NEC求补*********单操作数加一******减一***/
                if (fenge_4[0] == "0001" || fenge_4[0] == "0010" || fenge_4[0] == "0011" || fenge_4[0] == "1010" || fenge_4[0] == "0110"
                    || fenge_4[0] == "0100" || fenge_4[0] == "0101")
                {
                    if (fenge_4[3] == "000")//直接寻址
                    {
                        M_j = tbox_R[m_id].Text;
                    }
                    else if (fenge_4[3] == "001")//间接寻址
                    {
                        M_j = Convert.ToString(readMemory(Convert.ToInt32(tbox_R[m_id].Text.Replace(" ", ""), 16)), 2);
                    }
                    else if (fenge_4[3] == "010")//自增型间接寻址
                    {
                        M_j = Convert.ToString(readMemory(Convert.ToInt32(tbox_R[m_id].Text.Replace(" ", ""), 16)), 2);//在间接的基础上，修改RS的内容，经运算器加一在送回rs当前操作的寄存器
                        tbox_R[m_id].Text = Convert.ToString(Convert.ToInt32(tbox_R[m_id].Text) + 1, 16);
                    }
                    else if (fenge_4[3] == "011")//自增型双间址
                    {
                        M_j = Convert.ToString(readMemory(readMemory(Convert.ToInt32(tbox_R[m_id].Text.Replace(" ", ""), 16))), 2);//第一次访问主存取出操作数的地址送往地址寄存器
                        tbox_R[m_id].Text = Convert.ToString(Convert.ToInt32(tbox_R[m_id].Text.Replace(" ", ""), 16) + 1, 16);//第二次以此地址访问存储器取出操作数送入Y_j  //RS的内容加一
                    }
                    else if (fenge_4[3] == "100")//变址寻址
                    {
                        int pc_10 = int.Parse(textBox_PC.Text, System.Globalization.NumberStyles.HexNumber);
                        int R_10 = int.Parse(tbox_R[m_id].Text, System.Globalization.NumberStyles.HexNumber);

                        M_j = Convert.ToString(readMemory(Convert.ToInt32(Convert.ToString(pc_10 + R_10, 16))));
                        pc = pc + 2;
                        M[Convert.ToInt32(Convert.ToString(pc_10 + R_10, 16))]--;
                        
                    }
                    else MessageBox.Show("进入失败");
                }
                if (fenge_4[0] == "1110")// LDI载入立即数 
                {
                    M_j = fenge_4[1].Substring(0, 8);
                }
                if (fenge_4[0] == "1001")//装载指令ld
                {
                    M_j = Convert.ToString(readMemory(Convert.ToInt32(fenge_4[1].Substring(0, 8), 2)), 2);//把K的地址的内容送Rd
                }
                //***************************************************执行周期*****************************************************************
                if (fenge_4[0] == "0001") tbox_R[y_id].Text = (The_Method.BinaryStrAdd(Y_j.Replace(" ", ""), M_j.Replace(" ", "")));//加法
                if (fenge_4[0] == "0010") tbox_R[y_id].Text = (The_Method.BinaryStrSub(Y_j.Replace(" ", ""), M_j.Replace(" ", "")));//减法
                if (fenge_4[0] == "0011") tbox_R[y_id].Text = (The_Method.BinaryStrAnd(Y_j.Replace(" ", ""), M_j.Replace(" ", "")));//与
                if (fenge_4[0] == "1010") Move_data(Y_j.Replace(" ", ""), M_j.Replace(" ", "")); //数据传送指令
                if (fenge_4[0] == "0100") Add_one(M_j.Replace(" ", ""));//单操作数加一
                if (fenge_4[0] == "0101" && fenge_4[3] != "100")Sub_one(M_j.Replace(" ", ""));//单操作数减一
                if (fenge_4[0] == "1110") Ldi(M_j.Replace(" ", ""));//载入立即数ldi
                if (fenge_4[0] == "1001") Ld(M_j.Replace(" ", ""));//装载指令ld
            }

            public string buling_16(string s)
            {
                while (s.Length < 17)
                {
                    if (s.Length == 4 || s.Length == 8 || s.Length == 12)
                        s = " " + s;
                    s = "0" + s;
                }
                return s;
            }

            private void btn_danbu_Click(object sender, EventArgs e)
            {
                one_begin();
            }

            private void Form2_Load(object sender, EventArgs e)
            {

            }

        private void initMemory()//主存初始化
        {
            for (int i = 0; i < M.Length; i++)
            {
                M[i] = Convert.ToInt32("0f", 16);
            }
        }
        public int readMemory(int index)
        {
            if (index < 8)
            {
                return Convert.ToInt32(tbox_R[index].Text.Replace(" ", ""), 16); ;
            }
            else
                return M[index];
        }


        public void Add_one(string a)//单操作数加一
        {

            string b = The_Method.BinaryStrAdd_one(a, "1");
            if (ir.Split(" ".ToCharArray()).Length == 5 && ir.Split(" ".ToCharArray())[3] != "000")
                return;
            try
            {
                tbox_R[m_id].Text = Convert.ToString(Convert.ToInt32(b, 2), 16).ToUpper();
            }
            catch
            {
                tbox_R[m_id].Text = b;
            }

        }
        public  void Sub_one(string a)//单操作数减一
        {

            string b = The_Method.BinaryStrSub(Convert.ToString(Convert.ToInt32(a, 16), 2), "1");
            try
            {
                tbox_R[m_id].Text = Convert.ToString(Convert.ToInt32(b, 2), 16).ToUpper();
            }
            catch
            {
                tbox_R[m_id].Text = b;
            }
        }
        public void Move_data(string a, string b)
        {
            try
            {
                tbox_R[y_id].Text = Convert.ToString(Convert.ToInt32(b, 2), 16).ToUpper();
            }
            catch
            {
                tbox_R[y_id].Text = b;
            }

        }
        public  void Ldi(string a)
        {
            tbox_R[single_caozuoqi].Text = Convert.ToString(Convert.ToInt32(a, 2), 16).ToUpper();// 把数送到目的寄存器中
        }

        private void btn_last_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < richTextBox_jiqima.Lines.Length; i++)
                one_begin();
        }

        public void Ld(string a)
        {
            tbox_R[single_caozuoqi].Text = Convert.ToString(Convert.ToInt32(a, 2), 16).ToUpper();
        }
        
        
        
    }
}

