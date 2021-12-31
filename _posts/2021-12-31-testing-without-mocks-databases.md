---
layout: post
title:  "Testing without mocks - databases"
date:   2021-12-31 09:47:57+0100
categories: unit-testing, practices
---

Databases are at the heart of most LOB applications, and so they should also be at the heart your unit testing
strategies. In this post, i'll go into some of the mechanics of how to test without mocks. But because I'm finding that
this is a huge topic, this post only focusses on persistence (databases). 

This post combines information from previous post, including [why](../stop-mocking) you should 
stop using mocking libraries, some [tips and tricks](../testing-tricks). 

I'll try to provide some coding examples to these topics later. 

## TLDR;

When writing unit tests for LOB applications, I want to include the database as much as possible. It's 
a vital aspect of the system, so I won't ignore this aspect. Where possible and practical, I'll try to write tests
that include a real database. If this is not effective / efficient / possible, i'll try to see if I can
use or write an in-memory test double. Only if that's not possible will I revert to writing
(hand crafted) mocks. This approach gives me the most confidence that my system is actually working correctly.

# The elephant in the room: Persistence

Let's first talk about the elephant in the room when it comes to writing tests without mocks, 
which is the database (or better said, data persistence). 

When discussing mocking, most people agree that that truly external dependencies (such as 
external services) should be mocked. But when it comes to data persistence, it's not so clear. 

Some people consider the database to be one of those external systems, so they'll mock it out completely. 
Other people say that they design their software persistence ignorant and so they don't have to mock out 
the database. 

I would argue that testing your data persistence is actually really important. In line of business applications, 
data persistence is one of the most important concerns. It's the place where most performance problems
 stem from and the root of a lot of complexity. 

But there is **no silver bullet (tm)** when it comes to handling persistence. The right approach balances
out the requirements of the application that's being tested with the technologies used. 

Keep in mind, there is a huge variety in persistence technologies: 
- Relational databases (in all their various incarnations, such as MsSql, MySql, Postgress, Oracle, etc..)
- Event Stores (EventStore, Sql Stream Store, marten, etc.)
- Key Value Stores / document databases (MongoDB, AWS Dynamodb, Azure CosmosDB, RavenDB, etc..)
- Object stores (Azure Blob storage, S3, etc..)
- Indexers (elastic search, Azure Cognitive search, etc..)
- Message brokers, Queues, service busses, etc..  (yes, they actually do store data)
- many others. 

These technologies have vastly different api signatures, characteristics and capabilities. But where 
in the past, most applications just used one type of database (Relational), it's become mainstream now
to use a mix of these technologies. 

So, if you're building a LOB application, you're likely going to have to tackle several of these api's. 

# Heuristics on choosing an approach

There is no one size fits all approach on how to handle these persistence technologies in 
your unit tests. They are simply too diverse to make generalized statements about. So what I rely on
is a set of heuristics, that try to combine the unique requirements of the system with basic guiding 
principles. 

First of all, what I want to get out of my tests is (From my post on [well written tests](../well-written-tests)):
1. They are easy to write and maintain. 
2. They run fast enough (and ideally in parallel). 
3. They provide actual value (they help you write code and catch issues). 

This provides an interesting challenge. Usually there are these solutions: 

**Optimize for test value: Real db** You get the most value (3) when you run your tests against a **real** database 
(more on that below). However, this *can* come at the expense of how easy they are 
to write (1) and how fast they run (2). There are techniques you can use to improve (1) and (3) though, which i'll describe
below. 

**Optimize for test execution speed: Fake db**. You can fake your database, which optimizes for speedy execution of tests, because 
there is no real logic. However, you then decrease the value (3) because you don't know if the mock behaves exactly
as the real thing and it is extra code to write / maintain (2). Yes, a mocking library can help, but I get more 
mileage by hand crafting my mocks IF the api surface is not too big. More on that below. 

**Optimize for developer efficiency**. Most databases have a way to provide an in-memory implementation. For example,
for relational databases you can use [SQLite](https://www.sqlite.org/index.html), or an in-memory eventstore (SQL 
Steam Store). These usually are fast (2) and easy to use (1) but are an imperfect implementation so they may have 
differences in how they respond in certain situations. Therefor, they usually give you less confidence in the overall
solution and therefor less value (3). 

## Picking an approach
So, given these heuristics, how do you choose an approach?

First of all: **You don't have to limit yourself to a single choice.**. People so often look for the single and only
best way to handle an approach. But if you find that you have a situation that a simpler approach works great for 
80% of your tests but 20 % really requires a specialized and more complex approach, by all means, use both approaches. 

For example: 
> You have a straight forward application, not much business logic, relatively simple queries, etc..
> But there are a couple of rather complex  and very important queries. Use a simple approach like an in-memory implementation
> for the simple approach but use a real database for the complex queries. 

Another example when you use CQRS:
> You are applying CQRS in (parts) of your application. This means you have different access patterns 
> for your write model vs your read model. The write model typically involves reading / writing single items by ID
> whereas the read model involves complex queries. Use an in-memory approach for the write side, and a real
> db for the read model. 

Ok, another example *(because most people don't think of their software as simple / straight forward)*
> You have a complex application. Lots of business rules, complex queries, etc. 
> But you also have to manage some reference data. This can be covered using simple crud. 
> Use a real database for the complex scenarios, and a simple (in-memory) approach for the crud screens. 

Last example, I promise:
> You write your business logic tests using an in-memory database, because this allows you to focus
> on the business logic you've just added. However, you also have tests that cover complete business scenarios
> at api level. These tests have a bigger scope and there it makes sense to use a real database. 

So, ask yourself the following questions:

## 1. How much are you tied to the actual logic of the store?
> This is one of the most important questions when deciding what to do with the database. 

The more you rely on the specific implementation of the database (for example for performance reasons),
the less mileage you'll get from using a fake version. 

For example, if you use a relational database and heavily rely on joins, complex queries, materialized views, 
specific indexing patterns, or other fancy things, then you might not get much benefit from an in-memory 
implementation, because it doesn't give you the fidelity that you're looking for. Another example would be 
specific partitioning requirements in dynamodb or kafka for scalability reasons. 

However, if you're simply persisting objects, returning them and have only basic querying over them, or you 
are just testing straight forward business logic then an in-memory implementation is usually good enough. 
Usually, happy paths work really well with a in-memory double, but exceptions and platform specific 
optimizations do not. 

## 2. Is there a good in-memory implementation? 

Most often, a good in-memory implementation is worth it's weight in gold. Oh wait, software doesn't weigh much, but you 
get the gist. What I mean is, a good in-memory implementation can cover most common scenarios and that might make this 
a very effective option. Now, if you depend heavily on the implementation specifics of your database, for example 
for performance reasons, then you're probably better off NOT using a real database. 

If there is not a good in-memory implementation, perhaps you can write one. But if that's effective depends on the size of the api surface
and of course, how much you really need from your implementation. 

> I've had really good results testing my own business logic with in-memory event stores, in-memory sql databases (sql lite). Sometimes, 
> you have edge cases. For example in relational databases, each database subtle differences in sql dialect, and it's own way of handling
> indexes, triggers, constraints etc. These can trip you up if it's the only thing you rely on. 

> Another example is when using an in-memory event store. Regular business logic tests work great, but when it comes
> to testing your logic for when network connection failures occur, it can be better to work with a real database. Docker 
> works really well, but more on that later. 

## 3. How big / complex is the api surface? 

The size and complexity api surface of your persistence can also determine your strategy. A key value store, event 
store or document database typically has a very narrow api surface. This means it's relatively easy to create test 
doubles or even complete in-memory implementations for these relatively simple. 

The thing that's usually difficult to implement is the querying api. If you have something that uses linq expressions
or fairly straightforward filter parameters, then it may be possible to stick with in-memory test doubles. However, you
don't want to go overboard and have to implement a complete querying language, just to run your unit tests. 

On the other end of the spectrum are relational databases. Due to their nature of having (often complex) relations between
tables, and having very rich querying options over them, typically makes it very difficult to write your own mocks against. 

# Tips for Testing with in-memory implementations

An in-memory test double is usually great and my preferred option when writing business logic tests. 

Ideally, you have one provided to you, but if not, often they are not too difficult to write (assuming your api surface 
is not to big / complex). 

## Take control of eventual consistency

Eventual consistency is often used to improve the throughput of your system. This is often (but not required for)
CQRS or event sourced applications. How to handle this gracefully probably warrants an entire blog post, but for now,
a short discussion of this topic:

One of the difficult things I've found in unit testing is handling eventual consistency. Typically, you take an action,
then you verify if the side effects have occurred. Polling or retries are often used to handle this. 

But with eventual consistency, how do you distinguish if the action hasn't happened yet from a failure 
where the action is never going to happen? With everything being in-memory, a successful test will often run really
fast, but a failure can take a while, because you have to retry enough and wait long enough to be sure the test fails 
for the right reasons. 

One of the things I've used very successfully is to remove or completely control the eventual consistency from my 
in-memory implementations. 

1. When the action happens, synchronously process all side effects. 
2. Make the side effects observable. Create a Task that completes when the side effects have happened. This way,
you can listen for the effects to have happened and make things much more deterministic.
3. Control when to run the eventual consistent actions. For example, introduce a 'ProcessAllMessages()' method 
that will then execute your logic. 


## Testing your in-memory implementation
If you have an in-memory implementation, you must have some level of confidence that it actually works. 

One method I've used in the past is to create a body of tests that cover both a real and in-memory 
implementation. This makes sure that your in-memory version responds (within reason) the same as 
the real thing. It's also a good place to add additional tests once you find a discrepancy. 

To do this, create a base class where you place your tests. Then create two derived classes, that will set up the
real or in-memory instance. This way, the same tests run against two databases. 

# Tips for testing with hand-crafted test doubles

A hand crafted test double is handy when:
1. no good in-memory implementation is available
2. The api surface is reasonably narrow, but creating a real in-memory implementation would be too complex. 

Tips:

1. Make your test doubles easily observable.
2. Hide eventual consistency. 

# Tips for Testing with real db implementations

Testing with a real database implementation gives you a lot of confidence that
your code actually does what it needs to do. 

> It's still not going to be 100% the same. For example, in production, you'll likely use
> a sql server cluster, not a sql localdb instance. Or, you are using dynamodb, but locally 
> you have only a few partitions, so sharding is not so much of an issue. 

## Dockerize your setup

If possible, try to use docker to spin up your local databases. This means setting up your
environment is much easier (for other developers) and it (likely) makes your tests also 
portable to run on your build server without requiring software to be pre-installed. 

## Provide a proper level of isolation

A database is almost by definition a shared resource. Multiple instances of your application
and multiple threads within your application all compete with each other to access the same 
database.

In unit tests, sharing state between tests is a source of really hard to find bugs. When you run
a test in isolation, it works fine, but when running all of them (or even worse, only on the
build server) then they start failing. 

Also, when you run your tests in parallel, you don't want your tests to interfere with each other
either. 

> Try if possible to avoid sharing your database amongst your tests. 

So how do you do this? 

### Option 1: Use a database per test. 

This is an option if your database is not too big. With a small schema, it doesn't take much time to create
/ drop your database. 

I once used this technique, but after a while it became a bit too slow. Also, it required me to seed the database
every time with the same data. So, to avoid this, I created a small tool that created a database once, seeded
it with data, then created a backup of the database. I then restored the backup for each test. This proved to be
much faster. 

### Option 2: Use a schema / namespace / table per test.

I often use this technique when using dynamodb. If you allow your test to create a random table
name, then each test get's it's own table. So, they don't interfere with each other. I also did this when using elastic. 
Simply create an index per test. 

Some databases make it light weight to create / drop databases or database tenants. Then this
would also work to provide some form of isolation. 

> Don't forget to delete your data after your test. 

> Also.. provide a tool to clean up your data manually, because the cleanup logic doesn't always run. 

### Option 3: Share a database but be careful

One system I worked on in the past had a huge database schema. Recreating this database for each test
would have been completely impractical. So, in order to make the tests run at an ok speed, I only created
a new database per test collection. 

Within the test collection, I was sharing the database. This means that I had to be really careful
in how to write the tests. For example, it's fine to insert some data with a unique key, then read
that data back again. Since the key is unique for your test, you (likely) won't get collisions. 

Search is a tricky one here. Because, you can find data created by other tests. This means, you'll
either need to make sure you narrow your search appropriately or that you can handle arbitrary large
result sets (page through all data found until you find what you're looking for). 

### Not recommended techniques

I've seen various other techniques around this.

Using a real database instance but never cleaning it up is not something I recommend. It leaves too much 
crap behind, so after a while you have no idea what happened. 

Using locks, then reverting everything once done. I don't recommend this, because it doesn't represent
the way the transactions would normally complete. Also, you might get a lot of deadlocks in your database. 

Manual cleanup after the tests and having each test run separately is another pattern I don't really like. 
It's too easy to forget to add some cleanup code as the system grows and then you get test instability. 

# Conclusion

In this post, I've described various techniques on how you can handle databases when you don't want to 
use mocking libraries. As usual, it turned out to be a longer post than I had hoped, so I'll publish this
now and will come up with some code examples later. 