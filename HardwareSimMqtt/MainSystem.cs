using HardwareSimMqtt.EventArgsModel;
using HardwareSimMqtt.Model;
using HardwareSimMqtt.Model.BitMap;
using HardwareSimMqtt.Model.DataContainer;
using HardwareSimMqtt.Model.QueryJob;
using HardwareSimMqtt.UIComponent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformTimer = System.Windows.Forms.Timer;

namespace HardwareSimMqtt
{
    public class MainSystem
    {
        #region Window handle
        private Form ParentWindowHandle
        {
            get => Program.WndHandle2;
        }

        private UiMainPage _uiMainPageHandle = null;
        public UiMainPage UiMainPageHandle
        {
            get
            {
                if (_uiMainPageHandle == null)
                {
                    _uiMainPageHandle = new UiMainPage();
                }
                return _uiMainPageHandle;
            }
        }
        #endregion

        private WinformTimer _systemTimer = null;
        public WinformTimer SystemTimer
        {
            get
            {
                if (_systemTimer == null)
                {
                    _systemTimer = new WinformTimer();
                    _systemTimer.Enabled = true;
                    _systemTimer.Interval = 200;
                }
                return _systemTimer;
            }
        }


        private DataTable _bitSetDataTable = null;
        private DataTable BitSetDataTable
        {
            get
            {
                if (_bitSetDataTable == null)
                {
                    _bitSetDataTable = new DataTable();
                    int nColCount = Enum.GetNames(typeof(DeviceBitMask)).Length;

                    for (int nCol = nColCount - 1; nCol >= 0; nCol--)
                    {
                        _bitSetDataTable.Columns.Add(new DataColumn(String.Format("Bit{0}", nCol), typeof(uint)));
                    }

                    DataRow dr = _bitSetDataTable.NewRow();
                    for (int nCol = nColCount - 1; nCol >= 0; nCol--)
                    {
                        dr[String.Format("Bit{0}", nCol)] = 0;
                    }
                    _bitSetDataTable.Rows.Add(dr);
                }
                return _bitSetDataTable;
            }
        }

        public MainSystem()
        {
            InitializeWind();
        }

        private void InitializeWind()
        {
            SystemTimer.Start();
            UiMainPageHandle.DataGridViewBitSet.DataSource = BitSetDataTable;
            UiMainPageHandle.Dock = DockStyle.Fill;

            ParentWindowHandle.Controls.Add(UiMainPageHandle);
            Application.Run(ParentWindowHandle);
        }
    }
}
