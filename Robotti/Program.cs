using System;

public interface IRobottiKäsky
{
    void Suorita(Robotti robotti);
}

public class Käynnistä : IRobottiKäsky
{
    public void Suorita(Robotti robotti)
    {
        robotti.OnKäynnissä = true;
    }
}

public class Sammuta : IRobottiKäsky
{
    public void Suorita(Robotti robotti)
    {
        robotti.OnKäynnissä = false;
    }
}

public class YlösKäsky : IRobottiKäsky
{
    public void Suorita(Robotti robotti)
    {
        if (robotti.OnKäynnissä) robotti.Y += 1;
    }
}

public class AlasKäsky : IRobottiKäsky
{
    public void Suorita(Robotti robotti)
    {
        if (robotti.OnKäynnissä) robotti.Y -= 1;
    }
}

public class VasenKäsky : IRobottiKäsky
{
    public void Suorita(Robotti robotti)
    {
        if (robotti.OnKäynnissä) robotti.X -= 1;
    }
}

public class OikeaKäsky : IRobottiKäsky
{
    public void Suorita(Robotti robotti)
    {
        if (robotti.OnKäynnissä) robotti.X += 1;
    }
}

public class Robotti
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool OnKäynnissä { get; set; }
    public IRobottiKäsky?[] Käskyt { get; } = new IRobottiKäsky?[3];

    public void Suorita()
    {
        foreach (IRobottiKäsky? käsky in Käskyt)
        {
            käsky?.Suorita(this);
            Console.WriteLine($"Robotti: [{X} {Y} {OnKäynnissä}]");
        }
    }
}

class Program
{
    static void Main()
    {
        Robotti robotti = new Robotti();
        string[] sallitutKomennot = { "Käynnistä", "Sammuta", "Ylös", "Alas", "Oikea", "Vasen" };

        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine("Mitä komentoa syötetään robotille? Vaihtoehdot: Käynnistä, Sammuta, Ylös, Alas, Oikea, Vasen.");
            string syote = Console.ReadLine()?.Trim();

            while (!Array.Exists(sallitutKomennot, k => k.Equals(syote, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Virheellinen komento. Yritä uudelleen:");
                syote = Console.ReadLine()?.Trim();
            }

            robotti.Käskyt[i] = syote switch
            {
                "Käynnistä" => new Käynnistä(),
                "Sammuta" => new Sammuta(),
                "Ylös" => new YlösKäsky(),
                "Alas" => new AlasKäsky(),
                "Oikea" => new OikeaKäsky(),
                "Vasen" => new VasenKäsky(),
                _ => null
            };
        }

        robotti.Suorita();
    }
}
