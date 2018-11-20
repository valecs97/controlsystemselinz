using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Control_system
{
    class upload
    {
        private encryption en;
        public upload()
        {
            en = new encryption();
        }

        private int getTrafficStreet(int id)
        {
            var client = new RootProgram.RestClient();
            client.EndPoint = @"http://140.78.184.197:8080/streets/" + id.ToString();
            client.Method = HttpVerb.GET;
            var json = client.MakeRequest();
            if (json.Contains("Request failed"))
                return -1;
            int pos = json.IndexOf("countParticipants");
            return convertFirstInt(json, pos);
        }

        private Dictionary<Tuple<int,int>,int> getTrafficList(repositoryMap rm)
        {
            Dictionary<Tuple<int, int>, int> toReturn = new Dictionary<Tuple<int, int>, int>();
            foreach (road r in rm.getAllRoads())
            {
                int cap = getTrafficStreet(r.getId());
                toReturn.Add(new Tuple<int, int>(r.getFrom().getIntersectionNumber(), r.getTo().getIntersectionNumber()), cap);
            }
            return toReturn;
        }

        public int convertFirstInt(string command,int pos)
        {
            int first = command.IndexOfAny("0123456789".ToCharArray(), pos);
            int second = command.IndexOfAny("\",\n].".ToCharArray(), first);
            if (first == -1 || second == -1)
                return -1;
            return Int32.Parse(command.Substring(first, second - first));
        }

        public string convertFirstString(string command,int pos)
        {
            string toReturn = "";
            pos = command.IndexOfAny("qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray(), pos + 1);
            if (pos == -1)
                return toReturn;
            while ((command[pos] >= 'a' && command[pos] <= 'z') || (command[pos] >= 'A' && command[pos] <= 'Z'))
            {
                toReturn += command[pos];
                pos++;
            }
            return toReturn;
        }

        private int convertFirstIntWithComma(string command, int pos)
        {
            int first = command.IndexOfAny("0123456789".ToCharArray(), pos + 1);
            int second = command.IndexOf(',', first + 1);
            if (first == -1 || second == -1)
                return -1;
            return Int32.Parse(command.Substring(first, second - first));
        }

        private int convertFirstIntWithoutComma(string command, int pos)
        {
            int first = command.IndexOfAny("0123456789".ToCharArray(), pos + 1);
            int second = command.IndexOf(';', first + 1);
            if (first == -1 || second == -1)
                return -1;
            return Int32.Parse(command.Substring(first, second - first));
        }

        public string decodeCommand(string command, routeController rc,repositoryMap rm)
        {
            try
            {
                exception ex = new exception();
                command = ex.checkData(command);
                if (command == null)
                    return "fail";
                /*
                int pos = command.IndexOf("add_intersection");
                if (pos != -1)
                {
                    pos = command.IndexOf("number");
                    int no = convertFirstInt(command, pos);
                    if (no == -1)
                        return "fail";
                    rc.addIntersection(no);
                    return "succeded";
                }
                pos = command.IndexOf("add_road");
                if (pos != -1)
                {
                    pos = command.IndexOf("id");
                    int id = convertFirstInt(command, pos);
                    pos = command.IndexOf("from");
                    int from = convertFirstInt(command, pos);
                    pos = command.IndexOf("to");
                    int to = convertFirstInt(command, pos);
                    pos = command.IndexOf("capacity");
                    int capacity = convertFirstInt(command, pos);
                    pos = command.IndexOf("distance");
                    int distance = convertFirstInt(command, pos);
                    pos = command.IndexOf("pedestrian_only");
                    int pedestrianOnly = convertFirstInt(command, pos);
                    if (from == -1 || to == -1 || capacity == -1 || distance == -1 || pedestrianOnly == -1)
                        return "fail";
                    rc.addRoad(id, from, to, capacity, distance, Convert.ToBoolean(pedestrianOnly));
                    return "succeded";
                }
                */
                int pos = command.IndexOf("update_street_map");
                if (pos != -1)
                {
                    rm.requestStreetMap();
                    return "succeded";
                }
                pos = command.IndexOf("simple_route");
                if (pos != -1)
                {
                    pos = command.IndexOf("from");
                    int from = convertFirstInt(command, pos);
                    pos = command.IndexOf("to");
                    int to = convertFirstInt(command, pos);
                    if (from == -1 || to == -1)
                        return "fail";
                    List<int> route = rc.computeSimpleRoute(from, to);
                    string message = "{ \"route\":[";
                    for (int i = 0; i < route.Count; i++)
                    {
                        message += route[i].ToString();
                        if (i != route.Count - 1)
                            message += ",";
                    }
                    message += "]}";
                    return en.Encrypt(message, "Some random password");
                }
                pos = command.IndexOf("pedestrian_route");
                if (pos != -1)
                {
                    pos = command.IndexOf("from");
                    int from = convertFirstInt(command, pos);
                    pos = command.IndexOf("to");
                    int to = convertFirstInt(command, pos);
                    if (from == -1 || to == -1)
                        return "fail";
                    List<int> route = rc.computePedestrianRoute(from, to);
                    string message = "{ \"route\":[";
                    for (int i = 0; i < route.Count; i++)
                    {
                        message += route[i].ToString();
                        if (i != route.Count - 1)
                            message += ",";
                    }
                    message += "]}";
                    return en.Encrypt(message, "Some random password");
                }

                pos = command.IndexOf("traffic_route");
                if (pos != -1)
                {
                    pos = command.IndexOf("from");
                    int from = convertFirstInt(command, pos);
                    pos = command.IndexOf("to");
                    int to = convertFirstInt(command, pos);
                    if (from == -1 || to == -1)
                        return "fail";
                    Dictionary<Tuple<int, int>, int> heuristic = getTrafficList(rm);
                    List<int> route = rc.computeRouteWithTraffic(from, to, heuristic);
                    string message = "{\"route\":[";
                    for (int i = 0; i < route.Count; i++)
                    {
                        message += route[i].ToString();
                        if (i != route.Count - 1)
                            message += ",";
                    }
                    message += "]}";
                    return en.Encrypt(message, "Some random password");
                }
                /*
                pos = command.IndexOf("close_intersection");
                if (pos != -1 || (pos = command.IndexOf("open intersection")) == -1)
                {
                    maintananceController mc = new maintananceController(rm);
                    pos = command.IndexOf("intersection");
                    int no = convertFirstInt(command, pos);
                    if (no == -1)
                        return "fail";
                    mc.closeOrOpenIntersection(no);
                    return "succeded";
                }
                */
                pos = command.IndexOf("close_road");
                if (pos != -1 || (pos = command.IndexOf("open_road")) != -1)
                {
                    maintananceController mc = new maintananceController(rm);
                    pos = command.IndexOf("id");
                    int id = convertFirstInt(command, pos);
                    if (id == -1)
                        return "fail";
                    mc.closeOrOpenRoad(id);
                    return "succeded";
                }
                pos = command.IndexOf("traffic_light");
                if (pos != -1)
                {
                    trafficLightController tlc = new trafficLightController(rm);
                    pos = command.IndexOf("id");
                    int no = convertFirstInt(command, pos);
                    if (no == -1)
                        return "fail";

                    int time = tlc.getNextLight(no, getTrafficStreet(no));
                    return en.Encrypt("{ \"time\":\"" + time.ToString() + "\"}", "Some random password");
                }
                /*
                pos = command.IndexOf("close_intersection message");
                if (pos != -1)
                {
                    signController sg = new signController();
                    pos = command.IndexOf("intersection");
                    int no = convertFirstInt(command, pos);
                    if (no == -1)
                        return "fail";
                    return "{\n\"message\":\"" + sg.closeAnIntersection(no) + "\"\n}";
                }
                pos = command.IndexOf("close road message");
                if (pos != -1)
                {
                    signController sg = new signController();
                    pos = command.IndexOf("from");
                    int from = convertFirstInt(command, pos);
                    pos = command.IndexOf("to");
                    int to = convertFirstInt(command, pos);
                    if (from == -1 || to == -1)
                        return "fail";
                    return "{\n\"message\":\"" + sg.closeARoad(new Tuple<int, int>(from,to)) + "\"\n}";
                }
                pos = command.IndexOf("emergency car message");
                if (pos != -1)
                {
                    signController sg = new signController();
                    return "{\n\"message\":\"" + sg.emergencyMessage() + "\"\n}";
                }
                */
                return "fail";
            }
            catch (Exception)
            {
                return "fail";
            }
        }
    }
}
