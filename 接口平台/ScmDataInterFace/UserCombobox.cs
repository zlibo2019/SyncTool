using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace ScmDataInterFace
{
    public partial class UserCombobox : ComboBox
    {
        int columnPadding = 15;
        private float[] columnWidths = new float[0];  //项宽度
        private String[] columnNames = new String[0]; //项名称
        private int valueMemberColumnIndex = 0;       //valueMember属性列所在的索引


        public UserCombobox()
        {
            InitializeComponent();
        }
        /**/
        /// <summary>
        /// 绑定数据源或更改数据源时发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);

            InitializeColumns();
        }


        /**/
        /// <summary>
        /// 设置ValueMember属性发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnValueMemberChanged(EventArgs e)
        {
            base.OnValueMemberChanged(e);

            InitializeValueMemberColumn();
        }


        /**/
        /// <summary>
        /// 显示下拉框的时候触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);
            this.DropDownWidth = (int)CalculateTotalWidth();  //计算combobox下拉框的总宽度
        }



        /**/
        /// <summary>
        /// 初始化数据源各列的名称
        /// </summary>
        private void InitializeColumns()
        {
            PropertyDescriptorCollection propertyDescriptorCollection = DataManager.GetItemProperties();

            columnWidths = new float[propertyDescriptorCollection.Count];
            columnNames = new String[propertyDescriptorCollection.Count];

            for (int colIndex = 0; colIndex < propertyDescriptorCollection.Count; colIndex++)
            {
                String name = propertyDescriptorCollection[colIndex].Name;
                columnNames[colIndex] = name;
            }

            for (int i = 0; i < Items.Count; i++)
            {
                for (int colIndex = 0; colIndex < columnNames.Length; colIndex++)
                {
                    //如果 ListControl 项是被给定了该项以及属性名称的对象的属性，则返回该项的当前值
                    string item = Convert.ToString(FilterItemOnProperty(Items[i], columnNames[colIndex]));
                    Graphics g = this.CreateGraphics();
                    SizeF sizeF = g.MeasureString(item, Font);  //返回显示项字符串的大小
                    columnWidths[colIndex] = Math.Max(columnWidths[colIndex], sizeF.Width);
                }
            }
        }


        /**/
        /// <summary>
        /// 返回ValueMember在显示列中的索引位置
        /// </summary>
        private void InitializeValueMemberColumn()
        {
            int colIndex = 0;
            foreach (String columnName in columnNames)
            {
                if (String.Compare(columnName, ValueMember, true, CultureInfo.CurrentUICulture) == 0)
                {
                    valueMemberColumnIndex = colIndex;
                    break;
                }
                colIndex++;
            }
        }


        /**/
        /// <summary>
        /// 计算combobox下拉框的总宽度
        /// </summary>
        /// <returns></returns>
        private float CalculateTotalWidth()
        {

            columnPadding = 5;
            float totWidth = 0;
            foreach (int width in columnWidths)
            {
                totWidth += (width + columnPadding);
            }

            //总宽度加上垂直滚动条的宽度
            return totWidth + SystemInformation.VerticalScrollBarWidth;
        }

        /**/
        /// <summary>
        /// 获得各列的宽度和项的总宽度
        /// 引发 MeasureItem 事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            if (DesignMode)
                return;

            for (int colIndex = 0; colIndex < columnNames.Length; colIndex++)
            {
                //如果 ListControl 项是被给定了该项以及属性名称的对象的属性，则返回该项的当前值
                string item = Convert.ToString(FilterItemOnProperty(Items[e.Index], columnNames[colIndex]));
                SizeF sizeF = e.Graphics.MeasureString(item, Font);  //返回显示项字符串的大小
                columnWidths[colIndex] = Math.Max(columnWidths[colIndex], sizeF.Width);
            }

            float totWidth = CalculateTotalWidth(); //计算combobox下拉框项宽度

            e.ItemWidth = (int)totWidth;   //设置combobox下拉框项宽度
        }



        /**/
        /// <summary>
        /// 绘制下拉框的内容
        /// 引发 DrawItem 事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (DesignMode)
                return;

            e.DrawBackground();

            Rectangle boundsRect = e.Bounds;
            int lastRight = 0;

            using (Pen linePen = new Pen(SystemColors.GrayText))
            {
                using (SolidBrush brush = new SolidBrush(ForeColor))
                {
                    if (columnNames.Length == 0)
                    {
                        e.Graphics.DrawString(Convert.ToString(Items[e.Index]), Font, brush, boundsRect);
                    }
                    else
                    {

                        //循环各列
                        for (int colIndex = 0; colIndex < columnNames.Length; colIndex++)
                        {
                            string item = Convert.ToString(FilterItemOnProperty(Items[e.Index], columnNames[colIndex]));

                            boundsRect.X = lastRight;  //列的左边位置
                            boundsRect.Width = (int)columnWidths[colIndex] + columnPadding; //列的宽度
                            //boundsRect.Width = 20 + columnPadding; //列的宽度
                            lastRight = boundsRect.Right;

                            if (colIndex == valueMemberColumnIndex)//如果是valueMember项
                            {
                                using (Font boldFont = new Font(Font, FontStyle.Bold))
                                {

                                    //绘制项的内容
                                    e.Graphics.DrawString(item, boldFont, brush, boundsRect);
                                }
                            }
                            else
                            {
                                //绘制项的内容
                                e.Graphics.DrawString(item, Font, brush, boundsRect);
                            }


                            //绘制各项间的竖线
                            if (colIndex < columnNames.Length - 1)
                            {
                                e.Graphics.DrawLine(linePen, boundsRect.Right, boundsRect.Top, boundsRect.Right, boundsRect.Bottom);
                            }
                        }
                    }
                }
            }

            e.DrawFocusRectangle();
        }
    }
}
