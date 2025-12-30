using System;
using System.Collections.Generic;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class Asset
    {
        public long Id { get; set; }
        public long EntryTransactionId { get; set; } // Die Verknüpfung zur Buchung
        public string Description { get; set; }
        public string PurchaseDate { get; set; }      // Format: YYYY-MM-DD
        public decimal Amount { get; set; }           // Brutto-Betrag
        public int UsefulLifeYears { get; set; }      // AfA-Dauer
        public string Note { get; set; }
        public int Status { get; set; }               // 0 = Aktiv, 1 = Abgeschrieben/Verkauft

        // Hilfs-Property für Berechnungen: 
        // Liefert das Anschaffungsdatum als echtes DateTime Objekt
        public DateTime GetPurchaseDateAsDateTime()
        {
            DateTime.TryParse(PurchaseDate, out DateTime dt);
            return dt;
        }
    }
}
