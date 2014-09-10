using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WallRiskEngine
{
    public class API
    {
        const String pathToDll = @"wre-ensimag-c-4.1.dll";

        [DllImport(pathToDll, EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NORMmodelingCov(
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
        public static extern int WREmodelingReturns(
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
            Debug.Assert(nbAssets >= 1, "Asset Values matrix must have more than 2 assets");
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

    }
}
