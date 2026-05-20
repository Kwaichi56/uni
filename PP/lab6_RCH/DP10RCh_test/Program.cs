using RC;

var generator = new RCGenerator();
var handler = new RCHandler();
var rch = new RChain(generator, handler);

rch.Use((context, next) =>
{
    context.sum();
    bool rc = next(context);
    return rc;
})
.Use((context, next) =>
{
    context.sub();
    bool rc = next(context);
    return rc;
})
.Use((context, next) =>
{
    bool rc = next(context);
    context.mul();
    return rc;
});

generator.Start(new RCGenerator.Context(3, 5));
Console.WriteLine("--------------------------------------------");
var c = new RCGenerator.Context(10, 5);
generator.Start(c);
generator.Start(new RCGenerator.Context(4, 6, c.Z));

Console.WriteLine();
Console.WriteLine("===== Расширенный тест с методом max() =====");

var generator2 = new RCGenerator();
var handler2 = new RCHandler();
var rch2 = new RChain(generator2, handler2);

rch2.Use((context, next) =>
{
    Console.WriteLine($"[1 -> sum]   before: X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    context.sum();
    Console.WriteLine($"[1 -> sum]   after : X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    bool rc = next(context);
    Console.WriteLine($"[1 <- sum]   return: X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    return rc;
})
.Use((context, next) =>
{
    Console.WriteLine($"[2 -> max]   before: X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    context.max();
    Console.WriteLine($"[2 -> max]   after : X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    bool rc = next(context);
    Console.WriteLine($"[2 <- max]   before: X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    context.max();
    Console.WriteLine($"[2 <- max]   after : X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    return rc;
})
.Use((context, next) =>
{
    Console.WriteLine($"[3 -> sub]   before: X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    context.sub();
    Console.WriteLine($"[3 -> sub]   after : X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    bool rc = next(context);
    Console.WriteLine($"[3 <- sub]   return: X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    return rc;
})
.Use((context, next) =>
{
    Console.WriteLine($"[4 -> step]  pass  : X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    bool rc = next(context);
    Console.WriteLine($"[4 <- mul]   before: X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    context.mul();
    Console.WriteLine($"[4 <- mul]   after : X = {context.X}, Y = {context.Y}, Z = {context.Z}");
    return rc;
});

generator2.Start(new RCGenerator.Context(7, 2));
