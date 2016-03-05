using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Assets.GameObjects;

namespace Assets.AI
{
    class AI
    {
        public enum AIMode
        {
            Defensive,
            Offensive,
            Greedy
        }
        private AIMode mode;
        private Map map;
        private Stack<AICell> path;

        private PathFinder pathFinder;

        public AI(Map map)
        {
            // TODO: Complete member initialization
            this.map = map;
            mode = AIMode.Greedy;
            pathFinder = new PathFinder(map);
        }

        internal AIMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }
        AICell target;
        int criticalHealth = 40;

        internal string NextInstruction()
        {
            AICell currentCell = GetCurrentCell();
            Tank client = new List<Tank>(map.getTanks()).Find(delegate(Tank tank)
            {
                if (tank.ID == map.ClientID)
                    return true;
                return false;
            });
            if (client == null)
            {
                return "";
            }
            if (client.Health < criticalHealth)
            {
                Mode = AIMode.Defensive;
            }
            else if (client.Health >= 100)
            {
                Mode = AIMode.Offensive;
            }
            else
            {
                Mode = AIMode.Greedy;
            }
            System.Console.WriteLine("AI Mode : " + System.Enum.GetName(typeof(AIMode), mode));

            // if from the current position shooting will hit a tank and 
            // current cell is safe to reside shoot
            if (IsSafeCell(currentCell) && GoodToShoot())
            {
                return Constant.SHOOT;
            }
            // else tank can move to another cell or stay at the current cell

            // set the target (if target is not set or the target is not alive)

            if (target == null || map.getActor(target.X, target.Y) == null || !map.getActor(target.X, target.Y).Exists)
            {
                CalculateTarget();
            }
            else
            {
                targetCalculatedInLastTerm = false;
            }

            // calculate the shortest path (if current cell is not in the currently calculated
            // path or the target has been changed in the previous step)
            if (path == null || path.Count == 0 || !path.Contains(currentCell) || targetCalculatedInLastTerm)
            {
                path = pathFinder.CalculateShortestPath(target, currentCell);
            }
            if (path.Count > 0)
            {
                AICell nextCell = path.Peek(); // take the peek as if this is brick more method calls will pass before go to the next cell
                Console.WriteLine("Size of the path list : " + path.Count);
                Console.WriteLine("Next Cell : " + nextCell.X + "," + nextCell.Y);
                // Check if the next cell is walkable 
                if (!IsWalkable(nextCell))
                {
                    // if the current cell is safe to shoot at the brick
                    if (IsSafeCell(currentCell))
                    {
                        // if next cell is brick
                        if (nextCell.Type == Actors.ActorType.Brick)
                        {
                            // if tank is facing the brick shoot 
                            if (RotatedTowardsCell(nextCell))
                            {
                                return Constants.SHOOT_INSTRUCTION;
                            }
                            else
                            {
                                // Else rotate towards the next cell
                                return RotateTo(nextCell);
                            }
                        }
                        else
                        {
                            // If next cell is water or stone , as the current cell is safe,

                            return null;
                        }
                    }
                    else
                    {
                        // Move to a safe cell if current cell is not safe
                        return MoveToSafety();
                    }
                }
                else
                {
                    // if next cell is walkable and safe 
                    if (IsSafeCell(nextCell))
                    {
                        //  move to the next cell
                        return MoveToNeighbour(nextCell);
                    }
                    else
                    {
                        // else if current cell is safe stay
                        if (IsSafeCell(currentCell))
                        {
                            return null;
                        }
                        else
                        {
                            // Else move to somewhere safe
                            return MoveToSafety();
                        }
                    }
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the instruction needed to turn towards the provided neighbour cell
        /// </summary>
        /// <param name="nextCell">Next cell which the direction is calculated</param>
        /// <returns>A string representing the direction from Constants.*_INSTRUCTION</returns>
        private string RotateTo(AICell nextCell)
        {
            AICell currentCell = GetCurrentCell();

            if (currentCell.X > nextCell.X)
            {
                return Constants.LEFT_INSTRUCTION;
            }
            if (currentCell.X < nextCell.X)
            {
                return Constants.RIGHT_INSTRUCTION;
            }
            if (currentCell.Y > nextCell.Y)
            {
                return Constants.UP_INSTRUCTION;
            }
            if (currentCell.Y < nextCell.Y)
            {
                return Constants.DOWN_INSTRUCTION;
            }
            return null;
        }

        /// <summary>
        /// Issues a command which directs the tank to the provided neighbour cell
        /// </summary>
        /// <param name="nextCell"></param>
        /// <returns></returns>
        private string MoveToNeighbour(AICell nextCell)
        {
            return RotateTo(nextCell);
        }

        /// <summary>
        /// Issues a command that would direct to a safer cell other than the current cell if possible. Use only current cell is not safety
        /// </summary>
        /// <returns></returns>
        private string MoveToSafety()
        {
            AICell currentCell = GetCurrentCell();
            System.Console.WriteLine("Moving to Safety !");

            if (currentCell.Direction == DirectionConstants.Up)
            {
                // If current direction is up
                AICell cell = GetCell(currentCell.X, currentCell.Y - 1);
                // if upper cell is safe
                if (IsSafeCell(cell))
                {

                    //immediately move to upper cell
                    return Constants.UP_INSTRUCTION;

                }
            }
            else if (currentCell.Direction == DirectionConstants.Down)
            {
                AICell cell = GetCell(currentCell.X, currentCell.Y + 1);
                if (IsSafeCell(cell))
                {
                    return Constants.DOWN_INSTRUCTION;
                }
            }
            else if (currentCell.Direction == DirectionConstants.Left)
            {
                AICell cell = GetCell(currentCell.X - 1, currentCell.Y);
                if (IsSafeCell(cell))
                {
                    return Constants.LEFT_INSTRUCTION;
                }
            }
            else if (currentCell.Direction == DirectionConstants.Right)
            {
                AICell cell = GetCell(currentCell.X + 1, currentCell.Y);
                if (IsSafeCell(cell))
                {
                    return Constants.RIGHT_INSTRUCTION;
                }
            }

            AICell upCell = GetCell(currentCell.X, currentCell.Y - 1);
            AICell downCell = GetCell(currentCell.X, currentCell.Y + 1);
            AICell leftCell = GetCell(currentCell.X - 1, currentCell.Y);
            AICell rightCell = GetCell(currentCell.X + 1, currentCell.Y);

            string decision = null;

            if (IsSafeCell(upCell))
            {
                if (path.Contains(upCell) || upCell.Type == Actors.ActorType.CoinPile || upCell.Type == Actors.ActorType.Lifepack)
                {
                    return Constants.UP_INSTRUCTION;
                }
                else
                {
                    decision = Constants.UP_INSTRUCTION;
                }
            }
            if (IsSafeCell(downCell))
            {
                if (path.Contains(downCell) || downCell.Type == Actors.ActorType.CoinPile || downCell.Type == Actors.ActorType.Lifepack)
                {
                    return Constants.DOWN_INSTRUCTION;
                }
                else
                {
                    decision = Constants.DOWN_INSTRUCTION;
                }
            }
            if (IsSafeCell(leftCell))
            {
                if (path.Contains(leftCell) || leftCell.Type == Actors.ActorType.CoinPile || leftCell.Type == Actors.ActorType.Lifepack)
                {
                    return Constants.LEFT_INSTRUCTION;
                }
                else
                {
                    decision = Constants.LEFT_INSTRUCTION;
                }
            }
            if (IsSafeCell(rightCell))
            {
                if (path.Contains(rightCell) || rightCell.Type == Actors.ActorType.CoinPile || rightCell.Type == Actors.ActorType.Lifepack)
                {
                    return Constants.RIGHT_INSTRUCTION;
                }
                else
                {
                    decision = Constants.RIGHT_INSTRUCTION;
                }
            }

            if (decision != null)
            {
                return decision;
            }

            if (path.Contains(upCell) || upCell.Type == Actors.ActorType.CoinPile || upCell.Type == Actors.ActorType.Lifepack)
            {
                return Constants.UP_INSTRUCTION;
            }
            if (path.Contains(downCell) || downCell.Type == Actors.ActorType.CoinPile || downCell.Type == Actors.ActorType.Lifepack)
            {
                return Constants.DOWN_INSTRUCTION;
            }
            if (path.Contains(leftCell) || leftCell.Type == Actors.ActorType.CoinPile || leftCell.Type == Actors.ActorType.Lifepack)
            {
                return Constants.LEFT_INSTRUCTION;
            }
            if (path.Contains(rightCell) || rightCell.Type == Actors.ActorType.CoinPile || rightCell.Type == Actors.ActorType.Lifepack)
            {
                return Constants.RIGHT_INSTRUCTION;
            }
            return null;
        }

        private AICell GetCell(int x, int y)
        {
            AICell cell = new AICell();
            Actor actor = map.getActor(x, y);
            if (actor == null)
            {
                cell.X = x;
                cell.Y = y;
                cell.Type = Actors.ActorType.Empty;

            }
            else
            {
                cell.X = x;
                cell.Y = y;
                cell.Direction = actor.Direction;
                cell.Type = actor.Type;
            }
            return cell;
        }

        /// <summary>
        /// Gets a cell list of bullets in the map
        /// </summary>
        /// <returns>A list of cells where bullets are</returns>
        private List<AICell> GetBulletCells()
        {
            Bullet[] bullets = map.GetBulletArray();
            List<AICell> list = new List<AICell>();
            foreach (Bullet bullet in bullets)
            {
                AICell cell = new AICell();
                cell.Type = Actors.ActorType.Bullet;
                cell.Direction = bullet.Direction;
                cell.X = bullet.X;
                cell.Y = bullet.Y;
            }
            return list;
        }

        /// <summary>
        /// Checks weather the client is rotated towards the provided cell
        /// </summary>
        /// <param name="nextCell">Target cell</param>
        /// <returns>True if rotated towards the target cell</returns>
        private bool RotatedTowardsCell(AICell nextCell)
        {
            AICell currentCell = GetCurrentCell();

            if (currentCell.X > nextCell.X)
            {
                if (currentCell.Direction == DirectionConstants.Left)
                {
                    return true;
                }
                return false;
            }
            if (currentCell.X < nextCell.X)
            {
                if (currentCell.Direction == DirectionConstants.Right)
                {
                    return true;
                }
                return false;
            }
            if (currentCell.Y < nextCell.Y)
            {
                if (currentCell.Direction == DirectionConstants.Down)
                {
                    return true;
                }
                return false;
            }
            if (currentCell.Y > nextCell.Y)
            {
                if (currentCell.Direction == DirectionConstants.Up)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the provided cell is walkable
        /// </summary>
        /// <param name="nextCell"></param>
        /// <returns></returns>
        private bool IsWalkable(AICell nextCell)
        {
            switch (nextCell.Type)
            {
                case Actors.ActorType.Brick:
                case Actors.ActorType.Stone:
                case Actors.ActorType.Water:
                    return false;
                default:
                    return true;
            }
        }
        private bool targetCalculatedInLastTerm;

        /// <summary>
        /// Calculates the target in case of the target is not set or the ai mode has changed
        /// </summary>
        private void CalculateTarget()
        {
            targetCalculatedInLastTerm = true;
            switch (Mode)
            {
                case AIMode.Defensive:
                    this.target = CalculateTargetLifePack();
                    if (target == null)
                    {
                        this.target = CalculateTargetCoinPile();
                    }
                    if (target == null)
                    {
                        this.target = CalculateTargetBrick();
                    }
                    break;
                case AIMode.Greedy:
                    this.target = CalculateTargetCoinPile();
                    if (target == null)
                    {
                        this.target = CalculateTargetTank();
                    }
                    if (target == null)
                    {
                        this.target = CalculateTargetBrick();
                    }
                    break;
                case AIMode.Offensive:
                    this.target = CalculateTargetTank();
                    if (target == null)
                    {
                        this.target = CalculateTargetCoinPile();
                    }
                    if (target == null)
                    {
                        this.target = CalculateTargetBrick();
                    }
                    break;
            }
            if (target == null)
            {
                target = GetCurrentCell();
            }

        }

        private AICell CalculateTargetBrick()
        {
            List<AICell> bricks = GetBricks();
            AICell target = null;

            int length = 20000;

            foreach (AICell brick in bricks)
            {
                int tempLength = LengthBetweenCells(brick, GetCurrentCell());
                if (tempLength < length)
                {
                    length = tempLength;
                    target = brick;
                }
            }
            return target;
        }

        private List<AICell> GetBricks()
        {
            List<AICell> bricks = new List<AICell>();
            Brick[] brickArray = map.GetBricks();

            foreach (Brick brick in brickArray)
            {
                AICell bri = new AICell();
                bri.X = brick.X;
                bri.Y = brick.Y;
                bri.Health = brick.Health;
                bricks.Add(bri);
            }
            return bricks;
        }

        /// <summary>
        /// Calculate the target setting the optimum tank as the target
        /// </summary>
        private AICell CalculateTargetTank()
        {
            List<AICell> tanks = GetTankCells();

            AICell target = null;

            foreach (AICell tank in tanks)
            {

                if (target != null)
                {
                    if (target.Health > tank.Health)
                    {
                        target = tank;
                    }
                }
                else
                {
                    target = tank;
                }
            }
            return target;
        }
        /// <summary>
        /// Calculate the target setting the optimum coin pile as the target
        /// </summary>
        private AICell CalculateTargetCoinPile()
        {
            List<AICell> coins = GetCoins();
            AICell target = null;
            //not implemented yet. Has to consider the lifetime and the time to reach there

            int length = 20000;

            foreach (AICell coin in coins)
            {
                int tempLength = LengthBetweenCells(coin, GetCurrentCell());
                if (tempLength < length)
                {
                    length = tempLength;
                    target = coin;
                }
            }
            return target;
        }

        /// <summary>
        /// Calculates a rough approximation of the length between given cells
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="aICell"></param>
        /// <returns></returns>
        private int LengthBetweenCells(AICell cell1, AICell cell2)
        {
            // Has to improve more

            int length = Math.Abs(cell1.X - cell2.X) + Math.Abs(cell1.Y - cell2.Y);

            return length;
        }

        private List<AICell> GetCoins()
        {
            List<AICell> coins = new List<AICell>();
            CoinPile[] coinpiles = map.GetCoins();

            foreach (CoinPile coinPile in coinpiles)
            {
                AICell coin = new AICell();
                coin.Value = coinPile.Value;
                coin.LifeTime = coinPile.LifeTime;
                coin.X = coinPile.X;
                coin.Y = coinPile.Y;
                coin.Type = Actors.ActorType.CoinPile;
                coins.Add(coin);

            }
            return coins;
        }
        /// <summary>
        /// Calculate the target setting the optimum life pack as the target
        /// </summary>
        private AICell CalculateTargetLifePack()
        {
            List<AICell> lifePacks = GetLifePacks();
            AICell target = null;
            //not implemented yet. Has to consider the lifetime and the time to reach there

            int length = 20000;

            foreach (AICell lifePack in lifePacks)
            {
                int tempLength = LengthBetweenCells(lifePack, GetCurrentCell());
                if (tempLength < length)
                {
                    length = tempLength;
                    target = lifePack;
                }
            }
            return target;
        }

        private List<AICell> GetLifePacks()
        {
            List<AICell> lifepacks = new List<AICell>();
            LifePack[] lifeArray = map.GetLifePacks();

            foreach (LifePack pack in lifeArray)
            {
                AICell lifePack = new AICell();
                lifePack.Type = Actors.ActorType.Lifepack;
                lifePack.X = pack.X;
                lifePack.Y = pack.Y;
                lifePack.Value = pack.Value;
                lifePack.LifeTime = pack.LifeTime;
                lifepacks.Add(lifePack);
            }
            return lifepacks;
        }

        private AICell GetCurrentCell()
        {
            return GetCell(map.ClientX, map.ClientY);
        }

        /// <summary>
        /// Check whether there is a good chance of hitting if shot from current position
        /// </summary>
        /// <returns>true if there is a good chance of a hit</returns>
        private bool GoodToShoot()
        {

            AICell currentCell = GetCurrentCell();
            if (currentCell.Direction == DirectionConstants.Up && GoodToShootColumnUp())
            {
                return true;
            }
            if (currentCell.Direction == DirectionConstants.Down && GoodToShootColumnDown())
            {
                return true;
            }
            if (currentCell.Direction == DirectionConstants.Left && GoodToShootColumnLeft())
            {
                return true;
            }
            if (currentCell.Direction == DirectionConstants.Right && GoodToShootColumnRight())
            {
                return true;
            }
            return false;
        }

        private bool GoodToShootColumnRight()
        {
            return GoodToShootRow(DirectionConstants.Right);
        }

        private bool GoodToShootRow(DirectionConstants direction)
        {
            int k = 0;
            AICell currentCell = GetCurrentCell();

            if (direction == DirectionConstants.Right)
            {
                k = 1;
            }
            else if (direction == DirectionConstants.Left)
            {
                k = -1;
            }
            if (GetCell(currentCell.X + k, currentCell.Y).Type == Actors.ActorType.Brick)
            {
                return true;
            }
            for (int x = currentCell.X + k; x < Map.MAP_WIDTH && x > 0; x += k)
            {
                AICell cell = GetCell(x, currentCell.Y);
                if (cell.Type == Actors.ActorType.Stone)
                {
                    return false;
                }
                if (cell.Type == Actors.ActorType.Tank)
                {
                    return true;
                }
            }
            return false;
        }

        private bool GoodToShootColumn(DirectionConstants direction)
        {
            int k = 0;
            AICell currentCell = GetCurrentCell();

            if (direction == DirectionConstants.Down)
            {
                k = 1;
            }
            else if (direction == DirectionConstants.Up)
            {
                k = -1;
            }
            if (GetCell(currentCell.X, currentCell.Y + k).Type == Actors.ActorType.Brick)
            {
                return true;
            }
            for (int y = currentCell.Y + k; y < Map.MAP_HEIGHT && y > 0; y += k)
            {
                AICell cell = GetCell(currentCell.X, y);
                if (cell.Type == Actors.ActorType.Stone)
                {
                    return false;
                }
                if (cell.Type == Actors.ActorType.Tank)
                {
                    return true;
                }
            }
            return false;
        }

        private bool GoodToShootColumnLeft()
        {
            return GoodToShootRow(DirectionConstants.Left);
        }

        private bool GoodToShootColumnDown()
        {
            return GoodToShootColumn(DirectionConstants.Down);
        }

        private bool GoodToShootColumnUp()
        {
            return GoodToShootColumn(DirectionConstants.Up);
        }

        /// <summary>
        /// Check if the provided cell is safe from the bullets and the tanks plus walkable.
        /// </summary>
        /// <param name="cell">The cell which is needs to be checked</param>
        /// <returns>true if the cell is safe to reside or to move</returns>
        private bool IsSafeCell(AICell cell)
        {
            List<AICell> bullets = GetBulletCells();
            AICell currentCell = GetCurrentCell();
            int safeLengthFromBullets = 8;
            //   int criticalLengthFromBullet = 4;

            foreach (AICell tempCell in bullets)
            {
                //If the bullet is in the same row as the tank
                if (tempCell.Y == cell.Y)
                {
                    // If considered bullet is within 'safeLengthFromBullets' in the right side of the tank
                    if (tempCell.X - cell.X > 0 && tempCell.X - cell.X < safeLengthFromBullets)
                    {
                        // If the bullet is coming towards the tank
                        if (tempCell.Direction == DirectionConstants.Left)
                        {
                            return false;

                        }

                    }// If considered bullet is within 'safeLengthFromBullets' in the left side of the tank
                    else if (cell.X - tempCell.X > 0 && cell.X - tempCell.X < safeLengthFromBullets)
                    {
                        // If the bullet is coming towards the tank
                        if (tempCell.Direction == DirectionConstants.Right)
                        {
                            return false;

                        }
                    }
                }// if the bullet is in the same column as the tank
                else if (tempCell.X == cell.X)
                {
                    // If considered bullet is within 'safeLengthFromBullets' in the upper side of the tank
                    if (tempCell.Y - cell.Y > 0 && tempCell.Y - cell.Y < safeLengthFromBullets)
                    {
                        // If the bullet is coming towards the tank
                        if (tempCell.Direction == DirectionConstants.Up)
                        {
                            return false;
                        }

                    }// If considered bullet is within 'safeLengthFromBullets' in the bottom side of the tank
                    else if (cell.Y - tempCell.Y > 0 && cell.Y - tempCell.Y < safeLengthFromBullets)
                    {
                        // If the bullet is coming towards the tank
                        if (tempCell.Direction == DirectionConstants.Down)
                        {
                            return false;
                        }
                    }
                }
            }

            List<AICell> tanks = GetTankCells();

            int safeLengthFromTanks = 8;
            int criticalLengthFromTanks = 4;

            foreach (AICell tank in tanks)
            {
                Actor actor = map.getActor(tank.X, tank.Y);
                //If there is a tank at the tank cell
                if (actor.Type == Actors.ActorType.Tank)
                {
                    Tank tankTemp = actor as Tank;
                    //If the current tank is client tank ignore the iteration
                    if (tankTemp.ID == map.ClientID)
                    {
                        continue;
                    }
                }

                // If the tank is in the same row as the client tank
                if (tank.Y == cell.Y)
                {
                    //If considered tank is within 'safeLengthFromTank' in the right side of the tank
                    if (tank.X - cell.X > 0 && tank.X - cell.X < safeLengthFromTanks)
                    {
                        //If the tank is facing towards the tank
                        if (tank.Direction == DirectionConstants.Left)
                        {
                            return false;
                        }

                    }// If considered tank is within 'safeLengthFromTank' in the left side of the tank
                    else if (cell.X - tank.X > 0 && cell.X - tank.X < safeLengthFromTanks)
                    {
                        // If the bullet is coming towards the tank
                        if (tank.Direction == DirectionConstants.Right)
                        {
                            return false;

                        }
                    }
                }// if the tank is in the same column as the tank
                else if (tank.X == cell.X)
                {
                    //  If considered tank is within 'safeLengthFromTank' in the north side of the tank
                    if (tank.Y - cell.Y > 0 && tank.Y - cell.Y < safeLengthFromTanks)
                    {
                        // If the bullet is coming towards the tank
                        if (tank.Direction == DirectionConstants.Up)
                        {
                            return false;
                        }

                    }// If considered tank is within 'safeLengthFromTank' in the south side of the tank
                    else if (cell.Y - tank.Y > 0 && cell.Y - tank.Y < safeLengthFromBullets)
                    {
                        // If the bullet is coming towards the tank
                        if (tank.Direction == DirectionConstants.Down)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a list of tanks in the map
        /// </summary>
        /// <returns>A list of the tanks in the map</returns>
        private List<AICell> GetTankCells()
        {
            List<AICell> tanks = new List<AICell>();
            Tank[] tankArray = map.getTanks();
            foreach (Tank tank in tankArray)
            {
                if (tank.ID == map.ClientID)
                {
                    continue;
                }
                AICell cell = new AICell();
                cell.Type = Actors.ActorType.Tank;
                cell.X = tank.X;
                cell.Y = tank.Y;
                cell.Direction = tank.Direction;
                cell.Health = tank.Health;
                cell.Points = tank.points;

                tanks.Add(cell);
            }
            return tanks;
        }


    }
    class AICell
    {

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public Actors.ActorType Type
        {
            get;
            set;
        }

        public DirectionConstants Direction
        {
            get;
            set;
        }

        public int Health
        {
            get;
            set;
        }

        public int Points
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }

        public long LifeTime
        {
            get;
            set;
        }
    }
}
