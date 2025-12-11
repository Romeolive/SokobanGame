using System;
using SokobanMG.Core;
using SokobanMG.Enteties;

namespace SokobanGame.Core;


public class Game
    {
        private int width;
        private int height;
        private Entity[,] map;
        private int playerX;
        private int playerY;

        public Game(LevelData level)
        {
            width = level.Width;
            height = level.Height;
            playerX = level.Player.X;
            playerY = level.Player.Y;

            map = new Entity[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char c = level.Map[y][x];

                    map[y, x] = c switch
                    {
                        '#' => new Wall(),
                        '$' => new Box(),
                        '.' => new Goal(),
                        ' ' => new Floor(),
                        '@' => new Floor(),   // игрок рисуется отдельно
                        _ => new Floor()
                    };
                }
            }
        }

        public void Run()
        {
            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();
                Draw();

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Q)
                    break;

                MovePlayer(key);
            }
        }

        private void Draw()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == playerX && y == playerY)
                        Console.Write('@');
                    else
                        Console.Write(map[y, x].Symbol);
                }
                Console.WriteLine();
            }
        }

        private void MovePlayer(ConsoleKey key)
        {
            int dx = 0, dy = 0;

            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow: dy = -1; break;

                case ConsoleKey.S:
                case ConsoleKey.DownArrow: dy = 1; break;

                case ConsoleKey.A:
                case ConsoleKey.LeftArrow: dx = -1; break;

                case ConsoleKey.D:
                case ConsoleKey.RightArrow: dx = 1; break;

                default: return;
            }

            int nx = playerX + dx;
            int ny = playerY + dy;

            Entity target = map[ny, nx];

            // стена? — стоп
            if (!target.IsWalkable && !target.IsPushable)
                return;

            // если ящик
            if (target.IsPushable)
            {
                int bx = nx + dx;
                int by = ny + dy;

                Entity boxTarget = map[by, bx];

                // если дальше стенка или другой ящик — всё, нельзя
                if (!boxTarget.IsWalkable)
                    return;

                // перемещаем ящик
                if (boxTarget is Goal)
                    map[by, bx] = new BoxOnGoal();
                else
                    map[by, bx] = new Box();

                // клетка под старым ящиком
                if (target is BoxOnGoal)
                    map[ny, nx] = new Goal();
                else
                    map[ny, nx] = new Floor();
            }

            // теперь можно двигать игрока
            playerX = nx;
            playerY = ny;
        }

        
        
    }
