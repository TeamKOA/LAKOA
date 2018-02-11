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
    public abstract class Parameter : IComparer<byte[]>
    {
        private int index;
        private string name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public Parameter(int index)
        {
            this.index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Parameter CreateParameter(int index, DataTypes type)
        {
            switch (type)
            {
                case DataTypes.Int:
                    return new ParameterInt(index)
                        ;
                case DataTypes.String:
                    return new ParameterString(index);

                default:
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract DataTypes DataType { get; }

        /// <summary>
        /// The index of the corresponding data of each pupil
        /// </summary>
        public int Index { get => index; set => index = value; }
                              
        /// <summary>
        /// 
        /// </summary>
        public string Name { get => name; set => name = value; }
        
        /// <summary>
        /// 
        /// </summary>
        public enum DataTypes : byte
        {
            /// <summary>
            /// A numeric value
            /// </summary>
            Int,
            /// <summary>
            /// A string
            /// </summary>
            String
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract int Compare(byte[] x, byte[] y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract byte[] GetBytes(object value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Parameter[] CreateParameters(params DataTypes[] types)
        {
            Parameter[] parameters = new Parameter[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                switch (types[i])
                {
                    case DataTypes.Int:
                        parameters[i] = new ParameterInt(i);
                        break;

                    case DataTypes.String:
                        parameters[i] = new ParameterString(i);
                        break;

                    default:
                        throw new NotImplementedException("The parameter data type " + types[i] + " has not been implemented into the CreateParameters method.");
                }
            }

            return parameters;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParameterInt : Parameter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public ParameterInt(int index) : base(index)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override DataTypes DataType => DataTypes.Int;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(byte[] x, byte[] y)
        {
            return BitConverter.ToInt32(x, 0) - BitConverter.ToInt32(y, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override byte[] GetBytes(object value)
        {
            return GetBytes((int)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public int GetValue(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParameterString : Parameter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public ParameterString(int index) : base(index)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override DataTypes DataType => DataTypes.String;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(byte[] x, byte[] y)
        {
            return Encoding.ASCII.GetString(x).CompareTo(Encoding.ASCII.GetString(y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override byte[] GetBytes(object value)
        {
            return GetBytes((int)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string GetValue(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
