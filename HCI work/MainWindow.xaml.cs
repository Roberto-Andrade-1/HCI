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

        // cria a classe folder
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

        // cria a classe email
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

        // sair da app
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
        }

        // remover o email
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


        // chama a pagina do novo email
        private void NewEmail_Click(object sender, RoutedEventArgs e)
        {
            newMessage emailWindow = new newMessage(Folders);
            emailWindow.ShowDialog();
        }

        // mostra na nova pagina o email selecionado
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // seleciona o email da lista
            var selectedEmail = (sender as ListView).SelectedItem as Email;

            // abre a janela
            newMessage emailWindow = new newMessage(Folders);

            // preenche os campos com a informção do email
            emailWindow.subject.Text = selectedEmail.Subject;
            emailWindow.senders.Text = selectedEmail.Sender;
            emailWindow.content.Text = selectedEmail.Content;
            emailWindow.copies.Text = selectedEmail.Copies;
            emailWindow.recipient.Text = selectedEmail.Recepient;

            foreach (var attachment in selectedEmail.Attachments)
            {
                emailWindow.attachments.Items.Add(attachment);
            }

            // desativa os botões
            emailWindow.SendButton.IsEnabled = false;
            emailWindow.btnAddAttachment.IsEnabled = false;

            // faz com que seja de leitura
            emailWindow.subject.IsReadOnly = true;
            emailWindow.senders.IsReadOnly = true;
            emailWindow.content.IsReadOnly = true;
            emailWindow.copies.IsReadOnly = true;
            emailWindow.recipient.IsReadOnly = true;

            // mostra o email como dialogo
            emailWindow.ShowDialog();
        }

        // link para o botão para importar os emails em xml
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

        // associa os dados
        private void ImportData(string filePath)
        {
            try
            {
                // carrega o ficheiro xml escolhido
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                Folders.Clear();

                // elemento raiz
                XmlElement rootElement = xmlDoc.DocumentElement;
                if (rootElement.Name != "EmailData")
                {
                    MessageBox.Show("Invalid XML file. Root element 'EmailData' not found.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // vê o ficheiro a que pertence
                XmlNodeList FoldersNodes = rootElement.GetElementsByTagName("Folders");
                if (FoldersNodes.Count == 0)
                {
                    MessageBox.Show("Invalid XML file. 'Folders' element not found.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                XmlNode FoldersNode = FoldersNodes[0];

                // itera por todos os elementos desse ficheiro
                foreach (XmlNode folderNode in FoldersNode.ChildNodes)
                {
                    if (folderNode.Name == "Folder")
                    {
                        // vê o nome do ficheiro
                        string folderName = folderNode.Attributes["Name"]?.Value;
                        if (string.IsNullOrEmpty(folderName))
                        {
                            MessageBox.Show("Invalid XML file. Folder name not found.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // cria um objeto do tipo ficheiro
                        Folder folder = new Folder
                        {
                            Name = folderName,
                            Emails = new ObservableCollection<Email>()
                        };

                        // vê os emails dentro desse ficheiro
                        foreach (XmlNode emailNode in folderNode.ChildNodes)
                        {
                            if (emailNode.Name == "Email")
                            {
                                // vê os atributos do email
                                string subject = emailNode.Attributes["Subject"]?.Value;
                                string sender = emailNode.Attributes["Sender"]?.Value;
                                string content = emailNode.Attributes["Content"]?.Value;
                                string copies = emailNode.Attributes["Copies"]?.Value;
                                string recipients = emailNode.Attributes["Recipients"]?.Value;
                                string attachments = emailNode.Attributes["Attachments"]?.Value;

                                // cria um novo objeto do tipo email
                                Email email = new Email
                                {
                                    Subject = subject,
                                    Sender = sender,
                                    Content = content,
                                    Copies = copies,
                                    Recepient = recipients,
                                    Attachments = string.IsNullOrEmpty(attachments) ? new List<string>() : attachments.Split(',').Select(attachment => attachment.Trim()).ToList()
                                };

                                // adiciona o email ao ficheiro
                                folder.Emails.Add(email);
                            }
                        }
                        Folders.Add(folder);
                    }
                }

                NewEmailListView.ItemsSource = Folders.SelectMany(f => f.Emails);
            }
            catch (Exception ex)
            {
                // Mostra uma mensagem de erro caso aconteça durante a importação
                MessageBox.Show("Error occurred during import:\n" + ex.Message, "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // link para o botão para exportar os emails
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    // cria um novo documento do tipo xml
                    XmlDocument xmlDoc = new XmlDocument();

                    // cria o elemento raiz
                    XmlElement rootElement = xmlDoc.CreateElement("EmailData");
                    xmlDoc.AppendChild(rootElement);

                    // exporta os ficheiros
                    ExportFolders(rootElement);

                    // salva o ficheiro xml na pasta correta
                    xmlDoc.Save(filePath);

                    // mensagem de sucesso
                    MessageBox.Show("Export completed successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // mostra uma mensagem de erro caaso aconteça durante a exportação
                    MessageBox.Show("Error occurred during export:\n" + ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // associa os dados
        private void ExportFolders(XmlElement parentElement)
        {
            // cria o elemento do ficheiro
            XmlElement foldersElement = parentElement.OwnerDocument.CreateElement("Folders");
            parentElement.AppendChild(foldersElement);

            // exporta cada ficheiro
            foreach (var folder in Folders)
            {

                XmlElement folderElement = parentElement.OwnerDocument.CreateElement("Folder");
                folderElement.SetAttribute("Name", folder.Name);

                // exporta cada email dentro do ficheiro
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

            public RelayCommand(Action<object> execute) : this(execute, null) {}

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
