using AdventOfCode2024.Utilities;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace AdventOfCode2024.Days
{
    class Day23 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // 11011 too high
            // 1046 correct. I printed the wrong result!!

            // Anyhow, this solution is suuuperduper slow (for me 23 seconds). Part 2 is much better.

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

            if (fullOrSimpleData == DayDataType.Simple)
            {
                foreach (var connectionMapPair in connectionMap) //.Where(x => x.Value.Count >= 2)
                {
                    Console.WriteLine(connectionMapPair.Key + " => " + string.Join(',', connectionMapPair.Value));
                }
            }

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

        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // Makes a list for all rows where each is divided into a tuple, example: "kh-tc" becomes (kh, tc)
            List<(string from, string to)> connections = DataLoader.LoadRowData(23, fullOrSimpleData)
                .Select(row => row.Split('-'))
                .Select(x => (x[0], x[1])).ToList();

            // Connection map
            // Establishes all connections leading from all computers
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

            // Create groups of computers
            HashSet<string> visited = new HashSet<string>();
            List<List<string>> computerGroups = new List<List<string>>();
            foreach (var from in connectionMap.Keys)
            {
                // To get all connected computers we can do the following:
                // Select a starting computer. Check what computers it is connected to.
                // For each connected computer, calculate the intersection of its connections.
                // When this is done you should have a set that only contains computers that have direct connections to eachother.
                HashSet<string> groupIntersection = [from, .. connectionMap[from]];
                foreach (var str in groupIntersection)
                {
                    HashSet<string> compareSet = [str, .. connectionMap[str]];
                    groupIntersection.IntersectWith(compareSet);
                }

                // If groupintersection is a subset we have already visited this group of computers (but from another's perspeective)
                if (groupIntersection.Count > 0 && !groupIntersection.IsSubsetOf(visited))
                {
                    computerGroups.Add([.. groupIntersection]);
                    foreach (var item in groupIntersection)
                    {
                        visited.Add(item);
                    }
                }
            }

            // Print results
            TextUtilities.CFWLine("@Gra------|  @DYeAll sets:");
            int i = 0;
            foreach (var item in computerGroups)
            {
                TextUtilities.CFWLine($"@Red  {i,-3:D2}@Gra |  @Yel{string.Join("@Gra,@Yel", item)}@Gra");
                i++;
            }

            HashSet<List<string>> startsWith_t = computerGroups.Where(ContainsAnyStartingWithT).ToHashSet();
            int longestGroup = startsWith_t.Select(x => x.Count).Max();
            i = 0;
            TextUtilities.CFWLine($"@Gra------|  @DYeAll sets with longest length (@Cya{longestGroup}@DYe):");
            foreach (var item in startsWith_t.Where(x => x.Count == longestGroup))
            {
                TextUtilities.CFWLine($"@Red  {i,-3:D2}@Gra |  @Yel{string.Join("@Gra,@Yel", item)}@Gra");
            }

            // Help function for selecting only the lists containing one 't'
            bool ContainsAnyStartingWithT(List<string> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].StartsWith('t')) return true;
                }
                return false;
            }
        }


    }
}
