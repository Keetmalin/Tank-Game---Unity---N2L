using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameObjects
{
    class Bullet : GameObject
    {
        int id;

        //constructor
        public Bullet()
        {
            Type = GameObjectType.Bullet;
        }

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

        public const int BULLET_MOVE = 3;

        //bullet moving method
        public void Travel()
        {
            int temp;
            switch (getDirection())
            {
                case DirectionConstants.Down:
                    temp = getY() + BULLET_MOVE;
                    if (temp > Map.MAP_HEIGHT)
                    {
                        die();
                        break;
                    }
                    setY(temp);
                    break;
                case DirectionConstants.Up:
                    temp = getY() - BULLET_MOVE;
                    if (temp < 0)
                    {
                        die();
                        break;
                    }
                    setY(temp);
                    break;
                case DirectionConstants.Left:
                    temp = getX() - BULLET_MOVE;
                    if (temp < 0)
                    {
                        die();
                        break;
                    }
                    setX(temp);
                    break;
                case DirectionConstants.Right:
                    temp = getX() + BULLET_MOVE;
                    if (temp > Map.MAP_WIDTH)
                    {
                        die();
                        break;
                    }
                    setX(temp);

                    break;
            }
        }
    }

}


