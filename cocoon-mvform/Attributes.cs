using System;
using System.Windows.Forms;

namespace cocoon.mvform.attributes
{
    public class DataSource : Attribute
    {

        internal object dataSource;

        public DataSource(object dataSource)
        {

            this.dataSource = dataSource;

        }

    }

    public class ValueFor : Attribute
    {

        internal Control valueForControl;

        public ValueFor(Control control)
        {

            valueForControl = control;

        }

    }
    
}
