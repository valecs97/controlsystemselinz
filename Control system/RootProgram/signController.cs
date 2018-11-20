using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_system
{
    class signController
    {
        /*
        Returns string for the signs to show
        */
        public signController()
        {

        }
        ~signController()
        {

        }

        public string closeAnIntersection(int intersection)
        {
            return "Intersection number " + intersection.ToString() + " is closed !";
        }
        public string closeARoad(Tuple<int,int> road)
        {
            return "Road from intersection number " + road.Item1.ToString() + " to intersection " + road.Item2.ToString() +  " is closed !";
        }
        public string emergencyMessage()
        {
            return "Emergency car is incomming , please move your car aside of the road !";
        }
    }
}
