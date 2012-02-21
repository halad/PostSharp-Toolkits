# ﻿PostSharp Toolkits


Welcome to PostSharp Toolkits!

## What are PostSharp Toolkits?


PostSharp Toolkits is a collection of ready-made solutions for adding logging, exception handling and performance monitoring to your application with no changes to your source code!

Powered by [PostSharp](http://www.sharpcrafters.com), the most complete AOP solution for .NET, the PostSharp Toolkits build upon the raw power of Aspect-Oriented Programming to seamlessly apply those solutions throughout your application.


## What toolkits currently exist?

Currently, we're working on the **PostSharp Diagnostics Toolkit** - an instrumentation toolkit that adds diagnostics feautres, such as logging, exception handling, performance counters and feature tracking to your application.

The PostSharp Diagnostics Toolkit includes pluggable support for the leading logging frameworks, such as NLog. Support for additional frameworks is coming soon!

## How to install the PostSharp Diagnostics Toolkit?

The **PostSharp Diagnostics Toolkit** is available on NuGet. NLog support is available via the **PostSharp Diagnostics Toolkit for NLog** package.

### Getting started with PostSharp Diagnostics Toolkit for NLog

 - **Step 1:** Add the **PostSharp Diagnostics Toolkit for NLog** from NuGet to the assembly you wish to instrument. It downloads the required dependencies automatically (includes PostSharp 2.1 SP1, NLog and the required PostSharp Logging Toolkit base).  

 - **Step 2:** Configure NLog using the desired configuration (see the [official documentation](http://nlog-project.org/wiki/Configuration_file) for support), or get the **NLog.Config** package from NuGet.

 - **Step 3:** Rebuild your project. It is now enhanced with logging capabilities!

### How does this work?

To learn about how do the PostSharp Toolkits work, please read the blog post **Introducing PostSharp Toolkits** on the [SharpCrafters](http://www.sharpcrafters.com) website.

# Questions? Suggestions? Bugs?

Please visit our dedicated [PostSharp Toolkits Support Forum](http://www.sharpcrafters.com/forum/Group27.aspx) to let us know what you think!