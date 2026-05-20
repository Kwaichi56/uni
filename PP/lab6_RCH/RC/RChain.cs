using IRC;

namespace RC;

public class RChain : IResponsibilityChain<RCGenerator.Context>
{
    private readonly IRCGenerator<RCGenerator.Context> generator;
    private readonly IRCHandler<RCGenerator.Context> handler;
    private readonly List<Func<RCGenerator.Context, Func<RCGenerator.Context, bool>, bool>> pipeline = new();

    public RChain(IRCGenerator<RCGenerator.Context> gen, IRCHandler<RCGenerator.Context> h)
    {
        generator = gen;
        handler = h;
        generator.Next = Execute;
    }

    public IResponsibilityChain<RCGenerator.Context> Use(Func<RCGenerator.Context, Func<RCGenerator.Context, bool>, bool> step)
    {
        pipeline.Add(step);
        return this;
    }

    private bool Execute(RCGenerator.Context ctx)
    {
        int index = 0;

        bool NextStep(RCGenerator.Context c)
        {
            bool rc = false;
            if (index >= pipeline.Count) handler.LastUse(c);
            else rc = pipeline[index++](c, NextStep);
            return rc;
        }

        return NextStep(ctx);
    }
}
