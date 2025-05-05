using Raylib_cs;
using System;
using System.Reflection;

public class AmmoType
{
    public string nimi { get; set; }
    public string väri { get; set; }
    public int säde { get; set; }
    public int räjähdyksenKoko { get; set; }
    public float paino { get; set; }

    public int vahinko { get; set; }

    


    public Color Väri
    {
        get
        {
            // Hae Raylib_cs.Color-tyypin julkinen staattinen kenttä annetulla nimellä (case-insensitive)
            var field = typeof(Color).GetField(väri, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            if (field != null && field.GetValue(null) is Color c)
                return c;
            return Color.Black;
        }
    }
}
