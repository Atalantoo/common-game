using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Commons.Test;
using Commons.Lang;
using Commons.Game;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Diagnostics;

namespace Test
{
    [TestClass]
    public class PerformanceTest
    {
        static int batch_size = 100000;
        static string[][] inp = Arg();

        static string[][] Arg()
        {
            string[][] inp = new string[1][];
            inp[0] = new string[] { "4", "4" };
            return inp;
        }

        [TestMethod]
        public void Test_Impl1Object()
        {
            Batch(new Impl1Object());
        }

        [TestMethod]
        public void Test_Impl2Lambda()
        {
            Batch(new Impl2Lambda());
        }

        [TestMethod]
        public void Test_Impl3Function()
        {
            Batch(new Impl3Function());
        }

        [TestMethod]
        public void Test_Impl4Delegate()
        {
            Batch(new Impl4Delegate());
        }

        private static void Batch(IImpl impl)
        {
            for (int i = 0; i < batch_size; i++)
            {
                string[][] res = impl.DoTurn(inp);
                Asserts(res);
            }
        }

        private static void Asserts(string[][] res)
        {
            Assert.IsNotNull(res);
            Assert.AreEqual(4, res.Length);
            Assert.AreEqual(4, res[0].Length);
            Assert.AreEqual(4, res[1].Length);
            Assert.AreEqual(4, res[2].Length);
            Assert.AreEqual(4, res[3].Length);
            Assert.IsTrue(14 <= Count(res, "0"));
            Assert.AreEqual(2, Count(res, "2"));
        }

        private static int Count(string[][] matrix, string match)
        {
            int res = 0;
            for (int y = 0; y < matrix.Length; y++)
                for (int x = 0; x < matrix[0].Length; x++)
                    if (match.Equals(matrix[y][x]))
                        res++;
            return res;
        }
    }

    internal interface IImpl
    {
        string[][] DoTurn(string[][] input);
    }

    public class Impl1Object : IImpl
    {
        string[][] input;
        int width;
        int height;
        string[][] matrix;

        public string[][] DoTurn(string[][] newInput)
        {
            input = newInput;
            Update_size();
            Fill_with_zeros_items();
            Create_random_item();
            Create_random_item();
            return matrix;
        }

        void Update_size()
        {
            width = Int32.Parse(input[0][0]);
            height = Int32.Parse(input[0][1]);
        }

        void Fill_with_zeros_items()
        {
            matrix = new string[height][];
            for (int y = 0; y < height; y++)
            {
                matrix[y] = new string[width];
                for (int x = 0; x < width; x++)
                    matrix[y][x] = "0";
            }
        }

        void Create_random_item()
        {
            int[][] emptyItems = GetEmptyItems(matrix);
            Random rnd = new Random();
            int nextOne = rnd.Next(0, emptyItems.Length);
            matrix[emptyItems[nextOne][0]][emptyItems[nextOne][1]] = "2";
        }

        static int[][] GetEmptyItems(string[][] matrix)
        {
            int[][] res = new int[0][];
            string match = "0";
            for (int y = 0; y < matrix.Length; y++)
                for (int x = 0; x < matrix[0].Length; x++)
                    if (match.Equals(matrix[y][x]))
                        res = Arrays.Add(res, new int[] { y, x });
            return res;
        }
    }

    public class Impl2Lambda : IImpl
    {
        string[][] input;
        int width;
        int height;
        string[][] matrix;
        Game rules;

        public Impl2Lambda()
        {
            rules = Games.Get("SinglePlayer Game")
                   .Start(Turns.Get("Starting the Game")
                       .Start(Phases.Get("Setup phase")
                           .Next(Steps.Get(Update_size))
                           .Next(Steps.Get(Fill_with_zeros_items))
                           .Next(Steps.Get(Create_random_item))
                           .Next(Steps.Get(Create_random_item))))
               .Build();
        }

        public string[][] DoTurn(string[][] newInput)
        {
            input = newInput;
            rules.Turn("Starting the Game").Execute();
            return matrix;
        }

        void Update_size()
        {
            width = Int32.Parse(input[0][0]);
            height = Int32.Parse(input[0][1]);
        }

        void Fill_with_zeros_items()
        {
            matrix = new string[height][];
            for (int y = 0; y < height; y++)
            {
                matrix[y] = new string[width];
                for (int x = 0; x < width; x++)
                    matrix[y][x] = "0";
            }
        }

        void Create_random_item()
        {
            int[][] emptyItems = GetEmptyItems(matrix);
            Random rnd = new Random();
            int nextOne = rnd.Next(0, emptyItems.Length);
            matrix[emptyItems[nextOne][0]][emptyItems[nextOne][1]] = "2";
        }

        static int[][] GetEmptyItems(string[][] matrix)
        {
            int[][] res = new int[0][];
            string match = "0";
            for (int y = 0; y < matrix.Length; y++)
                for (int x = 0; x < matrix[0].Length; x++)
                    if (match.Equals(matrix[y][x]))
                        res = Arrays.Add(res, new int[] { y, x });
            return res;
        }
    }

    public class Impl3Function : IImpl
    {

        public string[][] DoTurn(string[][] newInput)
        {
            return Create_random_item.Invoke(
                Create_random_item.Invoke(
                    Fill_with_zeros_items.Invoke(
                        Update_size.Invoke(newInput))));
        }

        Func<string[][], int[]> Update_size = i =>
         new int[] { Int32.Parse(i[0][0]), Int32.Parse(i[0][1]) };

        Func<int[], string[][]> Fill_with_zeros_items = i =>
        {
            string[][] o = new string[i[0]][];
            for (int y = 0; y < i[0]; y++)
            {
                o[y] = new string[i[01]];
                for (int x = 0; x < i[01]; x++)
                    o[y][x] = "0";
            }
            return o;
        };
        Func<string[][], string[][]> Create_random_item = i =>
        {
            int[][] emptyItems = GetEmptyItems(i);
            Random rnd = new Random();
            int nextOne = rnd.Next(0, emptyItems.Length);
            i[emptyItems[nextOne][0]][emptyItems[nextOne][1]] = "2";
            return i;
        };

        static int[][] GetEmptyItems(string[][] matrix)
        {
            int[][] res = new int[0][];
            string match = "0";
            for (int y = 0; y < matrix.Length; y++)
                for (int x = 0; x < matrix[0].Length; x++)
                    if (match.Equals(matrix[y][x]))
                        res = Arrays.Add(res, new int[] { y, x });
            return res;
        }
    }

    public class Impl4Delegate : IImpl
    {
        string[][] input;
        int width;
        int height;
        string[][] matrix;

        delegate void Turn();
        delegate void Phase();
        delegate void Step();

        Action<string[][]> actionStart;
        Action<string[][]> actionTurn;

        public Impl4Delegate()
        {
            actionStart = (i) =>
            { // Starting_the_Game_Turn
                { // Setup_Phase
                    input = i;
                    Update_size();
                    Fill_with_zeros_items();
                    Create_random_item();
                    Create_random_item();
                }
            };
            actionTurn = (i) =>
            { // Starting_the_Game
                { // Setup Phase
                    input = i;
                    Update_size();
                    Fill_with_zeros_items();
                    Create_random_item();
                    Create_random_item();
                }
            };
        }

        public string[][] DoTurn(string[][] i)
        {
            actionStart.Invoke(i);
            return matrix;
        }

        void Update_size()
        {
            width = Int32.Parse(input[0][0]);
            height = Int32.Parse(input[0][1]);
        }

        void Fill_with_zeros_items()
        {
            matrix = new string[height][];
            for (int y = 0; y < height; y++)
            {
                matrix[y] = new string[width];
                for (int x = 0; x < width; x++)
                    matrix[y][x] = "0";
            }
        }

        void Create_random_item()
        {
            int[][] emptyItems = GetEmptyItems(matrix);
            Random rnd = new Random();
            int nextOne = rnd.Next(0, emptyItems.Length);
            matrix[emptyItems[nextOne][0]][emptyItems[nextOne][1]] = "2";
        }

        static int[][] GetEmptyItems(string[][] matrix)
        {
            int[][] res = new int[0][];
            string match = "0";
            for (int y = 0; y < matrix.Length; y++)
                for (int x = 0; x < matrix[0].Length; x++)
                    if (match.Equals(matrix[y][x]))
                        res = Arrays.Add(res, new int[] { y, x });
            return res;
        }

    }
}
