using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISLab2
{
    public partial class Form1 : Form
    {
        string[] rez_bit;
        int m;
        public Form1()
        {
            InitializeComponent();
        }
        
        public void Generate_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox3.Clear();
            m = Convert.ToInt32(textBox1.Text);
            MyGenerator gen = new MyGenerator();
            rez_bit =  gen.generate(m);
            for(int i = 0; i < m; i++)
            {
                textBox2.AppendText(rez_bit[i]);
            }
            label3.Text = "Длина последовательности: " + 64 * m; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyGenerator gen = new MyGenerator();
            if (gen.frequency_test(m, rez_bit))
                if(textBox3.Text=="")
                    textBox3.AppendText("Частотный тест пройден.");
                else
                    textBox3.AppendText(Environment.NewLine + "Частотный тест пройден.");
            else
                if (textBox3.Text == "")
                    textBox3.AppendText("Частотный тест не пройден.");
                else
                    textBox3.AppendText(Environment.NewLine + "Частотный тест не пройден.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MyGenerator gen = new MyGenerator();
            if (gen.serial_test(m, rez_bit))
                if (textBox3.Text == "")
                    textBox3.AppendText("Тест на последовательность одинаковых бит пройден.");
                else
                    textBox3.AppendText(Environment.NewLine + "Тест на последовательность одинаковых бит пройден.");
            else
                if (textBox3.Text == "")
                    textBox3.AppendText("Тест на последовательность одинаковых бит не пройден.");
                else
                    textBox3.AppendText(Environment.NewLine + "Тест на последовательность одинаковых бит не пройден.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MyGenerator gen = new MyGenerator();
            if (gen.random_excursions_test(m, rez_bit))
                if (textBox3.Text == "")
                    textBox3.AppendText("Расширенный тест на произвольные отклонения пройден.");
                else
                    textBox3.AppendText(Environment.NewLine + "Расширенный тест на произвольные отклонения пройден.");
            else
                if (textBox3.Text == "")
                    textBox3.AppendText("Расширенный тест на произвольные отклонения не пройден.");
                else
                    textBox3.AppendText(Environment.NewLine + "Расширенный тест на произвольные отклонения не пройден.");
        }

        private void ReadFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                // нашли файл, считали сообщение в файле до конца
                char[] message = sr.ReadToEnd().ToCharArray();
                m = (message.Length / 64);
                int len = message.Length;
                rez_bit = new string[m];
                string tmp = "";
                int k = 0;
                for (int i = 0; i < m; i++)
                { 
                    for(int j = k; j < k+64; j++)
                    {
                        tmp += message[j].ToString();
                    }
                    rez_bit[i] = tmp;
                    while(rez_bit[i].Length != 64)
                    {
                        rez_bit[i] += '0';
                    }
                    tmp = "";
                    k = k + 64;
                }
                sr.Close();
                if (rez_bit.Length <= 0) {
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox2.AppendText("Неверная последовательность! ");
                   
                }
                else
                {
                    // записали результат на экран
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox1.Text = Convert.ToString(m);
                    for (int i = 0; i < m; i++)
                    {
                        textBox2.AppendText(rez_bit[i]);
                    }
                    label3.Text = "Длина последовательности: " + 64 * m;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                // записываем результат
                sw.Write(textBox2.Text);
                sw.Close();
            }
        }
    }
}
