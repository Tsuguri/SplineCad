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

        public static bool ExportToIGS(List<Model> models, string file)
        {
            #region Local Functions

            int GetParameterLinesCount(Model model)
            {
                switch (model)
                {
                    case NurbsSurface ns:
                        return 4 + ns.UDivs.Count + 4 + ns.VDivs.Count + ns.Points.Length * 2 + 2;
                    case BSplineSurface bs:
                        return 4 + bs.Points.GetLength(0)  + 4 + bs.Points.GetLength(1) + 
                            bs.Points.Length * 2 + 2;
                    default:
                        return -1;
                }               
            }

            List<string> FillNurbsSurfaceParameters(NurbsSurface surf, ref int idx, int objIdx)
            {
                List<string> ret = new List<string>();

                string degU = (surf.Points.GetLength(0) - 1).ToString() + ", ";
                string degV = (surf.Points.GetLength(1) - 1).ToString() + ", ";

                //add necessary parameters
                ret.Add(("128, " + degU + degV + "3, 3, 0, 0, 0, 0, 0,").PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));

                //add U-knot vector
                ret.Add("0.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                ret.Add("0.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));

                for (int i = 0; i < surf.UDivs.Count; i++)
                {
                    ret.Add((surf.UDivs[i].Value.ToString() + ",").PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                }

                ret.Add("1.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                ret.Add("1.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));

                //Add V-knot vector
                ret.Add("0.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                ret.Add("0.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));

                for (int i = 0; i < surf.VDivs.Count; i++)
                {
                    ret.Add((surf.VDivs[i].Value.ToString() + ",").PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                }

                ret.Add("1.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                ret.Add("1.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));

                //add weights
                for (int i = 0; i < surf.Points.GetLength(0); i++)
                    for (int j = 0; j < surf.Points.GetLength(1); j++)
                    {
                        ret.Add((surf.Points[j, i].Position.W.ToString() + ",").PadRight(64, ' ') +
                        objIdx.ToString().PadLeft(8) + "P" +
                        (++idx).ToString().PadLeft(7, '0'));
                    }

                //add points coords
                for (int i = 0; i < surf.Points.GetLength(0); i++)
                    for (int j = 0; j < surf.Points.GetLength(1); j++)
                    {
                        Vector4 p = surf.Points[j, i].Position;
                        ret.Add((p.X.ToString() + ", " +
                                      p.Y.ToString() + ", " +
                                      p.Z.ToString() + ", ").PadRight(64, ' ') +
                            objIdx.ToString().PadLeft(8) + "P" + (++idx).ToString().PadLeft(7, '0'));
                    }

                ret.Add("0.0, 1.0, 0.0, 1.0;".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                return ret;
            }

            List<string> FillBsplineSurfaceParameters(BSplineSurface surf, ref int idx, int objIdx)
            {
                List<string> ret = new List<string>();

                string degU = (surf.Points.GetLength(0) - 1).ToString() + ", ";
                string degV = (surf.Points.GetLength(1) - 1).ToString() + ", ";

                int Udivs = 4 + surf.Points.GetLength(0);
                int Vdivs = 4 + surf.Points.GetLength(1);

                //add necessary parameters
                ret.Add(("128, " + degU + degV + "3, 3, 0, 0, 0, 0, 0,").PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));

                //add U-knot vector
                for (int i = 0; i < Udivs; i++)
                {
                    ret.Add((i.ToString() + ",").PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                }

                for (int i = 0; i < Vdivs; i++)
                {
                    ret.Add((i.ToString() + ",").PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                }


                //add weights
                for (int i = 0; i < surf.Points.GetLength(0); i++)
                    for (int j = 0; j < surf.Points.GetLength(1); j++)
                    {
                        ret.Add("1.0,".PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                        (++idx).ToString().PadLeft(7, '0'));
                    }

                //add points coords
                for (int i = 0; i < surf.Points.GetLength(0); i++)
                    for (int j = 0; j < surf.Points.GetLength(1); j++)
                    {
                        Vector3 p = surf.Points[j, i].Position;
                        ret.Add((p.X.ToString() + ", " +
                                      p.Y.ToString() + ", " +
                                      p.Z.ToString() + ", ").PadRight(64, ' ') +
                            objIdx.ToString().PadLeft(8) + "P" + (++idx).ToString().PadLeft(7, '0'));
                    }

                ret.Add(("0.0, " + (Udivs - 1).ToString() + ", 0.0, " + (Vdivs - 1).ToString() +
                    ";").PadRight(64, ' ') + objIdx.ToString().PadLeft(8) + "P" +
                    (++idx).ToString().PadLeft(7, '0'));
                return ret;
            }
            #endregion

            List<string> contents = new List<string>();

            contents.Add("SplineCAD conversion to IGES file. Shoop da woop!".PadRight(72, ' ') + "S0000001");

            //Copy-pasta of SolidWorks global section
            contents.Add("1H,,1H;,13HCzesc1.SLDPRT,40HC:/Users/wlodarskip/Downloads/Czesc1.IGS,   G      1");
            contents.Add("15HSolidWorks 2014,15HSolidWorks 2014,32,308,15,308,15,13HCzesc1.SLDPRT,G      2");
            contents.Add("1.,2,2HMM,50,0.125,13H170526.090550,1E-008,499990.,10Hwlodarskip,,11,0, G      3");
            contents.Add("13H170526.090550;                                                       G      4");

            int objIndex = 1;

            int idxD = 0;
            int idxP = 0;

            List<List<string>> surfacesParameters = new List<List<string>>();
            foreach(Model m in models)
            {
	            var em = m;
                if (em is TSplineSurface)
                    em = (em as TSplineSurface).ConvertToNurbs();

                int paramLines = GetParameterLinesCount(em);

                //Add rational b-spline type
                contents.Add("128".PadLeft(8, ' ') + "1".PadLeft(8, ' ') +
                             "0".PadLeft(8, ' ') + "1".PadLeft(8, ' ') +
                             "0".PadLeft(8, ' ') + "0".PadLeft(8, ' ') +
                             "0".PadLeft(8, ' ') + "".PadLeft(8, ' ') +
                             "1".PadLeft(8, ' ') + "D" + (++idxD).ToString().PadLeft(7, '0'));

                contents.Add("128".PadLeft(8, ' ') + "1".PadLeft(8, ' ') +
                             "0".PadLeft(8, ' ') +
                             paramLines.ToString().PadLeft(8, ' ') +
                             "0".PadLeft(8, ' ') + "".PadLeft(8, ' ') +
                             "".PadLeft(8, ' ') + "".PadLeft(8, ' ') +
                             "".PadLeft(8, ' ') + "D" + (++idxD).ToString().PadLeft(7, '0'));
                switch (em)
                {
                    case NurbsSurface ns:
                        surfacesParameters.Add(FillNurbsSurfaceParameters(ns, ref idxP, objIndex));
                        break;
                    case BSplineSurface bs:
                        surfacesParameters.Add(FillBsplineSurfaceParameters(bs, ref idxP, objIndex));
                        break;
                }

                objIndex += 2;
            }

            //append params of each surface
            foreach(List<string> Params in surfacesParameters)
            {
                contents.AddRange(Params);
            }

            //terminate
            contents.Add(("S" + "1".PadLeft(7, ' ') +
                         "G" + "0".PadLeft(7, ' ') +
                         "D" + "2".PadLeft(7, ' ') +
                         "P" + idxP.ToString().PadLeft(7, ' ')).PadRight(72, ' ') +
                         "T" + "1".PadLeft(7, ' '));

            File.WriteAllLines(file, contents);
            return true;
        }

    }
}
