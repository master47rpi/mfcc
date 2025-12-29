using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

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
    }
}
