using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ContactPlanner
{
    public partial class EventWindow : Form
    {
        const int MIN_SYMBOLS = 20;

        Event m_currentEvent;

        public EventWindow(Event _newEvent)
        {
            InitializeComponent();

            m_currentEvent = _newEvent;
            
            updateContactList();

            dateTimePicker.Value = m_currentEvent.Date;
            comboBoxPriority.SelectedIndex = priorityToIndex(m_currentEvent.Priority);
            textBoxHeader.Text = m_currentEvent.Header;
            textBoxDescription.Text = m_currentEvent.Description;
        }


        private void updateContactList()
        {
            listContacts.Items.Clear();

            foreach (var _contact in m_currentEvent.Contacts)
                listContacts.Items.Add(_contact.FirstName + " " + _contact.LastName);

            listContacts.Items.Add("<Нажмите на список, чтобы добавить>");
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        static public PriorityKind indexToPriority(int _index)
        {
            switch (_index)
            {
                case 0: return PriorityKind.Low;
                case 1: return PriorityKind.Middle;
                case 2: return PriorityKind.High;
            }

            throw new Exception();
        }


        static public int priorityToIndex(PriorityKind _priority)
        {
            switch (_priority)
            {
                case PriorityKind.Low: return 0;
                case PriorityKind.Middle: return 1;
                case PriorityKind.High: return 2;
            }

            throw new Exception();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if(textBoxDescription.Text.Length == 0)
            {
                MessageBox.Show("Поле \"Описание\" осталось пустым!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(dateTimePicker.Value.Date.CompareTo(System.DateTime.Now.Date) < 0)
            {
                MessageBox.Show("Выбрана дата из прошлого!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(textBoxHeader.Text.Length == 0)
            {
                if (textBoxDescription.Text.Length < MIN_SYMBOLS)
                    textBoxHeader.Text = textBoxDescription.Text;
                else
                {
                    StringBuilder strBuild = new StringBuilder();

                    for (int i = 0; i < MIN_SYMBOLS; ++i)
                        strBuild.Append(textBoxDescription.Text[i]);

                    textBoxHeader.Text = strBuild.ToString() + "...";
                }
            }

            if (!Data.Events.ContainsKey(dateTimePicker.Value.Date))
                Data.Events.Add(dateTimePicker.Value.Date, new List<Event>());

            if (Data.Events[dateTimePicker.Value.Date].Contains(m_currentEvent))
            {
                int indexEdited = Data.Events[dateTimePicker.Value.Date].IndexOf(m_currentEvent);

                Data.Events[dateTimePicker.Value.Date][indexEdited].Date = dateTimePicker.Value;
                Data.Events[dateTimePicker.Value.Date][indexEdited].Header = textBoxHeader.Text;
                Data.Events[dateTimePicker.Value.Date][indexEdited].Description = textBoxDescription.Text;
                Data.Events[dateTimePicker.Value.Date][indexEdited].Priority = indexToPriority(comboBoxPriority.SelectedIndex);
            }
            else
            {
                m_currentEvent.Date = dateTimePicker.Value;
                m_currentEvent.Header = textBoxHeader.Text;
                m_currentEvent.Description = textBoxDescription.Text;
                m_currentEvent.Priority = indexToPriority(comboBoxPriority.SelectedIndex);

                Data.Events[dateTimePicker.Value.Date].Add(m_currentEvent);
            }

            Data.LastDate = dateTimePicker.Value.Date;

            Close();
        }


        private void listContacts_MouseDown(object sender, MouseEventArgs e)
        {
            Form addContacts = new SelectContactWindow(m_currentEvent.Contacts);
            addContacts.ShowDialog(this);
            m_currentEvent.Contacts = SelectContactWindow.SelectedContacts;
            updateContactList();
        }
    }
}
