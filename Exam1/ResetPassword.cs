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
    public partial class ResetPassword : Form
    {
        private User currentUser;
        private readonly YanTaNotesDBEntities yanTaNotesDBEntities;
        public ResetPassword(User currentUser)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            yanTaNotesDBEntities = new YanTaNotesDBEntities();
            tbConfirmPassword.PasswordChar = '•';
            tbNewPassword.PasswordChar = '•';
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (!(tbConfirmPassword.Text == tbNewPassword.Text && !string.IsNullOrEmpty(tbNewPassword.Text)))
            {
                MessageBox.Show("Please enter two equal valid passwords!");
                return;
            }
            else
            {
                yanTaNotesDBEntities.Users.FirstOrDefault(q => q.id == currentUser.id).password = Utils.HashPassword(tbNewPassword.Text);
                yanTaNotesDBEntities.SaveChanges();
            }
            this.Dispose();
        }
    }
}
