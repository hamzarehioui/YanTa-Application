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
    public partial class YanTaMainWindow : Form
    {
        private bool selectionIsFile;
        private User currentUser;
        private readonly YanTaNotesDBEntities yanTaNotesDBEntities;
        private bool isStartup;
        private static int counter;
        private List<int> noteIDs;

        public YanTaMainWindow(User currentUser)
        {
            InitializeComponent();
            yanTaNotesDBEntities = new YanTaNotesDBEntities();
            this.currentUser = currentUser;
            isStartup = true;
            counter = yanTaNotesDBEntities.Notes.Where(x => x.User.id == currentUser.id).Count() + 1;
            RefreshTreeView();
            SetRegularMode();
            allNotesTreeView.HideSelection = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (noteEditorsTabControl.SelectedTab != null)
            {
                noteEditorsTabControl.TabPages.Remove(noteEditorsTabControl.SelectedTab);
            }
            else
            {
                MessageBox.Show("No more tabs to close!");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = "";
            if (allNotesTreeView.SelectedNode != null)
                selected = allNotesTreeView.SelectedNode.Text;

            if (allNotesTreeView.SelectedNode == null)
            {
                MessageBox.Show("No note is selected! Select a note to edit its metadata.");
                return;
            }

            if (tbAuthor.Enabled)
            {
                SaveNewMetadata();
                SetRegularMode();
                RefreshTreeView();
            }
            else
            {
                SetEditMode();
                
            }

            try
            {
                allNotesTreeView.SelectedNode = allNotesTreeView.Nodes[allNotesTreeView.Nodes.IndexOfKey(selected)];
            }
            catch { }

        }

        private void SetRegularMode()
        {
            tbAuthor.Enabled = false;
            tbComments.Enabled = false;
            tbCreated.Enabled = false;
            tbLastEdited.Enabled = false;
            tbTitle.Enabled = false;
            btnEdit.Text = "Edit";
        }

        private void SetEditMode()
        {
            if (selectionIsFile)
            {
                tbAuthor.Enabled = true;
                tbComments.Enabled = true;
                tbCreated.Enabled = false;
                tbLastEdited.Enabled = false;
                tbTitle.Enabled = true;
                btnEdit.Text = "Save";
            }
        }

        private void SaveNewContent()
        {
            if (noteEditorsTabControl.SelectedTab == null)
            {
                this.Dispose();
                return;
            }

            if (allNotesTreeView.Nodes.IndexOfKey(noteEditorsTabControl.SelectedTab.Text) != -1)
            {
                int selectedIndex = noteIDs[(int)allNotesTreeView.Nodes.IndexOfKey(noteEditorsTabControl.SelectedTab.Text)];
                var oldNote = yanTaNotesDBEntities.Notes.FirstOrDefault(q => q.id == selectedIndex);
                oldNote.Content = noteEditorsTabControl.SelectedTab.Controls[0].Text;
                oldNote.LastEdited = DateTime.Now;
            }
            else
            {
                var newNote = new Note
                {
                    Title = noteEditorsTabControl.SelectedTab.Text,
                    Author = tbAuthor.Text,
                    Content = noteEditorsTabControl.SelectedTab.Controls[0].Text,
                    Created = DateTime.Now,
                    LastEdited = DateTime.Now,
                    userID = currentUser.id,
                    User = yanTaNotesDBEntities.Users.FirstOrDefault(q => q.id == currentUser.id),
                    Comments = tbComments.Text
                };

                yanTaNotesDBEntities.Notes.Add(newNote);
            }

            yanTaNotesDBEntities.SaveChanges();
            RefreshTreeView();

        }

        private void SaveNewMetadata()
        {
            if (allNotesTreeView.Nodes.IndexOfKey(allNotesTreeView.SelectedNode.Text) != -1)
            {
                int selectedIndex = noteIDs[(int)allNotesTreeView.Nodes.IndexOfKey(allNotesTreeView.SelectedNode.Text)];
                
                if(yanTaNotesDBEntities.Notes.Where(x => x.User.id == currentUser.id && x.Title == tbTitle.Text).Count() == 0)
                {
                    var oldNote = yanTaNotesDBEntities.Notes.FirstOrDefault(q => q.id == selectedIndex);
                    oldNote.Title = tbTitle.Text;
                    oldNote.Author = tbAuthor.Text;
                    oldNote.LastEdited = DateTime.Now;
                    oldNote.Comments = tbComments.Text;
                }
                else
                {
                    MessageBox.Show("You did not change the title.");
                    return;
                }

                try
                {
                    noteEditorsTabControl.TabPages[noteEditorsTabControl.TabPages.IndexOfKey(selectedIndex.ToString())].Text = tbTitle.Text;
                }
                catch
                {

                }
            }

            yanTaNotesDBEntities.SaveChanges();
            RefreshTreeView();

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (allNotesTreeView.SelectedNode == null)
            {
                selectionIsFile = false;
            }
            else
            {
                selectionIsFile = true;
                int selectedIndex = noteIDs[(int)allNotesTreeView.Nodes.IndexOfKey(allNotesTreeView.SelectedNode.Text)];
                var oldNote = yanTaNotesDBEntities.Notes.FirstOrDefault(q => q.id == selectedIndex);
                tbTitle.Text = oldNote.Title;
                tbAuthor.Text = oldNote.Author;
                tbComments.Text = oldNote.Comments;
                tbLastEdited.Text = oldNote.LastEdited.ToString();
                tbCreated.Text = oldNote.Created.ToString();
                SetRegularMode();
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!selectionIsFile || allNotesTreeView.SelectedNode == null)
            {
                MessageBox.Show("Please select a note file to delete!");
            }
            else
            {

                DialogResult dialog = MessageBox.Show($"Are you sure you want to delete {allNotesTreeView.SelectedNode.Text}?", "Delete",
                   MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (dialog == DialogResult.Yes)
                {
                    int selectedIndex = noteIDs[(int)allNotesTreeView.SelectedNode.Index];

                    try { noteEditorsTabControl.TabPages.Remove(noteEditorsTabControl.TabPages[noteEditorsTabControl.TabPages.IndexOfKey(selectedIndex.ToString())]); } catch { }
                    yanTaNotesDBEntities.Notes.Remove(yanTaNotesDBEntities.Notes.Where(i => i.id == selectedIndex).ToList()[0]);
                    yanTaNotesDBEntities.SaveChanges();
                }

                RefreshTreeView();
            }

        }

        private void RefreshTreeView()
        {
            try
            {
                allNotesTreeView.Nodes.Clear();

                var notes = yanTaNotesDBEntities.Notes.Select(q => new
                {
                    Author = q.Author,
                    Created = q.Created,
                    LastEdited = q.LastEdited,
                    id = q.id,
                    Comments = q.Comments,
                    Content = q.Content,
                    Title = q.Title,
                    userID = q.User.id
                }).Where(q => q.userID == currentUser.id).ToList();

                if (notes.Count == 0 && isStartup)
                {
                    isStartup = false;
                    MessageBox.Show("You have no notes saved.");
                    return;
                }

                noteIDs = new List<int>();
                foreach (var note in notes)
                {
                    allNotesTreeView.Nodes.Add(note.Title, note.Title);
                    noteIDs.Add(note.id);
                }

            }
            catch
            {
                MessageBox.Show("An error occurred in the import.");
            }


        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var tabPage = new TabPage("New Note " + counter.ToString());
            var richTextBox = new System.Windows.Forms.RichTextBox();
            tabPage.Controls.Add(richTextBox);

            richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBox.Location = new System.Drawing.Point(3, 3);
            richTextBox.Name = "richTextBox";
            richTextBox.Size = new System.Drawing.Size(832, 530);
            richTextBox.TabIndex = 0;
            richTextBox.Text = "";

            tabPage.Controls.Add(richTextBox);
            tabPage.Location = new System.Drawing.Point(8, 39);
            tabPage.Name = counter.ToString();
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(838, 536);
            tabPage.TabIndex = 0;
            tabPage.Text = "New Note " + counter.ToString();
            tabPage.UseVisualStyleBackColor = true;
            noteEditorsTabControl.TabPages.Add(tabPage);

            counter++;

        }

        private void saveAndCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveNewContent();
            if (noteEditorsTabControl.SelectedTab != null)
            {
                noteEditorsTabControl.TabPages.Remove(noteEditorsTabControl.SelectedTab);
            }
            else
            {
                MessageBox.Show("No more tabs to close!");
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectionIsFile)
            {
                int selectedIndex = noteIDs[(int)allNotesTreeView.SelectedNode.Index];

                var note = yanTaNotesDBEntities.Notes.Select(q => new
                {
                    Author = q.Author,
                    Created = q.Created,
                    LastEdited = q.LastEdited,
                    id = q.id,
                    Comments = q.Comments,
                    Content = q.Content,
                    Title = q.Title,
                    userID = q.User.id
                }).Where(q => q.userID == currentUser.id && q.id == selectedIndex).ToList()[0];

                if (noteEditorsTabControl.TabPages.IndexOfKey(note.id.ToString()) != -1)
                {
                    noteEditorsTabControl.SelectedTab = noteEditorsTabControl.TabPages[noteEditorsTabControl.TabPages.IndexOfKey(note.id.ToString())];
                }
                else
                {
                    noteEditorsTabControl.TabPages.Add((counter - 1).ToString(), "newtab");
                    var tabPage = noteEditorsTabControl.TabPages[noteEditorsTabControl.TabPages.Count - 1];

                    var richTextBox = new System.Windows.Forms.RichTextBox();
                    tabPage.Controls.Add(richTextBox);

                    richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
                    richTextBox.Location = new System.Drawing.Point(3, 3);
                    richTextBox.Name = note.Content;
                    richTextBox.Size = new System.Drawing.Size(832, 530);
                    richTextBox.TabIndex = 0;
                    richTextBox.Text = note.Content;

                    tabPage.Controls.Add(richTextBox);
                    tabPage.Location = new System.Drawing.Point(8, 39);
                    tabPage.Name = note.id.ToString();
                    tabPage.Padding = new System.Windows.Forms.Padding(3);
                    tabPage.Size = new System.Drawing.Size(838, 536);
                    tabPage.TabIndex = 0;
                    tabPage.Text = note.Title;
                    tabPage.UseVisualStyleBackColor = true;
                    noteEditorsTabControl.SelectedTab = tabPage;

                }
            }
        }


        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            SetRegularMode();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (noteEditorsTabControl.SelectedTab != null)
                {
                    Clipboard.SetText(((RichTextBox)noteEditorsTabControl.SelectedTab.Controls[0]).SelectedText);
                }
            }
            catch (Exception)
            { }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (noteEditorsTabControl.SelectedTab != null)
                {
                    ((RichTextBox)noteEditorsTabControl.SelectedTab.Controls[0]).Paste();
                }
            }
            catch (Exception)
            { }
        }

        private void closeTabWIthoutSavingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noteEditorsTabControl.SelectedTab != null)
            {
                noteEditorsTabControl.TabPages.Remove(noteEditorsTabControl.SelectedTab);
            }
            else
            {
                MessageBox.Show("No more tabs to close!");
            }
        }
        public static void logoutToLoginForm()
        {
            Application.Run(new Login());
        }
        private void logoutFromSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(logoutToLoginForm));
            t.Start();
            this.Dispose();
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is in development. It shows intention :)");
        }

        private void sharedNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is in development. It shows intention :)");
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is in development. It shows intention :)");
        }

        private void resetPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ResetPassword(currentUser).ShowDialog(this);
        }
    }
}
