using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Windows.Shapes;
using System.Text;
using DryIoc;

namespace IntellisenseGui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region 属性
        private string _title = "IntellisenseGui";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ObservableCollection<string> _pathList = new();
        public ObservableCollection<string> PathList
        {
            get { return _pathList; }
            set { SetProperty(ref _pathList, value); }
        }

        // 是否更新字典文件
        private bool _isUpdateDirectory = false;
        public bool IsUpdateDirectory
        {
            get { return _isUpdateDirectory; }
            set { SetProperty(ref _isUpdateDirectory, value); }
        }

        // 日志输出


        private StringBuilder _logTemp = new();

        public StringBuilder LogTemp
        {
            get { return _logTemp; }
            set { _logTemp = value; _logText = value.ToString(); }
        }


        // 日志输出
        private string _logText;
        public string LogText
        {
            get
            {
                return _logText;
            }
            set { SetProperty(ref _logText, value); }
        }
        #endregion
        /// <summary>
        /// 拖拽文件获取路径
        /// </summary>
        public DelegateCommand<DragEventArgs> DropFileCommand { get; }

        /// <summary>
        /// 清空文件列表
        /// </summary>
        public DelegateCommand ClearCommand { get; }
        /// <summary>
        /// 执行翻译
        /// </summary>
        public DelegateCommand StartCommand { get; }
        public DelegateCommand DeleteFileCommand { get; }
        public DelegateCommand<string> AddFileCommand { get; }
        /// <summary>
        /// 替换模式列表
        /// </summary>
        public List<string> ChangeModeList { get; set; }

        /// <summary>
        /// 替换模式
        /// </summary>
        private string changeMode;

        public string ChangeMode
        {
            get { return changeMode; }
            set { SetProperty(ref changeMode, value); }
        }

        public MainWindowViewModel()
        {
            // 传实例
            Translator.GetMainVM(this);

            // 拖拽文件进入listbox
            DropFileCommand = new DelegateCommand<DragEventArgs>(DropFile);

            // 清空文件列表
            ClearCommand = new(() =>
            {
                PathList.Clear();
                Translator.AllFileName.Clear();
            });

            // 执行操作
            StartCommand = new(async () =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();
                Translator.IsUpdateDirectory = IsUpdateDirectory;
                Translator.ChangeMode= (Translator.changeMode)Enum.Parse(typeof(Translator.changeMode), ChangeMode);
                await Task.Run(() => Translator.ExecuteAsync("123"));

                sw.Stop();
                Debug.WriteLine("===运行时间===" + sw.Elapsed.ToString());
                MessageBox.Show("执行完成", "提示", MessageBoxButton.OK);
            });

            // 添加文件
            AddFileCommand = new((e) =>
            {
                PathList = new(PathList.Union(Translator.GetAllFileName(e)));
            });

            // 删除
            DeleteFileCommand = new(() =>
            {
                Debug.WriteLine("delete");
            });

            // 初始化替换模式列表
            ChangeModeList = Enum.GetValues(typeof(Translator.changeMode)).OfType<Translator.changeMode>().Select(v => v.ToString()).ToList();

        }

        private void DropFile(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] pathArr = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var path in pathArr)
                {
                    PathList = new(PathList.Union(Translator.GetAllFileName(path)));
                }
            }
        }


    }
}
