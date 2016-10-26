using System;
using System.Windows.Forms;
using System.Media;
using System.Drawing;
class RandomPractise : Form
{
    int q;
    Random rand = new Random();
    Question[] book = Program.book;
    Label questionLabel = new Label();
    Label question = new Label();
    int[] index;
    void init()
    {
        index = new int[book.Length];
        for (int i = 0; i < index.Length; i++) index[i] = i;
        for (int i = 0; i < 300; i++)
        {
            int one = rand.Next(0, index.Length - 1);
            int two = rand.Next(0, index.Length - 1);
            if (one != two)
            {
                int t = index[one];
                index[one] = index[two];
                index[two] = t;
            }
        }
    }
    public RandomPractise()
    {
        Text = "随机练习";
        init();
        questionLabel.Location = new Point(950, 120);
        question.Location = new Point(40, 40);
        question.Size = new Size(900, 200);

        question.Font = new Font("Consolas", 15, FontStyle.Bold);

        MenuStrip menu = new MenuStrip();
        ToolStripMenuItem seeAns = new ToolStripMenuItem("看答案", null, this.seeAns);
        ToolStripMenuItem nextQuestion = new ToolStripMenuItem("下一题", null, delegate { this.nextQuestion(); showQuestion(); });
        ToolStripMenuItem lastQuestion = new ToolStripMenuItem("上一题", null, delegate { this.lastQuestion(); showQuestion(); });

        menu.Items.Add(seeAns);
        menu.Items.Add(lastQuestion);
        menu.Items.Add(nextQuestion);

        Controls.Add(menu);
        Controls.Add(questionLabel);
        Controls.Add(question);

        showQuestion();
    }
    void seeAns(object o, EventArgs e)
    {
        foreach (Control c in Controls)
        {
            if (c is Choice)
            {
                Choice cho = c as Choice;
                if (cho.num == book[index[q]].ans)
                {
                    cho.chosen = true;
                    return;
                }
            }
        }
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
        questionLabel.Text = (1 + q) + "";
        question.Text = book[index[q]].str;
        for (int i = 0; i < 4; i++)
        {
            var cho = new Choice(i, (char)(i + 'A') + ". " + book[index[q]].choice[i], new Rectangle(40, 100 * i + 240, 900, 100), singleClick);
            Controls.Add(cho);
        }
    }
    void singleClick(object o, EventArgs e)
    {
        Choice cho = o as Choice;
        if (cho.num == book[index[q]].ans)
        {
            nextQuestion();
            showQuestion();
        }
        else
        {
            cho.chosen = false;
        }
    }
    void lastQuestion()
    {
        if (q != 0) q--;
    }
    void nextQuestion()
    {
        if (q != index.Length - 1) q++;
    }
}