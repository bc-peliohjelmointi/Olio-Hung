using Newtonsoft.Json;

namespace Valikkopeli
{
    /// <summary>
    /// Tämä luokka sisältää kaikki pelin asetukset
    /// Tämä luokka osaa tallentaa itsensä tiedostoon ja ladata
    /// itsensä tiedostosta.
    /// </summary>
    internal class GameSettings
    {
        // Kaikkien asetusten pitää olla public jotta JSON muunnos onnistuu
        // Niille voi laittaa myös oletusarvot.
        public float masterVolume = 1.0f;
        public int asteroidAmount = 3;

        // Käytetään Newtonsoft.Json kirjastoa asetusten tallentamiseen
        // ja lukemiseen
        public void SaveToDisk()
        {
            string jsonString = JsonConvert.SerializeObject(this);
            // Jos ei anna tarkempaa sijaintia, tiedosto kirjoitetaan samaan kansioon
            // missä pelin .exe tiedosto on.
            File.WriteAllText("settings.json", jsonString);
        }

        public void LoadFromDisk()
        {
            // Varmista että tiedosto on olemassa ennen kuin
            // se luetaan
            if (File.Exists("settings.json"))
            {
                string jsonString = File.ReadAllText("settings.json");

                GameSettings loaded = JsonConvert.DeserializeObject<GameSettings>(jsonString);

                // Tässä kohtaa voisi varmistaa että kaikki arvot
                // ovat järkeviä
                this.asteroidAmount = loaded.asteroidAmount;
                this.masterVolume = loaded.masterVolume;
            }
        }
    }
}