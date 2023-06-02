using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using static HCI_work.MainWindow;

namespace HCI_work
{
    /// <summary>
    /// Lógica interna para newMessage.xaml
    /// </summary>
    public partial class newMessage : Window
    {

        private ObservableCollection<Folder> folders;

        public newMessage(ObservableCollection<Folder> folders)
        {
            InitializeComponent();
            this.folders = folders;
            DataContext = this;

            AttachmentsList = new ObservableCollection<string>();
        }

        private ObservableCollection<string> attachmentsList;
        public ObservableCollection<string> AttachmentsList
        {
            get { return attachmentsList; }
            set
            {
                attachmentsList = value;
                OnPropertyChanged(nameof(AttachmentsList));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void btnAddAttachment_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Multimedia Files|*.mp3;*.mp4;*.avi;*.wmv;*.jpg;*.png|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    attachments.Items.Add(System.IO.Path.GetFileName(fileName));
                }
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string recipients = recipient.Text;
            string subjects = subject.Text;

            if (string.IsNullOrEmpty(recipients) || string.IsNullOrEmpty(subjects))
            {
                MessageBox.Show("Recipient and subject are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var email = new Email
            {
                Subject = subject.Text,
                Sender = senders.Text,
                Content = content.Text,
                Recepient = recipient.Text,
                Copies = copies.Text,
                Attachments = attachments.Items.Cast<string>().ToList()
        };

            foreach (var attachment in attachments.Items)
            {
                email.Attachments.Add(attachment.ToString());
            }

            AddToSentItems(email);

            MessageBox.Show("Email sent and added to Sent Items.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void AddToSentItems(Email email)
        {
            var sentItemsFolder = folders.FirstOrDefault(f => f.Name == "Sent");
            if (sentItemsFolder != null)
            {
                sentItemsFolder.Emails.Add(email);
            }
        }
    }

}

