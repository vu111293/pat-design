using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tools.Diagrams;
using PAT.GUI.LTSDrawing;
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.KWSNModule;

namespace PAT.GUI.ModuleGUI.KWSNDrawing
{
    public partial class SensorChannelEditForm : Form
    {
        private WSNSensorChannel _channel = null;
        private Hashtable _tranConditions = null;

        public SensorChannelEditForm(Route route, List<LTSCanvas.CanvasItemData> canvasItems)
        {
            InitializeComponent();

            _channel = (WSNSensorChannel)route;

            foreach (LTSCanvas.CanvasItemData itemData in canvasItems)
            {
                if (itemData.Item is StateItem)
                {
                    this.cmbSource.Items.Add(itemData.Item);
                    this.cmbDest.Items.Add(itemData.Item);

                    if (itemData.Item.Equals(route.From))
                        this.cmbSource.SelectedItem = itemData.Item;

                    if (itemData.Item.Equals(route.To))
                        this.cmbDest.SelectedItem = itemData.Item;
                }
            }

            foreach (ChannelType type in Enum.GetValues(typeof(ChannelType)))
            {
                this.cmbChannelType.Items.Add(type.ToString());
                if (type == _channel.Type)
                    cmbChannelType.SelectedItem = type.ToString();
            }

            foreach (ChannelMode mode in Enum.GetValues(typeof(ChannelMode)))
            {
                this.cmbChannelMode.Items.Add(mode.ToString());
                if (mode == _channel.Mode)
                    this.cmbChannelMode.SelectedItem = mode.ToString();
            }

            //initTransitionGuardControl(false);
        }

        //private void initTransitionGuardControl(bool blGetUI)
        //{
        //    _tranConditions = _channel.TransitionCondition;
        //    do
        //    {
        //        if (_tranConditions == null || _tranConditions.Count == 0 || blGetUI == true)
        //        {
        //            ChannelType type = (ChannelType)this.cmbChannelType.SelectedIndex;
        //            ChannelMode mode =  (ChannelMode)this.cmbChannelMode.SelectedIndex;
        //            List<string> trans = _channel.GetPNTransitions(_channel.GetPNId(type, mode));
        //            if (trans == null)
        //                break;

        //            _tranConditions = new Hashtable();
        //            foreach (string s in trans)
        //                _tranConditions[s] = null;
        //        }

        //        dgvTransitionGuards.Rows.Clear();
        //        foreach (string key in _tranConditions.Keys)
        //        {
        //            string[] data = new string[] { key, (string)_tranConditions[key] };
        //            dgvTransitionGuards.Rows.Add(data);
        //        }
        //    } while (false);
        //}

        private void btnOK_Click(object sender, EventArgs e)
        {
            _channel.Type = (ChannelType)this.cmbChannelType.SelectedIndex;
            _channel.Mode = (ChannelMode)this.cmbChannelMode.SelectedIndex;

            //if (_tranConditions != null)
            //{
            //    string key;
            //    foreach (DataGridViewRow row in dgvTransitionGuards.Rows)
            //    {
            //        key = (string)row.Cells[0].Value;
            //        _tranConditions[key] = row.Cells[1].Value;
            //    }

            //    _channel.TransitionCondition = _tranConditions;
            //}
        }

        //private void cmbChannelType_SelectionChangeCommitted(object sender, EventArgs e)
        //{
        //    //initTransitionGuardControl(true);
        //}

        //private void cmbChannelMode_SelectionChangeCommitted(object sender, EventArgs e)
        //{
        //    initTransitionGuardControl(true);
        //}
    }
}
