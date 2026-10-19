using ConsoleApp2.Core;
using ConsoleApp2.Entities;
using ConsoleApp2.Items;

namespace ConsoleApp2.GameLogic
{
    /// <summary>Главный игровой цикл: ходы, события, бои и сундуки.</summary>
    public class Game
    {
        public const int BossEveryTurns = 10;
        private const double ChestChance = 0.5;

        private readonly ConsoleUi _ui;
        private readonly Dice _dice;
        private readonly ItemFactory _items;
        private readonly EnemyFactory _enemies;

        public Player Player { get; }
        public int Turn { get; private set; }

        public Game(ConsoleUi ui, Dice dice, Player player, int turn)
        {
            _ui = ui;
            _dice = dice;
            _items = new ItemFactory(dice);
            _enemies = new EnemyFactory(dice);
            Player = player;
            Turn = turn;
        }

        /// <summary>Играет до смерти героя. Возвращает false, если игрок вышел из игры досрочно.</summary>
        public bool Run()
        {
            while (true)
            {
                if (!BetweenTurnsMenu())
                {
                    return false;
                }

                Turn++;
                PlayTurn();

                if (!Player.IsAlive)
                {
                    ShowGameOver();
                    return true;
                }
            }
        }

        /// <summary>Меню перед каждым ходом. Возвращает false, если выбран выход.</summary>
        protected virtual bool BetweenTurnsMenu()
        {
            ShowStatus();
            int choice = _ui.AskChoice("Что дальше?", "Идти дальше", "Выйти из игры");
            return choice == 1;
        }

        protected void ShowStatus()
        {
            _ui.Header($"Ход {Turn + 1}");
            _ui.Line($"Здоровье: {Player.Hp}/{Player.MaxHp} | Побеждено врагов: {Player.Kills}", ConsoleColor.White);
            _ui.Line($"Оружие: {Player.Weapon.Describe()}", Player.Weapon.Rarity.Color());
            _ui.Line($"Доспех: {Player.Armor.Describe()}", Player.Armor.Rarity.Color());
        }

        private void PlayTurn()
        {
            if (Turn % BossEveryTurns == 0)
            {
                _ui.Line();
                _ui.Line("Земля дрожит... Перед вами появляется БОСС!", ConsoleColor.Red);
                FightBoss();
            }
            else if (_dice.Chance(ChestChance))
            {
                OpenChest();
            }
            else
            {
                Fight(_enemies.CreateRandomEnemy());
            }
        }

        private void OpenChest()
        {
            _ui.Line();
            _ui.Line("Вы нашли сундук!", ConsoleColor.Yellow);
            var item = _items.CreateRandom();
            _ui.Line("Внутри: " + item.Describe(), item.Rarity.Color());
            item.Acquire(Player, _ui);
        }

        private void Fight(Enemy enemy)
        {
            if (new Battle(Player, enemy, _dice, _ui).Run())
            {
                Player.Kills++;
            }
        }

        private void FightBoss()
        {
            var boss = _enemies.CreateRandomBoss();
            if (new Battle(Player, boss, _dice, _ui).Run())
            {
                Player.Kills++;
                _ui.Line("Из поверженного босса выпала добыча!", ConsoleColor.Yellow);
                var loot = _items.CreateEquipment(Rarity.Rare);
                _ui.Line("Награда: " + loot.Describe(), loot.Rarity.Color());
                loot.Acquire(Player, _ui);
            }
        }

        protected virtual void ShowGameOver()
        {
            _ui.Header("ВЫ ПОГИБЛИ");
            _ui.Line($"Пройдено ходов: {Turn}", ConsoleColor.White);
            _ui.Line($"Побеждено врагов: {Player.Kills}", ConsoleColor.White);
        }
    }
}
