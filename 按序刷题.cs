using System;
using System.Windows.Forms;
using System.Media;
using System.Drawing;
using System.IO;
class OrderPractise : Form
{
    int q;
    Random rand = new Random();
    Question[] book = Program.book;
    Label questionLabel = new Label();
    Label question = new Label();
    bool autoNext = true;
    bool[] del;

    public OrderPractise()
    {
        Text = "按序练习";

        del = new bool[book.Length];

        Load += delegate
        { 
            try
            {
                if (File.Exists("config.txt"))
                {
                    var cin = new BinaryReader(new FileStream("config.txt", FileMode.Open));
                    for (int i = 0; i < book.Length; i++)
                    {
                        del[i] = cin.ReadBoolean();
                    }
                    cin.Close();
                }
            }
            catch
            {
                MessageBox.Show("你他妈的傻呀,谁叫你乱改文件内容的");
            } 
            for (q = 0; del[q] && q != book.Length - 1; this.nextQuestion()) ;
            showQuestion(); 
        };
        FormClosing += delegate
        {
            BinaryWriter cout = new BinaryWriter(new FileStream("config.txt", FileMode.Create));
            for (int i = 0; i < book.Length; i++)
            {
                cout.Write(del[i]);
            }
            cout.Close();
        };

        questionLabel.Location = new Point(950, 120);
        questionLabel.Font = new Font("Consolas", 15, FontStyle.Italic);
        question.Location = new Point(40, 40);
        question.Size = new Size(900, 200);
        question.Font = new Font("Consolas", 15, FontStyle.Bold);

        MenuStrip menu = new MenuStrip();
        ToolStripMenuItem seeAns = new ToolStripMenuItem("看答案", null, this.seeAns);
        ToolStripMenuItem nextQuestion = new ToolStripMenuItem("下一题", null, delegate { this.nextQuestion(); showQuestion(); });
        ToolStripMenuItem lastQuestion = new ToolStripMenuItem("上一题", null, delegate { this.lastQuestion(); showQuestion(); });
        ToolStripMenuItem autoNext = new ToolStripMenuItem("自动下一题");
        autoNext.Click += delegate
        {
            autoNext.Text = this.autoNext ? "不自动下一题" : "自动下一题";
            this.autoNext = !this.autoNext;
        };
        ToolStripMenuItem clean = new ToolStripMenuItem("重载题目");
        clean.Click += delegate
        {
            for (int i = 0; i < book.Length; i++) del[i] = false;
        };

        menu.Items.Add(clean);
        menu.Items.Add(seeAns);
        menu.Items.Add(autoNext);
        menu.Items.Add(lastQuestion);
        menu.Items.Add(nextQuestion);

        Button delete = new Button();
        delete.Font = new Font("楷体", 20, FontStyle.Bold);
        delete.Text = "删除此题";
        delete.Bounds = new Rectangle(950, 350, 200, 250);
        delete.Click += delegate
        {
            del[q] = true;
            this.nextQuestion();
            showQuestion();
        };


        NumericUpDown chooseQuestion = new NumericUpDown();
        chooseQuestion.SetBounds(950, 50, 200, 120);
        chooseQuestion.Maximum = book.Length;
        chooseQuestion.Minimum = 1;
        chooseQuestion.Font = new Font("Consolas", 15, FontStyle.Italic);
        chooseQuestion.ValueChanged += delegate
        {
            q = (int)chooseQuestion.Value;
            q--;
            showQuestion();
        };

        Controls.Add(chooseQuestion);
        Controls.Add(delete);
        Controls.Add(menu);
        Controls.Add(questionLabel);
        Controls.Add(question); 
    }
    void seeAns(object o, EventArgs e)
    {
        foreach (Control c in Controls)
        {
            if (c is Choice)
            {
                Choice cho = c as Choice;
                if (cho.num == book[q].ans)
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
        questionLabel.Text = q + 1 + "";
        question.Text = book[q].str;
        for (int i = 0; i < 4; i++)
        {
            var cho = new Choice(i, (char)(i + 'A') + ". " + book[q].choice[i], new Rectangle(40, 100 * i + 240, 900, 100), singleClick);
            Controls.Add(cho);
        }
    }
    void singleClick(object o, EventArgs e)
    {
        Choice cho = o as Choice;
        if (cho.num == book[q].ans)
        {
            if (autoNext)
            {
                nextQuestion();
                showQuestion();
            }
        }
        else
        {
            cho.chosen = false;
        }
    }
    void lastQuestion()
    {
        if (q != 0) q--;
        if (del[q] && q != 0) lastQuestion();
    }
    void nextQuestion()
    {
        if (q != book.Length - 1) q++;
        if (del[q] && q != book.Length - 1) nextQuestion();
    }
}