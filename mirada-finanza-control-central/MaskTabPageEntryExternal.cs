using System;
using System.Collections.Generic;
using System.Text;

namespace mirada_finanza_control_central
{

    internal class MaskTabPageEntryExternal : MaskTabPageBase
    {
        protected ComboBox comboBoxEntryCategory;
        protected TextBox textBoxEntryPostingType;

        // Der Konstruktor muss die Parameter an die Basisklasse "durchreichen"
        public MaskTabPageEntryExternal(
            TabControl _tabControl,
            TabPage _tabPage,
            Button _callingButton,
            DBManager _dbManager)
            : base(
                  _tabControl,
                  _tabPage,
                  _callingButton,
                  _dbManager)
        {
            // Hier kannst du spezifische Dinge für die interne Erfassung machen
            this.AssignControl(
                out this.comboBoxEntryCategory,
                nameof(this.comboBoxEntryCategory));

            this.AssignControl(
                out this.textBoxEntryPostingType,
                nameof(this.textBoxEntryPostingType));
        }

        public override void Clear()
        {
            // Erst die allgemeine Logik der Basisklasse (alle Textboxen etc. leeren)
            base.Clear();

            // Dann spezifische Dinge für DIESE Maske
            // z.B. ein spezielles Grid leeren, das nur hier existiert
        }

        public override void Show()
        {
            // Erst die Logik der Basisklasse (Tab wechseln, Button highlighten)
            base.Show();

            // Dann z.B. Daten frisch aus der DB laden
            this.LoadCategories();
            // RefreshMyData();
        }

        private void LoadCategories()
        {
            try
            {
                comboBoxEntryCategory.Items.Clear();
                var categories = dbManager.GetCategoryNames();
                comboBoxEntryCategory.Items.AddRange(categories.ToArray());

                if (comboBoxEntryCategory.Items.Count > 0)
                    comboBoxEntryCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Kategorien: " + ex.Message);
            }
        }

        public void comboBoxEntryCategorySelectedIndexChanged()
        {
            if (comboBoxEntryCategory.SelectedItem != null)
            {
                string selectedCategory = this.comboBoxEntryCategory.SelectedItem.ToString();

                // Typ aus DB holen
                string type = dbManager.GetTransactionTypeFromCategory(selectedCategory);

                // In das nicht-editierbare Feld schreiben
                this.textBoxEntryPostingType.Text = type;

                // Kleiner Bonus: Farbe ändern für bessere Übersicht
                if (type == "Einnahme")
                {
                    this.textBoxEntryPostingType.BackColor = Color.LightGreen;
                }
                else
                {
                    this.textBoxEntryPostingType.BackColor = Color.LightCoral;
                }
            }
        }
    }
}
