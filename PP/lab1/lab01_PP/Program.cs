using DP001_lib;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("--------------------------------------------");

        Calc calc1 = new Calc();
        int x = 6, y = 4;
        Console.WriteLine("calc.{0}({1},{2}) = {3}", "sum", x, y, calc1.sum(x, y));
        Console.WriteLine("calc.{0}({1},{2}) = {3}", "sub", x, y, calc1.sub(x, y));
        Console.WriteLine("calc.{0}({1},{2}) = {3}", "mul", x, y, calc1.mul(x, y));
        Console.WriteLine("calc.{0}({1},{2}) = {3}", "div", x, y, calc1.div(x, y));
        Console.WriteLine("calc.{0}({1},{2}) = {3}", "div", x, y, calc1.mod(x, y));

        Console.WriteLine("--------------------------------------------");
        Calc calc2 = new Calc(7);
        Console.WriteLine("calc2.result = {0}", calc2.result);
        Console.WriteLine(
                           "calc2.sum(5).sub(4).mul(2).div(3).mod(4).result = {0}",
                            calc2.sum(5).sub(4).mul(2).div(3).mod(4).result
                         );
        Calc calc3 = new Calc(10);
        Console.WriteLine("calc3.result = {0}", calc3.result);
        Console.WriteLine(
                           "calc3.mul(2).div(3).sum(4).mod(4).result = {0}",
                            calc3.mul(2).div(3).sum(4).mod(4).result
                         );

        Console.WriteLine("--------------------------------------------");

        var builder1 = Calc.getBuilder();
        ICalcResult calcresult1 = builder1.Sum(10).Sum(15).Mul(3).Build();
        var builder2 = Calc.getBuilder();
        ICalcResult calcresult2 = builder2.Mul(2).Mul(2).Sum(3).Build();
        ICalcResult calcresult3 = builder2.Mul(4).Mul(4).Div(2).Mod(3).Build();

        int r1 = calcresult1.result(10);   // ((10+10)+15)*3 = 105
        int r1x = calcresult1.result(-10); // ((-10+10)+15)*3 = 45
        int r2 = calcresult2.result(1);    // ((1*2)*2)    + 3  = 7
        int r2x = calcresult2.result(-1);  // (((-1)*2)*2) + 3 = -1
        int r3 = calcresult3.result(5);   // (((5*4)*4)/2)%3 = 1

        Console.WriteLine("r1  = {0}", r1);
        Console.WriteLine("r1x = {0}", r1x);
        Console.WriteLine("r2  = {0}", r2);
        Console.WriteLine("r2x = {0}", r2x);
        Console.WriteLine("r3  = {0}", r3);
        Console.ReadKey();
    }
}





