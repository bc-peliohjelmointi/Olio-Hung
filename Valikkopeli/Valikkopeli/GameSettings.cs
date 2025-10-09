using Newtonsoft.Json;
using System.IO;

namespace Valikkopeli
{
    public class GameSettings  // ← muutettu internal → public
    {
        public float masterVolume = 1.0f;
        public int asteroidAmount = 3;

        public void SaveToDisk()
        {
            string jsonString = JsonConvert.SerializeObject(this);
            File.WriteAllText("settings.json", jsonString);
        }

        public void LoadFromDisk()
        {
            if (File.Exists("settings.json"))
            {
                string jsonString = File.ReadAllText("settings.json");
                GameSettings loaded = JsonConvert.DeserializeObject<GameSettings>(jsonString);
                this.asteroidAmount = loaded.asteroidAmount;
                this.masterVolume = loaded.masterVolume;
            }
        }
    }
}
