using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_system
{
    public abstract class Metadata
    {
    }

    class priorityQueue
    {
        /*
        A priority queue implementation
        Because the c# doesnt contain one...
        */
        List<Tuple<int,intersection>> q;
        public priorityQueue()
        {
            q = new List<Tuple<int, intersection>>();
        }

        public void Add(int distance,intersection i)
        {
            if (q.Count()==0)
            {
                q.Add(new Tuple<int, intersection>(distance, i));
                return;
            }
            int le = 0;
            int ri = q.Count()-1;
            int mid = 0;
            while (le <= ri)
            {
                mid = (le + ri) / 2;
                if (distance < q[mid].Item1)
                {
                    le = mid + 1;
                }
                else if (distance > q[mid].Item1)
                {
                    ri = mid - 1;
                }
                else break;
            }
            q.Add(new Tuple<int, intersection>(distance, i));
            if (distance>q[mid].Item1)
            {
                for (int j = q.Count-1; j > mid; j--)
                    q[j] = q[j - 1];
                q[mid] = new Tuple<int, intersection>(distance, i);
            }
            else
            {
                for (int j = q.Count-1; j > mid+1; j--)
                    q[j] = q[j - 1];
                q[mid+1] = new Tuple<int, intersection>(distance, i);
            }
        }

        public intersection get()
        {
            intersection i = q[q.Count() - 1].Item2;
            q.RemoveAt(q.Count() - 1);
            return i;
        }

        public int Count()
        {
            return q.Count();
        }
    }
}
