﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Infrastructure;
using Trady.Core;

namespace Trady.Analysis.Indicator
{
    public class RelativeStrengthIndex<TInput, TOutput> : AnalyzableBase<TInput, decimal, decimal?, TOutput>
    {
        private readonly RelativeStrengthByTuple _rs;

        public RelativeStrengthIndex(IEnumerable<TInput> inputs, Func<TInput, decimal> inputMapper, int periodCount) : base(inputs, inputMapper)
        {
            _rs = new RelativeStrengthByTuple(inputs.Select(inputMapper), periodCount);

            PeriodCount = periodCount;
        }

        public int PeriodCount { get; }

        protected override decimal? ComputeByIndexImpl(IReadOnlyList<decimal> mappedInputs, int index) => 100 - (100 / (1 + _rs[index]));
    }

    public class RelativeStrengthIndexByTuple : RelativeStrengthIndex<decimal, decimal?>
    {
        public RelativeStrengthIndexByTuple(IEnumerable<decimal> inputs, int periodCount)
            : base(inputs, i => i, periodCount)
        {
        }
    }

    public class RelativeStrengthIndex : RelativeStrengthIndex<Candle, AnalyzableTick<decimal?>>
    {
        public RelativeStrengthIndex(IEnumerable<Candle> inputs, int periodCount)
            : base(inputs, i => i.Close, periodCount)
        {
        }
    }
}