using Raylib_cs;
using System;
using System.Numerics;

public class Tykki
{
    public Vector2 Sijainti;
    public float Suunta = -45f;
    public float LähtöNopeus = 10f;
    public Color Väri;
    // … existing fields …
    public int HP = 100;         // starting hit-points
    public bool Elossa => HP > 0;

    public Tykki(Vector2 sijainti, Color väri)
    {
        Sijainti = sijainti;
        Väri = väri;
    }

    public void Piirrä()
    {
        // Piipun pääpäätteen laskenta
        float rad = Raylib.DEG2RAD * Suunta;
        var direction = new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
        Vector2 piippuPää = Sijainti + direction * 40f;

        // Piirrä tykin runko ja piippu
        Raylib.DrawCircleV(Sijainti, 15, Väri);
        Raylib.DrawLineEx(Sijainti, piippuPää, 6f, Color.Black);
    }

    public Ammo Ammu(AmmoType tyyppi)
    {
        // Laske suuntavektori radiaaneina ja kerro lähtönopeudella
        float rad = Raylib.DEG2RAD * Suunta;
        var direction = new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
        Vector2 nopeus = direction * LähtöNopeus;

        return new Ammo(Sijainti, nopeus, tyyppi);
    }

    public void PiirräHP()
    {
        int barW = 40, barH = 6;
        Vector2 pos = Sijainti - new Vector2(barW / 2, 30);
        float pct = MathF.Max(HP, 0) / 100f;
        Rectangle bg = new Rectangle(pos.X, pos.Y, barW, barH);
        Rectangle fg = new Rectangle(pos.X, pos.Y, barW * pct, barH);
        Raylib.DrawRectangleRec(bg, Color.DarkGray);
        Raylib.DrawRectangleRec(fg, Color.Lime);
    }
}


