using pp00Lib;

namespace pp00
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Class1 first = new Class1();

            Console.WriteLine("введите строку s");
            string s = Console.ReadLine();

            Console.WriteLine("введите строку b");
            string b = Console.ReadLine();

            string upper = first.Up(s);
            Console.WriteLine(upper);

            string con = first.Concat(s, b);
            Console.WriteLine(con);

            int l = first.Len(s);
            Console.WriteLine(l);

            string inf = Class1.GetInFo();
            Console.WriteLine(inf);

        }
    }
}
