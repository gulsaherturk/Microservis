using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EventBus.UnitTest
{
    [TestClass]
    public class EventBusTests

    {

        private ServiceCollection services;

        public EventBusTests(ServiceCollection services)
        {
            this.services = services;
        }


        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
///////// burda kaldınnnnnn verison hatası varrrrrrrrr
//https://www.youtube.com/watch?v=wfv7Oi1Y-_E&list=PLRp4oRsit1bzd6v_1zwNjdBOnGNuvHjWy&index=6