﻿using System.Linq;
using Dsmviz.Datamodel.Dsi.Core;
using Dsmviz.Datamodel.Dsi.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Dsmviz.Datamodel.Dsi.Test.Core
{
    [TestClass]
    public class DsiRelationModelTest
    {
        DsiElementModel _elementsDataModel;
        IDsiElement _a;
        IDsiElement _b;
        IDsiElement _c;

        [TestInitialize]
        public void TestInitialize()
        {
            _elementsDataModel = new DsiElementModel();
            _a = _elementsDataModel.AddElement("a", "", null);
            _b = _elementsDataModel.AddElement("b", "", null);
            _c = _elementsDataModel.AddElement("c", "", null);
        }

        [TestMethod]
        public void WhenModelIsConstructedThenItIsEmpty()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);
        }

        [TestMethod]
        public void GivenModelIsNotEmptyWhenClearIsCalledThenItIsEmpty()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            model.AddRelation(_a.Name, _b.Name, "type", 2, null);
            Assert.AreEqual(1, model.ImportedRelationCount);

            model.Clear();

            Assert.AreEqual(0, model.ImportedRelationCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddRelationIsCalledThenItsHasOneRelation()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation = model.AddRelation(_a.Name, _b.Name, "type", 2, null);
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, model.ImportedRelationCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddRelationIsCalledThenTheRelationExists()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation = model.AddRelation(_a.Name, _b.Name, "type", 2, null);
            Assert.IsNotNull(relation);

            Assert.IsTrue(model.DoesRelationExist(relation.ConsumerId, relation.ProviderId));
        }

        [TestMethod]
        public void GivenAnRelationIsInTheModelWhenAddRelationIsCalledAgainForThatRelationThenTheNewRelationIsAdded()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.AddRelation(_a.Name, _b.Name, "type", 2, null);
            Assert.IsNotNull(relation1);

            Assert.AreEqual(1, model.ImportedRelationCount);

            List<IDsiRelation> relationsBefore = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relationsBefore.Count);
            Assert.AreEqual(_a.Id, relationsBefore[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsBefore[0].ProviderId);
            Assert.AreEqual("type", relationsBefore[0].Type);
            Assert.AreEqual(2, relationsBefore[0].Weight);

            IDsiRelation relation2 = model.AddRelation(_a.Name, _b.Name, "type", 3, null);
            Assert.IsNotNull(relation2);

            Assert.AreEqual(2, model.ImportedRelationCount);

            List<IDsiRelation> relationsAfter = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(2, relationsAfter.Count);
            Assert.AreEqual(_a.Id, relationsAfter[0].ConsumerId);
            Assert.AreEqual(_b.Id, relationsAfter[0].ProviderId);
            Assert.AreEqual("type", relationsAfter[0].Type);
            Assert.AreEqual(2, relationsAfter[0].Weight);
            Assert.AreEqual(_a.Id, relationsAfter[1].ConsumerId);
            Assert.AreEqual(_b.Id, relationsAfter[1].ProviderId);
            Assert.AreEqual("type", relationsAfter[1].Type);
            Assert.AreEqual(3, relationsAfter[1].Weight);
        }

        [TestMethod]
        public void GivenAnRelationIsInTheModelWhenAddRelationIsCalledWithAnotherProviderThenItHasTwoRelations()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.AddRelation(_a.Name, _b.Name, "type", 2, null);
            Assert.IsNotNull(relation1);

            Assert.AreEqual(1, model.ImportedRelationCount);

            IDsiRelation relation2 = model.AddRelation(_a.Name, _c.Name, "type", 3, null);
            Assert.IsNotNull(relation2);

            Assert.AreEqual(2, model.ImportedRelationCount);
        }

        [TestMethod]
        public void GivenAnRelationIsInTheModelWhenAddRelationIsCalledWithAnotherTypeThenItHasTwoRelations()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.AddRelation(_a.Name, _b.Name, "type1", 2, null);
            Assert.IsNotNull(relation1);

            Assert.AreEqual(1, model.ImportedRelationCount);

            IDsiRelation relation2 = model.AddRelation(_a.Name, _b.Name, "type2", 3, null);
            Assert.IsNotNull(relation2);

            Assert.AreEqual(2, model.ImportedRelationCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenImportRelationIsCalledThenItsHasOneRelation()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation = model.ImportRelation(_a.Id, _b.Id, "type", 2, null);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.ImportedRelationCount);
        }

        [TestMethod]
        public void GivenAnRelationIsInTheModelWhenDoesRelationExistIsCalledThenTrueIsReturned()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation = model.ImportRelation(_a.Id, _b.Id, "type", 2, null);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.ImportedRelationCount);

            Assert.IsTrue(model.DoesRelationExist(_a.Id, _b.Id));
        }

        [TestMethod]
        public void GivenAnRelationIsNotInTheModelWhenDoesRelationExistIsCalledThenFalseIsReturned()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation = model.ImportRelation(_a.Id, _b.Id, "type", 2, null);
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, model.ImportedRelationCount);

            Assert.IsFalse(model.DoesRelationExist(_a.Id, _c.Id));
        }

        [TestMethod]
        public void WhenAddRelationIsCalledUsingTwoDifferentTypesThenTwoRelationTypesAreFound()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.ImportRelation(_a.Id, _b.Id, "type1", 2, null);
            Assert.IsNotNull(relation1);

            Assert.AreEqual(1, model.ImportedRelationCount);

            IDsiRelation relation2 = model.ImportRelation(_a.Id, _b.Id, "type2", 2, null);
            Assert.IsNotNull(relation2);

            Assert.AreEqual(2, model.ImportedRelationCount);

            List<string> relationTypes = model.GetRelationTypes().ToList();
            Assert.AreEqual(2, relationTypes.Count);
            Assert.AreEqual("type1", relationTypes[0]);
            Assert.AreEqual("type2", relationTypes[1]);

            Assert.AreEqual(1, model.GetRelationTypeCount("type1"));
            Assert.AreEqual(1, model.GetRelationTypeCount("type2"));
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenGetElementsIsCalledTheyAreAllReturned()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.ImportRelation(_a.Id, _b.Id, "type1", 1, null);
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.ImportRelation(_b.Id, _c.Id, "type2", 2, null);
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.ImportRelation(_a.Id, _c.Id, "type3", 3, null);
            Assert.IsNotNull(relation3);

            List<IDsiRelation> relations = model.GetRelations().OrderBy(x => x.Weight).ToList();
            Assert.AreEqual(3, relations.Count);

            Assert.AreEqual(_a.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b.Id, relations[0].ProviderId);
            Assert.AreEqual("type1", relations[0].Type);
            Assert.AreEqual(1, relations[0].Weight);

            Assert.AreEqual(_b.Id, relations[1].ConsumerId);
            Assert.AreEqual(_c.Id, relations[1].ProviderId);
            Assert.AreEqual("type2", relations[1].Type);
            Assert.AreEqual(2, relations[1].Weight);

            Assert.AreEqual(_a.Id, relations[2].ConsumerId);
            Assert.AreEqual(_c.Id, relations[2].ProviderId);
            Assert.AreEqual("type3", relations[2].Type);
            Assert.AreEqual(3, relations[2].Weight);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenGetRelationsOfConsumerIsCalledThenItsHasReturnsOneRelation()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation = model.AddRelation(_a.Name, _b.Name, "type", 2, null);
            Assert.IsNotNull(relation);

            Assert.AreEqual(1, model.ImportedRelationCount);

            List<IDsiRelation> relations = model.GetRelationsOfConsumer(_a.Id).ToList();
            Assert.AreEqual(1, relations.Count);
            Assert.AreEqual(_a.Id, relations[0].ConsumerId);
            Assert.AreEqual(_b.Id, relations[0].ProviderId);
            Assert.AreEqual("type", relations[0].Type);
            Assert.AreEqual(2, relations[0].Weight);
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenAddRelationIsCalled4Times1TimeWithNotExistingConsumerThenResolvedPercentageIs75Percent()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.AddRelation("a", "b", "type1", 1, null);
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.AddRelation("b", "c", "type2", 2, null);
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.AddRelation("a", "c", "type3", 3, null);
            Assert.IsNotNull(relation3);
            IDsiRelation relation4 = model.AddRelation("d", "c", "type4", 4, null);
            Assert.IsNull(relation4);

            Assert.AreEqual(4, model.ImportedRelationCount);
            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.AreEqual(75.0, model.ResolvedRelationPercentage);
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenAddRelationIsCalled4Times1TimeWithNotExistingProviderThenResolvedPercentageIs75Percent()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.AddRelation("a", "b", "type1", 1, null);
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.AddRelation("b", "c", "type2", 2, null);
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.AddRelation("a", "c", "type3", 3, null);
            Assert.IsNotNull(relation3);
            IDsiRelation relation4 = model.AddRelation("c", "d", "type4", 4, null);
            Assert.IsNull(relation4);

            Assert.AreEqual(4, model.ImportedRelationCount);
            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.AreEqual(75.0, model.ResolvedRelationPercentage);
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenAddRelationIsCalled3TimesAndSkipRelation1TimeThenResolvedPercentageIs75Percent()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.AddRelation("a", "b", "type1", 1, null);
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.AddRelation("b", "c", "type2", 2, null);
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.AddRelation("a", "c", "type3", 3, null);
            Assert.IsNotNull(relation3);
            model.SkipRelation("d", "c", "type4");

            Assert.AreEqual(4, model.ImportedRelationCount);
            Assert.AreEqual(3, model.ResolvedRelationCount);
            Assert.AreEqual(75.0, model.ResolvedRelationPercentage);
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenAnElementIsRemovedThenAllRelationUsingThisElementAreRemoved()
        {
            DsiRelationModel model = new DsiRelationModel(_elementsDataModel);
            Assert.AreEqual(0, model.ImportedRelationCount);

            IDsiRelation relation1 = model.AddRelation("a", "b", "type1", 1, null);
            Assert.IsNotNull(relation1);
            IDsiRelation relation2 = model.AddRelation("b", "c", "type2", 2, null);
            Assert.IsNotNull(relation2);
            IDsiRelation relation3 = model.AddRelation("a", "c", "type3", 3, null);
            Assert.IsNotNull(relation3);
            Assert.AreEqual(3, model.ImportedRelationCount);

            _elementsDataModel.RemoveElement(_b);
            Assert.AreEqual(3, model.ImportedRelationCount);

            List<IDsiRelation> relations = model.GetRelations().OrderBy(x => x.Weight).ToList();
            Assert.AreEqual(1, relations.Count);

            Assert.AreEqual(_a.Id, relations[0].ConsumerId);
            Assert.AreEqual(_c.Id, relations[0].ProviderId);
            Assert.AreEqual("type3", relations[0].Type);
            Assert.AreEqual(3, relations[0].Weight);
        }
    }
}