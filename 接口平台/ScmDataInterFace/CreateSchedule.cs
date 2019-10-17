using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    /// <summary>
    /// 创建任务调度窗体
    /// </summary>
    /// <remarks>新建一个或编辑现有的调度任务时，调用该窗体编辑调度任务。</remarks>
    public partial class CreateSchedule : Form
    {
        /// <summary>
        /// 存储任务调度数据的DataRow
        /// </summary>
        private DataRow _DataRow = null;
        /// <summary>
        /// 存储任务内容的字符串
        /// </summary>
        private string _Process = "";
        /// <summary>
        /// 构造方法

        /// </summary>
        /// <param name="dr">任务调度的数据--DataRow类型</param>
        public CreateSchedule(DataRow dr)
        {
            InitializeComponent();
            _DataRow = dr;
        }
        /// <summary>
        /// 发生频率改变时，执行相应的界面控件调整

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>发生频率改变时，执行相应的界面控件调整。</remarks>
        private void rdbfreq_Changed(object sender, EventArgs e)
        {
            if (rdbfreq_day.Checked)
            {
                gp_freq.Controls.Clear();
                p1.Parent = gp_freq;
                p1.Dock = DockStyle.Fill;
            }
            if (rdbfreq_week.Checked)
            {
                gp_freq.Controls.Clear();
                p2.Parent = gp_freq;
                p2.Dock = DockStyle.Fill;
            }
            if (rdbfreq_month.Checked)
            {
                gp_freq.Controls.Clear();
                p3.Parent = gp_freq;
                p3.Dock = DockStyle.Fill;
            }
        }
        /// <summary>
        /// 窗体装载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>窗体装载时,初始化界面控件。</remarks>
        private void CreateSchedule_Load(object sender, EventArgs e)
        {
            dt_start.Value = DateTime.Now;
            dt_end.Value = DateTime.Now.AddDays(1);
            dt_one.Value = DateTime.Now;
            if (_DataRow["schedule_ID"] != null && _DataRow["schedule_ID"].ToString() != "")
                loadSchedule();
        }
        /// <summary>
        /// 初始化任务调度界面

        /// </summary>
        private void loadSchedule()
        {
            t_name.Text = _DataRow["schedule_name"].ToString();
            t_description.Text = _DataRow["schedule_description"].ToString();
            _Process = _DataRow["command_xml"].ToString();
            t_cmd.Text = "choose task";
            r_before.Checked = _DataRow["daemon"].ToString() == "0" ? true : false;
            r_last.Checked = _DataRow["daemon"].ToString() == "1" ? true : false;
            switch (_DataRow["freq_type"].ToString())
            {
                case "64"://启动时发生
                    rbsdu_autoexec.Checked = true;
                    break;
                case "128"://计算机空闲时运行
                    rbsdu_idle.Checked = true;
                    break;
                case "1"://一次
                    rbsdu_One.Checked = true;
                    int date = int.Parse(_DataRow["active_start_date"].ToString());
                    int time = int.Parse(_DataRow["active_start_time"].ToString());
                    dt_one.Text = Convert.ToString(date / 10000) + "-" + Convert.ToString(date % 10000 / 100).PadLeft(2, '0')
                        + "-" + Convert.ToString(date % 100).PadLeft(2, '0');
                    time_one.Text = Convert.ToString(time / 10000).PadLeft(2, '0') + ":" + Convert.ToString(time % 10000 / 100).PadLeft(2, '0')
                        + ":" + Convert.ToString(time % 100).PadLeft(2, '0');
                    break;
                case "4"://每天
                    rbsdu_More.Checked = true;
                    rdbfreq_day.Checked = true;
                    nm_dayinterval.Value = decimal.Parse(_DataRow["freq_interval"].ToString());
                    Getdayvalue();
                    break;
                case "8"://每周
                    rbsdu_More.Checked = true;
                    rdbfreq_week.Checked = true;
                    nm_week_interval.Value = decimal.Parse(_DataRow["freq_recurrence_factor"].ToString());
                    w1.Checked = (int.Parse(_DataRow["freq_interval"].ToString()) & 2) == 2 ? true : false;
                    w2.Checked = (int.Parse(_DataRow["freq_interval"].ToString()) & 4) == 4 ? true : false;
                    w3.Checked = (int.Parse(_DataRow["freq_interval"].ToString()) & 8) == 8 ? true : false;
                    w4.Checked = (int.Parse(_DataRow["freq_interval"].ToString()) & 16) == 16 ? true : false;
                    w5.Checked = (int.Parse(_DataRow["freq_interval"].ToString()) & 32) == 32 ? true : false;
                    w6.Checked = (int.Parse(_DataRow["freq_interval"].ToString()) & 64) == 64 ? true : false;
                    w7.Checked = (int.Parse(_DataRow["freq_interval"].ToString()) & 1) == 1 ? true : false;
                    Getdayvalue();
                    break;
                case "16"://每月
                    rbsdu_More.Checked = true;
                    rdbfreq_month.Checked = true;
                    rb_monthday.Checked = true;
                    nm_date.Value = decimal.Parse(_DataRow["freq_interval"].ToString());
                    nm_dmonth_interval.Value = decimal.Parse(_DataRow["freq_recurrence_factor"].ToString());
                    Getdayvalue();
                    break;
                case "32"://每月,相对于 freq_interval
                    rbsdu_More.Checked = true;
                    rdbfreq_month.Checked = true;
                    rb_interval.Checked = true;
                    cb_weeknum.SelectedIndex = (int)Math.Log((double)(int.Parse(_DataRow["freq_relative_interval"].ToString())), 2.0);
                    cb_week.SelectedIndex = int.Parse(_DataRow["freq_interval"].ToString()) -1 ;
                    nm_wmonth_interval.Value = decimal.Parse(_DataRow["freq_recurrence_factor"].ToString());
                    Getdayvalue();
                    break;
            }
        }
        /// <summary>
        /// 设置一天内的频率和开始结束日期等控件的属性

        /// </summary>
        private void Getdayvalue()
        {
            switch (_DataRow["freq_subday_type"].ToString())
            {
                case "1":
                    rb_day_one.Checked = true;
                    int time = int.Parse(_DataRow["active_start_time"].ToString());
                    time_day_one.Text = Convert.ToString(time / 10000).PadLeft(2, '0') + ":" + Convert.ToString(time % 10000 / 100).PadLeft(2, '0')
                        + ":" + Convert.ToString(time % 100).PadLeft(2, '0');
                    break;
                case "4":
                    rb_day_cycle.Checked = true;
                    time = int.Parse(_DataRow["active_start_time"].ToString());
                    int t_end = int.Parse(_DataRow["active_end_time"].ToString());
                    time_day_cycle.Text = Convert.ToString(time / 10000).PadLeft(2, '0') + ":" + Convert.ToString(time % 10000 / 100).PadLeft(2, '0')
                        + ":" + Convert.ToString(time % 100).PadLeft(2, '0');
                    time_end.Text = Convert.ToString(t_end / 10000).PadLeft(2, '0') + ":" + Convert.ToString(t_end % 10000 / 100).PadLeft(2, '0')
                        + ":" + Convert.ToString(t_end % 100).PadLeft(2, '0');
                    nm_day_interval.Value = decimal.Parse(_DataRow["freq_subday_interval"].ToString());
                    cb_subday_type.Text = "分钟";
                    break;
                case "8":
                    rb_day_cycle.Checked = true;
                    time = int.Parse(_DataRow["active_start_time"].ToString());
                    t_end = int.Parse(_DataRow["active_end_time"].ToString());
                    time_day_cycle.Text = Convert.ToString(time / 10000).PadLeft(2, '0') + ":" + Convert.ToString(time % 10000 / 100).PadLeft(2, '0')
                        + ":" + Convert.ToString(time % 100).PadLeft(2, '0');
                    time_end.Text = Convert.ToString(t_end / 10000).PadLeft(2, '0') + ":" + Convert.ToString(t_end % 10000 / 100).PadLeft(2, '0')
                        + ":" + Convert.ToString(t_end % 100).PadLeft(2, '0');
                    nm_day_interval.Value = decimal.Parse(_DataRow["freq_subday_interval"].ToString());
                    cb_subday_type.Text = "小时";
                    break;
                case "16":    // 功能：调度增加秒 20160127 by zlibo
                    rb_day_cycle.Checked = true;
                    time = int.Parse(_DataRow["active_start_time"].ToString());
                    t_end = int.Parse(_DataRow["active_end_time"].ToString());
                    time_day_cycle.Text = Convert.ToString(time / 10000).PadLeft(2, '0') + ":" + Convert.ToString(time % 10000 / 100).PadLeft(2, '0')
                        + ":" + Convert.ToString(time % 100).PadLeft(2, '0');
                    time_end.Text = Convert.ToString(t_end / 10000).PadLeft(2, '0') + ":" + Convert.ToString(t_end % 10000 / 100).PadLeft(2, '0')
                        + ":" + Convert.ToString(t_end % 100).PadLeft(2, '0');
                    nm_day_interval.Value = decimal.Parse(_DataRow["freq_subday_interval"].ToString());
                    cb_subday_type.Text = "秒";
                    break;
            }
            int date = int.Parse(_DataRow["active_start_date"].ToString());
            dt_start.Text = Convert.ToString(date / 10000) + "-" + Convert.ToString(date % 10000 / 100).PadLeft(2, '0')
                        + "-" + Convert.ToString(date % 100).PadLeft(2, '0');
            int date_end = int.Parse(_DataRow["active_end_date"].ToString());
            if (date_end == 99991231)
            {
                rd_nothing.Checked = true;
            }
            else
            {
                rd_end.Checked = true;
                dt_end.Text = Convert.ToString(date_end / 10000) + "-" + Convert.ToString(date_end % 10000 / 100).PadLeft(2, '0')
                + "-" + Convert.ToString(date_end % 100).PadLeft(2, '0');
            }
        }
        /// <summary>
        /// 设置执行一次或重复执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>决定执行一次或重复执行。</remarks>
        private void sduType_Changed(object sender, EventArgs e)
        {
            if (rbsdu_One.Checked)
            {
                p_one.Enabled = true;
                p_more.Enabled = false;
            }
            else if (rbsdu_More.Checked)
            {
                p_one.Enabled = false;
                p_more.Enabled = true;
            }
            else
            {
                p_one.Enabled = false;
                p_more.Enabled = false;
            }
        }
        /// <summary>
        /// 获得调度类型
        /// </summary>
        /// <returns>整型freq_type</returns>
        /// <remarks>获得调度类型,具体参数见《设计方案》</remarks>
        private int Getfreq_type()
        {
            if (rbsdu_One.Checked) return 1;
            else if (rbsdu_More.Checked)
            {
                if (rdbfreq_day.Checked)
                    return 4;
                else if (rdbfreq_week.Checked)
                    return 8;
                else if (rdbfreq_month.Checked && rb_monthday.Checked)
                    return 16;
                else if (rdbfreq_month.Checked && rb_interval.Checked)
                    return 32;
                else return 0;
            }
            else if (rbsdu_autoexec.Checked)
                return 64;
            else if (rbsdu_idle.Checked)
                return 128;
            else return 0;
        }
        /// <summary>
        /// 获得freq_interval值

        /// </summary>
        /// <returns>整型的freq_interval值</returns>
        /// <remarks>获得整型的freq_interval值。</remarks>
        private int Getfreq_interval()
        {
            if (rdbfreq_day.Checked)
            {
                return Convert.ToInt32(nm_dayinterval.Value);
            }
            if (rdbfreq_week.Checked)
            {
                int i = 0;
                if (w1.Checked) i += 2;
                if (w2.Checked) i += 4;
                if (w3.Checked) i += 8;
                if (w4.Checked) i += 16;
                if (w5.Checked) i += 32;
                if (w6.Checked) i += 64;
                if (w7.Checked) i += 1;
                return i;
            }
            else if (rdbfreq_month.Checked)
            {
                if (rb_monthday.Checked) return Convert.ToInt32(nm_date.Value);
                else if (rb_interval.Checked) return cb_week.SelectedIndex + 1;
                else return 0;
            }
            else return 0;
        }
        /// <summary>
        /// 获得freq_subday_type值

        /// </summary>
        /// <returns>整型freq_subday_type值</returns>
        /// <remarks>获得整型freq_subday_type值。</remarks>
        private int Getfreq_subday_type()
        {

            if (rb_day_one.Checked) return 1;
            else if (rb_day_cycle.Checked) //功能：调度增加秒 20160127 by zlibo
            {
                if (cb_subday_type.SelectedIndex == 0) return 8;
                else if (cb_subday_type.SelectedIndex == 1) return 4;
                else if (cb_subday_type.SelectedIndex == -1) return 8;
                else return 16;  
            }
            //return (cb_subday_type.SelectedIndex == 0)? 8: 4;
            else return 0;
        }
        /// <summary>
        /// 获得freq_subday_interval值

        /// </summary>
        /// <returns>整型freq_subday_interval值</returns>
        /// <remarks>获得整型freq_subday_interval值。</remarks>
        private int Getfreq_subday_interval()
        {
            if (rb_day_cycle.Checked)
                return Convert.ToInt32(nm_day_interval.Value);
            else return 0;
        }
        /// <summary>
        /// 获得freq_relative_interval值

        /// </summary>
        /// <returns>整型freq_relative_interval值</returns>
        /// <remarks>获得整型freq_relative_interval值。</remarks>
        private int Getfreq_relative_interval()
        {
            if (rdbfreq_month.Checked)
            {
                int li_index = cb_weeknum.SelectedIndex;

                return Convert.ToInt32(Math.Pow(2 , li_index));
            }
            else return 0;
        }
        /// <summary>
        /// 获得freq_recurrence_factor值

        /// </summary>
        /// <returns>整型freq_recurrence_factor值</returns>
        /// <remarks>获得整型freq_recurrence_factor值。</remarks>
        private int Getfreq_recurrence_factor()
        {
            if (rdbfreq_week.Checked)
            {
                return Convert.ToInt32(nm_week_interval.Value);
            }
            else if (rdbfreq_month.Checked)
            {
                if (rb_monthday.Checked)
                    return Convert.ToInt32(nm_dmonth_interval.Value);
                else if (rb_interval.Checked)
                    return Convert.ToInt32(nm_wmonth_interval.Value);
                else return 0;
            }
            else
                return 0;
        }
        /// <summary>
        /// 获得有效的开始日期

        /// </summary>
        /// <returns>获得整型的开始日期。</returns>
        private int Getactive_start_date()
        {
            if (rbsdu_One.Checked)
            {
                return Convert.ToInt32(Convert.ToDateTime(dt_one.Text).ToString("yyyyMMdd"));
            }
            else if (rbsdu_More.Checked)
            {
                return Convert.ToInt32(Convert.ToDateTime(dt_start.Text).ToString("yyyyMMdd"));
            }
            else
                return 19900101;
        }
        /// <summary>
        /// 获得有效的结束日期

        /// </summary>
        /// <returns>获得整型的结束日期。</returns>
        private int Getactive_end_date()
        {
            if (rd_end.Checked)
            {
                return Convert.ToInt32(Convert.ToDateTime(dt_end.Text).ToString("yyyyMMdd"));
            }
            else if (rd_nothing.Checked)
            {
                return 99991231;
            }
            else
                return 99991231;
        }
        /// <summary>
        /// 获得有效的开始时间

        /// </summary>
        /// <returns>获得整型的开始时间。</returns>
        private int Getactive_start_time()
        {
            if (rbsdu_One.Checked)
            {
                return Convert.ToInt32(Convert.ToDateTime(time_one.Text).ToString("HHmmss"));
            }
            else if (rbsdu_More.Checked)
            {
                if (rb_day_one.Checked)
                {
                    return Convert.ToInt32(Convert.ToDateTime(time_day_one.Text).ToString("HHmmss"));
                }
                else if (time_day_cycle.Checked)
                {
                    return Convert.ToInt32(Convert.ToDateTime(time_day_cycle.Text).ToString("HHmmss"));
                }
                else return 0;
            }
            else return 0;
        }
        /// <summary>
        /// 获得有效的结束时间

        /// </summary>
        /// <returns>获得整型的结束时间。</returns>
        private int Getactive_end_time()
        {
             if (rbsdu_More.Checked)
            {
                if (time_day_cycle.Checked)
                {
                    return Convert.ToInt32(Convert.ToDateTime(time_end.Text).ToString("HHmmss"));
                }
                else return 235959;
            }
            else return 235959;
        }
        /// <summary>
        /// 获得调度的描述

        /// </summary>
        /// <returns>获得表示调度的字符串描述。</returns>
        private string Getfreq_description()
        {
            if (rbsdu_autoexec.Checked)
            {
                return "当开机时自动启动";
            }
            else if (rbsdu_idle.Checked)
            {
                return "每当CPU空闲时启动";
            }
            else if (rbsdu_One.Checked)
            {
                //return dt_one.Text + " " + time_one.Text + "执行一次";
                return dt_one.Text + " " + time_one.Text + "run once";
            }
            else if (rbsdu_More.Checked)
            {
                if (rdbfreq_day.Checked)
                {
                    if (rb_day_one.Checked)
                        //return "每" + nm_dayinterval.Value.ToString() + "天发生，在" + time_day_one.Text + "。";
                        return ApplicationSetting.Translate("run every") + " " + nm_dayinterval.Value.ToString() +" "+ ApplicationSetting.Translate(" days,at") + time_day_one.Text + "。";
                    else if (rb_day_cycle.Checked)
                    {
                        //return "每" + nm_dayinterval.Value.ToString() + "天发生，每" + nm_day_interval.Value.ToString()
                        //    + cb_subday_type.Text + "，在" + time_day_cycle.Text + "和" + time_end.Text + "之间。";
                        return ApplicationSetting.Translate("run every") + " " + nm_dayinterval.Value.ToString() +" "+ ApplicationSetting.Translate(" days,every") + nm_day_interval.Value.ToString()
                            + cb_subday_type.Text + "," + ApplicationSetting.Translate("between") + time_day_cycle.Text + ApplicationSetting.Translate("and") + time_end.Text;
                    }
                    else return "";
                }
                if (rdbfreq_week.Checked)
                {
                    string week = "";
                    if (w1.Checked) week += " 星期一 ";
                    if (w2.Checked) week += " 星期二 ";
                    if (w3.Checked) week += " 星期三 ";
                    if (w4.Checked) week += " 星期四 ";
                    if (w5.Checked) week += " 星期五 ";
                    if (w6.Checked) week += " 星期六 ";
                    if (w7.Checked) week += " 星期日 ";

                    if (rb_day_one.Checked)
                    {
                        return "每" + nm_week_interval.Value.ToString() + "周在" + week + "发生，在" + time_day_one.Text + "。";
                    }
                    else if (rb_day_cycle.Checked)
                    {
                        return "每" + nm_week_interval.Value.ToString() + "周在" + week
                            + "发生，每" + nm_day_interval.Value.ToString()
                            + cb_subday_type.Text + "，在" + time_day_cycle.Text + "和" + time_end.Text + "之间。";
                    }
                    else return "";
                }
                if (rdbfreq_month.Checked)
                {
                    string temp = "";
                    if (rb_monthday.Checked)
                    {
                        temp = "每" + nm_dmonth_interval.Value.ToString() + "个月于当月的第" + nm_date.Value.ToString() + "天发生，";
                    }
                    else if (rb_interval.Checked)
                    {
                        temp = "每" + nm_wmonth_interval.Value.ToString() + "个月于当月的第" + cb_weeknum.Text + cb_week.Text + "发生，";
                    }
                    if (rb_day_one.Checked)
                    {
                        return temp + "在" + time_day_one.Text + "。";
                    }
                    else if (rb_day_cycle.Checked)
                    {
                        return temp + "在" + time_day_cycle.Text + "和" + time_end.Text + "之间。";
                    }
                    else return "";
                }
                else return "";
            }
            else return "";
        }
        /// <summary>
        /// OK 按钮执行的动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>关闭调度任务编辑窗体，确认当前修改的内容。</remarks>
        private void b_ok_Click(object sender, EventArgs e)
        {
            if (t_name.Text != ""/* && t_cmd.Text != ""*/)
            {
                _DataRow["schedule_name"] = t_name.Text.Trim();
                _DataRow["schedule_description"] = t_description.Text.Trim();
                _DataRow["freq_type"] = Getfreq_type();
                _DataRow["freq_interval"] = Getfreq_interval();
                _DataRow["freq_subday_type"] = Getfreq_subday_type();
                _DataRow["freq_subday_interval"] = Getfreq_subday_interval();
                _DataRow["freq_relative_interval"] = Getfreq_relative_interval();
                _DataRow["freq_recurrence_factor"] = Getfreq_recurrence_factor();
                _DataRow["active_start_date"] = Getactive_start_date();
                _DataRow["active_end_date"] = Getactive_end_date();
                _DataRow["active_start_time"] = Getactive_start_time();
                _DataRow["active_end_time"] = Getactive_end_time();
                _DataRow["freq_description"] = Getfreq_description();
                _DataRow["last_invoke_time"] = "0";
                _DataRow["daemon"] = r_before.Checked ? "0":"1";
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                if (t_name.Text == "")
                {
                    MessageBox.Show("please input schedule name"); 
                    t_name.Focus();
                }
                else if (t_cmd.Text == "")
                {
                    MessageBox.Show("please choose schedule command");
                    t_cmd.Focus();
                }
                else return;
            }
        }
        /// <summary>
        /// 调用任务生成器

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>调用任务生成器，编辑新任务或修改已有的任务。</remarks>
        private void btn_open_Click(object sender, EventArgs e)
        {
            string ls_processstruct = _DataRow["command"].ToString();
            FrmExecTask lfd = new FrmExecTask(ls_processstruct);
            DialogResult drlfd = lfd.ShowDialog();
            if (drlfd == DialogResult.OK)
            {
                t_cmd.Text = "choose task";
                _DataRow["command"] = lfd.gs_command;
            }
        }
        /// <summary>
        /// 清除任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>将当期调度任务中的任务删除。</remarks>
        private void btn_clear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除此动作吗？", "调度", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) 
                == DialogResult.OK)
            {
                t_cmd.Text = "";
                _DataRow["command"] = "";
                _DataRow["command_xml"] = "";
                _Process = "";
            }
        }
        /// <summary>
        /// 设置执行一次/循环执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>设置一天内执行一次还是循环执行。</remarks>
        private void dayChange(object sender, EventArgs e)
        {
            if (rb_day_one.Checked)
            {
                time_day_one.Enabled = true;
                pweek.Enabled = false;
            }
            else if (rb_day_cycle.Checked)
            {
                time_day_one.Enabled = false;
                pweek.Enabled = true;
            }
        }
        /// <summary>
        /// 取消编辑的调度任务

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>关闭编辑调度任务窗体，取消当前的操作。</remarks>
        private void b_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        /// <summary>
        /// 选中月份调度中的选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>决定选用第几天还是第几个周几等选项。</remarks>
        private void rb_monthday_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_monthday.Checked)
            {
                pm1.Enabled = true;
                pm2.Enabled = false;
            }
            else if (rb_interval.Checked)
            {
                pm1.Enabled = false;
                pm2.Enabled = true;
            }
        }
        /// <summary>
        /// 选中结束日期单选框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>决定结束日期输入框是否有效。</remarks>
        private void rd_end_CheckedChanged(object sender, EventArgs e)
        {
            if (rd_end.Checked)
                dt_end.Enabled = true;
            else
                dt_end.Enabled = false;
        }
        /// <summary>
        /// 离开每几天录入框时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>判断Text为空时赋1。</remarks>
        private void nm_dayinterval_Leave(object sender, EventArgs e)
        {
            UpDownBase up = (UpDownBase)sender;
            if (up.Text == "")
            {
                ((NumericUpDown)sender).Value = 2;
                ((NumericUpDown)sender).Value = 1;
            }
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    DateTime ldt_rq = dateTimePicker1.Value;
        //    int li_week = Scheduler.Get_WeekofYear(ldt_rq);
        //    MessageBox.Show(li_week.ToString());
        //}
    }
}