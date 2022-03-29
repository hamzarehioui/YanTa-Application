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
    public partial class Login : Form
    {
        private readonly YanTaNotesDBEntities yanTaNotesDBEntities;
        private User currentUser;
        public Login()
        {
            InitializeComponent();
            yanTaNotesDBEntities = new YanTaNotesDBEntities();
            tbPassword.PasswordChar = '•';
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            var hashed = Utils.HashPassword(tbPassword.Text);
            var trialAcc = yanTaNotesDBEntities.Users.Select(q => new
            {
                username = q.username,
                password = q.password,
                id = q.id
            }).Where(q => q.username == tbUsername.Text && q.password == hashed).Count();

            if (trialAcc == 0)
            {
                MessageBox.Show("Please login with a valid account, or register!");
                return;
            }
            else
            {
                currentUser = yanTaNotesDBEntities.Users.Where(q => q.username == tbUsername.Text).ToList()[0];
            }
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(loginToMainWindow));
            t.Start();
            this.Dispose();
        }

        private void loginToMainWindow()
        {
            Application.Run(new YanTaMainWindow(currentUser));
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            new Register().ShowDialog(this);
        }
    }
}
