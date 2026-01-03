using System;
using System.Collections.Generic;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }

        // Hilfseigenschaft für die Anzeige in Listen (z.B. ComboBoxen)
        // Zeigt "Name (Stadt)" an, damit man Kunden mit gleichem Namen unterscheiden kann
        public string DisplayName => $"{Name} ({City})";

        // Hilfseigenschaft für die komplette Anschrift (nützlich für Rechnungen)
        public string FullAddress => $"{Street}, {Zipcode} {City}";
    }
}
