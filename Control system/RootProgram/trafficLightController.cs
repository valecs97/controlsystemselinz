using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_system
{
    class trafficLightController
    {
        /*
        getNextLight - return the next traffic light to be green and also the time
        */
        repositoryMap rp;
        public trafficLightController(repositoryMap rp)
        {
            this.rp = rp;
        }
        ~trafficLightController()
        {

        }
        public int getNextLight(int intersectionNo,int capacity)
        {
            int time = rp.getInterstion(intersectionNo).getCurrentInRoad().getCapacity();
            int road = rp.getInterstion(intersectionNo).getGreenLight();
            if (capacity < time / 2)
                time = capacity;
            else
                time = time / 2;
            return time;
        }
    }
}
