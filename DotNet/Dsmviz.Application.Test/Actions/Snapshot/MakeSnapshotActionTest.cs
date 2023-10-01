using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dsmviz.Datamodel.Dsm.Interfaces;
using System.Collections.Generic;
using Moq;
using Dsmviz.Application.Actions.Snapshot;

namespace Dsmviz.Application.Test.Actions.Snapshot
{
    [TestClass]
    public class MakeSnapshotActionTest
    {
        private Mock<IDsmModel> _model;
        private Dictionary<string, string> _data;
        private const string Name = "name";

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();

            _data = new Dictionary<string, string>
            {
                ["name"] = Name
            };
        }

        [TestMethod]
        public void WhenCreatingNewActionThenActionAttributesMatch()
        {
            MakeSnapshotAction action = new MakeSnapshotAction(_model.Object, Name);

            Assert.AreEqual(1, action.Data.Count);
            Assert.AreEqual(Name, _data["name"]);
        }

        [TestMethod]
        public void WhenLoadingExistingActionThenActionAttributesMatch()
        {
            object[] args = { _model.Object, null, _data };
            MakeSnapshotAction action = new MakeSnapshotAction(args);

            Assert.AreEqual(1, action.Data.Count);
            Assert.AreEqual(Name, _data["name"]);
        }
    }
}
