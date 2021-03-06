﻿using Csp.Events.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

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
        public async Task WhenChanged_Test1Async()
        {
            var c1 = new Counter();


            int counter = 0;

            Bindings.WhenChanged(() => c1.Count, value => counter = value);

            c1.Count++;

            Assert.AreEqual(c1.Count, counter);

            await Task.Run(() => c1.Count++);

            Assert.AreEqual(c1.Count, counter);
        }

        [TestMethod]
        public void WhenChanged_Test2()
        {
            var c1 = new Counter();

            int counter = 0;

            var d = Bindings.WhenChanged(() => c1.Count, value => counter = value);

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

            var d = Bindings.WhenChanged(() => c1.Count, () => c2.Count);

            c1.Count = 5;

            Assert.AreEqual(c1.Count, c2.Count);
            Assert.AreEqual(c2.Count, 5);

        }

        [TestMethod]
        public void WhenChanged_Test4()
        {
            var c1 = new Counter();
            var c2 = new Counter();

            var d = Bindings.WhenChanged(() => c1.Count, () => c2.Count);

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

            var d = Bindings.Bind(() => c1.Count, () => c2.Count);

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

            var d = Bindings.Bind(() => c1.Count, () => c2.Count);

            c1.Count = 5;

            Assert.AreEqual(c1.Count, c2.Count);

            c2.Count = 4;

            Assert.AreEqual(c1.Count, c2.Count);

            d.Dispose();

            c1.Count++; // 5

            Assert.AreEqual(c1.Count, c2.Count + 1);


        }


        [TestMethod]
        public void ObserverProperty_Test1()
        {
            var c1 = new Counter();

            int n = 0;

            var d = c1.WhenChanged(() => c1.Count)
                        .Do(v => v > 0,
                         c => n = c.Count);

            c1.Count++;

            Assert.IsTrue(n > 0);

            c1.Count++;

            Assert.IsTrue(n == 2);

            d.Dispose();

            c1.Count++;

            Assert.IsTrue(n == 2);

        }

        [TestMethod]
        public void ObserverProperty_Test2()
        {
            var c1 = new Counter();

            int ctr = 0;

            var ev = EventBindingHandler<PropertyChangedEventHandler>
                        .Create(c1, "PropertyChanged", (_, e) =>
                        {
                            ctr++;
                        });

            c1.Count++;

            ev.Dispose();

            Assert.AreEqual(1, ctr);
        }

    }
}
