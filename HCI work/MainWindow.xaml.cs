using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Xml;
using System.Windows.Data;

namespace HCI_work
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Email SelectedEmail { get; set; }

        public ICommand RemoveCommand { get; }

        public Folder SelectedFolder { get; set; }

        public ObservableCollection<Folder> Folders { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Folders = new ObservableCollection<Folder>();

            var inboxFolder = new Folder{
                Name = "Inbox",
                Emails = new ObservableCollection<Email>()
            };
            inboxFolder.Emails.Add(new Email { Subject = "Inbox Email 1", Sender = "john@example.com", Content = "This is a inbox email.", Copies = "johndoe@mail.com", Recepient = "haifhasif", Attachments = new List<string> { "attachment1.pdf", "attachment2.docx" } });
            Folders.Add(inboxFolder);

            // Create dummy emails for Sent folder
            var sentFolder = new Folder
            {
                Name = "Sent",
                Emails = new ObservableCollection<Email>()
            };
            sentFolder.Emails.Add(new Email { Subject = "Sent Email 1", Sender = "john@example.com", Content = "This is a sent email.", Copies = "johndoe@mail.com", Recepient = "haifhasif", Attachments = new List<string> { "attachment1.pdf", "attachment2.docx" } });
            sentFolder.Emails.Add(new Email { Subject = "Sent Email 2", Sender = "jane@example.com", Content = "Another sent email.", Copies = "johndoe@mail.com", Recepient = "haifhasif", Attachments = new List<string> {} });
            Folders.Add(sentFolder);

            // Create dummy emails for Drafts folder
            var draftsFolder = new Folder
            {
                Name = "Drafts",
                Emails = new ObservableCollection<Email>()
            };
            draftsFolder.Emails.Add(new Email { Subject = "Draft Email 1", Sender = "john@example.com", Content = "This is a draft email.", Copies = "johndoe@mail.com", Recepient = "haifhasif", Attachments = new List<string> { } });
            draftsFolder.Emails.Add(new Email { Subject = "Draft Email 2", Sender = "jane@example.com", Content = "Another draft email.", Copies = "johndoe@mail.com", Recepient = "haifhasif", Attachments = new List<string> { } });
            Folders.Add(draftsFolder);

            // Create dummy emails for Trash folder
            var trashFolder = new Folder
            {
                Name = "Trash",
                Emails = new ObservableCollection<Email>()
            };
            trashFolder.Emails.Add(new Email { Subject = "Deleted Email 1", Sender = "john@example.com", Content = "This is a deleted email.", Copies = "johndoe@mail.com", Recepient = "haifhasif", Attachments = new List<string> { } });
            trashFolder.Emails.Add(new Email { Subject = "Deleted Email 2", Sender = "jane@example.com", Content = "Another deleted email.", Copies = "johndoe@mail.com", Recepient = "haifhasif", Attachments = new List<string> { } });
            Folders.Add(trashFolder);
        }

        // sair da app (funciona)
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        public class Folder : INotifyPropertyChanged
        {
            private string name { get; set; }

            private ObservableCollection<Email> emails;

            public string Name
            {
                get { return name; }
                set
                {
                    if (name != value)
                    {
                        name = value;
                        OnPropertyChanged(nameof(Name));
                    }
                }
            }

            public ObservableCollection<Email> Emails
            {
                get { return emails; }
                set
                {
                    if (emails != value)
                    {
                        emails = value;
                        OnPropertyChanged(nameof(Emails));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }


        public class Email : INotifyPropertyChanged
        {

            private string subject;
            public string Subject
            {
                get { return subject; }
                set
                {
                    if (subject != value)
                    {
                        subject = value;
                        OnPropertyChanged(nameof(Subject));
                    }
                }
            }

            private string sender;
            public string Sender
            {
                get { return sender; }
                set
                {
                    if (sender != value)
                    {
                        sender = value;
                        OnPropertyChanged(nameof(Sender));
                    }
                }
            }

            private string content;
            public string Content
            {
                get { return content; }
                set
                {
                    if (content != value)
                    {
                        content = value;
                        OnPropertyChanged(nameof(Content));
                    }
                }
            }

            private string copies;
            public string Copies
            {
                get { return copies; }
                set
                {
                    if (copies != value)
                    {
                        copies = value;
                        OnPropertyChanged(nameof(copies));
                    }
                }
            }

            private string recepient;
            public string Recepient
            {
                get { return recepient; }
                set
                {
                    if (recepient != value)
                    {
                        recepient = value;
                        OnPropertyChanged(nameof(recepient));
                    }
                }
            }

            private List<string> attachments;
            public List<string> Attachments
            {
                get { return attachments; }
                set
                {
                    if (attachments != value)
                    {
                        attachments = value;
                        OnPropertyChanged(nameof(Attachments));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // selecionar os diferentes folders
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem selectedTreeViewItem)
            {
                if (selectedTreeViewItem.Header is string folderName)
                {
                    SelectedFolder = Folders.FirstOrDefault(f => f.Name == folderName);
                    if (SelectedFolder != null)
                    {
                        NewEmailListView.ItemsSource = SelectedFolder.Emails;
                        EmailContentTextBlock.Text = string.Empty;
                    }
                }
                else if (selectedTreeViewItem.Header is Email selectedEmail)
                {
                    EmailContentTextBlock.Text = selectedEmail.Content;
                    EmailContentTextBlock.Text = selectedEmail.Sender;
                }
            }
        }

        // aparece o conteudo de cada email
        private void NewEmailListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Email selectedEmail = (Email)e.AddedItems[0];
                EmailContentTextBlock.Text = selectedEmail.Sender;
            }
            else
            {
                EmailContentTextBlock.Text = string.Empty;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (NewEmailListView.SelectedItem is Email selectedEmail && SelectedFolder != null)
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to delete this message?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Move the selected email to the "Trash" folder
                    if (SelectedFolder.Name == "Trash")
                    {
                        SelectedFolder.Emails.Remove(selectedEmail);
                    }
                    else
                    {
                        Folder trashFolder = Folders.FirstOrDefault(f => f.Name == "Trash");
                        if (trashFolder != null)
                        {
                            trashFolder.Emails.Add(selectedEmail);
                            SelectedFolder.Emails.Remove(selectedEmail);
                        }
                    }
                }
            }
        }

        private void NewEmail_Click(object sender, RoutedEventArgs e)
        {
            newMessage emailWindow = new newMessage(Folders);
            emailWindow.ShowDialog();
        }

        private void AddToSentItems(Email email)
        {
            var sentItemsFolder = Folders.FirstOrDefault(f => f.Name == "Sent");
            if (sentItemsFolder != null)
            {
                sentItemsFolder.Emails.Add(email);
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Get the selected email from the ListView
            var selectedEmail = (sender as ListView).SelectedItem as Email;

            // Open the newMessage window
            newMessage emailWindow = new newMessage(Folders);

            // Fill the fields with the data of the selected message
            emailWindow.subject.Text = selectedEmail.Subject;
            emailWindow.senders.Text = selectedEmail.Sender;
            emailWindow.content.Text = selectedEmail.Content;
            emailWindow.copies.Text = selectedEmail.Copies;
            emailWindow.recipient.Text = selectedEmail.Recepient;

            foreach (var attachment in selectedEmail.Attachments)
            {
                emailWindow.attachments.Items.Add(attachment);
            }

            // Disable the send button
            emailWindow.SendButton.IsEnabled = false;

            // Make the input fields read-only
            emailWindow.subject.IsReadOnly = true;
            emailWindow.senders.IsReadOnly = true;
            emailWindow.content.IsReadOnly = true;
            emailWindow.copies.IsReadOnly = true;
            emailWindow.recipient.IsReadOnly = true;

            // Show the email window as a dialog
            emailWindow.ShowDialog();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ImportData(filePath);
            }
        }

        private void ImportData(string filePath)
        {
            try
            {
                // Load the XML document from the specified file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                Folders.Clear();

                // Get the root element
                XmlElement rootElement = xmlDoc.DocumentElement;
                if (rootElement.Name != "EmailData")
                {
                    MessageBox.Show("Invalid XML file. Root element 'EmailData' not found.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get the folders element
                XmlNodeList FoldersNodes = rootElement.GetElementsByTagName("Folders");
                if (FoldersNodes.Count == 0)
                {
                    MessageBox.Show("Invalid XML file. 'Folders' element not found.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                XmlNode FoldersNode = FoldersNodes[0];

                // Iterate over each folder element
                foreach (XmlNode folderNode in FoldersNode.ChildNodes)
                {
                    if (folderNode.Name == "Folder")
                    {
                        // Get the folder name
                        string folderName = folderNode.Attributes["Name"]?.Value;
                        if (string.IsNullOrEmpty(folderName))
                        {
                            MessageBox.Show("Invalid XML file. Folder name not found.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Create a new Folder object
                        Folder folder = new Folder
                        {
                            Name = folderName,
                            Emails = new ObservableCollection<Email>()
                        };

                        // Get the emails within the folder
                        foreach (XmlNode emailNode in folderNode.ChildNodes)
                        {
                            if (emailNode.Name == "Email")
                            {
                                // Get the email attributes
                                string subject = emailNode.Attributes["Subject"]?.Value;
                                string sender = emailNode.Attributes["Sender"]?.Value;
                                string content = emailNode.Attributes["Content"]?.Value;
                                string copies = emailNode.Attributes["Copies"]?.Value;
                                string recipients = emailNode.Attributes["Recipients"]?.Value;
                                string attachments = emailNode.Attributes["Attachments"]?.Value;

                                // Create a new Email object
                                Email email = new Email
                                {
                                    Subject = subject,
                                    Sender = sender,
                                    Content = content,
                                    Copies = copies,
                                    Recepient = recipients,
                                    Attachments = string.IsNullOrEmpty(attachments) ? new List<string>() : attachments.Split(',').Select(attachment => attachment.Trim()).ToList()
                                };

                                // Add the email to the folder
                                folder.Emails.Add(email);
                            }
                        }
                        Folders.Add(folder);
                    }
                }

                // Show a success message
                MessageBox.Show("Import completed successfully!", "Import", MessageBoxButton.OK, MessageBoxImage.Information);

                NewEmailListView.ItemsSource = Folders.SelectMany(f => f.Emails);
            }
            catch (Exception ex)
            {
                // Show an error message if there was an exception during import
                MessageBox.Show("Error occurred during import:\n" + ex.Message, "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Create a new XML document
                    XmlDocument xmlDoc = new XmlDocument();

                    // Create the root element
                    XmlElement rootElement = xmlDoc.CreateElement("EmailData");
                    xmlDoc.AppendChild(rootElement);

                    // Export folders
                    ExportFolders(rootElement);

                    // Save the XML document to the specified file
                    xmlDoc.Save(filePath);

                    // Show a success message
                    MessageBox.Show("Export completed successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Show an error message if there was an exception during export
                    MessageBox.Show("Error occurred during export:\n" + ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportFolders(XmlElement parentElement)
        {
            // Create the folders element
            XmlElement foldersElement = parentElement.OwnerDocument.CreateElement("Folders");
            parentElement.AppendChild(foldersElement);

            // Export each folder
            foreach (var folder in Folders)
            {

                XmlElement folderElement = parentElement.OwnerDocument.CreateElement("Folder");
                folderElement.SetAttribute("Name", folder.Name);

                // Export each email within the folder
                foreach (var email in folder.Emails)
                {
                    XmlElement emailElement = parentElement.OwnerDocument.CreateElement("Email");
                    emailElement.SetAttribute("Subject", email.Subject);
                    emailElement.SetAttribute("Sender", email.Sender);
                    emailElement.SetAttribute("Content", email.Content);
                    emailElement.SetAttribute("Copies", string.Join(", ", email.Copies));
                    emailElement.SetAttribute("Recipients", string.Join(", ", email.Recepient));
                    emailElement.SetAttribute("Attachments", string.Join(", ", email.Attachments));

                    folderElement.AppendChild(emailElement);
                }

                foldersElement.AppendChild(folderElement);
            }
        }


        public class RelayCommand : ICommand
        {
            private readonly Action<object> execute;
            private readonly Func<object, bool> canExecute;

            public RelayCommand(Action<object> execute)
                : this(execute, null)
            {
            }

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
            {
                this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
                this.canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object parameter)
            {
                return canExecute?.Invoke(parameter) ?? true;
            }

            public void Execute(object parameter)
            {
                execute(parameter);
            }
        }



    }
}
