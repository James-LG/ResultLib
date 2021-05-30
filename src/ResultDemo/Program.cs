using ResultLib;
using System;

namespace ResultDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var result = SimpleResult(1).ContinueWith((ok) =>
            {
                Console.WriteLine("Hello from inside!");
                return SimpleResult(ok);
            });
        }

        private static Result<int, string> SimpleResult(int i)
        {
            if (i == 0)
            {
                return Result<int, string>.FromError("invalid integer");
            }
            else
            {
                return Result<int, string>.FromOk(i - 1);
            }
        }
    }
}
