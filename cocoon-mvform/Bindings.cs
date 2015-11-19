using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace cocoon.mvform.bindings
{

    public abstract class ModelControlBinding
    {

        public abstract void UpdateControl(Control control, object value);

        public abstract object UpdateModel(Control control);

        public abstract void UpdateDataSource(Control control, object value);

        public static object ChangeType(object value, Type conversionType)
        {

            if (value == null)
                if (conversionType.IsValueType)
                    return Activator.CreateInstance(conversionType);
                else
                    return null;

            if (value.GetType() == conversionType)
                return value;

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                conversionType = Nullable.GetUnderlyingType(conversionType);

            try
            {
                return TypeDescriptor.GetConverter(conversionType).ConvertFrom(value);
            }
            catch
            {
                return Convert.ChangeType(value, conversionType);

            }

        }

    }

    internal class TextBoxBinding : ModelControlBinding
    {
        public override void UpdateControl(Control control, object value)
        {
            ((TextBox)control).Text = (string)ChangeType(value, typeof(string));
        }

        public override void UpdateDataSource(Control control, object value)
        {
            throw new NotImplementedException();
        }

        public override object UpdateModel(Control control)
        {
            return ((TextBox)control).Text;
        }
    }

    internal class ComboBoxBinding : ModelControlBinding
    {
        public override void UpdateControl(Control control, object value)
        {
            ((ComboBox)control).SelectedItem = value;
        }

        public override void UpdateDataSource(Control control, object value)
        {
            ((ComboBox)control).DataSource = value;
        }

        public override object UpdateModel(Control control)
        {
            return ((ComboBox)control).SelectedItem;
        }
    }

    internal class ListBoxBinding : ModelControlBinding
    {
        public override void UpdateControl(Control control, object value)
        {
            ((ListBox)control).SelectedItem = value;
        }

        public override void UpdateDataSource(Control control, object value)
        {
            ((ListBox)control).DataSource = value;
        }

        public override object UpdateModel(Control control)
        {
            return ((ListBox)control).SelectedItem;
        }
    }

    internal class CheckBoxBinding : ModelControlBinding
    {
        public override void UpdateControl(Control control, object value)
        {
            ((CheckBox)control).Checked = (bool)ChangeType(value, typeof(bool));
        }

        public override void UpdateDataSource(Control control, object value)
        {
            throw new NotImplementedException();
        }

        public override object UpdateModel(Control control)
        {
            return ((CheckBox)control).Checked;
        }
    }

    internal class NumericUpDownBinding : ModelControlBinding
    {
        public override void UpdateControl(Control control, object value)
        {
            ((NumericUpDown)control).Value = (decimal)ChangeType(value, typeof(decimal));
        }

        public override void UpdateDataSource(Control control, object value)
        {
            throw new NotImplementedException();
        }

        public override object UpdateModel(Control control)
        {
            return ((NumericUpDown)control).Value;
        }
    }

    internal class DateTimePickerBinding : ModelControlBinding
    {
        public override void UpdateControl(Control control, object value)
        {
            ((DateTimePicker)control).Value = (DateTime)ChangeType(value, typeof(DateTime));
        }

        public override void UpdateDataSource(Control control, object value)
        {
            throw new NotImplementedException();
        }

        public override object UpdateModel(Control control)
        {
            return ((DateTimePicker)control).Value;
        }
    }

}
