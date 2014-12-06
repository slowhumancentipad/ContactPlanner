using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace ContactPlanner
{
    public partial class MainWindow : Form
    {
        BindingSource m_bindingEvents = new BindingSource();
        BindingSource m_bindingContacts = new BindingSource();

        List<Contact> m_currentContactsInDataGrid = new List<Contact>();
        List<Event> m_currentEventsInDataGrid = new List<Event>();

        string m_currentPathToDataFile = "data.cpf";

        bool isShowAll = false;

        Color m_colorLow = Color.LightYellow;
        Color m_colorMiddle = Color.Yellow;
        Color m_colorHigh = Color.Orange;

        Font m_systemFont = new Font("Microsoft Sans Serif", 8.25f);

        public MainWindow(string[] args)
        {
            Data.Contacts = new List<Contact>();
            Data.Events = new Dictionary<DateTime, List<Event>>();
            Data.LastId = 0;

            if (args.Length > 1)
                m_currentPathToDataFile = args[1];

            InitializeComponent();

            tabControl.DrawItem += new DrawItemEventHandler(tabControl_DrawItem);

            #region AddDataGridEventsColumn

            dataGridViewEvents.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "DateAndTime",
                    HeaderText = "Дата и время",
                    Width = 103,
                    Visible = false
                });

            dataGridViewEvents.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Time",
                    HeaderText = "Время",
                    Width = 50
                });

            dataGridViewEvents.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Header",
                    HeaderText = "Заголовок",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

            dataGridViewEvents.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Description",
                    HeaderText = "Описание",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

            #endregion // AddDataGridEventsColumn

            #region AddDataGridContactsColumn

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "LastName",
                    HeaderText = "Фамилия",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "FirstName",
                    HeaderText = "Имя",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "SecondName",
                    HeaderText = "Отчество",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Telephone",
                    HeaderText = "Телефон",
                    Width = dataGridViewContacts.Width / 6
                });

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Email",
                    HeaderText = "E-Mail",
                    Width = dataGridViewContacts.Width / 5
                });

            #endregion // AddDataGridContactsColumn

            Data.LastIndex = 0;

        }


        private List<Event> mergeEventList(List<Event> _list1, List<Event> _list2)
        {
            Comparison<Event> compareByTime =
                (lhs, rhs) =>
                {
                    if (lhs.getDate().Hour == rhs.getDate().Hour)
                        return lhs.getDate().Minute.CompareTo(rhs.getDate().Minute);

                    return lhs.getDate().Hour.CompareTo(rhs.getDate().Hour);
                };

            for (int i = 0; i < _list1.Count; ++i)
                if (_list1[i].getDate().Minute == 0)
                    for (int j = 0; j < _list2.Count; j++)
                        if (_list1[i].getDate() == _list2[j].getDate())
                            _list2.RemoveAt(j);

            List<Event> tmp = new List<Event>(_list1);

            tmp.AddRange(_list2);
            tmp.Sort(compareByTime);

            return tmp;
        }


        private void changeRowColorEventTo(int _index, Color _newColor)
        {
            for (int i = 0; i < dataGridViewEvents.Rows[_index].Cells.Count; i++)
                dataGridViewEvents.Rows[_index].Cells[i].Style.BackColor = _newColor;
        }


        private void changeColorEvents()
        {
            for (int i = 0; i < m_currentEventsInDataGrid.Count; ++i)
            {
                if (m_currentEventsInDataGrid[i].getPriority() == PriorityKind.Low)
                    changeRowColorEventTo(i, m_colorLow);
                else if (m_currentEventsInDataGrid[i].getPriority() == PriorityKind.Middle)
                    changeRowColorEventTo(i, m_colorMiddle);
                else if (m_currentEventsInDataGrid[i].getPriority() == PriorityKind.High)
                    changeRowColorEventTo(i, m_colorHigh);
            }
        }


        private void updateDataEvents()
        {

            Data.CurrentDate = monthCalendar.SelectionStart;

            List<Event> emptyTimeList = new List<Event>();
            DateTime timeForEmptyList = Data.CurrentDate.Date;

            for (int i = 0; i < 24; ++i)
            {
                emptyTimeList.Add(new Event(timeForEmptyList));
                timeForEmptyList = timeForEmptyList.AddHours(1);
            }

            this.Text = "Планировщик. Текущая дата: " + monthCalendar.SelectionStart.ToShortDateString();

            if (Data.Events.ContainsKey(monthCalendar.SelectionStart))
            {
                m_currentEventsInDataGrid = Data.Events[monthCalendar.SelectionStart];

                m_currentEventsInDataGrid = mergeEventList(m_currentEventsInDataGrid, emptyTimeList);

                if (Data.Events[monthCalendar.SelectionStart].Count != 0)
                    buttonDeleteEvent.Enabled = true;
                else
                    buttonDeleteEvent.Enabled = false;
            }
            else
            {
                m_currentEventsInDataGrid = emptyTimeList;
                buttonDeleteEvent.Enabled = false;
            }

            m_bindingEvents.DataSource = m_currentEventsInDataGrid;
            dataGridViewEvents.DataSource = m_bindingEvents;
            m_bindingEvents.ResetBindings(true);

            changeColorEvents();
        }


        private void updateDataContacts(List<Contact> _data)
        {
            this.Text = "Контакты";

            if (_data.Count == 0)
                buttonDeleteContact.Enabled = false;
            else
                buttonDeleteContact.Enabled = true;

            m_currentContactsInDataGrid = _data;

            m_bindingContacts.DataSource = _data;

            dataGridViewContacts.DataSource = m_bindingContacts;
            m_bindingContacts.ResetBindings(true);
        }


        private void updateBoldedDates(DateTime _currentDate)
        {
            if (Data.Events.ContainsKey(_currentDate))
            {
                if (Data.Events[_currentDate].Count == 0)
                {
                    buttonDeleteEvent.Enabled = false;

                    var result = (
                        from _date in monthCalendar.BoldedDates
                        where _date != _currentDate
                        select _date
                        ).ToArray<DateTime>();

                    monthCalendar.BoldedDates = result;
                }
                else
                {
                    monthCalendar.AddBoldedDate(_currentDate);
                    
                }

                monthCalendar.UpdateBoldedDates();
            }
        }


        private void searchContact(string _searchedStr)
        {
            List<Contact> result = new List<Contact>();

            if (_searchedStr.Length == 0)
                result = Data.Contacts;
            else
            {
                var searchedStrings = _searchedStr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var _contact in Data.Contacts)
                {
                    bool isFinded = true;

                    foreach (var _string in searchedStrings)
                        if (_contact.FirstName.Contains(_string) ||
                            _contact.SecondName.Contains(_string) ||
                            _contact.LastName.Contains(_string) ||
                            _contact.Telephone.Contains(_string) ||
                            _contact.Email.Contains(_string))
                            isFinded &= true;
                        else
                            isFinded &= false;

                    if (isFinded)
                        if (!result.Contains(_contact))
                            result.Add(_contact);
                }
            }

            updateDataContacts(result);
        }


        private void saveData(string _path)
        {
            if (Data.Events.Count == 0 && Data.Contacts.Count == 0)
                return;

            BinaryWriter binWriter = new BinaryWriter(File.Open(_path, FileMode.Create));

            binWriter.Write("ContactPlannerDataFileStart");
            binWriter.Write("EventsInfoStart");

            binWriter.Write(Convert.ToString(Data.Events.Count));  // Сохраняем кол-во элементов

            foreach (var _pair in Data.Events)
            {
                binWriter.Write("DATE");
                binWriter.Write(_pair.Key.Ticks);   // Сохраняем дату
                binWriter.Write(Convert.ToString(_pair.Value.Count)); // Сохраняем кол-во событий на эту дату

                foreach (var _event in _pair.Value)
                {
                    binWriter.Write("EventStart");

                    binWriter.Write("ID");
                    binWriter.Write(Convert.ToString(_event.getId()));    // Сохраняем ID события

                    binWriter.Write("DATE");
                    binWriter.Write(_event.getDate().Ticks); // Сохраняем точное время события в тиках

                    binWriter.Write("HEADER");
                    binWriter.Write(_event.Header);     // Сохраняем заголовок

                    binWriter.Write("DESCRIPTION");
                    binWriter.Write(_event.Description);// Сохраняем описание

                    binWriter.Write("PRIORITY");
                    binWriter.Write(Convert.ToString(EventWindow.priorityToIndex(_event.getPriority())));   // Сохраняем приоритет

                    binWriter.Write("EVENTCONTACTS");
                    saveContacts(_event.Contacts, binWriter);// Сохраняем контакты

                    binWriter.Write("EventEnd");
                }
            }

            binWriter.Write("EventsInfoEnd");

            saveContacts(Data.Contacts, binWriter);
            
            binWriter.Write("ContactPlannerDataFileEnd");

            binWriter.Close();
        }


        private void saveContacts(List<Contact> _data, BinaryWriter _writer)
        {
            _writer.Write("ContactsStart");
            _writer.Write(Convert.ToString(_data.Count));

            foreach (var _contact in _data)
            {
                _writer.Write("FIRSTNAME");
                _writer.Write(_contact.FirstName);

                _writer.Write("SECONDNAME");
                _writer.Write(_contact.SecondName);

                _writer.Write("LASTNAME");
                _writer.Write(_contact.LastName);

                _writer.Write("TELEPHONE");
                _writer.Write(_contact.Telephone);

                _writer.Write("EMAIL");
                _writer.Write(_contact.Email);
            }

            _writer.Write("ContactsEnd");
        }


        private void restoreData(string _path)
        {
            if (!File.Exists(_path))
                return;

            // FileNotFoundException не будет выброшен, т.к. проверили наличие файла
            BinaryReader binReader = new BinaryReader(File.Open(_path, FileMode.Open));

            try
            {
                if (binReader.ReadString() != "ContactPlannerDataFileStart")
                    throw new ReadDataException();
                if (binReader.ReadString() != "EventsInfoStart")
                    throw new ReadDataException();

                int countDates = Convert.ToInt32(binReader.ReadString());

                for (int i = 0; i < countDates; ++i)
                {
                    if(binReader.ReadString() != "DATE")
                        throw new ReadDataException();

                    long bufDate = binReader.ReadInt64();
                    int countEvents = Convert.ToInt32(binReader.ReadString());

                    DateTime date = new DateTime(bufDate);

                    Data.Events.Add(date, new List<Event>());

                    for (int j = 0; j < countEvents; ++j)
                    {
                       if (binReader.ReadString() != "EventStart")
                            throw new ReadDataException();

                        if (binReader.ReadString() != "ID")
                            throw new ReadDataException();
                        int bufId = Convert.ToInt32(binReader.ReadString());

                        if (binReader.ReadString() != "DATE")
                            throw new ReadDataException();
                        DateTime bufDateTime = new DateTime(binReader.ReadInt64());

                        if (binReader.ReadString() != "HEADER")
                            throw new ReadDataException();
                        String bufHeader = binReader.ReadString();

                        if (binReader.ReadString() != "DESCRIPTION")
                            throw new ReadDataException();
                        String bufDescription = binReader.ReadString();

                        if (binReader.ReadString() != "PRIORITY")
                            throw new ReadDataException();
                        int bufPriority = Convert.ToInt32(binReader.ReadString());

                        List<Contact> bufList = new List<Contact>();

                        if (binReader.ReadString() != "EVENTCONTACTS")
                            throw new ReadDataException();

                        restoreContact(bufList, binReader);

                        Data.Events[date].Add(new Event(
                                    bufDateTime
                                ,   bufHeader
                                ,   bufDescription
                                ,   bufList
                                ,   EventWindow.indexToPriority(bufPriority)
                                ,   bufId
                            )
                        );

                        if (binReader.ReadString() != "EventEnd")
                            throw new ReadDataException();
                    } // for ( countEvent )

                } // for ( countDates )

                if (binReader.ReadString() != "EventsInfoEnd")
                    throw new ReadDataException();

                restoreContact(Data.Contacts, binReader);

                if (binReader.ReadString() != "ContactPlannerDataFileEnd")
                    throw new ReadDataException();

                binReader.Close();
            }
            catch(ReadDataException)
            {
                MessageBox.Show("Файл данных поврежден! Чтение невозможно!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                binReader.Close(); // "Аварийное" закрытие потока 

                // Чистим то, что успело восстановиться до возникновения ошибки
                Data.Contacts.Clear();
                Data.Events.Clear();

                return;
            }
            catch(Exception _e)
            {
                MessageBox.Show(_e.StackTrace, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                binReader.Close(); // "Аварийное" закрытие потока 

                // Чистим то, что успело восстановиться до возникновения ошибки
                Data.Contacts.Clear();
                Data.Events.Clear();

                return;
            }
        }


        private void restoreContact(List<Contact> _data, BinaryReader _reader)
        {
            try
            {
                if (_reader.ReadString() != "ContactsStart")
                    throw new ReadDataException();

                int count = Convert.ToInt32(_reader.ReadString());

                for (int i = 0; i < count; ++i)
                {
                    
                    if (_reader.ReadString() != "FIRSTNAME")
                        throw new ReadDataException();
                    String bufName = _reader.ReadString();

                    if (_reader.ReadString() != "SECONDNAME")
                        throw new ReadDataException();
                    String bufSecName = _reader.ReadString();

                    if (_reader.ReadString() != "LASTNAME")
                        throw new ReadDataException();
                    String bufLastName = _reader.ReadString();

                    if (_reader.ReadString() != "TELEPHONE")
                        throw new ReadDataException();
                    String bufTel = _reader.ReadString();

                    if (_reader.ReadString() != "EMAIL")
                        throw new ReadDataException();
                    String bufEmail = _reader.ReadString();

                    _data.Add(new Contact(
                                bufName
                            ,   bufSecName
                            ,   bufLastName
                            ,   bufTel
                            ,   bufEmail
                        )
                    );
                }

                if (_reader.ReadString() != "ContactsEnd")
                    throw new ReadDataException();
            }
            catch (ReadDataException)
            {
                // Передаем управление вызвавшему методу
                throw;
            }
            catch(Exception)
            {
                // Передаем управление вызвавшему методу
                throw;
            }
        }


    }

    public class ReadDataException : Exception
    { }
}
