using System;

enum PääRaakaAine
{
    Nautaa,
    Kanaa,
    Kasviksia
}

enum Lisuke
{
    Perunaa,
    Riisiä,
    Pastaa
}

enum Kastike
{
    Pippuri,
    Chili,
    Tomaatti,
    Curry
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Pääraaka-aine (nautaa, kanaa, kasviksia):");
        PääRaakaAine pääaine = ValitseRaakaAine<PääRaakaAine>();

        Console.WriteLine("Lisukkeet (perunaa, riisiä, pastaa):");
        Lisuke lisuke = ValitseRaakaAine<Lisuke>();

        Console.WriteLine("Kastike (pippuri, chili, tomaatti, curry):");
        Kastike kastike = ValitseRaakaAine<Kastike>();

        Console.WriteLine($"\n{pääaine} ja {lisuke} {kastike}-kastikkeella");
    }

    static T ValitseRaakaAine<T>() where T : Enum
    {
        while (true)
        {
            string syöte = Console.ReadLine()?.Trim().ToLower();
            foreach (T arvo in Enum.GetValues(typeof(T)))
            {
                if (syöte == arvo.ToString().ToLower())
                {
                    return arvo;
                }
            }
            Console.WriteLine("Virheellinen syöte. Yritä uudelleen:");
        }
    }
}
