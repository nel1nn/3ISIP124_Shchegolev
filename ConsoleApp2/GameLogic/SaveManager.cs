using System.Text.Json;
using System.Text.Json.Serialization;
using ConsoleApp2.Entities;
using ConsoleApp2.Items;

namespace ConsoleApp2.GameLogic
{
    public class SaveManager
    {
        private record EquipmentData(string Name, Rarity Rarity, int Power);

        private record SaveData(int Turn, int Hp, int Kills, EquipmentData Weapon, EquipmentData Armor);

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly string _path;

        public SaveManager(string? path = null)
        {
            _path = path ?? Path.Combine(AppContext.BaseDirectory, "savegame.json");
        }

        public bool SaveExists => File.Exists(_path);

        public bool TrySave(Game game, out string error)
        {
            var player = game.Player;
            var data = new SaveData(
                game.Turn,
                player.Hp,
                player.Kills,
                new EquipmentData(player.Weapon.Name, player.Weapon.Rarity, player.Weapon.Power),
                new EquipmentData(player.Armor.Name, player.Armor.Rarity, player.Armor.Power));

            try
            {
                File.WriteAllText(_path, JsonSerializer.Serialize(data, Options));
                error = "";
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool TryLoad(out Player? player, out int turn, out string error)
        {
            player = null;
            turn = 0;

            try
            {
                var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(_path), Options);
                if (data is null || !IsValid(data))
                {
                    error = "файл сохранения повреждён";
                    return false;
                }

                var weapon = new Weapon(data.Weapon.Name, data.Weapon.Rarity, data.Weapon.Power);
                var armor = new Armor(data.Armor.Name, data.Armor.Rarity, data.Armor.Power);
                player = new Player(weapon, armor, data.Hp, data.Kills);
                turn = data.Turn;
                error = "";
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                error = ex is JsonException ? "файл сохранения повреждён" : ex.Message;
                return false;
            }
        }

        public void Delete()
        {
            try
            {
                File.Delete(_path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
            }
        }

        private static bool IsValid(SaveData d) =>
            d.Turn >= 0 && d.Kills >= 0 &&
            d.Hp is >= 1 and <= Player.StartMaxHp &&
            d.Weapon is { Power: > 0 } && d.Armor is { Power: > 0 } &&
            !string.IsNullOrWhiteSpace(d.Weapon.Name) && !string.IsNullOrWhiteSpace(d.Armor.Name) &&
            Enum.IsDefined(d.Weapon.Rarity) && Enum.IsDefined(d.Armor.Rarity);
    }
}
