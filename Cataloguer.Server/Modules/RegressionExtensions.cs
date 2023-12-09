using Accord.MachineLearning;
using Accord.Statistics.Models.Regression.Linear;

namespace Cataloguer.Server.Modules;

public static class RegressionExtensions
{
    public static MultivariateLinearRegression Learn(
        this ISupervisedLearning<MultivariateLinearRegression, double[], double[]> alghorithm, double[] xs, double[] ys, short maxPow)
    {
        if (maxPow <= 0) throw new ArgumentOutOfRangeException("Pow can't be negative and equals to zero!");
        
        double[][] newDataSet = new double[xs.Length][];

        for (int i = 0; i < xs.Length; i++)
        {
            var arr = new double[maxPow];
            // формируем массив х, возведенных в степень
            for (int j = 0; j < maxPow; j++)
            {
                arr[j] = Math.Pow(xs[i], j + 1);
            }

            newDataSet[i] = arr;
        }

        double[][] newYs = new double[ys.Length][];

        for (int i = 0; i < ys.Length; i++)
        {
            newYs[i] = new double[] { ys[i] };
        }

        return alghorithm.Learn(newDataSet, newYs);
    }

    public static double Transform(this MultivariateLinearRegression regression, double x, short pow)
    {
        double[] arr = new double[pow]; 
        for (int i = 0; i < pow; i++)
        {
            arr[i] = Math.Pow(x, i + 1);
        }

        double[][] inputs = new double[][] { arr };

        return regression.Transform(inputs)[0][0];
    }
}