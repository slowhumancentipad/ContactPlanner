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

        public SelectContactWindow(List<Contact> _userList)
        {
            InitializeComponent();

            SelectedContacts = new List<Contact>(_userList);

            m_listUser = new List<Contact>(_userList);

            var result = (
                from _contact in Data.Contacts
                where !_userList.Contains(_contact)
                select _contact
                ).ToList<Contact>();

            m_listAll = result;

            foreach (var _contact in m_listAll)
                listViewAll.Items.Add(_contact.FirstName + ' ' + _contact.LastName);

            foreach (var _contact in m_listUser)
                listViewSelected.Items.Add(_contact.FirstName + ' ' + _contact.LastName);
        }


        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void buttonToAll_Click(object sender, EventArgs e)
        {
            if (listViewSelected.SelectedIndices.Count == 0)
                return;

            foreach (var _index in listViewSelected.SelectedIndices)
            {
                m_listAll.Add(m_listUser[Convert.ToInt32(_index)]);
                m_listUser.RemoveAt(Convert.ToInt32(_index));
            }

            foreach (var _item in listViewSelected.SelectedItems)
            {
                ListViewItem selectedItem = (ListViewItem)_item;
                listViewAll.Items.Add(selectedItem.Text);
                listViewSelected.Items.Remove((ListViewItem)_item);
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            SelectedContacts = m_listUser;
            Close();
        }

        private void buttonToSelected_Click(object sender, EventArgs e)
        {
            if (listViewAll.SelectedIndices.Count == 0)
                return;

            foreach (var _index in listViewAll.SelectedIndices)
            {
                m_listUser.Add(m_listAll[Convert.ToInt32(_index)]);
                m_listAll.RemoveAt(Convert.ToInt32(_index));
            }

            foreach (var _item in listViewAll.SelectedItems)
            {
                ListViewItem selectedItem = (ListViewItem)_item;
                listViewSelected.Items.Add(selectedItem.Text);
                listViewAll.Items.Remove((ListViewItem)_item);
            }            
        }
    }
}
