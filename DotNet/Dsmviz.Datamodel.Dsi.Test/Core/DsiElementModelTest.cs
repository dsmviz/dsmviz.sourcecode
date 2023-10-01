﻿using System.Linq;
using Dsmviz.Datamodel.Dsi.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dsmviz.Datamodel.Dsi.Interface;
using System.Collections.Generic;

namespace Dsmviz.Datamodel.Dsi.Test.Core
{
    [TestClass]
    public class DsiElementModelTest
    {
        [TestMethod]
        public void WhenModelIsConstructedThenItIsEmpty()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);
        }

        [TestMethod]
        public void GivenModelIsNotEmptyWhenClearIsCalledThenItIsEmpty()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name", "type", null);
            Assert.AreEqual(1, model.CurrentElementCount);

            model.Clear();

            Assert.AreEqual(0, model.CurrentElementCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenAddElementIsCalledThenItsHasOneElement()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            IDsiElement element = model.AddElement("name", "type", null);
            Assert.IsNotNull(element);
            Assert.AreEqual(1, model.CurrentElementCount);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenAddElementIsCalledAgainForThatElementThenItStillHasOneElement()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            IDsiElement element1 = model.AddElement("name", "type", null);
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, model.CurrentElementCount);

            IDsiElement element2 = model.AddElement("name", "type", null);
            Assert.IsNull(element2);
            Assert.AreEqual(1, model.CurrentElementCount);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenAddElementIsCalledForAnotherElementThenItHasTwoElement()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            IDsiElement element1 = model.AddElement("name1", "type", null);
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, model.CurrentElementCount);

            IDsiElement element2 = model.AddElement("name2", "type", null);
            Assert.IsNotNull(element2);
            Assert.AreEqual(2, model.CurrentElementCount);
        }

        [TestMethod]
        public void GivenModelIsEmptyWhenImportElementIsCalledThenItsHasOneElement()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name", "type", null);
            Assert.AreEqual(1, model.CurrentElementCount);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenFindByIdIsCalledItsIdThenElementIsFound()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name", "type", null);

            IDsiElement foundElement = model.FindElementById(1);
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(1, foundElement.Id);
            Assert.AreEqual("name", foundElement.Name);
            Assert.AreEqual("type", foundElement.Type);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenFindByIdIsCalledWithAnotherIdThenElementIsNotFound()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name", "type", null);

            IDsiElement foundElement = model.FindElementById(2);
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenRemoveElementIsCalledThenElementIsNotFoundAnymoreByItsId()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name", "type", null);
            IDsiElement foundElementBefore = model.FindElementById(1);
            Assert.IsNotNull(foundElementBefore);

            model.RemoveElement(foundElementBefore);

            IDsiElement foundElementAfter = model.FindElementById(1);
            Assert.IsNull(foundElementAfter);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenFindByIdIsCalledWithItsNameThenElementIsFound()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name", "type", null);

            IDsiElement foundElement = model.FindElementByName("name");
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(1, foundElement.Id);
            Assert.AreEqual("name", foundElement.Name);
            Assert.AreEqual("type", foundElement.Type);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenFindByIdIsCalledWithAnotherNameThenElementIsNotFound()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name", "type", null);

            IDsiElement foundElement = model.FindElementByName("unknown");
            Assert.IsNull(foundElement);
        }

        [TestMethod]
        public void GivenAnElementIsInTheModelWhenRemoveElementIsCalledThenElementIsNotFoundAnymoreByItName()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name", "type", null);
            IDsiElement foundElementBefore = model.FindElementByName("name");
            Assert.IsNotNull(foundElementBefore);

            model.RemoveElement(foundElementBefore);

            IDsiElement foundElementAfter = model.FindElementByName("name");
            Assert.IsNull(foundElementAfter);
        }

        [TestMethod]
        public void WhenRenameElementIsCalledThenItCanBeFoundUnderThatName()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            IDsiElement element = model.AddElement("name", "type", null);
            Assert.IsNotNull(element);
            Assert.AreEqual(1, model.CurrentElementCount);

            model.RenameElement(element, "newname");
            Assert.AreEqual(1, model.CurrentElementCount);

            IDsiElement foundElement = model.FindElementByName("newname");
            Assert.IsNotNull(foundElement);
            Assert.AreEqual(1, foundElement.Id);
            Assert.AreEqual("newname", foundElement.Name);
            Assert.AreEqual("type", foundElement.Type);
        }

        [TestMethod]
        public void WhenAddElementIsCalledUsingTwoDifferentTypesThenTwoElementTypesAreFound()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            IDsiElement element1 = model.AddElement("name1", "type1", null);
            Assert.IsNotNull(element1);
            Assert.AreEqual(1, model.CurrentElementCount);

            IDsiElement element2 = model.AddElement("name2", "type2", null);
            Assert.IsNotNull(element2);
            Assert.AreEqual(2, model.CurrentElementCount);

            IDsiElement element3 = model.AddElement("name3", "type2", null);
            Assert.IsNotNull(element3);
            Assert.AreEqual(3, model.CurrentElementCount);

            List<string> elementTypes = model.GetElementTypes().ToList();
            Assert.AreEqual(2, elementTypes.Count);
            Assert.AreEqual("type1", elementTypes[0]);
            Assert.AreEqual("type2", elementTypes[1]);

            Assert.AreEqual(1, model.GetElementTypeCount("type1"));
            Assert.AreEqual(2, model.GetElementTypeCount("type2"));
        }

        [TestMethod]
        public void GivenMultipleElementAreInTheModelWhenGetElementsIsCalledTheyAreAllReturned()
        {
            DsiElementModel model = new DsiElementModel();
            Assert.AreEqual(0, model.CurrentElementCount);

            model.ImportElement(1, "name1", "type1", null);
            model.ImportElement(2, "name2", "type2", null);
            model.ImportElement(3, "name3", "type3", null);

            List<IDsiElement> elements = model.GetElements().ToList();
            Assert.AreEqual(3, elements.Count);

            Assert.AreEqual(1, elements[0].Id);
            Assert.AreEqual("name1", elements[0].Name);
            Assert.AreEqual("type1", elements[0].Type);

            Assert.AreEqual(2, elements[1].Id);
            Assert.AreEqual("name2", elements[1].Name);
            Assert.AreEqual("type2", elements[1].Type);

            Assert.AreEqual(3, elements[2].Id);
            Assert.AreEqual("name3", elements[2].Name);
            Assert.AreEqual("type3", elements[2].Type);
        }
    }
}
