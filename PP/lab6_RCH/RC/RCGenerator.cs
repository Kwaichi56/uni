using IRC;

namespace RC;

public class RCGenerator : IRCGenerator<RCGenerator.Context>
{
    public class Context
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public Context(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Context() : this(0, 0, 0) { }

        public int sum() => Z += X + Y;
        public int sub() => Z += X - Y;
        public int mul() => Z += X * Y;
        public int max() => Z += Math.Max(X, Y);
    }

    public Func<Context, bool>? Next { get; set; } = null;

    public void Start(Context context)
    {
        Next?.Invoke(context);
        Console.WriteLine("Start: X = {0}, Y = {1}, Z = {2}", context.X, context.Y, context.Z);
    }
}
