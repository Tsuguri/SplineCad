using SplineCAD.Objects;
using SplineCAD.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace SplineCAD.Utilities
{
    public static class FileManager
    {
        public static void ExportToIGS(List<Model> models, string file)
        {
            if (models.Count != 1 || !(models[0] is NurbsSurface)) //saving just 1 nurbs, TODO rest
                return;
            NurbsSurface surf = models[0] as NurbsSurface;


            List<string> contents = new List<string>();

            contents.Add("SplineCAD conversion to IGES file. Shoop da woop!".PadRight(72, ' ') + "S0000001");

            //Copy-pasta of SolidWorks global section
            contents.Add("1H,,1H;,13HCzesc1.SLDPRT,40HC:/Users/wlodarskip/Downloads/Czesc1.IGS,   G      1");
            contents.Add("15HSolidWorks 2014,15HSolidWorks 2014,32,308,15,308,15,13HCzesc1.SLDPRT,G      2");
            contents.Add("1.,2,2HMM,50,0.125,13H170526.090550,1E-008,499990.,10Hwlodarskip,,11,0, G      3");
            contents.Add("13H170526.090550;                                                       G      4");

            int paramLinesCount = 1;
            paramLinesCount += 4 + surf.UDivs.Count + 4 + surf.VDivs.Count;
            paramLinesCount += surf.Points.Length * 2 + 1;

            //Add rational b-spline type
            contents.Add("128".PadLeft(8, ' ') + "1".PadLeft(8, ' ') +
                         "0".PadLeft(8, ' ') + "1".PadLeft(8, ' ') +
                         "0".PadLeft(8, ' ') + "0".PadLeft(8, ' ') +
                         "0".PadLeft(8, ' ') + "".PadLeft(8, ' ') +
                         "1".PadLeft(8, ' ') + "D" + "1".PadLeft(7, '0'));

            contents.Add("128".PadLeft(8, ' ') + "1".PadLeft(8, ' ') +
                         "0".PadLeft(8, ' ') +
                         paramLinesCount.ToString().PadLeft(8, ' ') +
                         "0".PadLeft(8, ' ') + "".PadLeft(8, ' ') +
                         "".PadLeft(8, ' ') + "".PadLeft(8, ' ') +
                         "".PadLeft(8, ' ') + "D" + "2".PadLeft(7, '0'));
            int idx = 0;

            string degU = (surf.Points.GetLength(0) - 1).ToString() + ", ";
            string degV = (surf.Points.GetLength(1) - 1).ToString() + ", ";

            //add necessary parameters
            contents.Add(("128, " + degU + degV + "3, 3, 0, 0, 0, 0, 0,").PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));

            //add U-knot vector
            contents.Add("0.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));
            contents.Add("0.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));

            for(int i = 0; i < surf.UDivs.Count; i++)
            {
                contents.Add((surf.UDivs[i].Value.ToString() + ",").PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));
            }

            contents.Add("1.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));
            contents.Add("1.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));

            //Add V-knot vector
            contents.Add("0.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));
            contents.Add("0.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));

            for (int i = 0; i < surf.VDivs.Count; i++)
            {
                contents.Add((surf.VDivs[i].Value.ToString() + ",").PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));
            }

            contents.Add("1.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));
            contents.Add("1.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));

            //add weights
            for (int i = 0; i < surf.Points.Length; i++)
            {
                contents.Add("1.0,".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));
            }

            //add points coords
            for (int i = 0; i < surf.Points.GetLength(0); i++)
                for (int j = 0; j < surf.Points.GetLength(1); j++)
                {
                    Vector4 p = surf.Points[j, i].Position;
                    contents.Add((p.X.ToString() + ", " +
                                  p.Y.ToString() + ", " +
                                  p.Z.ToString() + ", ").PadRight(71, ' ') +
                        "1P" + (++idx).ToString().PadLeft(7, '0'));
                }

            contents.Add("0.0, 1.0, 0.0, 1.0;".PadRight(71, ' ') + "1P" +
                (++idx).ToString().PadLeft(7, '0'));

            //terminate
            contents.Add(("S" + "1".PadLeft(7, ' ') +
                         "G" + "0".PadLeft(7, ' ') +
                         "D" + "2".PadLeft(7, ' ') +
                         "P" + paramLinesCount.ToString().PadLeft(7, ' ')).PadRight(72, ' ') +
                         "T" + "1".PadLeft(7, ' '));

            File.WriteAllLines(file, contents);
        }

    }
}
