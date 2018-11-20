using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_system
{
    class maintananceController
    {
        /*
        Class for the maintanance services
        It sets the road/intersection usable or unusable
        */

        repositoryMap rp;
        public maintananceController(repositoryMap rp)
        {
            this.rp = rp;
        }

        public void closeOrOpenRoad(int id)
        {
            rp.getRoad(id).setUsable();
        }

        public void closeOrOpenIntersection(int no)
        {
            rp.getInterstion(no).setUsable();
        }
    }

    
}
