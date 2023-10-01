﻿using Dsmviz.Application.Sorting;

namespace Dsmviz.Application.Test.Stubs
{
    public class StubbedSortAlgorithm : ISortAlgorithm
    {
        private const string SortResult = "2,0,1";

        public StubbedSortAlgorithm(object[] args) { }

        public string Name => "Stub";

        public SortResult Sort()
        {
            return new SortResult(SortResult);
        }
    }
}
