# Photochop
Project for OU Software Engineering Fall 2016

------
###Getting started (on Windows)

1. `git clone https://github.com/nbgraham/Photochop.git`
2. Open administrator level command prompt  
    - Find your username with `net user`
    - Get access to local host port 800 with `netsh http add urlacl url=http://+:800/ user=[username]`  
          e.g. `netsh http add urlacl url=http://+:800/ user=Simon`
3. Run the solution in Visual Studio
4. Navigate to [homepage](localhost:800/servable/pages/Homepage.html) at localhost:800/servable/pages/Homepage.html  

------
Likely issues and how to fix them:

First of all, this is a Visual Studio 2015 project.

- VS 2015 is free software for us students,
and it would make things easier if everyone
developed in it. Please get and use it.

When adding new pages / web content in Visual Studio, make sure to set the "Copy to Output Directory" property to "Copy if newer".
- This can be found in the file's "properties" window.
- Otherwise the server won't find the file.

The first time you run the program, you're likely to get an error about insufficient permissions for the address. This can be fixed by running the following from an administrator-level command line:
```sh
netsh http add urlacl url=http://+:80/ user=Simon
```
Of course you'll have to change the "user" parameter to match your username, as given by "net user" command.

Once it launches successfully, navigate to
"localhost"
or
"localhost/servable/pages/Homepage.html"
to see our handiwork!  

-----
Uses [ImageProcessor](http://imageprocessor.org) library, specifically the [ImageFactory class](http://imageprocessor.org/imageprocessor/imagefactory/) and [Aforge.NET](http://www.aforgenet.com/).
