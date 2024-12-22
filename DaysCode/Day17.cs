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
            ((long A, long B, long C) reg, int[] opcodes) = ParseProgram(DataLoader.LoadRowData(17, fullOrSimpleData));
            register = reg;

            bool foundProgram = false;
            long startRegA = -1;
            while(!foundProgram)
            {
                outputs.Clear();
                firstOut = true;

                startRegA++;
                register.A = startRegA;
                instructionPointer = 0;

                // Reset output
                Console.CursorLeft = 0;
                Console.Write("                          ");
                Console.CursorLeft = 0;

                // Perform program
                while (instructionPointer < opcodes.Length - 1)
                {
                    int i = instructionPointer;

                    int program = opcodes[i];
                    int operand = opcodes[i + 1];

                    RunInstruction(program, operand);

                    // if Out is run
                    if (program == 5)
                    {
                        // Compare the output with the opcode.
                        // If they're different we have to go to the next value for register A
                        int lastIndex = outputs.Count - 1;
                        if (outputs[lastIndex] != opcodes[lastIndex])
                            break;

                        // If this is true we have reached the end
                        // and have the same program output
                        if (outputs.Count == opcodes.Length)
                            foundProgram = true;
                    }
                }
            }
            TextUtilities.CFWLine("@Gra >>> Lowest value for register A is @Yel" + startRegA.ToString());
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
        List<long> outputs = new List<long>();
        void Out(int operand)
        {
            int nr = (int)GetComboOperand(operand) % 8;
            outputs.Add(nr);

            if (firstOut)
                TextUtilities.CFW("@Yel" + nr.ToString());
            else
                TextUtilities.CFW("@DGy,@Yel" + nr.ToString());

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
    }
}
