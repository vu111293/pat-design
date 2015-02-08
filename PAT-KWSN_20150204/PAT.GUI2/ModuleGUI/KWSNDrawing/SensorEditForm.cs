using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PAT.Common.GUI.Drawing;
using PAT.GUI.LTSDrawing;
using PAT.Common.GUI.KWSNModule;

namespace PAT.GUI.ModuleGUI.KWSNDrawing
{
    public partial class SensorEditForm : Form
    {
        private WSNSensorItem _item;
        private Hashtable _tranConditions = null;

        public SensorEditForm(StateItem item)
        {
            _item = (WSNSensorItem)item;

            InitializeComponent();

            txtSensorName.Text = _item.Name;

            foreach (SensorType type in Enum.GetValues(typeof(SensorType)))
            {
                this.cmbSensorType.Items.Add(type.ToString());
                if (type == _item.NodeType)
                    this.cmbSensorType.SelectedItem = type.ToString();
            }

            foreach (SensorMode mode in Enum.GetValues(typeof(SensorMode)))
            {
                this.cmbSensorMode.Items.Add(mode.ToString());
                if (mode == _item.NodeMode)
                    this.cmbSensorMode.SelectedItem = mode.ToString();
            }
            if (this.cmbSensorMode.SelectedIndex < 0)
                this.cmbSensorMode.SelectedIndex = (int)SensorMode.Normal;

            //loadTransitionGuardControl(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _item.Name = txtSensorName.Text;
            _item.NodeMode = (SensorMode)this.cmbSensorMode.SelectedIndex;
            _item.NodeType = (SensorType)this.cmbSensorType.SelectedIndex;
            _item.IsInitialState = (_item.NodeType == SensorType.Source);

            //if (_tranConditions != null)
            //{
            //    string key;
            //    foreach (DataGridViewRow row in dgvTransitionConditions.Rows)
            //    {
            //        key = (string)row.Cells[0].Value;
            //        _tranConditions[key] = row.Cells[1].Value;
            //    }

            //    _item.TransitionCondition = _tranConditions;
            //}
        }

        private void cmbSensorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool showMode = (this.cmbSensorType.SelectedIndex == (int)SensorType.Intermediate);

            this.cmbSensorMode.Visible = showMode;
            this.labelSensorMode.Visible = showMode;
        }

        //private void cmbSensorType_SelectionChangeCommitted(object sender, EventArgs e)
        //{
        //    loadTransitionGuardControl(true);
        //}

        //private void cmbSensorMode_SelectionChangeCommitted(object sender, EventArgs e)
        //{
        //    loadTransitionGuardControl(true);
        //}

        ///// <summary>
        ///// Load PN Transition guard condition
        ///// </summary>
        ///// <param name="blGetUI"></param>
        //private void loadTransitionGuardControl(bool blGetUI)
        //{
        //    _tranConditions = _item.TransitionCondition;
        //    do
        //    {
        //        if (_tranConditions == null || _tranConditions.Count == 0 || blGetUI == true)
        //        {
        //            SensorType type = (SensorType)this.cmbSensorType.SelectedIndex;
        //            SensorMode mode = (SensorMode)this.cmbSensorMode.SelectedIndex;
        //            List<string> trans = _item.GetPNTransitions(_item.GetPNId(type, mode));
        //            if (trans == null)
        //                break;

        //            _tranConditions = new Hashtable();
        //            foreach (string s in trans)
        //                _tranConditions[s] = null;
        //        }

        //        dgvTransitionConditions.Rows.Clear();
        //        foreach (string key in _tranConditions.Keys)
        //        {
        //            string[] data = new string[] { key, (string)_tranConditions[key] };
        //            dgvTransitionConditions.Rows.Add(data);
        //        }
        //    } while (false);
        //}
    }
}
