using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PupilProjectPlanner;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                RealTest();
            }

            for (int i = 10; i < 10000; i += 10)
            {
                Console.WriteLine((i * 50).ToString("00000") + ", " + Benchmark(i, 50, i * 50, 3, out int[] granted) + ": " + granted[0] + ", " + granted[1] + ", " + granted[2] + " : " + (float)granted[0] / (i * 50) + ", " + (float)granted[1] / (i * 50) + ", " + (float)granted[2] / (i * 50));
            }
            for (int i = 200; i < 0; i += 10)
            {
                Console.WriteLine((i * 50).ToString("00000") + ", "
                    + (Benchmark(i, 50, i * 50, 3)
                    + Benchmark(i, 50, i * 50, 3)
                    + Benchmark(i, 50, i * 50, 3)
                    + Benchmark(i, 50, i * 50, 3)
                    + Benchmark(i, 50, i * 50, 3)) / 5);
            }
            while (true)
            {
                int i = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine((i * 50).ToString("0000") + ", "
                    + (Benchmark(i, 50, i * 50, 3)
                    + Benchmark(i, 50, i * 50, 3)
                    + Benchmark(i, 50, i * 50, 3)
                    + Benchmark(i, 50, i * 50, 3)
                    + Benchmark(i, 50, i * 50, 3)) / 5);
            }

            while (true)
            {
                int projectNum, ProjectSize, pupilNum, choiceCounts;
                Console.WriteLine("int projectNum, int ProjectSize, int pupilNum, int choiceCount");
                projectNum = Convert.ToInt32(Console.ReadLine());
                ProjectSize = Convert.ToInt32(Console.ReadLine());
                pupilNum = Convert.ToInt32(Console.ReadLine());
                choiceCounts = 3;

                Console.WriteLine(Benchmark(projectNum, ProjectSize, pupilNum, choiceCounts));
            }

            Dictionary<int, Project> projects = new Dictionary<int, Project>() {
                {0, new Project("A", 1000, 5) },
                {1, new Project("B", 1000, 5) },
                {2, new Project("C", 1000, 5) },
                {3, new Project("D", 1000, 5) },
                {4, new Project("E", 1000, 5) },
                {5, new Project("F", 1000, 5) },
                {6, new Project("G", 1000, 5) },
                {7, new Project("H", 1000, 5) },
                {8, new Project("I", 1000, 5) },
                {9, new Project("J", 1000, 5) },
                {10, new Project("K", 1000, 5) },
                {11, new Project("L", 1000, 5) },
                {12, new Project("M", 1000, 5) },
                {13, new Project("N", 1000, 5) },
                {14, new Project("O", 1000, 5) },
                {15, new Project("P", 1000, 5) },
                {16, new Project("Q", 1000, 5) },
            };

            Random random = new Random();
            List<Pupil> pupils = new List<Pupil>();
            int choiceCount = 3;

            for (int i = 0; i < 10000; i++)
            {
                Pupil pupil = new Pupil(i.ToString());
                List<Project> choices = new List<Project>(projects.Values);

                pupil.Choices = new Project[choiceCount];

                for (int c = 0; c < choiceCount; c++)
                {
                    pupil.Choices[c] = choices[random.Next(choices.Count)];
                }
                
                pupils.Add(pupil);
            }

            foreach (var pupil in pupils)
            {
                Console.WriteLine(pupil.DetailedToString());
            }

            Console.ReadKey();

             PupilProjectPlanner.PPP planner = new PupilProjectPlanner.PPP();

            var result = planner.Assign(projects.Values, pupils);

            foreach (var project in projects.Values)
            {
                Console.WriteLine(project.DetailedToString());
            }
            Console.WriteLine();
            foreach (var item in result)
            {
                Console.WriteLine(item.Name);
            }
            Console.ReadKey();
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
            projects[0].Condition = new Condition(Condition.Types.Equal, parameters[0], "String");
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
            var remaining = planner.Assign(projects, pupils, false, parameters);
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

        static long Benchmark(int projectNum, int ProjectSize, int pupilNum, int choiceCount, out int[] granted)
        {
            Parameter[] parameters = new Parameter[] { new ParameterInt(0) };

            Dictionary<int, Project> projects = new Dictionary<int, Project>();
            for (int i = 0; i < projectNum; i++)
            {
                Project p = new Project(i.ToString(), ProjectSize)
                {
                    Condition = new Condition(Condition.Types.Greater, parameters[0], -1)
                };
                projects.Add(i, p);
            }

            Random random = new Random();
            List<Pupil> pupils = new List<Pupil>();

            for (int i = 0; i < pupilNum; i++)
            {
                Pupil pupil = new Pupil(i.ToString(),null, 1);
                pupil.SetParameter(parameters[0], i);
                List<Project> choices = new List<Project>(projects.Values);

                pupil.Choices = new Project[choiceCount];

                for (int c = 0; c < choiceCount; c++)
                {
                    pupil.Choices[c] = choices[random.Next(choices.Count)];
                }

                pupils.Add(pupil);
            }

            PupilProjectPlanner.PPP planner = new PupilProjectPlanner.PPP();

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            var result = planner.Assign(projects.Values, pupils, false, out granted);
            return watch.ElapsedMilliseconds;
        }

        static long Benchmark(int projectNum, int ProjectSize, int pupilNum, int choiceCount)
        {
            return Benchmark(projectNum, ProjectSize, pupilNum, choiceCount, out int[] temp);
        }

        public static void RealTest()
        {
            Parameter[] parameters = new Parameter[]
            {

            };

            int[] statistic = new int[4];

            for (int i = 0; i < 1000; i++)
            {
                Project[] projects = new Project[]
            {
                new Project("Kanu", 8 + 0),
                new Project("Step Aerobic", 8 + 18),
                new Project("Trampolin", 9 + 11),
                new Project("Ausdauer", 7 + 9),
                new Project("Tanztheater", 0 + 28),
                new Project("Kämpfen", 9 + 11),
                new Project("Frauen-Fußball", 12 + 5 + 4),
                new Project("Basketball", 12 + 7),
                new Project("Handball", 9 + 6 + 6),
                new Project("Tennis", 11 + 5),
                new Project("Badminton", 12 + 9),
            };

            Pupil[] pupils = new Pupil[]
            {
                new Pupil("Albrecht Clara Linnéa", GetProjects(projects, new int[] {3, 11, 5}), parameters, new object[] {1, 0, 0}),
new Pupil("Altmann Marc", GetProjects(projects, new int[] {11, 8, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Aßmann Marina", GetProjects(projects, new int[] {7, 0, 0}), parameters, new object[] {1, 0, 0}),
new Pupil("Averdick Dela", GetProjects(projects, new int[] {10, 11, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Beckmann Felix", GetProjects(projects, new int[] {11, 10, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Birkenbeul Hannes", GetProjects(projects, new int[] {1, 2, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Bleckmann Lara-Sophie", GetProjects(projects, new int[] {10, 11, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Bleich Maria", GetProjects(projects, new int[] {6, 11, 1}), parameters, new object[] {0, 1, 0}),
new Pupil("Block Inga Lea", GetProjects(projects, new int[] {6, 8, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Bobe Julius", GetProjects(projects, new int[] {6, 11, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Boltzendahl Ruth", GetProjects(projects, new int[] {5, 8, 6}), parameters, new object[] {1, 1, 0}),
new Pupil("Borgaes Janne Sophie", GetProjects(projects, new int[] {7, 10, 1}), parameters, new object[] {0, 0, 1}),
new Pupil("Böringer Ella Philine", GetProjects(projects, new int[] {11, 9, 3}), parameters, new object[] {1, 1, 0}),
new Pupil("Borstel Jesper von", GetProjects(projects, new int[] {3, 5, 1}), parameters, new object[] {1, 0, 0}),
new Pupil("Brandes Keanu", GetProjects(projects, new int[] {1, 9, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Bretthauer Ben Arne", GetProjects(projects, new int[] {6, 5, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Brychcy Lina", GetProjects(projects, new int[] {5, 10, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Daseking Lilly", GetProjects(projects, new int[] {5, 1, 7}), parameters, new object[] {1, 0, 0}),
new Pupil("Diers Marlene", GetProjects(projects, new int[] {11, 7, 5}), parameters, new object[] {1, 0, 0}),
new Pupil("Dietrich Finja", GetProjects(projects, new int[] {5, 10, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Duden Paul", GetProjects(projects, new int[] {6, 4, 9}), parameters, new object[] {1, 0, 0}),
new Pupil("Ehrmuth Lenard", GetProjects(projects, new int[] {10, 6, 1}), parameters, new object[] {0, 0, 0}),
new Pupil("Eilers Felix", GetProjects(projects, new int[] {6, 3, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Finke Felix Johannes", GetProjects(projects, new int[] {3, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Frenzel David", GetProjects(projects, new int[] {2, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Fuhrberg Stefan", GetProjects(projects, new int[] {8, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Gottschalk Fenja", GetProjects(projects, new int[] {3, 2, 7}), parameters, new object[] {0, 0, 0}),
new Pupil("Gremmel Janne", GetProjects(projects, new int[] {9, 11, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Gremmel Christina Johanne", GetProjects(projects, new int[] {6, 10, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Greschok Miriam", GetProjects(projects, new int[] {3, 5, 2}), parameters, new object[] {1, 1, 0}),
new Pupil("Hartmann Mia-Lynn", GetProjects(projects, new int[] {3, 11, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Hellberg Lea Charlotte", GetProjects(projects, new int[] {5, 2, 7}), parameters, new object[] {0, 0, 0}),
new Pupil("Herbrich Patrick", GetProjects(projects, new int[] {2, 6, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Heyken Katja", GetProjects(projects, new int[] {3, 5, 2}), parameters, new object[] {1, 1, 0}),
new Pupil("Hoffmann Kyra", GetProjects(projects, new int[] {8, 11, 9}), parameters, new object[] {1, 0, 0}),
new Pupil("Horn Katrina", GetProjects(projects, new int[] {8, 2, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Hörr Koko Lana", GetProjects(projects, new int[] {11, 8, 7}), parameters, new object[] {1, 0, 0}),
new Pupil("Keßler Torben", GetProjects(projects, new int[] {4, 3, 1}), parameters, new object[] {0, 0, 0}),
new Pupil("Kinder Johanna", GetProjects(projects, new int[] {4, 5, 8}), parameters, new object[] {1, 1, 0}),
new Pupil("Kistenbrügge Luisa", GetProjects(projects, new int[] {10, 2, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Klein Sebastian", GetProjects(projects, new int[] {5, 11, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Klement Jakob ", GetProjects(projects, new int[] {11, 10, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Klötergens Anna", GetProjects(projects, new int[] {3, 5, 11}), parameters, new object[] {1, 0, 0}),
new Pupil("Klotz Elias", GetProjects(projects, new int[] {6, 3, 11}), parameters, new object[] {0, 0, 0}),
new Pupil("Knoll Johannes", GetProjects(projects, new int[] {11, 3, 8}), parameters, new object[] {1, 1, 0}),
new Pupil("Kokkelink Jan Niklas", GetProjects(projects, new int[] {0, 0, 0}), parameters, new object[] {1, 0, 0}),
new Pupil("Köps Jonathan", GetProjects(projects, new int[] {2, 3, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Kramer Nelly", GetProjects(projects, new int[] {5, 6, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Krankenhagen Anna Theresa", GetProjects(projects, new int[] {5, 3, 11}), parameters, new object[] {1, 0, 0}),
new Pupil("Krismann David", GetProjects(projects, new int[] {3, 4, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Krüger Ines", GetProjects(projects, new int[] {10, 11, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Lindemann Sabrina", GetProjects(projects, new int[] {4, 6, 11}), parameters, new object[] {0, 0, 1}),
new Pupil("Maack Merle", GetProjects(projects, new int[] {10, 7, 11}), parameters, new object[] {0, 0, 0}),
new Pupil("Madsen Jonas", GetProjects(projects, new int[] {11, 9, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Mai Leon", GetProjects(projects, new int[] {8, 11, 3}), parameters, new object[] {0, 0, 1}),
new Pupil("Mätzke Milena", GetProjects(projects, new int[] {6, 5, 7}), parameters, new object[] {1, 0, 0}),
new Pupil("Menzel Lia Marie", GetProjects(projects, new int[] {6, 10, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Meynecke Lisa", GetProjects(projects, new int[] {3, 7, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Möck Linus", GetProjects(projects, new int[] {6, 8, 11}), parameters, new object[] {1, 0, 0}),
new Pupil("Morison Tim", GetProjects(projects, new int[] {1, 2, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Neureiter Nico", GetProjects(projects, new int[] {2, 5, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Nieder Paul", GetProjects(projects, new int[] {5, 1, 11}), parameters, new object[] {1, 0, 0}),
new Pupil("Oelkers Tim", GetProjects(projects, new int[] {2, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Paul Annika", GetProjects(projects, new int[] {6, 10, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Peggau Anna", GetProjects(projects, new int[] {10, 11, 7}), parameters, new object[] {0, 0, 0}),
new Pupil("Prinz Antonia", GetProjects(projects, new int[] {10, 11, 7}), parameters, new object[] {0, 0, 1}),
new Pupil("Prinzing Pauline Emma", GetProjects(projects, new int[] {5, 1, 11}), parameters, new object[] {1, 0, 0}),
new Pupil("Reese Cosima", GetProjects(projects, new int[] {7, 6, 11}), parameters, new object[] {1, 0, 1}),
new Pupil("Rehbein Maxim", GetProjects(projects, new int[] {4, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Rein Maik", GetProjects(projects, new int[] {6, 3, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Renz Philipp", GetProjects(projects, new int[] {4, 8, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Rodewald Arndt", GetProjects(projects, new int[] {6, 3, 1}), parameters, new object[] {0, 0, 0}),
new Pupil("Rohwedder Kjell", GetProjects(projects, new int[] {6, 5, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Rostalski Marlon", GetProjects(projects, new int[] {2, 6, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Rott Emily", GetProjects(projects, new int[] {11, 7, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Rouwen Felix", GetProjects(projects, new int[] {6, 11, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Sassi Emira", GetProjects(projects, new int[] {11, 8, 9}), parameters, new object[] {1, 0, 0}),
new Pupil("Scheuermann Jonas", GetProjects(projects, new int[] {1, 2, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Schlosser Christian", GetProjects(projects, new int[] {8, 11, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Schrader Zoe-Eliza", GetProjects(projects, new int[] {6, 3, 7}), parameters, new object[] {0, 0, 0}),
new Pupil("Schulmeister Anna Kim Sophia", GetProjects(projects, new int[] {9, 7, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Schulz Theresa", GetProjects(projects, new int[] {6, 5, 7}), parameters, new object[] {1, 0, 0}),
new Pupil("Seidel Tim Benedikt", GetProjects(projects, new int[] {5, 2, 4}), parameters, new object[] {1, 0, 0}),
new Pupil("Siever Tom", GetProjects(projects, new int[] {8, 11, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Söllig Kerstin", GetProjects(projects, new int[] {11, 8, 2}), parameters, new object[] {1, 0, 0}),
new Pupil("Sommerfeld Fanny", GetProjects(projects, new int[] {7, 11, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Steingräber Frederik", GetProjects(projects, new int[] {8, 6, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Stoepke Daniel", GetProjects(projects, new int[] {2, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Thiemt Paul", GetProjects(projects, new int[] {4, 3, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Ton Anna Lien", GetProjects(projects, new int[] {6, 10, 5}), parameters, new object[] {1, 0, 0}),
new Pupil("Unger Leon", GetProjects(projects, new int[] {2, 5, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Verwold Jeanne Dinice", GetProjects(projects, new int[] {3, 6, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Wegmann Marius", GetProjects(projects, new int[] {1, 9, 11}), parameters, new object[] {0, 0, 0}),
new Pupil("Werhand Maren", GetProjects(projects, new int[] {5, 1, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Wittmann Laura", GetProjects(projects, new int[] {10, 5, 11}), parameters, new object[] {0, 0, 0}),
new Pupil("Wortmann Judith", GetProjects(projects, new int[] {7, 3, 11}), parameters, new object[] {1, 0, 0}),
new Pupil("Yazdan Pourfard Anahita", GetProjects(projects, new int[] {11, 6, 8}), parameters, new object[] {1, 0, 0}),
new Pupil("Zurmühl Sonja", GetProjects(projects, new int[] {3, 6, 7}), parameters, new object[] {0, 0, 0}),
new Pupil("0 0", GetProjects(projects, new int[] {0, 0, 0}), parameters, new object[] {0, 0, 0}),
new Pupil("Ahlborn Lukas Leonard", GetProjects(projects, new int[] {6, 2, 5}), parameters, new object[] {1, 0, 0}),
new Pupil("Apel Rico", GetProjects(projects, new int[] {4, 6, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Aumann Emily", GetProjects(projects, new int[] {2, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Bewig Mika Johannes", GetProjects(projects, new int[] {5, 11, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Bialowas Lisa", GetProjects(projects, new int[] {9, 11, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Blank Antonia", GetProjects(projects, new int[] {6, 2, 1}), parameters, new object[] {0, 0, 0}),
new Pupil("Böhme Elisabeth", GetProjects(projects, new int[] {10, 7, 11}), parameters, new object[] {0, 0, 0}),
new Pupil("Böning Elisabeth", GetProjects(projects, new int[] {3, 5, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Borcholt Dora Karoline", GetProjects(projects, new int[] {3, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Bormann Paula Nell", GetProjects(projects, new int[] {2, 6, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Brandes Chiara", GetProjects(projects, new int[] {5, 3, 4}), parameters, new object[] {1, 1, 0}),
new Pupil("Budde Marcel", GetProjects(projects, new int[] {11, 8, 9}), parameters, new object[] {0, 0, 0}),
new Pupil("Buhre Ella Frieda", GetProjects(projects, new int[] {10, 8, 7}), parameters, new object[] {0, 0, 0}),
new Pupil("Burgk Konstantin Johannes", GetProjects(projects, new int[] {5, 2, 4}), parameters, new object[] {1, 1, 0}),
new Pupil("Buttchereit Colin", GetProjects(projects, new int[] {5, 4, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Dolezal Celine", GetProjects(projects, new int[] {6, 3, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Dreinhöfer Clemens", GetProjects(projects, new int[] {6, 3, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Dürrkopf Damaris", GetProjects(projects, new int[] {5, 2, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Engelbart Jan Ole", GetProjects(projects, new int[] {2, 5, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Engmann Toni Farina", GetProjects(projects, new int[] {11, 7, 9}), parameters, new object[] {0, 0, 0}),
new Pupil("Flindt Henri Alexander", GetProjects(projects, new int[] {6, 3, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Franke Anna Victoria", GetProjects(projects, new int[] {5, 2, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Fulda Konstantin", GetProjects(projects, new int[] {5, 2, 3}), parameters, new object[] {0, 1, 0}),
new Pupil("Gaide Jannik", GetProjects(projects, new int[] {3, 6, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Galler Phillip", GetProjects(projects, new int[] {9, 11, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Grinev Alina", GetProjects(projects, new int[] {11, 8, 9}), parameters, new object[] {1, 0, 0}),
new Pupil("Grünhagen Fabius", GetProjects(projects, new int[] {5, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Grusie Hella Liane Medea", GetProjects(projects, new int[] {3, 6, 4}), parameters, new object[] {1, 1, 0}),
new Pupil("Gudat Antonia", GetProjects(projects, new int[] {2, 3, 6}), parameters, new object[] {1, 0, 0}),
new Pupil("Gülzow Jan", GetProjects(projects, new int[] {8, 10, 9}), parameters, new object[] {0, 1, 0}),
new Pupil("Günther Annika", GetProjects(projects, new int[] {5, 2, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Hagemeier Jordan Leona", GetProjects(projects, new int[] {6, 5, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Hanne Nils", GetProjects(projects, new int[] {11, 8, 10}), parameters, new object[] {1, 0, 0}),
new Pupil("Hartke Sara", GetProjects(projects, new int[] {5, 6, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Hartmann Jan", GetProjects(projects, new int[] {9, 11, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Heidner Sarah", GetProjects(projects, new int[] {2, 6, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Heims Lennart", GetProjects(projects, new int[] {8, 0, 0}), parameters, new object[] {0, 0, 0}),
new Pupil("Heine Charlotte Sophie", GetProjects(projects, new int[] {2, 6, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Heuer Fynn-Lukas", GetProjects(projects, new int[] {5, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Jerschke Aaron", GetProjects(projects, new int[] {6, 3, 4}), parameters, new object[] {0, 1, 0}),
new Pupil("John Paula", GetProjects(projects, new int[] {11, 10, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Jörren Tilmann", GetProjects(projects, new int[] {2, 5, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Kaminski Florian", GetProjects(projects, new int[] {2, 5, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Keller Elina", GetProjects(projects, new int[] {8, 10, 7}), parameters, new object[] {1, 0, 0}),
new Pupil("Keßler Mara", GetProjects(projects, new int[] {8, 10, 7}), parameters, new object[] {0, 0, 0}),
new Pupil("Kiesche Magnus", GetProjects(projects, new int[] {5, 6, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Knackstedt Jonas Sönke", GetProjects(projects, new int[] {6, 3, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Köhler Jonas Frederic", GetProjects(projects, new int[] {5, 2, 6}), parameters, new object[] {1, 0, 0}),
new Pupil("Krause Lennart", GetProjects(projects, new int[] {2, 6, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Kressmann Friederike", GetProjects(projects, new int[] {2, 5, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Krischke Linda", GetProjects(projects, new int[] {6, 3, 4}), parameters, new object[] {1, 0, 0}),
new Pupil("Kropf Andreas", GetProjects(projects, new int[] {3, 5, 6}), parameters, new object[] {1, 0, 0}),
new Pupil("Küken Jaris", GetProjects(projects, new int[] {5, 2, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Lambrecht Silas", GetProjects(projects, new int[] {5, 6, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Lehmann-Schmidtke Wolfgang Werner", GetProjects(projects, new int[] {4, 5, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Lichthardt Lisanne", GetProjects(projects, new int[] {11, 8, 9}), parameters, new object[] {0, 0, 0}),
new Pupil("Lies Johanna", GetProjects(projects, new int[] {5, 2, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Lipper Florian", GetProjects(projects, new int[] {5, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Meisner Lina", GetProjects(projects, new int[] {2, 6, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Menzel Sebastian", GetProjects(projects, new int[] {5, 3, 6}), parameters, new object[] {1, 1, 0}),
new Pupil("Meyer Jannik", GetProjects(projects, new int[] {0, 0, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Meyer Lea-Sophie", GetProjects(projects, new int[] {7, 11, 8}), parameters, new object[] {1, 0, 0}),
new Pupil("Modler Gesa", GetProjects(projects, new int[] {2, 3, 6}), parameters, new object[] {1, 0, 0}),
new Pupil("Müller Jonas", GetProjects(projects, new int[] {5, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Münter Lara", GetProjects(projects, new int[] {3, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Nagel Tessa", GetProjects(projects, new int[] {11, 9, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Paskowski Anna Celine", GetProjects(projects, new int[] {2, 6, 3}), parameters, new object[] {1, 0, 0}),
new Pupil("Pescht Leonie", GetProjects(projects, new int[] {0, 0, 0}), parameters, new object[] {0, 0, 0}),
new Pupil("Peuke Carolin", GetProjects(projects, new int[] {7, 10, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Peuke Saskia", GetProjects(projects, new int[] {7, 10, 8}), parameters, new object[] {0, 0, 0}),
new Pupil("Prinzing Caroline Christiane", GetProjects(projects, new int[] {4, 2, 5}), parameters, new object[] {1, 0, 0}),
new Pupil("Quante Leonie", GetProjects(projects, new int[] {5, 3, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Rath Katharina Franziska", GetProjects(projects, new int[] {8, 9, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Reimer Angelique", GetProjects(projects, new int[] {5, 6, 4}), parameters, new object[] {0, 0, 0}),
new Pupil("Rittmeier Richard", GetProjects(projects, new int[] {3, 5, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Romer Philipp", GetProjects(projects, new int[] {3, 6, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Rosch Carlotta", GetProjects(projects, new int[] {8, 11, 7}), parameters, new object[] {0, 1, 0}),
new Pupil("Rudolf Ella", GetProjects(projects, new int[] {3, 2, 6}), parameters, new object[] {1, 1, 0}),
new Pupil("Rühmkorb Sabrina", GetProjects(projects, new int[] {5, 3, 6}), parameters, new object[] {1, 0, 0}),
new Pupil("Sackmann Jan", GetProjects(projects, new int[] {3, 5, 6}), parameters, new object[] {1, 0, 0}),
new Pupil("Satow Felix", GetProjects(projects, new int[] {5, 2, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Schaer Evan", GetProjects(projects, new int[] {5, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Schafaczek Janine", GetProjects(projects, new int[] {8, 10, 9}), parameters, new object[] {0, 0, 0}),
new Pupil("Schäfer Mareike", GetProjects(projects, new int[] {11, 10, 8}), parameters, new object[] {1, 0, 0}),
new Pupil("Schläger Gesa Mareike", GetProjects(projects, new int[] {3, 6, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Schmitz Ferdinand", GetProjects(projects, new int[] {10, 8, 9}), parameters, new object[] {0, 0, 0}),
new Pupil("Schultz Sina", GetProjects(projects, new int[] {9, 7, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Schwarz Noah ", GetProjects(projects, new int[] {6, 3, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Siebert Janine", GetProjects(projects, new int[] {2, 4, 11}), parameters, new object[] {1, 0, 0}),
new Pupil("Stangl Paul Hanns", GetProjects(projects, new int[] {9, 11, 8}), parameters, new object[] {1, 1, 0}),
new Pupil("Strüber Anna", GetProjects(projects, new int[] {11, 8, 9}), parameters, new object[] {0, 0, 0}),
new Pupil("Sulis Caterina", GetProjects(projects, new int[] {5, 2, 6}), parameters, new object[] {1, 0, 0}),
new Pupil("Taaks Timo", GetProjects(projects, new int[] {5, 6, 2}), parameters, new object[] {0, 0, 0}),
new Pupil("Teichmüller Malte", GetProjects(projects, new int[] {3, 5, 6}), parameters, new object[] {0, 1, 0}),
new Pupil("Tergau Tonia Katharina", GetProjects(projects, new int[] {3, 6, 5}), parameters, new object[] {1, 1, 0}),
new Pupil("Theiler Eva Henrike", GetProjects(projects, new int[] {11, 8, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Ton Michael", GetProjects(projects, new int[] {6, 4, 3}), parameters, new object[] {0, 0, 0}),
new Pupil("Tschentscher Klara Henriette", GetProjects(projects, new int[] {3, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Vorwerk Lukas", GetProjects(projects, new int[] {10, 8, 11}), parameters, new object[] {0, 0, 0}),
new Pupil("Wegener Anna Katharina Luise", GetProjects(projects, new int[] {2, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Wehr Jannis", GetProjects(projects, new int[] {5, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Wende Jilin", GetProjects(projects, new int[] {8, 7, 9}), parameters, new object[] {0, 0, 0}),
new Pupil("Wiedehöft Laura Sophie", GetProjects(projects, new int[] {3, 6, 4}), parameters, new object[] {1, 1, 0}),
new Pupil("Wiegner Ellen", GetProjects(projects, new int[] {7, 8, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Wiermann Hannah", GetProjects(projects, new int[] {6, 2, 5}), parameters, new object[] {0, 0, 0}),
new Pupil("Wilkening Leona", GetProjects(projects, new int[] {2, 3, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Wittorf Anna Maria", GetProjects(projects, new int[] {7, 11, 10}), parameters, new object[] {0, 0, 0}),
new Pupil("Wortmann Rafael Bennet", GetProjects(projects, new int[] {4, 5, 2}), parameters, new object[] {0, 1, 0}),
new Pupil("Zoller Gideon", GetProjects(projects, new int[] {5, 2, 6}), parameters, new object[] {0, 0, 0}),
new Pupil("Zubarev Julian", GetProjects(projects, new int[] {5, 4, 6}), parameters, new object[] {1, 1, 0})


            };


            //Console.ReadKey();

            PupilProjectPlanner.PPP planner = new PupilProjectPlanner.PPP();

                var result = planner.Assign(projects, pupils, true, out int[] results, parameters);
                /*
                foreach (var project in projects)
                {
                    Console.WriteLine(project.DetailedToString());
                }
                Console.WriteLine();
                foreach (var item in result)
                {
                    Console.WriteLine(item.Name);
                }
                Console.WriteLine("\n" + results[0] + " : " + results[1] + " : " + results[2] + " | " + (results[0] + results[1] + results[2]) + " / " + pupils.Length);
                */
                statistic[0] += results[0];
                statistic[1] += results[1];
                statistic[2] += results[2];
                statistic[3] += (results[0] + results[1] + results[2]);
            }
            Console.WriteLine("---------");
            Console.WriteLine(statistic[0] / 1000 + " : " + statistic[1] / 1000 + " : " + statistic[2] / 1000 + " | " + statistic[3] / 1000);
            Console.ReadKey();
        }

        private static Project[] GetProjects(Project[] projects, int[] v)
        {
            Project[] r = new Project[v.Length];

            for (int i = 0; i < v.Length; i++)
            {
                if (v[i] > 0)
                    r[i] = projects[v[i] - 1];
                else
                    r[i] = null;
            }

            return r;
        }
    }
}
