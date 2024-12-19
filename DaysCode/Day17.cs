using AdventOfCode2024.Utilities;
using System.Diagnostics.Metrics;

namespace AdventOfCode2024.Days
{
    class Day17 : IDayChallenge
    {
        private (long A, long B, long C) register;

        Dictionary<,>

        public void RunFirstStar(DayDataType fullOrSimpleData)
        {

        }

        public void RunSecondStar(DayDataType fullOrSimpleData)
        {

        }

        void RunInstruction(int program, int operand)
        {

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
    }
}
