using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ISLab2
{
    public class MyGenerator
    {
        ulong[] rez; //64-битное целое представление слов последовательности
        string[] rez_bit;//двоичное представление слов последовательности

        public  ulong trippleDes(ulong toEncypt)
        {
            //преобразуем сообщение в массив байтов
            byte[] toEncArray = BitConverter.GetBytes(toEncypt);
            //инициализурем объект-оболочку для доступа к реализации 3DES
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = CipherMode.ECB;
            //создаем объект-шифратор
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //Результат
            byte[] resultArray = cTransform.TransformFinalBlock(toEncArray, 0, toEncArray.Length);
            tdes.Clear();
            //Возращаем результат 64-битном двоичном словом
            return BitConverter.ToUInt64(resultArray, 0);
     
        }

        public  void  writeFileSeq(string[] seq,int m)
        {
            StreamWriter sw = new StreamWriter("sequence.txt");
            for(int i = 0; i < m; i++)
            {
                sw.WriteLine(seq[i]);
            }
            sw.Close();
        }
        public string[] generate(int m)
        {
            rez = new ulong[m];
            rez_bit = new string[m];

            //Шаг 1
            ulong d = (ulong)DateTime.Now.ToBinary();//текущее дата в двоичном форамате
            ulong temp = trippleDes(d);
            ulong S0 = 1;
            ulong s_p = S0;
            //Шаг 2 
            for(int i = 0; i < m; i++)
            {
                rez[i] = trippleDes(temp ^ s_p);
                s_p = trippleDes(rez[i] ^ temp);
            }
            //Шаг 3
            for(int i = 0; i < m; i++)
            {
                //переводим каждый Х в двоичное представление, затем в строку
                rez_bit[i] = Convert.ToString((long)rez[i], 2);
                while (rez_bit[i].Length < 64)
                {
                    //заполняем старшие разряды нулями, если они непусты
                    rez_bit[i] = "0" + rez_bit[i];
                }
            }
            writeFileSeq(rez_bit,m);

            return rez_bit;
        }

        public bool frequency_test(int m,string[] rez_bit)
        {
            int[] X = new int[64 * m];

            //Шаг 1([0,1]->[-1,1])
            for(int i=0;i<m;i++)
                for(int j = 0; j < 64; j++)
                {
                    //Берем j-й элемент из слова i и преобразуем в int32
                    X[i * 64 + j] = Convert.ToInt32(rez_bit[i].Substring(j, 1));
                    //Преобразуем в последовательность -1,1
                    X[i * 64 + j] = (2 * X[i * 64 + j] - 1);
                }
            //Шаг 2 (вычисление суммы)
            int n = X.Length;
            double S = 0;
            for (int i = 0; i < n; i++)
            {
                S += X[i];
            }

            //Шаг3 (вычисление статистики)
            S = Math.Abs(S) / Math.Sqrt(n);

            //Шаг 4. Проверка гипотезы о случайности
            if (S <= 1.82138636)
                return true;

            else
                return false;
        }

        public bool serial_test(int m, string[] rez_bit)
        {
            byte[] X = new byte[64 * m];
            // преобразование последовательности из [0, 1] в [-1, 1]
            for (int i = 0; i < m; i++)
                for (int j = 0; j < 64; j++)
                    X[i * 64 + j] = Convert.ToByte(rez_bit[i].Substring(j, 1));
            //Шаг 1 (вычисляем частоту, с которой в послед встречается 1
            double p = 0;
            for(int i = 0; i < 64 * m; i++)
                if (X[i] == 1)
                    p++;
             p = p / (64 * m);
            
            //Шаг 2 (вычисление частоту несовпадения последующего элемента с текущим)
            int v = 0;
            for (int i = 0; i < 64 * m - 1; i++)
                if (X[i] != X[i + 1])
                    v++;
            v += 1;
            // Шаг 3 (вычисление статистики)
            int n = 64 * m;
            double S = Math.Abs(v - 2 * n * p * (1 - p)) / (2 * Math.Sqrt(2 * n) * p * (1 - p));
            
            // Шаг 4 (проверка гипотезы о случайности)
            if (S <= 1.82138636)
                return true;
            else
                return false;
        }
        public bool random_excursions_test(int m, string[] rez_bit)
        {
            int n = 64 * m;
            int[] X = new int[64 * m];
            //Шаг 1 ([0, 1] -> [-1, 1])
            for (int i=0;i<m;i++)
                for(int j = 0; j < 64; j++)
                {
                    X[i * 64 + j] = Convert.ToInt32(rez_bit[i].Substring(j, 1));
                    X[i * 64 + j] = (2 * X[i * 64 + j] - 1);
                }
            // Шаг 2 (вычисление сумм последовательно удлиняющихся последовательностей)
            int[] S = new int[n];
            for(int i = 0; i < n; i++)
            {
                S[i] = 0;
                for(int j = 0; j < i + 1; j++)
                {
                    S[i] += X[j];
                }
            }

            // Шаг 3 (добавление '0' в начало и конец последовательности сумм)
            int[] S_new = new int[n + 2];
            S_new[0] = 0;
            for (int i = 1; i < n + 1; i++)
                S_new[i] = S[i - 1];
            S_new[n + 1] = 0;

            //Шаг 4 (подсчет 0 в полученной послед.)
            int k = 0;
            for (int i = 0; i < n + 2; i++)
                if (S_new[i] == 0)
                    k++;
            int L = k - 1;
            // Шаг 5 (вычисление частоты срабатывания каждого из 18 состояний j)
            // (отклонения от ожидаемого посещения)
            int[] ksi = new int[18];
            for (int i = 0; i < n + 2; i++)
            {
                switch (S_new[i])
                {
                    case -9: ksi[0]++; break;
                    case -8: ksi[1]++; break;
                    case -7: ksi[2]++; break;
                    case -6: ksi[3]++; break;
                    case -5: ksi[4]++; break;
                    case -4: ksi[5]++; break;
                    case -3: ksi[6]++; break;
                    case -2: ksi[7]++; break;
                    case -1: ksi[8]++; break;

                    case 1: ksi[9]++; break;
                    case 2: ksi[10]++; break;
                    case 3: ksi[11]++; break;
                    case 4: ksi[12]++; break;
                    case 5: ksi[13]++; break;
                    case 6: ksi[14]++; break;
                    case 7: ksi[15]++; break;
                    case 8: ksi[16]++; break;
                    case 9: ksi[17]++; break;
                    default:; break;
                }
            }
            // Шаг 6 (вычисление статистик для каждого из состояний)
            double[] Y = new double[18];
            int[] j_ = { -9, -8, -7, -6, -5, -4, -3, -2, -1, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < 18; i++)
                Y[i] = Math.Abs(ksi[i] - L) / Math.Sqrt(2 * L * (4 * Math.Abs(j_[i]) - 2));
            bool rez = true;
            // Шаг 7 (проверка всех статистик, если хоть 1 не проходит условие - тест провален)
            for(int i =0; i<18 &&rez; i++)
            {
                if (Y[i] > 1.82138636)
                    rez = false;
            }

            return rez;
        }

    }
}
