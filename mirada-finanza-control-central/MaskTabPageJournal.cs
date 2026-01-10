using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class MaskTabPageJournal : MaskTabPageBase
    {
        protected DataGridView dataGridViewJournal;
        protected TextBox textBoxJournalNote;
        protected TextBox textBoxJournalAmount;
        protected TextBox textBoxJournalCategory;
        protected TextBox textBoxJournalVoucher;
        protected TextBox textBoxJournalVoucherDate;
        protected TextBox textBoxJournalReversalVoucher;
        protected TextBox textBoxJournalPostingText;
        protected TextBox textBoxJournalPostingType;
        protected TextBox textBoxJournalCreationDate;
        protected TextBox textBoxJournalInvoiceReference;
        protected TextBox textBoxJournalCustomerId;
        protected TextBox textBoxJournalSupplierId;
        protected TextBox textBoxJournalAsset;

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

            this.AssignControl(
                out this.dataGridViewJournal,
                nameof(this.dataGridViewJournal));

            this.AssignControl(
                out this.textBoxJournalNote,
                nameof(this.textBoxJournalNote));

            this.AssignControl(
                out this.textBoxJournalAmount,
                nameof(this.textBoxJournalAmount));

            this.AssignControl(
                out this.textBoxJournalCategory,
                nameof(this.textBoxJournalCategory));

            this.AssignControl(
                out this.textBoxJournalVoucher,
                nameof(this.textBoxJournalVoucher));

            this.AssignControl(
                out this.textBoxJournalVoucherDate,
                nameof(this.textBoxJournalVoucherDate));

            this.AssignControl(
                out this.textBoxJournalReversalVoucher,
                nameof(this.textBoxJournalReversalVoucher));

            this.AssignControl(
                out this.textBoxJournalPostingText,
                nameof(this.textBoxJournalPostingText));

            this.AssignControl(
                out this.textBoxJournalPostingType,
                nameof(this.textBoxJournalPostingType));

            this.AssignControl(
                out this.textBoxJournalCreationDate,
                nameof(this.textBoxJournalCreationDate));

            this.AssignControl(
                out this.textBoxJournalInvoiceReference,
                nameof(this.textBoxJournalInvoiceReference));

            this.AssignControl(
                out this.textBoxJournalCustomerId,
                nameof(this.textBoxJournalCustomerId));

            this.AssignControl(
                out this.textBoxJournalSupplierId,
                nameof(this.textBoxJournalSupplierId));

            this.AssignControl(
                out this.textBoxJournalAsset,
                nameof(this.textBoxJournalAsset));

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

        public void dataGridViewJournalCellFormatting(
            object _sender,
            DataGridViewCellFormattingEventArgs _e)
        {
            // Wir prüfen bei jeder Zelle, welcher Typ in der aktuellen Zeile steht
            // (Damit wir die Farbe für die gesamte Zeile festlegen können)
            var row = dataGridViewJournal.Rows[_e.RowIndex];

            // Prüfen, ob wir in der Spalte "Reversal" sind
            if (dataGridViewJournal.Columns[_e.ColumnIndex].Name == "Reversal" && _e.Value != null)
            {
                // Wir holen den Wert (0 oder 1)
                string val = _e.Value.ToString();

                if (val == "1")
                {
                    _e.Value = "Ja";
                    //e.CellStyle.ForeColor = Color.Red; // Stornos direkt rot markieren
                    //e.FormattingApplied = true; // Sagt dem Programm: "Ich hab's erledigt"
                }
                else if (val == "0")
                {
                    _e.Value = "Nein";
                    //e.FormattingApplied = true;
                }
            }

            // WICHTIG: "TransactionType" muss im SELECT deiner Refresh-Methode enthalten sein!
            if (row.Cells["TransactionType"].Value != null)
            {
                string type = row.Cells["TransactionType"].Value.ToString();
                bool reversal = Convert.ToBoolean(row.Cells["Reversal"].Value);

                if (type == "Einnahme")
                {
                    if (reversal == true)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 80, 80);
                    }
                    else
                    {
                        // Ein dezentes, helles Grün
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }
                else if (type == "Ausgabe")
                {
                    if (reversal == true)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(80, 180, 80);
                    }
                    else
                    {
                        // Ein dezentes, helles Rot (MistyRose oder helleres Lachs)
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                }
                else if (type == "Anlage")
                {
                    if (reversal == true)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(80, 180, 80);
                    }
                    else
                    {
                        // Ein dezentes, helles Rot (MistyRose oder helleres Lachs)
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                }
            }
        }

        public void dataGridViewJournalSelectionChanged(object sender, EventArgs e)
        {
            ClearJournalDetails();

            if (dataGridViewJournal.SelectedRows.Count > 0)
            {
                string voucherNr = dataGridViewJournal.SelectedRows[0].Cells["Voucher"].Value?.ToString();
                if (string.IsNullOrEmpty(voucherNr)) return;

                // DATEN ÜBER MANAGER HOLEN
                EntryTransaction entry = dbManager.GetTransactionByVoucher(voucherNr);
                Asset asset = dbManager.GetAssetByTransactionId(entry.ID);

                if (entry != null)
                {
                    // TEXTBOXEN BEFÜLLEN
                    textBoxJournalNote.Text = entry.Note ?? "";
                    textBoxJournalAmount.Text = entry.Amount.ToString("N2") + " €";
                    textBoxJournalCategory.Text = entry.EntryCategoryName;
                    textBoxJournalVoucher.Text = entry.Voucher;
                    textBoxJournalVoucherDate.Text = entry.TransDate;
                    textBoxJournalReversalVoucher.Text = entry.ReversalReferenceVoucher ?? "";
                    textBoxJournalPostingText.Text = entry.Description;
                    textBoxJournalPostingType.Text = entry.TransactionType;
                    textBoxJournalInvoiceReference.Text = entry.InvoiceReference;

                    // 1. Prüfung auf Customer
                    if (entry.CustomerId.HasValue && entry.CustomerId.Value != 0)
                    {
                        // Wir übergeben explizit .Value (das echte int)
                        var customer = dbManager.GetCustomerById(entry.CustomerId.Value);
                        if (customer != null)
                        {
                            textBoxJournalCustomerId.Text = customer.Name;
                        }
                    }
                    // 2. Prüfung auf Supplier
                    else if (entry.SupplierId.HasValue && entry.SupplierId.Value != 0)
                    {
                        var supplier = dbManager.GetSupplierById(entry.SupplierId.Value);
                        if (supplier != null)
                        {
                            textBoxJournalSupplierId.Text = supplier.Name;
                        }
                    }

                    // DATUM FORMATIEREN
                    if (!string.IsNullOrEmpty(entry.CreatedDate))
                    {
                        if (DateTime.TryParse(entry.CreatedDate, out DateTime dt))
                            textBoxJournalCreationDate.Text = dt.ToString("dd.MM.yyyy HH:mm");
                        else
                            textBoxJournalCreationDate.Text = entry.CreatedDate;
                    }
                    else
                    {
                        textBoxJournalCreationDate.Text = "Kein Datum";
                    }
                }

                if (asset.Id != 0)
                {
                    textBoxJournalAsset.Text = asset.Id.ToString();
                }
            }
            else
            {
                ClearJournalDetails();
            }
        }

        private void ClearJournalDetails()
        {
            textBoxJournalNote.Clear();
            textBoxJournalAmount.Clear();
            textBoxJournalCategory.Clear();
            textBoxJournalVoucher.Clear();
            textBoxJournalVoucherDate.Clear();
            textBoxJournalReversalVoucher.Clear();
            textBoxJournalPostingText.Clear();
            textBoxJournalPostingType.Clear();
            textBoxJournalCreationDate.Clear();
            textBoxJournalInvoiceReference.Clear();
            textBoxJournalCustomerId.Clear();
            textBoxJournalSupplierId.Clear();
        }

        public void buttonJournalPicturePDFClick(
            object _sender,
            EventArgs _e)
        {
            if (dataGridViewJournal.SelectedRows.Count == 0) return;

            string voucherNr = dataGridViewJournal.SelectedRows[0].Cells["Voucher"].Value?.ToString();
            if (string.IsNullOrEmpty(voucherNr)) return;

            try
            {
                // 1. DATEN HOLEN
                EntryTransaction doc = dbManager.GetAttachmentByVoucher(voucherNr);

                if (doc == null || doc.Attachment == null)
                {
                    MessageBox.Show("Kein Beleg für diesen Eintrag gefunden.");
                    return;
                }

                // 2. DATEI VORBEREITEN
                string extension = doc.FileExt.ToLower().Trim();
                if (!extension.StartsWith(".")) extension = "." + extension;

                // Temporären Pfad generieren
                string tempPath = Path.Combine(Path.GetTempPath(), $"Beleg_{voucherNr}{extension}");
                File.WriteAllBytes(tempPath, doc.Attachment);

                // 3. ANZEIGELOGIK (UI-Aufgabe)
                if (extension == ".pdf")
                {
                    // PDF im Standard-Viewer öffnen
                    Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
                }
                else if (new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(extension))
                {
                    // Bild in der modalen Form anzeigen
                    ZeigeBildModal(tempPath);
                }
                else
                {
                    // Unbekanntes Format? Einfach mal versuchen mit Windows-Standard zu öffnen
                    Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Öffnen des Belegs: " + ex.Message);
            }
        }

        protected void ZeigeBildModal(string filePath)
        {
            using (FormImageViewer viewer = new FormImageViewer())
            {
                // Wir laden das Bild in die PictureBox der neuen Form
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    viewer.pictureBox1.Image = Image.FromStream(stream);
                    viewer.Text = "Beleg-Ansicht: " + Path.GetFileName(filePath);
                    viewer.ShowDialog(); // ShowDialog macht die Form MODAL
                }
            }
        }
    }
}
