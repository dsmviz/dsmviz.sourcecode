﻿using System.Linq;
using Dsmviz.Datamodel.Dsm.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dsmviz.Datamodel.Dsm.Test.Core
{
    [TestClass]
    public class DsmDependenciesTest
    {
        DsmElement _root;
        DsmElement _a;
        DsmElement _a1;
        DsmElement _a2;
        DsmElement _b;
        DsmElement _b1;
        DsmElement _b2;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _root = new DsmElement(1, "root", "", null);

            _a = new DsmElement(2, "a", "", null);
            _root.InsertChildAtEnd(_a);
            _a1 = new DsmElement(3, "a1", "", null);
            _a.InsertChildAtEnd(_a1);
            _a2 = new DsmElement(4, "a2", "", null);
            _a.InsertChildAtEnd(_a2);

            _b = new DsmElement(5, "a", "", null);
            _root.InsertChildAtEnd(_b);
            _b1 = new DsmElement(6, "b1", "", null);
            _b.InsertChildAtEnd(_b1);
            _b2 = new DsmElement(7, "b2", "", null);
            _b.InsertChildAtEnd(_b2);
        }

        [TestMethod]
        public void WhenIdenticalIngoingRelationTypesAreAddedThenGetOutgoingRelationsReturnsThenAll()
        {
            DsmRelation a1Tob1Type1 = new DsmRelation(1, _a1, _b1, "type1", 2, null);
            DsmRelation a1Tob1Type2 = new DsmRelation(2, _a1, _b1, "type1", 3, null);

            DsmDependencies b1Dependencies = new DsmDependencies(_b1);

            b1Dependencies.AddIngoingRelation(a1Tob1Type1);
            Assert.AreEqual(1, b1Dependencies.GetIngoingRelations().Count());

            b1Dependencies.AddIngoingRelation(a1Tob1Type2);
            Assert.AreEqual(2, b1Dependencies.GetIngoingRelations().Count());

            b1Dependencies.RemoveIngoingRelation(a1Tob1Type1);
            Assert.AreEqual(1, b1Dependencies.GetIngoingRelations().Count());

            b1Dependencies.RemoveIngoingRelation(a1Tob1Type2);
            Assert.AreEqual(0, b1Dependencies.GetIngoingRelations().Count());
        }

        [TestMethod]
        public void WhenDifferentIngoingRelationTypesAreAddedThenGetOutgoingRelationsReturnsThenAll()
        {
            DsmRelation a1Tob1Type1 = new DsmRelation(1, _a1, _b1, "type1", 2, null);
            DsmRelation a1Tob1Type2 = new DsmRelation(2, _a1, _b1, "type2", 3, null);

            DsmDependencies b1Dependencies = new DsmDependencies(_b1);

            b1Dependencies.AddIngoingRelation(a1Tob1Type1);
            Assert.AreEqual(1, b1Dependencies.GetIngoingRelations().Count());

            b1Dependencies.AddIngoingRelation(a1Tob1Type2);
            Assert.AreEqual(2, b1Dependencies.GetIngoingRelations().Count());

            b1Dependencies.RemoveIngoingRelation(a1Tob1Type1);
            Assert.AreEqual(1, b1Dependencies.GetIngoingRelations().Count());

            b1Dependencies.RemoveIngoingRelation(a1Tob1Type2);
            Assert.AreEqual(0, b1Dependencies.GetIngoingRelations().Count());
        }

        [TestMethod]
        public void WhenIdenticalOutgoingRelationTypesAreAddedThenGetOutgoingRelationsReturnsThenAll()
        {
            DsmRelation a1Tob1Type1 = new DsmRelation(1, _a1, _b1, "type1", 2, null);
            DsmRelation a1Tob1Type2 = new DsmRelation(2, _a1, _b1, "type1", 3, null);

            DsmDependencies a1Dependencies = new DsmDependencies(_a1);

            a1Dependencies.AddOutgoingRelation(a1Tob1Type1);
            Assert.AreEqual(1, a1Dependencies.GetOutgoingRelations().Count());

            a1Dependencies.AddOutgoingRelation(a1Tob1Type2);
            Assert.AreEqual(2, a1Dependencies.GetOutgoingRelations().Count());

            a1Dependencies.RemoveOutgoingRelation(a1Tob1Type1);
            Assert.AreEqual(1, a1Dependencies.GetOutgoingRelations().Count());

            a1Dependencies.RemoveOutgoingRelation(a1Tob1Type2);
            Assert.AreEqual(0, a1Dependencies.GetOutgoingRelations().Count());
        }

        [TestMethod]
        public void WhenDifferentOutgoingRelationTypesAreAddedThenGetOutgoingRelationsReturnsThenAll()
        {
            DsmRelation a1Tob1Type1 = new DsmRelation(1, _a1, _b1, "type1", 2, null);
            DsmRelation a1Tob1Type2 = new DsmRelation(2, _a1, _b1, "type2", 3, null);

            DsmDependencies a1Dependencies = new DsmDependencies(_a1);

            a1Dependencies.AddOutgoingRelation(a1Tob1Type1);
            Assert.AreEqual(1, a1Dependencies.GetOutgoingRelations().Count());

            a1Dependencies.AddOutgoingRelation(a1Tob1Type2);
            Assert.AreEqual(2, a1Dependencies.GetOutgoingRelations().Count());

            a1Dependencies.RemoveOutgoingRelation(a1Tob1Type1);
            Assert.AreEqual(1, a1Dependencies.GetOutgoingRelations().Count());

            a1Dependencies.RemoveOutgoingRelation(a1Tob1Type2);
            Assert.AreEqual(0, a1Dependencies.GetOutgoingRelations().Count());
        }


        [TestMethod]
        public void WhenIdenticalOutgoingRelationTypesAreAddedThenTheirDirectWeightIsTheSumOfWeights()
        {
            DsmRelation a1Tob1Type1 = new DsmRelation(1, _a1, _b1, "type1", 2, null);
            DsmRelation a1Tob1Type2 = new DsmRelation(2, _a1, _b1, "type1", 3, null);

            DsmDependencies a1Dependencies = new DsmDependencies(_a1);

            a1Dependencies.AddOutgoingRelation(a1Tob1Type1);
            Assert.AreEqual(2, a1Dependencies.GetDirectDependencyWeight(_b1));

            a1Dependencies.AddOutgoingRelation(a1Tob1Type2);
            Assert.AreEqual(5, a1Dependencies.GetDirectDependencyWeight(_b1));

            a1Dependencies.RemoveOutgoingRelation(a1Tob1Type1);
            Assert.AreEqual(3, a1Dependencies.GetDirectDependencyWeight(_b1));

            a1Dependencies.RemoveOutgoingRelation(a1Tob1Type2);
            Assert.AreEqual(0, a1Dependencies.GetDirectDependencyWeight(_b1));
        }

        [TestMethod]
        public void WhenDifferentOutgoingRelationTypesAreAddedThenTheirDirectWeightIsTheSumOfWeights()
        {
            DsmRelation a1Tob1Type1 = new DsmRelation(1, _a1, _b1, "type1", 2, null);
            DsmRelation a1Tob1Type2 = new DsmRelation(2, _a1, _b1, "type2", 3, null);

            DsmDependencies a1Dependencies = new DsmDependencies(_a1);

            a1Dependencies.AddOutgoingRelation(a1Tob1Type1);
            Assert.AreEqual(2, a1Dependencies.GetDirectDependencyWeight(_b1));

            a1Dependencies.AddOutgoingRelation(a1Tob1Type2);
            Assert.AreEqual(5, a1Dependencies.GetDirectDependencyWeight(_b1));

            a1Dependencies.RemoveOutgoingRelation(a1Tob1Type1);
            Assert.AreEqual(3, a1Dependencies.GetDirectDependencyWeight(_b1));

            a1Dependencies.RemoveOutgoingRelation(a1Tob1Type2);
            Assert.AreEqual(0, a1Dependencies.GetDirectDependencyWeight(_b1));
        }

        [TestMethod]
        public void WhenAddDerivedDependencyWeightThenWeightAreSummedPerProvider()
        {
            DsmDependencies a1Dependencies = new DsmDependencies(_a1);

            Assert.AreEqual(0, a1Dependencies.GetDerivedDependencyWeight(_b1));
            a1Dependencies.AddDerivedWeight(_b1, 2);
            Assert.AreEqual(2, a1Dependencies.GetDerivedDependencyWeight(_b1));
            a1Dependencies.AddDerivedWeight(_b1, 3);
            Assert.AreEqual(5, a1Dependencies.GetDerivedDependencyWeight(_b1));

            a1Dependencies.RemoveDerivedWeight(_b1, 2);
            Assert.AreEqual(3, a1Dependencies.GetDerivedDependencyWeight(_b1));
            a1Dependencies.RemoveDerivedWeight(_b1, 3);
            Assert.AreEqual(0, a1Dependencies.GetDerivedDependencyWeight(_b1));
        }
    }
}
