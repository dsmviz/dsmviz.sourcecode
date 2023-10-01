﻿using System.Reflection;
using Dsmviz.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dsmviz.Application.Test
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Logger.Init(Assembly.GetExecutingAssembly(), true);
        }
    }
}
