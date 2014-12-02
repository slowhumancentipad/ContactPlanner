using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace ContactPlanner
{
    public partial class MainWindow : Form
    {
        private void tabControl_DrawItem(Object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabControl.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabControl.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {

                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new System.Drawing.SolidBrush(Color.Black);
                g.FillRectangle(Brushes.White, e.Bounds);
            }
            else
            {
                _textBrush = new SolidBrush(Color.White);
                g.FillRectangle(Brushes.Crimson, e.Bounds);

            }

            // Use our own font.
            Font _tabFont = new Font("Verdana", (float)16.0, FontStyle.Regular, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }


        private void buttonCreateEvent_Click(object sender, EventArgs e)
        {
            Form addEvent = new EventWindow(new Event(monthCalendar.SelectionStart, "", "", new List<Contact>(), PriorityKind.Low, ++Data.LastId));// DateTime.Now --> monthCalendar.SelectionStart
            addEvent.ShowDialog(this);
            updateBoldedDates();
            updateDataEvents();

            if (isShowAll)
                buttonShow_Click(sender, e);
        }


        private void monthCalendarUpdate(object sender, DateRangeEventArgs e)
        {
            updateDataEvents();

            if (isShowAll)
                buttonShow_Click(sender, e);
        }


        private void buttonDeleteEvent_Click(object sender, EventArgs e)
        {
            Data.Events[Data.CurrentDate].RemoveAt(dataGridViewEvents.CurrentRow.Index);
            updateBoldedDates();
            updateDataEvents();
        }


        private void buttonShow_Click(object sender, EventArgs e)
        {
            isShowAll = !isShowAll;

            if (isShowAll)
            {
                buttonShow.Text = "Отобразить события выбранной даты";

                List<Event> allEvents = new List<Event>();

                foreach (var _dayEvent in Data.Events)
                    allEvents.AddRange(_dayEvent.Value);

                var result = (
                    from _event in allEvents
                    orderby _event.Date.ToBinary() ascending
                    select _event
                    ).ToList<Event>();

                this.Text = "Планировщик. Все события.";

                m_bindingEvents.DataSource = allEvents;
                dataGridViewEvents.DataSource = m_bindingEvents;
                m_bindingEvents.ResetBindings(true);

                buttonDeleteEvent.Enabled = false;
            }
            else
            {
                buttonShow.Text = "Отобразить все события";
                updateDataEvents();
            }
        }


        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
                updateDataEvents();
            else if (tabControl.SelectedIndex == 1)
                updateDataContacts();
        }


        private void buttonAddContact_Click(object sender, EventArgs e)
        {
            Form addContact = new ContactWindow(new Contact("", "", "", "", ""));
            addContact.ShowDialog(this);
            updateDataContacts();
        }


        private void buttonDeleteContact_Click(object sender, EventArgs e)
        {
            Data.Contacts.RemoveAt(dataGridViewContacts.CurrentRow.Index);
            updateDataContacts();
        }


        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveContacts(Data.Contacts, "contacts.txt", Data.Contacts.Count);
            saveEvents(Data.Events, "events.txt");
        }


        private void MainWindow_Load(object sender, EventArgs e)
        {
            restoreContacts(Data.Contacts, "contacts.txt");
            restoreEvents(Data.Events, "events.txt");
            updateDataContacts();
            updateDataEvents();
        }


        private void dataGridViewEvents_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (isShowAll)
                return;

            Form editEvent = new EventWindow(Data.Events[monthCalendar.SelectionStart][e.RowIndex]);
            editEvent.ShowDialog(this);
            updateBoldedDates();
            updateDataEvents();
        }


        private void dataGridViewContacts_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Form editContact = new ContactWindow(Data.Contacts[e.RowIndex]);
            editContact.ShowDialog(this);
            updateDataContacts();
        }
    }
}
