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

            if(!isValidEmail(textBoxEmail.Text))
            {
                MessageBox.Show("Вы ввели некорректный E-Mail!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /**
        *    Должно начинаться с буквы/цифры
        *    Не дожно содержать символов кроме [0-9 a-z A-Z - _ .]
        *    Не должно содержать двух точек подряд
        *    Должно заканчиваться на букву/цифру
        **/

        private bool isValidEmail(string _mail)
        {
            if (!Char.IsLetterOrDigit(_mail, 0) || !Char.IsLetterOrDigit(_mail, _mail.Length - 1))
                return false;

            string allowedChar = "";

            for (int i = 0; i < 10; ++i )
                allowedChar += Convert.ToString(i);

            for (char i = 'a'; i < 'z' + 1; ++i)
                allowedChar += i;

            for (char i = 'A'; i < 'Z' + 1; ++i)
                allowedChar += i;

            allowedChar += "@_-.";

            // Выход за границу невозможен
            for (int i = 0; i < _mail.Length; ++i)
            {
                if (_mail[i] == '.')
                    if (_mail[i + 1] == '.')
                        return false;

                if (!allowedChar.Contains(Convert.ToString(_mail[i])))
                    return false;
            }

            return true;
        }
    }
}
