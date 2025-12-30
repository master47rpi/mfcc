using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;



namespace mirada_finanza_control_central
{
    internal class DBManager
    {
        private readonly string dbFile;
        private readonly string connString;

        // Der Manager baut sich seinen ConnectionString jetzt selbst
        public DBManager(string _dbFile = "mirada-finanza-control-central.db")
        {
            dbFile = _dbFile;
            connString = $"Data Source={_dbFile};version=3;";
        }

        // Hilfsmethode, um überall im Manager schnell eine Verbindung zu bekommen
        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connString);
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
                        PictureExtension TEXT,       -- Dateiendung (z.B. "".png"", "".jpg"")
                        Stock INT DEFAULT 0,
                        Stocked INT DEFAULT 0
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
                        PDF BLOB NOT NULL,
                        FOREIGN KEY (CustomerId) REFERENCES Customer(Id)
                    );";

                    // Invoice line.
                    string sqlInvoiceLine = @"
                    CREATE TABLE IF NOT EXISTS invoiceLine (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        InvoiceId INTEGER NOT NULL,        -- Verknüpfung zur Rechnung
                        ProductId INTEGER NOT NULL,        -- Verknüpfung zum Artikel
                        ProductName TEXT NOT NULL,
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

                    string sqlAsset = @"
                    CREATE TABLE IF NOT EXISTS asset (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EntryTransactionId INTEGER NOT NULL,
                        Description TEXT NOT NULL,
                        PurchaseDate DATE NOT NULL,
                        Amount REAL NOT NULL,           -- Der Brutto-Betrag (da Kleinunternehmer)
                        UsefulLifeYears INTEGER NOT NULL, -- AfA-Dauer in Jahren
                        Note TEXT,                      -- Optionales Notizfeld
                        Status INTEGER DEFAULT 0,
                        FOREIGN KEY(EntryTransactionId) REFERENCES entryTransaction(Id) ON DELETE CASCADE
                    );";

                    using (var cmd = new SQLiteCommand(sqlAsset, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string sqlSettings = @"
                    CREATE TABLE IF NOT EXISTS settings (
                        Id INTEGER PRIMARY KEY CHECK (Id = 1),
                        CompanyName TEXT,
                        Owner TEXT,
                        Street TEXT,
                        ZipCode TEXT,
                        City TEXT,
                        Country TEXT, 
                        Phone TEXT,
                        Email TEXT,
                        TaxNumber TEXT,
                        BankName TEXT,
                        IBAN TEXT,
                        BIC TEXT,
                        IntroText TEXT,
                        FooterText TEXT,
                        SmallBusinessNote TEXT,
                        CompanyImage BLOB,
                        CompanyImageExtension TEXT
                    );";

                    using (var cmd = new SQLiteCommand(sqlSettings, conn))
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

        public List<string> GetCategoryNames()
        {
            var categories = new List<string>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT Name FROM EntryCategory ORDER BY Name ASC";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(reader["Name"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Laden der Kategorien", ex);
            }
            return categories;
        }

        public string GetTransactionTypeFromCategory(string _categoryName)
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
                        cmd.Parameters.AddWithValue("@name", _categoryName);
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

        public string GetNextVoucher(int year)
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

        public bool PostEntryTransaction(
            EntryTransaction _entryTransaction)
        {
            bool ret = true;

            string assetId = "";
            long newId;

            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string sql = @"INSERT INTO EntryTransaction 
                (Voucher, Year, EntryCategoryName, TransactionType, Amount, Description, Note, TransDate, CreatedDate, Reversal, ReversalReferenceVoucher, Attachment, FileExt, InvoiceReference) 
                VALUES 
                (@vouch, @year, @cat, @type, @amount, @desc, @note, @tDate, @cDate, @rev, @revRef, @file, @ext, @invoiceReference)";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@vouch", _entryTransaction.Voucher);
                    cmd.Parameters.AddWithValue("@year", _entryTransaction.Year);
                    cmd.Parameters.AddWithValue("@cat", _entryTransaction.EntryCategoryName);
                    cmd.Parameters.AddWithValue("@type", _entryTransaction.TransactionType); // Aus dem farbigen Feld
                    cmd.Parameters.AddWithValue("@amount", _entryTransaction.Amount);
                    cmd.Parameters.AddWithValue("@desc", _entryTransaction.Description);
                    cmd.Parameters.AddWithValue("@note", _entryTransaction.Note);
                    cmd.Parameters.AddWithValue("@tDate", _entryTransaction.TransDate);
                    cmd.Parameters.AddWithValue("@cDate", _entryTransaction.CreatedDate);
                    cmd.Parameters.AddWithValue("@rev", _entryTransaction.Reversal);
                    cmd.Parameters.AddWithValue("@revRef", _entryTransaction.ReversalReferenceVoucher);
                    cmd.Parameters.AddWithValue("@invoiceReference", _entryTransaction.InvoiceReference);
                    cmd.Parameters.Add("@file", System.Data.DbType.Binary).Value = _entryTransaction.Attachment;
                    cmd.Parameters.AddWithValue("@ext", _entryTransaction.FileExt);

                    cmd.ExecuteNonQuery();

                    using (var cmdId = new SQLiteCommand("SELECT last_insert_rowid();", conn))
                    {
                        newId = (long)cmdId.ExecuteScalar();
                    }
                }

                if (_entryTransaction.TransactionType == "Anlage")
                {
                    Asset asset = new Asset
                    {
                        EntryTransactionId = newId,
                        Description = _entryTransaction.Description,
                        PurchaseDate = _entryTransaction.TransDate,
                        Amount = _entryTransaction.Amount,
                        UsefulLifeYears = 4,
                        Status = 0
                    };

                    PostAsset(
                        asset,
                        _entryTransaction);

                }
            }

            return ret;
        }

        public bool PostAsset(
            Asset _asset,
            EntryTransaction _entryTransaction)
        {
            bool ret = true;

            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                if (_entryTransaction.Reversal == false)
                {
                    string sqlAsset = @"INSERT INTO asset 
                    (EntryTransactionId, Description, PurchaseDate, Amount, UsefulLifeYears, Status) 
                    VALUES 
                    (@transId, @desc, @pDate, @amount, @years, @status)";

                    using (var cmdAsset = new SQLiteCommand(sqlAsset, conn))
                    {
                        cmdAsset.Parameters.AddWithValue("@transId", _asset.EntryTransactionId);
                        cmdAsset.Parameters.AddWithValue("@desc", _asset.Description); // Oder eigenes Feld für Asset-Name
                        cmdAsset.Parameters.AddWithValue("@pDate", _asset.PurchaseDate);
                        cmdAsset.Parameters.AddWithValue("@amount", _asset.Amount);
                        cmdAsset.Parameters.AddWithValue("@years", _asset.UsefulLifeYears);
                        cmdAsset.Parameters.AddWithValue("@status", _asset.Status);

                        cmdAsset.ExecuteNonQuery();
                    }
                }
                else
                {
                    long foundId = 0;
                    string voucherText = _entryTransaction.ReversalReferenceVoucher.Split(' ')[0];

                    // Wir suchen die ID zum Belegtext
                    string sqlFind = "SELECT Id FROM entryTransaction WHERE Voucher = @vouch AND Reversal = 0 LIMIT 1";
                    using (var cmd = new SQLiteCommand(sqlFind, conn))
                    {
                        cmd.Parameters.AddWithValue("@vouch", voucherText);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            foundId = Convert.ToInt64(result);
                        }
                    }

                    string sqlUpdateAsset = "UPDATE asset SET Status = 1 WHERE entryTransactionId = @id";
                    using (var cmdAsset = new SQLiteCommand(sqlUpdateAsset, conn))
                    {
                        cmdAsset.Parameters.AddWithValue("@id", foundId);
                        int assetsAffected = cmdAsset.ExecuteNonQuery();

                        if (assetsAffected > 0)
                        {
                            // Optional: Rückmeldung, dass ein Asset mit storniert wurde
                            MessageBox.Show("Zugehöriges Asset wurde ebenfalls storniert.");
                        }
                    }
                }
            }

            return ret;
        }

        public DataTable FetchEntryTransactionData()
        {
            string sql = @"
        SELECT 
            t.Voucher, 
            t.TransDate, 
            t.TransactionType, 
            t.Amount, 
            t.Reversal, 
            t.EntryCategoryName, 
            t.Description, 
            t.ReversalReferenceVoucher, 
            t.InvoiceReference, 
            COALESCE(a.Id, a_ref.Id) AS AssetId,
            t.Voucher || char(10) || t.TransDate AS Disp_VoucherDate, 
            t.TransactionType || char(10) || t.EntryCategoryName AS Disp_TypeCategory, 
            t.Amount || ' €' || char(10) || t.Description AS Disp_AmountDesc 
        FROM EntryTransaction t
        LEFT JOIN asset a ON t.Id = a.EntryTransactionId 
        LEFT JOIN EntryTransaction t_orig ON t.ReversalReferenceVoucher = t_orig.Voucher AND t.Reversal = 1
        LEFT JOIN asset a_ref ON t_orig.Id = a_ref.EntryTransactionId
        ORDER BY t.ID DESC";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Abrufen der Journal-Daten: " + ex.Message);
            }
        }

        public EntryTransaction GetTransactionByVoucher(string voucherNr)
        {
            const string sql = "SELECT * FROM EntryTransaction WHERE Voucher = @nr";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nr", voucherNr);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Wir befüllen das Model-Objekt mit den Daten aus der DB
                                return new EntryTransaction
                                {
                                    ID = Convert.ToInt64(reader["ID"]),
                                    Voucher = reader["Voucher"].ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    EntryCategoryName = reader["EntryCategoryName"].ToString(),
                                    TransactionType = reader["TransactionType"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    Note = reader["Note"]?.ToString(),
                                    TransDate = reader["TransDate"].ToString(),
                                    CreatedDate = reader["CreatedDate"]?.ToString(),
                                    Reversal = Convert.ToInt32(reader["Reversal"]) == 1, // Mapping auf bool
                                    ReversalReferenceVoucher = reader["ReversalReferenceVoucher"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DB-Fehler: " + ex.Message);
            }
            return null; // Nichts gefunden
        }

        public DataTable GetAvailableVouchersForReversal()
        {
            // Die Logik: Nur Belege, die keine Stornos sind UND die nicht bereits storniert wurden
            string sql = @"
        SELECT 
            Voucher, 
            (Voucher || ' - ' || Description) as DisplayText 
        FROM entryTransaction 
        WHERE Reversal = 0 
        AND Voucher NOT IN (
            SELECT ReversalReferenceVoucher 
            FROM entryTransaction 
            WHERE ReversalReferenceVoucher IS NOT NULL
        ) 
        ORDER BY ID DESC";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Laden der verfügbaren Belege: " + ex.Message);
            }
        }

        private DataTable GetData(string sql, Dictionary<string, object> parameters = null)
        {
            DataTable dt = new DataTable();
            using (var conn = new SQLiteConnection(connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public EntryTransaction GetTransactionForReversal(string voucher)
        {
            string sql = "SELECT * FROM entryTransaction WHERE Voucher = @Voucher LIMIT 1";
            var p = new Dictionary<string, object> { { "@Voucher", voucher } };

            DataTable dt = GetData(sql, p);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new EntryTransaction
                {
                    Voucher = row["Voucher"].ToString(),
                    Description = row["Description"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    Note = row["Note"].ToString(),
                    TransDate = row["TransDate"].ToString(),
                    EntryCategoryName = row["EntryCategoryName"].ToString(),
                    TransactionType = row["TransactionType"].ToString()
                };
            }
            return null;
        }

        public EntryTransaction GetAttachmentByVoucher(string voucherNr)
        {
            const string sql = "SELECT Attachment, FileExt FROM EntryTransaction WHERE Voucher = @nr";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nr", voucherNr);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader["Attachment"] != DBNull.Value)
                            {
                                return new EntryTransaction
                                {
                                    Attachment = (byte[])reader["Attachment"],
                                    FileExt = reader["FileExt"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Laden des Attachments aus der Datenbank.", ex);
            }
            return null;
        }

        public DataTable FetchAssetData()
        {
            string sql = @"
            SELECT 
                a.Id AS AssetNr,
                t.Voucher, 
                a.Description,
                a.PurchaseDate,
                a.Amount,
                a.UsefulLifeYears,
                a.EntryTransactionId,
                a.Status,
                a.Note
            FROM asset a
            INNER JOIN EntryTransaction t ON a.EntryTransactionId = t.Id
            WHERE a.Status = 0
            ORDER BY a.PurchaseDate DESC;";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Hilfsspalten für die Anzeige-Logik hinzufügen
                        dt.Columns.Add("Restwert", typeof(decimal));
                        dt.Columns.Add("MonateVerbleibend", typeof(int));
                        dt.Columns.Add("AbgeschriebenProzent", typeof(int));

                        DateTime heute = DateTime.Now;

                        foreach (DataRow row in dt.Rows)
                        {
                            decimal anschaffungspreis = Convert.ToDecimal(row["Amount"]);
                            DateTime kaufdatum = Convert.ToDateTime(row["PurchaseDate"]);
                            int nutzungsdauerJahre = Convert.ToInt32(row["UsefulLifeYears"]);
                            int nutzungsdauerMonate = nutzungsdauerJahre * 12;

                            // Monate berechnen
                            int vergangeneMonate = ((heute.Year - kaufdatum.Year) * 12) + heute.Month - kaufdatum.Month;
                            if (vergangeneMonate < 0) vergangeneMonate = 0;

                            decimal monatlicheRate = nutzungsdauerMonate > 0 ? anschaffungspreis / nutzungsdauerMonate : 0;
                            int restMonate = nutzungsdauerMonate - vergangeneMonate;
                            decimal restwert = anschaffungspreis - (monatlicheRate * vergangeneMonate);

                            if (restwert <= 0)
                            {
                                restwert = 0;
                                restMonate = 0;
                            }

                            row["Restwert"] = Math.Round(restwert, 2);
                            row["MonateVerbleibend"] = restMonate;

                            int prozent = nutzungsdauerMonate > 0 ? (vergangeneMonate * 100 / nutzungsdauerMonate) : 100;
                            row["AbgeschriebenProzent"] = prozent > 100 ? 100 : prozent;
                        }
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler bei der Anlagenberechnung: " + ex.Message);
            }
        }

        public DataRow FetchDashboardStats()
        {
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

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Berechnen der Statistik: " + ex.Message);
            }
        }

        public int SaveCustomer(Customer cust)
        {
            // Wir nutzen last_insert_rowid(), um die vom System vergebene ID zu erhalten
            string sql = @"
            INSERT INTO customer (Name, Street, Zipcode, City, Country, Email) 
            VALUES (@Name, @Street, @Zipcode, @City, @Country, @Email);
            SELECT last_insert_rowid();";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", cust.Name);
                        cmd.Parameters.AddWithValue("@Street", cust.Street ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Zipcode", cust.Zipcode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@City", cust.City ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Country", cust.Country ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", cust.Email ?? (object)DBNull.Value);

                        // ExecuteScalar liefert das Ergebnis von last_insert_rowid()
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Speichern des Kunden in der Datenbank: " + ex.Message);
            }
        }

        public DataTable FetchAllCustomers()
        {
            string sql = "SELECT Id, Name, Street, Zipcode, City, Country, Email FROM customer ORDER BY Id DESC";
            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Laden der Kunden aus der Datenbank: " + ex.Message);
            }
        }

        public void SaveProduct(Product prod)
        {
            const string sql = @"
        INSERT INTO product (Name, Description, Price, Picture, PictureExtension, Stock, Stocked) 
        VALUES (@Name, @Description, @Price, @Picture, @Extension, @Stock, @Stocked)";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", prod.Name);
                        cmd.Parameters.AddWithValue("@Description", prod.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Price", prod.Price);
                        cmd.Parameters.AddWithValue("@Stock", prod.Stock);
                        cmd.Parameters.AddWithValue("@Stocked", prod.Stocked);

                        // Bild-Logik mit Null-Prüfung
                        cmd.Parameters.AddWithValue("@Picture", prod.Picture ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Extension", prod.PictureExtension ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Speichern des Produkts: " + ex.Message);
            }
        }

        public DataTable FetchProductList()
        {
            // Picture wird hier bewusst nicht geladen (Performance!)
            string sql = "SELECT Id, Name, Description, Price, Stocked, Stock FROM product ORDER BY Name ASC";

            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Laden der Produktliste: " + ex.Message);
            }
        }

        public byte[] FetchProductPicture(int productId)
        {
            string sql = "SELECT Picture FROM product WHERE Id = @Id";
            try
            {
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", productId);
                        object result = cmd.ExecuteScalar();

                        return result as byte[];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler beim Laden des Produktbildes: " + ex.Message);
            }
        }

        public void SaveSettings(Settings s)
        {
            // INSERT OR REPLACE sorgt dafür, dass die Zeile mit ID 1 
            // entweder erstellt oder aktualisiert wird.
            string sql = @"
        INSERT OR REPLACE INTO settings (
            Id, CompanyName, Owner, Street, ZipCode, City, Country, 
            Phone, Email, TaxNumber, BankName, IBAN, BIC, IntroText, FooterText, SmallBusinessNote, 
            CompanyImage, CompanyImageExtension
        ) VALUES (1, @CN, @O, @S, @Z, @C, @CO, @P, @E, @T, @BN, @I, @B, @IT, @FT, @SBN, @IMG, @EXT)";

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CN", s.CompanyName ?? "");
                        cmd.Parameters.AddWithValue("@O", s.Owner ?? "");
                        cmd.Parameters.AddWithValue("@S", s.Street ?? "");
                        cmd.Parameters.AddWithValue("@Z", s.ZipCode ?? "");
                        cmd.Parameters.AddWithValue("@C", s.City ?? "");
                        cmd.Parameters.AddWithValue("@CO", s.Country ?? "");
                        cmd.Parameters.AddWithValue("@P", s.Phone ?? "");
                        cmd.Parameters.AddWithValue("@E", s.Email ?? "");
                        cmd.Parameters.AddWithValue("@T", s.TaxNumber ?? "");
                        cmd.Parameters.AddWithValue("@BN", s.BankName ?? "");
                        cmd.Parameters.AddWithValue("@I", s.IBAN ?? "");
                        cmd.Parameters.AddWithValue("@B", s.BIC ?? "");
                        cmd.Parameters.AddWithValue("@IT", s.IntroText ?? "");
                        cmd.Parameters.AddWithValue("@FT", s.FooterText ?? "");
                        cmd.Parameters.AddWithValue("@SBN", s.SmallBusinessNote ?? "");
                        cmd.Parameters.AddWithValue("@IMG", s.CompanyImage ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EXT", s.CompanyImageExtension ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Da dies im Hintergrund passiert, loggen wir es eher, statt den User zu unterbrechen
                Console.WriteLine("Fehler beim automatischen Speichern: " + ex.Message);
            }
        }

        public Settings GetSettings()
        {
            string sql = "SELECT * FROM settings";
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Settings
                            {
                                CompanyName = reader["CompanyName"]?.ToString(),
                                Owner = reader["Owner"]?.ToString(),
                                Street = reader["Street"]?.ToString(),
                                ZipCode = reader["ZipCode"]?.ToString(),
                                City = reader["City"]?.ToString(),
                                Country = reader["Country"]?.ToString(),
                                Phone = reader["Phone"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                TaxNumber = reader["TaxNumber"]?.ToString(),
                                BankName = reader["BankName"]?.ToString(),
                                IBAN = reader["IBAN"]?.ToString(),
                                BIC = reader["BIC"]?.ToString(),
                                IntroText = reader["IntroText"]?.ToString(),
                                FooterText = reader["FooterText"]?.ToString(),
                                SmallBusinessNote = reader["SmallBusinessNote"]?.ToString(),
                                CompanyImage = reader["CompanyImage"] as byte[],
                                CompanyImageExtension = reader["CompanyImageExtension"]?.ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Fehler beim Laden: " + ex.Message); }
            return new Settings();
        }

        public List<Product> GetAllProducts()
        {
            List<Product> list = new List<Product>();
            string sql = "SELECT * FROM product ORDER BY Name ASC"; // Tabelle heißt 'product' bei dir?

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Product
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"])
                                // Falls du noch mehr Felder hast (z.B. Beschreibung), hier ergänzen
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Laden der Produkte: " + ex.Message);
            }
            return list;
        }

        public void SaveFullInvoice(Invoice invoice)
        {

            

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        
                        invoice.InvoiceNumber = GetNextInvoiceNumber();
                        invoice.PDF = GenerateInvoicePdf(invoice);

                        // 1. Rechnungs-Kopf speichern
                        string sqlHead = @"INSERT INTO invoice (InvoiceNumber, CustomerId, DateCreated, TotalAmount, IsPaid, PDF) 
                                   VALUES (@nr, @custId, @date, @total, @paid, @pdf);
                                   SELECT last_insert_rowid();"; // Holt die ID der neuen Rechnung

                        long newInvoiceId;
                        using (var cmd = new SQLiteCommand(sqlHead, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@nr", invoice.InvoiceNumber);
                            cmd.Parameters.AddWithValue("@custId", invoice.CustomerId);
                            cmd.Parameters.AddWithValue("@date", invoice.DateCreated.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@total", invoice.TotalAmount);
                            cmd.Parameters.AddWithValue("@paid", invoice.IsPaid);

                            cmd.Parameters.AddWithValue("@pdf", invoice.PDF);

                            newInvoiceId = (long)cmd.ExecuteScalar(); // Die ID für die Zeilen speichern
                        }

                        // 2. Alle Zeilen speichern
                        string sqlLine = @"INSERT INTO invoiceLine (InvoiceId, ProductId, ProductName, Quantity, CurrentPrice, LineTotal, LineNum) 
                                   VALUES (@invId, @prodId, @prodName, @qty, @price, @total, @num);";

                        foreach (var line in invoice.Lines)
                        {
                            using (var cmd = new SQLiteCommand(sqlLine, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@invId", newInvoiceId);
                                cmd.Parameters.AddWithValue("@prodId", line.ProductId);
                                cmd.Parameters.AddWithValue("@prodName", line.ProductName);
                                cmd.Parameters.AddWithValue("@qty", line.Quantity);
                                cmd.Parameters.AddWithValue("@price", line.CurrentPrice);
                                cmd.Parameters.AddWithValue("@total", line.LineTotal);
                                cmd.Parameters.AddWithValue("@num", line.LineNum);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Fehler beim Speichern der Rechnung: " + ex.Message);
                    }
                }
            }
        }

        public string GetNextInvoiceNumber()
        {
            int currentYear = DateTime.Now.Year;
            int nextNumber = 1;

            // Wir suchen nach der höchsten Nummer, die mit dem aktuellen Jahr beginnt
            string sql = "SELECT InvoiceNumber FROM invoice WHERE InvoiceNumber LIKE @yearPrefix ORDER BY Id DESC LIMIT 1";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@yearPrefix", currentYear + "-%");
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string lastNumberStr = result.ToString(); // z.B. "2025-0042"
                        string suffix = lastNumberStr.Split('-')[1]; // Holt die "0042"
                        if (int.TryParse(suffix, out int lastNr))
                        {
                            nextNumber = lastNr + 1;
                        }
                    }
                }
            }

            // Gibt die Nummer im Format "2025-0001" zurück
            return $"{currentYear}-{nextNumber.ToString("D4")}";
        }

        public byte[] GenerateInvoicePdf(Invoice invoice)
        {
            // QuestPDF Lizenz (für Community/Privat kostenlos)
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // --- HEADER ---
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("MEIN SALON").FontSize(20).SemiBold().FontColor(Colors.Pink.Medium);
                            col.Item().Text("Inhaberin: Deine Frau");
                            col.Item().Text("Musterstraße 1, 12345 Stadt");
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Rechnung Nr: {invoice.InvoiceNumber}").FontSize(14).SemiBold();
                            col.Item().Text($"Datum: {invoice.DateCreated:dd.MM.yyyy}");
                        });
                    });

                    // --- CONTENT (TABELLE) ---
                    page.Content().PaddingVertical(20).Table(table =>
                    {
                        // Spalten definieren (5 Spalten wie im DataGridView)
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // Pos
                            columns.RelativeColumn();    // Artikelname
                            columns.ConstantColumn(50);  // Menge
                            columns.ConstantColumn(80);  // Einzelpreis
                            columns.ConstantColumn(80);  // Gesamt
                        });

                        // Header-Zeile der Tabelle
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("Artikel");
                            header.Cell().Element(CellStyle).Text("Menge");
                            header.Cell().Element(CellStyle).Text("Preis");
                            header.Cell().Element(CellStyle).Text("Gesamt");

                            static IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        });

                        // Daten-Zeilen aus invoice.Lines
                        foreach (var line in invoice.Lines)
                        {
                            table.Cell().Element(MainCellStyle).Text(line.LineNum.ToString());
                            table.Cell().Element(MainCellStyle).Text(line.ProductName);
                            table.Cell().Element(MainCellStyle).Text(line.Quantity.ToString());
                            table.Cell().Element(MainCellStyle).Text($"{line.CurrentPrice:N2} €");
                            table.Cell().Element(MainCellStyle).Text($"{line.LineTotal:N2} €");

                            static IContainer MainCellStyle(IContainer container) =>
                                container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        }
                    });

                    // --- TOTALS ---
                    page.Footer().Column(col =>
                    {
                        col.Item().AlignRight().Text($"Gesamtbetrag: {invoice.TotalAmount:N2} €").FontSize(14).Bold();
                        col.Item().PaddingTop(20).AlignCenter().Text("Vielen Dank für Ihren Besuch!").Italic();
                    });
                });
            }).GeneratePdf(); // Erzeugt direkt das byte[]
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> list = new List<Customer>();
            string sql = "SELECT * FROM customer ORDER BY Name ASC";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Customer
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<Invoice> GetAllInvoices()
        {
            List<Invoice> invoices = new List<Invoice>();

            using (var connection = new SQLiteConnection(connString))
            {
                connection.Open();
                // Wir holen alle Spalten, die zu deiner Invoice-Klasse passen
                string sql = "SELECT * FROM invoice ORDER BY DateCreated DESC";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            invoices.Add(new Invoice
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                DateCreated = Convert.ToDateTime(reader["DateCreated"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                TotalAmount = Convert.ToDouble(reader["TotalAmount"])
                                // Falls du weitere Felder hast (z.B. Brutto/Netto), hier ergänzen
                            });
                        }
                    }
                }
            }
            return invoices;
        }

        public byte[] GetInvoiceBlob(int invoiceId)
        {
            using (var connection = new SQLiteConnection(connString))
            {
                connection.Open();
                // Wir holen nur das eine Feld 'PdfData' für die spezifische ID
                string sql = "SELECT PDF FROM invoice WHERE Id = @id";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", invoiceId);
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (byte[])result;
                    }
                }
            }
            return null;
        }
    }
}
