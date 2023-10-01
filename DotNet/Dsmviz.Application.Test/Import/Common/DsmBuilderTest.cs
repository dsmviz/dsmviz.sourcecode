﻿using Dsmviz.Datamodel.Common.Interface;
using Dsmviz.Application.Import.Common;
using Dsmviz.Application.Sorting;
using Dsmviz.Application.Test.Stubs;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dsmviz.Application.Test.Import.Common
{
    [TestClass]
    public class DsmBuilderTest
    {
        Mock<IDsmModel> _dsmModel;

        Mock<IMetaDataItem> _createMetaDataItem;
        private const string MetaDataGroup = "group";
        private const string MetaDataItemName = "itemname";
        private const string MetaDataItemValue = "itemvalue";

        Mock<IDsmElement> _existingElement;
        Mock<IDsmElement> _createdElement;
        Mock<IDsmElement> _elementParent;
        private Mock<IDsmElement> _consumer;
        private Mock<IDsmElement> _provider;

        private const string ElementFullName = "a.b.c";
        private const string ElementName = "c";
        private const string ElementType = "etype";
        private const int ElementParentId = 1;

        Mock<IDsmRelation> _createdRelation;
        private const int ConsumerId = 2;
        private const int ProviderId = 3;
        private const string RelationType = "rtype";
        private const int RelationWeight = 4;

        [TestInitialize]
        public void TestInitialize()
        {
            _dsmModel = new Mock<IDsmModel>();

            _createMetaDataItem = new Mock<IMetaDataItem>();
 
            _existingElement = new Mock<IDsmElement>();
            _createdElement = new Mock<IDsmElement>();
            _elementParent = new Mock<IDsmElement>();
            _consumer = new Mock<IDsmElement>();
            _provider = new Mock<IDsmElement>();

            _dsmModel.Setup(x => x.GetElementById(ConsumerId)).Returns(_consumer.Object);
            _dsmModel.Setup(x => x.GetElementById(ProviderId)).Returns(_provider.Object);
            _consumer.Setup(x => x.Id).Returns(ConsumerId);
            _provider.Setup(x => x.Id).Returns(ProviderId);

            _createdRelation = new Mock<IDsmRelation>();

            _elementParent.Setup(x => x.Id).Returns(ElementParentId);

            SortAlgorithmFactory.RegisterAlgorithm(PartitionSortAlgorithm.AlgorithmName, typeof(StubbedSortAlgorithm));
        }

        [TestMethod]
        public void WhenMetaDataItemIsImportedThenMetaDataItemIsAddedToModel()
        {
            _dsmModel.Setup(x => x.AddMetaData(MetaDataGroup, MetaDataItemName, MetaDataItemValue)).Returns(_createMetaDataItem.Object);

            DsmBuilder policy = new DsmBuilder(_dsmModel.Object);
            IMetaDataItem metaDataItem = policy.ImportMetaDataItem(MetaDataGroup, MetaDataItemName, MetaDataItemValue);
            Assert.AreEqual(_createMetaDataItem.Object, metaDataItem);

            _dsmModel.Verify(x => x.AddMetaData(MetaDataGroup, MetaDataItemName, MetaDataItemValue), Times.Once());
        }

        [TestMethod]
        public void GivenElementIsNotInModelWhenElementIsImportedThenElementIsAddedToModel()
        {
            IDsmElement foundElement = null;
            _dsmModel.Setup(x => x.GetElementByFullname(ElementFullName)).Returns(foundElement);
            _dsmModel.Setup(x => x.AddElement(ElementName, ElementType, ElementParentId, 0, null)).Returns(_createdElement.Object);

            DsmBuilder policy = new DsmBuilder(_dsmModel.Object);
            IDsmElement element = policy.ImportElement(ElementFullName, ElementName, ElementType, _elementParent.Object, null);
            Assert.AreEqual(_createdElement.Object, element);

            _dsmModel.Verify(x => x.AddElement(ElementName, ElementType, ElementParentId, 0,null), Times.Once());
        }

        [TestMethod]
        public void GivenElementIsInModelWhenElementIsImportedThenElementIsNotAddedAgainToModel()
        {
            IDsmElement foundElement = _existingElement.Object;
            _dsmModel.Setup(x => x.GetElementByFullname(ElementFullName)).Returns(foundElement);

            DsmBuilder policy = new DsmBuilder(_dsmModel.Object);
            IDsmElement element = policy.ImportElement(ElementFullName, ElementName, ElementType, _elementParent.Object, null);
            Assert.AreEqual(_existingElement.Object, element);

            _dsmModel.Verify(x => x.AddElement(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int>(), null), Times.Never());
        }

        [TestMethod]
        public void WhenRelationIsImportedThenRelationIsAddedToModel()
        {
            _dsmModel.Setup(x => x.AddRelation(_consumer.Object, _provider.Object, RelationType, RelationWeight, null)).Returns(_createdRelation.Object);

            DsmBuilder policy = new DsmBuilder(_dsmModel.Object);
            IDsmRelation relation = policy.ImportRelation(ConsumerId, ProviderId, RelationType, RelationWeight, null);
            Assert.AreEqual(_createdRelation.Object, relation);

            _dsmModel.Verify(x => x.AddRelation(_consumer.Object, _provider.Object, RelationType, RelationWeight, null), Times.Once());
        }

        [TestMethod]
        public void WhenImportIsFinalizedThenElementOrderIsAssigned()
        {
            DsmBuilder policy = new DsmBuilder(_dsmModel.Object);

            policy.FinalizeImport(null);

            _dsmModel.Verify(x => x.AssignElementOrder(), Times.Once());
        }
    }
}
