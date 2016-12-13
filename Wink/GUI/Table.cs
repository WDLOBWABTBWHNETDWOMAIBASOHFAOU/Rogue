using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class Table : GameObject
    {
        class TableRowValuesIncompatibleException : Exception
        {
        }

        //I know very dirty, but there doesn't seem to be a good way to store Func objects with unkown parameter Type.
        List<Func<object, string>> columnTypes;

        List<List<object>> values;

        public Table()
        {
            columnTypes = new List<Func<object, string>>();
            values = new List<List<object>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">The Type of objects that will be in this column.</param>
        /// <param name="f">The Func that will describe how to extract the string to display.</param>
        public void AddColumn<T>(Func<T, string> f) where T : class
        {
            columnTypes.Add((Func<object, string>)f);
        }

        /// <summary>
        /// .ToString() will be used to extract the string that will be displayed.
        /// </summary>
        /// <param name="t">The Type of objects that will be in this column.</param>
        public void AddColumn<T>() where T : class
        {
            AddColumn<T>(obj => obj.ToString());
        }

        public void AddRow(List<object> values)
        {
            if (values.Count != columnTypes.Count)
            {
                throw new TableRowValuesIncompatibleException();
            }
            
            for (int i = 0; i < values.Count; i++)
            {
                ParameterInfo parameter = columnTypes[i].Method.GetParameters()[0];
                Type t = parameter.ParameterType;
                if (!values[i].GetType().Equals(t))
                {
                    throw new TableRowValuesIncompatibleException();
                }
            }

            this.values.Add(values);
        }
    }
}
