﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utilities
{
    public interface IDayChallenge
    {
        void RunFirstStar(DayDataType fullOrSimpleData);
        void RunSecondStar(DayDataType fullOrSimpleData);
    }
}
