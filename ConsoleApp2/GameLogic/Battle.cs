using ConsoleApp2.Core;
using ConsoleApp2.Entities;

namespace ConsoleApp2.GameLogic
{
    public class Battle
    {
        private readonly Player _player;
        private readonly Enemy _enemy;
        private readonly Dice _dice;
        private readonly ConsoleUi _ui;

        public Battle(Player player, Enemy enemy, Dice dice, ConsoleUi ui)
        {
            _player = player;
            _enemy = enemy;
            _dice = dice;
            _ui = ui;
        }

        public bool Run()
        {
            _player.IsFrozen = false;
            _player.StopDefending();

            string title = _enemy.IsBoss ? "БОСС" : "Враг";
            string race = _enemy.Name == _enemy.Race ? "" : $" (раса: {_enemy.Race})";
            _ui.Header($"{title}: {_enemy.Name}{race}");
            _ui.Line($"HP {_enemy.Hp} | Атака {_enemy.Attack} | Защита {_enemy.Defense} | Особенность: {_enemy.AbilityDescription}",
                _enemy.IsBoss ? ConsoleColor.Red : ConsoleColor.DarkRed);

            while (true)
            {
                PlayerTurn();
                if (!_enemy.IsAlive)
                {
                    _ui.Line($"{_enemy.Name} повержен!", ConsoleColor.Green);
                    return true;
                }

                EnemyTurn();
                if (!_player.IsAlive)
                {
                    return false;
                }
            }
        }

        private void PlayerTurn()
        {
            _player.StopDefending();
            _ui.Line();
            _ui.Line($"Вы: {_player.Hp}/{_player.MaxHp} HP | {_enemy.Name}: {_enemy.Hp}/{_enemy.MaxHp} HP", ConsoleColor.White);

            if (_player.IsFrozen)
            {
                _player.IsFrozen = false;
                _ui.Line("Вы заморожены и пропускаете ход!", ConsoleColor.Cyan);
                return;
            }

            int choice = _ui.AskChoice("Ваш ход:", "Атака", "Защита (40% уклониться, иначе блок доспехами)");
            if (choice == 1)
            {
                int damage = _player.CalculateDamageTo(_enemy);
                _enemy.TakeDamage(damage);
                _ui.Line($"Вы бьёте {_enemy.Name} и наносите {damage} урона.", ConsoleColor.Yellow);
            }
            else
            {
                _player.StartDefending();
                _ui.Line("Вы встаёте в защитную стойку.", ConsoleColor.Yellow);
            }
        }

        private void EnemyTurn()
        {
            var report = _enemy.AttackPlayer(_player, _dice);

            if (report.Dodged)
            {
                _ui.Line($"{_enemy.Name} атакует, но вы уклоняетесь! Урона нет.", ConsoleColor.Green);
                return;
            }

            var parts = new List<string>();
            if (report.Critical)
            {
                parts.Add("КРИТИЧЕСКИЙ УДАР");
            }
            if (report.ArmorIgnored)
            {
                parts.Add("броня игнорируется");
            }
            if (report.Blocked > 0)
            {
                parts.Add($"блок поглотил {report.Blocked}");
            }

            string extra = parts.Count > 0 ? $" ({string.Join(", ", parts)})" : "";
            _ui.Line($"{_enemy.Name} атакует и наносит {report.FinalDamage} урона{extra}.", ConsoleColor.Red);

            if (report.Froze)
            {
                _ui.Line("Вы заморожены! Следующий ход будет пропущен.", ConsoleColor.Cyan);
            }
        }
    }
}
