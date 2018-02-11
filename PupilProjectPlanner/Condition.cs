using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PupilProjectPlanner
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pupil"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        bool DoesFulfill(Pupil pupil, Parameter[] parameters);
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Condition : ICondition
    {
        private Types type;

        byte parameterA;
        byte[] parameterB;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private Condition(Types type, byte a, byte[] b)
        {
            this.type = type;
            parameterA = a;
            parameterB = b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Condition(Types type, byte a, int b) : this(type, a, BitConverter.GetBytes(b)) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Condition(Types type, byte a, string b) : this(type, a, Encoding.Unicode.GetBytes(b)) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Condition(Types type, byte a, byte b) : this(type, a, new byte[] { b }) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Condition(Types type, Parameter a, int b) : this(type, (byte)a.Index, BitConverter.GetBytes(b)) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Condition(Types type, Parameter a, string b) : this(type, (byte)a.Index, Encoding.Unicode.GetBytes(b)) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Condition(Types type, Parameter a, Parameter b) : this(type, (byte)a.Index, new byte[] { (byte)b.Index }) { }

        /// <summary>
        /// 
        /// </summary>
        public byte ParameterAID
        {
            get { return parameterA; }
            set { parameterA = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Types Type { get => type; set => type = value; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] ParameterB { get => parameterB; set => parameterB = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pupil"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool DoesFulfill(Pupil pupil, Parameter[] parameters)
        {
            int result;

            if (parameterB.Length == 1)
            {
                Parameter paramA = parameters[parameterA];
                Parameter paramB = parameters[parameterB[0]];

                result = paramA.Compare(pupil.GetParameter(paramA.Index), pupil.GetParameter(paramB.Index));
            }
            else
            {
                Parameter paramA = parameters[parameterA];

                result = paramA.Compare(pupil.GetParameter(paramA.Index), parameterB);
            }

            switch (type)
            {
                case Types.Equal:
                    return result == 0;
                case Types.UnEqual:
                    return result != 0;
                case Types.Greater:
                    return result == 1;
                case Types.Less:
                    return result == -1;
                default:
                    throw new InvalidOperationException("No type set!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Types : byte
        {
            /// <summary>
            /// 
            /// </summary>
            Equal,
            /// <summary>
            /// 
            /// </summary>
            UnEqual,
            /// <summary>
            /// 
            /// </summary>
            Greater,
            /// <summary>
            /// 
            /// </summary>
            Less
        }

        /// <summary>
        /// 
        /// </summary>
        public static ICondition None => new NoCondition();

        private struct NoCondition : ICondition
        {
            public bool DoesFulfill(Pupil pupil, Parameter[] parameters)
            {
                return true;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct ConditionPool : ICondition, IEnumerable<ICondition>
    {
        private ICondition[] conditions;

        private bool isOr;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="isOr"></param>
        public ConditionPool(ICondition[] conditions, bool isOr = false)
        {
            this.conditions = conditions;
            this.isOr = isOr;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOr { get => isOr; set => isOr = value; }

        /// <summary>
        /// 
        /// </summary>
        public ICondition[] Conditions { get => conditions; set => conditions = value; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pupil"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool DoesFulfill(Pupil pupil, Parameter[] parameters)
        {
            foreach (var condition in conditions)
            {
                if (condition.DoesFulfill(pupil, parameters))
                {
                    if (isOr)
                        return true;
                }
                else if (!isOr)
                    return false;
            }

            return !isOr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ICondition> GetEnumerator()
        {
            return ((IEnumerable<ICondition>)conditions).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ICondition>)conditions).GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        public static implicit operator ConditionPool(ICondition[] conditions)
        {
            return new ConditionPool(conditions);
        }
    }
}
