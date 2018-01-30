﻿namespace SomePipes
{
    public class ToStringProcessor<TIn> : IPipeProcessor<TIn, string>
    {
        public string Process(TIn data)
        {
            return data.ToString();
        }
    }
}