using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day9 : IDayChallenge
    {
        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            string data = DataLoader.LoadAllData(9, fullOrSimpleData);

            Stack<Element> numberStack = new Stack<Element>();
            Queue<Element> emptyQueue = new Queue<Element>();
            for (int i = 0; i < data.Length; i++)
            {
                int numberOfSpaces = (int)char.GetNumericValue(data[i]);

                if (i % 2 == 1) // Empty space
                {
                    emptyQueue.Enqueue(new Element(-1, numberOfSpaces));
                }
                else // Is number
                {
                    numberStack.Push(new Element((int)Math.Ceiling(i / 2f), numberOfSpaces));
                }
            }

            // print
            var numberStackArray = numberStack.ToArray();
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
            }


            List<Element> shiftedElements = new List<Element>();

            // Spaces to fill
            Element? emptyElement = emptyQueue.Dequeue(); // First

            // number to move
            Element? numberElement = numberStack.Pop(); // Last

            Console.WriteLine();
            int tracker = 0;
            while (true)
            {
                int emptyAmount = emptyElement.Spaces;
                int numberAmount = numberElement.Spaces;

                // If this happens, we don't have enough empty spaces.
                // That means for the next iteration, we can't pop a new number.
                // For the next iteration we will instead have to
                Element newElement;
                if (numberAmount > emptyAmount)
                {
                    // What will remain of the number element after the move
                    int diff = numberAmount - emptyAmount;

                    // Add to list of shifted elements
                    newElement = new Element(numberElement.Id, emptyAmount);

                    // Prepare for next iteration
                    // If unsuccessful dequeue we have reached the end
                    if (!emptyQueue.TryDequeue(out emptyElement)) break;    // Next in line
                    numberElement = new Element(numberElement.Id, diff);    // The rest of our numbers
                }
                else if (numberAmount < emptyAmount)
                {
                    var kkkkk = numberStackArray[^(tracker + 1)];
                    shiftedElements.Add(kkkkk);
                    tracker++;

                    // What will remain of the empty element after the move
                    int diff = emptyAmount - numberAmount;

                    // Add to list of shifted elements
                    newElement = new Element(numberElement.Id, numberAmount);

                    // Prepare for next iteration
                    // If unsuccessful pop we have reached the end
                    emptyElement = new Element(emptyElement.Id, diff);      // The rest of our empties   
                    if (!numberStack.TryPop(out numberElement)) break;      // Next in line
                }
                else // equal
                {
                    var kkkkk = numberStackArray[^(tracker + 1)];
                    shiftedElements.Add(kkkkk);
                    tracker++;

                    // Add to list of shifted elements
                    newElement = new Element(numberElement.Id, emptyAmount);

                    // Prepare for next iteration
                    // If any unsuccessful we have reached the end
                    if (!numberStack.TryPop(out numberElement))   break;
                    if (!emptyQueue.TryDequeue(out emptyElement)) break;
                    /*emptyElement = emptyQueue.Dequeue();    // First
                    numberElement = numberStack.Pop();        // Last*/
                }
                shiftedElements.Add(newElement);
            }


            /*for (int j = 0; j < shiftedElements.Count; j++)
            {
                var el = shiftedElements[j];
                for (int i = 0; i < el.Spaces; i++)
                {
                    if (el.Id == -1)
                        Console.Write(".");
                    else
                        Console.Write(el.Id.ToString());

                    Thread.Sleep(50);
                }
                Thread.Sleep(1000);
            }*/
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }

        record Element(int Id, int Spaces); // Id = -1 means empty space
    }
}
