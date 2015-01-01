using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContactPlanner
{
	public partial class SelectContactWindow : Form
	{
		List<Contact> m_listUser;
		List<Contact> m_listAll;

		static public List<Contact> SelectedContacts { get; set; }

		public SelectContactWindow ( List<Contact> _userList )
		{
			InitializeComponent();

			SelectedContacts = new List<Contact>( _userList );
			m_listUser = new List<Contact>( _userList );
			m_listAll = new List<Contact>();

			for ( int i = 0; i < Data.Contacts.Count; ++i )
			{
				bool has = false;

				for ( int j = 0; j < m_listUser.Count; ++j )
				{
					if ( Data.Contacts[ i ].FirstName == m_listUser[ j ].FirstName &&
						Data.Contacts[ i ].SecondName == m_listUser[ j ].SecondName &&
						Data.Contacts[ i ].LastName == m_listUser[ j ].LastName &&
						Data.Contacts[ i ].Telephone == m_listUser[ j ].Telephone &&
						Data.Contacts[ i ].Email == m_listUser[ j ].Email )
					{
						has = true;
						break;
					}
				}

				if ( !has )
					m_listAll.Add( Data.Contacts[ i ] );
			}

			foreach ( var _contact in m_listAll )
				listViewAll.Items.Add(
					_contact.FirstName + ' ' + _contact.LastName
					);

			foreach ( var _contact in m_listUser )
				listViewSelected.Items.Add(
					_contact.FirstName + ' ' + _contact.LastName
					);
		}


		private void buttonCancel_Click ( object sender, EventArgs e )
		{
			Close();
		}


		private void buttonToAll_Click ( object sender, EventArgs e )
		{
			if ( listViewSelected.SelectedIndices.Count == 0 )
				return;

			foreach ( var _index in listViewSelected.SelectedIndices )
			{
				m_listAll.Add( m_listUser[ Convert.ToInt32( _index ) ] );
				m_listUser.RemoveAt( Convert.ToInt32( _index ) );
			}

			foreach ( var _item in listViewSelected.SelectedItems )
			{
				ListViewItem selectedItem = ( ListViewItem )_item;
				listViewAll.Items.Add( selectedItem.Text );
				listViewSelected.Items.Remove( ( ListViewItem )_item );
			}
		}

		private void buttonDone_Click ( object sender, EventArgs e )
		{
			SelectedContacts = m_listUser;
			Close();
		}

		private void buttonToSelected_Click ( object sender, EventArgs e )
		{
			if ( listViewAll.SelectedIndices.Count == 0 )
				return;

			foreach ( var _index in listViewAll.SelectedIndices )
			{
				m_listUser.Add( m_listAll[ Convert.ToInt32( _index ) ] );
				m_listAll.RemoveAt( Convert.ToInt32( _index ) );
			}

			foreach ( var _item in listViewAll.SelectedItems )
			{
				ListViewItem selectedItem = ( ListViewItem )_item;
				listViewSelected.Items.Add( selectedItem.Text );
				listViewAll.Items.Remove( ( ListViewItem )_item );
			}
		}
	}
}
