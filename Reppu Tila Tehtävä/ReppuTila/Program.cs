using System;
using System.Collections.Generic;

namespace ReppuTila
{
    internal class Program
    {
        public class Tavara
        {
            public double Paino { get; set; } 
            public double Tilavuus { get; set; } 

            public Tavara(double paino, double tilavuus)
            {
                Paino = paino;
                Tilavuus = tilavuus;
            }
        }

        
        public class Nuoli : Tavara
        {
            public Nuoli() : base(0.1, 0.05) { } 
        }

        public class Jousi : Tavara
        {
            public Jousi() : base(1.0, 4.0) { } 
        }

        public class Köysi : Tavara
        {
            public Köysi() : base(1.0, 1.5) { } 
        }

        public class Vesi : Tavara
        {
            public Vesi() : base(2.0, 2.0) { } 
        }

        public class RuokaAnnos : Tavara
        {
            public RuokaAnnos() : base(1.0, 0.5) { } 
        }

        public class Miekka : Tavara
        {
            public Miekka() : base(5.0, 3.0) { } 
        }

        public class Reppu
        {
            private List<Tavara> tavarat; 
            public int MaxTavarat { get; set; } 
            public double MaxPaino { get; set; } 
            public double MaxTilavuus { get; set; } 

            public Reppu(int maxTavarat, double maxPaino, double maxTilavuus)
            {
                tavarat = new List<Tavara>();
                MaxTavarat = maxTavarat;
                MaxPaino = maxPaino;
                MaxTilavuus = maxTilavuus;
            }

            
            public double TavaroidenPaino()
            {
                double paino = 0;
                foreach (var tavara in tavarat)
                {
                    paino += tavara.Paino;
                }
                return paino;
            }

            
            public double TavaroidenTilavuus()
            {
                double tilavuus = 0;
                foreach (var tavara in tavarat)
                {
                    tilavuus += tavara.Tilavuus;
                }
                return tilavuus;
            }

            
            public bool Add(Tavara tavara)
            {
                if (tavarat.Count >= MaxTavarat ||
                    TavaroidenPaino() + tavara.Paino > MaxPaino ||
                    TavaroidenTilavuus() + tavara.Tilavuus > MaxTilavuus)
                {
                    return false; 
                }
                tavarat.Add(tavara);
                return true; 
            }

            
            public void TilanTarkistus()
            {
                Console.WriteLine($"Repussa on tällä hetkellä {tavarat.Count}/{MaxTavarat} tavaraa.");
                Console.WriteLine($"Repun paino: {TavaroidenPaino()}/{MaxPaino}.");
                Console.WriteLine($"Repun tilavuus: {TavaroidenTilavuus()}/{MaxTilavuus}.");
            }
        }

        static void Main(string[] args)
        {
            
            Reppu reppu = new Reppu(10, 30, 20);

            while (true)
            {
                
                reppu.TilanTarkistus();

                
                Console.WriteLine("Mitä haluat lisätä?");
                Console.WriteLine("1 - Nuoli");
                Console.WriteLine("2 - Jousi");
                Console.WriteLine("3 - Köysi");
                Console.WriteLine("4 - Vettä");
                Console.WriteLine("5 - Ruokaa");
                Console.WriteLine("6 - Miekka");

                string valinta = Console.ReadLine();

                Tavara uusiTavara = null;

                
                switch (valinta)
                {
                    case "1":
                        uusiTavara = new Nuoli();
                        break;
                    case "2":
                        uusiTavara = new Jousi();
                        break;
                    case "3":
                        uusiTavara = new Köysi();
                        break;
                    case "4":
                        uusiTavara = new Vesi();
                        break;
                    case "5":
                        uusiTavara = new RuokaAnnos();
                        break;
                    case "6":
                        uusiTavara = new Miekka();
                        break;
                    default:
                        Console.WriteLine("Virheellinen valinta, yritä uudelleen.");
                        continue; 
                }

               
                if (reppu.Add(uusiTavara))
                {
                    Console.WriteLine("Tavara lisätty onnistuneesti!");
                }
                else
                {
                    Console.WriteLine("Tavara ei mahtunut reppuun!");
                }
            }
        }
    }
}