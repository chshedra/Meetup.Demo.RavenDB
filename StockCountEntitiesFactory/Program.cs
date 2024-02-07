// See https://aka.ms/new-console-template for more information
using Meetup.Demo.Factory;

Console.WriteLine("Hello, World!");

var store = DocumentStoreHolder.Store;

using (var session = store.OpenSession()) { }
