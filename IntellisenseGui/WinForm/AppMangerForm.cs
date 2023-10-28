﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileDragDrop
{
    public partial class AppMangerForm : System.Windows.Forms.Form
    {
        private Action<ElevatedDragDropArgs> DropFile;
        public AppMangerForm(Action<ElevatedDragDropArgs> dropFile)
        {
            InitializeComponent();
            //AllowDrop设置为false
            ElevatedDragDropManager filter = new ElevatedDragDropManager();
            //开启拖放功能
            filter.EnableDragDrop(this.Handle);
            //设置拖放结束回调
            filter.ElevatedDragDrop += this.ElevatedDragDrop;
            //添加消息过滤器
            System.Windows.Forms.Application.AddMessageFilter(filter);
            DropFile = dropFile;
        }

        //拖放结束事件
        private void ElevatedDragDrop(System.Object sender, ElevatedDragDropArgs e)
        {
            try
            {
                if (e.HWnd == this.Handle)
                {
                    DropFile(e);
                }
            }
            catch (Exception ex)
            {
                //异常信息
                MessageBox.Show("ElevatedDragDrop error=" + (ex.TargetSite?.Name) + "!");
            }
        }
    }
}
