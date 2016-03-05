using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Assets.GameObjects;

/// <summary>
/// Represents a map. X direction is positive to the right and 
/// Y direction is positive to the south/downward
/// </summary>
public class Map
{
    private List<Tank> tanks;
    private List<Brick> bricks;
    private List<LifePack> lifePacks;
    private List<CoinPile> coinPiles;
    private List<Stone> stones;
    private List<Water> waters;
    private List<Bullet> bullets;
    public const int MAP_WIDTH = 10, MAP_HEIGHT = 10;
    private bool updatedForAI, updatedForView;
    private int clientID;

    public int ClientID
    {
        get
        {
            return clientID;
        }
        set
        {
            if (value >= 0)
                clientID = value;
        }
    }

    private int clientX, clientY;

    DirectionConstants clientDirection;

    public DirectionConstants ClientDirection
    {
        get
        {
            return clientDirection;
        }
        set
        {
            clientDirection = value;
        }
    }

    int nextGameObjectID;
    public const int NO_OF_PLAYERS = 5;
    bool joined;

    public Map()
    {
        tanks = new List<Tank>();
        bricks = new List<Brick>();
        lifePacks = new List<LifePack>();
        coinPiles = new List<CoinPile>();
        stones = new List<Stone>();
        waters = new List<Water>();
        bullets = new List<Bullet>();
        nextGameObjectID = 20;

        clientID = -1;// starts with an invalid client ID

        setUpdated();
    }

    public void clearTanks()
    {
        tanks.Clear();
    }
    public void clearBricks()
    {
        bricks.Clear();
    }
    public Bullet[] GetBulletArray()
    {
        return bullets.ToArray();
    }
    public int ClientY
    {
        get
        {
            return clientY;
        }
        set
        {
            clientY = value;
        }
    }

    public int ClientX
    {
        get
        {
            return clientX;
        }
        set
        {
            clientX = value;
        }
    }
    /// <summary>
    /// Automatically move the bullets within the map and change there existence accordingly.
    /// </summary>
    public void proceedBullets()
    {
        foreach (Tank tank in tanks)
        {
            if (tank.isShot())
            {
                Bullet bullet = new Bullet();
                bullet.X = tank.X;
                bullet.Y = tank.Y;
                bullet.Direction = tank.Direction;
                bullet.Alive = true;
                bullet.ID = nextGameObjectID++;
                bullets.Add(bullet);
            }
        }
        foreach (Bullet bullet in bullets)
        {
            if (!bullet.Alive)
            {
                bullets.Remove(bullet);
            }
            else
            {
                int firstX, firstY, endX, endY;
                firstX = bullet.X;
                firstY = bullet.Y;
                bullet.Travel();
                endX = bullet.X;
                endY = bullet.Y;
                if (firstX == endX)// moving in Y axis
                {
                    if (firstY > endY)
                    {
                        for (int i = endY; i < firstY; i++)
                        {
                            GameObject gameobject = getGameObject(firstX, i);
                            System.Type type = gameobject.GetType();
                            if (type == typeof(Tank) || type == typeof(Brick) || type == typeof(Stone))
                            {
                                bullet.die();
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = endY; i > firstY; i--)
                        {
                            GameObject gameobject = getGameObject(firstX, i);
                            System.Type type = gameobject.GetType();
                            if (type == typeof(Tank) || type == typeof(Brick) || type == typeof(Stone))
                            {
                                bullet.die();
                                break;
                            }
                        }
                    }
                }
                else if (firstY == endY)// moving in x axis
                {
                    if (firstX > endX)
                    {
                        for (int i = endX; i < firstX; i++)
                        {
                            GameObject gameobject = getGameObject(i, firstY);
                            System.Type type = gameobject.GetType();
                            if (type == typeof(Tank) || type == typeof(Brick) || type == typeof(Stone))
                            {
                                bullet.die();
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = endX; i > firstX; i--)
                        {
                            GameObject gameobject = getGameObject(i, firstY);
                            System.Type type = gameobject.GetType();
                            if (type == typeof(Tank) || type == typeof(Brick) || type == typeof(Stone))
                            {
                                bullet.die();
                                break;
                            }
                        }
                    }
                }

            }
        }
    }

    /// <summary>
    /// Adds an gameobject to the map
    /// </summary>
    /// <param name="gameobject"></param>
    public void setGameObject(GameObject gameobject)
    {
        if (gameobject.GetType() == typeof(Tank))
        {
            Tank update = (Tank)gameobject;

            foreach (Tank tank in tanks)
            {
                if (tank.ID == update.ID)
                {
                    tank.setCoins(update.getCoins());
                    tank.setDirection(update.getDirection());
                    tank.setHealth(update.getHealth());
                    tank.setPoints(update.getPoints());
                    tank.setX(update.getX());
                    tank.setY(update.getY());
                    if (update.isShot())
                    {
                        tank.setShot();
                    }
                    setUpdated();
                    return;
                }
            }

            tanks.Add(update);
            setUpdated();
        }
        else if (gameobject.GetType() == typeof(Brick))
        {
            Brick update = (Brick)gameobject;
            foreach (Brick brick in bricks)
            {
                if (brick.getX() == update.getX() && brick.getY() == update.getY())
                {
                    if (brick.Health < 10000)
                    {
                        brick.setHealth(update.getHealth());
                    }
                    setUpdated();
                    return;
                }
            }
            update.ID = nextGameObjectID++;
            update.Health = 100;
            bricks.Add(update);
            setUpdated();
        }
        else if (gameobject.GetType() == typeof(LifePack))
        {
            gameobject.ID = nextGameObjectID++;
            lifePacks.Add((LifePack)gameobject);
        }
        else if (gameobject.GetType() == typeof(CoinPile))
        {
            gameobject.ID = nextGameObjectID++;
            coinPiles.Add((CoinPile)gameobject);
        }
        else if (gameobject.GetType() == typeof(Stone))
        {
            gameobject.ID = nextGameObjectID++;
            stones.Add((Stone)gameobject);
        }
        else if (gameobject.GetType() == typeof(Water))
        {
            gameobject.ID = nextGameObjectID++;
            waters.Add((Water)gameobject);
        }
        setUpdated();
    }
    /// <summary>
    /// Gets the gameobject at given coords
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public GameObject getGameObject(int x, int y)
    {
        foreach (Tank gameobject in tanks)
        {
            if (gameobject.getX() == x && gameobject.getY() == y)
            {
                return gameobject;
            }
        }
        foreach (Brick gameobject in bricks)
        {
            if (gameobject.getX() == x && gameobject.getY() == y)
            {
                return gameobject;
            }
        }
        foreach (CoinPile gameobject in coinPiles)
        {
            if (gameobject.getX() == x && gameobject.getY() == y)
            {
                return gameobject;
            }
        }
        foreach (LifePack gameobject in lifePacks)
        {
            if (gameobject.getX() == x && gameobject.getY() == y)
            {
                return gameobject;
            }
        }
        foreach (Stone gameobject in stones)
        {
            if (gameobject.getX() == x && gameobject.getY() == y)
            {
                return gameobject;
            }
        }
        foreach (Water gameobject in waters)
        {
            if (gameobject.getX() == x && gameobject.getY() == y)
            {
                return gameobject;
            }
        }
        return null;
    }
    public bool isUpdatedForView()
    {
        if (updatedForView)
        {
            updatedForView = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool isUpdatedForAI()
    {
        if (updatedForAI)
        {
            updatedForAI = false;
            return updatedForAI;
        }
        else
        {
            return false;
        }
    }

    public void setUpdated()
    {
        updatedForView = true;
        updatedForAI = true;

    }
    private long lastTime = System.DateTime.Now.Ticks / 10000;

    public void setClientID(int clientID)
    {
        this.ClientID = clientID;
    }
    public int getClientID()
    {
        return ClientID;
    }
    public Tank[] getTanks()
    {
        return tanks.ToArray();
    }
    public bool Joined
    {
        set
        {
            joined = value;
        }
        get
        {
            return joined;
        }
    }
    public List<GameObject> getGameObjects()
    {
        List<GameObject> gameobjects = new List<GameObject>();
        gameobjects.AddRange(tanks);
        gameobjects.AddRange(bricks);
        gameobjects.AddRange(bullets);
        gameobjects.AddRange(coinPiles);
        gameobjects.AddRange(lifePacks);
        gameobjects.AddRange(stones);
        gameobjects.AddRange(waters);

        return gameobjects;
    }

    public List<Actor> getActors()
    {
        List<Actor> actors = new List<Actor>();
        actors.AddRange(tanks);
        actors.AddRange(bricks);
        actors.AddRange(bullets);
        actors.AddRange(coinPiles);
        actors.AddRange(lifePacks);
        actors.AddRange(stones);
        actors.AddRange(waters);

        return actors;
    }

    internal CoinPile[] GetCoins()
    {
        return coinPiles.ToArray();
    }

    internal LifePack[] GetLifePacks()
    {
        return lifePacks.ToArray();
    }

    internal Brick[] GetBricks()
    {
        return bricks.ToArray();
    }

    /// <summary>
    /// Clears the dead gameobjects in the map
    /// </summary>
    /// <returns></returns>
    public int clearDead()
    {
        int count = 0;
        count += tanks.RemoveAll(searchDead);
        count += coinPiles.RemoveAll(searchDead);
        count += lifePacks.RemoveAll(searchDead);
        count += bricks.RemoveAll(searchDead);
        count += bullets.RemoveAll(searchDead);
        count += coinPiles.RemoveAll(searchAquiredDead);
        count += lifePacks.RemoveAll(searchAquiredDead);
        System.Console.WriteLine("Deleted GameObjects : " + count);
        System.Console.WriteLine("No of Bricks : " + bricks.Count);
        return count;
    }
    /// <summary>
    /// Predicate for search the dead
    /// </summary>
    /// <returns></returns>
    private static bool searchDead(GameObject gameobject)
    {
        //System.Console.WriteLine("Health : " + gameobject.Health);
        return !gameobject.Alive;
    }
    /// <summary>
    /// Search for the coinpiles and lifepacks which have been run over by tanks
    /// </summary>
    /// <returns></returns>
    private bool searchAquiredDead(GameObject gameobject)
    {
        if (gameobject.Type == GameObjectType.CoinPile || gameobject.Type == GameObjectType.Lifepack)
        {
            foreach (Tank tank in tanks)
            {
                if (gameobject.X == tank.X && gameobject.Y == tank.Y)
                {
                    gameobject.Alive = false;
                    return true;
                }
            }
        }
        return false;
    }
}
