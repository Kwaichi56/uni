using IRC;

namespace RC;

public class RCHandler : IRCHandler<RCGenerator.Context>
{
    public void LastUse(RCGenerator.Context context)
    {
        Console.WriteLine("LastUse: X = {0}, Y = {1}, Z = {2}", context.X, context.Y, context.Z);
    }
}
