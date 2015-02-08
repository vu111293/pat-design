using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Fireball.Syntax;

namespace PAT.GUI.Forms
{
    public partial class LanguageOptions : Form
    {
         
        ArrayList _ModifiedLangs = null;

        Language _Current = null;

        public LanguageOptions()
        {
            InitializeComponent();

            _ModifiedLangs = new ArrayList();

            Dictionary<string, Language>.Enumerator langs = (Dictionary<string, Language>.Enumerator )FormMain.Languages.ListLanguages();

            while (langs.MoveNext())
            {
                cmbLanguages.Items.Add(langs.Current);
            }
            cmbLanguages.SelectedIndex = 0;
        }

        private void cmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language lang = (Language)FormMain.Languages.GetFromIndex(cmbLanguages.SelectedIndex);

            _Current = lang;

            lstStyles.Items.Clear();

            TextStyle[] styles = lang.Styles;

            foreach (TextStyle current in styles)
            {
                lstStyles.Items.Add(current);
            }

            lstStyles.SelectedIndex = 0;
        }

        private void lstStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            propGrid.SelectedObject = lstStyles.SelectedItem;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (Language current in _ModifiedLangs)
            {
                current.SaveStyles();
            }

            this.Close();
        }

        private void propGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _ModifiedLangs.Add(_Current);
        }
    }
}