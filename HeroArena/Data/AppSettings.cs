using System.IO;
using System.Text.Json;

namespace HeroArena.Data
{
    public class AppSettings
    {
        private static readonly string FilePath = "appsettings.json";

        public string ConnectionString { get; set; } =
            "Server=localhost;Database=ExerciceHero;Trusted_Connection=True;TrustServerCertificate=True;";

        public static AppSettings Load()
        {
            if (!File.Exists(FilePath))
            {
                var def = new AppSettings();
                Save(def);
                return def;
            }
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }

        public static void Save(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}