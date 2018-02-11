using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PupilProjectPlanner;
using BetterSerialization;

namespace UserInterface
{

    class SaveContainer : ISerializable
    {
        public Dictionary<string, Project> projectDict = new Dictionary<string, Project>();
        //public Dictionary<string, List<Condition>> projectConditions = new Dictionary<string, List<Condition>>();
        public Dictionary<string, Parameter> parameterDict = new Dictionary<string, Parameter>();

        public SaveContainer(Dictionary<string, Project> _projectDict, /*Dictionary<string, List<Condition>> _projectConditions,*/ Dictionary<string, Parameter> _parameterDict)
        {
            parameterDict = _parameterDict;
     //       projectConditions = _projectConditions;
            parameterDict = _parameterDict;
        }

        public Type SerializationType => throw new NotImplementedException();

        public uint SerializationDataLenth => throw new NotImplementedException();

        public void Populate(byte[] data, Deserializer deserializer)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize(Serializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
