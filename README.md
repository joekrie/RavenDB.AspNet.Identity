# RavenDB.AspNet.Identity #
An ASP.NET Identity provider for RavenDB

## Purpose ##

This fork seeks to port RavenDB.AspNet.Identity to ASP.NET 5 (vNext). The Identity framework in ASP.NET 5 breaks with ASP.NET 4. Also, this fork uses the AsyncDocumentSession instead of the synchronous version.

## Status ##

Most of the methods have been ported. A few new methods introduced to Identity in ASP.NET 5 are not yet implemented and throw a NotImplementException. 

The tests do dot currently run. Many of the dependencies have changed in ASP.NET 5. For example, UserManager now has 10 dependencies, whereas in ASP.NET 4 it had one.
