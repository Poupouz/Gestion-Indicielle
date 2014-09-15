using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WallRiskEngine
{
    public class API
    {
        const String pathToDll = @"wre-ensimag-c-4.1.dll";

        [DllImport(pathToDll, EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NORMmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );

        public static double[,] computeCovarianceMatrix(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] covMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int returnFromNorm = NORMmodelingCov(ref dataSize, ref nbAssets, returns, covMatrix, ref info);
            if (returnFromNorm != 0)
            {
                throw new Exception(); // Check out what went wrong here
            }
            return covMatrix;
        }

        [DllImport(pathToDll, EntryPoint = "WREmodelingReturns", CallingConvention = CallingConvention.Cdecl)]
        private static extern int WREmodelingReturns(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetsValues,
            ref int horizon,
            double[,] assetsReturns,
            ref int info
        );

        public static double[,] computeSimpleNetReturns(double[,] assetsValues, int horizon)
        { 
            int nbValues = assetsValues.GetLength(0);
            int nbAssets = assetsValues.GetLength(1);
            /* check the arguments */
            Debug.Assert(nbValues >= 2, "Asset Values matrix must have more than 2 values");
            Debug.Assert(nbAssets >= 1, "Asset Values matrix must have more than 1 assets");
            Debug.Assert(0 < horizon && horizon < nbValues, "Horizon must be positive and less than number of values of the matrix");
            for (int i = 0; i < nbValues; i++)
			{
                for (int j = 0; j < nbAssets; j++)
                {
                    Debug.Assert(assetsValues[i, j] > 0, "Asset Values matrix should have strictly positive values");
                }
			}
            double[,] assetsReturns = new double[nbValues-horizon, nbAssets];
            int info = 0;
            /* Call C library */
            int exitCode = WREmodelingReturns(ref nbValues, ref nbAssets, assetsValues, ref horizon, assetsReturns, ref info);
            /* Throw error if exit code != 0 */
            if (exitCode != 0)
            {
                throw new Exception();
            }
            return assetsReturns;
        }

        [DllImport(pathToDll, EntryPoint = "WREallocIT", CallingConvention = CallingConvention.Cdecl)]
        private static extern int WREallocIT(
	        ref int nbAssets,
            double[,] cov,
            double[] expectedReturns, 
	        double[] benchmarkCov,
            ref double benchmarkExpectedReturn,
            ref int nbEqConst, 
	        ref int nbIneqConst,
            double[,] C,
            double[] b,
            double[] minWeights,
            double[] maxWeights,
            ref double relativeTargetReturn,
	        double[] optimalWeights,
            ref int info
        );

        public static double[] OptimPortfolioWeight(
            double[,] cov,
            double[] expectedReturns,
            double[] benchmarkCov,
            double benchmarkExpectedReturn,
            double relativeTargetReturn)
        {
            int nbAssets = cov.GetLength(0);
            /* Check cov matrix */
            Debug.Assert(nbAssets == cov.GetLength(1), "Cov matrix should be a square matrix");
            Debug.Assert(nbAssets > 1, "Covariance should be at least of dimension 2");

            int nbEqConst = 1;
            int nbIneqConst = 0;

            double[,] C = new double[nbAssets, 1];

            for (int i = 0; i < nbAssets; i++)
            {
                C[i, 0] = 1;
            }

            double[] b = { 1 };

            double[] minWeights = new double[nbAssets];
            double[] maxWeights = new double[nbAssets];
            for (int i = 0; i < nbAssets; i++)
            {
                maxWeights[i] = 1;
            }

            double[] optimalWeights = new double[nbAssets];
            int info = 0;

            /* Call C library */
            int exitCode = WREallocIT(ref nbAssets, cov, expectedReturns, benchmarkCov, 
                ref benchmarkExpectedReturn, ref nbEqConst, ref nbIneqConst, C, b, minWeights,
                maxWeights, ref relativeTargetReturn, optimalWeights, ref info);
            /* Throw error if exit code != 0 */
            
            if (exitCode != 0)
            {
                if (exitCode == 5)
                {

                    throw new Exception("Error " + info.ToString() + ": Numerical problem in computation");
                    
                }
                else if (exitCode == 2)
                {
                    throw new Exception("Error " + info.ToString() + ": Memory allocation problem");
                }
                else if (exitCode == 6)
                {
                    throw new Exception("Error " + info.ToString() + ": Warning, computation not optimal");
                }
                else if ((exitCode / 100) > 1 && (exitCode / 100)<1.5)
                {
                    throw new Exception("Error " + info.ToString() + ": Error in the " + (exitCode % 100) + "th argument size");
                }
                else if ((exitCode / 100) > 3 && (exitCode / 100) < 3.5)
                {
                    throw new Exception("Error " + info.ToString() + ": Illegal value(s) of the " + (exitCode % 100) + "th argument");
                }
                else if (exitCode >= 40 && exitCode < 49)
                {
                    throw new Exception("Error " + info.ToString() + ": License has expired or bad license");
                }
                else
                {
                    throw new Exception();
                }
            }
            return optimalWeights;
        }
        
    }
}
