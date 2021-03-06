﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace ContactPlanner
{
    public partial class MainWindow : Form
    {
        private void tabControl_DrawItem(Object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush textBrush;

            // Get the item from the collection.
            TabPage tabPage = tabControl.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle tabBounds = tabControl.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {
                // Draw a different background color, and don't paint a focus rectangle.
                textBrush = new SolidBrush(Color.Black);
                g.FillRectangle(Brushes.White, e.Bounds);
            }
            else
            {
                textBrush = new SolidBrush(Color.White);
                g.FillRectangle(Brushes.Crimson, e.Bounds);
            }

            // Use our own font.
            Font tabFont = new Font(
                    "Verdana"
                ,   16.0f
                ,   FontStyle.Regular
                ,   GraphicsUnit.Pixel
                );

            // Draw string. Center the text.
            StringFormat stringFlags = new StringFormat();
            stringFlags.Alignment = StringAlignment.Center;
            stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(
                    tabPage.Text
                ,   tabFont
                ,   textBrush
                ,   tabBounds
                ,   new StringFormat(stringFlags)
                );
        }


        private void buttonCreateEvent_Click(object sender, EventArgs e)
        {
            Form addEvent = new EventWindow(
                new Event(
                        new DateTime(monthCalendar.SelectionStart.Ticks)
                    ,   ""
                    ,   ""
                    ,   new List<Contact>()
                    ,   PriorityKind.Low
                    ,   ++Data.LastId
                    )
                );

            addEvent.ShowDialog(this);
            updateBoldedDates(Data.LastDate);
            updateDataEvents();

            if (isShowAll)
                buttonShow_Click(sender, e);
        }


        private void monthCalendarUpdate(object sender, DateRangeEventArgs e)
        {
            updateDataEvents();

            if (isShowAll)
                buttonShow_Click(sender, e);

            dataGridViewEvents_RowStateChanged(
                    this
                ,   new DataGridViewRowStateChangedEventArgs(
                        dataGridViewEvents.SelectedRows[0]
                    ,   DataGridViewElementStates.None
                    )
                );
        }


        private void buttonDeleteEvent_Click(object sender, EventArgs e)
        {
            if (dataGridViewEvents.CurrentRow.Index == -1)
                return;

            Event selectedEvent = 
                m_currentEventsInDataGrid[dataGridViewEvents.CurrentRow.Index];

            Data.Events[selectedEvent.getDate().Date].Remove(selectedEvent);
            DateTime deletedEventDate = selectedEvent.getDate().Date;
            m_currentEventsInDataGrid.Remove(selectedEvent);

            updateDataEvents();
            updateBoldedDates(deletedEventDate);

            removeEmptyDays();

            dataGridViewEvents_RowStateChanged(
                    this
                ,   new DataGridViewRowStateChangedEventArgs(
                        dataGridViewEvents.SelectedRows[0]
                    ,   DataGridViewElementStates.None
                    )
                );
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
                    orderby _event.getDate().ToBinary() ascending
                    select _event
                    ).ToList<Event>();

                this.Text = "Планировщик. Все события.";

                dataGridViewEvents.Columns[0].Visible = false;
                dataGridViewEvents.Columns[1].Visible = true;

                m_currentEventsInDataGrid = result;
                m_bindingEvents.DataSource = m_currentEventsInDataGrid;
                dataGridViewEvents.DataSource = m_bindingEvents;
                m_bindingEvents.ResetBindings(true);

                changeColorEvents();
            }
            else
            {
                dataGridViewEvents.Columns[0].Visible = true;
                dataGridViewEvents.Columns[1].Visible = false;

                buttonShow.Text = "Отобразить все события";
                updateDataEvents();
            }

            if (dataGridViewEvents.SelectedRows.Count != 0)
                dataGridViewEvents_RowStateChanged(
                        this
                    ,   new DataGridViewRowStateChangedEventArgs(
                            dataGridViewEvents.SelectedRows[0]
                        ,   DataGridViewElementStates.None
                        )
                    );
        }


        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                updateDataEvents();
                dataGridViewEvents_RowStateChanged(
                    this
                ,   new DataGridViewRowStateChangedEventArgs(
                        dataGridViewEvents.SelectedRows[0]
                    ,   DataGridViewElementStates.None
                    )
                );
            }
            else if (tabControl.SelectedIndex == 1)
                updateDataContacts(Data.Contacts);

            textBoxSearch.Text = "<Введите текст для поиска среди контактов>";
        }


        private void buttonAddContact_Click(object sender, EventArgs e)
        {
            Form addContact = new ContactWindow(new Contact("", "", "", "", ""));
            addContact.ShowDialog(this);
            updateDataContacts(Data.Contacts);
        }


        private void buttonDeleteContact_Click(object sender, EventArgs e)
        {
            Contact contactDeleted = 
                Data.Contacts[dataGridViewContacts.CurrentRow.Index];

            foreach (var _pair in Data.Events)
                foreach(var _event in _pair.Value)
                    _event.Contacts.Remove(contactDeleted);

            Data.Contacts.Remove(contactDeleted);
            updateDataContacts(Data.Contacts);
        }


        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            removeEmptyDays();
            saveData(m_currentPathToDataFile);
        }


        private void MainWindow_Load(object sender, EventArgs e)
        {
            restoreData(m_currentPathToDataFile);

            foreach (var _pair in Data.Events)
                if(_pair.Value.Count != 0)
                    monthCalendar.AddBoldedDate(_pair.Key);

            monthCalendar.UpdateBoldedDates();

            updateDataContacts(Data.Contacts);
            updateDataEvents();

            dataGridViewEvents_RowStateChanged(
                    this
                ,   new DataGridViewRowStateChangedEventArgs(
                        dataGridViewEvents.Rows[0]
                    ,   DataGridViewElementStates.None
                    )
                );
        }


        private void dataGridViewEvents_CellContentDoubleClick(
                object sender
            ,   DataGridViewCellEventArgs e
            )
        {
            if (e.RowIndex == -1)
                return;

            Form editEvent = 
                new EventWindow(m_currentEventsInDataGrid[e.RowIndex]);
            editEvent.ShowDialog(this);
            updateBoldedDates(Data.LastDate);
            m_currentEventsInDataGrid = mergeEventList(Data.Events[monthCalendar.SelectionStart], newEmptyEventList());
            m_bindingEvents.DataSource = m_currentEventsInDataGrid;
            m_bindingEvents.ResetBindings(true);
            changeColorEvents();
            removeEmptyDays();
        }


        private void dataGridViewContacts_CellContentDoubleClick(
                object sender
            ,   DataGridViewCellEventArgs e
            )
        {
            if (e.RowIndex == -1)
                return;

            Form editContact = 
                new ContactWindow(m_currentContactsInDataGrid[e.RowIndex]);
            editContact.ShowDialog(this);
            updateDataContacts(Data.Contacts);
            textBoxSearch.Text = "<Введите текст для поиска среди контактов>";
        }


        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            const int KEY_ENTER = 13;

            if (e.KeyChar == KEY_ENTER)
                searchContact(textBoxSearch.Text);
        }


        private void textBoxSearch_Leave(object sender, EventArgs e)
        {
            if (textBoxSearch.Text.Length == 0)
                textBoxSearch.Text = "<Введите текст для поиска среди контактов>";
        }


        private void textBoxSearch_Enter(object sender, EventArgs e)
        {
            if (textBoxSearch.Text == "<Введите текст для поиска среди контактов>")
                textBoxSearch.Text = "";
        }


        private void toolStripMenuSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.InitialDirectory = 
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_currentPathToDataFile = saveFileDialog.FileName;
                saveData(m_currentPathToDataFile);
            }
        }


        private void toolStripMenuLoad_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = 
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_currentPathToDataFile = openFileDialog.FileName;
                MainWindow_Load(sender, e);
            }
        }


        private void dataGridViewContacts_ColumnHeaderMouseClick(
                object sender
            ,   DataGridViewCellMouseEventArgs e
            )
        {
            List<Contact> result = new List<Contact>();

            if (e.ColumnIndex == 0)
            {
                result = (
                    from _contact in m_currentContactsInDataGrid
                    orderby _contact.FirstName ascending
                    select _contact
                    ).ToList<Contact>();
            }
            else if (e.ColumnIndex == 1)
            {
                result = (
                    from _contact in m_currentContactsInDataGrid
                    orderby _contact.SecondName ascending
                    select _contact
                    ).ToList<Contact>();
            }
            else if (e.ColumnIndex == 2)
            {
                result = (
                    from _contact in m_currentContactsInDataGrid
                    orderby _contact.LastName ascending
                    select _contact
                    ).ToList<Contact>();
            }
            else if (e.ColumnIndex == 3)
            {
                result = (
                    from _contact in m_currentContactsInDataGrid
                    orderby _contact.Telephone ascending
                    select _contact
                    ).ToList<Contact>();
            }
            else
            {
                result = (
                    from _contact in m_currentContactsInDataGrid
                    orderby _contact.Email ascending
                    select _contact
                    ).ToList<Contact>();
            }

            m_currentContactsInDataGrid = result; // Для корректной работы редактирования
            m_bindingContacts.DataSource = result;
            m_bindingContacts.ResetBindings(true);
        }

        private void dataGridViewEvents_RowStateChanged(
                object sender
            ,   DataGridViewRowStateChangedEventArgs e
            )
        {
            if (Data.LastIndex < dataGridViewEvents.Rows.Count)
                for (int i = 0; i < dataGridViewEvents.Rows[Data.LastIndex].Cells.Count; ++i)
                    dataGridViewEvents.Rows[Data.LastIndex].Cells[i].Style.Font = m_systemFont;

            if (e.Row.Index == -1)
                return;

            for (int i = 0; i < dataGridViewEvents.Rows[e.Row.Index].Cells.Count; ++i)
                dataGridViewEvents.Rows[e.Row.Index].Cells[i].Style.Font = m_boldFont;

            Data.LastIndex = e.Row.Index;
        }

        private void dataGridViewEvents_SelectionChanged(
                object sender
            ,   EventArgs e
            )
        {
            if (dataGridViewEvents.SelectedRows.Count != 0)
                buttonDeleteEvent.Enabled =
                    m_currentEventsInDataGrid[
                        dataGridViewEvents.SelectedRows[0].Index
                        ]
                    .getPriority() !=
                    PriorityKind.Unspecified;
        }
    }
}
