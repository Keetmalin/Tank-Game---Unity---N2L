using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameObjects
{
    class Brick : GameObject
    {
        public Brick()
        {
            Type = GameObjectType.Brick;
            Health = 100000;
        }
    }
}
