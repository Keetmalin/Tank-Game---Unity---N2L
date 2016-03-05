using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameObjects
{
    class LifePack : GameObject
    {
        private long duration;
        private int value;

        private System.DateTime endTime;

        //construct
        public LifePack()
        {
            Type = GameObjectType.Lifepack;
        }

        //public getters setters
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

        public System.DateTime EndTime
        {
            get
            {
                return endTime;
            }
        }

        //getter setter methods
        public void setDuration(int duration)
        {
            endTime = System.DateTime.Now.AddMilliseconds(duration);
            this.Duration = duration;
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
    }
}
