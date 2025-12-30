using System;
using System.Collections.Generic;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class EntryTransaction
    {
        public long ID { get; set; }
        public string Voucher { get; set; }
        public int Year { get; set; }
        public string EntryCategoryName { get; set; }
        public string TransactionType { get; set; } // "Einnahme" oder "Ausgabe"
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string TransDate { get; set; }      // Als String für SQLite (YYYY-MM-DD)
        public string CreatedDate { get; set; }
        public byte[] Attachment { get; set; }     // Für das BLOB (Beleg-Datei)
        public string FileExt { get; set; }        // .pdf, .jpg, etc.
        public bool Reversal { get; set; }          // 0 = Normal, 1 = Storno
        public string ReversalReferenceVoucher { get; set; }
        public string InvoiceReference { get; set; }

        // Hilfs-Property: Falls du im Code lieber mit DateTime arbeitest
        public DateTime GetTransDateAsDateTime()
        {
            DateTime.TryParse(TransDate, out DateTime dt);
            return dt;
        }
    }
}
