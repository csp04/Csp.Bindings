﻿using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csp.Bindings.Test
{

    class Counter : INotifyPropertyChanged
    {
        public int Count { get; set; } = 0;

        public event PropertyChangedEventHandler PropertyChanged;
    }


    [TestClass]
    public class BindingsUnitTest
    {
        [TestMethod]
        public void WhenChanged_Test1()
        {
            var c1 = new Counter();

            int counter = 0;

            PropertyBindings.WhenChanged(() => c1.Count, value => counter = value);

            c1.Count++;

            Assert.AreEqual(c1.Count, counter);
        }

        [TestMethod]
        public void WhenChanged_Test2()
        {
            var c1 = new Counter();

            int counter = 0;

            var d = PropertyBindings.WhenChanged(() => c1.Count, value => counter = value);

            c1.Count++;

            Assert.AreEqual(c1.Count, counter);

            d.Dispose();

            c1.Count++;

            Assert.AreEqual(c1.Count, counter + 1);
        }

        [TestMethod]
        public void WhenChanged_Test3()
        {
            var c1 = new Counter();
            var c2 = new Counter();

            var d = PropertyBindings.WhenChanged(() => c1.Count, () => c2.Count);

            c1.Count = 5;

            Assert.AreEqual(c1.Count, c2.Count);
            Assert.AreEqual(c2.Count, 5);

        }

        [TestMethod]
        public void WhenChanged_Test4()
        {
            var c1 = new Counter();
            var c2 = new Counter();

            var d = PropertyBindings.WhenChanged(() => c1.Count, () => c2.Count);

            c1.Count = 5;

            Assert.AreEqual(c1.Count, c2.Count);

            d.Dispose();

            c1.Count++; // 6

            Assert.AreEqual(c2.Count, 5);

        }

        [TestMethod]
        public void Bind_Test1()
        {
            var c1 = new Counter();
            var c2 = new Counter();

            var d = PropertyBindings.Bind(() => c1.Count, () => c2.Count);

            c1.Count = 5;

            Assert.AreEqual(c1.Count, c2.Count);

            c2.Count = 4;

            Assert.AreEqual(c1.Count, c2.Count);
        }

        [TestMethod]
        public void Bind_Test2()
        {
            var c1 = new Counter();
            var c2 = new Counter();

            var d = PropertyBindings.Bind(() => c1.Count, () => c2.Count);

            c1.Count = 5;

            Assert.AreEqual(c1.Count, c2.Count);

            c2.Count = 4;

            Assert.AreEqual(c1.Count, c2.Count);

            d.Dispose();

            c1.Count++; // 5

            Assert.AreEqual(c1.Count, c2.Count + 1);
        }
    }
}