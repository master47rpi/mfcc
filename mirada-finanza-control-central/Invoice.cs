using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class Invoice
    {
        public int Id { get; set; }

        // Entspricht deiner InvoiceNumber TEXT UNIQUE
        public string InvoiceNumber { get; set; }

        public int CustomerId { get; set; }

        // Wir nutzen DateTime in C#, SQLite speichert es als TEXT
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Gesamtsumme der Rechnung
        public double TotalAmount { get; set; }

        // 0 = Offen, 1 = Bezahlt
        public int IsPaid { get; set; } = 0;

        public byte[] PDF { get; set; }

        // Die Liste der einzelnen Positionen (für das DataGridView)
        // Wir nutzen BindingList, damit das UI automatisch auf Änderungen reagiert
        public BindingList<InvoiceLine> Lines { get; set; } = new BindingList<InvoiceLine>();
    }
}
