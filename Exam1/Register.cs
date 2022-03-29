using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exam1
{
    public partial class Register : Form
    {
        private readonly YanTaNotesDBEntities yanTaNotesDBEntities;
        public Register()
        {
            InitializeComponent();
            yanTaNotesDBEntities = new YanTaNotesDBEntities();
            tbPassword.PasswordChar = '•';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbPassword.Text) || string.IsNullOrWhiteSpace(tbUsername.Text))
            {
                MessageBox.Show("Please ensure that you provide a username and a password!");
            }
            else
            {
                var username = tbUsername.Text;
                var password = Utils.HashPassword(tbPassword.Text);

                if (yanTaNotesDBEntities.Users.Any(o => o.username == username))
                {
                    MessageBox.Show("Please pick another username!");
                    return;
                }
                var user = new User
                {
                    username = username,
                    password = password,
                };
                yanTaNotesDBEntities.Users.Add(user);
                yanTaNotesDBEntities.SaveChanges();

                MessageBox.Show("New account created successfully!");
                this.Dispose();
            }
        }
    }
}
