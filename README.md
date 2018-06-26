# Prototype-implementation
Prototype implementation
- 生成exe后要将update_its.exe放到软件的安装目录下
- 默认新版本软件名为new_WpfApp.exe,旧版本软件名为old_WpfApp.exe


# 另一个程序实现更新
- 	进入pre_zip,在oldProgram下运行旧软件的WpfApp1，选择anotherProgram's.exe
	更新方式设为更新本软件
-	进入anotherProgram，运行test.exe,输入file:///+WpfApp1中配置文件的路径
	如：file:///D:\(电脑的路径)\pre_zip\oldProgram\Server\versionFolder\1_ini\1.ini
	然后点击更新
