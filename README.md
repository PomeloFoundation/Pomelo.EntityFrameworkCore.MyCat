# Pomelo.EntityFrameworkCore.MyCat

[![Travis build status](https://img.shields.io/travis/PomeloFoundation/Pomelo.EntityFrameworkCore.MyCat.svg?label=travis-ci&branch=master&style=flat-square)](https://travis-ci.org/PomeloFoundation/Pomelo.EntityFrameworkCore.MyCat)
[![AppVeyor build status](https://img.shields.io/appveyor/ci/Kagamine/Pomelo-EntityFrameworkCore-MyCat/master.svg?label=appveyor&style=flat-square)](https://ci.appveyor.com/project/Kagamine/pomelo-entityframeworkcore-MyCat/branch/master) [![NuGet](https://img.shields.io/nuget/v/Pomelo.EntityFrameworkCore.MyCat.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MyCat/) [![Join the chat at https://gitter.im/PomeloFoundation/Home](https://badges.gitter.im/PomeloFoundation/Home.svg)](https://gitter.im/PomeloFoundation/Home?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Pomelo.EntityFrameworkCore.MyCat is an Entity Framework Core provider and optimized for [MyCat](https://github.com/MyCATApache/Mycat-Server) (An open source MySQL cluster proxy which based on Cobar)

## What is MyCat?

![image](https://cloud.githubusercontent.com/assets/2216750/17903740/251f23fc-699f-11e6-991c-952db05f13a0.png)

[MyCAT](https://github.com/MyCATApache/Mycat-Server) is an Open-Source software, a large database cluster oriented to enterprises. MyCAT is an enforced database which is a replacement for MySQL and supports transaction and ACID. Regarded as MySQL cluster of enterprise database, MyCAT can take the place of expensive Oracle cluster. MyCAT is also a new type of database, which seems like a SQL Server integrated with the memory cache technology, NoSQL technology and HDFS big data. And as a new modern enterprise database product, MyCAT is combined with the traditional database and new distributed data warehouse. In a word, MyCAT is a fresh new middleware of database.

Mycat’s target is to smoothly migrate the current stand-alone database and applications to cloud side with low cost and to solve the bottleneck problem caused by the rapid growth of data storage and business scale.

## Why use MyCat?

- Based on Alibaba's open-source project Cobar, whose stability, reliability, excellent architecture and performance, as well as many mature use-cases make MyCAT have a good starting. Standing on the shoulders of giants, MyCAT feels confident enough to go farther.
- Extensively drawing on the best open-source projects and innovative ideas, which are integrated into the Mycat’s gene, make MyCAT be ahead of the other current similar open-source projects, even beyond some commercial products.
- MyCAT behind a strong technical team whose participants are experienced more than five years including some senior software engineer, architect, DBA, etc. Excellent technical team to ensure the product quality of Mycat.
- MyCAT does not rely on any commercial company. It’s unlike some open-source projects whose important features is enclosed in its commercial products and making open-source projects like a decoration.
- Supports individual databases like MySQL, SQL Server, Oracle, MongoDB, DB2...

## Getting Started

① Install Java8, MySQL 5.7, .NET Core SDK on your server

```bash
# add-apt-repository ppa:webupd8team/java
# apt-get update
# apt-get install oracle-java8-installer

# wget http://dev.mysql.com/get/mysql-apt-config_0.6.0-1_all.deb
# dpkg -i mysql-apt-config_0.6.0-1_all.deb
# apt-get update
# apt-get install mysql-community-server

# sh -c 'echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ trusty main" > /etc/apt/sources.list.d/dotnetdev.list'
# apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893
# apt-get update
# apt-get install dotnet-dev-1.0.0-preview2-003121
```

Ensure `utf8` is the default charset. You can replace the `my.cnf` file by using [this](https://gist.github.com/Kagamine/ccf10486b9854e0ceb14779dfadd3640).

② Download [Pomelo.EntityFrameworkCore.MyCat.Proxy](https://github.com/PomeloFoundation/Entity-Framework-Core-MyCat-Proxy/releases) and [MyCat Server](https://github.com/MyCATApache/Mycat-download)

③ Configure the `config.json` which in Pomelo.EntityFrameworkCore.MyCat.Proxy root path, set the `MyCatRootPath`.

④ Start the proxy by execute `nohup dotnet Pomelo.EntityFrameworkCore.MyCat.Proxy.dll`

⑤ Create .NET project. Add `Pomelo.EntityFrameworkCore.MyCat 1.0.0-*` into your project.

⑥ Configure your DbContext, declare the distributed mysql node address.

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    base.OnConfiguring(optionsBuilder);
    optionsBuilder.UseMyCat("server=192.168.0.129;database=blog;uid=test;pwd=test")
        .UseDataNode("192.168.0.129", "blog_1", "root", "123456")
        .UseDataNode("192.168.0.129", "blog_2", "root", "123456")
        .UseDataNode("192.168.0.129", "blog_3", "root", "123456")
        .UseDataNode("192.168.0.129", "blog_4", "root", "123456");
}
```

⑦ Create models and using the `dotnet ef migrations add Init` and `dotnet ef database update` to init your database.

⑧ Most of functions which provided in Entity Framework Core were supported. You are able to use .Where(), .Count(), .Sum() ... with `Pomelo.EntityFrameworkCore.MyCat`.

[View the sample on YouTube](https://youtu.be/q0CXfFNtMZo)

## Contribute

One of the easiest ways to contribute is to participate in discussions and discuss issues. You can also contribute by submitting pull requests with code changes.

## License

[MIT](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MyCat/blob/master/LICENSE)
