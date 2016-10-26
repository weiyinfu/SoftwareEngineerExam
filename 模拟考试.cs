using System;
using System.Windows.Forms;
using System.Media;
using System.Drawing;
class Paper
{
    public Question[] single = new Question[40];
    Random rand = new Random();

    public Paper()
    {
        Question[] book = Program.book;
        for (int i = 0; i < single.Length; i++)
        {
            single[i] = book[rand.Next(0, book.Length - 1)];
            for (int j = 0; j < i; j++)
            {
                if (single[j] == single[i])
                {
                    i--; break;
                }
            }
        }
    }
}
class Unsure
{
    public int num;
    public override string ToString()
    {
        string name = Exam.paper.single[num].str;
        return (1 + num) + ". " + name;
    }
    public Unsure(int num)
    {
        this.num = num;
    }
}
class Exam : Form
{
    public static Paper paper = new Paper();
    int q = 0;
    int[] singleAns = new int[paper.single.Length];
    int count = 2400;
    Choice[] question = new Choice[5];
    ListBox singleList = new ListBox();
    ListBox unSureList = new ListBox();
    Label time = new Label();
    Timer timer = new Timer();
    bool autoNextQuestion = true;

    public Exam()
    {
        paper = new Paper();
        Text ="模拟考试";
        WindowState = FormWindowState.Maximized;

        MenuStrip menu = new MenuStrip();
        ToolStripMenuItem handin = new ToolStripMenuItem("交卷", null, delegate { this.handin(); });
        ToolStripMenuItem lastQuestion = new ToolStripMenuItem("上一题", null, delegate { this.lastQuestion(); showQuestion(); });
        ToolStripMenuItem nextQuestion = new ToolStripMenuItem("下一题", null, delegate { this.nextQuestion(); showQuestion(); });
        ToolStripMenuItem autoNext = new ToolStripMenuItem("自动下一题"); 
        autoNext.Click+=delegate
        {
            if (autoNextQuestion) autoNext.Text = "不自动下一题";
            else autoNext.Text = "自动下一题";
            autoNextQuestion = !autoNextQuestion;
        };

        menu.Items.Add(handin);
        menu.Items.Add(autoNext);
        menu.Items.Add(lastQuestion);
        menu.Items.Add(nextQuestion);

        question[4] = new Choice(4, "", new Rectangle(40, 40, 900, 200), delegate { mark(); });
        for (int i = 0; i < 4; i++)
        {
            question[i] = new Choice(i, "", new Rectangle(40, 100 * i + 240, 900, 100), click);
        }

        time.ForeColor = Color.Tomato;
        time.Font = new Font("Consolas", 20, FontStyle.Bold);
        time.Location = new Point(960, 30);
        time.Size = new Size(300, 50);

        Font font = new Font("Consolas", 10, FontStyle.Bold);
        Label singleLable = new Label();
        singleLable.Font = font;
        singleLable.Location = new Point(950, 100);
        singleLable.Text = "单选题";
        singleList.Location = new Point(950, 120);
        singleList.Font = font;
        singleList.Size = new Size(300, 250);

        Label unSureLabel = new Label();
        unSureLabel.Font = font;
        unSureLabel.Location = new Point(950, 380);
        unSureLabel.Text = "标记题";
        unSureList.Location = new Point(950, 400);
        unSureList.Size = new Size(300, 320);
        unSureList.Font = new Font("Consolas", 10, FontStyle.Bold);

        singleList.SelectedIndexChanged += delegate { q = singleList.SelectedIndex; showQuestion(); };

        unSureList.SelectedIndexChanged += delegate
        {
            Unsure u = unSureList.SelectedItem as Unsure;
            if (u == null) return;
            q = u.num;
            showQuestion();
        };

        MainMenuStrip = menu;
        Controls.Add(menu);
        Controls.Add(singleList);
        Controls.Add(unSureList);
        Controls.Add(singleLable);
        Controls.Add(unSureLabel);
        Controls.Add(time);

        foreach (Choice c in question)
            Controls.Add(c);

        for (int i = 0; i < paper.single.Length; i++)
        {
            singleList.Items.Add((i + 1) + ". " + paper.single[i].str);
        }
        for (int i = 0; i < singleAns.Length; i++)
            singleAns[i] = -1;

        showQuestion();
        timer.Interval = 1000;
        timer.Tick += delegate { tick(); };
        timer.Start();
    }
    void handin()
    {
        timer.Stop();
        count = 0;
        MainMenuStrip.Items.RemoveAt(0);
        MainMenuStrip.Items.RemoveAt(0);
        Controls.Remove(time);
        judge();
    }
    bool marked()
    {
        foreach (object o in unSureList.Items)
        {
            Unsure u = o as Unsure;
            if (u.num == q)
            {
                return true;
            }
        }
        return false;
    }
    void mark()
    {
        foreach (object o in unSureList.Items)
        {
            Unsure u = o as Unsure;
            if (u.num == q)
            {
                unSureList.Items.Remove(o);
                question[4].chosen = false;
                return;
            }
        }
        question[4].chosen = true;
        unSureList.Items.Add(new Unsure(q));
        return;
    }
    void judge()
    {
        int score = 0;
        int singleError = 0;
        unSureList.Items.Clear();
        for (int i = 0; i < paper.single.Length; i++)
        {
            if (singleAns[i] != paper.single[i].ans)
            {
                singleError++;
                unSureList.Items.Add(new Unsure(i));
            }
        }
        score = paper.single.Length - singleError;
        if (score == paper.single.Length)
        {
            MessageBox.Show("你太牛逼了！竟然全对了。");
        }
        else
        {
            MessageBox.Show("满分" + paper.single.Length + "，你考" + score + ".\n错了" + singleError + "道单选\n错题在标记栏里");
        }
    }
    void tick()
    {
        count--;
        if (count == 0)
        {
            handin();
        }
        else
        {
            int minute = count / 60;
            int second = count % 60;
            time.Text = minute + " : " + second;
        }
    }
    void initChoice()
    {
        for (int i = 0; i < 4; i++)
        {
            question[i].ForeColor = ForeColor;
            question[i].Font = new Font("Consolas", 15);
        }
        question[paper.single[q].ans].Font = new Font("隶书", 19, FontStyle.Italic | FontStyle.Bold);
        question[paper.single[q].ans].ForeColor = Color.Green;
    }
    void showQuestion()
    {
        if (count == 0)
            initChoice();
        for (int i = 0; i < 4; i++)
        {
            question[i].Text = (char)(i + 'A') + ". " + paper.single[q].choice[i];
            question[i].num = i;
            if (singleAns[q] == i)
                question[i].chosen = true;
            else
                question[i].chosen = false;
        }
        if (marked()) question[4].chosen = true;
        else question[4].chosen = false;
        question[4].Text = (1 + q) + ". " + paper.single[q].str;
    }
    void click(object o, EventArgs e)
    {
        Choice cho = o as Choice;
        if (cho.num == singleAns[q])
        {
            cho.chosen = false;
            singleAns[q] = -1;
        }
        else
        {
            if (singleAns[q] != -1)
                question[singleAns[q]].chosen = false;
            cho.chosen = true;
            question[cho.num].chosen = true;
            singleAns[q] = cho.num;
            if (autoNextQuestion) nextQuestion();
            showQuestion();
        }
    }
    void lastQuestion()
    {
        if (q == 0) return;
        else q--;
    }
    void nextQuestion()
    {
        if (q == paper.single.Length - 1) return;
        else q++;
    }
}