using System;
using System.Windows.Forms;

namespace ContactPlanner
{
    public partial class ContactWindow : Form
    {
        Contact m_currentContact;

        public ContactWindow(Contact _newContact)
        {
            InitializeComponent();

            m_currentContact = _newContact;

            textBoxName.Text = m_currentContact.FirstName;
            textBoxSecondName.Text = m_currentContact.SecondName;
            textBoxLastName.Text = m_currentContact.LastName;
            maskedTextBoxTelephone.Text = m_currentContact.Telephone;
            textBoxEmail.Text = m_currentContact.Email;

            //if (textBoxName.Text.Length != 0)
            //    textBoxName.ReadOnly = true;

            //if (textBoxSecondName.Text.Length != 0)
            //    textBoxSecondName.ReadOnly = true;

            //if (textBoxLastName.Text.Length != 0)
            //    textBoxLastName.ReadOnly = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if(textBoxName.Text.Length != 0)
            {
                if(textBoxEmail.Text.Length == 0 &&
                    maskedTextBoxTelephone.Text == "(   )    -")
                {
                    MessageBox.Show("Поля \"Телефон\" либо \"E-Mail\" обязательны к заполнению!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Поле \"Имя\" обязательно к заполнению!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var _contact in Data.Contacts)
                if ((textBoxName.Text == _contact.FirstName &&
                   textBoxSecondName.Text == _contact.SecondName &&
                   textBoxLastName.Text == _contact.LastName) &&
                   (maskedTextBoxTelephone.Text == _contact.Telephone &&
                   textBoxEmail.Text == _contact.Email))
                {
                    MessageBox.Show("Контакты должы быть уникальными!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            if (Data.Contacts.Contains(m_currentContact))
            {
                int indexEdited = Data.Contacts.IndexOf(m_currentContact);

                Data.Contacts[indexEdited].FirstName = textBoxName.Text;
                Data.Contacts[indexEdited].SecondName = textBoxSecondName.Text;
                Data.Contacts[indexEdited].LastName = textBoxLastName.Text;
                Data.Contacts[indexEdited].Telephone = maskedTextBoxTelephone.Text;
                Data.Contacts[indexEdited].Email = textBoxEmail.Text;
            }
            else
            {
                Data.Contacts.Add(new Contact(
                            textBoxName.Text
                        , textBoxSecondName.Text
                        , textBoxLastName.Text
                        , maskedTextBoxTelephone.Text
                        , textBoxEmail.Text
                    )
                );
            }
            Close();
        }

        private void textKeyPressProc(object sender, KeyPressEventArgs e)
        {
            const int KEY_ENTER = 13;
            const int KEY_BACKSPACE = 8;
            const int KEY_SPACE = 32;

            Char key = e.KeyChar;

            if (!Char.IsLetter(key) &&
                key != KEY_SPACE &&
                key != KEY_BACKSPACE &&
                key != KEY_ENTER)
                e.Handled = true;

            if (key == KEY_ENTER)
                SelectNextControl(this, true, true, false, true);
        }
    }
}
