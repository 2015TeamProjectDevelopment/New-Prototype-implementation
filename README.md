# Prototype-implementation
Prototype implementation
- 生成exe后要将update_its.exe放到软件的安装目录下
- 默认新版本软件名为new_WpfApp.exe,旧版本软件名为old_WpfApp.exe


# 另一个程序实现更新
  1.运行ClassLibrary1（Shift F6 生成）
  2.打开test.sln
    项目的各个文件 右上角 引用那里，右键 删掉ClassLibrary1 
    然后重新添加1中你刚刚生成的dll（浏览 然后选择）
  3.使用之前的程序 生成一个ini文件
  4.运行test.exe 将url指向3中的文件，点击Update