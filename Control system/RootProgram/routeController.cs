using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Control_system
{
    class routeController
    {
        /*
        It receives one argument : map - the repository with the intersections and roads

        compute simple route : computes the shortest route from A to B , the only thing taken into consideration is if the road is usable or not
        compute route with traffic : computes the shortest route taking into consideration the traffic jams in the city
        compute pedestrian route : computes the shortest route without taking into consideration if the road can be used or not
        */
        private repositoryMap map;
        public routeController(repositoryMap map)
        {
            this.map = map;
        }
        ~routeController()
        {
            map = null;
        }
        public List<int> computeSimpleRoute(int start, int end)
        {
            road startRoad = map.getRoad(start);
            road endRoad = map.getRoad(end);
            intersection end1 = map.getInterstion(endRoad.getFrom().getIntersectionNumber());
            intersection end2 = map.getInterstion(endRoad.getTo().getIntersectionNumber());
            if (startRoad == null)
                throw new System.ArgumentException("Start can not be found !", "Map exception");
            intersection s1 = map.getInterstion(startRoad.getFrom().getIntersectionNumber());
            intersection s2 = map.getInterstion(startRoad.getTo().getIntersectionNumber());
            priorityQueue q = new priorityQueue();
            Dictionary<int, int> prev = new Dictionary<int, int>();
            Dictionary<int, int> dist = new Dictionary<int, int>();
            q.Add(0, s1);
            q.Add(0, s2);
            prev[s1.getIntersectionNumber()] = -1;
            prev[s2.getIntersectionNumber()] = -1;
            dist.Add(s1.getIntersectionNumber(), 0);
            dist.Add(s2.getIntersectionNumber(), 0);
            bool found = false;
            int trueEnding = end1.getIntersectionNumber();
            while (q.Count() != 0)
            {
                intersection current = q.get();
                if (current.isUsable())
                {
                    foreach (road r in current.getRoads())
                    {
                        if (r.isUsable() && !r.isPedestrianOnly())
                        {
                            if (dist.ContainsKey(r.getTo().getIntersectionNumber()) == false || dist[current.getIntersectionNumber()] + r.getDistance() < dist[r.getTo().getIntersectionNumber()])
                            {
                                dist[r.getTo().getIntersectionNumber()] = dist[current.getIntersectionNumber()] + r.getDistance();

                                q.Add(dist[r.getTo().getIntersectionNumber()], r.getTo());
                                prev[r.getTo().getIntersectionNumber()] = current.getIntersectionNumber();
                            }
                        }
                    }
                }
                if (current.getIntersectionNumber() == end1.getIntersectionNumber() || current.getIntersectionNumber() == end2.getIntersectionNumber())
                {
                    if (current.getIntersectionNumber() == end2.getIntersectionNumber())
                        trueEnding = end2.getIntersectionNumber();
                    found = true;
                    break;
                }
            }
            if (!found)
                return null;
            List<int> path = new List<int>();
            path.Add(trueEnding);
            int now = prev[trueEnding];
            while (now != -1)
            {
                path.Add(now);
                now = prev[now];
            }
            path.Reverse();
            List<int> convertedPath = new List<int>();
            convertedPath.Add(start);
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (!convertedPath.Contains(map.getRoad(path[i], path[i + 1]).getId()))
                    convertedPath.Add(map.getRoad(path[i], path[i + 1]).getId());
            }
            convertedPath.Add(end);

            return convertedPath;
        }
        public List<int> computeRouteWithTraffic(int start, int end, Dictionary<Tuple<int, int>, int> heuristic)
        {
            road startRoad = map.getRoad(start);
            road endRoad = map.getRoad(end);
            intersection end1 = map.getInterstion(endRoad.getFrom().getIntersectionNumber());
            intersection end2 = map.getInterstion(endRoad.getTo().getIntersectionNumber());
            if (startRoad == null)
                throw new System.ArgumentException("Start can not be found !", "Map exception");
            intersection s1 = map.getInterstion(startRoad.getFrom().getIntersectionNumber());
            intersection s2 = map.getInterstion(startRoad.getTo().getIntersectionNumber());
            priorityQueue q = new priorityQueue();
            Dictionary<int, int> prev = new Dictionary<int, int>();
            Dictionary<int, int> dist = new Dictionary<int, int>();
            q.Add(0, s1);
            q.Add(0, s2);
            prev[s1.getIntersectionNumber()] = -1;
            prev[s2.getIntersectionNumber()] = -1;
            dist.Add(s1.getIntersectionNumber(), 0);
            dist.Add(s2.getIntersectionNumber(), 0);
            bool found = false;
            int trueEnding = end1.getIntersectionNumber();
            while (q.Count() != 0)
            {
                intersection current = q.get();
                if (current.isUsable())
                {
                    foreach (road r in current.getRoads())
                    {
                        if (r.isUsable() && heuristic[new Tuple<int, int>(r.getFrom().getIntersectionNumber(), r.getTo().getIntersectionNumber())] < r.getCapacity() && !r.isPedestrianOnly())
                        {
                            if (dist.ContainsKey(r.getTo().getIntersectionNumber()) == false || dist[current.getIntersectionNumber()] + r.getDistance() < dist[r.getTo().getIntersectionNumber()])
                            {
                                dist[r.getTo().getIntersectionNumber()] = dist[current.getIntersectionNumber()] + r.getDistance();

                                q.Add(dist[r.getTo().getIntersectionNumber()] + heuristic[new Tuple<int, int>(r.getFrom().getIntersectionNumber(), r.getTo().getIntersectionNumber())], r.getTo());
                                prev[r.getTo().getIntersectionNumber()] = current.getIntersectionNumber();
                            }
                        }
                    }
                }
                if (current.getIntersectionNumber() == end1.getIntersectionNumber() || current.getIntersectionNumber() == end2.getIntersectionNumber())
                {
                    if (current.getIntersectionNumber() == end2.getIntersectionNumber())
                        trueEnding = end2.getIntersectionNumber();
                    found = true;
                    break;
                }
            }
            if (!found)
                return null;
            List<int> path = new List<int>();
            path.Add(trueEnding);
            int now = prev[trueEnding];
            while (now != -1)
            {
                path.Add(now);
                now = prev[now];
            }
            path.Reverse();
            List<int> convertedPath = new List<int>();
            convertedPath.Add(start);
            foreach (int i in path)
                System.Diagnostics.Trace.WriteLine(i);
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (!convertedPath.Contains(map.getRoad(path[i], path[i + 1]).getId()))
                    convertedPath.Add(map.getRoad(path[i], path[i + 1]).getId());
            }
            convertedPath.Add(end);
            return convertedPath;
        }
        public List<int> computePedestrianRoute(int start, int end)
        {
            road startRoad = map.getRoad(start);
            road endRoad = map.getRoad(end);
            intersection end1 = map.getInterstion(endRoad.getFrom().getIntersectionNumber());
            intersection end2 = map.getInterstion(endRoad.getTo().getIntersectionNumber());
            if (startRoad == null)
                throw new System.ArgumentException("Start can not be found !", "Map exception");
            intersection s1 = map.getInterstion(startRoad.getFrom().getIntersectionNumber());
            intersection s2 = map.getInterstion(startRoad.getTo().getIntersectionNumber());
            priorityQueue q = new priorityQueue();
            Dictionary<int, int> prev = new Dictionary<int, int>();
            Dictionary<int, int> dist = new Dictionary<int, int>();
            q.Add(0, s1);
            q.Add(0, s2);
            prev[s1.getIntersectionNumber()] = -1;
            prev[s2.getIntersectionNumber()] = -1;
            dist.Add(s1.getIntersectionNumber(), 0);
            dist.Add(s2.getIntersectionNumber(), 0);
            bool found = false;
            int trueEnding = end1.getIntersectionNumber();
            while (q.Count() != 0)
            {
                intersection current = q.get();
                foreach (road r in current.getRoads())
                {
                    if (dist.ContainsKey(r.getTo().getIntersectionNumber()) == false || dist[current.getIntersectionNumber()] + r.getDistance() < dist[r.getTo().getIntersectionNumber()])
                    {
                        dist[r.getTo().getIntersectionNumber()] = dist[current.getIntersectionNumber()] + r.getDistance();
                        q.Add(dist[r.getTo().getIntersectionNumber()], r.getTo());
                        prev[r.getTo().getIntersectionNumber()] = current.getIntersectionNumber();
                    }
                }
                if (current.getIntersectionNumber() == end1.getIntersectionNumber() || current.getIntersectionNumber() == end2.getIntersectionNumber())
                {
                    if (current.getIntersectionNumber() == end2.getIntersectionNumber())
                        trueEnding = end2.getIntersectionNumber();
                    found = true;
                    break;
                }
            }
            if (!found)
                return null;
            List<int> path = new List<int>();
            path.Add(trueEnding);
            int now = prev[trueEnding];
            while (now != -1)
            {
                path.Add(now);
                now = prev[now];
            }
            path.Reverse();
            List<int> convertedPath = new List<int>();
            convertedPath.Add(start);
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (!convertedPath.Contains(map.getRoad(path[i], path[i + 1]).getId()))
                    convertedPath.Add(map.getRoad(path[i], path[i + 1]).getId());
            }
            convertedPath.Add(end);

            return convertedPath;
        }

        public void addIntersection(int i)
        {
            map.addInterstion(new intersection(i));
        }

        public void addRoad(int id, int from, int to, int capacity, int distance, bool pedestrianOnly)
        {
            map.addRoad(id, from, to, distance, capacity, pedestrianOnly);
        }
    }
}
