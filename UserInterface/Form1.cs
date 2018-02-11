using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PupilProjectPlanner;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using BetterSerialization;
using Newtonsoft.Json;

namespace UserInterface
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Windows Forms Object
        /// </summary>
        /// 

        Condition[] conditions = new Condition[] { };
        List<Pupil> importedPupils = null;

        public Form1()
        {
            InitializeComponent();

        }

        private void Save(object sender, EventArgs e)
        {
            //PROJECTS
            string s = "";
            List<ProjectContainer> projects = new List<ProjectContainer>();
            foreach (Project proj in ProjectManager.projectDict.Values)
            {
                projects.Add(new ProjectContainer(proj.Name, proj.MaxMemberCount, proj.MinMemberCount));
            }
            string p1 = JsonConvert.SerializeObject(projects);
            //PARAMS
            string p2 = JsonConvert.SerializeObject(ProjectManager.parameterDict);
            //Conditions
            string p3 = JsonConvert.SerializeObject(ProjectManager.projectConditions);


            System.IO.File.WriteAllText(saveKoaDialog.FileName,  p1 + Environment.NewLine + p2 + Environment.NewLine + p3);
        }
        private void Loadx(object sender, EventArgs e)
        {
            StreamReader stream = new StreamReader(openKoaDialog.FileName);
            string[] json = new string[3];
            using (stream)
            {
                json[0] = stream.ReadLine();
                json[1] = stream.ReadLine();
                json[2] = stream.ReadLine();
            }

            //Projects
            List<ProjectContainer> projectContainers = new List<ProjectContainer>();
            projectContainers = JsonConvert.DeserializeObject<List<ProjectContainer>>(json[0]);
            ProjectManager.projectDict.Clear();

            foreach (ProjectContainer pc in projectContainers)
                ProjectManager.projectDict.Add(pc.Name, new Project(pc.Name, pc.MaxMembers, pc.MinMembers));

            List<string> list = new List<string>();
            foreach (string s in ProjectManager.projectDict.Keys)
            {
                list.Add(s);
            }
            projectDisplay.Lines = list.ToArray();
            //Params
            ProjectManager.parameterDict = JsonConvert.DeserializeObject<Dictionary<string, Parameter>>(json[1]);

            List<string> list1 = new List<string>();
            foreach (string s in ProjectManager.parameterDict.Keys)
            {
                list1.Add(s);
            }
            paramDisplay.Lines = list1.ToArray();

            ProjectManager.projectConditions = JsonConvert.DeserializeObject<Dictionary<string, List<Condition>>>(json[2]);
            List<string> list2 = new List<string>();
            foreach (string s in ProjectManager.projectConditions.Keys)
            {
                string ss = s + ": " + ProjectManager.projectConditions[s].Count + " Bedingung(en)";
                list2.Add(ss);
            }
            condDisplay.Lines = list2.ToArray();
        }

        private void importFromExcel_Click(object sender, EventArgs e)
        {
            browseExcel.FileOk += new CancelEventHandler(finishIt);
            browseExcel.ShowDialog();
        }

        private void finishIt(object sender, System.EventArgs e)
        {
            string path = (sender as OpenFileDialog).FileName;
            importedPupils = new List<Pupil>();
            try
            {
                importedPupils = Importer.Import(path, dataTable);
            }
            catch (Exception ex)
            {
                //debug.Text = ex.Message;
                if(ex.Message != "Cancelled")
                    MessageBox.Show(ex.Message + Environment.NewLine + "Der File, der importiert werden soll, darf nicht geöffnet sein.", "Import Error", MessageBoxButtons.OK);
                return;
            }
            
        }

        private void SaveExcelFile(object sender, EventArgs e)
        {
            Exporter.ExportToExcel(saveExcelDialog.FileName);
        }

        #region Parameter
        //Ist eigentlich Parameter, lol
        private void newCondCreate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(newCondName.Text) || ProjectManager.projectDict.Keys.Contains(newCondName.Text))
                return;

            if (newCondInt.Checked)
                ProjectManager.parameterDict.Add(newCondName.Text, new ParameterInt(ProjectManager.parameterDict.Count));
            else if (newCondString.Checked)
                ProjectManager.parameterDict.Add(newCondName.Text, new ParameterInt(ProjectManager.parameterDict.Count));

            List<string> list = new List<string>();

            foreach (string s in ProjectManager.parameterDict.Keys)
            {
                list.Add(s);
            }
            paramDisplay.Lines = list.ToArray();
        }

        //Ist eigentlich Parameter
        private void deleteCond_Click(object sender, EventArgs e)
        {
            if (ProjectManager.parameterDict.Keys.Contains(newCondName.Text))
                ProjectManager.parameterDict.Remove(newCondName.Text);

            List<string> list = new List<string>();

            foreach (string s in ProjectManager.parameterDict.Keys)
            {
                list.Add(s);
            }
            paramDisplay.Lines = list.ToArray();
        }

        private void ComboBox_Click(object sender, EventArgs e)
        {
            ComboBox combobox = ((ComboBox)sender);
            combobox.Items.Clear();
            string[] a = ProjectManager.parameterDict.Keys.ToArray();

            for (int i = 0; i < a.Length; i++)
            {
                combobox.Items.Add(a[i]);
            }
        }
        #endregion

        #region Project
        private void CreateNewProjButton_Click(object sender, EventArgs e)
        {
            if (!ProjectManager.projectDict.Keys.Contains(createNewProj.Text))
                ProjectManager.projectDict.Add(createNewProj.Text, new Project(createNewProj.Text, (int)newProjMax.Value, (int)NewProjMin.Value));
            ProjectManager.projectDict[createNewProj.Text].Condition = new ConditionPool();

            List<string> list = new List<string>();
            foreach (string s in ProjectManager.projectDict.Keys)
            {
                list.Add(s);
            }
            projectDisplay.Lines = list.ToArray();
        }

        private void ProjectCB_Click(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            cb.Items.Clear();
            foreach (string s in ProjectManager.projectDict.Keys)
            {
                cb.Items.Add(s);
            }
        }

        //Remove Project
        private void button5_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(removeProjBox.SelectedText))
                ProjectManager.projectDict.Remove(removeProjBox.SelectedText);

            List<string> list = new List<string>();

            foreach (string s in ProjectManager.projectDict.Keys)
            {
                list.Add(s);
            }
            projectDisplay.Lines = list.ToArray();
        }
        #endregion

        //Elon Musk is badass.

        #region Condition
        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(comparer.Text) || String.IsNullOrEmpty(selectProjBox.Text) || String.IsNullOrEmpty(addCondToPBox.Text))
                return;

            string ssss = comparer.Text;
            Condition.Types type = new Condition.Types();
            switch (comparer.Text)
            {
                case ">":
                    type = Condition.Types.Greater;
                    break;
                case "=":
                    type = Condition.Types.Equal;
                    break;
                case "<":
                    type = Condition.Types.Less;
                    break;
                case "nicht =":
                    type = Condition.Types.UnEqual;
                    break;
            }

            if (ProjectManager.projectConditions.ContainsKey(selectProjBox.SelectedText))
                ProjectManager.projectConditions[selectProjBox.SelectedText].Add(new Condition(type, ProjectManager.parameterDict[addCondToPBox.Text], addCondToPvalue.Text));
            else
                ProjectManager.projectConditions.Add(selectProjBox.SelectedText, new List<Condition> { new Condition(type, ProjectManager.parameterDict[addCondToPBox.Text], addCondToPvalue.Text) });

            List<string> list = new List<string>();
            foreach (string s in ProjectManager.projectConditions.Keys)
            {
                string ss = s + ": " + ProjectManager.projectConditions[s].Count + " Bedingung(en)";
                list.Add(ss);
            }
            condDisplay.Lines = list.ToArray();

            //ConditionPool p = (ConditionPool)ProjectManager.projectDict[selectProjBox.Text].Condition;
            //List<ICondition> l = new List<ICondition>();

            //if (p.Conditions != null)
            //    l = p.Conditions.ToList<ICondition>();

            //if (Int32.TryParse(addCondToPvalue.Text, out int result) == false)
            //    return;

            //l.Add(new Condition(type, ProjectManager.parameterDict[addCondToPBox.Text], result));
            //p.Conditions = l.ToArray();
            //ProjectManager.projectDict[selectProjBox.Text].Condition = p;

            //List<string> list = new List<string>();
            //foreach (Project pr in ProjectManager.projectDict.Values)
            //{
            //    if (((ConditionPool)pr.Condition).Conditions != null)
            //    {
            //        string ss = pr.Name + ": " + ((ConditionPool)pr.Condition).Conditions.Length + " Bedingungen" + Environment.NewLine;
            //        list.Add(ss);
            //    }
            //}

        }

        #endregion
        private void importDB_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("user id=lakoa;password=lakoa1337;server=localhost;Trusted_Connection=yes;database=sport;connection timeout=30");
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                importDB.Text = "BRUH";
            }

            SqlCommand cmd = new SqlCommand("SELECT * FROM choices", conn);
            cmd.ExecuteNonQuery();

            List<Pupil> pupils = new List<Pupil>();
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string name = reader["firstname"].ToString() + reader["lastname"].ToString();
                    Pupil p = new Pupil(name, new Project[] {
                        ProjectManager.projectDict[reader["first"].ToString()],
                        ProjectManager.projectDict[reader["second"].ToString()],
                        ProjectManager.projectDict[reader["third"].ToString()]
                        }, new Parameter[] { ProjectManager.parameterDict["year"] }, new object[] { Int32.Parse(reader["year"].ToString()) });
                    pupils.Add(p);
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.ToString());
            }
            conn.Close();
        }

        #region ClickEvents
        private void öffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openKoaDialog.FileOk += new CancelEventHandler(Loadx);
            openKoaDialog.ShowDialog();
        }

        private void speichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveKoaDialog.FileOk += new CancelEventHandler(Save);
            saveKoaDialog.ShowDialog();
        }
        #endregion

        private void startAlgorithm_Click(object sender, EventArgs e)
        {
            if(importedPupils == null)
            {
                MessageBox.Show("Sie müssen erst Schüler importieren, bevor Sie sie zuweisen können", "Error", MessageBoxButtons.OK);
                return;
            }

            PPP ppp = new PPP();
            ProjectManager.leftOvers = ppp.Assign(ProjectManager.projectDict.Values, importedPupils).ToList<Pupil>();

            MessageBox.Show("Fertig. Sie können nun die Ergebnisse als Excel-Tabelle speichern.", "Fertig", MessageBoxButtons.OK);

            saveExcelDialog.FileOk += new CancelEventHandler(SaveExcelFile);
            saveExcelDialog.ShowDialog();
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("LAKOA \r\nEntwickelt von TeamKOA(http://www.github.com/TeamKOA)\r\nCopyright 2017-2018 by TeamKOA", "Credits", MessageBoxButtons.OK);
        }
    }
}

