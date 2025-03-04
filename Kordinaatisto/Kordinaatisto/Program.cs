using System;

struct Koordinaatti
{
    public readonly int X;
    public readonly int Y;

    public Koordinaatti(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool OnVieressa(Koordinaatti toinen)
    {
        int dx = Math.Abs(X - toinen.X);
        int dy = Math.Abs(Y - toinen.Y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }
}

class Program
{
    static void Main(string[] args) 
    {
        Koordinaatti keskipiste = new Koordinaatti(0, 0);
        Koordinaatti[] testit = {
            new Koordinaatti(-1, -1),
            new Koordinaatti(-1, 0),
            new Koordinaatti(-1, 1),
            new Koordinaatti(0, -1),
            new Koordinaatti(0, 0),
            new Koordinaatti(0, 1),
            new Koordinaatti(1, -1),
            new Koordinaatti(1, 0),
            new Koordinaatti(1, 1)
        };

        foreach (var k in testit)
        {
            if (k.X == 0 && k.Y == 0)
            {
                Console.WriteLine($"Annettu koordinaatti {k.X},{k.Y} on koordinaatissa 0,0.");
            }
            else if (k.OnVieressa(keskipiste))
            {
                Console.WriteLine($"Annettu koordinaatti {k.X},{k.Y} on koordinaatin 0,0 vieressä.");
            }
        }

        Console.ReadLine(); 
    }
}
