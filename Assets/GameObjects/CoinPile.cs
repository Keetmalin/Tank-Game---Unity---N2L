using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameObjects
{
    class CoinPile : GameObject
    {
        private long duration;
        int value;

        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public long Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }

        public CoinPile()
        {
            Type = GameObjectType.CoinPile;
        }

        public void setLifeTime(int lifeTime)
        {
            endTime = System.DateTime.Now.AddMilliseconds(lifeTime);
            this.Duration = lifeTime;
        }
        public long getLifeTime()
        {
            return Duration;
        }
        public void setValue(int value)
        {
            this.Value = value;
        }
        public int getValue()
        {
            return Value;
        }

        private System.DateTime endTime;
        public System.DateTime EndTime
        {
            get
            {
                return endTime;
            }
        }
    }
}

