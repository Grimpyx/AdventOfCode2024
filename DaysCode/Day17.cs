using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day17 : IDayChallenge
    {
        private (long A, long B, long C) register;
        private int instructionPointer = 0;

        public void RunFirstStar(DayDataType fullOrSimpleData)
        {
            ((long A, long B, long C) reg, int[] opcodes) = ParseProgram(DataLoader.LoadRowData(17, fullOrSimpleData));
            register = reg;

            while(instructionPointer < opcodes.Length - 1)
            {
                int i = instructionPointer;

                int program = opcodes[i];
                int operand = opcodes[i + 1];

                RunInstruction(program, operand);
            }
        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {
            // After printing out each series from A=0 to A=200 I look at the patterns
            // and make up formulas for each digit. It so happens that the output
            // follows this pattern for each digit:
            // (F is the digit value, d is the digit index starting from 1)
            // F = mod( floor( A/(2^d) ), 8)
            // In csharp programming this can be written
            // F = (A/(1 << d) % 8
            // If A is an integer type it will automatically truncate
            // This worked for the simple dataset, but the pattern is different for the full dataset.

            // The digits follow a similar pattern 
            // First digit (starting at A=1): 2 1 0 5 3 5 5 3 2 1 0 1 3 7 3 3 1 0 5 3 7 on and on
            // For the second digit it is repeating but "delayed and elongated":
            // 2 (8 times), 1 (8 times), 0*8, 5*8, 3*8
            // Same numbers!
            // First digit starts at A=1, changing every one step.
            // Second digit at A=8, changing every 8 steps
            // Third digit at A=64, changing very 64 steps
            // Without finding the exact number series, assume S(A) gives the first digit.
            // S(A) gives first digit.
            // S( floor(A/8) ) gives second digit
            // S( floor(A/64) ) gives third digit

            // What does this mean?
            // Every 1 step changes the first digit
            // Every 8 (8^1) step changes the second digit
            // Every 64 (8^2) step changes the third digit
            // Every 512 (8^3) step changes the fourth digit
            // Every 4096 (8^4) step chagnes the fifth digit
            // on and on and on.
            // To shape our A, we will walk backward and start at its last digit:
            // We first change by 8^15 to get the last digit (^1)
            //  Then we change by 8^14 to get the last digit (^2)
            //  Then we change by 8^13 to get the last digit (^3)

            // Too high 110229415094682
            // Too low 105637558184346

            // Thanks to Gabba for this solution.
            // I was trying to do this but something went wrong on the way
            // https://www.reddit.com/r/adventofcode/comments/1hg38ah/comment/m2l7qx8/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
            // https://github.com/TrueNorthIT/AdventOfCode/blob/christian-waters/2024/day17/smart/Program.cs

            ((long A, long B, long C) reg, int[] opcodes) parsedInput = ParseProgram(DataLoader.LoadRowData(17, fullOrSimpleData));
            List<long> goal = parsedInput.opcodes.Select(x => (long)x).ToList();

            long currentA = 0;
            for (int goalIndex = goal.Count - 1; goalIndex >= 0; goalIndex -= 1)
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    // Add a value of 1 for each iteration of that digit position
                    long candidate = currentA + (1L << (goalIndex * 3)) * i;

                    // Get output for the candidate
                    ExecuteProgram(candidate);
                    var output = outputs.Select(x => (long)x); // convert to long such that we can use Sequence Equal

                    // If all other digits to the right are correct
                    // Skip ignores up until element [goalIndex]. We only care about goalIndex and the right hand side to be correct.
                    if (output.Skip(goalIndex).SequenceEqual(goal.Skip(goalIndex)))
                    {
                        currentA = candidate;
                        TextUtilities.CFWLine($"@GraSuccess finding digit @Gre{goalIndex} @Grato match the goal @Gre{goal[goalIndex]}@Gra, i={Convert.ToString(i, 8)} (octal)");
                        TextUtilities.CFWLine($"@GraOutput: @Gra[@Yel{string.Join("@DGy, @Yel", output)}@Gra]");
                        TextUtilities.CFWLine($"@GraCurrent is now @Yel{Convert.ToString(currentA, 8)}@Gra (octal)\n");
                        break;
                    }
                }
            }

            TextUtilities.CFWLine($"@Gra >>> @Gre{currentA}@Gra / @Red{Convert.ToString(currentA, 8)} @Gra(@Gredecimal @Gra/ @Redoctal@Gra)");



            return;

            byte[] goals = parsedInput.opcodes.Select(x => (byte)x).ToArray();
            int[] goalsMultiplier = new int[goals.Length];

            currentA = -1;

            List<byte> firstOutputs = new List<byte>();
            int[] collected = new int[goals.Length];
            for (int i = 0; i < collected.Length; i++)
            {
                collected[i] = 0;
            }

            for (int i = 0; i < 50; i++)
            {
                currentA += 1;
                ExecuteProgram(currentA);
                firstOutputs.Add(outputs[0]);

                TextUtilities.CFWLine("@GraA: @Gre" + currentA.ToString("000") + " @Gre-> @Gre" + outputs[0]);
                TextUtilities.CFWLine("@Yel" + string.Join("@DGy,@Yel", outputs) + "\n");
            }
            Console.WriteLine("\n\n");
            currentA = 0;
            for (int goalIndex = goals.Length - 1; goalIndex >= 0; goalIndex--)
            {
                for (int i = 0; i < 800; i++)
                {

                    byte goalByte = goals[goalIndex];
                    if (goalByte == firstOutputs[i])
                    {
                        collected[goalIndex] += i;
                        //currentA += collected[goalIndex] * (1L << (goalIndex * 3));
                        break;
                    }

                    if (i == firstOutputs.Count - 1)
                        throw new Exception("bombba");
                }
                //TextUtilities.CFWLine("@GraCollected: @Yel" + string.Join("@DGy,@Yel", collected));

                //ExecuteProgram(currentA);
                //TextUtilities.CFWLine("@GraOutput:    @Gre" + string.Join("@DGy,@Gre", outputs));
                // Borrow digits
                // 14/8 => 1, runs once with i = 0
                for (int i = 0; i < (collected[goalIndex] / 8); i++)
                {
                    TextUtilities.CFWLine("@RedCollected:   @Red" + string.Join("@DGy,@Red", collected));
                    Console.WriteLine(new string(' ', 2 * (goalIndex - 1) + 17) + 'V');
                    TextUtilities.CFWLine("@GraGoal:          @Gre" + string.Join("@DGy,@Gre", goals));
                    ExecuteProgram(GetA());
                    TextUtilities.CFWLine("@MgnOutput Before: " + string.Join("@DGy,@Mgn", outputs));
                    collected[goalIndex + 1] -= 2;
                    ExecuteProgram(GetA());
                    TextUtilities.CFWLine("@MgnOutput After:  " + string.Join("@DGy,@Mgn", outputs));
                    //currentA -= (1L << ((goalIndex + 1) * 3));
                    //TextUtilities.CFWLine("@RedCollected: @Red" + string.Join("@DGy,@Red", collected));
                }
                //ExecuteProgram(currentA);
                //TextUtilities.CFWLine("@GraOutput:    @Gre" + string.Join("@DGy,@Gre", outputs));
            }

            // Assemble A
            /*for (int c = collected.Length - 1; c >= 0; c--)
            {
                currentA += collected[c] * (1L << (c * 3));
                ExecuteProgram(currentA);
                Console.WriteLine(new string(' ', 2 * (c - 1) + 13) + 'V');
                TextUtilities.CFWLine("@GraOutput:    @Red" + string.Join("@DGy,@Red", outputs));
            }
            ExecuteProgram(currentA);*/

            currentA = GetA();

            Console.WriteLine();
            TextUtilities.CFWLine("@GraCollected: @Yel" + string.Join("@DGy,@Yel", collected));
            TextUtilities.CFWLine("@GraA:         @Yel" + currentA);
            TextUtilities.CFWLine("@GraGoal:      @Gre" + string.Join("@DGy,@Gre", goals));
            TextUtilities.CFWLine("@GraOutput:    @Gre" + string.Join("@DGy,@Gre", outputs));
            return;
            Console.WriteLine();

            long GetA()
            {
                long a = 0;
                for (int c = collected.Length - 1; c >= 0; c--)
                {
                    a += collected[c] * (1L << (c * 3));
                    //ExecuteProgram(a);
                    //Console.WriteLine(new string(' ', 2 * (c - 1) + 13) + 'V');
                    //TextUtilities.CFWLine("@GraOutput:    @Red" + string.Join("@DGy,@Red", outputs));
                }
                return a;
            }

            /*
            // print values
            for (int i = 0; i < 50; i++)
            {
                Console.Write(i + ": ");
                TryGetDigit(i, 2, out byte b1);
                Console.WriteLine();
            }*/

            /*TryGetDigit(341, 0, out byte b1);
            Console.WriteLine();
            TryGetDigit(341, 1, out byte b2);
            Console.WriteLine();
            TryGetDigit(341, 2, out byte b3);*/


            // Loop through the digits backward
            long stepSize1 = -1;
            byte d1 = 127, d2 = 88, digitsAfter = 255;

            // First iteration is special
            do
            {
                currentA += 1L << ((goals.Length - 1) * 3);
                ExecuteProgram(currentA);
                d1 = GetDigit(goals.Length - 1);
                TextUtilities.CFWLine("@GraA: @Gre" + currentA);
                TextUtilities.CFWLine("@Yel" + string.Join("@DGy,@Yel", outputs) + "  @DRe" + goals[goals.Length - 1]);
            } while (d1 != goals[goals.Length - 1]);


            for (int goalIndex = goals.Length - 2; goalIndex >= 0; goalIndex--)
            {
                TextUtilities.CFW("@DGy" + new string(' ', goalIndex * 2));
                TextUtilities.CFWLine("@RedV");

                // Loop forward to find the first number
                int timesAdded = 0;
                d1 = GetDigit(goalIndex);
                d2 = GetDigit(goalIndex + 1);
                while (d1 != goals[goalIndex])
                {
                    currentA += 1L << (goalIndex * 3);
                    timesAdded++;

                    ExecuteProgram(currentA);
                    d1 = GetDigit(goalIndex);
                    d2 = GetDigit(goalIndex + 1);

                    if (timesAdded > 7) // we have overflowed, meaning the correct answer is behind
                    {
                        // Reset to before
                        currentA -= 1L << ((goalIndex + 1) * 3);
                        timesAdded = 0;

                        /*// Start looping backward
                        while (d1 != goals[goalIndex] || d2 != goals[goalIndex + 1])
                        {
                            currentA -= 1L << (goalIndex * 3);
                            timesAdded++;

                            ExecuteProgram(currentA);
                            d1 = GetDigit(goalIndex);
                            d2 = GetDigit(goalIndex + 1);

                            TextUtilities.CFWLine("@DYe" + string.Join("@DGy,@DYe", outputs) + "  @DRe" + goals[goalIndex]);

                            if (timesAdded > 7) Console.WriteLine("ERROR");
                        }*/
                        ExecuteProgram(currentA);
                        TextUtilities.CFW("@DGy" + new string(' ', goalIndex * 2));
                        TextUtilities.CFWLine("@RedV");
                        continue;
                    }

                    //TextUtilities.CFWLine("@Yel" + string.Join("@DGy,@Yel", outputs) + "  @DRe" + goals[goalIndex]);

                    // If the digit after changes, we have overflowed
                    /*if (d2 != goals[goalIndex + 1])
                    {
                        currentA -= 1L << ((goalIndex + 1) * 3); // Go back ONCE in the digit before this one
                        ExecuteProgram(currentA);
                        TextUtilities.CFWLine("@DYe" + string.Join("@DGy,@DYe", outputs) + "  @DRe" + goals[goalIndex]);
                        break;
                    }
                    else*/
                    TextUtilities.CFWLine("@Yel" + string.Join("@DGy,@Yel", outputs) + "  @DRe" + goals[goalIndex]);
                }

                // If any digit is wrong, we have digit overflow.
                // How much should we go back?
                /*for (int i = goalIndex + 1; i < goals.Length - 1; i++)
                {
                    digitsAfter = GetDigit(i);
                    if (digitsAfter == goals[i])
                        break; // If it is correct
                    else
                    {
                        currentA -= 1L << (i * 3);
                        currentA += 1L << ((i-1) * 3);
                        ExecuteProgram(currentA);
                        TextUtilities.CFWLine("@DYe" + string.Join("@DGy,@DYe", outputs) + "  @DRe" + goals[goalIndex]);
                    }
                }*/
                /*digitsAfter = GetDigit(goalIndex + 1);
                while (digitsAfter != goals[goalIndex+1])
                {
                    currentA -= 1L << ((goalIndex + 1) * 3);
                    ExecuteProgram(currentA);
                    TextUtilities.CFWLine("@DYe" + string.Join("@DGy,@DYe", outputs) + "  @DRe" + goals[goalIndex]);
                    digitsAfter = GetDigit(goalIndex + 1);
                }*/

                TextUtilities.CFWLine("@Gre" + string.Join("@DGy,@Gre", outputs) + "  @Red" + goals[goalIndex] + "\n");
            }

            //TryGetDigit(currentA, goals.Length - 1, out _);
            Console.WriteLine();
            TextUtilities.CFWLine("@Gra >>> Goal:       @Yel" + string.Join("@DGy,@Yel", goals));
            TextUtilities.CFWLine("@Gra >>> Matched to: @Yel" + string.Join("@DGy,@Yel", outputs));
            TextUtilities.CFWLine("@Gra >>> Value of A: @Yel" + currentA);

            //register = reg;

            //List<List<long>> series = new List<List<long>>() { { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] }, { [] } };

            //bool foundProgram = false;
            //long startRegA = 0;

            //long v = (1L << 48);

            /*for (long counter = 0; counter < v; counter++)
            {
                outputs.Clear();
                firstOut = true;

                startRegA += 1;
                register.A = startRegA;
                instructionPointer = 0;

                // Perform program
                while (instructionPointer < opcodes.Length - 1)
                {
                    int i = instructionPointer;

                    int program = opcodes[i];
                    int operand = opcodes[i + 1];

                    RunInstruction(program, operand);
                }

                for (int p = 0; p < outputs.Count; p++)
                {
                    series[p].Add(outputs[p]);
                }

                //TextUtilities.CFWLine($"@Red{startRegA.ToString("0000000000")}@Gra: @Yel" + string.Join("@DGy,@Yel", outputs));
                if (startRegA <= (1 << 15)) TextUtilities.CFWLine(startRegA + ","+ outputs[0].ToString());
            }*/

            // Find loop
            /*bool foundLoop = false;
            int loopIndex = -1;
            List<long> comparison = new List<long>(series[0][..500000]);
            for (int i = 1; i < series[0].Count; i++)
            {

                for (int j = 0; j < comparison.Count && i+j < series.Count; j++)
                {
                    foundLoop = true;
                    // If not the same, the is no loop
                    
                    if (comparison[j] != series[0][i+j])
                    {
                        foundLoop = false;
                        break;
                    }
                }

                if (foundLoop)
                {
                    loopIndex = i;
                    break;
                }
                if (i % 1000 == 0) Console.WriteLine(i);
            }*/

            //TextUtilities.CFWLine("@Gra >>> Lowest value for register A is @Yel" + startRegA.ToString());

            byte GetDigit(int digit)
            {
                if (outputs.Count - 1 < digit)
                    return 255;
                return outputs[digit];
            }

            void ExecuteProgram(long A)
            {
                outputs.Clear();

                register.A = A;
                instructionPointer = 0;

                while (instructionPointer < parsedInput.opcodes.Length - 1)
                {
                    int i = instructionPointer;

                    int program = parsedInput.opcodes[i];
                    int operand = parsedInput.opcodes[i + 1];

                    RunInstruction(program, operand);
                }
            }
        }

        void RunInstruction(int program, int operand)
        {
            switch (program)
            {
                case 0:
                    Adv(operand);
                    break;
                case 1:
                    Bxl(operand);
                    break;
                case 2:
                    Bst(operand);
                    break;
                case 3:
                    Jnz(operand);
                    break;
                case 4:
                    Bxc(operand);
                    break;
                case 5:
                    Out(operand);
                    break;
                case 6:
                    Bdv(operand);
                    break;
                case 7:
                    Cdv(operand);
                    break;
            }
        }

        // Opcode 0
        void Adv(int operand)
        {
            long numerator = register.A;

            // 2^power
            int power = (int)GetComboOperand(operand);
            if (power > 63) throw new Exception("Bit shift resulted in overflow");
            long denominator = 1L << power;

            // Truncate and assign to register A
            register.A = numerator / denominator;

            instructionPointer += 2;
        }

        // Opcode 1
        void Bxl(int operand)
        {
            register.B = register.B ^ GetLiteralOperand(operand);

            instructionPointer += 2;
        }

        // Opcode 2
        void Bst(int operand)
        {
            register.B = GetComboOperand(operand) % 8;

            instructionPointer += 2;
        }

        // Opcode 3
        void Jnz(int operand)
        {
            if (register.A == 0)
            {
                instructionPointer += 2;
                return;
            }

            instructionPointer = (int)GetLiteralOperand(operand);
        }

        // Opcode 4
        void Bxc(int operand)
        {
            register.B ^= register.C;

            instructionPointer += 2;
        }

        // Opcode 5
        bool firstOut = true;
        List<byte> outputs = new List<byte>();
        void Out(int operand)
        {
            byte nr = (byte)(GetComboOperand(operand) % 8);
            outputs.Add(nr);

            /*if (firstOut)
                TextUtilities.CFW("@Yel" + nr.ToString());
            else
                TextUtilities.CFW("@DGy,@Yel" + nr.ToString());*/

            firstOut = false;

            instructionPointer += 2;
        }

        // Opcode 6
        void Bdv(int operand)
        {
            long numerator = register.A;

            // 2^power
            int power = (int)GetComboOperand(operand);
            if (power > 63) throw new Exception("Bit shift resulted in overflow");
            long denominator = 1L << power;

            // Truncate and assign to register A
            register.B = numerator / denominator;

            instructionPointer += 2;
        }

        // Opcode 7
        void Cdv(int operand)
        {
            long numerator = register.A;

            // 2^power
            int power = (int)GetComboOperand(operand);
            if (power > 63) throw new Exception("Bit shift resulted in overflow");
            long denominator = 1L << power;

            // Truncate and assign to register A
            register.C = numerator / denominator;

            instructionPointer += 2;
        }

        long GetLiteralOperand(int opcode) => opcode;

        long GetComboOperand(int opcode)
        {
            // Interpret opcode
            if (opcode >= 0 && opcode <= 3) return opcode;
            else if (opcode == 4) return register.A;
            else if (opcode == 5) return register.B;
            else if (opcode == 6) return register.C;

            // Unallowed opcodes
            throw new Exception("Opcode out of range.");
        }

        ((long A, long B, long C) reg, int[] opcodes) ParseProgram(string[] data)
        {
            int[] opcodes;
            (long A, long B, long C) reg;

            reg.A = long.Parse(data[0][12..]);
            reg.B = long.Parse(data[1][12..]);
            reg.C = long.Parse(data[2][12..]);

            opcodes = data[4][9..].Split(',').Select(c => int.Parse(c)).ToArray();

            return (reg, opcodes);
        }

        long combo(long[] registers, long value) => value switch
        {
            >= 0 and <= 3 => value,
            var reg => registers[reg - 4],
        };

        IEnumerable<long> run(long[] program, long a, long b, long c)
        {
            var registers = new long[] { a, b, c };
            long ip = 0;
            while (ip < program.Length)
            {
                var opCode = (OpCode)program[ip];
                var operand = program[ip + 1];
                switch (opCode)
                {
                    case OpCode.adv:
                        registers[0] = registers[0] / (1L << (int)combo(registers, operand));
                        break;
                    case OpCode.bxl:
                        registers[1] = registers[1] ^ operand;
                        break;
                    case OpCode.bst:
                        registers[1] = combo(registers, operand) % 8;
                        break;
                    case OpCode.jnz:
                        if (registers[0] != 0)
                        {
                            ip = operand;
                            ip -= 2;
                        }
                        break;
                    case OpCode.bxc:
                        registers[1] = registers[1] ^ registers[2];
                        break;
                    case OpCode.output:
                        yield return combo(registers, operand) % 8;
                        break;
                    case OpCode.bdv:
                        registers[1] = registers[0] / (1 << (int)combo(registers, operand));
                        break;
                    case OpCode.cdv:
                        registers[2] = registers[0] / (1 << (int)combo(registers, operand));
                        break;
                }
                ip += 2;
            }
        }

        enum OpCode
        {
            adv = 0,
            bxl = 1,
            bst = 2,
            jnz = 3,
            bxc = 4,
            output = 5,
            bdv = 6,
            cdv = 7,
        }
    }
}
