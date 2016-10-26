using System;
using System.Windows.Forms;
using System.Media;
using System.Drawing;
class Recite : Form
{
    int q;
    Random rand = new Random();
    Question[] book = Program.book;

    Label questionLabel = new Label();
    Label question = new Label();

    public Recite()
    {
        Text = "背题模式---按键盘任意键或者鼠标点击空白处";

        MouseDown += delegate
        {
            this.nextQuestion();
            showQuestion();
        };
        KeyDown += delegate
        {
            this.nextQuestion();
            showQuestion();
        };

        questionLabel.Location = new Point(950, 120);
        question.Location = new Point(40, 40);
        question.Size = new Size(900, 200);

        question.Font = new Font("Consolas", 15, FontStyle.Bold);

        MenuStrip menu = new MenuStrip();
        ToolStripMenuItem nextQuestion = new ToolStripMenuItem("下一题", null, delegate { this.nextQuestion(); showQuestion(); });
        ToolStripMenuItem lastQuestion = new ToolStripMenuItem("上一题", null, delegate { this.lastQuestion(); showQuestion(); });
         
        menu.Items.Add(lastQuestion);
        menu.Items.Add(nextQuestion);

        Controls.Add(menu);
        Controls.Add(questionLabel);
        Controls.Add(question);

        showQuestion();
    }
    //keyDown 不管用，不知为什么
    void keyDown(object o, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up:
            case Keys.Left: lastQuestion(); break;
            default: nextQuestion(); break;
        }
        showQuestion();
    }
    void showQuestion()
    {
        for (int i = Controls.Count - 1; i >= 0; i--)
        {
            if (Controls[i] is Choice)
            {
                Controls[i].Dispose();
            }
        }
        questionLabel.Text = q + 1 + "";
        question.Text = book[q].str;
        for (int i = 0; i < 4; i++)
        {
            var cho = new Choice(i, (char)(i + 'A') + ". " + book[q].choice[i], new Rectangle(40, 100 * i + 240, 900, 100), null);
            if (i == book[q].ans)
                cho.chosen = true;
            Controls.Add(cho);
        }
    }
    void lastQuestion()
    {
        q--;
        if (q == -1) q++;
    }
    void nextQuestion()
    {
        q++;
        if (q == book.Length) q--;
    }
}