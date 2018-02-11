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
    public class Pupil
    {
        private string name;
        private Project[] choices;

        private byte[][] parameters;

        private float choicePriority;

        private Project project;
        
        /// <summary>
        /// 
        /// </summary>
        public string Name { get => name; set => name = value; }
        /// <summary>
        /// 
        /// </summary>
        public Project[] Choices { get => choices; set => choices = value; }
        /// <summary>
        /// 
        /// </summary>
        public float ChoicePriority {
            get
            {
                //return choicePriority;
                float pupilPriority = 0;
                for (int i = 0; i < choices.Length; i++)
                {
                    if (Choices[i] == null)
                        pupilPriority += 1337;
                    else
                        pupilPriority += (float)(Choices[i].Priority / Math.Pow(2, i));
                }
                return pupilPriority;
            }
            set => choicePriority = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="preferation"></param>
        /// <returns></returns>
        public float GetPriority(float defaultValue, bool preferation = true)
        {
            if (preferation && (Name.Contains("Ehrmuth") || Name.Contains("Rostalski")))
            {
                return float.MaxValue;
            }

            float pupilPriority = 0;
            for (int i = 0; i < choices.Length; i++)
            {
                if (Choices[i] == null)
                    pupilPriority += defaultValue;
                else
                    pupilPriority += (float)(Choices[i].Priority / Math.Pow(2, i));
            }
            return pupilPriority;
        }

        /// <summary>
        /// 
        /// </summary>
        public Project Project { get => project; set => project = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte[] GetParameter(int index)
        {
            return parameters[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetParameter(int index, byte[] value)
        {
            parameters[index] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="parameters"></param>
        public void SetParameter(int index, object value, Parameter[] parameters)
        {
            this.parameters[index] = parameters[index].GetBytes(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public void SetParameter(Parameter parameter, object value)
        {
            parameters[parameter.Index] = parameter.GetBytes(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="parameterValues"></param>
        public void SetParameters(Parameter[] parameters, object[] parameterValues)
        {
            if (this.parameters.Length != parameters.Length)
                this.parameters = new byte[parameters.Length][];

            for (int i = 0; i < parameters.Length; i++)
            {
                SetParameter(parameters[i], parameterValues[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Pupil(string name)
        {
            this.name = name;
            choices = new Project[0];
            parameters = new byte[0][];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="choices"></param>
        /// <param name="parameterCount"></param>
        public Pupil(string name, Project[] choices, int parameterCount)
        {
            this.name = name;
            this.choices = choices;
            parameters = new byte[parameterCount][];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="choices"></param>
        /// <param name="parameters"></param>
        public Pupil(string name, Project[] choices, byte[][] parameters)
        {
            this.name = name;
            this.choices = choices;
            this.parameters = parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="choices"></param>
        /// <param name="parameters"></param>
        /// <param name="parameterValues"></param>
        public Pupil(string name, Project[] choices, Parameter[] parameters, object[] parameterValues) : this(name, choices, parameters.Length)
        {
            SetParameters(parameters, parameterValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DetailedToString()
        {
            string result = "Pupil : " + name;
            foreach (var choice in choices)
            {
                result += "\n    " + choice.Name;
            }
            return result;
        }
    }
}
