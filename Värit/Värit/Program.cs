using System;

public class Tavara
{
    public override string ToString()
    {
        return GetType().Name;
    }
}

public class Miekka : Tavara { }
public class Jousi : Tavara { }
public class Kirves : Tavara { }

public class VaritettyTavara<T> where T : Tavara
{
    private T tavara;
    private ConsoleColor vari;

    public VaritettyTavara(T tavara, ConsoleColor vari)
    {
        this.tavara = tavara;
        this.vari = vari;
    }

    public void NaytaTavara()
    {
        Console.ForegroundColor = vari;
        Console.WriteLine(tavara);
        Console.ResetColor();
    }
}

class Program
{
    static void Main()
    {
        VaritettyTavara<Miekka> sininenMiekka = new VaritettyTavara<Miekka>(new Miekka(), ConsoleColor.Blue);
        VaritettyTavara<Jousi> punainenJousi = new VaritettyTavara<Jousi>(new Jousi(), ConsoleColor.Red);
        VaritettyTavara<Kirves> vihreaKirves = new VaritettyTavara<Kirves>(new Kirves(), ConsoleColor.Green);

        sininenMiekka.NaytaTavara();
        punainenJousi.NaytaTavara();
        vihreaKirves.NaytaTavara();
    }
}
