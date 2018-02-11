using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using PupilProjectPlanner;

namespace UserInterface
{
    class Exporter
    {

        public static void ExportToExcel(string path)
        {
            Application app = new Application();

            Workbook wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            app.Visible = true;
            app.Range["A1"].Cells.Select();
            Worksheet worksheet = wb.Sheets[1];

            for (int x = 0; x < ProjectManager.projectDict.Count; x++)
            {
                app.Range["A1"].Cells.Select();
                Pupil[] pupils = ProjectManager.projectDict.Values.ToArray<Project>()[x].Members.ToArray();
                app.ActiveCell.Offset[0, x].Value2 = ProjectManager.projectDict.Values.ElementAt<Project>(x).Name;

                for (int y = 0; y < pupils.Length; y++)
                {
                    app.ActiveCell.Offset[y + 1, x].Value2 = pupils[y].Name;
                }

            }
            app.Range["A1"].Cells.Select();
            app.ActiveCell.Offset[0, ProjectManager.projectDict.Count + 1].Value2 = "Nicht zugewiesen";

            for (int y = 0; y < ProjectManager.leftOvers.Count; y++)
            {
                app.ActiveCell.Offset[y + 1, ProjectManager.projectDict.Count + 1].Value2 = ProjectManager.leftOvers.ElementAt<Pupil>(y).Name;
            }

            wb.SaveAs(path);
        }

    }
}