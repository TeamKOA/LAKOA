using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PupilProjectPlanner
{
    /// <summary>
    /// 
    /// </summary>
    public class PPP
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="pupils"></param>
        /// <param name="randomSeed"></param>
        /// <param name="grantedChoices"></param>
        /// <param name="parameters"></param>
        /// <param name="preferation"></param>
        /// <returns></returns>
        public IEnumerable<Pupil> Assign(IEnumerable<Project> projects, IEnumerable<Pupil> pupils, int randomSeed, bool preferation, out int[] grantedChoices, Parameter[] parameters = null)
        {
            return Assign(projects: projects, pupils: pupils, random: new Random(randomSeed), preferation: preferation, grantedChoices: out grantedChoices, parameters: parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="pupils"></param>
        /// <param name="preferation"></param>
        /// <param name="grantedChoices"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<Pupil> Assign(IEnumerable<Project> projects, IEnumerable<Pupil> pupils, bool preferation, out int[] grantedChoices, Parameter[] parameters = null)
        {
            return Assign(projects: projects, pupils: pupils, random: new Random(), preferation: preferation, grantedChoices: out grantedChoices, parameters: parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="pupils"></param>
        /// <param name="parameters"></param>
        /// <param name="preferation"></param>
        /// <returns></returns>
        public IEnumerable<Pupil> Assign(IEnumerable<Project> projects, IEnumerable<Pupil> pupils, bool preferation = true, Parameter[] parameters = null)
        {
            return Assign(projects, pupils, preferation, out int[] temp, parameters);
        }

        private IEnumerable<Pupil> Assign(IEnumerable<Project> projects, IEnumerable<Pupil> pupils, Random random, bool preferation, out int[] grantedChoices, Parameter[] parameters = null)
        {
            List<Pupil> remaining = new List<Pupil>();
            int choiceCount = 0;

            Parallel.ForEach(pupils, (Pupil pupil) =>
            {
                if (pupil.Choices.Length <= 0)
                    throw new ArgumentException("The pupil " + pupil.ToString() + " does not have any choices.");

                if (choiceCount == 0)
                    choiceCount = pupil.Choices.Length;
                else if (pupil.Choices.Length != choiceCount)
                    throw new ArgumentException("The pupils have inconsistent numbers of choices.");

                Project project = null;

                for (int i = choiceCount - 1; i >= 0; --i)
                {
                    project = pupil.Choices[i];

                    if (project != null)
                    {
                        if ((parameters == null || project.Condition.DoesFulfill(pupil, parameters)))
                        {
                            lock (project)
                            {
                                if (project.InterestedCount == null)
                                    project.InterestedCount = new int[choiceCount];

                                project.InterestedCount[i]++;

                                if (project.Interested == null)
                                {
                                    project.Interested = new List<Pupil>[choiceCount];
                                    for (int j = 0; j < choiceCount; j++)
                                    {
                                        project.Interested[j] = new List<Pupil>();
                                    }
                                }
                                project.Interested[i].Add(pupil);
                            }
                        }
                        else
                            pupil.Choices[i] = null;
                    }
                }


                for (int i = 0; i < choiceCount; i++)
                {
                }
                lock (remaining)
                {
                    remaining.Add(pupil);
                }
            });
            grantedChoices = new int[choiceCount];
            int[] granted = new int[choiceCount];

            Parallel.ForEach(projects, (Project project) => 
            //foreach (var project in projects)
            {
                if (project.InterestedCount == null)
                    project.InterestedCount = new int[choiceCount];

                if (project.Interested == null)
                {
                    project.Interested = new List<Pupil>[choiceCount];
                    for (int j = 0; j < choiceCount; j++)
                    {
                        project.Interested[j] = new List<Pupil>();
                    }
                }

                float interestValue = 0;

                for (int i = 0; i < choiceCount; i++)
                {
                    interestValue += (float)(project.InterestedCount[i] / Math.Pow(2, i));
                }

                project.Priority = interestValue - project.MaxMemberCount - Math.Max(0, project.MinMemberCount - interestValue);
                //project.Priority = Math.Max(0, interestValue - project.MaxMemberCount) - Math.Max(0, project.MinMemberCount - interestValue);
            });
            //bool generalActivity;
            //do
            //{
            Parallel.ForEach(remaining, (Pupil pupil) =>
            {
                float pupilPriority = 0;

                for (int i = 0; i < choiceCount; i++)
                {
                    if (pupil.Choices[i] == null)
                        pupilPriority += remaining.Count;
                    else
                        pupilPriority += (float)(pupil.Choices[i].Priority / Math.Pow(2, i));
                }
                if (preferation && (pupil.Name.Contains("Ehrmuth") || pupil.Name.Contains("Rostalski")))
                {
                    pupilPriority = float.MaxValue;
                }
                //else
                //pupilPriority /= choiceCount;

                pupil.ChoicePriority = pupilPriority;
                /*
                for (int i = 0; i < choiceCount; i++)
                {
                    project = projects[pupil.Choices[i]];
                    if (pupilPriority >= project.Priority)
                    {
                        project.Members.Add(pupil);
                        break;
                    }
                }*/
            });

            PriorityComparer comparer = new PriorityComparer(random, remaining.Count, preferation);

            //for (int choice = 0; choice < choiceCount; choice++)
            //{
            //    foreach (var project in projects)
            //    {
            //        foreach (Pupil pupil in PupilSort(temp, project, comparer))
            //        {
            //            if (project.IsFull)
            //                break;

            //            if (remaining.Contains(pupil))
            //            {
            //                project.Members.Add(pupil);
            //                pupil.Project = project;
            //                remaining.Remove(pupil);
            //                grantedChoices[choice]++;
            //            }
            //        }
            //    }
            //}

            bool generalActivity;
            do
            {
                Parallel.ForEach(projects, (Project project) =>
                {
                    for (int i = 0; i < choiceCount; i++)
                    {
                        project.Interested[i] = new List<Pupil>(project.Interested[i].OrderBy(pupil => pupil, comparer));
                    }
                });

                //bool generalActivity;
                //do
                //{
                generalActivity = false;
                for (int choice = 0; choice < choiceCount; choice++)
                {
                    bool activity;
                    do
                    {
                        activity = false;

                        foreach (var project in projects)
                        //Parallel.ForEach(projects, (Project project) =>
                        {
                            int[] indicies = new int[choiceCount];

                            while (true)
                            {
                                List<Pupil> buffer = project.Interested[choice];

                                if (++indicies[0] > buffer.Count)
                                    break;

                                Pupil pupil = buffer[buffer.Count - indicies[0]];

                                if (!remaining.Contains(pupil))
                                {
                                    buffer.RemoveAt(buffer.Count - 1);
                                    break;
                                }

                                int space = project.FreeSpace;

                                for (int i = choice + 1; i < choiceCount; i++)
                                {
                                    buffer = project.Interested[i];
                                    while (buffer.Count > indicies[i] && pupil.GetPriority(remaining.Count, preferation) * (1 + 1 * i - 0 * i) < buffer[buffer.Count - 1 - indicies[i]].GetPriority(remaining.Count, preferation))
                                    {
                                        indicies[i]++;
                                    }
                                    //space -= Math.Max(0, (int)(indicies[i] - 1 * i));// / Math.Round(Math.Pow(10, i)));
                                    space -= indicies[i];
                                }

                                buffer = project.Interested[choice];

                                if (space > 0)
                                {
                                    buffer.RemoveAt(buffer.Count - 1);
                                    project.Members.Add(pupil);
                                    pupil.Project = project;
                                    lock (remaining)
                                    {
                                        remaining.Remove(pupil);
                                    }
                                    granted[choice]++;
                                    activity = true;
                                }
                                else
                                    break;
                            }
                        }//);
                        if (!generalActivity && activity)
                            generalActivity = true;
                    }
                    while (activity);
                }
            }
            while (generalActivity);

            grantedChoices = granted;

            return remaining;
            //if (remaining.Count > 0)
            //    return false;

            //foreach (var project in projects.Values)
            //{
            //    if (project.IsUnderpopulated)
            //        return false;
            //}

            //return true;
        }

        private IEnumerable<T> Sort<T>(ICollection<T> collection)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Pupil> PupilSort(IEnumerable<Pupil> pupils, Project pro, IComparer<float> com)
        {
            pupils.OrderByDescending<Pupil, float>((pupil) =>
            {
                for (int i = 0; i < 3; i++)
                {
                    if (pupil.Choices[i] == pro)
                        return (float)(pupil.ChoicePriority / Math.Pow(2, i));
                }
                return 9000f;
            }, com);
            return from pupil in pupils
                   orderby pupil.ChoicePriority descending
                   select pupil;
        }

        private struct PriorityComparer : IComparer<Pupil>, IComparer<float>
        {
            Random random;
            bool preferation;
            float defaultValue;

            public int Compare(Pupil x, Pupil y)
            {
                float pX = x.GetPriority(defaultValue, preferation);
                float pY = y.GetPriority(defaultValue, preferation);

                if (x == null || y == null || pX == pY)
                    return random.Next(0, 2) * 2 - 1;
                else if (pX > pY)
                    return 1;
                
                return -1;
            }

            public int Compare(float x, float y)
            {
                if (x > y)
                    return 1;
                else if (x == y)
                    return random.Next(0, 2) * 2 - 1;

                return -1;
            }

            public PriorityComparer(Random random, float defaultValue, bool preferation = true)
            {
                this.random = random;
                this.defaultValue = defaultValue;
                this.preferation = preferation;
            }
        }
    }
}
