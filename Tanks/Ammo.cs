using Raylib_cs;
using System.Numerics;

public class Ammo
{
    public Vector2 Sijainti;
    public Vector2 Nopeus;
    public AmmoType Tyyppi;

    public Ammo(Vector2 sijainti, Vector2 nopeus, AmmoType tyyppi)
    {
        Sijainti = sijainti;
        Nopeus = nopeus;
        Tyyppi = tyyppi;
    }

    public void Päivitä()
    {
        Nopeus.Y += 0.2f * Tyyppi.paino; // painovoima
        Sijainti += Nopeus;
    }

    public void Piirrä()
    {
        Raylib.DrawCircleV(Sijainti, Tyyppi.säde, Tyyppi.Väri);
    }
}
