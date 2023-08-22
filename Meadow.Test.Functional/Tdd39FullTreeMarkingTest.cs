using System;
using Meadow.Attributes;
using Meadow.Test.Functional.TDDAbstractions;
using Meadow.Utility;

namespace Meadow.Test.Functional
{
    public class Tdd39FullTreeMarkingTest : IFunctionalTest
    {
        private class Caller
        {
            [FullTreeRead]
            public virtual void MarkedCall(Action code)
            {
                code();
            }

            public virtual void NoneMarkedCall(Action code)
            {
                code();
            }
        }

        private class CallersChild : Caller
        {
            [FullTreeRead]
            public override void NoneMarkedCall(Action code)
            {
                base.NoneMarkedCall(code);
            }
        }


        [FullTreeRead()]
        private class TotallyMarkedCaller
        {
            public void RegularCall1(Action code)
            {
                code();
            }

            public void RegularCall2(Action code)
            {
                code();
            }
        }

        [FullTreeRead(nameof(SelectivelyMarkedCaller.Method2))]
        private class SelectivelyMarkedCaller
        {
            public void Method1(Action code)
            {
                code();
            }

            public void Method2(Action code)
            {
                code();
            }

            public void Method3(Action code)
            {
                code();
            }
        }

        public void Main()
        {
            new Caller().MarkedCall(() => Console.WriteLine(
                "Caller.MarkedCall is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));

            new Caller().NoneMarkedCall(() => Console.WriteLine(
                "Caller.NoneMarkedCall is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));

            new CallersChild().MarkedCall(() => Console.WriteLine(
                "CallersChild.MarkedCall is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));

            new CallersChild().NoneMarkedCall(() => Console.WriteLine(
                "CallersChild.NoneMarkedCall is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));

            new TotallyMarkedCaller().RegularCall1(() => Console.WriteLine(
                "TotallyMarkedCaller.RegularCall1 is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));

            new TotallyMarkedCaller().RegularCall2(() => Console.WriteLine(
                "TotallyMarkedCaller.RegularCall2 is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));

            new SelectivelyMarkedCaller().Method1(() => Console.WriteLine(
                "SelectivelyMarkedCaller.Method1 is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));

            new SelectivelyMarkedCaller().Method2(() => Console.WriteLine(
                "SelectivelyMarkedCaller.Method2 is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));
            new SelectivelyMarkedCaller().Method3(() => Console.WriteLine(
                "SelectivelyMarkedCaller.Method3 is marked: "
                + FullTreeMark.IsMarkedFullTreeRead()));
        }
    }
}