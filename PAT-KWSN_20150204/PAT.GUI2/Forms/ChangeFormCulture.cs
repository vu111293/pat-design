using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Reflection;
using Fireball.Docking;
using PAT.GUI;

namespace PAT.Common.GUI
{
    /// <summary>
    /// ChangeFormCulture provides methods to change the culture used by a single or all forms in an application
    /// </summary>
    public class ChangeFormCulture
    {
        /// <summary>
        /// ChangeAllForms changes the culture of all existing forms in the application
        /// </summary>
        /// <param name="culture">The culture name to change the forms to</param>
        public static void ChangeAllForms(string culture)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            PAT.Common.Ultility.Ultility.SetCulture(Thread.CurrentThread.CurrentUICulture);
            PAT.GUI.Properties.Resources.Culture = Thread.CurrentThread.CurrentUICulture;

            FormCollection forms = Application.OpenForms;
            for (int i = forms.Count -1; i >= 0 ; i--)
            {
                Form form = forms[i];
                ChangeForm(form, culture);
            }            
        }
        /// <summary>
        /// ChangeForm changes the culture of an existing form by forcing a reload of its resources
        /// </summary>
        /// <param name="form">The form for which the culture should be changed</param>
        /// <param name="culture">The culture name to change the form to</param>
        /// <remarks>This method changes the CurrentUICulture to the given culture</remarks>
        public static void ChangeForm(Form form, string culture)
        {
            ComponentResourceManager resourceManager = new ComponentResourceManager(form.GetType());

            TreeSearchApply(resourceManager, form);
           
            ApplyCultureToForm(resourceManager, form);

            if(form is FormMain)
            {
                TreeSearchApply(resourceManager, (form as FormMain).ContextMenuStrip);
                if ((form as FormMain).Output_Window != null && !(form as FormMain).Output_Window.IsDisposed)
                {
                    ChangeForm((form as FormMain).Output_Window, culture);
                }
                if ((form as FormMain).ErrorListWindow != null && !(form as FormMain).ErrorListWindow.IsDisposed)
                {
                    ChangeForm((form as FormMain).Output_Window, culture);
                }
            }
            else if (form is ResourceFormInterface)
            {
                (form as ResourceFormInterface).InitializeResourceText();
            }
               

            try
            {
                resourceManager.ApplyResources(form, "$this");
                if (form is FormMain)
                {
                    (form as FormMain).Hide();
                    (form as FormMain).Width = (form as FormMain).Width + 1;
                    (form as FormMain).Height = (form as FormMain).Height + 1;
                    (form as FormMain).RepositionToolbar();
                    (form as FormMain).Show();
                    //(form as FormMain).Refresh();
                    //(form as FormMain).Validate();
                }
            }
            catch
            {

            }

        }

        //private static void ApplyMenuResources(ComponentResourceManager resourceManager, Form form)
        //{
        //    if (form.Menu != null)
        //    {
        //        FieldInfo[] fieldInfos = form.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        //        foreach (FieldInfo fieldInfo in fieldInfos)
        //        {
        //            if (fieldInfo.FieldType == typeof(System.Windows.Forms.MenuItem))
        //            {
        //                MenuItem menuItem = (MenuItem)fieldInfo.GetValue(form);
        //                resourceManager.ApplyResources(menuItem, fieldInfo.Name);
        //            }
        //        }
        //    }
        //}


        /// <summary>
        /// Applies localized resources to the specified <see cref="Form"/> object according to the 
        ///   <see cref="Thread.CurrentUICulture"/>.
        /// </summary>
        /// <param name="form">The <see cref="Form"/> object to which changed UI culture should be applied.</param>
        private static void ApplyCultureToForm(ComponentResourceManager resources, Form form)
        {
            try
            {
                // Create a resource manager for the form and determine its fields via reflection.
                // Create and fill a collection, containing all infos needed to apply localized resources.
                //ComponentResourceManager resources = new ComponentResourceManager(form.GetType());
                FieldInfo[] fields = form.GetType().GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly |
                                                              BindingFlags.NonPublic);

                List<ChangeInfo> changeInfos = new List<ChangeInfo>(fields.Length + 1);

                changeInfos.Add(new ChangeInfo("$this", form, form.GetType()));
                for (int index = 0; index < fields.Length; index++)
                {
                    changeInfos.Add(new ChangeInfo(fields[index].Name, fields[index].GetValue(form), fields[index].FieldType));
                }

                changeInfos.TrimExcess();

                // Call SuspendLayout for Form and all fields derived from Control, so assignment of 
                //   localized resources doesn't change layout immediately.
                for (int index = 0; index < changeInfos.Count; index++)
                {
                    if (changeInfos[index].Type.IsSubclassOf(typeof (Control)))
                    {
                        changeInfos[index].Type.InvokeMember("SuspendLayout", BindingFlags.InvokeMethod, null,
                                                             changeInfos[index].Value, null);
                    }
                }

                //if (this.applyText)
                {
                    // If available, assign localized text to Form and fields.
                    for (int index = 0; index < changeInfos.Count; index++)
                    {
                        if (changeInfos[index].Type.GetProperty("Text", typeof (String)) != null &&
                            changeInfos[index].Type.GetProperty("Text", typeof (String)).CanWrite)
                        {
                            String text = resources.GetString(changeInfos[index].Name + ".Text");
                            if (!string.IsNullOrEmpty(text))
                            {
                                changeInfos[index].Type.InvokeMember("Text", BindingFlags.SetProperty, null,
                                                                     changeInfos[index].Value, new object[] {text});
                            }
                        }
                    }
                }

                //if (this.applySize)
                //{
                //    // If available, assign localized sizes to Form and fields.
                //    object size;
                //    Control control;
                //    int index = 0;
                //    if (this.preserveFormSize)
                //    {
                //        // Skip the form entry in changeInfos collection.
                //        index = 1;
                //    }
                //    for (; index < changeInfos.Count; index++)
                //    {
                //        if (changeInfos[index].Type.GetProperty("Size", typeof(Size)) != null &&
                //            changeInfos[index].Type.GetProperty("Size", typeof(Size)).CanWrite)
                //        {
                //            size = resources.GetObject(changeInfos[index].Name + ".Size");
                //            if (size != null && size.GetType() == typeof(Size))
                //            {
                //                if (changeInfos[index].Type.IsSubclassOf(typeof(Control)))
                //                {// In case of an inheritor of Control take into account the Anchor property.
                //                    control = (Control)changeInfos[index].Value;
                //                    if ((control.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right))
                //                    {
                //                        // Control is bound to the left and right edge, so preserve its width.
                //                        size = new Size(control.Width, ((Size)size).Height);
                //                    }
                //                    if ((control.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom))
                //                    {
                //                        // Control is bound to the top and bottom edge, so preserve its height.
                //                        size = new Size(((Size)size).Width, control.Height);
                //                    }
                //                    control.Size = (Size)size;
                //                }
                //                else
                //                {
                //                    changeInfos[index].Type.InvokeMember("Size", BindingFlags.SetProperty, null,
                //                        changeInfos[index].Value, new object[] { size });
                //                }
                //            }
                //        }

                //        if (changeInfos[index].Type.GetProperty("ClientSize", typeof(Size)) != null &&
                //            changeInfos[index].Type.GetProperty("ClientSize", typeof(Size)).CanWrite)
                //        {
                //            size = resources.GetObject(changeInfos[index].Name + ".ClientSize");
                //            if (size != null && size.GetType() == typeof(Size))
                //            {
                //                if (changeInfos[index].Type.IsSubclassOf(typeof(Control)))
                //                {// In case of an inheritor of Control take into account the Anchor property.
                //                    control = (Control)changeInfos[index].Value;
                //                    if ((control.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right))
                //                    {
                //                        // Control is bound to the left and right edge, so preserve the width of its client area.
                //                        size = new Size(control.ClientSize.Width, ((Size)size).Height);
                //                    }
                //                    if ((control.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom))
                //                    {
                //                        // Control is bound to the top and bottom edge, so preserve the height of its client area.
                //                        size = new Size(((Size)size).Width, control.ClientSize.Height);
                //                    }
                //                    control.ClientSize = (Size)size;
                //                }
                //                else
                //                {
                //                    changeInfos[index].Type.InvokeMember("ClientSize", BindingFlags.SetProperty, null,
                //                        changeInfos[index].Value, new object[] { size });
                //                }
                //            }
                //        }
                //    }
                //}

                //if (this.applyLocation)
                //{
                //    // If available, assign localized locations to Form and fields.
                //    object location;
                //    Control control;
                //    int index = 0;
                //    if (this.preserveFormLocation)
                //    {
                //        // Skip the form entry in changeInfos collection.
                //        index = 1;
                //    }
                //    for (; index < changeInfos.Count; index++)
                //    {
                //        if (changeInfos[index].Type.GetProperty("Location", typeof(Point)) != null &&
                //            changeInfos[index].Type.GetProperty("Location", typeof(Point)).CanWrite)
                //        {
                //            location = resources.GetObject(changeInfos[index].Name + ".Location");
                //            if (location != null && location.GetType() == typeof(Point))
                //            {
                //                if (changeInfos[index].Type.IsSubclassOf(typeof(Control)))
                //                {// In case of an inheritor of Control take into account the Anchor property.
                //                    control = (Control)changeInfos[index].Value;
                //                    if ((control.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == AnchorStyles.Right)
                //                    {
                //                        // Control is bound to the right but not the left edge, so preserve its x-coordinate.
                //                        location = new Point(control.Left, ((Point)location).Y);
                //                    }
                //                    if ((control.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == AnchorStyles.Bottom)
                //                    {
                //                        // Control is bound to the bottom but not the top edge, so preserve its y-coordinate.
                //                        location = new Point(((Point)location).X, control.Top);
                //                    }
                //                    control.Location = (Point)location;
                //                }
                //                else
                //                {
                //                    changeInfos[index].Type.InvokeMember("Location", BindingFlags.SetProperty, null,
                //                        changeInfos[index].Value, new object[] { location });
                //                }
                //            }
                //        }
                //    }
                //}

                //if (this.applyRightToLeft)
                //{
                //    // If available, assign localized RightToLeft values to Form and fields.
                //    object rightToLeft;
                //    for (int index = 0; index < changeInfos.Count; index++)
                //    {
                //        if (changeInfos[index].Type.GetProperty("RightToLeft", typeof(RightToLeft)) != null &&
                //            changeInfos[index].Type.GetProperty("RightToLeft", typeof(RightToLeft)).CanWrite)
                //        {
                //            rightToLeft = resources.GetObject(changeInfos[index].Name + ".RightToLeft");
                //            if (rightToLeft != null && rightToLeft.GetType() == typeof(RightToLeft))
                //            {
                //                changeInfos[index].Type.InvokeMember("RightToLeft", BindingFlags.SetProperty, null,
                //                    changeInfos[index].Value, new object[] { rightToLeft });
                //            }
                //        }
                //    }
                //}

                //#if !Prior2
                //            if (this.applyRightToLeftLayout)
                //            {
                //                // If available, assign localized RightToLeftLayout values to Form and fields.
                //                object rightToLeftLayout;
                //                for (int index = 0; index < changeInfos.Count; index++)
                //                {
                //                    if (changeInfos[index].Type.GetProperty("RightToLeftLayout", typeof(bool)) != null &&
                //                        changeInfos[index].Type.GetProperty("RightToLeftLayout", typeof(bool)).CanWrite)
                //                    {
                //                        rightToLeftLayout = resources.GetObject(changeInfos[index].Name + ".RightToLeftLayout");
                //                        if (rightToLeftLayout != null && rightToLeftLayout.GetType() == typeof(bool))
                //                        {
                //                            changeInfos[index].Type.InvokeMember("RightToLeftLayout", BindingFlags.SetProperty, null,
                //                                changeInfos[index].Value, new object[] { rightToLeftLayout });
                //                        }
                //                    }
                //                }
                //            }
                //#endif

                //if (this.applyToolTip)
                {
                    // If available, assign localized ToolTipText to fields.
                    // Also search for a ToolTip component in the current form.
                    ToolTip toolTip = null;
                    for (int index = 1; index < changeInfos.Count; index++)
                    {
                        if (changeInfos[index].Type == typeof (ToolTip))
                        {
                            toolTip = (ToolTip) changeInfos[index].Value;
                            resources.ApplyResources(toolTip, changeInfos[index].Name);
                            changeInfos.Remove(changeInfos[index]);
                        }

                        if (changeInfos[index].Type.GetProperty("ToolTipText", typeof (String)) != null &&
                            changeInfos[index].Type.GetProperty("ToolTipText", typeof (String)).CanWrite)
                        {
                            String text = resources.GetString(changeInfos[index].Name + ".ToolTipText");
                            if (text != null)
                            {
                                changeInfos[index].Type.InvokeMember("ToolTipText", BindingFlags.SetProperty, null,
                                                                     changeInfos[index].Value, new object[] {text});
                            }
                        }
                    }

                    if (toolTip != null)
                    {
                        // Form contains a ToolTip component.
                        // If available, assign localized tooltips to Form and fields.
                        for (int index = 0; index < changeInfos.Count; index++)
                        {
                            if (changeInfos[index].Type.IsSubclassOf(typeof (Control)))
                            {
                                string text = resources.GetString(changeInfos[index].Name + ".ToolTip");
                                if (text != null)
                                {
                                    toolTip.SetToolTip((Control) changeInfos[index].Value, text);
                                }
                            }
                        }
                    }
                }

                //if (this.applyHelp)
                //{
                //    // Search for a HelpProvider component in the current form.
                //    HelpProvider helpProvider = null;
                //    for (int index = 1; index < changeInfos.Count; index++)
                //    {
                //        if (changeInfos[index].Type == typeof(HelpProvider))
                //        {
                //            helpProvider = (HelpProvider)changeInfos[index].Value;
                //            resources.ApplyResources(helpProvider, changeInfos[index].Name);
                //            changeInfos.Remove(changeInfos[index]);
                //            break;
                //        }
                //    }

                //    if (helpProvider != null)
                //    {
                //        // If available, assign localized help to Form and fields.
                //        String text;
                //        object help;
                //        for (int index = 0; index < changeInfos.Count; index++)
                //        {
                //            if (changeInfos[index].Type.IsSubclassOf(typeof(Control)))
                //            {
                //                text = resources.GetString(changeInfos[index].Name + ".HelpKeyword");
                //                if (text != null)
                //                {
                //                    helpProvider.SetHelpKeyword((Control)changeInfos[index].Value, text);
                //                }
                //                help = resources.GetObject(changeInfos[index].Name + ".HelpNavigator");
                //                if (help != null && help.GetType() == typeof(HelpNavigator))
                //                {
                //                    helpProvider.SetHelpNavigator((Control)changeInfos[index].Value, (HelpNavigator)help);
                //                }
                //                text = resources.GetString(changeInfos[index].Name + ".HelpString");
                //                if (text != null)
                //                {
                //                    helpProvider.SetHelpString((Control)changeInfos[index].Value, text);
                //                }
                //                help = resources.GetObject(changeInfos[index].Name + ".ShowHelp");
                //                if (help != null && help.GetType() == typeof(bool))
                //                {
                //                    helpProvider.SetShowHelp((Control)changeInfos[index].Value, (bool)help);
                //                }
                //            }
                //        }
                //    }
                //}

                // Call ResumeLayout for Form and all fields derived from Control to resume layout logic.
                for (int index = changeInfos.Count - 1; index >= 0; index--)
                {
                    if (changeInfos[index].Type.IsSubclassOf(typeof (Control)))
                    {
                        changeInfos[index].Type.InvokeMember("ResumeLayout", BindingFlags.InvokeMethod, null,
                                                             changeInfos[index].Value, new object[] {true});
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        private static void TreeSearchApply(ComponentResourceManager resourceManager, Control control)
        {
            if(control != null)
            {
                
                try
                {
                    resourceManager.ApplyResources(control, control.Name);                    
                }
                catch
                {

                }
                
                
                if (control is Form)
                {
                    MenuStrip toolstrip = (control as Form).MainMenuStrip;
                    if (toolstrip != null)
                    {
                        foreach (ToolStripItem item in toolstrip.Items)
                        {
                            TreeSearchApply(resourceManager, item);
                        }    
                    }                    
                }

                ContextMenuStrip contextMenuStrip = control.ContextMenuStrip;
                if (contextMenuStrip != null)
                {
                    foreach (ToolStripItem item in contextMenuStrip.Items)
                    {
                        TreeSearchApply(resourceManager, item);
                    }
                }
                
                if (control is ToolStrip)
                {
                    ToolStrip toolstrip = control as ToolStrip;
                    foreach (ToolStripItem item in toolstrip.Items)
                    {
                        TreeSearchApply(resourceManager, item);
                    }
                }
                else if (control is DockContainer)
                {
                    DockContainer container = control as DockContainer;
                    if ((container.ActiveDocument as Control) != null)
                    {
                        (container.ActiveDocument as Control).Hide();
                        (container.ActiveDocument as Control).Show();
                    }
                }
                else
                {
                    if (control.Controls.Count > 0)
                    {
                        foreach (Control control1 in control.Controls)
                        {
                            TreeSearchApply(resourceManager, control1);
                        }
                    }
                }                
            }
        }

        private static void TreeSearchApply(ComponentResourceManager resourceManager, ToolStripItem control)
        {
            if (control != null)
            {
                try
                {
                    resourceManager.ApplyResources(control, control.Name);
                }
                catch
                {
                    
                }
                

                if (control is ToolStripDropDownItem)
                {
                    ToolStripDropDownItem dropdown = control as ToolStripDropDownItem;
                    foreach (ToolStripItem item in dropdown.DropDownItems)
                    {
                            TreeSearchApply(resourceManager, item as ToolStripMenuItem);
                    }    
                }
                
            }
        }


        /// <summary>
        /// ChangeFormUsingInitializeComponent changes the culture of an existing form by removing all of its
        /// controls and adding them back by calling InitializeComponent
        /// </summary>
        /// <param name="form">The form for which the culture should be changed</param>
        /// <param name="culture">The culture name to change the form to</param>
        /// <remarks>This method changes the CurrentUICulture to the given culture</remarks>
        public static void ChangeFormUsingInitializeComponent(Form form, string culture)
        {
            // get the form's private InitializeComponent method
            MethodInfo initializeComponentMethodInfo = form.GetType().GetMethod(
                "InitializeComponent", BindingFlags.Instance | BindingFlags.NonPublic);

            if (initializeComponentMethodInfo != null)
            {
                // the form has an InitializeComponent method that we can invoke

                // save all controls
                List<Control> controls = new List<Control>();
                foreach (Control control in form.Controls)
                {
                    controls.Add(control);
                }

                // remove all controls
                foreach (Control control in controls)
                {
                    form.Controls.Remove(control);
                }

                //int X = form.Location.X;
                //int Y = form.Location.Y;

                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);

                // call the InitializeComponent method to add back controls
                initializeComponentMethodInfo.Invoke(form, new object[] { });

                //form.Location = new Point(X, Y);
            }
        }

        #region ChangeInfo class
        /// <summary>
        /// Encapsulates all information needed to apply localized resources to a form or field.
        /// </summary>
        private class ChangeInfo
        {
            #region instance fields
            /// <summary>
            /// Gets the name of the form or field.
            /// </summary>
            public string Name
            {
                get { return name; }
            }

            /// <summary>
            /// Stores the name of the form or field.
            /// </summary>
            private string name;

            /// <summary>
            /// Gets the instance of the form or field.
            /// </summary>
            public object Value
            {
                get { return this.value; }
            }

            /// <summary>
            /// Stores the instance of the form or field.
            /// </summary>
            private object value;

            /// <summary>
            /// Gets the <see cref="Type"/> object of the form or field.
            /// </summary>
            public Type Type
            {
                get { return type; }
            }

            /// <summary>
            /// Stores the <see cref="Type"/> object of the form or field.
            /// </summary>
            private Type type;
            #endregion

            #region construction
            /// <summary>
            /// Initializes a new instance of the <see cref="ChangeInfo"/> class.
            /// </summary>
            /// <param name="name">The name of the form or field.</param>
            /// <param name="value">The instance of the form or field.</param>
            /// <param name="type">The <see cref="Type"/> object of the form or field.</param>
            public ChangeInfo(string name, object value, Type type)
            {
                this.name = name;
                this.value = value;
                this.type = type;
            }
            #endregion
        }
        #endregion
    }
}