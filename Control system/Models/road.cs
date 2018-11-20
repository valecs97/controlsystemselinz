using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_system
{
    class road
    {
        /*
        Structure of the intersection
        from - pointer to the intersection
        to - pointer to the intersection
        distance - numbers of distance units from one end to the other
        capacity - capacity of the road
        usable - if the road is being maintaned or not
        pedestrianOnly - if the road can not be used by cars

        Constructor received 4 arguments : from ,to, distance and capacity
        */
        private intersection from;
        private intersection to;
        private road brodah;
        private int id;
        private int distance;
        private int capacity;
        private bool usable;
        private bool pedestrianOnly;
        public road(int id, intersection from, intersection to, int distance, int capacity, bool pedestrianOnly)
        {
            this.id = id;
            this.from = from;
            this.to = to;
            this.distance = distance;
            this.capacity = capacity;
            usable = true;
            this.pedestrianOnly = pedestrianOnly;
            brodah = null;
        }

        public road(int id, intersection from, intersection to, int distance, int capacity, bool usable, bool pedestrianOnly)
        {
            this.id = id;
            this.from = from;
            this.to = to;
            this.distance = distance;
            this.capacity = capacity;
            this.usable = usable;
            this.pedestrianOnly = pedestrianOnly;
            brodah = null;
        }

        public road(int id, bool usable, int capacity, int distance)
        {
            this.id = id;
            this.distance = distance;
            this.capacity = capacity;
            this.usable = usable;
            pedestrianOnly = false;
            brodah = null;
        }

        public road(int id)
        {
            this.id = id;
            pedestrianOnly = false;
            brodah = null;
        }

        ~road()
        {

        }

        public int getId()
        {
            return id;
        }

        public intersection getFrom()
        {
            return from;
        }

        public intersection getTo()
        {
            return to;
        }

        public int getDistance()
        {
            return distance;
        }

        public int getCapacity()
        {
            return capacity;
        }

        public void setBrodah(road brodah)
        {
            this.brodah = brodah;
        }

        public void setFrom(intersection from)
        {
            this.from = from;
        }
        public void setTo(intersection to)
        {
            this.to = to;
        }

        public void setUsable()
        {
            if (usable)
                usable = false;
            else
                usable = true;
            if (brodah != null)
                brodah.setUsable(0);
        }

        public void setUsable(int onlyThisRoad)
        {
            if (usable)
                usable = false;
            else
                usable = true;
        }

        public bool isUsable()
        {
            return usable;
        }

        public bool isPedestrianOnly()
        {
            return pedestrianOnly;
        }

        public string toString()
        {
            string toReturn = id.ToString() + " ";
            toReturn += from.getIntersectionNumber().ToString() + " ";
            toReturn += to.getIntersectionNumber().ToString() + " ";
            toReturn += distance.ToString() + " ";
            toReturn += capacity.ToString() + " ";
            toReturn += Convert.ToInt32(usable).ToString() + " ";
            toReturn += Convert.ToInt32(pedestrianOnly).ToString() + " ";
            return toReturn;
        }
    }
}
