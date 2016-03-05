using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameObjects
{
    public class GameObject
    {
        //Game object variables
        private int health;
        private int id;
        private int x, y;
        private GameObjectType type;

        private DirectionConstants direction;

        private bool alive;

        //constructor
        public GameObject()
        {
            alive = true;
            id = -1;
        }

        //internal getter setter
        internal GameObjectType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        //public getters setters 
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
            }
        }


        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public DirectionConstants Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        //getter setter for Alive variable
        public bool Alive
        {
            get
            {
                if (Type == GameObjectType.Tank || Type == GameObjectType.Brick)
                {
                    if (health <= 0)
                    {
                        return false;
                    }
                }
                else if (Type == GameObjectType.Lifepack)
                {
                    LifePack pack = (LifePack)this;

                    if (pack.EndTime.CompareTo(System.DateTime.Now) <= 0)
                    {
                        return false;
                    }
                }
                else if (Type == GameObjectType.CoinPile)
                {
                    CoinPile coin = (CoinPile)this;
                    if (coin.EndTime.CompareTo(System.DateTime.Now) <= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            set
            {
                alive = value;
            }
        }

        //getter setter methods
        public int getHealth()
        {
            return Health;
        }

        public int getX()
        {
            return X;
        }
        public int getY()
        {
            return Y;
        }
        public DirectionConstants getDirection()
        {
            return Direction;
        }

        public void setHealth(int health)
        {
            this.Health = health;
            if (health <= 0)
            {
                die();
            }
        }

        public void setX(int x)
        {
            this.X = x;
        }
        public void setY(int y)
        {
            this.Y = y;
        }
        public void setDirection(DirectionConstants direction)
        {
            this.Direction = direction;
        }

        
        public void die()
        {
            this.Alive = false;
        }

        public bool isAlive()
        {
            return Alive;
        }
    }
}

