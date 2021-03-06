﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсач
{
    public partial class Form2 : Form
    {
        public Form1 form1;
        public List<Студент> lst = new List<Студент>();
        int rowAdd;
        int rating;
        int sort_order = -1;
        bool first_sort = true;
        int current_column = 50;
        public int n;
        public Form2()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        { 
            rowAdd = dataGridView1.RowCount;
            студентBindingSource.DataSource = lst;
            //выставляем фильтрацию рейтинга такую же, как и при предыдущем открытии приложении
            radioButton3.Checked = Properties.Settings.Default.rb1;
            radioButton2.Checked = Properties.Settings.Default.rb2;
            radioButton1.Checked = Properties.Settings.Default.rb3;
            textBox4.Text = Properties.Settings.Default.tb4;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //закрытие
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //открытие формы с изменением студента
            if (dataGridView1.CurrentRow == null)
                return;
            n = dataGridView1.CurrentRow.Index;
            EditStudentForm formS = new EditStudentForm();

            formS.Form2 = this;
            formS.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //открытие формы с добавлением студента
            AddStudentForm formS = new AddStudentForm();

            formS.Form2 = this;
            formS.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {//удаление
            if (dataGridView1.CurrentCell == null)
                return;
            if (dataGridView1.SelectedRows.Count > 1)
            {//если выбрано несколько групп
                if (MessageBox.Show(
                    "Вы действительно хотите удалить несколько студентов?", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {//добавляем в listDel выбранные строки и удаляем эти строки из lstG

                    List<Студент> listDel = new List<Студент>();
                    foreach (DataGridViewRow item in dataGridView1.SelectedRows)
                        listDel.Add(lst[item.Index]);
                    foreach (Студент item in listDel)
                        lst.Remove(item);
                    студентBindingSource.ResetBindings(false);
                    Filter();
                    return;
                }
                else
                    return;
            }
            //удаление одной строки
            string stname = (string)dataGridView1.CurrentRow.Cells["имя"].Value;
            if (MessageBox.Show(
                    "Вы действительно хотите удалить студента " + stname + " ?", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            lst.RemoveAt(dataGridView1.CurrentRow.Index);
            студентBindingSource.ResetBindings(false);
            Filter();
        }

        private void button6_Click(object sender, EventArgs e)
        {//фильтрация
            Filter();
           
        }
        public void Filter()
        {//проверка на перевод из строки в число    
            if (textBox4.Text != "")
                if (!int.TryParse(textBox4.Text, out rating))
                {
                    MessageBox.Show("Рейтинг должен быть задан числом");
                    textBox4.Focus();
                    return;
                }
            //фильтрация
            dataGridView1.CurrentCell = null;
            for (int i = 0; i < dataGridView1.Rows.Count - rowAdd; i++)
            {
                if (TestRow(i))
                    //если строка проходит по критериям, показываем её
                    dataGridView1.Rows[i].Visible = true;
                else
                    //иначе скрываем её
                    dataGridView1.Rows[i].Visible = false;
            }
        }
        private bool TestRow(int i)
        {            
            //критерии
            if (textBox1.Text != "" && lst[i].Имя.ToUpper().StartsWith(textBox1.Text.ToUpper()) == false) return false;

            if (textBox2.Text != "" && lst[i].Год.ToString().ToUpper().StartsWith(textBox2.Text.ToUpper()) == false) return false;

            if (radioButton3.Checked && textBox4.Text != "" && lst[i].Рейтинг < int.Parse(textBox4.Text)) return false;

            if (radioButton2.Checked && textBox4.Text != "" && lst[i].Рейтинг > int.Parse(textBox4.Text)) return false;

            if (radioButton1.Checked && textBox4.Text != "" && lst[i].Рейтинг != int.Parse(textBox4.Text)) return false;

            if (textBox3.Text != "" && lst[i].Телефон.ToUpper().StartsWith(textBox3.Text.ToUpper()) == false) return false;

            return true;
        }

        private void button7_Click(object sender, EventArgs e)
        {//очищаем текстбоксы
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //показываем все строки в таблице
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i].Visible = true;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            //запоминаем фильтрацию рейтинга
            Properties.Settings.Default.tb4 = textBox4.Text;
            Properties.Settings.Default.rb1 = radioButton3.Checked;
            Properties.Settings.Default.rb2 = radioButton2.Checked;
            Properties.Settings.Default.rb3 = radioButton1.Checked;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {//сортировка
            if (dataGridView1.Rows.Count > rowAdd)
            {
                //если нажали на столбец не в первый раз подряд, нужно стереть добавление стрелочки в прошлый раз
                if (current_column == e.ColumnIndex)
                    dataGridView1.Columns[e.ColumnIndex].HeaderText = dataGridView1.Columns[e.ColumnIndex].HeaderText.Substring(0, dataGridView1.Columns[e.ColumnIndex].HeaderText.Length - 1);
                //если нажали на другой столбец или нажали в первый раз
                if (current_column != e.ColumnIndex)
                {
                    sort_order = -1;
                    //если нажали на другой столбец, надо стереть изменения в предыдущем столбце
                    if (!first_sort)
                        dataGridView1.Columns[current_column].HeaderText = dataGridView1.Columns[current_column].HeaderText.Substring(0, dataGridView1.Columns[current_column].HeaderText.Length - 1);
                    //если нажали в первый раз, стирать ничего не надо
                    else
                        first_sort = false;
                    //запоминаем колонку
                    current_column = e.ColumnIndex;
                }


                sort_order = -1 * sort_order;
                //sort_order = -1 - по убыванию
                //sort_order = 1 - по возрастанию
                //добавляем стрелку
                if (sort_order == 1)
                    dataGridView1.Columns[e.ColumnIndex].HeaderText = dataGridView1.Columns[e.ColumnIndex].HeaderText + "↓";
                if (sort_order == -1)
                    dataGridView1.Columns[e.ColumnIndex].HeaderText = dataGridView1.Columns[e.ColumnIndex].HeaderText + "↑";
                //сортировка
                switch (dataGridView1.Columns[e.ColumnIndex].Name)
                {//сортируем таблицу в зависимости от того, какое имя у выбранного столбца
                    case "имя":
                        lst.Sort(delegate (Студент a1, Студент a2)
                        {
                            return sort_order * a1.Имя.CompareTo(a2.Имя);
                        });
                        break;
                    case "год":
                        lst.Sort(delegate (Студент a1, Студент a2)
                        {
                            return sort_order * a1.Год.CompareTo(a2.Год);
                        });
                        break;
                    case "рейтинг":
                        lst.Sort(delegate (Студент a1, Студент a2)
                        {
                            return sort_order * a1.Рейтинг.CompareTo(a2.Рейтинг);
                        });
                        break;
                    
                    case "телефон":
                        lst.Sort(delegate (Студент a1, Студент a2)
                        {
                            return sort_order * a1.Телефон.CompareTo(a2.Телефон);
                        });
                        break;
                    default:
                        return;

                }
                студентBindingSource.ResetBindings(false);
                Filter();

                
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {//запускаем форму со статистикой
            Stats formS = new Stats();
            List<double> y = new List<double>();
            for (int i = 0; i < dataGridView1.Rows.Count - rowAdd; i++)
            {
                if (dataGridView1.Rows[i].Visible)
                    y.Add(Convert.ToDouble(dataGridView1["рейтинг", i].Value));
                    
            }
            //передаем форме список отфильтрованных значений
            formS.Y = y;
            formS.ShowDialog();
        }
    }
}
