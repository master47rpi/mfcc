using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class MaskTabPageCustomerEntry : MaskTabPageBase
    {
        protected TextBox textBoxCustomerEntryName;
        protected TextBox textBoxCustomerEntryStreet;
        protected TextBox textBoxCustomerEntryZipCode;
        protected TextBox textBoxCustomerEntryCity;
        protected TextBox textBoxCustomerEntryCountry;
        protected TextBox textBoxCustomerEntryEmail;



        // Der Konstruktor muss die Parameter an die Basisklasse "durchreichen"
        public MaskTabPageCustomerEntry(
            TabControl _tabControl,
            TabPage _tabPage,
            Button _callingButton,
            DBManager _dbManager)
            : base(
                  _tabControl,
                  _tabPage,
                  _callingButton,
                  _dbManager)
        {
            // Hier kannst du spezifische Dinge für die interne Erfassung machen
            this.AssignControl(
                out this.textBoxCustomerEntryName,
                nameof(this.textBoxCustomerEntryName));

            this.AssignControl(
                out this.textBoxCustomerEntryStreet,
                nameof(this.textBoxCustomerEntryStreet));

            this.AssignControl(
                out this.textBoxCustomerEntryZipCode,
                nameof(this.textBoxCustomerEntryZipCode));

            this.AssignControl(
                out this.textBoxCustomerEntryCity,
                nameof(this.textBoxCustomerEntryCity));

            this.AssignControl(
                out this.textBoxCustomerEntryCountry,
                nameof(this.textBoxCustomerEntryCountry));

            this.AssignControl(
                out this.textBoxCustomerEntryEmail,
                nameof(this.textBoxCustomerEntryEmail));

        }

        public override void Clear()
        {
            // Erst die allgemeine Logik der Basisklasse (alle Textboxen etc. leeren)
            base.Clear();

            // Dann spezifische Dinge für DIESE Maske
            // z.B. ein spezielles Grid leeren, das nur hier existiert
        }

        public override void Show()
        {
            // Erst die Logik der Basisklasse (Tab wechseln, Button highlighten)
            base.Show();

            // Dann z.B. Daten frisch aus der DB laden
            this.RefreshData();
        }

        public void RefreshData()
        {
        }

        public void buttonCustomerEntrySaveClick()
        {
            tabPageCustomerEntrySaveCustomer();
        }

        private void tabPageCustomerEntrySaveCustomer()
        {
            // 1. Model-Objekt befüllen
            Customer newCust = new Customer
            {
                Name = textBoxCustomerEntryName.Text.Trim(),
                Street = textBoxCustomerEntryStreet.Text.Trim(),
                Zipcode = textBoxCustomerEntryZipCode.Text.Trim(),
                City = textBoxCustomerEntryCity.Text.Trim(),
                Country = textBoxCustomerEntryCountry.Text.Trim(),
                Email = textBoxCustomerEntryEmail.Text.Trim()
            };

            // Validierung: Name ist Pflicht (Bleibt in der UI, da es die Benutzerführung betrifft)
            if (string.IsNullOrWhiteSpace(newCust.Name))
            {
                MessageBox.Show("Bitte geben Sie mindestens einen Namen ein.", "Eingabe fehlt");
                return;
            }

            try
            {
                // 2. Über Manager speichern und ID erhalten
                int newId = dbManager.SaveCustomer(newCust);

                MessageBox.Show($"Kunde erfolgreich unter ID {newId} angelegt!", "Erfolg");

                // 3. UI aufräumen
                tabPageCustomerEntryClear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Datenbankfehler");
            }
        }

        protected void tabPageCustomerEntryClear()
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


    }
}
