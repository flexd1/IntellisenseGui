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
using FileDragDrop;
using ImTools;
using System.Windows.Forms.Integration;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using IntellisenseGui.Views;

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
        /// 翻译模式列表
        /// </summary>
        public List<string> TranslateModeList { get; set; }

        /// <summary>
        ///翻译模式
        /// </summary>
        private string translateMode;

        public string TranslateMode
        {
            get { return translateMode; }
            set { SetProperty(ref translateMode, value); }
        }

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

        /// <summary>
        /// MainWindow实例
        /// </summary>
        private MainWindow _mainWindow;

        public MainWindowViewModel()
        {
            // 传实例
            Translator.GetMainVM(this);

            Translator.LogPrint("程序启动");

            // 初始化combobox
            InitComboBox();

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
                Translator.ChangeMode = (Translator.changeMode)Enum.Parse(typeof(Translator.changeMode), ChangeMode);
                await Task.Run(() => Translator.ExecuteAsync("123"));

                sw.Stop();
                Debug.WriteLine("===运行时间===" + sw.Elapsed.ToString());
                MessageBox.Show("执行完成", "提示", MessageBoxButton.OK);
            });

            // 添加文件
            AddFileCommand = new((e) =>
            {
                if (string.IsNullOrWhiteSpace(e))
                {
                    MessageBox.Show("请输入文件或文件夹路径", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                PathList = new(PathList.Union(Translator.GetAllFileName(e)));
            });

            // 删除
            DeleteFileCommand = new(() =>
            {
                Debug.WriteLine("delete");
            });


        }

        /// <summary>
        /// 初始化下拉框选项
        /// </summary>
        private void InitComboBox()
        {
            // 初始化替换模式列表
            ChangeModeList = Enum.GetValues(typeof(Translator.changeMode)).OfType<Translator.changeMode>().Select(v => v.ToString()).ToList();
            ChangeModeList[0] += "(推荐)";

            // 初始化翻译模式列表
            TranslateModeList = Enum.GetValues(typeof(Translator.translateMode)).OfType<Translator.translateMode>().Select(v => v.ToString()).ToList();
            TranslateModeList[0] += "(推荐)";
        }

        /// <summary>
        /// 拖入文件事件
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// 拖入winform事件
        /// </summary>
        /// <param name="e"></param>
        private void DropFile2(ElevatedDragDropArgs e)
        {
            foreach (var path in e.Files)
            {
                PathList = new(PathList.Union(Translator.GetAllFileName(path)));
            }
        }

        public void View_Loaded(object sender, EventArgs e)
        {
            _mainWindow = (MainWindow)sender;
            this.Button_ShowDragDropWindowClick(this, EventArgs.Empty);
            //窗口位置变更
            _mainWindow.LocationChanged += View_LocationChanged;
        }


        [DllImport("user32.dll")]
        private static extern int SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);

        //App管理列表(Form)
        private AppMangerForm appMangerListBoxForm { set; get; }

        /// <summary>
        /// 窗口位置变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void View_LocationChanged(object sender, EventArgs e)
        {
            //临时坐标计算
            double left = _mainWindow.Left + _mainWindow.ActualWidth;
            //对象验证
            if (appMangerListBoxForm != null)
            {
                appMangerListBoxForm.Left = (int)left;
                appMangerListBoxForm.Top = (int)_mainWindow.Top;
            }
        }

        /// <summary>
        /// sets the owner of a System.Windows.Forms.Form to a System.Windows.Window
        /// </summary>
        /// <param name="form"></param>
        /// <param name="owner"></param>
        public static void SetOwner(System.Windows.Forms.Form form, System.Windows.Window owner)
        {
            WindowInteropHelper helper = new WindowInteropHelper(owner);
            SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());
        }

        /// <summary>
        /// 显示拖拽窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ShowDragDropWindowClick(object sender, EventArgs e)
        {
            //获取主窗口位置
            var startLeft = _mainWindow.Left + _mainWindow.ActualWidth;
            var startTop = _mainWindow.Top;

            //窗口验证
            if (this.appMangerListBoxForm == null)
            {
                //消息转发到WinForm
                //注:添加WindowsFormsIntegration引用
                WindowsFormsHost.EnableWindowsFormsInterop();
                //显示窗口
                this.appMangerListBoxForm = new AppMangerForm(DropFile2);
                this.appMangerListBoxForm.Left = (int)startLeft;
                this.appMangerListBoxForm.Top = (int)startTop;

                //设置窗口所有者
                SetOwner(this.appMangerListBoxForm, _mainWindow);
            }

            //更新位置
            this.appMangerListBoxForm.Top = (int)startTop;
            this.appMangerListBoxForm.Left = (int)startLeft;

            //显示窗口
            this.appMangerListBoxForm.Show();
        }

        /// <summary>
        /// 隐藏拖拽窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_HideDragDropWindowClick(object sender, RoutedEventArgs e)
        {
            if (appMangerListBoxForm != null)
            {
                this.appMangerListBoxForm.Hide();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Button_ShowDragDropWindowClick(this, EventArgs.Empty);
        }
    }
}
