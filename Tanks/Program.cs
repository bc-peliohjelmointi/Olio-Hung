using System;
using System.Collections.Generic;
using System.IO;
using Raylib_cs;
using System.Numerics;
using System.Text.Json;

class Program
{
    const int ScreenWidth = 1280;
    const int ScreenHeight = 720;

    // Ammus-tyypit JSONista
    static List<AmmoType> ammusTyypit = new();

    // Pelin olennot
    static List<Tykki> tykit = new();
    static List<Ammo> ammukset = new();
    static Terrain maasto;

    // Vuoron ja ammustyypin seuranta
    static int currentPlayer = 0;
    static int currentAmmoType = 0;

    // Peli‐over‐tila
    static bool gameOver = false;
    static int winner = -1;

    // Lataus‐mekaniikka (charge shot)
    static bool isCharging = false;
    static float chargePower = 0f;
    static float minPower = 5f;   // perustaso
    static float maxPower = 50f;  // yläraja
    static float chargeRate = 20f;  // yksikköä / sekunti

    static void Main()
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "Tykkipeli - Raylib CS");
        Raylib.SetTargetFPS(60);

        // Lataa ammustyypit
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Ammukset.json");
        var json = File.ReadAllText(jsonPath);
        ammusTyypit = JsonSerializer.Deserialize<List<AmmoType>>(json);

        // Generoi maasto ja kaksi tykkiä
        maasto = new Terrain(ScreenWidth, ScreenHeight);
        tykit.Add(new Tykki(new Vector2(200, maasto.GetHeightAt(200)), Color.Blue));
        tykit.Add(new Tykki(new Vector2(1000, maasto.GetHeightAt(1000)), Color.Red));

        // Pääsilmukka
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }

        Raylib.CloseWindow();
    }

    static void ResetGame()
    {
        foreach (var t in tykit) t.HP = 100;
        ammukset.Clear();
        currentPlayer = 0;
        currentAmmoType = 0;
        gameOver = false;
        winner = -1;
        isCharging = false;
        chargePower = 0f;
    }

    static void Update()
    {
        // Jos peli ohi, odota R‐restarttia
        if (gameOver)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.R))
                ResetGame();
            return;
        }

        var cannon = tykit[currentPlayer];

        // ─── Kääntö ja lähtövoima ───
        if (Raylib.IsKeyDown(KeyboardKey.Left)) cannon.Suunta -= 1f;
        if (Raylib.IsKeyDown(KeyboardKey.Right)) cannon.Suunta += 1f;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) cannon.LähtöNopeus += 0.1f;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) cannon.LähtöNopeus -= 0.1f;

        // ─── Ammustyypin valinta Q/E ───
        if (Raylib.IsKeyPressed(KeyboardKey.Q) && ammusTyypit.Count > 1)
            currentAmmoType = (currentAmmoType - 1 + ammusTyypit.Count) % ammusTyypit.Count;
        if (Raylib.IsKeyPressed(KeyboardKey.E) && ammusTyypit.Count > 1)
            currentAmmoType = (currentAmmoType + 1) % ammusTyypit.Count;

        // ─── Aloita lataus pidettyä Spacea ───
        if (Raylib.IsKeyDown(KeyboardKey.Space))
        {
            if (!isCharging)
            {
                isCharging = true;
                chargePower = 0f;
            }
            chargePower += chargeRate * Raylib.GetFrameTime();
            if (chargePower > maxPower) chargePower = maxPower;
        }

        // ─── Vapauta Space ja ammu ───
        if (isCharging && Raylib.IsKeyReleased(KeyboardKey.Space))
        {
            isCharging = false;

            // Määritä lopullinen lähtönopeus
            float finalSpeed = minPower + chargePower;
            float oldSpeed = cannon.LähtöNopeus;
            cannon.LähtöNopeus = finalSpeed;

            ammukset.Add(cannon.Ammu(ammusTyypit[currentAmmoType]));

            // Palauta talteenotto‐nopeus
            cannon.LähtöNopeus = oldSpeed;
        }

        // ─── Päivitä ammusten sijainnit ja törmäykset ───
        for (int i = ammukset.Count - 1; i >= 0; i--)
        {
            var shot = ammukset[i];
            shot.Päivitä();

            // 1) osuma maastoon?
            if (maasto.OnkoOsuma(shot.Sijainti))
            {
                ammukset.RemoveAt(i);
                currentPlayer = (currentPlayer + 1) % tykit.Count;
                continue;
            }

            // 2) osuma vastustajan tykkiin?
            int otherIdx = (currentPlayer + 1) % tykit.Count;
            var targetTank = tykit[otherIdx];
            float dist = Vector2.Distance(shot.Sijainti, targetTank.Sijainti);
            if (dist < shot.Tyyppi.säde + 15) // 15 = tykin “säde”
            {
                targetTank.HP -= shot.Tyyppi.vahinko;
                ammukset.RemoveAt(i);

                if (!targetTank.Elossa)
                {
                    gameOver = true;
                    winner = currentPlayer + 1; // Pelaaja 1 tai 2
                }
                else
                {
                    currentPlayer = otherIdx;
                }
            }
        }
    }

    static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.SkyBlue);

        // Piirrä kenttä ja oliot
        maasto.Piirrä();
        foreach (var t in tykit)
        {
            t.Piirrä();
            t.PiirräHP();
        }
        foreach (var a in ammukset)
            a.Piirrä();

        // ─── UI ───
        if (gameOver)
        {
            string text = $"Pelaaja {winner} voittaa! (R = restart)";
            int w = Raylib.MeasureText(text, 40);
            Raylib.DrawText(text, ScreenWidth / 2 - w / 2, ScreenHeight / 2 - 20, 40, Color.Gold);
        }
        else
        {
            Raylib.DrawText($"Vuoro: Pelaaja {currentPlayer + 1}", 20, 20, 20, Color.Black);
            var sel = ammusTyypit[currentAmmoType];
            Raylib.DrawText($"Ammus: {sel.nimi}", 20, 50, 20, Color.DarkGray);

            // Näytä latauspalkki jos lataat
            if (isCharging)
            {
                Rectangle bg = new Rectangle(20, 80, 200, 10);
                Raylib.DrawRectangleRec(bg, Color.DarkGray);
                float pct = chargePower / maxPower;
                Rectangle fg = new Rectangle(20, 80, 200 * pct, 10);
                Raylib.DrawRectangleRec(fg, Color.Gold);
            }
        }

        Raylib.EndDrawing();
    }
}
