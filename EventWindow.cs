﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ContactPlanner
{
	public partial class EventWindow : Form
	{
		const int MIN_SYMBOLS = 20;

		Event m_currentEvent;

		public EventWindow ( Event _newEvent )
		{
			InitializeComponent();

			m_currentEvent = _newEvent;

			updateContactList();

			dateTimePicker.Value = m_currentEvent.getDate();
			comboBoxPriority.SelectedIndex =
				priorityToIndex( m_currentEvent.getPriority() );
			textBoxHeader.Text = m_currentEvent.Header;
			textBoxDescription.Text = m_currentEvent.Description;
		}


		private void updateContactList ()
		{
			listContacts.Items.Clear();

			foreach ( var _contact in m_currentEvent.Contacts )
				listContacts.Items.Add(
					_contact.FirstName + " " + _contact.LastName
					);

			listContacts.Items.Add( "<Кликните на список, чтобы добавить>" );
		}

		private void buttonCancel_Click ( object sender, EventArgs e )
		{
			Close();
		}


		static public PriorityKind indexToPriority ( int _index )
		{
			switch ( _index )
			{
				case 0:
					return PriorityKind.Low;
				case 1:
					return PriorityKind.Middle;
				case 2:
					return PriorityKind.High;
			}

			throw new Exception();
		}


		static public int priorityToIndex ( PriorityKind _priority )
		{
			switch ( _priority )
			{
				case PriorityKind.Unspecified:
				case PriorityKind.Low:
					return 0;
				case PriorityKind.Middle:
					return 1;
				case PriorityKind.High:
					return 2;
			}

			throw new Exception();
		}

		private void buttonSave_Click ( object sender, EventArgs e )
		{
			if ( textBoxDescription.Text.Length == 0 )
			{
				MessageBox.Show(
						"Поле \"Описание\" осталось пустым!"
					,	"Ошибка!"
					,	MessageBoxButtons.OK
					,	MessageBoxIcon.Error
					);
				return;
			}

			if ( dateTimePicker.Value.Date.CompareTo( System.DateTime.Now.Date ) < 0 )
			{
				MessageBox.Show(
						"Выбрана дата из прошлого!"
					,	"Ошибка!"
					,	MessageBoxButtons.OK
					,	MessageBoxIcon.Error
					);
				return;
			}

			if ( textBoxHeader.Text.Length == 0 )
				textBoxHeader.Text = "<Нет темы>";

			if ( !Data.Events.ContainsKey( dateTimePicker.Value.Date ) )
				Data.Events.Add( dateTimePicker.Value.Date, new List<Event>() );

			// Если произошли изменения в дате 
			if ( m_currentEvent.getDate() != dateTimePicker.Value )
			{
				foreach ( var _event in Data.Events[ dateTimePicker.Value.Date ] )
				{
					if ( _event.getDate() == dateTimePicker.Value )
					{
						MessageBox.Show(
								"Событие с такой же датой уже существует!"
							,	"Ошибка!"
							,	MessageBoxButtons.OK
							,	MessageBoxIcon.Error
							);
						return;
					}
				}
			}

			if ( Data.Events[ dateTimePicker.Value.Date ].Contains( m_currentEvent ) )
			{
				int indexEdited =
					Data.Events[ dateTimePicker.Value.Date ].IndexOf( m_currentEvent );

				Data.Events[ dateTimePicker.Value.Date ][ indexEdited ]
					.setDate( dateTimePicker.Value );
				Data.Events[ dateTimePicker.Value.Date ][ indexEdited ]
					.Header = textBoxHeader.Text;
				Data.Events[ dateTimePicker.Value.Date ][ indexEdited ]
					.Description = textBoxDescription.Text;
				Data.Events[ dateTimePicker.Value.Date ][ indexEdited ]
					.setPriority( indexToPriority( comboBoxPriority.SelectedIndex ) );
			}
			else
			{
				m_currentEvent.setDate( dateTimePicker.Value );
				m_currentEvent.Header = textBoxHeader.Text;
				m_currentEvent.Description = textBoxDescription.Text;
				m_currentEvent.setPriority(
					indexToPriority( comboBoxPriority.SelectedIndex )
					);

				Data.Events[ dateTimePicker.Value.Date ].Add( m_currentEvent );
			}

			Data.LastDate = dateTimePicker.Value.Date;

			Close();
		}


		private void listContacts_MouseDown ( object sender, MouseEventArgs e )
		{
			Form addContacts = new SelectContactWindow( m_currentEvent.Contacts );
			addContacts.ShowDialog( this );
			m_currentEvent.Contacts = SelectContactWindow.SelectedContacts;
			updateContactList();
		}
	}
}
