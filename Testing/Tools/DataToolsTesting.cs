using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IPConnect_Testing.Testing.DataObjects;

namespace Testing.Tools
{
    [TestClass]
    public class DataToolsTesting
    {
        [TestMethod]
        [TestCategory("Data")]
        public void GenericDataRowConvert()
        {
            LagTestDataList lagTestDataList = new LagTestDataList();
            lagTestDataList.Populate();

            var x = "m/d";

        }
    }
}
