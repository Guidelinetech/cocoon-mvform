using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using cocoon.mvform.attributes;
using cocoon.mvform.bindings;

namespace cocoon.mvform
{
    public class ModelViewBinder<T>
    {

        internal Type modelType;
        internal PropertyInfo[] modelProps;
        internal Dictionary<Control, PropertyInfo> modelFields = new Dictionary<Control, PropertyInfo>();
        internal Dictionary<Control, object> dataSources = new Dictionary<Control, object>();
        internal Dictionary<Type, ModelControlBinding> bindings = new Dictionary<Type, ModelControlBinding>();

        public ModelViewBinder(Control view, List<Type> ignoredControlTypes = null)
        {

            //add default implementations
            SetModelControlBinding(new TextBoxBinding());
            SetModelControlBinding(new CheckBoxBinding());
            SetModelControlBinding(new ComboBoxBinding());
            SetModelControlBinding(new ListBoxBinding());
            SetModelControlBinding(new NumericUpDownBinding());
            SetModelControlBinding(new DateTimePickerBinding());

            //get model and view info
            modelType = typeof(T);
            modelProps = modelType.GetProperties();

            //process properties and controls
            ProcessView(view, ignoredControlTypes);

        }

        public void ProcessView(Control[] views, List<Type> ignoredControlTypes = null)
        {

            foreach (Control view in views)
                ProcessView(view, ignoredControlTypes);

        }

        public void ProcessView(Control view, List<Type> ignoredControlTypes = null)
        {

            if (ignoredControlTypes == null)
                ignoredControlTypes = new List<Type>() { typeof(Label), typeof(Button), typeof(PictureBox) };

            foreach (PropertyInfo prop in modelProps)
            {

                DataSource dataSourceAttribute = prop.GetCustomAttribute<DataSource>(true);
                ValueFor valueForAttribute = prop.GetCustomAttribute<ValueFor>(true);

                if (valueForAttribute != null)
                {
                    modelFields.Add(valueForAttribute.valueForControl, prop);

                    if (dataSourceAttribute != null)
                        dataSources.Add(valueForAttribute.valueForControl, dataSourceAttribute.dataSource);

                }
                else
                    foreach (Control control in view.Controls)
                    {

                        if (ignoredControlTypes.Contains(control.GetType()))
                            continue;

                        if (control.Name == prop.Name || (control.Tag is string && (string)control.Tag == prop.Name) || control.Name == prop.Name + control.GetType().Name)
                        {
                            modelFields.Add(control, prop);

                            if (dataSourceAttribute != null)
                                dataSources.Add(control, dataSourceAttribute.dataSource);

                            break;

                        }
                    }

            }

        }

        public void AddDataSources(object dataSourcesObject)
        {
            
            PropertyInfo[] props = dataSourcesObject.GetType().GetProperties();

            foreach (PropertyInfo prop in props)
                foreach (var field in modelFields)
                {
                    Control control = field.Key;
                    if (control.Name == prop.Name || (control.Tag is string && (string)control.Tag == prop.Name) || control.Name == prop.Name + control.GetType().Name)
                        dataSources.Add(field.Key, prop.GetValue(dataSourcesObject));
                }

        }

        public void UpdateView(T model)
        {
            
            //update datasources
            foreach (var data in dataSources)
            {

                Control control = data.Key;
                object dataSource = data.Value;

                if (bindings.ContainsKey(control.GetType()))
                    try
                    {
                        bindings[control.GetType()].UpdateDataSource(control, dataSource);
                    }
                    catch
                    {
                    }
                else
                    throw new NotImplementedException(string.Format("Binding for type '{0}' not implemented.", control.GetType()));

            }

            //update fields
            foreach (var field in modelFields)
            {

                Control control = field.Key;
                PropertyInfo prop = field.Value;

                if (bindings.ContainsKey(control.GetType()))
                    try
                    {
                        bindings[control.GetType()].UpdateControl(control, prop.GetValue(model));
                    }
                    catch
                    {
                    }
                else
                    throw new NotImplementedException(string.Format("Binding for type '{0}' not implemented.", control.GetType()));

            }

        }

        public T UpdateModel(T model, bool includeInvisibleControls = true)
        {

            //update model fields
            foreach (var field in modelFields)
            {

                Control control = field.Key;
                PropertyInfo prop = field.Value;

                if (!includeInvisibleControls && !control.Visible)
                    continue;

                if (bindings.ContainsKey(control.GetType()))
                {
                    object value = bindings[control.GetType()].UpdateModel(control);
                    prop.SetValue(model, ModelControlBinding.ChangeType(value, prop.PropertyType));
                }
                else
                    throw new NotImplementedException(string.Format("Binding for type '{0}' not implemented.", control.GetType()));

            }

            return model;

        }

        public void SetModelControlBinding(ModelControlBinding binding)
        {

            if (bindings.ContainsKey(binding.ControlType))
                bindings[binding.ControlType] = binding;
            else
                bindings.Add(binding.ControlType, binding);

        }

    }

}
