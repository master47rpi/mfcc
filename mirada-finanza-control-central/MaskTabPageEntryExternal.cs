using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace mirada_finanza_control_central
{

    internal class MaskTabPageEntryExternal : MaskTabPageBase
    {
        protected ComboBox comboBoxEntryCategory;
        protected TextBox textBoxEntryPostingType;
        protected NumericUpDown numericUpDownAmount;
        protected DateTimePicker dateTimePickerVoucherDate;
        protected TextBox textBoxText;
        protected TextBox textBoxEntryNote;
        protected CheckBox checkBoxReversal;
        protected ComboBox comboBoxReferenceVoucher;
        protected CheckBox checkBoxSettleInvoice;
        protected ComboBox comboBoxEntryTransactionOpenInvoices;
        protected ComboBox comboBoxEntryCustomer;
        protected ComboBox comboBoxEntrySupplier;
        protected PictureBox pictureBoxEntryPreview;
        protected Label labelSelectedFilename;

        private byte[] selectedFileBytes = null;
        private string selectedFileExt = "";

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

            this.AssignControl(
                out this.numericUpDownAmount,
                nameof(this.numericUpDownAmount));

            this.AssignControl(
                out this.dateTimePickerVoucherDate,
                nameof(this.dateTimePickerVoucherDate));

            this.AssignControl(
                out this.textBoxText,
                nameof(this.textBoxText));

            this.AssignControl(
                out this.textBoxEntryNote,
                nameof(this.textBoxEntryNote));

            this.AssignControl(
                out this.checkBoxReversal,
                nameof(this.checkBoxReversal));

            this.AssignControl(
                out this.comboBoxReferenceVoucher,
                nameof(this.comboBoxReferenceVoucher));

            this.AssignControl(
                out this.checkBoxSettleInvoice,
                nameof(this.checkBoxSettleInvoice));

            this.AssignControl(
                out this.comboBoxEntryTransactionOpenInvoices,
                nameof(this.comboBoxEntryTransactionOpenInvoices));

            this.AssignControl(
                out this.comboBoxEntryCustomer,
                nameof(this.comboBoxEntryCustomer));

            this.AssignControl(
                out this.comboBoxEntrySupplier,
                nameof(this.comboBoxEntrySupplier));

            this.AssignControl(
                out this.pictureBoxEntryPreview,
                nameof(this.pictureBoxEntryPreview));

            this.AssignControl(
                out this.labelSelectedFilename,
                nameof(this.labelSelectedFilename));
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

        public void buttonVoucherPostClick(
            object _sender,
            EventArgs _e)
        {
            try
            {
                // 1. Daten validieren
                if (comboBoxEntryCategory.SelectedItem == null) { MessageBox.Show("Bitte Kategorie wählen!"); return; }
                if (!decimal.TryParse(numericUpDownAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Bitte einen gültigen Betrag eingeben!"); return;
                }

                // 2. Jahr vom Belegdatum holen (wichtig für Nacherfassungen!)
                int voucherYear = dateTimePickerVoucherDate.Value.Year;
                string voucher = dbManager.GetNextVoucher(voucherYear);

                EntryTransaction entryTransaction = new EntryTransaction
                {
                    Voucher = voucher,
                    Year = voucherYear,
                    EntryCategoryName = comboBoxEntryCategory.SelectedItem.ToString(),
                    TransactionType = textBoxEntryPostingType.Text,
                    Amount = amount,
                    Description = textBoxText.Text,
                    Note = textBoxEntryNote.Text,
                    TransDate = dateTimePickerVoucherDate.Value.ToString("yyyy-MM-dd"),
                    CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    // Wenn Bytes da sind (Länge > 0), dann nimm sie, sonst null
                    Attachment = (selectedFileBytes != null && selectedFileBytes.Length > 0) ? selectedFileBytes : null,
                    FileExt = (selectedFileBytes != null && selectedFileBytes.Length > 0) ? selectedFileExt : null,
                    Reversal = checkBoxReversal.Checked ? true : false,
                    // Bei ReversalReferenceVoucher prüfen wir auf null beim SelectedValue
                    ReversalReferenceVoucher = checkBoxReversal.Checked ? comboBoxReferenceVoucher.SelectedValue?.ToString() : null,
                    // InvoiceReference einfach null lassen, wenn sie leer ist
                    InvoiceReference = checkBoxSettleInvoice.Checked ? comboBoxEntryTransactionOpenInvoices.SelectedValue?.ToString() : null,

                    // Wir prüfen den Typ und konvertieren den SelectedValue in ein int?
                    CustomerId = (textBoxEntryPostingType.Text == "Einnahme" && comboBoxEntryCustomer.SelectedValue != null)
                 ? Convert.ToInt32(comboBoxEntryCustomer.SelectedValue)
                 : (int?)null,

                    SupplierId = (textBoxEntryPostingType.Text != "Einnahme" && comboBoxEntrySupplier.SelectedValue != null)
                 ? Convert.ToInt32(comboBoxEntrySupplier.SelectedValue)
                 : (int?)null
                };

                if (checkBoxSettleInvoice.Checked && comboBoxEntryTransactionOpenInvoices.SelectedValue != null)
                {
                    int selectedId = (int)comboBoxEntryTransactionOpenInvoices.SelectedValue;
                    dbManager.MarkInvoiceAsPaid(selectedId);
                }

                dbManager.PostEntryTransaction(entryTransaction);

                MessageBox.Show($"Erfolgreich gespeichert! Belegnummer: {voucher}", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // RefreshJournal();
                // dataGridViewAssetsRefresh();
                // tabPageOverviewRefresh();
                ClearForm(); // Felder leeren für den nächsten Beleg
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            numericUpDownAmount.Value = 0;
            textBoxText.Clear();
            textBoxEntryNote.Clear();
            comboBoxEntryCategory.SelectedIndex = -1;
            textBoxEntryPostingType.Clear();
            textBoxEntryPostingType.BackColor = SystemColors.Control;
            checkBoxReversal.Checked = false;
            comboBoxReferenceVoucher.SelectedIndex = -1;
            // Datum lassen wir meistens stehen, falls man mehrere Belege vom selben Tag hat

            comboBoxReferenceVoucher.Enabled = false;
            comboBoxReferenceVoucher.DataSource = null;
            comboBoxReferenceVoucher.Text = "";
            numericUpDownAmount.Enabled = true;
            comboBoxEntryCategory.Enabled = true;
            textBoxEntryPostingType.Enabled = true;
            textBoxText.Enabled = true;
            textBoxEntryNote.Enabled = true;
            dateTimePickerVoucherDate.Enabled = true;

            selectedFileBytes = null;
            selectedFileExt = "";
            pictureBoxEntryPreview.Image = null;
            labelSelectedFilename.Text = "";

            checkBoxSettleInvoice.Checked = false;
            comboBoxEntryTransactionOpenInvoices.SelectedIndex = -1;
            comboBoxEntryTransactionOpenInvoices.Enabled = false;
            comboBoxEntryTransactionOpenInvoices.DataSource = null;
            comboBoxEntryTransactionOpenInvoices.Text = "";

            comboBoxEntryCustomer.SelectedIndex = -1;
            comboBoxEntryCustomer.Enabled = false;
            comboBoxEntryCustomer.DataSource = null;
            comboBoxEntryCustomer.Text = "";

            comboBoxEntrySupplier.SelectedIndex = -1;
            comboBoxEntrySupplier.Enabled = false;
            comboBoxEntrySupplier.DataSource = null;
            comboBoxEntrySupplier.Text = "";
        }

        public void checkBoxReversalCheckedChanged(
            object _sender,
            EventArgs _e)
        {
            if (checkBoxReversal.Checked)
            {
                comboBoxReferenceVoucher.Enabled = true;
                LoadVouchersIntoComboBox();
                comboBoxReferenceVoucher.Focus();
                // comboBoxReferenceVoucher.DroppedDown = true; // Öffnet die Liste sofort
            }
            else
            {
                comboBoxReferenceVoucher.Enabled = false;
                comboBoxReferenceVoucher.DataSource = null;
                comboBoxReferenceVoucher.Text = "";
                numericUpDownAmount.Enabled = true;
                comboBoxEntryCategory.Enabled = true;
                textBoxEntryPostingType.Enabled = true;
                textBoxText.Enabled = true;
                textBoxEntryNote.Enabled = true;
                dateTimePickerVoucherDate.Enabled = true;

                ClearForm();
            }
        }

        protected void LoadVouchersIntoComboBox()
        {
            try
            {
                // Daten vom Manager holen
                DataTable dt = dbManager.GetAvailableVouchersForReversal();

                // UI binden
                comboBoxReferenceVoucher.DataSource = null; // Reset
                comboBoxReferenceVoucher.DisplayMember = "DisplayText";
                comboBoxReferenceVoucher.ValueMember = "Voucher";
                comboBoxReferenceVoucher.DataSource = dt;

                // Optional: Falls die Box leer starten soll
                comboBoxReferenceVoucher.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void comboBoxReferenceVoucherSelectedIndexChanged(
            object _sender,
            EventArgs _e)
        {
            if (comboBoxReferenceVoucher.SelectedValue != null && checkBoxReversal.Checked)
            {
                string selectedVoucher = comboBoxReferenceVoucher.SelectedValue.ToString();

                // DATEN ÜBER MANAGER HOLEN
                EntryTransaction original = dbManager.GetTransactionForReversal(selectedVoucher);

                if (original != null)
                {
                    // Felder automatisch füllen
                    textBoxText.Text = "STORNO zu: " + original.Description;
                    numericUpDownAmount.Value = original.Amount;
                    textBoxEntryNote.Text = "STORNO zu: " + original.Note;

                    if (DateTime.TryParse(original.TransDate, out DateTime vDate))
                        dateTimePickerVoucherDate.Value = vDate;

                    comboBoxEntryCategory.Text = original.EntryCategoryName;
                    textBoxEntryPostingType.Text = original.TransactionType;

                    if (original.CustomerId.HasValue && original.CustomerId.Value != 0)
                    {
                        comboBoxEntryCustomer.Text = dbManager.GetCustomerById(original.CustomerId.Value).Name;
                    }
                    if (original.SupplierId.HasValue && original.SupplierId.Value != 0)
                    {
                        comboBoxEntrySupplier.Text = dbManager.GetSupplierById(original.SupplierId.Value).Name;
                    }

                    // --- FELDER SPERREN ---
                    SetEntryFieldsEnabled(false);
                }
            }
        }

        protected void SetEntryFieldsEnabled(bool enabled)
        {
            numericUpDownAmount.Enabled = enabled;
            comboBoxEntryCategory.Enabled = enabled;
            textBoxEntryPostingType.Enabled = enabled;
            textBoxText.Enabled = enabled;
            textBoxEntryNote.Enabled = enabled;
            dateTimePickerVoucherDate.Enabled = enabled;
        }

        public void buttonLoadFileClick(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Bilder & PDF|*.jpg;*.jpeg;*.png;*.pdf";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Datei in Bytes umwandeln
                    selectedFileBytes = File.ReadAllBytes(ofd.FileName);
                    // Dateiendung merken
                    selectedFileExt = Path.GetExtension(ofd.FileName);

                    labelSelectedFilename.Text = Path.GetFileName(ofd.FileName);

                    // Vorschau, falls es ein Bild ist
                    if (selectedFileExt.ToLower() != ".pdf")
                    {
                        using (var ms = new MemoryStream(selectedFileBytes))
                        {
                            pictureBoxEntryPreview.Image = Image.FromStream(ms);
                        }
                    }
                }
            }
        }

        public void textBoxEntryPostingTypeTextChanged(
            object _sender,
            EventArgs _e)
        {
            string selectedType = textBoxEntryPostingType.Text;

            if (selectedType == "Einnahme")
            {
                comboBoxEntryCustomer.Enabled = true;
                comboBoxEntrySupplier.Enabled = false;

                // Nur die Kundenliste befüllen
                FillEntryComboBox(comboBoxEntryCustomer, "Customer");

                // Lieferantenliste leeren
                comboBoxEntrySupplier.DataSource = null;
            }
            else
            {
                comboBoxEntryCustomer.Enabled = false;
                comboBoxEntrySupplier.Enabled = true;

                // Nur die Lieferantenliste befüllen
                FillEntryComboBox(comboBoxEntrySupplier, "Supplier");

                // Kundenliste leeren
                comboBoxEntryCustomer.DataSource = null;
            }
        }

        protected void FillEntryComboBox(
            ComboBox _comboBox,
            string _type)
        {
            try
            {
                // Daten vom DBManager holen
                DataTable dt = (_type == "Customer")
                               ? dbManager.FetchAllCustomers()
                               : dbManager.FetchAllSuppliers();

                // Wichtig: Zuerst DataSource auf null setzen, um alte Bindungen zu lösen
                _comboBox.DataSource = null;

                _comboBox.DisplayMember = "Name"; // Was man sieht
                _comboBox.ValueMember = "Id";     // Was im Hintergrund gespeichert wird
                _comboBox.DataSource = dt;

                // Suche erleichtern (Tippen schlägt Namen vor)
                _comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                _comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

                _comboBox.SelectedIndex = -1; // Startet leer
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der {_type}-Liste: {ex.Message}");
            }
        }

        public void checkBoxSettleInvoiceCheckedChanged(
            object _sender,
            EventArgs _e)
        {
            // Dropdown aktivieren/deaktivieren
            comboBoxEntryTransactionOpenInvoices.Enabled = checkBoxSettleInvoice.Checked;

            if (checkBoxSettleInvoice.Checked)
            {

                comboBoxEntryTransactionOpenInvoices.Enabled = true;
                // Offene Rechnungen laden
                var openInvoices = dbManager.GetOpenInvoices();
                comboBoxEntryTransactionOpenInvoices.DataSource = openInvoices;
                comboBoxEntryTransactionOpenInvoices.DisplayMember = "InvoiceNumber";
                comboBoxEntryTransactionOpenInvoices.ValueMember = "Id";

                // Automatisierung: Typ und Kategorie für die Frau vorfüllen
                // (Ersetze 'txtType'/'txtCategory' durch deine tatsächlichen Control-Namen)
                comboBoxEntryCategory.Text = "Produktverkauf";
                textBoxEntryPostingType.Text = "Einnahme";
            }
        }

        public void comboBoxEntryTransactionOpenInvoicesSelectionChangeCommitted(
            object _sender,
            EventArgs _e)
        {
            // 1. Das ausgewählte Rechnungs-Objekt holen
            if (comboBoxEntryTransactionOpenInvoices.SelectedItem is Invoice selectedInv)
            {
                // Betrag automatisch aus der Rechnung in das Betrags-Feld übernehmen
                numericUpDownAmount.Text = selectedInv.TotalAmount.ToString();
                textBoxText.Text = $"Zahlung zu Rechnung {selectedInv.InvoiceNumber}";

                // Automatisierung: Typ und Kategorie für die Frau vorfüllen
                // (Ersetze 'txtType'/'txtCategory' durch deine tatsächlichen Control-Namen)
                comboBoxEntryCategory.Text = "Produktverkauf";
                textBoxEntryPostingType.Text = "Einnahme";
            }
        }
    }
}
