using DHI.Generic.MikeZero.DFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHI.WF.DfsMath
{
    public class Calculator
    {
        /// <summary>
        /// Multiply a DFS file with spatially variable factor, number of items and number of cells must be identical
        /// </summary>
        /// <param name="input_file">Input filename (dfs1,dfs2,dfs2,dfsu)</param>
        /// <param name="output_file">Output filename (dfs1,dfs2,dfs2,dfsu)</param>
        /// <param name="factor_file">Factor filename (dfs1,dfs2,dfs2,dfsu)</param>
        public void Multiply(string input_file, string output_file, string factor_file)
        {
            var dfs = DfsFileFactory.DfsGenericOpen(input_file);
            File.Copy(input_file, output_file, true);
            var factor_dfs = DfsFileFactory.DfsGenericOpen(factor_file);
            var out_dfs = DfsFileFactory.DfsGenericOpenEdit(output_file);

            var delVal = dfs.FileInfo.DeleteValueFloat;

            var timesteps = dfs.FileInfo.TimeAxis.NumberOfTimeSteps;
            var nitems = dfs.ItemInfo.Count;

            var factors = new List<float[]>();

            for (int k = 0; k < nitems; k++)
            {
                var item = k + 1; // Item numbers are 1-based
                var factor = (float[])factor_dfs.ReadItemTimeStep(item, 0).Data;
                factors.Add(factor);   
            }
            
      
            for (int i = 0; i < timesteps; i++)
            {
                for (int k = 0; k < nitems; k++)
                {
                    var item = k + 1; // Item numbers are 1-based
                    var current = (float[])dfs.ReadItemTimeStep(item, i).Data;
                    float[] output = new float[current.Length];

                    for (int j = 0; j < current.Length; j++)
                    {
                        if (current[j] == delVal)
                        {
                            output[j] = delVal;
                        }
                        else
                        {
                            output[j] = current[j] * factors[k][j];
                        }
                    }

                    out_dfs.WriteItemTimeStep(item, i, 0.0, output);
                }
            }
        }
    }
}
