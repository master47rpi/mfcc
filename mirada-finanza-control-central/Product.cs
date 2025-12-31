using System;
using System.Collections.Generic;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // In SQLite ist REAL ein double, in C# nutzen wir für Preise 
        // wegen der Rechengenauigkeit lieber decimal.
        public decimal Price { get; set; }

        // Das Bild als Byte-Array (BLOB)
        public byte[] Picture { get; set; }

        public string PictureExtension { get; set; }

        public int Stock { get; set; }

        // Kennzeichen, ob das Produkt überhaupt lagergeführt ist 
        // (0 = Nein, 1 = Ja)
        public int Stocked { get; set; }

        // Hilfseigenschaft: Gibt zurück, ob ein Bild vorhanden ist
        public bool HasPicture => Picture != null && Picture.Length > 0;

        // Hilfseigenschaft für die Anzeige in Listen
        public string DisplayInfo => $"{Name} ({Price:C2})";
    }
}
