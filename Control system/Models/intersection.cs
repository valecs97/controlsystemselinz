using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_system
{
    class intersection
    {
        /*
        Structure of the intersection
        no - the number of the intersection (the id)... all the intersections are represented by an id
        outRoads - all the roads that go from the intersection
        inRoads - all the roads that come to the intersection
        usable - if the intersection can be used (maybe it is being maintaned)
        currentGreenLight - remembers which road has the next greed light

        Constructor received only one argument : the id
        getRoads() - return the roads that go from the intersection
        getGreenLight() - return the next traffic light that should be green (also another function will decide for how long should the light stay)

        The other function are self explanatory
        */
        private int no;
        private List<road> outRoads;
        private List<road> inRoads;
        private List<int> roads;
        private bool usable;
        private int currentGreenLight;
        public intersection(int no)
        {
            this.no = no;
            outRoads = new List<road>();
            inRoads = new List<road>();
            usable = true;
            currentGreenLight = 0;
        }
        public intersection(string value)
        {
            string[] splited = value.Split(' ');
            no = Int32.Parse(splited[0]);
            usable = Convert.ToBoolean(Int32.Parse(splited[1]));
            currentGreenLight = Int32.Parse(splited[2]);
            outRoads = new List<road>();
            inRoads = new List<road>();
        }

        ~intersection()
        {

        }

        public List<road> getRoads()
        {
            return outRoads;
        }

        public int getIntersectionNumber()
        {
            return no;
        }

        public void addRoad(road r)
        {
            outRoads.Add(r);
        }
        
        public void addInRoad(road r)
        {
            inRoads.Add(r);
        }

        public void setUsable()
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

        public void setRoads(List<int> roads)
        {
            this.roads = roads;
        }

        public bool containsRoads(List<int> roadss)
        {
            int match = roads.Count;
            foreach (int r in roads)
            {
                foreach (int i in roadss)
                {
                    if (i == r)
                        match--;
                }
                if (match == 0)
                    return true;
            }
            return false;
        }

        public int getGreenLight()
        {
            int toReturn = currentGreenLight;
            int tries = 0;
            do
            {
                currentGreenLight++;
                if (currentGreenLight == inRoads.Count())
                    currentGreenLight = 0;
                tries++;
            } while (!inRoads[currentGreenLight].isUsable() && tries != inRoads.Count);
            if (tries!=inRoads.Count)
                return toReturn;
            return -1;
        }

        public road getCurrentInRoad()
        {
            return inRoads[currentGreenLight];
        }

        public string toString()
        {
            string toReturn = no.ToString() + " ";
            toReturn += Convert.ToInt32(usable).ToString() + " ";
            toReturn += currentGreenLight + " ";
            return toReturn;
        }
    }
}
