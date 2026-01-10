using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class MaskTabPageAsset : MaskTabPageBase
    {
        protected DataGridView dataGridViewAssets;


        // Der Konstruktor muss die Parameter an die Basisklasse "durchreichen"
        public MaskTabPageAsset(
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
                out this.dataGridViewAssets,
                nameof(this.dataGridViewAssets));

            // Entfernt die Linien zwischen den Spaltenköpfen
            dataGridViewAssets.EnableHeadersVisualStyles = false;
            dataGridViewAssets.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewAssets.ColumnHeadersDefaultCellStyle.BackColor;
            // Dies erzwingt, dass kein Rahmen gezeichnet wird, wenn die Zelle gemalt wird
            dataGridViewAssets.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

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
            this.RefreshData();
        }

        public void RefreshData()
        {
            // Optik-Reset
            dataGridViewAssets.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dataGridViewAssets.DefaultCellStyle.SelectionForeColor = Color.Black;

            try
            {
                // 1. Daten fix und fertig berechnet vom Manager holen
                DataTable dt = dbManager.FetchAssetData();

                // 2. Binden
                dataGridViewAssets.DataSource = null;
                dataGridViewAssets.AutoGenerateColumns = true;
                dataGridViewAssets.DataSource = dt;

                // 3. Styling
                if (dataGridViewAssets.Columns.Count > 0)
                {
                    SetAssetGridHeaders();

                    // Währungsformate
                    dataGridViewAssets.Columns["Amount"].DefaultCellStyle.Format = "C2";
                    dataGridViewAssets.Columns["Restwert"].DefaultCellStyle.Format = "C2";

                    // Unnötiges ausblenden
                    string[] toHide = { "EntryTransactionId", "Status", "AbgeschriebenProzent", "Note" };
                    foreach (string col in toHide)
                    {
                        if (dataGridViewAssets.Columns.Contains(col))
                            dataGridViewAssets.Columns[col].Visible = false;
                    }

                    dataGridViewAssets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void SetAssetGridHeaders()
        {
            var cols = dataGridViewAssets.Columns;
            if (cols.Contains("AssetNr")) cols["AssetNr"].HeaderText = "Anlagen-Nr.";
            if (cols.Contains("Voucher")) cols["Voucher"].HeaderText = "Beleg-Nr.";
            if (cols.Contains("Description")) cols["Description"].HeaderText = "Bezeichnung";
            if (cols.Contains("PurchaseDate")) cols["PurchaseDate"].HeaderText = "Kaufdatum";
            if (cols.Contains("Amount")) cols["Amount"].HeaderText = "Anschaffungspreis";
            if (cols.Contains("UsefulLifeYears")) cols["UsefulLifeYears"].HeaderText = "Dauer (Jahre)";
            if (cols.Contains("Restwert")) cols["Restwert"].HeaderText = "Akt. Buchwert";
            if (cols.Contains("MonateVerbleibend")) cols["MonateVerbleibend"].HeaderText = "Restlaufzeit (Monate)";
        }
    }
}
