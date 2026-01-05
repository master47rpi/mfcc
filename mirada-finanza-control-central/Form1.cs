using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace mirada_finanza_control_central
{
    public partial class Form1 : Form
    {
        // Die Datenbankdatei liegt im gleichen Ordner wie die .exe
        string dbFile;
        string connString;

        DBManager dbManager;
        private Invoice currentInvoice;

        private byte[] selectedFileBytes = null;
        private string selectedFileExt = "";

        private byte[] selectedFileBytesProductPicture = null;
        private string selectedFileExtensionProductPicture = "";

        private byte[] selectedFileBytesCompanyLogo = null;
        private string selectedFileExtensionCompanyLogo = "";

        public Form1()
        {
            // 1. ZUERST DIE LIZENZ PRÜFEN
            if (!PerformLicenseCheck())
            {
                // Wenn ungültig, ist hier Schluss.
                Environment.Exit(0);
            }

            InitializeComponent();

            dbManager = new DBManager();
            dbManager.SetupDatabase();
            this.LoadCategories();

            foreach (Control ctrl in panelSettings.Controls) // panelSettings ist dein scrollbares Panel
            {
                if (ctrl is TextBox tb)
                {
                    tb.Leave += (s, e) => AutoSaveSettings();
                }
                // Falls du GroupBoxen nutzt, musst du auch deren Controls durchlaufen:
                if (ctrl is GroupBox gb)
                {
                    foreach (Control subCtrl in gb.Controls)
                    {
                        if (subCtrl is TextBox subTb) subTb.Leave += (s, e) => AutoSaveSettings();
                    }
                }
            }

            // Speichern beim Verlassen des Tabs:
            tabPageSettings.Leave += (s, e) => AutoSaveSettings();

            // Speichern beim Schließen des Programms:
            this.FormClosing += (s, e) => AutoSaveSettings();

            // Entfernt die Linien zwischen den Spaltenköpfen
            dataGridViewJournal.EnableHeadersVisualStyles = false;
            dataGridViewJournal.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewJournal.ColumnHeadersDefaultCellStyle.BackColor;
            // Dies erzwingt, dass kein Rahmen gezeichnet wird, wenn die Zelle gemalt wird
            dataGridViewJournal.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

            // Entfernt die Linien zwischen den Spaltenköpfen
            dataGridViewAssets.EnableHeadersVisualStyles = false;
            dataGridViewAssets.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewAssets.ColumnHeadersDefaultCellStyle.BackColor;
            // Dies erzwingt, dass kein Rahmen gezeichnet wird, wenn die Zelle gemalt wird
            dataGridViewAssets.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

            // Entfernt die Linien zwischen den Spaltenköpfen
            dataGridViewCustomers.EnableHeadersVisualStyles = false;
            dataGridViewCustomers.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewCustomers.ColumnHeadersDefaultCellStyle.BackColor;
            // Dies erzwingt, dass kein Rahmen gezeichnet wird, wenn die Zelle gemalt wird
            dataGridViewCustomers.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

            // Entfernt die Linien zwischen den Spaltenköpfen
            dataGridViewProducts.EnableHeadersVisualStyles = false;
            dataGridViewProducts.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewProducts.ColumnHeadersDefaultCellStyle.BackColor;
            // Dies erzwingt, dass kein Rahmen gezeichnet wird, wenn die Zelle gemalt wird
            dataGridViewProducts.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

            RefreshJournal();
            dataGridViewAssetsRefresh();
            tabPageOverviewRefresh();
            tabPageCustomersRefresh();
            tabPageProductsRefresh();

            LoadSettingsIntoUI();

            // 1. TABS AUSBLENDEN (TRICK)
            // Wir machen die Reiter 1 Pixel hoch und schalten auf Flat-Style um
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;

            tabControl.SelectedTab = tabPageOverview;
            HighlightButton(buttonOverview);

            CreateNewInvoice();
        }

        private void CreateNewInvoice()
        {
            currentInvoice = new Invoice();
            currentInvoice.DateCreated = DateTime.Now;
            currentInvoice.InvoiceNumber = ""; // Oder Logik für nächste Nummer

            // Jetzt das Grid an die Liste der Positionen hängen
            // currentInvoice.Lines muss eine BindingList sein!
            dataGridViewInvoiceEntry.DataSource = currentInvoice.Lines;
        }

        private void buttonEntry_Click(object sender, EventArgs e)
        {
            tabPageEntry.Refresh();
            dataGridViewJournal.Refresh();
            tabControl.SelectedTab = tabPageEntry;
            HighlightButton(buttonEntry);
        }

        // Kleiner Helfer, um den aktiven Button optisch hervorzuheben
        private void HighlightButton(Button activeBtn)
        {
            buttonAbout.BackColor = Color.DarkSlateBlue;
            buttonOverview.BackColor = Color.DarkSlateBlue;
            buttonEntry.BackColor = Color.DarkSlateBlue;
            buttonJournal.BackColor = Color.DarkSlateBlue;
            buttonAssets.BackColor = Color.DarkSlateBlue;
            buttonSettings.BackColor = Color.DarkSlateBlue;
            buttonExport.BackColor = Color.DarkSlateBlue;
            buttonCustomerEntry.BackColor = Color.DarkSlateBlue;
            buttonCustomers.BackColor = Color.DarkSlateBlue;
            buttonItemEntry.BackColor = Color.DarkSlateBlue;
            buttonItems.BackColor = Color.DarkSlateBlue;
            buttonInvoiceEntry.BackColor = Color.DarkSlateBlue;
            buttonInvoices.BackColor = Color.DarkSlateBlue;

            // Hebe den aktiven Button hervor (z.B. dunkleres Blau)
            activeBtn.BackColor = Color.DodgerBlue;
        }

        private void buttonJournal_Click(object sender, EventArgs e)
        {
            tabPageJournal.Refresh();
            dataGridViewJournal.Refresh();
            tabControl.SelectedTab = tabPageJournal;
            HighlightButton(buttonJournal);
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

        private void comboBoxEntryCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEntryCategory.SelectedItem != null)
            {
                string selectedCategory = comboBoxEntryCategory.SelectedItem.ToString();

                // Typ aus DB holen
                string type = dbManager.GetTransactionTypeFromCategory(selectedCategory);

                // In das nicht-editierbare Feld schreiben
                textBoxEntryPostingType.Text = type;

                // Kleiner Bonus: Farbe ändern für bessere Übersicht
                if (type == "Einnahme")
                {
                    textBoxEntryPostingType.BackColor = Color.LightGreen;
                }
                else
                {
                    textBoxEntryPostingType.BackColor = Color.LightCoral;
                }
            }
        }

        private void buttonVoucherPost_Click(object sender, EventArgs e)
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
                    InvoiceReference = checkBoxSettleInvoice.Checked ? comboBoxEntryTransactionOpenInvoices.SelectedValue?.ToString() : null
                };

                if (checkBoxSettleInvoice.Checked && comboBoxEntryTransactionOpenInvoices.SelectedValue != null)
                {
                    int selectedId = (int)comboBoxEntryTransactionOpenInvoices.SelectedValue;
                    dbManager.MarkInvoiceAsPaid(selectedId);
                }

                dbManager.PostEntryTransaction(entryTransaction);

                MessageBox.Show($"Erfolgreich gespeichert! Belegnummer: {voucher}", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshJournal();
                dataGridViewAssetsRefresh();
                tabPageOverviewRefresh();
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
        }

        private void RefreshJournal()
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
                    string[] hiddenCols = { "Voucher", "TransDate", "TransactionType", "EntryCategoryName", "Amount", "Description", "Reversal", "ReversalReferenceVoucher", "InvoiceReference", "AssetId" };
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

        // Kleine Hilfsmethode, um den Code noch cleaner zu machen
        private void SetColumnStyle(string name, string header, int width, int displayIndex)
        {
            if (dataGridViewJournal.Columns[name] != null)
            {
                var col = dataGridViewJournal.Columns[name];
                col.HeaderText = header;
                col.Width = width;
                col.DisplayIndex = displayIndex;
                col.Visible = true;
            }
        }

        void ConfigureDisplayColumn(string name, string header, int width, int displayIndex)
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

        private void dataGridViewJournal_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Wir prüfen bei jeder Zelle, welcher Typ in der aktuellen Zeile steht
            // (Damit wir die Farbe für die gesamte Zeile festlegen können)
            var row = dataGridViewJournal.Rows[e.RowIndex];

            // Prüfen, ob wir in der Spalte "Reversal" sind
            if (dataGridViewJournal.Columns[e.ColumnIndex].Name == "Reversal" && e.Value != null)
            {
                // Wir holen den Wert (0 oder 1)
                string val = e.Value.ToString();

                if (val == "1")
                {
                    e.Value = "Ja";
                    //e.CellStyle.ForeColor = Color.Red; // Stornos direkt rot markieren
                    //e.FormattingApplied = true; // Sagt dem Programm: "Ich hab's erledigt"
                }
                else if (val == "0")
                {
                    e.Value = "Nein";
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

        private void dataGridViewJournal_SelectionChanged(object sender, EventArgs e)
        {
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

        // Hilfsmethode zum Leeren der Felder
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
        }

        private void checkBoxReversal_CheckedChanged(object sender, EventArgs e)
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

        private void LoadVouchersIntoComboBox()
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

        private void comboBoxReferenceVoucher_SelectedIndexChanged(object sender, EventArgs e)
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

                    // --- FELDER SPERREN ---
                    SetEntryFieldsEnabled(false);
                }
            }
        }

        // Hilfsmethode, um den Code sauber zu halten
        private void SetEntryFieldsEnabled(bool enabled)
        {
            numericUpDownAmount.Enabled = enabled;
            comboBoxEntryCategory.Enabled = enabled;
            textBoxEntryPostingType.Enabled = enabled;
            textBoxText.Enabled = enabled;
            textBoxEntryNote.Enabled = enabled;
            dateTimePickerVoucherDate.Enabled = enabled;
        }

        private void buttonLoadFile_Click(object sender, EventArgs e)
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

        private void buttonOverview_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPageOverview;
            HighlightButton(buttonOverview);
        }

        private void buttonAssets_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPageAssets;
            HighlightButton(buttonAssets);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            comboBoxDataExportYearFillYearDropdown();
            tabControl.SelectedTab = tabPageDataexport;
            HighlightButton(buttonExport);

        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            LoadSettingsIntoUI();
            tabControl.SelectedTab = tabPageSettings;
            HighlightButton(buttonSettings);

        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPageAbout;
            HighlightButton(buttonAbout);
        }

        private void buttonJournalPicturePDF_Click(object sender, EventArgs e)
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

        private void ZeigeBildModal(string filePath)
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

        private void textBoxEntryPostingType_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewAssetsRefresh()
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

        private void SetAssetGridHeaders()
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

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabPageOverview)
            {
                tabPageOverviewRefresh();
            }
            else if (tabControl.SelectedTab == tabPageCustomers)
            {
                tabPageCustomersRefresh();
            }
        }

        private void tabPageOverviewRefresh()
        {
            try
            {
                // 1. Daten vom Manager holen
                DataRow stats = dbManager.FetchDashboardStats();

                if (stats != null)
                {
                    // 2. UI-Blöcke aktualisieren (Monate)
                    UpdateOverviewBlock(stats, "M0", textBoxOverviewRevenueCurrentMonth, textBoxOverviewCostsCurrentMonth, textBoxOverviewEarningsCurrentMonth);
                    UpdateOverviewBlock(stats, "M1", textBoxOverviewRevenuePreMonth, textBoxOverviewCostsPreMonth, textBoxOverviewEarningsPreMonth);
                    UpdateOverviewBlock(stats, "M2", textBoxOverviewRevenuePrePreMonth, textBoxOverviewCostsPrePreMonth, textBoxOverviewEarningsPrePreMonth);

                    // 3. UI-Blöcke aktualisieren (Jahre)
                    UpdateOverviewBlock(stats, "Y0", textBoxOverviewRevenueCurrentYear, textBoxOverviewCostsCurrentYear, textBoxOverviewEarningsCurrentYear);
                    UpdateOverviewBlock(stats, "Y1", textBoxOverviewRevenuePreYear, textBoxOverviewCostsPreYear, textBoxOverviewEarningsPreYear);
                    UpdateOverviewBlock(stats, "Y2", textBoxOverviewRevenuePrePreYear, textBoxOverviewCostsPrePreYear, textBoxOverviewEarningsPrePreYear);
                }

                // 4. Dynamische Labels setzen
                UpdateOverviewLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard Error: " + ex.Message);
            }
        }

        // Hilfsmethode für die Labels
        private void UpdateOverviewLabels()
        {
            labelOverviewCurrentMonth.Text = DateTime.Now.ToString("MMMM yyyy");
            labelOverviewPreMonth.Text = DateTime.Now.AddMonths(-1).ToString("MMMM yyyy");
            labelOverviewPrePreMonth.Text = DateTime.Now.AddMonths(-2).ToString("MMMM yyyy");
            labelOverviewCurrentYear.Text = "Jahr " + DateTime.Now.Year;
            labelOverviewPreYear.Text = "Jahr " + (DateTime.Now.Year - 1);
            labelOverviewPrePreYear.Text = "Jahr " + (DateTime.Now.Year - 2);
        }

        // Deine bestehende Block-Update-Logik (leicht angepasst auf DataRow)
        private void UpdateOverviewBlock(DataRow row, string suffix, TextBox txtRev, TextBox txtExp, TextBox txtEarn)
        {
            // Sicherer Abruf: Wenn die DB NULL liefert, nehmen wir 0
            decimal rev = row["Revenue" + suffix] == DBNull.Value ? 0 : Convert.ToDecimal(row["Revenue" + suffix]);
            decimal exp = row["Expenses" + suffix] == DBNull.Value ? 0 : Convert.ToDecimal(row["Expenses" + suffix]);

            decimal earn = rev - exp;

            txtRev.Text = rev.ToString("C2");
            txtExp.Text = exp.ToString("C2");
            txtEarn.Text = earn.ToString("C2");

            txtEarn.ForeColor = earn >= 0 ? Color.Green : Color.Red;
        }

        private void tabPageCustomerEntrySaveCustomer()
        {
            // 1. Model-Objekt befüllen
            Customer newCust = new Customer
            {
                Name = textBoxCustomerEntryName.Text.Trim(),
                Street = textBoxCustomerEntryStreet.Text.Trim(),
                Zipcode = textBoxCustomerEntryZipCode.Text.Trim(),
                City = textBoxCustomerEntryCity.Text.Trim(),
                Country = textBoxCustomerEntryCountry.Text.Trim(),
                Email = textBoxCustomerEntryEmail.Text.Trim()
            };

            // Validierung: Name ist Pflicht (Bleibt in der UI, da es die Benutzerführung betrifft)
            if (string.IsNullOrWhiteSpace(newCust.Name))
            {
                MessageBox.Show("Bitte geben Sie mindestens einen Namen ein.", "Eingabe fehlt");
                return;
            }

            try
            {
                // 2. Über Manager speichern und ID erhalten
                int newId = dbManager.SaveCustomer(newCust);

                MessageBox.Show($"Kunde erfolgreich unter ID {newId} angelegt!", "Erfolg");

                // 3. UI aufräumen
                tabPageCustomerEntryClear();
                tabPageCustomersRefresh(); // Falls vorhanden, Liste aktualisieren
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Datenbankfehler");
            }
        }

        private void tabPageCustomerEntryClear()
        {
            textBoxCustomerEntryName.Clear();
            textBoxCustomerEntryStreet.Clear();
            textBoxCustomerEntryZipCode.Clear();
            textBoxCustomerEntryCity.Clear();
            textBoxCustomerEntryCountry.Clear();
            textBoxCustomerEntryEmail.Clear();

            // Setzt den Fokus zurück auf das Namensfeld
            textBoxCustomerEntryName.Focus();
        }

        private void buttonCustomerEntrySave_Click(object sender, EventArgs e)
        {
            tabPageCustomerEntrySaveCustomer();
        }

        private void tabPageCustomersRefresh()
        {
            try
            {
                // 1. Daten vom Manager holen
                DataTable dt = dbManager.FetchAllCustomers();

                // 2. Binden
                dataGridViewCustomers.DataSource = null;
                dataGridViewCustomers.AutoGenerateColumns = true;
                dataGridViewCustomers.DataSource = dt;

                // 3. Optik-Finishing
                if (dataGridViewCustomers.Columns.Count > 0)
                {
                    ApplyCustomerGridStyle();
                }

                dataGridViewCustomers.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ApplyCustomerGridStyle()
        {
            // Umbruch-Style definieren
            DataGridViewCellStyle wrapStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True };
            dataGridViewCustomers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Spalten konfigurieren
            var cols = dataGridViewCustomers.Columns;

            if (cols["Id"] != null) cols["Id"].Width = 40;

            if (cols["Name"] != null)
            {
                cols["Name"].Width = 110;
                cols["Name"].DefaultCellStyle = wrapStyle;
            }

            if (cols["Street"] != null)
            {
                cols["Street"].Width = 120;
                cols["Street"].DefaultCellStyle = wrapStyle;
            }

            if (cols["Zipcode"] != null) cols["Zipcode"].Width = 70;

            if (cols["Email"] != null)
            {
                cols["Email"].Width = 220;
                cols["Email"].DefaultCellStyle = wrapStyle;
            }

            // Allgemeine Einstellungen
            dataGridViewCustomers.AllowUserToAddRows = false;
            dataGridViewCustomers.ReadOnly = true;
            dataGridViewCustomers.RowHeadersVisible = false;
            dataGridViewCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCustomers.MultiSelect = false;
            dataGridViewCustomers.EnableHeadersVisualStyles = false;
            dataGridViewCustomers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        }

        private void buttonCustomerEntry_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabPageCustomerEntry;
            HighlightButton(buttonCustomerEntry);
        }

        private void buttonCustomers_Click(object sender, EventArgs e)
        {
            tabPageCustomers.Refresh();
            tabControl.SelectedTab = tabPageCustomers;
            HighlightButton(buttonCustomers);
        }

        private void buttonProductEntryAddPicture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                // Filter für gängige Bildformate
                ofd.Filter = "Bilder (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Produktbild auswählen";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Bild in die PictureBox laden
                    pictureBoxProductEntry.Image = Image.FromFile(ofd.FileName);

                    // Die Dateiendung für die DB speichern
                    selectedFileExtensionProductPicture = Path.GetExtension(ofd.FileName).ToLower();
                }
            }
        }

        private void tabPageProductEntrySave()
        {
            // 1. Validierung (Pflichtfelder)
            string name = textBoxProductEntryName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Bitte geben Sie einen Produktnamen an!", "Eingabe fehlt");
                return;
            }

            // Preis-Validierung
            string priceRaw = textBoxProductEntryPrice.Text.Replace(",", ".");
            if (!decimal.TryParse(priceRaw, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Bitte geben Sie einen gültigen Preis ein!", "Ungültiger Wert");
                return;
            }

            try
            {
                // 2. Produkt-Objekt erstellen
                Product newProduct = new Product
                {
                    Name = name,
                    Description = textBoxProductEntryDescription.Text.Trim(),
                    Price = price,
                    Stocked = checkBoxProductEntryIsStocked.Checked ? 1 : 0
                };

                // Lagerbestand nur setzen, wenn Lagerführung aktiv ist
                if (newProduct.Stocked == 1)
                {
                    int.TryParse(textBoxProductEntryStock.Text, out int currentStock);
                    newProduct.Stock = currentStock;
                }
                else
                {
                    newProduct.Stock = 0;
                }

                // Bild-Daten aus den globalen Variablen der Form übernehmen
                if (pictureBoxProductEntry.Image != null)
                {
                    newProduct.Picture = selectedFileBytesProductPicture; // Die Bytes, die beim Laden des Bildes gespeichert wurden
                    newProduct.PictureExtension = selectedFileExtensionProductPicture;
                }

                // 3. Speichern über DBManager
                dbManager.SaveProduct(newProduct);

                MessageBox.Show("Produkt erfolgreich gespeichert!", "Erfolg");

                // 4. UI aufräumen
                tabPageProductEntryClear();
                // Falls du eine Liste hast: tabPageProductsRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message);
            }
        }

        private void buttonPictureEntry_LoadFile(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                // Filter für gängige Bildformate
                ofd.Filter = "Bilder (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Produktbild auswählen";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Datei in Bytes umwandeln
                    selectedFileBytesProductPicture = File.ReadAllBytes(ofd.FileName);
                    // Dateiendung merken
                    selectedFileExtensionProductPicture = Path.GetExtension(ofd.FileName);

                    labelSelectedFilename.Text = Path.GetFileName(ofd.FileName);

                    // Vorschau, falls es ein Bild ist
                    if (selectedFileExtensionProductPicture.ToLower() != ".pdf")
                    {
                        using (var ms = new MemoryStream(selectedFileBytesProductPicture))
                        {
                            pictureBoxProductEntry.Image = Image.FromStream(ms);
                        }
                    }
                }
            }
        }

        private void tabPageProductEntryClear()
        {
            textBoxProductEntryName.Clear();
            textBoxProductEntryDescription.Clear();
            textBoxProductEntryPrice.Clear();
            pictureBoxProductEntry.Image = null; // Bild löschen
            selectedFileExtensionProductPicture = "";           // Endung leeren
            selectedFileBytesProductPicture = null;
            checkBoxProductEntryIsStocked.Checked = false;
            textBoxProductEntryStock.Clear();
        }

        private void buttonProductEntrySave_Click(object sender, EventArgs e)
        {
            tabPageProductEntrySave();
            tabPageProductEntryClear();
            tabPageProductsRefresh();
        }

        private void buttonItemEntry_Click(object sender, EventArgs e)
        {
            tabPageProductEntry.Refresh();
            tabControl.SelectedTab = tabPageProductEntry;
            HighlightButton(buttonItemEntry);
        }

        private void buttonItems_Click(object sender, EventArgs e)
        {
            tabPageProducts.Refresh();
            tabControl.SelectedTab = tabPageProducts;
            HighlightButton(buttonItems);
        }

        private void tabPageProductsRefresh()
        {
            try
            {
                // 1. Daten ohne Bilder holen
                DataTable dt = dbManager.FetchProductList();

                // 2. Binden
                dataGridViewProducts.DataSource = null;
                dataGridViewProducts.AutoGenerateColumns = true;
                dataGridViewProducts.DataSource = dt;

                // 3. Optik-Finishing
                if (dataGridViewProducts.Columns.Count > 0)
                {
                    ApplyProductGridStyle();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ApplyProductGridStyle()
        {
            var cols = dataGridViewProducts.Columns;

            // Header-Texte verschönern
            if (cols.Contains("Price"))
            {
                cols["Price"].HeaderText = "Preis";
                cols["Price"].DefaultCellStyle.Format = "C2"; // C2 formatiert direkt als Währung (€)
                // cols["Price"].Width = 80;
                cols["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            if (cols.Contains("Stocked")) cols["Stocked"].HeaderText = "Lagerartikel";
            if (cols.Contains("Stock")) cols["Stock"].HeaderText = "Bestand";
            if (cols.Contains("Description")) cols["Description"].HeaderText = "Beschreibung";

            // Allgemeine Grid-Einstellungen
            dataGridViewProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewProducts.AllowUserToAddRows = false;
            dataGridViewProducts.ReadOnly = true;
            dataGridViewProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewProducts.RowHeadersVisible = false;
        }

        private void dataGridViewProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Wir prüfen, ob wir gerade die Spalte "Stocked" zeichnen
            if (dataGridViewProducts.Columns[e.ColumnIndex].Name == "Stocked" && e.Value != null)
            {
                int val = Convert.ToInt32(e.Value);
                e.Value = (val == 1) ? "Ja" : "Nein";
                e.FormattingApplied = true; // Sagt dem Grid: "Ich hab's erledigt, fass es nicht mehr an"
            }
        }

        private void dataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                // Sicherstellen, dass die Zelle "Id" existiert und nicht null ist
                var cellValue = dataGridViewProducts.SelectedRows[0].Cells["Id"].Value;
                if (cellValue != null && int.TryParse(cellValue.ToString(), out int productId))
                {
                    LoadProductDetails(productId);
                }
            }
        }

        private void LoadProductDetails(int productId)
        {
            try
            {
                // 1. Bild-Daten vom Manager holen
                byte[] imageBytes = dbManager.FetchProductPicture(productId);

                // 2. Vorhandenes Bild aufräumen (Speichermanagement)
                if (pictureBoxProducts.Image != null)
                {
                    pictureBoxProducts.Image.Dispose();
                    pictureBoxProducts.Image = null;
                }

                // 3. Umwandeln und anzeigen
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        // Wir erzeugen eine Kopie für die PictureBox, damit der Stream geschlossen werden kann
                        pictureBoxProducts.Image = Image.FromStream(ms);
                    }
                }
            }
            catch
            {
                pictureBoxProducts.Image = null; // Im Fehlerfall Bild leeren
            }
        }

        private void AutoSaveSettings()
        {
            Settings s = new Settings
            {
                CompanyName = textBoxSettingsCompanyName.Text.Trim(),
                Owner = textBoxSettingsOwner.Text.Trim(),
                Street = textBoxSettingsStreet.Text.Trim(),
                ZipCode = textBoxSettingsZipCode.Text.Trim(),
                City = textBoxSettingsCity.Text.Trim(),
                Country = textBoxSettingsCountry.Text.Trim(),
                Phone = textBoxSettingsPhone.Text.Trim(),
                Email = textBoxSettingsEmail.Text.Trim(),
                TaxNumber = textBoxSettingsTaxNumber.Text.Trim(),
                BankName = textBoxSettingsBankname.Text.Trim(),
                IBAN = textBoxSettingsIBAN.Text.Trim(),
                BIC = textBoxSettingsBIC.Text.Trim(),
                IntroText = textBoxSettingsInvoiceIntroText.Text.Trim(),
                FooterText = textBoxSettingsInvoiceFooterText.Text.Trim(),
                SmallBusinessNote = textBoxSettingsInvoiceSmallBusinessNote.Text.Trim(),

                // Nutze die globalen Variablen für das Logo
                CompanyImage = selectedFileBytesCompanyLogo,
                CompanyImageExtension = selectedFileExtensionCompanyLogo
            };

            dbManager.SaveSettings(s);
        }

        private void LoadSettingsIntoUI()
        {
            var s = dbManager.GetSettings();
            if (s == null) return;

            textBoxSettingsCompanyName.Text = s.CompanyName;
            textBoxSettingsOwner.Text = s.Owner;
            textBoxSettingsStreet.Text = s.Street;
            textBoxSettingsZipCode.Text = s.ZipCode;
            textBoxSettingsCity.Text = s.City;
            textBoxSettingsCountry.Text = s.Country;
            textBoxSettingsPhone.Text = s.Phone;
            textBoxSettingsEmail.Text = s.Email;
            textBoxSettingsTaxNumber.Text = s.TaxNumber;
            textBoxSettingsBankname.Text = s.BankName;
            textBoxSettingsIBAN.Text = s.IBAN;
            textBoxSettingsBIC.Text = s.BIC;
            textBoxSettingsInvoiceIntroText.Text = s.IntroText;
            textBoxSettingsInvoiceFooterText.Text = s.FooterText;
            textBoxSettingsInvoiceSmallBusinessNote.Text = s.SmallBusinessNote;
            /*
            if (s.CompanyImage != null)
            {
                using (var ms = new MemoryStream(s.CompanyImage))
                {
                    pictureBoxLogo.Image = Image.FromStream(ms);
                }
                selectedFileBytesCompanyLogo = s.CompanyImage;
                selectedFileExtensionCompanyLogo = s.CompanyImageExtension;
            }*/
        }

        private void buttonInvoiceEntry_Click(object sender, EventArgs e)
        {
            LoadProductsIntoGridComboBox();
            LoadCustomersIntoComboxBoxInvoiceEntryCustomers();
            CreateNewInvoice();
            SetupInvoiceGrid();
            tabPageInvoiceEntry.Refresh();
            tabControl.SelectedTab = tabPageInvoiceEntry;
            HighlightButton(buttonInvoiceEntry);
        }

        private void SetupInvoiceGrid()
        {
            dataGridViewInvoiceEntry.AutoGenerateColumns = false;
            dataGridViewInvoiceEntry.DataSource = currentInvoice.Lines;

            // Alle Spalten sauber mappen
            dataGridViewInvoiceEntry.Columns["colProductId"].DataPropertyName = "ProductId";
            dataGridViewInvoiceEntry.Columns["colProductName"].DataPropertyName = "ProductName";
            dataGridViewInvoiceEntry.Columns["colQuantity"].DataPropertyName = "Quantity";
            dataGridViewInvoiceEntry.Columns["colPrice"].DataPropertyName = "CurrentPrice";
            dataGridViewInvoiceEntry.Columns["colTotal"].DataPropertyName = "LineTotal";
        }

        private void LoadProductsIntoGridComboBox()
        {
            try
            {
                var products = dbManager.GetAllProducts();

                if (dataGridViewInvoiceEntry.Columns["colProductId"] is DataGridViewComboBoxColumn comboCol)
                {
                    comboCol.DataSource = products;
                    comboCol.DisplayMember = "Name";
                    comboCol.ValueMember = "Id";

                    // WICHTIG: Erlaube der Spalte, dass sie "nichts" (null) enthalten darf
                    comboCol.ValueType = typeof(int?);

                    comboCol.DefaultCellStyle.NullValue = null;

                    dataGridViewInvoiceEntry.Columns["colProductId"].ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Produkte: " + ex.Message);
            }
        }

        private void dataGridViewInvoiceEntry_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 1. Sicherstellen, dass wir nicht im Header sind und die richtige Spalte geändert wurde
            if (e.RowIndex < 0) return;

            // Prüfen, ob die Spalte 'colProductId' (deine ComboBox-Spalte) geändert wurde
            if (dataGridViewInvoiceEntry.Columns[e.ColumnIndex].Name == "colProductId")
            {
                var row = dataGridViewInvoiceEntry.Rows[e.RowIndex];
                var cell = row.Cells["colProductId"];

                if (cell.Value != null && cell.Value != DBNull.Value)
                {
                    int selectedProductId = (int)cell.Value;

                    // Das Produkt-Objekt aus der Liste der ComboBox suchen
                    var comboCol = (DataGridViewComboBoxColumn)dataGridViewInvoiceEntry.Columns["colProductId"];
                    var productList = (List<Product>)comboCol.DataSource;
                    var selectedProduct = productList.FirstOrDefault(p => p.Id == selectedProductId);

                    if (selectedProduct != null)
                    {
                        // Jetzt befüllen wir die anderen Spalten automatisch
                        row.Cells["colProductName"].Value = selectedProduct.Name;
                        row.Cells["colPrice"].Value = selectedProduct.Price;

                        // Standardmäßig Menge 1 setzen, falls noch nichts drin steht
                        if (row.Cells["colQuantity"].Value == null || row.Cells["colQuantity"].Value == DBNull.Value)
                        {
                            row.Cells["colQuantity"].Value = 1.0;
                        }

                        // Zeilensumme berechnen
                        double qty = Convert.ToDouble(row.Cells["colQuantity"].Value);
                        double price = Convert.ToDouble(selectedProduct.Price); // Preis aus dem Produkt-Objekt

                        // Jetzt die Berechnung und Zuweisung
                        row.Cells["colTotal"].Value = qty * price;

                        // Wichtig: Auch das Model im Hintergrund (InvoiceLine) aktualisieren
                        // Falls du BindingList nutzt, wird das oft automatisch gemacht, 
                        // aber sicher ist sicher:
                        // UpdateGrandTotal();
                    }
                }
            }
            // Falls Menge oder Preis manuell geändert werden (nach der Auswahl)
            else if (dataGridViewInvoiceEntry.Columns[e.ColumnIndex].Name == "colQuantity" ||
                     dataGridViewInvoiceEntry.Columns[e.ColumnIndex].Name == "colPrice")
            {
                var row = dataGridViewInvoiceEntry.Rows[e.RowIndex];
                double qty = Convert.ToDouble(row.Cells["colQuantity"].Value ?? 0);
                double price = Convert.ToDouble(row.Cells["colPrice"].Value ?? 0);
                row.Cells["colTotal"].Value = qty * price;

                // UpdateGrandTotal();
            }
        }

        private void buttonInvoiceEntryPost_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validierung: Hat sie überhaupt etwas eingegeben?
                if (currentInvoice.Lines.Count == 0)
                {
                    MessageBox.Show("Die Rechnung enthält keine Positionen.", "Stopp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Speichern über den DBManager
                // Die Rechnungsnummer wird innerhalb dieser Methode automatisch generiert
                dbManager.SaveFullInvoice(currentInvoice);

                // 3. Erfolgsmeldung mit der neuen Nummer
                MessageBox.Show($"Rechnung {currentInvoice.InvoiceNumber} wurde erfolgreich gespeichert!",
                                "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 4. Formular für die nächste Rechnung leeren
                ResetInvoiceForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetInvoiceForm()
        {
            // Ein ganz neues Objekt anlegen
            currentInvoice = new Invoice();
            currentInvoice.DateCreated = DateTime.Now;

            // Das Grid an die neue (leere) Liste binden
            dataGridViewInvoiceEntry.DataSource = currentInvoice.Lines;

            // Falls du Textboxen für Kunde oder Notizen hast, diese leeren
            // txtCustomerName.Clear();
            // txtTotalAmount.Text = "0,00 €";

            // Fokus wieder auf das erste Feld (z.B. Produkt-Auswahl), 
            // damit sie sofort weitertippen kann
            dataGridViewInvoiceEntry.Focus();
        }

        private void LoadCustomersIntoComboxBoxInvoiceEntryCustomers()
        {
            // Daten vom DBManager holen
            var customers = dbManager.GetAllCustomers();

            // Die ComboBox konfigurieren
            comboxBoxInoviceEntryCustomers.DataSource = customers;
            comboxBoxInoviceEntryCustomers.DisplayMember = "Name"; // Was angezeigt wird
            comboxBoxInoviceEntryCustomers.ValueMember = "Id";     // Was im Hintergrund als Wert zählt

            // Optional: Erstmal keinen Kunden auswählen
            comboxBoxInoviceEntryCustomers.SelectedIndex = -1;
        }

        private void comboxBoxInoviceEntryCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboxBoxInoviceEntryCustomers.SelectedValue != null && comboxBoxInoviceEntryCustomers.SelectedValue is int customerId)
            {
                // Wir speichern die ID im aktuellen Invoice-Objekt
                currentInvoice.CustomerId = customerId;
            }
        }

        private void dataGridViewInvoicesRefresh()
        {
            // 1. Daten holen
            var invoices = dbManager.GetAllInvoices();

            // 2. Automatische Spalten deaktivieren
            dataGridViewInvoices.AutoGenerateColumns = false;

            // 3. Spalten definieren (nur falls sie noch nicht existieren)
            if (dataGridViewInvoices.Columns.Count == 0)
            {
                dataGridViewInvoices.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "InvoiceNumber",
                    HeaderText = "Rechnungs-Nr.",
                    Name = "InvoiceNumber"
                });

                dataGridViewInvoices.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "CustomerId",
                    HeaderText = "Kundennummer",
                    Name = "CustomerId"
                });

                dataGridViewInvoices.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "DateCreated",
                    HeaderText = "Erstellungsdatum",
                    Name = "DateCreated",
                    DefaultCellStyle = { Format = "dd.MM.yyyy" }
                });

                dataGridViewInvoices.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "TotalAmount",
                    HeaderText = "Gesamtbetrag",
                    Name = "TotalAmount",
                    DefaultCellStyle = { Format = "C2" } // Währungsformat
                });
            }

            // 4. Daten binden
            dataGridViewInvoices.DataSource = new BindingList<Invoice>(invoices);

            // Kleiner Bonus: Spaltenbreite automatisch anpassen
            dataGridViewInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Der Button-Klick für die PDF-Anzeige
        private void buttonInvoicesPDF_Click(object sender, EventArgs e)
        {
            if (dataGridViewInvoices.CurrentRow != null)
            {
                // 1. Die ID der gewählten Rechnung aus dem Grid holen
                var selectedInvoice = (Invoice)dataGridViewInvoices.CurrentRow.DataBoundItem;

                // 2. Das BLOB gezielt über die ID nachladen
                byte[] pdfData = dbManager.GetInvoiceBlob(selectedInvoice.Id);

                if (pdfData != null && pdfData.Length > 0)
                {
                    try
                    {
                        // 3. Temporäre Datei im Windows-Temp-Verzeichnis erstellen
                        string tempPath = Path.Combine(Path.GetTempPath(), $"Rechnung_{selectedInvoice.InvoiceNumber}.pdf");

                        // Datei schreiben
                        File.WriteAllBytes(tempPath, pdfData);

                        // 4. Die PDF öffnen
                        Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler beim Öffnen des PDF-Anhangs: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Zu dieser Rechnung wurde kein PDF-Anhang gefunden.", "Info");
                }
            }
        }

        private void buttonInvoices_Click(object sender, EventArgs e)
        {
            dataGridViewInvoicesRefresh();
            tabPageInvoices.Refresh();
            tabControl.SelectedTab = tabPageInvoices;
            HighlightButton(buttonInvoices);
        }

        private void checkBoxSettleInvoice_CheckedChanged(object sender, EventArgs e)
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

        private void comboBoxEntryTransactionOpenInvoices_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // 1. Das ausgewählte Rechnungs-Objekt holen
            if (comboBoxEntryTransactionOpenInvoices.SelectedItem is Invoice selectedInv)
            {
                // Betrag automatisch aus der Rechnung in das Betrags-Feld übernehmen
                textBoxJournalAmount.Text = selectedInv.TotalAmount.ToString();
                textBoxText.Text = $"Zahlung zu Rechnung {selectedInv.InvoiceNumber}";

                // Automatisierung: Typ und Kategorie für die Frau vorfüllen
                // (Ersetze 'txtType'/'txtCategory' durch deine tatsächlichen Control-Namen)
                comboBoxEntryCategory.Text = "Produktverkauf";
                textBoxEntryPostingType.Text = "Einnahme";
            }
        }

        private void buttonInvoicesIsCancelled_Clicked(object sender, EventArgs e)
        {
            // Wir reagieren nur, wenn die Checkbox manuell angehakt wird
            if (buttonInvoicesIsCancelled.Checked && buttonInvoicesIsCancelled.Enabled)
            {
                // 1. Prüfen, ob überhaupt eine Zeile im Grid ausgewählt ist
                if (dataGridViewInvoices.CurrentRow != null)
                {
                    // 2. Das dahinterliegende Invoice-Objekt extrahieren
                    var selectedInvoice = (Invoice)dataGridViewInvoices.CurrentRow.DataBoundItem;

                    // 3. Die ID nutzen
                    int currentId = selectedInvoice.Id;

                    var result = MessageBox.Show($"Möchten Sie die Rechnung {selectedInvoice.InvoiceNumber} wirklich stornieren?",
                                                 "Bestätigung", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // In DB auf storniert setzen
                        dbManager.MarkInvoiceAsCancelled(currentId, true);

                        // Control sperren
                        buttonInvoicesIsCancelled.Enabled = false;

                        // Grid aktualisieren, damit der Status (IsCancelled) auch im Grid ankommt
                        dataGridViewInvoicesRefresh();
                    }
                    else
                    {
                        // Haken zurücksetzen, wenn abgebrochen wurde
                        buttonInvoicesIsCancelled.Checked = false;
                    }
                }
                else
                {
                    MessageBox.Show("Bitte wählen Sie zuerst eine Rechnung in der Liste aus.");
                    buttonInvoicesIsCancelled.Checked = false;
                }
            }
        }

        private void dataGridViewInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewInvoices.CurrentRow != null)
            {
                var inv = (Invoice)dataGridViewInvoices.CurrentRow.DataBoundItem;

                // Checkbox auf den Wert der Rechnung setzen
                buttonInvoicesIsCancelled.Checked = inv.IsCancelled;

                // Wenn storniert, dann deaktivieren, sonst aktiv lassen
                buttonInvoicesIsCancelled.Enabled = !inv.IsCancelled;

                textBoxInvoicesIsCancelled.Text = inv.IsCancelled ? "Ja" : "Nein";
                textBoxInvoicesIsPaid.Text = inv.IsPaid == 1 ? "Ja" : "Nein";

                LoadInvoiceDetails(inv.Id);
            }
        }

        private void LoadInvoiceDetails(int invoiceId)
        {
            var lines = dbManager.GetInvoiceLines(invoiceId);

            dataGridViewInvoicesLines.AutoGenerateColumns = false;
            dataGridViewInvoicesLines.Columns.Clear(); // Falls du sie nicht im Designer festlegst

            // Spalten definieren
            dataGridViewInvoicesLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LineNum",
                HeaderText = "Pos.",
                Width = 40
            });

            dataGridViewInvoicesLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductName",
                HeaderText = "Produkt",
                Width = 225
            });

            dataGridViewInvoicesLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                HeaderText = "Menge",
                Width = 80,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dataGridViewInvoicesLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CurrentPrice",
                HeaderText = "Einzelpreis",
                Width = 100,
                DefaultCellStyle = { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dataGridViewInvoicesLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LineTotal",
                HeaderText = "Gesamtpreis",
                Width = 100,
                DefaultCellStyle = { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // Daten binden
            dataGridViewInvoicesLines.DataSource = new BindingList<InvoiceLine>(lines);
        }

        private void comboBoxDataExportYearFillYearDropdown()
        {
            int currentYear = DateTime.Now.Year;
            comboBoxDataExportYear.Items.Clear();
            for (int i = 0; i < 6; i++)
            {
                comboBoxDataExportYear.Items.Add(currentYear - i);
            }
            comboBoxDataExportYear.SelectedIndex = 1; // Standardmäßig das letzte Jahr
        }

        private void buttonDataExportExportData_Click(object sender, EventArgs e)
        {
            // Sicherstellen, dass ein Jahr ausgewählt ist
            if (comboBoxDataExportYear.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie ein Jahr aus.");
                return;
            }

            int selectedYear = (int)comboBoxDataExportYear.SelectedItem;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Datei|*.csv";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sfd.FileName = $"Belegbuchungen_{selectedYear}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 1. Daten mit allen Spalten aus dem DBManager holen
                    DataTable dt = dbManager.GetFullTransactionTableByYear(selectedYear);

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        MessageBox.Show($"Keine Buchungsdaten für das Jahr {selectedYear} gefunden.",
                                        "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // 2. Export starten
                    ExportToCSV(sfd.FileName, dt);

                    MessageBox.Show($"Der Export von {dt.Rows.Count} Datensätzen für das Jahr {selectedYear} war erfolgreich.",
                                    "Export abgeschlossen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kritischer Fehler beim Export: " + ex.Message,
                                    "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void ExportToCSV(string filePath, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            // Spaltenüberschriften
            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                     .Select(column => column.ColumnName)
                                     .ToArray();
            sb.AppendLine(string.Join(";", columnNames));

            // Datenzeilen
            foreach (DataRow row in dt.Rows)
            {
                var fields = row.ItemArray.Select(field =>
                {
                    if (field == null || field == DBNull.Value) return "";

                    string val = field.ToString();

                    // Semikolon verhindern (zerstört sonst die CSV-Struktur)
                    val = val.Replace(";", ",");
                    // Zeilenumbrüche in Beschreibungen entfernen
                    val = val.Replace(Environment.NewLine, " ").Replace("\n", " ");

                    // Spezialbehandlung für Zahlen (Punkt zu Komma für deutsches Excel)
                    if (field is decimal || field is double || field is float)
                    {
                        val = val.Replace(".", ",");
                    }

                    return val;
                });

                sb.AppendLine(string.Join(";", fields));
            }

            // Mit UTF-8 Encoding speichern, damit Umlaute korrekt sind
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private void btnExportEuerFinal_Click(object sender, EventArgs e)
        {
            if (comboBoxDataExportYear.SelectedItem == null) return;
            int year = (int)comboBoxDataExportYear.SelectedItem;

            // 1. Speicherdialog konfigurieren
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Datei|*.csv";
            sfd.FileName = $"EÜR_Zusammenfassung_{year}.csv";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 2. Daten laden
                    DataTable summary = dbManager.GetEuerSummaryTable(year);
                    decimal afaYear = dbManager.GetAfaSumForYear(year);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"EÜR ZUSAMMENFASSUNG;Jahr: {year};Erstellt am: {DateTime.Now:dd.MM.yyyy}");
                    sb.AppendLine("Kategorie;Typ;Summe (EUR)");

                    decimal einnahmen = 0;
                    decimal ausgaben = 0;

                    foreach (DataRow row in summary.Rows)
                    {
                        string cat = row["Kategorie"].ToString();
                        string type = row["Typ"].ToString();
                        decimal amt = Convert.ToDecimal(row["Gesamtsumme"]);

                        // Anlagen-Käufe werden in der EÜR nicht direkt als Ausgabe gezählt, 
                        // sondern nur über die AfA (Abschreibung). Daher markieren wir sie nur.
                        if (cat.Contains("AfA") || cat.Contains("Anlage"))
                        {
                            sb.AppendLine($"{cat} (Anschaffungskosten - Info);Info;{amt:F2}".Replace(".", ","));
                            continue;
                        }

                        if (type == "Einnahme") einnahmen += amt;
                        else ausgaben += amt;

                        sb.AppendLine($"{cat};{type};{amt:F2}".Replace(".", ","));
                    }

                    // 3. Verfeinerte AfA als Ausgabe hinzufügen
                    ausgaben += afaYear;
                    sb.AppendLine($"Abschreibungen (AfA monatsgenau);Ausgabe;{afaYear:F2}".Replace(".", ","));

                    // 4. Abschlussrechnung
                    sb.AppendLine("");
                    sb.AppendLine($";GESAMT EINNAHMEN;{einnahmen:F2}".Replace(".", ","));
                    sb.AppendLine($";GESAMT AUSGABEN;{ausgaben:F2}".Replace(".", ","));
                    sb.AppendLine($";ERGEBNIS (Gewinn/Verlust);{einnahmen - ausgaben:F2}".Replace(".", ","));

                    // 5. Datei schreiben
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                    MessageBox.Show($"EÜR für {year} wurde erfolgreich gespeichert!\n\nGewinn/Verlust: {einnahmen - ausgaben:C2}",
                                    "Export Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim EÜR-Export: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool PerformLicenseCheck()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Ich habe den Namen hier an deine Ordnerstruktur angepasst
            string folder = Path.Combine(appData, "Mirada-Finanza-Control-Central");
            string licensePath = Path.Combine(folder, "license.txt");

            try
            {
                // ERSTELLT DEN ORDNER, FALLS ER FEHLT
                // Wenn er schon existiert, passiert einfach gar nichts (kein Fehler)
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                if (File.Exists(licensePath))
                {
                    string[] lines = File.ReadAllLines(licensePath);
                    if (lines.Length >= 2)
                    {
                        string userName = lines[0].Trim();
                        string licenseKey = lines[1].Trim();

                        if (LicenseValidator.IsLicenseValid(userName, licenseKey))
                        {
                            return true; // Lizenz ok!
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: Fehler beim Erstellen des Ordners oder Lesen der Datei loggen
            }

            // Wenn wir hier landen: Ordner existiert nun zwar, aber keine gültige Lizenz gefunden
            MessageBox.Show(
                "Keine gültige Lizenz gefunden!\n\n" +
                $"Bitte hinterlegen Sie die Datei 'license.txt' in folgendem Ordner:\n{folder}\n\n" +
                "Das Programm wird nun beendet.",
                "Lizenz erforderlich",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }
    }
}
