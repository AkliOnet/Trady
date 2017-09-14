﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Helper;

namespace Trady.Analysis.Infrastructure
{
    public abstract class CumulativeAnalyzableBase<TInput, TMappedInput, TOutputToMap, TOutput> : AnalyzableBase<TInput, TMappedInput, TOutputToMap, TOutput>
    {
        protected CumulativeAnalyzableBase(IEnumerable<TInput> inputs, Func<TInput, TMappedInput> inputMapper) : base(inputs, inputMapper)
        {
        }

        protected sealed override TOutputToMap ComputeByIndexImpl(IReadOnlyList<TMappedInput> mappedInputs, int index)
        {
            var tick = default(TOutputToMap);
            if (index < InitialValueIndex)
                tick = ComputeNullValue(mappedInputs, index);
            else if (index == InitialValueIndex)
                tick = ComputeInitialValue(mappedInputs, index);
            else
            {
                int idx = Cache.Keys.DefaultIfEmpty(InitialValueIndex).Where(k => k >= InitialValueIndex).Max();
                for (int i = idx; i < index; i++)
                {
                    var prevTick = ComputeByIndex(mappedInputs, i);
                    tick = ComputeCumulativeValue(mappedInputs, i + 1, prevTick);
                    Cache.AddOrUpdate(i + 1, tick);
                }
            }
            return tick;
        }

        protected virtual int InitialValueIndex { get; } = 0;

        protected virtual TOutputToMap ComputeNullValue(IReadOnlyList<TMappedInput> mappedInputs, int index) => default(TOutputToMap);

        protected abstract TOutputToMap ComputeInitialValue(IReadOnlyList<TMappedInput> mappedInputs, int index);

        protected abstract TOutputToMap ComputeCumulativeValue(IReadOnlyList<TMappedInput> mappedInputs, int index, TOutputToMap prevOutputToMap);
    }

    public abstract class CumulativeAnalyzableBase<TInput, TOutput> : CumulativeAnalyzableBase<TInput, TInput, TOutput, TOutput>
    {
        public CumulativeAnalyzableBase(IEnumerable<TInput> inputs)
            : base(inputs, i => i)
        {
        }
    }
}