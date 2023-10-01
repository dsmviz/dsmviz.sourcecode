﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Application.Actions.Element;
using System.Collections.Generic;
using Dsmviz.Application.Sorting;
using Dsmviz.Application.Test.Stubs;

namespace Dsmviz.Application.Test.Actions.Element
{
    [TestClass]
    public class ElementSortActionTest
    {
        private Mock<IDsmModel> _model;
        private Mock<IDsmElement> _element;

        private Dictionary<string, string> _data;

        private const int ElementId = 1;
        private const string UsedAlgorithm = "MockedAlgorithm";
        private const string SortResult = "2,0,1";
        private const string InverseSortResult = "1,2,0";

        [TestInitialize()]
        public void Setup()
        {
            _model = new Mock<IDsmModel>();
            _element = new Mock<IDsmElement>();

            SortAlgorithmFactory.RegisterAlgorithm(UsedAlgorithm, typeof(StubbedSortAlgorithm));

            _element.Setup(x => x.Id).Returns(ElementId);
            _model.Setup(x => x.GetElementById(ElementId)).Returns(_element.Object);

            _data = new Dictionary<string, string>
            {
                ["element"] = ElementId.ToString(),
                ["algorithm"] = UsedAlgorithm,
                ["order"] = SortResult
            };
        }

        [TestMethod]
        public void WhenDoActionThenElementsChildrenAreSorted()
        {
            ElementSortAction action = new ElementSortAction(_model.Object, _element.Object, UsedAlgorithm);
            action.Do();
            Assert.IsTrue(action.IsValid());

            _model.Verify(x => x.ReorderChildren(_element.Object, It.Is<SortResult>(i => i.Data == SortResult)), Times.Once());
        }

        [TestMethod]
        public void WhenUndoActionThenElementsChildrenAreSortIsReverted()
        {
            object[] args = { _model.Object, null, _data };
            ElementSortAction action = new ElementSortAction(args);
            action.Undo();
            Assert.IsTrue(action.IsValid());

            _model.Verify(x => x.ReorderChildren(_element.Object, It.Is<SortResult>(i => i.Data == InverseSortResult)), Times.Once());
        }

        [TestMethod]
        public void GivenLoadedActionWhenGettingDataThenActionAttributesMatch()
        {
            object[] args = { _model.Object, null, _data };
            ElementSortAction action = new ElementSortAction(args);

            Assert.AreEqual(3, action.Data.Count);
            Assert.AreEqual(ElementId.ToString(), _data["element"]);
            Assert.AreEqual(UsedAlgorithm, _data["algorithm"]);
            Assert.AreEqual(SortResult, _data["order"]);
        }
    }
}
