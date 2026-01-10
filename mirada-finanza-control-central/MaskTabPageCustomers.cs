using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace mirada_finanza_control_central
{
    internal class MaskTabPageCustomers : MaskTabPageBase
    {
        protected DataGridView dataGridViewCustomers;
        protected TextBox textBoxCustomersId;
        protected TextBox textBoxCustomersName;
        protected TextBox textBoxCustomersStreet;
        protected TextBox textBoxCustomersCity;
        protected TextBox textBoxCustomersZipCode;
        protected TextBox textBoxCustomersCountry;
        protected TextBox textBoxCustomersEmail;
        protected Button buttonCustomersDelete;

        // Der Konstruktor muss die Parameter an die Basisklasse "durchreichen"
        public MaskTabPageCustomers(
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
                out this.dataGridViewCustomers,
                nameof(this.dataGridViewCustomers));

            this.AssignControl(
                out this.textBoxCustomersId,
                nameof(this.textBoxCustomersId));

            this.AssignControl(
                out this.textBoxCustomersName,
                nameof(this.textBoxCustomersName));

            this.AssignControl(
                out this.textBoxCustomersStreet,
                nameof(this.textBoxCustomersStreet));

            this.AssignControl(
                out this.textBoxCustomersCity,
                nameof(this.textBoxCustomersCity));

            this.AssignControl(
                out this.textBoxCustomersZipCode,
                nameof(this.textBoxCustomersZipCode));

            this.AssignControl(
                out this.textBoxCustomersCountry,
                nameof(this.textBoxCustomersCountry));

            this.AssignControl(
                out this.textBoxCustomersEmail,
                nameof(this.textBoxCustomersEmail));

            this.AssignControl(
                out this.buttonCustomersDelete,
                nameof(this.buttonCustomersDelete));

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
            try
            {
                // 1. Daten vom Manager holen
                DataTable dt = dbManager.FetchAllCustomers();

                // 2. Binden
                dataGridViewCustomers.DataSource = null;
                dataGridViewCustomers.AutoGenerateColumns = false;

                dataGridViewCustomers.Columns["Id"].DataPropertyName = "Id";
                dataGridViewCustomers.Columns["Name"].DataPropertyName = "Name";


                dataGridViewCustomers.DataSource = dt;

                // 3. Optik-Finishing
                if (dataGridViewCustomers.Columns.Count > 0)
                {
                    ApplyCustomerGridStyle();
                }

                dataGridViewCustomers.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void ApplyCustomerGridStyle()
        {
            // Umbruch-Style definieren
            DataGridViewCellStyle wrapStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True };
            dataGridViewCustomers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Allgemeine Einstellungen
            dataGridViewCustomers.AllowUserToAddRows = false;
            dataGridViewCustomers.ReadOnly = true;
            dataGridViewCustomers.RowHeadersVisible = false;
            dataGridViewCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCustomers.MultiSelect = false;
            dataGridViewCustomers.EnableHeadersVisualStyles = false;
            dataGridViewCustomers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        }

        public void dataGridViewCustomersSelectionChanged()
        {
            // 1. Sicherheitscheck: Gibt es eine aktive Zeile mit Daten?
            if (dataGridViewCustomers.CurrentRow != null &&
                dataGridViewCustomers.CurrentRow.DataBoundItem != null)
            {
                // 2. Die ID aus der Zelle holen (Achte darauf, dass "Id" der Name der Spalte im Designer ist!)
                var cellValue = dataGridViewCustomers.CurrentRow.Cells["Id"].Value;

                if (cellValue != null && int.TryParse(cellValue.ToString(), out int selectedId))
                {
                    // 3. Daten laden
                    Customer customer = dbManager.GetCustomerById(selectedId);

                    if (customer != null)
                    {
                        // 4. Textboxen befüllen
                        textBoxCustomersId.Text = customer.Id.ToString();
                        textBoxCustomersName.Text = customer.Name;
                        textBoxCustomersStreet.Text = customer.Street;
                        textBoxCustomersCity.Text = customer.City;
                        textBoxCustomersZipCode.Text = customer.Zipcode;
                        textBoxCustomersCountry.Text = customer.Country;
                        textBoxCustomersEmail.Text = customer.Email;

                        textBoxCustomersName.Focus();
                        buttonCustomersDelete.Enabled = true;
                    }
                }
            }
            else
            {
                // Nichts ausgewählt -> Felder leeren
                ClearCustomerDetailFields();
                buttonCustomersDelete.Enabled = false;
            }
        }

        public void buttonCustomersNewClick(
            object _sender,
            EventArgs _e)
        {
            try
            {
                // 1. Nur die ID in der Datenbank reservieren
                int newId = dbManager.CreateEmptyCustomer();

                // 2. Das Grid aktualisieren (ruft deine Fetch-Methode auf)
                RefreshData();

                // 3. Den neuen Eintrag im Grid suchen und selektieren
                foreach (DataGridViewRow row in dataGridViewCustomers.Rows)
                {
                    if (Convert.ToInt32(row.Cells["Id"].Value) == newId)
                    {
                        row.Selected = true;
                        dataGridViewCustomers.CurrentCell = row.Cells["Id"];
                        break;
                    }
                }

                textBoxCustomersId.Text = newId.ToString();

                // Andere Infos von DB holen.
                Customer customer = dbManager.GetCustomerById(newId);
                textBoxCustomersId.Text = customer.Id.ToString();
                textBoxCustomersName.Text = customer.Name;
                textBoxCustomersStreet.Text = customer.Street;
                textBoxCustomersCity.Text = customer.City;
                textBoxCustomersZipCode.Text = customer.Zipcode;
                textBoxCustomersCountry.Text = customer.Country;
                textBoxCustomersEmail.Text = customer.Email;

                textBoxCustomersName.Focus();
                buttonCustomersDelete.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen der Kundennummer: " + ex.Message);
            }
        }

        private void SaveCurrentCustomerFromUI()
        {
            // 1. Validierung: Haben wir überhaupt eine ID?
            if (string.IsNullOrWhiteSpace(textBoxCustomersId.Text) || !int.TryParse(textBoxCustomersId.Text, out int customerId))
            {
                return;
            }

            try
            {
                // 2. Objekt mit den aktuellen Werten aus den Textboxen erstellen
                Customer customerToUpdate = new Customer
                {
                    Id = customerId,
                    Name = textBoxCustomersName.Text.Trim(),
                    Street = textBoxCustomersStreet.Text.Trim(),
                    Zipcode = textBoxCustomersZipCode.Text.Trim(),
                    City = textBoxCustomersCity.Text.Trim(),
                    Country = textBoxCustomersCountry.Text.Trim(),
                    Email = textBoxCustomersEmail.Text.Trim()
                };

                // 3. Ab in die Datenbank damit
                dbManager.UpdateCustomer(customerToUpdate);

                // 4. Das Grid links aktualisieren, damit dort nicht noch der alte Name steht
                if (dataGridViewCustomers.CurrentRow != null)
                {
                    DataRowView row = (DataRowView)dataGridViewCustomers.CurrentRow.DataBoundItem;
                    // Nur aktualisieren, wenn der Wert im Grid wirklich anders ist (verhindert Flackern)
                    if (row["Name"].ToString() != customerToUpdate.Name)
                    {
                        row["Name"] = customerToUpdate.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim automatischen Speichern: " + ex.Message);
            }
        }

        public void textBoxCustomersNameLeave()
        {
            SaveCurrentCustomerFromUI();
        }

        public void textBoxCustomersStreetLeave()
        {
            SaveCurrentCustomerFromUI();
        }

        public void textBoxCustomersZipCodeLeave()
        {
            SaveCurrentCustomerFromUI();
        }

        public void textBoxCustomersCityLeave()
        {
            SaveCurrentCustomerFromUI();
        }

        public void textBoxCustomersCountryLeave()
        {
            SaveCurrentCustomerFromUI();
        }

        public void textBoxCustomersEmailLeave()
        {
            SaveCurrentCustomerFromUI();
        }

        public void tabPageCustomersLeave()
        {
            SaveCurrentCustomerFromUI();
        }

        public void buttonCustomersDeleteClick(
            object _sender,
            EventArgs _e)
        {
            // 1. Prüfen, ob überhaupt jemand im Grid ausgewählt ist
            if (dataGridViewCustomers.CurrentRow == null) return;

            // 2. ID und Name für die Bestätigung holen
            // "Id" muss der Name deiner Spalte im DataGridView-Designer sein
            int id = Convert.ToInt32(dataGridViewCustomers.CurrentRow.Cells["Id"].Value);
            string name = textBoxCustomersName.Text; // Wir nehmen den Namen direkt aus der Textbox

            // 3. Sicherheitsabfrage
            var result = MessageBox.Show(
                $"Möchtest du den Kunden '{name}' (ID: {id}) wirklich unwiderruflich löschen?",
                "Löschen bestätigen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 4. Aus DB löschen
                    dbManager.DeleteCustomer(id);

                    // 5. UI aufräumen
                    RefreshData(); // Grid neu laden
                    ClearCustomerDetailFields(); // Alle Textboxen rechts leeren

                    MessageBox.Show("Kunde wurde erfolgreich gelöscht.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Löschen: " + ex.Message);
                }
            }
        }

        protected void ClearCustomerDetailFields()
        {
            textBoxCustomersId.Text = "";
            textBoxCustomersName.Text = "";
            textBoxCustomersStreet.Text = "";
            textBoxCustomersCity.Text = "";
            textBoxCustomersZipCode.Text = "";
            textBoxCustomersCountry.Text = "";
            textBoxCustomersEmail.Text = "";
        }

    }
}
