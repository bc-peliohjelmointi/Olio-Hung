using System;

namespace Nuolia_Kaupan
{
    public enum Kärki
    {
        puu = 3,
        teräs = 5,
        timantti = 50
    }

    public enum Sulka
    {
        lehti = 0,
        kanansulka = 1,
        kotkansulka = 5
    }

    public class Nuoli
    {
        private int pituusCm;
        private Kärki kärkiMateriaali;
        private Sulka sulkaMateriaali;

        public Nuoli(Kärki kärki, Sulka sulka, int pituus)
        {
            kärkiMateriaali = kärki;
            sulkaMateriaali = sulka;
            pituusCm = pituus;
        }

        // Getter-metodit
        public Kärki GetKärki()
        {
            return kärkiMateriaali;
        }

        public Sulka GetSulat()
        {
            return sulkaMateriaali;
        }

        public int GetPituus()
        {
            return pituusCm;
        }

        public int PalautaHinta()
        {
            int kärjenHinta = (int)kärkiMateriaali;
            int sulanHinta = (int)sulkaMateriaali;
            double varrenHinta = pituusCm * 0.05;

            return kärjenHinta + sulanHinta + (int)varrenHinta;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tervetuloa nuolikauppaan! Minkälaisen nuolen haluat?");

            Kärki kärkiValinta;
            while (true)
            {
                Console.Write("Minkälainen kärki (puu, teräs, timantti)?: ");
                string? kärkiSyöte = Console.ReadLine()?.ToLower();
                if (Enum.TryParse(kärkiSyöte, true, out kärkiValinta))
                    break;
                Console.WriteLine("Virheellinen syöte, yritä uudelleen.");
            }

            Sulka sulkaValinta;
            while (true)
            {
                Console.Write("Minkälaiset sulat (lehti, kanansulka, kotkansulka)?: ");
                string? sulkaSyöte = Console.ReadLine()?.ToLower();
                if (Enum.TryParse(sulkaSyöte, true, out sulkaValinta))
                    break;
                Console.WriteLine("Virheellinen syöte, yritä uudelleen.");
            }

            int pituus;
            while (true)
            {
                Console.Write("Nuolen pituus sentteinä (60-100): ");
                if (int.TryParse(Console.ReadLine(), out pituus) && pituus >= 60 && pituus <= 100)
                    break;
                Console.WriteLine("Virheellinen pituus, anna arvo välillä 60-100.");
            }

            Nuoli uusiNuoli = new Nuoli(kärkiValinta, sulkaValinta, pituus);

            Console.WriteLine($"Nuolen tiedot:");
            Console.WriteLine($" - Kärki: {uusiNuoli.GetKärki()}");
            Console.WriteLine($" - Sulat: {uusiNuoli.GetSulat()}");
            Console.WriteLine($" - Pituus: {uusiNuoli.GetPituus()} cm");
            Console.WriteLine($"Tämän nuolen hinta on {uusiNuoli.PalautaHinta()} kultarahaa.");
        }
    }
}
