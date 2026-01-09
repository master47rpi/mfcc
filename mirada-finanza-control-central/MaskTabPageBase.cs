using System;
using System.Collections.Generic;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class MaskTabPageBase
    {
        private static readonly List<MaskTabPageBase> maskTabPageInstances = new List<MaskTabPageBase>();

        protected TabControl tabControl;
        protected TabPage tabPage;
        protected Button callerButton;
        protected DBManager dbManager;

        public MaskTabPageBase(
            TabControl _tabControl,
            TabPage _tabPage,
            Button _callerButton,
            DBManager _dbManager)
        {
            this.tabControl = _tabControl;
            this.tabPage = _tabPage;
            this.callerButton = _callerButton;
            this.dbManager = _dbManager;

            maskTabPageInstances.Add(this);
        }

        protected void AssignControl<T>(
            out T _controlVariable,
            string _name) where T : Control
        {
            var found = this.tabPage.Controls.Find(_name, true).FirstOrDefault() as T;

            if (found == null)
            {
                MessageBox.Show($"Kritischer Fehler: Control '{_name}' ({typeof(T).Name}) nicht gefunden!");
            }

            _controlVariable = found;
        }


        public virtual void Clear()
        {
            this.ClearControlsRecursive(this.tabPage.Controls);
        }

        public virtual void Show()
        {
            this.Clear();
            this.tabPage.Refresh();
            this.tabControl.SelectedTab = this.tabPage;
            this.HighlightCallerButton();

        }

        public void HighlightCallerButton()
        {
            foreach (var instance in maskTabPageInstances)
            {
                if (instance.callerButton != null)
                {
                    instance.callerButton.BackColor = Color.DarkSlateBlue;
                }
            }

            this.callerButton.BackColor = Color.DodgerBlue;
        }

        private void ClearControlsRecursive(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                // 1. TextBoxen leeren
                if (c is TextBox textBox)
                {
                    textBox.Clear();
                }
                // 2. ComboBoxen zurücksetzen (erster Eintrag oder leer)
                else if (c is ComboBox comboBox)
                {
                    if (comboBox.Items.Count > 0)
                        comboBox.SelectedIndex = 0; // Oder -1 für komplett leer
                    else
                        comboBox.Text = string.Empty;
                }
                // 3. CheckBoxen abwählen
                else if (c is CheckBox checkBox)
                {
                    checkBox.Checked = false;
                }
                // 4. Numerische Felder auf 0 setzen
                else if (c is NumericUpDown numeric)
                {
                    numeric.Value = numeric.Minimum;
                }
                // 5. DataGridViews leeren (falls vorhanden)
                else if (c is DataGridView grid)
                {
                    if (grid.DataSource != null)
                    {
                        // Wenn eine DataSource da ist, setzen wir diese auf null
                        // oder leeren die Liste dahinter
                        grid.DataSource = null;
                    }
                    else
                    {
                        // Nur wenn KEINE DataSource da ist, darf Rows.Clear() gerufen werden
                        grid.Rows.Clear();
                    }
                }

                // REKURSION: Wenn dieses Control selbst Kinder hat (z.B. ein Panel oder GroupBox),
                // suchen wir dort drin auch weiter!
                if (c.HasChildren)
                {
                    ClearControlsRecursive(c.Controls);
                }
            }
        }
    }
}
