using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PupilProjectPlanner
{
    /// <summary>
    /// 
    /// </summary>
    public class Project
    {
        private string name;
        private int maxMemberCount;
        private int minMemberCount;

        private ICondition condition = PupilProjectPlanner.Condition.None;

        private List<Pupil> members;

        private List<Pupil>[] interested;
        private int[] interestedCount;
        private float priority;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxMemberCount"></param>
        /// <param name="minMemberCount"></param>
        public Project(string name, int maxMemberCount, int minMemberCount = 0)
        {
            this.name = name;
            this.maxMemberCount = maxMemberCount;
            this.minMemberCount = minMemberCount;

            members = new List<Pupil>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get => name; set => name = value; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxMemberCount { get => maxMemberCount; set => maxMemberCount = value; }
        /// <summary>
        /// 
        /// </summary>
        public int MinMemberCount { get => minMemberCount; set => minMemberCount = value; }

        /// <summary>
        /// 
        /// </summary>
        public List<Pupil> Members { get => members; set => members = value; }

        /// <summary>
        /// 
        /// </summary>
        public int FreeSpace
        {
            get { return Math.Max(maxMemberCount - members.Count, 0); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFull
        {
            get { return members.Count >= maxMemberCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOverpopulated
        {
            get { return members.Count > maxMemberCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUnderpopulated
        {
            get { return members.Count < minMemberCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Overpopulation
        {
            get { return Math.Max(members.Count - maxMemberCount, 0); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int[] InterestedCount { get => interestedCount; set => interestedCount = value; }
        /// <summary>
        /// 
        /// </summary>
        public float Priority {
            get
            {
                //return priority;
                float interestValue = 0;

                for (int i = 0; i < 3; i++)
                {
                    interestValue += (float)(InterestedCount[i] / Math.Pow(2, i));
                }
                return interestValue - MaxMemberCount - Members.Count - Math.Max(0, MinMemberCount + Members.Count - interestValue);
            }
            set => priority = value; }
        /// <summary>
        /// 
        /// </summary>
        public List<Pupil>[] Interested { get => interested; set => interested = value; }
        /// <summary>
        /// 
        /// </summary>
        public ICondition Condition { get => condition; set => condition = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DetailedToString()
        {
            string result = "Project " + " : \"" + name + "\"";
            foreach (var member in members)
            {
                result += "\n    " + member.Name;
            }
            return result;
        }
    }
}
