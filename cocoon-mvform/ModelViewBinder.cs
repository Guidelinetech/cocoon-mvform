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

        internal T model;
        internal Type modelType;
        internal PropertyInfo[] modelProps;
        internal ContainerControl view;
        internal Dictionary<Control, PropertyInfo> modelFields = new Dictionary<Control, PropertyInfo>();
        internal Dictionary<Control, object> dataSources = new Dictionary<Control, object>();

        internal Dictionary<Type, ModelControlBinding> bindings = new Dictionary<Type, ModelControlBinding>();

        public ModelViewBinder(ContainerControl view, T model)
        {

            //add default implementations
            SetModelControlBinding(typeof(TextBox), new TextBoxBinding());
            SetModelControlBinding(typeof(CheckBox), new CheckBoxBinding());
            SetModelControlBinding(typeof(ComboBox), new ComboBoxBinding());
            SetModelControlBinding(typeof(ListBox), new ListBoxBinding());

            //get model and view info
            this.model = model;
            this.view = view;

            modelType = typeof(T);
            modelProps = modelType.GetProperties();

            //process properties and controls
            foreach (PropertyInfo prop in modelProps)
            {

                var dataSourceAttribute = prop.GetCustomAttribute<DataSource>(true);
                var valueForAttribute = prop.GetCustomAttribute<ValueFor>(true);

                if (valueForAttribute != null)
                {
                    modelFields.Add(valueForAttribute.valueForControl, prop);

                    if (dataSourceAttribute != null)
                        dataSources.Add(valueForAttribute.valueForControl, dataSourceAttribute.dataSource);

                }
                else
                    foreach (Control control in view.Controls)
                        if (control.Name.StartsWith(prop.Name))
                        {
                            modelFields.Add(control, prop);

                            if (dataSourceAttribute != null)
                                dataSources.Add(control, dataSourceAttribute.dataSource);

                        }

            }

            //update view with model
            UpdateView();

        }

        public void UpdateView()
        {

            //update datasources
            foreach (var data in dataSources)
            {

                Control control = data.Key;
                object dataSource = data.Value;

                if (bindings.ContainsKey(control.GetType()))
                    bindings[control.GetType()].UpdateDataSource(control, dataSource);
                else
                    throw new NotImplementedException(string.Format("Binding for type '{0}' not implemented.", control.GetType()));

            }

            //update fields
            foreach (var field in modelFields)
            {

                Control control = field.Key;
                PropertyInfo prop = field.Value;

                if (bindings.ContainsKey(control.GetType()))
                    bindings[control.GetType()].UpdateControl(control, prop.GetValue(model));
                else
                    throw new NotImplementedException(string.Format("Binding for type '{0}' not implemented.", control.GetType()));

            }
        }

        public T UpdateModel()
        {

            //update model fields
            foreach (var field in modelFields)
            {

                Control control = field.Key;
                PropertyInfo prop = field.Value;

                if (bindings.ContainsKey(control.GetType()))
                    prop.SetValue(model, bindings[control.GetType()].UpdateModel(control));
                else
                    throw new NotImplementedException(string.Format("Binding for type '{0}' not implemented.", control.GetType()));

            }

            return model;

        }

        public void SetModelControlBinding(Type controlType, ModelControlBinding binding)
        {

            if (bindings.ContainsKey(controlType))
                bindings[controlType] = binding;
            else
                bindings.Add(controlType, binding);

        }
        
    }


    
}
