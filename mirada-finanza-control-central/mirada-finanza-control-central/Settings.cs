using System;
using System.Collections.Generic;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class Settings
    {
        // Firma & Inhaber
        public string CompanyName { get; set; }
        public string Owner { get; set; }

        // Anschrift
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        // Kontakt
        public string Phone { get; set; }
        public string Email { get; set; }

        // Steuer & Bank (Wichtig für die Fußzeile)
        public string TaxNumber { get; set; }    // Deine Steuernummer
        public string BankName { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }

        // Rechnungs-Konfiguration (GroupBox 4)
        public string InvoicePrefix { get; set; }     // z.B. "RE-"
        public string IntroText { get; set; }         // Text oben auf der Rechnung
        public string FooterText { get; set; }        // Text ganz unten (z.B. Dankeschön)
        public string SmallBusinessNote { get; set; } // Der § 19 UStG Hinweis

        // Logo Daten
        public byte[] CompanyImage { get; set; }      // Das Bild als Byte-Array
        public string CompanyImageExtension { get; set; } // .jpg, .png etc.

        // Hilfs-Eigenschaft für die Anzeige oder den Druck
        public string FullAddress => $"{Street}, {ZipCode} {City}";
    }
}
