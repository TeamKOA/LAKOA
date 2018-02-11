using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserInterface
{
    class ConditionManager
    {

        static List<string> conditions = new List<string>();

        /// <summary>
        /// Adds a condition
        /// </summary>
        /// <param name="name">Name of condition</param>
        public static void AddCondition(string name)
        {
            conditions.Add(name);
            conditions.Sort();
        }
        /// <summary>
        /// Removes a condition
        /// </summary>
        /// <param name="name">Name of condition</param>
        public static void RemoveCondition(string name)
        {
            conditions.Remove(name);
        }

        public static bool Contains(string name)
        {
            return conditions.Contains(name);
        }
        
        public static string GetConditionsAsString()
        {           
            string output = "";
            foreach(string s in conditions)
            {
                output += s + Environment.NewLine;
            }

            return output;
        }

        public static void SetCBItems(ComboBox combobox)
        {
            combobox.Items.Clear();
            string[] a = conditions.ToArray();

            for (int i = 0; i < conditions.Count; i++)
            {
                combobox.Items.Add(a[i]);
                
            }        

        }
    }
}
