using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using PupilProjectPlanner;
using System.Windows.Forms;

namespace UserInterface
{
    class Importer
    {

        public static List<Pupil> Import(string path, DataGridView dt, TextBox debug = null)
        {
            List<string> errorNames = new List<string>();
            List<Pupil> allPupil = new List<Pupil>();
            DataTable t = new DataTable();
            dt.Rows.Clear();
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    t = result.Tables[0];
                }

                BindingSource bs = new BindingSource();
                bs.DataSource = t;
                dt.DataSource = bs;

                int nameIndex = 0;
                Dictionary<int, Project> projectIndex = new Dictionary<int, Project>();
                Dictionary<int, string> parameterIndex = new Dictionary<int, string>();

                object[] header = t.Rows[0].ItemArray;

                for (int i = 0; i < header.Length; i++)
                {
                    string s = header[i] as string;
                    if (s != null)
                        if (s.ToLower() == "name")
                            nameIndex = i;
                }

                for (int i = 0; i < header.Length; i++)
                {
                    string s = header[i] as string;

                    if (s != null)
                        if (ProjectManager.projectDict.ContainsKey(s))
                            projectIndex.Add(i, ProjectManager.projectDict[s]);
                }

                for (int i = 0; i < header.Length; i++)
                {
                    string s = header[i] as string;

                    if (s != null)
                        if (ProjectManager.parameterDict.ContainsKey(s))
                            parameterIndex.Add(i, s);
                }
                string d = "NameIndex: " + nameIndex + Environment.NewLine;
                foreach (int i in projectIndex.Keys)
                    d += i + "->" + projectIndex[i].Name + Environment.NewLine;
                foreach (int i in parameterIndex.Keys)
                    d += i + "->" + parameterIndex[i] + Environment.NewLine;

                if (debug != null)
                    debug.Text = d;

                for (int i = 1; i < t.Rows.Count; i++)
                {
                    object[] o = t.Rows[i].ItemArray;

                    if (!String.IsNullOrWhiteSpace(o[nameIndex] as string))
                    {

                        Pupil p = new Pupil(o[nameIndex] as string);
                        Dictionary<int, Project> choices = new Dictionary<int, Project>();
                        List<Project> projects = new List<Project>();

                        foreach (int pi in projectIndex.Keys)
                        {
                            //if(debug != null)
                            //debug.Text += o[pi] + ", ";
                            if (o[pi] != null && o[pi] != System.DBNull.Value)
                            {
                                choices.Add((int)((double)o[pi]), projectIndex[pi]);
                            }
                        }

                        for (int c = 1; c < choices.Count + 1; c++)
                        {
                            if (!choices.Keys.Contains(c))
                                errorNames.Add(p.Name);

                            projects.Add((!choices.Keys.Contains(c)) ? null : choices[c]);
                        }

                        while (projects.Count < 3)
                        {
                            if (!errorNames.Contains(p.Name))
                                errorNames.Add(p.Name);
                            projects.Add(null);
                        }

                        p.Choices = projects.ToArray(); ;

                        //foreach (Project pppp in projects)
                        //{
                        // if (debug != null)
                        //    debug.Text += pppp.Name + " / ";

                        //}
                        //projects above - params below
                        List<Parameter> parameter = new List<Parameter>();
                        List<object> values = new List<object>();

                        foreach (int pai in parameterIndex.Keys)
                        {
                            parameter.Add(ProjectManager.parameterDict[parameterIndex[pai]]);
                            values.Add((int)((double)o[pai])); //TODO: Make it so it can handle string
                        }
                        p.SetParameters(parameter.ToArray(), values.ToArray());

                        allPupil.Add(p);
                        //if(debug != null)
                        //debug.Text += p.DetailedToString();
                    }
                }


            }
            if (errorNames.Count > 0)
            {
                string s = "Folgende Schüler hatten fehlerhafte Wahlen:" + Environment.NewLine;
                foreach (string name in errorNames)
                    s += name + ", ";
                s.Remove(s.Length - 2, 2);
                s += Environment.NewLine + "Wollen sie trotzdem fortfahren?";

                DialogResult result = MessageBox.Show(s, "Fehlerhafte Wahlen", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    throw new Exception("Cancelled");
            }

            return allPupil;
        }

    }
}
