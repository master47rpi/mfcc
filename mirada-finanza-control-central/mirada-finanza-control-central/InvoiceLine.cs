using System;
using System.Collections.Generic;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class InvoiceLine
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int? ProductId { get; set; }

        // Der Name des Produkts (hilfreich für die Anzeige im Grid)
        public string ProductName { get; set; }

        public double Quantity { get; set; } = 1.0;

        // Entspricht deinem CurrentPrice (Preis zum Zeitpunkt des Verkaufs)
        public double CurrentPrice { get; set; }

        // Quantity * CurrentPrice
        public double LineTotal { get; set; }

        // Die Sortierreihenfolge auf der Rechnung
        public double LineNum { get; set; }

        // WinForms braucht diesen leeren Konstruktor für neue Zeilen!
        public InvoiceLine() { }
    }
}
