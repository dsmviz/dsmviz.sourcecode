using Dsmviz.Datamodel.Common.Core;
using Dsmviz.Datamodel.Common.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dsmviz.Datamodel.Common.Test.Core
{
    [TestClass]
    public class DsiMetaDataItemTest
    {
        [TestMethod]
        public void WhenItemIsConstructedThenPropertiesAreSetAccordingArguments()
        {
            IMetaDataItem item = new MetaDataItem("name", "value");
            Assert.AreEqual("name", item.Name);
            Assert.AreEqual("value", item.Value);
        }
    }
}
