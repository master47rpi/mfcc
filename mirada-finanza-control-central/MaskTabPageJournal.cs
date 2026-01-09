using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class MaskTabPageJournal : MaskTabPageBase
    {
        protected DataGridView dataGridViewJournal;

        // Der Konstruktor muss die Parameter an die Basisklasse "durchreichen"
        public MaskTabPageJournal(
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
                out this.dataGridViewJournal,
                nameof(this.dataGridViewJournal));

            this.dataGridViewJournal.EnableHeadersVisualStyles = false;
            this.dataGridViewJournal.ColumnHeadersDefaultCellStyle.SelectionBackColor = this.dataGridViewJournal.ColumnHeadersDefaultCellStyle.BackColor;
            // Dies erzwingt, dass kein Rahmen gezeichnet wird, wenn die Zelle gemalt wird
            this.dataGridViewJournal.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
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
            // UI-Einstellungen vorab
            dataGridViewJournal.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dataGridViewJournal.DefaultCellStyle.SelectionForeColor = Color.Black;

            try
            {
                // 1. DATEN HOLEN (über den Manager)
                DataTable dt = dbManager.FetchEntryTransactionData();

                // 2. DATEN BINDEN
                dataGridViewJournal.DataSource = null;
                dataGridViewJournal.AutoGenerateColumns = true;
                dataGridViewJournal.DataSource = dt;

                // 3. OPTIK (Spalten konfigurieren)
                if (dataGridViewJournal.Columns.Count > 0)
                {
                    // Original-Daten-Spalten ausblenden
                    string[] hiddenCols = { "Voucher", "TransDate", "TransactionType", "EntryCategoryName", "Amount", "Description", "Reversal", "ReversalReferenceVoucher", "InvoiceReference", "AssetId", "CustomerId", "SupplierId" };
                    foreach (var colName in hiddenCols)
                    {
                        if (dataGridViewJournal.Columns[colName] != null)
                            dataGridViewJournal.Columns[colName].Visible = false;
                    }

                    // Anzeige-Spalten konfigurieren (Ich gehe davon aus, ConfigureDisplayColumn existiert noch bei dir)
                    ConfigureDisplayColumn("Disp_VoucherDate", "Beleg\nDatum", 100, 0);
                    ConfigureDisplayColumn("Disp_TypeCategory", "Typ\nKategorie", 140, 1);
                    ConfigureDisplayColumn("Disp_AmountDesc", "Betrag\nBeschreibung", 140, 2);

                    // Storno- & Asset-Spalten
                    //SetColumnStyle("Reversal", "Storno", 60, 3);
                    //SetColumnStyle("ReversalReferenceVoucher", "Stornobeleg", 102, 4);
                    //SetColumnStyle("InvoiceReference", "Rechnungsbeleg", 102, 5);
                    //SetColumnStyle("AssetId", "Anlagen-Nr.", 102, 6);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Journals: " + ex.Message);
            }
        }

        private void ConfigureDisplayColumn(
            string name,
            string header,
            int width,
            int displayIndex)
        {
            if (dataGridViewJournal.Columns[name] != null)
            {
                dataGridViewJournal.Columns[name].HeaderText = header;
                dataGridViewJournal.Columns[name].Width = width;
                dataGridViewJournal.Columns[name].DisplayIndex = displayIndex;
                dataGridViewJournal.Columns[name].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                dataGridViewJournal.Columns[name].Visible = true;
            }
        }
    }
}
