using Microsoft.VisualBasic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace mirada_finanza_control_central
{
    public partial class Form1 : Form
    {
        // Die Datenbankdatei liegt im gleichen Ordner wie die .exe
        string dbFile;
        string connString;

        private byte[] selectedFileBytes = null;
        private string selectedFileExt = "";

        private byte[] selectedFileBytesProductPicture = null;
        private string selectedFileExtensionProductPicture = "";

        public Form1()
        {
            InitializeComponent();
            /*
            Type dgvType = dataGridViewJournal.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridViewJournal, true, null);
            */

            // Die Datenbankdatei liegt im gleichen Ordner wie die .exe
            dbFile = "mirada-finanza-control-central.db";
            connString = $"Data Source={dbFile};version=3;";

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

            this.SetupDatabase();
            this.LoadCategories();



            RefreshJournal();
            dataGridViewAssetsRefresh();
            tabPageOverviewRefresh();
            tabPageCustomersRefresh();
            tabPageProductsRefresh();

            // 1. TABS AUSBLENDEN (TRICK)
            // Wir machen die Reiter 1 Pixel hoch und schalten auf Flat-Style um
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;

            tabControl.SelectedTab = tabPageOverview;
            HighlightButton(buttonOverview);


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

        public void SetupDatabase()
        {
            try
            {
                // Falls du die Datei komplett neu erzwingen willst (nur zum Testen!):
                // if (File.Exists(dbFile)) SQLiteConnection.CreateFile(dbFile);

                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();

                    // 1. Tabelle für Kategorien erstellen
                    string sqlEntryCategory = @"
                    CREATE TABLE IF NOT EXISTS EntryCategory (
                        Name TEXT PRIMARY KEY,
                        TransactionType TEXT NOT NULL
                    );";

                    // 2. Tabelle für die Transaktionen (Belege) erstellen
                    string sqlEntryTransaction = @"
                    CREATE TABLE IF NOT EXISTS entryTransaction (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Voucher TEXT NOT NULL,
                        Year INTEGER NOT NULL,
                        EntryCategoryName TEXT NOT NULL,
                        TransactionType TEXT NOT NULL,
                        Amount DECIMAL(10,2) NOT NULL,
                        Description TEXT,
                        Note TEXT,
                        TransDate TEXT NOT NULL,
                        CreatedDate TEXT NOT NULL,
                        Attachment BLOB,
                        FileExt TEXT,
                        Reversal INTEGER DEFAULT 0,
                        ReversalReferenceVoucher TEXT,
                        InvoiceReference TEXT,
                        AssetId TEXT,
                        FOREIGN KEY(EntryCategoryName) REFERENCES EntryCategory(Name)
                    );";

                    // Customers
                    string sqlCustomer = @"
                    CREATE TABLE IF NOT EXISTS customer (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Street TEXT,
                        Zipcode TEXT,
                        City TEXT,
                        Country TEXT,
                        Email TEXT
                    );";

                    // Product
                    string sqlProduct = @"
                    CREATE TABLE IF NOT EXISTS product (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT,
                        Price REAL NOT NULL,        -- Brutto-Preis
                        Picture BLOB,               -- Das Bild selbst als Binärdaten
                        PictureExtension TEXT       -- Dateiendung (z.B. "".png"", "".jpg"")
                    );";

                    // Invoice
                    string sqlInvoice = @"
                    CREATE TABLE IF NOT EXISTS invoice (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        InvoiceNumber TEXT NOT NULL UNIQUE, -- RE-2025-001
                        CustomerId INTEGER,
                        DateCreated TEXT NOT NULL,          
                        TotalAmount REAL NOT NULL,
                        IsPaid INTEGER DEFAULT 0,           -- 0 = Offen, 1 = Bezahlt
                        FOREIGN KEY (CustomerId) REFERENCES Customer(Id)
                    );";

                    // Invoice line.
                    string sqlInvoiceLine = @"
                    CREATE TABLE IF NOT EXISTS invoiceLine (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        InvoiceId INTEGER NOT NULL,        -- Verknüpfung zur Rechnung
                        ProductId INTEGER NOT NULL,        -- Verknüpfung zum Artikel
                        Quantity REAL NOT NULL DEFAULT 1,  -- Menge (z.B. 1 oder 1.5)
                        CurrentPrice REAL NOT NULL,        -- Preis zum Zeitpunkt des Verkaufs (wichtig!)
                        LineTotal REAL NOT NULL,           -- Quantity * CurrentPrice
                        LineNum REAL NOT NULL, -- Line number
                        FOREIGN KEY (InvoiceId) REFERENCES invoice(Id) ON DELETE CASCADE,
                        FOREIGN KEY (ProductId) REFERENCES product(Id)
                    );";

                    using (var cmd = new SQLiteCommand(sqlEntryCategory, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand(sqlEntryTransaction, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand(sqlCustomer, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand(sqlProduct, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand(sqlInvoice, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand(sqlInvoiceLine, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 3. Standard-Kategorien einfügen (INSERT OR IGNORE verhindert Duplikate)
                    string sqlDefaults = @"
                        INSERT OR IGNORE INTO EntryCategory (Name, TransactionType) VALUES 
                        -- 1. Einnahmen
                        ('Verkauf (Produkt)', 'Einnahme'),
                        ('Social Media Einnahme (FaceBook/TikTok/YouTube/...)', 'Einnahme'),
                        ('Rückerstattung (erhaltene Ausgabe)', 'Einnahme'),
                        ('Sonstige Gutschrift', 'Einnahme'),

                        -- 2. Ausgaben
                        ('Material (Bastelzeug, Stoffe)', 'Ausgabe'),
                        ('Versandkosten / Porto', 'Ausgabe'),
                        ('Bürobedarf / Kleingerät (TEDI etc.)', 'Ausgabe'),
                        ('Software / Internet / IT', 'Ausgabe'),

                        -- 3. Anlagen
                        ('Anschaffung über 800€ (AfA)', 'Anlage');";

                    using (var cmd = new SQLiteCommand(sqlDefaults, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Falls was schief geht (z.B. fehlende Schreibrechte)
                System.Windows.Forms.MessageBox.Show("Datenbank-Fehler: " + ex.Message);
            }
        }

        private void LoadCategories()
        {
            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    // Wir sortieren nach Name, damit man die Shops schneller findet
                    string sql = "SELECT Name FROM EntryCategory ORDER BY Name ASC";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            comboBoxEntryCategory.Items.Clear(); // Falls die Liste aktualisiert wird
                            while (reader.Read())
                            {
                                comboBoxEntryCategory.Items.Add(reader["Name"].ToString());
                            }
                        }
                    }
                }

                // Optional: Den ersten Eintrag vorselektieren
                if (comboBoxEntryCategory.Items.Count > 0) comboBoxEntryCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Kategorien: " + ex.Message);
            }
        }

        private string GetTransactionTypeFromCategory(string categoryName)
        {
            string transType = "";
            string dbFile = "mirada-finanza-control-central.db";
            string connString = $"Data Source={dbFile};Version=3;";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    string sql = "SELECT TransactionType FROM EntryCategory WHERE Name = @name";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", categoryName);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            transType = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Abrufen des Typs: " + ex.Message);
            }

            return transType;
        }

        private void comboBoxEntryCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEntryCategory.SelectedItem != null)
            {
                string selectedCategory = comboBoxEntryCategory.SelectedItem.ToString();

                // Typ aus DB holen
                string type = GetTransactionTypeFromCategory(selectedCategory);

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

        private string GetNextVoucher(int year)
        {
            int nextId = 1;
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                // Wir suchen die höchste Nummer für das spezifische Jahr
                string sql = "SELECT Voucher FROM EntryTransaction WHERE Year = @year ORDER BY Voucher DESC LIMIT 1";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@year", year);
                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        // Format ist "2025-0001", wir splitten am Bindestrich
                        string lastVoucher = result.ToString();
                        string suffix = lastVoucher.Split('-')[1];
                        nextId = int.Parse(suffix) + 1;
                    }
                }
            }
            // Gibt z.B. "2025-0001" zurück
            return $"{year}-{nextId:D4}";
        }

        private int GetNextAssetId()
        {
            int nextId = 1; // Standardwert, falls noch nichts in der DB ist
            string sql = "SELECT MAX(AssetId) FROM Entrytransaction";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        object result = cmd.ExecuteScalar();

                        // Falls die Tabelle leer ist oder noch nie eine AssetId vergeben wurde
                        if (result != null && result != DBNull.Value)
                        {
                            nextId = Convert.ToInt32(result) + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Abrufen der nächsten Anlagen-Nummer: " + ex.Message);
            }

            return nextId;
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
                string voucher = GetNextVoucher(voucherYear);

                string assetId = "";
                // New AssetId if its an asset.
                if (textBoxEntryPostingType.Text == "Anlage" && checkBoxReversal.Checked == false)
                {
                    assetId = GetNextAssetId().ToString();
                }

                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    string sql = @"INSERT INTO EntryTransaction 
                (Voucher, Year, EntryCategoryName, TransactionType, Amount, Description, Note, TransDate, CreatedDate, Reversal, ReversalReferenceVoucher, Attachment, FileExt, InvoiceReference, AssetId) 
                VALUES 
                (@vouch, @year, @cat, @type, @amount, @desc, @note, @tDate, @cDate, @rev, @revRef, @file, @ext, @invoiceReference, @assetId)";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@vouch", voucher);
                        cmd.Parameters.AddWithValue("@year", voucherYear);
                        cmd.Parameters.AddWithValue("@cat", comboBoxEntryCategory.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@type", textBoxEntryPostingType.Text); // Aus dem farbigen Feld
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@desc", textBoxText.Text);
                        cmd.Parameters.AddWithValue("@note", textBoxEntryNote.Text);
                        cmd.Parameters.AddWithValue("@tDate", dateTimePickerVoucherDate.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@cDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


                        // --- NEUE PARAMETER ---
                        // Wenn Checkbox an, dann 1, sonst 0
                        cmd.Parameters.AddWithValue("@rev", checkBoxReversal.Checked ? 1 : 0);

                        // Wenn Checkbox an, nimm den Wert aus dem ComboBox-Value (Referenz-Voucher)
                        // Falls kein Storno, speichern wir DBNull.Value
                        cmd.Parameters.AddWithValue("@revRef", checkBoxReversal.Checked ? comboBoxReferenceVoucher.SelectedValue : DBNull.Value);

                        cmd.Parameters.AddWithValue("@invoiceReference", DBNull.Value);

                        cmd.Parameters.AddWithValue("@assetId", assetId != "" ? assetId : DBNull.Value);

                        if (selectedFileBytes != null)
                        {
                            cmd.Parameters.Add("@file", System.Data.DbType.Binary).Value = selectedFileBytes;
                            cmd.Parameters.AddWithValue("@ext", selectedFileExt);
                        }
                        else
                        {
                            cmd.Parameters.Add("@file", System.Data.DbType.Binary).Value = DBNull.Value;
                            cmd.Parameters.AddWithValue("@ext", DBNull.Value);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

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
        }

        private void RefreshJournal()
        {
            dataGridViewJournal.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dataGridViewJournal.DefaultCellStyle.SelectionForeColor = Color.Black;

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    // Wir holen alle wichtigen Spalten
                    string sql = "SELECT " +
             "Voucher, TransDate, TransactionType, Amount, Reversal, EntryCategoryName, Description, ReversalReferenceVoucher, InvoiceReference, AssetId, " +
             "Voucher || char(10) || TransDate AS Disp_VoucherDate, " +
             "TransactionType || char(10) || EntryCategoryName AS Disp_TypeCategory, " +
             "Amount || ' €' || char(10) || Description AS Disp_AmountDesc " +
             "FROM EntryTransaction ORDER BY ID DESC";

                    // , 
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // Der "Auto-Reset":
                            dataGridViewJournal.DataSource = null; // Alte Bindung lösen
                            dataGridViewJournal.AutoGenerateColumns = true; // Spalten automatisch erstellen
                            dataGridViewJournal.DataSource = dt; // Neue Daten binden
                        }
                    }
                }

                // Jetzt sind die Spalten da! Jetzt können wir sie hübsch machen:
                if (dataGridViewJournal.Columns.Count > 0)
                {


                    // 1. Alle Original-Daten-Spalten ausblenden
                    string[] hiddenCols = { "Voucher", "TransDate", "TransactionType", "EntryCategoryName", "Amount", "Description" };
                    foreach (var colName in hiddenCols)
                    {
                        if (dataGridViewJournal.Columns[colName] != null)
                            dataGridViewJournal.Columns[colName].Visible = false;
                    }

                    // 1. Anzeige-Spalten konfigurieren und nach vorne schieben
                    ConfigureDisplayColumn("Disp_VoucherDate", "Beleg\nDatum", 100, 0);
                    ConfigureDisplayColumn("Disp_TypeCategory", "Typ\nKategorie", 140, 1);
                    ConfigureDisplayColumn("Disp_AmountDesc", "Betrag\nBeschreibung", 140, 2);

                    // 2. Storno-Spalten nach hinten schieben (hoher Index = weiter rechts)
                    if (dataGridViewJournal.Columns["Reversal"] != null)
                    {
                        dataGridViewJournal.Columns["Reversal"].HeaderText = "Storno";
                        dataGridViewJournal.Columns["Reversal"].Width = 60;
                        dataGridViewJournal.Columns["Reversal"].DisplayIndex = 3;
                        dataGridViewJournal.Columns["Reversal"].Visible = true;
                    }

                    if (dataGridViewJournal.Columns["ReversalReferenceVoucher"] != null)
                    {
                        dataGridViewJournal.Columns["ReversalReferenceVoucher"].HeaderText = "Stornobeleg";
                        dataGridViewJournal.Columns["ReversalReferenceVoucher"].Width = 102;
                        dataGridViewJournal.Columns["ReversalReferenceVoucher"].DisplayIndex = 4;
                        dataGridViewJournal.Columns["ReversalReferenceVoucher"].Visible = true;
                    }

                    if (dataGridViewJournal.Columns["InvoiceReference"] != null)
                    {
                        dataGridViewJournal.Columns["InvoiceReference"].HeaderText = "Rechnungsbeleg";
                        dataGridViewJournal.Columns["InvoiceReference"].Width = 102;
                        dataGridViewJournal.Columns["InvoiceReference"].DisplayIndex = 5;
                        dataGridViewJournal.Columns["InvoiceReference"].Visible = true;
                    }

                    if (dataGridViewJournal.Columns["AssetId"] != null)
                    {
                        dataGridViewJournal.Columns["AssetId"].HeaderText = "Anlagen-Nr.";
                        dataGridViewJournal.Columns["AssetId"].Width = 102;
                        dataGridViewJournal.Columns["AssetId"].DisplayIndex = 5;
                        dataGridViewJournal.Columns["AssetId"].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden: " + ex.Message);
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
            // 1. Prüfen, ob eine Zeile ausgewählt ist
            if (dataGridViewJournal.SelectedRows.Count > 0)
            {
                // Wir holen die Belegnummer (Voucher) aus der ausgewählten Zeile
                string voucherNr = dataGridViewJournal.SelectedRows[0].Cells["Voucher"].Value?.ToString();

                if (string.IsNullOrEmpty(voucherNr)) return;

                // 2. Datenbank-Abruf mit SQLite
                try
                {
                    using (var conn = new SQLiteConnection(connString))
                    {
                        conn.Open();
                        // Wir holen Note und CreatedDate basierend auf dem Voucher (Belegnummer)
                        string sql = "SELECT Note, CreatedDate, Amount, EntryCategoryName, Voucher, TransDate, ReversalReferenceVoucher, Reversal, Description, TransactionType, InvoiceReference, AssetId FROM EntryTransaction WHERE Voucher = @nr";

                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@nr", voucherNr);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // 3. Textboxen befüllen
                                    textBoxJournalNote.Text = reader["Note"]?.ToString() ?? "";
                                    textBoxJournalAmount.Text = reader["Amount"]?.ToString();
                                    textBoxJournalCategory.Text = reader["EntryCategoryName"]?.ToString();
                                    textBoxJournalVoucher.Text = reader["Voucher"]?.ToString();
                                    textBoxJournalVoucherDate.Text = reader["TransDate"]?.ToString();
                                    textBoxJournalReversalVoucher.Text = reader["ReversalReferenceVoucher"]?.ToString();
                                    textBoxJournalReversal.Text = reader["Reversal"]?.ToString();
                                    textBoxJournalPostingText.Text = reader["Description"]?.ToString();
                                    textBoxJournalPostingType.Text = reader["TransactionType"]?.ToString();

                                    // Datum formatieren, falls vorhanden
                                    if (reader["CreatedDate"] != DBNull.Value)
                                    {
                                        DateTime dt = Convert.ToDateTime(reader["CreatedDate"]);
                                        textBoxJournalCreationDate.Text = dt.ToString("dd.MM.yyyy HH:mm");
                                    }
                                    else
                                    {
                                        textBoxJournalCreationDate.Text = "Kein Datum";
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Fehler im Debug-Fenster ausgeben, um das Scrollen nicht zu blockieren
                    System.Diagnostics.Debug.WriteLine("Fehler beim Laden der Details: " + ex.Message);
                }
            }
            else
            {
                // Nichts ausgewählt -> Felder leeren
                textBoxJournalNote.Clear();
                textBoxJournalCreationDate.Clear();
            }
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
            // Wir holen Belegnummer und Beschreibung, damit man weiß, was man auswählt
            string sql = "SELECT Voucher, (Voucher || ' - ' || Description) as DisplayText FROM entryTransaction WHERE Reversal = 0 " +
                "AND Voucher NOT IN (SELECT ReversalReferenceVoucher FROM entryTransaction WHERE ReversalReferenceVoucher IS NOT NULL) " +
                "ORDER BY ID DESC";

            // Hier dein üblicher Code zum Laden in eine DataTable
            DataTable dt = GetData(sql); // Deine Hilfsmethode für SQL

            comboBoxReferenceVoucher.DisplayMember = "DisplayText";
            comboBoxReferenceVoucher.ValueMember = "Voucher";
            comboBoxReferenceVoucher.DataSource = dt;
        }

        private DataTable GetData(string sql, Dictionary<string, object> parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        // Falls Parameter übergeben wurden (z.B. für WHERE-Klauseln)
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Daten: " + ex.Message);
            }
            return dt;
        }

        private void comboBoxReferenceVoucher_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxReferenceVoucher.SelectedValue != null && checkBoxReversal.Checked)
            {
                string selectedVoucher = comboBoxReferenceVoucher.SelectedValue.ToString();
                selectedVoucher = selectedVoucher.Split(' ')[0];
                string sql = "SELECT * FROM entryTransaction WHERE Voucher = @Voucher LIMIT 1";

                // Parameter für die Suche vorbereiten
                var paramsDict = new Dictionary<string, object> { { "@Voucher", selectedVoucher } };

                DataTable dt = GetData(sql, paramsDict);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // Felder automatisch füllen
                    textBoxText.Text = "STORNO zu: " + row["Description"].ToString();
                    numericUpDownAmount.Value = Convert.ToDecimal(row["Amount"]);
                    textBoxEntryNote.Text = "STORNO zu: " + row["Note"].ToString();
                    dateTimePickerVoucherDate.Value = Convert.ToDateTime(row["TransDate"]);
                    comboBoxEntryCategory.Text = row["EntryCategoryName"].ToString();
                    textBoxEntryPostingType.Text = row["TransactionType"].ToString();

                    // --- FELDER SPERREN ---
                    // Damit der Storno eine exakte Kopie bleibt (außer Belegnummer)
                    numericUpDownAmount.Enabled = false;
                    comboBoxEntryCategory.Enabled = false;
                    textBoxEntryPostingType.Enabled = false;
                    textBoxText.Enabled = false;
                    textBoxEntryNote.Enabled = false;
                    dateTimePickerVoucherDate.Enabled = false;
                }
            }
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
            tabControl.SelectedTab = tabPageDataexport;
            HighlightButton(buttonExport);
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
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

            byte[] fileData = null;
            string extension = "";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    // Wir holen Attachment und die Dateiendung
                    string sql = "SELECT Attachment, FileExt FROM EntryTransaction WHERE Voucher = @nr";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nr", voucherNr);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader["Attachment"] != DBNull.Value)
                            {
                                fileData = (byte[])reader["Attachment"];
                                extension = reader["FileExt"].ToString().ToLower().Trim();
                            }
                        }
                    }
                }

                if (fileData == null)
                {
                    MessageBox.Show("Kein Beleg für diesen Eintrag gefunden.");
                    return;
                }

                // Punkt vor die Extension setzen, falls er fehlt
                if (!extension.StartsWith(".")) extension = "." + extension;

                // Temporäre Datei erstellen
                string tempPath = Path.Combine(Path.GetTempPath(), $"Beleg_{voucherNr}{extension}");
                File.WriteAllBytes(tempPath, fileData);

                // Unterscheidung: PDF oder Bild
                if (extension == ".pdf")
                {
                    // PDF im Standard-Browser öffnen (z.B. Firefox)
                    Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
                }
                else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp")
                {
                    // Bild in deiner neuen modalen Form anzeigen
                    ZeigeBildModal(tempPath);
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
            dataGridViewJournal.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dataGridViewJournal.DefaultCellStyle.SelectionForeColor = Color.Black;

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    // Wir holen alle wichtigen Spalten
                    string sql = @"SELECT 
                        t1.AssetId,
                        t1.Voucher, 
                        t1.TransDate, 
                        t1.TransactionType, 
                        t1.Amount, 
                        t1.Description 
                    FROM EntryTransaction t1
                    LEFT JOIN EntryTransaction t2 ON t1.Voucher = t2.ReversalReferenceVoucher
                    WHERE t1.TransactionType = 'Anlage' 
                      AND t1.Reversal = 0           -- Der Beleg selbst ist kein Storno
                      AND t2.Voucher IS NULL        -- Es existiert kein Storno, der auf diesen Beleg zeigt
                    ORDER BY t1.ID DESC;";

                    // , 
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // Der "Auto-Reset":
                            dataGridViewAssets.DataSource = null; // Alte Bindung lösen
                            dataGridViewAssets.AutoGenerateColumns = true; // Spalten automatisch erstellen
                            dataGridViewAssets.DataSource = dt; // Neue Daten binden

                            // 1. Spalten für die Berechnung hinzufügen
                            dt.Columns.Add("Restwert", typeof(decimal));
                            dt.Columns.Add("MonateVerbleibend", typeof(int));

                            DateTime heute = DateTime.Now;

                            foreach (DataRow row in dt.Rows)
                            {
                                decimal anschaffungspreis = Convert.ToDecimal(row["Amount"]);
                                DateTime kaufdatum = DateTime.Parse(row["TransDate"].ToString());

                                // Differenz in Monaten berechnen
                                int vergangeneMonate = ((heute.Year - kaufdatum.Year) * 12) + heute.Month - kaufdatum.Month;

                                // Abschreibungs-Logik (36 Monate Standard)
                                decimal monatlicheRate = anschaffungspreis / 36;
                                int restMonate = 36 - vergangeneMonate;
                                decimal restwert = anschaffungspreis - (monatlicheRate * vergangeneMonate);

                                // Werte korrigieren, falls Gerät älter als 3 Jahre
                                if (restwert < 0) restwert = 0;
                                if (restMonate < 0) restMonate = 0;

                                row["Restwert"] = Math.Round(restwert, 2);
                                row["MonateVerbleibend"] = restMonate;
                            }

                            // Spalten im Grid schön benennen
                            dataGridViewAssets.Columns["Restwert"].HeaderText = "Aktueller Buchwert";
                            dataGridViewAssets.Columns["Restwert"].DefaultCellStyle.Format = "C2";
                            dataGridViewAssets.Columns["MonateVerbleibend"].HeaderText = "Restlaufzeit (Monate)";
                            dataGridViewAssets.Columns["AssetId"].DisplayIndex = 0;

                            // --- AB HIER: Spalten sprechend machen ---
                            if (dataGridViewAssets.Columns.Count > 0)
                            {
                                dataGridViewAssets.Columns["AssetId"].HeaderText = "Anlagen-Nr.";
                                dataGridViewAssets.Columns["Voucher"].HeaderText = "Beleg-Nr.";
                                dataGridViewAssets.Columns["TransDate"].HeaderText = "Kaufdatum";
                                dataGridViewAssets.Columns["TransactionType"].HeaderText = "Typ";
                                dataGridViewAssets.Columns["Amount"].HeaderText = "Betrag (Brutto)";
                                dataGridViewAssets.Columns["Description"].HeaderText = "Bezeichnung / Gerät";

                                // Bonus: Betrag rechtsbündig und mit €-Zeichen formatieren
                                dataGridViewAssets.Columns["Amount"].DefaultCellStyle.Format = "C2"; // Currency Format
                                dataGridViewAssets.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                                // Spaltenbreite automatisch anpassen
                                dataGridViewAssets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden: " + ex.Message);
            }
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
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    -- MONATE (Revenue)
                    SUM(CASE WHEN TransactionType = 'Einnahme' AND strftime('%m-%Y', TransDate) = strftime('%m-%Y', 'now') THEN Amount ELSE 0 END) as RevenueM0,
                    SUM(CASE WHEN TransactionType = 'Einnahme' AND strftime('%m-%Y', TransDate) = strftime('%m-%Y', 'now', '-1 month') THEN Amount ELSE 0 END) as RevenueM1,
                    SUM(CASE WHEN TransactionType = 'Einnahme' AND strftime('%m-%Y', TransDate) = strftime('%m-%Y', 'now', '-2 month') THEN Amount ELSE 0 END) as RevenueM2,
                    -- MONATE (Expenses)
                    SUM(CASE WHEN TransactionType IN ('Ausgabe', 'Anlage') AND strftime('%m-%Y', TransDate) = strftime('%m-%Y', 'now') THEN Amount ELSE 0 END) as ExpensesM0,
                    SUM(CASE WHEN TransactionType IN ('Ausgabe', 'Anlage') AND strftime('%m-%Y', TransDate) = strftime('%m-%Y', 'now', '-1 month') THEN Amount ELSE 0 END) as ExpensesM1,
                    SUM(CASE WHEN TransactionType IN ('Ausgabe', 'Anlage') AND strftime('%m-%Y', TransDate) = strftime('%m-%Y', 'now', '-2 month') THEN Amount ELSE 0 END) as ExpensesM2,
                    -- JAHRE
                    SUM(CASE WHEN TransactionType = 'Einnahme' AND strftime('%Y', TransDate) = strftime('%Y', 'now') THEN Amount ELSE 0 END) as RevenueY0,
                    SUM(CASE WHEN TransactionType = 'Einnahme' AND strftime('%Y', TransDate) = strftime('%Y', 'now', '-1 year') THEN Amount ELSE 0 END) as RevenueY1,
                    SUM(CASE WHEN TransactionType = 'Einnahme' AND strftime('%Y', TransDate) = strftime('%Y', 'now', '-2 year') THEN Amount ELSE 0 END) as RevenueY2,
                    SUM(CASE WHEN TransactionType IN ('Ausgabe', 'Anlage') AND strftime('%Y', TransDate) = strftime('%Y', 'now') THEN Amount ELSE 0 END) as ExpensesY0,
                    SUM(CASE WHEN TransactionType IN ('Ausgabe', 'Anlage') AND strftime('%Y', TransDate) = strftime('%Y', 'now', '-1 year') THEN Amount ELSE 0 END) as ExpensesY1,
                    SUM(CASE WHEN TransactionType IN ('Ausgabe', 'Anlage') AND strftime('%Y', TransDate) = strftime('%Y', 'now', '-2 year') THEN Amount ELSE 0 END) as ExpensesY2
                FROM EntryTransaction 
                WHERE Reversal = 0 
                  AND Voucher NOT IN (SELECT IFNULL(ReversalReferenceVoucher, '') FROM EntryTransaction WHERE ReversalReferenceVoucher IS NOT NULL)";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Daten zuweisen & Gewinne berechnen
                            tapPageOverviewUpdateBlock(
                                reader,
                                "M0",
                                textBoxOverviewRevenueCurrentMonth,
                                textBoxOverviewCostsCurrentMonth,
                                textBoxOverviewEarningsCurrentMonth);

                            tapPageOverviewUpdateBlock(
                                reader,
                                "M1",
                                textBoxOverviewRevenuePreMonth,
                                textBoxOverviewCostsPreMonth,
                                textBoxOverviewEarningsPreMonth);

                            tapPageOverviewUpdateBlock(
                                reader,
                                "M2",
                                textBoxOverviewRevenuePrePreMonth,
                                textBoxOverviewCostsPrePreMonth,
                                textBoxOverviewEarningsPrePreMonth);

                            tapPageOverviewUpdateBlock(
                                reader,
                                "Y0",
                                textBoxOverviewRevenueCurrentYear,
                                textBoxOverviewCostsCurrentYear,
                                textBoxOverviewEarningsCurrentYear);

                            tapPageOverviewUpdateBlock(
                                reader,
                                "Y1",
                                textBoxOverviewRevenuePreYear,
                                textBoxOverviewCostsPreYear,
                                textBoxOverviewEarningsPreYear);

                            tapPageOverviewUpdateBlock(
                                reader,
                                "Y2",
                                textBoxOverviewRevenuePrePreYear,
                                textBoxOverviewCostsPrePreYear,
                                textBoxOverviewEarningsPrePreYear);
                        }
                    }

                    // Dynamische Beschriftung der Header
                    labelOverviewCurrentMonth.Text = DateTime.Now.ToString("MMMM yyyy");
                    labelOverviewPreMonth.Text = DateTime.Now.AddMonths(-1).ToString("MMMM yyyy");
                    labelOverviewPrePreMonth.Text = DateTime.Now.AddMonths(-2).ToString("MMMM yyyy");
                    labelOverviewCurrentYear.Text = "Jahr " + DateTime.Now.Year;
                    labelOverviewPreYear.Text = "Jahr " + DateTime.Now.AddYears(-1).Year;
                    labelOverviewPrePreYear.Text = "Jahr " + DateTime.Now.AddYears(-2).Year;
                }
            }
            catch (Exception ex) { MessageBox.Show("Dashboard Error: " + ex.Message); }
        }

        private void tapPageOverviewUpdateBlock(SQLiteDataReader r, string suffix, TextBox tRev, TextBox tExp, TextBox tProf)
        {
            decimal rev = r["Revenue" + suffix] != DBNull.Value ? Convert.ToDecimal(r["Revenue" + suffix]) : 0;
            decimal exp = r["Expenses" + suffix] != DBNull.Value ? Convert.ToDecimal(r["Expenses" + suffix]) : 0;
            decimal prof = rev - exp;

            tRev.Text = rev.ToString("C2");
            tExp.Text = exp.ToString("C2");
            tProf.Text = prof.ToString("C2");
            tProf.ForeColor = prof >= 0 ? Color.ForestGreen : Color.Firebrick;
        }

        private void tabPageCustomerEntrySaveCustomer()
        {
            // 1. Daten aus den Textboxen sammeln
            string name = textBoxCustomerEntryName.Text.Trim();
            string street = textBoxCustomerEntryStreet.Text.Trim();
            string zip = textBoxCustomerEntryZipCode.Text.Trim();
            string city = textBoxCustomerEntryCity.Text.Trim();
            string country = textBoxCustomerEntryCountry.Text.Trim();
            string email = textBoxCustomerEntryEmail.Text.Trim();

            // Validierung: Name ist Pflicht
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Bitte geben Sie mindestens einen Namen ein.", "Eingabe fehlt");
                return;
            }

            // 2. SQL Insert (ID wird automatisch per AUTOINCREMENT vergeben)
            string sql = @"INSERT INTO customer (Name, Street, Zipcode, City, Country, Email) 
                   VALUES (@Name, @Street, @Zipcode, @City, @Country, @Email);
                   SELECT last_insert_rowid();"; // Gibt die neue ID direkt zurück

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        // Parameter für maximale Sicherheit (SQL-Injection Schutz)
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Street", street);
                        cmd.Parameters.AddWithValue("@Zipcode", zip);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@Country", country);
                        cmd.Parameters.AddWithValue("@Email", email);

                        // ExecuteScalar, weil wir die neue ID als Rückgabewert erwarten
                        object newId = cmd.ExecuteScalar();

                        MessageBox.Show($"Kunde erfolgreich unter ID {newId} angelegt!", "Erfolg");
                    }
                }

                // 3. Maske für den nächsten Kunden leeren
                tabPageCustomerEntryClear();

                // Optional: Hier die Kundenliste im Grid aktualisieren
                tabPageCustomersRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern des Kunden: " + ex.Message, "Datenbankfehler");
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

            // SQL-Abfrage: Alle Kunden laden, Neueste zuerst
            string sql = "SELECT Id, Name, Street, Zipcode, City, Country, Email FROM customer ORDER BY Id DESC";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // Daten binden
                            dataGridViewCustomers.DataSource = null; // Alte Bindung lösen
                            dataGridViewCustomers.AutoGenerateColumns = true; // Spalten automatisch erstellen
                            dataGridViewCustomers.DataSource = dt; // Neue Daten binden

                            // Ein Style-Objekt erstellen, das Umbrüche erlaubt
                            DataGridViewCellStyle wrapStyle = new DataGridViewCellStyle();
                            wrapStyle.WrapMode = DataGridViewTriState.True;

                            dataGridViewCustomers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                            // --- Optik-Finishing (wie bei den Anlagen) ---
                            if (dataGridViewCustomers.Columns.Count > 0)
                            {
                                // 1. Spaltenbreiten optimieren
                                if (dataGridViewCustomers.Columns["Id"] != null)
                                {
                                    dataGridViewCustomers.Columns["Id"].Width = 40; // ID schön schmal halten
                                }

                                if (dataGridViewCustomers.Columns["Name"] != null)
                                {
                                    dataGridViewCustomers.Columns["Name"].Width = 110; // ID schön schmal halten
                                    dataGridViewCustomers.Columns["Name"].DefaultCellStyle = wrapStyle;
                                }

                                if (dataGridViewCustomers.Columns["Street"] != null)
                                {
                                    dataGridViewCustomers.Columns["Street"].Width = 120; // ID schön schmal halten
                                    dataGridViewCustomers.Columns["Street"].DefaultCellStyle = wrapStyle;
                                }

                                if (dataGridViewCustomers.Columns["ZipCode"] != null)
                                {
                                    dataGridViewCustomers.Columns["ZipCode"].Width = 70; // ID schön schmal halten
                                }

                                if (dataGridViewCustomers.Columns["Email"] != null)
                                {
                                    dataGridViewCustomers.Columns["Email"].Width = 220; // ID schön schmal halten
                                    dataGridViewCustomers.Columns["Email"].DefaultCellStyle = wrapStyle;
                                }

                                // 2. Bearbeitung im Grid verhindern
                                dataGridViewCustomers.AllowUserToAddRows = false;
                                dataGridViewCustomers.ReadOnly = true;
                                dataGridViewCustomers.RowHeadersVisible = false;

                                // 3. Selektions-Stil (Ganze Zeile)
                                dataGridViewCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                                dataGridViewCustomers.MultiSelect = false;

                                // 4. Hintergrundfarbe und Header-Design (wie besprochen)
                                dataGridViewCustomers.EnableHeadersVisualStyles = false;
                                dataGridViewCustomers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

                                // Fokus entfernen
                                dataGridViewCustomers.ClearSelection();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Kundenliste: " + ex.Message);
            }
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
            string name = textBoxProductEntryName.Text.Trim();
            string description = textBoxProductEntryDescription.Text.Trim();
            string priceRaw = textBoxProductEntryPrice.Text.Replace(",", ".");

            if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("Name fehlt!"); return; }

            if (!double.TryParse(priceRaw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double price))
            {
                MessageBox.Show("Preis ungültig!"); return;
            }

            string sql = @"INSERT INTO product (Name, Description, Price, Picture, PictureExtension) 
                   VALUES (@Name, @Description, @Price, @Picture, @Extension)";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Price", price);

                        // Bild-Logik
                        if (pictureBoxProductEntry.Image != null)
                        {
                            // Bild in Byte-Array umwandeln
                            byte[] imageBytes = selectedFileBytesProductPicture;
                            cmd.Parameters.AddWithValue("@Picture", imageBytes);
                            cmd.Parameters.AddWithValue("@Extension", selectedFileExtensionProductPicture);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Picture", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Extension", DBNull.Value);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Produkt inklusive Bild gespeichert!");
                tabPageProductEntryClear();
            }
            catch (Exception ex) { MessageBox.Show("Fehler: " + ex.Message); }
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
            string sql = "SELECT Id, Name, Description, Price FROM product ORDER BY Name ASC";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridViewProducts.DataSource = dt;
                        }
                    }
                }

                // Optik-Finishing
                if (dataGridViewProducts.Columns.Count > 0)
                {
                    //dataGridViewProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    if (dataGridViewProducts.Columns.Contains("Price"))
                    {
                        dataGridViewProducts.Columns["Price"].DefaultCellStyle.Format = "N2";
                        dataGridViewProducts.Columns["Price"].Width = 80;
                        dataGridViewProducts.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler beim Laden: " + ex.Message); }
        }

        private void dataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                // Die Id aus der ausgewählten Zeile holen
                var selectedId = dataGridViewProducts.SelectedRows[0].Cells["Id"].Value;

                LoadProductDetails(selectedId);
            }
        }

        private void LoadProductDetails(object productId)
        {
            string sql = "SELECT Picture, PictureExtension FROM product WHERE Id = @Id";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", productId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                byte[] fileBytes = reader["Picture"] as byte[];
                                string ext = reader["PictureExtension"]?.ToString().ToLower();

                                if (fileBytes != null && fileBytes.Length > 0)
                                {
                                    using (var ms = new MemoryStream(fileBytes))
                                    {
                                        pictureBoxProducts.Image = Image.FromStream(ms);
                                    }
                                }
                                else
                                {
                                    pictureBoxProducts.Image = null;
                                }
                            }
                        }
                    }
                }
            }
            catch { /* Fehler ignorieren oder loggen */ }
        }
    }
}
