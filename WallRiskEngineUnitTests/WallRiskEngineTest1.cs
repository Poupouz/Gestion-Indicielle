using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WallRiskEngineUnitTests
{
    [TestClass]
    public class APITest
    {
        [TestMethod]
        public void SimpleNetReturnsTest()
        {
            double[,] returns = { {0.05, 0.1, 0.6}, {0.001, 0.4, 0.56}, {0.7, 0.001, 0.12}, {0.3, 0.2, 0.1},
                            {0.1, 0.2, 0.3}};

            double[,] netReturns = WallRiskEngine.API.computeSimpleNetReturns(returns, 1);
        }

        public void SimpleNetReturnsTest()
        {
            double[,] returns = { {0.05, 0.1, 0.6}, {0.001, 0.4, 0.56}, {0.7, 0.001, 0.12}, {0.3, 0.2, 0.1},
                            {0.1, 0.2, 0.3}};

            double[,] netReturns = WallRiskEngine.API.computeCovarianceMatrix(returns);
        }
    }
}
