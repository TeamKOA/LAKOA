using PupilProjectPlanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    class ProjectManager
    {

        public static Dictionary<string, Project> projectDict = new Dictionary<string, Project>();
        public static Dictionary<string, List<Condition>> projectConditions = new Dictionary<string, List<Condition>>();
        public static Dictionary<string, Parameter> parameterDict = new Dictionary<string, Parameter>();
        public static List<Pupil> leftOvers = new List<Pupil>();

         void Stuff()
         {
            
            foreach(string s in projectDict.Keys)
            {
                ICondition[] conds = new ICondition[projectConditions[s].Count];
                Condition[] c = projectConditions[s].ToArray();
                for (int i = 0; i < c.Length; i++)
                {
                    conds[i] = c[i];
                }
                projectDict[s].Condition = new ConditionPool(conds);
            }

            Project[] projects = projectDict.Values.ToArray();

        }

        static void Sample()
        {
            // Define projects
            Project[] projects = new Project[]
            {
                new Project("Some Project", 5),
                new Project("Some other project that has a minSize", 10, 5)
            };

            // Define Parameters
            Parameter[] parameters = Parameter.CreateParameters(
                Parameter.DataTypes.String,
                Parameter.DataTypes.Int,
                Parameter.DataTypes.Int);

            // Define conditions
            projects[0].Condition = new Condition(Condition.Types.Equal, parameters[0], "static value");
            projects[1].Condition = new ConditionPool(new ICondition[]
            {
                new Condition(Condition.Types.Greater, parameters[1], parameters[2]),
                new Condition(Condition.Types.UnEqual, parameters[2], 0)
            });

            // Insert some pupils
            Pupil[] pupils = new Pupil[]
            {
                new Pupil("Some Pupil", new Project[] {projects[0], projects[1] }, parameters.Length),
                new Pupil("Some other Pupil", new Project[] {projects[1], projects[0] }, parameters, new object[]
                {
                    "Random String", 11, 6
                })
            };

            pupils[0].SetParameter(parameters[0], "Some class thing or so");
            pupils[0].SetParameter(parameters[1], 5);
            pupils[0].SetParameter(parameters[2], 3);

            PupilProjectPlanner.PPP planner = new PupilProjectPlanner.PPP();
            var remaining = planner.Assign(projects, pupils);// REMOVED Arg parameters
            //      ^
            //      |
            //    Those guys could not be satisfied with participation :-(

            foreach (var project in projects)
            {
                var members = project.Members;
                string awesomeString = project.DetailedToString();
            }

            // or you can do this
            foreach (var pupil in pupils)
            {
                var project = pupil.Project;
                string anotherAwesomeString = pupil.DetailedToString();
            }
        }
    }
}
