namespace Ovitehrävä
{
    internal class Program
    {
        // Luettelo oven tiloille
        enum OvenTila
        {
            Lukossa,
            Kiinni,
            Auki
        }

        static void Main(string[] args)
        {
            // Oven tila
            OvenTila ovenTila = OvenTila.Lukossa;

            // Pääluuppi
            while (true)
            {
                // Näytä nykyinen tila
                Console.WriteLine($"Ovi on {ovenTila}. Mitä haluat tehdä?");

                // Kysy komento
                string komento = Console.ReadLine()?.ToLower();

                // Tarkista komento ja päivitä oven tila
                switch (komento)
                {
                    case "poista lukitus":
                        if (ovenTila == OvenTila.Lukossa)
                        {
                            ovenTila = OvenTila.Kiinni;
                            Console.WriteLine("Lukitus poistettu.");
                        }
                        else
                        {
                            Console.WriteLine("Ovi ei ole lukossa, joten lukitusta ei voi poistaa.");
                        }
                        break;

                    case "avaa":
                        if (ovenTila == OvenTila.Kiinni)
                        {
                            ovenTila = OvenTila.Auki;
                            Console.WriteLine("Ovi avattiin.");
                        }
                        else if (ovenTila == OvenTila.Lukossa)
                        {
                            Console.WriteLine("Ovi on lukossa. Poista lukitus ennen avaamista.");
                        }
                        else
                        {
                            Console.WriteLine("Ovi on jo auki.");
                        }
                        break;

                    case "sulje":
                        if (ovenTila == OvenTila.Auki)
                        {
                            ovenTila = OvenTila.Kiinni;
                            Console.WriteLine("Ovi suljettiin.");
                        }
                        else
                        {
                            Console.WriteLine("Ovi ei ole auki, joten sitä ei voi sulkea.");
                        }
                        break;

                    case "lukitse":
                        if (ovenTila == OvenTila.Kiinni)
                        {
                            ovenTila = OvenTila.Lukossa;
                            Console.WriteLine("Ovi lukittiin.");
                        }
                        else if (ovenTila == OvenTila.Auki)
                        {
                            Console.WriteLine("Ovi on auki. Sulje ovi ennen lukitsemista.");
                        }
                        else
                        {
                            Console.WriteLine("Ovi on jo lukossa.");
                        }
                        break;

                    default:
                        Console.WriteLine("Tuntematon komento. Yritä uudelleen.");
                        break;
                }
            }
        }
    }
}