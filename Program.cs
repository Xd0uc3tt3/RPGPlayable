using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RPGPlayable
{
    static class GameSettings
    {
        public const int PlayerMaxHP = 50;
        public const int PlayerSwordDamage = 2;

        public const int BasicEnemyHP = 5;
        public const int ShieldEnemyHP = 15;
        public const int CowardEnemyHP = 7;

        public const int ShieldEnemyShield = 20;

        public const int MedkitHeal = 5;
        public const int ShieldAmount = 20;

        public const int MaxWaves = 5;
        public const int BaseEnemySpawn = 3;
        public const int EnemyScaling = 2;
        public const int BaseItemSpawn = 2;

        public const int LavaDamage = 2;
    }

    class EnemyManager
    {
        public List<Enemy> Enemies { get; private set; }
        private Random rand = new Random();

        public EnemyManager(List<Enemy> enemies)
        {
            Enemies = enemies;
        }

        public void Respawn(Map map, Player player, int count)
        {
            int spawned = 0;

            while (spawned < count)
            {
                int x = rand.Next(map.Width);
                int y = rand.Next(map.Height);

                bool isWalkable = map.IsWalkable(x, y);
                bool isPlayerThere = (player.X == x && player.Y == y);
                bool isEnemyThere = GetEnemyAt(x, y) != null;

                if (!(isWalkable && !isPlayerThere && !isEnemyThere))
                    continue;

                Enemy enemy;
                int type = rand.Next(3);

                if (type == 0)
                {
                    enemy = new BasicEnemy(x, y);
                }
                else if (type == 1)
                {
                    enemy = new ShieldedEnemy(x, y);
                }
                else
                {
                    enemy = new CowardEnemy(x, y);
                }

                Enemies.Add(enemy);
                spawned++;
            }
        }

        public Enemy GetEnemyAt(int x, int y)
        {
            return Enemies.Find(e => e.X == x && e.Y == y && !e.Health.IsDead);
        }

        public void RemoveDead()
        {
            Enemies.RemoveAll(e => e.Health.IsDead);
        }

        public void Update(Game game)
        {
            foreach (var enemy in Enemies)
            {
                enemy.TakeTurn(game);
            }
                
        }
    }

    class ItemManager
    {
        public List<Item> Items { get; private set; }
        private Random rand = new Random();

        public ItemManager(List<Item> items)
        {
            Items = items;
        }

        public void Respawn(Map map, Player player, int count)
        {
            int spawned = 0;

            while (spawned < count)
            {
                int x = rand.Next(0, map.Width);
                int y = rand.Next(0, map.Height);

                bool isWalkable = map.IsWalkable(x, y);
                bool isPlayerThere = (player.X == x && player.Y == y);
                bool isEnemyThere = map.Enemies.Exists(e => e.X == x && e.Y == y && !e.Health.IsDead);

                if (!isWalkable || isPlayerThere || isEnemyThere)
                    continue;

                Item item;
                int type = rand.Next(3);

                if (type == 0)
                {
                    item = new Medkit(x, y);
                }
                else if (type == 1)
                {
                    item = new Sword(x, y);
                }
                else
                {
                    item = new Sheild(x, y);
                }

                Items.Add(item);
                spawned++;
            }
        }

        public void Remove(Item item)
        {
            Items.Remove(item);
        }

        public Item GetItemAt(int x, int y)
        {
            return Items.Find(i => i.X == x && i.Y == y);
        }
    }

    class Health
    {
        public int Current { get; private set; }
        public int Max { get; private set; }

        public int ShieldCurrent { get; private set; } = 0;
        public int ShieldMax { get; private set; } = 0;

        public bool IsDead => Current <= 0;

        public Health(int max)
        {
            Max = max;
            Current = max;
        }

        public void SetShield(int shield)
        {
            ShieldMax = shield;
            ShieldCurrent = shield;
        }

        public void TakeDamage(int amount)
        {
            if (ShieldCurrent > 0)
            {
                if (amount <= ShieldCurrent)
                {
                    ShieldCurrent -= amount;
                }
                else
                {
                    int leftover = amount - ShieldCurrent;
                    ShieldCurrent = 0;
                    Current -= leftover;
                    if (Current < 0) Current = 0;
                }
            }
            else
            {
                Current -= amount;
                if (Current < 0) Current = 0;
            }
        }
        public void Heal(int amount)
        {
            Current += amount;
            if (Current > Max)
            {
                Current = Max;
            }
        }
    }

    abstract class Item
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public char Mark { get; protected set; }

        protected Item(int x, int y, char mark)
        {
            X = x;
            Y = y;
            Mark = mark;
        }

        public abstract void OnPickup(Player player);
    }

    class Medkit : Item
    {
        public Medkit(int x, int y) : base(x, y, 'M') { }

        public override void OnPickup(Player player)
        {
            player.Health.Heal(GameSettings.MedkitHeal);
        }

    }

    class Sword : Item
    {
        public Sword(int x, int y) : base(x, y, 'S') { }

        public override void OnPickup(Player player)
        {
            player.EquipSword();
        }
    }

    class Sheild : Item
    {
        public Sheild(int x, int y) : base(x, y, 'O') { }

        public override void OnPickup(Player player)
        {
            player.EquipShield();
        }

    }


    abstract class Entity
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public char Mark { get; protected set; }
        public Health Health { get; protected set; }

        protected Entity(int x, int y, char mark, int hp)
        {
            X = x;
            Y = y;
            Mark = mark;
            Health = new Health(hp);
        }

        public abstract void TakeTurn(Game game);
    }

    class Player : Entity
    {
        public Player(int x, int y) : base(x, y, 'P', GameSettings.PlayerMaxHP) { }
        public bool HasSword { get; private set; } = false;

        public Enemy LastEnemyEncountered { get; private set; } = null;

        public override void TakeTurn(Game game)
        {
            var key = Console.ReadKey(true).Key;

            int dx = 0, dy = 0;

            switch (key)
            {
                case ConsoleKey.W:
                    dy = -1; 
                    break;
                case ConsoleKey.S:
                    dy = 1; 
                    break;
                case ConsoleKey.A:
                    dx = -1; 
                    break;
                case ConsoleKey.D:
                    dx = 1; break;

                default: 
                    return;
            }

            AttemptMove(game, dx, dy);
        }

        private void AttemptMove(Game game, int dx, int dy)
        {
            int nx = X + dx;
            int ny = Y + dy;

            Enemy enemy = game.GetEnemyAt(nx, ny);
            if (enemy != null)
            {
                LastEnemyEncountered = enemy;

                if (HasSword == true)
                {
                    enemy.Health.TakeDamage(GameSettings.PlayerSwordDamage);
                }

                enemy.Health.TakeDamage(GameSettings.PlayerSwordDamage);
                return;
            }

            Item item = game.GetItemAt(nx, ny);
            if (item != null)
            {
                item.OnPickup(this);
                game.RemoveItem(item);
                return;
            }

            if (game.Map.IsWalkable(nx, ny))
            {
                X = nx;
                Y = ny;

                int damage = game.Map.GetLavaDamage(nx, ny);
                if (damage > 0)
                {
                    Health.TakeDamage(damage);
                }
            }
        }

        public void EquipSword()
        {
            HasSword = true;
        }

        public void EquipShield()
        {
            Health.SetShield(GameSettings.ShieldAmount);
        }
    }

    abstract class Enemy : Entity
    {
        protected static Random random = new Random();

        protected Enemy(int x, int y, char mark, int hp) : base(x, y, mark, hp)
        {
        }

        public abstract override void TakeTurn(Game game);
    }

    class BasicEnemy : Enemy
    {
        public BasicEnemy(int x, int y) : base(x, y, 'E', GameSettings.BasicEnemyHP) { }

        public override void TakeTurn(Game game)
        {
            int dx = Math.Sign(game.Player.X - X);
            int dy = Math.Sign(game.Player.Y - Y);

            AttemptMove(game, dx, dy);
        }

        private void AttemptMove(Game game, int dx, int dy)
        {
            int nx = X + dx;
            int ny = Y + dy;

            if (game.Player.X == nx && game.Player.Y == ny)
            {
                int damage = random.Next(1, 6);
                game.Player.Health.TakeDamage(damage);
                return;
            }

            if (game.Map.IsWalkable(nx, ny) && game.GetEnemyAt(nx, ny) == null)
            {
                X = nx;
                Y = ny;

                int damage = game.Map.GetLavaDamage(nx, ny);
                if (damage > 0)
                {
                    Health.TakeDamage(damage);
                }
            }
        }
    }

    class ShieldedEnemy : Enemy
    {
        private bool movedLastTurn = false;
        public ShieldedEnemy(int x, int y) : base(x, y, 'B', GameSettings.ShieldEnemyHP)
        {
            Health.SetShield(GameSettings.ShieldEnemyShield);
        }

        public override void TakeTurn(Game game)
        {
            if (movedLastTurn)
            {
                movedLastTurn = false;
                return;
            }

            int dx = Math.Sign(game.Player.X - X);
            int dy = Math.Sign(game.Player.Y - Y);

            AttemptMove(game, dx, dy);

            movedLastTurn = true;
        }

        private void AttemptMove(Game game, int dx, int dy)
        {
            int nx = X + dx;
            int ny = Y + dy;

            if (game.Player.X == nx && game.Player.Y == ny)
            {
                int damage = new Random().Next(2, 6);
                game.Player.Health.TakeDamage(damage);
                return;
            }

            if (game.Map.IsWalkable(nx, ny) && game.GetEnemyAt(nx, ny) == null)
            {
                X = nx;
                Y = ny;

                int damage = game.Map.GetLavaDamage(nx, ny);
                if (damage > 0)
                {
                    Health.TakeDamage(damage);
                }
            }
        }
    }

    class CowardEnemy : Enemy
    {
        public CowardEnemy(int x, int y) : base(x, y, 'C', GameSettings.CowardEnemyHP) { }

        public override void TakeTurn(Game game)
        {
            int dx = Math.Sign(game.Player.X - X);
            int dy = Math.Sign(game.Player.Y - Y);

            if (Health.Current <= 2)
            {
                dx *= -1;
                dy *= -1;
            }

            AttemptMove(game, dx, dy);
        }

        private void AttemptMove(Game game, int dx, int dy)
        {
            int nx = X + dx;
            int ny = Y + dy;

            if (game.Player.X == nx && game.Player.Y == ny)
            {
                int damage = random.Next(1, 6);
                game.Player.Health.TakeDamage(damage);
                return;
            }

            if (game.Map.IsWalkable(nx, ny) && game.GetEnemyAt(nx, ny) == null)
            {
                X = nx;
                Y = ny;

                int damage = game.Map.GetLavaDamage(nx, ny);
                if (damage > 0)
                {
                    Health.TakeDamage(damage);
                }
            }
        }
    }

    class Map
    {
        private char[,] tiles;
        public int Width { get; }
        public int Height { get; }

        public Player Player { get; private set; }
        public List<Enemy> Enemies { get; private set; }

        public List<Item> Items { get; private set; }

        public Map(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            Height = lines.Length;
            Width = lines[0].Length;

            tiles = new char[Width, Height];
            Enemies = new List<Enemy>();
            Items = new List<Item>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char c = lines[y][x];

                    if (c == 'P')
                    {
                        Player = new Player(x, y);
                        tiles[x, y] = '.';
                    }
                    else if (c == 'E')
                    {
                        Enemies.Add(new BasicEnemy(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'B')
                    {
                        Enemies.Add(new ShieldedEnemy(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'C')
                    {
                        Enemies.Add(new CowardEnemy(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'M')
                    {
                        Items.Add(new Medkit(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'S')
                    {
                        Items.Add(new Sword(x, y));
                        tiles[x, y] = '.';
                    }
                    else if (c == 'O')
                    {
                        Items.Add(new Sheild(x, y));
                        tiles[x, y] = '.';
                    }
                    else
                    {
                        tiles[x, y] = c;
                    }
                }
            }
        }

        public bool IsWalkable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false;
            }

            return tiles[x, y] == '.' || tiles[x, y] == '~';
        }

        public int GetLavaDamage(int x, int y)
        {
            char tile = tiles[x, y];
            if (tile == '~')
            {
                return GameSettings.LavaDamage;
            }
            return 0;
        }

        public void Draw(Player player, List<Enemy> enemies)
        {
            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (player.X == x && player.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(player.Mark);
                    }
                        
                    else if (enemies.Exists(e => e.X == x && e.Y == y && !e.Health.IsDead))
                    {
                        var enemy = enemies.Find(e => e.X == x && e.Y == y && !e.Health.IsDead);

                        if (enemy.Mark == 'E')
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else if (enemy.Mark == 'B')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        }
                        else if (enemy.Mark == 'C')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                        }

                        Console.Write(enemy.Mark);
                    }
                    else if (Items.Exists(i => i.X == x && i.Y == y))
                    {
                        var item = Items.Find(i => i.X == x && i.Y == y);

                        if (item.Mark == 'M')
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else if (item.Mark == 'S')
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        else 
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        }

                        Console.Write(item.Mark);
                    }
                    else
                    {
                        char tile = tiles[x, y];

                        if (tile == '#')
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else if (tile == '~')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        }
                        else if (tile == '.')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }

                        Console.Write(tile);
                    }

                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine($"Player HP: {player.Health.Current} ");

            if (player.Health.ShieldMax > 0 && player.Health.ShieldCurrent > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Shield: {player.Health.ShieldCurrent} ");
            }

            Console.WriteLine("                    ");

            if (player.LastEnemyEncountered != null)
            {
                var enemy = player.LastEnemyEncountered;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine();
                Console.WriteLine("Last Enemy Encountered:"        );
                Console.WriteLine($"Type: {enemy.Mark}                 ");
                Console.WriteLine($"HP: {enemy.Health.Current}/{enemy.Health.Max}     ");
                if (enemy.Health.ShieldMax > 0)
                {
                    Console.WriteLine($"Shield: {enemy.Health.ShieldCurrent}/{enemy.Health.ShieldMax}     ");
                }
                else
                {
                    Console.WriteLine("                     ");
                }
            }
        }
    }

    class Game
    {
        public Map Map { get; }
        public Player Player { get; }
        private EnemyManager enemyManager;
        private ItemManager itemManager;

        private int currentWave = 1;
        private const int maxWaves = GameSettings.MaxWaves;

        private Random rand = new Random();

        public Game()
        {
            Map = new Map("map.txt");

            Player = Map.Player;
            enemyManager = new EnemyManager(Map.Enemies);
            itemManager = new ItemManager(Map.Items);

        }



        public void Run()
        {
            while (!Player.Health.IsDead)
            {
                enemyManager.RemoveDead();

                if (enemyManager.Enemies.Count == 0)
                {
                    if (currentWave == maxWaves)
                    {
                        break;
                    }

                    currentWave++;
                    Console.Clear();

                    enemyManager.Respawn(Map, Player, GameSettings.BaseEnemySpawn + currentWave * GameSettings.EnemyScaling);
                    itemManager.Respawn(Map, Player, GameSettings.BaseItemSpawn + currentWave * GameSettings.EnemyScaling);
                }

                Map.Draw(Player, enemyManager.Enemies);

                Player.TakeTurn(this);

                enemyManager.Update(this);

                if (Player.Health.IsDead)
                {
                    break;
                }

            }

            Console.Clear();

            if (Player.Health.IsDead)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Game Over.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You Win!");
            }
        }

        public Enemy GetEnemyAt(int x, int y)
        {
            return enemyManager.GetEnemyAt(x, y);
        }

        public Item GetItemAt(int x, int y)
        {
            return itemManager.GetItemAt(x, y);
        }

        public void RemoveItem(Item item)
        {
            itemManager.Remove(item);
        }

    }


    internal class Program
    {
        static void Main(string[] args)
        {
            new Game().Run();
        }
    }
}
