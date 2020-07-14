# 考试系统部署说明

1. 使用Microsoft SQL Server Management Studio打开并执行`Scripts.sql`文件。

2. 打开`release`文件夹下的`Web.config`文件，在其中找到如下代码段

   ```xml
   <connectionStrings>
       <add name="ExaminationSystemDbEntities" connectionString="metadata=res://*/Models.ESModel.csdl|res://*/Models.ESModel.ssdl|res://*/Models.ESModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=www.barteam.cn;initial catalog=ExaminationSystemDb;persist security info=True;user id=sa;password=123456;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
   </connectionStrings>
   ```

   将`data source=www.barteam.cn`等号右边修改为数据库所在的服务器地址
     将`user id=sa`等号右边修改为登录数据库的用户名
     将`password=123456`等号右边修改为登录数据库的用户密码

***

以下为常规网站部署方法：

1. 打开IIS Manager，在右侧连接选项卡中找到本机下的网站文件夹，点击右键，选择添加网站。
2. 自定义网站名称
3. 内容目录中的物理路径需要选择需要发布网站的项目文件夹，即解压后的`release`文件夹
4. 自定义端口
5. 点击确定即可

***

> 网站部署成功后会默认创建一个管理员账号（账号：admin    密码：123456）