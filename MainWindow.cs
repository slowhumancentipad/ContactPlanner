using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ContactPlanner
{
    public partial class MainWindow : Form
    {
        BindingSource m_bindingEvents = new BindingSource();
        BindingSource m_bindingContacts = new BindingSource();

        bool isShowAll = false;


        public MainWindow()
        {
            Data.Contacts = new List<Contact>();
            Data.Events = new Dictionary<DateTime, List<Event>>();
            Data.LastId = 0;

            InitializeComponent();

            tabControl.DrawItem += new DrawItemEventHandler(tabControl_DrawItem);

            #region AddDataGridEventsColumn

            dataGridViewEvents.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Date",
                    HeaderText = "Дата и время",
                    Width = 100
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

            dataGridViewEvents.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Priority",
                    HeaderText = "Приоритет",
                    Width = 70
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

            updateDataContacts(Data.Contacts);
            updateDataEvents();
            updateBoldedDates();
        }


        private void saveContacts(List<Contact> _data, String _path, int _count)
        {
            StreamWriter fileContacts = new StreamWriter(_path);

            saveContacts(_data, fileContacts, _count);

            fileContacts.Close();
        }


        private void saveContacts(List<Contact> _data, StreamWriter _fileContacts, int _count)
        {

            _fileContacts.WriteLine(_count);

            if (_data.Count != 0)
            {
                for (int i = 0; i < _count; ++i)
                {
                    _fileContacts.WriteLine(_data[i].FirstName);
                    _fileContacts.WriteLine(_data[i].SecondName);
                    _fileContacts.WriteLine(_data[i].LastName);
                    _fileContacts.WriteLine(_data[i].Telephone);
                    _fileContacts.WriteLine(_data[i].Email);
                }
            }
        }


        private void saveEvents(Dictionary<DateTime, List<Event>> _data, string _path)
        {
            if (_data.Count != 0)
            {
                StreamWriter fileEvents = new StreamWriter(_path);

                fileEvents.WriteLine(_data.Count);  // Сохраняем кол-во элементов

                foreach (var _pair in _data)
                {
                    fileEvents.WriteLine(_pair.Key.Ticks);   // Сохраняем дату
                    fileEvents.WriteLine(_pair.Value.Count); // Сохраняем кол-во событий на эту дату

                    foreach(var _event in _pair.Value)
                    {
                        fileEvents.WriteLine(_event.getId());    // Сохраняем ID события
                        fileEvents.WriteLine(_event.Date.Ticks); // Сохраняем точное время события в тиках
                        fileEvents.WriteLine(_event.Header);     // Сохраняем заголовок
                        fileEvents.WriteLine(_event.Description);// Сохраняем описание
                        fileEvents.WriteLine(EventWindow.priorityToIndex(_event.Priority));   // Сохраняем приоритет

                        saveContacts(_event.Contacts, fileEvents, _event.Contacts.Count);// Сохраняем контакты
                    }
                }

                fileEvents.Close();
            }
        }


        private void restoreContacts(List<Contact> _data, string _path)
        {
            if (!File.Exists(_path))    // Если такого файла не существует, то и нечего восстанавливать
                return;

            System.IO.StreamReader fileContacts = new System.IO.StreamReader(_path);

            restoreContacts(_data, fileContacts);

            fileContacts.Close();
        }


        private void restoreContacts(List<Contact> _data, StreamReader _fileContacts)
        {
            int count = Convert.ToInt32(_fileContacts.ReadLine());   // Считываем кол-во контактов

            for (int i = 0; i < count; ++i)
            {
                String  bufName
                    ,   bufSecName
                    ,   bufLastName
                    ,   bufTel
                    ,   bufEmail;

                bufName = _fileContacts.ReadLine();
                bufSecName = _fileContacts.ReadLine();
                bufLastName = _fileContacts.ReadLine();
                bufTel = _fileContacts.ReadLine();
                bufEmail = _fileContacts.ReadLine();

                _data.Add(new Contact(
                            bufName
                        ,   bufSecName
                        ,   bufLastName
                        ,   bufTel
                        ,   bufEmail
                    )
                );
            }
        }


        private void restoreEvents(Dictionary<DateTime, List<Event>> _data, string _path)
        {
            if (!File.Exists(_path))    // Если такого файла не существует, то и нечего восстанавливать
                return;

            System.IO.StreamReader fileEvents = new System.IO.StreamReader(_path);

            int countDates = Convert.ToInt32(fileEvents.ReadLine());

            for (int i = 0; i < countDates; ++i)
            {
                long bufDate = Convert.ToInt64(fileEvents.ReadLine());
                int countEvents = Convert.ToInt32(fileEvents.ReadLine());

                DateTime date = new DateTime(bufDate);

                _data.Add(date, new List<Event>());

                for (int j = 0; j < countEvents; ++j)
                {
                    int bufId = Convert.ToInt32(fileEvents.ReadLine());
                    DateTime bufDateTime = new DateTime(Convert.ToInt64(fileEvents.ReadLine()));
                    String bufHeader = fileEvents.ReadLine();
                    String bufDescription = fileEvents.ReadLine();
                    int bufPriority = Convert.ToInt32(fileEvents.ReadLine());

                    List<Contact> bufList = new List<Contact>();

                    restoreContacts(bufList, fileEvents);

                    _data[date].Add(new Event(
                                bufDateTime
                            ,   bufHeader
                            ,   bufDescription
                            ,   bufList
                            ,   EventWindow.indexToPriority(bufPriority)
                            ,   bufId
                        )
                    );
                }


            }

            fileEvents.Close();
        }


        private void updateDataEvents()
        {
            Data.CurrentDate = monthCalendar.SelectionStart;

            this.Text = "Планировщик. Текущая дата: " + monthCalendar.SelectionStart.ToShortDateString();

            if (Data.Events.ContainsKey(monthCalendar.SelectionStart))
            {
                m_bindingEvents.DataSource = Data.Events[monthCalendar.SelectionStart];

                if (Data.Events[monthCalendar.SelectionStart].Count != 0)
                    buttonDeleteEvent.Enabled = true;
                else
                    buttonDeleteEvent.Enabled = false;
            }
            else
            {
                m_bindingEvents.DataSource = new List<Event>();
                buttonDeleteEvent.Enabled = false;
            }

            dataGridViewEvents.DataSource = m_bindingEvents;
            m_bindingEvents.ResetBindings(true);
        }


        private void updateDataContacts(List<Contact> _data)
        {
            this.Text = "Контакты";

            if (_data.Count == 0)
                buttonDeleteContact.Enabled = false;
            else
                buttonDeleteContact.Enabled = true;

            m_bindingContacts.DataSource = _data;

            dataGridViewContacts.DataSource = m_bindingContacts;
            m_bindingContacts.ResetBindings(true);
        }


        private void updateBoldedDates()
        {
            if (Data.Events.ContainsKey(Data.LastDate))
            {
                if (Data.Events[Data.LastDate].Count == 0)
                {
                    buttonDeleteEvent.Enabled = false;

                    var result = (
                        from _date in monthCalendar.BoldedDates
                        where _date != monthCalendar.SelectionStart
                        select _date
                        ).ToArray<DateTime>();

                    monthCalendar.BoldedDates = result;
                }
                else
                {
                    monthCalendar.AddBoldedDate(Data.LastDate);
                    monthCalendar.UpdateBoldedDates();
                }
            }
        }


        private void searchContact(string _searchedStr)
        {
            if (_searchedStr.Length == 0)
                return;

            var searchedStrings = _searchedStr.Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);

            List<Contact> result = new List<Contact>();

            foreach(var _contact in Data.Contacts)
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

                if(isFinded)
                    if(!result.Contains(_contact))
                        result.Add(_contact);
            }

            updateDataContacts(result);
        }
    }

}
