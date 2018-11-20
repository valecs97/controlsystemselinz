using Control_system.RootProgram;
using System;
using System.Collections.Generic;
using System.IO;

namespace Control_system
{
    class repositoryMap
    {
        /*
        Repository that stores the intersections and roads
        A intersection the characterised by the id and the pointer to the class
        A road is characterised by a tuple (from intersction x to intersection y) and a pointer to the road class
        noOfIntersections - stores the number of intersections
        */
        private int noOfIntersections;
        private Dictionary<int, intersection> intersections;
        private Dictionary<Tuple<int, int>, road> roads;
        public repositoryMap()
        {
            roads = new Dictionary<Tuple<int, int>, road>();
            intersections = new Dictionary<int, intersection>();
            noOfIntersections = 0;
            try
            {
                foreach (string line in File.ReadAllLines(@"C:\Controlsystem\intersections.txt"))
                {
                    if (line != "")
                    {
                        intersection toAdd = new intersection(line);
                        intersections.Add(toAdd.getIntersectionNumber(), toAdd);
                    }
                }
                noOfIntersections = intersections.Count;
                foreach (string line in File.ReadAllLines(@"C:\Controlsystem\roads.txt"))
                {
                    if (line != "")
                    {
                        string[] separator = line.Split(' ');
                        road toAdd = new road(Int32.Parse(separator[0]), intersections[Int32.Parse(separator[1])], intersections[Int32.Parse(separator[2])], Int32.Parse(separator[3]), Int32.Parse(separator[4]), Convert.ToBoolean(Int32.Parse(separator[5])), Convert.ToBoolean(Int32.Parse(separator[6])));
                        roads.Add(new Tuple<int, int>(toAdd.getFrom().getIntersectionNumber(), toAdd.getTo().getIntersectionNumber()), toAdd);
                        intersections[Int32.Parse(separator[1])].addRoad(toAdd);
                        intersections[Int32.Parse(separator[2])].addRoad(toAdd);
                    }

                }
                noOfIntersections = intersections.Count;
            }
            catch (Exception)
            {
                requestStreetMap();
            }
        }
        public repositoryMap(int nul)
        {
            roads = new Dictionary<Tuple<int, int>, road>();
            intersections = new Dictionary<int, intersection>();
            noOfIntersections = 0;
        }
        ~repositoryMap()
        {
            saveToFile();
        }
        public void saveToFile()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Controlsystem\intersections.txt"))
            {
                foreach (KeyValuePair<int, intersection> value in intersections)
                {
                    file.WriteLine(value.Value.toString());
                }
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Controlsystem\roads.txt"))
            {
                foreach (KeyValuePair<Tuple<int, int>, road> value in roads)
                {
                    file.WriteLine(value.Value.toString());
                }
            }

        }

        public void requestStreetMap()
        {
            string endPoint = @"http://140.78.184.197:8080/streets/streetmap";
            var client = new RestClient(endPoint);
            var json = client.MakeRequest();
            createStreetRepo(json);
        }

        public void createStreetRepo(string data)
        {
            roads = new Dictionary<Tuple<int, int>, road>();
            intersections = new Dictionary<int, intersection>();
            noOfIntersections = 0;
            upload up = new upload();
            int pos = 0, pos2 = 0;
            while ((pos = data.IndexOf("id", pos)) != -1)
            {
                int id = up.convertFirstInt(data, pos);
                pos = data.IndexOf("connectedStreetsLeft", pos);
                pos2 = data.IndexOf("]", pos);
                List<int> leftRoads = new List<int>();
                while (pos < pos2 && pos != -1)
                {
                    leftRoads.Add(up.convertFirstInt(data, pos));
                    pos = data.IndexOfAny(",]".ToCharArray(), pos + 1);
                }
                pos2 = data.IndexOf("connectedStreetsRight", pos);
                pos = data.IndexOf("]", pos + 1);
                List<int> rightRoads = new List<int>();
                while (pos2 < pos && pos2 != -1)
                {
                    rightRoads.Add(up.convertFirstInt(data, pos2));
                    pos2 = data.IndexOfAny(",]".ToCharArray(), pos2 + 1);
                }
                pos = data.IndexOf("closed", pos);
                pos = data.IndexOf(':', pos);
                bool usable = false;
                if (up.convertFirstString(data, pos) == "false")
                    usable = true;
                pos = data.IndexOf("distance", pos);
                int distance = up.convertFirstInt(data, pos);
                road r = new road(id, usable, distance, distance);
                road rOut = new road(id, usable, distance, distance);
                leftRoads.Add(r.getId());
                intersection left = containsTheRoads(leftRoads);
                if (left == null)
                {
                    left = new intersection(noOfIntersections);
                    left.setRoads(leftRoads);
                    noOfIntersections++;
                    intersections.Add(left.getIntersectionNumber(), left);
                }
                left.addInRoad(r);
                left.addRoad(r);
                rightRoads.Add(r.getId());
                intersection right = containsTheRoads(rightRoads);
                if (right == null)
                {
                    right = new intersection(noOfIntersections);
                    right.setRoads(rightRoads);
                    noOfIntersections++;
                    intersections.Add(right.getIntersectionNumber(), right);
                }
                right.addInRoad(r);
                right.addRoad(r);
                r.setFrom(left);
                r.setTo(right);
                rOut.setFrom(right);
                rOut.setTo(left);
                roads.Add(new Tuple<int, int>(r.getFrom().getIntersectionNumber(), r.getTo().getIntersectionNumber()), r);
                roads.Add(new Tuple<int, int>(rOut.getFrom().getIntersectionNumber(), rOut.getTo().getIntersectionNumber()), rOut);
                r.setBrodah(rOut);
                rOut.setBrodah(r);
            }
            saveToFile();
        }

        public void addInterstion(intersection i)
        {
            intersections.Add(i.getIntersectionNumber(), i);
            noOfIntersections++;
            saveToFile();
        }
        public List<road> getAllRoads()
        {
            List<road> toReturn = new List<road>();
            foreach (road r in roads.Values)
            {
                toReturn.Add(r);
            }
            return toReturn;
        }
        public int getNumberOfInterstion()
        {
            return noOfIntersections;
        }
        public intersection getInterstion(int no)
        {
            return intersections[no];
        }

        public void addRoad(int id, int from, int to, int distance, int capacity, bool pedestrianOnly)
        {
            road r = new road(id, intersections[from], intersections[to], distance, capacity, pedestrianOnly);
            intersections[from].addRoad(r);
            intersections[to].addInRoad(r);
            roads.Add(new Tuple<int, int>(from, to), r);
            saveToFile();
        }
        public road getRoad(int from, int to)
        {
            return roads[new Tuple<int, int>(from, to)];
        }

        public road getRoad(int id)
        {
            foreach (road r in roads.Values)
            {
                if (r.getId() == id)
                    return r;
            }
            return null;
        }

        public intersection containsTheRoads(List<int> roads)
        {
            foreach (intersection i in intersections.Values)
            {
                if (i.containsRoads(roads))
                    return i;
            }
            return null;
        }
    }
}
