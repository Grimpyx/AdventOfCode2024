using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day23 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // 11011 too high
            // 1046 correct. I prindet the wrong result!!

            // Makes a list for all rows where each is divided into a tuple, example: "kh-tc" becomes (kh, tc)
            List<(string from, string to)> connections = DataLoader.LoadRowData(23, fullOrSimpleData)
                .Select(row => row.Split('-'))
                .Select(x => (x[0], x[1])).ToList();

            // Connection map
            Dictionary<string, HashSet<string>> connectionMap = new Dictionary<string, HashSet<string>>();
            foreach (var connection in connections)
            {
                if (connectionMap.TryGetValue(connection.from, out var connectionSet))
                    connectionSet.Add(connection.to);
                else
                    connectionMap.Add(connection.from, new HashSet<string>([connection.to]));


                if (connectionMap.TryGetValue(connection.to, out connectionSet))
                    connectionSet.Add(connection.from);
                else
                    connectionMap.Add(connection.to, new HashSet<string>([connection.from]));
            }

            /*foreach (var connectionMapPair in connectionMap) //.Where(x => x.Value.Count >= 2)
            {
                Console.WriteLine(connectionMapPair.Key + " => " + string.Join(',', connectionMapPair.Value));
            }*/

            HashSet<HashSet<string>> completedSets = new HashSet<HashSet<string>>();
            foreach (var connection in connectionMap)
            {
                string[] group = new string[3];
                group[0] = connection.Key;

                foreach (var next in connection.Value)
                {
                    group[1] = next;

                    foreach (var last in connectionMap[next])
                    {
                        group[2] = last;

                        if (connectionMap[last].Contains(group[0]) && group[0] != group[2])
                        {
                            HashSet<string> groupSet = new HashSet<string>([group[0], group[1], group[2]]);
                            bool abandon = false;
                            foreach (var set in completedSets)
                            {
                                if (groupSet.SetEquals(set))
                                {
                                    abandon = true;
                                    break;
                                }
                            }
                            if (abandon) continue;
                            completedSets.Add(groupSet);

                            if (fullOrSimpleData == DayDataType.Simple) Console.WriteLine(string.Join(',', group));
                        }
                    }
                }
            }

            Console.WriteLine(" >>> Result: " + completedSets.Select(set => set.Select(x => x[0])).Where(x => x.Contains('t')).Count());

            Console.WriteLine();

            /*// Create groups of computers
            HashSet<string> visited = new HashSet<string>();

            List<List<string>> computerGroups = new List<List<string>>();
            foreach (var from in connectionMap.Keys)
            {
                Queue<string> queue = new Queue<string>();
                queue.Enqueue(from);
                List<string> group = [];

                // Step through the group

                while (queue.Count > 0)
                {
                    string current = queue.Dequeue();

                    if (!visited.Add(current))
                        continue; // if already visited

                    group.Add(current);

                    foreach (var connected in connectionMap[current])
                    {
                        queue.Enqueue(connected);
                    }

                }
                if (group.Count > 0)
                    computerGroups.Add(group);
            }

            foreach (var item in computerGroups)
            {
                Console.WriteLine(string.Join(',', item));
            }*/


            Console.WriteLine();


        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }


    }
}
