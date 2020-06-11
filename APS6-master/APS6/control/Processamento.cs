
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace APS6.control
{
    class Processamento
    {
        public Bitmap temp;
        public Bitmap novo;
        public Bitmap colorido;
        public Bitmap preProcess(Bitmap imagem)
        {
            temp = imagem;
            var retangulo = new Rectangle(0, 0, temp.Width, temp.Height);
            novo = new Bitmap(temp.Width, temp.Height);
            Color cor;
            int rgb;
            for (int i = 0; i < temp.Width; i++)
            {
                for (int j = 0; j < temp.Height; j++)
                {
                    cor = temp.GetPixel(i, j);
                    rgb = (int)((.299 * cor.R) + (.587 * cor.G) + (.114 * cor.B));
                    novo.SetPixel(i, j, Color.FromArgb(rgb, rgb, rgb));
                }
            }
            return novo;
        }
        public Bitmap binarizar(Bitmap imagem)
        {
            temp = (Bitmap)imagem;
            Color cor;
            int rgb;
            var retangulo = new Rectangle(0, 0, temp.Width, temp.Height);
            for (int i = 0; i < imagem.Width; i++)
            {
                for (int j = 0; j < imagem.Height; j++)
                {
                    cor = imagem.GetPixel(i, j);
                    rgb = (int)((cor.R) + (cor.G) + (cor.B) / 3);
                    if (rgb < 40)
                    {
                        temp.SetPixel(i, j, Color.Black);
                    }
                }
            }
            novo = temp.Clone(retangulo, PixelFormat.Format1bppIndexed);
            return novo;

        }
        public Bitmap afinar(Bitmap imagem)
        {
            bool[][] t = toBool(imagem);
            t = ZhangSuenThinning(t);
            temp = (Bitmap)toImage(t);
            bool[][] tu = toBool(temp);
            tu = ZhangSuenThinning(tu);
            temp = (Bitmap)toImage(tu);

            return temp;
        }
        public String extrairMinucias(Bitmap imagem)
        {
            String enviaBd = "";
            Color[] p = new Color[8];
            Rectangle ret = new Rectangle(0, 0, imagem.Width, imagem.Height);
            colorido = imagem.Clone(ret, PixelFormat.Format32bppArgb);
            for (int j = 2; j <= (imagem.Height - 2); j++)
            {
                for (int i = 2; i <= (imagem.Width - 2); i++)
                {
                    Color cor = imagem.GetPixel(i, j);
                    String teste = cor.Name;
                    if (teste == "ff000000")
                    {
                        p[0] = imagem.GetPixel(i, (j - 1));
                        p[1] = imagem.GetPixel((i + 1), (j - 1));
                        p[2] = imagem.GetPixel(i + 1, j);
                        p[3] = imagem.GetPixel(i + 1, j + 1);
                        p[4] = imagem.GetPixel(i, j + 1);
                        p[5] = imagem.GetPixel(i - 1, j + 1);
                        p[6] = imagem.GetPixel(i - 1, j);
                        p[7] = imagem.GetPixel(i - 1, j - 1);
                        int seMinucia = verificaMinucia(p);
                        Console.WriteLine(seMinucia);
                        if (seMinucia == 1)
                        {
                            enviaBd = (enviaBd + "T" + i + j);
                            colorido.SetPixel(i, j, Color.Green);
                        }
                        else if (seMinucia == 2)
                        {
                            enviaBd = (enviaBd + "B" + i + j);
                            colorido.SetPixel(i, j, Color.Purple);
                        }
                    }
                }
            }
            return enviaBd;
        }
        public Bitmap centro(Bitmap imagem)
        {

            int areax = 75;
            int areay = 75;
            temp = (Bitmap)imagem;
            int x = temp.Width / 2 - areax / 2;
            int y = temp.Height / 2 - areay / 2;
            Bitmap croppedBitmap = CropBitmap(temp, x, y, 75, 75);
            return croppedBitmap;
        }
        public Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);
            return cropped;
        }
        public static bool[][] toBool(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            bool[][] s = new bool[bmp.Height][];
            for (int y = 0; y < bmp.Height; y++)
            {
                s[y] = new bool[bmp.Width];
                for (int x = 0; x < bmp.Width; x++)
                    s[y][x] = bmp.GetPixel(x, y).GetBrightness() < 0.3;
            }
            return s;

        }


        public static Image toImage(bool[][] s)
        {
            Bitmap bmp = new Bitmap(s[0].Length, s.Length);
            using (Graphics g = Graphics.FromImage(bmp)) g.Clear(Color.White);
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                    if (s[y][x]) bmp.SetPixel(x, y, Color.Black);

            return (Bitmap)bmp;
        }
        public static T[][] ArrayClone<T>(T[][] A)
        { return A.Select(a => a.ToArray()).ToArray(); }
        public static bool[][] ZhangSuenThinning(bool[][] s)
        {
            bool[][] temp = ArrayClone(s);  // make a deep copy to start.. 
            int count = 0;
            do  // the missing iteration
            {
                count = step(1, temp, s);
                temp = ArrayClone(s);      // ..and on each..
                count += step(2, temp, s);
                temp = ArrayClone(s);      // ..call!
            }
            while (count > 0);

            return s;
        }

        static int step(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (SuenThinningAlg(a, b, temp, stepNo == 2))
                    {
                        // still changes happening?
                        if (s[a][b]) count++;
                        s[a][b] = false;
                    }
                }
            }
            return count;
        }

        static bool SuenThinningAlg(int x, int y, bool[][] s, bool even)
        {
            bool p2 = s[x][y - 1];
            bool p3 = s[x + 1][y - 1];
            bool p4 = s[x + 1][y];
            bool p5 = s[x + 1][y + 1];
            bool p6 = s[x][y + 1];
            bool p7 = s[x - 1][y + 1];
            bool p8 = s[x - 1][y];
            bool p9 = s[x - 1][y - 1];


            int bp1 = NumberOfNonZeroNeighbors(x, y, s);
            if (bp1 >= 2 && bp1 <= 6) //2nd condition
            {
                if (NumberOfZeroToOneTransitionFromP9(x, y, s) == 1)
                {
                    if (even)
                    {
                        if (!((p2 && p4) && p8))
                        {
                            if (!((p2 && p6) && p8))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (!((p2 && p4) && p6))
                        {
                            if (!((p4 && p6) && p8))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        static int NumberOfZeroToOneTransitionFromP9(int x, int y, bool[][] s)
        {
            bool p2 = s[x][y - 1];
            bool p3 = s[x + 1][y - 1];
            bool p4 = s[x + 1][y];
            bool p5 = s[x + 1][y + 1];
            bool p6 = s[x][y + 1];
            bool p7 = s[x - 1][y + 1];
            bool p8 = s[x - 1][y];
            bool p9 = s[x - 1][y - 1];

            int A = Convert.ToInt32((!p2 && p3)) + Convert.ToInt32((!p3 && p4)) +
                    Convert.ToInt32((!p4 && p5)) + Convert.ToInt32((!p5 && p6)) +
                    Convert.ToInt32((!p6 && p7)) + Convert.ToInt32((!p7 && p8)) +
                    Convert.ToInt32((!p8 && p9)) + Convert.ToInt32((!p9 && p2));
            return A;
        }
        static int NumberOfNonZeroNeighbors(int x, int y, bool[][] s)
        {
            int count = 0;
            if (s[x - 1][y]) count++;
            if (s[x - 1][y + 1]) count++;
            if (s[x - 1][y - 1]) count++;
            if (s[x][y + 1]) count++;
            if (s[x][y - 1]) count++;
            if (s[x + 1][y]) count++;
            if (s[x + 1][y + 1]) count++;
            if (s[x + 1][y - 1]) count++;
            return count;
        }
        public int verificaMinucia(Color[] pixels)
        {
            int retorno = 0;
            bool[] bits = new bool[8];
            int minucias = 0;
            int par = 0;
            int impar = 0;
            int[] cpar = new int[4];
            int[] cimpar = new int[4];
            for (int i = 0; i < pixels.Length; i++)
            {
                String esta;
                esta = pixels[i].Name;
                if (pixels[i].Name.Equals("ff000000"))
                {
                    if (i == 0)
                    {
                        cpar[par] = i;
                        par++;
                    }
                    else if (i % 2 == 0)
                    {
                        cpar[par] = i;
                        par++;
                    }
                    else
                    {
                        cimpar[impar] = i;
                        impar++;
                    }
                    bits[i] = true;
                    minucias++;
                }
                else bits[i] = false;
            }
            if (minucias == 1)
            {
                retorno = 1;
            }
            else if (minucias == 2)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (bits[c] == true)
                    {
                        int prime = c - 1;
                        int seg = c + 1;
                        if (prime < 0) prime = 7;
                        if (seg > 7) seg = 0;

                        if (bits[prime] == true || bits[seg] == true)
                        {
                            retorno = 1;
                            c = 8;
                        }
                    }
                }
            }
            else if (par == 3)
            {
                retorno = 2;
            }
            else if (impar == 3)
            {
                retorno = 2;
            }
            else if (minucias > 2 && minucias < 6)
            {
                if (par == 2 && minucias > 2)
                {
                    if (cpar[0] == cpar[1] - 4)
                    {
                        int verimpar = cpar[0] - 1;
                        int verimpar2 = cpar[0] + 1;
                        int verimpar3 = cpar[1] - 1;
                        int verimpar4 = cpar[1] + 1;
                        if (verimpar < 0) verimpar = 7;
                        if (bits[verimpar] == true && bits[verimpar2] == true)
                        {
                            retorno = 2;
                        }
                        else if (bits[verimpar3] == true && bits[verimpar4] == true)
                        {
                            retorno = 2;
                        }
                        else retorno = 0;
                    }
                    else
                    {
                        int conta = cpar[1] + 3;
                        if (conta > 7)
                        {
                            if (cpar[0] == 0)
                            {
                                if (conta == 9) conta = 3;
                            }else if (conta == 9) conta = 1;
                        }
                        if (bits[conta] == true)
                        {
                            retorno = 2;
                        }
                    }
                }
                else if (impar == 2 && minucias > 2)
                {
                    if (cimpar[0] == cimpar[1] - 4)
                    {
                        int verpar = cimpar[0] - 1;
                        int verpar2 = cimpar[0] + 1;
                        int verpar3 = cimpar[1] - 1;
                        int verpar4 = cimpar[1] + 1;
                        if (verpar4 > 7) verpar4 = 0;
                        if (bits[verpar] && bits[verpar2])
                        {
                            retorno = 2;
                        }
                        else if (bits[verpar3] && bits[verpar4])
                        {
                            retorno = 2;
                        }
                        else retorno = 0;
                    }
                    else
                    {
                        int conta2 = cimpar[1] + 3;
                        if (conta2 == 8) conta2 = 1;
                        else if (conta2 == 10)
                        {
                            if (cimpar[0] == 1) conta2 = 4;
                            else conta2 = 2;
                        }
                        if (bits[conta2] == true)
                        {
                            retorno = 2;
                        }
                    }
                }
            }
            return retorno;
        }
    }
}
