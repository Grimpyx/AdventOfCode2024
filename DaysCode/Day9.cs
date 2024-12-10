using AdventOfCode2024.Utilities;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode2024.Days
{
    class Day9 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            // Right answer was 6395800119709!

            string data = DataLoader.LoadAllData(9, fullOrSimpleData);
            bool isSimple = fullOrSimpleData == DayDataType.Simple;

            // print
            /*var numberStackArray = numberStack.ToArray();
            var emptyQueueArray = emptyQueue.ToArray();
            if (fullOrSimpleData == DayDataType.Simple)
            {

                for (int i = 0; i < numberStackArray.Length; i++)
                {
                    var numElement = numberStackArray[^(i+1)];
                    TextUtilities.CFW($"@Gre{new string(numElement.Id.ToString()[0], numElement.Spaces)}");

                    if (i < emptyQueueArray.Length) // If not last element
                    {
                        var emptElement = emptyQueueArray[i];
                        TextUtilities.CFW($"@Gre{new string('.', emptElement.Spaces)}");
                    }
                }
            }*/

            // Parse input data
            LinkedList<Element> linkedList = Parse(data);

            // Write start
            TextUtilities.CFWLine("@YelStart:");
            WriteElementLinkedList(linkedList, "@Gra");
            Console.WriteLine(new string('-', 80) + '\n');
            TextUtilities.CFWLine("@YelStart:");

            // Perform defragmentation
            Compress(linkedList);

            // Write result
            Console.WriteLine("\nCompleted list:");
            WriteElementLinkedList(linkedList, "@Yel");
            Console.WriteLine();
            TextUtilities.CFWLine($"@Gra >>> Checksum: {GetChecksum(linkedList)}");

            void Compress(LinkedList<Element> list)
            {
                LinkedListNode<Element>? empty = list.First;
                if (empty!.Value.Id != -1) TryGetNextEmpty(ref empty);

                LinkedListNode<Element>? number = list.Last;

                while (true)
                {
                    if (empty == null || number == null) return;

                    if (empty.Value.Spaces > number.Value.Spaces)
                    {
                        if (isSimple) WriteElementLinkedList(linkedList, "@Red");
                        int diff = empty.Value.Spaces - number.Value.Spaces;

                        // Replace empty with number
                        empty.Value = number.Value;

                        // Add additional empty node to fill out the rest of the empty spaces
                        list.AddAfter(empty, new Element(-1, diff));
                        empty = empty.Next;

                        // Number is empty, so we remove it and grab the next one
                        list.RemoveLast();
                        if (!TryGetNextNumber(list, out number)) return;

                        if (isSimple) WriteElementLinkedList(linkedList, "@Red");
                    }
                    else if (empty.Value.Spaces < number.Value.Spaces)
                    {
                        if (isSimple) WriteElementLinkedList(linkedList, "@Gre");
                        int diff = number.Value.Spaces - empty.Value.Spaces;

                        // Replace empty with number
                        empty.Value = new Element(number.Value.Id, empty.Value.Spaces);

                        // We have rest numbers, so we decrease its number of spaces
                        number.Value = new Element(number.Value.Id, diff);

                        // The empty node has been filled is empty, so we grab the next one (dont remove it since it contains numbers now)
                        if (!TryGetNextEmpty(ref empty)) return;

                        if (isSimple) WriteElementLinkedList(linkedList, "@Gre");
                    }
                    else
                    {
                        if (isSimple) WriteElementLinkedList(linkedList, "@Blu");
                        empty.Value = number.Value;
                        list.RemoveLast();
                        if (!TryGetNextEmpty(ref empty)) return;
                        if (!TryGetNextNumber(list, out number)) return;

                        if (isSimple) WriteElementLinkedList(linkedList, "@Blu");
                    }

                    
                    if(isSimple)
                    {
                        WriteElement(empty!.Value, "@Gre");
                        WriteElement(number!.Value, "@Red");
                        TextUtilities.CFWLine("@Gra\n" + new string('-', 80));
                        Console.WriteLine("");
                    }
                }

                
            }

            bool TryGetNextEmpty(ref LinkedListNode<Element>? fromNode)//, out LinkedListNode<Element>? nextNode)
            {
                if (fromNode == null) return false;

                do
                {
                    fromNode = fromNode.Next;
                    if (fromNode == null) return false;
                } while (fromNode.Value.Id != -1 || fromNode.Value.Spaces <= 0);
                return true;
            }

            bool TryGetNextNumber(LinkedList<Element> l, out LinkedListNode<Element>? nextNode)//(ref LinkedListNode<Element>? fromNode)//, out LinkedListNode<Element>? nextNode)
            {
                nextNode = l.Last;
                if (nextNode == null) return false;

                while (nextNode.Value.Id == -1 || nextNode.Value.Spaces <= 0)
                {
                    nextNode = nextNode.Previous;
                    l.RemoveLast();
                    if (nextNode == null) return false;
                }
                return true;
            }


        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // 6418515859774 too low 
            // My initial defrag process didn't work. When I looked at another solution, I
            // was convinced I was doing the exact same thing. So I remade the function and it worked.
            // I made the code much more clean. So it's possible I just made a small error somewhere.

            // 6418529470362 correct!

            // Parse input data
            string data = DataLoader.LoadAllData(9, fullOrSimpleData);
            bool isSimple = fullOrSimpleData == DayDataType.Simple;
            LinkedList<Element> linkedList = Parse(data);

            // Write start
            if (isSimple)
            {
                TextUtilities.CFWLine("@YelStart:");
                WriteElementLinkedList(linkedList, "@Gra");
                Console.WriteLine(new string('-', 80) + '\n');
            }

            // Perform defragmentation
            PerformDefrag(linkedList);

            // print result
            if (isSimple)
            {
                // Write result
                Console.WriteLine("\nCompleted list:");
                WriteElementLinkedList(linkedList, "@Yel");
                Console.WriteLine();
            }
            TextUtilities.CFWLine($"@Gra >>> Checksum: @Yel{GetChecksum(linkedList)}");

            void PerformDefrag(LinkedList<Element> linkedList)
            {
                // Populate the queue of all nodes
                Queue<LinkedListNode<Element>> filesToMove = new Queue<LinkedListNode<Element>>();
                LinkedListNode<Element>? file = linkedList.Last;
                while (file != null)
                {
                    if (file.Value.Id != -1)
                        filesToMove.Enqueue(file);
                    file = file.Previous;
                }

                // Start looping through
                while (filesToMove.Count > 0)
                {
                    file = filesToMove.Dequeue();
                    LinkedListNode<Element>? empty = linkedList.First;

                    // Try to find valid spot to fill
                    while (empty != null)
                    {
                        // This is needed such that we stop looking for empty
                        // when we reach the file. This is because we don't want to move the file to its right!
                        if (empty == file)
                        {
                            empty = null;
                            break;
                        }

                        // If we find a valid empty with enough space we break
                        if (empty.Value.Id == -1 && empty.Value.Spaces >= file.Value.Spaces)
                            break;

                        // choose next empty for the next iteration
                        empty = empty.Next;
                    }
                    if (empty == null) continue; // found none. Skip to next number

                    // Now, we should have a number and a space to put it.
                    // We now swap the elements to switch place
                    int numberID = file.Value.Id;
                    int emptySpaces = empty.Value.Spaces;
                    int numberSpaces = file.Value.Spaces;


                    file.Value = new Element(-1, numberSpaces);
                    empty.Value = new Element(numberID, numberSpaces);

                    // If there are extra spaces left we have to add them
                    int diff = emptySpaces - numberSpaces;
                    if (diff > 0)
                    {
                        linkedList.AddAfter(empty, new Element(-1, diff));
                    }

                    if (isSimple) WriteElementLinkedList(linkedList, "@Gra");
                }
            }
        }

        long GetChecksum(LinkedList<Element> checksumList)
        {
            long result = 0;
            int position = 0;

            foreach (var node in checksumList)
            {
                for (int i = 0; i < node.Spaces; i++)
                {
                    if (node.Id != -1) result += position * node.Id;
                    position++;
                }
            }
            return result;
        }

        void WriteElement(Element e, string color)
        {
            for (int i = 0; i < e.Spaces; i++)
            {
                if (e.Id == -1)
                    TextUtilities.CFW(color + ". ");
                else
                    TextUtilities.CFW(color + e.Id.ToString() + " ");
            }
        }

        void WriteElementLinkedList(LinkedList<Element> eList, string color)
        {
            foreach (var item in eList)
            {
                WriteElement(item, color);
            }
            Console.WriteLine();
        }

        LinkedList<Element> Parse(string s)
        {
            LinkedList<Element> linkedList = new LinkedList<Element>();
            for (int i = 0; i < s.Length; i++)
            {
                int numberOfSpaces = (int)char.GetNumericValue(s[i]);

                if (i % 2 == 1) // Empty space
                {
                    linkedList.AddLast(new Element(-1, numberOfSpaces));
                }
                else // Is number
                {
                    linkedList.AddLast(new Element((int)Math.Ceiling(i / 2f), numberOfSpaces));
                }
            }
            return linkedList;
        }

        record Element(int Id, int Spaces); // Id = -1 means empty space
    }
}
