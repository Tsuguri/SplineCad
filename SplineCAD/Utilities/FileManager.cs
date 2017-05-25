using SplineCAD.Objects;
using SplineCAD.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SplineCAD.Utilities
{
    public static class FileManager
    {
        public static void ExportToIGS(List<Model> models, string file)
        {
            if (models.Count != 1 || !(models[0] is NurbsSurface)) //saving just 1 nurbs, TODO rest
                return;
            string[] contents = new string[2];
            contents[0] = "Dobrowolski - 1/10";
            contents[1] = "Joasia - 10/10";
            File.WriteAllLines(file, contents);
        }

    }
}
