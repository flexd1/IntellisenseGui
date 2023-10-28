# IntellisenseGui
基于WPF+Winform生成Visual Studio本地化IntelliSense文件的可视化工具

我们对提示文件的本地化有两种方式
- 在源文件路径中创建一个符合本地化标识的文件夹[符合本地化标识](https://github.com/dotnet/docs/issues/27283https://learn.microsoft.com/zh-cn/dotnet/core/install/localized-intellisense?WT.mc_id=dotnet-35129-website)，并将本地化内容放入（推荐）
- 直接在源文件中添加入本地化内容

## 如何使用

### 1. 将需要翻译的文件或文件夹拖入框内，或手动输入文件地址
![image](https://github.com/flexd1/IntellisenseGui/assets/56830251/64694422-1bad-44b7-a8b3-e7b2ae54e2a3)
### 2. 在项目中已存在部分字典文件，但不可能不全，如需更新需勾选上
![image](https://github.com/flexd1/IntellisenseGui/assets/56830251/9c91e6d5-0fc6-4ea6-99a5-5e8a43b6ca10)
### 3. 选择所需模式（推荐默认选项）并点击执行
![image](https://github.com/flexd1/IntellisenseGui/assets/56830251/3e377628-e30c-4d25-8a32-d18c48a7fa90)
### 4. 执行完成后会看到本地化文件夹，重启visual studio后即可生效
![image](https://github.com/flexd1/IntellisenseGui/assets/56830251/b7c44740-7f70-441f-a089-4e7efc740704)

