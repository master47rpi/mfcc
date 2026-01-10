using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

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

        MaskTabPageEntryInternal maskTabPageEntryInternal;
        MaskTabPageEntryExternal maskTabPageEntryExternal;
        MaskTabPageOverview maskTabPageOverview;
        MaskTabPageJournal maskTabPageJournal;
        MaskTabPageAsset maskTabPageAsset;
        MaskTabPageCustomerEntry maskTabPageCustomerEntry;
        MaskTabPageCustomers maskTabPageCustomers;

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
            dataGridViewCustomers.EnableHeadersVisualStyles = false;
            dataGridViewCustomers.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewCustomers.ColumnHeadersDefaultCellStyle.BackColor;
            // Dies erzwingt, dass kein Rahmen gezeichnet wird, wenn die Zelle gemalt wird
            dataGridViewCustomers.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

            // Entfernt die Linien zwischen den Spaltenköpfen
            dataGridViewProducts.EnableHeadersVisualStyles = false;
            dataGridViewProducts.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridViewProducts.ColumnHeadersDefaultCellStyle.BackColor;
            // Dies erzwingt, dass kein Rahmen gezeichnet wird, wenn die Zelle gemalt wird
            dataGridViewProducts.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

            tabPageOverviewRefresh();
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

            maskTabPageEntryInternal = new MaskTabPageEntryInternal(
                this.tabControl,
                this.tabPageEntryInternal,
                this.buttonEntryInternal,
                this.dbManager);

            maskTabPageEntryExternal = new MaskTabPageEntryExternal(
                this.tabControl,
                this.tabPageEntry,
                this.buttonEntry,
                this.dbManager);

            maskTabPageOverview = new MaskTabPageOverview(
                this.tabControl,
                this.tabPageOverview,
                this.buttonOverview,
                this.dbManager);

            maskTabPageJournal = new MaskTabPageJournal(
                this.tabControl,
                this.tabPageJournal,
                this.buttonJournal,
                this.dbManager);

            maskTabPageAsset = new MaskTabPageAsset(
                this.tabControl,
                this.tabPageAssets,
                this.buttonAssets,
                this.dbManager);

            maskTabPageCustomerEntry = new MaskTabPageCustomerEntry(
                this.tabControl,
                this.tabPageCustomerEntry,
                this.buttonCustomersNew,
                this.dbManager);

            maskTabPageCustomers = new MaskTabPageCustomers(
                this.tabControl,
                this.tabPageCustomers,
                this.buttonCustomers,
                this.dbManager);
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
            maskTabPageEntryExternal.Show();
        }

        // Kleiner Helfer, um den aktiven Button optisch hervorzuheben
        private void HighlightButton(Button activeBtn)
        {
            buttonAbout.BackColor = Color.DarkSlateBlue;
            buttonSettings.BackColor = Color.DarkSlateBlue;
            buttonExport.BackColor = Color.DarkSlateBlue;
            buttonSuppliers.BackColor = Color.DarkSlateBlue;
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
            maskTabPageJournal.Show();
        }

        private void comboBoxEntryCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.maskTabPageEntryExternal.comboBoxEntryCategorySelectedIndexChanged();
        }

        private void buttonVoucherPost_Click(
            object _sender,
            EventArgs _e)
        {
            maskTabPageEntryExternal.buttonVoucherPostClick(
                _sender,
                _e);
        }

        private void dataGridViewJournal_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            maskTabPageJournal.dataGridViewJournalCellFormatting(
                sender,
                e);
        }

        private void dataGridViewJournal_SelectionChanged(
            object _sender,
            EventArgs _e)
        {
            maskTabPageJournal.dataGridViewJournalSelectionChanged(
                _sender,
                _e);
        }

        private void checkBoxReversal_CheckedChanged(
            object _sender,
            EventArgs _e)
        {
            maskTabPageEntryExternal.checkBoxReversalCheckedChanged(
                _sender,
                _e);
        }

        private void comboBoxReferenceVoucher_SelectedIndexChanged(
            object _sender,
            EventArgs _e)
        {
            maskTabPageEntryExternal.comboBoxReferenceVoucherSelectedIndexChanged(
                _sender,
                _e);
        }

        private void buttonLoadFile_Click(
            object _sender,
            EventArgs _e)
        {
            maskTabPageEntryExternal.buttonLoadFileClick(
                _sender,
                _e);
        }

        private void buttonOverview_Click(object sender, EventArgs e)
        {
            maskTabPageOverview.Show();
        }

        private void buttonAssets_Click(object sender, EventArgs e)
        {
            maskTabPageAsset.Show();
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

        private void buttonJournalPicturePDF_Click(
            object _sender,
            EventArgs _e)
        {
            maskTabPageJournal.buttonJournalPicturePDFClick(
                _sender,
                _e);
        }

        private void textBoxEntryPostingType_TextChanged(
            object _sender,
            EventArgs _e)
        {
            maskTabPageEntryExternal.textBoxEntryPostingTypeTextChanged(
                _sender,
                _e);
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabPageOverview)
            {
                tabPageOverviewRefresh();
            }
            else if (tabControl.SelectedTab == tabPageSuppliers)
            {
                tabPageSuppliersRefresh();
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

        private void buttonCustomerEntrySave_Click(object sender, EventArgs e)
        {
            maskTabPageCustomerEntry.buttonCustomerEntrySaveClick();
        }

        private void tabPageSuppliersRefresh()
        {
            try
            {
                // 1. Daten vom Manager holen
                DataTable dt = dbManager.FetchAllSuppliers();

                // 2. Binden
                dataGridViewSuppliers.DataSource = null;
                dataGridViewSuppliers.AutoGenerateColumns = false;

                dataGridViewSuppliers.Columns[0].DataPropertyName = "Id";
                dataGridViewSuppliers.Columns[1].DataPropertyName = "Name";


                dataGridViewSuppliers.DataSource = dt;

                // 3. Optik-Finishing
                if (dataGridViewSuppliers.Columns.Count > 0)
                {
                    ApplySupplierGridStyle();
                }

                dataGridViewSuppliers.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ApplySupplierGridStyle()
        {
            // Umbruch-Style definieren
            DataGridViewCellStyle wrapStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True };
            dataGridViewSuppliers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Allgemeine Einstellungen
            dataGridViewSuppliers.AllowUserToAddRows = false;
            dataGridViewSuppliers.ReadOnly = true;
            dataGridViewSuppliers.RowHeadersVisible = false;
            dataGridViewSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewSuppliers.MultiSelect = false;
            dataGridViewSuppliers.EnableHeadersVisualStyles = false;
            dataGridViewSuppliers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        }

        private void buttonSuppliers_Click(object sender, EventArgs e)
        {
            tabPageSuppliers.Refresh();
            tabControl.SelectedTab = tabPageSuppliers;
            HighlightButton(buttonSuppliers);
        }

        private void buttonCustomers_Click(object sender, EventArgs e)
        {
            maskTabPageCustomers.Show();
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

        private void checkBoxSettleInvoice_CheckedChanged(
            object _sender,
            EventArgs _e)
        {
            maskTabPageEntryExternal.checkBoxSettleInvoiceCheckedChanged(
                _sender,
                _e);
        }

        private void comboBoxEntryTransactionOpenInvoices_SelectionChangeCommitted(
            object _sender,
            EventArgs _e)
        {
            maskTabPageEntryExternal.comboBoxEntryTransactionOpenInvoicesSelectionChangeCommitted(
                _sender,
                _e);
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

        private void dataGridViewCustomers_SelectionChanged(object sender, EventArgs e)
        {
            maskTabPageCustomers.dataGridViewCustomersSelectionChanged();
        }

        private void buttonCustomersNew_Click(
            object _sender,
            EventArgs _e)
        {
            maskTabPageCustomers.buttonCustomersNewClick(
                _sender,
                _e);
        }

        private void textBoxCustomersName_Leave(object sender, EventArgs e)
        {
            maskTabPageCustomers.textBoxCustomersNameLeave();
        }

        private void textBoxCustomersStreet_Leave(object sender, EventArgs e)
        {
            maskTabPageCustomers.textBoxCustomersStreetLeave();
        }

        private void textBoxCustomersZipCode_Leave(object sender, EventArgs e)
        {
            maskTabPageCustomers.textBoxCustomersZipCodeLeave();
        }

        private void textBoxCustomersCity_Leave(object sender, EventArgs e)
        {
            maskTabPageCustomers.textBoxCustomersCityLeave();
        }

        private void textBoxCustomersCountry_Leave(object sender, EventArgs e)
        {
            maskTabPageCustomers.textBoxCustomersCountryLeave();
        }

        private void textBoxCustomersEmail_Leave(object sender, EventArgs e)
        {
            maskTabPageCustomers.textBoxCustomersEmailLeave();
        }

        private void tabPageCustomers_Leave(object sender, EventArgs e)
        {
            maskTabPageCustomers.tabPageCustomersLeave();
        }

        private void buttonCustomersDelete_Click(
            object _sender,
            EventArgs _e)
        {
            maskTabPageCustomers.buttonCustomersDeleteClick(
                _sender,
                _e);
        }

        private void dataGridViewSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            // 1. Sicherheitscheck: Gibt es eine aktive Zeile mit Daten?
            if (dataGridViewSuppliers.CurrentRow != null &&
                dataGridViewSuppliers.CurrentRow.DataBoundItem != null)
            {
                // 2. Die ID aus der Zelle holen (Achte darauf, dass "Id" der Name der Spalte im Designer ist!)
                var cellValue = dataGridViewSuppliers.CurrentRow.Cells[0].Value;

                if (cellValue != null && int.TryParse(cellValue.ToString(), out int selectedId))
                {
                    // 3. Daten laden
                    Supplier supplier = dbManager.GetSupplierById(selectedId);

                    if (supplier != null)
                    {
                        // 4. Textboxen befüllen
                        textBoxSuppliersId.Text = supplier.Id.ToString();
                        textBoxSuppliersName.Text = supplier.Name;
                        textBoxSuppliersStreet.Text = supplier.Street;
                        textBoxSuppliersCity.Text = supplier.City;
                        textBoxSuppliersZipCode.Text = supplier.Zipcode;
                        textBoxSuppliersCountry.Text = supplier.Country;
                        textBoxSuppliersEmail.Text = supplier.Email;

                        textBoxCustomersName.Focus();
                        buttonSuppliersDelete.Enabled = true;
                    }
                }
            }
            else
            {
                // Nichts ausgewählt -> Felder leeren
                ClearSupplierDetailFields();
                buttonSuppliersDelete.Enabled = false;
            }
        }

        private void buttonSuppliersNew_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Nur die ID in der Datenbank reservieren
                int newId = dbManager.CreateEmptySupplier();

                // 2. Das Grid aktualisieren (ruft deine Fetch-Methode auf)
                tabPageSuppliersRefresh();

                // 3. Den neuen Eintrag im Grid suchen und selektieren
                foreach (DataGridViewRow row in dataGridViewSuppliers.Rows)
                {
                    if (Convert.ToInt32(row.Cells[0].Value) == newId)
                    {
                        row.Selected = true;
                        dataGridViewSuppliers.CurrentCell = row.Cells[0];
                        break;
                    }
                }

                textBoxSuppliersId.Text = newId.ToString();

                // Andere Infos von DB holen.
                Supplier supplier = dbManager.GetSupplierById(newId);
                textBoxSuppliersId.Text = supplier.Id.ToString();
                textBoxSuppliersName.Text = supplier.Name;
                textBoxSuppliersStreet.Text = supplier.Street;
                textBoxSuppliersCity.Text = supplier.City;
                textBoxSuppliersZipCode.Text = supplier.Zipcode;
                textBoxSuppliersCountry.Text = supplier.Country;
                textBoxSuppliersEmail.Text = supplier.Email;

                textBoxSuppliersName.Focus();
                buttonSuppliersDelete.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen der Lieferantennummer: " + ex.Message);
            }
        }

        private void SaveCurrentSupplierFromUI()
        {
            // 1. Validierung: Haben wir überhaupt eine ID?
            if (string.IsNullOrWhiteSpace(textBoxSuppliersId.Text) || !int.TryParse(textBoxSuppliersId.Text, out int supplierId))
            {
                return;
            }

            try
            {
                // 2. Objekt mit den aktuellen Werten aus den Textboxen erstellen
                Supplier supplierToUpdate = new Supplier
                {
                    Id = supplierId,
                    Name = textBoxSuppliersName.Text.Trim(),
                    Street = textBoxSuppliersStreet.Text.Trim(),
                    Zipcode = textBoxSuppliersZipCode.Text.Trim(),
                    City = textBoxSuppliersCity.Text.Trim(),
                    Country = textBoxSuppliersCountry.Text.Trim(),
                    Email = textBoxSuppliersEmail.Text.Trim()
                };

                // 3. Ab in die Datenbank damit
                dbManager.UpdateSupplier(supplierToUpdate);

                // 4. Das Grid links aktualisieren, damit dort nicht noch der alte Name steht
                if (dataGridViewSuppliers.CurrentRow != null)
                {
                    DataRowView row = (DataRowView)dataGridViewSuppliers.CurrentRow.DataBoundItem;
                    // Nur aktualisieren, wenn der Wert im Grid wirklich anders ist (verhindert Flackern)
                    if (row[1].ToString() != supplierToUpdate.Name)
                    {
                        row[1] = supplierToUpdate.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim automatischen Speichern: " + ex.Message);
            }
        }

        private void textBoxSuppliersName_Leave(object sender, EventArgs e)
        {
            SaveCurrentSupplierFromUI();
        }

        private void textBoxSuppliersStreet_Leave(object sender, EventArgs e)
        {
            SaveCurrentSupplierFromUI();
        }

        private void textBoxSuppliersZipCode_Leave(object sender, EventArgs e)
        {
            SaveCurrentSupplierFromUI();
        }

        private void textBoxSuppliersCity_Leave(object sender, EventArgs e)
        {
            SaveCurrentSupplierFromUI();
        }

        private void textBoxSuppliersCountry_Leave(object sender, EventArgs e)
        {
            SaveCurrentSupplierFromUI();
        }

        private void textBoxSuppliersEmail_Leave(object sender, EventArgs e)
        {
            SaveCurrentSupplierFromUI();
        }

        private void tabPageSuppliers_Leave(object sender, EventArgs e)
        {
            SaveCurrentSupplierFromUI();
        }

        private void buttonSuppliersDelete_Click(object sender, EventArgs e)
        {
            // 1. Prüfen, ob überhaupt jemand im Grid ausgewählt ist
            if (dataGridViewSuppliers.CurrentRow == null) return;

            // 2. ID und Name für die Bestätigung holen
            // "Id" muss der Name deiner Spalte im DataGridView-Designer sein
            int id = Convert.ToInt32(dataGridViewSuppliers.CurrentRow.Cells[0].Value);
            string name = textBoxSuppliersName.Text; // Wir nehmen den Namen direkt aus der Textbox

            // 3. Sicherheitsabfrage
            var result = MessageBox.Show(
                $"Möchtest du den Lieferanten '{name}' (ID: {id}) wirklich unwiderruflich löschen?",
                "Löschen bestätigen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 4. Aus DB löschen
                    dbManager.DeleteSupplier(id);

                    // 5. UI aufräumen
                    tabPageSuppliersRefresh(); // Grid neu laden
                    ClearSupplierDetailFields(); // Alle Textboxen rechts leeren

                    MessageBox.Show("Kunde wurde erfolgreich gelöscht.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Löschen: " + ex.Message);
                }
            }
        }

        private void ClearSupplierDetailFields()
        {
            textBoxSuppliersId.Text = "";
            textBoxSuppliersName.Text = "";
            textBoxSuppliersStreet.Text = "";
            textBoxSuppliersCity.Text = "";
            textBoxSuppliersZipCode.Text = "";
            textBoxSuppliersCountry.Text = "";
            textBoxSuppliersEmail.Text = "";
        }

        private void buttonEntryInternal_Click(object sender, EventArgs e)
        {
            // Clear mask and show mask.
            maskTabPageEntryInternal.Show();
        }
    }
}
