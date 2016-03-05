using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameObjects
{
    public class Tank : GameObject
    {
        public bool shot;
        public int coins, points;

        //constructor
        public Tank()
        {
            Type = GameObjectType.Tank;
        }

        public void setID(int id)
        {
            this.ID = id;
        }
        public int getID()
        {
            return ID;
        }
        public void setShot()
        {
            shot = true;
        }
        public bool isShot()
        {
            return shot;
        }
        public void setCoins(int coins)
        {
            this.coins = coins;
        }
        public int getCoins()
        {
            return coins;
        }
        public void setPoints(int points)
        {
            this.points = points;
        }
        public int getPoints()
        {
            return points;
        }
    }
}
