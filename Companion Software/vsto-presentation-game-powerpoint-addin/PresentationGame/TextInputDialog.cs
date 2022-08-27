using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace PresentationGame
{
    public partial class TextInputDialog : Form
    {
        private readonly string _defaultAnswer;
        private readonly string _question;

        public TextInputDialog(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            _question = question;
            _defaultAnswer = defaultAnswer;

            AnswerObservable = Observable.Create<string>(observer =>
            {
                if (ShowDialog() == DialogResult.OK) observer.OnNext(txtInput.Text);

                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public IObservable<string> AnswerObservable { get; }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void TextInputDialog_Load(object sender, EventArgs e)
        {
            lblQuestion.Text = _question;
            txtInput.Text = _defaultAnswer;
            txtInput.SelectAll();
        }
    }
}